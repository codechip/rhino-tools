namespace Rhino.Commons
{
	using System;
	using Exceptions;

	/// <summary>
	/// Helper class for guard statements, which allow prettier
	/// code for guard clauses
	/// </summary>
	public class Guard
	{
		GuardSpotException exception = new GuardSpotException();
		/// <summary>
		/// Will throw a <see cref="InvalidOperationException"/> if the assertion
		/// is true, with the specificied message.
		/// </summary>
		/// <param name="assertion">if set to <c>true</c> [assertion].</param>
		/// <param name="message">The message.</param>
		/// <example>
		/// Sample usage:
		/// <code>
		/// Guard.Against(string.IsNullOrEmpty(name), "Name must have a value");
		/// </code>
		/// </example>
		public static void Against(bool assertion, string message)
		{
			if (assertion == false)
				return;
			throw new InvalidOperationException(message);
		}

		/// <summary>
		/// Will throw exception of type <typeparamref name="TException"/>
		/// with the specified message if the assertion is true
		/// </summary>
		/// <typeparam name="TException"></typeparam>
		/// <param name="assertion">if set to <c>true</c> [assertion].</param>
		/// <param name="message">The message.</param>
		/// <example>
		/// Sample usage:
		/// <code>
		/// <![CDATA[
		/// Guard.Against<ArgumentException>(string.IsNullOrEmpty(name), "Name must have a value");
		/// ]]>
		/// </code>
		/// </example>
		public static void Against<TException>(bool assertion, string message) where TException : Exception
		{
			if (assertion == false)
				return;
			throw (TException)Activator.CreateInstance(typeof(TException), message);
		}

		/// <summary>
		/// Keep checking as assertion go by
		/// </summary>
		/// <param name="assertion">if set to <c>true</c> log message.</param>
		/// <param name="message">friendly message if [assertion] is <c>true</c>.</param>
		public Guard Check(bool assertion, string message)
		{
			if (assertion) exception.RecordMessage(message);
			return this;
		}

		/// <summary>
		/// Ask if there is any problem detected, expected to throw <see cref="GuardSpotException"/> if problems detected. 
		/// </summary>
		public void Report()
		{
			if (exception.GetErrorSummary().Count > 0) throw exception;
		}
	}
}
