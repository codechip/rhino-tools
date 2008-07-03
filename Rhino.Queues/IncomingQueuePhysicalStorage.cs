using System.IO;
using System.Linq;
using Rhino.Queues.Impl;

namespace Rhino.Queues
{
	public class IncomingQueuePhysicalStorage : IQueuePhysicalStorage
	{
		private readonly string queueDirectory;

		public IncomingQueuePhysicalStorage(string queueDirectory)
		{
			this.queueDirectory = queueDirectory;
		}

		public void CreateQueue(string queueName)
		{
			new IncomingMessageRepository(queueName, queueDirectory).CreateQueueStorage();
		}

		public string[] GetQueueNames()
		{
			return Directory.GetFiles(queueDirectory, "*.queue")
				.Select(s=>Path.GetFileNameWithoutExtension(s))
				.ToArray();
		}
	}
}