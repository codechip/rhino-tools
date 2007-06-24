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

namespace Rhino.Commons
{
	public static class UnitOfWork
	{
		public const string CurrentUnitOfWorkKey = "CurrentUnitOfWork.Key";

        private static IUnitOfWork globalNonThreadSafeUnitOfwork;
	    
	    /// <summary>
	    /// Mostly intended to support mocking of the unit of work.
	    /// NOT thread safe!
	    /// </summary>
	    public static void RegisterGlobalUnitOfWork(IUnitOfWork global)
	    {
            globalNonThreadSafeUnitOfwork = global;
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
			if(nestingOptions == UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork && 
			   existing != null)
			{
				existing.IncremementUsages();
				return existing;
			}
			IUnitOfWorkImplementor unitOfWorkImplementor = IoC.Resolve<IUnitOfWorkFactory>().Create(connection, existing);
			Local.Data[CurrentUnitOfWorkKey] = unitOfWorkImplementor;
			return unitOfWorkImplementor;
		}

	    /// <summary>
		/// The current unit of work.
		/// </summary>
		public static IUnitOfWork Current
		{
			get
			{
				IUnitOfWork unitOfWork = (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
				if (unitOfWork == null)
					throw new InvalidOperationException("You are not in a unit of work");
				return unitOfWork;
			}
		}

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <value>The current session.</value>
	    public static ISession CurrentSession
	    {
	        get
	        {
	            return IoC.Resolve<IUnitOfWorkFactory>().CurrentSession;
	        }
	    }

		/// <summary>
		/// Called internally to clear the current UoW and move to the previous one.
		/// </summary>
		internal static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork)
		{
			IUnitOfWorkImplementor previous = unitOfWork.Previous;
			Local.Data[CurrentUnitOfWorkKey] = previous;
		}
	}
}
