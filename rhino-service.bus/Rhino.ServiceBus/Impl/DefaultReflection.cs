using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Sagas;
using System.Linq;

namespace Rhino.ServiceBus.Impl
{
    public class DefaultReflection : IReflection
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(DefaultReflection));

        public object CreateInstance(string typeName)
        {
            var type = Type.GetType(typeName, true);
            return Activator.CreateInstance(type);
        }

        public object CreateInstance(Type type, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(type, args);
            }
            catch (Exception e)
            {
                throw new MissingMethodException("No parameterless constructor defined for this object: " + type, e);
            }
        }

        public Type GetType(string type)
        {
            return Type.GetType(type);
        }

        public void InvokeAdd(object instance, object item)
        {
            Type type = instance.GetType();
            MethodInfo method = type.GetMethod("Add", new[] {item.GetType()});
            method.Invoke(instance, new[] {item});
        }

        public void Set(object instance, string name, Func<Type, object> generateValue)
        {
            var type = instance.GetType();
            var property = type.GetProperty(name);
            if (property == null)
            {
                logger.InfoFormat("Could not find property {0} to set on {1}", name, type);
                return;
            }
            var value = generateValue(property.PropertyType);
            property.SetValue(instance, value, null);
        }

        public void Set(object instance, string name, object value)
        {
            Set(instance, name, type => value);
        }

        public object Get(object instance, string name)
        {
            var type = instance.GetType();
            var property = type.GetProperty(name);
            if (property == null)
            {
                logger.InfoFormat("Could not find property {0} to get on {1}", name, type);
                return null;
            }
            return property.GetValue(instance, null);
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

        public string GetNamespaceForXml(object msg)
        {
            var type = msg.GetType();
            if(type.Namespace==null && type.Name.StartsWith("<>"))
                throw new InvalidOperationException("Anonymous types are not supported");

            if (type.Namespace == null)//global types?
            {
                return type.Name
                    .ToLowerInvariant();
            }
            return type.Namespace.Split('.')
                .Last().ToLowerInvariant() + "." + type.Name.ToLowerInvariant();
        }

        public string GetName(object msg)
        {
            return msg.GetType().Name;
        }

        public string GetAssemblyQualifiedNameWithoutVersion(object msg)
        {
            var type = msg.GetType();
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        public IEnumerable<string> GetProperties(object value)
        {
            return value.GetType().GetProperties()
                .Select(x => x.Name);
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