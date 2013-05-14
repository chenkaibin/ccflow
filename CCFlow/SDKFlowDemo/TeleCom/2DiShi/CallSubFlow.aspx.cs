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
    public partial class CallSubFlow : System.Web.UI.Page
    {
        #region 流程引擎传来的变量.
        /// <summary>
        /// 父流程ID
        /// </summary>
        public Int64 ParentWorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["ParentWorkID"]);
            }
        }
        #endregion 流程引擎传来的变量

        protected void Page_Load(object sender, EventArgs e)
        {
            int shebeiID = int.Parse(this.Request.QueryString["SheBeiID"]);
            /*判断是否被发送过.*/
            BP.Demo.tab_wf_commonkpioptivalue en = new BP.Demo.tab_wf_commonkpioptivalue();
            en.OID = shebeiID;
            en.Retrieve(); //根据设备ID 查询出来该设备的信息.
            if (en.WorkID != 0)
                throw new Exception("@此流程已经启动了。");
        }
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            //step1: 吊起子子流程。
            Int64 workid  = BP.WF.Dev2Interface.Node_CreateBlankWork("027", null, null,BP.Web.WebUser.No, 
                "自动调用"+BP.Web.WebUser.Name,
                this.ParentWorkID,"026");

            //step2: 执行子子流程的发送.
            Hashtable ht = new Hashtable();
            if (this.CB_IsShiShi.Checked)
                ht.Add("IsShiShi", 1);
            else
                ht.Add("IsShiShi", 0);
            ht.Add("OID", workid);

            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("027", workid, ht, null,0, this.TB_FZR.Text);

            //step3: 更新设备WorkID. 
            int shebeiID = int.Parse(this.Request.QueryString["SheBeiID"]);
            BP.Demo.tab_wf_commonkpioptivalue en = new BP.Demo.tab_wf_commonkpioptivalue();
            en.OID = shebeiID;
            en.Retrieve(); //根据设备ID 查询出来该设备的信息
            en.WorkID = (int)objs.VarWorkID; 
            //从返回对象里，获取它的子子流程WorkID，从而标记上来此设备启动的Workid，与利用此workid来关联该流程的状态。
            en.Update();

             //step4: 提示发送消息。
            this.Session["info"] = objs.ToMsgOfHtml();
            this.Response.Redirect("ShowMsg.aspx?ss" + BP.DA.DataType.CurrentDataTime, true);
        }
    }
}