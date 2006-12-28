using Castle.ActiveRecord;

namespace Exesto.Model
{
	[ActiveRecord]
	public class Question
	{
		private int id;
		private string title;
		private string answer;
		private Subject subject;
		private string content;
		

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public virtual string Content
		{
			get { return content; }
			set { content = value; }
		}

		[Property]
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}


		[Property]
		public virtual string Answer
		{
			get { return answer; }
			set { answer = value; }
		}


		[BelongsTo]
		public virtual Subject Subject
		{
			get { return subject; }
			set { subject = value; }
		}

	}
}