using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace Rhino.Proxy.CodeDOM
{
	public class CodeAsExpression : CodeSnippetExpression
	{
		public CodeAsExpression(Type type, CodeVariableReferenceExpression varaible)
		{
			Value = string.Format("({0} as {1})", varaible.VariableName, CodeDOMHelper.GetGoodTypeName(type));
		}

	
	}
}
