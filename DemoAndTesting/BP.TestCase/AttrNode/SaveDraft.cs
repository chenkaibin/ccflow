using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.CT;

namespace BP.CT.NodeAttr
{
    public  class SaveDraft : TestBase
    {
        /// <summary>
        /// 保存草稿-保存草稿
        /// </summary>
        public SaveDraft()
        {
            this.Title = "保存草稿";
            this.DescIt = "新建立一个流程实例，保存草稿是否可以？";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            if (Glo.IsEnableDraft == false)
                throw new Exception("@此测试需要在Web.config 的 IsEnableDraft = 1 的状态下才能测试它。");

            string fk_flow = "032";
            string userNo = "zhanghaicheng";

            Flow fl = new Flow(fk_flow);

            // zhoutianjiao 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No, null, null, WebUser.No, null, 0, null);

            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fl.No, 3201,  workid);

            #region 检查保存的草稿数据是否完整。
            GERpt rpt = fl.HisFlowData;
            rpt.OID = workid;
            rpt.RetrieveFromDBSources();
            if (rpt.WFState != WFState.Blank)
                throw new Exception("@保存错误,此 GERpt 应该是 Blank 状态,现在是:" + rpt.WFState);

            bool isHave = false;
            DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fl.No);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["OID"].ToString() == workid.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == true)
                throw new Exception("@不应该找到草稿.。");
            #endregion

            //设置成草稿.
            BP.WF.Dev2Interface.Node_SetDraft(fl.No, workid);

            #region 检查保存的草稿数据是否完整。
            rpt = fl.HisFlowData;
            rpt.OID = workid;
            rpt.RetrieveFromDBSources();
            if (rpt.WFState != WFState.Draft && Glo.IsEnableDraft)
                throw new Exception("@,此 GERpt 应该是 Draft 状态,现在是:" + rpt.WFState);

            isHave = false;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fl.No);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["OID"].ToString() == workid.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@没有从接口里找到草稿。");
            #endregion

        }
    }
}
