using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;

namespace BP.WF
{
	/// <summary>
	/// 选择接受人属性
	/// </summary>
    public class SelectAccperAttr
    {
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 到人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
    }
	/// <summary>
	/// 选择接受人
	/// 节点的到人员有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class SelectAccper : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        ///工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(SelectAccperAttr.WorkID);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.WorkID, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(SelectAccperAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 到人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(SelectAccperAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SelectAccperAttr.FK_Emp, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 选择接受人
        /// </summary>
        public SelectAccper()
        {

        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_SelectAccper");
                map.EnDesc = "选择接受人信息";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBInt(SelectAccperAttr.FK_Node, 0, "FK_Node", true, false);
                map.AddTBInt(SelectAccperAttr.WorkID, 0, "WorkID", true, false);
                map.AddTBString(SelectAccperAttr.FK_Emp, null, "FK_Emp", true, false, 0, 20, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
	/// <summary>
	/// 选择接受人
	/// </summary>
    public class SelectAccpers : EntitiesMyPK
    {
        /// <summary>
        /// 他的到人员
        /// </summary>
        public Emps HisEmps
        {
            get
            {
                Emps ens = new Emps();
                foreach (SelectAccper ns in this)
                {
                    ens.AddEntity(new Emp(ns.FK_Emp));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (SelectAccper ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        /// <summary>
        /// 选择接受人
        /// </summary>
        public SelectAccpers() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SelectAccper();
            }
        }
    }
}
