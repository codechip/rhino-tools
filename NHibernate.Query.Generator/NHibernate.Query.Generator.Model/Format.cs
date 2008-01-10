namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	using System.Security.AccessControl;
	using Castle.ActiveRecord;
	using Iesi.Collections.Generic;

	[ActiveRecord]
	public class Format
	{
		private int id;
		private MediaType mediaType;
		private ISet<ResourceType> resourceTypes = new HashedSet<ResourceType>();
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

		[BelongsTo]
		public virtual MediaType MediaType
		{
			get { return mediaType; }
			set { mediaType = value; }
		}

		[HasAndBelongsToMany(
			Table = "FormatsToResourceType",
			ColumnKey = "FormatId",
			ColumnRef = "ResourceTypeId"
			)]
		public virtual ISet<ResourceType> ResourceTypes
		{
			get { return resourceTypes; }
			set { resourceTypes = value; }
		}
	}
}