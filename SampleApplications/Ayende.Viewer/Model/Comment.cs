using System;
using Castle.ActiveRecord;

namespace Model
{
	[ActiveRecord]
	public class Comment : ActiveRecordBase<Comment>
	{
		private Guid commentId;
		private Post post;
		private DateTime created;
		private DateTime modified;
		private string author;
		private string email;
		private string homepage;
		private string content;

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public Guid CommentId
		{
			get { return commentId; }
			set { commentId = value; }
		}

		[BelongsTo(NotNull = true)]
		public Post Post
		{
			get { return post; }
			set { post = value; }
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

		[Property(NotNull = true)]
		public string Author
		{
			get { return author; }
			set { author = value; }
		}

		[Property(NotNull = true)]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property(NotNull = true)]
		public string HomePage
		{
			get { return homepage; }
			set { homepage = value; }
		}

		[Property(NotNull = true, Length = 8000)]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}
	}
}