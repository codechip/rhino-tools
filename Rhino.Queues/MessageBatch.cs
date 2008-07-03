using System;

namespace Rhino.Queues
{
	public class MessageBatch
	{
		public bool IsEmpty
		{
			get
			{
				return DestinationBatches.Length == 0;
			}
		}
		
		public Guid Id { get; set; }

		public SingleDestinationMessageBatch[] DestinationBatches { get; set; }

		public MessageBatch()
		{
			DestinationBatches = new SingleDestinationMessageBatch[0];
			Id = Guid.NewGuid();
		}
	}
}