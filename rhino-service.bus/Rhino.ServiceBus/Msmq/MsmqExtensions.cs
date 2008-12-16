using System;
using System.ComponentModel;
using System.Messaging;
using System.Runtime.InteropServices;
using System.Transactions;
using Rhino.ServiceBus.Exceptions;

namespace Rhino.ServiceBus.Msmq
{
    public static class MsmqExtensions
    {
        public static void MoveToSubQueue(
            this MessageQueue queue, 
            string subQueueName,
            Message message)
        {
            var fullSubQueueName = @"DIRECT=OS:.\" + queue.QueueName + ";" + subQueueName;
            IntPtr queueHandle = IntPtr.Zero;
            var error = NativeMethods.MQOpenQueue(fullSubQueueName, NativeMethods.MQ_MOVE_ACCESS,
                                                   NativeMethods.MQ_DENY_NONE, ref queueHandle);
            if(error!=0)
                throw new TransportException("Failed to open queue: " + fullSubQueueName,
                    Marshal.GetExceptionForHR(error));
            try
            {
                Transaction current = Transaction.Current;
                IDtcTransaction transaction = null;
                if (current != null && queue.Transactional)
                {
                    transaction = TransactionInterop.GetDtcTransaction(current);
                }

                error = NativeMethods.MQMoveMessage(queue.ReadHandle, queueHandle, 
                    message.LookupId, transaction);
                if (error != 0)
                    throw new TransportException("Failed to move message to queue: " + fullSubQueueName,
                        Marshal.GetExceptionForHR(error));
            }
            finally
            {
                error = NativeMethods.MQCloseQueue(queueHandle);
                if (error != 0)
                    throw new TransportException("Failed to close queue: " + fullSubQueueName,
                        Marshal.GetExceptionForHR(error));
            
            }
        }

        public static MessageQueue CreateQueue(this Uri queueUri, QueueAccessMode accessMode)
        {
            var description = MsmqUtil.GetQueuePath(queueUri);

            MessageQueue queue;
            try
            {
                queue = new MessageQueue(description, accessMode);
            }
            catch (Exception e)
            {
                throw new MessagePublicationException("Could not open queue (" + queueUri + ")", e);
            }
            return queue;
        }

    }
}