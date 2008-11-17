using System;
using System.Web.UI.WebControls;
using Cuyahoga.Core.Util;
using Cuyahoga.Modules.Redirection.Domain;
using Cuyahoga.Web.UI;
using Cuyahoga.Web.Util;

namespace Cuyahoga.Modules.Redirection.Web
{
    public partial class Redirections : BaseModuleControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(RedirectionModule.ShouldRedirect)
            {
                string url = RedirectionModule.RedirectToCurrentFile();
                Response.Redirect(url);
            }
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

        public void rptFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var redirectionUrl = (RedirectionUrl)e.Item.DataItem;

            var downloadUrl = UrlHelper.GetUrlFromSection(RedirectionModule.Section) + "/redirect/" + redirectionUrl.Id;
            var hplFileImg = (HyperLink)e.Item.FindControl("hplFileImg");
            hplFileImg.NavigateUrl = downloadUrl;
            hplFileImg.ImageUrl = base.Page.ResolveUrl("~/Modules/Downloads/Images/"
                                                       + FileTypesMap.GetIconFilename(System.IO.Path.GetExtension(redirectionUrl.Url)));

            var hplFile = (HyperLink)e.Item.FindControl("hplFile") ;
            hplFile.NavigateUrl = downloadUrl;

            var lblDateModified = (Label)e.Item.FindControl("lblDateModified");
            lblDateModified.Text = TimeZoneUtil.AdjustDateToUserTimeZone(
                redirectionUrl.DatePublished, this.Page.User.Identity).ToString();
            
            var lblPublisher = (Label)e.Item.FindControl("lblPublisher");
            lblPublisher.Text += " Published at " + redirectionUrl.Publisher.FullName;
         
            var lblNumberOfDownloads = (Label)e.Item.FindControl("lblNumberOfDownloads");
            lblNumberOfDownloads.Text += "Downloads: " + redirectionUrl.NumberOfDownloads;

        }
    }
}