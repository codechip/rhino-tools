using System;
using System.CodeDom;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

class ReflectionHelper
{
	public const BindingFlags DefaultBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

	public static bool HasSerializationConstructor(Type baseClass)
	{
		return baseClass.GetConstructor(DefaultBindingFlags,null,
		                                new Type[]{typeof(SerializationInfo), typeof(StreamingContext)},
		                                null)!=null;
	}

	public static bool HasGetObjectData(Type baseClass)
	{
		return
			null != baseClass.GetMethod("GetObjectData", new Type[] {typeof (SerializationInfo), typeof (StreamingContext)});
	}

	public static bool IsSerializationConstructor(ConstructorInfo constructorInfo)
	{
		return AreSerializableParameters(constructorInfo.GetParameters());
	}

	public static bool IsGetObjectData(MethodInfo method)
	{
		if (method.Name != "GetObjectData")
			return false;
		return AreSerializableParameters(method.GetParameters());
	}

	public static bool AreSerializableParameters(ParameterInfo[] parameters)
	{
		if (parameters.Length != 2)
			return false;
		if (parameters[0].ParameterType != typeof (SerializationInfo))
			return false;
		if (parameters[1].ParameterType != typeof (StreamingContext))
			return false;
		return true;
	}

	public static string GetMangledMethodName(MethodInfo method, string postFix)
	{
		StringBuilder sb = new StringBuilder(method.Name);
		foreach (ParameterInfo parameterInfo in method.GetParameters())
		{
			sb.Append("_").Append(parameterInfo.ParameterType.Name);
		}
		if (postFix != null)
			sb.Append(postFix);
		sb.Replace("`", "_").Replace(".", "_").Replace("&", "_ref_").Replace("[]", "_Array_");
		return sb.ToString();
	}

	public static string GetMangledPropertyName(PropertyInfo propertyInfo)
	{
		StringBuilder sb = new StringBuilder(propertyInfo.Name);
		foreach (ParameterInfo parameterInfo in propertyInfo.GetIndexParameters())
		{
			sb.Append("_").Append(parameterInfo.Name).Append("_").Append(parameterInfo.ParameterType.FullName);
		}

		sb.Replace("`", "_").Replace(".", "_").Replace("&", "_ref_").Replace("+", "_inner_").Replace("[]", "_Array_");
		return sb.ToString();
	}

	public static Type GetParameterType(ParameterInfo parameterInfo)
	{
		Type type = parameterInfo.ParameterType;
		//for out / ref parameters
		type = type.IsByRef ? type.GetElementType() : type;
		return type;
	}

	public static void AssertBothHaveSameVisibility(MethodInfo getter, MethodInfo setter, PropertyInfo propertyInfo)
	{
		//not supporting properties with different visibility at the moment
		if (getter != null && setter != null && getter.IsPublic != setter.IsPublic)
			throw new NotSupportedException(
				string.Format("Property {0}.{1} has mixed visabilities, which are not supported by Rhino.Proxy at the moment",
				              propertyInfo.DeclaringType.FullName,
				              propertyInfo.Name));
	}

	public static string GetMangledTypeName(Type baseClass, Type[] interfaces)
	{
		StringBuilder sb = new StringBuilder("Rhino_Proxy_");
		sb.Append(GetTypeName(baseClass));
		foreach (Type type in interfaces)
		{
			sb.Append("_").Append(GetTypeName(type));
		}
		return sb.ToString();
	}

	/// <summary>
	/// Gets the name of a type, taking into consideration nested types.
	/// </summary>
	public static String GetTypeName(Type type)
	{
		StringBuilder nameBuilder = new System.Text.StringBuilder();
		if (type.Namespace != null)
			nameBuilder.Append(type.Namespace.Replace('.', '_'));
		if (type.DeclaringType != null)
			nameBuilder.Append(type.DeclaringType.Name).Append("_");
#if DOTNET2
            if (type.IsGenericType)
            {
                Type[] args = type.GetGenericArguments();
                foreach (Type arg in args)
                {
                    string argName = GetTypeName(arg);
                    nameBuilder.Append(argName).Append("_");
                }
            }
#endif
		if (type.IsArray)
			nameBuilder.Append("ArrayOf_").Append(GetTypeName(type.GetElementType()));
		else
			nameBuilder.Append(type.Name);
		return nameBuilder.Replace('`','_').ToString();
	}

	public static MemberAttributes GetMethodVisiblity(MethodInfo method)
	{
		if (method.IsPublic)
			return MemberAttributes.Public;
		else if ((method.Attributes & MethodAttributes.FamANDAssem) != 0)
			return MemberAttributes.FamilyAndAssembly;
		else if ((method.Attributes & MethodAttributes.Family) != 0)
			return MemberAttributes.Family;
		else if ((method.Attributes & MethodAttributes.Assembly) != 0)
			return MemberAttributes.Assembly;
		else
		{
			throw new NotSupportedException(string.Format("Can't figure out {0} visiblilty!", method));
		}
	}
}