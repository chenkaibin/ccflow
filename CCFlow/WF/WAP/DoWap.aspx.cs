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
using BP.Web;
using BP.DA;
using BP.WF;
using BP.En;

namespace CCFlow.WF.WAP
{
    public partial class WAP_DoWap : WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            switch (this.DoType)
            {
                case "Out":
                    WebUser.Exit();
                    this.Response.Redirect("Login.aspx", true);
                    break;
                default:
                    break;
            }


        }
    }
}
