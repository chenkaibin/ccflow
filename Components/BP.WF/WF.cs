using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.Web;
using BP.En;
using BP.DA;
using BP.En;

namespace BP.WF
{
    public class DoWhatList
    {
        public const string DoNode = "DoNode";
        public const string Start = "Start";
        public const string Start5 = "Start5";
        ///// <summary>
        ///// 调起我的流程
        ///// </summary>
        //public const string CallMyFlow = "CallMyFlow";
        //public const string CallMyFlowSmall = "CallMyFlowSmall";

        public const string StartSmall = "StartSmall";
        public const string StartSmallSingle = "StartSmallSingle";
        public const string MyWork = "MyWork";
        public const string Login = "Login";
        public const string FlowSearch = "FlowSearch";
        public const string FlowSearchSmall = "FlowSearchSmall";
        public const string FlowSearchSmallSingle = "FlowSearchSmallSingle";

        public const string Emps = "Emps";
        public const string EmpWorks = "EmpWorks";
        public const string EmpWorksSmall = "EmpWorksSmall";
        public const string EmpWorksSmallSingle = "EmpWorksSmallSingle";

        public const string MyFlow = "MyFlow";
        public const string FlowFX = "FlowFX";
        public const string DealWork = "DealWork";
        public const string DealWorkInSmall = "DealWorkInSmall";
        public const string DealWorkInSmallSingle = "DealWorkInSmallSingle";

        public const string Tools = "Tools";
        public const string ToolsSmall = "ToolsSmall";

        public const string RuningSmall = "RuningSmall";

    }
    public enum FlowShowType
    {
        /// <summary>
        /// 当前工作
        /// </summary>
        MyWorks,
        /// <summary>
        /// 新建
        /// </summary>
        WorkNew,
        /// <summary>
        /// 工作步骤
        /// </summary>
        WorkStep,
        /// <summary>
        /// 工作图片
        /// </summary>
        WorkImages
    }
    public enum WorkProgress
    {
        /// <summary>
        /// 正常运行
        /// </summary>
        Runing,
        /// <summary>
        /// 预警
        /// </summary>
        Alert,
        /// <summary>
        /// 逾期
        /// </summary>
        Timeout
    }

}