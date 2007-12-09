using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Binsor.Presentation.Framework.Data;
using Rhino.Mocks;
using Castle.Windsor;
using System.Windows.Input;
using Rhino.Commons;

namespace Binsor.Presentation.Framework.Tests
{
    [TestFixture]
    public class MenuItemDataFixture
    {
        [Test]
        public void When_command_is_explicitly_set_on_menu_item_data_should_not_try_to_resolve_command_by_name_from_IoC()
		{
            MockRepository mocks = new MockRepository();
            MenuItemData mid = new MenuItemData { CommandName = "test" };

            IWindsorContainer container = mocks.CreateMock<IWindsorContainer>();
            ICommand command = mocks.Stub<ICommand>();
            mid.Command = command;

            using (mocks.Record())
            {
                Expect.Call(container.Resolve<ICommand>("test")).Repeat.Never();
            }

            using (mocks.Playback())
            {
                using (IoC.UseLocalContainer(container))
                {
                    Assert.AreSame(command, mid.Command);
                }
            }
        }

        [Test]
        public void When_command_is_not_specifid_should_try_to_resolve_command_instance_from_IoC_container()
		{
            MockRepository mocks = new MockRepository();
            MenuItemData mid = new MenuItemData { CommandName = "test" };

            IWindsorContainer container = mocks.Stub<IWindsorContainer>();
            ICommand command = mocks.Stub<ICommand>();
            using (mocks.Record())
            {
                SetupResult.For(container.Resolve<ICommand>("test")).Return(command);
            }

            using (mocks.Playback())
            {
                using (IoC.UseLocalContainer(container))
                {
                    Assert.AreSame(command, mid.Command);
                }
            }

        }
    }
}
