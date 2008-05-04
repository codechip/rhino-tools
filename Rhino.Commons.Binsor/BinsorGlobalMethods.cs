namespace Rhino.Commons.Binsor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.CompilerServices;

	[CompilerGlobalScope]
	public class BinsorGlobalMethods
	{
		/// <summary>
		/// Get all the types that inherit from <typeparamref name="T"/> in
		/// all the assemblies that were passed.
		/// </summary>
		/// <typeparam name="T">The base type to look for</typeparam>
		/// <param name="assemblyNames">The assembly names.</param>
		/// <returns></returns>
		public static IEnumerable<Type> AllTypesBased<T>(params string[] assemblyNames)
		{
			return AllTypesInternal(assemblyNames, delegate(Type type)
			{
				return typeof(T).IsAssignableFrom(type);
			});
		}

		private static IEnumerable<Type> AllTypesInternal(string[] assemblyNames, Predicate<Type> match)
		{
			foreach (Assembly assembly in AllAssemblies(assemblyNames))
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsClass == false || type.IsAbstract)
						continue;
					if (match(type) == false)
						continue;
					yield return type;
				}
			}
		}

		public static IEnumerable<Type> AllTypesWithAttribute<T>(params string[] assemblyNames)
		{
			return AllTypesInternal(assemblyNames, delegate(Type type)
			{
				return type.IsDefined(typeof(T), true);
			});
		}

		/// <summary>
		/// Loads all the assemblies from the list, preferring to use the
		/// Load context, but loading using the LoadFrom context if needed
		/// </summary>
		/// <param name="assemblyNames">The assembly names.</param>
		public static IEnumerable<Assembly> AllAssemblies(params string[] assemblyNames)
		{
			List<Assembly> assemblies = new List<Assembly>();
			foreach (string assembly in assemblyNames)
			{
				try
				{
					if (assembly.Contains(".dll") || assembly.Contains(".dll"))
					{
						if (Path.GetDirectoryName(assembly) ==
							Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory))
						{
							assemblies.Add(Assembly.Load(Path.GetFileNameWithoutExtension(assembly)));

						}
						else // no choice but to use the LoadFile, with the different context :-(
						{
							assemblies.Add(Assembly.LoadFile(assembly));
						}
					}
					else
					{
						assemblies.Add(Assembly.Load(assembly));
					}
				}
				catch 
				{
					// ignoring this exception, because we can't load the dll
				}
			}
			return assemblies;
		}
	}
}