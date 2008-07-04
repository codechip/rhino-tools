namespace BerkeleyDb
{
	public static class BerkeleyDbTransactionExtensions
	{
		public static Txn InnerTransaction(this BerkeleyDbTransaction self)
		{
			if(self ==null)
				return null;
			return self.InnerTransaction;
		}
	}
}