using System;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NHibernate.Mapping.Hbm;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing
{
	public class SchemaEditorTestNode : ISchemaEditorNode
	{
		private FieldReferenceCollection activeAttributes;
		private SchemaEditorNodeCollection activeNodes;
		private string name;
		private object val;
		private FieldReferenceCollection attributes;
		private FieldReferenceCollection attributesReadOnly;
		private FieldReferenceCollection activeAttributesReadOnly;
		private SchemaEditorNodeCollection activeNodesReadOnly;
		private NodeFieldReferenceCollection nodeFieldsReadOnly;
		private NodeFieldReferenceCollection nodeFields;
		private NodeFieldReference fieldReference;
		private bool conainsSchemaError;
		private bool childhasErrors;

		public SchemaEditorTestNode(string name, object val, NodeFieldReference fieldReference)
		{
			this.activeAttributes = new FieldReferenceCollection();
			this.attributes = new FieldReferenceCollection();
			this.nodeFields = new NodeFieldReferenceCollection();
			this.activeNodes = new SchemaEditorNodeCollection();
			this.fieldReference = fieldReference;
			this.name = name;
			this.val = val;
			this.attributesReadOnly = FieldReferenceCollection.ReadOnly(attributes);
			this.activeAttributesReadOnly = FieldReferenceCollection.ReadOnly(activeAttributes);
			this.activeNodesReadOnly = SchemaEditorNodeCollection.ReadOnly(activeNodes);
			this.nodeFieldsReadOnly = NodeFieldReferenceCollection.ReadOnly(nodeFields);
		}

		public SchemaEditorNodeCollection ActiveNodes
		{
			get { return activeNodesReadOnly; }
		}

		public NodeFieldReference FieldReference
		{
			get { return fieldReference; }
		}

		/// <summary>
		/// Collection of references to all the fields that represent elements.
		/// Read only collection.
		/// </summary>
		public NodeFieldReferenceCollection NodeFields
		{
			get { return nodeFieldsReadOnly; }
		}

		public FieldReferenceCollection ActiveAttributes
		{
			get { return activeAttributesReadOnly; }
		}

		public FieldReferenceCollection Attributes
		{
			get { return attributesReadOnly; }
		}

		/// <summary>
		/// Add an attribute to the collection.
		/// </summary>
		/// <param name="attribute">the new attribute</param>
		public void AddAttribute(AttributeFieldReference attribute)
		{
			attributes.Add(attribute);
		}

		/// <summary>
		/// Add an attribute to the active attributes collection
		/// </summary>
		/// <param name="activeAttribute">The new active attribute</param>
		public void ActivateAttribute(AttributeFieldReference activeAttribute)
		{
			if (!activeAttributes.Contains(activeAttribute))
				activeAttributes.Add(activeAttribute);
		}

		/// <summary>
		/// Remove an attribute from the active attributes collection
		/// </summary>
		/// <param name="inActiveAttribute">The attribute to remove from the active collection</param>
		public void DeactivateAttribute(AttributeFieldReference inActiveAttribute)
		{
			activeAttributes.Remove(inActiveAttribute);
		}

		/// <summary>
		/// This removes a node from the active nodes list
		/// </summary>
		/// <param name="node"></param>
		public void RemoveActiveNode(ISchemaEditorNode node)
		{
			Validation.NotNull(node);
			activeNodes.Remove(node);
		}

		/// <summary>
		/// Add a node's field to the <see cref="ISchemaEditorNode.NodeFields"/> collection
		/// </summary>
		/// <param name="fieldReference">The parameter to add</param>
		public void AddNodeField(NodeFieldReference fieldReference)
		{
			nodeFields.Add(fieldReference);
		}

		public object Value
		{
			get { return val; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has value.
		/// This may mean that the object is not null, or that it is an array with more than 
		/// zero elements.
		/// </summary>
		public bool HasValue
		{
			get{ return ReflectionUtil.HasValue(Value); }
		}

		public string Name
		{
			get { return name; }
		}

		public bool HasSchemaError
		{
			get { return conainsSchemaError; }
			set { conainsSchemaError = value;}
		}

		public bool ChildHasSchemaError
		{
			get {return childhasErrors;}
			set {childhasErrors=value;}
		}

		public void AddActiveNode(ISchemaEditorNode child)
		{
			activeNodes.Add(child);
		}
	}
}