using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Demo;
using System.Collections;


namespace CCFlow.SDKFlowDemo.TeleCom._2DiShi
{
    public partial class S1_PaiDan : System.Web.UI.Page
    {
        #region 流程引擎传来的变量.
        /// <summary>
        /// 工作ID，在建立草稿时已经产生了.
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        /// <summary>
        ///  流程编号.
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion 流程引擎传来的变量

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            //执行发送, 此发送，普通节点到分流点发送，并且指定接受人。
            SendReturnObjs sendObjs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, null, null, 0, "guobaogeng");

            //提示发送消息。
            this.Session["info"] = sendObjs.ToMsgOfHtml();
            this.Response.Redirect("ShowMsg.aspx?ss" + BP.DA.DataType.CurrentDataTime, true);
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {

        }
        protected void Btn_Track_Click(object sender, EventArgs e)
        {
            BP.WF.Dev2Interface.UI_Window_FlowChartTruck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
        }
    }
}