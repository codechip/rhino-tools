import Rhino.Commons
import Rhino.Commons.Repositories
import System.Reflection
import Castle.Core
import Castle.Services.Transaction
import Castle.Facilities.ActiveRecordIntegration from Exesto.Web
import Exesto.Web.Services
import Castle.Facilities.AutomaticTransactionManagement

def OnNewTransaction(transaction as ITransaction, 
		transactionMode as TransactionMode, 
		isolationMode as IsolationMode):
    transaction.Enlist( TransactionScopeResourceAdapter(transactionMode) )

Component(transaction_manager, 
	ITransactionManager, DefaultTransactionManager);

IoC.Container.AddFacility("automatic_transaction_management_facility", TransactionFacility())

IoC.Container.Kernel.ComponentCreated += do(model as ComponentModel, instance as object):
    if instance isa ITransactionManager:
        cast(ITransactionManager, instance).TransactionCreated+= OnNewTransaction

activeRecordAssemblies = ( Assembly.Load("Exesto.Model"), )

Component(active_record_repository, IRepository, ARRepository)
Component(active_record_unit_of_work, IUnitOfWorkFactory, ActiveRecordUnitOfWorkFactory,
	assemblies: activeRecordAssemblies )
	
Component(just_service, JustService)