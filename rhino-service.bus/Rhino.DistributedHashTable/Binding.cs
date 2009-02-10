namespace Rhino.DistributedHashTable
{
	using System;
	using System.ServiceModel;

	public static class Binding
	{
		public static NetTcpBinding DhtDefault
		{
			get
			{
				return new NetTcpBinding
				{
					OpenTimeout = TimeSpan.FromMilliseconds(500),
					CloseTimeout = TimeSpan.FromMilliseconds(250),
				};
			}
		}
	}
}