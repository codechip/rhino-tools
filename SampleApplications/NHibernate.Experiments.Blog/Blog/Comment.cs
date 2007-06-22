namespace MyBlog
{
	public class Comment
	{
		private int id;
		private Post post;
		private string name;
		private string email;
		private string homePage;
		private int ip;
		private string text;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual Post Post
		{
			get { return post; }
			set { post = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual string Email
		{
			get { return email; }
			set { email = value; }
		}

		public virtual string HomePage
		{
			get { return homePage; }
			set { homePage = value; }
		}

		public virtual int Ip
		{
			get { return ip; }
			set { ip = value; }
		}

		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}
	}
}