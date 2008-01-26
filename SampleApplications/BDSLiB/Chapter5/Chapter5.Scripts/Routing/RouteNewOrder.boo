return if msg.message_type != "NewOrder"

HandleWith NewOrderHandler:
	lines = []
	for line in msg.order_lines:
		lines.Add( OrderLine( line.product, line.qty ) )
	return NewOrderMessage( msg.customer_id, msg.type, lines.ToArray(OrderLine) )