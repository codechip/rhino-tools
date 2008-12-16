using System;
using System.Collections.Generic;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Internal
{
    public interface IReflection
    {
        object CreateInstance(string typeName);

        object CreateInstance(Type type, params object[]args);

        void Set(object instance, string name, Func<Type,object> generateValue);

        void Set(object instance, string name, object value);

        object ForAllOf<T>(object instance, Func<T, T> func);

        Type GetGenericTypeOf(Type type, object msg);

        Type GetGenericTypeOf(Type type, Type paramType);

        void InvokeConsume(object consumer, object msg);

        Type[] GetMessagesConsumed(IMessageConsumer consumer);

        Type[] GetMessagesConsumed(Type consumerType, Predicate<Type> filter);

        object InvokeSagaPersisterGet(object persister, Guid correlationId);

        void InvokeSagaPersisterSave(object persister, ISaga entity);

        void InvokeSagaPersisterComplete(object persister, ISaga entity);

        string GetNamespaceForXml(object msg);

        string GetAssemblyQualifiedNameWithoutVersion(object msg);

        IEnumerable<string> GetProperties(object value);

        object Get(object instance, string name);

        string GetName(object msg);

        Type GetType(string type);

        void InvokeAdd(object instance, object item);
    }
}