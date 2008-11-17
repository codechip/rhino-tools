using System;
using Cuyahoga.Core.Domain;
using Cuyahoga.Modules.Redirection.Domain;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.Redirection.Web
{
    public partial class EditRedirection : ModuleAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            UrlId = Int32.Parse(Request.QueryString["FileId"]);
            if (UrlId > 0)
            {
                Url = RedirectionModule.GetbyId(UrlId);
                if (!IsPostBack)
                    BindUrl();// I HATE WebForms
                btnDelete.Visible = true;
                btnDelete.Attributes.Add("onclick", "return confirm('Are you sure?');");
            }
            else
            {
                Url = new RedirectionUrl
                {
                    Section = RedirectionModule.Section
                };
                calDatePublished.SelectedDate = DateTime.Now;
                btnDelete.Visible = false;
            }
        }


        private void BindUrl()
        {
            txtUrl.Text = Url.Url;
            txtTitle.Text = Url.Title;
            calDatePublished.SelectedDate = Url.DatePublished;
        }

        public void btnSave_Click(object sender, System.EventArgs e)
        {
            if (!IsValid)
                return;

            Url.Publisher = User.Identity as User;
            Url.Title = txtTitle.Text;
            Url.DatePublished = calDatePublished.SelectedDate;
            Url.Url = txtUrl.Text;
            
            try
            {
                RedirectionModule.Save(Url);
                Context.Response.Redirect("EditRedirections.aspx" + base.GetBaseQueryString());
            }
            catch (Exception ex)
            {
                ShowError("Error saving url: " + ex.Message);
            }
        }

        public void btnCancel_Click(object sender, System.EventArgs e)
        {
            Context.Response.Redirect("EditRedirections.aspx" + base.GetBaseQueryString());
        }

        public void btnDelete_Click(object sender, System.EventArgs e)
        {
            RedirectionModule.Delete(Url);
            Context.Response.Redirect("EditRedirections.aspx" + base.GetBaseQueryString());
        }

        public int UrlId { get; set; }
        public RedirectionUrl Url { get; set; }
        public RedirectionModule RedirectionModule
        {
            get
            {
                return (RedirectionModule)Module;
            }
        }
    }
}