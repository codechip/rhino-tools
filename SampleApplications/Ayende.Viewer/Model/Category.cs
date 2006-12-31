using System;
using Castle.ActiveRecord;
using Iesi.Collections.Generic;

namespace Model
{
	[ActiveRecord]
	public class Category : ActiveRecordBase<Category>
	{
		private Guid categoryId;
		private string name;
		private ISet<Post> posts = new HashedSet<Post>();

		public Category()
		{

		}

		public Category(string name)
		{
			this.name = name;
		}

		[PrimaryKey(PrimaryKeyType.Guid)]
		public Guid CategoryId
		{
			get { return categoryId; }
			set { categoryId = value; }
		}

		[Property(Length = 512, NotNull = true)]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[HasAndBelongsToMany(Table = "CategoriesPosts", Inverse = true, Lazy = true,
			ColumnKey = "CategoryId", ColumnRef = "PostId")]
		public ISet<Post> Posts
		{
			get { return posts; }
			set { posts = value; }
		}
	}
}