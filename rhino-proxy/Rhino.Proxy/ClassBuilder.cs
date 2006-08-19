using System;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using Rhino.Proxy.CodeDOM;
using Rhino.Proxy.Serialization;
using Rhino.Proxy.Utils;

namespace Rhino.Proxy
{
	public class ClassBuilder
	{
		private readonly GeneratorContext context;
		private readonly Type baseClass;
		private Type[] interfaces;
		private CodeCompileUnit unit;

		/// <summary>
		/// all the generated methods, keyed by mangled named
		/// </summary>
		private Hashtable generatedMethods = new Hashtable();

		private Set requiredAssemblies = new Set();
		private MixinHelper mixinHelper;
		private string classFullName;

		private const string targetFieldName = "__rhino_proxy_target";

		private const string intercepterFieldName = "__rhino_proxy_interceptor";

		private const string intercepterPropertyName = "__rhino_proxy_interceptor_property";

		private const string mixinFieldName = "__rhino_proxy_mixin";
		private const string nameSpace = "Rhino.Proxy";

		public ClassBuilder(GeneratorContext context, Type baseClass, Type[] interfaces)
		{
			this.context = context;
			this.baseClass = baseClass;
			mixinHelper = new MixinHelper(context);
			this.interfaces = mixinHelper.JoinInterfacesAndMixins(interfaces);
			classFullName = ReflectionHelper.GetMangledTypeName(baseClass, interfaces);
		}

		public string[] ReferencedAssembliesLocations
		{
			get { return (string[]) requiredAssemblies.ToArray(typeof (string)); }
		}

		public string FullName
		{
			get { return "Rhino.Proxy." + classFullName; }
		}

		public CodeCompileUnit CreateCodeCompileUnit(ConstructorParameters ctorParams)
		{
			unit = new CodeCompileUnit();
			CodeTypeDeclaration type = CreateProxiedClass();
			type.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof (SerializableAttribute))));
			CreateInterceptorProperty(type);
			CreateConstructors(ctorParams, type);
			AddTypeImplementation(type,baseClass, false);
			foreach (Type interfaceType in interfaces)
			{
				AddTypeImplementation(type, interfaceType, true);
			}
			bool shouldAddSerialization = !baseClass.IsClass || baseClass.IsSerializable;
			if (shouldAddSerialization)
				AddSerialization(type);
			return unit;
		}

		private void AddTypeImplementation(CodeTypeDeclaration type, Type clazz, bool addInterfaces)
		{
			if (context.ShouldSkip(clazz))
				return;
			CreateMethods(type, clazz);
			CreateProperties(type, clazz);
			if(!addInterfaces)
				return;
			foreach (Type interfaceType in clazz.GetInterfaces())
			{
				AddTypeImplementation(type, interfaceType,true);	
			}
		}

		private void AddSerialization(CodeTypeDeclaration type)
		{
			AddSerializationConstructor(type);
			type.BaseTypes.Add(new CodeTypeReference(typeof (ISerializable)));
			CodeMemberMethod getObjectData = new CodeMemberMethod();
			getObjectData.Name = "GetObjectData";
			type.Members.Add(getObjectData);
			getObjectData.Attributes = MemberAttributes.Public;
			bool hasGetObjectData = ReflectionHelper.HasGetObjectData(baseClass);
			if (hasGetObjectData)
				getObjectData.Attributes |= MemberAttributes.Override;
			getObjectData.Parameters.Add(new CodeParameterDeclarationExpression(typeof (SerializationInfo), "info"));
			getObjectData.Parameters.Add(new CodeParameterDeclarationExpression(typeof (StreamingContext), "context"));

			//Type type = tyepof(ProxyObjectReference)
			CodeVariableDeclarationStatement typeVar =
				new CodeVariableDeclarationStatement(typeof (Type),
				                                     "type",
				                                     new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof (Type)),
				                                                                    "GetType",
				                                                                    new CodePrimitiveExpression(
				                                                                    	typeof (ProxyObjectReference).
				                                                                    		AssemblyQualifiedName)));
			getObjectData.Statements.Add(typeVar);
			//info.SetType(type);
			CodeMethodInvokeExpression setType =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "SetType",
				                               new CodeVariableReferenceExpression("type"));
			getObjectData.Statements.Add(setType);
			CodeMethodInvokeExpression addIntercetor =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "AddValue",
				                               new CodePrimitiveExpression("__interceptor"),
				                               new CodeVariableReferenceExpression(intercepterFieldName));
			getObjectData.Statements.Add(addIntercetor);
			CodeMethodInvokeExpression addMixin =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "AddValue",
				                               new CodePrimitiveExpression("__mixins"),
				                               new CodeVariableReferenceExpression(mixinFieldName));
			getObjectData.Statements.Add(addMixin);

			CodeArrayCreateExpression interfacesArray = new CodeArrayCreateExpression(typeof (string));
			foreach (Type interfaceType in interfaces)
			{
				interfacesArray.Initializers.Add(new CodePrimitiveExpression(interfaceType.AssemblyQualifiedName));
			}
			CodeMethodInvokeExpression addInterfaces =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "AddValue",
				                               new CodePrimitiveExpression("__interfaces"),
				                               interfacesArray);
			getObjectData.Statements.Add(addInterfaces);
			CodeMethodInvokeExpression addBaseType =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "AddValue",
				                               new CodePrimitiveExpression("__baseType"),
				                               new CodeTypeOfExpression(baseClass));
			getObjectData.Statements.Add(addBaseType);
			CodeMethodInvokeExpression addDelegateToBase =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "AddValue",
				                               new CodePrimitiveExpression("__delegateToBase"),
				                               new CodePrimitiveExpression(hasGetObjectData));
			getObjectData.Statements.Add(addDelegateToBase);
			if (hasGetObjectData == false)
			{
				CodeMethodInvokeExpression getSerializableMembers =
					new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof (FormatterServices)),
					                               "GetSerializableMembers",
					                               new CodeTypeOfExpression(baseClass));
				CodeMethodInvokeExpression objectData =
					new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof (FormatterServices)),
					                               "GetObjectData",
					                               new CodeThisReferenceExpression(),
					                               getSerializableMembers);
				CodeMethodInvokeExpression addObjectData =
					new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
					                               "AddValue",
					                               new CodePrimitiveExpression("__data"),
					                               objectData);
				getObjectData.Statements.Add(addObjectData);
			}
			else
			{
				CodeMethodInvokeExpression callToBase =
					new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(),
					                               "GetObjectData",
					                               CodeDOMHelper.GetParameterReferences(getObjectData));
				getObjectData.Statements.Add(callToBase);
			}
		}

		private void AddSerializationConstructor(CodeTypeDeclaration type)
		{
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			if (ReflectionHelper.HasSerializationConstructor(baseClass))
			{
				ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("info"));
				ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("context"));
			}
			type.Members.Add(ctor);
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (SerializationInfo), "info"));
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (StreamingContext), "context"));
			CodeMethodInvokeExpression getInterceptor =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "GetValue",
				                               new CodePrimitiveExpression("__interceptor"),
				                               new CodeTypeOfExpression(typeof (IInterceptor)));
			CodeAssignStatement assignInterceptor =
				new CodeAssignStatement(new CodeVariableReferenceExpression(intercepterFieldName),
				                        new CodeCastExpression(typeof (IInterceptor), getInterceptor));
			ctor.Statements.Add(assignInterceptor);
			CodeMethodInvokeExpression getMixin =
				new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("info"),
				                               "GetValue",
				                               new CodePrimitiveExpression("__mixins"),
				                               new CodeTypeOfExpression(typeof (object[])));
			CodeAssignStatement assignMixin =
				new CodeAssignStatement(new CodeVariableReferenceExpression(mixinFieldName),
				                        new CodeCastExpression(typeof (object[]), getMixin));
			ctor.Statements.Add(assignMixin);
		}


		private CodeTypeDeclaration CreateProxiedClass()
		{
			CodeNamespace ns = new CodeNamespace(nameSpace);
			unit.Namespaces.Add(ns);
			CodeTypeDeclaration type = new CodeTypeDeclaration(classFullName);
			if (baseClass.IsPublic == false)
			{
				type.TypeAttributes = TypeAttributes.NestedAssembly;
			}
			ns.Types.Add(type);
			type.BaseTypes.Add(baseClass);
			AddRequiredAssemblies(baseClass);
			foreach (Type interfaceType in interfaces)
			{
				AddRequiredAssemblies(interfaceType);
				type.BaseTypes.Add(interfaceType);
			}
			return type;
		}

		private void AddRequiredAssemblies(Type type)
		{
			AddAssembly(type.Assembly.Location);
			foreach (AssemblyName assembly in type.Assembly.GetReferencedAssemblies())
			{
				Assembly asm = Assembly.Load(assembly);
				AddAssembly(asm.Location);
			}
			if (type.BaseType != null)
			{
				AddRequiredAssemblies(type.BaseType);
			}
			foreach (Type interfaceType in type.GetInterfaces())
			{
				AddRequiredAssemblies(interfaceType);
			}
		}

		private void AddAssembly(string assemblyLocation)
		{
			requiredAssemblies.Add(assemblyLocation);
		}

		private void CreateConstructors(ConstructorParameters ctorParams, CodeTypeDeclaration type)
		{
			CodeMemberField intercepterField = new CodeMemberField(typeof (IInterceptor), intercepterFieldName);
			type.Members.Add(intercepterField);
			CodeMemberField mixinField = new CodeMemberField(typeof (object[]), mixinFieldName);
			type.Members.Add(mixinField);
			CodeMemberField targetField = new CodeMemberField(typeof (object), targetFieldName);
			type.Members.Add(targetField);
			int count = 0;
			foreach (ConstructorInfo constructorInfo in baseClass.GetConstructors(ReflectionHelper.DefaultBindingFlags))
			{
				if (ReflectionHelper.IsSerializationConstructor(constructorInfo))
					continue;
				CreateConstructor(ctorParams, intercepterField, targetField, constructorInfo.GetParameters(), type);
				count += 1;
			}
			if (count == 0)
			{
				CreateConstructor(ctorParams, intercepterField, targetField, new ParameterInfo[0], type);
			}
		}

		private void CreateConstructor(
			ConstructorParameters ctorParams,
			CodeMemberField intercepterField,
			CodeMemberField targetField,
			ParameterInfo[] parameters,
			CodeTypeDeclaration type)
		{
			CodeConstructor ctor = new CodeConstructor();
			ctor.Attributes = MemberAttributes.Public;
			type.Members.Add(ctor);
			CodeParameterDeclarationExpression interceptorArg =
				new CodeParameterDeclarationExpression(typeof (IInterceptor), "__interceptor");
			ctor.Parameters.Add(interceptorArg);
			CodeAssignStatement interceptorRefAssign =
				new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), intercepterField.Name),
				                        new CodeArgumentReferenceExpression(interceptorArg.Name));
			ctor.Statements.Add(interceptorRefAssign);
			if (ctorParams == ConstructorParameters.InterceptorAndTarget)
			{
				CodeParameterDeclarationExpression targetArg = new CodeParameterDeclarationExpression(typeof (object), "__target");
				ctor.Parameters.Add(targetArg);
				CodeAssignStatement targetRefAssign =
					new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), targetField.Name),
					                        new CodeArgumentReferenceExpression(targetArg.Name));
				ctor.Statements.Add(targetRefAssign);
			}
			if (context.HasMixins)
			{
				CodeParameterDeclarationExpression mixingArg = new CodeParameterDeclarationExpression(typeof (object[]), "__mixin");
				ctor.Parameters.Add(mixingArg);
				CodeAssignStatement mixingAssign =
					new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mixinFieldName),
					                        new CodeArgumentReferenceExpression(mixingArg.Name));
				ctor.Statements.Add(mixingAssign);
			}
			else
			{
				CodeAssignStatement mixingAssign =
					new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mixinFieldName),
					                        new CodeArrayCreateExpression(typeof (object), new CodePrimitiveExpression(0)));
				ctor.Statements.Add(mixingAssign);
			}

			CodeDOMHelper.AddMethodParameters(ctor.Parameters, parameters);
			foreach (ParameterInfo parameterInfo in parameters)
			{
				ctor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression(parameterInfo.Name));
			}
		}

		private void CreateProperties(CodeTypeDeclaration codeDomType, Type type)
		{
			foreach (PropertyInfo propertyInfo in type.GetProperties(ReflectionHelper.DefaultBindingFlags))
			{
				string mangledPropertyName = ReflectionHelper.GetMangledPropertyName(propertyInfo);
				if (generatedMethods.ContainsKey(mangledPropertyName))
					continue;
				if (!ValidationUtil.IsValidPropertyToOverride(propertyInfo))
					continue;
				MethodInfo getter = propertyInfo.GetGetMethod(true);

				MethodInfo setter = propertyInfo.GetSetMethod(true);
				ReflectionHelper.AssertBothHaveSameVisibility(getter, setter, propertyInfo);
				CodeMemberProperty prop = new CodeMemberProperty();
				if (propertyInfo.DeclaringType.IsInterface)
				{
					prop.PrivateImplementationType = new CodeTypeReference(CodeDOMHelper.GetGoodTypeName(propertyInfo.DeclaringType));
				}

				prop.Name = propertyInfo.Name;
				prop.Type = GetTypeReference(propertyInfo.PropertyType);
				prop.Attributes = ReflectionHelper.GetMethodVisiblity(getter ?? setter);
				if (propertyInfo.DeclaringType.IsInterface == false)
					prop.Attributes |= MemberAttributes.Override;
				CodeDOMHelper.AddMethodParameters(prop.Parameters, propertyInfo.GetIndexParameters());
				if (getter != null)
				{
					CodeMemberMethod getterGenerated = CreateProxiedMethod(getter, codeDomType);
					prop.GetStatements.AddRange(getterGenerated.Statements);
				}
				if (setter != null)
				{
					CodeMemberMethod proxiedMethod = CreateProxiedMethod(setter, codeDomType);
					prop.SetStatements.AddRange(proxiedMethod.Statements);
				}
				generatedMethods.Add(mangledPropertyName, prop);
				codeDomType.Members.Add(prop);
			}
		}

		private CodeTypeReference GetTypeReference(Type type)
		{
			AddRequiredAssemblies(type);
			return new CodeTypeReference(type);
		}

		private void CreateMethods(CodeTypeDeclaration generatedType, Type type)
		{
			foreach (MethodInfo method in type.GetMethods(ReflectionHelper.DefaultBindingFlags))
			{
				if (context.ShouldSkip(method))
					continue;
				if (ReflectionHelper.IsGetObjectData(method))
					continue;
				string mangledMethodName = ReflectionHelper.GetMangledMethodName(method, null);
				if (generatedMethods.ContainsKey(mangledMethodName))
					continue;
				if (!ValidationUtil.IsValidMethodToOverride(method, false))
					continue;
				CodeMemberMethod overrideMethod = CreateProxiedMethod(method, generatedType);
				generatedMethods.Add(mangledMethodName, overrideMethod);
				generatedType.Members.Add(overrideMethod);
			}
		}

		private CodeMemberMethod CreateProxiedMethod(MethodInfo method, CodeTypeDeclaration type)
		{
			CodeMemberMethod overrideMethod = CreateProxiedMethodSignature(method);
			if(method.DeclaringType.IsInterface)
				overrideMethod.PrivateImplementationType = new CodeTypeReference(CodeDOMHelper.GetGoodTypeName(method.DeclaringType));
			CodeDOMHelper.AddDefaultValuesToOutParams(overrideMethod);
			CodeVariableDeclarationStatement invocationLocal = CreateInvocation(method, overrideMethod, type);

			CodeVariableDeclarationStatement paramArray = new CodeVariableDeclarationStatement(typeof (object[]), "__paramArray");
			overrideMethod.Statements.Add(paramArray);
			CodeArrayCreateExpression parameters =
				new CodeArrayCreateExpression(typeof (object), CodeDOMHelper.GetParameterReferences(overrideMethod));
			CodeAssignStatement assignParamArray =
				new CodeAssignStatement(new CodeVariableReferenceExpression(paramArray.Name), parameters);
			overrideMethod.Statements.Add(assignParamArray);

			CodeMethodInvokeExpression mie = CreateMethodInvocationExpresson(invocationLocal, paramArray);
			CodeVariableDeclarationStatement result = CreateCallToInterceptor(method, mie, overrideMethod);

			CodeDOMHelper.CreateSetStatementsForAllOutOrRefParams(overrideMethod, paramArray);

			if (method.ReturnType != typeof (void))
			{
				CodeMethodReturnStatement ret = new CodeMethodReturnStatement(new CodeVariableReferenceExpression(result.Name));
				overrideMethod.Statements.Add(ret);
			}
			return overrideMethod;
		}

		private void CreateInterceptorProperty(CodeTypeDeclaration type)
		{
			CodeMemberProperty prop = new CodeMemberProperty();
			type.Members.Add(prop);
			prop.Type = GetTypeReference(typeof (IInterceptor));
			prop.Name = intercepterPropertyName;
			CodeBinaryOperatorExpression intercepterIsNull =
				new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression(intercepterFieldName),
				                                 CodeBinaryOperatorType.ValueEquality,
				                                 new CodePrimitiveExpression(null));
			CodeConditionStatement branch = new CodeConditionStatement(intercepterIsNull);

			CodePropertyReferenceExpression ctorOnlyInterceptor =
				new CodePropertyReferenceExpression(
					new CodeTypeReferenceExpression(GetTypeReference(typeof (ConstructorOnlyInterceptor))), "Current");
			CodeMethodReturnStatement trueReturn = new CodeMethodReturnStatement(ctorOnlyInterceptor);
			branch.TrueStatements.Add(trueReturn);
			CodeFieldReferenceExpression intercepterField =
				new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), intercepterFieldName);
			CodeMethodReturnStatement falseReturn = new CodeMethodReturnStatement(intercepterField);
			branch.FalseStatements.Add(falseReturn);
			prop.GetStatements.Add(branch);
		}

		private CodeMethodInvokeExpression CreateMethodInvocationExpresson(
			CodeVariableDeclarationStatement invocationLocal, CodeVariableDeclarationStatement paramArray)
		{
			CodePropertyReferenceExpression intercetprorProperty =
				new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), intercepterPropertyName);
			return
				new CodeMethodInvokeExpression(intercetprorProperty,
				                               "Intercept",
				                               new CodeVariableReferenceExpression(invocationLocal.Name),
				                               new CodeVariableReferenceExpression(paramArray.Name));
		}

		private CodeVariableDeclarationStatement CreateCallToInterceptor(
			MethodInfo method, CodeMethodInvokeExpression mie, CodeMemberMethod overrideMethod)
		{
			if (method.ReturnType != typeof (void))
			{
				CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement(method.ReturnType, "__result");
				overrideMethod.Statements.Add(result);
				CodeCastExpression cast = new CodeCastExpression(method.ReturnType, mie);
				CodeAssignStatement assignResult = new CodeAssignStatement(new CodeVariableReferenceExpression(result.Name), cast);
				overrideMethod.Statements.Add(assignResult);
				return result;
			}
			else
			{
				overrideMethod.Statements.Add(mie);
				return null;
			}
		}

		private CodeVariableDeclarationStatement CreateInvocation(
			MethodInfo method, CodeMemberMethod overrideMethod, CodeTypeDeclaration type)
		{
			string methodInfoFieldName = CreateMethodInfoField(method, type);
			CodeMemberMethod callOriginalMethod = CreateCallOriginalMethod(method, type);

			CodeFieldReferenceExpression methodInfoParam =
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type.Name), methodInfoFieldName);
			CodeMethodReferenceExpression methodDelegate =
				new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), callOriginalMethod.Name);
			CodeDOMHelper.AddGenericConstraints(method, methodDelegate.TypeArguments);
			CodeObjectCreateExpression delegateCtor =
				new CodeObjectCreateExpression(typeof (MethodInvocationDelegate), methodDelegate);
			
			CodeArrayCreateExpression typeParams = new CodeArrayCreateExpression(
				typeof(Type));
			foreach (CodeTypeReference argument in methodDelegate.TypeArguments)
			{
				typeParams.Initializers.Add(new CodeTypeOfExpression(argument));
			}
			
			CodeObjectCreateExpression ctor =
				new CodeObjectCreateExpression(typeof (Invocation),
				                               delegateCtor,
				                               methodInfoParam,
				                               typeParams,
				                               new CodeThisReferenceExpression(),
				                               GetMethodTargetExpression(method));
			CodeVariableDeclarationStatement local =
				new CodeVariableDeclarationStatement(typeof (IInvocation), "__method_invocation");
			CodeAssignStatement assign = new CodeAssignStatement(new CodeVariableReferenceExpression(local.Name), ctor);
			overrideMethod.Statements.Add(local);
			overrideMethod.Statements.Add(assign);
			return local;
		}

		private CodeExpression GetMethodTargetExpression(MethodInfo method)
		{
			if (mixinHelper.IsMixinMethod(method))
			{
				return
					new CodeArrayIndexerExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mixinFieldName),
						new CodePrimitiveExpression(mixinHelper.GetMixinIndex(method)));
			}
			else
			{
				return new CodeThisReferenceExpression();
			}
		}

		private CodeMemberMethod CreateProxiedMethodSignature(MethodInfo method)
		{
			CodeMemberMethod genMethod = new CodeMemberMethod();
			genMethod.Name = method.Name;
			genMethod.ReturnType = GetTypeReference(method.ReturnType);

			genMethod.Attributes = ReflectionHelper.GetMethodVisiblity(method);

			if (method.DeclaringType.IsInterface == false)
				genMethod.Attributes |= MemberAttributes.Override;

			CodeDOMHelper.AddMethodParameters(genMethod.Parameters, method.GetParameters());
			CodeDOMHelper.AddGenericConstraints(method, genMethod.TypeParameters);
			return genMethod;
		}

		private CodeMemberMethod CreateCallOriginalMethod(MethodInfo method, CodeTypeDeclaration type)
		{
			CodeMemberMethod callOriginalMethod = new CodeMemberMethod();
			type.Members.Add(callOriginalMethod);
			callOriginalMethod.Name = ReflectionHelper.GetMangledMethodName(method, "_CallOriginal");
			CodeDOMHelper.AddGenericConstraints(method, callOriginalMethod.TypeParameters);
			callOriginalMethod.Attributes = MemberAttributes.Private;
			callOriginalMethod.ReturnType = GetTypeReference(typeof (object));
			callOriginalMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof (object[]), "args"));
			if (method.IsAbstract == false)
				CreateCallToBase(callOriginalMethod, method);
			else if (mixinHelper.IsMixinMethod(method))
				CreateCallToMixin(method, callOriginalMethod);
			else
				CreateCallToInterfaceMethod(method, callOriginalMethod);
			return callOriginalMethod;
		}

		private void CreateCallToMixin(MethodInfo method, CodeMemberMethod callOriginalMethod)
		{
			int index = mixinHelper.GetMixinIndex(method);
			CodeVariableDeclarationStatement mixin = new CodeVariableDeclarationStatement(method.DeclaringType, "mixin");
			mixin.InitExpression =
				new CodeCastExpression(method.DeclaringType,
				                       new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(mixinFieldName),
				                                                      new CodePrimitiveExpression(index)));
			callOriginalMethod.Statements.Add(mixin);

			MethodCallHelper.CreateMethodCall(new CodeVariableReferenceExpression(mixin.Name),
			                                  method,
			                                  callOriginalMethod.Statements);
		}

		private void CreateCallToBase(CodeMemberMethod callOriginalMethod, MethodInfo method)
		{
			MethodCallHelper.CreateMethodCall(new CodeBaseReferenceExpression(), method, callOriginalMethod.Statements);
		}

		/// <summary>
		/// Generate the following:
		/// 
		/// MethodDeclaringType target = this.__target as MethodDeclaringType;
		///	if(target == null)
		///	{
		///		throw not supported;
		///	}
		/// else
		/// {
		///		return target.Method(args);
		/// }
		/// </summary>
		private void CreateCallToInterfaceMethod(MethodInfo method, CodeMemberMethod callOriginalMethod)
		{
			CodeVariableDeclarationStatement target =
				new CodeVariableDeclarationStatement(method.DeclaringType,
				                                     "methodTarget",
				                                     new CodeAsExpression(method.DeclaringType,
				                                                          new CodeVariableReferenceExpression(targetFieldName)));
			callOriginalMethod.Statements.Add(target);
			CodeConditionStatement ifTargetNull =
				new CodeConditionStatement(new CodeIsNull(new CodeVariableReferenceExpression(target.Name)));
			callOriginalMethod.Statements.Add(ifTargetNull);

			MethodCallHelper.CreateMethodCall(new CodeVariableReferenceExpression(target.Name),
			                                  method,
			                                  ifTargetNull.FalseStatements);

			CodeObjectCreateExpression ctor = new CodeObjectCreateExpression(typeof (NotSupportedException));
			ctor.Parameters.Add(
				new CodePrimitiveExpression(
					string.Format(
						"You tried to call Proceed() on an proxy whose type does not support the method: {0} (from {1}).\r\nTry changing the type or intercepting the method without calling Proceed()",
						method,
						method.DeclaringType)));
			CodeThrowExceptionStatement raise = new CodeThrowExceptionStatement(ctor);
			callOriginalMethod.Statements.Add(raise);
		}

		private string CreateMethodInfoField(MethodInfo method, CodeTypeDeclaration type)
		{
			ArrayList paramTypes = new ArrayList();
			foreach (ParameterInfo parameterInfo in method.GetParameters())
			{
				//sadly, we need to do this in order to handle out/ref types
				//correctly.
				CodeMethodInvokeExpression mie =
					new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof (Type)),
					                               "GetType",
					                               new CodePrimitiveExpression(parameterInfo.ParameterType.AssemblyQualifiedName));
				paramTypes.Add(mie);
			}
			string methodInfoFieldName = ReflectionHelper.GetMangledMethodName(method, "_MethodInfo");
			CodeMemberField methodInfoField = new CodeMemberField(typeof (MethodInfo), methodInfoFieldName);
			type.Members.Add(methodInfoField);
			methodInfoField.Attributes = MemberAttributes.Static;

			CodeArrayCreateExpression types =
				new CodeArrayCreateExpression(typeof (Type), (CodeExpression[]) paramTypes.ToArray(typeof (CodeExpression)));
			methodInfoField.InitExpression =
				new CodeMethodInvokeExpression(new CodeTypeOfExpression(method.DeclaringType),
				                               "GetMethod",
				                               new CodePrimitiveExpression(method.Name),
				                               CodeDOMHelper.CreateDefaultBindingExpression(),
				                               new CodePrimitiveExpression(null),
				                               types,
				                               new CodePrimitiveExpression(null));
			return methodInfoFieldName;
		}
	}
}