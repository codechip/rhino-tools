import System
import System.Reflection
import Rhino.Igloo
import Rhino.Commons from Rhino.Commons.NHibernate as nh
import Rhino.Commons from Rhino.Commons.ActiveRecord as ar

facility  "transaction.facility", RhinoTransactionFacility
facility "rhino.igloo.facility", RhinoIglooFacility:
	assemblies = (Assembly.Load("MyApp.Web"), Assembly.Load("MyApp.Controllers"))

activeRecordAssemblies = ( Assembly.Load("MyApp.Model"), )

component "active_record_repository", IRepository, ARRepository
component "active_record_unit_of_work", IUnitOfWorkFactory, Rhino.Commons.ActiveRecordUnitOfWorkFactory:
	assemblies = activeRecordAssemblies 

component "context_provider", IContextProvider, HttpContextProvider
