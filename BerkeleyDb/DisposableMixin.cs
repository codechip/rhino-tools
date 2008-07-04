using System;

namespace BerkeleyDb
{
	public abstract class DisposableMixin : IDisposable
	{
		public bool IsDisposed { get; private set; }

		public void Dispose()
		{
			Dispose(true);
			IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		protected abstract void Dispose(bool disposing);


		~DisposableMixin()
		{
			Dispose(false);
		}

		
	}
}