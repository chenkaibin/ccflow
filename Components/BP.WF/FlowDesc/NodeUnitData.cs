using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.En;
using System.Collections;
using BP.Port;

namespace BP.WF
{
    /// <summary>
    /// 节点数据
    /// </summary>
    public class NodeUnitDataAttr:BP.En.EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 完成日期
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人员
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 数量
        /// </summary>
        public const string MyNum = "MyNum";
        #endregion
    }
    /// <summary>
    /// 这里存放每个节点数据汇总的信息.	 
    /// </summary>
    public class NodeUnitData : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// FID
        /// </summary>
        public int FID
        {
            get
            {
                return this.GetValIntByKey(NodeUnitDataAttr.FID);
            }
            set
            {
                this.SetValByKey(NodeUnitDataAttr.FID, value);
            }
        }
        /// <summary>
        /// WorkID
        /// </summary>
        public int WorkID
        {
            get
            {
                return this.GetValIntByKey(NodeUnitDataAttr.WorkID);
            }
            set
            {
                this.SetValByKey(NodeUnitDataAttr.WorkID, value);
            }
        }
        /// <summary>
        /// FK_Node
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeUnitDataAttr.FK_Node);
            }
            set
            {
                SetValByKey(NodeUnitDataAttr.FK_Node, value);
            }
        }
        public string FK_NodeT
        {
            get
            {
                return this.GetValRefTextByKey(NodeUnitDataAttr.FK_Node);
            }
        }
        #endregion

        #region 构造函数
        public string FlowNo = null;
        /// <summary>
        /// 节点数据汇总
        /// </summary>
        public NodeUnitData() 
        {

        }
        public NodeUnitData(string fk_flow)
        {
            this.FlowNo = fk_flow;
        }
        /// <summary>
        /// 节点数据汇总
        /// </summary>
        /// <param name="_oid">节点数据汇总ID</param>	
        public NodeUnitData(string fk_flow,string mypk)
        {
            this.FlowNo = fk_flow;
            this.MyPK = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                {
                    this._enMap.PhysicsTable = "V" + this.FlowNo;
                    return this._enMap;
                }

                Map map = new Map("ViewFlow");
                map.EnDesc = "节点数据汇总";
                map.EnType = En.EnType.View;
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddMyPK();
                map.AddTBInt(NodeUnitDataAttr.WorkID, 0, "WorkID", true, true);
                map.AddDDLEntities(NodeUnitDataAttr.FK_Node, null, "节点", new NodeExts(), false);
                map.AddDDLSysEnum(NodeUnitDataAttr.NodeState, 0, "节点状态", false, false);
                map.AddDDLEntities(NodeUnitDataAttr.Rec, null, "操作员", new WF.Port.WFEmps(), false);


                map.AddTBDateTime(NodeUnitDataAttr.RDT, null, "记录日期", false, true);
                map.AddTBDateTime(NodeUnitDataAttr.CDT, null, "完成日期", false, true);

                map.AddDDLEntities(NodeUnitDataAttr.FK_NY, null, "年月", new BP.Pub.NYs(), false);

                map.AddTBString(NodeUnitDataAttr.Emps, null, "操作员s", true, false, 0, 400, 10);

                map.AddTBMyNum();
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 节点数据汇总集合
    /// </summary>
    public class NodeUnitDatas : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeUnitData();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点数据汇总集合
        /// </summary>
        public NodeUnitDatas()
        {
        }
        /// <summary>
        /// 节点数据汇总集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public NodeUnitDatas(string FK_Node)
        {
            this.Retrieve(NodeUnitDataAttr.FK_Node, FK_Node);
        }
        #endregion
    }
}
