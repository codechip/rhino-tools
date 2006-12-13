using System;

namespace Rhino.Commons
{
	public interface ITransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}
}