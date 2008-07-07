using System;
using MbUnit.Framework;
using Rhino.Queues.Cfg;

namespace Rhino.Queues.Tests.Units
{
	[TestFixture]
	public class ConfigurationTests
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException), "Cannot build configuration 'foo' before setting an endpoint mapping for 'foo' using Map('foo').To('http://some/end/point');")]
		public void Cannot_build_cofiguration_without_endpoint_mapped_to_configuration_name()
		{
			new Configuration("foo").BuildQueueFactory();
		}

		[Test]
		[ExpectedArgumentNullException]
		public void Cannot_create_configuraiton_with_null_name()
		{
			new Configuration(null);
		}

		[Test]
		[ExpectedArgumentException]
		public void Cannot_create_configuraiton_with_empty_name()
		{
			new Configuration("");
		}
	}
}