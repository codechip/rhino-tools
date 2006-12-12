using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts
{
	public sealed class StringAssert
	{
		private StringAssert(){}


		public static void Like( string input, string pattern)
		{
			Like(input,pattern,string.Empty);
		}

		public static void Like(string input, string pattern, string message, params object[] args)
		{
			Assert.DoAssert(new LikeAssert(input,pattern,message,args));
		}
		
		private class LikeAssert : AssertBase
		{
			string input;
			string pattern;

			public LikeAssert(string input, string pattern,string message, object[] args) : base(message, args)
			{
				this.input = input;
				this.pattern = pattern;
			}


			public override void Assert()
			{
				if(Regex.IsMatch(input,pattern)==false)
					Fail();
			}
		}

	}
}
