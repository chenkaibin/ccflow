using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
//using BP.ZHZS.Base;

namespace BP.WF
{
    /// <summary>
    /// 节点表单属性	  
    /// </summary>
    public class FrmNodeAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Frm = "FK_Frm";
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 是否readonly.
        /// </summary>
        public const string IsEdit = "IsEdit";
        /// <summary>
        /// IsPrint
        /// </summary>
        public const string IsPrint = "IsPrint";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// FK_Flow
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
    }
    /// <summary>
    /// 节点表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FrmNode : EntityMyPK
    {
        #region 基本属性
        public string FrmUrl
        {
            get
            {
                switch (this.HisFrmType)
                {
                    case Sys.FrmType.Column4Frm:
                        return "FrmFix";
                    case Sys.FrmType.CCForm:
                        return "Frm";
                    default:
                        throw new Exception("err,未处理。");
                }
            }
        }
        private Frm _hisFrm = null;
        public Frm HisFrm
        {
            get
            {
                if (this._hisFrm == null)
                {
                    this._hisFrm = new Frm(this.FK_Frm);
                    this._hisFrm.HisFrmNode = this;
                }
                return this._hisFrm;
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        public BP.Sys.FrmType HisFrmType
        {
            get
            {
                return (BP.Sys.FrmType)this.GetValIntByKey(FrmNodeAttr.FrmType);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmType, (int)value);
            }
        }
        public int FrmType
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FrmType);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FrmType, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Node, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.Idx);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.Idx, value);
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        public string FK_Frm
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Frm);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Frm, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.FK_Flow, value);
            }
        }
        public bool IsEdit
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsEdit);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsEdit, value);
            }
        }
        public bool IsPrint
        {
            get
            {
                return this.GetValBooleanByKey(FrmNodeAttr.IsPrint);
            }
            set
            {
                this.SetValByKey(FrmNodeAttr.IsPrint, value);
            }
        }
        public int IsEditInt
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.IsEdit);
            }
        }
        public int IsPrintInt
        {
            get
            {
                return this.GetValIntByKey(FrmNodeAttr.IsPrint);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNode() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="mypk"></param>
        public FrmNode(string mypk):base(mypk)
        {
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="fk_node">节点</param>
        /// <param name="fk_frm">表单</param>
        public FrmNode(int fk_node, string fk_frm)
        {
            int i = this.Retrieve(FrmNodeAttr.FK_Node, fk_node, FrmNodeAttr.FK_Frm, fk_frm);
            if (i == 0)
            {
                this.IsPrint = false;
                this.IsEdit = false;
                return;
                throw new Exception("@表单关联信息已被删除。");
            }
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

                Map map = new Map("WF_FrmNode");
                map.EnDesc = "节点表单";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddMyPK();
                map.AddTBString(FrmNodeAttr.FK_Frm, null, "表单编号", true, true, 1, 20, 20);
                map.AddTBInt(FrmNodeAttr.FK_Node, 0, "节点编号", true, false);
                map.AddTBString(FrmNodeAttr.FK_Flow, null, "流程", true, true, 1, 20, 20);

                map.AddTBInt(FrmNodeAttr.FrmType, 0, "表单类型", true, false);


                map.AddTBInt(FrmNodeAttr.IsEdit, 1, "是否可以更新", true, false);
                map.AddTBInt(FrmNodeAttr.IsPrint, 0, "IsPrint", true, false);

                map.AddTBInt(FrmNodeAttr.Idx, 0, "顺序号", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public void DoUp()
        {
            this.DoOrderUp(FrmNodeAttr.FK_Node, this.FK_Node.ToString(), FrmNodeAttr.Idx);
        }
        public void DoDown()
        {
            this.DoOrderDown(FrmNodeAttr.FK_Node, this.FK_Node.ToString(), FrmNodeAttr.Idx);
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_Frm + "_" + this.FK_Node;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 节点表单
    /// </summary>
    public class FrmNodes : EntitiesMM
    {
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (FrmNode ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        public FrmNodes() { }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FrmNodes(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Node, NodeID);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public FrmNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Node, NodeNo);
            qo.addOrderBy(FrmNodeAttr.Idx);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmNode();
            }
        }
        /// <summary>
        /// 节点表单s
        /// </summary>
        /// <param name="sts">节点表单</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Nodes sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Node st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        /// <param name="NodeNo">工作节点编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Node, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FrmNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Frm));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Nodes GetHisNodes(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmNodeAttr.FK_Frm, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FrmNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
    }
}
