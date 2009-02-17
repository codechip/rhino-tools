using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Rhino.Commons.Test.Components;
using Component=Castle.MicroKernel.Registration.Component;

namespace Rhino.Commons.Test.Binsor
{
	public class CalculatorFacility
		: AbstractFacility
	{
		#region Overrides of AbstractFacility

		protected override void Init()
		{
			Kernel.Register(Component.For<ShippingCalculator>()
				.Named("calculator")
				.DependsOn(Property.ForKey("ShippingCost").Eq(5m)));
		}

		#endregion
	}
}
