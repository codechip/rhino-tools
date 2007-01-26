using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord(DiscriminatorValue = "C", DiscriminatorColumn = "subclass", DiscriminatorType = "String")]
	public class Cat : ActiveRecordBase<Cat>
	{
		[Field(Column = "subclass", Update = false, Insert = false, Access = PropertyAccess.Field)]
		protected string subclass = "C";

		int id;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}

	[ActiveRecord(DiscriminatorValue = "D")]
	public class DomesticCat : Cat
	{
		public DomesticCat()
		{
			subclass = "D";
		}

		[Property]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _name;
	}
}
