import Rhino.Commons
import Rhino.Commons.Repositories
import System.Reflection

activeRecordAssemblies = ( Assembly.Load("Exesto.Model"), )

Component(active_record_repository, IRepository, ARRepository)
Component(active_record_unit_of_work, IUnitOfWorkFactory, ActiveRecordUnitOfWorkFactory,
	assemblies: activeRecordAssemblies )