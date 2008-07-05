using System;
using System.Threading;

namespace BerkeleyDb
{
	public class EnvContainer : DisposableMixin
	{
		private readonly Env inner;
		private int refCount;

		public bool CanDispose
		{
			get
			{
				Thread.MemoryBarrier();
				return refCount == 0;
			}
		}

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
			return value==0;
		}

		protected override void Dispose(bool disposing)
		{
			inner.Dispose();
		}
	}
}