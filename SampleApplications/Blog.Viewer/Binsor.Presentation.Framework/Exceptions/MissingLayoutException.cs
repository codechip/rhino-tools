namespace Binsor.Presentation.Framework.Exceptions
{
	using System;

	[Serializable]
	public class MissingLayoutException : Exception
	{
		public MissingLayoutException(string message)
			: base(message)
		{
		}
	}
}