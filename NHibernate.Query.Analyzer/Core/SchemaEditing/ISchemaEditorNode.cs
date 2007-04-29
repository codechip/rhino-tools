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