using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binsor.Presentation.Framework.Exceptions
{
	[Serializable]
	public class DuplicateLayoutException : Exception
	{
		public DuplicateLayoutException(string message) : base(message)
		{
		}
	}
}
