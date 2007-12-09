namespace Binsor.Presentation.Framework.Impl
{
	using System.Collections.Generic;
	using Exceptions;
	using Interfaces;

	public class DefaultLayoutRegistry : ILayoutRegistry
	{
		private Dictionary<string, ILayout> layouts = new Dictionary<string, ILayout>();

		public void AddView(IView view)
		{
		}

		public void RegisterLayout(ILayout layout)
		{
			if (layouts.ContainsKey(layout.Name))
				throw new DuplicateLayoutException("Layout names must be unique. A layout named '" + layout.Name + "' already exists.");
			layouts.Add(layout.Name, layout);
		}

		public ILayout GetLayout(string layoutName)
		{
			return layouts[layoutName];
		}
	}
}
