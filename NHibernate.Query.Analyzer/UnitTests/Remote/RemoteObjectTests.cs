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
using Ayende.NHibernateQueryAnalyzer.ProjectLoader;
using Ayende.NHibernateQueryAnalyzer.Tests.Utilties;
using MbUnit.Framework;

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