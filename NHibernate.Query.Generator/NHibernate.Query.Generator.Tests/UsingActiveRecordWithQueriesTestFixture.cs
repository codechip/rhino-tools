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


using System.Collections;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord.Queries;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.ActiveRecord;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using MbUnit.Framework;
using Query;

namespace NHibernate.Query.Generator.Tests
{
	using System;
	using System.IO;

	/// <summary>
    /// Here are the tests that shows usage of NQG generated code.
    /// We are using Active Record because it is so much fun to do so.
    /// </summary>
    [TestFixture]
    public class UsingActiveRecordWithQueriesTestFixture
    {
        private SessionScope sessionScope;

        [Test]
		[Ignore("reverted the patch that supported it")]
		public void QueryWithJoinOverSamePropertyName()
        {
            Project[] projects = Project.FindAll(Where.Project.Componnet.Component.Version == "v1.0");
            Assert.AreEqual(1, projects.Length);
            Assert.AreEqual("v1.0", projects[0].Componnet.Component.Version);
        }

        [Test]
        public void CanProjectProperties()
        {
            ProjectionQuery<User> query = new ProjectionQuery<User>(
                Where.User.Name == "Ayende",
                ProjectBy.User.Name && ProjectBy.User.Email);
            object[] execute = query.Execute()[0];
            Assert.AreEqual("Ayende", execute[0]);
            Assert.AreEqual("Ayende at ayende dot com", execute[1]);
        }

        [Test]
        public void CanProjectStronglyTyped()
        {
            ProjectionQuery<User, UserDetails> query = new ProjectionQuery<User, UserDetails>(
                Where.User.Name == "Ayende",
                ProjectBy.User.Name && ProjectBy.User.Email);
            UserDetails execute = query.Execute()[0];
            Assert.AreEqual("Ayende", execute.Name);
            Assert.AreEqual("Ayende at ayende dot com", execute.Email);

        }

        class UserDetails
        {
            public string Name;
            public string Email;

            public UserDetails(string name, string email)
            {
                Name = name;
                Email = email;
            }
        }
        [Test]
        public void CanExtendWhere()
        {
            User findOne = User.FindOne(Where.User.IsInGroup("Administrators"));
            Assert.IsNotNull(findOne);

            findOne = User.FindOne(Where.User.IsInGroup("Users"));
            Assert.IsNull(findOne);
        }

    	[Test]
    	public void ShouldNotDuplicateEntityCheck()
    	{
    		TextWriter old = Console.Out;
    		StringWriter sw = new StringWriter();
			Console.SetOut(sw);
    		Blog first = Blog.FindFirst();
    		Post.FindAll(Where.Post.Blog == first);

    		string log = sw.GetStringBuilder().ToString();

			Assert.IsFalse(log.Contains("Blog = @p1"), "Should only have single parameters");

			Console.SetOut(old);
    	}

    	[Test]
        public void CanQueryOverCollectionsExistance()
        {
            User one = User.FindOne(Where.User.Blogs.Exists(
                                        Where.Blog.Name == "Ayende @ Blog"
                                        ));
            Assert.IsNotNull(one);
        }

        [Test]
        public void CanQueryOverCollections()
        {
            /* Equals to: 
             * DetachedCriteria f = DetachedCriteria.For(typeof(User))
                  .CreateCriteria("Blogs", JoinType.LeftOuterJoin)
                      .Add(Expression.Expression.Eq("Name", "Ayende @ Blog"));*/
            User one = User.FindOne(Where.User.Blogs
                                        .With(JoinType.LeftOuterJoin)
                                            .Name == "Ayende @ Blog");

            Assert.IsNotNull(one);
        }

        [Test]
        public void CanQueryOverCollections_Many_To_Many()
        {
             User one = User.FindOne(Where.User.Name == "Ayende" &&
                    Where.User.Roles.With().Name == "Administrator");
            Assert.IsNotNull(one);
        }

        [Test]
        public void CanUseOrderringOnCompositeProperties()
        {
            WeirdClass weird = new WeirdClass();
            weird.Key.Department = "Foo";
            weird.Key.Level = 3;
            weird.Address.Street = "NHibernate == Fun";
            weird.Create();

            WeirdClass weird2 = new WeirdClass();
            weird2.Key.Department = "XYZ";
            weird2.Key.Level = 5;
            weird2.Address.Street = "Active Record == Easy Fun";
            weird2.Create();

            WeirdClass[] findAll = WeirdClass.FindAll(OrderBy.WeirdClass.Address.Street);
            Assert.AreEqual(weird2.Address.Street, findAll[0].Address.Street);
            Assert.AreEqual(weird.Address.Street, findAll[1].Address.Street);

            findAll = WeirdClass.FindAll(OrderBy.WeirdClass.Address.Street.Desc);
            Assert.AreEqual(weird.Address.Street, findAll[0].Address.Street);
            Assert.AreEqual(weird2.Address.Street, findAll[1].Address.Street);
        }

        [Test]
        public void CanQueryByCompositeIdNestedType()
        {
            WeirdClass weird = new WeirdClass();
            weird.Key.Department = "Foo";
            weird.Key.Level = 3;

            weird.Create();

            WeirdClass item = WeirdClass.FindOne(Where.WeirdClass.Key.Department == "Foo");

            Assert.IsNotNull(item);
        }

        [Test]
        public void CanUseOrderringOnComponentProperties()
        {
            WeirdClass weird = new WeirdClass();
            weird.Key.Department = "Foo";
            weird.Key.Level = 3;

            weird.Create();

            WeirdClass weird2 = new WeirdClass();
            weird2.Key.Department = "Bar";
            weird2.Key.Level = 5;

            weird2.Create();


            WeirdClass[] weirds = WeirdClass.FindAll(OrderBy.WeirdClass.Key.Department); 
			//WeirdClass[] weirds = WeirdClass.FindAll(new OrderByClause("this.Key.Department")); 
			Assert.AreEqual("Bar", weirds[0].Key.Department);
            Assert.AreEqual("Foo", weirds[1].Key.Department);
        }

		[Test]
		public void CanUseOrderringOnComponentPropertiesByDetachedCriteria()
		{
			WeirdPropertyClass property =  new WeirdPropertyClass();
			property.IntegerProperty = 10;
			property.StringProperty = "cdef";
			property.Create();

			OtherWeirdClass weird = new OtherWeirdClass();
			weird.Key.Department = "Foo";
			weird.Key.Level = 3;
			weird.Address.Street = "Active Record == Easy Fun";
			weird.Property = property;

			weird.Create();

			WeirdPropertyClass property2 = new WeirdPropertyClass();
			property2.IntegerProperty = 5;
			property2.StringProperty = "abcd";
			property2.Create();

			OtherWeirdClass weird2 = new OtherWeirdClass();
			weird2.Key.Department = "Bar";
			weird2.Key.Level = 5;
			weird2.Address.Street = "Active Record == Easy Fun";
			weird2.Property = property2;

			weird2.Create();

			// normally we'do it this way:
			//WeirdClass[] weirds = WeirdClass.FindAll(OrderBy.WeirdClass.Key.Department);

			// but if we want to dynamically build our queries:
			QueryBuilder<OtherWeirdClass> qb = new QueryBuilder<OtherWeirdClass>();
			// create a query 
			qb &= (Where.OtherWeirdClass.Address.Street.Eq("Active Record == Easy Fun"));

			// need the session
			ISessionFactory sf = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(OtherWeirdClass));
			ISession currentSession = SessionScope.Current.GetSession(sf);

			// set the property to order by
			List<OrderByClause> orders = new List<OrderByClause>();
			orders.Add(new OrderByClause("StringProperty","this.Property").Asc);

			DetachedCriteria detachedCriteria = qb.ToDetachedCriteria("",orders.ToArray());

			ICriteria critMain = detachedCriteria.GetExecutableCriteria(currentSession);


			ICollection<OtherWeirdClass> list = critMain.List<OtherWeirdClass>();
			OtherWeirdClass[] weirds = new OtherWeirdClass[2];
			list.CopyTo(weirds,0);
			Assert.AreEqual("Bar", weirds[0].Key.Department);
			Assert.AreEqual("Foo", weirds[1].Key.Department);
		}

		[Test]
		public void CanUseOrderringOnOSimplePropertiesByDetachedCriteria()
		{
			User user = new User();
			user.Name = "zzz";
			user.Email = "Ayende at ayende dot com";
			user.Save();

			QueryBuilder<User> qb = new QueryBuilder<User>();
			// create a query 
			qb &= (Where.User.Email.Eq("Ayende at ayende dot com"));

			// need the session
			ISessionFactory sf = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(OtherWeirdClass));
			ISession currentSession = SessionScope.Current.GetSession(sf);

			// set the property to order by
			List<OrderByClause> orders = new List<OrderByClause>();
			orders.Add(new OrderByClause("Name", "this").Desc);

			DetachedCriteria detachedCriteria = qb.ToDetachedCriteria("", orders.ToArray());

			ICriteria critMain = detachedCriteria.GetExecutableCriteria(currentSession);

			ICollection<User> list = critMain.List<User>();
			User[] users = new User[2];
			list.CopyTo(users, 0);
			Assert.AreEqual(2, users.Length);
			Assert.AreEqual("zzz", users[0].Name);
			Assert.AreEqual("Ayende", users[1].Name);
		}


        [Test]
        public void CanUseOrderringOnSimpleProperties()
        {
            User user = new User();
            user.Name = "zzz";
            user.Email = "f@f.com";
            user.Save();

            User[] users = User.FindAll(OrderBy.User.Name.Desc);
            Assert.AreEqual(2, users.Length);
            Assert.AreEqual("zzz", users[0].Name);
            Assert.AreEqual("Ayende", users[1].Name);

            users = User.FindAll(OrderBy.User.Name); //desfault ascending
            Assert.AreEqual(2, users.Length);
            Assert.AreEqual("Ayende", users[0].Name);
            Assert.AreEqual("zzz", users[1].Name);
        }

        [Test]
        public void HaveObjectOrientedQueryingSyntax()
        {
            Post post = Post.FindOne();
            Comment[] commentFromDb =
                Comment.FindAll(Where.Comment.Content == "Active Record Rocks!" || Where.Comment.Content.Like("NHibernate", MatchMode.Anywhere)
                                && Where.Comment.Post == post);
            Assert.AreEqual(1, commentFromDb.Length);
        }

        [Test]
        public void CanQueryNormallyForNullAssoications()
        {
            Post findOne = Post.FindOne(Where.Post.Blog != null);
            Assert.IsNotNull(findOne);

            findOne = Post.FindOne(Where.Post.Blog == null);
            Assert.IsNull(findOne);
        }

        [Test]
        public void CanQueryWithAutoJoins()
        {
            User ayende = User.FindOne(Where.User.Name == "Ayende");

            Post[] findAll =
                Post.FindAll(Where.Post.Blog.Author == ayende && Where.Post.Blog.Name == "Ayende @ Blog" &&
                             Where.Post.Title == "I love Active Record");

            Assert.AreEqual(1, findAll.Length);
        }


        [Test]
        public void CanQueryUsingAllInOverloads()
        {
            Post post = Post.FindOne(Where.Post.Title == "I love Active Record");

            Comment[] c = Comment.FindAll(Where.Comment.Post.In(post));
            Assert.AreEqual(1, c.Length);

            c = Comment.FindAll(Where.Comment.Post.In(post));
            Assert.AreEqual(1, c.Length);

            List<Post> list = new List<Post>();
            list.Add(post);
            c = Comment.FindAll(Where.Comment.Post.In((ICollection<Post>)list));
            Assert.AreEqual(1, c.Length);
        }

        [Test]
        public void CanQueryUsingIsNullOrIsNotNull()
        {
            Comment[] c = Comment.FindAll(Where.Comment.Post.IsNull);
            Assert.AreEqual(0, c.Length);

            c = Comment.FindAll(Where.Comment.Post.IsNotNull);
            Assert.AreEqual(1, c.Length);

        }

        [Test]
        public void CanQueryOnSubclasses()
        {
            Cat[] cats = Cat.FindAll();
            Assert.AreEqual(2, cats.Length);

            cats = DomesticCat.FindAll(Where.DomesticCat);
            Assert.AreEqual(1, cats.Length);

            Assert.IsTrue(cats[0] is DomesticCat);
        }

        public class LazyInPlaceConfigurationSource : InPlaceConfigurationSource
        {
            public LazyInPlaceConfigurationSource()
            {
                SetIsLazyByDefault(true);
            }
        }

        [TestFixtureSetUp]
        public void OneTimeSetup()
        {
            InPlaceConfigurationSource source = new LazyInPlaceConfigurationSource();

            Hashtable properties = new Hashtable();

            properties.Add("show_sql", "true");

            properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
            properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
            properties.Add("connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            properties.Add("connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
            properties.Add("connection.release_mode", "on_close");

            source.Add(typeof(ActiveRecordBase), properties);
            ActiveRecordStarter.Initialize(source,
                                           typeof(WeirdClass),
										   typeof(WeirdPropertyClass),
										   typeof(OtherWeirdClass),
										   typeof(Post),
                                           typeof(Blog),
                                           typeof(User),
                                           typeof(Role),
                                           typeof(Comment),
                                           typeof(Cat),
                                           typeof(DomesticCat),
                                           typeof(Project),
                                           typeof(InstalledComponnet),
                                           typeof(Componnet));
        }

        [SetUp]
        public void TestInitialize()
        {
            sessionScope = new SessionScope();
            ISessionFactoryHolder sessionFactoryHolder = ActiveRecordMediator.GetSessionFactoryHolder();
            Configuration configuration = sessionFactoryHolder.GetConfiguration(typeof(ActiveRecordBase));
            ISession session = sessionFactoryHolder.CreateSession(typeof(ActiveRecordBase));
            SchemaExport export = new SchemaExport(configuration);
            export.Execute(false, true, false, false, session.Connection, null);

            User ayende = new User();
            ayende.Name = "Ayende";
            ayende.Email = "Ayende at ayende dot com";
            ayende.Save();

            Role role = new Role();
            role.Name = "Administrator";
            role.Users.Add(ayende);
            role.Save();

            Blog blog = new Blog();
            blog.Name = "Ayende @ Blog";
            blog.Author = ayende;
            blog.Save();

            Post post = new Post();
            post.Title = "I love Active Record";
            post.Contnet = "Indeed I do!";
            post.Blog = blog;
            post.Save();

            Comment comment = new Comment();
            comment.Author = "Stranger";
            comment.Content = "Active Record Rocks!";
            comment.Post = post;
            comment.Save();

            Cat cat = new Cat();
            cat.Save();

            DomesticCat domesticCat = new DomesticCat();
            domesticCat.Name = "Domestic Cat";
            domesticCat.Save();

            Componnet componnet = new Componnet("v1.0");
            componnet.Save();
            InstalledComponnet ic = new InstalledComponnet(componnet);
            ic.Save();

            new Project(ic).Save();
        }

        [Test]
        public void CanUseJoinedBaseClass()
        {
            PropertyQueryBuilder<MesajIst> email = Where.MesajIst.Email;
        }

        [TearDown]
        public void TestCleanup()
        {
            sessionScope.Dispose();
        }
    }
}
