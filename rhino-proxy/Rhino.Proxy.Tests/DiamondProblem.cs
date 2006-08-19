using System;
using System.Text;
using NUnit.Framework;

namespace Rhino.Proxy.Tests
{
    [TestFixture]
    public class DiamondProblem
    {
        public interface IFirst
        {
            void OriginalMethod1();
            void OriginalMethod2();
        }
        public class FirstImpl : IFirst
        {
            // NON-virtual method
            public void OriginalMethod1() { }
            // VIRTUAL method
            public virtual void OriginalMethod2() { }
        }
        public interface ISecond : IFirst
        {
            void ExtraMethod();
        }

        [Test]
        public void CreateProxyOfTypeWithMixinCausingDiamondWhenMethodIsNonVirtual()
        {
            ProxyGenerator generator = new ProxyGenerator();
            object proxy = generator.CreateClassProxy(typeof(FirstImpl), new Type[]{typeof(ISecond)},
                                                   new StandardInterceptor()); 
            Assert.IsTrue(proxy is FirstImpl);
            Assert.IsTrue(proxy is IFirst);
            Assert.IsTrue(proxy is ISecond);
        }
    }
}
