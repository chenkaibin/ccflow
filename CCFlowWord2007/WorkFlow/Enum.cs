using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 附件开放类型
    /// </summary>
    public enum FJOpen
    {
        /// <summary>
        /// 不开放
        /// </summary>
        None,
        /// <summary>
        /// 对操作员开放
        /// </summary>
        ForEmp,
        /// <summary>
        /// 对工作ID开放
        /// </summary>
        ForWorkID,
        /// <summary>
        /// 对流程ID开放
        /// </summary>
        ForFID
    }
    /// <summary>
    /// 分流规则
    /// </summary>
    public enum FLRole
    {
        /// <summary>
        /// 按照接受人
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按照部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按照岗位
        /// </summary>
        ByStation
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL
    }
    /// <summary>
    /// 节点签字类型
    /// </summary>
    public enum SignType
    {
        /// <summary>
        /// 单签
        /// </summary>
        OneSign,
        /// <summary>
        /// 会签
        /// </summary>
        Countersign
    }
    /// <summary>
    /// 节点状态
    /// </summary>
    public enum NodeState
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init=0,
        /// <summary>
        /// 已经完成
        /// </summary>
        Complete=1,
        /// <summary>
        /// 扣分状态
        /// </summary>
        CutCent=2,
        /// <summary>
        /// 强制终止
        /// </summary>
        Stop=3,
        /// <summary>
        /// 删除
        /// </summary>
        Delete=4,
        /// <summary>
        /// 退回
        /// </summary>
        Back=5
    }
    /// <summary>
    /// 节点工作类型
    /// 节点工作类型( 0, 审核节点, 1 信息采集节点,  2, 开始节点)
    /// </summary>
    public enum NodeWorkType
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        StartWork = 0,
        /// <summary>
        /// 开始节点分流
        /// </summary>
        StartWorkFL = 1,
        /// <summary>
        /// 标准审核节点
        /// </summary>
        StandardChecks = 2,
        /// <summary>
        /// 数量审核节点
        /// </summary>
        NumChecks = 3,
        /// <summary>
        /// 会签(多个人的工作)
        /// </summary>
        MulChecks = 4,
        /// <summary>
        /// 合流节点
        /// </summary>
        WorkHL = 5,
        /// <summary>
        /// 分流节点
        /// </summary>
        WorkFL = 6,
        /// <summary>
        /// 分合流
        /// </summary>
        WorkFHL = 7,
        /// <summary>
        /// 普通工作
        /// </summary>
        Work = 8
    }
    /// <summary>
    /// 流程节点类型
    /// </summary>
    public enum FNType
    {
        /// <summary>
        /// 平面节点
        /// </summary>
        Plane = 0,
        /// <summary>
        /// 分合流
        /// </summary>
        River = 1,
        /// <summary>
        /// 支流
        /// </summary>
        Branch = 2
    }
    /// <summary>
    /// 
    /// </summary>
    public enum NodePosType
    {
        Start,
        Mid,
        End
    }
}
