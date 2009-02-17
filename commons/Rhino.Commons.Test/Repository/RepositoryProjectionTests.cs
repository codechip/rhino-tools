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
using Castle.ActiveRecord;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Criterion;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public abstract class RepositoryProjectionTests : RepositoryTestsBase
    {
        [SetUp]
        public void TestInitialize()
        {
            CurrentContext.CreateUnitOfWork();
            CreateExampleObjectsInDb();
        }


        [TearDown]
        public void TestCleanup()
        {
            CurrentContext.DisposeUnitOfWork();
        }


        protected void CreateExampleObjectsInDb() 
        {
            parentsInDb = new List<Parent>();

            parentsInDb.Add(CreateExampleParentObject("Parent1", 100, new Child(), new Child()));
            parentsInDb.Add(CreateExampleParentObject("Parent2", 200, new Child(), new Child()));
            parentsInDb.Add(CreateExampleParentObject("Parent3", 300, new Child(), new Child()));

            SaveAndFlushToDatabase(parentsInDb);
        }


        [Test]
        public void CanReportOneMatchingCriteria()
        {
            DetachedCriteria where = DetachedCriteria.For<Parent>()
                .Add(Expression.Eq("Name", "Parent1"));

            ParentDto dto = Repository<Parent>.ReportOne<ParentDto>(where, ProjectByNameAndAge);
            AssertDtoCreatedFrom(parentsInDb[0], dto);
        }


        [Test]
        public void CanReportOneMatchingCriterion()
        {
            SimpleExpression whereName = Expression.Eq("Name", "Parent1");
            SimpleExpression whereAge = Expression.Eq("Age", 100);

            ParentDto dto =
                Repository<Parent>.ReportOne<ParentDto>(ProjectByNameAndAge, whereName, whereAge);
            AssertDtoCreatedFrom(parentsInDb[0], dto);
        }


        [Test, ExpectedException(typeof (NonUniqueResultException))]
        public void ReportOneWillThrowIfMoreThanOneMatch()
        {
            SimpleExpression whereName = Expression.Like("Name", "Parent%");
            Repository<Parent>.ReportOne<ParentDto>(ProjectByNameAndAge, whereName);
        }


        [Test]
        public void CanReportAll()
        {
            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(ProjectByNameAndAge);

            Assert.AreEqual(parentsInDb.Count, dtos.Count);
            AssertDtosCreatedFrom(parentsInDb, dtos);
        }


        [Test]
        public void CanReportAllDistinct()
        {
            IList<Parent> parents = LoadAll<Parent>();
            parents[0].Age = parents[1].Age;
            parents[0].Name = parents[1].Name;
            UnitOfWork.Current.Flush();

            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(ProjectByNameAndAge, true);

            Assert.AreEqual(parents.Count - 1, dtos.Count);
        }


        [Test]
        public void CanReportAllWithSorting()
        {
            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(ProjectByNameAndAge, Order.Desc("Name"));

            AssertDtosCreatedFrom(parentsInDb, dtos);
            AssertDtosSortedByName(dtos, "Desc");
        }


        [Test]
        public void CanReportAllMatchingCriteria()
        {
            DetachedCriteria where = DetachedCriteria.For<Parent>()
                .Add(Expression.Eq("Name", "Parent2"));

            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(where, ProjectByNameAndAge);
            Assert.AreEqual(1, dtos.Count);
            AssertDtoCreatedFrom(parentsInDb[1], Collection.First(dtos));
        }


        [Test]
        public void CanReportAllMatchingCriteriaWithSorting()
        {
            DetachedCriteria where = DetachedCriteria.For<Parent>()
                .Add(Expression.Like("Name", "Parent%"));

            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(where,
                                                        ProjectByNameAndAge,
                                                        Order.Desc("Name"));

            Assert.AreEqual(parentsInDb.Count, dtos.Count);
            AssertDtosSortedByName(dtos, "Desc");
        }


        [Test]
        public void CanReportAllMatchingCriterion()
        {
            SimpleExpression whereName = Expression.Eq("Name", "Parent1");
            SimpleExpression whereAge = Expression.Eq("Age", 100);

            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(ProjectByNameAndAge, whereName, whereAge);
            Assert.AreEqual(1, dtos.Count);
            AssertDtoCreatedFrom(parentsInDb[0], Collection.First(dtos));
        }


        [Test]
        public void CanReportAllMatchingCriterionWithSorting()
        {
            SimpleExpression whereName = Expression.Like("Name", "Parent%");

            Order[] orderBy = {Order.Desc("Name")};
            ICollection<ParentDto> dtos =
                Repository<Parent>.ReportAll<ParentDto>(ProjectByNameAndAge,
                                                        orderBy,
                                                        whereName);
            Assert.AreEqual(parentsInDb.Count, dtos.Count);
            AssertDtosSortedByName(dtos, "Desc");
        }


        private static void AssertDtoCreatedFrom(Parent parent, ParentDto dto)
        {
            Assert.AreEqual(parent.Age, dto.Age);
            Assert.AreEqual(parent.Name, dto.Name);
        }


        private void AssertDtosCreatedFrom(IList<Parent> parents, ICollection<ParentDto> dtos)
        {
            foreach (Parent parent in parents)
            {
                Predicate<ParentDto> matchByNameAndAge =
                    delegate(ParentDto dto) { return dto.Age == parent.Age && dto.Name == parent.Name; };

                if (Collection.Find(dtos, matchByNameAndAge) == null)
                    Assert.Fail("Expected Dto not found");
            }
        }


        private void AssertDtosSortedByName(ICollection<ParentDto> dtos, string sortOrder)
        {
            Comparison<ParentDto> sortedByName = delegate(ParentDto x, ParentDto y) {
                                                     return x.Name.CompareTo(y.Name);
                                                 };
            AssertSorted(dtos, sortOrder, sortedByName);
        }


        private static ProjectionList ProjectByNameAndAge
        {
            get
            {
                return Projections.ProjectionList()
                    .Add(Projections.Property("Name"))
                    .Add(Projections.Property("Age"));
            }
        }
    }


    [TestFixture]
    public class ActiveRecordRepositoryProjectionTests : RepositoryProjectionTests
    {
        [TestFixtureSetUp]
        public override void OneTimeTestInitialize()
        {
            base.OneTimeTestInitialize();
            string path =
                Path.GetFullPath(@"Repository\Windsor-AR.config");
            InitializeNHibernateAndIoC(PersistenceFramework.ActiveRecord, path, MappingInfoForRepositoryTests);
        }
    }


    public class ParentSummaryDto
    {
        private string name;
        private int age;
        private int _numberOfChildren;


        public ParentSummaryDto(string name, int age, int numberOfChildren)
        {
            this.name = name;
            this.age = age;
            _numberOfChildren = numberOfChildren;
        }


        public int NumberOfChildren
        {
            get { return _numberOfChildren; }
            set { _numberOfChildren = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }


    public class ParentDto
    {
        private string name;
        private int age;


        public ParentDto(string name, int age)
        {
            this.name = name;
            this.age = age;
        }


        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
