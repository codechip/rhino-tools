using System.Messaging;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class UnserializableMessageWillBeForwardedToErrorQueue : MsmqTestBase
    {
        [Fact]
        public void Message_send_should_get_routed_to_error_queue()
        {
            object o1 = null;
            
            Transport.MessageArrived += o => o1 = o;
            queue.Send("blah blah not valid");

            using (var errorQueue = new MessageQueue(testQueuePath + ";errors"))
            {
                var errMsg = errorQueue.Receive();
                Assert.NotNull(errMsg);
                Assert.Null(o1);
            }
        }

        [Fact]
        public void Should_raise_event()
        {

            bool wasCalled = false;
            Transport.MessageSerializationException += (message, exception) => wasCalled = true;
            queue.Send("blah blah not valid");

            using (var errorQueue = new MessageQueue(testQueuePath + ";errors"))
            {
                errorQueue.Receive();// wait for message to be processed.
            }

            Assert.True(wasCalled);
        }
    }
}