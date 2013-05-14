using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.WorkOpt
{
    public partial class WF_WorkOpt_CC : BP.Web.WebPage
    {
        #region 变量.
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 变量.

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Flow fl = new BP.WF.Flow(this.FK_Flow);
            string sql = "SELECT Title FROM "+fl.PTable+" WHERE OID="+this.WorkID;
            string title = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);
            if (title==null)
                this.Pub1.AddFieldSet("错误","系统出现异常，请联系管理员。");

            this.Title = "工作抄送";
            this.Pub1.AddTable("width='100%' border=1");
            this.Pub1.AddCaptionLeft("请选择或者输入人员(多个人员用逗号隔开),然后点发送按钮...");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("接受人:");
            TextBox tb = new TextBox();
            tb.ID = "TB_Accepter";
            tb.Width = 500;
            this.Pub1.AddTD(tb);
            Button mybtn = new Button();
            mybtn.CssClass = "Btn";
            mybtn.Text = "选择接受人";
            mybtn.OnClientClick += "javascript:ShowIt(" + tb.ClientID + ");";
            this.Pub1.AddTD(mybtn);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("标题:");
            tb = new TextBox();
            tb.ID = "TB_Title";
            tb.Width = 500;
            tb.Text = title;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("消<BR>息<BR>内<br>容");
            tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Width = 500;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 12;
            this.Pub1.AddTD("width='90%' colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "btn";
            btn.Click += new EventHandler(btn_Click);
            btn.Text = "执行抄送";
            this.Pub1.AddTD(btn);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            string accepters = this.Pub1.GetTextBoxByID("TB_Accepter").Text;
            accepters = accepters.Trim();
            if (string.IsNullOrEmpty(accepters))
            {
                this.Alert("接受人不能为空");
                return;
            }
            string title = this.Pub1.GetTextBoxByID("TB_Title").Text;
            if (string.IsNullOrEmpty(title))
            {
                this.Alert("标题不能为空");
                return;
            }
            string doc = this.Pub1.GetTextBoxByID("TB_Doc").Text;

            /*检查人员是否有问题.*/
            string[] emps = accepters.Split(',');
            BP.WF.Port.WFEmp myemp = new BP.WF.Port.WFEmp();
            string errMsg = "";
            foreach (string emp in emps)
            {
                if (string.IsNullOrEmpty(emp))
                    continue;

                myemp.No = emp;
                if (myemp.IsExits == false)
                    errMsg += "@人员(" + emp + ")拼写错误。";
            }

            if (string.IsNullOrEmpty(errMsg) == false)
            {
                this.Alert(errMsg);
                return;
            }

            foreach (string emp in emps)
            {
                if (string.IsNullOrEmpty(emp))
                    continue;
                myemp.No = emp;
                myemp.Retrieve();

                //执行抄送.
                BP.WF.Dev2Interface.Node_CC(this.FK_Flow,this.WorkID, emp, myemp.Name, title, doc);
            }

            this.WinCloseWithMsg("抄送成功...");
        }
    }
}