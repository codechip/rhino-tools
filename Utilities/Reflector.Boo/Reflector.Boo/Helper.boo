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

import Reflector.CodeModel
import System
import System.Collections
import System.Globalization
import System.IO
import System.Text

internal class TextFormatter(IFormatter):

	public def constructor():
		self.writer = StringWriter(CultureInfo.InvariantCulture)
		self.allowProperties = false
		self.newLine = false
		self.indent = 0

	private def ApplyIndent():
		if self.newLine:
			for num1 in range(0, self.indent):
				self.writer.Write('    ')
			self.newLine = false

	public override def ToString() as string:
		return self.writer.ToString()

	public def Write(text as string):
		self.ApplyIndent()
		self.writer.Write(text)

	private def WriteBold(text as string):
		self.ApplyIndent()
		self.writer.Write(text)

	private def WriteColor(text as string, color as int):
		self.ApplyIndent()
		self.writer.Write(text)

	public def WriteComment(text as string):
		self.WriteColor(text, 8421504)

	public def WriteDeclaration(text as string):
		self.WriteBold(text)
		
	public def WriteDeclaration(value as string, target as object):
		pass

	public def WriteIndent():
		self.indent += 1

	public def WriteKeyword(text as string):
		self.WriteColor(text, 8421504) if text == 'pass'
		self.WriteColor(text, 128)

	public def WriteLine():
		self.writer.WriteLine()
		self.newLine = true

	public def WriteLiteral(text as string):
		self.WriteColor(text, 8388608)

	public def WriteOutdent():
		self.indent -= 1

	public def WriteProperty(propertyName as string, propertyValue as string):
		if self.allowProperties:
			raise NotSupportedException()

	public def WriteReference(text as string, toolTip as string, reference as object):
		self.ApplyIndent()
		self.writer.Write(text)

	public AllowProperties as bool:
		get:
			return self.allowProperties
		set:
			self.allowProperties = value

	private allowProperties as bool

	private indent as int

	private newLine as bool

	private writer as StringWriter
	
		
internal class TypeInformation:
	typeDec
	
	def constructor(_typeDec as ITypeReference):
		typeDec = _typeDec
		
	def IsVisible(visibility as IVisibilityConfiguration):
		return Helper.IsVisible(typeDec as IType, visibility)
		
	def GetEvents(visibility as IVisibilityConfiguration, configuration):
		return Helper.GetEvents(typeDec,visibility)
		
	def GetMethods(visibility as IVisibilityConfiguration, configuration):
		return Helper.GetMethods(typeDec,visibility)
		
	def GetProperties(visibility as IVisibilityConfiguration, configuration):
		return Helper.GetProperties(typeDec,visibility)
		
	def GetFields(visibility as IVisibilityConfiguration, configuration):
		return Helper.GetFields(typeDec,visibility)
		
	def GetNestedTypes(visibility as IVisibilityConfiguration, configuration):
		return Helper.GetNestedTypes(typeDec,visibility)
		
	def GetMethod(name as string):
		return Helper.GetMethod(typeDec,name)
		
	IsDelegate:
		get:return Helper.IsDelegate(typeDec)
		
	IsEnumeration:
		get:return Helper.IsEnumeration(typeDec)
		
	IsValueType:
		get:return Helper.IsValueType(typeDec)
		
	NameWithResolutionScope:
		get:return Helper.GetNameWithResolutionScope(typeDec)
			
	AssemblyName:
		get:return Helper.GetAssemblyReference(typeDec)
			
	Name:
		get:return Helper.GetName(typeDec as ITypeReference)
		
internal class EventInformation:
	eventDec
	
	def constructor(_eventDec as IEventDeclaration):
		eventDec = _eventDec
		
	AddMethod:
		get:return Helper.GetAddMethod(eventDec)
			
	RemoveMethod:
		get:return Helper.GetRemoveMethod(eventDec)
			
	InvokeMethod:
		get:return Helper.GetInvokeMethod(eventDec)
			
	Visibility:
		get:return Helper.GetVisibility(eventDec as IEventReference)
			
	IsStatic:
		get:return Helper.IsStatic(eventDec as IEventReference)
		


internal final class Helper:

	private def constructor():
		pass

	public static def GetAddMethod(value as IEventReference) as IMethodDeclaration:
		declaration1 as IEventDeclaration = value.Resolve()
		if declaration1.AddMethod is not null:
			return declaration1.AddMethod.Resolve()
		return null

	public static def GetAssemblyReference(value as IType) as IAssemblyReference:
		reference1 = (value as ITypeReference)
		if reference1 is not null:
			reference2 = (reference1.Owner as ITypeReference)
			if reference2 is not null:
				return Helper.GetAssemblyReference(reference2)
			reference3 = (reference1.Owner as IModuleReference)
			if reference3 is not null:
				module1 as IModule = reference3.Resolve()
				return module1.Assembly
			reference4 = (reference1.Owner as IAssemblyReference)
			if reference4 is not null:
				return reference4
		raise NotSupportedException()

	public static def GetEvents(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		collection1 as IEventDeclarationCollection = value.Events
		if collection1.Count > 0:
			for declaration1 as IEventDeclaration in collection1:
				if (visibility is null) or Helper.IsVisible(declaration1, visibility):
					list1.Add(declaration1)
			list1.Sort()
		return list1

	public static def GetFields(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		collection1 as IFieldDeclarationCollection = value.Fields
		if collection1.Count > 0:
			for declaration1 as IFieldDeclaration in collection1:
				if (visibility is null) or Helper.IsVisible(declaration1, visibility):
					list1.Add(declaration1)
			list1.Sort()
		return list1

	public static def GetGetMethod(value as IPropertyReference) as IMethodDeclaration:
		declaration1 as IPropertyDeclaration = value.Resolve()
		if declaration1.GetMethod is not null:
			return declaration1.GetMethod.Resolve()
		return null

	private static def GetInterfaces(value as ITypeDeclaration) as ICollection:
		list1 = ArrayList(0)
		list1.AddRange(value.Interfaces)
		if value.BaseType is not null:
			declaration1 as ITypeDeclaration = value.BaseType.Resolve()
			for reference1 as ITypeReference in declaration1.Interfaces:
				if list1.Contains(reference1):
					list1.Remove(reference1)
		for reference2 as ITypeReference in value.Interfaces:
			declaration2 as ITypeDeclaration = reference2.Resolve()
			for reference3 as ITypeReference in declaration2.Interfaces:
				if list1.Contains(reference3):
					list1.Remove(reference3)
		referenceArray1 as (ITypeReference) = array(ITypeReference, list1.Count)
		list1.CopyTo(referenceArray1, 0)
		return referenceArray1

	public static def GetInterfaces(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		for reference1 as ITypeReference in Helper.GetInterfaces(value):
			if Helper.IsVisible(reference1, visibility):
				list1.Add(reference1)
		list1.Sort()
		return list1

	public static def GetInvokeMethod(value as IEventReference) as IMethodDeclaration:
		declaration1 as IEventDeclaration = value.Resolve()
		if declaration1.InvokeMethod is not null:
			return declaration1.InvokeMethod.Resolve()
		return null

	public static def GetMethod(value as ITypeDeclaration, methodName as string) as IMethodDeclaration:
		collection1 as IMethodDeclarationCollection = value.Methods
		for num1 in range(0, collection1.Count):
			if methodName == collection1[num1].Name:
				return collection1[num1]
		return null

	public static def GetMethods(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		collection1 as IMethodDeclarationCollection = value.Methods
		if collection1.Count > 0:
			for declaration1 as IMethodDeclaration in collection1:
				if (visibility is null) or Helper.IsVisible(declaration1, visibility):
					list1.Add(declaration1)
			for declaration2 as IPropertyDeclaration in value.Properties:
				if declaration2.SetMethod is not null:
					list1.Remove(declaration2.SetMethod.Resolve())
				if declaration2.GetMethod is not null:
					list1.Remove(declaration2.GetMethod.Resolve())
			for declaration3 as IEventDeclaration in value.Events:
				if declaration3.AddMethod is not null:
					list1.Remove(declaration3.AddMethod.Resolve())
				if declaration3.RemoveMethod is not null:
					list1.Remove(declaration3.RemoveMethod.Resolve())
				if declaration3.InvokeMethod is not null:
					list1.Remove(declaration3.InvokeMethod.Resolve())
			list1.Sort()
		return list1

	public static def GetName(value as IEventReference) as string:
		return value.Name

	public static def GetName(value as IFieldReference) as string:
		type1 as IType = value.FieldType
		type2 as IType = value.DeclaringType
		if type1.Equals(type2):
			reference1 = (type1 as ITypeReference)
			if (reference1 is not null) and Helper.IsEnumeration(reference1):
				return value.Name
		return ((value.Name + ' : ') + value.FieldType.ToString())

	public static def GetName(value as IMethodReference) as string:
		collection1 as ITypeCollection = value.GenericArguments
		if collection1.Count > 0:
			using writer1 = StringWriter(CultureInfo.InvariantCulture):
				for num1 in range(0, collection1.Count):
					if num1 != 0:
						writer1.Write(', ')
					type1 as IType = collection1[num1]
					if type1 is not null:
						writer1.Write(type1.ToString())
					else:
						writer1.Write('???')
				return (((value.Name + '<') + writer1.ToString()) + '>')
		return value.Name

	public static def GetName(value as IPropertyReference) as string:
		collection1 as IParameterDeclarationCollection = value.Parameters
		if collection1.Count > 0:
			using writer1 = StringWriter(CultureInfo.InvariantCulture):
				for num1 in range(0, collection1.Count):
					if num1 != 0:
						writer1.Write(', ')
					writer1.Write(collection1[num1].ParameterType.ToString())
				return ((((value.Name + '[') + writer1.ToString()) + '] : ') + value.PropertyType.ToString())
		return ((value.Name + ' : ') + value.PropertyType.ToString())

	public static def GetName(value as ITypeReference) as string:
		if value is null:
			raise NotSupportedException()
		collection1 as ITypeCollection = value.GenericArguments
		if collection1.Count > 0:
			using writer1 = StringWriter(CultureInfo.InvariantCulture):
				for num1 in range(0, collection1.Count):
					if num1 != 0:
						writer1.Write(',')
					type1 as IType = collection1[num1]
					if type1 is not null:
						writer1.Write(type1.ToString())
				return (((value.Name + '<') + writer1.ToString()) + '>')
		return value.Name

	public static def GetNameWithDeclaringType(value as IEventReference) as string:
		return ((Helper.GetNameWithResolutionScope((value.DeclaringType as ITypeReference)) + '.') + Helper.GetName(value))

	public static def GetNameWithDeclaringType(value as IFieldReference) as string:
		return ((Helper.GetNameWithResolutionScope((value.DeclaringType as ITypeReference)) + '.') + Helper.GetName(value))

	public static def GetNameWithDeclaringType(value as IMethodReference) as string:
		reference1 = (value.DeclaringType as ITypeReference)
		if reference1 is null:
			type1 = (value.DeclaringType as IArrayType)
			if type1 is null:
				raise NotSupportedException()
			return ((type1.ToString() + '.') + Helper.GetNameWithParameterList(value))
		return ((Helper.GetNameWithResolutionScope(reference1) + '.') + Helper.GetNameWithParameterList(value))

	public static def GetNameWithDeclaringType(value as IPropertyReference) as string:
		return ((Helper.GetNameWithResolutionScope((value.DeclaringType as ITypeReference)) + '.') + Helper.GetName(value))

	public static def GetNameWithParameterList(value as IMethodReference) as string:
		using writer1 = StringWriter(CultureInfo.InvariantCulture):
			writer1.Write(Helper.GetName(value))
			writer1.Write('(')
			collection1 as IParameterDeclarationCollection = value.Parameters
			for num1 in range(0, collection1.Count):
				if num1 != 0:
					writer1.Write(', ')
				writer1.Write(collection1[num1].ParameterType.ToString())
			if value.CallingConvention == MethodCallingConvention.VariableArguments:
				if value.Parameters.Count > 0:
					writer1.Write(', ')
				writer1.Write('...')
			writer1.Write(')')
			if (value.Name != '.ctor') and (value.Name != '.cctor'):
				writer1.Write(' : ')
				writer1.Write(value.ReturnType.Type.ToString())
			return writer1.ToString()

	public static def GetNameWithResolutionScope(value as ITypeReference) as string:
		if value is null:
			raise NotSupportedException()
		reference1 = (value.Owner as ITypeReference)
		if reference1 is not null:
			return ((Helper.GetNameWithResolutionScope(reference1) + '+') + Helper.GetName(value))
		text1 as string = value.Namespace
		if text1.Length == 0:
			return Helper.GetName(value)
		return ((text1 + '.') + Helper.GetName(value))

	public static def GetNestedTypes(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		collection1 as ITypeDeclarationCollection = value.NestedTypes
		if collection1.Count > 0:
			for declaration1 as ITypeDeclaration in collection1:
				if Helper.IsVisible(declaration1, visibility):
					list1.Add(declaration1)
			list1.Sort()
		return list1

	public static def GetProperties(value as ITypeDeclaration, visibility as IVisibilityConfiguration) as ICollection:
		list1 = ArrayList(0)
		collection1 as IPropertyDeclarationCollection = value.Properties
		if collection1.Count > 0:
			for declaration1 as IPropertyDeclaration in collection1:
				if (visibility is null) or Helper.IsVisible(declaration1, visibility):
					list1.Add(declaration1)
			list1.Sort()
		return list1

	public static def GetRemoveMethod(value as IEventReference) as IMethodDeclaration:
		declaration1 as IEventDeclaration = value.Resolve()
		if declaration1.RemoveMethod is not null:
			return declaration1.RemoveMethod.Resolve()
		return null

	public static def GetResolutionScope(value as ITypeReference) as string:
		if not (value.Owner isa IModule):
			declaration1 = (value.Owner as ITypeDeclaration)
			if declaration1 is null:
				raise NotSupportedException()
			return ((Helper.GetResolutionScope(declaration1) + '+') + Helper.GetName(declaration1))
		return value.Namespace

	public static def GetSetMethod(value as IPropertyReference) as IMethodDeclaration:
		declaration1 as IPropertyDeclaration = value.Resolve()
		if declaration1.SetMethod is not null:
			return declaration1.SetMethod.Resolve()
		return null

	public static def GetVisibility(value as IEventReference) as MethodVisibility:
		declaration1 as IMethodDeclaration = Helper.GetAddMethod(value)
		declaration2 as IMethodDeclaration = Helper.GetRemoveMethod(value)
		declaration3 as IMethodDeclaration = Helper.GetInvokeMethod(value)
		if ((declaration1 is not null) and (declaration2 is not null)) and (declaration3 is not null):
			if (declaration1.Visibility == declaration2.Visibility) and (declaration1.Visibility == declaration3.Visibility):
				return declaration1.Visibility
		else:
			if (declaration1 is not null) and (declaration2 is not null):
				if declaration1.Visibility == declaration2.Visibility:
					return declaration1.Visibility
			else:
				if (declaration1 is not null) and (declaration3 is not null):
					if declaration1.Visibility == declaration3.Visibility:
						return declaration1.Visibility
				else:
					if (declaration2 is not null) and (declaration3 is not null):
						if declaration2.Visibility == declaration3.Visibility:
							return declaration2.Visibility
					else:
						if declaration1 is not null:
							return declaration1.Visibility
						if declaration2 is not null:
							return declaration2.Visibility
						if declaration3 is not null:
							return declaration3.Visibility
		return MethodVisibility.Public

	public static def GetVisibility(value as IPropertyReference) as MethodVisibility:
		declaration1 as IMethodDeclaration = Helper.GetGetMethod(value)
		declaration2 as IMethodDeclaration = Helper.GetSetMethod(value)
		visibility1 as MethodVisibility = MethodVisibility.Public
		if (declaration2 is not null) and (declaration1 is not null):
			if declaration1.Visibility == declaration2.Visibility:
				visibility1 = declaration1.Visibility
			return visibility1
		if declaration2 is not null:
			return declaration2.Visibility
		if declaration1 is not null:
			visibility1 = declaration1.Visibility
		return visibility1

	public static def IsDelegate(value as ITypeReference) as bool:
		if value is not null:
			if (value.Name == 'MulticastDelegate') and (value.Namespace == 'System'):
				return false
			declaration1 as ITypeDeclaration = value.Resolve()
			if declaration1 is null:
				return false
			reference1 as ITypeReference = declaration1.BaseType
			return ((((reference1 is not null) and (reference1.Namespace == 'System')) and ((reference1.Name == 'MulticastDelegate') or (reference1.Name == 'Delegate'))) and (reference1.Namespace == 'System'))
		return false

	public static def IsEnumeration(value as ITypeReference) as bool:
		if value is not null:
			declaration1 as ITypeDeclaration = value.Resolve()
			if declaration1 is null:
				return false
			reference1 as ITypeReference = declaration1.BaseType
			return (((reference1 is not null) and (reference1.Name == 'Enum')) and (reference1.Namespace == 'System'))
		return false

	public static def IsStatic(value as IEventReference) as bool:
		flag1 = false
		if Helper.GetAddMethod(value) is not null:
			flag1 |= Helper.GetAddMethod(value).Static
		if Helper.GetRemoveMethod(value) is not null:
			flag1 |= Helper.GetRemoveMethod(value).Static
		if Helper.GetInvokeMethod(value) is not null:
			flag1 |= Helper.GetInvokeMethod(value).Static
		return flag1

	public static def IsStatic(value as IPropertyReference) as bool:
		declaration1 as IMethodDeclaration = Helper.GetSetMethod(value)
		declaration2 as IMethodDeclaration = Helper.GetGetMethod(value)
		flag1 = false
		flag1 |= ((declaration1 is not null) and declaration1.Static)
		return (flag1 | ((declaration2 is not null) and declaration2.Static))

	public static def IsValueType(value as ITypeReference) as bool:
		if value is not null:
			declaration1 as ITypeDeclaration = value.Resolve()
			if declaration1 is null:
				return false
			reference1 as ITypeReference = declaration1.BaseType
			return (((reference1 is not null) and ((reference1.Name == 'ValueType') or (reference1.Name == 'Enum'))) and (reference1.Namespace == 'System'))
		return false

	public static def IsVisible(value as IEventReference, visibility as IVisibilityConfiguration) as bool:
		if Helper.IsVisible(value.DeclaringType, visibility):
			converterGeneratedName1 = Helper.GetVisibility(value)
			if (converterGeneratedName1 == MethodVisibility.PrivateScope) or (converterGeneratedName1 == MethodVisibility.Private):
				return visibility.Private
			else:
				if converterGeneratedName1 == MethodVisibility.FamilyAndAssembly:
					return visibility.FamilyAndAssembly
				else:
					if converterGeneratedName1 == MethodVisibility.Assembly:
						return visibility.Assembly
					else:
						if converterGeneratedName1 == MethodVisibility.Family:
							return visibility.Family
						else:
							if converterGeneratedName1 == MethodVisibility.FamilyOrAssembly:
								return visibility.FamilyOrAssembly
							else:
								if converterGeneratedName1 == MethodVisibility.Public:
									return visibility.Public
			raise NotSupportedException()
		return false

	public static def IsVisible(value as IFieldReference, visibility as IVisibilityConfiguration) as bool:
		if Helper.IsVisible(value.DeclaringType, visibility):
			declaration1 as IFieldDeclaration = value.Resolve()
			if declaration1 is not null:
				converterGeneratedName2 = declaration1.Visibility
				if converterGeneratedName2 == FieldVisibility.PrivateScope:
					return visibility.Private
				else:
					if converterGeneratedName2 == FieldVisibility.Private:
						return visibility.Private
					else:
						if converterGeneratedName2 == FieldVisibility.FamilyAndAssembly:
							return visibility.FamilyAndAssembly
						else:
							if converterGeneratedName2 == FieldVisibility.Assembly:
								return visibility.Assembly
							else:
								if converterGeneratedName2 == FieldVisibility.Family:
									return visibility.Family
								else:
									if converterGeneratedName2 == FieldVisibility.FamilyOrAssembly:
										return visibility.FamilyOrAssembly
									else:
										if converterGeneratedName2 == FieldVisibility.Public:
											return visibility.Public
				raise NotSupportedException()
			return true
		return false

	public static def IsVisible(value as IMethodReference, visibility as IVisibilityConfiguration) as bool:
		if Helper.IsVisible(value.DeclaringType, visibility):
			declaration1 as IMethodDeclaration = value.Resolve()
			converterGeneratedName3 = declaration1.Visibility
			if (converterGeneratedName3 == MethodVisibility.PrivateScope) or (converterGeneratedName3 == MethodVisibility.Private):
				return visibility.Private
			else:
				if converterGeneratedName3 == MethodVisibility.FamilyAndAssembly:
					return visibility.FamilyAndAssembly
				else:
					if converterGeneratedName3 == MethodVisibility.Assembly:
						return visibility.Assembly
					else:
						if converterGeneratedName3 == MethodVisibility.Family:
							return visibility.Family
						else:
							if converterGeneratedName3 == MethodVisibility.FamilyOrAssembly:
								return visibility.FamilyOrAssembly
							else:
								if converterGeneratedName3 == MethodVisibility.Public:
									return visibility.Public
			raise NotSupportedException()
		return false

	public static def IsVisible(value as IPropertyReference, visibility as IVisibilityConfiguration) as bool:
		if Helper.IsVisible(value.DeclaringType, visibility):
			converterGeneratedName4 = Helper.GetVisibility(value)
			if (converterGeneratedName4 == MethodVisibility.PrivateScope) or (converterGeneratedName4 == MethodVisibility.Private):
				return visibility.Private
			else:
				if converterGeneratedName4 == MethodVisibility.FamilyAndAssembly:
					return visibility.FamilyAndAssembly
				else:
					if converterGeneratedName4 == MethodVisibility.Assembly:
						return visibility.Assembly
					else:
						if converterGeneratedName4 == MethodVisibility.Family:
							return visibility.Family
						else:
							if converterGeneratedName4 == MethodVisibility.FamilyOrAssembly:
								return visibility.FamilyOrAssembly
							else:
								if converterGeneratedName4 == MethodVisibility.Public:
									return visibility.Public
			raise NotSupportedException()
		return false

	public static def IsVisible(value as IType, visibility as IVisibilityConfiguration) as bool:
		reference1 = (value as ITypeReference)
		if reference1 is null:
			raise NotSupportedException()
		reference2 = (reference1.Owner as ITypeReference)
		if (reference2 is null) or Helper.IsVisible(reference2, visibility):
			declaration1 as ITypeDeclaration = reference1.Resolve()
			if declaration1 is not null:
				converterGeneratedName5 = declaration1.Visibility
				if (converterGeneratedName5 == TypeVisibility.Private) or (converterGeneratedName5 == TypeVisibility.NestedPrivate):
					return visibility.Private
				else:
					if (converterGeneratedName5 == TypeVisibility.Public) or (converterGeneratedName5 == TypeVisibility.NestedPublic):
						return visibility.Public
					else:
						if converterGeneratedName5 == TypeVisibility.NestedFamily:
							return visibility.Family
						else:
							if converterGeneratedName5 == TypeVisibility.NestedAssembly:
								return visibility.Assembly
							else:
								if converterGeneratedName5 == TypeVisibility.NestedFamilyAndAssembly:
									return visibility.FamilyAndAssembly
								else:
									if converterGeneratedName5 == TypeVisibility.NestedFamilyOrAssembly:
										return visibility.FamilyOrAssembly
				raise NotImplementedException()
			return true
		return false
