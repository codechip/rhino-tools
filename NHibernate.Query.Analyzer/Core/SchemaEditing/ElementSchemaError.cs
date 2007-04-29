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
using Ayende.NHibernateQueryAnalyzer.Utilities;

namespace Ayende.NHibernateQueryAnalyzer.SchemaEditing
{
	/// <summary>
	/// Summary description for ElementSchemaError.
	/// </summary>
	public class ElementSchemaError : ISchemaError
	{
		NodeFieldReference field;

		ISchemaEditorNode parentNode;
		private const string oneOptionErrMsg = "The element '{0}' is required to appear on '{1}' {2} time(s), but appears {3} time(s)!",
			errMsg = "Some of the elements ({0}) are required to appear on '{1}' {2} time(s), but appears {3} time(s)!";

		public ElementSchemaError(NodeFieldReference field, ISchemaEditorNode parentNode)
		{
			this.field = field;
			this.parentNode = parentNode;
		}

		public NodeFieldReference Field
		{
			get { return field; }
		}

		public ISchemaEditorNode ParentNode
		{
			get { return parentNode; }
		}

		public string Message
		{
			get
			{
				if(field.Names.Length==1)
					return string.Format(oneOptionErrMsg,
						field.Name,
						parentNode.Name,
						field.AmountRequired, 
						field.AmountExisting);
				else
					return string.Format(errMsg,
						Text.Join(", ",field.Names),
						parentNode.Name,
						field.AmountRequired, 
						field.AmountExisting);
			}
		}

		IFieldReference ISchemaError.Field
		{
			get { return field; }
		}
	}
}
