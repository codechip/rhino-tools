# decide if this message match this DSL script
return if msg.type != "NewOrder" and msg.version == "1.0"

# decide which handle is going to handle it
HandleWith NewOrderHandler:
	# define a new list
	lines = [] 
	# add order lines to the list
	for line in msg.order_lines: 
		lines.Add( OrderLine( line.product, line.qty ) )
	# create internal message representation
	return NewOrderMessage( 
		msg.customer_id, 
		msg.type, 
		lines.ToArray(OrderLine) ) 
