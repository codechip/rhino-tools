using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Windows.Forms;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;
using Message = System.Messaging.Message;

namespace Rhino.ServiceBus.Viewer
{
    public partial class MainView : Form
    {
        private readonly TreeNode Subscriptions;
        private readonly TreeNode Messages;
        private readonly TreeNode Errors;
        public MainView()
        {
            InitializeComponent();
            Subscriptions = QueueTree.Nodes[0];
            Messages = QueueTree.Nodes[1];
            Errors = QueueTree.Nodes[2];

            var queues = MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName);
            foreach (var queue in queues)
            {
                Queues.Nodes.Add(new TreeNode
                {
                    Text = MsmqUtil.GetQueueUri(queue).ToString(),
                    Tag = queue,
                });
            }
        }

        private void Queues_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var queue = ((MessageQueue)Queues.SelectedNode.Tag);

            QueueTree.BeginUpdate();

            HandleSubscriptions(queue);

            HandleMessages(queue);

            HandleErrors(queue);

            QueueTree.EndUpdate();
        }

        private const int ErrorDescriptionMessageMarker = 0xE7707;
        
        private void HandleErrors(MessageQueue queue)
        {
            Errors.Nodes.Clear();

            using (var errQueue = new MessageQueue(".\\" + queue.QueueName + ";errors"))
            {
                errQueue.Formatter = new XmlMessageFormatter(new[] {typeof (string)});
                foreach (var message in errQueue.GetAllMessages())
                {
                    if(message.AppSpecific == ErrorDescriptionMessageMarker)
                        continue;

                    var  err = errQueue.ReceiveByCorrelationId(message.Id);
                    
                    Messages.Nodes.Add(new TreeNode
                    {
                        Text = message.Label,
                        Tag = new StreamReader(message.BodyStream).ReadToEnd() + 
                        "\r\n-----------\r\n" +
                        err.Body
                    });
                }
            }
        }

        private void HandleMessages(MessageQueue queue)
        {
            Messages.Nodes.Clear();

            foreach (var message in queue.GetAllMessages())
            {
                Messages.Nodes.Add(new TreeNode
                {
                    Text = message.Label,
                    Tag = new StreamReader(message.BodyStream).ReadToEnd()
                });
            }
        }

        private void HandleSubscriptions(MessageQueue queue)
        {
            Subscriptions.Nodes.Clear();

            var subs = new Dictionary<string, List<string>>();

            var serializer = new XmlMessageSerializer(new DefaultReflection());

            using (var subscriptionsQueue = new MessageQueue(".\\" + queue.QueueName + ";subscriptions"))
            {
                foreach (var message in subscriptionsQueue.GetAllMessages())
                {
                    var sub = (AddSubscription)serializer.Deserialize(message.BodyStream)[0];

                    List<string> value;
                    if (subs.TryGetValue(sub.Type, out value) == false)
                    {
                        subs[sub.Type] = value = new List<string>();
                    }
                    value.Add(sub.Endpoint);
                }
            }

            foreach (var sub in subs)
            {
                var node = new TreeNode
                {
                    Text = sub.Key,
                };
                foreach (var type in sub.Value)
                {
                    node.Nodes.Add(new TreeNode
                    {
                        Text = type
                    });
                }
                Subscriptions.Nodes.Add(node);
            }
        }

        private void QueueTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var msg = QueueTree.SelectedNode.Tag as string;
            if (msg == null)
                return;

            MsgText.Text = msg;
        }
    }
}
