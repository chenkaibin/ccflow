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
    public partial class S2_WangLuoFenXi : System.Web.UI.Page
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
            string info = "";
            /*step1 : 发送前要检查是否有设备没有启动。*/
            string sql = "SELECT COUNT(*) FROM tab_wf_commonkpioptivalue WHERE ParentWorkID="+this.WorkID+" AND WORKID=0";
            int numUnSendSubFlow=DBAccess.RunSQLReturnValInt(sql);
            if (numUnSendSubFlow > 0)
            {
                info += "<br>有" + numUnSendSubFlow+"设备没有发起子流程,所以您不能执行地市局的此流程发送.";
            }

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

            /*step3 : 执行向下发送。*/
            string msg= BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID).ToMsgOfHtml();

            //提示发送消息。
            this.Session["info"] = msg;
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