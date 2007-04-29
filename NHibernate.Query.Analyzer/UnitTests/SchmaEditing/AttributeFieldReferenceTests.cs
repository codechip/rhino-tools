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


using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using NHibernate.Mapping.Hbm;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing
{
	[TestFixture]
	public class AttributeFieldReferenceTests
	{
		private hibernatemapping hm;
		private AttributeFieldReference fieldReference;

		[SetUp]
		public void SetUp()
		{
			hm = new hibernatemapping();
			fieldReference = new AttributeFieldReference(hm, typeof (hibernatemapping).GetField("defaultcascade"), null);
		}

		[Test]
		public void FieldRefernceProperties()
		{
			Assert.IsFalse(fieldReference.HasValue, "null field is marked as having value");
			Assert.AreEqual("default-cascade", fieldReference.Name, "Bad name");
			Assert.AreEqual(typeof (cascadeStyle), fieldReference.Type, "Bad type");

		}

		[Test]
		public void ChangeFieldValue()
		{
			fieldReference.HasValueChanged += new AttributeFieldReference.AttributeFieldReferenceEventHandler(fieldReference_HasValueChanged_True);
			fieldReference.Value = cascadeStyle.deleteorphan;
			Assert.AreEqual(cascadeStyle.deleteorphan, fieldReference.Value, "Value was not set correctly.");
			Assert.IsTrue(fieldReference.HasValue, "HasValue was not changed.");
		}

		[Test]
		public void ChangeFieldToNull()
		{
			fieldReference.Value = cascadeStyle.deleteorphan;
			fieldReference.HasValueChanged += new AttributeFieldReference.AttributeFieldReferenceEventHandler(fieldReference_HasValueChanged_False);
			fieldReference.Value = cascadeStyle.none;
			Assert.AreEqual(cascadeStyle.none, fieldReference.Value, "Value was not set to default value correctly.");
			Assert.IsFalse(fieldReference.HasValue, "HasValue was not changed.");
		}

		private void fieldReference_HasValueChanged_True(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e)
		{
			Assert.IsTrue(e.HasValue);
		}

		private void fieldReference_HasValueChanged_False(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e)
		{
			Assert.IsFalse(e.HasValue);
		}

	}
}