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
using Castle.ActiveRecord;

namespace Rhino.Commons
{
	public class ActiveRecordUnitOfWorkAdapter : BaseUnitOfWorkFactory, IActiveRecordUnitOfWork
	{
		private ISessionScope scope;
		private IUnitOfWorkImplementor previous;
		private int usageCount = 1;
		private ActiveRecordTransactionAdapter transactionAdapter;

		public ActiveRecordUnitOfWorkAdapter(ISessionScope scope, IUnitOfWorkImplementor previous)
		{
			this.scope = scope;
			this.previous = previous;
		}

		public bool IsInActiveTransaction
		{
			get
			{
				return transactionAdapter != null && transactionAdapter.IsActive;
			}
		}

		public void IncrementUsages()
		{
			usageCount++;
		}

		public IUnitOfWorkImplementor Previous
		{
			get { return previous; }
		}

		public ISessionScope Scope
		{
			get { return scope; }
		}

		public void Flush()
		{
			scope.Flush();
		}

		public RhinoTransaction BeginTransaction()
		{
			return BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public RhinoTransaction BeginTransaction(IsolationLevel isolationLevel)
		{
			TransactionScope transactionScope = new TransactionScope(TransactionMode.New, isolationLevel, OnDispose.Rollback);
			transactionAdapter = new ActiveRecordTransactionAdapter(transactionScope);
			return transactionAdapter;
		}


	    ///<summary>
		///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///</summary>
		///<filterpriority>2</filterpriority>
		public void Dispose()
		{
			usageCount -= 1;
			if (usageCount != 0)
				return;
			UnitOfWork.DisposeUnitOfWork(this);
			scope.Dispose();
		}
	}
}
