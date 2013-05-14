using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Demo;

namespace CCFlow.SDKFlowDemo.TeleComDemo.ShengJu
{
    public partial class S3_Checker : System.Web.UI.Page
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

        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID).ToMsgOfHtml();

            // 这里应当转向一个界面来显示这些信息。
            this.Session["info"] = msg;
            this.Response.Redirect("ShowMsg.aspx?ss=" + DataType.CurrentDataTime, true);
        }
    }
}