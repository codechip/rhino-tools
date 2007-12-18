using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace RhinoIglooSample.Web
{
    using Controllers;
    using Rhino.Igloo;

    public partial class Login2 : BasePage
    {
        private LoginController controller;

        public LoginController Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            Result.Text = Controller
                .Authenticate()
                .ToString();
        }
    }
}
