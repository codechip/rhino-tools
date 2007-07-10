using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using NHibernate;
using NHibernate.Expression;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTestsBase : NHibernateInMemoryTestFixtureBase {
        protected IList<Parent> parentsInDb;


        [TestFixtureSetUp]
        public void OneTimeTestInitialize()
        {
            sessionFactory = null;

            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Repository\Windsor.config"));
            OneTimeInitalize(path, typeof(Parent).Assembly);
        }


        protected void SaveInCurrentSession(IList<Parent> toSave)
        {
            foreach (Parent parent in toSave)
                UnitOfWork.CurrentSession.Save(parent);
        }


        protected static void FlushAndClearCurrentSession() {
            UnitOfWork.Current.Flush();
            UnitOfWork.CurrentSession.Clear();
        }


        public static Parent CreateExampleParentObject(string parentName, int age) 
        {
            Parent parent = new Parent();
            parent.Name = parentName;
            parent.Age = age;

            Child child1 = new Child();
            child1.Parent = parent;
            parent.Children.Add(child1);

            Child child2 = new Child();
            child2.Parent = parent;
            parent.Children.Add(child2);
            return parent;
        }


        protected static IList<T> LoadAll<T>()
        {
            DetachedCriteria criteria = DetachedCriteria.For<T>();
            criteria.SetResultTransformer(CriteriaUtil.DistinctRootEntity);
            return criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<T>();
        }
    }
}