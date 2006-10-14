using System.Collections;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using NHibernate.Cfg;
using NHibernate.Expression;
using NHibernate.Query.Generator.Tests.ActiveRecord;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
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

			source.Add(typeof (ActiveRecordBase), properties);
			ActiveRecordStarter.Initialize(source,
			                               typeof (WeirdClass),
			                               typeof (Post),
			                               typeof (Blog),
			                               typeof (User),
			                               typeof (Comment));
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
		}

		[TearDown]
		public void TestCleanup()
		{
			sessionScope.Dispose();
		}
	
		
		/*using (new SessionScope())
		{
				
			Console.WriteLine("Final!");
				
			/*
			 * Doesn't compiles! Type safety in queries.
			 * 
			 * Post.FindAll(
					Where.Post.Blog.Author == ayende && Where.Blog.Name == "Ayende"
					);
			//				ISession session = ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof(Post));
		}*/
	}
}