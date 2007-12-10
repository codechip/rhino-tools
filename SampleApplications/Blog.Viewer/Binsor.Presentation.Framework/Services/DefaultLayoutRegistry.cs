namespace Binsor.Presentation.Framework.Services
{
	using System.Collections.Generic;
	using System.Windows;
	using Exceptions;
	using Interfaces;

	public class DefaultLayoutRegistry : ILayoutRegistry
	{
		private Dictionary<string, ILayout> layouts = new Dictionary<string, ILayout>();
		private ILayoutDecoratorResolver layoutDeocratorResolver;

		public DefaultLayoutRegistry(ILayoutDecoratorResolver layoutDeocratorResolver)
		{
			this.layoutDeocratorResolver = layoutDeocratorResolver;
		}

		public void AddView(IView view)
		{
			foreach (ILayout layout in layouts.Values)
			{
				if(layout.CanAccept(view))
					layout.AddView(view);
			}
		}


		public void Register(FrameworkElement frameworkElement)
		{
			ILayout layout = layoutDeocratorResolver.GetLayoutDecoratorFor(frameworkElement);
			if (layout == null)
				return;
			Register(layout);
		}

		public void Register(ILayout layout)
		{
			if (layouts.ContainsKey(layout.Name))
				throw new DuplicateLayoutException("Layout names must be unique. A layout named '" + layout.Name + "' already exists.");
			layouts.Add(layout.Name, layout);
		}

		public ILayout GetLayout(string layoutName)
		{
			ILayout result;
			if(layouts.TryGetValue(layoutName, out result)==false)
			throw new MissingLayoutException("Could not find layout: " + layoutName);
			return result;
		}
	}
}
