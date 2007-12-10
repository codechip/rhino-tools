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

	[SkipRegistrationForLayout]
	public class PanelDecoratingLayout : ILayout
	{
		private readonly Panel element;

		public PanelDecoratingLayout(Panel element)
		{
			this.element = element;
		}

		public Panel Element
		{
			get { return element; }
		}

		public string Name
		{
			get { return element.Name; }
		}

		public virtual void AddView(IView view)
		{
			element.Children.Add((UIElement)view);
		}
	}
}
