using System.IO;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;
using Component=Castle.MicroKernel.Registration.Component;
namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorPreRegistrationWithExtensions_TestCase
		: BaseTest
	{
		[Test]
		public void CanExtendComponentsRegisteredInCode()
		{
			using (IWindsorContainer container = new RhinoContainer())
			{
				container.Register(Component.For<GstCalculator>()
					.Named("gstcalculator")
					.DependsOn(Property.ForKey("GstRate").Eq(15m)));

				IWindsorInstaller installer = BinsorScript.Inline(new StringBuilder().AppendLine("extend \"gstcalculator\":")
				                    	.AppendLine("	GstRate = decimal(50)").ToString());
				installer.Install(container, null);
				GstCalculator calculator = container.Resolve<GstCalculator>();
				Assert.AreEqual(50m, calculator.GstRate);
			}
		}
		[Test]
		public void CanExtendComponentsRegisteredInFacilities()
		{
			using (IWindsorContainer container = new RhinoContainer())
			{
				string path = Path.GetFullPath(@"Binsor\PreRegistrationWithExtensions.boo");
				BooReader.Read(container, path);
				ShippingCalculator calculator = container.Resolve<ShippingCalculator>();
				Assert.AreEqual(120m, calculator.ShippingCost);
				
			}
		}
	}
}
