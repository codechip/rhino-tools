namespace Blog.Model
{
	using System;
	using Castle.ActiveRecord;
	using Castle.Components.Validator;

	[ActiveRecord("Subtext_Config")]
	public class Blog : ActiveRecordBase<Blog>
	{
		private int id;
		private string email;
		private string title;
		private string author;
		private string subtitle;
		private string skin;
		private string application;
		private int? timeZone;
		private bool? isActive;
		private string language;
		private int? itemCount;
		private DateTime? lastUpdated;
		private string news;
		private string secondaryCss;
		private int? postCount;
		private int? storyCount;
		private int? pingTrackCount;
		private int? commentCount;
		private bool? isAggregated;
		private int? flag;
		private string skinCssFile;
		private int blogGroup;
		private int? daysTillCommentsClose;
		private int? numberOfRecentComments;
		private int? recentCommentsLength;
		private int? categoryListPostCount;
		private string feedBurnerName;
		private string host;
		private string licenseUrl;

		[PrimaryKey("BlogId", Generator = PrimaryKeyType.Assigned)]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(NotNull = true, Length = 50)]
		[ValidateLength(0, 50), ValidateNonEmpty]
		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property(NotNull = true, Length = 100)]
		[ValidateLength(0, 100), ValidateNonEmpty]
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		[Property(NotNull = true, Length = 250)]
		[ValidateLength(0, 250), ValidateNonEmpty]
		public virtual string Subtitle
		{
			get { return subtitle; }
			set { subtitle = value; }
		}

		[Property(NotNull = true, Length = 250)]
		[ValidateLength(0, 250), ValidateNonEmpty]
		public virtual string LicenseUrl
		{
			get { return licenseUrl; }
			set { licenseUrl = value; }
		}

		[Property(NotNull = true, Length = 50)]
		[ValidateLength(0, 50), ValidateNonEmpty]
		public virtual string Skin
		{
			get { return skin; }
			set { skin = value; }
		}

		[Property(NotNull = true, Length = 50)]
		[ValidateLength(0, 50), ValidateNonEmpty]
		public virtual string Application
		{
			get { return application; }
			set { application = value; }
		}

		[Property]
		public virtual int? TimeZone
		{
			get { return timeZone; }
			set { timeZone = value; }
		}

		[Property]
		public virtual bool? IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}

		[Property(Length = 10)]
		[ValidateLength(0, 10)]
		public virtual string Language
		{
			get { return language; }
			set { language = value; }
		}

		[Property]
		public virtual int? ItemCount
		{
			get { return itemCount; }
			set { itemCount = value; }
		}

		[Property]
		public virtual DateTime? LastUpdated
		{
			get { return lastUpdated; }
			set { lastUpdated = value; }
		}

		[Property(ColumnType = "StringClob", SqlType = "NTEXT")]
		public virtual string News
		{
			get { return news; }
			set { news = value; }
		}


		[Property(ColumnType = "StringClob")]
		public virtual string SecondaryCss
		{
			get { return secondaryCss; }
			set { secondaryCss = value; }
		}

		[Property]
		public virtual int? PostCount
		{
			get { return postCount; }
			set { postCount = value; }
		}

		[Property]
		public virtual int? StoryCount
		{
			get { return storyCount; }
			set { storyCount = value; }
		}

		[Property]
		public virtual int? PingTrackCount
		{
			get { return pingTrackCount; }
			set { pingTrackCount = value; }
		}

		[Property]
		public virtual int? CommentCount
		{
			get { return commentCount; }
			set { commentCount = value; }
		}

		[Property]
		public virtual bool? IsAggregated
		{
			get { return isAggregated; }
			set { isAggregated = value; }
		}

		[Property]
		public virtual int? Flag
		{
			get { return flag; }
			set { flag = value; }
		}

		[Property(Length = 100)]
		[ValidateLength(0, 100)]
		public virtual string SkinCssFile
		{
			get { return skinCssFile; }
			set { skinCssFile = value; }
		}

		[Property(Length = 100)]
		[ValidateLength(0, 100)]
		public virtual string Host
		{
			get { return host; }
			set { host = value; }
		}

		[Property(Length = 100)]
		[ValidateLength(0, 100)]
		public virtual string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property(NotNull = true)]
		public virtual int BlogGroup
		{
			get { return blogGroup; }
			set { blogGroup = value; }
		}

		[Property]
		public virtual int? DaysTillCommentsClose
		{
			get { return daysTillCommentsClose; }
			set { daysTillCommentsClose = value; }
		}

		[Property]
		public virtual int? NumberOfRecentComments
		{
			get { return numberOfRecentComments; }
			set { numberOfRecentComments = value; }
		}

		[Property]
		public virtual int? RecentCommentsLength
		{
			get { return recentCommentsLength; }
			set { recentCommentsLength = value; }
		}

		[Property]
		public virtual int? CategoryListPostCount
		{
			get { return categoryListPostCount; }
			set { categoryListPostCount = value; }
		}

		[Property(Length = 64)]
		[ValidateLength(0, 64)]
		public virtual string FeedBurnerName
		{
			get { return feedBurnerName; }
			set { feedBurnerName = value; }
		}
	}
}