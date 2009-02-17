namespace Rhino.Commons.Binsor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
    using System.Runtime.CompilerServices;

	[CompilerGlobalScope]
	public static class BinsorGlobalMethods
	{
		[Boo.Lang.Extension]
#if DOTNET35
		public static Type GetFirstInterface(this Type type)
#else 
        public static Type GetFirstInterface(Type type)
#endif
		{
			return GetFirstInterface(type, delegate { return true; });
		}

		[Boo.Lang.Extension]
#if DOTNET35
        public static Type GetFirstInterface(this Type type, Predicate<Type> match)
#else 
        public static Type GetFirstInterface(Type type, Predicate<Type> match)
#endif
		{
            Type[] interfaces = type.GetInterfaces();
			interfaces = Array.FindAll(interfaces, match);
			if(interfaces.Length!=1)
            {
                throw new InvalidOperationException(
                    "Could not find service interface for "+ type +" because it implements "+interfaces.Length +" interfaces matching the given predicate."+ Environment.NewLine + 
                    "GetFirstInterface() will only work on types implementing a single interface.");
            }
            return interfaces[0];
        }

	    /// <summary>
		/// Get all the types that inherit from <typeparamref name="T"/> in
		/// all the assemblies that were passed.
		/// </summary>
		/// <typeparam name="T">The base type to look for</typeparam>
		/// <param name="assemblyNames">The assembly names.</param>
		/// <returns></returns>
        public static TypeEnumerable AllTypesBased<T>(params string[] assemblyNames)
		{
            return new TypeEnumerable(AllTypesInternal(assemblyNames, delegate(Type type)
            {
                return typeof(T).IsAssignableFrom(type);
            }));
		}

        /// <summary>
        /// Get all the types in all the assemblies that were passed.
        /// </summary>
        /// <param name="assemblyNames">The assembly names.</param>
        /// <returns></returns>
        public static TypeEnumerable AllTypes(params string[] assemblyNames)
        {
            return new TypeEnumerable(AllTypesInternal(assemblyNames, delegate
            {
                return true;
            }));
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

        public static TypeEnumerable AllTypesWithAttribute<T>(params string[] assemblyNames)
		{
            return new TypeEnumerable(AllTypesInternal(assemblyNames, delegate(Type type)
            {
                return type.IsDefined(typeof(T), true);
            }));
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
