using NUnit.Framework;

namespace Rhino.Proxy.Tests
{
    [TestFixture]
    public class ProxyGeneratorFixture
    {
        [Test]
        public void CanCreateProxyOfInterface()
        {
            ProxyGenerator pg = new ProxyGenerator();
            IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), new TestInterceptor());
            Assert.IsNotNull(demo, "Should have gotten not null instnace");
        }

        [Test]
        public void CanInterceptCalls()
        {
            ProxyGenerator pg = new ProxyGenerator();
            TestInterceptor testInterceptor = new TestInterceptor();
			IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), testInterceptor);
            demo.Foo();
            Assert.AreEqual(1, testInterceptor.Invocations.Count);
        }

        [Test]
        public void CanInterceptArguments()
        {
            ProxyGenerator pg = new ProxyGenerator();
            TestInterceptor testInterceptor = new TestInterceptor();
			IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), testInterceptor);
            demo.WithArg("foo");
            InvocationAndArgs invocation = (InvocationAndArgs)testInterceptor.Invocations[0];
            Assert.AreEqual("foo", invocation.Args[0]);
        }

        [Test]
        public void CanChangeReturnValue()
        {
            ProxyGenerator pg = new ProxyGenerator();
            TestInterceptor testInterceptor = new TestInterceptor();
            testInterceptor.returnValue = "bar";
			IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), testInterceptor);
            string ret = demo.Ret();
            Assert.AreEqual("bar", ret );
            testInterceptor.returnValue = "foo";
            ret = demo.Ret();
            Assert.AreEqual("foo", ret );
        }

        [Test]
        public void CanOverrideOutParam()
        {
            ProxyGenerator pg = new ProxyGenerator();
            TestInterceptor testInterceptor = new TestInterceptor();
            testInterceptor.returnValue = "bar";
			IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), testInterceptor);
            int i = 5;
            testInterceptor.setArgs = new object[]{ 10 };
            demo.WithOutParam(out i);
            Assert.AreEqual(10, i);
        }

        [Test]
        public void CanOverrideRefParam()
        {
            ProxyGenerator pg = new ProxyGenerator();
            TestInterceptor testInterceptor = new TestInterceptor();
            testInterceptor.returnValue = "bar";
			IDemo demo = (IDemo)pg.CreateProxy(typeof(IDemo), testInterceptor);
            int i = 5;
            testInterceptor.setArgs = new object[] { 10 };
            demo.WithRefParam(ref i);
            Assert.AreEqual(10, i);
        }
    }
}
