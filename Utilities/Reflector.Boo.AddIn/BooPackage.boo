namespace Reflector.Boo

import System
import System.ComponentModel
import Reflector

class BooLanguagePackage(IPackage):
	private languageManager as ILanguageManager
	
	boo as BooLanguage
	
	def Load(serviceProvider as IServiceProvider):
		boo = BooLanguage()
		languageManager = cast(ILanguageManager, serviceProvider.GetService(typeof(ILanguageManager)))
		for i in range(languageManager.Languages.Count - 1):
			if languageManager.Languages[i].Name == 'Boo':
				languageManager.UnregisterLanguage(languageManager.Languages[i])
		languageManager.RegisterLanguage(boo)
	
	def Unload():
		languageManager.UnregisterLanguage(boo)

[assembly: System.Reflection.AssemblyCompany("mailto: isurge@msn.com")]
