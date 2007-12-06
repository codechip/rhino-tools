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

class ExpressionUtil:

	boo as BooLanguageWriter
	referenceUtil as ReferenceUtil	
	writeUtil as WriteUtil
	expressionWriters as IDictionary
	statementUtil as StatementUtil
	
	def constructor(boo as BooLanguageWriter):
		self.boo = boo
	
	def Init():
		self.referenceUtil = boo.ReferenceWriter
		self.writeUtil = boo.UtilityWriter
		self.statementUtil = boo.StatementWriter
		RegisterWriters()
		
	def WriteExpression([required] expression as IExpression, [required] formatter as IFormatter):
		writer = GetWriter(expression)
		raise ArgumentException("Invalid expression ${expression.GetType().Name}",'expression') if writer is null
		writer(expression, formatter)

	def RegisterWriters():
		expressionWriters = {}
		expressionWriters.Add(	IAssignExpression,					WriteAssignExpression)
		expressionWriters.Add(	ILiteralExpression,					WriteLiteralExpression)
		expressionWriters.Add(	ITypeOfExpression,					WriteTypeOfExpression)
		expressionWriters.Add(	IMemberInitializerExpression,		WriteMemberInitializerExpression)
		expressionWriters.Add(	IFieldReferenceExpression, 			WriteFieldReferenceExpression)
		expressionWriters.Add(	ITypeReferenceExpression, 			WriteTypeReferenceExpression)
		expressionWriters.Add(	IEventReferenceExpression,			WriteEventReferenceExpression)
		expressionWriters.Add(	IMethodReferenceExpression,			WriteMethodReferenceExpression)
		expressionWriters.Add(	IArgumentListExpression,			WriteArgumentListExpression)
		expressionWriters.Add(	IStackAllocateExpression, 			WriteStackAllocateExpression)
		expressionWriters.Add(	IPropertyReferenceExpression,		WritePropertyReferenceExpression)
		expressionWriters.Add(	IArrayCreateExpression,	 			WriteArrayCreateExpression)
#		expressionWriters.Add(	IArrayCreateExpression,	 	WriteArrayInitializerExpression)
		expressionWriters.Add(	IBaseReferenceExpression,	 		WriteBaseReferenceExpression)
		expressionWriters.Add(	IUnaryExpression,	 				WriteUnaryExpression)
		expressionWriters.Add(	IBinaryExpression,			 		WriteBinaryExpression)
		expressionWriters.Add(	ITryCastExpression,			 		WriteTryCastExpression)
		expressionWriters.Add(	ICanCastExpression,			 		WriteCanCastExpression)
		expressionWriters.Add(	ICastExpression,			 		WriteCastExpression)
		expressionWriters.Add(	IConditionExpression,		 		WriteConditionExpression)
		expressionWriters.Add(	INullCoalescingExpression,			WriteNullCoalescingExpression)
		expressionWriters.Add(	IDelegateCreateExpression,			WriteDelegateCreateExpression)
		expressionWriters.Add(	IArgumentReferenceExpression, 		WriteArgumentReferenceExpression)
		expressionWriters.Add(	IVariableDeclarationExpression, 	WriteVariableDeclarationExpression)
		expressionWriters.Add(	IVariableReferenceExpression, 		WriteVariableReferenceExpression)
		expressionWriters.Add(	IPropertyIndexerExpression, 		WritePropertyIndexerExpression)
		expressionWriters.Add(	IArrayIndexerExpression, 			WriteArrayIndexerExpression)
		expressionWriters.Add(	IMethodInvokeExpression, 			WriteMethodInvokeExpression)
		expressionWriters.Add(	IDelegateInvokeExpression, 			WriteDelegateInvokeExpression)
		expressionWriters.Add(	IObjectCreateExpression, 			WriteObjectCreateExpression)
		expressionWriters.Add(	IThisReferenceExpression, 			WriteThisReferenceExpression)
		expressionWriters.Add(	IAddressOfExpression, 				WriteAddressOfExpression)
		expressionWriters.Add(	IAddressReferenceExpression, 		WriteAddressReferenceExpression)
		expressionWriters.Add(	IAddressOutExpression, 				WriteAddressOutExpression)
		expressionWriters.Add(	IAddressDereferenceExpression, 		WriteAddressDereferenceExpression)
		expressionWriters.Add(	ISizeOfExpression, 					WriteSizeOfExpression)
		expressionWriters.Add(	IStatement, 						WriteStatementExpression)
		expressionWriters.Add(	IGenericDefaultExpression, 			WriteGenericDefaultExpression)
		expressionWriters.Add(	ITypeOfTypedReferenceExpression, 	WriteTypeOfTypedReferenceExpression)
		expressionWriters.Add(	IValueOfTypedReferenceExpression, 	WriteValueOfTypedReferenceExpression)
		expressionWriters.Add(	ITypedReferenceCreateExpression, 	WriteTypedReferenceCreateExpression)
#		expressionWriters.Add(	IObjectCreateExpression, 		WriteObjectInitializeExpression)
		expressionWriters.Add(	ISnippetExpression, 				WriteSnippetExpression)
		
	def GetWriter(expression as IExpression) as callable:
		expressionType = expression.GetType()
		for interfaceType as Type in expressionWriters.Keys:
			if interfaceType.IsAssignableFrom(expressionType):
				return expressionWriters[interfaceType]
		return null

	def WriteFieldReferenceExpression(value as IFieldReferenceExpression, formatter as IFormatter):
		if not boo.IsNullTarget(value.Target):
			self.WriteTargetExpression(value.Target, formatter)
			formatter.Write(".")
		self.WriteFieldReference(value.Field, formatter)
	
	def GetBinaryOperator(selector as BinaryOperator):
		if selector == BinaryOperator.Add:
			return '+='
		elif selector == BinaryOperator.Subtract:
			return '-='
		elif selector == BinaryOperator.Multiply:
			return'*='
		elif selector == BinaryOperator.Divide:
			return '/='
		elif selector == BinaryOperator.BitwiseOr:
			return '|='
		elif selector == BinaryOperator.BitwiseAnd:
			return '&='
		elif selector == BinaryOperator.BitwiseExclusiveOr:
			return '^='
		elif selector == BinaryOperator.Modulus:
			return '%='
		else:
			return "/*unknown operator - should never happen*/"	
			
	def WriteAssignExpression(statement as IAssignExpression, formatter as IFormatter):
		binaryExpression as IBinaryExpression = statement.Expression as IBinaryExpression
		if binaryExpression != null:
			if statement.Target.Equals(binaryExpression.Left):
				operatorText = GetBinaryOperator(binaryExpression.Operator)
				if operatorText.Length != 0:
					WriteExpression(statement.Target, formatter)
					formatter.Write(' ')
					formatter.Write(operatorText)
					formatter.Write(' ')
					WriteExpression(binaryExpression.Right, formatter)
					return
		
		WriteExpression(statement.Target, formatter)
		formatter.Write(' = ')
		brackets as bool = boo.Brackets
		boo.Brackets = false
		WriteExpression(statement.Expression, formatter)
		boo.Brackets = brackets
	
	def WriteTargetExpression(expression as IExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		boo.Brackets = true
		self.WriteExpression(expression, formatter)
		boo.Brackets = brackets
			
	def WriteLiteralExpression(expression as ILiteralExpression, formatter as IFormatter):
		if expression.Value == null:
			formatter.WriteLiteral('null')
		elif expression.Value isa Char: 
			writeUtil.WriteChar(expression.Value, formatter)
		elif expression.Value isa string:
			writeUtil.WriteString(expression.Value, formatter)
		elif expression.Value isa byte:
			valueByte = cast(byte,expression.Value)
			writeUtil.WriteInteger(valueByte, formatter, (valueByte < 16) or (((valueByte % 10) == 0) and (valueByte <= 200)))
		elif expression.Value isa sbyte:
			valueShortByte = cast(sbyte,expression.Value) 
			writeUtil.WriteInteger(valueShortByte, formatter, (valueShortByte < 16) or (((valueShortByte % 10) == 0) and (valueShortByte <= 100)))
		elif expression.Value isa short:
			valueShort = cast(short,expression.Value)
			writeUtil.WriteInteger(valueShort, formatter, (valueShort < 16) or (((valueShort % 10) == 0) and (valueShort < 1000)))
		elif expression.Value isa ushort:
			valueUnsignedShort = cast(ushort,expression.Value)
			writeUtil.WriteInteger(valueUnsignedShort, formatter, (valueUnsignedShort < 16) or (((valueUnsignedShort % 10) == 0) and (valueUnsignedShort < 1000)))
		elif expression.Value isa int:
			valueInt = cast(int,expression.Value)
			writeUtil.WriteInteger(valueInt, formatter, (valueInt < 16) or (((valueInt % 10) == 0) and (valueInt < 1000)))
		elif expression.Value isa uint:
			valueUnsignedInt = cast(uint,expression.Value) 
			writeUtil.WriteInteger(valueUnsignedInt, formatter, (valueUnsignedInt < 16) or (((valueUnsignedInt % 10) == 0) and (valueUnsignedInt < 1000)))
		elif expression.Value isa long:
			valueLong = cast(long,expression.Value) 
			writeUtil.WriteInteger(valueLong, formatter, (valueLong < 16) or (((valueLong % 10) == 0) and (valueLong < 1000)))
		elif expression.Value isa ulong:
			valueUnsignedLong = cast(ulong,expression.Value) 
			writeUtil.WriteInteger(valueUnsignedLong, formatter, (valueUnsignedLong < 16) or (((valueUnsignedLong % 10) == 0) and (valueUnsignedLong < 1000)))
		elif expression.Value isa single:
			valueSingle = cast(single,expression.Value) 
			formatter.WriteLiteral(valueSingle.ToString(CultureInfo.InvariantCulture) + 'f')
		elif expression.Value isa double:
			valueDouble = cast(double,expression.Value)
			formatter.WriteLiteral(valueDouble.ToString('R', CultureInfo.InvariantCulture))
		elif expression.Value isa decimal:
			valueDecimal = cast(decimal,expression.Value)
			formatter.WriteLiteral(valueDecimal.ToString(CultureInfo.InvariantCulture))
		elif expression.Value isa bool:
			valueBool = cast(bool,expression.Value)		
			if valueBool:
				formatter.WriteLiteral('true')
			else:
				formatter.WriteLiteral('false')
		elif expression.Value isa (byte):
			formatter.WriteComment('( ')
			bytes as (byte) = expression.Value
			if bytes.Length > 16:
				formatter.WriteLine()
				formatter.WriteIndent()
			
			for i as int in range(bytes.Length):
				formatter.Write(', ') if i != 0
				if (i % 16) == 0:
					formatter.WriteLine()
				formatter.WriteComment('0x' + bytes[i].ToString('X2', CultureInfo.InvariantCulture))
			
			if bytes.Length > 16:
				formatter.WriteOutdent()
				formatter.WriteLine()
			
			formatter.WriteComment(' )')
		else:
			raise ArgumentException('expression')	
	
	def WriteTypeOfExpression(expression as ITypeOfExpression, formatter as IFormatter):
		formatter.WriteKeyword('typeof')
		formatter.Write('(')
		boo.WriteType(expression.Type, formatter)
		typeReference = expression.Type as ITypeReference
		if (typeReference != null) and (typeReference.GenericArguments.Count > 0):
			formatter.Write('[')
			for i as int in range(typeReference.GenericArguments.Count - 1):
				formatter.Write(',')
			formatter.Write(']')
		
		formatter.Write(')')
	
	def WriteMemberInitializerExpression(expression as IMemberInitializerExpression, formatter as IFormatter):
		referenceUtil.WriteMemberReference(expression.Member as IMemberReference, formatter)
		formatter.Write(': ')
		self.WriteExpression(expression.Value, formatter)

	
	def WriteFieldReference(fieldReference as IFieldReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(fieldReference.Name, referenceUtil.ReferenceDescription.Get(fieldReference), 
			fieldReference, formatter)
	
	def WriteTypeReferenceExpression(expression as ITypeReferenceExpression, formatter as IFormatter):
		boo.WriteType(expression.Type, formatter)

	def WriteEventReferenceExpression(value as IEventReferenceExpression, formatter as IFormatter):
		if not boo.IsNullTarget(value.Target):
			self.WriteTargetExpression(value.Target, formatter)
			formatter.Write('.')
		self.WriteEventReference(value.Event, formatter)
	
	def WriteEventReference(eventReference as IEventReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(eventReference.Name, referenceUtil.ReferenceDescription.Get(eventReference), 
			eventReference, formatter)
	
	def WriteMethodReferenceExpression(value as IMethodReferenceExpression, formatter as IFormatter):
		if not boo.IsNullTarget(value.Target):
			if value.Target isa IBinaryExpression:
				formatter.Write('(')
				self.WriteExpression(value.Target, formatter)
				formatter.Write(')')
			else:
				self.WriteTargetExpression(value.Target, formatter)
			
			formatter.Write('.')
		
		self.WriteMethodReference(value.Method, formatter)
	
	def WriteMethodReference(methodReference as IMethodReference, formatter as IFormatter):
		methodInstanceReference = methodReference
		if methodInstanceReference != null:
			writeUtil.WriteIdentifier(methodReference.Name, referenceUtil.ReferenceDescription.Get(methodReference), 
				methodInstanceReference.GenericMethod, formatter)
			boo.WriteGenericArgumentList(methodInstanceReference.GenericArguments, formatter)
		else:
			writeUtil.WriteIdentifier(methodReference.Name, referenceUtil.ReferenceDescription.Get(methodReference), 
				methodReference, formatter)

	def WriteArgumentListExpression(expression as IArgumentListExpression, formatter as IFormatter):
		formatter.WriteKeyword('__arglist')
	
	def WriteStackAllocateExpression(expression as IStackAllocateExpression, formatter as IFormatter):
		formatter.WriteKeyword('stackalloc')
		formatter.Write(' ')
		formatter.Write('[')
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(']')
		boo.WriteAs(formatter)
		boo.WriteType(expression.Type, formatter)
	
	def WritePropertyReference(propertyReference as IPropertyReference, formatter as IFormatter):
		writeUtil.WriteIdentifier(propertyReference.Name, referenceUtil.ReferenceDescription.Get(propertyReference), 
			propertyReference, formatter)
	
	def WritePropertyReferenceExpression(value as IPropertyReferenceExpression, formatter as IFormatter):
		if not boo.IsNullTarget(value.Target):
			self.WriteTargetExpression(value.Target, formatter)
			formatter.Write('.')
		
		self.WritePropertyReference(value.Property, formatter)

	def WriteArrayCreateExpression(expression as IArrayCreateExpression, formatter as IFormatter):
		if expression.Initializer is not null:
			self.WriteArrayInitializerExpression(expression.Initializer, formatter)
			return
		if expression.Dimensions.Count==1:
			formatter.WriteKeyword('array')
		else:
			formatter.WriteKeyword('matrix')
		formatter.Write('(')
		self.WriteArrayElementType(expression.Type, formatter)
		formatter.Write(',')
		boo.WriteExpressionList(expression.Dimensions, formatter)
		formatter.Write(')')
		
	def WriteArrayElementType(type as IType, formatter as IFormatter):
		arrayType as IArrayType = type as IArrayType
		if arrayType != null:
			formatter.Write(' (')
			self.WriteArrayElementType(arrayType.ElementType, formatter)
			formatter.Write(') ')
		else:
			boo.WriteType(type, formatter)

	def WriteArrayInitializerExpression(expression as IBlockExpression, formatter as IFormatter):
		formatter.Write(' ( ')
		if expression.Expressions.Count > 16:
			formatter.WriteLine()
			formatter.WriteIndent()
		
		for i as int in range(expression.Expressions.Count):
			formatter.WriteLine() if i> 0 and (i % 16) == 0
			self.WriteExpression(expression.Expressions[i], formatter)
			formatter.Write(', ')
			
		
		if expression.Expressions.Count > 16:
			formatter.WriteOutdent()
			formatter.WriteLine()
		
		formatter.Write(' ) ')
	

	def WriteBaseReferenceExpression(expression as IBaseReferenceExpression, formatter as IFormatter):
		formatter.WriteKeyword('super')
	
	def WriteUnaryExpression(expression as IUnaryExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		boo.Brackets = true
		selector = expression.Operator
		if selector == UnaryOperator.BitwiseNot:
			formatter.Write('~')
			self.WriteExpression(expression.Expression, formatter)
		elif selector == UnaryOperator.BooleanNot:
			formatter.WriteKeyword('not')
			formatter.Write(' ')
			self.WriteExpression(expression.Expression, formatter)
		elif selector == UnaryOperator.Negate:
			formatter.Write('-')
			self.WriteExpression(expression.Expression, formatter)
		elif selector == UnaryOperator.PreIncrement:
			formatter.Write('++')
			self.WriteExpression(expression.Expression, formatter)
		elif selector == UnaryOperator.PreDecrement:
			formatter.Write('--')
			self.WriteExpression(expression.Expression, formatter)
		elif selector == UnaryOperator.PostIncrement:
			self.WriteExpression(expression.Expression, formatter)
			formatter.Write('++')
		elif selector == UnaryOperator.PostDecrement:
			self.WriteExpression(expression.Expression, formatter)
			formatter.Write('--')
		else:
			raise NotSupportedException(expression.Operator.ToString())
		
		boo.Brackets  = brackets
	
	def WriteBinaryExpression(expression as IBinaryExpression, formatter as IFormatter):
		if boo.Brackets:
			formatter.Write('(')
		
		brackets as bool = boo.Brackets
		boo.Brackets = true
		self.WriteExpression(expression.Left, formatter)
		formatter.Write(' ')
		self.WriteBinaryOperator(expression.Operator, formatter)
		formatter.Write(' ')
		self.WriteExpression(expression.Right, formatter)
		boo.Brackets = brackets
		if boo.Brackets:
			formatter.Write(')')
		
	
	def WriteBinaryOperator(operatorType as BinaryOperator, formatter as IFormatter):
		selector = operatorType
		if selector == BinaryOperator.Add:
			formatter.Write('+')
		elif selector == BinaryOperator.Subtract:
			formatter.Write('-')
		elif selector == BinaryOperator.Multiply:
			formatter.Write('*')
		elif selector == BinaryOperator.Divide:
			formatter.Write('/')
		elif selector == BinaryOperator.Modulus:
			formatter.Write('%')
		elif selector == BinaryOperator.ShiftLeft:
			formatter.Write('<<')
		elif selector == BinaryOperator.ShiftRight:
			formatter.Write('>>')
		elif selector == BinaryOperator.ValueInequality:
			formatter.Write('!=')
		elif selector == BinaryOperator.IdentityInequality:
			formatter.WriteKeyword('is not')
		elif selector == BinaryOperator.ValueEquality:
			formatter.Write('==')
		elif selector == BinaryOperator.IdentityEquality:
			formatter.WriteKeyword('is')
		elif selector == BinaryOperator.BitwiseOr:
			formatter.Write('|')
		elif selector == BinaryOperator.BitwiseAnd:
			formatter.Write('&')
		elif selector == BinaryOperator.BitwiseExclusiveOr:
			formatter.Write('^')
		elif selector == BinaryOperator.BooleanOr:
			formatter.WriteKeyword('or')
		elif selector == BinaryOperator.BooleanAnd:
			formatter.WriteKeyword('and')
		elif selector == BinaryOperator.LessThan:
			formatter.Write('<')
		elif selector == BinaryOperator.LessThanOrEqual:
			formatter.Write('<=')
		elif selector == BinaryOperator.GreaterThan:
			formatter.Write('>')
		elif selector == BinaryOperator.GreaterThanOrEqual:
			formatter.Write('>=')
		else:
			raise NotSupportedException(operatorType.ToString())
		
	
	def WriteTryCastExpression(expression as ITryCastExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		if brackets:
			formatter.Write('(')
		
		boo.Brackets = true
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(' ')
		formatter.WriteKeyword('as')
		formatter.Write(' ')
		boo.WriteType(expression.TargetType, formatter)
		if brackets:
			formatter.Write(')')
		
		boo.Brackets = brackets		
			
	
	def WriteCanCastExpression(expression as ICanCastExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		if brackets:
			formatter.Write('(')
		
		boo.Brackets = true
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(' ')
		formatter.WriteKeyword('isa')
		formatter.Write(' ')
		boo.WriteType(expression.TargetType, formatter)
		if brackets:
			formatter.Write(')')
		
		boo.Brackets = brackets
	
	def WriteCastExpression(expression as ICastExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		if brackets:
			formatter.Write('(')
		
		boo.Brackets = true
		formatter.WriteKeyword('cast')
		formatter.Write('(')
		boo.WriteType(expression.TargetType, formatter)
		formatter.Write(', ')
		negativeNumber  = boo.IsNegativeNumber(expression.Expression)
		if negativeNumber:
			formatter.Write('(')
		
		self.WriteExpression(expression.Expression, formatter)
		
		if negativeNumber:
			formatter.Write(')')
		
		if brackets:
			formatter.Write(')')
		
		boo.Brackets = brackets
	
		

		
	def WriteConditionExpression(value as IConditionExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		if brackets:
			formatter.Write('(')
		
		boo.Brackets = true
		formatter.WriteKeyword("iif")
		formatter.Write('(')
		self.WriteExpression(value.Condition, formatter)
		formatter.Write(', ')
		self.WriteExpression(value.Then, formatter)
		formatter.Write(', ')
		self.WriteExpression(value.Else, formatter)
		formatter.Write(')')
		if brackets:
			formatter.Write(')')
		
		boo.Brackets = brackets
	
	def WriteNullCoalescingExpression(value as INullCoalescingExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		if brackets:
			formatter.Write('(')
		
		boo.Brackets = true
		self.WriteExpression(value.Condition, formatter)
		formatter.Write(' ?? ')
		self.WriteExpression(value.Expression, formatter)
		if brackets:
			formatter.Write(')')
		
		boo.Brackets = brackets
		
	def WriteDelegateCreateExpression(expression as IDelegateCreateExpression, formatter as IFormatter):
		formatter.Write('(')
		self.WriteTargetExpression(expression.Target, formatter)
		formatter.Write('.')
		self.WriteMethodReference(expression.Method, formatter)
		formatter.Write(')')
		boo.WriteAs(formatter)
		boo.WriteType(expression.DelegateType, formatter)
		
	def WriteArgumentReferenceExpression(expression as IArgumentReferenceExpression, formatter as IFormatter):
		if (writeUtil.ValueParameterDeclaration != null) and (writeUtil.ValueParameterDeclaration == expression.Parameter.Resolve()):
			formatter.WriteKeyword('value')
		else:
			textFormatter as TextFormatter = TextFormatter()
			boo.WriteParameterDeclaration(expression.Parameter.Resolve(), textFormatter, null)
			if expression.Parameter.Name != null:
				writeUtil.WriteIdentifier(expression.Parameter.Name, textFormatter.ToString(), null, formatter)
					

	def WriteVariableDeclarationExpression(expression as IVariableDeclarationExpression, formatter as IFormatter):
		self.WriteVariableDeclaration(expression.Variable, formatter)
	
	def WriteVariableDeclaration(variableDeclaration as IVariableDeclaration, formatter as IFormatter):
		writeUtil.WriteDeclaration(variableDeclaration.Name, formatter)
		boo.WriteAs(formatter)
		boo.WriteType(variableDeclaration.VariableType, formatter)
	
	def WriteVariableReferenceExpression(expression as IVariableReferenceExpression, formatter as IFormatter):
		self.WriteVariableReference(expression.Variable, formatter)
	
	def WriteVariableReference(variableReference as IVariableReference, formatter as IFormatter):
		textFormatter as TextFormatter = TextFormatter()
		self.WriteVariableDeclaration(variableReference.Resolve(), textFormatter)
		writeUtil.WriteIdentifier(variableReference.Resolve().Name, textFormatter.ToString(), null, formatter)
	
	def WritePropertyIndexerExpression(expression as IPropertyIndexerExpression, formatter as IFormatter):
		self.WriteTargetExpression(expression.Target.Target, formatter)
		formatter.Write('[')
		boo.WriteExpressionList(expression.Indices, formatter)
		formatter.Write(']')
	
	def WriteArrayIndexerExpression(expression as IArrayIndexerExpression, formatter as IFormatter):
		self.WriteTargetExpression(expression.Target, formatter)
		formatter.Write('[')
		boo.WriteExpressionList(expression.Indices, formatter)
		formatter.Write(']')
	


	def WriteValueTypeConstructor(expression as IMethodInvokeExpression, methodReference as IMethodReference, methodReferenceExpression as IMethodReferenceExpression, formatter as IFormatter):
		self.WriteExpression(methodReferenceExpression.Target, formatter)
		formatter.Write(' = ')
		boo.WriteType(methodReference.DeclaringType, formatter)
		formatter.Write('(')
		boo.WriteExpressionList(expression.Arguments, formatter)
		formatter.Write(')')
	
	def WriteDelegateInvocation(expression as IMethodInvokeExpression,methodReference as IMethodReference, methodReferenceExpression as IMethodReferenceExpression, formatter as IFormatter):
		brackets as bool = boo.Brackets
		boo.Brackets = true
		self.WriteExpression(methodReferenceExpression.Target, formatter)
		formatter.Write('(')
		boo.WriteExpressionList(expression.Arguments, formatter)
		formatter.Write(')')
		boo.Brackets = brackets
	
	def WriteStringConcentration(expression as IMethodInvokeExpression,methodReference as IMethodReference, formatter as IFormatter):
		stringOnly as bool = ((expression.Arguments.Count == methodReference.Parameters.Count) or (methodReference.CallingConvention == MethodCallingConvention.VariableArguments))
		if stringOnly:
			for parameter as IParameterDeclaration in methodReference.Parameters:
				if (not boo.IsType(parameter.ParameterType, 'System', 'String')) and (not boo.IsType(parameter.ParameterType, 'System', 'Object')):
					stringOnly = false
		
		if stringOnly:
			brackets as bool = boo.Brackets
			if brackets:
				formatter.Write('(')
			
			for i as int in range(expression.Arguments.Count):
				formatter.Write(' + ') if i != 0
				boo.Brackets = true
				self.WriteExpression(expression.Arguments[i], formatter)
				
			
			if brackets:
				formatter.Write(')')
			
			boo.Brackets = brackets
	
	def WriteMethodInvokeExpression(expression as IMethodInvokeExpression, formatter as IFormatter):
		methodReferenceExpression as IMethodReferenceExpression = expression.Method as IMethodReferenceExpression
		if methodReferenceExpression != null:
			methodReference as IMethodReference = methodReferenceExpression.Method
			if (methodReference.Name == '.ctor') and (TypeInformation(methodReference.DeclaringType).IsValueType):
				WriteValueTypeConstructor(expression, methodReference, methodReferenceExpression, formatter)
				return
			
			if (methodReference.Name == 'Invoke') and (TypeInformation(methodReference.DeclaringType).IsDelegate):
				WriteDelegateInvocation(expression, methodReference, methodReferenceExpression, formatter)
				return
			
			if (methodReference.Name == 'Concat') and (boo.IsType(methodReference.DeclaringType, 'System', 'String')):
				WriteStringConcentration(expression, methodReference, formatter)
				return
				
			
			self.WriteMethodReferenceExpression(methodReferenceExpression, formatter)
			formatter.Write('(')
			boo.WriteExpressionList(expression.Arguments, formatter)
			formatter.Write(')')
			return
			
		formatter.WriteComment("/* using function pointer*/ ")
		formatter.Write('*')
		self.WriteExpression(expression.Method, formatter)
		formatter.Write('(')
		boo.WriteExpressionList(expression.Arguments, formatter)
		formatter.Write(')')
	
	
	def WriteDelegateInvokeExpression(expression as IDelegateInvokeExpression, formatter as IFormatter):
		self.WriteTargetExpression(expression.Target, formatter) if expression.Target != null
		
		formatter.Write('(')
		boo.WriteExpressionList(expression.Arguments, formatter)
		formatter.Write(')')
	
	def WriteObjectCreateExpression(expression as IObjectCreateExpression, formatter as IFormatter):
		formatter.Write(' ')
		boo.WriteTypeReference(expression.Constructor.DeclaringType as ITypeReference, formatter, 
			referenceUtil.ReferenceDescription.Get(expression.Constructor), expression.Constructor)
		formatter.Write('(')
		boo.WriteExpressionList(expression.Arguments, formatter)
		formatter.Write(')')
	
	def WriteThisReferenceExpression(expression as IThisReferenceExpression, formatter as IFormatter):
		formatter.WriteKeyword('self')
	
	def WriteAddressOfExpression(expression as IAddressOfExpression, formatter as IFormatter):
		brackets as bool = (expression.Expression isa IArrayIndexerExpression)
		formatter.WriteKeyword('__addressof__')
		if brackets:
			formatter.Write('(')
		
		self.WriteExpression(expression.Expression, formatter)
		if brackets:
			formatter.Write(')')
	
	def WriteAddressReferenceExpression(expression as IAddressReferenceExpression, formatter as IFormatter):
		formatter.WriteComment('/*ref*/')
		formatter.Write(' ')
		self.WriteExpression(expression.Expression, formatter)
	
	def WriteAddressOutExpression(expression as IAddressOutExpression, formatter as IFormatter):
		formatter.WriteComment('/*out*/')
		formatter.Write(' ')
		self.WriteExpression(expression.Expression, formatter)
	
	def WriteAddressDereferenceExpression(expression as IAddressDereferenceExpression, formatter as IFormatter):
		formatter.Write('*(')
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(')')
	
	def WriteSizeOfExpression(expression as ISizeOfExpression, formatter as IFormatter):
		formatter.WriteKeyword('sizeof')
		formatter.Write('(')
		boo.WriteType(expression.Type, formatter)
		formatter.Write(')')
	
	def WriteStatementExpression(expression as IStatement, formatter as IFormatter):
		brackets as bool = boo.Brackets
		statementLineBreak = statementUtil.LineBreak
		statementUtil.LineBreak = false
		boo.Brackets = false
		if brackets:
			formatter.Write('(')
		
		statementUtil.WriteStatement(expression, formatter)
		if brackets:
			formatter.Write(')')
		
		statementUtil.LineBreak = statementLineBreak
		boo.Brackets = brackets	

	def WriteGenericDefaultExpression(expression as IGenericDefaultExpression, formatter as IFormatter):
		formatter.WriteComment('/* not implemented */')
		formatter.WriteKeyword('default')
		formatter.Write('(')
		boo.WriteType(expression.GenericArgument, formatter)
		formatter.Write(')')
	
		
	def WriteTypeOfTypedReferenceExpression(expression as ITypeOfTypedReferenceExpression, formatter as IFormatter):
		formatter.WriteComment('/*__reftype*/')
		formatter.Write('(')
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(')')
	
	def WriteValueOfTypedReferenceExpression(value as IValueOfTypedReferenceExpression, formatter as IFormatter):
		formatter.WriteComment('/*__refvalue*/')
		formatter.Write('(')
		self.WriteExpression(value.Expression, formatter)
		formatter.Write(', ')
		boo.WriteType(value.TargetType, formatter)
		formatter.Write(')')
	
	def WriteTypedReferenceCreateExpression(expression as ITypedReferenceCreateExpression, formatter as IFormatter):
		formatter.WriteComment('/*__makeref*/')
		formatter.Write('(')
		self.WriteExpression(expression.Expression, formatter)
		formatter.Write(')')

	def WriteObjectInitializeExpression(value as IObjectCreateExpression, formatter as IFormatter):
		genericArgument as IGenericArgument = value.Type as IGenericArgument
		if genericArgument != null:
			formatter.WriteComment('/* not implemented */')
			formatter.WriteKeyword('default')
			formatter.Write('(')
			boo.WriteType(value.Type, formatter)
			formatter.Write(')')
		else:
			formatter.Write(' ')
			boo.WriteType(value.Type, formatter)
			formatter.Write('()')
		
	def WriteSnippetExpression(expression as ISnippetExpression, formatter as IFormatter):
		formatter.WriteComment(expression.Value)
	



