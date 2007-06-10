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
using System.IO;
using System.Text;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Binsor;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class BinsorTestCase
	{
		private IWindsorContainer _container;

		[SetUp]
		public void TestInitialize()
		{
			_container = new RhinoContainer();
			string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Binsor\Windsor.boo"));
			Console.WriteLine(path);
			BooReader.Read(_container, path);
		}

		[Test]
		public void CanReadComponentFromConfiguration()
		{
			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test]
		public void CanUseSpecilaizedGenerics()
		{
			IRepository<Fubar> resolve = _container.Resolve<IRepository<Fubar>>();
			bool is_instance_of_fake = typeof(FakeRepository<Fubar>).IsInstanceOfType(resolve);
			Assert.IsTrue(is_instance_of_fake);
		}

		[Test]
		public void CanPassComponentReferences()
		{
			FakeRepository<Fubar> fakeRepository = (FakeRepository<Fubar>)_container.Resolve<IRepository<Fubar>>();
			
			Assert.IsNotNull(fakeRepository.Inner);
			Assert.IsTrue(fakeRepository.Inner is NHRepository<Fubar>);
			
		}
		
		[Test]
		public void CanPassPrimitiveParameters()
		{
			EmailSender sender = _container.Resolve<ISender>() as EmailSender;
			Assert.AreEqual("example.dot.org", sender.Host);
		}

		[Test]
		public void CanUseLoops()
		{
			Fubar foo1 = (Fubar)_container.Resolve("foo_1");
			Assert.IsNotNull(foo1);
			Assert.AreEqual(1, foo1.Foo);
			Fubar foo2 = (Fubar)_container.Resolve("foo_2");
			Assert.IsNotNull(foo2);
			Assert.AreEqual(2, foo2.Foo);
			Fubar foo3 = (Fubar)_container.Resolve("foo_3");
			Assert.IsNotNull(foo3);
			Assert.AreEqual(3, foo3.Foo);
		}

		[Test]
		public void CanSpecifyLifeStyle()
		{
			IHandler handler = _container.Kernel.GetHandler("defualt_repository");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType); 
		}

        
	}
}

