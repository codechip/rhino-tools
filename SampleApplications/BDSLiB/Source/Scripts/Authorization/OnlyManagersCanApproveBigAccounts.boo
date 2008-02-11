operation "/order/approve"

if Principal.IsInRole("Managers"):
	Allow("Managers can always approve orders")
	return

if Entity.TotalCost >= 10_000:
	Deny("Only managers can approve orders of more than 10,000")
	
Allow("All users can approve orders less than 10,000")