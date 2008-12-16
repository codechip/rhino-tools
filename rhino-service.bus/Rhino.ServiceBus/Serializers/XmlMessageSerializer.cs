using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Rhino.ServiceBus.Internal;
using System.Linq;

namespace Rhino.ServiceBus.Serializers
{
    public class XmlMessageSerializer : IMessageSerializer
    {
        private readonly IReflection reflection;

        public XmlMessageSerializer(IReflection reflection)
        {
            this.reflection = reflection;
        }

        public void Serialize(object[] mesages, Stream messageStream)
        {
            var namespaces = GetNamespaces(mesages);
            var messagesElement = new XElement(namespaces["esb"] + "messages");
            var xml = new XDocument(messagesElement);

            foreach (var m in mesages)
            {
                if (m == null)
                    continue;

                WriteObject(reflection.GetName(m), m, messagesElement, namespaces);
            }

            messagesElement.Add(
                namespaces.Select(x => new XAttribute(XNamespace.Xmlns + x.Key, x.Value))
                );

            var streamWriter = new StreamWriter(messageStream);
            var writer = XmlWriter.Create(streamWriter, new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            });
            if (writer == null)
                throw new InvalidOperationException("Could not create xml writer from stream");

            xml.WriteTo(writer);
            writer.Flush();
            streamWriter.Flush();
        }

        private void WriteObject(string name, object value, XContainer parent, IDictionary<string, XNamespace> namespaces)
        {
            if (ShouldPutAsString(value))
            {
                var ns = reflection.GetNamespaceForXml(value);
                XNamespace xmlNs;
                if (namespaces.TryGetValue(ns, out xmlNs) == false)
                {
                    namespaces[ns] = xmlNs = reflection.GetAssemblyQualifiedNameWithoutVersion(value);
                }
                XName elementName = xmlNs + name;
                parent.Add(new XElement(elementName, FormatAsString(value)));
            }
            else if (value is IEnumerable)
            {
                XElement list = GetContentWithNamespace(value, namespaces, name);
                parent.Add(list);
                foreach (var item in ((IEnumerable)value))
                {
                    if (item == null)
                        continue;

                    WriteObject("value", item, list, namespaces);
                }
            }
            else
            {
                XElement content = GetContentWithNamespace(value, namespaces, name);
                parent.Add(content);
                foreach (var property in reflection.GetProperties(value))
                {
                    var propVal = reflection.Get(value, property);
                    if (propVal == null)
                        continue;
                    WriteObject(property, propVal, content, namespaces);
                }
            }
        }

        private XElement GetContentWithNamespace(object value, IDictionary<string, XNamespace> namespaces, string name)
        {
            var xmlNsAlias = reflection.GetNamespaceForXml(value);
            XNamespace xmlNs;
            if (namespaces.TryGetValue(xmlNsAlias, out xmlNs) == false)
            {
                namespaces[xmlNsAlias] = xmlNs = reflection.GetAssemblyQualifiedNameWithoutVersion(value);
            }

            return new XElement(xmlNs + name);
        }

        private bool ShouldPutAsString(object value)
        {
            return value is ValueType || value is string || value is Uri;
        }

        public static object FromString(Type type, string value)
        {
            if (type == typeof(string))
                return value;

            if (type == typeof(Uri))
                return new Uri(value);

            if (type.IsPrimitive)
                return Convert.ChangeType(value, type);

            if (type == typeof(Guid))
                return new Guid(value);

            if (type == typeof(DateTime))
                return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);

            if (type == typeof(TimeSpan))
                return XmlConvert.ToTimeSpan(value);

            if (type.IsEnum)
                return Enum.Parse(type, value);

            throw new SerializationException("Don't know how to deserialize type: " + type + " from '" + value + "'");
        }

        private static string FormatAsString(object value)
        {
            if (value == null)
                return string.Empty;
            if (value is bool)
                return value.ToString().ToLower();
            if (value is string)
                return value as string;
            if (value is Uri)
                return value.ToString();

            if (value is DateTime)
                return ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            if (value is TimeSpan)
            {
                var ts = (TimeSpan)value;
                return string.Format("P0Y0M{0}DT{1}H{2}M{3}S", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
            if (value is Guid)
                return ((Guid)value).ToString();

            return value.ToString();
        }

        private IDictionary<string, XNamespace> GetNamespaces(object[] mesages)
        {
            var namespaces = new Dictionary<string, XNamespace>
            {
                {"esb", "http://servicebus.hibernatingrhinos.com/2008/12/20/esb"},
            };
            foreach (var msg in mesages)
            {
                namespaces.Add(reflection.GetNamespaceForXml(msg), reflection.GetAssemblyQualifiedNameWithoutVersion(msg));
            }
            return namespaces;
        }

        public object[] Deserialize(Stream message)
        {
            var namespaces = GetNamespaces(new object[0]);
            var document = XDocument.Load(XmlReader.Create(message));
            if (document.Root == null)
                throw new SerializationException("document doesn't have root element");

            if (document.Root.Name != namespaces["esb"] + "messages")
                throw new SerializationException("message doesn't have root element named 'messages'");

            var msgs = new List<object>();
            foreach (var element in document.Root.Elements())
            {
                var type = reflection.GetType(element.Name.NamespaceName);
                var msg = ReadObject(type, element);
                msgs.Add(msg);
            }
            return msgs.ToArray();
        }

        private object ReadObject(Type type, XElement element)
        {
            if (CanParseFromString(type))
            {
                return FromString(type, element.Value);
            }
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ReadList(type, element);
            }
            object instance = reflection.CreateInstance(type);
            foreach (var prop in element.Elements())
            {
                reflection.Set(instance,
                    prop.Name.LocalName,
                    typeFromProperty =>
                    {
                        var propType = reflection.GetType(prop.Name.NamespaceName);
                        return ReadObject(propType ?? typeFromProperty, prop);
                    });
            }
            return instance;
        }

        private static bool CanParseFromString(Type type)
        {
            if (type.IsPrimitive)
                return true;

            if (type == typeof(string))
                return true;

            if (type == typeof(Uri))
                return true;

            if (type == typeof(DateTime))
                return true;

            if (type == typeof(TimeSpan))
                return true;

            if (type == typeof(Guid))
                return true;

            if (type.IsEnum)
                return true;

            return false;
        }

        private object ReadList(Type type, XContainer element)
        {
            object instance;
            Type elementType;
            if (type.IsArray)
            {
                instance = reflection.CreateInstance(type, element.Elements().Count());
                elementType = type.GetElementType();
            }
            else
            {
                instance = reflection.CreateInstance(type);
                elementType = type.GetGenericArguments()[0];
            }
            int index = 0;
            var array = instance as Array;
            foreach (var value in element.Elements())
            {
                var itemType = reflection.GetType(value.Name.NamespaceName);
                object o = ReadObject(itemType ?? elementType, value);
                if (array != null)
                    array.SetValue(o, index);
                else
                    reflection.InvokeAdd(instance, o);

                index += 1;
            }
            return instance;
        }
    }
}