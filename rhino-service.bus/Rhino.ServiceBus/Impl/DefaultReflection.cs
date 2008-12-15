using System;
using System.Collections.Generic;
using log4net;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;
using System.Linq;

namespace Rhino.ServiceBus.Impl
{
    public class DefaultReflection : IReflection
    {
        private ILog logger = LogManager.GetLogger(typeof(DefaultReflection));

        public object CreateInstance(string typeName)
        {
            var type = Type.GetType(typeName, true);
            return Activator.CreateInstance(type);
        }

        public void Set(object instance, string name, object value)
        {
            var type = instance.GetType();
            var property = type.GetProperty(name);
            if (property == null)
            {
                logger.InfoFormat("Could not find property {0} to set on {1}", name, type);
                return;
            }
            if (property.PropertyType == typeof(Guid))
            {
                value = new Guid((string)value);
            }
            property.SetValue(instance, value, null);
        }

        public Type GetGenericTypeOf(Type type, object msg)
        {
            return GetGenericTypeOf(type, msg.GetType());
        }

        public Type GetGenericTypeOf(Type type, Type paramType)
        {
            return type.MakeGenericType(paramType);
        }

        public void InvokeConsume(object consumer, object msg)
        {
            var type = consumer.GetType();
            var consume = type.GetMethod("Consume", new[] { msg.GetType() });
            consume.Invoke(consumer, new[] { msg });
        }

        public object InvokeSagaPersisterGet(object persister, Guid correlationId)
        {
            var type = persister.GetType();
            var method = type.GetMethod("Get");
            return method.Invoke(persister, new object[] { correlationId });
        }

        public void InvokeSagaPersisterSave(object persister, ISaga entity)
        {
            var type = persister.GetType();
            var method = type.GetMethod("Save");
            method.Invoke(persister, new object[] { entity });
        }

        public void InvokeSagaPersisterComplete(object persister, ISaga entity)
        {
            var type = persister.GetType();
            var method = type.GetMethod("Complete");
            method.Invoke(persister, new object[] { entity });
        }

        public Type[] GetMessagesConsumed(IMessageConsumer consumer)
        {
            var consumerType = consumer.GetType();
            return GetMessagesConsumed(consumerType, type => false);
        }

        public Type[] GetMessagesConsumed(Type consumerType, Predicate<Type> filter)
        {
            var list = new HashSet<Type>();
            var toRemove = new HashSet<Type>();

            var interfaces = consumerType.GetInterfaces();

            foreach (var type in interfaces)
            {
                if (type.IsGenericType == false)
                    continue;

                var definition = type.GetGenericTypeDefinition();

                if (filter(definition))
                {
                    toRemove.Add(type.GetGenericArguments()[0]);
                    continue;
                }

                if (definition != typeof(ConsumerOf<>))
                    continue;

                list.Add(type.GetGenericArguments()[0]);
            }
            list.ExceptWith(toRemove);
            return list.ToArray();
        }

        public object ForAllOf<T>(object instance, Func<T, T> func)
        {
            if (instance is ValueType)
                return instance;
            foreach (var property in instance.GetType().GetProperties())
            {
                if (property.PropertyType != typeof(T))
                    continue;
                T converter = func((T)property.GetValue(instance, null));
                property.SetValue(instance, converter, null);
            }
            return instance;
        }
    }
}