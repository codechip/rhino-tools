using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binsor.Presentation.Framework.Interfaces
{
	public interface ILayoutSelector
	{
		bool CanAccept(ILayout layout, IView view);
	}
}
