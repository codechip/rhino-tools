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
using Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing;
using NHibernate.Mapping.Hbm;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchmaEditing
{
	[TestFixture]
	public class NodeFieldReferenceTests
	{
		private hibernatemapping hm;
		private ArrayNodeFieldReference fieldReference;
		private ISchemaEditorNode parent;

		[SetUp]
		public void SetUp()
		{
			hm = new hibernatemapping();
			parent = new SchemaEditorTestNode("hibernate-mapping",hm,fieldReference);
			fieldReference = new ArrayNodeFieldReference(hm, typeof (hibernatemapping).GetField("Items"), new SchemaEditorNodeTestFactory(), parent);
		}

		[Test]
		public void NodeFieldReferenceNames()
		{
			CollectionAssert.Contains(fieldReference.Names,"subclass");
			CollectionAssert.Contains(fieldReference.Names, "class");
			CollectionAssert.Contains(fieldReference.Names,"joined-subclass");
		}

		[Test]
		public void NodeFieldReferenceTypes()
		{
			CollectionAssert.Contains(fieldReference.Types,typeof(@class));
			CollectionAssert.Contains(fieldReference.Types, typeof(subclass));
			CollectionAssert.Contains(fieldReference.Types,typeof(joinedsubclass));
		}

		[Test]
		public void AddToField()
		{
			@class clz = new @class();
			ISchemaEditorNode child = fieldReference.AddValue(clz);
			CollectionAssert.Contains(parent.ActiveNodes,child);
            CollectionAssert.Contains(hm.Items, clz);
			Assert.AreSame(clz,child.Value);
			Assert.AreEqual("class",child.Name);
		}

		[Test]
		public void RemoveFromField()
		{
			@class clz = new @class();
			ISchemaEditorNode child = fieldReference.AddValue(clz);
			fieldReference.RemoveValue(child);
			CollectionAssert.DoesNotContain(parent.ActiveNodes,child);
			CollectionAssert.DoesNotContain(hm.Items, clz);
		}

		[Test]
		public void AccessingArrayAfterRemovingFromIt()
		{
			@class clz1 = new @class(),
				clz2 = new @class(),
				clz3 = new @class();
			ISchemaEditorNode child1 = fieldReference.AddValue(clz1),
				child2 = fieldReference.AddValue(clz2),
				child3 = fieldReference.AddValue(clz3);
			fieldReference.RemoveValue(child2);
			Assert.AreEqual(2,fieldReference.AmountExisting);
			Assert.AreEqual(clz1,hm.Items[0]);
			Assert.AreEqual(clz3,hm.Items[1]);

			
		}

		[Test]
		public void TypeMatching()
		{
			Assert.AreEqual(typeof(@class),fieldReference.TypeMatching("class"));
			Assert.AreEqual(typeof(joinedsubclass),fieldReference.TypeMatching("joined-subclass"));
		}
	}
}