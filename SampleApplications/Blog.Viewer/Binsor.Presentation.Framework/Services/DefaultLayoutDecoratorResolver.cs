namespace Binsor.Presentation.Framework.Services
{
	using Interfaces;
	using System.Collections.Generic;
	using System.Windows.Controls;
	using Castle.Core;
	using System;
	using Layouts;
	using Castle.MicroKernel;
	using Rhino.Commons;
	using System.Collections;

	public class DefaultLayoutDecoratorResolver : ILayoutDecoratorResolver
	{
		public ILayout GetLayoutDecoratorFor<T>(T element)
			where T : System.Windows.FrameworkElement
		{
			var hash = new Hashtable();
			hash["element"] = element;
			return IoC.Container.Resolve<ILayout>(element.Name, hash);
		}
	}
}