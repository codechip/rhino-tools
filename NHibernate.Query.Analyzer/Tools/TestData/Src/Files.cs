using System;

namespace TestProject
{
	/// <summary>
	/// Summary description for Files.
	/// </summary>
	public class Files
	{
		string filename;
		int id;

		public TestProject Test
		{
			get { return test; }
			set { test = value; }
		}

		TestProject test;

		public string Filename
		{
			get { return filename; }
			set { filename = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}


		public Files()
		{
		}
	}
}
