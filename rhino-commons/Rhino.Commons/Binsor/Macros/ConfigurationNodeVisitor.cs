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
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Rhino.Commons.Binsor.Configuration;

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class ConfigurationNodeVisitor : DepthFirstVisitor
	{
		private string _name;
		private Expression _node;
		private bool _isAttribute;
		private CompilerErrorCollection _compileErrors;

		public string Name
		{
			get { return _name; }
		}

		public Expression Node
		{
			get { return _node;}
		}

		public bool IsAttribute
		{
			get { return _isAttribute; }
		}

		public bool GetNode(Expression expression, CompilerErrorCollection compileErrors)
		{
			return GetNode(expression, false, compileErrors);
		}	

		public bool GetNode(Expression expression, bool asAttribute,
		                    CompilerErrorCollection compileErrors)
		{
			_isAttribute = asAttribute;
			_compileErrors = compileErrors;

			Visit(expression);

			if (_node == null)
			{
				_compileErrors.Add(CompilerErrorFactory.CustomError(expression.LexicalInfo,
					"Unrecgonized configuration node syntax"));
				return false;
			}

			return true;
		}

		public override void OnStringLiteralExpression(StringLiteralExpression literal)
		{
			literal.Value = _name = NormalizeName(literal.Value);
			_node = !_isAttribute ? literal : CreateAttribute(_name);
		}

		public override void OnReferenceExpression(ReferenceExpression reference)
		{
			_name = NormalizeName(reference.Name);
			_node = _isAttribute ? CreateAttribute(_name) : new StringLiteralExpression(_name);
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression child)
		{
			ReferenceExpression target = (ReferenceExpression) child.Target;
			_name = NormalizeName(target.Name);
			_node = CreateBuilderIfNeeded(child, _name);
		}

		public override void OnMemberReferenceExpression(MemberReferenceExpression node)
		{
			_name = NormalizeName(node.ToString());
			_node = _isAttribute ? CreateAttribute(_name) : new StringLiteralExpression(_name);
		}

		private static Expression CreateAttribute(string name)
		{
			MethodInvocationExpression attribute = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(AttributeBuilder).FullName)
				);
			attribute.Arguments.Add(new StringLiteralExpression(name));
			return attribute;
		}

		private Expression CreateBuilderIfNeeded(MethodInvocationExpression child, string name)
		{
			int argsCount = child.Arguments.Count;

			if (argsCount >= 1)
			{
				ReferenceExpression builder = child.Arguments[0] as ReferenceExpression;
				if (builder != null)
				{
					Block block;
					if (argsCount > 2 || (argsCount == 2 && !MacroHelper.IsNewBlock(child, out block)))
					{
						_compileErrors.Add(CompilerErrorFactory.CustomError(
							child.Arguments[0].LexicalInfo,
							"Builder syntax must be in the format builder, prop: value,..."));
						return null;
					}

					MethodInvocationExpression builderCtor = new MethodInvocationExpression(builder);
					builderCtor.Arguments.Add(new StringLiteralExpression(name));
					builderCtor.NamedArguments.Extend(child.NamedArguments);
					return builderCtor;
				}
			}
	
			return new StringLiteralExpression(name);
		}

		private string NormalizeName(string name)
		{
			bool isAttribute = name.StartsWith("@");

			if (isAttribute)
			{
				name = name.Substring(1);
			}

			_isAttribute |= isAttribute;

			return name;
		}
	}
}