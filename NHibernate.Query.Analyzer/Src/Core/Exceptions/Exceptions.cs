using System;
using System.Runtime.Serialization;

namespace Ayende.NHibernateQueryAnalyzer.Exceptions
{
	

	/// <summary>
	/// UnknownFileTypeException.
	/// </summary>
	public class UnknownFileTypeException : FileLoadingException
	{
		public UnknownFileTypeException() : base()
		{
		}

		public UnknownFileTypeException(string message) : base(message)
		{
		}

		public UnknownFileTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public UnknownFileTypeException(string message, Exception inner) : base(message, inner)
		{
		}

	}

	/// <summary>
	/// ParametersNotAllowedException.
	/// </summary>
	public class FileLoadingException : ApplicationException
	{
		public FileLoadingException() : base()
		{
		}

		public FileLoadingException(string message) : base(message)
		{
		}

		public FileLoadingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public FileLoadingException(string message, Exception inner) : base(message, inner)
		{
		}

	}
}