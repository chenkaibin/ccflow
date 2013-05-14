using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace CCFlow.WF.WAP
{
    public partial class WAP_Do : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Redirect(this.Request.RawUrl.Replace("/WAP/", "/WF/"), true);
        }
    }
}
