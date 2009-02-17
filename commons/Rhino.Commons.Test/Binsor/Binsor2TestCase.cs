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
using System.IO;
using System.Reflection;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.Facilities.ActiveRecordIntegration;
using Castle.MicroKernel;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Commons.Facilities;
using Rhino.Commons.Test.Components;

namespace Rhino.Commons.Test.Binsor
{
	[TestFixture]
	public class Binsor2TestCase : ConfigurationAsserts
	{
		private IWindsorContainer _container;

		[SetUp]
		public override void TestInitialize()
		{
            base.TestInitialize();
			string path = Path.GetFullPath(@"Binsor\Windsor2.boo");
			_container = new RhinoContainer(path);
		}

		[Test]
		public void CanDefineActiveRecordFacility()
		{
			IFacility[] facilities = _container.Kernel.GetFacilities();
			foreach (IFacility facility in facilities)
			{
				ActiveRecordFacility ar = facility as ActiveRecordFacility;
				if (ar != null)
				{
					IConfiguration config = ar.FacilityConfig;
					Assert.IsNotNull(config);
					AssertAttribute(config, "isWeb", "true");
					AssertAttribute(config, "isDebug", "true");
					IConfiguration assemblies = AssertChild(config, "assemblies");
					Assert.AreEqual(1, assemblies.Children.Count);
					Assert.AreEqual(Assembly.Load("Rhino.Commons.NHibernate").ToString(),
					                assemblies.Children[0].Value);

					int configCount = 0;
					foreach (IConfiguration child in config.Children)
					{
						if (child.Name == "config")
						{
							++configCount;

							string type = child.Attributes["type"];
							if (string.IsNullOrEmpty(type))
							{
								AssertKeyValueAttrib(child, "add", "show_sql", "true");
								AssertKeyValueAttrib(child, "add", "command_timeout", "5000");
								AssertKeyValueAttrib(child, "add", "cache.foo.use_query_cache", "false");
								AssertKeyValueAttrib(child, "add", "dialect", "NHibernate.Dialect.MsSql2005Dialect");
								AssertKeyValueAttrib(child, "add", "connection.provider", "NHibernate.Connection.DriverConnectionProvider");
								AssertKeyValueAttrib(child, "add", "connection.driver_class", "NHibernate.Driver.SqlClientDriver");
								AssertKeyValueAttrib(child, "add", "connection.connection_string", "connectionString1");
							}
							else
							{
								AssertAttribute(child, "type", typeof(Fubar).AssemblyQualifiedName);
								AssertKeyValueAttrib(child, "add", "dialect", "NHibernate.Dialect.SQLiteDialect");
								AssertKeyValueAttrib(child, "add", "connection.provider", "NHibernate.Connection.DriverConnectionProvider");
								AssertKeyValueAttrib(child, "add", "connection.driver_class", "NHibernate.Driver.SQLite20Driver");
								AssertKeyValueAttrib(child, "add", "connection.connection_string", "connectionString2");
								AssertKeyValueAttrib(child, "add", "connection.release_mode", "on_close");
							}
						}
					}
					Assert.AreEqual(2, configCount);

					return;
				}
			}
			Assert.Fail("Could not find AR Facility");
		}

		[Test]
		public void CanRegisterFacilityInstance()
		{
			IFacility[] facilities = _container.Kernel.GetFacilities();
			foreach (IFacility facility in facilities)
			{
				ActiveRecordUnitOfWorkFacility ar = facility as ActiveRecordUnitOfWorkFacility;
				if (ar != null)
				{
					Assert.AreEqual(1, ar.Assemblies.Length);
					return;
				}
			}
			Assert.Fail("Could not find AR UoW Facility");
		}

		[Test]
		public void CanReadComponentFromConfiguration()
		{
			bool has_repos = _container.Kernel.HasComponent(typeof(IRepository<>));
			Assert.IsTrue(has_repos, "should have generic repository!");
		}

		[Test]
		public void CanUseMoreThanSingleFile()
		{
			bool has_repos = _container.Kernel.HasComponent(typeof(IDisposable));
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
		public void CanPassComponentReferencesByType()
		{
			FakeRepository<Fubar> fakeRepository = (FakeRepository<Fubar>)_container.Resolve<IRepository<Fubar>>("fubar_repository");

			Assert.IsNotNull(fakeRepository.Inner);
			Assert.IsTrue(fakeRepository.Inner is NHRepository<Fubar>);
		}

		[Test]
		public void CanPassMultipleDependenciesOnSingleLine()
		{
			EmailSender sender = (EmailSender) _container.Resolve<ISender>();
			Assert.AreEqual("example.dot.org", sender.Host);
			Assert.AreEqual(3, sender.To.Length);
			Assert.In("Kaitlyn", sender.To);
			Assert.In("Lauren", sender.To);
			Assert.In("Matthew", sender.To);
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
			IHandler handler = _container.Kernel.GetHandler("default_repository");
			Assert.AreEqual(LifestyleType.Transient, handler.ComponentModel.LifestyleType);
		}

		[Test]
		public void ContainerUsesXmlConfigurationIfNotBooExtension()
		{
			RhinoContainer container = new RhinoContainer(@"Binsor\Windsor.xml");

			ISender sender = container.Resolve<ISender>();
			Assert.IsNotNull(sender);
		}

		[Test]
		public void CanDefineConfigurationWithAttributes()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			AssertAttribute(component, "tag1", "important");
			AssertAttribute(component, "tag2", "priority");
		}

		[Test]
		public void CanDefineConfigurationWithChildren()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			AssertChild(component, "protocol");
			AssertChild(component, "configuration");
			AssertChild(component, "father", "Murray");

			IConfiguration child = AssertChild(component, "name", "David Beckham");
			AssertAttribute(child, "age", "32");
		}

		[Test]
		public void CanDefineConfigurationWithChildrenAndAttributes()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration child = AssertChild(component, "location");
			AssertAttribute(child, "latitude", "100");
			AssertAttribute(child, "longitude", "80");
		}

		[Test]
		public void CanDefineConfigurationsWithList()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration hosts = AssertChild(component, "hosts");
			Assert.AreEqual(2, hosts.Children.Count);
			foreach (IConfiguration child in hosts.Children)
			{
				Assert.AreEqual("host", child.Name);
			}
		}

		[Test]
		public void CanDefineConfigurationsWithKeymap()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration parameters = AssertChild(component, "parameters");
			IConfiguration address = AssertChild(parameters, "address");
			Assert.AreEqual(4, address.Children.Count);
			AssertKeyValue(address, "item", "line1", "124 Fletcher");
			AssertKeyValue(address, "item", "city", "Valley Stream");
			AssertKeyValue(address, "item", "state", "NY");
			AssertKeyValue(address, "item", "zipCode", "11512");
		}

		[Test]
		public void CanDefineConfigurationsWithKeymapAndHashLiteral()
		{
			IHandler handler = _container.Kernel.GetHandler("email_sender3");
			Assert.IsNotNull(handler);

			IConfiguration component = handler.ComponentModel.Configuration;
			IConfiguration parameters = AssertChild(component, "parameters");
			IConfiguration address = AssertChild(parameters, "address2");
			Assert.AreEqual(4, address.Children.Count);
			AssertKeyValue(address, "item", "line1", "3747 Dartbrook");
			AssertKeyValue(address, "item", "city", "Rockwall");
			AssertKeyValue(address, "item", "state", "TX");
			AssertKeyValue(address, "item", "zipCode", "75032");
		}

		[Test]
		public void CanDefineConfigurationsForFactorySupportFacilityWithAccessor()
		{
			Fubar foo = (Fubar)_container.Resolve("foo_instance");
			Assert.IsNotNull(foo);
			Assert.AreEqual("Instance", foo.Foo);
		}

		[Test]
		public void CanDefineConfigurationsForEventWiringFacility()
		{
			EmailListener listener = _container.Resolve<EmailListener>();
			Assert.IsNotNull(listener);
			Assert.IsNull(listener.Message);

			ISender sender = _container.Resolve<ISender>(typeof(EmailSender).FullName);
			Assert.IsNotNull(sender);

			sender.Send("Events are alive!");
			Assert.AreEqual("Events are alive!", listener.Message);
		}

		[Test]
		public void CanDefineConfigurationsForEventWiringFacilityWithDynamicListeners()
		{
			foreach ( string s in new string[] { "a", "b" } )
			{
				EmailListener listener = _container.Resolve<EmailListener>( "someListener." + s );
				Assert.IsNotNull( listener );
				Assert.IsNull( listener.Message );

				ISender sender = _container.Resolve<ISender>("somePublisher." + s );
				Assert.IsNotNull( sender );

				sender.Send( "Events are alive!" );
				Assert.AreEqual( "Events are alive!", listener.Message );				
			}
		}
		
		[Test]
		public void CanUseEnvironmentInfoPassedInToContainer()
		{
			Assert.IsFalse(_container.Kernel.HasComponent("foo_bar"));

			string path = Path.GetFullPath(@"Binsor\Windsor2.boo");
			RhinoContainer container = new RhinoContainer(path, new TestEnvironmentInfo("Binsor2"));
			Fubar fubar = container.Resolve<Fubar>("foo_bar");
			Assert.IsNotNull(fubar);
			Assert.AreEqual("Binsor2", fubar.Foo);
		}

		[Test]
		public void CanDefineConfigurationWithMaps()
		{
			MyComponent component = _container.Resolve<MyComponent>();
			Assert.AreEqual(2, component.SomeMapping.Count);
			Assert.AreEqual("value1", component.SomeMapping["key1"]);
			Assert.AreEqual("value2", component.SomeMapping["key2"]);
		}
	}
}

