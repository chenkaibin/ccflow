using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.OneWork
{
public partial class WF_WorkOpt_OneWork : BP.Web.MasterPage
{
    public string WorkID
    {
        get
        {
            return this.Request.QueryString["WorkID"];
        }
    }
    public string FK_Node
    {
        get
        {
            return this.Request.QueryString["FK_Node"];
        }
    }
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.RegisterClientScriptBlock("s",
    "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

        OneWorkXmls xmls = new OneWorkXmls();
        xmls.RetrieveAll();

        string pageId = this.PageID;
        this.Pub1.Add("\t\n<div id='tabsJ'  align='center'>");
        this.Pub1.Add("\t\n<ul>");
        foreach (BP.WF.XML.OneWorkXml item in xmls)
        {
            string url = item.No + ".aspx?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
            if (item.No == pageId)
            {
                this.Page.Title = "OneWork:"+item.Name;
                this.Pub1.AddLi("<a href=\"" + url + "\" ><span><b>" + item.Name + "</b></span></a>");
            }
            else
                this.Pub1.AddLi("<a href=\"" + url + "\" ><span>" + item.Name + "</span></a>");
        }
        this.Pub1.Add("\t\n</ul>");
        this.Pub1.Add("\t\n</div>");
    }
}

}