using System;
using System.Collections;
using System.Collections.Generic;
using Rhino.Queues.Impl;
using Rhino.Queues.Network;
using Rhino.Queues.Storage.InMemory;

namespace Rhino.Queues.Cfg
{
	public class Configuration
	{
		private int listenersCount = 3;
		private int sendersCount = 3;
		private readonly string name;
		private readonly List<string> registeredQueues = new List<string>();
		private readonly List<string> registeredDurableQueues = new List<string>();

		private readonly IDictionary<string, string> endpointMapping =
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		public Configuration(string name)
		{
			if (name == null)
				throw new ArgumentNullException(name);
			if (name == "")
				throw new ArgumentException("name cannot be empty", "name");

			this.name = name;
		}

		public FluentEndpointMapping Map(string targetName)
		{
			return new FluentEndpointMapping(this, targetName);
		}

		public class FluentEndpointMapping
		{
			private readonly Configuration configuration;
			private readonly string name;

			public FluentEndpointMapping(Configuration configuration, string name)
			{
				this.configuration = configuration;
				this.name = name;
			}

			public Configuration To(string endPoint)
			{
				configuration.endpointMapping.Add(name, endPoint);
				return configuration;
			}
		}

		public Configuration RegisterQueue(string queue)
		{
			registeredQueues.Add(queue);
			return this;
		}

		public Configuration RegisterQueues(params string[] queues)
		{
			registeredQueues.AddRange(queues);
			return this;
		}


		public Configuration RegisterDurableQueue(string queue)
		{
			registeredDurableQueues.Add(queue);
			return this;
		}

		public Configuration RegisterDurableQueues(params string[] queues)
		{
			registeredDurableQueues.AddRange(queues);
			return this;
		}

		public Configuration ListenerThreads(int count)
		{
			listenersCount = count;
			return this;
		}

		public Configuration SenderThreads(int count)
		{
			sendersCount = count;
			return this;
		}

		public IQueueFactory BuildQueueFactory()
		{
			if (endpointMapping.ContainsKey(name) == false)
			{
				var msg =
					string.Format(
						"Cannot build configuration '{0}' before setting an endpoint mapping for '{0}' using Map('{0}').To('http://some/end/point');",
						name);
				throw new InvalidOperationException(msg);
			}

			return new QueueFactoryImpl(
				name, 
				new InMemoryMessageStorageFactory(), 
				endpointMapping, 
				registeredQueues, 
                registeredDurableQueues,
				new ListenerFactory(listenersCount), 
				new SenderFactory(sendersCount));
		}
	}


}