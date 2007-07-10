using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using MbUnit.Framework;
using NHibernate.Expression;

namespace Rhino.Commons.Test.Repository
{
    [TestFixture]
    public class RepositoryTests : RepositoryTestsBase
    {
        [SetUp]
        public void TestInitialize()
        {
            CreateUnitOfWork();

            parentsInDb = new List<Parent>();

            parentsInDb.Add(CreateExampleParentObject("Parent1", 100));
            parentsInDb.Add(CreateExampleParentObject("Parent2", 200));

            SaveInCurrentSession(parentsInDb);
            FlushAndClearCurrentSession();
        }


        [TearDown]
        public void TestCleanup()
        {
            UnitOfWork.Current.Dispose();
        }


        [Test]
        public void CanSaveAndLoad()
        {
            Parent loaded = Repository<Parent>.Load(parentsInDb[0].Id);

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
            Assert.AreEqual(parentsInDb.Count, new HashedSet<Parent>(loaded).Count);
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

    public class Parent
    {
        private Guid id = Guid.NewGuid();
        private int version = -1;
        private string name;
        private int age;
        private IList<Child> children = new List<Child>();

        public virtual Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual int Version
        {
            get { return version; }
            set { version = value; }
        }

        public virtual int Age
        {
            get { return age; }
            set { age = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual IList<Child> Children
        {
            get { return children; }
            set { children = value; }
        }
    }


    public class Child
    {
        private Guid id = Guid.NewGuid();
        private int version = -1;
        private string name = "SomeChild";
        private Parent parent;


        public virtual Parent Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual int Version
        {
            get { return version; }
            set { version = value; }
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}