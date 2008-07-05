using System.Linq;
using BerkeleyDb;

namespace Rhino.Queues.Impl
{
	public class BerkeleyDbPhysicalStorage : IQueuePhysicalStorage
	{
		private readonly string path;

		public BerkeleyDbPhysicalStorage(string path)
		{
			this.path = path;
		}

		public void CreateInputQueue(string queueName)
		{
			using(var env = new BerkeleyDbEnvironment(path))
			using(var tx = env.BeginTransaction())
			{
				if(env.Exists("queue-list") == false)
					env.CreateTree("queue-list");
				using(var queueList = env.OpenTree("queue-list"))
				{
					queueList.Put(queueName, queueName);
					env.CreateTree(queueName + ".tree");
					env.CreateQueue(queueName + ".queue", 17 /* guid + marker */);
				}
				tx.Commit();
			}
		}

		public string[] GetQueueNames()
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			{
				if (env.Exists("queue-list") == false)
					env.CreateTree("queue-list");
				using (var queueList = env.OpenTree("queue-list"))
				{
					string[] array = (from de in queueList.Select()
					                  select de.Key.ToString()).ToArray();
					tx.Commit();
					return array;
				}
			}
		}

		public void CreateOutputQueue(string queueName)
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			{
				if (env.Exists("queue-list") == false)
					env.CreateTree("queue-list");
				using (var queueList = env.OpenTree("queue-list"))
				{
					queueList.Put(queueName, queueName);
					env.CreateTree(queueName + ".tree");
					env.CreateTree(queueName + ".batches");
					env.CreateTree(queueName + ".deadLetters");
					env.CreateQueue(queueName + ".queue", 17 /* guid + marker */);
				}
				tx.Commit();
			}
		}
	}
}