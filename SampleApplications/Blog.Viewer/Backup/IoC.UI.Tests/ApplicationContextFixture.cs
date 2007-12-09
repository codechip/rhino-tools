namespace IoC.UI.Tests
{
	using System;
	using System.Windows.Forms;
	using Data;
	using Impl;
	using Interfaces;
	using MbUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class ApplicationContextFixture
	{
		[Test]
		public void WillInitializeAllModuleLoadersOnStart()
		{

			MockRepository mocks = new MockRepository();
			IModuleLoader mockLoader1 = mocks.DynamicMock<IModuleLoader>();
			IModuleLoader mockLoader2 = mocks.DynamicMock<IModuleLoader>();
			IModuleLoader mockLoader3 = mocks.DynamicMock<IModuleLoader>();
			IShellView stubShell = mocks.Stub<IShellView>();
			DefaultApplicationContext context = mocks.PartialMock<DefaultApplicationContext>(
				stubShell, new IModuleLoader[] { mockLoader1, mockLoader2, mockLoader3 });

			using (mocks.Record())
			{
				//we may have order dependnecies, let us verify
				//that it does this in order
				using (mocks.Ordered())
				{
					mockLoader1.Initialize(context, stubShell);
					mockLoader2.Initialize(context, stubShell);
					mockLoader3.Initialize(context, stubShell);
				}

				//force context to ignore these calls
				Expect.Call(context.GetShellAsForm()).Return(null).Repeat.Once();
				Expect.Call(delegate { context.RunForm(null); }).Repeat.Once();
			}

			using (mocks.Playback())
			{
				context.Start();
			}
		}

		[Test]
		public void WillThrowIfShellIsNotForm()
		{
			IShellView stubShell = MockRepository.GenerateStub<IShellView>();
			DefaultApplicationContext context = new DefaultApplicationContext(stubShell, new IModuleLoader[] { });
			try
			{
				context.GetShellAsForm();
				Assert.Fail("Should have thrown");
			}
			catch (Exception ex)
			{
				StringAssert.Like(ex.Message,
					@"The IShellView implementation IShellViewProxy[\w\d]+ should be derived from System\.Windows\.Forms\.Form");
			}
		}

		[Test]
		public void WillCastShellToFormIfInheritFromForm()
		{
			DefaultApplicationContext context = new DefaultApplicationContext(new DemoForm(), new IModuleLoader[] { });
			Form form = context.GetShellAsForm();
			Assert.IsNotNull(form);
		}

		[Test]
		public void CallingStartWillShowTheForm()
		{
			DemoForm shell = new DemoForm();
			DefaultApplicationContext context = new DefaultApplicationContext(shell, new IModuleLoader[] { });
			context.Start();
			Assert.IsTrue(shell.WasShown);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException),
		   "The IShellView implementation 'null' should be derived from System.Windows.Forms.Form")]
		public void WillRaiseReadableErrorIfShellIsNull()
		{
			DefaultApplicationContext context = new DefaultApplicationContext(null, new IModuleLoader[] { });
			context.Start();
		}

		public class DemoForm : Form, IShellView
		{
			public bool WasShown;

			public DemoForm()
			{
				Load += new EventHandler(DemoForm_Load);
			}

			void DemoForm_Load(object sender, EventArgs e)
			{
				WasShown = true;
				this.Close();
			}

			public void AddMenuItems(params MenuItemData[] items)
			{

			}
		}
	}
}