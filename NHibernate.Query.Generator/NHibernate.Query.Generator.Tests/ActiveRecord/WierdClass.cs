using System;
using System.Collections.Generic;
using System.Text;

using Castle.ActiveRecord;
namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord]
	public class WeirdClass : ActiveRecordBase<WeirdClass>
	{
		WeirdKey key = new WeirdKey();
		Address address = new Address();
		
		[CompositeKey]
		public WeirdKey Key
		{
			get { return key; }
			set { key = value; }
		}

		[Nested]
		public Address Address
		{
			get { return address; }
			set { address = value; }
		}
	}
	
	public class Address
	{
		string street;

		[Property]
		public string Street
		{
			get { return street; }
			set { street = value; }
		}
	}

	[Serializable]
	public class WeirdKey
	{
		string department;
		int level;

		[KeyProperty]
		public string Department
		{
			get { return department; }
			set { department = value; }
		}

		[KeyProperty]
		public int Level
		{
			get { return level; }
			set { level = value; }
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
