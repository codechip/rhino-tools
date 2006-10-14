using System;
using System.Collections;
using Ayende.NHibernateQueryAnalyzer.Model;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Tests.TestUtilities;
using Ayende.NHibernateQueryAnalyzer.UnitTests.Asserts;
using NUnit.Framework;

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
			ListAssert.In(TestDataUtil.TestDllFile,current.Files);
			ListAssert.In(TestDataUtil.TestMappingFile,current.Files);
			ListAssert.NotIn(TestDataUtil.TestConfigFile,current.Files);
		}


        [Test]
        public void HandleNullables()
        {
            current.BuildProject();
            IList hrg = current.RunHql("from Files f where f.Size is null order by f.Id");
            Assert.AreEqual(2, hrg.Count);
            RemoteObject ro = hrg[0] as RemoteObject;
            Assert.AreEqual("1", ro["Id"]);
            Assert.AreEqual("one.txt", ro["Filename"]);
            Assert.AreEqual("null", ro["Size"]);
            
            ro = hrg[1] as RemoteObject;
            Assert.AreEqual("3", ro["Id"]);
            Assert.AreEqual("three.txt", ro["Filename"]);
            Assert.AreEqual("null", ro["Size"]);

            hrg = current.RunHql("from Files f where f.Size is not null order by f.Id");
            Assert.AreNotEqual(0,hrg.Count);
            foreach (RemoteObject remoteObject in hrg)
            {
                Assert.AreNotEqual("null",remoteObject["Size"]);
            }
        }
	}
}