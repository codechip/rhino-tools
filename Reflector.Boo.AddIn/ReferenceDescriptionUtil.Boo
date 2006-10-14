namespace Reflector.Boo

import System
import System.IO
import System.Collections
import System.Globalization
import Reflector
import Reflector.CodeModel
import Reflector.CodeModel.Memory

class ReferenceDescriptionUtil:
	
	boo as BooLanguageWriter
	
	def constructor(boo as BooLanguageWriter):
		self.boo = boo
	
	def Get(fieldReference as IFieldReference) as string:
		formatter as IFormatter = TextFormatter()
		boo.WriteType(fieldReference.FieldType, formatter)
		formatter.Write(' ')
		formatter.Write(self.Get(fieldReference.DeclaringType as ITypeReference))
		formatter.Write('.')
		formatter.WriteDeclaration(fieldReference.Name)
		return formatter.ToString()
	
	def Get(typeReference as ITypeReference) as string:
		return TypeInformation(typeReference).NameWithResolutionScope
		
	def Get(value as IMethodReference) as string:
		formatter as IFormatter = TextFormatter()
		if boo.IsConstructor(value):
			formatter.Write(self.Get(value.DeclaringType as ITypeReference))
			formatter.Write('.')
			formatter.WriteKeyword("constructor")
		else:
			formatter.WriteKeyword("def")
			formatter.Write(' ')
			formatter.Write(TypeInformation(value.DeclaringType as ITypeReference).NameWithResolutionScope)
			formatter.Write('.')
			formatter.Write(value.Name)
		
		boo.WriteGenericArgumentList(value.GenericArguments, formatter)
		formatter.Write('(')
		boo.WriteParameterDeclarationList(value.Parameters, formatter, null)
		if value.CallingConvention == MethodCallingConvention.VariableArguments:
			if value.Parameters.Count > 0:
				formatter.Write(', ')
			
			formatter.WriteKeyword('__arglist')
		
		formatter.Write(')')
		
		if not boo.IsConstructor(value) and not boo.IsType(value.ReturnType.Type,"System","Void"):
			boo.WriteAs(formatter)
			boo.WriteType(value.ReturnType.Type, formatter)
			
		
		return formatter.ToString()
		
	def Get(propertyReference as IPropertyReference) as string:
		formatter as IFormatter = TextFormatter()
		propertyName as string = propertyReference.Name
		if propertyName == 'Item':
			propertyName = 'self'
		
		formatter.Write(self.Get(propertyReference.DeclaringType as ITypeReference))
		formatter.Write('.')
		formatter.WriteDeclaration(propertyName)
		parameters as IParameterDeclarationCollection = propertyReference.Parameters
		if parameters.Count > 0:
			formatter.Write('[')
			boo.WriteParameterDeclarationList(parameters, formatter, null)
			formatter.Write(']')
		
		boo.WriteAs(formatter)
		boo.WriteType(propertyReference.PropertyType, formatter)
		formatter.WriteLine()
		formatter.WriteKeyword('pass')
		return formatter.ToString()
	
	def Get(eventReference as IEventReference) as string:
		formatter as IFormatter = TextFormatter()
		formatter.WriteKeyword('event')
		formatter.Write(' ')
		formatter.Write(self.Get(eventReference.DeclaringType as ITypeReference))
		formatter.Write('.')
		formatter.WriteDeclaration(eventReference.Name)
		boo.WriteAs(formatter)
		boo.WriteType(eventReference.EventType, formatter)
		return formatter.ToString()
	
