using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord]
	public class InstalledComponnet : ActiveRecordBase<InstalledComponnet>
	{
		private int id;
		private Componnet component;

		public InstalledComponnet(Componnet component)
		{
			this.component = component;
		}

		public InstalledComponnet()
		{

		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public Componnet Component
		{
			get { return component; }
			set { component = value; }
		}
	}
}