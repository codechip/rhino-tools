namespace MyBlog
{
	public class Tag
	{
		private int id;
		private object entity;
		private string name;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual object Entity
		{
			get { return entity; }
			set { entity = value; }
		}


		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}