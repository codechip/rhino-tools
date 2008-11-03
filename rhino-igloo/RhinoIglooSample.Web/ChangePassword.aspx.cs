namespace RhinoIglooSample.Web
{
    using System;
    using Controllers;
    using Rhino.Igloo;

    public partial class ChangePassword : BasePage
    {
        private ChangePasswordController controller;

        public ChangePasswordController Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            if(Controller.ChangePassword())
            {
                SuccessMessage.Text = Scope.SuccessMessage;
            }
            else
            {
                ErrorMessage.Text = Scope.ErrorMessage;
            }
        }
    }
}
