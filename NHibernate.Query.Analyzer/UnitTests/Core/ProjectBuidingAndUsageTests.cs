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
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using MbUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Core
{
	/// <summary>
	/// A TestFixture for the <see cref="Ayende.NHibernateQueryAnalyzer.Model.Project"/> 
	/// class
	/// </summary>
	[TestFixture]
	public class ProjectBuidingAndUsageTests
	{
		private Project current;
		private string prjName = "NewProject";

		[SetUp]
		public void Setup()
		{
			current = new Project(prjName, new Project.Context());
			current.AddFile(TestDataUtil.TestConfigFile);
			current.AddFile(TestDataUtil.TestDllFile);
			current.AddFile(TestDataUtil.TestMappingFile);
		}

		[TearDown]
		public void TearDown()
		{
			current.Dispose();
		}

		[Test]
		public void GetNamedQuery()
		{
			Query q = new Query();
			q.Name = "name";
			current.Queries.Add(q);
			Assert.AreSame(q, current.GetQueryWithName(q.Name));
			Assert.IsNull(current.GetQueryWithName("not existing"));
		}

		[Test]
		public void ProjectHelperFileRecogniztion()
		{
			Project.Context context = new Project.Context();
			Project.Context.IFileAdd config = context.CreateFileAdd(@"C:\test\driven\design\application.exe.config");
			Project.Context.IFileAdd exe = context.CreateFileAdd(@"C:\test\driven\design\application.exe");
			Project.Context.IFileAdd dll = context.CreateFileAdd(@"C:\test\driven\design\application.dll");
			Project.Context.IFileAdd cfg = context.CreateFileAdd(@"C:\test\driven\design\application.cfg.xml");
			Project.Context.IFileAdd mapping = context.CreateFileAdd(@"C:\test\driven\design\application.hbm.xml");
			Project.Context.IFileAdd unknown = context.CreateFileAdd(@"C:\test\driven\design\application.odd");

			Assert.AreEqual(typeof (Project.Context.AddAppConfig), config.GetType());
			Assert.AreEqual(typeof (Project.Context.AddAssembly), exe.GetType());
			Assert.AreEqual(typeof (Project.Context.AddAssembly), dll.GetType());
			Assert.AreEqual(typeof (Project.Context.AddConfiguration), cfg.GetType());
			Assert.AreEqual(typeof (Project.Context.AddMapping), mapping.GetType());
			Assert.AreEqual(typeof (Project.Context.AddUnkown), unknown.GetType());
			
		}

		[Test]
		public void AddingFileTwice()
		{
			Project addFileTwice = new Project("Adding File Twice");
			addFileTwice.AddFile(@"C:\test\driven\design\application.cfg.xml");
			Assert.AreEqual(1, addFileTwice.Files.Count, "Didn't add a file to Files collections");
			addFileTwice.AddFile(@"C:\test\driven\design\application.cfg.xml");
			Assert.AreEqual(1, addFileTwice.Files.Count, "Add a file multiply times");
		}

		[Test]
		public void AddConfigChangeAppSetup()
		{
			Project changeAppSetup = new Project("Add Config Change App Setup");
			changeAppSetup.AddFile(TestDataUtil.TestConfigFile);
			changeAppSetup.AddFile(TestDataUtil.TestDllFile);
			changeAppSetup.AddFile(TestDataUtil.TestMappingFile);

			changeAppSetup.HandleFiles();
			Assert.AreEqual(TestDataUtil.TestConfigFile, changeAppSetup.AppDomainSetup.ConfigurationFile, "Configuration file was not set");
		}


		[Test]
		public void RemoveFile()
		{
			current.RemoveFile(TestDataUtil.TestConfigFile);
			Assert.AreEqual(2,current.Files.Count);
			CollectionAssert.Contains(current.Files, TestDataUtil.TestDllFile);
			CollectionAssert.Contains(current.Files,TestDataUtil.TestMappingFile);
			CollectionAssert.DoesNotContain(current.Files,TestDataUtil.TestConfigFile);
		}


        [Test]
        public void HandleNullables()
        {
            current.BuildProject();
            IList hrg = current.RunHql("from Files f where f.Size is null order by f.Id");
            Assert.AreEqual(6, hrg.Count);
            RemoteObject ro = hrg[0] as RemoteObject;
            Assert.AreEqual("1", ro["Id"]);
            Assert.AreEqual("one.txt", ro["Filename"]);
            Assert.AreEqual(null, ro["Size"]);
            
            ro = hrg[1] as RemoteObject;
            Assert.AreEqual("2", ro["Id"]);
            Assert.AreEqual("two.txt", ro["Filename"]);
            Assert.AreEqual(null, ro["Size"]);
        }
	}
}