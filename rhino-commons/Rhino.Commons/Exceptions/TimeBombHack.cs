using System;

namespace Rhino.Commons.Exceptions
{
	public static class TimeBombHack
	{
		public static void Until(DateTime date, string message)	
		{
			if (DateTime.Today > date)
				throw new HackExpiredException(string.Format("The hack ({0}) expired on ({1}). You really should fix it already", message, date));
		}
	}
}
