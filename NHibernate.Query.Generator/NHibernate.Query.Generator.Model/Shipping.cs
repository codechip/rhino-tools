using System;
using System.Collections.Generic;
using System.Text;
using Castle.ActiveRecord;

namespace NHibernate.Query.Generator.Model
{
	[ActiveRecord]
	public class Canistar
	{
		private int id;
		private DateTime dateOffset;
		private Lot lot;
		private CanistarState state;

		[Property]
		public CanistarState State
		{
			get { return state; }
			set { state = value; }
		}

		[PrimaryKey]
		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public virtual DateTime DateOffset
		{
			get { return dateOffset; }
			set { dateOffset = value; }
		}

		[Nested]
		public virtual Lot Lot
		{
			get { return lot; }
			set { lot = value; }
		}
	}

	public class Lot
	{
		private DateTime expiration;

		[Property]
		public virtual DateTime Expiration
		{
			get { return expiration; }
			set { expiration = value; }
		}
	}

	public enum CanistarState
	{
		Wait,
		Packager
	}
}
