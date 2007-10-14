
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
			array: (calcGst, calcShipping, calcTotal) }
	  } } )