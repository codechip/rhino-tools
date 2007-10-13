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
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.Commons.Binsor
{
    internal class TransformUnknownReferences : ProcessMethodBodiesWithDuckTyping
    {
        private readonly ConstructorInfo _componentReferenceConstructor =
            typeof (ComponentReference).GetConstructor(new Type[] {typeof (string)});

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            IEntity entity = NameResolutionService.Resolve(node.Name);
            if (entity != null)
            {
                base.OnReferenceExpression(node);
                return;
            }
            if (node.Name.StartsWith("@"))
            {
                string refComponentName = node.Name.Substring(1);
                StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(refComponentName);
                ExternalConstructor constructor =
                    new ExternalConstructor(TypeSystemServices, _componentReferenceConstructor);
                MethodInvocationExpression invocation = CodeBuilder.CreateConstructorInvocation(constructor, literal);
                node.ParentNode.Replace(node, invocation);
                return;
            }
            else if (node.ParentNode is MethodInvocationExpression)
            {
                MethodInvocationExpression mie = (MethodInvocationExpression) node.ParentNode;
                //Transform the first parameter of Component ctor to string.
            	string target = mie.Target.ToString();
                if ((target == "Component" || target == "Facility") && mie.Arguments[0] == node)
                {
                    StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(node.Name);
                    mie.Replace(node, literal);
                    return;
                }
            }
            else if (
                //search for the left side of a key in a hash literal expression
                node.ParentNode is ExpressionPair 
                && ((ExpressionPair) node.ParentNode).First == node
                && node.ParentNode.ParentNode is HashLiteralExpression)
            {
                ExpressionPair parent = (ExpressionPair) node.ParentNode;
                StringLiteralExpression literal = CodeBuilder.CreateStringLiteral(node.Name);
                parent.First = literal;
                parent.Replace(node, literal);
                return;
            }

            base.OnReferenceExpression(node);
        }
    }
}