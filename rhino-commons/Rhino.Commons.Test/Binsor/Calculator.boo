
import Rhino.Commons
import Rhino.Commons.Test.Components from Rhino.Commons.Test
import Rhino.Commons.Test.Binsor

calcGst = Component("calc.gst", AbstractCalculator, GstCalculator,
	gstRate: decimal(1.20) )
 
calcShipping = Component("calc.shipping", AbstractCalculator, ShippingCalculator,
	shippingCost: decimal(0.0),
	fragileShippingPremium: decimal(0.0) )
   
calcTotal = Component("calc.total", AbstractCalculator, TotalCalculator)
 
Component("costCalculator.default", ICostCalculator, DefaultCostCalculator,
	calculators: (calcGst, calcShipping, calcTotal ) )