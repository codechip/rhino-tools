namespace Rhino.Commons.Exceptions
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	/// <summary>
	/// Raise exception if guard detected problem in the reports.
	/// </summary>
	[Serializable]
	public class GuardSpotException: Exception
	{
		private IList<string> errors = new List<string>();

		internal void RecordMessage(string message)
		{
			errors.Add(message);
		}

		/// <summary>
		/// Detail error message
		/// </summary>
		/// <returns></returns>
		public IList<string> GetErrorSummary()
		{
			return errors;
		}
	}
}
