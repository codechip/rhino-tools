using System;
using System.Collections;
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Tests.Utilties;
using NUnit.Framework;

namespace Ayende.NHibernateQueryAnalyzer.Tests.Utilities
{
	[TestFixture]
	public class RemoteObjectTests
	{
		[Test]
		public void ObjectType()
		{
			RemoteObject sso = RemoteObject.Create("blah blah blah");
			Assert.AreEqual(typeof (string).Name, sso.TypeName, "Invalid typename returned");
			Assert.AreEqual(typeof (string).FullName, sso.FullTypeName, "Invalid typename returned");
        }
        
        [Test]
        public void CanHandleClassWithTypeProperty()
        {
            RemoteObject ro = RemoteObject.Create(typeof(RemoteList));
            Assert.AreEqual(2, ro.Properties.Length);
            Assert.AreEqual("RemoteList", ro["Name"]);
            Assert.AreEqual("Ayende.NHibernateQueryAnalyzer.ProjectLoader.RemoteList", ro["FullName"]);
        }


        [Test]
		public void ObjectValue()
		{
			string val = "blah blah blah";
			RemoteObject sso = RemoteObject.Create(val);
			Assert.IsTrue(sso.IsSimpleType);
			Assert.AreEqual(val, sso.Value, "Wrong object name");
			sso = RemoteObject.Create(new ArrayList());
			Assert.IsFalse(sso.IsSimpleType);
			Assert.AreEqual("System.Collections.ArrayList", sso.Value);
		}

		[Test]
		public void PropertyList()
		{
			RemoteObject sso = RemoteObject.Create(new ReflectionUtilTests.TestForReflection());
			//We are just testing that it works, since the deep testing is in ReflectionUtilTests
			Assert.AreEqual("232", sso["Integer"]);
			Assert.AreEqual(typeof (string[]).FullName, ((RemoteObject)sso["StringArray"]).FullTypeName);
			Assert.AreEqual(ReflectionUtilTests.TestForReflection.InitName, sso.Name);
		}

		[Test]
		public void GetSSOForFieldOrProperty()
		{
			RemoteObject sso = RemoteObject.Create(new ReflectionUtilTests.TestForReflection());
			RemoteObject inner = sso.GetValueOf("Integer");
			Assert.AreEqual(typeof (int).Name, inner.TypeName);
			Assert.AreEqual("232", inner.Value);
		}
	}
}