using System;

namespace Rhino.Queues.Network
{
	public interface IListener : IDisposable
	{
		void Start();
	}
}