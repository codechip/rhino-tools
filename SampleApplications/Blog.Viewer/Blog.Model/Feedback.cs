namespace Blog.Model
{
	using System;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;

	[ActiveRecord("Subtext_Feedback")]
	public class Feedback : ActiveRecordBase<Feedback>
	{
		private int id;
		private string title;
		private string body;
		private Blog blog;
		private Entry entry;
		private string author;
		private bool? isBlogAuthor;
		private string email;
		private string url;
		private FeedbackType feedbackType;
		private int statusFlag;
		private bool commentAPI;
		private string referrer;
		private string ipAddress;
		private string userAgent;
		string feedbackChecksumHash;
		private DateTime? dateCreated;
		private DateTime? dateModified;

		[PrimaryKey(Generator = PrimaryKeyType.Assigned)]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(NotNull = true, Length = 256)]
		[ValidateLength(0,256), ValidateNonEmpty]
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		[Property(ColumnType = "StringClob", SqlType = "NTEXT")]
		public virtual string Body
		{
			get { return body; }
			set { body = value; }
		}

		[BelongsTo("BlogId")]
		public virtual Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		[BelongsTo("EntryId")]
		public virtual Entry Entry
		{
			get { return entry; }
			set { entry = value; }
		}
		
		[Property(NotNull = true, Length = 100)]
		[ValidateLength(0, 100), ValidateNonEmpty]
		public virtual string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property]
		public virtual bool? IsBlogAuthor
		{
			get { return isBlogAuthor; }
			set { isBlogAuthor = value; }
		}

		[Property(NotNull = true, Length = 128)]
		[ValidateLength(0, 128), ValidateNonEmpty]
		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property(NotNull = false, Length = 256)]
		[ValidateLength(0, 256), ValidateNonEmpty]
		public virtual string Url
		{
			get { return url; }
			set { url = value; }
		}

		[Property(NotNull = true)]
		public virtual FeedbackType FeedbackType
		{
			get { return feedbackType; }
			set { feedbackType = value; }
		}

		[Property(NotNull = true)]
		public virtual int StatusFlag
		{
			get { return statusFlag; }
			set { statusFlag = value; }
		}

		[Property(NotNull = true)]
		public virtual bool CommentAPI
		{
			get { return commentAPI; }
			set { commentAPI = value; }
		}

		[Property(NotNull = true, Length = 256)]
		[ValidateLength(0, 256), ValidateNonEmpty]
		public virtual string Referrer
		{
			get { return referrer; }
			set { referrer = value; }
		}

		[Property(NotNull = true, Length = 16)]
		[ValidateLength(0, 16), ValidateNonEmpty]
		public virtual string IpAddress
		{
			get { return ipAddress; }
			set { ipAddress = value; }
		}

		[Property(NotNull = true, Length = 128)]
		[ValidateLength(0, 128), ValidateNonEmpty]
		public virtual string UserAgent
		{
			get { return userAgent; }
			set { userAgent = value; }
		}

		[Property(NotNull = true, Length = 32)]
		[ValidateLength(0, 32), ValidateNonEmpty]
		public virtual string FeedbackChecksumHash
		{
			get { return feedbackChecksumHash; }
			set { feedbackChecksumHash = value; }
		}

		[Property(NotNull = true)]
		public virtual DateTime? DateCreated
		{
			get { return dateCreated; }
			set { dateCreated = value; }
		}

		[Property(NotNull = true)]
		public virtual DateTime? DateModified
		{
			get { return dateModified; }
			set { dateModified = value; }
		}
	}
}