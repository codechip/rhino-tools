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
using System.Reflection;
using Castle.Windsor;
using NHibernate;
using NHibernate.Metadata;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace Rhino.Commons
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
			Register(windsorContainer.Kernel, sessionFactory, repository, isCandidateForRepository);
		}

		/// <summary>
		/// Registers the interfaces of the entities defined in the session factory in the kernel.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="sessionFactory">The session factory.</param>
		/// <param name="repository">The repository type to map to <seealso cref="IRepository{T}"/>.</param>
		/// <param name="isCandidateForRepository">The is candidate for repository.</param>
		/// <remarks>
		/// The reason that we have the <paramref name="isCandidateForRepository"/> is to avoid registering services
		/// for interfaces that are not related to the domain (INotifyPropertyChanged, as a good example).
		/// </remarks>
		public static void Register(
			IKernel kernel,
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
					kernel.Register(
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