using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp;

namespace Rhino.Proxy
{
	public class CodeDOMProxyBuilder
	{
		private ReaderWriterLock rwLock = new ReaderWriterLock();
		private Hashtable typesCache = new Hashtable();

		private Type GetProxiedType(
			Type baseClass, Type[] interfaces, ConstructorParameters ctorParams, GeneratorContext context)
		{
			rwLock.AcquireReaderLock(0);
			try
			{
				string name = ReflectionHelper.GetMangledTypeName(baseClass, interfaces);
				Type type = (Type) typesCache[name];
				if(type!=null)
					return type;
				rwLock.UpgradeToWriterLock(0);
				//Need to check again, because several threads may be waiting for this.
				//and we may grab a writer lock after another thread already go it.
				type = (Type)typesCache[name];
				if (type != null)
					return type;
				
				ClassBuilder builder = new ClassBuilder(context, baseClass, interfaces);
				CodeCompileUnit unit = builder.CreateCodeCompileUnit(ctorParams);
				type = GenerateCode(builder.FullName, unit, builder.ReferencedAssembliesLocations);
				typesCache.Add(name,type);
				return type;
			}
			finally
			{
				rwLock.ReleaseLock();
			}
		}


		public Type CreateInterfaceProxy(Type baseClass, Type[] interfaces)
		{
			return GetProxiedType(baseClass, interfaces, ConstructorParameters.InterceptorAndTarget, new GeneratorContext());
		}

		public Type CreateProxy(Type baseClass, Type[] interfaces)
		{
			return GetProxiedType(baseClass, interfaces, ConstructorParameters.InterceptorOnly, new GeneratorContext());
		}

		private string DEBUG_WriteCodeToFile_DEBUG(CSharpCodeProvider provider, CodeCompileUnit unit)
		{
			string defaultFile = "code.cs";
			StringWriter sw = new StringWriter();
			provider.GenerateCodeFromCompileUnit(unit, sw, new CodeGeneratorOptions());
			string code = sw.ToString();
			File.WriteAllText(defaultFile, code);
			return defaultFile;
		}

		private CompilerParameters CreateCompilerParameters(string[] assemblies)
		{
			CompilerParameters cp = new CompilerParameters();
			cp.OutputAssembly = "DynamicAssemblyProxyGen.dll";
			cp.GenerateInMemory = true;
			cp.IncludeDebugInformation = true;
			cp.ReferencedAssemblies.Add(typeof(IInterceptor).Assembly.Location);
			foreach (string assembly in assemblies)
			{
				cp.ReferencedAssemblies.Add(assembly);
			}
			return cp;
		}

		private Type GenerateCode(string fullClassName, CodeCompileUnit unit, string[] assembliesLocations)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerParameters cp = CreateCompilerParameters(assembliesLocations);
			string debug = DEBUG_WriteCodeToFile_DEBUG(provider, unit);
			CompilerResults results = provider.CompileAssemblyFromFile(cp, debug);
			if (results.Errors.HasErrors)
			{
				StringBuilder sb = new StringBuilder("Errors in compilation!");
				sb.AppendLine();
				foreach (CompilerError error in results.Errors)
				{
					sb.Append(error).AppendLine();
				}
				throw new CompilationException(sb.ToString());
			}
			Assembly compiledAssembly = results.CompiledAssembly;
			return compiledAssembly.GetType(fullClassName);
		}

		public Type CreateCustomClassProxy(Type baseClass, Type[] interfaces, GeneratorContext context)
		{
			return GetProxiedType(baseClass, interfaces, ConstructorParameters.InterceptorOnly, context);
		}

		public Type CreateCustomInterfaceProxy(Type[] interfaces, Type type, GeneratorContext context)
		{
			return GetProxiedType(type, interfaces, ConstructorParameters.InterceptorAndTarget, context);
		}
	}
}