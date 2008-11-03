suggest_register_as_preferred:
	when order.Total > 1000 and not customer.IsPreferred

apply_discount 5.precentage():
	when order.Total > 1000 and customer.IsPreferred
	
deny_sale:
	when order.Total < 100