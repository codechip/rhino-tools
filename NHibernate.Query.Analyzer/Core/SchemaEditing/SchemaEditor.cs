using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Allows for Object Oriented way to edit xml documents 
	/// that are obey a schema.
	/// </summary>
	public class SchemaEditor
	{
		#region Variables
		private Type rootClass;
		private XmlSerializer serializer;
		private object root;
		private ISchemaEditorNode rootNode;
		public ISchemaEditorNodeFactory factory;
		#endregion 

		#region c'tors
		
		public SchemaEditor(Type rootClass, ISchemaEditorNodeFactory factory)
		{
			this.factory = factory;
			serializer = new XmlSerializer(rootClass);
			this.rootClass = rootClass;
		}
		
		#endregion 

		#region Properties
		public object Root
		{
			get { return root; }
		}

		public ISchemaEditorNode RootNode
		{
			get { return rootNode; }
		}
		#endregion 

		#region Implementation Details
		
		internal static void BuildNodeState(ISchemaEditorNode node, ISchemaEditorNodeFactory factory)
		{
			if (ReflectionUtil.HasValue(node.Value) == false)
				return;
			PopulateAttributes(node);
			PopulateElements(node, factory);
			PopulateTextField(node);
			CheckSchemaCompliance(node);
		}

		private static void PopulateTextField(ISchemaEditorNode node)
		{
			Type type = node.Value.GetType();
			FieldInfoCollection fields = TextFields(type);
			if(fields.Count==0)//there can be only one text field
				return;
			TextNodeFieldReference fieldReference = new TextNodeFieldReference(node.Value, fields[0]);
			node.AddAttribute(fieldReference);
		}

		private static void PopulateElements(ISchemaEditorNode node, ISchemaEditorNodeFactory factory)
		{
			Type nodeObjectType = node.Value.GetType();
			foreach (FieldInfo field in ElementFields(nodeObjectType))
			{
				if(field.FieldType.IsArray)
					AddArrayField(node,field, factory);
				else
					AddField(node,field, factory);
			}
		}

		private static void AddField(ISchemaEditorNode node, FieldInfo field, ISchemaEditorNodeFactory factory)
		{
			object fieldValue = field.GetValue(node.Value);
			NodeFieldReference fieldReference = new NodeFieldReference(node.Value, field, factory, node);
			node.AddNodeField(fieldReference);
			if (ReflectionUtil.HasValue(fieldValue) )
				AddActiveChildNode(node,fieldReference, fieldValue, factory);
		}

		private static void AddArrayField(ISchemaEditorNode node, FieldInfo field, ISchemaEditorNodeFactory factory)
		{
			object fieldValue = field.GetValue(node.Value);
			ArrayNodeFieldReference fieldReference = new ArrayNodeFieldReference(node.Value, 
				field, factory, node);
			node.AddNodeField(fieldReference);
			if (ReflectionUtil.HasValue(fieldValue))
			{
				foreach (object o in (Array) fieldValue)
				{
					AddActiveChildNode(node, fieldReference, o, factory);
				}
			}	
		}

		private static ISchemaEditorNode AddActiveChildNode(ISchemaEditorNode parentNode, NodeFieldReference fieldReference, object fieldValue, ISchemaEditorNodeFactory factory)
		{
			ISchemaEditorNode child =  fieldReference.AddExistingValue(fieldValue);
			BuildNodeState(child, factory);
			return child;
		}

		private static void PopulateAttributes(ISchemaEditorNode node)
		{
			Type type = node.Value.GetType();
			NodeAttributeBridge nodeAttributeBridge = new NodeAttributeBridge(node);
			foreach (FieldInfo field in AttributeFields(type))
			{
				AttributeFieldReference attributeFieldReference = new AttributeFieldReference(node.Value, field, SpecifiedField(field));
				node.AddAttribute(attributeFieldReference);
				attributeFieldReference.HasValueChanged += new AttributeFieldReference.AttributeFieldReferenceEventHandler(nodeAttributeBridge.AttributeHandler);

				if (attributeFieldReference.HasValue == false)
					continue;
				node.ActivateAttribute(attributeFieldReference);
			}
		}

		private static FieldInfoCollection ElementFields(Type type)
		{
			return ReflectionUtil.GetFieldsWithOutAttributes(type, typeof (XmlAttributeAttribute), typeof (XmlIgnoreAttribute), typeof (XmlTextAttribute));
		}

		private static FieldInfoCollection AttributeFields(Type type)
		{
			return ReflectionUtil.GetFieldsWithAttribute(type, typeof (XmlAttributeAttribute));
		}

		private static FieldInfoCollection TextFields(Type type)
		{
			return ReflectionUtil.GetFieldsWithAttribute(type, typeof (XmlTextAttribute));
		}

		private static FieldInfo SpecifiedField(FieldInfo field)
		{
			return field.DeclaringType.GetField(field.Name+"Specified", BindingFlags.Instance|BindingFlags.Public);
		}

		internal class NodeAttributeBridge
		{
			private ISchemaEditorNode node;

			public NodeAttributeBridge(ISchemaEditorNode node)
			{
				this.node = node;
			}

			public void AttributeHandler(AttributeFieldReference sender, AttributeFieldReferenceEventArgs e)
			{
				if (node.Attributes.Contains(sender) == false)
					throw new InvalidOperationException("HasValueChanged was called to a node that doesn't contain the attribute.");
				if (e.HasValue)
					node.ActivateAttribute(sender);
				else
					node.DeactivateAttribute(sender);
			}
		}
		#endregion 

		#region Public Methods

		public static string GetTypeName(Type type)
		{
			XmlRootAttribute[] att = (XmlRootAttribute[]) type.GetCustomAttributes(typeof (XmlRootAttribute), false);
			if (att.Length == 0 || att[0].ElementName == "")
				return type.Name;
			return att[0].ElementName;
		}


		public ISchemaEditorNode CreateChild(ISchemaEditorNode parent, Type type)
		{
			NodeFieldReference fieldReference = parent.NodeFields[type];
			if (fieldReference == null)
				throw new ArgumentException("The node '" + type.Name + "' cannot be created on the node '" + parent + "'");
			object obj = Activator.CreateInstance(type);
			ISchemaEditorNode child = fieldReference.AddValue(obj);
			BuildNodeState(child, factory);
			return child;
		}

		public void RemoveChild(ISchemaEditorNode parent, ISchemaEditorNode node)
		{
			node.FieldReference.RemoveValue(parent);
		}


		public XmlSerializer Serializer
		{
			get { return serializer; }
		}

		public void Read(XmlReader sourceDocument)
		{
			object obj = serializer.Deserialize(sourceDocument);
			ReadObjectRoot(obj);
		}

		public void ReadObjectRoot(object obj)
		{
			root = obj;
			RootNodeFieldReference fieldReference = new RootNodeFieldReference(root);
			rootNode = factory.CreateRoot(fieldReference, root, GetTypeName(root.GetType()));
			fieldReference.ParentNode = rootNode;
			BuildNodeState(rootNode, factory);
		}

		public void CreateDefaultRoot()
		{
			ReadObjectRoot(Activator.CreateInstance(rootClass));
		}

		public void Write(TextWriter destinationDocument)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "\t";
			xmlWriterSettings.NewLineOnAttributes = true;
			XmlWriter writer = XmlWriter.Create(destinationDocument, xmlWriterSettings);
			serializer.Serialize(writer, root);
		}

		public override string ToString()
		{
			StringWriter stringWriter = new StringWriter();
			Write(stringWriter);
			return stringWriter.ToString();
		}

		#endregion 

		#region Static Methods
		
		public static SchemaErrorCollection CheckSchemaCompliance(ISchemaEditorNode node)
		{
			SchemaErrorCollection errors = new SchemaErrorCollection();
			NodeContainsErrors(node, errors);
			return errors;
		}

		public static StringDictionary GetEnumFieldsAndXMLNames(Type type)
		{
			StringDictionary xmlNames = new StringDictionary();
			string name;
			foreach (FieldInfo field in type.GetFields())
			{
				if (field.Name == "value__")
					continue;
				name = field.Name;
				XmlEnumAttribute[] attribs = (XmlEnumAttribute[]) field.
					GetCustomAttributes(typeof(XmlEnumAttribute),false);
				if(attribs !=null && attribs.Length>0)
					name = attribs[0].Name;
				xmlNames.Add(field.Name,name);
			}
			return xmlNames;
		}

		private static bool NodeContainsErrors(ISchemaEditorNode node, SchemaErrorCollection errors)
		{
			bool errorFound = false;
			foreach (IFieldReference attribute in node.Attributes)
			{
				if(attribute.IsRequired && attribute.HasValue==false)
				{
					node.HasSchemaError = true;
					errorFound = true;
					errors.Add(new AttributeSchemaError(attribute, node));
				}
			}

			foreach (NodeFieldReference fieldReference in node.NodeFields)
			{
				if(fieldReference.AmountRequired>fieldReference.AmountExisting)
				{
					node.HasSchemaError = true;
					errorFound =true;
					errors.Add(new ElementSchemaError(fieldReference, node));
				}
			}

			node.HasSchemaError = errorFound;
			errorFound = false;

			foreach (ISchemaEditorNode activeNode in node.ActiveNodes)
			{
				if(NodeContainsErrors(activeNode,errors))
				{
					errorFound = true;
				}
			}
		
			node.ChildHasSchemaError = errorFound;
			return node.ChildHasSchemaError || node.HasSchemaError;
		}

		#endregion 
	}
}