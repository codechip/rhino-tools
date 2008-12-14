using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Serializers
{
    public class JsonSerializer : IMessageSerializer
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        private readonly IReflection reflection;

        public JsonSerializer(IReflection reflection)
        {
            this.reflection = reflection;
        }

        public void Serialize(object[] obj, ITransportMessage message)
        {

            var serialize = serializer.Serialize(
                from o in obj
                where o != null
                let type = o.GetType()
                select new
                {
                    Type = type.FullName + ", " + type.Assembly.FullName,
                    Values = reflection.ForAllOf<DateTime>(o, d => d.ToUniversalTime())
                }
                );
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(serialize);
            streamWriter.Flush();
            message.BodyStream = stream;
        }

        public object[] Deserialize(ITransportMessage message)
        {
            var streamReader = new StreamReader(message.BodyStream);
            var deserializeObjects = (object[])serializer.DeserializeObject(streamReader.ReadToEnd());
            var messages = new List<object>();
            foreach (IDictionary<string, object> serializedObject in deserializeObjects)
            {
                var typeName = (string)serializedObject["Type"];

                var msg = serializedObject["Values"];
                var values = msg as IDictionary<string, object>;
                if (values != null)
                {
                    msg = SetMessagesValues(values, typeName);
                }
                messages.Add(msg);
            }
            return messages.ToArray();
        }

        private object SetMessagesValues(IDictionary<string, object> values, string typeName)
        {
            object msg = reflection.CreateInstance(typeName);
            foreach (var kvp in values)
            {
                object value = kvp.Value;
                if (kvp.Value is DateTime)
                {
                    value = ((DateTime)kvp.Value).ToLocalTime();
                }
                reflection.Set(msg, kvp.Key, value);
            }
            return msg;
        }
    }
}