using Castle.MicroKernel;

namespace Rhino.Testing.Tests.AutoMocking
{
	public class NeedIKernel
	{
		private IKernel kernel;

		public IKernel Kernel
		{
			get { return kernel; }
		}

		public NeedIKernel(IKernel kernel)
		{
			this.kernel = kernel;
		}
	}
}