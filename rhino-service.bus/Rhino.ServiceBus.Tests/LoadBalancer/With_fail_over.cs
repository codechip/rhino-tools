using System;
using System.Messaging;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using Xunit;

namespace Rhino.ServiceBus.Tests.LoadBalancer
{
    public class With_fail_over : LoadBalancingTestBase
    {
        private readonly IWindsorContainer container;

        public With_fail_over()
        {
            var interpreter = new XmlInterpreter(@"LoadBalancer\BusWithLoadBalancer.config");
            container = new WindsorContainer(interpreter);
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());

            container.Register(
                Component.For<MsmqLoadBalancer>()
                    .DependsOn(new
                    {
                        threadCount = 1,
                        endpoint = new Uri(loadBalancerQueue),
                        SecondaryLoadBalancer = TestQueueUri2.Uri
                    })
                );
        }

        [Fact]
        public void when_start_load_balancer_that_has_secondary_will_start_sending_heartbeats_to_secondary()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                loadBalancer.Start();

                Message peek = testQueue2.Peek();
                object[] msgs = container.Resolve<IMessageSerializer>().Deserialize(peek.BodyStream);

                Assert.IsType<HeartBeat>(msgs[0]);
                var beat = (HeartBeat)msgs[0];
                Assert.Equal(loadBalancer.Endpoint.Uri, beat.From);
            }
        }


        [Fact]
        public void When_Primary_loadBalacer_recieve_workers_it_sends_them_to_secondary_loadBalancer()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                var wait = new ManualResetEvent(false);
                int timesCalled = 0;
                loadBalancer.SentNewWorkerPersisted += () =>
                {
                    timesCalled += 1;
                    if (timesCalled == 3)
                        wait.Set();
                };


                loadBalancer.Start();

                bus.Start();

                bus.Send(loadBalancer.Endpoint,
                         new ReadyToWork
                         {
                             Endpoint = new Uri("msmq://app1/work1")
                         });

                bus.Send(loadBalancer.Endpoint,
                         new ReadyToWork
                         {
                             Endpoint = new Uri("msmq://app1/work1")
                         });

                bus.Send(loadBalancer.Endpoint,
                         new ReadyToWork
                         {
                             Endpoint = new Uri("msmq://app2/work1")
                         });


                wait.WaitOne();

                var messageSerializer = container.Resolve<IMessageSerializer>();

                using (var workers = new MessageQueue(testQueuePath2, QueueAccessMode.SendAndReceive))
                {
                    int app1 = 0;
                    int app2 = 0;

                    foreach (Message msg in workers.GetAllMessages())
                    {
                        object msgFromQueue = messageSerializer.Deserialize(msg.BodyStream)[0];

                        var newWorkerPersisted = msgFromQueue as NewWorkerPersisted;
                        if (newWorkerPersisted == null)
                            continue;

                        if (newWorkerPersisted.Endpoint.ToString() == "msmq://app1/work1")
                            app1 += 1;
                        else if (newWorkerPersisted.Endpoint.ToString() == "msmq://app2/work1")
                            app2 += 1;
                    }

                    Assert.Equal(app1, 1);
                    Assert.Equal(app2, 1);
                }
            }
        }


        [Fact]
        public void When_Primary_loadBalacer_recieve_endPoints_it_sends_them_to_secondary_loadBalancer()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                var wait = new ManualResetEvent(false);
                int timesCalled = 0;
                loadBalancer.SentNewEndpointPersisted += () =>
                {
                    timesCalled += 1;
                    if (timesCalled == 2)
                        wait.Set();
                };

                loadBalancer.Start();


                using (var loadBalancerMsmqQueue = new MessageQueue(MsmqUtil.GetQueuePath(loadBalancer.Endpoint), QueueAccessMode.SendAndReceive))
                {
                    var queuePath = MsmqUtil.GetQueuePath(TestQueueUri2);
                    loadBalancerMsmqQueue.Send(new Message
                    {
                        ResponseQueue = new MessageQueue(queuePath),
                        Body = "a"
                    }, loadBalancerMsmqQueue.GetTransactionType());

                    loadBalancerMsmqQueue.Send(new Message
                    {
                        ResponseQueue = new MessageQueue(queuePath),
                        Body = "a"
                    }, loadBalancerMsmqQueue.GetTransactionType());

                    queuePath = MsmqUtil.GetQueuePath(TransactionalTestQueueUri);
                    loadBalancerMsmqQueue.Send(new Message
                    {
                        ResponseQueue = new MessageQueue(queuePath),
                        Body = "a"
                    }, loadBalancerMsmqQueue.GetTransactionType());
                }


                wait.WaitOne();

                var messageSerializer = container.Resolve<IMessageSerializer>();

                using (var workers = new MessageQueue(testQueuePath2, QueueAccessMode.SendAndReceive))
                {
                    int work1 = 0;
                    int work2 = 0;

                    foreach (Message msg in workers.GetAllMessages())
                    {
                        object msgFromQueue = messageSerializer.Deserialize(msg.BodyStream)[0];

                        var newEndpointPersisted = msgFromQueue as NewEndpointPersisted;
                        if (newEndpointPersisted == null)
                            continue;

                        var endpoint = newEndpointPersisted.PersistedEndpoint.ToString()
                            .ToLower()
                            .Replace(Environment.MachineName.ToLower(), "localhost");
                        if (endpoint == TestQueueUri2.Uri.ToString().ToLower())
                            work1 += 1;
                        else if (endpoint == TransactionalTestQueueUri.Uri.ToString().ToLower())
                            work2 += 1;
                    }

                    Assert.Equal(work1, 1);
                    Assert.Equal(work2, 1);
                }


            }
        }
    }
}