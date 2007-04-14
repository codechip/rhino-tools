using Rhino.Igloo;

namespace Rhino.Igloo.Tests
{
	public class FakeContextProvider : IContextProvider
	{
		private static IContext current;

		public static void SetTheCurrentContext(IContext theCurrentContext)
		{
			current = theCurrentContext;
		}

		public IContext Current
		{
			get { return current; }
		}
	}
}
