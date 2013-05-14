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

namespace CCFlow.SDKFlowDemo.TelecomDemo.Parent
{
    public partial class S1_Start : System.Web.UI.Page
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

        public void CheckPhysicsTable()
        {
            BP.Demo.tab_wf_commonkpiopti tab_wf_commonkpiopti = new BP.Demo.tab_wf_commonkpiopti();
            tab_wf_commonkpiopti.CheckPhysicsTable();

            BP.Demo.tab_wf_commonkpiopti_main aa = new BP.Demo.tab_wf_commonkpiopti_main();
            aa.CheckPhysicsTable();

            BP.Demo.tab_wf_commonkpioptivalue bb = new BP.Demo.tab_wf_commonkpioptivalue();
            bb.CheckPhysicsTable();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //检查物理表。
            CheckPhysicsTable();

            if (BP.Web.WebUser.No == null)
                throw new Exception("@登录信息丢失.");

            // 首先根据WorkID获取 tab_wf_commonkpiopti_main 的数据,
            tab_wf_commonkpiopti_main main = new tab_wf_commonkpiopti_main();
            main.WorkID = this.WorkID;
            if (main.Retrieve(tab_wf_commonkpiopti_mainAttr.WorkID,this.WorkID) == 0)
            {
                /*说明没有找到这个数据,就要向这个数据库里插入一条对应的流程数据记录。*/
                main.wf_title = "测试:WF_title ";
                main.wf_send_time = DataType.CurrentDataCNOfShort;
                main.wf_send_phone = "18660153393"; //用户部门.
                main.wf_send_department = WebUser.FK_Dept; //用户部门.
                main.wf_send_user = WebUser.No;
                main.wf_no = "Bill" + DateTime.Now.ToString("yyyyMMddHH"); /*单据编号*/
                // 这里还有其它的值没有设置，根据业务情况设置它们。
                //main.Insert(); /*执行向主流程上插入一条*/
            }
            else
            {
                /* 
                 * 这里要修改一些变化的信息，比如：单据编号或者发送人的电话，发送时间。
                 * 有可能此人n天以前启动了一个草稿就退出了，有一些字段是根据当前的环境变化而变化的.
                 */
                main.wf_title = "测试:wf_title ";
                main.wf_send_time = DataType.CurrentDataCNOfShort;
                main.wf_send_phone = "18660153393"; //用户部门.
                main.wf_send_department = WebUser.FK_Dept; //用户部门.
                main.wf_send_user = WebUser.No;
                main.wf_no = "Bill" + DateTime.Now.ToString("yyyyMMddHH"); /*单据编号*/
            }
        }
        /// <summary>
        /// 绑定表单信息
        /// </summary>
        /// <param name="main"></param>
        public void BindSheetInfo(tab_wf_commonkpiopti_main main)
        {
            this.TB_wf_send_time.Text = main.wf_send_time; /*发起时间*/
            this.TB_wf_no.Text = main.wf_no;  /*单据编号*/

            this.TB_FaQiRen.Text = main.wf_send_user;
            this.TB_ZBName.Text = main.techology; /* 技术信息，这里要为每个字段赋值
                                                   * 包括从表与从表的从表。
                                                   */
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Send_Click(object sender, EventArgs e)
        {
            //调用保存方法.
            Btn_Save_Click(null, null);

            // 查找出已经保存的主表数据。
             tab_wf_commonkpiopti_main tab_wf_commonkpiopti_main = new BP.Demo.tab_wf_commonkpiopti_main();
            tab_wf_commonkpiopti_main.Retrieve(tab_wf_commonkpiopti_mainAttr.WorkID, this.WorkID);

            // 发当前工作，让他发送到，主线程的下一个节点上去。
            string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID).ToMsgOfHtml();

            //为市局启动工作任务, 查询出来集合，该表单的集合.
             tab_wf_commonkpioptis tab_wf_commonkpioptis = new tab_wf_commonkpioptis();
            tab_wf_commonkpioptis.Retrieve(tab_wf_commonkpioptiAttr.tab_wf_commonkpiopti_main, 
                tab_wf_commonkpiopti_main.OID);

            // 遍历市局这个集合.
            foreach (tab_wf_commonkpiopti tab_wf_commonkpiopti in tab_wf_commonkpioptis)
            {
                // 调用 创建空白工作，为市局生成一个开始节点的待办工作，并接受它的WorkID.
                Int64 subFlowWorkID = BP.WF.Dev2Interface.Node_CreateBlankWork("026", null,
                    null, tab_wf_commonkpiopti.wf_send_user, "自动发起任务:" + WebUser.No, this.WorkID,this.FK_Flow);

                // 给子流程赋WorkID.
                tab_wf_commonkpiopti.WorkID = subFlowWorkID;
                tab_wf_commonkpiopti.ParentWorkID = this.WorkID;
                tab_wf_commonkpiopti.Update();

                // 执行sql 更新设备的 ParentWorkID .
                string sql = "UPDATE tab_wf_commonkpioptivalue SET ParentWorkID=" + subFlowWorkID + " WHERE wf_commonkpioptivalue_id=" + tab_wf_commonkpiopti.OID;
                DBAccess.RunSQL(sql);

                msg += "@子流程 - 市局:" + tab_wf_commonkpiopti.region_id + "已经启动,任务已经下达给" + tab_wf_commonkpiopti.wf_send_user + " 处理 .";
            }

            // 这里应当转向一个界面来显示这些信息。
            this.Session["info"] = msg;
            this.Response.Redirect("ShowMsg.aspx?ss=" + DataType.CurrentDataTime, true);
        }
        /// <summary>
        /// 执行保存工单
        /// 1,向省局业务主表里写数据.
        /// 2,向市局工单临时表里写数据。
        /// 3,向设备工单临时表里写数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            /* 
             * 保存说明: 向省局主表里写数据, 这里用En30框架来描述业务逻辑的实现步骤. 
             * 您可以直接用sql，或者用自己的方式把数据存入 tab_wf_commonkpiopti_main表里.
             */

            #region 第一步: 保存主表数据.
            // 建立一个空白的数据.
            tab_wf_commonkpiopti_main mainFlow = new  tab_wf_commonkpiopti_main();

            // 根据WorkID 查询出来这条数据信息, 执行这个方法，就可以获得主建OID,
            // 一定可以查询出来一条数据，因为在page_load中已经判断。
             mainFlow.Retrieve(tab_wf_commonkpiopti_mainAttr.WorkID, this.WorkID); /*查询出来数据.*/

            //首先要给这个主表的, 赋予流程的基本信息. 
            mainFlow.WorkID = this.WorkID;
            mainFlow.fk_flow = this.FK_Flow; // 流程编号
            mainFlow.wf_category= "02"; // 流程类别编号。

            mainFlow.wf_title = "自定义流程标题:"+DataType.CurrentData+" , "+WebUser.No; // 流程标题
            mainFlow.wf_no = this.TB_wf_no.Text; // 单据编号
            mainFlow.wf_send_user = WebUser.No; // 当前操作员.
            mainFlow.wf_send_department = WebUser.FK_Dept; // 当前操作人员部门.
            mainFlow.wf_send_phone = "18660153393"; //当前操作员电话
            mainFlow.wf_send_time = DataType.CurrentDataTime; // 当前时间.
            // 其次要给其它业务字段赋值.
            mainFlow.techology = this.TB_ZBName.Text; /*指标信息.*/

            //最后更新到数据库里, 完成主表的数据写入。
            int i= mainFlow.Update(); // 执行更新
            if (i == 0)
                mainFlow.Insert();
            #endregion 保存主表数据.


            #region 第二步： 清除目标从表数据与从从表数据, 可能你们的方法与我们的方法不同.
            tab_wf_commonkpioptis shiJus = new tab_wf_commonkpioptis();
            shiJus.Retrieve(tab_wf_commonkpioptiAttr.tab_wf_commonkpiopti_main, mainFlow.OID); // 清除市局数据.
            foreach (tab_wf_commonkpiopti shiju in shiJus)
            {
                //删除该市局下面的设备信息。
                 tab_wf_commonkpioptivalue shebeiEn = new  tab_wf_commonkpioptivalue();
                shebeiEn.Delete(tab_wf_commonkpioptivalueAttr.wf_commonkpioptivalue_id, shiju.OID);
                
                //删除该市局数据。
                shiju.Delete();
            }
            #endregion 清除目标从表数据与从从表数据.


            #region 第三步: 保存从表数据.(市局数据)
            // new 市局数据.
            tab_wf_commonkpiopti shijuEn = new  tab_wf_commonkpiopti();

            // 设置基础信息
            shijuEn.WorkID = this.WorkID;
            shijuEn.tab_wf_commonkpiopti_main = mainFlow.OID; // 关联的主键
            shijuEn.fk_flow = this.FK_Flow;
            shijuEn.wf_no = "111-222-3333"; //单据编号
            shijuEn.WorkID = 0; // 这个时间还没有产生WorkID
            shijuEn.wf_send_user = "zhoutianjiao"; // 子线程的处理人.

            //设置业务数据信息.
            shijuEn.region_id = "济南";

            // 将市局数据-插入到数据库.
            shijuEn.Insert();
            #endregion 第三步: 保存从表数据. (市局数据)


            #region 第四步: 保存从从表数据.(市局-设备数据)
            // 生成济南的第1个设备信息, 这些信息放在子线程中去, new 一个设备信息.
            tab_wf_commonkpioptivalue shebei = new  tab_wf_commonkpioptivalue();

            // 给他赋值  - 流程信息字段
            shebei.wf_commonkpioptivalue_id = shijuEn.OID; //关联主键
            shebei.fk_flow =this.FK_Flow;
            shebei.WorkID =0; // 这个时间还没有产生WorkID
           // shebei.fid = 0; // 这个时间还没有产生fid
            shebei.fuzeren = "guobaogeng"; // 现在指定下一节点的工作人员

            // 给他赋值  - 业务字段.
            shebei.addr = "济南高新区xx路xx号";
            shebei.remark = "abc-abc";

            // 插入到数据库.
            shebei.Insert();

            // 在建立一个设备信息。
            shebei = new tab_wf_commonkpioptivalue();
            // 给他赋值  - 流程信息字段
            shebei.wf_commonkpioptivalue_id = shijuEn.OID; //关联主键
            shebei.fk_flow = this.FK_Flow;
            shebei.WorkID = 0; // 这个时间还没有产生WorkID
           // shebei.fid = 0; // 这个时间还没有产生fid
            shebei.fuzeren = "fuhui"; // 现在指定下一节点的工作人员

            // 给他赋值  - 业务字段.
            shebei.addr = "济南历城区xx路xx号";
            shebei.remark = "abc-123";

            // 插入到数据库.
            shebei.Insert();

            #endregion 保存从从表数据. (市局-设备数据)

            if (sender == null)
                this.Response.Write("保存成功");
        }

        protected void Btn_Chat_Click(object sender, EventArgs e)
        {
            BP.WF.Dev2Interface.UI_Window_FlowChart(this.FK_Flow);
        }
    }
}