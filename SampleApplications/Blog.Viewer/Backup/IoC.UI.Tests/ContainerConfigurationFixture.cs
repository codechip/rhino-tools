namespace IoC.UI.Tests
{
	using System;
	using System.Windows.Forms;
	using Castle.Windsor;
	using Demo;
	using Impl;
	using Interfaces;
	using MbUnit.Framework;
	using Rhino.Commons;

	[TestFixture]
	public class ContainerConfigurationFixture
	{
		private IWindsorContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new RhinoContainer("assembly://IoC.UI.Tests/Windsor.boo");
		}

		[RowTest]
		[Row(typeof(IView), typeof(DemoView))]
		[Row(typeof(ILayout), typeof(DemoLayout))]
		[Row(typeof(IModuleLoader), typeof(DemoModuleLoader))]
		[Row(typeof(IPresenter), typeof(DemoPresenter))]
		[Row(typeof(IShellView), typeof(DemoShellView))]
		[Row(typeof(IApplicationContext), typeof(DefaultApplicationContext))]
		public void WillRecognizeTypesAutomatically(Type interfaceType, Type concreteType)
		{
			Assert.AreEqual(concreteType, container.Resolve(interfaceType).GetType());
		}

		[Test]
		public void WillFillTheMenuItemsFromConfiguration()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			
			Assert.AreEqual("File_Exit", resolve.Items[0].Name);
			Assert.AreEqual("File_Exit", resolve.Items[0].Command);
			Assert.AreEqual("File", resolve.Items[0].Parent);
			Assert.AreEqual("F4, Alt", resolve.Items[0].Shortcut);

			Assert.AreEqual("Help_About", resolve.Items[1].Name);
			Assert.AreEqual("Help_About", resolve.Items[1].Command);
			Assert.AreEqual("Help", resolve.Items[1].Parent);
			Assert.AreEqual("F1", resolve.Items[1].Shortcut);
		}

		[Test]
		public void WillTranslateFromShortcutTextToKeysEnum()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			Assert.AreEqual(Keys.Alt | Keys.F4, resolve.Items[0].ShortcutKey);
			Assert.AreEqual(Keys.F1, resolve.Items[1].ShortcutKey);
		}

		[Test]
		public void CanDefineNamedMenuesWithoutCommands()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			Assert.AreEqual("Help", resolve.Items[2].Name);
			Assert.IsNull(resolve.Items[2].Command);
		}

		[Test]
		public void EmptyShortCutWillBeInterpretedAsNullShortcutKey()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			Assert.IsNull(resolve.Items[3].ShortcutKey);
		
		}

		[Test]
		public void CanSetMenuTextFromConfig()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			Assert.AreEqual("H&elp", resolve.Items[2].Text);
		}

		[Test]
		public void CanDefineMenuWithoutShortcutKey()
		{
			DemoModuleLoader resolve = (DemoModuleLoader)container.Resolve<IModuleLoader>();
			Assert.IsNull(resolve.Items[2].ShortcutKey);
		}

		[RowTest]
		[Row(typeof(IView))]
		[Row(typeof(ILayout))]
		[Row(typeof(IPresenter))]
		public void EnsureTrasientTypes(Type type)
		{
			object a = container.Resolve(type);
			object b = container.Resolve(type);
			Assert.AreNotSame(a, b,
							  "Should get different instances, " + type + " should be transient ");
		}

		[RowTest]
		[Row(typeof(IModuleLoader))]
		[Row(typeof(IShellView))]
		[Row(typeof(IApplicationContext))]
		public void EnsureSingletonTypes(Type type)
		{
			object a = container.Resolve(type);
			object b = container.Resolve(type);
			Assert.AreSame(a, b,
							  "Should get different instances, " + type + " should be singleton ");
		}
	}
}