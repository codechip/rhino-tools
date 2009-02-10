namespace Rhino.DistributedHashTable.Util
{
	using System;
	using System.ServiceModel;

	public static class ServiceUtil
	{
		public static void Execute<TSrv>(Uri uri, Action<TSrv> action)
		{
			bool success = false;
			var channel = ChannelFactory<TSrv>.CreateChannel(new NetTcpBinding(), new EndpointAddress(uri));
			var communicationObject = (ICommunicationObject) channel;
			try
			{
				action(channel);
				communicationObject.Close();
				success = true;
			}
			finally
			{
				if (success == false)
					communicationObject.Abort();
			}
		}
	}
}