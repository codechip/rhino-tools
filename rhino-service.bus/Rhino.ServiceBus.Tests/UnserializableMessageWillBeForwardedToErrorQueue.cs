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

            var errMsg = errorQueue.Receive();
            Assert.NotNull(errMsg);
            Assert.Null(o1);
        }
    }
}