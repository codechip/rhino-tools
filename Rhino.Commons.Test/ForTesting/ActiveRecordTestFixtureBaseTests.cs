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
using System.IO;
using System.Reflection;
using MbUnit.Framework;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.ForTesting
{
    [TestFixture]
    public class ActiveRecordTestFixtureBaseTests : TestFixtureBaseTests
    {

        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_SQLite_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.SQLite);

            //not sure why we can't do this - my lack of ActiveRecord knowledge (Christian Crowhurst)
//            VerifyCanCreateUseAndDisposeSession();

            VerifyCanCreateUseAndDisposeUnitOfWork();
        }


        [Test]
        public void Limitation_MustUseTheSameDatabaseEngineWithinSolutionTests()
        {
            FixtureInitialize(PersistenceFramework.ActiveRecord,
                              WindsorFilePath,
                              DatabaseEngine.SQLite,
                              "",
                              MappingInfo.FromAssemblyContaining<AREntity>());

            try
            {
                FixtureInitialize(PersistenceFramework.ActiveRecord,
                                  WindsorFilePath,
                                  DatabaseEngine.MsSqlCe,
                                  "",
                                  MappingInfo.FromAssemblyContaining<AREntity>());
                Assert.Fail("Exception expected");
            }
            catch (InvalidOperationException e)
            {
                string messgae =
                    "ActiveRecord implementation of UnitOfWorkContext has been throttled so that every " +
                    "context created within a running application must use the same database engine. " +
                    "This limitation is a consequence of not being able to register an existing " +
                    "SessionFactory with the ActiveRecord framework.";
                Assert.AreEqual(messgae, e.Message);
            }
        }

        [Test]
        public void CreateSessionFailsUnexpectedly()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.SQLite);

            try
            {
                //this will fail - which is probably not expected
                CurrentContext.CreateSession();
                Assert.Fail("Exception was expected");
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Must be inside a scope for InMemory tests to work", e.Message);
            }
        }



        [Test]
        public void CanSwitchBetweenPersistentFrameworksWithinSameTest()
        {
            MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();

            FixtureInitialize(PersistenceFramework.ActiveRecord,
                              ActiveRecordWindsorFilePath,
                              DatabaseEngine.SQLite,
                              "",
                              mappingInfo);
            VerifyCanCreateUseAndDisposeUnitOfWork();

            FixtureInitialize(PersistenceFramework.NHibernate,
                  NHibernateWindsorFilePath,
                  DatabaseEngine.SQLite,
                  "",
                  mappingInfo);
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }


        protected override PersistenceFramework FrameworkToTest
        {
            get { return PersistenceFramework.ActiveRecord; }
        }


        protected override string AnotherWindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"..\..\ForTesting\AnotherWindsor-AR.config"));
            }
        }

        protected override string WindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"..\..\ForTesting\Windsor-AR.config"));
            }
        }
    }
}