using System;
using System.CodeDom;
using System.Reflection;

class MethodCallHelper
{
	public static void CreateMethodCall(CodeExpression baseTarget, MethodInfo methodToCall, CodeStatementCollection statements)
	{
		CodeStatementCollection declareOutOrRefParameters = new CodeStatementCollection();
		CodeStatementCollection assignOurOrRefParameters = new CodeStatementCollection();
		CodeExpression[] args =
			CodeDOMHelper.UnpackParametersFromArray(methodToCall, declareOutOrRefParameters, assignOurOrRefParameters);
		statements.AddRange(declareOutOrRefParameters);

		if (methodToCall.ReturnType != typeof (void))
		{
			CodeExpression expression = GetOriginalCallExpression(baseTarget, methodToCall, args);
			CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement(methodToCall.ReturnType,"result");
			statements.Add(result);
			CodeVariableReferenceExpression resultRef = new CodeVariableReferenceExpression("result");
			CodeAssignStatement assign = new  CodeAssignStatement(resultRef,expression);
			statements.Add(assign);
			statements.AddRange(assignOurOrRefParameters);
			CodeMethodReturnStatement ret = new CodeMethodReturnStatement(resultRef);
			statements.Add(ret);
		}
		else
		{
			CodeStatement statement = GetOriginalCallStatement(baseTarget, methodToCall, args);
			statements.Add(statement);
			statements.AddRange(assignOurOrRefParameters);
			CodeMethodReturnStatement ret = new CodeMethodReturnStatement(new CodePrimitiveExpression(null));
			statements.Add(ret);
		}
	}

	private static CodeStatement GetOriginalCallStatement(CodeExpression target, MethodInfo method, CodeExpression[] args)
	{
		if (method.IsSpecialName == false)
		{
			CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression(target, method.Name, args);
			CodeDOMHelper.AddGenericConstraints(method, mie.Method.TypeArguments);
		
			return
				new CodeExpressionStatement(mie);
		}
		else
		{
			CodePropertyReferenceExpression property =
				new CodePropertyReferenceExpression(target, method.Name.Substring(4));

			if (method.Name.StartsWith("set_"))
			{
				return CodeDOMHelper.CreateSetterPropertyCall(method, property, args);
			}
			else
			{
				throw new NotSupportedException("oops");
			}
		}
	}

	public static CodeExpression GetOriginalCallExpression(CodeExpression target, MethodInfo method, CodeExpression[] args)
	{
		if (method.IsSpecialName == false)
		{
			CodeMethodInvokeExpression mie = new CodeMethodInvokeExpression(target, method.Name, args);
			CodeDOMHelper.AddGenericConstraints(method, mie.Method.TypeArguments);
			return mie;
		}
		else
		{
			CodePropertyReferenceExpression property = new CodePropertyReferenceExpression(target, method.Name.Substring(4));
			return CodeDOMHelper.CreateGetterPropertyCall(method, args, property);
		}
	}
}