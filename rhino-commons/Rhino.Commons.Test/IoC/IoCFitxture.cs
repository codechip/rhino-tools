using System;
using System.Collections.Generic;
using Castle.Windsor;
using NHibernate.Expression;
using MbUnit.Framework;

namespace Rhino.Commons.Test.IoCTests
{
    [TestFixture]
    public class IoCFitxture
    {
        [TearDown]
        public void TestCleanup()
        {
            IoC.Initialize(null);
        }
        
        [Test]
        public void LocalContainerOverideGlobalOne()
        {
            WindsorContainer container = new WindsorContainer();
            IoC.Initialize(container);
            Assert.AreSame(container,IoC.Container);
            WindsorContainer localContainer = new WindsorContainer();
            using(IoC.UseLocalContainer(localContainer))
            {
                Assert.AreSame(localContainer, IoC.Container);
            }
            Assert.AreSame(container, IoC.Container);
        }
    	
    }
}
    