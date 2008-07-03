using System;
using MbUnit.Framework;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class QueueMessageTests
	{
		private QueueMessage msg;

		[SetUp]
		public void Setup()
		{
			msg = new QueueMessage();
		}

		[Test]
		public void Can_set_and_get_body()
		{
			var body = new byte[] {1, 2, 3};
			msg.Body = body;
			Assert.AreEqual(body, msg.Body);
		}


		[Test]
		public void Can_set_and_get_correlation_id()
		{
			var guid = Guid.NewGuid();
			msg.CorrelationId = guid;
			Assert.AreEqual(guid, msg.CorrelationId);
		}

		[Test]
		public void Can_get_and_set_headers()
		{
			msg.Headers["customer-header"] = "bar";
			Assert.AreEqual("bar", msg.Headers["customer-header"]);
		}

		[Test]
		public void Missing_header_return_null()
		{
			Assert.IsNull(msg.Headers["customer-header"]);
		}

		[Test]
		public void On_init_msg_id_will_not_be_empty()
		{
			Assert.IsFalse(Guid.Empty.Equals(msg.Id));
		}

		[Test]
		public void Cannot_set_message_id_externally()
		{
			Assert.IsNull(
				typeof(QueueMessage).GetProperty("Id").GetSetMethod()
				);
		}
	}
}