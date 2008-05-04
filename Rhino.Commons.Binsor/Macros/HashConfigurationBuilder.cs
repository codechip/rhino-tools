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
using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Rhino.Commons.Binsor.Configuration;

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class HashConfigurationBuilder : DepthFirstVisitor
	{
		private bool _skip;
		private bool _found;
		private bool _applied;
		private bool _attributesOnly;
		private CompilerErrorCollection _compileErrors;
		private HashLiteralExpression _configuration;
		private Dictionary<string, object> _childContext;

		public bool BuildConfig(Block block, CompilerErrorCollection compileErrors)
		{
			return Build(block, false, compileErrors);
		}

		public bool BuildAttributes(Block block, CompilerErrorCollection compileErrors)
		{
			return Build(block, true, compileErrors);
		}

		private bool Build(Block block, bool attributesOnly, CompilerErrorCollection compileErrors)
		{
			_attributesOnly = attributesOnly;
			_compileErrors = compileErrors;
			_configuration = new HashLiteralExpression();
			_childContext = new Dictionary<string, object>();
			return BuildHashConfiguration(block);
		}

		public bool HasConfiguration
		{
			get { return _configuration.Items.Count > 0; }	
		}

		public HashLiteralExpression HashConfiguration
		{
			get { return _configuration; }
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			_found = true;
			_applied = MacroHelper.IsAssignment(node, out node)
			           && BuildConfigurationNode(node, false);
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression child)
		{
			_found = true;
			if ((_skip = _attributesOnly) == false)
			{
				_applied = BuildConfigurationChild(child);	
			}
		}

		public override void OnStringLiteralExpression(StringLiteralExpression literal)
		{
			_found = true;
			if ((_skip = _attributesOnly) == false)
			{
				_applied = BuildConfigurationChild(literal);
			}
		}

		private bool BuildHashConfiguration(Block block)
		{
			_skip = false;
			_applied = true;

			if (block.HasStatements)
			{
				for(int i = 0; i < block.Statements.Count;)
				{
					Statement statement = block.Statements[i];
					ExpressionStatement expression = statement as ExpressionStatement;

					if (expression == null)
					{
						_compileErrors.Add(CompilerErrorFactory.CustomError(
						                   statement.LexicalInfo, "Unrecgonized configuration syntax"));
						return false;
					}

					_found = false;
					Visit(expression.Expression);

					if (!_found)
					{
						_compileErrors.Add(CompilerErrorFactory.CustomError(
						                   	expression.LexicalInfo, "Unrecgonized configuration syntax"));
						return false;
					}

					if (_applied)
					{
						if (!_skip)
						{
							block.Statements.RemoveAt(i);
						}
						else ++i;
					}
					else
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool BuildConfigurationNode(BinaryExpression node, bool asAttribute)
		{
			return BuildConfigurationNode(node, asAttribute, _configuration);
		}

		private bool BuildConfigurationNode(BinaryExpression node, bool asAttribute,
		                                    HashLiteralExpression configuration)
		{
			ConfigurationNodeVisitor nodeVisitor = new ConfigurationNodeVisitor();
			if (!nodeVisitor.GetNode(node.Left, asAttribute, _compileErrors)) return false;

			if (nodeVisitor.IsAttribute || asAttribute)
			{
				return BuildConfigurationAttribute(nodeVisitor.Node, node.Right, configuration);
			}

			if ((_skip =_attributesOnly) == false)
			{
				return BuildConfigurationChild(nodeVisitor.Node, node.Right);
			}

			return true;
		}

		private bool BuildConfigurationAttribute(Expression node, Expression value,
		                                         HashLiteralExpression configuration)
		{
			ArrayLiteralExpression attributes;
			if (MacroHelper.IsCompoundAssignment(value, out attributes))
			{
				// @attrib1=value1, attrib2=value2: Multiple attributes

				foreach(Expression attribute in attributes.Items)
				{
					if (attribute == attributes.Items[0])
					{
						configuration.Items.Add(new ExpressionPair(node, attribute));
					}
					else if (!BuildAttribute(attribute, configuration))
					{
						return false;
					}
				}
			}
			else
			{
				// @attrib=value: Single attribute
				configuration.Items.Add(new ExpressionPair(node, value));
			}

			return true;
		}

		private bool BuildConfigurationChild(StringLiteralExpression child)
		{
			return BuildConfigurationChild(child, null, null);
		}

		private bool BuildConfigurationChild(Expression node, Expression value)
		{
			ArrayLiteralExpression attribs;
			List<Expression> attributes = null;

			if (MacroHelper.IsCompoundAssignment(value, out attribs))
			{
				attributes = new List<Expression>();

				foreach(Expression attribute in attribs.Items)
				{
					if (attribute == attribs.Items[0])
					{
						value = attribute;
					}
					else
					{
						attributes.Add(attribute);
					}
				}
			}

			return BuildConfigurationChild(node, value, attributes);
		}

		private bool BuildConfigurationChild(MethodInvocationExpression child)
		{
			Block configBlock;
			List<Expression> attributes = new List<Expression>();
			ConfigurationNodeVisitor nodeVisitor = new ConfigurationNodeVisitor();
			if (!nodeVisitor.GetNode(child, _compileErrors)) return false;

			if (!MacroHelper.IsNewBlock(child, out configBlock))
			{
				attributes.AddRange(child.Arguments);

				return BuildConfigurationChild(nodeVisitor.Node, null, attributes);
			}
			else
			{
				if (configBlock.HasStatements)
				{
					HashConfigurationBuilder nested = new HashConfigurationBuilder();
					if (nested.BuildConfig(configBlock, _compileErrors) &&
					    nested.HasConfiguration)
					{
						_configuration.Items.Add(new ExpressionPair(nodeVisitor.Node,
						                                            nested.HashConfiguration));
						return true;
					}
				}
				else
				{
					return BuildConfigurationChild(nodeVisitor.Node, null, null);
				}
			}

			return false;
		}

		private bool BuildConfigurationChild(Expression child, Expression value,
		                                     ICollection<Expression> attributes)
		{
			child = EnsureUniqueChild(child);

			if (attributes != null && attributes.Count > 0)
			{
				HashLiteralExpression childAttributes = new HashLiteralExpression();
				if (value != null)
				{
					childAttributes.Items.Add(new ExpressionPair(
					                          	new StringLiteralExpression("value"), value));
				}

				// child=value, attrib2=value2: Child with attributes
				foreach(Expression attribute in attributes)
				{
					if (!BuildAttribute(attribute, childAttributes))
					{
						return false;
					}
				}

				_configuration.Items.Add(new ExpressionPair(child, childAttributes));
			}
			else
			{
				// child=value: Child without attributes
				_configuration.Items.Add(
					new ExpressionPair(child, value ?? new StringLiteralExpression("")));
			}

			return true;
		}

		private Expression EnsureUniqueChild(Expression node)
		{
			if (node is StringLiteralExpression)
			{
				StringLiteralExpression name = (StringLiteralExpression)node;
				if (_childContext.ContainsKey(name.Value))
				{
					node = CreateChild(name);
				}
				else
				{
					_childContext.Add(name.Value, null);
				}
			}

			return node;
		}

		private bool BuildAttribute(Expression expression, HashLiteralExpression configuration)
		{
			BinaryExpression attribute;
			if (MacroHelper.IsAssignment(expression, out attribute))
			{
				BuildConfigurationNode(attribute, true, configuration);
			}
			else
			{
				_compileErrors.Add(CompilerErrorFactory.CustomError(expression.LexicalInfo,
					"Attributes must be in the format @attrib1=value1, attrib2=value2,..."));
				return false;
			}

			return true;
		}

		private static Expression CreateChild(StringLiteralExpression name)
		{
			MethodInvocationExpression child = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(ChildBuilder).FullName)
				);
			child.Arguments.Add(name);
			return child;
		}
	}
}