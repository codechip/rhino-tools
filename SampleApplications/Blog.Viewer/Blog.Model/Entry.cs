namespace Blog.Model
{
	using System;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;

	[ActiveRecord("SubText_Content")]
	public class Entry : ActiveRecordBase<Entry>
	{
		private string author;
		private Blog blog;
		private DateTime dateAdded;
		private DateTime? dateSyndicated;
		private DateTime? dateUpdated;
		private string description;
		private string email;
		private string entryName;
		private int feedBackCount;
		private int id;
		private PostConfig postConfig;
		private PostType postType;
		private string text;
		private string title;

		[PrimaryKey(Generator = PrimaryKeyType.Assigned)]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(NotNull = true, Length = 255)]
		[ValidateLength(0, 255), ValidateNonEmpty]
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		[Property(NotNull = true)]
		public virtual DateTime DateAdded
		{
			get { return dateAdded; }
			set { dateAdded = value; }
		}

		[Property(NotNull = true)]
		public virtual PostType PostType
		{
			get { return postType; }
			set { postType = value; }
		}

		[Property]
		[ValidateLength(0, 50), ValidateNonEmpty]
		public virtual string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property]
		[ValidateLength(0, 50), ValidateNonEmpty]
		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

		[BelongsTo("BlogId")]
		public virtual Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		[Property(Length = 500)]
		[ValidateLength(0, 500)]
		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}

		[Property]
		public virtual DateTime? DateUpdated
		{
			get { return dateUpdated; }
			set { dateUpdated = value; }
		}

		[Property("`Text`", ColumnType = "StringClob", SqlType = "NTEXT")]
		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}

		[Property(NotNull = true)]
		public virtual int FeedBackCount
		{
			get { return feedBackCount; }
			set { feedBackCount = value; }
		}

		[Property]
		public virtual PostConfig PostConfig
		{
			get { return postConfig; }
			set { postConfig = value; }
		}

		[Property(Length = 100)]
		[ValidateLength(0, 100)]
		public virtual string EntryName
		{
			get { return entryName; }
			set { entryName = value; }
		}

		[Property]
		public virtual DateTime? DateSyndicated
		{
			get { return dateSyndicated; }
			set { dateSyndicated = value; }
		}
	}
}