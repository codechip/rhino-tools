using System.IO;
using NMemcached.Commands.Modifications;

namespace NMemcached.Commands.Modifications
{
	public class IncrCommand : AbstractArithmeticOperation
	{
		protected override ulong ArithmeticOperation(ulong cachedValue)
		{
			return cachedValue + Value;
		}
	}
}