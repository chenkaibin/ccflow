using System;
using System.Data;
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


namespace CCFlow.SDKFlowDemo.TeleComDemo.ShengJu
{
    public partial class S2_WatherSubFlow : System.Web.UI.Page
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
            try
            {
                BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Node,this.WorkID, BP.Web.WebUser.No);
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.Message);
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            string info = "";
            /*step2 : 检查子子流程是否都已经完成，如果没有完成了，就提示错误。*/
            GenerWorkFlows gwfs = BP.WF.Dev2Interface.DB_SubFlows(this.WorkID);
            if (gwfs.Count > 0)
            {
                info += "<br>如下子流程没有完成，您不能提交.";
                foreach (GenerWorkFlow item in gwfs)
                    info += "<br>" + item.Title;
            }
            if (string.IsNullOrEmpty(info) == false)
            {
                this.Response.Write(info);
                return;
            }

            string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID).ToMsgOfHtml();
            this.Response.Write("<font color=red>"+msg+"</font>");
        }
    }
}