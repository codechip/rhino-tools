using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord("Comments")]
	public class Comment : ActiveRecordBase<Comment>
	{
		int id;
		string author;
		string content;
		Post post;
		
		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		[BelongsTo]
		public Post Post
		{
			get { return post; }
			set { post = value; }
		}
	}
}