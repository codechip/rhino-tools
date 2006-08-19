using System;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Text;

internal class CodeDOMHelper
{
	public static CodeExpression CreateDefaultBindingExpression()
	{
		return
			new CodeCastExpression(typeof(BindingFlags), new CodePrimitiveExpression((int)ReflectionHelper.DefaultBindingFlags));
	}

	public static CodeVariableReferenceExpression[] GetParameterReferences(CodeMemberMethod method)
	{
		ArrayList parametersRefs = new ArrayList();
		foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
		{
			parametersRefs.Add(new CodeVariableReferenceExpression(parameter.Name));
		}
		return (CodeVariableReferenceExpression[])parametersRefs.ToArray(typeof(CodeVariableReferenceExpression));
	}

	public static CodeExpression CreateGetterPropertyCall(MethodInfo method, CodeExpression[] args, CodePropertyReferenceExpression property)
	{
		if (args.Length == 0)
			return property;
		else if (property.PropertyName != "Item")
		{
			return new CodeIndexerExpression(property, args);
		}
		else
		{
			return new CodeIndexerExpression(new CodeBaseReferenceExpression(), args);
		}
	}

	public static CodeAssignStatement CreateSetterPropertyCall(
		MethodInfo method, CodePropertyReferenceExpression property, CodeExpression[] args)
	{
		CodeAssignStatement assign;
		if (args.Length == 1)
		{
			assign = new CodeAssignStatement(property, args[0]);
		}
		else
		{
			ArrayList list = new ArrayList(args);
			list.RemoveAt(list.Count - 1);
			CodeExpression[] argsWithoutValue = (CodeExpression[])list.ToArray(typeof(CodeExpression));

			CodeIndexerExpression propWithIndex;
			if (property.PropertyName != "Item")
			{
				propWithIndex = new CodeIndexerExpression(property, argsWithoutValue);
			}
			else
			{
				propWithIndex = new CodeIndexerExpression(new CodeBaseReferenceExpression(),
														  argsWithoutValue);
			}
			assign = new CodeAssignStatement(propWithIndex, args[args.Length - 1]);
		}
		return assign;
	}

	public static CodeExpression[] UnpackParametersFromArray(
		MethodInfo method, CodeStatementCollection declareOutOrRefParameters, CodeStatementCollection assignOurOrRefParameters)
	{
		ArrayList parameters = new ArrayList();
		int indexer = 0;
		foreach (ParameterInfo parameterInfo in method.GetParameters())
		{
			CodeArrayIndexerExpression arg =
				new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("args"),
											   new CodeSnippetExpression(indexer.ToString()));
			indexer += 1;
			Type type = parameterInfo.ParameterType;
			if (type.IsByRef)
				type = type.GetElementType();
			CodeCastExpression cast = new CodeCastExpression(type, arg);
			CodeDirectionExpression direction = new CodeDirectionExpression(FieldDirection.In, cast);
			if (parameterInfo.IsOut)
				direction.Direction = FieldDirection.Out;
			else if (parameterInfo.ParameterType.IsByRef)
				direction.Direction = FieldDirection.Ref;
			if (direction.Direction != FieldDirection.In)
			{
				CodeVariableDeclarationStatement var =
					new CodeVariableDeclarationStatement(type, string.Format("{0}_FakeOutRef", parameterInfo.Name), cast);
				declareOutOrRefParameters.Add(var);
				direction.Expression = new CodeVariableReferenceExpression(var.Name);
				CodeAssignStatement backToArray = new CodeAssignStatement(arg, new CodeVariableReferenceExpression(var.Name));
				assignOurOrRefParameters.Add(backToArray);
			}
			parameters.Add(direction);
		}
		return (CodeExpression[])parameters.ToArray(typeof(CodeExpression));
	}

	public static void AddMethodParameters(
		CodeParameterDeclarationExpressionCollection parametersDeclaration, ParameterInfo[] parameters)
	{
		foreach (ParameterInfo parameterInfo in parameters)
		{
			Type type = ReflectionHelper.GetParameterType(parameterInfo);
			CodeParameterDeclarationExpression arg = new CodeParameterDeclarationExpression(type, parameterInfo.Name);
			if (parameterInfo.IsOut)
				arg.Direction = FieldDirection.Out;
			else if (parameterInfo.ParameterType.IsByRef) //ref param
				arg.Direction = FieldDirection.Ref;
			parametersDeclaration.Add(arg);
		}
	}

	public static void CreateSetStatementsForAllOutOrRefParams(
		CodeMemberMethod overrideMethod, CodeVariableDeclarationStatement paramArray)
	{
		int index = 0;
		foreach (CodeParameterDeclarationExpression parameter in overrideMethod.Parameters)
		{
			if (parameter.Direction == FieldDirection.Out || parameter.Direction == FieldDirection.Ref)
			{
				CodeArrayIndexerExpression outParamValue =
					new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(paramArray.Name),
												   new CodePrimitiveExpression(index));

				CodeAssignStatement assignStatement =
					new CodeAssignStatement(new CodeVariableReferenceExpression(parameter.Name),
											new CodeCastExpression(parameter.Type, outParamValue));
				overrideMethod.Statements.Add(assignStatement);
			}
			index += 1;
		}
	}

	public static void AddDefaultValuesToOutParams(CodeMemberMethod overrideMethod)
	{
		foreach (CodeParameterDeclarationExpression parameter in overrideMethod.Parameters)
		{
			if (parameter.Direction == FieldDirection.Out)
			{
				CodeAssignStatement assignDefault =
					new CodeAssignStatement(new CodeVariableReferenceExpression(parameter.Name),
											new CodeDefaultValueExpression(parameter.Type));
				overrideMethod.Statements.Add(assignDefault);
			}
		}
	}

	public static void AddGenericConstraints(MethodInfo method, CodeTypeParameterCollection typeParameters)
	{
		foreach (Type type in method.GetGenericArguments())
		{
			CodeTypeParameter parameter = new CodeTypeParameter(type.Name);
			foreach (Type constraint in type.GetGenericParameterConstraints())
			{
				parameter.Constraints.Add(new CodeTypeReference(constraint));
			}
			typeParameters.Add(parameter);
		}
	}

	public static void AddGenericConstraints(MethodInfo method, CodeTypeReferenceCollection arguments)
	{
		CodeTypeParameterCollection collection = new CodeTypeParameterCollection();
		AddGenericConstraints(method, collection);
		foreach (CodeTypeParameter typeParameter in collection)
		{
			arguments.Add(typeParameter.Name);
		}
	}

	public static string GetGoodTypeName(Type type)
	{
		if (type.IsGenericType == false)
			return type.FullName.Replace('+', '.');//inner class
		StringBuilder sb = new StringBuilder();
		//remove the `1, etc
		Type genericDefination = type.GetGenericTypeDefinition();
		sb.Append(genericDefination.FullName.Substring(0, genericDefination.FullName.Length - 2));
		sb.Append("<");
		bool first = true;
		foreach (Type argument in type.GetGenericArguments())
		{
			if (!first)
				sb.Append(", ");
			sb.Append(GetGoodTypeName(argument));
			first = false;
		}
		sb.Append(">");
		return sb.ToString();
	}
}