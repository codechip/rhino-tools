#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CommandLine;
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
            ApplicationOptions options = new ApplicationOptions();
            if (Parser.ParseArgumentsWithUsage(args, options)==false)
                Environment.Exit(2);
            string inputFilePattern = options.InputFilePattern;
            targetExtention = options.Lang.ToString().ToLower();
            outputDir = options.OutputDirectory;

            try
            {
                SetupCodeProvider();
                string directoryName = Path.GetDirectoryName(inputFilePattern);
                if (string.IsNullOrEmpty(directoryName))
                    directoryName = ".";
                string fileName = Path.GetFileName(inputFilePattern);
                foreach (string file in Directory.GetFiles(directoryName, fileName))
                {
                    OutputFile(file, options.BaseNamespace);
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

        private static void OutputFile(string file, string baseNamespace)
        {
            string fileExt = Path.GetExtension(file);
            string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + "." + targetExtention);
            // hbm file
            if (fileExt.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                GenerateSingleFile(File.OpenText(file), outputFile, baseNamespace);
            }
            else if (fileExt.EndsWith("exe", StringComparison.InvariantCultureIgnoreCase) ||
                     fileExt.EndsWith("dll", StringComparison.InvariantCultureIgnoreCase)) // Active Record...
            {
                GenerateFromActiveRecordAssembly(file, baseNamespace);
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

        private static void GenerateFromActiveRecordAssembly(string file, string baseNamespace)
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
                    GenerateSingleFile(new StringReader((string)Get(xmlVisitor, "Xml")), genFile, baseNamespace);
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
                string genFile = "Where." + asm.GetName().Name;
                if (i > 0)
                {
                    genFile += i.ToString();
                }
                genFile += "." + targetExtention;
                GenerateSingleFile(new StringReader(xml), Path.Combine(outputDir, genFile), baseNamespace);
                i += 1;
            }

        }

        private static bool IsActiveRecordType(System.Type type)
        {
            foreach (object customAttribute in type.GetCustomAttributes(false))
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

        private static void GenerateSingleFile(TextReader input, string destinationFile, string baseNamespace)
        {
            QueryGenerator generator = new QueryGenerator(input, provider, baseNamespace);
            using (StreamWriter outputStream = File.CreateText(destinationFile))
            {
                generator.Generate(outputStream);
            }
            Console.WriteLine("Successfuly created file {0}", destinationFile);
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
