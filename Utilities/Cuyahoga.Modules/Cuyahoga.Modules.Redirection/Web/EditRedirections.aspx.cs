using System;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Util;
using Cuyahoga.Modules.Redirection.Domain;
using Cuyahoga.Web.UI;

namespace Cuyahoga.Modules.Redirection.Web
{
    public partial class EditRedirections : ModuleAdminBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnNew.Attributes.Add("onclick", String.Format("document.location.href='EditRedirection.aspx{0}&FileId=-1'", GetBaseQueryString()));

            rptFiles.DataSource = RedirectionModule.GetAllUrls();
            rptFiles.DataBind();
        }

        public RedirectionModule RedirectionModule
        {
            get
            {
                return (RedirectionModule)Module;
            }
        }

        public void rptFiles_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            var file = (RedirectionUrl)e.Item.DataItem;
            if(file==null)
                return;

            var litDateModified = (Literal)e.Item.FindControl("litDateModified");
            litDateModified.Text = TimeZoneUtil.AdjustDateToUserTimeZone(file.DatePublished, this.User.Identity).ToString();

            var hplEdit = (HyperLink)e.Item.FindControl("hpledit");
            hplEdit.NavigateUrl = String.Format("~/Modules/Redirections/EditRedirection.aspx{0}&FileId={1}", GetBaseQueryString(), file.Id);
        }
    }
}