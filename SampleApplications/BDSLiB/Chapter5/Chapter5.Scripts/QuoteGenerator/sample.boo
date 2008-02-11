specification @vacations:
	requires @scheduling_work
	requires @external_connections
	
specification @salary:
	users_per_machine 150
	
specification @taxes:
	users_per_machine 50

specification @pension:
	same_machine_as @health_insurance
