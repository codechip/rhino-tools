namespace Reflector.Boo

import System
import System.IO
import System.Collections
import System.Globalization
import Reflector
import Reflector.CodeModel
import Reflector.CodeModel.Memory

class StatementUtil:

	boo as BooLanguageWriter
	writeUtil as WriteUtil
	statementWriters as IDictionary
	expressionWriter as ExpressionUtil
	
	[Property(LineBreak)]
	statementLineBreak as bool
	
	def constructor(boo as BooLanguageWriter):
		self.boo = boo
	
	def Init():
		self.writeUtil = boo.UtilityWriter
		self.expressionWriter = boo.ExpressionWriter
		RegisterWriters()
	
	def WriteStatement(statement as IStatement, formatter as IFormatter):
		raise ArgumentNullException('statement') if statement is null
		raise ArgumentNullException('formatter') if formatter is null
		
		writer as callable = GetWriter(statement)
		raise ArgumentException("Invalid statement ${statement.GetType().Name}",'statement') if writer is null
	
		writer(statement, formatter)
	
	def GetWriter(statement as IStatement):
		statementType = statement.GetType()
		for interfaceType as Type in statementWriters.Keys:
			if interfaceType.IsAssignableFrom(statementType):
				return statementWriters[interfaceType]
		return null

	def RegisterWriters():
		statementWriters = {}
		statementWriters.Add(	IBlockStatement,				WriteBlockStatement)
		statementWriters.Add(	IExpressionStatement,			WriteExpressionStatement)
		statementWriters.Add(	IGotoStatement,					WriteGotoStatement)
		statementWriters.Add(	ILabeledStatement,				WriteLabeledStatement)
		statementWriters.Add(	IConditionStatement,				WriteConditionStatement)
		statementWriters.Add(	IMethodReturnStatement,			WriteMethodReturnStatement)
		statementWriters.Add(	IForStatement,					WriteForStatement)
		statementWriters.Add(	IForEachStatement,				WriteForEachStatement)
		statementWriters.Add(	IUsingStatement,					WriteUsingStatement)
		statementWriters.Add(	IFixedStatement,					WriteFixedStatement)
		statementWriters.Add(	IWhileStatement,					WriteWhileStatement)
		statementWriters.Add(	IDoStatement,					WriteDoStatement)
		statementWriters.Add(	ITryCatchFinallyStatement,		WriteTryCatchFinallyStatement)
		statementWriters.Add(	IThrowExceptionStatement,		WriteThrowExceptionStatement)
		statementWriters.Add(	IBreakStatement,					WriteBreakStatement)
		statementWriters.Add(	IContinueStatement,				WriteContinueStatement)
		statementWriters.Add(	ICommentStatement,				WriteCommentStatement)
		statementWriters.Add(	IAttachEventStatement,			WriteAttachEventStatement)
		statementWriters.Add(	IRemoveEventStatement,			WriteRemoveEventStatement)
		statementWriters.Add(	ISwitchStatement,				WriteSwitchStatement)
		statementWriters.Add(	ILockStatement,					WriteLockStatement)
		statementWriters.Add(	IMemoryCopyStatement,			WriteMemoryCopyStatement)
		statementWriters.Add(	IMemoryInitializeStatement,		WriteMemoryInitializeStatement)
	
	
	def WriteBlockStatement(statement as IBlockStatement, formatter as IFormatter):
		if statement.Statements.Count > 0:
			for statment as IStatement in statement.Statements:
				self.WriteStatement(statment, formatter)	
	
	def WriteExpressionStatement(statement as IExpressionStatement, formatter as IFormatter):
		expressionWriter.WriteExpression(statement.Expression, formatter)
		if self.statementLineBreak:
			formatter.WriteLine()
	
	def WriteGotoStatement(statement as IGotoStatement, formatter as IFormatter):
		formatter.WriteKeyword('goto ')
		formatter.Write(statement.Name)
		formatter.WriteComment('# goto is considered harmful :-)')
		formatter.WriteLine()
	
	def WriteLabeledStatement(statement as ILabeledStatement, formatter as IFormatter):
		formatter.WriteOutdent()
		writeUtil.WriteDeclaration(statement.Name, formatter)
		formatter.Write(':')
		if statement.Statement != null:
			formatter.WriteLine()
			formatter.WriteIndent()
			self.WriteStatement(statement.Statement, formatter)
		else:
			formatter.WriteIndent()
			formatter.WriteLine()

	def WriteConditionalStatementFragement(condition as IExpression, formatter as IFormatter):
		formatter.Write(' ')
		brackets  = boo.Brackets
		boo.Brackets = false
		expressionWriter.WriteExpression(condition, formatter)
		boo.Brackets = brackets
		formatter.Write(':')
		

	def WriteConditionStatement(statement as IConditionStatement, formatter as IFormatter):
		formatter.WriteKeyword('if')
		WriteConditionalStatementFragement(statement.Condition, formatter)
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Then != null:
			self.WriteStatement(statement.Then, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
		while (statement.Else != null) and (statement.Else.Statements.Count > 0):
			elseStatement as IConditionStatement = statement.Else.Statements[0] as IConditionStatement
			if (elseStatement != null) and (statement.Else.Statements.Count == 1):
				formatter.WriteKeyword('elif')
				WriteConditionalStatementFragement(elseStatement.Condition, formatter)
				formatter.WriteLine()
				formatter.WriteIndent()
				self.WriteStatement(elseStatement.Then, formatter)
				formatter.WriteOutdent()
				formatter.WriteLine()
				statement = elseStatement
			else:
				formatter.WriteKeyword('else')
				formatter.Write(':')
				formatter.WriteLine()
				formatter.WriteIndent()
				if statement.Else != null:
					self.WriteStatement(statement.Else, formatter)
				formatter.WriteOutdent()
				formatter.WriteLine()
				break
	
	def WriteMethodReturnStatement(statement as IMethodReturnStatement, formatter as IFormatter):
		formatter.WriteKeyword('return')
		if statement.Expression != null:
			formatter.Write(' ')
			brackets as bool = boo.Brackets
			castExpression as ICastExpression = statement.Expression as ICastExpression
			boo.Brackets = false if castExpression != null	
			expressionWriter.WriteExpression(statement.Expression, formatter)
			boo.Brackets = brackets
		
		formatter.WriteLine()
	


			
				
	def WriteForStatement(statement as IForStatement, formatter as IFormatter):
		if statement.Increment != null:
			WriteStatement(statement.Increment, formatter)
		
		formatter.WriteKeyword('while')
		if statement.Condition != null:
			brackets = boo.Brackets
			boo.Brackets = false
			expressionWriter.WriteExpression(statement.Condition, formatter)
			boo.Brackets = brackets
			formatter.WriteLine()
		else:
			formatter.Write(" ")
			formatter.WriteKeyword("true")
			formatter.Write(":")
			formatter.WriteLine()
			
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteStatement(statement.Body, formatter)
		
		if statement.Increment != null:
			self.WriteStatement(statement.Increment, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
		
	def WriteForEachStatement(statement as IForEachStatement, formatter as IFormatter):
		oldStatementLineBreak as bool = self.statementLineBreak
		self.statementLineBreak = false
		formatter.WriteKeyword('for')
		formatter.Write(' ')
		expressionWriter.WriteVariableDeclaration(statement.Variable, formatter)
		formatter.Write(' ')
		formatter.WriteKeyword('in')
		formatter.Write(' ')
		expressionWriter.WriteExpression(statement.Expression, formatter)
		formatter.Write(':')
		self.statementLineBreak = oldStatementLineBreak
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteBlockStatement(statement.Body, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
	


	
	def WriteUsingStatement(statement as IUsingStatement, formatter as IFormatter):
		oldStatementLineBreak = self.statementLineBreak
		self.statementLineBreak = false
		formatter.WriteKeyword('using')
		formatter.Write(' ')
		expressionWriter.WriteVariableDeclaration((statement.Variable as IVariableDeclarationExpression).Variable, formatter)
		formatter.Write(' ')
		formatter.WriteKeyword('=')
		formatter.Write(' ')
		expressionWriter.WriteExpression(statement.Expression, formatter)
		formatter.Write(':')
		self.statementLineBreak = oldStatementLineBreak
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteBlockStatement(statement.Body, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()

	def WriteFixedStatement(statement as IFixedStatement, formatter as IFormatter):
		oldStatementLineBreak as bool = self.statementLineBreak
		self.statementLineBreak = false
		formatter.WriteKeyword('fixed')
		formatter.Write(' ')
		expressionWriter.WriteVariableDeclaration(statement.Variable, formatter)
		formatter.Write(' ')
		formatter.WriteKeyword('=')
		formatter.Write(' ')
		expressionWriter.WriteExpression(statement.Expression, formatter)
		formatter.Write(':')
		self.statementLineBreak = oldStatementLineBreak
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteBlockStatement(statement.Body, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
	
	def WriteWhileStatement(statement as IWhileStatement, formatter as IFormatter):
		formatter.WriteKeyword('while')
		formatter.Write(' ')
		if statement.Condition != null:
			brackets = boo.Brackets
			boo.Brackets = false
			expressionWriter.WriteExpression(statement.Condition, formatter)
			boo.Brackets = brackets
		else:
			formatter.WriteLiteral('true')
		
		formatter.Write(':')
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteStatement(statement.Body, formatter)
	
		formatter.WriteOutdent()
		formatter.WriteLine()
	
	def WriteDoStatement(statement as IDoStatement, formatter as IFormatter):
		formatter.WriteKeyword('while')
		formatter.Write(' ')
		formatter.WriteLiteral('true')
		formatter.Write(':')	
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteStatement(statement.Body, formatter)
		formatter.WriteLine()
		formatter.WriteKeyword('if')
		formatter.Write(' ')
		formatter.WriteKeyword('not')
		formatter.Write(' ')
		if statement.Condition != null:
			brackets as bool = boo.Brackets
			boo.Brackets = false
			expressionWriter.WriteExpression(statement.Condition, formatter)
			boo.Brackets = brackets
		else:
			formatter.WriteLiteral('true')
		formatter.WriteIndent()
		formatter.WriteLine()
		formatter.WriteKeyword('break')
		formatter.WriteLine()
		formatter.WriteOutdent()
		formatter.WriteLine()
		formatter.WriteOutdent()
		formatter.WriteLine()
		
	def WriteTryCatchFinallyStatement(statement as ITryCatchFinallyStatement, formatter as IFormatter):
		needFinally = true
		formatter.WriteKeyword('try')
		formatter.Write(':')
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Try != null:
			self.WriteStatement(statement.Try, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
		for catchClause as ICatchClause in statement.CatchClauses:
			needFinally = false
			formatter.WriteKeyword('except')
			catchType as IType = catchClause.Variable.VariableType
			hiddenName as bool = boo.IsEmptyCatchClauseName(catchClause)
			hiddenType as bool = boo.IsType(catchType,"System","Object") or boo.IsType(catchType,"System","Exception")
			if (not hiddenName) or (not hiddenType):
				formatter.Write(' ')
				if not hiddenName:
					writeUtil.WriteDeclaration(catchClause.Variable.Name, formatter)
				if not hiddenType:
					boo.WriteAs(formatter)
					boo.WriteType(catchType, formatter)
			
			
			if catchClause.Condition != null:
				formatter.Write(' ')
				formatter.WriteKeyword('when')
				formatter.Write(' ')
				expressionWriter.WriteExpression(catchClause.Condition, formatter)
				
			formatter.Write(':')
			formatter.WriteLine()
			formatter.WriteIndent()
			if catchClause.Body != null:
				self.WriteStatement(catchClause.Body, formatter)
			
			formatter.WriteOutdent()
			formatter.WriteLine()
		
		if (statement.Fault != null) and (statement.Fault.Statements.Count > 0):
			needFinally = false
			formatter.WriteKeyword('fault')
			formatter.Write(':')
			formatter.WriteLine()
			formatter.WriteIndent()
			if statement.Fault != null:
				self.WriteStatement(statement.Fault, formatter)
			
			formatter.WriteOutdent()
			formatter.WriteLine()
		
		if (needFinally) or ((statement.Finally != null) and (statement.Finally.Statements.Count > 0)):
			formatter.WriteKeyword('ensure')
			formatter.Write(':')
			formatter.WriteLine()
			formatter.WriteIndent()
			if statement.Finally != null:
				self.WriteStatement(statement.Finally, formatter)
			
			formatter.WriteOutdent()
			formatter.WriteLine()
		
	def WriteThrowExceptionStatement(statement as IThrowExceptionStatement, formatter as IFormatter):
		formatter.WriteKeyword('raise')
		if statement.Expression != null:
			formatter.Write(' ')
			expressionWriter.WriteExpression(statement.Expression, formatter)
		
		formatter.WriteLine()
	
	def WriteBreakStatement(statement as IBreakStatement, formatter as IFormatter):
		formatter.WriteKeyword('break')
		formatter.WriteLine()
	
	def WriteContinueStatement(statement as IContinueStatement, formatter as IFormatter):
		formatter.WriteKeyword('continue')
		formatter.WriteLine()
	





		
	def WriteCommentStatement(statement as ICommentStatement, formatter as IFormatter):
		self.WriteComment(statement.Comment, formatter)
	
	def WriteComment(comment as IComment, formatter as IFormatter):
		parts as (string) = comment.Text.Split(char('\n'))
		for part as string in parts:
			formatter.WriteComment('# ')
			formatter.WriteComment(part)
			formatter.WriteLine()

	def WriteAttachEventStatement(statement as IAttachEventStatement, formatter as IFormatter):
		expressionWriter.WriteEventReferenceExpression(statement.Event, formatter)
		formatter.Write(' += ')
		expressionWriter.WriteExpression(statement.Listener, formatter)
		formatter.WriteLine()
	
	def WriteRemoveEventStatement(statement as IRemoveEventStatement, formatter as IFormatter):
		expressionWriter.WriteEventReferenceExpression(statement.Event, formatter)
		formatter.Write(' -= ')
		expressionWriter.WriteExpression(statement.Listener, formatter)
		formatter.WriteLine()
	

	def WriteSwitchStatement(statement as ISwitchStatement, formatter as IFormatter):
		formatter.WriteKeyword('given')
		formatter.Write(' ')
		expressionWriter.WriteExpression(statement.Expression, formatter)
		formatter.Write(':')
		formatter.WriteLine()
		formatter.WriteIndent()
		for switchCase as ISwitchCase in statement.Cases:
			conditionCase as IConditionCase = switchCase as IConditionCase
			if conditionCase != null:
				self.WriteSwitchCaseCondition(conditionCase.Condition, formatter)
				formatter.WriteLine()
				formatter.WriteIndent()
				if conditionCase.Body != null:
					self.WriteStatement(conditionCase.Body, formatter)
				
				formatter.WriteOutdent()
				formatter.WriteLine()
			
			defaultCase as IDefaultCase = switchCase as IDefaultCase
			if defaultCase != null:
				formatter.WriteKeyword('default')
				formatter.Write(':')
				formatter.WriteLine()
				formatter.WriteIndent()
				if defaultCase.Body != null:
					self.WriteStatement(defaultCase.Body, formatter)
				
				formatter.WriteOutdent()
				formatter.WriteLine()
			
		
		formatter.WriteOutdent()
		formatter.WriteLine()
	
	def WriteSwitchCaseCondition(condition as IExpression, formatter as IFormatter):
		binaryExpression as IBinaryExpression = condition as IBinaryExpression
		if (binaryExpression != null) and (binaryExpression.Operator == BinaryOperator.BooleanOr):
			self.WriteSwitchCaseCondition(binaryExpression.Left, formatter)
			self.WriteSwitchCaseCondition(binaryExpression.Right, formatter)
		else:
			formatter.WriteKeyword('given')
			formatter.Write(' ')
			expressionWriter.WriteExpression(condition, formatter)
			formatter.Write(':')
			formatter.WriteLine()
		
	


	
		
	def WriteLockStatement(statement as ILockStatement, formatter as IFormatter):
		formatter.WriteKeyword('lock')
		formatter.Write(' ')
		expressionWriter.WriteExpression(statement.Expression, formatter)
		formatter.Write(':')
		formatter.WriteLine()
		formatter.WriteIndent()
		if statement.Body != null:
			self.WriteBlockStatement(statement.Body, formatter)
		
		formatter.WriteOutdent()
		formatter.WriteLine()
	



	def WriteMemoryCopyStatement(statement as IMemoryCopyStatement, formatter as IFormatter):
		formatter.WriteKeyword('memcpy')
		formatter.Write('(')
		expressionWriter.WriteExpression(statement.Source, formatter)
		formatter.Write(', ')
		expressionWriter.WriteExpression(statement.Destination, formatter)
		formatter.Write(', ')
		expressionWriter.WriteExpression(statement.Length, formatter)
		formatter.Write(')')
		formatter.WriteLine()
	
	def WriteMemoryInitializeStatement(statement as IMemoryInitializeStatement, formatter as IFormatter):
		formatter.WriteComment('/*mem-init*/')
		formatter.Write('(')
		expressionWriter.WriteExpression(statement.Offset, formatter)
		formatter.Write(', ')
		expressionWriter.WriteExpression(statement.Value, formatter)
		formatter.Write(', ')
		expressionWriter.WriteExpression(statement.Length, formatter)
		formatter.Write(')')
		formatter.WriteLine()
