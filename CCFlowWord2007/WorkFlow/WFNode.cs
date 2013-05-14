using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BP.DA;

namespace CCFlowWord2007.WorkFlow
{
    /// <summary>
    /// 流程节点
    /// </summary>
    public class WFNode
    {
        public WFNode()
        { }
        #region Model

        /// <summary>
        /// 
        /// </summary>
        public int NodeID { get; set; }

        /// <summary>
        /// 流程步骤
        /// </summary>
        public int? Step { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 允许分配工作否?
        /// </summary>
        public bool? IsTask { get; set; }
        /// <summary>
        /// 是否可以强制删除了流程(对合流点有效)
        /// </summary>
        public bool? IsForceKill { get; set; }
        /// <summary>
        /// 通过率
        /// </summary>
        public decimal? PassRate { get; set; }
        /// <summary>
        /// 运行模式(对普通节点有效),枚举类型:0 普通;1 合流;2 分流;3 分合流;
        /// </summary>
        public int? RunModel { get; set; }
        /// <summary>
        /// 焦点字段
        /// </summary>
        public string FocusField { get; set; }
        /// <summary>
        /// 投递规则
        /// </summary>
        public int? DeliveryWay { get; set; }
        /// <summary>
        /// 接受人SQL
        /// </summary>
        public string RecipientSQL { get; set; }
        /// <summary>
        /// 表单类型,枚举类型:0 傻瓜表单;1 自由表单;2 自定义表单;3 SDK表单;9 禁用(对多表单流程有效);
        /// </summary>
        public int? FormType { get; set; }
        /// <summary>
        /// 表单URL
        /// </summary>
        public string FormUrl { get; set; }
        /// <summary>
        /// 完成后处理SQL
        /// </summary>
        public string DoWhat { get; set; }
        /// <summary>
        /// 转向处理
        /// </summary>
        public int? TurnToDeal { get; set; }
        /// <summary>
        /// 发送后提示信息
        /// </summary>
        public string TurnToDealDoc { get; set; }
        /// <summary>
        /// 生命周期从
        /// </summary>
        public string DTFrom { get; set; }
        /// <summary>
        /// 生命周期到
        /// </summary>
        public string DTTo { get; set; }
        /// <summary>
        /// 发送按钮标签
        /// </summary>
        public string SendLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? SendEnable { get; set; }
        /// <summary>
        /// 保存按钮标签
        /// </summary>
        public string SaveLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? SaveEnable { get; set; }
        /// <summary>
        /// 退回按钮标签
        /// </summary>
        public string ReturnLab { get; set; }
        /// <summary>
        /// 退回规则
        /// </summary>
        public ReturnRoleKind ReturnRole { get; set; }
        /// <summary>
        /// 抄送按钮标签
        /// </summary>
        public string CCLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? CCEnable { get; set; }
        /// <summary>
        /// 移交按钮标签
        /// </summary>
        public string ShiftLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? ShiftEnable { get; set; }
        /// <summary>
        /// 删除流程按钮标签
        /// </summary>
        public string DelLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? DelEnable { get; set; }
        /// <summary>
        /// 结束流程按钮标签
        /// </summary>
        public string EndFlowLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? EndFlowEnable { get; set; }
        /// <summary>
        /// 报告按钮标签
        /// </summary>
        public string RptLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? RptEnable { get; set; }
        /// <summary>
        /// 打印单据按钮标签
        /// </summary>
        public string PrintDocLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? PrintDocEnable { get; set; }
        /// <summary>
        /// 附件按钮标签
        /// </summary>
        public string AthLab { get; set; }
        /// <summary>
        /// 附件权限
        /// </summary>
        public AttachmentRoleKind FJOpen { get; set; }
        /// <summary>
        /// 轨迹按钮标签
        /// </summary>
        public string TrackLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? TrackEnable { get; set; }
        /// <summary>
        /// 选项按钮标签
        /// </summary>
        public string OptLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? OptEnable { get; set; }
        /// <summary>
        /// 接受人按钮标签
        /// </summary>
        public string SelectAccepterLab { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? SelectAccepterEnable { get; set; }
        /// <summary>
        /// 警告期限(0不警告)
        /// </summary>
        public int? WarningDays { get; set; }
        /// <summary>
        /// 限期(天)
        /// </summary>
        public int? DeductDays { get; set; }
        /// <summary>
        /// 扣分(每延期1天扣)
        /// </summary>
        public decimal? DeductCent { get; set; }
        /// <summary>
        /// 最高扣分
        /// </summary>
        public decimal? MaxDeductCent { get; set; }
        /// <summary>
        /// 工作得分
        /// </summary>
        public decimal? SwinkCent { get; set; }
        /// <summary>
        /// 超时处理,枚举类型:0 不处理;1 自动转入下一步;2 自动转到指定的人员;3 向指定的人员发送消息;4 删除流程;5 执行SQL;
        /// </summary>
        public int? OutTimeDeal { get; set; }
        /// <summary>
        /// 处理内容
        /// </summary>
        public string DoOutTime { get; set; }
        /// <summary>
        /// flow
        /// </summary>
        public string FK_Flow { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public int? NodeWorkType { get; set; }
        /// <summary>
        /// 流程名
        /// </summary>
        public string FlowName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FK_FlowSort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FK_FlowSortT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FrmAttr { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Doc { get; set; }
        /// <summary>
        /// 是否可以抄送
        /// </summary>
        public bool? IsCanCC { get; set; }
        /// <summary>
        /// 是否可以查看工作报告?
        /// </summary>
        public bool? IsCanRpt { get; set; }
        /// <summary>
        /// 是否可以终止流程
        /// </summary>
        public bool? IsCanOver { get; set; }
        /// <summary>
        /// 是否是保密步骤
        /// </summary>
        public bool? IsSecret { get; set; }
        /// <summary>
        /// 是否可以删除流程
        /// </summary>
        public bool? IsCanDelFlow { get; set; }
        /// <summary>
        /// 是否可以移交
        /// </summary>
        public bool? IsHandOver { get; set; }
        /// <summary>
        /// 审核模式(对审核节点有效),枚举类型:0 单签;1 汇签;
        /// </summary>
        public int? SignType { get; set; }
        /// <summary>
        /// 分流规则
        /// </summary>
        public int? FLRole { get; set; }
        /// <summary>
        /// 流程节点类型
        /// </summary>
        public int? FNType { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public int? NodePosType { get; set; }
        /// <summary>
        /// 是否有节点完成条件
        /// </summary>
        public bool? IsCCNode { get; set; }
        /// <summary>
        /// 是否有流程完成条件
        /// </summary>
        public bool? IsCCFlow { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public string HisStas { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string HisDeptStrs { get; set; }
        /// <summary>
        /// 转到的节点
        /// </summary>
        public string HisToNDs { get; set; }
        /// <summary>
        /// 单据IDs
        /// </summary>
        public string HisBillIDs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HisEmps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HisSubFlows { get; set; }
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable { get; set; }
        /// <summary>
        /// 显示的表单
        /// </summary>
        public string ShowSheets { get; set; }
        /// <summary>
        /// 岗位分组节点
        /// </summary>
        public string GroupStaNDs { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public int? X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public int? Y { get; set; }

        #endregion Model


        #region  Method

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public WFNode(int nodeId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT * FROM WF_Node WHERE NodeID = " + nodeId);
            var dt = DBAccess.RunSQLReturnTable(strSql.ToString());
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["NodeID"].ToString() != "")
                {
                    NodeID = int.Parse(dt.Rows[0]["NodeID"].ToString());
                }
                if (dt.Rows[0]["Step"].ToString() != "")
                {
                    Step = int.Parse(dt.Rows[0]["Step"].ToString());
                }
                Name = dt.Rows[0]["Name"].ToString();
                if (dt.Rows[0]["IsTask"].ToString() != "")
                {
                    IsTask = Convert.ToBoolean(dt.Rows[0]["IsTask"]);
                }
                if (dt.Rows[0]["IsForceKill"].ToString() != "")
                {
                    IsForceKill = Convert.ToBoolean(dt.Rows[0]["IsForceKill"]);
                }
                if (dt.Rows[0]["PassRate"].ToString() != "")
                {
                    PassRate = decimal.Parse(dt.Rows[0]["PassRate"].ToString());
                }
                if (dt.Rows[0]["RunModel"].ToString() != "")
                {
                    RunModel = int.Parse(dt.Rows[0]["RunModel"].ToString());
                }
                FocusField = dt.Rows[0]["FocusField"].ToString();
                if (dt.Rows[0]["DeliveryWay"].ToString() != "")
                {
                    DeliveryWay = int.Parse(dt.Rows[0]["DeliveryWay"].ToString());
                }
                RecipientSQL = dt.Rows[0]["RecipientSQL"].ToString();
                if (dt.Rows[0]["FormType"].ToString() != "")
                {
                    FormType = int.Parse(dt.Rows[0]["FormType"].ToString());
                }
                FormUrl = dt.Rows[0]["FormUrl"].ToString();
                DoWhat = dt.Rows[0]["DoWhat"].ToString();
                if (dt.Rows[0]["TurnToDeal"].ToString() != "")
                {
                    TurnToDeal = int.Parse(dt.Rows[0]["TurnToDeal"].ToString());
                }
                TurnToDealDoc = dt.Rows[0]["TurnToDealDoc"].ToString();
                DTFrom = dt.Rows[0]["DTFrom"].ToString();
                DTTo = dt.Rows[0]["DTTo"].ToString();
                SendLab = dt.Rows[0]["SendLab"].ToString();
                if (dt.Rows[0]["SendEnable"].ToString() != "")
                {
                    SendEnable = Convert.ToBoolean(dt.Rows[0]["SendEnable"]);
                }
                SaveLab = dt.Rows[0]["SaveLab"].ToString();
                if (dt.Rows[0]["SaveEnable"].ToString() != "")
                {
                    SaveEnable = Convert.ToBoolean(dt.Rows[0]["SaveEnable"]);
                }
                ReturnLab = dt.Rows[0]["ReturnLab"].ToString();
                if (dt.Rows[0]["ReturnRole"].ToString() != "")
                {
                    ReturnRole = (ReturnRoleKind)Convert.ToInt32(dt.Rows[0]["ReturnRole"]);
                }
                CCLab = dt.Rows[0]["CCLab"].ToString();
                if (dt.Rows[0]["CCEnable"].ToString() != "")
                {
                    CCEnable = Convert.ToBoolean(dt.Rows[0]["CCEnable"]);
                }
                ShiftLab = dt.Rows[0]["ShiftLab"].ToString();
                if (dt.Rows[0]["ShiftEnable"].ToString() != "")
                {
                    ShiftEnable = Convert.ToBoolean(dt.Rows[0]["ShiftEnable"]);
                }
                DelLab = dt.Rows[0]["DelLab"].ToString();
                if (dt.Rows[0]["DelEnable"].ToString() != "")
                {
                    DelEnable = Convert.ToBoolean(dt.Rows[0]["DelEnable"]);
                }
                EndFlowLab = dt.Rows[0]["EndFlowLab"].ToString();
                if (dt.Rows[0]["EndFlowEnable"].ToString() != "")
                {
                    EndFlowEnable = Convert.ToBoolean(dt.Rows[0]["EndFlowEnable"]);
                }
                RptLab = dt.Rows[0]["RptLab"].ToString();
                if (dt.Rows[0]["RptEnable"].ToString() != "")
                {
                    RptEnable = Convert.ToBoolean(dt.Rows[0]["RptEnable"]);
                }
                PrintDocLab = dt.Rows[0]["PrintDocLab"].ToString();
                if (dt.Rows[0]["PrintDocEnable"].ToString() != "")
                {
                    PrintDocEnable = Convert.ToBoolean(dt.Rows[0]["PrintDocEnable"]);
                }
                AthLab = dt.Rows[0]["AthLab"].ToString();
                if (dt.Rows[0]["FJOpen"].ToString() != "")
                {
                    FJOpen = (AttachmentRoleKind)Convert.ToInt32(dt.Rows[0]["FJOpen"]);
                }
                TrackLab = dt.Rows[0]["TrackLab"].ToString();
                if (dt.Rows[0]["TrackEnable"].ToString() != "")
                {
                    TrackEnable = Convert.ToBoolean(dt.Rows[0]["TrackEnable"]);
                }
                OptLab = dt.Rows[0]["OptLab"].ToString();
                if (dt.Rows[0]["OptEnable"].ToString() != "")
                {
                    OptEnable = Convert.ToBoolean(dt.Rows[0]["OptEnable"]);
                }
                SelectAccepterLab = dt.Rows[0]["SelectAccepterLab"].ToString();
                if (dt.Rows[0]["SelectAccepterEnable"].ToString() != "")
                {
                    SelectAccepterEnable = Convert.ToBoolean(dt.Rows[0]["SelectAccepterEnable"]);
                }
                if (dt.Rows[0]["WarningDays"].ToString() != "")
                {
                    WarningDays = int.Parse(dt.Rows[0]["WarningDays"].ToString());
                }
                if (dt.Rows[0]["DeductDays"].ToString() != "")
                {
                    DeductDays = int.Parse(dt.Rows[0]["DeductDays"].ToString());
                }
                if (dt.Rows[0]["DeductCent"].ToString() != "")
                {
                    DeductCent = decimal.Parse(dt.Rows[0]["DeductCent"].ToString());
                }
                if (dt.Rows[0]["MaxDeductCent"].ToString() != "")
                {
                    MaxDeductCent = decimal.Parse(dt.Rows[0]["MaxDeductCent"].ToString());
                }
                if (dt.Rows[0]["SwinkCent"].ToString() != "")
                {
                    SwinkCent = decimal.Parse(dt.Rows[0]["SwinkCent"].ToString());
                }
                if (dt.Rows[0]["OutTimeDeal"].ToString() != "")
                {
                    OutTimeDeal = int.Parse(dt.Rows[0]["OutTimeDeal"].ToString());
                }
                DoOutTime = dt.Rows[0]["DoOutTime"].ToString();
                FK_Flow = dt.Rows[0]["FK_Flow"].ToString();
                if (dt.Rows[0]["NodeWorkType"].ToString() != "")
                {
                    NodeWorkType = int.Parse(dt.Rows[0]["NodeWorkType"].ToString());
                }
                FlowName = dt.Rows[0]["FlowName"].ToString();
                FK_FlowSort = dt.Rows[0]["FK_FlowSort"].ToString();
                FK_FlowSortT = dt.Rows[0]["FK_FlowSortT"].ToString();
                FrmAttr = dt.Rows[0]["FrmAttr"].ToString();
                Doc = dt.Rows[0]["Doc"].ToString();
                if (dt.Rows[0]["IsCanCC"].ToString() != "")
                {
                    IsCanCC = Convert.ToBoolean(dt.Rows[0]["IsCanCC"]);
                }
                if (dt.Rows[0]["IsCanRpt"].ToString() != "")
                {
                    IsCanRpt = Convert.ToBoolean(dt.Rows[0]["IsCanRpt"]);
                }
                if (dt.Rows[0]["IsCanOver"].ToString() != "")
                {
                    IsCanOver = Convert.ToBoolean(dt.Rows[0]["IsCanOver"]);
                }
                if (dt.Rows[0]["IsSecret"].ToString() != "")
                {
                    IsSecret = Convert.ToBoolean(dt.Rows[0]["IsSecret"]);
                }
                if (dt.Rows[0]["IsCanDelFlow"].ToString() != "")
                {
                    IsCanDelFlow = Convert.ToBoolean(dt.Rows[0]["IsCanDelFlow"]);
                }
                if (dt.Rows[0]["IsHandOver"].ToString() != "")
                {
                    IsHandOver = Convert.ToBoolean(dt.Rows[0]["IsHandOver"]);
                }
                if (dt.Rows[0]["SignType"].ToString() != "")
                {
                    SignType = int.Parse(dt.Rows[0]["SignType"].ToString());
                }
                if (dt.Rows[0]["FLRole"].ToString() != "")
                {
                    FLRole = int.Parse(dt.Rows[0]["FLRole"].ToString());
                }
                if (dt.Rows[0]["FNType"].ToString() != "")
                {
                    FNType = int.Parse(dt.Rows[0]["FNType"].ToString());
                }
                if (dt.Rows[0]["NodePosType"].ToString() != "")
                {
                    NodePosType = int.Parse(dt.Rows[0]["NodePosType"].ToString());
                }
                if (dt.Rows[0]["IsCCNode"].ToString() != "")
                {
                    IsCCNode = Convert.ToBoolean(dt.Rows[0]["IsCCNode"]);
                }
                if (dt.Rows[0]["IsCCFlow"].ToString() != "")
                {
                    IsCCFlow = Convert.ToBoolean(dt.Rows[0]["IsCCFlow"]);
                }
                HisStas = dt.Rows[0]["HisStas"].ToString();
                HisDeptStrs = dt.Rows[0]["HisDeptStrs"].ToString();
                HisToNDs = dt.Rows[0]["HisToNDs"].ToString();
                HisBillIDs = dt.Rows[0]["HisBillIDs"].ToString();
                HisEmps = dt.Rows[0]["HisEmps"].ToString();
                HisSubFlows = dt.Rows[0]["HisSubFlows"].ToString();
                PTable = dt.Rows[0]["PTable"].ToString();
                ShowSheets = dt.Rows[0]["ShowSheets"].ToString();
                GroupStaNDs = dt.Rows[0]["GroupStaNDs"].ToString();
                if (dt.Rows[0]["X"].ToString() != "")
                {
                    X = int.Parse(dt.Rows[0]["X"].ToString());
                }
                if (dt.Rows[0]["Y"].ToString() != "")
                {
                    Y = int.Parse(dt.Rows[0]["Y"].ToString());
                }
            }
        }
        #endregion  Method
    }

    /// <summary>
    /// 退回规则
    /// </summary>
    public enum ReturnRoleKind
    {
        /// <summary>
        /// 不能退回
        /// </summary>
        UnEnable = 0,

        /// <summary>
        /// 退回上一节点
        /// </summary>
        BackOne = 1,

        /// <summary>
        /// 退回任意节点
        /// </summary>
        BackEvery = 2,

        /// <summary>
        /// 退回指定节点
        /// </summary>
        BackAppointed = 3
    }

    /// <summary>
    /// 附件权限
    /// </summary>
    public enum AttachmentRoleKind
    {
        /// <summary>
        /// 关闭附件
        /// </summary>
        Close = 0,

        /// <summary>
        /// 操作员
        /// </summary>
        User = 1,

        /// <summary>
        /// 工作ID
        /// </summary>
        Work = 2,

        /// <summary>
        /// 流程ID
        /// </summary>
        WorkFlow = 3
    }
}
