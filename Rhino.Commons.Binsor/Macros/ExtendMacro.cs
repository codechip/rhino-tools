namespace Rhino.Commons.Binsor.Macros
{
	using System;
	using System.Collections.Generic;
	using Boo.Lang.Compiler.Ast;

	[CLSCompliant(false)]
	public class ExtendMacro : BaseBinsorToplevelMacro<Extend>
	{
		protected override IEnumerable<Expression> ProcessConstructorArgument(int argIndex, Expression arg)
		{
			if (argIndex == 0 && arg is BinaryExpression)
			{
				BinaryExpression expression = (BinaryExpression)arg;

				if (expression.Operator == BinaryOperatorType.LessThan)
				{
					yield return expression.Left;
					yield return expression.Right;
				}
			}
			else
			{
				yield return arg;
			}
		}
	}
}