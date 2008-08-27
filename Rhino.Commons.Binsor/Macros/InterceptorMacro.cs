namespace Rhino.Commons.Binsor.Macros
{
	using System;
	using Boo.Lang.Compiler.Ast;

	[CLSCompliant(false)]
    public class InterceptorMacro : BaseBinsorExtensionMacro<InterceptorExtension>
    {
        public InterceptorMacro() : base("interceptor", false, new string[] { "component" })
        {
        }

        protected override bool ExpandExtension(ref MethodInvocationExpression extension, MacroStatement macro, MacroStatement parent, ref Statement expansion)
        {
            extension.Arguments.Add(macro.Arguments[0]);
            return true;
        }
    }
}
