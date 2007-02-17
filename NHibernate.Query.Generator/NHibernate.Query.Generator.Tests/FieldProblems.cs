using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MbUnit.Framework;

namespace NHibernate.Query.Generator.Tests
{
	[TestFixture]
	public class FieldProblems
	{
		[Test]
		public void HbmWithJustAssemblyDeclaration()
		{
			string resource = "NHibernate.Query.Generator.Tests.WithTopLevelAssemblySpecification.hbm.xml";
			Assembly assembly = TestUtil.GetAssemblyFromResource(resource);

			System.Type whereType = assembly.GetType("Query.Where+Query_Broker`1");
			Assert.IsNotNull(whereType, "We should have gotten a Broker type");

			Assert.IsNotNull(whereType.GetProperty("Id"), "Should have an id property");
			PropertyInfo property = whereType.GetProperty("Name");
			Assert.IsNotNull(property, "Should have a property called name");
			Assert.IsNotNull(property.PropertyType.GetProperty("FirstName"),
				"Should have Name.FirstName");
			Assert.IsNotNull(property.PropertyType.GetProperty("LastName"),
				"Should have Name.LastName");
			Assert.IsNotNull(property.PropertyType.GetProperty("MiddleName"),
				"Should have Name.MiddleName");
			Assert.IsNotNull(property.PropertyType.GetProperty("Prefix"),
				"Should have Name.Prefix");
			Assert.IsNotNull(property.PropertyType.GetProperty("Suffix"),
				"Should have Name.Suffix");
			Assert.IsNotNull(property.PropertyType.GetProperty("DisplayName"),
			"Should have Name.DisplayName");

			System.Type orderByType = assembly.GetType("Query.OrderBy+Broker+Name");
			Assert.IsNotNull(orderByType, "Should have an order by for Broker.Name");

			Assert.IsNotNull(orderByType.GetProperty("FirstName"), "Should have a FirstName");
			Assert.IsNotNull(orderByType.GetProperty("LastName"), "Should have a LastName");
			Assert.IsNotNull(orderByType.GetProperty("MiddleName"), "Should have a MiddleName");
			Assert.IsNotNull(orderByType.GetProperty("Prefix"), "Should have a  Prefix");
			Assert.IsNotNull(orderByType.GetProperty("Suffix"), "Should have a Suffix");
			Assert.IsNotNull(orderByType.GetProperty("DisplayName"), "Should have a DisplayName");
		}

	}
}
