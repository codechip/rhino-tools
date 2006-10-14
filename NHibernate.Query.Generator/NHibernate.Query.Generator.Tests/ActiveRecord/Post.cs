using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord("Posts")]
	public class Post : ActiveRecordBase<Post>
	{
		int id;
		string title;
		string contnet;
		Blog blog;
		ISet<Comment> comments = new HashedSet<Comment>();

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[Property]
		public string Contnet
		{
			get { return contnet; }
			set { contnet = value; }
		}

		[BelongsTo]
		public Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		[HasMany]
		public ISet<Comment> Comments
		{
			get { return comments; }
			set { comments = value; }
		}
	}
}