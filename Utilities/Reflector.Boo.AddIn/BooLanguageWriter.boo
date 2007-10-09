#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


namespace Reflector.Boo

import System
import System.IO
import System.Collections
import System.Collections.Generic as SCG
import System.Globalization
import Reflector
import Reflector.CodeModel
import Reflector.CodeModel.Memory

class BooLanguageWriter(ILanguageWriter):
"""Implememnts an ILanguageWriter for Boo"""
	formatter as IFormatter
	[Property(Configuration)]
	configuration as ILanguageWriterConfiguration
	
	[Property(Brackets)]
	brackets = true
	# what is this?
	typeDeclaration as ITypeDeclaration
	
	specialTypes = { "Void"		:	"void",
					"Object"	:	"object",
					"String"	:	"string",
					"SByte"		:	"sbyte",
					"Byte"		:	"byte",
					"Int16"		:	"short",
					"UInt16"	:	"ushort",
					"Int32"		:	"int",
					"UInt32"	:	"uint",
					"Int64"		:	"long",
					"UInt64"	:	"ulong",
					"Char"		:	"char",
					"Boolean"	:	"bool",
					"Single"	:	"float",
					"Double"	:	"double",
					"Decimal"	:	"decimal"
					}
	
	[Property(ExpressionWriter)]
	expressionUtil as ExpressionUtil
	[Property(StatementWriter)]
	statementUtil as StatementUtil
	[Property(ReferenceWriter)]
	referenceUtil as ReferenceUtil
	[Property(UtilityWriter)]
	writeUtil as WriteUtil
		
	def constructor(formatter as IFormatter, configuration as ILanguageWriterConfiguration):
		self.configuration = configuration
		self.formatter = formatter
		self.writeUtil = WriteUtil(self)
		self.expressionUtil = ExpressionUtil(self)
		self.statementUtil = StatementUtil(self)
		self.referenceUtil = ReferenceUtil(self)
		
		self.writeUtil.Init()
		self.expressionUtil.Init()
		self.statementUtil.Init()
		self.referenceUtil.Init()
		
		
	def WriteAssembly(value as IAssembly):
		formatter.Write("# Assembly ")
		formatter.WriteDeclaration(value.Name)
		formatter.Write(", Version ${value.Version}") if value.Version is not null
		formatter.WriteLine()
		formatter.WriteLine()
		
		WriteCustomAttributes(value,formatter, "assembly:") if configuration["ShowCustomAttributes"] == "true"
		formatter.WriteProperty("Location", value.Location)
		formatter.WriteProperty("Name", value.ToString())
		
	def WriteAssemblyReference(value as IAssemblyReference):
		raise ArgumentNullException('value') if value is null
		
		formatter.Write('# Assembly Reference ')
		formatter.WriteDeclaration(value.Name)
		formatter.WriteLine()
		formatter.WriteProperty('Version', value.Version.ToString())
		formatter.WriteProperty('Name', value.ToString())
	
	def WriteModule(value as IModule):
		raise ArgumentNullException('value') if value is null 
		
		formatter.Write('# Module ')
		formatter.WriteDeclaration(value.Name)
		formatter.WriteLine()
		
		if configuration["ShowCustomAttributes"] == "true":
			formatter.WriteLine()
			WriteCustomAttributes(value, formatter, 'module:')
			formatter.WriteLine()

		formatter.WriteProperty('Version', value.Version.ToString())
		formatter.WriteProperty('Location', value.Location)
		location as string = Environment.ExpandEnvironmentVariables(value.Location)
		if File.Exists(location):
			formatter.WriteProperty('Size', "${FileInfo(location).Length} Bytes")
	
	def WriteModuleReference(value as IModuleReference):
		raise ArgumentNullException('value') if value is null 
		
		self.formatter.Write('# Module Reference ')
		self.formatter.WriteDeclaration(value.Name)
		self.formatter.WriteLine()

	def WriteResource(resource as IResource):
		self.formatter.Write('# ')
		if resource.Visibility == ResourceVisibility.Public:
			self.formatter.WriteKeyword('public')
		elif resource.Visibility == ResourceVisibility.Private:
			self.formatter.WriteKeyword('private')
		
		self.formatter.Write(' ')
		self.formatter.WriteKeyword('resource')
		self.formatter.Write(' ')
		self.formatter.WriteDeclaration(resource.Name)
		self.formatter.WriteLine()
		embeddedResource as IEmbeddedResource = resource as IEmbeddedResource
		if embeddedResource != null:
			self.formatter.WriteProperty('Size', embeddedResource.Value.Length.ToString(CultureInfo.InvariantCulture) + ' bytes')
		
		fileResource as IFileResource = resource as IFileResource
		if fileResource != null:
			self.formatter.WriteProperty('Location', fileResource.Location)
		
	def WriteNamespace(namespaceDeclaration as INamespace):
		if namespaceDeclaration.Name.Length != 0:
			self.formatter.WriteKeyword('namespace')
			self.formatter.Write(' ')
			writeUtil.WriteDeclaration(namespaceDeclaration.Name, self.formatter)
			self.formatter.WriteLine()
			self.formatter.WriteLine()
		
		if configuration["ShowNamespaceBody"] == "true":
		
			types = SCG.List of ITypeDeclaration()
			for typeDeclaration as ITypeDeclaration in namespaceDeclaration.Types:
				information = TypeInformation(typeDeclaration)
				if information.IsVisible(self.configuration.Visibility):
					types.Add(typeDeclaration)
				
			types.Sort() if configuration["SortAlphabetically"] == "true"
			
			firstline = true;
			for type in types:
				unless firstline:
					formatter.WriteLine()
					firstline = false
				self.WriteTypeDeclaration(type)
				formatter.WriteLine()
				
			self.formatter.WriteLine()
	
	def WriteTypeDeclaration(value as ITypeDeclaration):
		raise ArgumentNullException('value') if value is null
		
		return if (value.Namespace.Length == 0) and (value.Name == '<Module>')
		
		if (self.configuration["ShowCustomAttributes"] == "true") and (value.Attributes.Count != 0):
			attributes = SCG.List[of ICustomAttribute]()
			for attribute in value.Attributes:
				attributes.Add(attribute)
			
			writeUtil.RemoveIndexerAttribute(value, attributes)			
			
			if attributes.Count > 0:
				for attribute in attributes:
					formatter.Write('[')
					self.WriteCustomAttribute(attribute, formatter)	
					formatter.Write(']')
					formatter.WriteLine()

		writeUtil.WriteVisibility(value.Visibility, formatter)
		
		formatter.Write(' ')
		baseClassStartPrinted = false
		information = TypeInformation(value)
		if information.IsDelegate:
			writeUtil.WriteDelegateDeclaration(value, formatter)
		else:
			if information.IsEnumeration:
				baseClassStartPrinted = writeUtil.WriteEnumDeclaration(value, formatter)
			elif (information.IsValueType) and (not value.Abstract):
				writeUtil.WriteStructDeclaration(value, formatter)
			elif value.Interface:
				writeUtil.WriteInterfaceDeclaration(value, formatter)
			else:
				baseClassStartPrinted = writeUtil.WriteClassDeclaration(value, formatter)
							
			for interfaceType as ITypeReference in value.Interfaces:
				if not baseClassStartPrinted:
					writeUtil.WriteBaseClassStart(interfaceType, formatter)
				else:
					formatter.Write(", ")
					self.WriteType(interfaceType, formatter)
				baseClassStartPrinted = true
			
			writeUtil.WriteBaseClassEnd(formatter, baseClassStartPrinted)
				
			self.WriteGenericParameterConstraintList(self.GetNestedGenericArgumentList(value), formatter)
			
		formatter.WriteProperty('Name', information.NameWithResolutionScope)
		writeUtil.WriteDeclaringAssembly(information.AssemblyName, formatter)
		if self.configuration["ShowTypeDeclarationBody"] == "true":
			writeUtil.WriteTypeMembers(information, value, formatter)
			
		
	def WriteEventDeclaration(value as IEventDeclaration):
		writeUtil.WriteEventDeclaration(value,formatter)
		
	def WriteFieldDeclaration(value as IFieldDeclaration):
		writeUtil.WriteFieldDeclaration(value,formatter)
		
	def WriteMethodDeclaration(value as IMethodDeclaration):
		writeUtil.WriteMethodDeclaration(value,formatter)
		
	def WritePropertyDeclaration(value as IPropertyDeclaration):
		writeUtil.WritePropertyDeclaration(value,formatter)
		
	def WriteStatement(value as IStatement):
		statementUtil.WriteStatement(value,formatter)
	
	def WriteExpression(value as IExpression):
		expressionUtil.WriteExpression(value, formatter)

#region Implementation Details
	def WriteCustomAttributes(value as ICustomAttributeProvider, formatter as IFormatter, target as string):
		raise ArgumentNullException("value") if value is null
		for i as int in range(value.Attributes.Count):
			formatter.Write("[")
			if target is not null:
				formatter.WriteKeyword(target)
				formatter.Write(" ")
			WriteCustomAttribute(value.Attributes[i],formatter)
			formatter.Write("]")
			formatter.WriteLine()
			
	def WriteCustomAttribute(attribute as ICustomAttribute, formatter as IFormatter):
		type = attribute.Constructor.DeclaringType as ITypeReference
		name = type.Name
		name = name[0:-9] if name.EndsWith("Attribute")
		WriteReference(name, formatter,
			GetMethodReferenceDescription(attribute.Constructor),
			attribute.Constructor)
			
		if attribute.Arguments.Count>0:
			formatter.Write("(")
			WriteExpressionList(attribute.Arguments,formatter)
			formatter.Write(")")
	
	def WriteExpressionList(expressions as IExpressionCollection, formatter as IFormatter):
		prevBrackets = brackets
		
		for i as int in range(expressions.Count):
			formatter.Write(", ") if i > 0
			brackets = false
			expressionUtil.WriteExpression(expressions[i],formatter)
		
		brackets = prevBrackets
	
	def GetMethodReferenceDescription(method as IMethodReference) as string:
		text = TextFormatter()
		typeInfo = TypeInformation(method.DeclaringType)
		if IsConstructor(method):	
			text.Write("${typeInfo.NameWithResolutionScope}.${typeInfo.Name}")
		else:
			WriteType(method.ReturnType.Type, text)
			text.Write(" ${typeInfo.NameWithResolutionScope}.${method.Name}")
		
		WriteGenericArgumentList(method.GenericArguments, text)
		
		text.Write("(")
		WriteParameterDeclarationList(method.Parameters, text, configuration)
		text.Write(")")
		return text.ToString()
	
	def IsConstructor(value as IMethodReference):
		return IsConstructor(value.Name)
		
	def IsConstructor(value as string):
		return value == ".ctor" or value == ".cctor"
	
	def WriteReference(name as string, formatter as IFormatter, toolTip as string, reference):
		raise ArgumentNullException("name") if name is null
		raise ArgumentNullException("toolTip") if toolTip is null
		raise ArgumentNullException("reference") if reference is null
		name = "constructor" if IsConstructor(name)
		formatter.WriteReference(name,toolTip,reference)
	
	def IsType(type as IType, namespaceName as string, name as string) as bool:
		typeReference = type as ITypeReference
		if typeReference != null:
			return typeReference.Namespace == namespaceName and typeReference.Name == name
		return false
	
	def WriteAs(text as IFormatter):
		text.Write(' ')
		text.WriteKeyword('as')
		text.Write(' ')
		
	def WritePass(text as IFormatter):
		text.WriteComment('pass')
		
	def WriteArrayType(arrayType as IArrayType, formatter as IFormatter):
		formatter.Write("(")
		WriteType(arrayType.ElementType, formatter)
		formatter.Write(")")		
	
	def WriteGenericArgumentList(value as ICollection, formatter as IFormatter):
		if value.Count>0:
			formatter.Write("[")
			i = 0
			for type as IType in value:
				formatter.Write(", ") if i++ > 0
				formatter.WriteKeyword("of")
				formatter.Write(" ")
				WriteType(type,formatter)
			formatter.Write("]")
	
	def WriteTypeReference(value as ITypeReference, formatter as IFormatter, 
							description as string, target as object):
		WriteTypeReference(value, formatter, description, target, null)
		
	def WriteTypeReference(value as ITypeReference, formatter as IFormatter, 
							description as string, target as object, genericArguments as (IType)):
		name = value.Name
		if specialTypes.Contains(name):
			formatter.WriteKeyword(specialTypes[name])
			return
		declaringType = value.Owner as ITypeReference			
		if declaringType is not null and not declaringType.Equals(typeDeclaration):
			if genericArguments is null:
				valArguments as ITypeCollection = value.GenericArguments
				genericArguments = array(IType, valArguments.Count)
				valArguments.CopyTo(genericArguments, 0)
			
			count = 0
			if genericArguments.Length > 0:
				count = declaringType.GenericArguments.Count
				if count == 0:
					typeDeclaration as ITypeDeclaration = declaringType.Resolve()
					if typeDeclaration != null:
						count = typeDeclaration.GenericArguments.Count
				
			declaringTypeGenericArguments as (IType) = null
			if count <= genericArguments.Length:
				declaringTypeGenericArguments = array(IType, count)
				Array.Copy(genericArguments, 0, declaringTypeGenericArguments, 0, declaringTypeGenericArguments.Length)
			
			WriteTypeReference(declaringType, formatter, string.Empty, declaringType, declaringTypeGenericArguments)
			formatter.Write('.')
			if declaringTypeGenericArguments != null:
				tmp as (IType) = array(IType, genericArguments.Length - declaringTypeGenericArguments.Length)
				Array.Copy(genericArguments, declaringTypeGenericArguments.Length, tmp, 0, tmp.Length)
				genericArguments = tmp
			
		else:
			genericName = name
			arity  = value.GenericArguments.Count
			if arity != 0:
				genericName += '`' + arity.ToString(CultureInfo.InvariantCulture)
			
			namespaceName = value.Namespace
			if (namespaceName != null) and (namespaceName.Length != 0):
				lastNamespace as string = namespaceName
				index as int = lastNamespace.LastIndexOf(Char.Parse('.'))
				if index != -1:
					lastNamespace = lastNamespace.Substring(index + 1)
				
				if lastNamespace == name:
					name = namespaceName + '.' + name

		typeInstanceReference = value
		if typeInstanceReference != null:
			formatter.WriteReference(name, description, typeInstanceReference.GenericType)
			if genericArguments == null:
				arguments as ITypeCollection = value.GenericArguments
				genericArguments = array(IType, arguments.Count)
				arguments.CopyTo(genericArguments, 0)
			
			WriteGenericArgumentList(genericArguments, formatter)
		else:
			formatter.WriteReference(name, description, target)
			if genericArguments != null:
				WriteGenericArgumentList(genericArguments, formatter)
	
	def WriteParameterDeclarationList(parameters as IParameterDeclarationCollection, formatter as IFormatter, 
										configuration as ILanguageWriterConfiguration):
		for i in range(parameters.Count):
			formatter.Write(', ') if i > 0
			WriteParameterDeclaration(parameters[i], formatter, configuration)
	
	def WriteParameterDeclaration(value as IParameterDeclaration, formatter as IFormatter, 
									configuration as ILanguageWriterConfiguration):
		attributes = ArrayList()
		attributes.AddRange(value.Attributes)
		inAttribute  = GetCustomAttribute(value, 'System.Runtime.InteropServices', 'InAttribute')
		outAttribute  = GetCustomAttribute(value, 'System.Runtime.InteropServices', 'OutAttribute')
		paramArrayAttribute = GetCustomAttribute(value, 'System', 'ParamArrayAttribute')
		renderOutAttribute  = false
		if inAttribute is null:
			if outAttribute is not null:
				referenceType = value.ParameterType as IReferenceType
				if referenceType is not null:
					attributes.Remove(outAttribute)
					renderOutAttribute = true
			if paramArrayAttribute is not null:
				attributes.Remove(paramArrayAttribute)
			
		if (configuration is not null) and (configuration["ShowCustomAttributes"] == "true") and (attributes.Count != 0):
			formatter.Write('[')
			for i as int in range(attributes.Count):
				formatter.Write(', ') if i > 0
				WriteCustomAttribute(attributes[i], formatter)
			formatter.Write(']')
			formatter.Write(' ')
		
		if paramArrayAttribute is not null:
			formatter.Write('*')
		
		if renderOutAttribute:
			formatter.WriteComment('/*')
			formatter.WriteKeyword('out')
			formatter.WriteComment('*/')
			formatter.Write(' ')
		
		referenceType = value.ParameterType as IReferenceType
		if referenceType is not null and not renderOutAttribute:
			formatter.WriteKeyword('ref')
			formatter.Write(' ')
		
		parameterName = value.Name
#		defaultValue as IExpression = value.DefaultValue
		if (parameterName != null) and parameterName.Length > 0:
			writeUtil.WriteIdentifier(parameterName, formatter)
			#formatter.Write(' ')
		
		WriteAs(formatter) if configuration is not null
		
		if referenceType is not null:
			WriteType(referenceType.ElementType, formatter)
		else:
			WriteType(value.ParameterType, formatter)
			
#		if defaultValue != null:
#			formatter.WriteComment(' /* = ')
#			expressionUtil.WriteExpression(defaultValue, formatter)
#			formatter.WriteComment(' */')
			
		
	def GetCustomAttribute(value as ICustomAttributeProvider, namespaceName as string, 
								name as string) as ICustomAttribute:
		for customAttribute as ICustomAttribute in value.Attributes:
			type as IType = customAttribute.Constructor.DeclaringType
			if IsType(type, namespaceName, name):
				return customAttribute
		return null

	def WriteType(type as IType, formatter as IFormatter):
		if type isa ITypeReference:
			desc = TypeInformation(type).NameWithResolutionScope
			WriteTypeReference(type,formatter, desc, type)
			return
			
		if type isa IArrayType:
			WriteArrayType(type,formatter)
			return
		
		pointer = type as IPointerType
		if pointer:
			formatter.WriteComment("/* pointer */ ")
			WriteType(pointer.ElementType, formatter)
			formatter.Write("*")
			return
		
		reference = type as IReferenceType
		if reference:
#			formatter.WriteComment("/* ref */ ")
			formatter.WriteKeyword('ref ')
			WriteType(reference.ElementType,formatter)
			return
		
		declaration1 = type as IVariableDeclaration
		if ((declaration1 != null) and declaration1.Pinned):
			formatter.WriteComment("/* pinned */ ")
			WriteType(declaration1.VariableType,formatter)
			return
		
		optional = type as IOptionalModifier
		if optional:
			formatter.WriteComment("/* optional {")
			WriteType(optional.Modifier, formatter)
			formatter.WriteComment("} */ ")
			WriteType(optional.ElementType, formatter)
			return
		
		requiredModifier = type as IRequiredModifier
		if requiredModifier:
			modifier = requiredModifier.Modifier as ITypeReference
			if modifier is not null and IsType(modifier, "System.Runtime.CompilerServices", "IsVolatile"):
				formatter.WriteComment("/* temorary syntax */")
				formatter.WriteKeyword("volatile")
				formatter.Write(" ")
			else:
				formatter.WriteComment("/* required {")
				WriteType(requiredModifier.Modifier, formatter)
				formatter.WriteComment("} */ ")
				WriteType(requiredModifier.ElementType, formatter)
			return
		
		funcPointer = type as IFunctionPointer
		if funcPointer:					
			formatter.WriteComment("/* function pointer. */")
			formatter.Write(" ")
			formatter.Write("{")
			for i as int in range(funcPointer.Parameters.Count):
				formatter.Write(", ") if i > 0
				WriteType(funcPointer.Parameters[i].ParameterType, formatter)
			formatter.Write("}")
			WriteAs(formatter)
			WriteType(funcPointer.ReturnType.Type,formatter)
			return
		
		genericParam = type as IGenericParameter
		if genericParam:
			formatter.WriteDeclaration(genericParam.Name)
			return
		
		genericArg = type as IGenericArgument
		if genericArg:
			arg  = genericArg.Resolve()
			genericDeclaration = arg as IGenericParameter
			if genericDeclaration:
				desc = genericDeclaration.Name + "# Generic argument"
				formatter.WriteReference(genericDeclaration.Name, desc, null)
			else:
				WriteType(arg, formatter)
			return
			
		raise NotSupportedException("Unknown kind of type ${type}.")
	
	def IsSequentialLayout(typeDeclaration as ITypeDeclaration) as bool:
		for customAttribute as ICustomAttribute in typeDeclaration.Attributes:
			if self.IsType(customAttribute.Constructor.DeclaringType, 'System.Runtime.InteropServices', 'StructLayoutAttribute'):
				if customAttribute.Arguments.Count >= 1:
					fieldReferenceExpression as IFieldReferenceExpression = customAttribute.Arguments[0] as IFieldReferenceExpression
					if fieldReferenceExpression != null:
						if (fieldReferenceExpression.Field.Name == 'Sequential') and (self.IsType(fieldReferenceExpression.Field.DeclaringType, 'System.Runtime.InteropServices', 'LayoutKind')):
							return true
						
					literalExpression as ILiteralExpression = customAttribute.Arguments[0] as ILiteralExpression
					if (literalExpression != null) and (literalExpression.Value != null):
						if literalExpression.Value.Equals(0):
							return true
		
		return false
	
	def IsComInterface(typeDeclaration as ITypeDeclaration) as bool:
		if typeDeclaration.Interface:
			for customAttribute as ICustomAttribute in typeDeclaration.Attributes:
				if self.IsType(customAttribute.Constructor.DeclaringType, 'System.Runtime.InteropServices', 'GuidAttribute'):
					return true
				
		return false
	
	def IsComImportConstructor(methodDeclaration as IMethodDeclaration, typeDeclaration as ITypeDeclaration) as bool:
		declaringType as ITypeDeclaration = (methodDeclaration.DeclaringType as ITypeReference).Resolve()
		if not declaringType.Interface:
			if (methodDeclaration.SpecialName) and (methodDeclaration.RuntimeSpecialName) and (methodDeclaration.Name == '.ctor') and (methodDeclaration.Parameters.Count == 0):
				if (not methodDeclaration.Abstract) and (methodDeclaration.Body == null):
					for customAttribute as ICustomAttribute in declaringType.Attributes:
						if self.IsType(customAttribute.Constructor.DeclaringType, 'System.Runtime.InteropServices', 'ComImportAttribute'):
							return true
		return false 

	def GetUnderlyingFieldReference(eventDeclaration as IEventDeclaration) as IFieldReference:
		information as EventInformation = EventInformation(eventDeclaration)
		addMethod as IMethodDeclaration = information.AddMethod
		removeMethod as IMethodDeclaration = information.RemoveMethod
		invokeMethod as IMethodDeclaration = information.InvokeMethod
		if (addMethod != null) and (removeMethod != null) and (invokeMethod == null):
			fieldReference1 as IFieldReference = self.GetUnderlyingFieldReference(eventDeclaration, addMethod, 'Combine')
			if fieldReference1 != null:
				fieldReference2 as IFieldReference = self.GetUnderlyingFieldReference(eventDeclaration, removeMethod, 'Remove')
				if fieldReference2 != null:
					if fieldReference1.Equals(fieldReference2):
						return fieldReference1
		return null

	def GetNestedGenericArgumentList(value as ITypeDeclaration) as (IType):
		index as int = 0
		owner as ITypeReference = value.Owner as ITypeReference
		index = owner.GenericArguments.Count if owner is not null
		
		genericArguments as ITypeCollection = value.GenericArguments
		source as (IType) = array(IType, genericArguments.Count)
		genericArguments.CopyTo(source, 0)
		count as int = source.Length - index
		target as (IType) = array(IType, count)
		Array.Copy(source, index, target, 0, count)
		return target
		
	def GetUnderlyingFieldReference(eventDeclaration as IEventDeclaration, methodDeclaration as IMethodDeclaration, delegateMethodName as string) as IFieldReference:
		if methodDeclaration.Body isa IBlockStatement:
			blockStatement = methodDeclaration.Body as IBlockStatement
			if (blockStatement.Statements.Count == 1):
				assignStatement as IAssignExpression = blockStatement.Statements[0] as IAssignExpression
				if assignStatement != null:
					fieldReferenceExpression as IFieldReferenceExpression = assignStatement.Target as IFieldReferenceExpression
					if fieldReferenceExpression != null:
						castExpression as ICastExpression = assignStatement.Expression as ICastExpression
						if (castExpression != null) and (castExpression.TargetType.Equals(eventDeclaration.EventType)):
							methodInvokeExpression as IMethodInvokeExpression = castExpression.Expression as IMethodInvokeExpression
							if (methodInvokeExpression != null) and (methodInvokeExpression.Arguments.Count == 2) and (methodInvokeExpression.Arguments[0].Equals(fieldReferenceExpression)):
								methodReferenceExpression as IMethodReferenceExpression = methodInvokeExpression.Method as IMethodReferenceExpression
								if methodReferenceExpression != null:
									if self.IsType(methodReferenceExpression.Method.DeclaringType, 'System', 'Delegate'):
										if (methodReferenceExpression.Method.Name == delegateMethodName) and (methodReferenceExpression.Method.Parameters.Count == 2):
											return fieldReferenceExpression.Field
											
		return null
	
	def WriteGenericParameterConstraintList(value as ICollection, formatter as IFormatter):
		if value.Count > 0:
			for type as IType in value:
				hasConstraints as bool = false
				parameter as IGenericParameter = type as IGenericParameter
				if parameter != null:
					valueTypeConstraint as bool = false
					constraints as ArrayList = ArrayList()
					for constraint as IType in parameter.Constraints:
						if constraint isa IValueTypeConstraint:
							valueTypeConstraint = true
						
						if (valueTypeConstraint) and ((constraint isa IDefaultConstructorConstraint) or (self.IsType(constraint, 'System', 'ValueType'))):
							continue  
						
						if not IsType(constraint, 'System', 'Object'):
							constraints.Add(constraint)
						
					
					if constraints.Count > 0:
						formatter.Write(' ')
						writeUtil.WriteDeclaration(parameter.Name, formatter)
						WriteAs(formatter)
						formatter.Write('{')
						for j as int in range(constraints.Count):
							formatter.Write(', ') if j != 0
							
							constraint as IType = constraints[j]
							self.WriteGenericParameterConstraint(constraint, formatter)
						
						
						hasConstraints = true
					
				
				if parameter.Attributes.Count > 0:
					for customAttribute as ICustomAttribute in parameter.Attributes:
						if IsType(customAttribute.Constructor.DeclaringType, 'System.Runtime.CompilerServices', 'NewConstraintAttribute'):
							if hasConstraints:
								formatter.Write(', ')
							else:
								formatter.Write('{')
								hasConstraints = true
							formatter.WriteKeyword('constructor')
							formatter.Write('()')
				
				formatter.Write('}') if hasConstraints
				
	def WriteGenericParameterConstraint(value as IType, formatter as IFormatter):
		defaultConstructorConstraint as IDefaultConstructorConstraint = value as IDefaultConstructorConstraint
		if defaultConstructorConstraint != null:
			formatter.WriteKeyword('constructor')
			formatter.Write('()')
			return
		
		referenceTypeConstraint as IReferenceTypeConstraint = value as IReferenceTypeConstraint
		if referenceTypeConstraint != null:
			formatter.WriteKeyword('class')
			return
		
		valueTypeConstraint as IValueTypeConstraint = value as IValueTypeConstraint
		if valueTypeConstraint != null:
			formatter.WriteKeyword('struct')
			return
		
		self.WriteType(value, formatter)
	
	def IsNullTarget(value as IExpression) as bool:
		if value != null:
			typeReferenceExpression as ITypeReferenceExpression = value as ITypeReferenceExpression
			if typeReferenceExpression != null:
				if self.IsType(typeReferenceExpression.Type, string.Empty, '<Module>'):
					return true
			return false
		return true
	
	def IsNegativeNumber(expression as IExpression) as bool:
		literealExpression as ILiteralExpression = expression as ILiteralExpression
		if literealExpression != null:
			if literealExpression.Value isa sbyte:
				valueSByte as sbyte = literealExpression.Value
				return (valueSByte < 0)
			
			if literealExpression.Value isa short:
				valueShort as short = literealExpression.Value
				return (valueShort < 0)
			
			if literealExpression.Value isa int:
				valueInt as int = literealExpression.Value
				return (valueInt < 0)
			
			if literealExpression.Value isa long:
				valueLong as long = literealExpression.Value
				return (valueLong < 0)
			
		
		return false
	
		#This is not a good implementation, but I don't have
		#access to the visitors, and I'm not going to write my own
	def IsEmptyCatchClauseName(catchClause as ICatchClause):
		return true if catchClause.Variable.Name == "obj1"
		return false
	
	def IsFinalizeInvoke(statement as IStatement) as bool:
		expressionStatement as IExpressionStatement = statement as IExpressionStatement
		if expressionStatement != null:
			methodInvokeExpression as IMethodInvokeExpression = expressionStatement.Expression as IMethodInvokeExpression
			if (methodInvokeExpression != null) and (methodInvokeExpression.Arguments.Count == 0):
				methodReferenceExpression as IMethodReferenceExpression = methodInvokeExpression.Method as IMethodReferenceExpression
				if methodReferenceExpression != null:
					baseReferenceExpression as IBaseReferenceExpression = methodReferenceExpression.Target as IBaseReferenceExpression
					if baseReferenceExpression != null:
						if methodReferenceExpression.Method.Name == 'Finalize':
							return true
		return false
	

	def IsDllImport(value as IMethodDeclaration) as bool:
		return (self.GetCustomAttribute(value, 'System.Runtime.InteropServices', 'DllImportAttribute') != null)
	
	def IsInternalCall(value as IMethodDeclaration) as bool:
		return ((self.GetMethodImplAttribute(value) & 4096) == 4096)
	
	def GetMethodImplAttribute(value as IMethodDeclaration) as int:
		customAttribute as ICustomAttribute = self.GetCustomAttribute(value, 'System.Runtime.CompilerServices', 'MethodImplAttribute')
		if customAttribute != null:
			return self.GetMethodImplAttributeValue(customAttribute.Arguments[0]) | self.GetMethodImplAttributeMethodCodeType(customAttribute)
		
		return 0
	
	def IsInterfaceImplementation(methodDeclaration as IMethodDeclaration) as bool:
		if methodDeclaration.Overrides.Count == 1:
			return (methodDeclaration.Name.IndexOf(Char.Parse('.'), 1) != -1)
		
		return false

	def GetMethodImplAttributeValue(expression as IExpression) as int:
		literalExpression as ILiteralExpression = expression as ILiteralExpression
		if literalExpression != null:
			return cast(int, literalExpression.Value)
		
		binaryExpression as IBinaryExpression = expression as IBinaryExpression
		if (binaryExpression != null) and (binaryExpression.Operator == BinaryOperator.BitwiseOr):
			return (self.GetMethodImplAttributeValue(binaryExpression.Left) | self.GetMethodImplAttributeValue(binaryExpression.Right))
		
		fieldReferenceExpression as IFieldReferenceExpression = expression as IFieldReferenceExpression
		if fieldReferenceExpression != null:
			selector = fieldReferenceExpression.Field.Name
			if selector == 'Unmanaged':
				return 4
			elif selector == 'Synchronized':
				return 32
			elif selector == 'PreserveSig':
				return 128
			elif selector == 'NoInlining':
				return 8
			elif selector == 'Managed':
				return 0
			elif selector == 'InternalCall':
				return 4096
			elif selector == 'ForwardRef':
				return 16
			
		
		castExpression as ICastExpression = expression as ICastExpression
		if castExpression != null:
			return self.GetMethodImplAttributeValue(castExpression.Expression)
		
		raise NotSupportedException(expression.GetType().Name)
	

	def GetMethodImplAttributeMethodCodeType(customAttribute as ICustomAttribute) as int:
		for i as int in range(customAttribute.Arguments.Count):
			methodCodeTypeExpression = customAttribute.Arguments[i] as IMemberInitializerExpression
			if methodCodeTypeExpression != null:
				fieldReference = methodCodeTypeExpression.Member as IPropertyReference
				if (fieldReference != null) and (fieldReference.Name == 'MethodCodeType'):
					valueExpression = methodCodeTypeExpression.Value as IFieldReferenceExpression
					if valueExpression != null:
						selector = valueExpression.Field.Name
						if selector == 'Runtime':
							return 3
						elif selector == 'Native':
							return 1
						elif selector == 'OPTIL':
							return 2
						elif selector == 'IL':
							return 0
		
		return 0
	




#endregion
