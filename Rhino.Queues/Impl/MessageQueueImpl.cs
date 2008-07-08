using System;
using System.Collections.Generic;
using System.Transactions;
using Rhino.Queues.Storage;
using System.Linq;

namespace Rhino.Queues.Impl
{
	public class MessageQueueImpl : IMessageQueueImpl, ISinglePhaseNotification
	{
		private Transaction transaction;
		private readonly Destination destination;
		private readonly IMessageStorage incomingStorage;
		private readonly IMessageStorage outgoingStorage;
		private readonly IQueueFactoryImpl queueFactory;
		private readonly List<Action> commitSyncronizations = new List<Action>();
		private readonly List<Action> rollbackSyncronizations = new List<Action>();

		public MessageQueueImpl(Destination destination,
			IMessageStorage incomingStorage,
			IMessageStorage outgoingStorage,
			IQueueFactoryImpl queueFactory)
		{
			this.destination = destination;
			this.incomingStorage = incomingStorage;
			this.outgoingStorage = outgoingStorage;
			this.queueFactory = queueFactory;
		}

		public void Send(object msg)
		{
			string endpointMapping = queueFactory.GetEndpointFromDestination(destination);
			if (msg == null)
				throw new ArgumentNullException("msg");
			if (msg.GetType().IsSerializable == false)
				throw new ArgumentException("Message " + msg.GetType().Name + " must be serializable");

			var now = SystemTime.Now();
			AddCommitSyncronization(() =>
			{
				outgoingStorage.Add(endpointMapping, new TransportMessage
				{
					Destination = destination,
					Message = msg,
					SendAt = now
				});
			});
		}

		private void AddCommitSyncronization(Action action)
		{
			if (CurrentTransaction == null)
			{
				action();// execute immediately if there is no transaction
				return;
			}
			commitSyncronizations.Add(action);
		}

		private Transaction CurrentTransaction
		{
			get
			{
				if (transaction == null)
				{
					if (Transaction.Current == null)
						return null;
					transaction = Transaction.Current;
					transaction.EnlistVolatile(this, EnlistmentOptions.None);
				}
				return transaction;
			}
		}


		public object Recieve()
		{
			var message = incomingStorage.GetMessagesFor(destination.Queue).FirstOrDefault();
			while (message == null)
			{
				if (incomingStorage.WaitForNewMessages(destination.Queue) == false)
					return null;
				message = incomingStorage.GetMessagesFor(destination.Queue).FirstOrDefault();
			}
			AddRollbackSyncronization(() =>
			{
				incomingStorage.Add(destination.Queue, message);
			});
			return message.Message;
		}

		private void AddRollbackSyncronization(Action action)
		{
			if (CurrentTransaction != null)// register only if there is transaction
			{
				rollbackSyncronizations.Add(action);
			}
		}

		public void PutAll(TransportMessage[] msgs)
		{
			foreach (var msg in msgs)
			{
				var copy = msg;
				AddCommitSyncronization(() =>
				{
					incomingStorage.Add(destination.Queue, copy);
				});
			}
		}

		void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
		{
			preparingEnlistment.Done();
		}

		void IEnlistmentNotification.Commit(Enlistment enlistment)
		{
			ExecuteCommitSyncronizations();
			enlistment.Done();
		}

		void IEnlistmentNotification.Rollback(Enlistment enlistment)
		{
			ExecuteRollbackSyncronizations();
			enlistment.Done();
		}

		void IEnlistmentNotification.InDoubt(Enlistment enlistment)
		{
			enlistment.Done();
		}

		void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			ExecuteCommitSyncronizations();
			singlePhaseEnlistment.Committed();
		}

		private void ExecuteRollbackSyncronizations()
		{
			foreach (var action in rollbackSyncronizations)
			{
				action();
			}
			commitSyncronizations.Clear();
			rollbackSyncronizations.Clear();
		}

		private void ExecuteCommitSyncronizations()
		{
			foreach (var action in commitSyncronizations)
			{
				action();
			}
			commitSyncronizations.Clear();
			rollbackSyncronizations.Clear();
		}

		public void Dispose()
		{
			
		}
	}
}