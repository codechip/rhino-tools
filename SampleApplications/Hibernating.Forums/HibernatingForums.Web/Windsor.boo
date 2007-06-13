import System.Reflection
import Castle.MonoRail.Framework
import Castle.MonoRail.WindsorExtension

Facility( "rails", RailsFacility )

webAsm = Assembly.Load("HibernatingForums.Web")
activeRecordAssemblies = ( Assembly.Load("HibernatingForums.Model"), )

for type in webAsm.GetTypes():
	if typeof(Controller).IsAssignableFrom(type):
		Component(type.Name, type)
	elif typeof(ViewComponent).IsAssignableFrom(type):
		Component(type.Name, type)


Component("active_record_repository", IRepository, ARRepository)
Component("active_record_unit_of_work", 
	IUnitOfWorkFactory, 
	ActiveRecordUnitOfWorkFactory,
	assemblies: activeRecordAssemblies )