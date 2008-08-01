//using System;
//using System.Collections.Generic;
//using System.Transactions;
//using Rhino.Queues.Storage;
//using System.Linq;

//namespace Rhino.Queues.Impl
//{
//    public class DiskMessageQueueImpl : IMessageQueueImpl, ISinglePhaseNotification
//    {
//        private Transaction transaction;
//        private readonly Destination destination;
//        private readonly IMessageStorage incomingStorage;
//        private readonly IMessageStorage outgoingStorage;
//        private readonly IQueueFactoryImpl queueFactory;
//        private readonly List<Action> commitSyncronizations = new List<Action>();
//        private readonly List<Action> rollbackSyncronizations = new List<Action>();

//        public DiskMessageQueueImpl(
//            Destination destination,
//            IMessageStorage incomingStorage,
//            IMessageStorage outgoingStorage,
//            IQueueFactoryImpl queueFactory)
//        {
//            this.destination = destination;
//            this.incomingStorage = incomingStorage;
//            this.outgoingStorage = outgoingStorage;
//            this.queueFactory = queueFactory;
//        }

//        public Message Send(object msg)
//        {
//            string endpointMapping = queueFactory.GetEndpointFromDestination(destination);
//            if (msg == null)
//                throw new ArgumentNullException("msg");
//            if (msg.GetType().IsSerializable == false)
//                throw new ArgumentException("Message " + msg.GetType().Name + " must be serializable");

//            var now = SystemTime.Now();
//            var message = new Message
//            {
//                Value = msg,
//                Destination = destination,
//                SentAt = now,
//            };
//            AddCommitSyncronization(() =>
//            {
//                outgoingStorage.Add(endpointMapping, new TransportMessage
//                {
//                    Destination = destination,
//                    Message = message,
//                    SendAt = now
//                });
//            });
//            return message;
//        }

//        private void AddCommitSyncronization(Action action)
//        {
//            if (CurrentTransaction == null)
//            {
//                action();// execute immediately if there is no transaction
//                return;
//            }
//            commitSyncronizations.Add(action);
//        }

//        private Transaction CurrentTransaction
//        {
//            get
//            {
//                if (transaction == null)
//                {
//                    if (Transaction.Current == null)
//                        return null;
//                    transaction = Transaction.Current;
//                    transaction.EnlistVolatile(this, EnlistmentOptions.None);
//                }
//                return transaction;
//            }
//        }


//        public Message Recieve()
//        {
//            var message = incomingStorage.PullMessagesFor(destination.Queue).FirstOrDefault();
//            while (message == null)
//            {
//                if (incomingStorage.WaitForNewMessages(destination.Queue) == false)
//                    return null;
//                message = incomingStorage.PullMessagesFor(destination.Queue).FirstOrDefault();
//            }
//            AddRollbackSyncronization(() =>
//            {
//                incomingStorage.Add(destination.Queue, message);
//            });
//            return message.Message;
//        }

//        private void AddRollbackSyncronization(Action action)
//        {
//            if (CurrentTransaction != null)// register only if there is transaction
//            {
//                rollbackSyncronizations.Add(action);
//            }
//        }

//        public void PutAll(TransportMessage[] msgs)
//        {
//            AddCommitSyncronization(() =>
//                {
//                    foreach (var msg in msgs)
//                    {
//                        incomingStorage.Add(destination.Queue, msg);
//                    }
//                });

//        }

//        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
//        {
//            preparingEnlistment.Prepared();
//        }

//        void IEnlistmentNotification.Commit(Enlistment enlistment)
//        {
//            ExecuteCommitSyncronizations();
//            enlistment.Done();
//        }

//        void IEnlistmentNotification.Rollback(Enlistment enlistment)
//        {
//            ExecuteRollbackSyncronizations();
//            enlistment.Done();
//        }

//        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
//        {
//            enlistment.Done();
//        }

//        void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
//        {
//            ExecuteCommitSyncronizations();
//            singlePhaseEnlistment.Committed();
//        }

//        private void ExecuteRollbackSyncronizations()
//        {
//            foreach (var action in rollbackSyncronizations)
//            {
//                action();
//            }
//            commitSyncronizations.Clear();
//            rollbackSyncronizations.Clear();
//        }

//        private void ExecuteCommitSyncronizations()
//        {
//            foreach (var action in commitSyncronizations)
//            {
//                action();
//            }
//            commitSyncronizations.Clear();
//            rollbackSyncronizations.Clear();
//        }

//        public void Dispose()
//        {

//        }
//    }
//}