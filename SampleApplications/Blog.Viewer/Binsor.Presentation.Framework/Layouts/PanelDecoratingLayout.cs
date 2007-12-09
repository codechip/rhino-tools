using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binsor.Presentation.Framework.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace Binsor.Presentation.Framework.Layouts
{
	using Attributes;

	[SkipAutomaticRegistration("Layouts are configured manually, because they are tied to specific elements.")]
	public class PanelDecoratingLayout : ILayout
	{
		private readonly Panel element;
		private readonly string[] acceptableViewNames;

		public PanelDecoratingLayout(
			Panel element, 
			string[] acceptableViewNames
			)
		{
			this.element = element;
			this.acceptableViewNames = acceptableViewNames;
		}

		public Panel Element
		{
			get { return element; }
		}

		public string Name
		{
			get { return element.Name; }
		}

		public bool CanAccept(IView view)
		{
			return acceptableViewNames.Contains(view.Name);
		}

		public void AddView(IView view)
		{
			element.Children.Add((UIElement)view);
		}
	}
}
