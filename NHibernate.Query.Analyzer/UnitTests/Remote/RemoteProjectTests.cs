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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Utilities;
using NHibernate.Driver;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Remote
{
	[TestFixture]
	public class RemoteProjectTests
	{
		private RemoteProject current;
		private string hqlQuery = "from TestProject t order by t.Id";
		private string asmFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.dll");
		private string conStr = string.Format("Data Source={0};New=False;UTF8Encoding=True;Version=3", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\TestProject.db"));

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			string[] asms = { asmFilename },
					 mappings = { Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestProject\Ayende.NHibernateQueryAnalyzer.TestProject.hbm.xml") };
			current = new RemoteProject();
			current.Cfg.SetProperty("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			current.Cfg.SetProperty("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect");
			current.Cfg.SetProperty("hibernate.connection.driver_class", typeof(SQLite20Driver).AssemblyQualifiedName);
			current.Cfg.SetProperty("hibernate.connection.connection_string", conStr);
			current.BuildInternalProject(asms, mappings, new string[0], new ArrayList());
		}


		[TestFixtureTearDown]
		public void TestFixtureTestCleanup()
		{
			current.Dispose();
		}

		[Test]
		public void RunHql()
		{
			HqlResultGraph hrg = current.RunHql(hqlQuery);
			Assert.AreEqual(2003, hrg.Graph.Count, "Bad number of results from test database");
			object tp = hrg.Graph[0];
			Assert.AreEqual(1, ReflectionUtil.GetPropertyValue(tp, "Id"));
			Assert.AreEqual("Testing", ReflectionUtil.GetPropertyValue(tp, "Data"));
		}

		[Test]
		public void BuiltProject()
		{
			Assert.IsNotNull(Type.GetType("TestProject.TestProject, Ayende.NHibernateQueryAnalyzer.TestProject", true), "Unable to load type from dynamically loaded assembly");
			Assert.IsNotNull(current.Factory, "Factory was not built");
		}
	}
}