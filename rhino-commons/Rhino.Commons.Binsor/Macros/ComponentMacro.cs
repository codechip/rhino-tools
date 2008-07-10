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


namespace Rhino.Commons.Binsor.Macros
{
	using System;
	using System.Collections.Generic;
	using Boo.Lang.Compiler.Ast;

	[CLSCompliant(false)]
	public class ComponentMacro : BaseBinsorToplevelMacro<Component>
	{
		private Expression implementation;
		private ArrayLiteralExpression services;

		protected override void ProcessConstructorArguments(MethodInvocationExpression create,
															MacroStatement macro)
		{
			base.ProcessConstructorArguments(create, macro);

			if (services != null)
			{
				create.Arguments.Add(services);
			}

			if (implementation != null)
			{
				create.Arguments.Add(implementation);
			}
		}

		protected override IEnumerable<Expression> ProcessConstructorArgument(int argIndex, Expression arg)
		{
			if (argIndex == 0)
			{
				if (arg is BinaryExpression)
				{
					BinaryExpression binary = (BinaryExpression)arg;

					switch (binary.Operator)
					{
						case BinaryOperatorType.Assign:
							yield return binary.Left;
							if (binary.Right is BinaryExpression)
							{
								BinaryExpression impl = (BinaryExpression)binary.Right;
								if (impl.Operator == BinaryOperatorType.LessThan)
								{
									implementation = impl.Left;
									AddService(impl.Right);
									yield break;
								}
							}
							else
							{
								yield return binary.Right;
								yield break;
							}
							break;

						case BinaryOperatorType.LessThan:
							if (binary.Left is BinaryExpression)
							{
								BinaryExpression impl = (BinaryExpression)binary.Left;
								if (impl.Operator == BinaryOperatorType.ReferenceEquality)
								{
									yield return impl.Left;
									implementation = impl.Right;
								}
								else
								{
									break;
								}
							}
							else
							{
								implementation = binary.Left;
							}
							AddService(binary.Right);
							yield break;

						case BinaryOperatorType.ReferenceEquality:
							yield return binary.Left;
							yield return binary.Right;
							yield break;
					}
				}
			}

			if (services != null)
			{
				AddService(arg);
				yield break;
			}

			yield return arg;
		}

		private void AddService(Expression expression)
		{
			if (services == null)
			{
				services = new ArrayLiteralExpression(expression.LexicalInfo);
			}
			services.Items.Add(expression);
		}
	}
}
