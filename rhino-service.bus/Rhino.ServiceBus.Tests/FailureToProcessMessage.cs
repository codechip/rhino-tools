using System;
using System.Messaging;
using System.Text;
using System.Threading;
using Rhino.ServiceBus.Impl;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class FailureToProcessMessage : MsmqTestBase
    {
        readonly ManualResetEvent gotFirstMessage = new ManualResetEvent(false);
        readonly ManualResetEvent gotSecondMessage = new ManualResetEvent(false);
        bool first = true;

        [Fact]
        public void A_message_that_fails_processing_should_go_back_to_queue_on_non_transactional_queue()
        {
            Transport.MessageArrived += ThrowOnFirstAction();

            Transport.Send(TestQueueUri, DateTime.Today);

            gotFirstMessage.WaitOne();

            gotSecondMessage.Set();
        }

        [Fact]
        public void A_message_that_fails_processing_should_go_back_to_queue_on_transactional_queue()
        {
            TransactionalTransport.MessageArrived += ThrowOnFirstAction();

            TransactionalTransport.Send(TransactionalTestQueueUri, DateTime.Today);

            gotFirstMessage.WaitOne();

            Assert.NotNull(transactionalQueue.Peek());

            gotSecondMessage.Set();
        }

        [Fact]
        public void When_a_message_fails_enough_times_it_is_moved_to_error_queue_using_non_transactional_queue()
        {
            int count = 0;
            Transport.MessageArrived += o =>
            {
                Interlocked.Increment(ref count);
                throw new InvalidOperationException();
            };

            Transport.Send(TestQueueUri, DateTime.Today);

            using (var errorQueue = new MessageQueue(testQueuePath + ";errors"))
            {
                Assert.NotNull(errorQueue.Peek());
                Assert.Equal(5, count);
            }
        }

        [Fact]
        public void When_a_failed_message_arrives_to_error_queue_will_have_another_message_explaining_what_happened()
        {
            int count = 0;
            Transport.MessageArrived += o =>
            {
                Interlocked.Increment(ref count);
                throw new InvalidOperationException();
            };

            Transport.Send(TestQueueUri, DateTime.Today);

            using (var errorQueue = new MessageQueue(testQueuePath + ";errors"))
            {
                errorQueue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                errorQueue.MessageReadPropertyFilter.SetAll();
                errorQueue.Peek();//for debugging

                var messageCausingError = errorQueue.Receive();
                Assert.NotNull(messageCausingError);
                errorQueue.Peek();//for debugging
                var messageErrorDescription = errorQueue.Receive();
                var error = (string)messageErrorDescription.Body;
                Assert.Contains(
                    "System.InvalidOperationException: Operation is not valid due to the current state of the object.",
                    error);

                Assert.Equal(
                    messageCausingError.Id,
                    messageErrorDescription.CorrelationId);
            }
        }

        [Fact]
        public void When_a_message_fails_enough_times_it_is_moved_to_error_queue_using_transactional_queue()
        {
            int count = 0;
            TransactionalTransport.MessageArrived += o =>
            {
                Interlocked.Increment(ref count);
                throw new InvalidOperationException();
            };

            TransactionalTransport.Send(TransactionalTestQueueUri, DateTime.Today);

            using (var errorQueue = new MessageQueue(transactionalTestQueuePath + ";errors"))
            {
                Assert.NotNull(errorQueue.Peek());
                Assert.Equal(5, count);
            }
        }

        private Func<CurrentMessageInformation, bool> ThrowOnFirstAction()
        {
            return o =>
            {
                if (first)
                {
                    first = false;
                    try
                    {
                        throw new InvalidOperationException();
                    }
                    finally
                    {
                        gotFirstMessage.Set();
                    }
                }
                gotSecondMessage.WaitOne();
                return true;
            };
        }
        
        
    }
    public class With_flat_queue_strategy : MsmqFlatQueueTestBase
    {
        public class FailureToProcessMessage : With_flat_queue_strategy
        {
            readonly ManualResetEvent gotFirstMessage = new ManualResetEvent(false);
            readonly ManualResetEvent gotSecondMessage = new ManualResetEvent(false);
            bool first = true;

            [Fact]
            public void A_message_that_fails_processing_should_go_back_to_queue_on_non_transactional_queue()
            {
                Transport.MessageArrived += ThrowOnFirstAction();

                Transport.Send(TestQueueUri, DateTime.Today);

                gotFirstMessage.WaitOne();

                gotSecondMessage.Set();
            }

            [Fact(Skip = "There is a race condition between the transport and the consumer")]
            public void A_message_that_fails_processing_should_go_back_to_queue_on_transactional_queue()
            {
                TransactionalTransport.MessageArrived += ThrowOnFirstAction();

                TransactionalTransport.Send(TransactionalTestQueueUri, DateTime.Today);

                gotFirstMessage.WaitOne();

                Assert.NotNull(transactionalQueue.Peek());

                gotSecondMessage.Set();
            }

            [Fact]
            public void When_a_message_fails_enough_times_it_is_moved_to_error_queue_using_non_transactional_queue()
            {
                int count = 0;
                Transport.MessageArrived += o =>
                {
                    Interlocked.Increment(ref count);
                    throw new InvalidOperationException();
                };

                Transport.Send(TestQueueUri, DateTime.Today);

                using (var errorQueue = new MessageQueue(testQueuePath + "#errors"))
                {
                    Assert.NotNull(errorQueue.Peek());
                    Assert.Equal(5, count);
                }
            }

            [Fact]
            public void When_a_failed_message_arrives_to_error_queue_will_have_another_message_explaining_what_happened()
            {
                int count = 0;
                Transport.MessageArrived += o =>
                {
                    Interlocked.Increment(ref count);
                    throw new InvalidOperationException();
                };

                Transport.Send(TestQueueUri, DateTime.Today);

                using (var errorQueue = new MessageQueue(testQueuePath + "#errors"))
                {
                    errorQueue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                    errorQueue.MessageReadPropertyFilter.SetAll();
                    errorQueue.Peek();//for debugging

                    var messageCausingError = errorQueue.Receive();
                    Assert.NotNull(messageCausingError);
                    errorQueue.Peek();//for debugging
                    var messageErrorDescription = errorQueue.Receive();
                    var error = (string)messageErrorDescription.Body;
                    Assert.Contains(
                        "System.InvalidOperationException: Operation is not valid due to the current state of the object.",
                        error);

                    Assert.Equal(
                        messageCausingError.Id,
                        messageErrorDescription.CorrelationId);
                }
            }

            [Fact]
            public void When_a_message_fails_enough_times_it_is_moved_to_error_queue_using_transactional_queue()
            {
                int count = 0;
                TransactionalTransport.MessageArrived += o =>
                {
                    Interlocked.Increment(ref count);
                    throw new InvalidOperationException();
                };

                TransactionalTransport.Send(TransactionalTestQueueUri, DateTime.Today);

                using (var errorQueue = new MessageQueue(transactionalTestQueuePath + "#errors"))
                {
                    Assert.NotNull(errorQueue.Peek());
                    Assert.Equal(5, count);
                }
            }

            private Func<CurrentMessageInformation, bool> ThrowOnFirstAction()
            {
                return o =>
                {
                    if (first)
                    {
                        first = false;
                        try
                        {
                            throw new InvalidOperationException();
                        }
                        finally
                        {
                            gotFirstMessage.Set();
                        }
                    }
                    gotSecondMessage.WaitOne();
                    return true;
                };
            }
        }
    }
}
    
    
    
