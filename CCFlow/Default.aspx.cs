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
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;

namespace CCFlow
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                
            if (this.Request.RawUrl.ToLower().Contains("wap"))
            {
                this.Response.Redirect("./WF/WAP/", true);
                return;
            }

            if (this.Request.QueryString["IsCheckUpdate"] == "1")
                this.Response.Redirect("/WF/Admin/XAP/Designer.aspx?IsCheckUpdate=1", true);
            else
                this.Response.Redirect("/WF/Admin/XAP/Designer.aspx", true);
            return;
        }
    }
}