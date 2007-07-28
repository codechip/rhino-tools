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
    public class NHibernateTestFixtureBaseTests : TestFixtureBaseTests
    {
        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSqlCe()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSqlCe);
            VerifyCanCreateUseAndDisposeSession();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSqlCe_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSqlCe);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_SQLite()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.SQLite);
            VerifyCanCreateUseAndDisposeSession();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_SQLite_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.SQLite);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }
        
        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSql2005()
        {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSql2005);
            VerifyCanCreateUseAndDisposeSession();
        }


        [Test]
        public virtual void CanCreateUnitOfWorkContextFor_MsSql2005_IoC()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }

        [Test]
        public virtual void NewUnitOfWorkContextCreatedForDifferentDatabaseNames()
        {
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005, "TestDb1");
            VerifyCanCreateUnitOfWorkContextFor(WindsorFilePath, DatabaseEngine.MsSql2005, "TestDb2");

            Assert.AreEqual(2, Contexts.Count);
        }


        [Test]
        public void CanSwitchBetweenPersistentFrameworksWithinSameTest()
        {
            MappingInfo mappingInfo = MappingInfo.FromAssemblyContaining<AREntity>();

            FixtureInitialize(PersistenceFramework.NHibernate,
                              NHibernateWindsorFilePath,
                              DatabaseEngine.SQLite,
                              "",
                              mappingInfo);
            VerifyCanCreateUseAndDisposeUnitOfWork();

            FixtureInitialize(PersistenceFramework.ActiveRecord,
                              ActiveRecordWindsorFilePath,
                              DatabaseEngine.SQLite,
                              "",
                              mappingInfo);
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }



        protected override PersistenceFramework FrameworkToTest
        {
            get { return PersistenceFramework.NHibernate; }
        }


        protected override string AnotherWindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"..\..\ForTesting\AnotherWindsor-NH.config"));
            }
        }

        protected override string WindsorFilePath
        {
            get
            {
                return
                    Path.GetFullPath(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                     @"..\..\ForTesting\Windsor-NH.config"));
            }
        }
    }
}