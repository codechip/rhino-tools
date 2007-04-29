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



using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.UserInterface.SchemaEditing
{
	public class WinFormsSchemaEditorNode : TreeNode, ISchemaEditorNode
	{
		private System.Text.RegularExpressions.Regex fullType = new Regex(@"(?:(?:\w[\w|\d]*)\.)*(\w[\w|\d]*), (?:(?:\w[\w|\d]*)\.)*(?:\w[\w|\d]*)", RegexOptions.Compiled);
		private FieldReferenceCollection activeAttributes;
		private SchemaEditorNodeCollection activeNodes;
		private object val;
		private FieldReferenceCollection attributes;
		private FieldReferenceCollection attributesReadOnly;
		private FieldReferenceCollection activeAttributesReadOnly;
		private SchemaEditorNodeCollection activeNodesReadOnly;
		private NodeFieldReferenceCollection nodeFieldsReadOnly;
		private NodeFieldReferenceCollection nodeFields;
		private NodeFieldReference fieldReference;
		private readonly SchemaEditorView view;
		private bool containsSchemaError;
		private string nodeName;
		private bool childHasSchemaError;

		public WinFormsSchemaEditorNode(SchemaEditorView view,  NodeFieldReference fieldReference, object val, string name)
		{
			this.activeAttributes = new FieldReferenceCollection();
			this.attributes = new FieldReferenceCollection();
			this.nodeFields = new NodeFieldReferenceCollection();
			this.activeNodes = new SchemaEditorNodeCollection();
			this.fieldReference = fieldReference;
			this.view = view;
			this.val = val;
			this.attributesReadOnly = FieldReferenceCollection.ReadOnly(attributes);
			this.activeAttributesReadOnly = FieldReferenceCollection.ReadOnly(activeAttributes);
			this.activeNodesReadOnly = SchemaEditorNodeCollection.ReadOnly(activeNodes);
			this.nodeFieldsReadOnly = NodeFieldReferenceCollection.ReadOnly(nodeFields);
			this.nodeName = name;
			RefreshTitle();
		}

		public void RefreshTitle()
		{
			string valueName = ReflectionUtil.GetNameOrEmpty(val);
			Match m = fullType.Match(valueName);
			if(m.Success)
				valueName = m.Groups[1].Value;
			if (valueName!="")
                Text = nodeName + ": " + valueName;
			else
				Text = nodeName;
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
			Nodes.Remove((TreeNode)node);
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

		string ISchemaEditorNode.Name
		{
			get { return Text; }
		}

		public bool HasSchemaError
		{
			get { return containsSchemaError; }
			set
			{
				containsSchemaError = value;
				SetImageIndex( value ? 2 : 0);
			}
		}

		public bool ChildHasSchemaError
		{
			get { return childHasSchemaError; }
			set
			{
				childHasSchemaError = value;
				if(HasSchemaError==false)//to override child's setting
					SetImageIndex( value ? 1 : 0);
			}
		}

		//This is a stupid method that I had to create because
		//if you've an image for the node you /have/ to have a 
		//selected image, otherwise you get the zero based one.
		private void SetImageIndex(int index)
		{
			ImageIndex = index;
			SelectedImageIndex = index;
		}

		public void AddActiveNode(ISchemaEditorNode child)
		{
			Nodes.Add((TreeNode)child);
			activeNodes.Add(child);
		}

		public void DisplayCurrentElementAttributes(ListView attributesList)
		{
			attributesList.Items.Clear();
			foreach (AttributeFieldReference attribute in Attributes)
			{
				string val = attribute.HasValue ? 
					GetValue(attribute) : 
					GetDefaultValue(attribute);
				ListViewItem lvi = new ListViewItem(new string[]{attribute.Name,val} );
				
				if(attribute.IsRequired)
				{
					if(attribute.HasValue==false)			
						lvi.ImageIndex = 2;
					else
						lvi.ImageIndex = 0;
				}
				
				attributesList.Items.Add(lvi);
			}
		}

		private static string GetValue(AttributeFieldReference attribute)
		{
			if(attribute.Type.IsEnum==false)
				return attribute.Value.ToString();
			else
				return GetEnumXmlName(attribute,attribute.Value.ToString());
		}

		private string GetDefaultValue(AttributeFieldReference attribute)
		{
			if (attribute.DefaultValue == null)
				return "";
			if (attribute.Type.IsEnum==false)
				return attribute.DefaultValue.ToString();
			//this is an enum
			return GetEnumXmlName(attribute,attribute.DefaultValue.ToString());
		}

		private static string GetEnumXmlName(AttributeFieldReference attribute, string fieldName)
		{
			return SchemaEditor.GetEnumFieldsAndXMLNames(attribute.Type)[fieldName];
		}
	}
}