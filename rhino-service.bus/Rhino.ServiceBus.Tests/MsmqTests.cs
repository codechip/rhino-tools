using System;
using System.Messaging;
using System.Threading;
using System.Transactions;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqBehaviorTests : MsmqTestBase
    {
        [Fact]
        public void Can_rollback_message_to_transactional_queue()
        {
            using (var tx = new TransactionScope())
            {
                transactionalQueue.Send("foo", MessageQueueTransactionType.Automatic);
                tx.Complete();
            }

            Assert.NotNull(transactionalQueue.Peek(TimeSpan.FromSeconds(1)));

            using (new TransactionScope())
            {
                var receive = transactionalQueue.Receive(TimeSpan.FromSeconds(1), MessageQueueTransactionType.Automatic);
                //do not complete tx
            }

            var peek = transactionalQueue.Peek(TimeSpan.FromMilliseconds(1));
            Assert.NotNull(peek);
        }

        [Fact]
        public void Can_use_enumerator_to_go_through_all_messages()
        {
            queue.Send("a");
            queue.Send("b");
            queue.Send("c");

            var enumerator2 = queue.GetMessageEnumerator2();
            Assert.True(enumerator2.MoveNext(TimeSpan.FromSeconds(0)));
            Assert.True(enumerator2.MoveNext(TimeSpan.FromSeconds(0)));
            Assert.True(enumerator2.MoveNext(TimeSpan.FromSeconds(0)));
            Assert.False(enumerator2.MoveNext(TimeSpan.FromSeconds(0)));
        }

        [Fact]
        public void When_begin_peeking_for_messages_will_get_one_per_message()
        {
            int count = 0;
            var wait = new ManualResetEvent(false);
            queue.BeginPeek(TimeSpan.FromSeconds(1), null, delegate(IAsyncResult ar)
            {
                Interlocked.Increment(ref count);
                queue.EndPeek(ar);
                wait.Set();
            });

            queue.Send("test1", MessageQueueTransactionType.None);
            queue.Send("test2", MessageQueueTransactionType.None);

            wait.WaitOne();

            Assert.Equal(1, count);
        }

        [Fact]
        public void Can_call_begin_peek_from_begin_peek()
        {
            int count = 0;
            var wait = new ManualResetEvent(false);

            queue.BeginPeek(TimeSpan.FromSeconds(1), null, delegate(IAsyncResult ar)
            {
                Interlocked.Increment(ref count);
                queue.EndPeek(ar);

                queue.BeginPeek(TimeSpan.FromSeconds(1), null, delegate(IAsyncResult ar2)
                {
                    Interlocked.Increment(ref count);
                    queue.EndPeek(ar2);
                    wait.Set();
                });
            });

            queue.Send("test1");
            queue.Send("test2");
            wait.WaitOne();

            Assert.Equal(2, count);
        }

        [Fact]
        public void Trying_to_send_message_with_large_label()
        {
            queue.Send(new Message
            {
                Label = new string('a', 249),
                Body = "send"
            });
        }

        [Fact]
        public void When_peeking_and_there_is_no_message()
        {
            IAsyncResult asyncResult = queue.BeginPeek(
                TimeSpan.FromMilliseconds(1), null, delegate { });
            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.False(asyncResult.CompletedSynchronously);

            Assert.Throws<MessageQueueException>(
                "Timeout for the requested operation has expired.",
                () => queue.EndPeek(asyncResult));
        }


        [Fact]
        public void When_peeking_and_there_is_no_message_should_get_the_perfect_code()
        {
            IAsyncResult asyncResult = queue.BeginPeek(
                TimeSpan.FromMilliseconds(1), null, delegate { });
            asyncResult.AsyncWaitHandle.WaitOne();

            Assert.False(asyncResult.CompletedSynchronously);

            MessageQueueErrorCode errorCode = 0;
            try
            {
                queue.EndPeek(asyncResult);
                Assert.False(true, "should not get this");
            }
            catch (MessageQueueException e)
            {
                errorCode = e.MessageQueueErrorCode;
            }

            Assert.Equal(MessageQueueErrorCode.IOTimeout, errorCode);
        }
    }
}
