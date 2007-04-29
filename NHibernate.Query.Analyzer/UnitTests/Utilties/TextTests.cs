#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.Collections;
using System.Collections.Specialized;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using MbUnit.Framework;

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