#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
