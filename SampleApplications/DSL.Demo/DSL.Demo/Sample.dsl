OnCreate Account:
	Entity.AccountNumber = date.Now.Ticks
	
OnCreate Order:
	if Entity.Total > Entity.Account.MaxOrderTotal:
		BeginManualApprovalFor Entity