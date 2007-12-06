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

namespace Rhino.Commons.Binsor.Macros
{
	[CLSCompliant(false)]
	public class CreateUsingMacro : BaseBinsorExtensionMacro<FactorySupportExtension>
	{
		public CreateUsingMacro() : base("createUsing", true, "component", "extend")
		{	
		}

		protected override bool ExpandExtension(ref MethodInvocationExpression extension,
												MacroStatement macro, MacroStatement parent,
												ref Statement expansion)
		{
			CreateUsingVisitor visitor = new CreateUsingVisitor();
			return visitor.InitializeExtension(extension, macro, Errors);
		}
	}

	internal class CreateUsingVisitor : ComponentMethodVisitor
	{
		private MethodInvocationExpression _extension;
		private CompilerErrorCollection _compileErrors;
		private StringLiteralExpression _instanceAcessor;

		public bool InitializeExtension(MethodInvocationExpression extension,
		                                MacroStatement macro,
		                                CompilerErrorCollection compileErrors)
		{
			_extension = extension;
			_compileErrors = compileErrors;

			if (macro.Arguments.Count != 1 ||
			    (!ExtractMethod(macro.Arguments[0]) && _instanceAcessor == null))
			{
				_compileErrors.Add(CompilerErrorFactory.CustomError(macro.LexicalInfo,
					"A createUsing statement must be in the form @factory.<CreateMethod>[()]"));
				return false;
			}

			if (_instanceAcessor == null)
			{
				_extension.Arguments.Add(Component);
				_extension.Arguments.Add(Method);
			}
			else
			{
				_extension.Arguments.Add(_instanceAcessor);
			}

			return true;
		}

		public override void OnReferenceExpression(ReferenceExpression accessor)
		{
			_instanceAcessor = new StringLiteralExpression(accessor.Name);
		}
	}
}
