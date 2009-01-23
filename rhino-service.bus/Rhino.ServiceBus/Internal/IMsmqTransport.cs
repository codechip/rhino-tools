using System;
using System.Messaging;
using System.Transactions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Msmq;

namespace Rhino.ServiceBus.Internal
{
    public interface IMsmqTransport : ITransport
    {
        MessageQueue Queue { get; }

        void RaiseAdministrativeMessageProcessingCompleted(CurrentMessageInformation information, Exception ex);

        bool RaiseAdministrativeMessageArrived(CurrentMessageInformation information);

        void ProcessMessage(Message message,
                            MessageQueue messageQueue,
                            TransactionScope tx,
                            Func<CurrentMessageInformation, bool> messageRecieved,
                            Action<CurrentMessageInformation, Exception> messageCompleted);
    }
}