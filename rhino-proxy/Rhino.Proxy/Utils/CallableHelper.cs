using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rhino.Proxy.Utils
{
	public class CallableHelper
	{
		public void Create(MethodInfo method)
		{
			CodeTypeDeclaration callable = new CodeTypeDeclaration(ReflectionHelper.GetMangledMethodName(method, "_Callabe"));
			string delegateTypeName = ReflectionHelper.GetMangledMethodName(method, "_Delegate");
			CreateDelegate(callable, delegateTypeName, method);
			CreateFieldAndCtor(callable, delegateTypeName);
			CodeMemberMethod call = new	 CodeMemberMethod();
			call.Name = "Call";
			call.Attributes = MemberAttributes.Public;
			MethodCallHelper.CreateMethodCall(null, method, call.Statements);
			
		}

		private static void CreateFieldAndCtor(CodeTypeDeclaration callable, string delegateTypeName)
		{
			CodeMemberField delegateField = new CodeMemberField(new CodeTypeReference(delegateTypeName),"__delegate" );
			callable.Members.Add(delegateField);
			CodeConstructor ctor = new CodeConstructor();
			string argName = "aDelegte";
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(delegateTypeName), argName));
			ctor.Statements.Add(
				new CodeAssignStatement(new CodeVariableReferenceExpression(argName),
				                        new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), delegateField.Name)));
		}

		private static void CreateDelegate(CodeTypeDeclaration callable, string delegateTypeName, MethodInfo method)
		{
			CodeTypeDelegate methodDelegate = new CodeTypeDelegate(delegateTypeName);
			methodDelegate.ReturnType = new CodeTypeReference(method.ReturnType);
			methodDelegate.Attributes = MemberAttributes.Public;
			foreach (ParameterInfo parameter in method.GetParameters())
			{
				CodeParameterDeclarationExpression arg = new CodeParameterDeclarationExpression();
				arg.Name = parameter.Name;
				arg.Type = new CodeTypeReference(ReflectionHelper.GetParameterType(parameter));
				if(parameter.IsOut)
					arg.Direction = FieldDirection.Out;
				else if(parameter.ParameterType.IsByRef)
					arg.Direction =FieldDirection.Ref;
				methodDelegate.Parameters.Add(arg);
			}
			callable.Members.Add(methodDelegate);
		}
	}
}
