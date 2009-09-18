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
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Castle.MicroKernel;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class LifestyleMacro : BaseBinsorExtensionMacro<LifestyleExtension>
	{
		public LifestyleMacro()
			: base("lifestyle", true, "component", "extend")
		{
		}

		protected override bool ExpandExtension(ref MethodInvocationExpression extension,
												MacroStatement macro, MacroStatement parent,
												ref Statement expansion)
		{
			if (macro.Arguments.Count < 1)
			{
				AddMissingLifestyleTypeError(macro);
				return false;
			}

			ReferenceExpression lifestyle = macro.Arguments[0] as ReferenceExpression;
			if (lifestyle == null)
			{
				AddMissingLifestyleTypeError(macro);
				return false;
			}

			macro.Arguments.RemoveAt(0);
			IEntity entity = NameResolutionService.Resolve(lifestyle.Name);
			if (entity == null || entity.EntityType != EntityType.Type)
			{
				AddMissingLifestyleTypeError(macro);
				return false;
			}

			Type lifestyleType = ((ExternalType)entity).ActualType;

			return (InitializeLifestyleExtension(ref extension, lifestyle, lifestyleType) &&
					ArgumentsToCreateNamedArguments(macro.Arguments, extension));
		}

		protected override MethodInvocationExpression CreateExtension()
		{
			return null;
		}

		private bool InitializeLifestyleExtension(ref MethodInvocationExpression extension,
												  ReferenceExpression lifestyle, Type lifestyleType)
		{
			if (typeof(LifestyleExtension).IsAssignableFrom(lifestyleType))
			{
				extension = new MethodInvocationExpression(lifestyle);
			}
			else
			{
				if (!typeof(ILifestyleManager).IsAssignableFrom(lifestyleType))
				{
					AddCompilerError(lifestyle.LexicalInfo,
									 "A custom lifestyle statement must specify a type that implements " +
									 typeof(ILifestyleManager).FullName);
					return false;
				}

				extension = new MethodInvocationExpression(
					AstUtil.CreateReferenceExpression(typeof(Custom).FullName)
					);
				extension.Arguments.Add(AstUtil.CreateReferenceExpression(lifestyleType.FullName));
			}

			return true;
		}

		private void AddMissingLifestyleTypeError(MacroStatement macro)
		{
			AddCompilerError(macro.LexicalInfo, "A lifestyle statement must specify a valid lifestyle");
		}
	}
}
