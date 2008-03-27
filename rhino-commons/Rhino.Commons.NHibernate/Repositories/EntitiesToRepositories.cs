using System;
using System.Reflection;
using Castle.Windsor;
using NHibernate;
using NHibernate.Metadata;
using Castle.MicroKernel.Registration;

namespace Rhino.Commons.NHibernate.Repositories
{
	/// <summary>
	/// This class takes the responsability of inspecting NHibernate's entities
	/// and extracting the relevant domain interfaces as separate <see cref="IRepository{T}"/> 
	/// services.
	/// </summary>
	public class EntitiesToRepositories
	{
		/// <summary>
		/// Registers the interfaces of the entities defined in the session factory in the container.
		/// </summary>
		/// <param name="windsorContainer">The windsor container.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <param name="repository">The repository type to map to <seealso cref="IRepository{T}"/>.</param>
		/// <param name="assemblies">The assemblies in which domain interfaces are defined</param>
		/// <remarks>
		/// The reason that we have the <paramref name="assemblies"/> collection is to avoid registering services
		/// for interfaces that are not related to the domain (INotifyPropertyChanged, as a good example).
		/// </remarks>
		public static void Register(
			IWindsorContainer windsorContainer,
			ISessionFactory sessionFactory,
			Type repository,
			params Assembly[] assemblies
			)
		{
			if(typeof(IRepository<>).IsAssignableFrom(repository))
				throw new ArgumentException("Repository must be a type inheriting from IRepository<T>, and must be an open generic type. Sample: typeof(NHRepository<>).");

			foreach (IClassMetadata meta in sessionFactory.GetAllClassMetadata().Values)
			{
				Type mappedClass = meta.GetMappedClass(EntityMode.Poco);
				if (mappedClass == null)
					continue;
				foreach (Type interfaceType in mappedClass.GetInterfaces())
				{
					if(IsDefinedInAssemblies(interfaceType, assemblies)==false)
						continue;
					windsorContainer.Register(
						Component.For(typeof(IRepository<>).MakeGenericType(interfaceType))
							.ImplementedBy(repository.MakeGenericType(interfaceType))
							.CustomDependencies(Property.ForKey("ConcreteType").Eq(mappedClass))
						);
				}
			}
		}

		private static bool IsDefinedInAssemblies(Type type, Assembly[] assemblies)
		{
			return Array.IndexOf(assemblies, type.Assembly) != -1;
		}
	}
}