using System;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Rhino.Commons.Tasks
{
	public class Concat : Task
	{
		private ITaskItem[] items;
		private string output;
		private string seperator = " ";

		[Required]
		public ITaskItem[] Items
		{
			get { return items; }
			set { items = value; }
		}

		[Output]
		public string Output
		{
			get { return output; }
			set { output = value; }
		}

		public string Seperator
		{
			get { return seperator; }
			set { seperator = value; }
		}

		public override bool Execute()
		{
			StringBuilder sb = new StringBuilder();
			foreach (ITaskItem taskItem in items)
			{
				sb.Append(taskItem).Append(seperator);
			}
			output = sb.ToString();
			return true;
		}
	}
}
