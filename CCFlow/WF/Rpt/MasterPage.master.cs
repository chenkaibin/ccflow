using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.En;
using BP.WF.XML;

namespace CCFlow.WF.Rpt
{
    public partial class WF_Rpt_MasterPage : BP.Web.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("sds",
            "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            this.Page.RegisterClientScriptBlock("d",
          "<link href='/WF/Comm/Style/Tabs.css' rel='stylesheet' type='text/css' />");

            RptXmls xmls = new RptXmls();
            xmls.RetrieveAll();

            this.Pub1.Add("<div id='tabsJ' style='height=30px;' >");
            this.Pub1.AddUL();
            foreach (RptXml item in xmls)
            {
                if (item.No == this.PageID + "_" + this.DoType)
                    this.Pub1.Add("<li><a href=#><span><b><img src='" + item.ICON + "' border=0 />" + item.Name + "</b></span></a></li>");
                else
                    this.Pub1.AddLi("<a href='" + item.URL + "&FK_Flow=" + this.Request["FK_Flow"] + "'><span><img src='" + item.ICON + "' border=0 />" + item.Name + "</span></a>");
            }
            this.Pub1.AddULEnd();
            this.Pub1.AddDivEnd();
        }
    }
}