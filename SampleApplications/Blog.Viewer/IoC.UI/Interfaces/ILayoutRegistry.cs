using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoC.UI.Interfaces
{
	public interface ILayoutRegistry
	{
		void AddLayout(IView view);
		void RegisterLayout(ILayout layout);
	}
}
