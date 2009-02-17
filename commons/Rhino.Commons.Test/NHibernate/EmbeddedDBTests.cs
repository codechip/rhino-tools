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

using System.Collections.Generic;
using NHibernate;
using MbUnit.Framework;
using NHibernate.Criterion;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.NHibernate
{
	[TestFixture]
    public class EmbeddedDBTests : DatabaseTestFixtureBase
    {
        private ISession session;
        private SMS sms;
		
		[TestFixtureSetUp]
		public void OneTimeTestInitialize()
		{
			InitializeNHibernateAndIoC(PersistenceFramework.NHibernate, "", DatabaseEngine.SQLite, MappingInfo.FromAssemblyContaining<SMS>());
		}

        [SetUp]
        public void TestInitialize()
        {
            session = CurrentContext.CreateSession();
            sms = new SMS();
            this.sms.Message = "R U There?";
            session.Save(this.sms);
            session.Flush();

        }

        [TearDown]
        public void TestCleanup()
        {
            CurrentContext.DisposeSession(session);
        }

        [Test]
        public void CanSaveAndLoadSMS()
        {

            session.Evict(sms);//remove from session cache

            SMS loaded = session.Load<SMS>(sms.Id);
            Assert.AreEqual(sms.Message, loaded.Message);
        }

        [Test]
        public void CanUseCriteriaBatch()
        {
            ICollection<SMS> loadedMSGs = null;
            new CriteriaBatch(session)
                .Add(DetachedCriteria.For<SMS>(), Order.Asc("id"))
                .OnRead<SMS>(delegate(ICollection<SMS> msgs) { loadedMSGs = msgs; })
                .Execute();
            Assert.IsNotNull(loadedMSGs);
        }

        [Test]
        public void CanUseCriteriaBatchForUniqueResult()
        {
            ICollection<SMS> loadedMSGs = null;
            SMS loadedMsg = null;
            new CriteriaBatch(session)
                .Add(DetachedCriteria.For<SMS>(), Order.Asc("id"))
                    .OnRead<SMS>(delegate(ICollection<SMS> msgs) { loadedMSGs = msgs; })
                .Add(DetachedCriteria.For<SMS>())
                    .Paging(0, 1)
                    .OnRead<SMS>(delegate(SMS msg) { loadedMsg = msg; })
                .Execute();
            Assert.IsNotNull(loadedMSGs);
            Assert.IsNotNull(loadedMsg);
        }

        [Test]
        public void CanUseCriteriaBatchWithAutomaticCountQuery()
        {
            ICollection<SMS> loadedMSGs = null;
            int msg_count = 0;
            SMS loadedMsg = null;
            new CriteriaBatch(session)
             .Add(DetachedCriteria.For<SMS>(), Order.Asc("id"))
                 .OnRead<SMS>(delegate(ICollection<SMS> msgs, int count) { loadedMSGs = msgs;
                                                                             msg_count = count;})
             .Add(DetachedCriteria.For<SMS>())
                 .Paging(0, 1)
                  .OnRead<SMS>(delegate(SMS msg) { loadedMsg = msg; })
            .Execute();
            Assert.IsNotNull(loadedMSGs);
            Assert.AreEqual(1, msg_count);
            Assert.IsNotNull(loadedMsg);

        }		
    }

    public class SMS
    {
        int id;
        string message;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }	
}
