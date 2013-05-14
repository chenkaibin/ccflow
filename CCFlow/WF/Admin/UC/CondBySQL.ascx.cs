using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.Admin.UC
{
    public partial class WF_Admin_UC_CondBySQL : BP.Web.UC.UCBase3
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public new string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return this.FK_MainNode;
                }
            }
        }
        public int FK_MainNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_MainNode"]);
            }
        }
        public int ToNodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["ToNodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CondType HisCondType
        {
            get
            {
                return (CondType)int.Parse(this.Request.QueryString["CondType"]);
            }
        }
        public string GetOperVal
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItemStringVal;
            }
        }
        public string GetOperValText
        {
            get
            {
                if (this.IsExit("TB_Val"))
                    return this.GetTBByID("TB_Val").Text;
                return this.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        public string GenerMyPK
        {
            get
            {
                return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.SQL.ToString();
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "按SQL设置";

            Cond cond = new Cond();
            cond.MyPK = this.GenerMyPK;
            cond.RetrieveFromDBSources();

            this.AddBR();
            this.AddBR();

            this.AddFieldSet("设置SQL");
            TextBox tb = new TextBox();
            tb.ID = "TB_SQL";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 10;
            tb.Columns = 80;
            tb.Text = cond.OperatorValueStr;
            this.Add(tb);
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " Save ";
            this.AddBR();
            this.Add(btn);
            btn.Click += new EventHandler(btn_Click);

            btn = new Button();
            btn.ID = "Btn_Del";
            btn.CssClass = "Btn";
            btn.Text = "Delete";
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Click);
            this.Add(btn);

            this.AddHR();
            this.Add("<b>说明:</b>在文本框里设置一个SQL，它返回一行一列，此值必须为0或大于等于1的正<BR>整数, 来表示该条件是否成立。SQL 支持ccflow的表达式。");
            this.AddFieldSetEnd();

            //this.AddTable();
            //this.AddTR();
            //this.AddTDTitle("数据");
            //this.AddTREnd();
            //this.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
          

            Cond cond = new Cond();
            cond.Delete(CondAttr.ToNodeID, this.ToNodeID, CondAttr.DataFrom, (int)ConnDataFrom.SQL);
            Button btn = sender as Button;
            if (btn.ID == "Btn_Del")
            {
                this.Response.Redirect(this.Request.RawUrl, true);
                return;
            }

            string sql = this.GetTextBoxByID("TB_SQL").Text;
            if (string.IsNullOrEmpty(sql))
            {
                this.Alert("请填写sql语句.");
                return;
            }

            cond.MyPK = this.GenerMyPK;
            cond.HisDataFrom = ConnDataFrom.SQL;
            
            cond.NodeID = this.FK_MainNode;
            cond.FK_Node = this.FK_MainNode;

            cond.FK_Flow = this.FK_Flow;
            cond.ToNodeID = this.ToNodeID;
            cond.OperatorValue = sql;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;

            cond.Insert();
            this.Alert("保存成功");

            //switch (this.HisCondType)
            //{
            //    case CondType.Flow:
            //    case CondType.Node:
            //        cond.Update();
            //        this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr, true);
            //        return;
            //    case CondType.Dir:
            //        cond.ToNodeID = this.ToNodeID;
            //        cond.Update();
            //        this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
            //        return;
            //    default:
            //        throw new Exception("未设计的情况。");
            //}
        }
    }
}