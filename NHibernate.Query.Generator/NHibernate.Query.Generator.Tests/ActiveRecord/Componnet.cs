using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord]
	public class Componnet  : ActiveRecordBase<Componnet>
	{
		private int id;
		private string version;

		public Componnet(string version)
		{
			this.version = version;
		}

		public Componnet()
		{

		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Version
		{
			get { return version; }
			set { version = value; }
		}
	}
}