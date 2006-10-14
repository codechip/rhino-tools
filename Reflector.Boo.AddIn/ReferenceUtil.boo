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
		
	

