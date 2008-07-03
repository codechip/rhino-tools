using MbUnit.Framework;
using Rhino.Queues.Impl;

namespace Rhino.Queues.Tests
{
	[TestFixture]
	public class Queue_url_tests
	{
		[Test]
		public void Can_get_Queue_url()
		{
			ValidationUtil.ValidateQueueUrl("queue://localhost/foobar");
		}

		[Test]
		[ExpectedArgumentException("invalid queue url: foo")]
		public void Queue_url_must_be_url()
		{
			ValidationUtil.ValidateQueueUrl("foo");
		}

		[Test]
		[ExpectedArgumentException("only 'queue://' url are supported")]
		public void Queue_url_must_start_with_queue()
		{
			ValidationUtil.ValidateQueueUrl("http://foo");
		}

		[Test]
		[ExpectedArgumentException("empty queue name is invalid, exepected 'queue://localhost/someQueueName'")]
		public void Queue_url_must_contain_queue_name()
		{
			ValidationUtil.ValidateQueueUrl("queue://foo");
		}

		[Test]
		[ExpectedArgumentException("invalid queue name '/', queue names cannot contain '/'")]
		public void Queue_url_must_contain_queue_name_when_using_double_slash()
		{
			ValidationUtil.ValidateQueueUrl("queue://foo//");
		}

		[Test]
		[ExpectedArgumentException("empty queue name is invalid, exepected 'queue://localhost/someQueueName'")]
		public void Queue_url_must_contain_queue_name_when_using_single_slash()
		{
			ValidationUtil.ValidateQueueUrl("queue://foo/");
		}

		[Test]
		[ExpectedArgumentException("urls with query paraemters are not suppoerted")]
		public void Queue_url_must_contain_queue_name_question_marks()
		{
			ValidationUtil.ValidateQueueUrl("queue://foo/blah?");
		}

		[Test]
		[ExpectedArgumentException("invalid queue name '*', queue names cannot contain '*'")]
		public void Queue_url_must_contain_queue_name_when_using_star()
		{
			ValidationUtil.ValidateQueueUrl("queue://foo/*");
		}

	}
}