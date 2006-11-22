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
		int? size;

		public int? Size
		{
			get { return size; }
			set { size = value; }
		}

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
