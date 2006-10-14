using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts;
using NHibernate.Mapping.Hbm;
using NUnit.Framework;

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
			ListAssert.In("subclass", fieldReference.Names);
			ListAssert.In("class", fieldReference.Names);
			ListAssert.In("joined-subclass", fieldReference.Names);
		}

		[Test]
		public void NodeFieldReferenceTypes()
		{
			ListAssert.In(typeof(@class), fieldReference.Types);
			ListAssert.In(typeof(subclass), fieldReference.Types);
			ListAssert.In(typeof(joinedsubclass), fieldReference.Types);
		}

		[Test]
		public void AddToField()
		{
			@class clz = new @class();
			ISchemaEditorNode child = fieldReference.AddValue(clz);
			ListAssert.In(child,parent.ActiveNodes);
            ListAssert.In(clz,hm.Items);
			Assert.AreSame(clz,child.Value);
			Assert.AreEqual("class",child.Name);
		}

		[Test]
		public void RemoveFromField()
		{
			@class clz = new @class();
			ISchemaEditorNode child = fieldReference.AddValue(clz);
			fieldReference.RemoveValue(child);
			ListAssert.NotIn(child,parent.ActiveNodes);
			ListAssert.NotIn(clz,hm.Items);
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