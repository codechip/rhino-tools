import Rhino.Commons.Test.Binsor from Rhino.Commons.Test

for type in AllTypesBased of IView("Rhino.Commons.Test"):
	component type
	
for type in AllTypesWithAttribute of ControllerAttribute("Rhino.Commons.Test"):
	component type