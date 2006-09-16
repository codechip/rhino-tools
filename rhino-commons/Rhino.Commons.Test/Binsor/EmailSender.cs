using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Commons.Test.Binsor
{
	public interface ISender
	{
		void Send(string msg);
	}
	
	public class EmailSender : ISender
	{
		private string host;

		public string Host
		{
			get { return host; }
			set { host = value; }
		}

		public void Send(string msg)
		{
			
		}
	}
}
