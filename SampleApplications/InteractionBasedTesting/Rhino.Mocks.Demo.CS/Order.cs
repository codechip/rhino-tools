using System;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo(Rhino.Mocks.RhinoMocks.StrongName)]


namespace Rhino.Mocks.Demo
{
	public class Order
	{
		private decimal total;

		public decimal Total
		{
			get { return total; }
			set { total = value; }
		}

		public decimal GetTotal()
		{
			decimal salesHistory;
			salesHistory = GetSalesHistory();
			if (salesHistory >= 1000)
			{
				if (total >= 100)
				{
					return total - (total * 0.10m);
				}
				else if (total >= 60)
				{
					return total - (total * 0.05m);
				}
			}
			return total;
		}

		protected internal virtual decimal GetSalesHistory()
		{
			decimal salesHistory;
			using (SqlConnection sqlConnection = new SqlConnection("dummy"))
			{
				sqlConnection.Open();
				salesHistory = DataAccess.GetSalesHistory(sqlConnection, this);
			}
			return salesHistory;
		}
	}

	public static class DataAccess
	{
		static Random random = new Random();
		public static decimal GetSalesHistory(SqlConnection connection,
			Order order)
		{
			return random.Next(10, 10000);
		}
	}
}

