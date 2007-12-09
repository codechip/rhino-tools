using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binsor.Presentation.Framework.Interfaces
{
	using System.Windows;

	public interface ILayoutRegistry
	{
		void AddView(IView view);
		void Register(ILayout layout);
		void Register(FrameworkElement frameworkElement);
	}
}
