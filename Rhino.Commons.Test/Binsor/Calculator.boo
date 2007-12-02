
import Rhino.Commons
import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons.Test.Binsor

# Using Dependencies

calcGst = Component( "calc.gst", AbstractCalculator, GstCalculator,
	gstRate: decimal(1.20) )
 
calcShipping = Component( "calc.shipping", AbstractCalculator, ShippingCalculator,
	shippingCost: decimal(0.0),
	fragileShippingPremium: decimal(0.0) )
   
calcTotal = Component( "calc.total", AbstractCalculator, TotalCalculator )
 
Component( "costCalculator.default", ICostCalculator, DefaultCostCalculator,
	calculators: (calcGst, calcShipping, calcTotal) )

# Using Parameters Only

calcGst2 = Component( "calc.gst2", AbstractCalculator, GstCalculator,
	{ parameters: {
		gsRate: 1.20
	  } } )
 
calcShipping2 = Component( "calc.shipping2", AbstractCalculator, ShippingCalculator,
	{ parameters: {
		shippingCost: 0.0,
		fragileShippingPremium: 0.0
	  } } )
   
calcTotal2 = Component( "calc.total2", AbstractCalculator, TotalCalculator )
 
Component( "costCalculator.default2", ICostCalculator, DefaultCostCalculator,
	{ parameters: {
		calculators: {
			array: (calcGst2, calcShipping2, calcTotal2) }
	  } } )
	  
# Using Binsor Macros

# Using Dependencies

component 'calc_gst', AbstractCalculator, GstCalculator:
	gstRate = decimal(1.20)
 
component 'calc_shipping', AbstractCalculator, ShippingCalculator:
	shippingCost = decimal(0.0)
	fragileShippingPremium = decimal(0.0)
   
component 'calc_total', AbstractCalculator, TotalCalculator
 
component 'cost_calculator_default', ICostCalculator, DefaultCostCalculator:
	calculators = ( @calc_gst, @calc_shipping, @calc_total )

# Using Parameters Only

component 'calc_gst2', AbstractCalculator, GstCalculator:
	parameters:
		gsRate = 1.20
 
component 'calc_shipping2', AbstractCalculator, ShippingCalculator:
	parameters:
		shippingCost = 0.0
		fragileShippingPremium = 0.0
   
component 'calc_total2', AbstractCalculator, TotalCalculator
 
component 'cost_calculator_default2', ICostCalculator, DefaultCostCalculator:
	parameters:
		calculators:
			array = ( @calc_gst2, @calc_shipping2, @calc_total2 )