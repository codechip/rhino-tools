using System;
using System.Collections;
using System.Collections.Specialized;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Utilties
{
	[TestFixture]
	public class TextTests
	{
		[Test]
		public void Join()
		{
			StringCollection sc = new StringCollection();
			sc.Add("first");
			sc.Add("second");
			sc.Add("third");
			Assert.AreEqual("first, second, third", Text.Join(", ", sc), "Join doesn't join string collection properly");
		}

		[Test]
		public void BooleanParseValidValues()
		{
			Assert.IsTrue(Text.ParseBool("true", false), "'true' failed parsing");
			Assert.IsTrue(Text.ParseBool("True", false), "'True' failed parsing");
			Assert.IsFalse(Text.ParseBool("false", true), "'false' failed parsing");
			Assert.IsFalse(Text.ParseBool("False", true), "'False' failed parsing");
		}

		[Test]
		public void BooleanParseEmptyValues()
		{
			Assert.IsTrue(Text.ParseBool("", true), "default value: true was not returned");
			Assert.IsFalse(Text.ParseBool("", false), "default value: true was not returned");
		}

		[Test]
		public void BooleanParseNullValues()
		{
			Assert.IsTrue(Text.ParseBool(null, true), "default value: true was not returned");
			Assert.IsFalse(Text.ParseBool(null, false), "default value: true was not returned");
		}

		[Test]
		[ExpectedException(typeof (FormatException))]
		public void BooleanParseInvalidValues()
		{
			Assert.IsTrue(Text.ParseBool("yes", true));
		}

		[Test]
		public void NotNullOrEmpty()
		{
			Assert.IsTrue(Text.NotNullOrEmpty("foobar"), "Good string was marked as empty");
			Assert.IsFalse(Text.NotNullOrEmpty(""), "Empty string was marked good");
			Assert.IsFalse(Text.NotNullOrEmpty(null), "Null string was marked good");
		}

		[Test]
		public void ObjectStateToString()
		{
			SimpleTestObject sto = new SimpleTestObject();
			sto.Nine.Add("foobar");
			string text = Text.ObjectStateToString(sto);
			Assert.IsTrue(text.IndexOf("SimpleTestObject") != -1, "ObjectStateToString doesn't include typename");
			Assert.IsTrue(text.IndexOf("One: first") != -1, "ObjectStateToString doesn't include string property");
			Assert.IsTrue(text.IndexOf("Four: 4") != -1, "ObjectStateToString doesn't include int property");
			Assert.IsTrue(text.IndexOf("Seven: True") != -1, "ObjectStateToString doesn't include boolean property");
			Assert.IsTrue(text.IndexOf("Nine: {IList}") != -1, "ObjectStateToString doesn't include complex (IList) property");
		}

		#region Simple Test Object for ObjectStateToString()

		private class SimpleTestObject
		{
			private string one = "first";
			private int four = 4;

			public IList Nine
			{
				get { return nine; }
			}

			private bool seven = true;
			private IList nine = new ArrayList();

			public string One
			{
				get { return one; }
			}

			public int Four
			{
				get { return four; }
			}

			public bool Seven
			{
				get { return seven; }
			}
		}

		#endregion
	}
}