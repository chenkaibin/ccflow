using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AppDemo_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.Request.QueryString["DoType"] == "Logout")
            BP.Web.WebUser.Exit();

        if (!Page.IsPostBack)
        {
            string strUser = string.Empty;
            string strPass = string.Empty;
            if (Request.QueryString["username"] != null)
            {
                strUser = Request.QueryString["username"].ToString();

                if (Request.QueryString["password"] != null)
                {
                    strPass = Request.QueryString["password"].ToString();
                    this.Login1.Login(strUser, strPass);
                }
            }

        }
    }
}