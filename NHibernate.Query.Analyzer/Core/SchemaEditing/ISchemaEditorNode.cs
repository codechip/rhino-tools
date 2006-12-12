namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// A node that is used for SchemaEditor
	/// </summary>
	public interface ISchemaEditorNode
	{
		/// <summary>
		/// Collection of all the existing nodes under this one
		/// Read only collection.
		/// </summary>
		SchemaEditorNodeCollection ActiveNodes { get; }

		/// <summary>
		/// Collection of references to all the fields that represent elements.
		/// Read only collection.
		/// </summary>
		NodeFieldReferenceCollection NodeFields { get; }

		/// <summary>
		/// List of all the fields that are attributes and are has value.
		/// Read only collection
		/// </summary>
		FieldReferenceCollection ActiveAttributes { get; }

		/// <summary>
		/// List of all the fields that are attributes regardless of whatever they have value or not.
		/// Read only collection
		/// </summary>
		FieldReferenceCollection Attributes { get; }

		/// <summary>
		/// Add an attribute to the collection.
		/// </summary>
		/// <param name="attribute">the new attribute</param>
		void AddAttribute(AttributeFieldReference attribute);

		/// <summary>
		/// Add an attribute to the active attributes collection.
		/// Adding an attribute twice is a no-op
		/// </summary>
		/// <param name="activeAttribute">The new active attribute</param>
		void ActivateAttribute(AttributeFieldReference activeAttribute);

		/// <summary>
		/// Remove an attribute from the active attributes collection
		/// </summary>
		/// <param name="inActiveAttribute">The attribute to remove from the active collection</param>
		void DeactivateAttribute(AttributeFieldReference inActiveAttribute);

		/// <summary>
		/// Adds a node to the active nodes list
		/// </summary>
		void AddActiveNode(ISchemaEditorNode node);

		/// <summary>
		/// This removes a node from the active nodes list
		/// </summary>
		void RemoveActiveNode(ISchemaEditorNode node);

		/// <summary>
		/// Add a node's field to the <see cref="NodeFields"/> collection
		/// </summary>
		/// <param name="fieldReference">The parameter to add</param>
		void AddNodeField(NodeFieldReference fieldReference);

		/// <summary>
		/// The real object that this object represent
		/// </summary>
		object Value { get; }


		/// <summary>
		/// The parent field reference that refer to this object.
		/// </summary>
		NodeFieldReference FieldReference { get; }
		
		/// <summary>
		/// Gets a value indicating whether this instance has value.
		/// This may mean that the object is not null, or that it is an array with more than 
		/// zero elements.
		/// </summary>
		bool HasValue { get; }

		/// <summary>
		/// Get the xml name of the current node.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// This node has schema errors.
		/// </summary>
		bool HasSchemaError { get; set; }

		/// <summary>
		/// A child of this node has a schema error
		/// </summary>
		bool ChildHasSchemaError { get; set; }
	}
}