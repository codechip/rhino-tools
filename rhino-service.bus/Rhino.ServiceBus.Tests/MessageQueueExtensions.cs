using System.Messaging;

namespace Rhino.ServiceBus.Tests
{
    public static class MessageQueueExtensions
    {
        public static void TxSafeSend(this MessageQueue q, object msg)
        {
            q.Send(msg, q.Transactional? MessageQueueTransactionType.Single : MessageQueueTransactionType.None);
        }
    }
}