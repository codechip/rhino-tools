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
using System.Text;
using Castle.ActiveRecord;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTestsBase : DatabaseTestFixtureBase
    {
        public virtual void OneTimeTestInitialize()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        protected List<Parent> parentsInDb;

        protected static MappingInfo MappingInfoForRepositoryTests
        {
            get
            {
                return MappingInfo.FromAssemblyContaining<Parent>();
            }
        }


        protected void SaveInCurrentSession(object toSave)
        {
            SaveInCurrentSession(new object[] { toSave });
        }


        protected void SaveInCurrentSession<T>(IEnumerable<T> objectToSave) where T : class
        {
            foreach (T obj in objectToSave)
                UnitOfWork.CurrentSession.Save(obj);
        }


        protected void SaveAndFlushToDatabase(object toSave) 
        {
            SaveInCurrentSession(toSave);
            UnitOfWork.Current.Flush();
        }

        protected void SaveAndFlushToDatabase<T>(IEnumerable<T> objectToSave) where T: class
        {
            SaveInCurrentSession(objectToSave);
            UnitOfWork.Current.Flush();
        }


        public static Parent CreateExampleParentObject(string parentName, int age, params Child[] children)
        {
            Parent parent = new Parent();
            parent.Name = parentName;
            parent.Age = age;

            foreach (Child child in children)
            {
                child.Parent = parent;
                parent.Children.Add(child); 
            }

            return parent;
        }


        protected Parent CreateExampleParentObjectInDb(string name, int age)
        {
            Parent parent = CreateExampleParentObject(name, age);
            SaveAndFlushToDatabase(parent);
            return parent;
        }


        protected static List<T> ExpectedList<T>(params T[] items) 
        {
            List<T> expected = new List<T>();
            expected.AddRange(items);
            return expected;
        }


        protected static IList<T> LoadAll<T>()
        {
            DetachedCriteria criteria = DetachedCriteria.For<T>();
            criteria.SetResultTransformer(new DistinctRootEntityResultTransformer());
            return criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<T>();
        }


        protected static void AssertCollectionsEqual<T>(ICollection<T> expected, ICollection<T> actual) 
        {
            Assert.AreEqual(expected.Count, actual.Count, "collections do not contain same number of elements");

            foreach (T obj in expected)
            {
                if (Collection.SelectAll(actual, delegate(T x) { return x.Equals(obj);}).Count != 1)
                    Assert.Fail("collections do not contain the same items");

            }
        }


        protected void AssertSorted<T>(ICollection<T> parents, string sortOrder, Comparison<T> sortBy)
        {
            List<T> actual = new List<T>(parents);
            List<T> sorted = new List<T>(parents);
            sorted.Sort(sortBy);
            if (sortOrder == "Desc") sorted.Reverse();

            for (int i = 0; i < sorted.Count; i++)
                Assert.AreEqual(sorted[i], actual[i], "element not in expected position");
        }
    }

	public interface IParent
	{
		[PrimaryKey(PrimaryKeyType.Assigned)]
		Guid Id { get; set; }

		[Version(UnsavedValue = "-1")]
		int Version { get; set; }

		[Property]
		int Age { get; set; }

		[Property]
		string Name { get; set; }

		[HasMany(Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Fetch = FetchEnum.Join)]
		IList<Child> Children { get; set; }

		string ToString();
	}

	[ActiveRecord]
    public class Parent : IParent
	{
        private Guid id = Guid.NewGuid();
        private int version = -1;
        private string name;
        private int age;
        private IList<Child> children = new List<Child>();

        [PrimaryKey(PrimaryKeyType.Assigned)]
        public virtual Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        [Version(UnsavedValue = "-1")]
        public virtual int Version
        {
            get { return version; }
            set { version = value; }
        }

        [Property]
        public virtual int Age
        {
            get { return age; }
            set { age = value; }
        }

        [Property]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        [HasMany(Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Fetch = FetchEnum.Join)]
        public virtual IList<Child> Children
        {
            get { return children; }
            set { children = value; }
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("id = " + id);
            builder.AppendLine("version = " + version);
            builder.AppendLine("name = " + (name == null ? "null" : name));
            builder.AppendLine("age = " + age);
            return builder.ToString();
        }
    }

    [ActiveRecord]
    public class Child
    {
        private Guid id = Guid.NewGuid();
        private int version = -1;
        private string name = "SomeChild";
        private Parent parent;


        [BelongsTo]
        public virtual Parent Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        [PrimaryKey(PrimaryKeyType.Assigned)]
        public virtual Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        [Version(UnsavedValue = "-1")]
        public virtual int Version
        {
            get { return version; }
            set { version = value; }
        }

        [Property]
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("id = " + id);
            builder.AppendLine("version = " + version);
            builder.AppendLine("name = " + (name == null ? "null" : name));
            return builder.ToString();
        }
    }
}
