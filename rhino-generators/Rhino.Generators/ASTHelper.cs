using System.Text;
using DDW;

namespace Rhino.Generators
{
	public static class ASTHelper
	{
		public static string GetName(IdentifierExpression expression)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string part in expression.Identifier)
			{
				sb.Append(part).Append('.');
			}
			return sb.ToString(0, sb.Length - 1);//remove lasst dot
		}
	}
}