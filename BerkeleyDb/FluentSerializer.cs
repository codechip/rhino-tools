using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BerkeleyDb
{
	public class FluentSerializer
	{
		private readonly object objectToSerialize;

		public FluentSerializer(object o)
		{
			objectToSerialize = o;
		}

		public void To(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				new BinaryFormatter().Serialize(ms, objectToSerialize);
			}
		}

		public object From(byte[] buffer)
		{
			using (var ms = new MemoryStream(buffer))
			{
				return new BinaryFormatter().Deserialize(ms);
			}
		}
	}
}