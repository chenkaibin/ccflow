using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Sys;
using BP.DA;
using BP.WF;

namespace CCFlow.WF.Admin.XAP
{
    public partial class Designer : System.Web.UI.Page
    {
        public bool IsCheckUpdate
        {
            get
            {
                return true;

                if (this.Request.QueryString["IsCheckUpdate"] == null)
                    return false;
                return true;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //如果没有Port_Dept 表就可能没有安装ccflow.
                DBAccess.RunSQL("SELECT * FROM Port_Dept WHERE 1=2");
            }
            catch
            {
                /*数据库链接不通或者有异常，说明没有安装.*/
                this.Response.Redirect("../DBInstall.aspx", true);
                return;
            }

            if (this.IsCheckUpdate == false)
            {
                #region 执行admin登陆.
                Emp emp = new Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp, true);
                }
                else
                {
                    emp.No = "admin";
                    emp.Name = "admin";
                    emp.FK_Dept = "01";
                    emp.Pass = "pub";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp, true);
                    //throw new Exception("admin 用户丢失，请注意大小写。");
                }
                #endregion 执行admin登陆.
                return;
            }

            string sql = "";
            string msg = "";
            try
            {
                msg = "@登陆时错误。";
                DBAccess.RunSQL("DELETE Sys_Enum WHERE EnumKey IN ('DeliveryWay','RunModel','OutTimeDeal')");

                BP.Port.Unit u = new BP.Port.Unit();
                u.CheckPhysicsTable();

                //部门
                BP.Port.Dept d = new BP.Port.Dept();
                d.CheckPhysicsTable();

                int i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Port_Unit");
                if (i == 0)
                    DBAccess.RunSQL("INSERT INTO Port_Unit (No,Name, ParentNo)VALUES('1','济南驰骋信息技术有限公司','-1')");

                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.CheckPhysicsTable();

                Flow fl = new Flow();
                fl.CheckPhysicsTable();

                Node nd = new Node();
                nd.CheckPhysicsTable();

                SMS sms = new SMS();
                sms.CheckPhysicsTable();

                #region 执行admin登陆. 2012-12-25 新版本.
                Emp emp = new Emp();
                emp.No = "admin";
                if (emp.RetrieveFromDBSources() == 1)
                {
                    BP.Web.WebUser.SignInOfGener(emp, true);
                }
                else
                {
                    emp.No = "admin";
                    emp.Name = "admin";
                    emp.FK_Dept = "01";
                    emp.Pass = "pub";
                    emp.Insert();
                    BP.Web.WebUser.SignInOfGener(emp, true);
                    //throw new Exception("admin 用户丢失，请注意大小写。");
                }
                #endregion 执行admin登陆.
            }
            catch (Exception ex)
            {
                 this.Response.Write("问题出处:" + ex.Message +"<hr>"+ msg + "<br>详细信息:@" + ex.StackTrace + "<br>@<a href='../DBInstall.aspx' >点这里到系统升级界面。</a>");
                return;
            }
        }
    }
}