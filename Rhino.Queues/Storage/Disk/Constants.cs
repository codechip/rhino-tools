namespace Rhino.Queues.Storage.Disk
{
	using System;

	public static class Constants
	{
		public static int OperationSeparator = 0x42FEBCA1;
		public static byte[] OperationSeparatorBytes = BitConverter.GetBytes(OperationSeparator);

		public static Guid TransactionSeparatorGuid 
			= new Guid("b75bfb12-93bb-42b6-acb1-a897239ea3a5");

		public static byte[] TransactionSeparator
			= TransactionSeparatorGuid.ToByteArray();

	}
}