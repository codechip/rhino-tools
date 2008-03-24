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
using System.IO;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Criterion;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTests : RepositoryTestsBase
    {
        [SetUp]
        public  void TestInitialize()
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
            parentsInDb.Add(CreateExampleParentObject("Parent4", 800, new Child()));

            //verifying that the test objects above satisfy assumptions made in tests
            AssumedParentObjectNamesAreUnique(parentsInDb);

            SaveAndFlushToDatabase(parentsInDb);
        }


        private static void AssumedParentObjectNamesAreUnique(List<Parent> list)
        {
            foreach (Parent parent in list)
            {
                List<Parent> match = list.FindAll(delegate(Parent x) {
                                                      return x.Name == parent.Name;
                                                  });
                if (match.Count > 1) 
                    Assert.Fail("Assumed that parent names are unique");
            }
        }


        [Test]
        public void CanSaveAndGet()
        {
            UnitOfWork.CurrentSession.Clear();

            Parent loaded = Repository<Parent>.Get(parentsInDb[0].Id);

            Assert.AreEqual(parentsInDb[0].Name, loaded.Name);
            Assert.AreEqual(2, parentsInDb[0].Children.Count);
        }


        #region FindAll tests


        [Test]
        public void FindAllWillNotRemoveDuplicates()
        {
            ICollection<Parent> loaded = Repository<Parent>.FindAll(DetachedCriteria.For<Parent>());
            Assert.AreNotEqual(parentsInDb.Count, loaded.Count, "reference to a parent object for each child returned");

            //must remove duplicates manually like so:
            Assert.AreEqual(parentsInDb.Count, Collection.ToUniqueCollection(loaded).Count);
        }


        [Test]
        public void CanFindAllMatchingCriterion()
        {
            //create another object named Parent4
            CreateExampleParentObjectInDb("Parent4", 1);

            AbstractCriterion whereName = Expression.Eq("Name", "Parent4");
            ICollection<Parent> loaded = Repository<Parent>.FindAll(whereName);

            Assert.AreEqual(2, Collection.ToUniqueCollection(loaded).Count);
        }


        [Test]
        public void CanFindAllMatchingCriterionWithMultipleSortOrders()
        {
            Parent secondPosition = CreateExampleParentObjectInDb("ThisTestOnly", 3);
            Parent firstPosition = CreateExampleParentObjectInDb("ThisTestOnly", 9999);
            CreateExampleParentObjectInDb("NoMatch", 5);

            AbstractCriterion whereName = Expression.Eq("Name", "ThisTestOnly");
            ICollection<Parent> loaded = Repository<Parent>.FindAll(OrderByNameAndAgeDescending, whereName);

            AssertCollectionsEqual(ExpectedList(firstPosition, secondPosition), loaded);
        }


        [Test]
        public void CanFindAllMatchingCriteriaWithMultipleSortOrders()
        {
            Parent secondPosition = CreateExampleParentObjectInDb("ThisTestOnly", 3);
            Parent firstPosition = CreateExampleParentObjectInDb("ThisTestOnly", 9999);
            CreateExampleParentObjectInDb("NoMatch", 5);

            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(WhereNameEquals("ThisTestOnly"), OrderByNameAndAgeDescending);

            AssertCollectionsEqual(ExpectedList(firstPosition, secondPosition), loaded);
        }

        [Test]
        public void CanFindAllMatchingCriteriaWithMultipleSortOrders_Paginated()
        {
            CreateExampleParentObjectInDb("ThisTestOnly", 2);
            CreateExampleParentObjectInDb("ThisTestOnly", 1);
            Parent secondPosition = CreateExampleParentObjectInDb("ThisTestOnly", 4);
            Parent thirdPosition = CreateExampleParentObjectInDb("ThisTestOnly", 3);
            Parent firstPosition = CreateExampleParentObjectInDb("ThisTestOnly_First", 1);
            CreateExampleParentObjectInDb("X", 1);

            DetachedCriteria whereName = WhereNameIn("ThisTestOnly", "ThisTestOnly_First");
            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(whereName, 0, 3, OrderByNameAndAgeDescending);

            AssertCollectionsEqual(ExpectedList(firstPosition, secondPosition, thirdPosition), loaded);
        }


        [Test]
        public void CanFindAllMatchingCriterionWithMultipleSortOrders_Paginated()
        {
            CreateExampleParentObjectInDb("ThisTestOnly", 2);
            CreateExampleParentObjectInDb("ThisTestOnly", 1);
            Parent secondPosition = CreateExampleParentObjectInDb("ThisTestOnly", 4);
            Parent thirdPosition = CreateExampleParentObjectInDb("ThisTestOnly", 3);
            Parent firstPosition = CreateExampleParentObjectInDb("ThisTestOnly_First", 1);
            CreateExampleParentObjectInDb("X", 1);
            
            AbstractCriterion whereName = Expression.In("Name", new object[] { "ThisTestOnly", "ThisTestOnly_First" });
            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(0, 3, OrderByNameAndAgeDescending, whereName);

            AssertCollectionsEqual(ExpectedList(firstPosition, secondPosition, thirdPosition), loaded);
        }

        [Test]
        public void CanFindAllMatchingCriterion_Paginated()
        {
            CreateExampleParentObjectInDb("X", 1);
            CreateExampleParentObjectInDb("ThisTestOnly", 1);
            CreateExampleParentObjectInDb("ThisTestOnly", 2);
            CreateExampleParentObjectInDb("ThisTestOnly", 3);


            AbstractCriterion whereName = Expression.Eq("Name", "ThisTestOnly");
            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(0, 2, whereName);

            Assert.AreEqual(2, loaded.Count, "2 objects returned");
            Assert.AreEqual("ThisTestOnly", Collection.First(loaded).Name, "first expected object returned");
            Assert.AreEqual("ThisTestOnly", Collection.Last(loaded).Name, "second expected object returned");
        }



        #endregion


        [Test]
        public void FindOneWillReturnFirstInstancesMatchingWhereClause()
        {
            //setup
            IList<Parent> parents = LoadAll<Parent>();
            parents[0].Children[0].Name = "A";
            parents[0].Children[1].Name = "A";
            parents[1].Children[0].Name = "B";
            parents[1].Children[1].Name = "C";
            UnitOfWork.Current.Flush();

            //run test
            DetachedCriteria whereChildNameEquals_A = DetachedCriteria.For<Parent>()
                .CreateAlias("Children", "c")
                .Add(Expression.Eq("c.Name", "A"));

            Parent match = Repository<Parent>.FindOne(whereChildNameEquals_A);
            Assert.AreEqual(parents[0], match);
        }


        #region FindFirst tests


        [Test]
        public void CanFindFirstMatchingCriteria()
        {
            Parent match = Repository<Parent>.FindOne(WhereNameEquals("Parent1"));
            Assert.AreEqual(parentsInDb[0], match);
        }


        [Test]
        public void CanFindFirstMatchingCriterion()
        {
            Parent match = Repository<Parent>.FindOne(Expression.Eq("Name", "Parent1"));
            Assert.AreEqual(parentsInDb[0], match);
        }


        [Test]
        public void FindFirstWillReturnFirstInstanceInAnOrderedListMatchingWhereClause()
        {
            Parent match = Repository<Parent>.FindFirst(WhereNameIn("Parent1", "Parent4"), OrderByNameDecending);
            Assert.AreEqual(parentsInDb[2], match);
        }

        [Test]
        public void FindFirstWillReturnFirstInstanceInAnOrderedList()
        {
            Parent match = Repository<Parent>.FindFirst(Order.Asc("Name"));
            Assert.AreEqual(parentsInDb[0], match);
        }


        [Test, ExpectedException(typeof(NonUniqueResultException))]
        public void FindFirstWillThrowExceptionWhereMoreThanOneMatch()
        {
            //create another object named Parent1
            CreateExampleParentObjectInDb("Parent1", 1);

            Repository<Parent>.FindOne(WhereNameEquals("Parent1"));
        }



        #endregion

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

		[Test]
		public void CanUseRepositoryToCreateInstanceOfType()
		{
			Parent parent = Repository<Parent>.Create();	
			Assert.IsNotNull(parent);
		}

		[Test]
		public void CanUseRepositoryToCreateInstanceOfType_MappedToAbstract()
		{
			IoC.Container.Kernel.GetHandler(typeof (IRepository<IParent>))
				.AddCustomDependencyValue("ConcreteType", typeof (Parent));
			Parent parent = Repository<IParent>.Create() as Parent;
			Assert.IsNotNull(parent);
		}

        private Order OrderByNameDecending
        {
            get { return Order.Desc("Name"); }
        }


        private static Order[] OrderByNameAndAgeDescending
        {
            get { return new Order[] {Order.Desc("Name"), Order.Desc("Age")}; }
        }


        private DetachedCriteria WhereNameEquals(string nameToMatch)
        {
            return DetachedCriteria.For<Parent>().Add(Expression.Eq("Name", nameToMatch));
        }


        private DetachedCriteria WhereNameIn(params string[] names)
        {
            return DetachedCriteria.For<Parent>().Add(Expression.In("Name", names));
        }
    }
}
