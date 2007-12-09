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
	using System.Windows;

	public class DefaultLayoutDecoratorResolver : ILayoutDecoratorResolver
	{
		private IKernel kernel;

		public DefaultLayoutDecoratorResolver(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public ILayout GetLayoutDecoratorFor(FrameworkElement element)
		{
			if (kernel.HasComponent(element.Name) == false)
				return null;

			var hash = new Hashtable();
			hash["element"] = element;
			return (ILayout)kernel.Resolve(element.Name, hash);
		}
	}
}