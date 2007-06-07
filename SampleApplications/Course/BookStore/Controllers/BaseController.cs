using System;
using System.Collections.Generic;
using System.Text;
using BookStore.UI;

namespace BookStore.Controllers
{
    public abstract class BaseController<TView>
        where TView : IView
    {
        public abstract TView View { get; }

        public void NotImplemented()
        {
            View.ShowMessage("Left for the students, do your homework!");
        }

        public virtual void Run()
        {
            try
            {
                DoRun();
            }
            catch (Exception e)
            {
                View.ShowError("Failed to run controller", e);
            }
        }

        protected abstract void DoRun();
    }
}
