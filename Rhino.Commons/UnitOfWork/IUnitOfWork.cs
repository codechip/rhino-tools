using System;
using System.Data;
using NHibernate;

namespace Rhino.Commons
{
	public interface IUnitOfWork : IDisposable
	{
		void Flush();
		bool IsInActiveTransaction { get; }

		RhinoTransaction BeginTransaction();
		RhinoTransaction BeginTransaction(IsolationLevel isolationLevel);
	}
}