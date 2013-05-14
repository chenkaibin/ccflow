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
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_TestFlow : WebPage
    {
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        public void BindFlowList()
        {
            this.Title = "感谢您选择驰骋工作流程引擎-流程设计&测试界面";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Browser.Cookies == false)
            {
                this.Response.Write("您的浏览器不支持cookies功能，无法使用改系统。");
                return;
            }

            //   if (this.Request.QueryString["IsTest"] == "1")

            BP.SystemConfig.DoClearCash_del();

            Emp emp1 = new Emp("admin");
            WebUser.SignInOfGenerLang(emp1, this.Lang);

            // this.BindFlowList();
            if (this.FK_Flow == null)
            {
                this.Ucsys1.AddFieldSet("关于流程测试");
                this.Ucsys1.AddUL();
                this.Ucsys1.AddLi("现在是流程测试状态，此功能紧紧提供给流程设计人员使用。");
                this.Ucsys1.AddLi("提供此功能的目的是，快速的让各个角色人员登陆，以便减少登陆的繁琐麻烦。");
                this.Ucsys1.AddLi("点左边的流程列表后，系统自动显示能够发起此流程的工作人员，点一个工作人员就直接登陆了。");
                this.Ucsys1.AddULEnd();
                this.Ucsys1.AddFieldSetEnd();
                return;
            }

            if (this.RefNo != null)
            {
                Emp emp = new Emp(this.RefNo);
                BP.Web.WebUser.SignInOfGenerLang(emp, this.Lang);
                this.Session["FK_Flow"] = this.FK_Flow;
                if (this.Request.QueryString["Type"] != null)
                {
                    string url = "../../WAP/MyFlow.aspx?FK_Flow=" + this.FK_Flow;
                    if (this.Request.QueryString["IsWap"] == "1")
                        this.Response.Redirect("../../WF/WAP/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01", true);
                    else
                        this.Response.Redirect("../../WF/MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + int.Parse(this.FK_Flow) + "01", true);
                }
                else
                {
                    this.Response.Redirect("../Port/Home.htm?FK_Flow=" + this.FK_Flow, true);
                }
                return;
            }

            BP.Web.WebUser.SysLang = this.Lang;
            Flow fl = new Flow(this.FK_Flow);
            fl.DoCheck();

            int nodeid = int.Parse(this.FK_Flow + "01");
            Emps emps = new Emps();
            emps.RetrieveInSQL_Order("select fk_emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nodeid + " )", "FK_Dept");

            if (emps.Count == 0)
                emps.RetrieveInSQL("select fk_emp from wf_NodeEmp WHERE fk_node=" + int.Parse(this.FK_Flow + "01") + " ");

            if (emps.Count == 0)
                emps.RetrieveInSQL("select no from port_emp where fk_dept in (select fk_dept from wf_nodeDept where fk_node='" + nodeid + "') ");

            BP.WF.Node nd = new BP.WF.Node(nodeid);
            if (emps.Count == 0)
                emps.RetrieveInSQL(nd.DeliveryParas);

            if (emps.Count == 0)
            {
                this.Ucsys1.AddMsgOfWarning("Error",
                      "错误原因 <h2>@1，可能是您没有正确的设置岗位、部门、人员。<br>@2，可能是没有给开始节点设置工作岗位。</h2>");
                return;
            }

            this.Ucsys1.AddFieldSet("可发起(<font color=red>" + fl.Name + "</font>)流程的人员");
            this.Ucsys1.AddTable("align=center");
            this.Ucsys1.AddCaptionLeft("流程编号:" + fl.No + " 名称:" + fl.Name);
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("Users");
            // this.Ucsys1.AddTDTitle("ccflow5版");
            this.Ucsys1.AddTDTitle("应用程序模式");
            this.Ucsys1.AddTDTitle("博客模式");
            //this.Ucsys1.AddTDTitle("特小窗口模式");
            //this.Ucsys1.AddTDTitle("手机模式");
            this.Ucsys1.AddTDTitle("Dept");
            //this.Ucsys1.AddTDTitle("SDK");
            this.Ucsys1.AddTREnd();
            bool is1 = false;
            foreach (Emp emp in emps)
            {
                is1 = this.Ucsys1.AddTR(is1);
                this.Ucsys1.AddTD(emp.No + "," + emp.Name);
                this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=Start5&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />Internet Explorer</a>");
                this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=Start&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />Internet Explorer</a>");                
                //this.Ucsys1.AddTD("<a href='./../Port.aspx?DoWhat=StartSmallSingle&UserNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "'  ><img src='./../Img/IE.gif' border=0 />Internet Explorer</a>");
                //this.Ucsys1.AddTD("<a href=\"javascript:WinOpen('TestFlow.aspx?RefNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&IsWap=1','470px','600px','" + emp.No + "');\"  ><img src='./../Img/Mobile.gif' border=0 width=25px height=18px />Mobile</a> ");
                this.Ucsys1.AddTD(emp.FK_DeptText);
                //this.Ucsys1.AddTD("<a href='TestSDK.aspx?RefNo=" + emp.No + "&FK_Flow=" + this.FK_Flow + "&Lang=" + BP.Web.WebUser.SysLang + "&Type=" + this.Request.QueryString["Type"] + "&IsWap=1'  >SDK</a> ");
                this.Ucsys1.AddTREnd();
            }
            this.Ucsys1.AddTableEndWithBR();
            this.Ucsys1.AddFieldSetEnd();
        }
    }
}