using System;

namespace Rhino.Commons
{
	public interface RhinoTransaction : IDisposable
	{
		void Commit();
		void Rollback();
	}
}