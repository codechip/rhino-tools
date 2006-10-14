using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing;
using NHibernate.Mapping.Hbm;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchmaEditing
{

	[TestFixture]
	public class NodeFieldReferenceCollectionTests
	{
		[Test]
		public void IndexByAllNames()
		{
			NodeFieldReferenceCollection nodes = new NodeFieldReferenceCollection();
			hibernatemapping hm = new hibernatemapping();
			NodeFieldReference field = new NodeFieldReference(hm,
				typeof(hibernatemapping).GetField("Items"),
				new SchemaEditorNodeTestFactory(), new SchemaEditorTestNode("blah", null, null));
			nodes.Add(field);
			Assert.AreSame(field,nodes[typeof(@class)]);
			Assert.AreSame(field,nodes[typeof(@joinedsubclass)]);
		}
	}
}
