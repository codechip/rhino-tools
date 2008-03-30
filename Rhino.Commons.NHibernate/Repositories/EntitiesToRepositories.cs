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
		/// <param name="isCandidateForRepository">The is candidate for repository.</param>
		/// <remarks>
		/// The reason that we have the <paramref name="isCandidateForRepository"/> is to avoid registering services
		/// for interfaces that are not related to the domain (INotifyPropertyChanged, as a good example).
		/// </remarks>
		public static void Register(
			IWindsorContainer windsorContainer,
			ISessionFactory sessionFactory,
			Type repository,
			Predicate<Type> isCandidateForRepository
			)
		{
			if (ImplementsOpenIRepository(repository) == false)
				throw new ArgumentException("Repository must be a type inheriting from IRepository<T>, and must be an open generic type. Sample: typeof(NHRepository<>).");

			foreach (IClassMetadata meta in sessionFactory.GetAllClassMetadata().Values)
			{
				Type mappedClass = meta.GetMappedClass(EntityMode.Poco);
				if (mappedClass == null)
					continue;
				foreach (Type interfaceType in mappedClass.GetInterfaces())
				{
					if (isCandidateForRepository(interfaceType) == false)
						continue;
					windsorContainer.Register(
						Component.For(typeof(IRepository<>).MakeGenericType(interfaceType))
							.ImplementedBy(repository.MakeGenericType(interfaceType))
							.DependsOn(Property.ForKey("ConcreteType").Eq(mappedClass))
						);
				}
			}
		}

		/// <summary>
		/// I hate open generic types in the CLR
		/// </summary>
		private static bool ImplementsOpenIRepository(Type repository)
		{
			if (repository.IsGenericTypeDefinition == false)
				return false;
			foreach (Type type in repository.GetInterfaces())
			{
				if (type.GetGenericTypeDefinition() == typeof(IRepository<>))
					return true;
			}
			return false;
		}
	}
}