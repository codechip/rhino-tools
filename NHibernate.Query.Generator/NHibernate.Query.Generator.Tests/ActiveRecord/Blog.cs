using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord("Blogs")]
	public class Blog : ActiveRecordBase<Blog>
	{
		int id;
		string name;
		User author;
		ISet<Post> posts = new HashedSet<Post>();

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name;}
			set { name = value; }
		}

		[BelongsTo]
		public User Author
		{
			get { return author;  }
			set { author = value; }
		}

		[HasMany]
		public ISet<Post> Posts
		{
			get { return posts; }
			set { posts = value; }
		}
	}
}