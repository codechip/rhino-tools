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

class ReferenceUtil:
	
	boo as BooLanguageWriter
	[Property(ReferenceDescription)]
	referenceDesc as ReferenceDescriptionUtil
	writeUtil as WriteUtil
	
	def constructor(boo as BooLanguageWriter):
		self.boo = boo
		
	def Init():
		referenceDesc = ReferenceDescriptionUtil(boo)
		self.writeUtil = boo.UtilityWriter 
	
	def WriteMemberReference(memberReference as IMemberReference, formatter as IFormatter):
		if memberReference isa IFieldReference :
			self.WriteFieldReference(memberReference, formatter)
		
		if memberReference isa  IMethodReference:
			self.WriteMethodReference(memberReference, formatter)
		
		if memberReference isa  IPropertyReference:
			self.WritePropertyReference(memberReference, formatter)
		
		if memberReference isa  IEventReference:
			self.WriteEventReference(memberReference, formatter)
	
	def WritePropertyReference(propertyReference as IPropertyReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(propertyReference.Name, referenceDesc.Get(propertyReference), propertyReference, 
			formatter)
	
	def WriteEventReference(eventReference as IEventReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(eventReference.Name, referenceDesc.Get(eventReference), eventReference, 
			formatter)
	
	def WriteFieldReference(fieldReference as IFieldReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(fieldReference.Name, referenceDesc.Get(fieldReference), 
			fieldReference, formatter)
	
	def WriteMethodReference(methodReference as IMethodReference, formatter as IFormatter):
		methodInstanceReference = methodReference
		if methodInstanceReference != null:
			writeUtil.WriteIdentifier(methodReference.Name, referenceDesc.Get(methodReference), 
				methodInstanceReference.GenericMethod, formatter)
			boo.WriteGenericArgumentList(methodInstanceReference.GenericArguments, formatter)
		else:
			writeUtil.WriteIdentifier(methodReference.Name, referenceDesc.Get(methodReference),
				methodReference, formatter)
		
	

