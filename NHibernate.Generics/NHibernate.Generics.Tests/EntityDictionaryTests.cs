using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace NHibernate.Generics.Tests
{
    [TestFixture]
    public class EntityDictionaryTests
    {
        Blog fun;

        ISessionFactory factory;
        ISession session;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            factory = DatabaseTests.CreateSessionFactory();
        }

        [SetUp]
        public void Setup()
        {
            session = factory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            session.Dispose();
        }
        [Test]
        public void SaveValuesToDicAndThenLoad()
        {
            fun = new Blog("Fun");
            fun.Attributes.Add("IsTest", "True");
            session.Save(fun);

            session.Dispose();

            session = factory.OpenSession();

            Blog fromDb = (Blog)session.Load(typeof(Blog), fun.BlogID);
            Assert.AreEqual(1, fun.Attributes.Count);
            Assert.AreEqual("True", fun.Attributes["IsTest"]);
        }

        [Test]
        public void EnumeratingEntityDictionary()
        {
            fun = new Blog("Fun");
            fun.Attributes.Add("IsTest", "True");
            bool enteredLoop = false;
            foreach (KeyValuePair<string, string> pair in fun.Attributes)
            {
                enteredLoop=true;
                Assert.AreEqual("IsTest",pair.Key);
                Assert.AreEqual("True", pair.Value);
            }
            Assert.IsTrue(enteredLoop);
        }
    
    }
}
