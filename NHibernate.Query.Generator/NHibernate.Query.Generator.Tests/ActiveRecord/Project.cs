using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord]
	public class Project : ActiveRecordBase<Project>
	{
		private int id;
		private InstalledComponnet componnet;

		public Project()
		{
		}

		public Project(InstalledComponnet componnet)
		{
			this.componnet = componnet;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo]
		public InstalledComponnet Componnet
		{
			get { return componnet; }
			set { componnet = value; }
		}
	}
}