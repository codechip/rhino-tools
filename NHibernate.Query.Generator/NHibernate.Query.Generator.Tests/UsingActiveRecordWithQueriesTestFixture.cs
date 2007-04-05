using System;
using System.Collections;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using Castle.ActiveRecord.Queries;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.ActiveRecord;
using NHibernate.Tool.hbm2ddl;
using MbUnit.Framework;
using Query;

namespace NHibernate.Query.Generator.Tests
{
	/// <summary>
	/// Here are the tests that shows usage of NQG generated code.
	/// We are using Active Record because it is so much fun to do so.
	/// </summary>
	[TestFixture]
	public class UsingActiveRecordWithQueriesTestFixture
	{
		private SessionScope sessionScope;

		[Test]
		public void QueryWithJoinOverSamePropertyName()
		{
			Project[] projects = Project.FindAll(Where.Project.Componnet.Component.Version == "v1.0");
			Assert.AreEqual(1, projects.Length );
			Assert.AreEqual("v1.0", projects[0].Componnet.Component.Version );
		}

		[Test]
		public void CanProjectProperties()
		{
			ProjectionQuery<User> query = new ProjectionQuery<User>(
				Where.User.Name == "Ayende",
				ProjectBy.User.Name && ProjectBy.User.Email);
			object[] execute = query.Execute()[0];
			Assert.AreEqual("Ayende",execute[0] );
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
			Assert.AreEqual("Bar", weirds[0].Key.Department);
			Assert.AreEqual("Foo", weirds[1].Key.Department);
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

		[Test]
		public void CanProjectPropertiesFromAssociation()
		{
			ProjectionQuery<Blog> query = new ProjectionQuery<Blog>(
				For.Blog.ProjectBy(ProjectBy.Blog.Name && ProjectBy.Blog.Author.Name),
				ProjectBy.Blog.Name && ProjectBy.Blog.Author.Name);
			object[] execute = query.Execute()[0];
			Assert.AreEqual("Ayende @ Blog", execute[0]);
			Assert.AreEqual("Ayende", execute[1]);
		}

		[Test]
		public void CanAutoGroupProjections()
		{
			ProjectionQuery<Payment> query = new ProjectionQuery<Payment>(
				For.Payment.ProjectBy(ProjectBy.Payment.Amount.Max() && ProjectBy.Payment.PayDate),
				OrderBy.Payment.PayDate.Desc,
				ProjectBy.Payment.Amount.Max() && ProjectBy.Payment.PayDate);
			IList<object[]> execute = query.Execute();
			Assert.AreEqual(2, execute.Count);
			Assert.AreEqual(100, execute[0][0]);
			Assert.AreEqual(DateTime.Today, execute[0][1]);
			Assert.AreEqual(200, execute[1][0]);
			Assert.AreEqual(DateTime.Today.AddDays(-7), execute[1][1]);
		}


		[Test]
		public void CanProjectDistinct()
		{
			ProjectionQuery<Payment> query = new ProjectionQuery<Payment>(
				For.Payment.ProjectBy(ProjectBy.Payment.Distinct() && ProjectBy.Payment.PayDate),
				OrderBy.Payment.PayDate.Desc,
				ProjectBy.Payment.Distinct() && ProjectBy.Payment.PayDate);
			IList<object[]> execute = query.Execute();
			Assert.AreEqual(2, execute.Count);
			Assert.AreEqual(DateTime.Today, execute[0][1]);
			Assert.AreEqual(DateTime.Today.AddDays(-7), execute[1][1]);
		}

		[TestFixtureSetUp]
		public void OneTimeSetup()
		{
			InPlaceConfigurationSource source = new InPlaceConfigurationSource();

			Hashtable properties = new Hashtable();

			properties.Add("hibernate.show_sql", "true");

			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SQLite20Driver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "Data Source=:memory:;Version=3;New=True;");
			properties.Add("hibernate.connection.release_mode", "on_close");

			source.Add(typeof (ActiveRecordBase), properties);
			ActiveRecordStarter.Initialize(source,
			                               typeof (WeirdClass),
			                               typeof (Post),
			                               typeof (Blog),
			                               typeof (User),
			                               typeof (Comment),
										   typeof (Cat),
										   typeof (DomesticCat),
										   typeof(Project),
										   typeof(InstalledComponnet),
										   typeof(Componnet),
											 typeof(Payment));
		}

		[SetUp]
		public void TestInitialize()
		{
			sessionScope = new SessionScope();
			ISessionFactoryHolder sessionFactoryHolder = ActiveRecordMediator.GetSessionFactoryHolder();
			Configuration configuration = sessionFactoryHolder.GetConfiguration(typeof (ActiveRecordBase));
			ISession session = sessionFactoryHolder.CreateSession(typeof (ActiveRecordBase));
			SchemaExport export = new SchemaExport(configuration);
			export.Execute(false, true, false, false, session.Connection, null);

			User ayende = new User();
			ayende.Name = "Ayende";
			ayende.Email = "Ayende at ayende dot com";
			ayende.Save();

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

			Payment payment = new Payment();
			payment.Amount = 100;
			payment.PayDate = DateTime.Today;
			payment.Save();

			Payment payment1 = new Payment();
			payment1.Amount = 100;
			payment1.PayDate = DateTime.Today;
			payment1.Save();

			Payment payment2 = new Payment();
			payment2.Amount = 200;
			payment2.PayDate = DateTime.Today.AddDays(-7);
			payment2.Save();

			Payment payment3 = new Payment();
			payment3.Amount = 200;
			payment3.PayDate = DateTime.Today.AddDays(-7);
			payment3.Save();
		}

		[TearDown]
		public void TestCleanup()
		{
			sessionScope.Dispose();
		}
	}
}
