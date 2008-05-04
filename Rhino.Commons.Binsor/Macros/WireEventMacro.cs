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
using Boo.Lang.Compiler.Ast;

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class WireEventMacro : BaseBinsorExtensionMacro<EventWireExtension>
	{
		public WireEventMacro() : base("wireEvent", false, "component", "extend")
		{	
		}

		protected override bool ExpandExtension(ref MethodInvocationExpression extension,
		                                        MacroStatement macro, MacroStatement parent,
		                                        ref Statement expansion)
		{
			Expression eventName = ObtainEventName(macro);
			if (eventName == null)
			{
				AddCompilerError(macro.LexicalInfo,
				                 "A wireEvent statement must be in the form wireEvent eventName");
				return false;
			}

			HashLiteralExpression listeners = ObtainListeners(macro);
			if (listeners != null && listeners.Items.Count > 0)
			{
				extension.Arguments.Add(eventName);
				extension.Arguments.Add(listeners);
				return true;
			}

			return false;
		}

		private Expression ObtainEventName(MacroStatement macro)
		{
			if (macro.Arguments.Count == 1)
			{
				return GetNameExpression(macro);
			}

			return null;
		}

		private HashLiteralExpression ObtainListeners(MacroStatement macro)
		{
			HashLiteralExpression listeners = new HashLiteralExpression();
			ComponentMethodVisitor componentMethod = new ComponentMethodVisitor();

			foreach (Statement statement in macro.Block.Statements)
			{
				ExpressionStatement expression = statement as ExpressionStatement;
				if (expression == null || !(expression.Expression is MethodInvocationExpression))
				{
					AddSubscriberSyntaxError(statement);
					return null;
				}

				MethodInvocationExpression mie = (MethodInvocationExpression) expression.Expression;
				ReferenceExpression to = mie.Target as ReferenceExpression;
				if (to == null || mie.Arguments.Count != 1 ||
					!"to".Equals(to.Name, StringComparison.InvariantCultureIgnoreCase))
				{
					AddSubscriberSyntaxError(statement);
					return null;
				}

				MemberReferenceExpression listener = mie.Arguments[0] as MemberReferenceExpression;
				if (listener == null || !componentMethod.ExtractMethod(listener))
				{
					AddSubscriberSyntaxError(statement);
					return null;					
				}

				listeners.Items.Add(new ExpressionPair(componentMethod.Component, componentMethod.Method));
			}

			return listeners;
		}

		private void AddSubscriberSyntaxError(Statement statement)
		{
			AddCompilerError(statement.LexicalInfo,
							 "A subscriber statement must be in the form to @listener.Method");			
		}
	}
}
