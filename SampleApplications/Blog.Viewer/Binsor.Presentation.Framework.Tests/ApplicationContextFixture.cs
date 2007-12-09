namespace Binsor.Presentation.Framework.Tests
{
    using System;
    using Data;
    using Services;
    using Interfaces;
    using MbUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class ApplicationContextFixture
    {
        [Test]
        public void When_application_context_is_started_it_will_initialized_all_the_module_loader()
		{

            MockRepository mocks = new MockRepository();
            IModuleLoader mockLoader1 = mocks.DynamicMock<IModuleLoader>();
            IModuleLoader mockLoader2 = mocks.DynamicMock<IModuleLoader>();
            IModuleLoader mockLoader3 = mocks.DynamicMock<IModuleLoader>();
            IApplicationShell stubShell = mocks.Stub<IApplicationShell>();
            DefaultApplicationContext context = mocks.PartialMock<DefaultApplicationContext>(
                stubShell, 
				mocks.DynamicMock<ILayoutRegistry>(),
				new IModuleLoader[] { mockLoader1, mockLoader2, mockLoader3 });

            //we may have order dependnecies, let us verify
            //that it does this in order
            using (mocks.Record())
            using (mocks.Ordered())
            {
                mockLoader1.Initialize(context, stubShell);
                mockLoader2.Initialize(context, stubShell);
                mockLoader3.Initialize(context, stubShell);
            }

            using (mocks.Playback())
            {
                context.Start();
            }
        }
    }
}