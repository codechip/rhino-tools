using System;
using System.IO;
using System.Xml;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using NHibernate.Mapping.Hbm;
using MbUnit.Framework;
using System.Reflection;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing
{
	[TestFixture]
	public class SchemaEditorTests
	{
		protected SchemaEditor schemaEditor;

		private const string ClassName = "Ayende.NHibernateQueryAnalyzer.Core.Project, Ayende.NHibernateQueryAnalyzer";
		#region MappingXml = file(Project.hbm.xml);
        private static string MappingXml = new StreamReader(
            Assembly.GetExecutingAssembly().
                GetManifestResourceStream("Ayende.NHibernateQueryAnalyzer.UnitTests.Mapping.xml"))
            .ReadToEnd();

		#endregion

		[SetUp]
		public void SetUp()
		{
			this.schemaEditor = new SchemaEditor(typeof (hibernatemapping), new SchemaEditorNodeTestFactory());
			this.schemaEditor.Read(new XmlTextReader(new StringReader(MappingXml)));
		}

		[Test]
		public void RootElement()
		{
			Assert.IsNotNull(this.schemaEditor.Root);
			Assert.AreEqual("hibernatemapping",this.schemaEditor.Root.GetType().Name);
		}

		[Test]
		public void RootSchema()
		{
			ISchemaEditorNode node = schemaEditor.RootNode;
			Assert.IsNotNull(node, "Null value returned for the root node");
			Assert.IsTrue(node.HasValue,"Root node has no value!");
			Assert.AreEqual("hibernate-mapping", node.Name, "Bad root node name");
			Assert.AreEqual(schemaEditor.Root, schemaEditor.RootNode.Value);
		}

		[Test]
		public void GraphNodesValues()
		{
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0];
			Assert.AreEqual("class", node.Name, "Can't get the class's name");
			Assert.AreEqual(ClassName, node.ActiveAttributes["name"].Value, "Couldn't get the expected property value");
		}

		[Test]
		public void DeepGraphNodeTraversal()
		{
			//							/hibernate-mapping/class/id/generator
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0].ActiveNodes[0];
			Assert.AreEqual("generator", node.Name, "Wrong node name");
			Assert.AreEqual("class", node.ActiveAttributes[0].Name);
			Assert.AreEqual("identity", node.ActiveAttributes[0].Value);
		}

		[Test]
		public void DeepDelete()
		{
			ISchemaEditorNode @class = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			ISchemaEditorNode id = schemaEditor.CreateChild(@class, typeof(id));
			/*ISchemaEditorNode generator = */schemaEditor.CreateChild(id, typeof(generator));
			schemaEditor.RemoveChild(@class, id);
			Assert.IsNull(@class.ActiveNodes["id"]);
		}

		[Test]
		public void MissingAttributesAreNull()
		{
			//									/hibernate-mapping/class/id
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0];
			Assert.IsNull(node.ActiveAttributes["column"]);
		}

		[Test]
		public void CountActiveNodesForMapping()
		{
			Assert.AreEqual(1,schemaEditor.RootNode.ActiveNodes.Count);
		}

		[Test]
		public void AllAttributesWithNonExistingAttributeReturnNull()
		{
			//									/hibernate-mapping/class/id			
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0];
			Assert.IsNull(node.Attributes["non-exisitng-attributes"], "AllAttributes returned non null value for non existing attributes");
		}

		[Test]
		public void AllAttributesReturnBothValidAndInvalidAttributes()
		{
			//									/hibernate-mapping/class/id			
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0];
			Assert.IsNotNull(node.Attributes["name"], "Couldn't find attribute's 'name' which HasValue==true");
			Assert.IsNotNull(node.Attributes["column"], "Couldn't find attribute's 'column' which HasValue==false");
		}

		[Test]
		public void ChangingValueToNull()
		{
			//							/hibernate-mapping/class/id/generator
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0].ActiveNodes[0];
			AttributeFieldReference classAttribute = (AttributeFieldReference)node.ActiveAttributes[0];
			int prevCount = node.ActiveAttributes.Count;
			Assert.IsTrue(node.ActiveAttributes[0].HasValue, "Attribute doesn't report that it has a value");
			classAttribute.HasValueChanged += new AttributeFieldReference.AttributeFieldReferenceEventHandler(nodeAttributes_EventHanlder_False);
			classAttribute.Value = null;
			Assert.AreEqual(prevCount - 1, node.ActiveAttributes.Count, "Active attributes count was not decrease when setting attribute to null");

		}

		private void nodeAttributes_EventHanlder_False(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e)
		{
			Assert.IsFalse(e.HasValue, "Attribute's has value was not changed");
		}

		private void nodeAttributes_EventHanlder_True(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e)
		{
			Assert.IsTrue(e.HasValue, "Attribute's has value was not changed");
		}

		[Test]
		public void SetAttributeValueToNonNull()
		{
			//									/hibernate-mapping/class/id
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0].ActiveNodes[0];
			((AttributeFieldReference)node.Attributes["column"]).HasValueChanged += new AttributeFieldReference.AttributeFieldReferenceEventHandler(nodeAttributes_EventHanlder_True);
			node.Attributes["column"].Value = "id";
			Assert.IsNotNull(node.ActiveAttributes["column"], "Attribute was not placed in the Attributes collection");
		}

		[Test]
		public void ActiveAttributesInAttributes()
		{
			ISchemaEditorNode node = schemaEditor.RootNode.ActiveNodes[0];
			Assert.IsTrue(node.ActiveAttributes.Count>0);
			foreach (AttributeFieldReference attribute in node.ActiveAttributes)
				CollectionAssert.Contains(schemaEditor.RootNode.ActiveNodes[0].Attributes,attribute);
		}

		[Test]
		public void AddNode()
		{
			ISchemaEditorNode node = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			hibernatemapping hm = schemaEditor.RootNode.Value as hibernatemapping;
			Assert.IsNotNull(node, "Returned a null node");
			CollectionAssert.Contains(schemaEditor.RootNode.ActiveNodes,node);
			Assert.IsNotNull(node.Value, "Newly created node with a null value");
			CollectionAssert.Contains(hm.Items, node.Value);
			Assert.AreEqual(typeof (@class), node.Value.GetType());
		}


		[Test]
		public void RemoveNode()
		{
			ISchemaEditorNode node = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			hibernatemapping hm = schemaEditor.RootNode.Value as hibernatemapping;
			CollectionAssert.Contains(schemaEditor.RootNode.ActiveNodes,node);
			schemaEditor.RemoveChild(schemaEditor.RootNode, node);
			CollectionAssert.DoesNotContain(schemaEditor.RootNode.ActiveNodes,node);
			CollectionAssert.DoesNotContain(hm.Items,node.Value);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void RemoveNodeWhenNotExisting()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(idbag));
			schemaEditor.RemoveChild(id,clazz);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void AddRootNode()
		{
			schemaEditor.RootNode.FieldReference.AddValue(null);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void RemoveRootNode()
		{
			schemaEditor.RootNode.FieldReference.RemoveValue(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddInvalidNode()
		{
			schemaEditor.CreateChild(schemaEditor.RootNode, typeof(int));
		}

		[Test]
		public void AddNodeDeep()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id));
			@class clazzValue = clazz.Value as @class;
			Assert.IsNotNull(id, "Returned a null node");
			CollectionAssert.Contains(clazz.ActiveNodes,id);
			Assert.IsNotNull(id.Value, "Newly created node with a null value");
			Assert.AreSame(id.Value,clazzValue.Item1/*id or composite-id*/,"Didn't set id in class");
			Assert.AreEqual(typeof (id), id.Value.GetType());
		}

		[Test]
		public void RemoveNodeDeep()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id));
			@class clazzValue = clazz.Value as @class;
			schemaEditor.RemoveChild(clazz,id);
			CollectionAssert.DoesNotContain(clazz.ActiveNodes,id);
			Assert.IsNull(clazzValue.Item/*id or composite-id*/,"Didn't reset id in class");
		}


		[Test]
		public void AddNodeDeepArray()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				oneToOne = schemaEditor.CreateChild(clazz,typeof(onetoone));
			@class clazzValue = clazz.Value as @class;
			Assert.IsNotNull(oneToOne, "Returned a null node");
			CollectionAssert.Contains(clazz.ActiveNodes,oneToOne);
			Assert.IsNotNull(oneToOne.Value, "Newly created node with a null value");
			CollectionAssert.Contains(clazzValue.Items, oneToOne.Value);
			Assert.AreEqual(typeof (onetoone), oneToOne.Value.GetType());
		}

		[Test]
		public void RemoveNodeDeepArray()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				oneToOne = schemaEditor.CreateChild(clazz,typeof(onetoone));
			@class clazzValue = clazz.Value as @class;
			schemaEditor.RemoveChild(clazz, oneToOne);
			CollectionAssert.DoesNotContain(clazz.ActiveNodes,oneToOne);
			CollectionAssert.DoesNotContain(clazzValue.Items,oneToOne.Value);
		}

		/// <summary>
		/// Tests the node fields.
		/// </summary>
		[Test]
		public void NodeFields()
		{
			ISchemaEditorNode node = schemaEditor.RootNode;
			Assert.IsNotNull(node.NodeFields[typeof(@class)]);
			Assert.IsNotNull(node.NodeFields[typeof(joinedsubclass)]);
		}

		[Test]
		public void AddNodeArraySingleSingleArray()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id)),
				generator = schemaEditor.CreateChild(id,typeof(generator)),
				param = schemaEditor.CreateChild(generator,typeof(param));
			@class clazzValue = clazz.Value as @class;
			Assert.AreSame(
				((id)clazzValue.Item1).	generator.		param[0],	
				clazz.ActiveNodes[0].	ActiveNodes[0].	ActiveNodes[0].Value,
				"Can't repreduce object graph on three level copy"
				);
			Assert.AreSame(param, clazz.ActiveNodes[0].	ActiveNodes[0].	ActiveNodes[0], "return node was not put into the object graph");
		}

		[Test]
		public void AddNodeArrayArraySingle()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				array = schemaEditor.CreateChild(clazz,typeof(array)),
				key = schemaEditor.CreateChild(array,typeof(key));
			@class clazzValue = clazz.Value as @class;
			Assert.AreSame(
				((array)clazzValue.		Items[0]).		key,	
				clazz.ActiveNodes[0].	ActiveNodes[0].	Value,
				"Can't repreduce object graph on three level copy"
				);
			Assert.AreSame(key, clazz.ActiveNodes[0].ActiveNodes[0], "return node was not put into the object graph");
		}

		[Test]
		public void RequirementsAreObserved()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id));
			Assert.IsTrue(id.HasSchemaError,"Didn't find that node cotains schema's error");
			Assert.AreEqual(1,id.FieldReference.AmountRequired,"didn't recognize a require tag");
			Assert.AreEqual(1, id.FieldReference.AmountExisting,"didn't count propertly");
			Assert.AreEqual(0, id.NodeFields[typeof(generator)].AmountExisting,"didn't count propertly");
			Assert.AreEqual(1, id.NodeFields[typeof(generator)].AmountRequired,"didn't count propertly");
		}

		[Test]
		public void HasSchemaErrors()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id));
			SchemaErrorCollection errors = SchemaEditor.CheckSchemaCompliance(id);
			Assert.IsTrue(id.HasSchemaError,"Didn't detect errors");
			Assert.AreEqual(1,errors.Count,"Wrong number of errors");	
		}

		[Test]
		public void CorrectNodeFields()
		{
			ISchemaEditorNode @class = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			int count=0;
			string names = "";
			foreach (NodeFieldReference reference in @class.NodeFields)
			{
				count+=reference.Names.Length;
				foreach (string s in reference.Names)
				{
					names += s + " - ";
				}
			}
			Assert.AreEqual(28,count,"Didn't get the right count of fields");
		}

		[Test]
		public void HasAttributeSchemaError()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			SchemaErrorCollection errors = SchemaEditor.CheckSchemaCompliance(clazz);
			Assert.AreEqual(typeof(AttributeSchemaError),errors[0].GetType());
			Assert.AreEqual("Attribute 'name' on element 'class' is required!",errors[0].Message);
		}

		[Test]
		public void HasElementSchemaError()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class));
			SchemaErrorCollection errors = SchemaEditor.CheckSchemaCompliance(clazz);
			Assert.AreEqual(typeof(ElementSchemaError),errors[1].GetType());
			StringAssert.Like(errors[1].Message,
				@"Some of the elements \((id|composite-id), (id|composite-id)\) are required to appear on 'class' 1 time\(s\), but appears 0 time\(s\)!");
		}

		[Test]
		public void HadRecursiveSchemaError()
		{
			ISchemaEditorNode clazz = schemaEditor.CreateChild(schemaEditor.RootNode, typeof(@class)),
				id = schemaEditor.CreateChild(clazz,typeof(id));
			SchemaErrorCollection errors = SchemaEditor.CheckSchemaCompliance(clazz);
			Assert.IsTrue(id.HasSchemaError,"Didn't recognize error");
			Assert.IsTrue(clazz.HasSchemaError,"Didn't recognize error");
			Assert.AreEqual(2,errors.Count,"Wrong number of errors");
		}

		[Test]
		public void SerializingProduceSameAsInput()
		{
		    string actual = schemaEditor.ToString();
		    Assert.AreEqual(MappingXml, actual);
		}
	}
}