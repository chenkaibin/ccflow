using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_Action : WebPage
    {
        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }
        public string NodeID
        {
            get
            {
                return this.Request.QueryString["NodeID"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return "ND" + this.Request.QueryString["NodeID"];
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
            if (this.DoType == "Del")
            {
                FrmEvent delFE = new FrmEvent();
                delFE.MyPK = this.FK_MapData + "_" + this.Request.QueryString["RefXml"];
                delFE.Delete();
            }

            this.Pub3.AddCaptionLeft("节点表单/节点/流程:事件");

            FrmEvents ndevs = new FrmEvents();
            ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);

            EventLists xmls = new EventLists();
            xmls.RetrieveAll();

            BP.WF.XML.EventSources ess = new EventSources();
            ess.RetrieveAll();

            string myEvent = this.Event;
            BP.WF.XML.EventList myEnentXml = null;
            foreach (EventSource item in ess)
            {
                this.Pub1.AddFieldSet(item.Name);
                this.Pub1.AddUL();
                foreach (BP.WF.XML.EventList xml in xmls)
                {
                    if (xml.EventType != item.No)
                        continue;

                    FrmEvent nde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, xml.No) as FrmEvent;
                    if (nde == null)
                    {
                        if (myEvent == xml.No)
                        {
                            myEnentXml = xml;
                            this.Pub1.AddLi("<font color=green><b>" + xml.Name + "</b></font>");
                        }
                        else
                            this.Pub1.AddLi("Action.aspx?NodeID=" + this.NodeID + "&Event=" + xml.No + "&FK_Flow=" + this.FK_Flow, xml.Name);
                    }
                    else
                    {
                        if (myEvent == xml.No)
                        {
                            myEnentXml = xml;
                            this.Pub1.AddLi("<font color=green><b>" + xml.Name + "</b></font>");
                        }
                        else
                        {
                            this.Pub1.AddLi("Action.aspx?NodeID=" + this.NodeID + "&Event=" + xml.No + "&MyPK=" + nde.MyPK + "&FK_Flow=" + this.FK_Flow, "<b>" + xml.Name + "</b>");
                        }
                    }
                }
                this.Pub1.AddULEnd();
                this.Pub1.AddFieldSetEnd();
            }

            if (myEnentXml == null)
            {
                this.Pub2.AddFieldSet("帮助");
                this.Pub2.AddH2("事件是ccflow与您的应用程序接口，");
                this.Pub2.AddFieldSetEnd();
                return;
            }

            FrmEvent mynde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, myEvent) as FrmEvent;
            if (mynde == null)
                mynde = new FrmEvent();


            this.Title = "设置:事件接口=》" + myEnentXml.Name;

            this.Pub2.AddFieldSet(myEnentXml.Name);
            int col = 80;
            this.Pub2.Add("内容类型:");
            DDL ddl = new DDL();
            ddl.BindSysEnum("EventDoType");
            ddl.ID = "DDL_EventDoType";
            ddl.SetSelectItem((int)mynde.HisDoType);
            this.Pub2.Add(ddl);

            this.Pub2.Add("&nbsp;要执行的内容<br>");
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Columns = col;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 10;
            tb.Text = mynde.DoDoc;
            this.Pub2.Add(tb);
            this.Pub2.AddBR();

            tb = new TextBox();
            tb.ID = "TB_MsgOK";
            tb.Columns = col;
            tb.Text = mynde.MsgOKString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 4;

            this.Pub2.Add("执行成功信息提示(可为空)<br>");
            this.Pub2.Add(tb);
            this.Pub2.AddBR();

            this.Pub2.Add("执行失败信息提示(可为空)<br>");
            tb = new TextBox();
            tb.ID = "TB_MsgErr";
            tb.Columns = col;
            tb.Text = mynde.MsgErrorString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 4;
            this.Pub2.Add(tb);
            this.Pub2.AddBR();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "Save";
            btn.CssClass = "Btn";

            btn.Click += new EventHandler(btn_Click);
            this.Pub2.Add("&nbsp;&nbsp;");
            this.Pub2.Add(btn);
            if (this.MyPK != null)
                this.Pub2.Add("&nbsp;&nbsp;<a href=\"javascript:DoDel('" + this.NodeID + "','" + this.Event + "')\"><img src='/WF/Img/Btn/Delete.gif' />删除</a>");
            this.Pub2.AddFieldSetEnd();

        }
        void btn_Click(object sender, EventArgs e)
        {
            FrmEvent fe = new FrmEvent();
            fe.MyPK = this.FK_MapData + "_" + this.Event;
            fe.RetrieveFromDBSources();

            EventLists xmls = new EventLists();
            xmls.RetrieveAll();
            foreach (EventList xml in xmls)
            {
                if (xml.No != this.Event)
                    continue;

                string doc = this.Pub2.GetTextBoxByID("TB_Doc").Text.Trim();
                if (doc == "" || doc == null)
                {
                    if (fe.MyPK.Length > 3)
                        fe.Delete();
                    continue;
                }

                fe.MyPK = this.FK_MapData + "_" + xml.No;
                fe.DoDoc = doc;
                fe.FK_Event = xml.No;
                fe.FK_MapData = this.FK_MapData;
                fe.HisDoType = (EventDoType)this.Pub2.GetDDLByID("DDL_EventDoType").SelectedItemIntVal;
                fe.MsgOKString = this.Pub2.GetTextBoxByID("TB_MsgOK").Text;
                fe.MsgErrorString = this.Pub2.GetTextBoxByID("TB_MsgErr").Text;
                fe.Save();
                this.Response.Redirect("Action.aspx?NodeID=" + this.NodeID + "&MyPK=" + fe.MyPK + "&Event=" + xml.No, true);
                return;
            }
        }
    }
}