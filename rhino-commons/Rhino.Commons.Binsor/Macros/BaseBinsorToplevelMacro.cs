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
	using Boo.Lang.Compiler.Ast;
	using Rhino.Commons.Binsor.Extensions;

    [CLSCompliant(false)]
	public class BaseBinsorToplevelMacro<T> : AbstractBinsorMacro
	{
		protected MethodInvocationExpression create;

		public override Statement Expand(MacroStatement macro)
		{
			create = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(T).FullName)
				);

			if (ConfigureBinsorMacro(macro))
			{
				return new ExpressionStatement(create);				
			}
				
			return null;
		}

		private bool ConfigureBinsorMacro(MacroStatement macro)
		{
			ProcessConstructorArguments(create, macro);
			return ProcessAttributes(macro) &&
				   ProcessExtensions(macro) &&
				   ProcessStatements(macro);
		}

		private bool ProcessAttributes(MacroStatement macro)
		{
			HashConfigurationBuilder builder = new HashConfigurationBuilder();
			if (!builder.BuildAttributes(macro.Block, Errors))
			{
				return false;
			}

			if (builder.HasConfiguration)
			{
				MethodInvocationExpression extension = new MethodInvocationExpression(
					AstUtil.CreateReferenceExpression(typeof(ConfigurationExtension).FullName)
					);
				extension.Arguments.Add(builder.HashConfiguration);
				RegisterExtension(macro, extension);
			}
			return true;
		}

		protected virtual bool ProcessStatements(MacroStatement macro)
		{
			return ProcessStatements(macro,
									 delegate(Statement statement)
									 {
										 return ProcessProperties(statement);
									 });
		}

		protected virtual bool ProcessProperties(Statement statement)
		{
			ExpressionStatement expression = statement as ExpressionStatement;
			if (expression != null)
			{
				return PropertiesToExpressionPairs(expression.Expression, create.NamedArguments);				
			}
			return true;
		}

		private bool ProcessExtensions(MacroStatement macro)
		{
			return ApplyExtensions(macro, delegate(Expression extension)
			                              {
			                              	 create.Arguments.Add(extension);
			                              });
		}
	}
}
