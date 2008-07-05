namespace BerkeleyDb
{
	/// <summary>
	/// Sometimes you really feel the inability to get mixins in C#
	/// </summary>
	public abstract class SerializationAndDisposableMiixin : DisposableMixin
	{
		protected static FluentSerializer Deserialize
		{
			get { return new FluentSerializer(null); }
		}

		protected static FluentSerializer Serialize(object o)
		{
			return new FluentSerializer(o);
		}
	}
}