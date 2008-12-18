using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;

namespace Rhino.ServiceBus.Impl
{
    public class RhinoServiceBusFacility : AbstractFacility
    {
        private readonly List<Type> messageModules = new List<Type>();
        private readonly List<MessageOwner> messageOwners = new List<MessageOwner>();
        private readonly Type serializerImpl = typeof (XmlMessageSerializer);
        private readonly Type transportImpl = typeof (MsmqTransport);
        private Uri endpoint;
        private Uri logEndpoint;
        private int numberOfRetries = 5;
        private readonly Type subscriptionStorageImpl = typeof (MsmqSubscriptionStorage);
        private int threadCount = 1;
        private Type queueStrategyImpl = typeof (SubQueueStrategy);

        public RhinoServiceBusFacility AddMessageModule<TModule>()
            where TModule : IMessageModule
        {
            messageModules.Add(typeof (TModule));
            return this;
        }

        public RhinoServiceBusFacility UseFlatQueueStructure()
        {
            queueStrategyImpl = typeof (FlatQueueStrategy);
            return this;
        }

        public RhinoServiceBusFacility UseSubqueuesQueueStructure()
        {
            queueStrategyImpl = typeof(FlatQueueStrategy);
            return this;
        }

        protected override void Init()
        {
            Kernel.ComponentModelCreated += Kernel_OnComponentModelCreated;

            ReadBusConfiguration();
            ReadMessageOwners();

            foreach (Type type in messageModules)
            {
                Kernel.AddComponent(type.FullName, type);
            }

            if (logEndpoint != null)
            {
                Kernel.Register(
                    Component.For<MessageLoggingModule>()
                        .DependsOn(new {logQueue = logEndpoint})
                    );
                messageModules.Insert(0, typeof (MessageLoggingModule));
            }

            Kernel.Register(
                Component.For<IServiceBus, IStartableServiceBus>()
                    .ImplementedBy<DefaultServiceBus>()
                    .DependsOn(new
                    {
                        messageOwners = messageOwners.ToArray(),
                    })
                    .Parameters(Parameter.ForKey("modules")
                                    .Eq(CreateModuleConfigurationNode())
                    ),
                Component.For<IReflection>()
                    .ImplementedBy<DefaultReflection>(),
                Component.For<IQueueStrategy>()
                    .ImplementedBy(queueStrategyImpl).DependsOn(new{endpoint}),
                Component.For<ISubscriptionStorage>()
                    .ImplementedBy(subscriptionStorageImpl)
                    .DependsOn(new
                    {
                        subscriptionQueue = endpoint
                    }),
                Component.For<ITransport>()
                    .ImplementedBy(transportImpl)
                    .DependsOn(new
                    {
                        numberOfRetries,
                        threadCount,
                        endpoint,
                    }),
                Component.For<IMessageSerializer>()
                    .ImplementedBy(serializerImpl)
                );
        }

        private static void Kernel_OnComponentModelCreated(ComponentModel model)
        {
            if (typeof (IMessageConsumer).IsAssignableFrom(model.Implementation) == false)
                return;

            model.LifestyleType = LifestyleType.Transient;
        }

        private void ReadMessageOwners()
        {
            IConfiguration messageConfig = FacilityConfig.Children["messages"];
            if (messageConfig == null)
                throw new ConfigurationErrorsException("Could not find 'messages' node in confiuration");

            foreach (IConfiguration configuration in messageConfig.Children)
            {
                if (configuration.Name != "add")
                    throw new ConfigurationErrorsException("Unknown node 'messages/" + configuration.Name + "'");

                string assemblyName = configuration.Attributes["assembly"];
                Assembly asm;
                try
                {
                    asm = Assembly.Load(assemblyName);
                }
                catch (Exception e)
                {
                    throw new ConfigurationErrorsException("Could not load assembly " + assemblyName, e);
                }
                string uriString = configuration.Attributes["endpoint"];
                Uri ownerEndpoint;
                try
                {
                    ownerEndpoint = new Uri(uriString);
                }
                catch (Exception e)
                {
                    throw new ConfigurationErrorsException("Invalid endpoint url: " + uriString, e);
                }

                messageOwners.Add(new MessageOwner
                {
                    Assembly = asm,
                    Endpoint = ownerEndpoint
                });
            }
        }

        private IConfiguration CreateModuleConfigurationNode()
        {
            var config = new MutableConfiguration("array");
            foreach (Type type in messageModules)
            {
                config.CreateChild("item", "${" + type.FullName + "}");
            }
            return config;
        }


        private void ReadBusConfiguration()
        {
            IConfiguration busConfig = FacilityConfig.Children["bus"];
            if (busConfig == null)
                throw new ConfigurationErrorsException("Could not find 'bus' node in confiuration");

            string retries = busConfig.Attributes["numberOfRetries"];
            int result;
            if (int.TryParse(retries, out result))
                numberOfRetries = result;

            string threads = busConfig.Attributes["threadCounts"];
            if (int.TryParse(threads, out result))
                threadCount = result;

            string uriString = busConfig.Attributes["endpoint"];
            if (Uri.TryCreate(uriString, UriKind.Absolute, out endpoint) == false)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'endpoint' on 'bus' has an invalid value '" + uriString + "'");
            }

            uriString = busConfig.Attributes["logEndpoint"];
            if (uriString == null)
                return;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out logEndpoint) == false)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'logEndpoint' on 'bus' has an invalid value '" + uriString + "'");
            }
        }
    }
}
