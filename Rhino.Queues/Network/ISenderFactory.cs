using System;
using Rhino.Queues.Storage;

namespace Rhino.Queues.Network
{
	public interface ISenderFactory : IDisposable
	{
		ISender Create(IMessageStorage storage);
	}
}