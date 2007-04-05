using System;
using System.Collections.Generic;
using System.Text;

using Castle.ActiveRecord;
namespace NHibernate.Query.Generator.Tests.ActiveRecord
{
	[ActiveRecord("Payments")]
	public class Payment : ActiveRecordBase<Payment>
	{
		private int id;
		private DateTime payDate;
		private Double amount;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public DateTime PayDate
		{
			get { return payDate; }
			set { payDate = value; }
		}

		[Property]
		public Double Amount
		{
			get { return amount; }
			set { amount = value; }
		}

	}
}
