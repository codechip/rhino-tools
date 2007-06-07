using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;

namespace BookStore.Controllers
{
    public abstract class BaseMenuController : BaseController<ISelectOptionsView>
    {
        readonly ISelectOptionsView view = ApplicationFactory.CreateSelectOptionsView();

        public override ISelectOptionsView View
        {
            get { return view; }
        }

        protected override void DoRun()
        {
            View.Display();
        }

    }
}
