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


class WriteUtil:
	
	boo as BooLanguageWriter
	expressionUtil as ExpressionUtil
	[Property(ValueParameterDeclaration)]
	valueParameterDeclaration as IParameterDeclaration 	
	statementUtil as StatementUtil	
	specialMethodsNames as IDictionary
	referenceUtil as ReferenceUtil
	
	def constructor(boo as BooLanguageWriter):
		self.boo = boo
	
	def Init():
		self.expressionUtil = boo.ExpressionWriter
		self.statementUtil = boo.StatementWriter
		self.referenceUtil = boo.ReferenceWriter
		
	def WriteFieldDeclaration(value as IFieldDeclaration, formatter as IFormatter):
		raise ArgumentNullException('value') if value is null
		
		boo.WriteCustomAttributes(value, formatter, null) if boo.Configuration["ShowCustomAttributes"] == "true"
		
		if not self.IsEnumerationElement(value):
			selector = value.Visibility
			if selector == FieldVisibility.Public:
				formatter.WriteKeyword('public')
			elif selector == FieldVisibility.Private:
				formatter.WriteKeyword('private')
			elif selector == FieldVisibility.PrivateScope:
				formatter.WriteComment('/* private scope */')
			elif selector == FieldVisibility.Assembly:
				formatter.WriteKeyword('internal')
			elif selector == FieldVisibility.Family:
				formatter.WriteKeyword('protected')
			elif selector == FieldVisibility.FamilyOrAssembly:
				formatter.WriteKeyword('protected internal')
			elif selector == FieldVisibility.FamilyAndAssembly:
				formatter.WriteKeyword('internal protected')
			else:
				raise NotSupportedException()
			
			formatter.Write(' ')
			
			if (value.Static) and (value.Literal):
				formatter.WriteKeyword('const')
				formatter.Write(' ')
			else:
				if value.Static:
					formatter.WriteKeyword('static')
					formatter.Write(' ')
				
				if value.ReadOnly:
					formatter.WriteKeyword('readonly')
					formatter.Write(' ')
		
		self.WriteDeclaration(value.Name, formatter)
		if not self.IsEnumerationElement(value):
			boo.WriteAs(formatter)
			boo.WriteType(value.FieldType, formatter)
		
		initializer as IExpression = value.Initializer
		if initializer != null:
			formatter.Write(' = ')
			expressionUtil.WriteExpression(initializer, formatter)
		
		formatter.WriteLine()
		
		self.WriteDeclaringType(value.DeclaringType as ITypeReference, formatter)
	
	def WriteEventDeclaration([required] value as IEventDeclaration, [required] formatter as IFormatter):
		if (boo.Configuration["ShowCustomAttributes"] == "true") and (value.Attributes.Count != 0):
			boo.WriteCustomAttributes(value, formatter, null)
			formatter.WriteLine()
		
		fieldReference as IFieldReference = boo.GetUnderlyingFieldReference(value)
		if fieldReference != null:
			fieldDeclaration as IFieldDeclaration = fieldReference.Resolve()
			if fieldDeclaration != null:
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (fieldDeclaration.Attributes.Count != 0):
					boo.WriteCustomAttributes(fieldDeclaration, formatter, 'field:')
					formatter.WriteLine()
				
			
		
		information as EventInformation = EventInformation(value)
		addMethod as IMethodDeclaration = information.AddMethod
		removeMethod as IMethodDeclaration = information.RemoveMethod
		invokeMethod as IMethodDeclaration = information.InvokeMethod
		interfaceType as ITypeReference = null
		if (addMethod != null) and (boo.IsInterfaceImplementation(addMethod)):
			interfaceType = addMethod.Overrides[0].DeclaringType as ITypeReference
		elif (removeMethod != null) and (boo.IsInterfaceImplementation(removeMethod)):
			interfaceType = removeMethod.Overrides[0].DeclaringType as ITypeReference
		elif (invokeMethod != null) and (boo.IsInterfaceImplementation(invokeMethod)):
			interfaceType = invokeMethod.Overrides[0].DeclaringType as ITypeReference
		
		declaringType as ITypeDeclaration = (value.DeclaringType as ITypeReference).Resolve()
		if (not declaringType.Interface) and (interfaceType == null):
			selector = information.Visibility
			if selector == MethodVisibility.Public:
				formatter.WriteKeyword('public')
			elif selector == MethodVisibility.Private:
				formatter.WriteKeyword('private')
			elif selector == MethodVisibility.PrivateScope:
				formatter.WriteComment('/* private scope */')
			elif selector == MethodVisibility.Family:
				formatter.WriteKeyword('protected')
			elif selector == MethodVisibility.Assembly:
				formatter.WriteKeyword('internal')
			elif selector == MethodVisibility.FamilyOrAssembly:
				formatter.WriteKeyword('protected internal')
			elif selector == MethodVisibility.FamilyAndAssembly:
				formatter.WriteKeyword('internal protected')
			else:
				raise NotSupportedException()
			
			formatter.Write(' ')
		
		if information.IsStatic:
			formatter.WriteKeyword('static')
			formatter.Write(' ')
		
		formatter.WriteKeyword('event')
		formatter.Write(' ')
		boo.WriteType(value.EventType, formatter)
		formatter.Write(' ')
		eventName as string = value.Name
		if interfaceType != null:
			index as int = eventName.LastIndexOf(Char.Parse('.'))
			if index != -1:
				eventName = eventName.Substring(index + 1)
				boo.WriteType(interfaceType, formatter)
				formatter.Write('.')
			
		
		self.WriteDeclaration(eventName, formatter)
		formatter.Write(':')
		
		missingBodies as bool = (((addMethod == null) or (not addMethod.Body isa IStatement)) and ((removeMethod == null) or (not removeMethod.Body isa IStatement)) and ((invokeMethod == null) or (not invokeMethod.Body isa IStatement)))
		if (missingBodies) or (fieldReference != null):
			formatter.WriteIndent()
			formatter.WriteLine()
			boo.WritePass(formatter)
			formatter.WriteOutdent()
			formatter.WriteLine()	
		else:
			hasBody as bool = (((addMethod != null) and (addMethod.Body isa IStatement)) or ((removeMethod != null) and (removeMethod.Body isa IStatement)) or ((invokeMethod != null) and (invokeMethod.Body isa IStatement)))
			formatter.WriteIndent()
			formatter.WriteLine()
			
			if addMethod != null:
				if not hasBody:
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (addMethod.ReturnType.Attributes.Count != 0):
					boo.WriteCustomAttributes(addMethod.ReturnType, formatter, 'return:')
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (addMethod.Attributes.Count != 0):
					boo.WriteCustomAttributes(addMethod, formatter, null)
					formatter.Write(' ')
				
				formatter.WriteKeyword('add')
				formatter.Write(':')
				formatter.WriteIndent()
				formatter.WriteLine()
					
				if addMethod.Body isa IStatement:
					#formatter.Write(addMethod.Body.GetType().FullName)
					statementUtil.WriteStatement(addMethod.Body, formatter)
				else:
					boo.WritePass(formatter)

				formatter.WriteOutdent()
				formatter.WriteLine()
				
			
			if removeMethod != null:
				if not hasBody:
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (removeMethod.ReturnType.Attributes.Count != 0):
					boo.WriteCustomAttributes(removeMethod.ReturnType, formatter, 'return:')
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (removeMethod.Attributes.Count != 0):
					boo.WriteCustomAttributes(removeMethod, formatter,null)
					formatter.Write(' ')
				
				formatter.WriteKeyword('remove')
				formatter.Write(':')
				formatter.WriteLine()
				formatter.WriteIndent()

				if removeMethod.Body isa IStatement:
					#formatter.Write(removeMethod.Body.GetType().FullName)
					statementUtil.WriteStatement(removeMethod.Body, formatter)
				else:
					boo.WritePass(formatter)
				
				formatter.WriteOutdent()
				formatter.WriteLine()
			
			if invokeMethod != null:
				if not hasBody:
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (invokeMethod.ReturnType.Attributes.Count != 0):
					boo.WriteCustomAttributes(invokeMethod.ReturnType, formatter, 'return:')
					formatter.Write(' ')
				
				if (boo.Configuration["ShowCustomAttributes"] == "true") and (invokeMethod.Attributes.Count != 0):
					boo.WriteCustomAttributes(invokeMethod, formatter,null)
					formatter.Write(' ')
				
				formatter.WriteKeyword('raise')
				formatter.Write(':')
				formatter.WriteLine()
				formatter.WriteIndent()

				if invokeMethod.Body isa IStatement:
					statementUtil.WriteStatement(invokeMethod.Body, formatter)
				else:
					boo.WritePass(formatter)
				
			formatter.WriteOutdent()
			formatter.WriteLine()
		
		self.WriteDeclaringType(value.DeclaringType as ITypeReference, formatter)
	
	def WriteMethodVisibility(methodDeclaration as IMethodDeclaration, formatter as IFormatter):
		declaringType as ITypeDeclaration = (methodDeclaration.DeclaringType as ITypeReference).Resolve()
		if (not declaringType.Interface) and (not boo.IsInterfaceImplementation(methodDeclaration)):
			if (not methodDeclaration.SpecialName) or (methodDeclaration.Name != '.cctor'):
				selector = methodDeclaration.Visibility
				if selector == MethodVisibility.Public:
					formatter.WriteKeyword('public')
				elif selector == MethodVisibility.Private:
					formatter.WriteKeyword('private')
				elif selector == MethodVisibility.PrivateScope:
					formatter.WriteComment('/* private scope */')
				elif selector == MethodVisibility.Family:
					formatter.WriteKeyword('protected')
				elif selector == MethodVisibility.Assembly:
					formatter.WriteKeyword('internal')
				elif selector == MethodVisibility.FamilyOrAssembly:
					formatter.WriteKeyword('protected internal')
				elif selector == MethodVisibility.FamilyAndAssembly:
					formatter.WriteKeyword('internal protected')
				else:
					raise NotSupportedException()
				
				formatter.Write(' ')
	
	def WriteMethodAttributes(methodDeclaration as IMethodDeclaration, formatter as IFormatter):
		declaringType as ITypeDeclaration = (methodDeclaration.DeclaringType as ITypeReference).Resolve()
		if (not declaringType.Interface) and (not boo.IsInterfaceImplementation(methodDeclaration)):
			if methodDeclaration.Static:
				formatter.WriteKeyword('static')
				formatter.Write(' ')
			
			if (not methodDeclaration.NewSlot) and (methodDeclaration.Final):
				formatter.WriteKeyword('sealed')
				formatter.Write(' ')
			
			if methodDeclaration.Visibility != MethodVisibility.Private:
				if methodDeclaration.Virtual:
					if methodDeclaration.Abstract:
						formatter.WriteKeyword('abstract')
						formatter.Write(' ')
					elif (methodDeclaration.NewSlot) and (not methodDeclaration.Final):
						formatter.WriteKeyword('virtual')
						formatter.Write(' ')
					
					if not methodDeclaration.NewSlot:
						formatter.WriteKeyword('override')
						formatter.Write(' ')
					
		
	def WriteMethodDeclaration(value as IMethodDeclaration, formatter as IFormatter):
		raise ArgumentNullException('value') if value is null
		
		statementUtil.LineBreak = true
		if (boo.Configuration["ShowCustomAttributes"] == "true"):
			boo.WriteCustomAttributes(value.ReturnType, formatter, 'return:')
		
		if (boo.Configuration["ShowCustomAttributes"] == "true"):
			boo.WriteCustomAttributes(value, formatter, null)
		
		if self.WriteMethodDeclarationFinalizer(value, formatter, boo.Configuration):
			return
		
		self.WriteMethodVisibility(value, formatter)
		self.WriteMethodAttributes(value, formatter)
		if (boo.IsDllImport(value)) or (boo.IsInternalCall(value)):
#			if (boo.GetCustomAttribute(value, "System.Runtime.InteropServices", "DllImportAttribute") != null):
				formatter.WriteKeyword('extern')
				formatter.Write(' ')
		
#		formatter.Write(' ')
		formatter.WriteKeyword('def')
		formatter.Write(' ')
		methodName  = value.Name
		if boo.IsConstructor(value):
			formatter.WriteKeyword("constructor")
		else:
			if boo.IsInterfaceImplementation(value):
				index as int = methodName.LastIndexOf(Char.Parse('.'))
				if index != -1:
					methodName = methodName.Substring(index + 1)
					boo.WriteType(value.Overrides[0].DeclaringType, formatter)
					formatter.Write('.')
				
			self.WriteDeclaration(methodName, formatter)
			
		boo.WriteGenericArgumentList(value.GenericArguments, formatter)
		formatter.Write('(')
		boo.WriteParameterDeclarationList(value.Parameters, formatter, boo.Configuration)
		if value.CallingConvention == MethodCallingConvention.VariableArguments:
			if value.Parameters.Count > 0:
				formatter.Write(', ')
			
			formatter.WriteKeyword('__arglist')
		
		formatter.Write(')')
		
		if not boo.IsConstructor(value) and not boo.IsType(value.ReturnType.Type, "System","Void"):
			boo.WriteAs(formatter)
			boo.WriteType(value.ReturnType.Type, formatter)
		
		formatter.Write(':')
		
		boo.WriteGenericParameterConstraintList(value.GenericArguments, formatter)
				
		self.WriteDeclaringType(value.DeclaringType as ITypeReference, formatter)
		if value.Body isa IBlockStatement:
			self.WriteMethodBody(value, value.Body, formatter)
	
	def WriteMethodDeclarationFinalizer(methodDeclaration as IMethodDeclaration, formatter as IFormatter, configuration as ILanguageWriterConfiguration) as bool:
		if methodDeclaration.Body isa IBlockStatement:
			blockStatement = methodDeclaration.Body as IBlockStatement
			if (blockStatement.Statements.Count == 1):
				if (methodDeclaration.Name == 'Finalize') and (methodDeclaration.Visibility == MethodVisibility.Family):
					tryCatchFinallyStatement as ITryCatchFinallyStatement = blockStatement.Statements[0] as ITryCatchFinallyStatement
					if (tryCatchFinallyStatement != null) and (tryCatchFinallyStatement.CatchClauses.Count == 0) and (tryCatchFinallyStatement.Finally != null) and (tryCatchFinallyStatement.Finally.Statements.Count == 1):
						if boo.IsFinalizeInvoke(tryCatchFinallyStatement.Finally.Statements[0]):
							declaringType = methodDeclaration.DeclaringType as ITypeReference
							formatter.WriteKeyword('destructor')
							formatter.Write('():')
							formatter.WriteLine()
							self.WriteMethodBody(methodDeclaration, tryCatchFinallyStatement.Try, formatter)
							return true
										
					if (blockStatement.Statements.Count == 1) and (boo.IsFinalizeInvoke(blockStatement.Statements[0])):
						declaringType = methodDeclaration.DeclaringType as ITypeReference
						formatter.WriteKeyword('destructor')
						formatter.Write('():')
						formatter.WriteLine()
						boo.WritePass(formatter)
						return true
					
		return false
	
	def WriteConstructorInitializer(value as IConstructorDeclaration, formatter as IFormatter):
		return false if value is null or value.Initializer.Arguments.Count == 0
		
		initMethod = value.Initializer.Method as IMethodReferenceExpression
		
		return false if initMethod is null
		
		if initMethod.Target isa IBaseReferenceExpression:
			method = 'super'
		elif initMethod.Target isa IThisReferenceExpression:
			method = 'constructor'
		else:
			method = '?'
		
		formatter.WriteReference(method, referenceUtil.ReferenceDescription.Get(initMethod.Method), initMethod)
		formatter.Write('(')
		boo.WriteExpressionList(value.Initializer.Arguments, formatter)
		formatter.Write(')')
		formatter.WriteLine()
		
		return true
				
	def WriteMethodBody(value as IMethodDeclaration, statement as IBlockStatement, formatter as IFormatter):
		if boo.Configuration["ShowMethodDeclarationBody"] == "true":
			formatter.WriteLine()
			formatter.WriteIndent()
			wroteLine = WriteConstructorInitializer(value as IConstructorDeclaration, formatter)
			if statement is not null and statement.Statements.Count > 0:
				statementUtil.WriteBlockStatement(statement, formatter)
			elif not wroteLine:
				boo.WritePass(formatter)
			formatter.WriteOutdent()
			formatter.WriteLine()			
		
	def WritePropertyDeclaration(value as IPropertyDeclaration, formatter as IFormatter):
		raise ArgumentNullException('value') if value is null
		
		if (boo.Configuration["ShowCustomAttributes"] == "true"):
			boo.WriteCustomAttributes(value, formatter, null)
#			formatter.WriteLine()
		
		getMethod as IMethodDeclaration = null
		if value.GetMethod != null:
			getMethod = value.GetMethod.Resolve()
		
		setMethod as IMethodDeclaration = null
		if value.SetMethod != null:
			setMethod = value.SetMethod.Resolve()
		
		hasSameVisibility as bool = true
		hasSameAttributes as bool = true
		if (getMethod != null) and (setMethod != null):
			hasSameVisibility = hasSameVisibility & (getMethod.Visibility == setMethod.Visibility)
			hasSameAttributes = hasSameAttributes & (getMethod.Static == setMethod.Static)
			hasSameAttributes = hasSameAttributes & (getMethod.Final == setMethod.Final)
			hasSameAttributes = hasSameAttributes & (getMethod.Virtual == setMethod.Virtual)
			hasSameAttributes = hasSameAttributes & (getMethod.Abstract == setMethod.Abstract)
			hasSameAttributes = hasSameAttributes & (getMethod.NewSlot == setMethod.NewSlot)
		
		if hasSameVisibility:
			if getMethod != null:
				self.WriteMethodVisibility(getMethod, formatter)
			elif setMethod != null:
				self.WriteMethodVisibility(setMethod, formatter)
			
		
		if hasSameAttributes:
			if getMethod != null:
				self.WriteMethodAttributes(getMethod, formatter)
			elif setMethod != null:
				self.WriteMethodAttributes(setMethod, formatter)
			
		
		interfaceType as ITypeReference = null
		if (getMethod != null) and (boo.IsInterfaceImplementation(getMethod)):
			interfaceType = getMethod.Overrides[0].DeclaringType as ITypeReference
		elif (setMethod != null) and (boo.IsInterfaceImplementation(setMethod)):
			interfaceType = setMethod.Overrides[0].DeclaringType as ITypeReference
		
		propertyName as string = value.Name
		if interfaceType != null:
			index as int = propertyName.LastIndexOf(char('.'))
			if index != -1:
				propertyName = propertyName.Substring(index + 1)
				boo.WriteType(interfaceType, formatter)
				formatter.Write('.')
			
		
		parameters as IParameterDeclarationCollection = value.Parameters
		if parameters.Count > 0:
			formatter.WriteDeclaration('self')
			formatter.Write('[')
			boo.WriteParameterDeclarationList(parameters, formatter, boo.Configuration)
			formatter.Write(']')
		else:
			self.WriteDeclaration(propertyName, formatter)
	
		boo.WriteAs(formatter)
		boo.WriteType(value.PropertyType, formatter)
			
		if value.Initializer != null:
			formatter.Write(' = ')
			expressionUtil.WriteExpression(value.Initializer, formatter)
		
		hasBody as bool = (((getMethod != null) and (getMethod.Body != null)) or ((setMethod != null) and (setMethod.Body != null)))
		
		formatter.Write(':')
		formatter.WriteLine()
		formatter.WriteIndent()
				
		if getMethod != null:
		
			if not hasSameVisibility:
				self.WriteMethodVisibility(getMethod, formatter)
			
			if not hasSameAttributes:
				self.WriteMethodAttributes(getMethod, formatter)
			
			if (boo.Configuration["ShowCustomAttributes"] == "true") and (getMethod.ReturnType.Attributes.Count != 0):
				boo.WriteCustomAttributes(getMethod.ReturnType, formatter, 'return:')
				if hasBody:
					formatter.WriteLine()
				else:
					formatter.Write(' ')
				
			
			if (boo.Configuration["ShowCustomAttributes"] == "true") and (getMethod.Attributes.Count != 0):
				boo.WriteCustomAttributes(getMethod, formatter, null)
#				formatter.WriteLine()
			
			formatter.WriteKeyword('get')
			formatter.Write(':')
			if getMethod.Body != null and getMethod.Body isa IBlockStatement:
				formatter.WriteLine()
				formatter.WriteIndent()
				statementUtil.WriteBlockStatement(getMethod.Body, formatter)
				formatter.WriteOutdent()
				formatter.WriteLine()
			elif boo.Configuration['ShowMethodDeclarationBody'] == 'true':
				boo.WritePass(formatter)
		
			formatter.WriteLine() unless boo.Configuration['ShowMethodDeclarationBody'] == 'true'
		
		if setMethod != null:
			lastValueParameter as IParameterDeclaration = self.valueParameterDeclaration
			self.valueParameterDeclaration = setMethod.Parameters[setMethod.Parameters.Count - 1]
						
			if not hasSameVisibility:
				self.WriteMethodVisibility(setMethod, formatter)
			
			if not hasSameAttributes:
				self.WriteMethodAttributes(setMethod, formatter)
			
			if (boo.Configuration["ShowCustomAttributes"] == "true") and (setMethod.ReturnType.Attributes.Count != 0):
				boo.WriteCustomAttributes(setMethod.ReturnType, formatter, 'return:')
			
			if (boo.Configuration["ShowCustomAttributes"] == "true") and (self.valueParameterDeclaration.Attributes.Count != 0):
				boo.WriteCustomAttributes(self.valueParameterDeclaration, formatter, 'param:')
				
			
			if (boo.Configuration["ShowCustomAttributes"] == "true") and (setMethod.Attributes.Count != 0):
				boo.WriteCustomAttributes(setMethod, formatter, null)
			
#			formatter.WriteLine() unless setMethod.Attributes.Count > 0 or valueParameterDeclaration.Attributes.Count > 0 or setMethod.ReturnType.Attributes.Count > 0
			formatter.WriteKeyword('set')
			formatter.Write(':')
			if setMethod.Body is not null and setMethod.Body isa IBlockStatement:
				formatter.WriteLine()
				formatter.WriteIndent()
				statementUtil.WriteBlockStatement(setMethod.Body, formatter)
				formatter.WriteOutdent()
				formatter.WriteLine()
			elif boo.Configuration['ShowMethodDeclarationBody'] == 'true':
				boo.WritePass(formatter)
			
			self.valueParameterDeclaration = lastValueParameter
		
		formatter.WriteOutdent()
		formatter.WriteLine() unless boo.Configuration['ShowMethodDeclarationBody'] == 'true'
		self.WriteDeclaringType(value.DeclaringType as ITypeReference, formatter)
	


	
	def WriteSequential(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):		
		members as ArrayList = ArrayList()
		members.AddRange(value.Fields)
		members.AddRange(value.Methods)
		members.AddRange(value.Properties)
		members.AddRange(value.Events)
		for propertyDeclaration as IPropertyDeclaration in value.Properties:
			getMethod as IMethodDeclaration = null
			if propertyDeclaration.GetMethod != null:
				getMethod = propertyDeclaration.GetMethod.Resolve()
			
			setMethod as IMethodDeclaration = null
			if propertyDeclaration.SetMethod != null:
				setMethod = propertyDeclaration.SetMethod.Resolve()
			
			index as int = -1
			index = members.IndexOf(setMethod)
			if index != -1:
				members.Remove(propertyDeclaration)
				index = members.IndexOf(setMethod)
				members[index] = propertyDeclaration
			
			index = members.IndexOf(getMethod)
			if index != -1:
				members.Remove(propertyDeclaration)
				index = members.IndexOf(getMethod)
				members[index] = propertyDeclaration
			
		
		for eventDeclaration as IEventDeclaration in value.Events:
			addMethod as IMethodDeclaration = null
			if eventDeclaration.AddMethod != null:
				addMethod = eventDeclaration.AddMethod.Resolve()
			
			removeMethod as IMethodDeclaration = null
			if eventDeclaration.RemoveMethod != null:
				removeMethod = eventDeclaration.RemoveMethod.Resolve()
			
			invokeMethod as IMethodDeclaration = null
			if eventDeclaration.InvokeMethod != null:
				invokeMethod = eventDeclaration.InvokeMethod.Resolve()
			
			members.Remove(addMethod)
			members.Remove(removeMethod)
			members.Remove(invokeMethod)
		
		for memberDeclaration as IMemberDeclaration in members:
			fieldDeclaration as IFieldDeclaration = memberDeclaration as IFieldDeclaration
			if fieldDeclaration != null:
				boo.WriteFieldDeclaration(fieldDeclaration)
				formatter.WriteLine()
			
			methodDeclaration as IMethodDeclaration = memberDeclaration as IMethodDeclaration
			if methodDeclaration != null:
				WriteMethodDeclaration(methodDeclaration, formatter)
				formatter.WriteLine()
			
			propertyDeclaration as IPropertyDeclaration = memberDeclaration as IPropertyDeclaration
			if propertyDeclaration != null:
				WritePropertyDeclaration(propertyDeclaration,  formatter)
				formatter.WriteLine()
			
			eventDeclaration as IEventDeclaration = memberDeclaration as IEventDeclaration
			if eventDeclaration != null:
				WriteEventDeclaration(eventDeclaration, formatter)
				formatter.WriteLine()

	def WriteEvents(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):
		events as ICollection = information.GetEvents(boo.Configuration.Visibility, boo.Configuration)
		if events.Count > 0:
			formatter.WriteLine()
			formatter.WriteComment('# Events')
			formatter.WriteLine()
			for eventDeclaration as IEventDeclaration in events:
				WriteEventDeclaration(eventDeclaration, formatter)
				formatter.WriteLine()
	
	def WriteMethods(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):
		methods as ArrayList = ArrayList()
		methods.AddRange(information.GetMethods(boo.Configuration.Visibility, boo.Configuration))
		for i as int in range(methods.Count):
			methodDeclaration as IMethodDeclaration = methods[i]
			if boo.IsComImportConstructor(methodDeclaration, value):
				methods.RemoveAt(i)
		
		if methods.Count > 0:
			formatter.WriteLine()
			formatter.WriteComment('# Methods')
			formatter.WriteLine()
			for methodDeclaration as IMethodDeclaration in methods:
				WriteMethodDeclaration(methodDeclaration, formatter)
				formatter.WriteLine()
					
	def WriteProperties(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):
		properties as ICollection = information.GetProperties(boo.Configuration.Visibility, boo.Configuration)
		if properties.Count > 0:
			formatter.WriteLine()
			formatter.WriteComment('# Properties')
			formatter.WriteLine()
			for propertyDeclaration as IPropertyDeclaration in properties:
				WritePropertyDeclaration(propertyDeclaration, formatter)
				formatter.WriteLine() if boo.Configuration["ShowMethodDeclarationBody"] == "true"
			formatter.WriteLine() unless boo.Configuration["ShowMethodDeclarationBody"] == "true"
				
	def WriteFields(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):
		excludeFieldList as ArrayList = ArrayList()
		for eventDeclaration as IEventDeclaration in value.Events:
			fieldReference as IFieldReference = boo.GetUnderlyingFieldReference(eventDeclaration)
			if fieldReference != null:
				excludeFieldList.Add(fieldReference)
			
		fields = SCG.List of IFieldDeclaration()
		for fieldDeclaration as IFieldDeclaration in information.GetFields(boo.Configuration.Visibility, boo.Configuration):
			if (not fieldDeclaration.SpecialName) or (not fieldDeclaration.RuntimeSpecialName) or (fieldDeclaration.FieldType.Equals(value)):
				if not excludeFieldList.Contains(fieldDeclaration):
					fields.Add(fieldDeclaration)

		if fields.Count > 0:
			if information.IsEnumeration:
				fields.Sort() do(lhs as IFieldDeclaration, rhs as FieldDeclaration):
					return ((lhs.Initializer as ILiteralExpression).Value as IComparable).CompareTo((rhs.Initializer as ILiteralExpression).Value)
			formatter.WriteLine()
			formatter.WriteComment('# Fields')
			formatter.WriteLine()
#			formatter.WriteIndent() if information.IsEnumeration
			for fieldDeclaration in fields:
				boo.WriteFieldDeclaration(fieldDeclaration)
#				formatter.WriteLine()
#			formatter.WriteOutdent() if information.IsEnumeration		
	
	def WriteNestedTypes(information as TypeInformation,  value as ITypeDeclaration, formatter as IFormatter):		
		nestedTypes as ICollection = information.GetNestedTypes(boo.Configuration.Visibility, boo.Configuration)
		if nestedTypes.Count > 0:
			formatter.WriteLine()
			formatter.WriteComment('# Nested Types')
			for nestedTypeDeclaration as ITypeDeclaration in nestedTypes:
				formatter.WriteLine()
				boo.WriteTypeDeclaration(nestedTypeDeclaration)
					
	def WriteTypeMembers(information as TypeInformation, value as ITypeDeclaration, formatter as IFormatter):
		if information.IsDelegate:
			formatter.WriteLine()
		else:
			formatter.WriteIndent()
			if (boo.IsSequentialLayout(value)) or boo.IsComInterface(value):
				WriteSequential(information, value, formatter)
			else:
				WriteEvents(information, value, formatter)
				WriteMethods(information, value, formatter)				
				WriteProperties(information, value, formatter)
				WriteFields(information, value, formatter)
				WriteNestedTypes(information, value, formatter)
				
			
			formatter.WriteOutdent()
		
	def WriteDeclaringAssembly(value as IAssemblyReference, formatter as IFormatter):
		raise ArgumentNullException('value') if value is null
		
		using writer = StringWriter(CultureInfo.InvariantCulture):
			writer.Write("${value.Name}, Version=")
			if value.Version is not null:
				writer.Write(value.Version.ToString())
			else:
				writer.Write('?')
			
			formatter.WriteProperty('Assembly', writer.ToString())
		
	def WriteDeclaringType(typeReference as ITypeReference, formatter as IFormatter):
		information as TypeInformation = TypeInformation(typeReference)
		formatter.WriteProperty('Declaring Type', information.NameWithResolutionScope)
		self.WriteDeclaringAssembly(information.AssemblyName, formatter)
	
	def WriteClassDeclaration(value as ITypeDeclaration, formatter as IFormatter) as bool:
		if (value.Abstract) and (value.Sealed):
			formatter.WriteKeyword('static')
			formatter.Write(' ')
		else:
			if value.Abstract:
				formatter.WriteKeyword('abstract')
				formatter.Write(' ')
			
			if value.Sealed:
				formatter.WriteKeyword('sealed')
				formatter.Write(' ')
			
		formatter.WriteKeyword('class')
		formatter.Write(' ')
		self.WriteDeclaration(value.Name, formatter)
		boo.WriteGenericArgumentList(boo.GetNestedGenericArgumentList(value), formatter)
		baseType as ITypeReference = value.BaseType
		if (baseType != null) and (not boo.IsType(baseType, 'System', 'Object')):
			WriteBaseClassStart(baseType,formatter)
			return true
		return false
	
	def WriteStructDeclaration(value as ITypeDeclaration, formatter as IFormatter):
		formatter.WriteKeyword('struct')
		formatter.Write(' ')
		self.WriteDeclaration(value.Name, formatter)
		boo.WriteGenericArgumentList(boo.GetNestedGenericArgumentList(value), formatter)
				
	def WriteInterfaceDeclaration(value as ITypeDeclaration, formatter as IFormatter) as bool:
		formatter.WriteKeyword('interface')
		formatter.Write(' ')
		self.WriteDeclaration(value.Name, formatter)
		boo.WriteGenericArgumentList(boo.GetNestedGenericArgumentList(value), formatter)
		
	def WriteEnumDeclaration(value as ITypeDeclaration, formatter as IFormatter) as bool:
		formatter.WriteKeyword('enum')
		formatter.Write(' ')
		self.WriteDeclaration(value.Name, formatter)
		for field as IFieldDeclaration in value.Fields:
			if (field.SpecialName) and (field.RuntimeSpecialName) and (not field.FieldType.Equals(value)):
				if not boo.IsType(field.FieldType, 'System', 'Int32'):
					WriteBaseClassStart(field.FieldType, formatter)
					return true
		return false				
	
	def WriteBaseClassStart(type as ITypeReference, formatter as IFormatter):
		formatter.Write('(')
		boo.WriteType(type, formatter)
	
	def WriteBaseClassEnd(formatter as IFormatter, isDerived as bool):
		formatter.Write(')') if isDerived
		formatter.Write(':')
		formatter.WriteLine()
			
	def WriteDelegateDeclaration(value as ITypeDeclaration, formatter as IFormatter):
		information = TypeInformation(value) 
		formatter.WriteKeyword('callable')
		formatter.Write(' ')
		invokeMethod as IMethodDeclaration = information.GetMethod('Invoke')
		formatter.Write(' ')
		WriteDeclaration(value.Name, formatter)
		boo.WriteGenericArgumentList(boo.GetNestedGenericArgumentList(value), formatter)
		formatter.Write(' (')
		boo.WriteParameterDeclarationList(invokeMethod.Parameters, formatter, boo.Configuration)
		formatter.Write(')')
		boo.WriteGenericParameterConstraintList(boo.GetNestedGenericArgumentList(value), formatter)
		if not boo.IsType(invokeMethod.ReturnType.Type,"System","Void"):
			boo.WriteAs(formatter)
			boo.WriteType(invokeMethod.ReturnType.Type, formatter)
			
	
	def WriteDeclaration(name as string, formatter as IFormatter):
		if keywords.Contains(name):
			formatter.Write('@')	
		formatter.WriteDeclaration(name)
	
	def WriteVisibility(selector as TypeVisibility, formatter as IFormatter):
		if selector == TypeVisibility.Public or selector == TypeVisibility.NestedPublic:
			formatter.WriteKeyword('public')
		elif selector == TypeVisibility.Private or selector == TypeVisibility.NestedAssembly:
			formatter.WriteKeyword('internal')
		elif selector == TypeVisibility.NestedPrivate:
			formatter.WriteKeyword('private')
		elif selector == TypeVisibility.NestedFamily:
			formatter.WriteKeyword('protected')
		elif selector == TypeVisibility.NestedFamilyAndAssembly:
			formatter.WriteKeyword('protected internal')
		elif selector == TypeVisibility.NestedFamilyOrAssembly:
			formatter.WriteKeyword('internal protected')
		else:
			raise NotSupportedException()
		
	def WriteIdentifier(name as string, description as string, target as object, formatter as IFormatter):
		if keywords.Contains(name):
			formatter.Write('@')
		formatter.WriteReference(name, description, target)

	def WriteIdentifier(name as string, formatter as IFormatter):
		if keywords.Contains(name):
			formatter.Write('@')
		formatter.Write(name)
		
	def WriteInteger(formattable as IFormattable, formatter as IFormatter, asDecimal as bool):
		if asDecimal:
			formatter.WriteLiteral(formattable.ToString(null, CultureInfo.InvariantCulture))
		else:
			formatter.WriteLiteral('0x' + formattable.ToString('x', CultureInfo.InvariantCulture))
	
	def WriteChar(ch as Char, formatter as IFormatter):
		text as string = string((ch,))
		text = self.EscapeStringLiteral(text, char('\''))
		formatter.WriteKeyword("char")
		formatter.Write("(")
		formatter.WriteLiteral('\'' + text + '\'')
		formatter.Write(")")
	
	def WriteString(text as string, formatter as IFormatter):
		text = self.EscapeStringLiteral(text, char('"'))
		index as int = text.IndexOf('\\')
		if index != -1:
			index = text.Replace('\\\\', '').IndexOf('\\')
			if index == -1:
				text = text.Replace('\\\\', '\\')
				formatter.WriteLiteral('"""' + text + '"""')
				return
		formatter.WriteLiteral('"' + text + '"')
	
	def RemoveIndexerAttribute(value as ITypeDeclaration, attributes as IList):
		for i as int in range(attributes.Count):
				attribute as ICustomAttribute = attributes[i] 
				if boo.IsType(attribute.Constructor.DeclaringType, 'System.Reflection', 'DefaultMemberAttribute'):
					for propertyDeclaration as IPropertyDeclaration in value.Properties:
						if propertyDeclaration.Parameters.Count > 0:
							literalExpression as ILiteralExpression = attribute.Arguments[0] as ILiteralExpression
							if literalExpression != null:
								if propertyDeclaration.Name.Equals(literalExpression.Value):
									attributes.RemoveAt(i)
									return

		
	def EscapeStringLiteral(text as string, quote as Char) as string:
		using writer = StringWriter(CultureInfo.InvariantCulture):
			for i as int in range(text.Length):
				selector = text[i]
				if selector == char('\t'):
					writer.Write('\\t')
				elif selector ==char('\r'):
					writer.Write('\\r')
				elif selector == char('\n'):
					writer.Write('\\n')
				#char('\v') - Boo doesn't support the \v literal
				elif selector == cast(char,11):
					writer.Write('\\v')
				elif selector == char('\a'):
					writer.Write('\\a')
				elif selector == char('\b'):
					writer.Write('\\b')
				elif selector == char('\f'):
					writer.Write('\\f')
				elif selector == char('\0'):
					writer.Write('\\0')
				elif selector == char('\\'):
					writer.Write('\\\\')
				elif selector == char('"'):
					if quote == char('"'):
						writer.Write('\\"')
					else:
						writer.Write('"')
					
				elif selector == char('\''):
					if quote == char('\''):
						writer.Write('\\\'')
					else:
						writer.Write('\'')
					
				elif selector == char('\u2028') or selector == char('\u2029'):
					writer.Write('\\u' + cast(ushort, selector).ToString('x4', CultureInfo.InvariantCulture))
				else:
					value = cast(ushort,text[i])
					if (value < 32) or ((value > 126) and (value < 256)):
						writer.Write('\\x' + value.ToString('x4', CultureInfo.InvariantCulture))
					elif value > 256:
						writer.Write('\\u' + value.ToString('x4', CultureInfo.InvariantCulture))
					else:
						writer.Write(selector)
				
			return writer.ToString()	
	
			
	def IsEnumerationElement(value as IFieldDeclaration) as bool:
		fieldType as IType = value.FieldType
		declaringType as IType = value.DeclaringType
		if fieldType.Equals(declaringType):
			typeReference as ITypeReference = fieldType as ITypeReference
			if typeReference != null:
				return TypeInformation(typeReference.Resolve()).IsEnumeration
		return false
	
	keywords = [	# Reserved Words
						"abstract", "and", "as", "break", "callable", "cast", "class", "const", "constructor", "destructor", "continue", "def", "do", "elif", "else", "enum", "ensure", "event", "except", "failure", "final", "for", "from", "given", "get", "goto", "if", "interface", "in", "include", "import", "is", "isa", "mixin", "namespace", "not", "or", "otherwise", "override", "pass", "raise", "retry", "self", "struct", "return", "set", "success", "try", "transient", "virtual", "while", "when", "unless", "yield", "public", "protected", "private", "internal", "static", "bool", "string", "object", "byte", "sbyte", "short", "ushort", "char", "int", "uint", "long", "ulong", "single", "double", "decimal", "date", "timespan", "void", 
						# builtins
						"len", "__addressof__", "__eval__", "__switch__", "array", "matrix", "typeof", "assert", "print", "gets", "prompt", "enumerate", "zip", "filter", "map", "join", "cat", "iterator", "shell", "abs", 
						# standard macros and attributes
						"using", "lock", "required", "getter", "setter", "property", "checked", "unchecked", "rawArrayIndexing", "normalArrayIndexing", 
						# literals
						"false", "null", "self", "super", "true"]
