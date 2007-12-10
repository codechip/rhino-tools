import namespaces from "assembly://Binsor.Presentation.Framework/DefaultImport.boo"

class WellKnown:

	public static Types = (
		ICommand, 
		IView, 
		ILayout, 
		ILayoutRegistry, 
		ILayoutSelector,
		IModuleLoader, 
		IPresenter, 
		ILayoutDecoratorResolver, 
		IApplicationContext )

	public static Singletons = (
		ICommand, 
		IModuleLoader, 
		ILayoutRegistry, 
		ILayoutDecoratorResolver, 
		IApplicationContext )

	public static Assemblies = ("Binsor.Presentation.Framework", )


def InitializeContainer( *assemblies as (string) ):
	InitializeContainer( assemblies )
	
def InitializeContainer( assemblies as IEnumerable ):
	facility 'common.resolvers', AddCommonResolversFacility
	for assembly as string in cat(WellKnown.Assemblies, assemblies):		
		for type as Type in LoadAssembly(assembly).GetTypes():
			continue unless type.IsClass and not type.IsAbstract
			continue if type.IsDefined(SkipAutomaticRegistrationAttribute, false)
			ProcessType(type)
			
def ProcessType(type as Type):
	for wellKnownType in WellKnown.Types:
		continue unless wellKnownType.IsAssignableFrom(type)
		lifeStyle = LifestyleType.Transient
		lifeStyle = LifestyleType.Singleton if wellKnownType in WellKnown.Singletons
		component ExtractName(type), ExtractInterface(wellKnownType, type), type, lifeStyle
		
		
def ExtractName(service as Type):
	att as (ComponentAttribute) = service .GetCustomAttributes(ComponentAttribute, false)
	return service.Name if att.Length == 0
	return att[0].Name

def ExtractInterface(service as Type, impl as Type):
	for interfaceType in impl.GetInterfaces():
		return interfaceType if service.IsAssignableFrom(interfaceType)
	return service
	
def PrintRegisteredComponents():
	# quick way to do reflection, quack
	kernel as duck = IoC.Container.Kernel
	for handler in kernel.NamingSubSystem.GetHandlers():
		print "${handler.ComponentModel.Name} => ${handler.ComponentModel.Service}"
		
def LoadAssembly(assembly as string):
	if assembly.Contains(".dll") or assembly.Contains(".dll"):
		if Path.GetDirectoryName(assembly) == Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory):
			return Assembly.Load(Path.GetFileNameWithoutExtension(assembly))
		else: # no choice but to use the LoadFile, with the different context :-(
			return Assembly.LoadFile(assembly)
	else:
		return Assembly.Load(assembly)