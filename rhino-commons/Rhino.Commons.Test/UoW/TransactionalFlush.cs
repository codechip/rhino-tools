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
using log4net;
using log4net.Appender;
using log4net.Config;
using MbUnit.Framework;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.UoW
{
    using Util;

    [TestFixture]
    public class TransactionalFlush : DatabaseTestFixtureBase
    {
        const string TransactionLog = "NHibernate.Transaction.AdoTransaction";

        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            //turn on log4net logging (and supress output to console)
            BasicConfigurator.Configure(new MemoryAppender());

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"UoW\Windsor.config"));
            InitializeNHibernateAndIoC(PersistenceFramework.NHibernate,
                                      path,
                                      DatabaseEngine.MsSqlCe,
                                      MappingInfo.FromAssemblyContaining<SimpleObject>());
        }


        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogManager.ResetConfiguration();
        }


        [SetUp]
        public void TestInitialize()
        {
            CurrentContext.CreateUnitOfWork();
        }

        [TearDown]
        public void TestCleanup()
        {
            //Cleanup the top level UnitOfWork
            CurrentContext.DisposeUnitOfWork();
        }


        [Test]
        public void WillFlushToDatabaseWithinTransactionAndDisposeOfTransactionAtEnd()
        {
            UnitOfWork.CurrentSession.Save(new SimpleObject());

            IList<string> logMessages = With.Log(TransactionLog, delegate 
            {   
                UnitOfWork.Current.TransactionalFlush();
            });


            bool hasCommited = false;
            foreach (string msg in logMessages)
            {
                if (msg.ToLower().Contains("commit"))
                    hasCommited  = true;
            }
            Assert.IsTrue(hasCommited);
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

            bool hasRolledBack = false  ;
            foreach (string msg in logMessages  )
            {
                if (msg.ToLower().Contains("rollback"))
                    hasRolledBack = true;
            }
            Assert.IsTrue(hasRolledBack);
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