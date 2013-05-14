using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.Admin.UC
{
    public partial class WF_Admin_UC_CondDept : BP.Web.UC.UCBase3
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
        public int FK_Attr
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Attr"]);
                }
                catch
                {
                    try
                    {
                        return this.DDL_Attr.SelectedItemIntVal;
                    }
                    catch
                    {
                        return 0;
                    }
                }
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
        public string GetOperValText
        {
            get
            {
                if (this.Pub1.IsExit("TB_Val"))
                    return this.Pub1.GetTBByID("TB_Val").Text;
                return this.Pub1.GetDDLByID("DDL_Val").SelectedItem.Text;
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "部门条件";
            if (this.Request.QueryString["DoType"] == "Del")
            {
                Cond nd = new Cond(this.MyPK);
                nd.Delete();
                this.Response.Redirect("CondDept.aspx?CondType=" + (int)this.HisCondType + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + nd.NodeID + "&FK_Node=" + this.FK_MainNode + "&ToNodeID=" + nd.ToNodeID, true);
                return;
            }
            this.BindCond();
        }
        public void BindCond()
        {
            Cond cond = new Cond();
            cond.MyPK = this.GenerMyPK;
            cond.RetrieveFromDBSources();

            BP.WF.Node nd = new BP.WF.Node(this.FK_MainNode);
            BP.WF.Node tond = new BP.WF.Node(this.ToNodeID);

            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("colspan=4", "部门选择");
            this.Pub1.AddTREnd();

            Depts sts = new Depts();
            sts.RetrieveAllFromDBSource();

            int idx = -1;
            foreach (Dept st in sts)
            {
                idx++;
                if (idx == 0)
                    this.Pub1.AddTR();

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + st.No;
                cb.Text = st.Name;
                if (cond.OperatorValue.ToString().Contains("@" + st.No + "@"))
                    cb.Checked = true;

                this.Pub1.AddTD(cb);

                if (idx == 3)
                {
                    idx = -1;
                    this.Pub1.AddTREnd();
                }
            }

            switch (idx)
            {
                case 0:
                    this.Pub1.AddTD();
                    this.Pub1.AddTD();
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                    break;
                case 1:
                    this.Pub1.AddTD();
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                    break;
                case 2:
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                    break;
            }


            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD class=TD colspan=4 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = "Save";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_Del";
            btn.CssClass = "Btn";
            btn.Text = "Delete";
            btn.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            this.Pub1.Add("</TD>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public Label Lab_Msg
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Msg");
            }
        }
        public Label Lab_Note
        {
            get
            {
                return this.Pub1.GetLabelByID("Lab_Note");
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public DDL DDL_Attr
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Attr");
            }
        }
        public DDL DDL_Oper
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_Oper");
            }
        }
        public DDL DDL_ConnJudgeWay
        {
            get
            {
                return this.Pub1.GetDDLByID("DDL_ConnJudgeWay");
            }
        }
        public string GenerMyPK
        {
            get
            {
                return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.Depts.ToString();
            }
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            Cond cond = new Cond();
            cond.Delete(CondAttr.ToNodeID, this.ToNodeID, CondAttr.DataFrom, (int)ConnDataFrom.Depts);

            Button btn = sender as Button;
            if (btn.ID == "Btn_Del")
            {
                this.Response.Redirect(this.Request.RawUrl, true);
                return;
            }

            cond.MyPK = this.GenerMyPK;
            if (cond.RetrieveFromDBSources() == 0)
            {
                cond.HisDataFrom = ConnDataFrom.Depts;
                cond.NodeID = this.FK_MainNode;
                cond.FK_Flow = this.FK_Flow;
                cond.ToNodeID = this.ToNodeID;
                cond.Insert();
            }

            string val = "";
            Depts sts = new Depts();
            sts.RetrieveAllFromDBSource();
            foreach (Dept st in sts)
            {
                if (this.Pub1.IsExit("CB_" + st.No) == false)
                    continue;
                if (this.Pub1.GetCBByID("CB_" + st.No).Checked)
                    val += "@" + st.No;
            }
            if (val == "")
            {
                cond.Delete();
                return;
            }

            val += "@";
            cond.OperatorValue = val;
            cond.FK_Flow = this.FK_Flow;
            cond.HisCondType = this.HisCondType;
            cond.FK_Node = this.FK_Node;
            switch (this.HisCondType)
            {
                case CondType.Flow:
                case CondType.Node:
                    cond.Update();
                    this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr, true);
                    return;
                case CondType.Dir:
                    cond.ToNodeID = this.ToNodeID;
                    cond.Update();
                    this.Response.Redirect("CondDept.aspx?MyPK=" + cond.MyPK + "&FK_Flow=" + cond.FK_Flow + "&FK_Node=" + cond.FK_Node + "&FK_MainNode=" + cond.NodeID + "&CondType=" + (int)cond.HisCondType + "&FK_Attr=" + cond.FK_Attr + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    return;
                default:
                    throw new Exception("未设计的情况。");
            }
        }
    }
}