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

namespace CCFlow.WF.UC
{
    public partial class FHLFlow : BP.Web.UC.UCBase3
    {
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Button Btn_Return
        {
            get
            {
                return this.ToolBar1.GetBtnByID("Btn_Return");
            }
        }
        public Button Btn_Del
        {
            get
            {
                return this.ToolBar1.GetBtnByID("Btn_Del");
            }
        }
        public Button Btn_Close
        {
            get
            {
                return this.ToolBar1.GetBtnByID("Btn_Close");
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "Deal Work";
            this.ToolBar1.Add("<input class=Btn type=button onclick=\"javascript:window.location.href='ReturnWorkSmall.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "'\" value='退回' >");
            this.ToolBar1.AddBtn("Btn_Del", "终止");
            this.ToolBar1.AddBtn("Btn_Close", "关闭");
            this.Btn_Close.OnClientClick = "window.close();";
            this.Btn_Del.OnClientClick = "return confirm('are you sure?')";
            this.Btn_Del.Click += new EventHandler(Btn_Del_Click);

            GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
            WorkFlow wf = new WorkFlow(this.FK_Flow, this.FID);
            WorkNode wn = new WorkNode(this.FID, gwf.FK_Node);
            WorkNode wnPri = wn.GetPreviousWorkNode_FHL(this.WorkID); // 他的上一个节点.
            try
            {
                this.UCEn1.BindColumn4(wnPri.HisWork, "ND" + wnPri.HisNode.NodeID);
                this.UCEn1.Add(wnPri.HisWork.WorkEndInfo);
            }
            catch
            {
                this.WinCloseWithMsg("此工作已经终止或者被删除。");
            }
        }
        void Btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                WorkFlow wf14 = new WorkFlow(this.FK_Flow, this.WorkID);
                wf14.DoDeleteWorkFlowByReal(true);
                this.Alert("执行成功.");
                this.WinClose();
            }
            catch (Exception ex)
            {
                this.WinCloseWithMsg(ex.Message);
            }
        }
    }
}