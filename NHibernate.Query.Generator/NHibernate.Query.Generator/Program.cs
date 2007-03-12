using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace NHibernate.Query.Generator
{
    internal class Program
    {
        private static string targetExtention;
        private static string outputDir;
        private static CodeDomProvider provider = null;

        private static void Main(string[] args)
        {
            string inputFilePattern = GetCommandLineArguments(args);
            try
            {
                SetupCodeProvider();
                string directoryName = Path.GetDirectoryName(inputFilePattern);
                if (string.IsNullOrEmpty(directoryName))
                    directoryName = ".";
                string fileName = Path.GetFileName(inputFilePattern);
                foreach (string file in Directory.GetFiles(directoryName, fileName))
                {
                    OutputFile(file);
                }
                OutputQueryBuilder();
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(
                    "A type load error occured!\r\nThis usually happens if NHibernate Query Generator is unable to load all the required assemblies.");
                Dictionary<string, bool> reported = new Dictionary<string, bool>();
                foreach (Exception loaderException in e.LoaderExceptions)
                {
                    if (reported.ContainsKey(loaderException.Message) == false)
                    {
                        Console.WriteLine(loaderException.Message);
                        reported.Add(loaderException.Message, true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured:");
                Console.Write(e);
            }
        }

        private static void OutputFile(string file)
        {
            string fileExt = Path.GetExtension(file);
            string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + "." + targetExtention);
            // hbm file
            if (fileExt.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                GenerateSingleFile(File.OpenText(file), outputFile);
            }
            else if (fileExt.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) ||
                     fileExt.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase)) // Active Record...
            {
                GenerateFromActiveRecordAssembly(file);
            }
        }

        private static void OutputQueryBuilder()
        {
            //write query builders so user can just include the whole directory.
            Stream namedExp =
                typeof(Program).Assembly.GetManifestResourceStream("NHibernate.Query.Generator.QueryBuilders.QueryBuilder." +
                                                                    targetExtention);
            File.WriteAllText(Path.Combine(outputDir, "QueryBuilder." + targetExtention), new StreamReader(namedExp).ReadToEnd());
            Console.WriteLine("Successfuly created file: {0}\\QueryBuilder.{1}", outputDir, targetExtention);
        }

        private static void GenerateFromActiveRecordAssembly(string file)
        {
            string fullPath = Path.GetFullPath(file);
            RegisterAssemblyResolver(fullPath);

            Assembly asm = Assembly.LoadFile(fullPath);
            Assembly activeRecordAssembly = GetActiveRecordAsembly(asm);
            if (activeRecordAssembly == null)
            {
                throw new InvalidOperationException(string.Format("Could not find Active Record assembly referenced from {0}", asm));
            }
            object activeRecordModelBuilder =
                Activator.CreateInstance(
                    activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.ActiveRecordModelBuilder"));
            ArrayList models = new ArrayList();
            foreach (System.Type type in asm.GetTypes())
            {
                if (IsActiveRecordType(type) == false)
                    continue;
                object model = Invoke(activeRecordModelBuilder, "Create", type);
                if (model == null)
                    continue;
                models.Add(model);
            }

            object graphConnectorVisitor =
                Activator.CreateInstance(
                    activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.GraphConnectorVisitor"),
                    new object[] { Get(activeRecordModelBuilder, "Models") });

            Invoke(graphConnectorVisitor, "VisitNodes", models);

            object semanticVisitor =
                Activator.CreateInstance(
                    activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.SemanticVerifierVisitor"),
                    new object[] { Get(activeRecordModelBuilder, "Models") });

            Invoke(semanticVisitor, "VisitNodes", models);

            foreach (object model in models)
            {
                bool isNestedType = (bool)Get(model, "IsNestedType");
                bool isDiscriminatorSubClass = (bool)Get(model, "IsDiscriminatorSubClass");
                bool isJoinedSubClass = (bool)Get(model, "IsJoinedSubClass");

                if (!isNestedType && !isDiscriminatorSubClass && !isJoinedSubClass)
                {
                    object xmlVisitor =
                        Activator.CreateInstance(
                            activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.XmlGenerationVisitor"));

                    Invoke(xmlVisitor, "CreateXml", model);

                    System.Type type = (System.Type)Get(model, "Type");
                    string genFile = Path.Combine(outputDir, "Where." + type.Name + "." + targetExtention);
                    GenerateSingleFile(new StringReader((string)Get(xmlVisitor, "Xml")), genFile);
                }
            }

            object assemblyXmlGenerator =
                Activator.CreateInstance(
                    activeRecordAssembly.GetType("Castle.ActiveRecord.Framework.Internal.AssemblyXmlGenerator"),
                    new object[] { });

            string[] xmls = (string[])Invoke(assemblyXmlGenerator, "CreateXmlConfigurations", asm);
            int i = 0;
            foreach (string xml in xmls)
            {
                string genFile = "Where." + asm.FullName;
                if (i > 0)
                {
                    genFile += i.ToString();
                }
                genFile += "." + targetExtention;
                GenerateSingleFile(new StringReader(xml), genFile);
                i += 1;
            }

        }

        private static bool IsActiveRecordType(System.Type type)
        {
            foreach (object customAttribute in type.GetCustomAttributes(true))
            {
                if (customAttribute.GetType().Name == "ActiveRecordAttribute")
                    return true;
            }
            return false;
        }

        public static object Invoke(object obj, string name, params object[] args)
        {
            return obj.GetType().GetMethod(name).Invoke(obj, args);
        }


        public static object Get(object obj, string name)
        {
            return obj.GetType().GetProperty(name).GetValue(obj, null);
        }

        static List<Assembly> visited = new List<Assembly>();
        /// <summary>
        /// This is needed to make sure that we work with different versions of Active Record
        /// </summary>
        private static Assembly GetActiveRecordAsembly(Assembly assembly)
        {
            if (visited.Contains(assembly))
                return null;
            visited.Add(assembly);
            System.Type type = assembly.GetType("Castle.ActiveRecord.ActiveRecordBase", false);
            if (type != null)
                return type.Assembly;
            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                Assembly refAsm = Assembly.Load(assemblyName);
                Assembly result = GetActiveRecordAsembly(refAsm);
                if (result != null)
                    return result;
            }
            return null;
        }

        private static void RegisterAssemblyResolver(string fullPath)
        {
            string dir = Path.GetDirectoryName(fullPath);
            AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs args)
            {
                string asmFileName = args.Name.Split(',')[0];
                string exeFileName = Path.Combine(dir, asmFileName + ".exe");
                if (File.Exists(exeFileName))
                    return Assembly.LoadFile(exeFileName);
                string dllFileName = Path.Combine(dir, asmFileName + ".dll");
                if (File.Exists(dllFileName))
                    return Assembly.LoadFile(dllFileName);
                return null;
            };
        }

        private static void GenerateSingleFile(TextReader input, string destinationFile)
        {
            QueryGenerator generator = new QueryGenerator(input, provider);
            using (StreamWriter outputStream = File.CreateText(destinationFile))
            {
                generator.Generate(outputStream);
            }
            Console.WriteLine("Successfuly created file {0}", destinationFile);
        }

        private static string GetCommandLineArguments(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("      NHibernate.Query.Generator <cs or vb> <*.hbm.xml> <output-dir>");
                Console.WriteLine("      NHibernate.Query.Generator <cs or vb> asssembly.dll <output-dir>");
                Environment.Exit(1);
            }
            targetExtention = args[0];
            outputDir = args[2];
            return args[1];
        }

        private static void SetupCodeProvider()
        {
            switch (targetExtention.ToLower())
            {
                case "vb":
                    provider = new VBCodeProvider();
                    break;
                case "cs":
                    provider = new CSharpCodeProvider();
                    break;
                default:
                    Console.WriteLine("Unknown language element, expected 'cs' or 'vb', but got {0}", targetExtention);
                    Environment.Exit(1);
                    break;
            }
        }
    }
}
