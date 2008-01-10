namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	using System.Security.AccessControl;
	using Castle.ActiveRecord;
	using Iesi.Collections.Generic;

	[ActiveRecord]
	public class MediaType
	{
		private int id;
		private string name;
		[Property]
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		private ISet<Format> formats = new HashedSet<Format>();

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[HasMany]
		public virtual ISet<Format> Formats
		{
			get { return formats; }
			set { formats = value; }
		}
	}
}