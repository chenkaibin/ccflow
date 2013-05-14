using System;
using System.Collections;
using System.Configuration;
using System.Data;
using BP;
using System.Web;
using System.Web.Security;
using System.Web.UI; 
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
public partial class Face_MasterPage : BP.Web.MasterPage
{
    private string _pageID = null;
    public string PageID
    {
        get
        {
            if (_pageID == null)
            {
                string url = System.Web.HttpContext.Current.Request.RawUrl;
                int i = url.LastIndexOf("/") + 1;
                int i2 = url.IndexOf(".aspx") - 5;
                try
                {
                    url = url.Substring(i);
                    _pageID = url.Substring(0, url.IndexOf(".aspx"));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + url + " i=" + i + " i2=" + i2);
                }
            }
            return _pageID;
        }
    }
    /// <summary>
    /// DoType
    /// </summary>
    public string DoType
    {
        get
        {
            return "";
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.RegisterClientScriptBlock("s",
            "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

        if (this.Request.RawUrl.ToLower().Contains("login.aspx") == false)
        {
            if (BP.Web.WebUser.No == null)
            {
                this.Response.Redirect("Login.aspx", true);
                return;
            }
        }

        Response.AddHeader("P3P", "CP=CAO PSA OUR");
        Response.AddHeader("Cache-Control", "no-store");
        Response.AddHeader("Expires", "0");
        Response.AddHeader("Pragma", "no-cache");

        BP.WF.XML.ToolBars ens = new BP.WF.XML.ToolBars();
        ens.RetrieveAll();

        int numCC = 0;
        int num = 0;
        if (BP.Web.WebUser.NoOfRel != null)
        {
            BP.DA.Paras ps = new BP.DA.Paras();
            string sql, sql2;
            if (BP.Web.WebUser.IsAuthorize)
            {
                BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(BP.Web.WebUser.No);
                ps.SQL = "SELECT COUNT(*) AS Num FROM WF_EmpWorks WHERE ( WFState=" + (int)BP.WF.WFState.Runing + " ) AND FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp  AND FK_Flow IN " + emp.AuthorFlows;
                ps.AddFK_Emp();
            }
            else
            {
                ps.AddFK_Emp();
                ps.SQL = "SELECT COUNT(*) AS Num FROM WF_EmpWorks WHERE WFState=" + (int)BP.WF.WFState.Runing + " and FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            }
            num = BP.DA.DBAccess.RunSQLReturnValInt(ps);

            ps = new BP.DA.Paras();
            ps.SQL = "SELECT COUNT(MyPK) FROM WF_CCList WHERE Sta=0 AND CCTo=" + BP.SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.AddFK_Emp();

            numCC = BP.DA.DBAccess.RunSQLReturnValInt(ps);
        }

        string msg = "待办";
        string msgCC = "抄送";
        if (num != 0)
        {
            msg = "<div id='blink'>待办-" + num + "</div>";
            string script = "";
            script += "<script language=javascript>";
            script += "function changeColor(){";
            script += " var color='#f00|#0f0|#00f|#880|#808|#088|yellow|green|blue|gray';";
            script += " color=color.split('|'); ";
            script += " try{";
            script += " document.getElementById('blink').style.color=color[parseInt(Math.random() * color.length)] ";
            script += " catch ()";
            script += " }";
            script += " setInterval('changeColor()',200);";
            script += "</script> ";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "s", script);
        }
        if (numCC != 0)
        {
            msgCC = "<div id='blink'>抄送-" + numCC + "</div>";
            string script = "";
            script += "<script language=javascript>";
            script += "function changeColor1(){";
            script += " var color='#f00|#0f0|#00f|#880|#808|#088|yellow|green|blue|gray';";
            script += " color=color.split('|'); ";
            script += " document.getElementById('blink').style.color=color[parseInt(Math.random() * color.length)] ";
            script += " }";
            script += " setInterval('changeColor1()',300);";
            script += "</script> ";
            this.Page.ClientScript.RegisterClientScriptBlock(typeof(string), "scc", script);
        }

        #region 菜单输出区域


        string dotype = this.PageID;
        if (BP.WF.Glo.IsShowTitle)
        {
            this.Pub1.Add("<DIV class='wp' >");
            this.Pub1.Add("<div  id='Top' class='top' /><h2><img src='/DataUser/ICON/" + SystemConfig.CompanyID + "/LogBiger.png'  border=0 /></h2></div>");
        }

        this.Page.RegisterClientScriptBlock("d",
           "<link href='/DataUser/Style/Skin/T" + BP.Web.WebUser.Style + "/Style.css' rel='stylesheet' type='text/css' />");

        this.Pub1.Add("<DIV ID=nv class='nv' >");
        this.Pub1.Add("<UL>");
        foreach (BP.WF.XML.ToolBar en in ens)
        {
            if (en.No == dotype)
            {
                if (en.No == "EmpWorks")
                    this.Pub1.Add("<li class=current ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + msg + "</span></a></li>");
                else if (en.No == "CC")
                    this.Pub1.Add("<li class=current ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + msgCC + "</span></a></li>");
                else
                    this.Pub1.Add("<li class=current ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + en.Name + "</span></a></li>");

            }
            else
            {
                if (en.No == "EmpWorks")
                    this.Pub1.Add("<li class='Barli' ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + msg + "</span></a></li>");
                else if (en.No == "CC")
                    this.Pub1.Add("<li class='Barli' ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + msgCC + "</span></a></li>");
                else
                    this.Pub1.Add("<li class='Barli' ><a href=\"" + en.Url + "\" target='_self' title='" + en.Title + "' ><span>" + en.Name + "</span></a></li>");
            }
        }

        if (BP.Web.WebUser.NoOfRel == null)
            this.Pub1.Add("</UL> <div style='float:right;margin-right:30px;display:inline-block;line-height:35px;color:white' >未登录</div>");
        else
            this.Pub1.Add("</UL> <div style='float:right;margin-right:30px;display:inline-block;line-height:35px;color:white' >您好:" + BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + "</div>");

        this.Pub1.Add("</DIV>");
        if (BP.WF.Glo.IsShowTitle)
            this.Pub1.Add("</DIV>");
        #endregion 菜单输出区域
    }
}
