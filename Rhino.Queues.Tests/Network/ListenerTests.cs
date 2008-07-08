using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Queues.Impl;
using Rhino.Queues.Network;

namespace Rhino.Queues.Tests.Network
{
	[TestFixture]
	public class ListenerTests
	{
		[Test]
		public void Can_start_and_stop()
		{
			var listener = new Listener(null, 1, "http://localhost/test/");
			listener.Start();
			listener.Dispose();
		}

		[Test]
		public void Will_reject_requests_without_queue()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();

			using(var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				using (var sr = request.GetRequestStream())
				{
					new BinaryFormatter().Serialize(sr, new[]
					{
						new TransportMessage
						{
							Id = new Guid("ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b"),
							Destination = new Destination
							{
								Queue = "", 
								Server = "self"
							}
						}
					});
				}
				AssertExpectedFailure(request, HttpStatusCode.BadRequest,
					"Message ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b doesn't have a queue specified");
			}
		}

		[Test]
		public void Will_reject_requests_without_destination()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				using (var sr = request.GetRequestStream())
				{
					new BinaryFormatter().Serialize(sr, new[]
					{
						new TransportMessage
						{
							Id = new Guid("ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b"),
							Destination = null
						}
					});
				}
				AssertExpectedFailure(request, HttpStatusCode.BadRequest,
					"Message ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b doesn't have a queue specified");
			}
		}

		[Test]
		public void Will_reject_requests_with_unknown_queues()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				using(var sr = request.GetRequestStream())
				{
					new BinaryFormatter().Serialize(sr, new[]
					{
						new TransportMessage
						{
							Id = new Guid("ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b"),
							Destination = new Destination
							{
								Queue = "does-not-exists", 
								Server = "self"
							}
						}
					});
				}
				AssertExpectedFailure(request, HttpStatusCode.NotFound, "Message ccfb2e24-4065-4ca9-9d20-8232cf8d6d8b specified queue does-not-exists which doesn't exists");
			}
		}


		[Test]
		public void Will_reject_requests_with_no_data()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			queueFactory.Stub(x => x.HasQueue("test")).Return(true).Repeat.Any();

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				request.Headers["queue"] = "test";
				request.GetRequestStream().Dispose();
				AssertExpectedFailure(request, HttpStatusCode.BadRequest, "Failed to deserialize messages because: Attempting to deserialize an empty stream.");
			}
		}

		[Test]
		public void Will_reject_requests_with_bad_data()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			queueFactory.Stub(x => x.HasQueue("test")).Return(true).Repeat.Any();

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				request.Headers["queue"] = "test";
				using (var sr = request.GetRequestStream())
				{
					sr.WriteByte(15);
					sr.WriteByte(12);
					sr.WriteByte(13);
				}
				AssertExpectedFailure(request, HttpStatusCode.BadRequest, "Failed to deserialize messages because: End of Stream encountered before parsing was completed.");
			}
		}

		[Test]
		public void Will_report_error_when_failed_to_add_messages_to_queue()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var messageQueue = MockRepository.GenerateStub<IMessageQueueImpl>();
			queueFactory.Stub(x => x.HasQueue("test")).Return(true).Repeat.Any();
			queueFactory.Stub(x => x.OpenQueueImpl("test")).Return(messageQueue);

			messageQueue.Stub(x => x.PutAll(Arg<TransportMessage[]>.Is.Anything))
				.Throw(new Exception("my error message"));

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				request.Headers["queue"] = "test";
				using (var sr = request.GetRequestStream())
				{
					new BinaryFormatter().Serialize(sr, new[] { new TransportMessage
					{
						Destination = new Destination
						{
							Queue = "test", Server = "self"
						}
					}, });
				}
				AssertExpectedFailure(request, HttpStatusCode.InternalServerError,
					"Failed to accept messages because: my error message");
			}
		}

		[Test]
		public void Will_report_success_when_added_messages_to_queue()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();
			var messageQueue = MockRepository.GenerateStub<IMessageQueueImpl>();
			queueFactory.Stub(x => x.HasQueue("test")).Return(true).Repeat.Any();
			queueFactory.Stub(x => x.OpenQueueImpl("test")).Return(messageQueue);

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				request.Method = "PUT";
				using (var sr = request.GetRequestStream())
				{
					new BinaryFormatter().Serialize(sr, new[] { new TransportMessage
					{
						Destination = new Destination
						{
							Queue = "test", Server = "self"
						}
					}, });
				}
				request.GetResponse().Close();// no error raised
			}
		}

		[Test]
		public void Will_reject_non_PUT_requests()
		{
			var queueFactory = MockRepository.GenerateStub<IQueueFactoryImpl>();

			using (var listener = new Listener(queueFactory, 1, "http://localhost/test/"))
			{
				listener.Start();

				var request = WebRequest.Create("http://localhost/test/");
				AssertExpectedFailure(request, HttpStatusCode.BadRequest, "Only PUT requests are allowed");
			}
		}

		private void AssertExpectedFailure(WebRequest request, HttpStatusCode statusCode, string expectedContent)
		{
			try
			{
				request.GetResponse();
				Assert.Fail("Expected failure");
			}
			catch (WebException we)
			{
				var response = ((HttpWebResponse)we.Response);
				Assert.AreEqual(response.StatusCode, statusCode);
				Assert.AreEqual(expectedContent, 
				                new StreamReader(response.GetResponseStream())
				                	.ReadToEnd()
				                	.Trim());
			}
		}
	}
}