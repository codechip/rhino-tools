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
	
