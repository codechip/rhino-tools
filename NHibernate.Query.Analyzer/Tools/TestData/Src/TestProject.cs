using System;
using System.Collections;

namespace TestProject
{
	/// <summary>
	/// Summary description for TestFile.
	/// </summary>
	public class TestProject
	{
		public TestProject()
		{
			
		}

		int id;
		string data;
		private IList files;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Data
		{
			get { return data; }
			set { data = value; }
		}

		public IList Files
		{
			get { return files;}
			set { files = value; }
		}
	}
}
