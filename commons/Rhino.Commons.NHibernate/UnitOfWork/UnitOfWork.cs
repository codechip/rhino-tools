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
using System.Data;
using NHibernate;
using Rhino.Commons.HttpModules;

namespace Rhino.Commons
{
	public static class UnitOfWork
	{
		public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";
		public const string CurrentLongConversationIdKey = "CurrentLongConversationId.Key";
		public const string CurrentLongPrivateKey = "CurrentLongPrivate.Key";


		private static IUnitOfWork globalNonThreadSafeUnitOfwork;

		/// <summary>
		/// Signals the start of an application/user transaction that spans multiple page requests
		/// </summary>
		/// <remarks>
		/// Used in conjunction with <see cref="UnitOfWorkApplication"/>, will ensure that the current UoW
		/// (see <see cref="Current"/>) is kept intact across multiple page requests. 
		/// <para>
		/// Note: This method does not start a physical database transaction.
		/// </para>
		/// </remarks>
		public static void StartLongConversation()
		{
			if (InLongConversation)
				throw new InvalidOperationException("You are already in a long conversation");

			Local.Data[CurrentLongConversationIdKey] = Guid.NewGuid();
		}

		/// <summary>
		/// Signals the start of an application/user transaction that spans multiple page requests, 
		/// but is not loaded without explicitly specifying the conversation key.
		/// </summary>
		/// <remarks>
		/// Used in conjunction with <see cref="UnitOfWorkApplication"/>, will ensure that the current UoW
		/// (see <see cref="Current"/>) is kept intact across multiple page requests. Review the <see cref="LongConversationManager"/> for details.
		/// <para>
		/// Note: This method does not start a physical database transaction.
		/// </para>
		/// </remarks>
		public static Guid StartPrivateConversation()
		{
			if (InLongConversation)
				throw new InvalidOperationException("You are already in a long conversation");
			LongConversationIsPrivate = true;
			return (Guid)(Local.Data[CurrentLongConversationIdKey] = Guid.NewGuid());
		}

		/// <summary>
		/// Signals the end of the current application/user transaction <seealso cref="StartLongConversation"/>
		/// </summary>
		/// <remarks>
		/// Actual disposal of the current UoW is deferred until the end the current page request 
		/// </remarks>
		public static void EndLongConversation()
		{
			Local.Data[CurrentLongConversationIdKey] = null;
		}


		public static bool InLongConversation
		{
			get { return CurrentLongConversationId != null; }
		}

		public static bool IsStarted
		{
			get
			{
				if (globalNonThreadSafeUnitOfwork != null)
					return true;
				return Local.Data[CurrentUnitOfWorkKey] != null;
			}
		}

		public static Guid? CurrentLongConversationId
		{
			get { return (Guid?)Local.Data[CurrentLongConversationIdKey]; }
			internal set { Local.Data[CurrentLongConversationIdKey] = value; }
		}

		public static bool LongConversationIsPrivate
		{
			get { return Local.Data[CurrentLongPrivateKey] != null && ((bool)Local.Data[CurrentLongPrivateKey]); }
			internal set { Local.Data[CurrentLongPrivateKey] = value; }
		}

		public static IDisposable SetCurrentSessionName(string name)
		{
			return IoC.Resolve<IUnitOfWorkFactory>().SetCurrentSessionName(name);
		}

		/// <summary>
		/// NOT thread safe! Mostly intended to support mocking of the unit of work. 
		/// You must pass a null argument when finished  to ensure atomic units of work UnitOfWorkRegisterGlobalUnitOfWork(null);
		/// You can also call Dispose() on the result of this method, or put it in a using statement (preferred)
		/// </summary>
		public static IDisposable RegisterGlobalUnitOfWork(IUnitOfWork global)
		{
			globalNonThreadSafeUnitOfwork = global;
			return new DisposableAction(delegate
			{
				globalNonThreadSafeUnitOfwork = null;
			});
		}

		public static IUnitOfWork Start(UnitOfWorkNestingOptions nestingOptions)
		{
			return Start(null, nestingOptions);
		}

		public static IUnitOfWork Start()
		{
			return Start(null, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
		}

		public static IUnitOfWork Start(IDbConnection connection)
		{
			return Start(connection, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
		}

		/// <summary>
		/// Start a Unit of Work
		/// is called
		/// </summary>
		/// <returns>
		/// An IUnitOfwork object that can be used to work with the current UoW.
		/// </returns>
		public static IUnitOfWork Start(IDbConnection connection, UnitOfWorkNestingOptions nestingOptions)
		{
			if (globalNonThreadSafeUnitOfwork != null)
				return globalNonThreadSafeUnitOfwork;
			IUnitOfWorkImplementor existing = (IUnitOfWorkImplementor)Local.Data[CurrentUnitOfWorkKey];
			if (nestingOptions == UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork &&
				existing != null)
			{
				existing.IncrementUsages();
				return existing;
			}
			Current = IoC.Resolve<IUnitOfWorkFactory>().Create(connection, existing);
			foreach (IUnitOfWorkAware workAware in IoC.ResolveAll<IUnitOfWorkAware>())
				workAware.UnitOfWorkStarted(Current);
			return Current;
		}

		/// <summary>
		/// The current unit of work.
		/// </summary>
		public static IUnitOfWork Current
		{
			get
			{
				if (!IsStarted)
					throw new InvalidOperationException("You are not in a unit of work");
				return globalNonThreadSafeUnitOfwork ?? (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
			}
			internal set { Local.Data[CurrentUnitOfWorkKey] = value; }
		}

		/// <summary>
		/// Gets the current session.
		/// </summary>
		/// <value>The current session.</value>
		public static ISession CurrentSession
		{
			get { return IoC.Resolve<IUnitOfWorkFactory>().CurrentSession; }
			internal set { IoC.Resolve<IUnitOfWorkFactory>().CurrentSession = value; }
		}

		/// <summary>
		/// Gets the current session. Must be used when a MultiplNHibernateUnitOfWorkFactory is used
		/// </summary>
		/// <param name="typeOfEntity">the concrete type of entity mapped in hbm files</param>
		/// <value>The current session for this entity.</value>
		public static ISession GetCurrentSessionFor(Type typeOfEntity)
		{
			return IoC.Resolve<IUnitOfWorkFactory>().GetCurrentSessionFor(typeOfEntity);
		}

		public static ISession GetCurrentSessionFor(string name)
		{
			return IoC.Resolve<IUnitOfWorkFactory>().GetCurrentSessionFor(name);
		}


		/// <summary>
		/// Sets the current session. Must be used when a MultiplNHibernateUnitOfWorkFactory is used
		/// </summary>
		/// <param name="typeOfEntity">the concrete type of entity mapped in hbm files</param>
		/// <param name="session">the session to set</param>
		public static void SetCurrentSessionFor(Type typeOfEntity, ISession session)
		{
			IoC.Resolve<IUnitOfWorkFactory>().SetCurrentSession(typeOfEntity, session);
		}


		/// <summary>
		/// Called internally to clear the current UoW and move to the previous one.
		/// </summary>
		public static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork)
		{
			IUnitOfWorkAware[] awareImplmenters = IoC.ResolveAll<IUnitOfWorkAware>();
			foreach (IUnitOfWorkAware workAware in awareImplmenters)
				workAware.UnitOfWorkDisposing(Current);
			
			IUnitOfWork disposed = null;
			if(IsStarted) disposed = Current;
			Current = unitOfWork.Previous;
			
			foreach (IUnitOfWorkAware workAware in awareImplmenters)
				workAware.UnitOfWorkDisposed(disposed);
		}
        /// <summary>
        /// Called when finished with UnitOfWorkFactory to explicately close the SessionFactory
        /// </summary>
        public static void DisposeUnitOfWorkFactory()
        {
            IoC.Resolve<IUnitOfWorkFactory>().Dispose();
        }
	}
}
