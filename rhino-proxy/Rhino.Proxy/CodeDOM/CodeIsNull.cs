using System.CodeDom;

namespace Rhino.Proxy.CodeDOM
{
	public class CodeIsNull : CodeBinaryOperatorExpression
	{
		public CodeIsNull(CodeExpression reference)
			: base(reference, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null))
		{
		}
	}
}