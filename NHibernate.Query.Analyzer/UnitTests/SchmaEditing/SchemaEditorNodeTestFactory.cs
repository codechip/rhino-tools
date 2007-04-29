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
using Ayende.NHibernateQueryAnalyzer.SchemaEditing;

namespace Ayende.NHibernateQueryAnalyzer.Tests.SchemaEditing
{
	/// <summary>
	/// Summary description for SchemaEditorNodeTestFactory.
	/// </summary>
	public class SchemaEditorNodeTestFactory : ISchemaEditorNodeFactory
	{
		/// <summary>
		/// Create a node that is an active node of the parent node and that contains the obj
		/// as the internal value.
		/// </summary>
		/// <returns>The newly created node, already registered as an active node of the parent node.</returns>
		public ISchemaEditorNode CreateNode(NodeFieldReference fieldReference, object obj, string name)
		{
			ISchemaEditorNode child = new SchemaEditorTestNode(name, obj, fieldReference);
			return child;
		}

		public ISchemaEditorNode CreateRoot(RootNodeFieldReference reference, object rootObj, string name)
		{
			return CreateNode(reference,rootObj, name);
		}

	}
}