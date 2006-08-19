using System;
using System.Reflection;

public class ValidationUtil
{
    public static bool IsValidPropertyToOverride(PropertyInfo propertyInfo)
    {
        MethodInfo getMethod = propertyInfo.GetGetMethod(true);
        if (getMethod!=null && !IsValidMethodToOverride(getMethod, true))
                return false;
        MethodInfo setMethod = propertyInfo.GetSetMethod(true);
        if (setMethod != null && !IsValidMethodToOverride(setMethod, true))
            return false;
        return true;
    }

    public static bool IsValidMethodToOverride(MethodInfo method, bool allowSpecialNames)
    {
        if( !method.IsVirtual ||
            method.IsFinal ||
            method.IsSpecialName != allowSpecialNames|| //get_ / set_ or add_ / remove_
            (method.Name == "Finalize" && method.DeclaringType == typeof (object)))
            return false;
            
        if( IsPublicType(method.ReturnType))
            return false;
        foreach (ParameterInfo parameterInfo in method.GetParameters())
        {
            if(IsPublicType(parameterInfo.ParameterType))
                return false;
        }
        return true;
    }

    public static bool IsPublicType(Type returnType)
    {
        if (returnType.IsByRef)
            return IsPublicType(returnType.GetElementType());
        return returnType.IsPublic == false || (returnType.IsArray && returnType.GetElementType().IsPublic == false);
    }
}