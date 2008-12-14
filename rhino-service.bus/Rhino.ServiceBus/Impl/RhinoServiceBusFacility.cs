using System;
using System.Collections.Generic;
using System.Configuration;
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
        private Type subscriptionStorageImpl = typeof (MsmqSubscriptionStorage);
        private Type transportImpl = typeof(MsmqTransport);
        private Type serializerImpl = typeof (JsonSerializer);
        private int threadCount = 1;
        private int numberOfRetries = 5;
        private Uri endpoint;
        private Uri errorEndpoint;
        private Uri subscriptionQueue;
        private List<Type> messageModules = new List<Type>();

        public RhinoServiceBusFacility UseMsmqSubscription()
        {
            subscriptionStorageImpl = typeof (MsmqSubscriptionStorage);
            return this;
        }

        public RhinoServiceBusFacility UseMsmqTransport()
        {
            transportImpl = typeof (MsmqTransport);
            return this;
        }

        public RhinoServiceBusFacility UseJsonSerialization()
        {
            serializerImpl = typeof (JsonSerializer);
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
            Kernel.ComponentRegistered+=Kernel_OnComponentRegistered;

            ReadBusConfiguration();
            ReadSubscriptionConfiguration();

            foreach (var type in messageModules)
            {
                Kernel.AddComponent(type.FullName, type);
            }

            Kernel.Register(
                Component.For<IServiceBus,IStartableServiceBus>()
                    .ImplementedBy<DefaultServiceBus>()
                    .Parameters(Parameter.ForKey("modules").Eq(CreateModuleConfigurationNode())),
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
                        errorEndpoint,
                        endpoint
                    }),
                Component.For<IMessageSerializer>()
                    .ImplementedBy(serializerImpl)
                );
        }

        private IConfiguration CreateModuleConfigurationNode()
        {
            var config = new MutableConfiguration("array");
            foreach (var type in messageModules)
            {
                config.CreateChild("item", "${" + type.FullName + "}");
            }
            return config;
        }

        private void Kernel_OnComponentRegistered(string key, IHandler handler)
        {
            if (typeof(IMessageConsumer).IsAssignableFrom(handler.ComponentModel.Implementation)==false)
                return;

            handler.ComponentModel.LifestyleType = LifestyleType.Transient;
        }

        private void ReadSubscriptionConfiguration()
        {
            var subscriptionConfig = FacilityConfig.Children["subscription"];


            var uriString = subscriptionConfig.Attributes["queue"];
            try
            {
                subscriptionQueue = new Uri(uriString);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'queue' on 'subscription' has an invalid value '" + uriString + "'"
                    , e);
            }

        }

        private void ReadBusConfiguration()
        {
            var busConfig = FacilityConfig.Children["bus"];

            var retries = busConfig.Attributes["numberOfRetries"];
            int result;
            if (int.TryParse(retries, out result))
                numberOfRetries = result;

            var threads = busConfig.Attributes["threadCounts"];
            if (int.TryParse(threads,out result))
                threadCount = result;

            var uriString = busConfig.Attributes["endpoint"];
            try
            {
                endpoint = new Uri(uriString);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'endpoint' on 'bus' has an invalid value '" + uriString + "'"        
                    ,e);
            }

            uriString = busConfig.Attributes["errorEndpoint"];
            try
            {
                errorEndpoint = new Uri(uriString);
            }
            catch (Exception e)
            {
                throw new ConfigurationErrorsException(
                    "Attribute 'errorEndpoint' on 'bus' has an invalid value '" + uriString + "'"
                    , e);
            }
        }
    }
}