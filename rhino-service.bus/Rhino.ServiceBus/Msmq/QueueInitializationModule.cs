using System;
using System.Messaging;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;

namespace Rhino.ServiceBus.Msmq
{
	public class QueueInitializationModule : IMessageModule
	{
		private readonly IQueueStrategy queueStrategy;

		public QueueInitializationModule(IQueueStrategy queueStrategy)
		{
			this.queueStrategy = queueStrategy;
		}

		public void Init(ITransport transport)
		{
			try
			{
				queueStrategy.InitializeQueue(transport.Endpoint);
			}
			catch (Exception e)
			{
				throw new TransportException(
					"Could not open queue: " + transport.Endpoint + Environment.NewLine +
					"Queue path: " + MsmqUtil.GetQueuePath(transport.Endpoint), e);
			}
		}

		public void Stop(ITransport transport)
		{
			//do nothing
		}
	}
}