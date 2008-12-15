using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
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
        private Uri endpoint;
        private int numberOfRetries = 5;
        private Type serializerImpl = typeof(JsonSerializer);
        private Uri subscriptionQueue;
        private Type subscriptionStorageImpl = typeof(MsmqSubscriptionStorage);
        private int threadCount = 1;
        private Type transportImpl = typeof(MsmqTransport);

        public RhinoServiceBusFacility UseMsmqSubscription()
        {
            subscriptionStorageImpl = typeof(MsmqSubscriptionStorage);
            return this;
        }

        public RhinoServiceBusFacility UseMsmqTransport()
        {
            transportImpl = typeof(MsmqTransport);
            return this;
        }

        public RhinoServiceBusFacility UseJsonSerialization()
        {
            serializerImpl = typeof(JsonSerializer);
            return this;
        }

        public RhinoServiceBusFacility AddMessageModule<TModule>()
            where TModule : IMessageModule
        {
            messageModules.Add(typeof(TModule));
            return this;
        }

        public RhinoServiceBusFacility ThreadCount(int count)
        {
            threadCount = count;
            return this;
        }

        public RhinoServiceBusFacility NumberOfRetries(int count)
        {
            numberOfRetries = count;
            return this;
        }

        protected override void Init()
        {
            Kernel.ComponentRegistered += Kernel_OnComponentRegistered;

            ReadBusConfiguration();
            ReadManagementConfiguration();
            ReadMessageOwners();

            foreach (Type type in messageModules)
            {
                Kernel.AddComponent(type.FullName, type);
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
                Component.For<ISubscriptionStorage>()
                    .ImplementedBy(subscriptionStorageImpl)
                    .DependsOn(new
                    {
                        subscriptionQueue
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

        private static void Kernel_OnComponentRegistered(string key, IHandler handler)
        {
            if (typeof(IMessageConsumer).IsAssignableFrom(handler.ComponentModel.Implementation) == false)
                return;

            handler.ComponentModel.LifestyleType = LifestyleType.Transient;
        }

        private void ReadManagementConfiguration()
        {
            IConfiguration busConfig = FacilityConfig.Children["subscriptions"];
            if (busConfig == null)
                throw new ConfigurationErrorsException("Could not find 'subscriptions' node in confiuration");

            string uriString = busConfig.Attributes["endpoint"];
            try
            {
                subscriptionQueue = new Uri(uriString);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'endpoint' on 'subscriptions' has an invalid value '" + uriString + "'"
                    , e);
            }
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
            try
            {
                endpoint = new Uri(uriString);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'endpoint' on 'bus' has an invalid value '" + uriString + "'"
                    , e);
            }
        }
    }
}