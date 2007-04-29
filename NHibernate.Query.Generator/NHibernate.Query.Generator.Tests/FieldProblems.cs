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
