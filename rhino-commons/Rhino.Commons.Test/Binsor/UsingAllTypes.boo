import Rhino.Commons.Test.Binsor from Rhino.Commons.Test

for type in AllTypesBased of IView("Rhino.Commons.Test"):
	component type
	
for type in AllTypesWithAttribute of ControllerAttribute("Rhino.Commons.Test"):
	component type
	
for type in AllTypes("Rhino.Commons.Test").WhereNamespaceEq("Rhino.Commons.Test.Binsor"):
	if not Kernel.HasComponent(type):  
		component type

for type in AllTypes("Rhino.Commons.NHibernate") \
	.Where({ t as System.Type | t.Name.Contains("NHRepository") }):
	component "nh.repos", type.GetFirstInterface(), type

for type in AllTypes("Rhino.Commons.Test") \
	.Where({ t as System.Type | t.Name == "Fubar54" }):
	component "foo54", type.GetFirstInterface({ t as System.Type | t.Name == "IFoo54" }), type