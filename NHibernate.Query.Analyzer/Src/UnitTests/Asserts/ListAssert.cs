using System;
using System.Collections;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts
{
	public sealed class ListAssert
	{
		private ListAssert(){}

		public static void In(object objInList, IList list)
		{
			In(objInList,list, string.Empty);
		}

		public static void In(object objInList, IList list, string message, params object [] args)
		{
			Assert.DoAssert(new InListAssert(objInList, list, message, args));
		}

		public static void NotIn(object objNotInList, IList list)
		{
			NotIn(objNotInList,list, string.Empty);
		}

		public static void NotIn(object objInList, IList list, string message, params object [] args)
		{
			Assert.DoAssert(new NotInListAssert(objInList, list, message, args));
		}

		private class InListAssert : AssertBase
		{
			private readonly IList list;
			private readonly object objInList;


			public InListAssert(object objInList, IList list, string message, object[] args):base(message,args)
			{
				this.objInList = objInList;
				this.list = list;
			}
			public override void Assert()
			{
				if(list.Contains(objInList)==false)
					Fail();
			}
		}
	}

	internal class NotInListAssert : AssertBase
	{
		private readonly object objNotInList;
		private readonly IList list;

		public NotInListAssert(object objNotInList, IList list, string message, object[] args):base(message,args)
		{
			this.objNotInList = objNotInList;
			this.list = list;
		}

		public override void Assert()
		{
			if(list.Contains(objNotInList))
				Fail();
		}
	}
}
