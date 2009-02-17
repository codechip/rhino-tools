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

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public abstract class AbstractBinsorMacro : AbstractAstMacro
	{
		private static readonly string ExtensionsKey = "__Extensions__";

		protected Expression GetNameExpression(MacroStatement macro)
		{
			ReferenceExpression referenceExpression = macro.Arguments[0] as ReferenceExpression;
			if (referenceExpression != null &&
				NameResolutionService.Resolve(referenceExpression.Name) == null)
			{
				return CodeBuilder.CreateStringLiteral(referenceExpression.Name);
			}
			else
			{
				return macro.Arguments[0];
			}
		}

		protected virtual void ProcessConstructorArguments(MethodInvocationExpression create,
		                                                   MacroStatement macro)
		{
			int argIndex = 0;

			foreach (Expression argument in macro.Arguments)
			{
				foreach (Expression ctorArg in ProcessConstructorArgument(argIndex++, argument))
				{
					if (ctorArg != null)
					{
						create.Arguments.Add(ctorArg);
					}
				}
			}
		}

		protected virtual IEnumerable<Expression> ProcessConstructorArgument(int argIndex, Expression arg)
		{
			yield return arg;
		}

		protected bool ArgumentsToCreateNamedArguments(ExpressionCollection arguments,
		                                               MethodInvocationExpression create)
		{
			foreach (Expression argument in arguments)
			{
				BinaryExpression parameter;
				if (!EnsureAssignment(argument, out parameter))
				{
					return false;
				}

				create.NamedArguments.Add(
					new ExpressionPair(parameter.Left, parameter.Right)
					);
			}

			return true;
		}

		protected bool PropertiesToExpressionPairs(Expression expression,
		                                           ExpressionPairCollection pairs)
		{
			BinaryExpression assignment;
			if (!EnsureAssignment(expression, out assignment))
			{
				return false;
			}

			ArrayLiteralExpression properties;

			if (MacroHelper.IsCompoundAssignment(assignment.Right, out properties))
			{
				foreach (Expression item in properties.Items)
				{
					if (item == properties.Items[0])
					{
						pairs.Add(new ExpressionPair(assignment.Left, item));
					}
					else
					{
						BinaryExpression property;
						if (!EnsureAssignment(item, out property))
						{
							return false;
						}

						pairs.Add(new ExpressionPair(property.Left, property.Right));
					}
				}
			}
			else
			{
				pairs.Add(new ExpressionPair(assignment.Left, assignment.Right));
			}

			return true;
		}

		protected static bool ProcessStatements(MacroStatement macro, Predicate<Statement> action)
		{
			if (macro.Block.HasStatements)
			{
				foreach (Statement statement in macro.Block.Statements)
				{
					if (!action(statement)) return false;
				}
			}

			return true;
		}

		protected void AddCompilerError(LexicalInfo lexicalInfo, string msg)
		{
			Errors.Add(CompilerErrorFactory.CustomError(lexicalInfo, msg));
		}

		protected static void RegisterExtension(MacroStatement macro, Expression extension)
		{
			List<Expression> extenstions = macro[ExtensionsKey] as List<Expression>;
			if (extenstions == null)
			{
				extenstions = new List<Expression>();
				macro[ExtensionsKey] = extenstions;
			}
			
			extenstions.Add(extension);
		}


		protected static void PromoteExtensions(MacroStatement macro, MacroStatement parent)
		{
			ApplyExtensions(macro, delegate(Expression extension)
						{
							RegisterExtension(parent, extension);
						});
		}

		protected static bool ApplyExtensions(MacroStatement macro,
		                                      Action<Expression> onExtension)
		{
			List<Expression> extenstions = macro[ExtensionsKey] as List<Expression>;
			if (extenstions != null && extenstions.Count > 0)
			{
				foreach(Expression extenstion in extenstions)
				{
					onExtension(extenstion);
				}
			}
			return true;
		}

		protected bool EnsureNoStatements(MacroStatement macro, string name)
		{
			if (macro.Block.HasStatements)
			{
				AddCompilerError(macro.LexicalInfo,
					string.Format("A {0} statement can have no statements", name));
				return false;
			}
			return true;
		}

		protected bool EnsureAssignment(Expression expression,
		                                out BinaryExpression assignment)
		{
			if (!MacroHelper.IsAssignment(expression, out assignment))
			{
				AddCompilerError(expression.LexicalInfo, GetType().Name +
					" properties must be in the format of name1=value2,name2=value2,...");
				return false;
			}
			return true;
		}
	}
}
