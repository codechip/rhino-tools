using System.IO;
using NMemcached.Commands.Retrieval;

namespace NMemcached.Commands.Retrieval
{
	public class GetCommand : GetsCommand
	{
		protected override string ValueLineFormat
		{
			get { return "VALUE {0} {1} {2}"; }
		}
	}
}