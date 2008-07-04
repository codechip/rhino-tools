using System;
using System.Threading;

namespace BerkeleyDb
{
	public class EnvContainer : DisposableMixin
	{
		private readonly Env inner;
		private int refCount;

		public EnvContainer(Env inner)
		{
			this.inner = inner;
		}

		public Env AddRef()
		{
			if(IsDisposed)
				throw new ObjectDisposedException("EnvContainer");
			Interlocked.Increment(ref refCount);
			return inner;
		}

		public bool Release()
		{
			var value = Interlocked.Decrement(ref refCount);
			if(value==0)
			{
				Dispose();
				return true;
			}
			return false;
		}

		protected override void Dispose(bool disposing)
		{
			inner.Dispose();
		}
	}
}