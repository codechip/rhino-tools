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
        public static Message TryGetMessageFromQueue(this MessageQueue queue, string messageId)
        {
            try
            {
                return queue.ReceiveById(
                    messageId,
                    queue.GetTransactionType());
            }
            catch (InvalidOperationException)// message was read before we could read it
            {
                return null;
            }
        }

        public static Guid GetMessageId(this Message self)
        {
            if (self.Extension.Length == 0)
                self.Extension = Guid.NewGuid().ToByteArray();
            return new Guid(self.Extension);
        }

        public static MessageQueueTransactionType GetTransactionType(this MessageQueue self)
        {
            if (self.Transactional)
            {
                if (Transaction.Current == null)
                    return MessageQueueTransactionType.Single;
                return MessageQueueTransactionType.Automatic;
            }
            return MessageQueueTransactionType.None;
        }

        public static MessageQueueTransactionType GetSingleMessageTransactionType(this MessageQueue self)
        {
            if (self.Transactional)
            {
                return MessageQueueTransactionType.Single;
            }
            return MessageQueueTransactionType.None;
        }

        public static void ConsumeMessage(this MessageQueue queue, Message message)
        {
            ConsumeMessage(queue, message.Id);    
        }

        public static void ConsumeMessage(this MessageQueue queue, string msgId)
        {
            try
            {
                queue.ReceiveById(msgId, MessageQueueTransactionType.Single);
            }
            catch (InvalidOperationException)
            {
                // could not find message in queue
            }
        }


        public static void MoveToSubQueue(
            this MessageQueue queue,
            string subQueueName,
            Message message)
        {
            var fullSubQueueName = @"DIRECT=OS:.\" + queue.QueueName + ";" + subQueueName;
            IntPtr queueHandle = IntPtr.Zero;
            var error = NativeMethods.MQOpenQueue(fullSubQueueName, NativeMethods.MQ_MOVE_ACCESS,
                                                   NativeMethods.MQ_DENY_NONE, ref queueHandle);
            if (error != 0)
                throw new TransportException("Failed to open queue: " + fullSubQueueName,
                    new Win32Exception(error));
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
                        new Win32Exception(error));
            }
            finally
            {
                error = NativeMethods.MQCloseQueue(queueHandle);
                if (error != 0)
                    throw new TransportException("Failed to close queue: " + fullSubQueueName,
                        new Win32Exception(error));

            }
        }
		
        public static MessageQueue CreateQueue(this Uri queueUri, QueueAccessMode accessMode)
        {
            var queuePath = MsmqUtil.GetQueuePath(queueUri);
        	return MsmqUtil.CreateQueue(queuePath, accessMode);
        }

		

        /// <summary>
        /// Gets the count.
        /// http://blog.codebeside.org/archive/2008/08/27/counting-the-number-of-messages-in-a-message-queue-in.aspx
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static int GetCount(this MessageQueue self)
        {
            if (!MessageQueue.Exists(self.MachineName + @"\" + self.QueueName))
            {
                return 0;
            }

            var props = new NativeMethods.MQMGMTPROPS { cProp = 1 };
            try
            {
                props.aPropID = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(props.aPropID, NativeMethods.PROPID_MGMT_QUEUE_MESSAGE_COUNT);

                props.aPropVar = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.MQPROPVariant)));
                Marshal.StructureToPtr(new NativeMethods.MQPROPVariant { vt = NativeMethods.VT_NULL }, props.aPropVar, false);

                props.status = Marshal.AllocHGlobal(sizeof(int));
                Marshal.WriteInt32(props.status, 0);

                int result = NativeMethods.MQMgmtGetInfo(null, "queue=" + self.FormatName, ref props);
                if (result != 0)
                    throw new Win32Exception(result);

                if (Marshal.ReadInt32(props.status) != 0)
                {
                    return 0;
                }

                var propVar = (NativeMethods.MQPROPVariant)Marshal.PtrToStructure(props.aPropVar, typeof(NativeMethods.MQPROPVariant));
                if (propVar.vt != NativeMethods.VT_UI4)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(propVar.ulVal);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(props.aPropID);
                Marshal.FreeHGlobal(props.aPropVar);
                Marshal.FreeHGlobal(props.status);
            }
        }


    }



}
