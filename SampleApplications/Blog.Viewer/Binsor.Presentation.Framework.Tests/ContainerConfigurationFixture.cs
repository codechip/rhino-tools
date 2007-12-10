namespace Binsor.Presentation.Framework.Tests
{
    using System;
    using Castle.Windsor;
    using Demo;
    using Services;
    using Interfaces;
    using MbUnit.Framework;
    using Rhino.Commons;
    using Rhino.Mocks;
    using Binsor.Presentation.Framework.Data;
    using System.Windows.Input;

    [TestFixture]
    public class ContainerConfigurationFixture
    {
        private IWindsorContainer container;

        [SetUp]
        public void SetUp()
        {
            container = new RhinoContainer("assembly://Binsor.Presentation.Framework.Tests/Windsor.boo");
        }

        [RowTest]
        [Row(typeof(IView), typeof(DemoView))]
        [Row(typeof(ILayout), typeof(DemoLayout))]
        [Row(typeof(IModuleLoader), typeof(DemoModuleLoader))]
        [Row(typeof(IPresenter), typeof(DemoPresenter))]
        public void Configration_should_recognize_well_know_types_and_register_them_automatically(Type interfaceType, Type concreteType)
		{
            Assert.AreEqual(concreteType, container.Kernel.GetHandler(interfaceType).ComponentModel.Implementation);
        }

        [Test]
        public void Can_specify_menus_names_and_command_names_using_declerative_configuration()
		{
            DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();

            Assert.AreEqual("File_Exit", resolve.Items[0].Name);
            Assert.AreEqual("File_Exit", resolve.Items[0].CommandName);
            Assert.AreEqual("File", resolve.Items[0].Parent);

            Assert.AreEqual("Help_About", resolve.Items[1].Name);
            Assert.AreEqual("Help_About", resolve.Items[1].CommandName);
            Assert.AreEqual("Help", resolve.Items[1].Parent);
        }

        [Test]
        public void Can_define_menu_names_without_requiring_a_command()
		{
            DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
            Assert.AreEqual("Help", resolve.Items[2].Name);
            Assert.IsNull(resolve.Items[2].Command);
        }

        [Test]
        public void Can_specify_menu_header_from_declerative_configuration()
		{
            DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
            Assert.AreEqual("H&elp", resolve.Items[2].Header);
        }

        [RowTest]
        [Row(typeof(IView))]
        [Row(typeof(ILayout))]
        [Row(typeof(IPresenter))]
        public void Views_layouts_and_presenters_should_be_transients(Type type)
		{
            object a = container.Resolve(type);
            object b = container.Resolve(type);
            Assert.AreNotSame(a, b,
                              "Should get different instances, " + type + " should be transient ");
        }

        [RowTest]
        [Row(typeof(IModuleLoader))]
		[Row(typeof(IApplicationContext))]
        public void ModuleLoaders_and_application_context_should_be_singletons(Type type)
		{
            container.Kernel.AddComponentInstance<IApplicationShell>(MockRepository.GenerateStub<IApplicationShell>());
            object a = container.Resolve(type);
            object b = container.Resolve(type);
            Assert.AreSame(a, b,
                              "Should get different instances, " + type + " should be singleton ");
        }

    	[Test]
		public void When_application_context_is_retrieved_from_container_it_layout_property_is_not_null()
		{
			container.Kernel.AddComponentInstance<IApplicationShell>(MockRepository.GenerateStub<IApplicationShell>());
			Assert.IsNotNull(container.Resolve<IApplicationContext>().Layouts);
    	}
    }
}