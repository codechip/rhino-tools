using System;

namespace TestProject
{
	using Nullables;

	/// <summary>
	/// Summary description for Employee.
	/// </summary>
	public class Employee
	{
		private int id;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string name;

		public NullableDateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		private NullableDateTime created;

		public Employee()
		{
		}

		public int Id
		{
			get { return id;}
			set { id=value;}
		}
	}
}
