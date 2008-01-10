namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class ResourceType
	{
		private int id;
		private string name;
		[Property]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}