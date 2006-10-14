using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord("Users")]
	public class User : ActiveRecordBase<User>
	{
		int id;
		string name;
		string email;
		ISet<Blog> blogs = new HashedSet<Blog>();

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[HasMany]
		public ISet<Blog> Blogs
		{
			get { return blogs; }
			set { blogs = value; }
		}
	}
}