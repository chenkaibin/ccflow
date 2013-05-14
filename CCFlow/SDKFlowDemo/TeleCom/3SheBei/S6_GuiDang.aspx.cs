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
using BP.Web;
using BP.Port;
using System.Collections;
using System.Data;
namespace CCFlow.SDKFlowDemo.TeleCom._3SheBei
{
    public partial class S6_GuiDang : System.Web.UI.Page
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
        protected void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.ID)
            {
                case "Btn_Send":
                    this.Send();
                    break;
                case "Btn_Save":
                    this.Save();
                    break;
                case "Btn_Return":
                    BP.WF.Dev2Interface.UI_Window_Return(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                    break;
                case "Btn_Track":
                    BP.WF.Dev2Interface.UI_Window_FlowChartTruck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                    break;
                default:
                    throw new Exception("@业务逻辑未实现" + btn.Text);
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        public void Send()
        {
            // call 子子流程，执行发送并获取发送后返回对象。
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, null, null, 0,null);

            //提示发送消息。
            this.Session["info"] = objs.ToMsgOfHtml();
            this.Response.Redirect("ShowMsg.aspx?ss" + BP.DA.DataType.CurrentDataTime, true);
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
        }
    }
}