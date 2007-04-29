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
using System.Reflection;
using System.Xml.Serialization;
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	public class NodeFieldReference : IFieldReference
	{
		protected string[] names;
		protected Type[] types;
		protected ISchemaEditorNode parentNode;
		protected FieldInfo field;
		protected object parent;
		protected ISchemaEditorNodeFactory factory;
		private ISchemaEditorNode childNode;
		protected int amountRequired;

		protected NodeFieldReference(){}//For RootNodeReference

		public NodeFieldReference(object parent, FieldInfo field, 
			ISchemaEditorNodeFactory factory, ISchemaEditorNode parentNode)
		{
			Validation.NotNull(parent, field, factory, parentNode);
			this.parent = parent;
			this.field = field;
			this.factory = factory;
			this.parentNode = parentNode;
			GetFieldInformation(field);
		}

		public virtual string[] Names
		{
			get { return names; }
		}

		public virtual Type[] Types
		{
			get { return types; }
		}

		public virtual object Value
		{
			get { return field.GetValue(parent); }
			set { field.SetValue(parent, value); }
		}

		public string Name
		{
			get
			{
				return Names[0];
			}
		}

		/// <summary>
		/// Return true if the inner value is an array with 
		/// elements or a non-null reference.
		/// </summary>
		public virtual bool HasValue
		{
			get { return ReflectionUtil.HasValue(Value); }
		}

		public Type Type
		{
			get { return Types[0]; }
		}

		/// <summary>
		/// Whatever this is a required field.
		/// </summary>
		public bool IsRequired
		{
			get { return AmountRequired>0; }
		}

		public virtual ISchemaEditorNode ParentNode
		{
			get { return parentNode; }
			set { parentNode = value; }
		}

		public virtual ISchemaEditorNode AddValue(object obj)
		{
			field.SetValue(parent,obj);
			return AddExistingValue(obj);
		}

		public virtual ISchemaEditorNode AddExistingValue(object obj)
		{
			childNode = factory.CreateNode(this,obj,SchemaEditor.GetTypeName(obj.GetType()));
			parentNode.AddActiveNode(childNode);
			return childNode;
		}

		public virtual void RemoveValue(ISchemaEditorNode parentNode)
		{
			//in case there is an attempt to remove a node that doesn't belong to this tree
			if(ParentNode != parentNode)
				throw new InvalidOperationException("The node '"+parentNode.Name+"' does not belong to the the node '"+ParentNode.Name+"' so it couldn't be removed");

			ParentNode.RemoveActiveNode(childNode);
			field.SetValue(parent,null);
		}

		protected void GetFieldInformation(FieldInfo field)
		{
			XmlElementAttribute[] elements = (XmlElementAttribute[]) field.GetCustomAttributes(typeof (XmlElementAttribute), false);
			if (elements.Length == 0)
			{
				names = new string[1];
				names[0] = field.Name;
				types = new Type[1];
				types[0] = field.FieldType;
			}
			else
			{
				names = new string[elements.Length];
				types = new Type[elements.Length];
				for (int i = 0; i < elements.Length; i++)
				{
					names[i] = elements[i].ElementName;
					types[i] = elements[i].Type;
					if(types[i]==null)
					{
						types[i] = field.FieldType;
						if(types[i].IsArray)
							types[i] = types[i].GetElementType();
					}
				}
			}
			RequiredTagAttribute [] required = (RequiredTagAttribute[])field.GetCustomAttributes(typeof(RequiredTagAttribute),false);
			if(required.Length>0)
				amountRequired = required[0].MinimumAmount;
		}

		public Type TypeMatching(string name)
		{
			for (int i = 0; i < Names.Length; i++)
			{
				if (name.Equals(Names[i]))
					return Types[i];
			}
			return null;
		}

		public int AmountRequired
		{
			get { return amountRequired; }
		}

		public virtual int AmountExisting
		{
			get { return HasValue ? 1 : 0; }
		}
	}
}