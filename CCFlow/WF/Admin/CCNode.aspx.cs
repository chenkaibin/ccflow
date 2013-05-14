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

    public partial class WF_Admin_CCNode : WebPage
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
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

        #endregion 属性
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Bind();
        }
        public void Bind()
        {
            Flow fl = new Flow(this.FK_Flow);

            this.Ucsys1.AddTable("width=100%");
            this.Ucsys1.AddCaptionLeft("为流程:" + fl.Name + "，设置抄送节点。");
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("步骤");
            this.Ucsys1.AddTDTitle("节点名称");
            this.Ucsys1.AddTREnd();

            Nodes nds = fl.HisNodes;
            FlowNodes fnds = new FlowNodes();
            fnds.Retrieve(FlowNodeAttr.FK_Flow, this.FK_Flow);

            foreach (BP.WF.Node nd in nds)
            {
                this.Ucsys1.AddTR();
                this.Ucsys1.AddTDIdx(nd.Step);
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + nd.NodeID;
                cb.Text = nd.Name;
                cb.Checked = fnds.Contains(FlowNodeAttr.FK_Node, nd.NodeID);
                this.Ucsys1.AddTD(cb);
                this.Ucsys1.AddTREnd();
            }

            this.Ucsys1.AddTRSum();
            this.Ucsys1.AddTD();
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "  保 存  ";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Ucsys1.AddTD(btn);
            this.Ucsys1.AddTREnd();
            this.Ucsys1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            Flow fl = new Flow(this.FK_Flow);

            FlowNodes fnds = new FlowNodes();
            fnds.Delete(FlowNodeAttr.FK_Flow, this.FK_Flow);

            BP.WF.Nodes nds = fl.HisNodes;
            foreach (BP.WF.Node nd in nds)
            {
                CheckBox cb = this.Ucsys1.GetCBByID("CB_" + nd.NodeID);
                if (cb.Checked == false)
                    continue;


                FlowNode fn = new FlowNode();
                fn.FK_Flow = this.FK_Flow;
                fn.FK_Node = nd.NodeID;
                fn.Insert();
            }
            this.Response.Redirect(this.Request.RawUrl, true);
        }
    }

}