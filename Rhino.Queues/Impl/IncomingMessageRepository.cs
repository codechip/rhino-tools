using System;
using BerkeleyDb;

namespace Rhino.Queues.Impl
{
	public class IncomingMessageRepository : IIncomingMessageRepository
	{
		private readonly string name;
		private readonly string path;

		public string Name
		{
			get { return name; }
		}

		public IncomingMessageRepository(string name, string path)
		{
			this.name = name;
			this.path = path;
		}

		public void Save(params QueueMessage[] msgs)
		{
			using(var env = new	BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				foreach (var msg in msgs)
				{
					queue.AppendAssociation(tree, msg);
				}
				tx.Commit();
			}
		}

		public void PurgeAllMessages()
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				tree.Truncate();
				queue.Truncate();
				tx.Commit();
			}
		}

		public QueueMessage GetEarliestMessage()
		{
			using (var env = new BerkeleyDbEnvironment(path))
			using (var tx = env.BeginTransaction())
			using (var tree = env.OpenTree(name + ".tree"))
			using (var queue = env.OpenQueue(name + ".queue"))
			{
				var  val = queue.Consume();
				if( val==null)
					return null;
				var id = (Guid) val;
				var msg = tree.Get(id);
				tree.Delete(id);
				tx.Commit();
				return (QueueMessage)msg;
			}
		}
		
	}
}