using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Controllers
{
    public class ReportsMenuController : BaseMenuController
    {
        public ReportsMenuController()
        {
            View.AddCommand("Books Checked Out By User", NotImplemented);
            View.AddCommand("Copies By Book", NotImplemented);
            View.AddCommand("Late Returns", NotImplemented);
            View.AddCommand("Fines", NotImplemented);
        }
    }
}
