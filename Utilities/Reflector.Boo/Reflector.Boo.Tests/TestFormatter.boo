namespace Reflector.Boo.Tests

import Reflector.CodeModel
import System
import System.IO
import System.Globalization

class TestFormatter(IFormatter):
"""Description of TestFormatter"""
	
	[property(AllowProperties)]
	allowProperties = false
	newLine = false
	indent = 0
	
	writer = StringWriter(CultureInfo.InvariantCulture)
		
	private def ApplyIndent():
		if self.newLine:
			for num1 in range(self.indent):
				self.writer.Write('	')
			self.newLine = false
	
	public def Write(value as string):
		self.ApplyIndent()
		self.writer.Write(value)
	
	public def WriteKeyword(value as string):
		self.ApplyIndent()
		self.writer.Write("K:")
		self.writer.Write(value)
	
	public def WriteLiteral(value as string):
		self.ApplyIndent()
		self.writer.Write("L:")
		self.writer.Write(value)
	
	public def WriteComment(value as string):
		self.ApplyIndent()
		self.writer.Write("C:")
		self.writer.Write(value)
	
	public def WriteDeclaration(value as string):
		self.ApplyIndent()
		self.writer.Write("D:")
		self.writer.Write(value)
	
	public def WriteDeclaration(value as string, target as object):
		self.WriteDeclaration(value)
	
	public def WriteReference(value as string, description as string, target as object):
		self.ApplyIndent()
		self.writer.Write("R:")
		self.writer.Write(value)
	
	public def WriteIndent():
		self.indent += 1
	
	public def WriteOutdent():
		self.indent -= 1
	
	public def WriteLine():
		self.writer.WriteLine()
		self.newLine = true
	
	public def WriteProperty(name as string, value as string):
		if self.AllowProperties:
			raise NotSupportedException()

		#self.ApplyIndent()
		#self.writer.Append("P:")
		#self.writer.Append(value)


	override def ToString():
		return writer.ToString()
