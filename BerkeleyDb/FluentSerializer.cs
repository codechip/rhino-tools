using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BerkeleyDb
{
	public class FluentSerializer
	{
		private static IDictionary<SerializationFormat, Action<Stream, object>> serializationActions = new Dictionary
			<SerializationFormat, Action<Stream, object>>
		{
			{
				SerializationFormat.String, (s, o) =>
				{
					var array = o.ToString().ToCharArray();
					var writer = new BinaryWriter(s);
					writer.Write(array.Length);
					writer.Write(array);
				}
			},
			{
				SerializationFormat.Guid, (s, o) =>
				{
					new BinaryWriter(s).Write(((Guid) o).ToByteArray());
				}
			},
			{
				SerializationFormat.Binary, (s, o) =>
				{
					new BinaryFormatter().Serialize(s, o);
				}
			},
		};

		private static IDictionary<SerializationFormat, Func<Stream, object>> deserializationActions = new Dictionary
			<SerializationFormat, Func<Stream, object>>
		{
			{
				SerializationFormat.String, (s) =>
				{
					var reader = new BinaryReader(s);
					var len = reader.ReadInt32();
					var chars = reader.ReadChars(len);
					return new string(chars);
				}
			},
			{
				SerializationFormat.Guid, (s) =>
				{
					var reader = new BinaryReader(s);
					var bytes = reader.ReadBytes(16);
					return new Guid(bytes);
				}
			},
			{
				SerializationFormat.Binary, (s) =>
				{
					return new BinaryFormatter().Deserialize(s);
				}
			},
		};

		private readonly object objectToSerialize;

		public FluentSerializer(object o)
		{
			objectToSerialize = o;
		}

		public void To(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				ms.WriteByte((byte)SerializationFormat);
				serializationActions[SerializationFormat](ms, objectToSerialize);
			}
		}

		public object From(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				var format = (SerializationFormat) ms.ReadByte();
				return deserializationActions[format](ms);
			}
		}

		public byte[] ToBuffer()
		{
			using (var ms = new MemoryStream())
			{
				ms.WriteByte((byte)SerializationFormat);
				serializationActions[SerializationFormat](ms, objectToSerialize);
				return ms.ToArray();
			}
		}

		public SerializationFormat SerializationFormat
		{
			get
			{
				if(objectToSerialize is string)
					return SerializationFormat.String;
				if(objectToSerialize is Guid)
					return SerializationFormat.Guid;
				return SerializationFormat.Binary;
			}
		}
	}

	public enum SerializationFormat : byte
	{
		String,
		Guid,
		Binary
	}
	
}