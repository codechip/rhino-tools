using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace Model
{
	[ActiveRecord]
	public class Post : ActiveRecordBase<Post>
	{
		private string content;
		private DateTime created, modified;
		private Guid postId;
		private string title;
		private ISet<Category> categories = new HashedSet<Category>();

		[Property(NotNull = true, ColumnType = "StringClob", SqlType = "NTEXT")]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		[Property(NotNull = true)]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		[Property(NotNull = true)]
		public DateTime Modified
		{
			get { return modified; }
			set { modified = value; }
		}

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public Guid PostId
		{
			get { return postId; }
			set { postId = value; }
		}

		[Property(Length = 512, NotNull = true)]
		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		[HasAndBelongsToMany(Table="CategoriesPosts", ColumnKey = "PostId", ColumnRef = "CategoryId", Lazy = true)]
		public ISet<Category> Categories
		{
			get { return categories; }
			set { categories = value; }
		}

		public void AddToCategory(Category category)
		{
			category.Posts.Add(this);
			Categories.Add(category);
		}
	}
}
