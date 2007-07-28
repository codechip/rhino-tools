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
using MbUnit.Framework;
using NHibernate.Expression;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTests : RepositoryTestsBase
    {
        [SetUp]
        public void TestInitialize()
        {
            CurrentContext.CreateUnitOfWork();
            CreateObjectsInDb();
        }


        [TearDown]
        public void TestCleanup()
        {
            CurrentContext.DisposeUnitOfWork();
        }


        protected void CreateObjectsInDb()
        {
            parentsInDb = new List<Parent>();

            parentsInDb.Add(CreateExampleParentObject("Parent1", 100, new Child(), new Child()));
            parentsInDb.Add(CreateExampleParentObject("Parent2", 200, new Child(), new Child()));

            SaveInCurrentSession(parentsInDb);
            FlushAndClearCurrentSession();
        }


        [Test]
        public void CanSaveAndGet()
        {
            Parent loaded = Repository<Parent>.Get(parentsInDb[0].Id);

            Assert.AreEqual(parentsInDb[0].Name, loaded.Name);
            Assert.AreEqual(2, parentsInDb[0].Children.Count);
        }


        [Test]
        public void FindAllWillNotRemoveDuplicates()
        {
            ICollection<Parent> loaded = Repository<Parent>.FindAll(DetachedCriteria.For<Parent>());
            Assert.AreEqual(parentsInDb[0].Children.Count + parentsInDb[1].Children.Count,
                            loaded.Count);

            //must remove duplicates manually like so:
            Assert.AreEqual(parentsInDb.Count, Collection.ToUniqueCollection(loaded).Count);
        }


        [Test]
        public void FindOneWillReturnFirstInstancesMatchingWhereClause()
        {
            //setup
            IList<Parent> parents = LoadAll<Parent>();
            parents[0].Children[0].Name = "A";
            parents[0].Children[1].Name = "A";
            parents[1].Children[0].Name = "B";
            parents[1].Children[1].Name = "C";
            FlushAndClearCurrentSession();

            //run test
            DetachedCriteria whereChildNameEquals_A = DetachedCriteria.For<Parent>()
                .CreateAlias("Children", "c")
                .Add(Expression.Eq("c.Name", "A"));

            Parent match = Repository<Parent>.FindOne(whereChildNameEquals_A);
            Assert.AreEqual(parents[0].Id, match.Id);
        }


        [Test]
        public void CanCountNumberOfObjectsPersistedInDb()
        {
            Assert.AreEqual(parentsInDb.Count, Repository<Parent>.Count());
        }


        [Test]
        public void CanCountNumberOfObjectsPersistedInDbMatchingWhereClause()
        {
            Assert.AreEqual(1, Repository<Parent>.Count(WhereNameEquals(parentsInDb[0].Name)));
        }

        [Test]
        public void CanDeleteAllObjectsPersistedInDb()
        {
            Repository<Parent>.DeleteAll();
            UnitOfWork.Current.Flush();
            Assert.AreEqual(0, Repository<Parent>.Count());
        }


        [Test]
        public void CanDeleteObjectsMatchingWhereClause()
        {
            Repository<Parent>.DeleteAll(WhereNameEquals(parentsInDb[0].Name));

            UnitOfWork.Current.Flush();
            Assert.AreEqual(parentsInDb.Count - 1, Repository<Parent>.Count());
        }


        [Test]
        public void ExistsReturnsTrueIfAtLeastOneInstanceFoundInDb()
        {
            Assert.IsTrue(Repository<Parent>.Exists());
        }


        [Test]
        public void ExistsReturnsTrueIfAtLeastOnInstancesInDbMatchesWhereClause()
        {
            Assert.IsFalse(Repository<Parent>.Exists(WhereNameEquals("No object with this name")));
            Assert.IsTrue(Repository<Parent>.Exists(WhereNameEquals(parentsInDb[0].Name)));
        }


        private DetachedCriteria WhereNameEquals(string nameToMatch)
        {
            return DetachedCriteria.For<Parent>().Add(Expression.Eq("Name", nameToMatch));
        }
    }



    [TestFixture]
    public class NHibernateRepositoryTests : RepositoryTests
    {
        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            string path =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Repository\Windsor.config"));
            FixtureInitialize(PersistenceFramework.NHibernate, path, MappingInfo.FromAssemblyContaining<Parent>());
        }
    }



    [TestFixture]
    public class ActiveRecordRepositoryTests : RepositoryTests
    {
        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            string path =
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Repository\Windsor-AR.config"));
            FixtureInitialize(PersistenceFramework.ActiveRecord, path, MappingInfo.FromAssemblyContaining<Parent>());
        }
    }
}