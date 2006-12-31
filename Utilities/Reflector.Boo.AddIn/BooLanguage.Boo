namespace Reflector.Boo
import System
import Reflector
import Reflector.CodeModel

class BooLanguage(ILanguage):
"""Implement a ILanguage interface for Boo"""
	
	[Getter(Name)]
	name = "Boo"
	
	[Getter(FileExtension)]
	fileExtention = ".boo"
	
	[Getter(Translate)]
	translate = true
	
	def GetWriter(formatter as IFormatter, configuration as ILanguageWriterConfiguration) as ILanguageWriter:
		return BooLanguageWriter(formatter, configuration)
