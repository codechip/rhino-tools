//using System;
//using System.Collections.Generic;
//using Rhino.Queues.Impl;

//namespace Rhino.Queues.Storage.Disk
//{
//    using System.IO;
//    using System.Runtime.Serialization.Formatters.Binary;
//    using System.Transactions;

//    public class DiskMessageStorage : MessageStorageBase
//    {
//        private readonly IDictionary<string, IPersistentQueueImpl> queuesByName =
//            new Dictionary<string, IPersistentQueueImpl>(StringComparer.InvariantCultureIgnoreCase);


//        public DiskMessageStorage(IEnumerable<string> queues)
//        {
//            foreach (var endpoint in new HashSet<string>(queues))
//            {
//                queuesByName.Add(endpoint, new PersistentQueue(endpoint));
//            }
//        }
//        public override IEnumerable<TransportMessage> PullMessagesFor(string name, Predicate<TransportMessage> predicate)
//        {
//            var queue = GetQueue(name);
//            var rejectedEntires = new HashSet<Entry>();
//            while (true)
//            {
//                var entry = queue.Dequeue();
//                if(entry == null)
//                    continue;
//                // we have gone a full cycle, and are now reading the 
//                // items we rejected previous, break to avoid infinite loop
//                if(rejectedEntires.Contains(entry))
//                    yield break;
//                var transportMessage = Deserialize(entry);
//                if(predicate(transportMessage)==false)
//                {
//                    rejectedEntires.Add(entry);
//                    queue.Requeue(entry);
//                    continue;
//                }
//                yield return transportMessage;
//            }
//        }

//        private TransportMessage Deserialize(Entry dequeue)
//        {
//            using(var stream = new MemoryStream(dequeue.Data))
//            {
//                return (TransportMessage)new BinaryFormatter().Deserialize(stream);
//            }
//        }

//        public override IEnumerable<TransportMessage> PullMessagesFor(string name)
//        {
//            return PullMessagesFor(name, x => true);
//        }

//        public override bool Exists(string name)
//        {
//            return queuesByName.ContainsKey(name);
//        }

//        public override IEnumerable<string> Queues
//        {
//            get
//            {
//                return queuesByName.Keys;
//            }
//        }

//        protected override void OnAdd(string name, TransportMessage message)
//        {

//        }

//        private IPersistentQueueImpl GetQueue(string name)
//        {
//            IPersistentQueueImpl queue;
//            if (queuesByName.TryGetValue(name, out queue) == false)
//                throw new ArgumentException("Queue '" + name + "' was not registered");
//            return queue;
//        }

//        public override bool WaitForNewMessages(string name)
//        {
//            return GetQueue(name).WaitForNewMessage();
//        }
//    }
//}