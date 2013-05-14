using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_DBInstall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检查一下数据库是否链接成功.
            try
            {
                switch (BP.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 ");
                        break;
                    case DBType.Oracle:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 FROM DUAL ");
                        break;
                    case DBType.Informix:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 FROM DUAL ");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Response.Write("<h1>数据库连接错误</h1><hr> <font color=red>请参考安装说明书检查web.config数据库连接设置，错误信息:</font><br>" + ex.Message);
                return;
            }
            #endregion

            this.Pub1.AddH3("ccflow 数据库修复与安装工具");
            this.Pub1.AddHR();
            if (this.Request.QueryString["DoType"] == "OK")
            {
                this.Pub1.AddFieldSet("提示");
                this.Pub1.Add("ccflow数据库初始化成功.");
                this.Pub1.AddBR("<a href='./XAP/Designer.aspx?IsCheckUpdate=1' >进入流程设计器.</a>");
                this.Response.Redirect("./XAP/Designer.aspx?IsCheckUpdate=1", true);
                this.Pub1.AddFieldSetEnd();
                return;
            }

            //try
            //{
            if (BP.DA.DBAccess.IsExitsObject("WF_Flow") == true)
            {
                this.Pub1.AddFieldSet("提示");
                this.Pub1.Add("数据已经安装，如果您要重新安装，您需要手工的清除数据库里对象。");
                this.Pub1.AddFieldSetEnd();

                this.Pub1.AddFieldSet("修复数据表");
                this.Pub1.Add("把最新的版本的与当前的数据表结构，做一个自动修复, 修复内容：缺少列，缺少列注释，列注释不完整或者有变化。 <br> <a href='DBInstall.aspx?DoType=FixDB' >执行...</a>。");
                this.Pub1.AddFieldSetEnd();

                if (this.Request.QueryString["DoType"] == "FixDB")
                {
                    string rpt = BP.PubClass.DBRpt(BP.DBLevel.High);
                    this.Pub1.AddMsgGreen("同步数据表结构成功, 部分错误不会影响系统运行.",
                        "执行成功，希望在系统每次升级后执行此功能，不会对你的数据库数据产生影响。<br> <a href='./XAP/Designer.aspx'>进入流程设计器.</a>");
                    //string scrpts = BP.SystemConfig.PhysicalApplicationPath + "\\WF\\Admin\\DBInstall.sql";
                    //BP.DA.DBAccess.RunSQLScript(scrpts);
                }
                return;
            }

            //}
            //catch (Exception ex)
            //{
            //    this.Pub1.AddFieldSet("提示:数据库连接没有配置好");
            //    this.Pub1.Add("1, 请打开web.config文件配置 appSettings - AppCenterDSN 节点中的数据库连接信息。");
            //    this.Pub1.AddBR("2, 支持的数据库类型在，AppCenterDBType中配置，分别是MSSQL2000,Oracle,DB2,Access,MySQL ");
            //    this.Pub1.AddBR("<hr>错误信息:" + ex.Message);
            //    this.Pub1.AddFieldSetEnd();
            //    return;
            //}

            // this.Pub1.AddH2("数据库安装向导...");

            this.Pub1.AddFieldSet("选择安装语言.");
            BP.WF.XML.Langs langs = new BP.WF.XML.Langs();
            langs.RetrieveAll();
            RadioButton rb = new RadioButton();
            foreach (BP.WF.XML.Lang lang in langs)
            {
                rb = new RadioButton();
                rb.Text = lang.Name;
                rb.ID = "RB_" + lang.No;
                rb.GroupName = "ch";
                this.Pub1.Add(rb);
                this.Pub1.AddBR();
            }
            this.Pub1.GetRadioButtonByID("RB_CH").Checked = true;
            this.Pub1.AddFieldSetEndBR();


            this.Pub1.AddFieldSet("选择数据库安装类型.");
            rb = new RadioButton();
            rb.Text = "SQLServer2000,2005,2008";
            rb.ID = "RB_SQL";
            rb.GroupName = "sd";
            rb.Checked = true;
            this.Pub1.Add(rb);
            this.Pub1.AddBR();

            rb = new RadioButton();
            rb.Text = "Oracle,Oracle 10g";
            rb.ID = "RB_Oracle";
            rb.GroupName = "sd";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();

            rb = new RadioButton();
            rb.Text = "Informix (首先需要执行:D:\\ccflow\\trunk\\CCFlow\\WF\\Data\\Install\\Informix.sql)";
            rb.ID = "RB_DB2";
            rb.GroupName = "sd";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();

            rb = new RadioButton();
            rb.Text = "MySQL";
            rb.ID = "RB_MYSQL";
            rb.GroupName = "sd";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            this.Pub1.AddFieldSetEnd();


            this.Pub1.AddFieldSet("应用环境模拟.");
            rb = new RadioButton();
            rb.Text = "集团公司，企业单位。";
            rb.ID = "RB_Inc";
            rb.GroupName = "hj";
            rb.Checked = true;
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            rb = new RadioButton();
            rb.Text = "政府机关，事业单位。";
            rb.ID = "RB_Gov";
            rb.GroupName = "hj";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            this.Pub1.AddFieldSetEndBR();

            this.Pub1.AddFieldSet("是否装载演示流程模板?");
            rb = new RadioButton();
            rb.Text = "是:我要安装demo流程模板、表单模板，以方便我学习ccflow与ccform.";
            rb.ID = "RB_DemoOn";
            rb.GroupName = "hjd";
            rb.Checked = true;
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            rb = new RadioButton();
            rb.Text = "否:不安装。";
            rb.ID = "RB_DemoOff";
            rb.GroupName = "hjd";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            this.Pub1.AddFieldSetEndBR();

            Button btn = new Button();
            btn.ID = "Btn_s";
            btn.Text = "接受CCFlow的LPGL开源软件协议并安装(估计在3-5分钟内安装完成)";
            btn.CssClass = "Btn";
            btn.UseSubmitBehavior = false;
            btn.OnClientClick = "this.disabled=true;";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }
        void btn_Click(object sender, EventArgs e)
        {
            string lang = "CH";
            string db = "SQLServer";
            string hj = "Inc";

            if (this.Pub1.GetRadioButtonByID("RB_SQL").Checked)
                db = "SQLServer";

            if (this.Pub1.GetRadioButtonByID("RB_Oracle").Checked)
                db = "Oracle";

            if (this.Pub1.GetRadioButtonByID("RB_DB2").Checked)
                db = "DB2";

            if (this.Pub1.GetRadioButtonByID("RB_MYSQL").Checked)
                db = "MySQL";

            BP.WF.XML.Langs langs = new BP.WF.XML.Langs();
            langs.RetrieveAll();
            foreach (BP.WF.XML.Lang xml in langs)
            {
                if (this.Pub1.GetRadioButtonByID("RB_" + xml.No).Checked)
                    lang = xml.No;
            }

            if (this.Pub1.GetRadioButtonByID("RB_Inc").Checked)
                hj = "Inc";

            if (this.Pub1.GetRadioButtonByID("RB_Gov").Checked)
                hj = "Gov";

            hj = "Inc";
            //运行。
            BP.WF.Glo.DoInstallDataBase(lang, hj,this.Pub1.GetRadioButtonByID("RB_DemoOn").Checked);

            //加注释.
            BP.PubClass.AddComment();

            // 装载演示数据.
            if (this.Pub1.GetRadioButtonByID("RB_DemoOn").Checked)
            {
                BP.Port.Emp emp = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(emp);
                BP.WF.DTS.LoadTemplete l = new BP.WF.DTS.LoadTemplete();
                string msg = l.Do() as string;
            }

            try
            {
                //增加图片签名
                BP.WF.DTS.GenerSiganture gs = new BP.WF.DTS.GenerSiganture();
                gs.Do();
            }
            catch
            {
            }


            this.Response.Redirect("DBInstall.aspx?DoType=OK", true);
        }
    }
}