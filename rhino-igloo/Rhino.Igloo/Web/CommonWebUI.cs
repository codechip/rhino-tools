using System;
using Rhino.Commons;

namespace Rhino.Igloo
{
    internal static class CommonWebUI
    {
        /// <summary>
        /// Does injection on the WebUI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal static void WebUI_InjectComponent(object sender, EventArgs e)
        {
            ComponentRepository repository = IoC.Resolve<ComponentRepository>();
            repository.InjectControllers(sender);
        }
    }
}
