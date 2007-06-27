Component("active_record_repository", IRepository, ARRepository)
Component("active_record_unit_of_work", 
	IUnitOfWorkFactory, 
	ActiveRecordUnitOfWorkFactory)