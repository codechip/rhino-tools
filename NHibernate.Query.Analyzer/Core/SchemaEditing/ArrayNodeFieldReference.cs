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
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Encapsulate access to an array field where each reference is an element
	/// in the array
	/// </summary>
	public class ArrayNodeFieldReference : NodeFieldReference
	{
		internal delegate void ElementRemovedEventHandler(int indexStart);
		internal event ElementRemovedEventHandler ElementRemoved;

		public ArrayNodeFieldReference(object parent, FieldInfo field, ISchemaEditorNodeFactory factory, ISchemaEditorNode node)
			: base(parent,field, factory, node){}


		#region Properties

		public override int AmountExisting
		{
			get
			{
				if(HasValue==false)
					return 0;
				return ((Array)Value).Length;
			}
		}

		#endregion

		public override ISchemaEditorNode AddValue(object obj)
		{
			Validation.NotNull(obj);
			ReflectionUtil.AddToArray(field, ParentNode.Value, obj);
			return AddExistingValue(obj);
		}

		public override ISchemaEditorNode AddExistingValue(object obj)
		{
			int index = ((Array) field.GetValue(ParentNode.Value)).Length - 1;
			ArrayElementNodeFieldReference fieldReference = new ArrayElementNodeFieldReference(index, this);
			ElementRemoved += new ElementRemovedEventHandler(fieldReference.AdjustIndex);
			ISchemaEditorNode node = factory.CreateNode(fieldReference, obj, SchemaEditor.GetTypeName(obj.GetType()));
			fieldReference.ParentNode = node;
			ParentNode.AddActiveNode(node);
			return node;
		}

		protected void OnElementRemoved(int indexStart)
		{
			if (ElementRemoved != null)
				ElementRemoved(indexStart);
		}

		public override void RemoveValue(ISchemaEditorNode childToRemove)
		{
			//in case there is an attempt to remove a node that doesn't belong to this tree
			if(ParentNode.ActiveNodes.Contains(childToRemove)==false)
				throw new InvalidOperationException("The node '"+childToRemove.Name+"' does not belong to the the node '"+ParentNode.Name+"' so it couldn't be removed");
			//Call on the node's ArrayElementNodeFieldReference, which does the real work.
			childToRemove.FieldReference.RemoveValue(ParentNode);
		}



		internal class ArrayElementNodeFieldReference : ArrayNodeFieldReference
		{
			protected int index;
			protected ArrayNodeFieldReference parentReferene;

			public override int AmountExisting
			{
				get { return parentReferene.AmountExisting; }
			}

			public ArrayElementNodeFieldReference(int index, ArrayNodeFieldReference parentReferene) : base(parentReferene.parent, parentReferene.field, parentReferene.factory, parentReferene.parentNode)
			{
				Validation.NotNull(parentReferene);
				this.index = index;
				this.parentReferene = parentReferene;
			}


			public override ISchemaEditorNode AddValue(object obj)
			{
				return parentReferene.AddValue(obj);
			}

			public override ISchemaEditorNode AddExistingValue(object obj)
			{
				return parentReferene.AddExistingValue(obj);
			}

			public int Index
			{
				get { return index; }
			}

			/// <summary>
			/// This is called when an element is removed from the array,
			/// the indexStart is the position of the removed element, all array element
			/// references above it will move themselves one index lower to compensate for this.
			/// </summary>
			public void AdjustIndex(int indexStart)
			{
				if (index > indexStart)
					index--;
			}

			public override void RemoveValue(ISchemaEditorNode node)
			{
				//in case there is an attempt to remove a node that doesn't belong to this tree
				if(node.ActiveNodes.Contains(ParentNode)==false)
					throw new InvalidOperationException("The node '"+node.Name+"' does not belong to the the node '"+ParentNode.Name+"' so it couldn't be removed");
				ReflectionUtil.RemoveFromArray(field, node.Value, index);
				node.RemoveActiveNode(ParentNode);
				ParentNode = null;
				//Remove the assosiation, so this field refernce would be garbage collected.
				parentReferene.ElementRemoved -= new ElementRemovedEventHandler(AdjustIndex);
				parentReferene.OnElementRemoved(index);
			}

		}
	}
}