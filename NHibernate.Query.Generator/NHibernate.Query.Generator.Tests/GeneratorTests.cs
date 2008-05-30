#region license
// Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Ayende Rahien nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using MbUnit.Core.Exceptions;
using Microsoft.CSharp;
using NHibernate.Criterion;
using MbUnit.Framework;
using Query;
using Expr = NHibernate.Criterion.Expression;

namespace NHibernate.Query.Generator.Tests
{
	[TestFixture]
	public class GeneratorTests
	{
		private string code;
		
		[SetUp]
		public void TestInitialize()
		{
			Stream sampleStream = GetSampleStream();
			code = TestUtil.GenerateCode(sampleStream);
		}

		[Test]
		public void CanGenerateCodeThatDoesnotProduceErrors()
		{
			AssertCodeCompiles();
		}

		[Test]
		public void CanGenerateAssemblyFromXmlMapping()
		{
			AssertCodeCompiles();

			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			Assert.IsNotNull(asm);
		}

		[Test]
		public void GeneratedAssemblyHasWhereTypeWithNestedCustomerType()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);

			System.Type whereType = asm.GetType("Query.Where");
			Assert.IsNotNull(whereType, "Should have gotten an assembly with a where type");

			PropertyInfo customerProperty = whereType.GetProperty("Customer");


			Assert.IsNotNull(customerProperty, "Where type should have property Customer");
		}

		[Test]
		public void GeneratedAssemblyHasWhereTypeUnqualifiedNameCompositeCustomer()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);

			System.Type whereType = asm.GetType("Query.Where");
			Assert.IsNotNull(whereType, "Should have gotten an assembly with a where type");

			PropertyInfo customerProperty = whereType.GetProperty("CompositeCustomer");


			Assert.IsNotNull(customerProperty, "Where type should have property CompositeCustomer");
		}

		/// <summary>
		/// Hard to explain it by the name of the test, but in general, I want to test
		/// that it has OrderBy.CompositeCustomer.CustomerId, and not 
		/// OrderBy.CompositeCustomer.Key.CustomerId
		/// </summary>
		[Test]
		public void CompositeCustomerOrderringHavePropertyWithoutKey()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);

			System.Type compositeCustomerOrderring = asm.GetType("Query.OrderBy+CompositeCustomer");
			PropertyInfo property = compositeCustomerOrderring.GetProperty("CustomerId");
			Assert.IsNotNull(property);
			Assert.AreEqual(typeof(OrderByClause), property.PropertyType);
		}

		[Test]
		public void ComponentsGetOrderByAsWell()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);

			System.Type homeType = asm.GetType("Query.OrderBy+Customer+Home");
			PropertyInfo property = homeType.GetProperty("Phone");
			Assert.IsNotNull(property);
			Assert.AreEqual(typeof(OrderByClause), property.PropertyType);
	
		}

		[Test]
		public void NestedTypeOnWhereHasPropertiesForEachOfthePersistentPropertiesInTheMapping()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:property", "Customer", typeof(PropertyQueryBuilder<>));
		}

		[Test]
		public void NestedTypeOnWhereHasPropertiesForPrimaryKey()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:id[@name='Id']", "Customer", typeof(IdQueryBuilder<>));
		}

		[Test]
		public void NestedTypeOnWhereHasPropertiesForEachOftheCompositeKeyPropertiesInTheMapping()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:composite-id/nh:key-property",
									  "CompositeCustomer",
									  typeof(PropertyQueryBuilder<>));
		}


		[Test]
		public void NestedTypeSupportsManyToOne()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:many-to-one", "Customer", "Query.Where+Query_Address`1");
		}

		[Test]
		public void NestedTypeSupportsManyToOneCompositeId()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:composite-id/nh:key-many-to-one",
									  "CompositeCustomer",
									  "Query.Where+Query_BadCustomer`1");
		}

		[Test]
		public void NestedTypeSupportIDProperty()
		{
			AssertWhereHasPropertyForXPath("/nh:hibernate-mapping/nh:class/nh:id[@name='Id']", "Customer", typeof(IdQueryBuilder<>));
		}


		[Test]
		public void GeneratedWhereTypeIsPartial()
		{
			int indexOf = code.IndexOf("public partial class Where");
			Assert.AreNotEqual(-1, indexOf, "Should have a partial where class");
		}

		[Test]
		public void GetNonNullObjectFromProperty()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			System.Type whereType = asm.GetType("Query.Where");
			object customer = whereType.GetProperty("Customer").GetValue(null, null);
			PropertyInfo property = customer.GetType().GetProperty("Name");
			object result = property.GetValue(customer, null);
			Assert.IsNotNull(result, "should have gotten a result");
		}


		[Test]
		public void PropertyReturnsNamedExpressionWithNameSetToPropertyName()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			object customer = asm.GetType("Query.Where").GetProperty("Customer").GetValue(null, null);
			System.Type customerType = customer.GetType();
			PropertyInfo property = customerType.GetProperty("Name");
			object result = property.GetValue(customer, null);
			FieldInfo field = result.GetType().GetField("myName", BindingFlags.Instance | BindingFlags.NonPublic);
			object value = field.GetValue(result);
			Assert.AreEqual("Name", value);
		}

		[Test]
		public void CreatesStronglyTypedPropertiesForQueries()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			System.Type queries = asm.GetType("Query.Queries");
			Assert.IsNotNull(queries, "failed to find Matches type");
			PropertyInfo property = queries.GetProperty("MyQuery");
			Assert.IsNotNull(property, "Didn't find MyQuery propery");
			object result = property.GetValue(null, null);
			Assert.AreEqual("MyQuery", result, "didn't get the query name");


			PropertyInfo property2 = queries.GetProperty("HerQuery");
			Assert.IsNotNull(property2, "Didn't find MyQuery propery");
			object result2 = property2.GetValue(null, null);
			Assert.AreEqual("HerQuery", result2, "didn't get the query name");
		}

		[Test]
		public void GenerateCodeForSubClassesOutsideClassNode()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			PropertyInfo customerProp = asm.GetType("Query.Where").GetProperty("ValuedCustomer");
			object customer = customerProp.GetValue(null, null);
			System.Type valuedCustType = customer.GetType();
			Assert.IsNotNull(valuedCustType);
			PropertyInfo property = valuedCustType.GetProperty("Bar");
			Assert.IsNotNull(property);
			object value = property.GetValue(customer, null);
			Assert.IsNotNull(value);

			PropertyInfo propertyName = valuedCustType.GetProperty("Name");
			Assert.IsNotNull(propertyName, "should be able to get property from parent");
			value = propertyName.GetValue(customer, null);
			Assert.IsNotNull(value);
			
		}

		[Test]
		public void GenerateCodeForJoinedSubClassInClass()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			object customer =
				asm.GetType("Query.Where").GetProperty("BadCustomer").
					GetValue(null, null);
			System.Type customerType = customer.GetType();
			Assert.IsNotNull(customerType);
			PropertyInfo property = customerType.GetProperty("Foo");
			Assert.IsNotNull(property);
			object value = property.GetValue(customer, null);
			Assert.IsNotNull(value);


			PropertyInfo propertyName = customerType.GetProperty("Name");
			Assert.IsNotNull(propertyName, "should be able to get property from parent");
			value = propertyName.GetValue(customer, null);
			Assert.IsNotNull(value);
	
		}

		[Test]
		public void GenerateCodeForSubClass()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			PropertyInfo custType = asm.GetType("Query.Where").GetProperty("ValuedCustomer2");
			object customer = custType.GetValue(null, null);
			System.Type valuedCustType = customer.GetType();
			Assert.IsNotNull(valuedCustType);
			PropertyInfo property = valuedCustType.GetProperty("Bar");
			Assert.IsNotNull(property);
			object value = property.GetValue(customer, null);
			Assert.IsNotNull(value);
		}

		[Test]
		public void GenerateCodeForJoinedSubClass()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			object custoer = asm.GetType("Query.Where").GetProperty("BadCustomer2").GetValue(null, null);
			System.Type customerType = custoer.GetType();
			Assert.IsNotNull(customerType);
			PropertyInfo property = customerType.GetProperty("Foo");
			Assert.IsNotNull(property);
			object value = property.GetValue(custoer, null);
			Assert.IsNotNull(value);
		}

		[Test]
		public void GenerateCodeForUnionSubClass()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			object custoer = asm.GetType("Query.Where").GetProperty("CrazyCustomer").GetValue(null, null);
			System.Type customerType = custoer.GetType();
			Assert.IsNotNull(customerType);
			PropertyInfo property = customerType.GetProperty("Id");
			Assert.IsNotNull(property);
			object value = property.GetValue(custoer, null);
			Assert.IsNotNull(value);
		}


		[Test]
		public void GeneratePropertiesForComponents()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			System.Type whereType = asm.GetType("Query.Where");
			PropertyInfo prop = whereType.GetProperty("Customer");
			object customer = prop.GetValue(null, null);
			System.Type customerType = customer.GetType();
			object home = customerType.GetProperty("Home").GetValue(customer, null);
			System.Type homeType = home.GetType();
			object phone = homeType.GetProperty("Phone").GetValue(home, null);
			Assert.AreEqual(typeof(PropertyQueryBuilder<>), phone.GetType().GetGenericTypeDefinition());
			object address = homeType.GetProperty("Address").GetValue(home, null);
			Assert.AreEqual("Query.Where+Query_Address`1", address.GetType().GetGenericTypeDefinition().FullName);
		}

		[Test]
		public void GenerateCollectionForComponents()
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			System.Type whereType = asm.GetType("Query.Where");
			PropertyInfo prop = whereType.GetProperty("Customer");
			object customer = prop.GetValue(null, null);
			System.Type customerType = customer.GetType();
			object contactdata = customerType.GetProperty("ContactData").GetValue(customer, null);
			System.Type contactdataType = contactdata.GetType();
			object addresses = contactdataType.GetProperty("Addresses").GetValue(contactdata, null);
			Assert.AreEqual(typeof(CollectionQueryBuilder<>), addresses.GetType().BaseType.GetGenericTypeDefinition());
			Assert.AreEqual("Query.Where+Query_Customer`1+Query_ContactData`1+Query_Collection_Addresses", addresses.GetType().GetGenericTypeDefinition().FullName);
		}

		#region Util Methods

		private void AssertWhereHasPropertyForXPath(string xpathQuery, string property, System.Type expectedPropetyType)
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			AssertWhereHasPropertyForXPath(asm, xpathQuery, property, expectedPropetyType);
		}

		private void AssertWhereHasPropertyForXPath(string xpathQuery, string property, string expectedPropetyType)
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			AssertWhereHasPropertyForXPath(asm, xpathQuery, property, asm.GetType(expectedPropetyType));
		}

		private void AssertOrderByHasPropertyForXPath(string xpathQuery, string className, System.Type type)
		{
			Assembly asm = TestUtil.GetAssemblyFromCode(code);
			System.Type orderByType = asm.GetType("Query.OrderBy+"+className);
			AssertHasPropertyForXPath(type, className, orderByType, xpathQuery);
		}
		private void AssertWhereHasPropertyForXPath(
			Assembly asm, string xpathQuery, string property, System.Type expectedPropetyType)
		{
			System.Type whereType = asm.GetType("Query.Where");
			AssertHasPropertyForXPath(expectedPropetyType, property, whereType, xpathQuery);
		}

		private static void AssertHasPropertyForXPath(System.Type expectedPropetyType, string property, System.Type parentType, string xpathQuery)
		{
			PropertyInfo prop = parentType.GetProperty(property);
			object customer = prop.GetValue(null, null);

			Assert.IsNotNull(customer);

			foreach (XmlNode node in GetSelectNodes(xpathQuery))
			{
				AssertHasMatchingProperty(customer, node, expectedPropetyType);
			}
		}

		private static XmlNodeList GetSelectNodes(string xpathQuery)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(GetSampleStream());

			XmlNamespaceManager nsMgr = new XmlNamespaceManager(xdoc.NameTable);
			nsMgr.AddNamespace("nh", "urn:nhibernate-mapping-2.2");

			XmlNodeList selectNodes = xdoc.SelectNodes(xpathQuery, nsMgr);
			Assert.AreNotEqual(0, selectNodes.Count, "Mapping should have at least one property");
			return selectNodes;
		}

		private static void AssertHasMatchingProperty(object customer, XmlNode node, System.Type expectedType)
		{
			string propName = node.Attributes["name"].Value;
			System.Type customerType = customer.GetType();
			PropertyInfo property = customerType.GetProperty(propName);
			Assert.IsNotNull(property, "{0} should have a property named {1}", customerType.Name, propName);
			Assert.AreEqual(expectedType, property.PropertyType.GetGenericTypeDefinition());
		}

		private void AssertCodeCompiles()
		{
			CompilerResults results = TestUtil.CompileCode(code);
			if (results.Errors.Count != 0)
			{
				StringBuilder sb = new StringBuilder();
				foreach (CompilerError error in results.Errors)
				{
					sb.Append(error.ToString()).AppendLine();
				}
				throw new AssertionException(sb.ToString());
			}
		}

		private static Stream GetSampleStream()
		{
			return Assembly.GetExecutingAssembly().GetManifestResourceStream("NHibernate.Query.Generator.Tests.Sample.hbm.xml");
		}

		#endregion
	}
}
