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
using NHibernate.Expression;
using Rhino.Commons.ForTesting;

namespace Rhino.Commons.Test.Repository
{
    public class RepositoryTestsBase : TestFixtureBase
    {
        protected IList<Parent> parentsInDb;


        protected void SaveInCurrentSession(IList<Parent> toSave)
        {
            foreach (Parent parent in toSave)
                UnitOfWork.CurrentSession.Save(parent);
        }


        protected static void FlushAndClearCurrentSession()
        {
            UnitOfWork.Current.Flush();
            UnitOfWork.CurrentSession.Clear();
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


        protected static IList<T> LoadAll<T>()
        {
            DetachedCriteria criteria = DetachedCriteria.For<T>();
            criteria.SetResultTransformer(CriteriaUtil.DistinctRootEntity);
            return criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<T>();
        }
    }

    [ActiveRecord]
    public class Parent
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
    }
}