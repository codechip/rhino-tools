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
using System.IO;
using log4net.Appender;
using log4net.Config;
using MbUnit.Framework;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.UoW
{
    [TestFixture]
    public class TransactionalFlush : NHibernateEmbeddedDBTestFixtureBase
    {
        const string TransactionLog = "NHibernate.Transaction.AdoTransaction";

        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            sessionFactory = null;

            //turn on log4net logging (and supress output to console)
            BasicConfigurator.Configure(new MemoryAppender());

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\UoW\Windsor.config"));
            FixtureInitialize(path, typeof(SimpleObject).Assembly);
        }

        [SetUp]
        public void TestInitialize()
        {
            CreateUnitOfWork();
        }

        [TearDown]
        public void TestCleanup()
        {
            //Cleanup the top level UnitOfWork
            UnitOfWork.Current.Dispose();
        }


        [Test]
        public void WillFlushToDatabaseWithinTransactionAndDisposeOfTransactionAtEnd()
        {
            UnitOfWork.CurrentSession.Save(new SimpleObject());

            IList<string> logMessages = With.Log(TransactionLog, delegate 
            {   
                UnitOfWork.Current.TransactionalFlush();
            });

            Assert.AreEqual("begin", logMessages[0]);
            Assert.AreEqual("commit", logMessages[1]);
            Assert.Contains("running AdoTransaction.Dispose()", logMessages[2]);
        }


        [Test]
        public void WillRollbackAndDisposeTransactionIfFlushThrows()
        {
            SimpleObject obj = new SimpleObject();
            obj.TwoCharactersMax = "This string is too big";
            UnitOfWork.CurrentSession.Save(obj);

            IList<string> logMessages = With.Log(TransactionLog, delegate 
            {
                try
                {
                    UnitOfWork.Current.TransactionalFlush();
                }
                catch
                {
                    ;
                }
            });

            Assert.AreEqual("begin", logMessages[0]);
            Assert.AreEqual("commit", logMessages[1]); //fails
            Assert.AreEqual("rollback", logMessages[2]);
            Assert.Contains("running AdoTransaction.Dispose()", logMessages[3]);
        }


        [Test]
        public void WillNotStartTransactionIfAlreadyStarted()
        {
            UnitOfWork.Current.BeginTransaction();

            IList<string> logMessages = With.Log(TransactionLog, delegate
            {
                UnitOfWork.Current.TransactionalFlush();
            });

            Assert.NotIn("begin", logMessages);
        }
    }

    public class SimpleObject
    {
        private string twoCharacterMax;
        private Guid id = Guid.NewGuid();
        private int concurrencyId = -1;


        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public int ConcurrencyId
        {
            get { return concurrencyId; }
            set { concurrencyId = value; }
        }

        public string TwoCharactersMax
        {
            get { return twoCharacterMax; }
            set { twoCharacterMax = value; }
        }
    }
}