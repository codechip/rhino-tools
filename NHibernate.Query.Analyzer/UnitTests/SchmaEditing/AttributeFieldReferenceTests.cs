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