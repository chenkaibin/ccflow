using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using System.Collections;
using BP.Port;

namespace BP.WF
{
   
    /// <summary>
    /// 这里存放每个节点的信息.	 
    /// </summary>
    public class Node : Entity
    {
        #region 参数属性
        /// <summary>
        /// 超时处理方式
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// 子线程类型
        /// </summary>
        public SubThreadType HisSubThreadType
        {
            get
            {
                return (SubThreadType)this.GetValIntByKey(NodeAttr.SubThreadType);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubThreadType, (int)value);
            }
        }
        #endregion

        #region 外键属性.
        public CC HisCC
        {
            get
            {
                CC obj = this.GetRefObject("HisCC") as CC;
                if (obj == null)
                {
                    obj = new CC();
                    obj.NodeID = this.NodeID;
                    obj.Retrieve();
                    this.SetRefObject("HisCC", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 他的将要转向的方向集合
        /// 如果他没有到转向方向,他就是结束节点.
        /// 没有生命周期的概念,全部的节点.
        /// </summary>
        public Nodes HisToNodes
        {
            get
            {
                Nodes obj = this.GetRefObject("HisToNodes") as Nodes;
                if (obj == null)
                {
                    obj = new Nodes();
                    obj.AddEntities(this.HisToNDs);
                    this.SetRefObject("HisToNodes", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 他的工作
        /// </summary>
        public Work HisWork
        {
            get
            {
                Work obj=null;
                if (this.IsStartNode)
                {
                    obj = new BP.WF.GEStartWork(this.NodeID);
                    obj.HisNode = this;
                    obj.NodeID = this.NodeID;
                    return obj;
                    this.SetRefObject("HisWork", obj);
                }
                else
                {
                    obj = new BP.WF.GEWork(this.NodeID);
                    obj.HisNode = this;
                    obj.NodeID = this.NodeID;
                    return obj;
                    //this.SetRefObject("HisWork", obj);
                }
               // return obj;

                /* 放入缓存就没有办法执行数据的clone. */

               // Work obj = this.GetRefObject("HisWork") as Work;
               // if (obj == null)
               // {
               //     if (this.IsStartNode)
               //     {
               //         obj = new BP.WF.GEStartWork(this.NodeID);
               //         obj.HisNode = this;
               //         obj.NodeID = this.NodeID;
               //         this.SetRefObject("HisWork", obj);
               //     }
               //     else
               //     {
               //         obj = new BP.WF.GEWork(this.NodeID);
               //         obj.HisNode = this;
               //         obj.NodeID = this.NodeID;
               //         this.SetRefObject("HisWork", obj);
               //     }
               // }
               //// obj.GetNewEntities.GetNewEntity;
               //// obj.Row = null;
               // return obj;
            }
        }
        /// <summary>
        /// 他的工作s
        /// </summary>
        public Works HisWorks
        {
            get
            {
                Works obj = this.HisWork.GetNewEntities as Works;
                return obj;
                ////Works obj = this.GetRefObject("HisWorks") as Works;
                ////if (obj == null)
                ////{
                //    this.SetRefObject("HisWorks",obj);
                //}
                //return obj;
            }
        }

        /// <summary>
        /// 流程
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                Flow  obj =this.GetRefObject("Flow") as Flow;
                if (obj == null)
                {
                    obj=new Flow(this.FK_Flow);
                    this.SetRefObject("Flow", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// HisFrms
        /// </summary>
        public Frms HisFrms
        {
            get
            {
                Frms frms = new Frms();
                FrmNodes fns = new FrmNodes(this.NodeID);
                foreach (FrmNode fn in fns)
                    frms.AddEntity(fn.HisFrm);
                return frms;

                //this.SetRefObject("HisFrms", obj);
                //Frms obj = this.GetRefObject("HisFrms") as Frms;
                //if (obj == null)
                //{
                //    obj = new Frms();
                //    FrmNodes fns = new FrmNodes(this.NodeID);
                //    foreach (FrmNode fn in fns)
                //        obj.AddEntity(fn.HisFrm);
                //    this.SetRefObject("HisFrms", obj);
                //}
                //return obj;
            }
        }
        /// <summary>
        /// 他的将要来自的方向集合
        /// 如果他没有到来的方向,他就是开始节点.
        /// </summary>
        public Nodes FromNodes
        {
            get
            {
                Nodes obj = this.GetRefObject("HisFromNodes") as Nodes;
                if (obj == null)
                {
                    // 根据方向生成到达此节点的节点。
                    Directions ens = new Directions();
                    if (this.IsStartNode)
                        obj = new Nodes();
                    else
                        obj = ens.GetHisFromNodes(this.NodeID);
                    this.SetRefObject("HisFromNodes", obj);
                }
                return obj;
            }
        }
        public BillTemplates BillTemplates
        {
            get
            {
                BillTemplates obj= this.GetRefObject("BillTemplates") as BillTemplates;
                if (obj == null)
                {
                    obj = new BillTemplates(this.NodeID);
                    this.SetRefObject("BillTemplates", obj);
                }
                return obj;
            }
        }
        public NodeStations NodeStations
        {
            get
            {
                NodeStations obj = this.GetRefObject("NodeStations") as NodeStations;
                if (obj == null)
                {
                    obj = new NodeStations(this.NodeID);
                    this.SetRefObject("NodeStations", obj);
                }
                return obj;
            }
        }
        public NodeDepts NodeDepts
        {
            get
            {
                NodeDepts obj = this.GetRefObject("NodeDepts") as NodeDepts;
                if (obj == null)
                {
                    obj = new NodeDepts(this.NodeID);
                    this.SetRefObject("NodeDepts", obj);
                }
                return obj;
            }
        }
        public NodeEmps NodeEmps
        {
            get
            {
                NodeEmps obj = this.GetRefObject("NodeEmps") as NodeEmps;
                if (obj == null)
                {
                    obj = new NodeEmps(this.NodeID);
                    this.SetRefObject("NodeEmps", obj);
                }
                return obj;
            }
        }
        public FrmNodes FrmNodes
        {
            get
            {
                FrmNodes obj = this.GetRefObject("FrmNodes") as FrmNodes;
                if (obj == null)
                {
                    obj = new FrmNodes(this.NodeID);
                    this.SetRefObject("FrmNodes", obj);
                }
                return obj;
            }
        }
        public MapData MapData
        {
            get
            {
                MapData obj = this.GetRefObject("MapData") as MapData;
                if (obj == null)
                {
                    obj = new MapData("ND"+this.NodeID);
                    this.SetRefObject("MapData", obj);
                }
                return obj;
            }
        }
        #endregion

        #region 初试化全局的 Node
        public override string PK
        {
            get
            {
                return "NodeID";
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
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 初试化全局的
        /// </summary>
        /// <returns></returns>
        public NodePosType GetHisNodePosType()
        {
            string nodeid = this.NodeID.ToString();
            if (nodeid.Substring(nodeid.Length - 2) == "01")
                return NodePosType.Start;

            if (this.FromNodes.Count == 0)
                return NodePosType.Mid;

            if (this.HisToNodes.Count == 0)
                return NodePosType.End;

            return NodePosType.Mid;
        }
        /// <summary>
        /// 检查流程
        /// </summary>
        /// <param name="fl"></param>
        /// <returns></returns>
        public static string CheckFlow(Flow fl)
        {
            Nodes nds = new Nodes();
            nds.Retrieve(NodeAttr.FK_Flow, fl.No);

            if (nds.Count == 0)
                return "流程[" + fl.No + fl.Name + "]中没有节点数据，您需要注册一下这个流程。";

            // 更新是否是有完成条件的节点。
            DA.DBAccess.RunSQL("UPDATE WF_Node SET IsCCNode=0,IsCCFlow=0  WHERE FK_Flow='" + fl.No + "'");

            //DA.DBAccess.RunSQL("UPDATE WF_Node SET IsCCNode=1 WHERE NodeID IN (SELECT NodeID FROM WF_NodeCompleteCondition)");
            //   DA.DBAccess.RunSQL("UPDATE WF_Node SET IsCCFlow=1 WHERE NodeID IN (SELECT NodeID FROM WF_FlowCompleteCondition) ");

            DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE Node=0 OR ToNode=0");
            DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE Node  NOT IN (SELECT NODEID FROM WF_NODE )");
            DA.DBAccess.RunSQL("DELETE FROM WF_Direction WHERE ToNode  NOT IN (SELECT NODEID FROM WF_NODE) ");

            //更新流程类别
            // DA.DBAccess.RunSQL("UPDATE WF_Node SET FK_FlowSort=(SELECT FK_FlowSort FROM WF_Flow WHERE WF_Flow.No=WF_Node.FK_Flow) ");
            //   DA.DBAccess.RunSQL("UPDATE WF_Node SET FK_FlowSortT=(SELECT Name FROM WF_FlowSort WHERE No=WF_NODE.FK_FlowSort)");


            // 单据信息，岗位，节点信息。
            foreach (Node nd in nds)
            {
                DA.DBAccess.RunSQL("UPDATE WF_Node SET FK_FlowSort='" + fl.FK_FlowSort + "',FK_FlowSortT='" + fl.FK_FlowSortText + "'");

                BP.Sys.MapData md = new BP.Sys.MapData();
                md.No = "ND" + nd.NodeID;
                if (md.IsExits == false)
                {
                    nd.CreateMap();
                }

                // 工作岗位。
                NodeStations stas = new NodeStations(nd.NodeID);
                string strs = "";
                foreach (NodeStation sta in stas)
                    strs += "@" + sta.FK_Station;

                nd.HisStas = strs;

                // 工作部门。
                NodeDepts ndpts = new NodeDepts(nd.NodeID);
                strs = "";
                foreach (NodeDept ndp in ndpts)
                    strs += "@" + ndp.FK_Dept;

                nd.HisDeptStrs = strs;

                // 可执行人员。
                NodeEmps ndemps = new NodeEmps(nd.NodeID);
                strs = "";
                foreach (NodeEmp ndp in ndemps)
                    strs += "@" + ndp.FK_Emp;
                nd.HisEmps = strs;

                // 子流程。
                NodeFlows ndflows = new NodeFlows(nd.NodeID);
                strs = "";
                foreach (NodeFlow ndp in ndflows)
                    strs += "@" + ndp.FK_Flow;
                nd.HisSubFlows = strs;

                // 节点方向.
                strs = "";
                Directions dirs = new Directions(nd.NodeID,0);
                foreach (Direction dir in dirs)
                    strs += "@" + dir.ToNode;
                nd.HisToNDs = strs;

                // 单据
                strs = "";
                BillTemplates temps = new BillTemplates(nd);
                foreach (BillTemplate temp in temps)
                    strs += "@" + temp.No;
                nd.HisBillIDs = strs;

                // 检查节点的位置属性。
                nd.HisNodePosType = nd.GetHisNodePosType();
                nd.DirectUpdate();
            }

            //// 处理抄送人员对象。
            //string sql = "select FK_Station from WF_FlowStation WHERE fk_flow='" + fl.No + "'";
            //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //string mystas = "";
            //foreach (DataRow dr in dt.Rows)
            //{
            //    mystas += dr[0].ToString() + ",";
            //}
         //   fl.CCStas = mystas;

            // 处理岗位分组.
            string sql = "SELECT HisStas, COUNT(*) as NUM FROM WF_Node WHERE FK_Flow='" + fl.No + "' GROUP BY HisStas";
           DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string stas = dr[0].ToString();
                string nodes = "";
                foreach (Node nd in nds)
                {
                    if (nd.HisStas == stas)
                        nodes += "@" + nd.NodeID;
                }

                foreach (Node nd in nds)
                {
                    if (nodes.Contains("@" + nd.NodeID.ToString()) == false)
                        continue;

                    nd.GroupStaNDs = nodes;
                    nd.DirectUpdate();
                }
            }

            /* 判断流程的类型 */
            sql = "SELECT Name FROM WF_Node WHERE (NodeWorkType=" + (int)NodeWorkType.StartWorkFL + " OR NodeWorkType=" + (int)NodeWorkType.WorkFHL + " OR NodeWorkType=" + (int)NodeWorkType.WorkFL + " OR NodeWorkType=" + (int)NodeWorkType.WorkHL + ") AND (FK_Flow='" + fl.No + "')";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                fl.HisFlowType = FlowType.Panel;
            else
                fl.HisFlowType = FlowType.FHL;

            //fl.EnsName = fl.HisStartNode.EnsName;
            //fl.EnName = fl.HisStartNode.EnName;
            //fl.StartNodeID = fl.HisStartNode.NodeID;
            fl.DirectUpdate();
            return null;
        }
        protected override bool beforeUpdate()
        {
            if (this.IsStartNode)
            {
                this.SetValByKey(BtnAttr.ReturnRole, (int)ReturnRole.CanNotReturn);
                this.SetValByKey(BtnAttr.ShiftEnable, 0);
                //  this.SetValByKey(BtnAttr.CCRole, 0);
                this.SetValByKey(BtnAttr.EndFlowEnable, 0);
            }

            #region 检查质量考核点.
            if (this.IsEval)
            {
                /*如果是质量考核点，检查节点表单是否具别质量考核的特别字段？*/
                //string sql = "SELECT COUNT(*) FROM Sys_MapAttr WHERE FK_MapData='ND" + this.NodeID + "' AND KeyOfEn IN ('EvalEmpNo','EvalEmpName','EvalEmpCent')";
                //if (DBAccess.RunSQLReturnValInt(sql) != 3)
                //    throw new Exception("@您设置了当前节点为质量考核节点，但是您没有在该节点表单中设置必要的节点考核字段详细请参考操作手册。");
            }
            #endregion

            #region 对访问规则进行检查
            switch (this.HisDeliveryWay)
            {
                case DeliveryWay.ByStation:
                    //if (nd.HisStations.Count == 0)
                    //    rpt += "<font color=red>您设置了该节点的访问规则是按岗位，但是您没有为节点绑定岗位。</font>";
                    break;
                case DeliveryWay.ByDept:
                    //if (nd.HisDepts.Count == 0)
                    //    rpt += "<font color=red>您设置了该节点的访问规则是按部门，但是您没有为节点绑定部门。</font>";
                    break;
                case DeliveryWay.ByBindEmp:
                    //if (nd.HisNodeEmps.Count == 0)
                    //    rpt += "<font color=red>您设置了该节点的访问规则是按人员，但是您没有为节点绑定人员。</font>";
                    break;
                case DeliveryWay.BySQL:
                    //if (nd.IsStartNode)
                    //{
                    //    rpt += "<font color=red>开始节点不支持按SQL 设置访问规则。</font>";
                    //    break;
                    //}
                    //if (nd.DeliveryParas.Trim().Length == 0)
                    //    rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询，但是您没有在节点属性里设置查询sql，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册 。</font>";
                    //else
                    //{
                    //    try
                    //    {
                    //        DataTable testDB = DBAccess.RunSQLReturnTable(nd.DeliveryParas);
                    //        if (testDB.Columns.Contains("No") == false || testDB.Columns.Contains("Name") == false)
                    //        {
                    //            rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询，设置的sql不符合规则，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册 。</font>";
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询,执行此语句错误." + ex.Message + "</font>";
                    //    }
                    //}
                    break;
               
                case DeliveryWay.BySelected: /* 由上一步发送人员选择 */
                    //if (nd.IsStartNode)
                    //{
                    //    rpt += "<font color=red>开始节点不能设置指定的选择人员访问规则。</font>";
                    //    break;
                    //}

                    //if (attrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, "FK_Emp") == false)
                    //{
                    //    /*检查节点字段是否有FK_Emp字段*/
                    //    rpt += "<font color=red>您设置了该节点的访问规则是按指定节点表单人员，但是您没有在节点表单中增加FK_Emp字段，详细参考开发手册 。</font>";
                    //}
                    break;
                case DeliveryWay.ByPreviousOper: /* 由上一步发送人员选择 */
                case DeliveryWay.ByPreviousOperSkip: /* 由上一步发送人员选择 */
                    //if (nd.IsStartNode)
                    //{
                    //    rpt += "<font color=red>节点访问规则设置错误:开始节点。</font>";
                    //    break;
                    //}
                    break;
                default:
                    break;
            }
            #endregion

            #region 更新流程判断条件的标记。
            DBAccess.RunSQL("UPDATE WF_Node SET IsCCNode=0,IsCCFlow=0  WHERE FK_Flow='" + this.FK_Flow + "'");
            DBAccess.RunSQL("UPDATE WF_Node SET IsCCNode=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond WHERE CondType=0) AND FK_Flow='" + this.FK_Flow + "'");
            DBAccess.RunSQL("UPDATE WF_Node SET IsCCFlow=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond WHERE CondType=1) AND FK_Flow='" + this.FK_Flow + "'");
            #endregion

            Flow fl = new Flow();
            fl.No = this.FK_Flow;
            fl.RetrieveFromDBSources();

            Node.CheckFlow(fl);
            this.FlowName = fl.Name;

            DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + this.Name + "' WHERE No='ND" + this.NodeID + "'");
            switch (this.HisRunModel)
            {
                case RunModel.Ordinary:
                    if (this.IsStartNode)
                        this.HisNodeWorkType = NodeWorkType.StartWork;
                    else
                        this.HisNodeWorkType = NodeWorkType.Work;
                    break;
                case RunModel.FL:
                    if (this.IsStartNode)
                        this.HisNodeWorkType = NodeWorkType.StartWorkFL;
                    else
                        this.HisNodeWorkType = NodeWorkType.WorkFL;
                    break;
                case RunModel.HL:
                    if (this.IsStartNode)
                        throw new Exception("@您不能设置开始节点为合流节点。");
                    else
                        this.HisNodeWorkType = NodeWorkType.WorkHL;
                    break;
                case RunModel.FHL:
                    if (this.IsStartNode)
                        throw new Exception("@您不能设置开始节点为分合流节点。");
                    else
                        this.HisNodeWorkType = NodeWorkType.WorkFHL;
                    break;
                case RunModel.SubThread:
                    this.HisNodeWorkType = NodeWorkType.SubThreadWork;
                    break;
                default:
                    throw new Exception("eeeee");
                    break;
            }
            return base.beforeUpdate();
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 内部编号
        /// </summary>
        public string No
        {
            get
            {
                try
                {
                    return this.NodeID.ToString().Substring(this.NodeID.ToString().Length - 2);
                }
                catch(Exception ex)
                {
                    Log.DefaultLogWriteLineInfo(ex.Message+" - "+this.NodeID);
                    throw new Exception("@没有获取到它的NodeID = "+this.NodeID);
                }
            }
        }
        /// <summary>
        /// 启动参数
        /// </summary>
        public string SubFlowStartParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.SubFlowStartParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubFlowStartParas, value);
            }
        }
        /// <summary>
        /// 子线程启动方式
        /// </summary>
        public SubFlowStartWay SubFlowStartWay
        {
            get
            {
                return (SubFlowStartWay)this.GetValIntByKey(NodeAttr.SubFlowStartWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.SubFlowStartWay, (int)value);
            }
        }

        public FormType HisFormType
        {
            get
            {
                return (FormType)this.GetValIntByKey(NodeAttr.FormType);
            }
            set
            {
                this.SetValByKey(NodeAttr.FormType, (int)value);
            }
        }

        /// <summary>
        /// OID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// FormUrl 
        /// </summary>
        public string FormUrl
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FormUrl);
            }
            set
            {
                this.SetValByKey(NodeAttr.FormUrl, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(EntityOIDNameAttr.Name);
            }
            set
            {
                this.SetValByKey(EntityOIDNameAttr.Name, value);
            }
        }
        /// <summary>
        /// 需要天数（限期）
        /// </summary>
        public float DeductDays
        {
            get
            {
                float i= this.GetValFloatByKey(NodeAttr.DeductDays);
                if (i == 0)
                    return 1;
                return i;
            }
            set
            {
                this.SetValByKey(NodeAttr.DeductDays, value);
            }
        }
        /// <summary>
        /// 最高扣分
        /// </summary>
        public float MaxDeductCent
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.MaxDeductCent);
            }
            set
            {
                this.SetValByKey(NodeAttr.MaxDeductCent, value);
            }
        }
        /// <summary>
        /// 最高辛苦加分
        /// </summary>
        public float SwinkCent
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.SwinkCent);
            }
            set
            {
                this.SetValByKey(NodeAttr.SwinkCent, value);
            }
        }
        /// <summary>
        /// 保存方式 @0=仅节点表 @1=节点与NDxxxRtp表.
        /// </summary>
        public SaveModel SaveModel
        {
            get
            {
                return (SaveModel)this.GetValIntByKey(NodeAttr.SaveModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.SaveModel, (int)value);
            }
        }
        /// <summary>
        /// 流程步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }

        /// <summary>
        /// 最终期限( 需要天数（限期）+警告天数)
        /// </summary>
        public float NeedCompleteDays
        {
            get
            {
                return this.DeductDays;
            }
        }
        /// <summary>
        /// 扣分率（分/天）
        /// </summary>
        public float DeductCent
        {
            get
            {
                return this.GetValFloatByKey(NodeAttr.DeductCent);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeductCent, value);
            }
        }
        /// <summary>
        /// 是否是开始节点
        /// </summary>
        public bool IsStartNode
        {
            get
            {
                if (this.No == "01")
                    return true;
                return false;

                //if (this.HisNodePosType == NodePosType.Start)
                //    return true;
                //else
                //    return false;
            }
        }
        /// <summary>
        /// x
        /// </summary>
        public int X
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.X);
            }
            set
            {
                this.SetValByKey(NodeAttr.X, value);
            }
        }
        public float WarningDays
        {
            get
            {
                if (this.GetValFloatByKey(NodeAttr.WarningDays) == 0)
                    return this.DeductDays;
                else
                    return this.DeductDays - this.GetValFloatByKey(NodeAttr.WarningDays);
            }
        }
        /// <summary>
        /// y
        /// </summary>
        public int Y
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Y);
            }
            set
            {
                this.SetValByKey(NodeAttr.Y, value);
            }
        }
        /// <summary>
        /// 水执行它？
        /// </summary>
        public int WhoExeIt
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.WhoExeIt);
            }
            set
            {
                this.SetValByKey(NodeAttr.WhoExeIt, value);
            }
        }
         
        /// <summary>
        /// 位置
        /// </summary>
        public NodePosType NodePosType
        {
            get
            {
                return (NodePosType)this.GetValIntByKey(NodeAttr.NodePosType);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodePosType, (int)value);
            }
        }
        /// <summary>
        /// 运行模式
        /// </summary>
        public RunModel HisRunModel
        {
            get
            {
                return (RunModel)this.GetValIntByKey(NodeAttr.RunModel);
            }
            set
            {
                this.SetValByKey(NodeAttr.RunModel, (int)value);
            }
        }
        /// <summary>
        /// 焦点字段
        /// </summary>
        public string FocusField
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FocusField);
            }
        }
        /// <summary>
        /// 节点的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FK_Flow);
            }
            set
            {
                SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 获取它的上一步的分流点
        /// </summary>
        private Node _GetHisPriFLNode(Nodes nds)
        {
            foreach (Node mynd in nds)
            {
                if (mynd.IsFL)
                    return mynd;
                else
                    return _GetHisPriFLNode(mynd.FromNodes);
            }
            return null;
        }
        /// <summary>
        /// 它的上一步分流节点
        /// </summary>
        public Node HisPriFLNode
        {
            get
            {
                return _GetHisPriFLNode(this.FromNodes);
            }
        }
        public string TurnToDealDoc
        {
            get
            {
                string s= this.GetValStrByKey(NodeAttr.TurnToDealDoc);
                if (this.HisTurnToDeal == TurnToDeal.SpecUrl)
                {
                    if (s.Contains("?"))
                        s += "&1=1";
                    else
                        s += "?1=1";
                }
                return s;
            }
            set
            {
                SetValByKey(NodeAttr.TurnToDealDoc, value);
            }
        }
        /// <summary>
        /// 可跳转的节点
        /// </summary>
        public string JumpSQL
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.JumpSQL);
            }
            set
            {
                SetValByKey(NodeAttr.JumpSQL, value);
            }
        }
     
        public string FlowName
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.FlowName);
            }
            set
            {
                SetValByKey(NodeAttr.FlowName, value);
            }
        }
        //public string EnsName
        //{
        //    get
        //    {
        //        return this.GetValStrByKey(NodeAttr.EnsName);
        //    }
        //    set
        //    {
        //        SetValByKey(NodeAttr.EnsName, value);
        //    }
        //}
        //public string EnName
        //{
        //    get
        //    {
        //        string ms = this.GetValStrByKey(NodeAttr.EnName);
        //        if (ms == null || ms.Trim() == "")
        //        {
        //            ms = this.HisWorks.GetNewEntity.ToString();
        //            this.SetValByKey(NodeAttr.EnName, ms);
        //            this.Update(NodeAttr.EnName, ms);
        //        }
        //        return ms;
        //    }
        //    set
        //    {
        //        SetValByKey(NodeAttr.EnName, value);
        //    }
        //}
        public string PTable
        {
            get
            {
                
                return "ND" + this.NodeID;
            }
            set
            {
                SetValByKey(NodeAttr.PTable, value);
            }
        }
        /// <summary>
        /// 要显示在后面的表单
        /// </summary>
        public string ShowSheets
        {
            get
            {
                string s = this.GetValStrByKey(NodeAttr.ShowSheets);
                if (s == "")
                    return "@";
                return s;
            }
            set
            {
                SetValByKey(NodeAttr.ShowSheets, value);
            }
        }
        /// <summary>
        /// Doc
        /// </summary> 
        public string Doc
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.Doc);
            }
            set
            {
                SetValByKey(NodeAttr.Doc, value);
            }
        }
        public string GroupStaNDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.GroupStaNDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.GroupStaNDs, value);
            }
        }

        public string HisToNDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisToNDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisToNDs, value);
            }
        }
        public string HisDeptStrs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisDeptStrs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisDeptStrs, value);
            }
        }
        public string HisStas
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisStas);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisStas, value);
            }
        }
        public string HisEmps
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisEmps);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisEmps, value);
            }
        }
        public string HisBillIDs
        {
            get
            {
                return this.GetValStrByKey(NodeAttr.HisBillIDs);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisBillIDs, value);
            }
        }
        #endregion

        #region 扩展属性
      
     
        /// <summary>
        /// 是不是多岗位工作节点.
        /// </summary>
        public bool IsMultiStations
        {
            get
            {
                if (this.NodeStations.Count > 1)
                    return true;
                return false;
            }
        }
        public string HisStationsStr
        {
            get
            {
                string s = "";
                foreach (NodeStation ns in this.NodeStations)
                {
                    s += ns.FK_StationT + ",";
                }
                return s;
            }
        }
      

       

        
        #endregion

        #region 公共方法
        /// <summary>
        /// 得到一个工作data实体
        /// </summary>
        /// <param name="workId">工作ID</param>
        /// <returns>如果没有就返回null</returns>
        public Work GetWork(Int64 workId)
        {
            Work wk = this.HisWork;
            wk.SetValByKey("OID", workId);
            if (wk.RetrieveFromDBSources() == 0)
                return null;
            else
                return wk;
            return wk;
        }
        #endregion

        #region 节点的工作类型
        /// <summary>
        /// 转向处理
        /// </summary>
        public TurnToDeal HisTurnToDeal
        {
            get
            {
                return (TurnToDeal)this.GetValIntByKey(NodeAttr.TurnToDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.TurnToDeal, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        public CCRole HisCCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(NodeAttr.CCRole);
            }
        }
        /// <summary>
        /// 删除流程规则
        /// </summary>
        public DelWorkFlowRole HisDelWorkFlowRole
        {
            get
            {
                return (DelWorkFlowRole)this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }
        /// <summary>
        /// 附件开放类型
        /// </summary>
        public FJOpen HisFJOpen
        {
            get
            {
                return (FJOpen)this.GetValIntByKey(NodeAttr.FJOpen);
            }
            set
            {
                this.SetValByKey(NodeAttr.FJOpen, (int)value);
            }
        }
        /// <summary>
        /// 分流规则
        /// </summary>
        public FLRole HisFLRole
        {
            get
            {
                return (FLRole)this.GetValIntByKey(NodeAttr.FLRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.FLRole, (int)value);
            }
        }
        /// <summary>
        /// Int type
        /// </summary>
        public NodeWorkType HisNodeWorkType
        {
            get
            {
#warning 2012-01-24修订,没有自动计算出来属性。
                switch (this.HisRunModel)
                {
                    case RunModel.Ordinary:
                        if (this.IsStartNode)
                            return NodeWorkType.StartWork;
                        else
                            return NodeWorkType.Work;
                    case RunModel.FL:
                        if (this.IsStartNode)
                            return NodeWorkType.StartWorkFL;
                        else
                            return NodeWorkType.WorkFL;
                    case RunModel.HL:
                            return NodeWorkType.WorkHL;
                    case RunModel.FHL:
                        return NodeWorkType.WorkFHL;
                    case RunModel.SubThread:
                        return NodeWorkType.SubThreadWork;
                    default:
                        throw new Exception("@没有判断类型NodeWorkType.");
                }
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeWorkType, (int)value);
            }
        }
        public string HisNodeWorkTypeT
        {
            get
            {
                return this.HisNodeWorkType.ToString();

                //Sys.SysEnum se = new Sys.SysEnum(NodeAttr.NodeWorkType, this.GetValIntByKey(NodeAttr.NodeWorkType));
                //return se.Lab;
            }
        }
        #endregion

        #region 推算属性 (对于节点位置的判断)
       
        /// <summary>
        /// 类型
        /// </summary>
        public NodePosType HisNodePosType
        {
            get
            {
                if (SystemConfig.IsDebug)
                {
                    this.SetValByKey(NodeAttr.NodePosType, (int)this.GetHisNodePosType());
                    return this.GetHisNodePosType();
                }
                return (NodePosType)this.GetValIntByKey(NodeAttr.NodePosType);
            }
            set
            {
                if (value == NodePosType.Start)
                    if (this.No != "01")
                        value = NodePosType.Mid;

                this.SetValByKey(NodeAttr.NodePosType, (int)value);
            }
        }
        /// <summary>
        /// 是不是结束节点
        /// </summary>
        public bool IsEndNode
        {
            get
            {
                if (this.HisNodePosType == NodePosType.End)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否允许子线程接受人员重复(对子线程点有效)?
        /// </summary>
        public bool IsAllowRepeatEmps
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsAllowRepeatEmps);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsAllowRepeatEmps, value);
            }
        }
        /// <summary>
        /// 是否可以在退回后原路返回？
        /// </summary>
        public bool IsBackTracking
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsBackTracking);
            }
        }
        /// <summary>
        /// 是否启用自动记忆功能
        /// </summary>
        public bool IsRememberMe
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsRM);
            }
        }
        public bool IsCanDelFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCanDelFlow);
            }
        }
        public bool IsForceKill
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsForceKill);
            }
        }
        /// <summary>
        /// 是否保密步骤
        /// </summary>
        public bool IsSecret
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsSecret);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsSecret, value);
            }
        }
        public decimal PassRate
        {
            get
            {
                return this.GetValDecimalByKey(NodeAttr.PassRate);
            }
            set
            {
                this.SetValByKey(NodeAttr.PassRate, value);
            }  
        }
        /// <summary>
        /// 是否允许分配工作
        /// </summary>
        public bool IsTask
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsTask);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsTask, value);
            }
        }
        public bool IsCanOver
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCanOver);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCanOver, value);
            }
        }
        public bool IsCanRpt
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCanRpt);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCanRpt, value);
            }
        }
        /// <summary>
        /// 是否可以移交
        /// </summary>
        public bool IsHandOver
        {
            get
            {
                if (this.IsStartNode)
                    return false;

                return this.GetValBooleanByKey(NodeAttr.IsHandOver);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsHandOver, value);
            }
        }
        public bool IsCanHidReturn_del
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCanHidReturn);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCanHidReturn, value);
            }
        }
        public bool IsCanReturn
        {
            get
            {
                if (this.HisReturnRole == ReturnRole.CanNotReturn)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 已读回执
        /// </summary>
        public ReadReceipts ReadReceipts
        {
            get
            {
                return (ReadReceipts)this.GetValIntByKey(NodeAttr.ReadReceipts);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReadReceipts, (int)value);
            }
        }
        /// <summary>
        /// 退回规则
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// 是不是中间节点
        /// </summary>
        public bool IsMiddleNode
        {
            get
            {
                if (this.HisNodePosType == NodePosType.Mid)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 是否是工作质量考核点
        /// </summary>
        public bool IsEval
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsEval);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsEval, value);
            }
        }
        /// <summary>
        /// 是否有节点完成条件。
        /// </summary>
        public bool IsCCNode
        {
            get
            {
                return false;
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCCNode, value);
            }
        }
        public string HisSubFlows
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.HisSubFlows);
            }
            set
            {
                this.SetValByKey(NodeAttr.HisSubFlows, value);
            }
        }
        public string FrmAttr
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FrmAttr);
            }
            set
            {
                this.SetValByKey(NodeAttr.FrmAttr, value);
            }
        }
        /// <summary>
        /// 就是否有子流程
        /// </summary>
        public bool IsHaveSubFlow
        {
            get
            {
                if (this.HisSubFlows.Length > 2)
                    return true;
                else
                    return false;
            }
        }
        public bool IsHL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkHL:
                    case NodeWorkType.WorkFHL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 是否是分流
        /// </summary>
        public bool IsFL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkFL:
                    case NodeWorkType.WorkFHL:
                    case NodeWorkType.StartWorkFL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 是否分流合流
        /// </summary>
        public bool IsFLHL
        {
            get
            {
                switch (this.HisNodeWorkType)
                {
                    case NodeWorkType.WorkHL:
                    case NodeWorkType.WorkFL:
                    case NodeWorkType.WorkFHL:
                    case NodeWorkType.StartWorkFL:
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 是否有流程完成条件
        /// </summary>
        public bool IsCCFlow
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsCCFlow);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsCCFlow, value);
            }
        }
        /// <summary>
        /// 接受人sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                string s= this.GetValStringByKey(NodeAttr.DeliveryParas);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// 是不是PC工作节点
        /// </summary>
        public bool IsPCNode
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 工作性质
        /// </summary>
        public string NodeWorkTypeText
        {
            get
            {
                return this.HisNodeWorkType.ToString();
            }
        }
        #endregion

        #region 公共方法 (用户执行动作之后,所要做的工作)
        /// <summary>
        /// 用户执行动作之后,所要做的工作		 
        /// </summary>
        /// <returns>返回消息,运行的消息</returns>
        public string AfterDoTask()
        {
            return "";
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public Node() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="_oid">节点ID</param>	
        public Node(int _oid)
        {
            this.NodeID = _oid;
            if (SystemConfig.IsDebug)
            {
                if (this.RetrieveFromDBSources() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            }
            else
            {
                // 去掉缓存.
                this.RetrieveFromDBSources();
                //if (this.Retrieve() <= 0)
                //    throw new Exception("Node Retrieve 错误没有ID=" + _oid);
            }
        }
        public Node(string ndName)
        {
            ndName = ndName.Replace("ND", "");
            this.NodeID = int.Parse(ndName);

            if (SystemConfig.IsDebug)
            {
                if (this.RetrieveFromDBSources() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + ndName);
            }
            else
            {
                if (this.Retrieve() <= 0)
                    throw new Exception("Node Retrieve 错误没有ID=" + ndName);
            }
        }
        public string EnName
        {
            get
            {
                return "ND" + this.NodeID;
            }
        }
        public string EnsName
        {
            get
            {
                return "ND" + this.NodeID + "s";
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

                Map map = new Map("WF_Node");
                map.EnDesc = "节点"; // "节点";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 100, 10);
                map.AddTBInt(NodeAttr.Step, (int)NodeWorkType.Work,  "流程步骤", true, false);

                map.AddTBInt(NodeAttr.NodeWorkType, 0, "节点类型", false, false);
                map.AddTBInt(NodeAttr.SubThreadType, 0, "子线程ID", false, false);

                map.AddTBString(NodeAttr.FK_Flow, null, "FK_Flow", false, false, 0, 3, 10);

                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 100, 10);
                map.AddTBString(NodeAttr.FK_FlowSort, null, "FK_FlowSort", false, true, 0, 4, 10);
                map.AddTBString(NodeAttr.FK_FlowSortT, null, "FK_FlowSortT", false, true, 0, 100, 10);
                map.AddTBString(NodeAttr.FrmAttr, null, "FrmAttr", false, true, 0, 300, 10);

                map.AddTBFloat(NodeAttr.WarningDays, 0,  "警告期限(0不警告)", false, false); // "警告期限(0不警告)"
                map.AddTBFloat(NodeAttr.DeductDays, 1, "限期(天)", false, false); //"限期(天)"
                map.AddTBFloat(NodeAttr.DeductCent, 2, "扣分(每延期1天扣)", false, false); //"扣分(每延期1天扣)"
                map.AddTBFloat(NodeAttr.MaxDeductCent, 10, "最高扣分", false, false); //"最高扣分"
                map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"), "工作得分", false, false); //"工作得分"

                //map.AddDDLSysEnum(NodeAttr.DoWhere, 0, "在那里处理", true, true);
                //map.AddDDLSysEnum(NodeAttr.RunType, 0, "执行类型", true, true);
                //map.AddTBStringDoc(NodeAttr.DoWhat, null, "工作完成后处理什么", true, false);
                //map.AddTBStringDoc(NodeAttr.DoWhatMsg, null, "提示执行信息", true, false);



                map.AddTBString(NodeAttr.Doc, null, "描述", true, false, 0, 100, 10);
                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true);

                map.AddTBInt(NodeAttr.ReturnRole, 2, "退回规则", true, true);
                map.AddTBInt(NodeAttr.DeliveryWay, 0, "访问规则", true, true);
                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 500, 10);


                map.AddTBInt(NodeAttr.CCRole, 0, "抄送规则", true, true);
                map.AddTBInt(BtnAttr.DelEnable, 0, "删除规则", true, true);
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", true, true);
                map.AddTBInt(NodeAttr.SaveModel, 0, "保存模式", true, true);

                //map.AddBoolean(NodeAttr.IsCanReturn, true, "是否可以退回", true, true);
                //map.AddBoolean(NodeAttr.IsCanHidReturn, false, "是否可以隐性退回", true, true);
                //map.AddBoolean(NodeAttr.CCRole, true, "是否可以抄送", true, true);


                map.AddBoolean(NodeAttr.IsCanRpt, true, "是否可以查看工作报告?", true, true, true);
                map.AddBoolean(NodeAttr.IsCanOver, false, "是否可以终止流程", true, true);
                map.AddBoolean(NodeAttr.IsSecret, false, "是否是保密步骤", true, true);
                map.AddBoolean(NodeAttr.IsCanDelFlow, false, "是否可以删除流程", true, true);
                map.AddBoolean(NodeAttr.IsForceKill, false, "是否可以强制删除了流程(对合流点有效)", true, true);
                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "是否允许子线程接受人员重复(对子线程点有效)?", true, true, true);
                map.AddBoolean(NodeAttr.IsBackTracking, false, "是否可以在退回后原路返回(只有启用退回功能才有效)", true, true, true);
                map.AddBoolean(NodeAttr.IsRM, true, "是否起用投递路径自动记忆功能?", true, true, true);

                map.AddBoolean(NodeAttr.IsHandOver, false, "是否可以移交", true, true);
                map.AddTBInt(NodeAttr.PassRate, 100, "通过率", true, true);

                //map.AddDDLSysEnum(NodeAttr.SignType, 0, "审核模式(对审核节点有效)", true, true, 
                //    NodeAttr.SignType, "@0=单签@1=汇签");

                map.AddTBInt(NodeAttr.RunModel, 0, "运行模式(对普通节点有效)", true, true);

                //map.AddDDLSysEnum(NodeAttr.RunModel, 0, "运行模式(对普通节点有效)", true, true,
                //    NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=子线程");
                //map.AddDDLSysEnum(NodeAttr.FLRole, 0, "分流规则", true, true, 
                //    NodeAttr.FLRole, "@0=按接受人@1=按部门@2=按岗位");

                map.AddTBInt(NodeAttr.FLRole, 0, "分流规则", true, true);
                map.AddTBInt(NodeAttr.FJOpen, 0, "附件权限", true, true);
                map.AddTBInt(NodeAttr.WhoExeIt, 0, "谁执行它", true, true);
                map.AddTBInt(NodeAttr.ReadReceipts, 0, "已读回执", true, true);

                
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, "附件权限", true, true, NodeAttr.FJOpen,
                //    "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                //// 流程的节点分为干流支流. FNType  @0=平面节点@1=干流@2=支流.
                //map.AddTBInt(NodeAttr.FNType, (int)FNType.Plane, "流程节点类型", false, false);

                //默认为自由表单.
                map.AddDDLSysEnum(NodeAttr.FormType, 1, "表单类型", true, true);

                map.AddTBString(NodeAttr.FormUrl, "http://", "表单URL", true, false, 0, 200, 10);

                map.AddTBString(NodeAttr.DeliveryParas, null, "接受人SQL", true, false, 0, 300, 10, true);

                map.AddTBInt(NodeAttr.TurnToDeal, 0, "转向处理", false, false);
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "发送后提示信息", true, false, 0, 2000, 10, true);
                map.AddTBInt(NodeAttr.NodePosType, 0, "位置", false, false);

                map.AddTBInt(NodeAttr.IsCCNode, 0, "是否有节点完成条件", false, false);
                map.AddTBInt(NodeAttr.IsCCFlow, 0, "是否有流程完成条件", false, false);

                map.AddTBString(NodeAttr.HisStas, null, "岗位", false, false, 0, 1000, 10);
                map.AddTBString(NodeAttr.HisDeptStrs, null, "部门", false, false, 0, 1000, 10);

                map.AddTBString(NodeAttr.HisToNDs, null, "转到的节点", false, false, 0, 300, 10);
                map.AddTBString(NodeAttr.HisBillIDs, null, "单据IDs", false, false, 0, 200, 10);
                map.AddTBString(NodeAttr.HisEmps, null, "HisEmps", false, false, 0, 4000, 10);
                map.AddTBString(NodeAttr.HisSubFlows, null, "HisSubFlows", false, false, 0, 200, 10);

                map.AddTBString(NodeAttr.PTable, null, "物理表", false, false, 0, 100, 10);

                map.AddTBString(NodeAttr.ShowSheets, null, "显示的表单", false, false, 0, 100, 10);
                map.AddTBString(NodeAttr.GroupStaNDs, null, "岗位分组节点", false, false, 0, 200, 10);
                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.JumpSQL, null, "可跳转的节点", true, false, 0, 200, 10, true);

                map.AddTBAtParas(1000);

                // 启动子线程参数 2013-01-04
                map.AddTBInt(NodeAttr.SubFlowStartWay, 0, "子线程启动方式", true, false);
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10);

                //map.AddTBDate(FlowAttr.LifeCycleFrom, BP.DA.DataType.CurrentData, "生命周期从", true, false);
                //map.AddTBDate(FlowAttr.LifeCycleTo, DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd"), "到", true, false);

                map.AttrsOfOneVSM.Add(new NodeEmps(), new Emps(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name, DeptAttr.No, "接受人员");
                map.AttrsOfOneVSM.Add(new NodeDepts(), new Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name, DeptAttr.No, "接受部门");
                map.AttrsOfOneVSM.Add(new NodeStations(), new Stations(), NodeStationAttr.FK_Node, NodeStationAttr.FK_Station, DeptAttr.Name, DeptAttr.No, "岗位");


                RefMethod rm = new RefMethod();
                rm.Title = "设计表单" ; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoMapData";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "单据&单据"; //"单据&单据";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "/WF/Img/FileType/doc.gif";


                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =   "调用外部程序接口"; // "调用外部程序接口";
                rm.ClassMethodName = this.ToString() + ".DoFAppSet";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =   "调用事件接口"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单显示"; // this.ToE("DoAction", "调用事件接口"); // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoShowSheets";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点完成条件"; // "节点完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "消息收听"; // "节点完成条件";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "特性集"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoFeatureSet";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("DoCondFL", "分流完成规则"); // "分流完成规则";
                //rm.ClassMethodName = this.ToString() + ".DoCondFL";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 我能处理当前的节点吗？
        /// </summary>
        /// <returns></returns>
        public bool CanIdoIt()
        {


            return false;
        }

        public string DoListen()
        {
            PubClass.WinOpen("/WF/Admin/Listen.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=", "单据", "cdn",
                600, 600, 100, 150);
            return null;
        }
        public string DoFeatureSet()
        {
            PubClass.WinOpen("/WF/Admin/FeatureSetUI.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=", "单据", "cdn",
                800, 600, 20,150);
            return null;
        }
        public string DoShowSheets()
        {
            PubClass.WinOpen("/WF/Admin/ShowSheets.aspx?CondType=0&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=", "单据", "cdn", 400, 400, 200, 300);
            return null;
        }
        public string DoCond()
        {
            PubClass.WinOpen("/WF/Admin/Cond.aspx?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=", "单据", "cdn", 400, 400, 200, 300);
            return null;
        }
        
        public string DoMapData()
        {
            switch (this.HisFormType)
            {
                case FormType.FreeForm:
                    PubClass.WinOpen("/WF/MapDef/CCForm/Frm.aspx?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "设计表单", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case FormType.FixForm:
                    PubClass.WinOpen("/WF/MapDef/MapDef.aspx?PK=ND" + this.NodeID, "设计表单", "sheet", 800, 500, 210, 300);
                    break;
            }
            return null;
        }
        public string DoAction()
        {
            PubClass.WinOpen("/WF/Admin/Action.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "单据", "Bill", 800, 500, 200, 300);
            return null;
        }
        /// <summary>
        /// 转向
        /// </summary>
        /// <returns></returns>
        public string DoTurn()
        {
            PubClass.WinOpen("/WF/Admin/TurnTo.aspx?FK_Node=" + this.NodeID, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            return null;
        }
       
        public string DoBill()
        {
            PubClass.WinOpen("/WF/Admin/Bill.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "单据", "Bill", 800, 500, 200, 300);
            return null;
        }
        public string DoFAppSet()
        {
            PubClass.WinOpen("/WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "单据", "sd", 800, 500, 200, 200);
            //  PubClass.WinOpen("./../WF/Admin/FAppSet.aspx?NodeID=" + this.NodeID, 400, 500);
            return null;
        }
        #endregion

        protected override bool beforeDelete()
        {
            //判断是否可以被删除.
             int num = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Node="+this.NodeID);
             if (num != 0)
                throw new Exception("@该节点["+this.NodeID+","+this.Name+"]有待办工作存在，您不能删除它.");

            // 删除它的节点。
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            md.Delete();

            // 删除分组.
            BP.Sys.GroupFields gfs = new BP.Sys.GroupFields();
            gfs.Delete(BP.Sys.GroupFieldAttr.EnName, md.No);

            //删除它的明细。
            BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(md.No);
            dtls.Delete();

            //删除框架
            BP.Sys.MapFrames frams = new BP.Sys.MapFrames(md.No);
            frams.Delete();

            // 删除多选
            BP.Sys.MapM2Ms m2ms = new BP.Sys.MapM2Ms(md.No);
            m2ms.Delete();

            // 删除扩展
            BP.Sys.MapExts exts = new BP.Sys.MapExts(md.No);
            exts.Delete();

            //删除节点与岗位的对应.
            BP.DA.DBAccess.RunSQL("DELETE WF_NodeStation WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE WF_NodeEmp WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE WF_NodeDept WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE WF_NodeFlow WHERE FK_Node=" + this.NodeID);
            BP.DA.DBAccess.RunSQL("DELETE WF_FrmNode WHERE FK_Node=" + this.NodeID);

            return base.beforeDelete();
        }
        /// <summary>
        /// 文书流程
        /// </summary>
        /// <param name="md"></param>
        private void AddDocAttr(BP.Sys.MapData md)
        {
            /*如果是单据流程？ */
            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();

            //attr = new BP.Sys.MapAttr();
            //attr.FK_MapData = md.No;
            //attr.HisEditType = BP.En.EditType.UnDel;
            //attr.KeyOfEn = "Title";
            //attr.Name = "标题";
            //attr.MyDataType = BP.DA.DataType.AppString;
            //attr.UIContralType = UIContralType.TB;
            //attr.LGType = FieldTypeS.Normal;
            //attr.UIVisible = true;
            //attr.UIIsEnable = true;
            //attr.MinLen = 0;
            //attr.MaxLen = 300;
            //attr.IDX = 1;
            //attr.UIIsLine = true;
            //attr.IDX = -100;
            //attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "KeyWord";
            attr.Name = "主题词";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.UIIsLine = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = -99;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "FZ";
            attr.Name = "附注";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.UIIsLine = true;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.IDX = -98;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = "DW_SW";
            attr.Name = "收文单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.UIIsLine = true;
            attr.IDX = 1;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_FW";
            attr.Name = "发文单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_BS";
            attr.Name = "主报(送)单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "DW_CS";
            attr.Name = "抄报(送)单位";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = true;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "NumPrint";
            attr.Name = "印制份数";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 10;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "JMCD";
            attr.Name = "机密程度";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.DDL;
            attr.LGType = FieldTypeS.Enum;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.UIBindKey = "JMCD";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = "PRI";
            attr.Name = "紧急程度";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.DDL;
            attr.LGType = FieldTypeS.Enum;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.UIIsLine = false;
            attr.UIBindKey = "PRI";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "GWWH";
            attr.Name = "公文文号";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = true;
            attr.UIIsEnable = true;
            attr.MinLen = 0;
            attr.MaxLen = 300;
            attr.IDX = 1;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.UIIsLine = false;
            attr.Insert();
        }
        /// <summary>
        /// 修复map
        /// </summary>
        public string RepareMap()
        {
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            if (md.RetrieveFromDBSources() == 0)
            {
                this.CreateMap();
                return "";
            }

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "OID";
                attr.Name = "WorkID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, "FID", MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.KeyOfEn = "FID";
                attr.Name = "FID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.DefVal = "0";
                attr.Insert();
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, WorkAttr.RDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.RDT;
                attr.Name = "接受时间";  //"接受时间";
                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.Tag = "1";
                attr.Insert();
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, WorkAttr.CDT, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.CDT;
                if (this.IsStartNode)
                    attr.Name = "发起时间"; //"发起时间";
                else
                    attr.Name = "完成时间"; //"完成时间";

                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }


            if (attr.IsExit(MapAttrAttr.KeyOfEn, WorkAttr.Rec, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.Rec;
                if (this.IsStartNode == false)
                    attr.Name = "记录人"; // "记录人";
                else
                    attr.Name = "发起人"; //"发起人";

                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MaxLen = 20;
                attr.MinLen = 0;
                attr.DefVal = "@WebUser.No";
                attr.Insert();
            }


            if (attr.IsExit(MapAttrAttr.KeyOfEn, WorkAttr.Emps, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = WorkAttr.Emps;
                attr.Name = WorkAttr.Emps;
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MaxLen = 400;
                attr.MinLen = 0;
                attr.Insert();
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, StartWorkAttr.FK_Dept, MapAttrAttr.FK_MapData, md.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = StartWorkAttr.FK_Dept;
                attr.Name = "操作员部门"; //"操作员部门";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;
                attr.UIBindKey = "BP.Port.Depts";
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 20;
                attr.Insert();
            }

            Flow fl = new Flow(this.FK_Flow);
            if (fl.IsMD5
                && attr.IsExit(MapAttrAttr.KeyOfEn, WorkAttr.MD5, MapAttrAttr.FK_MapData, md.No) == false)
            {
                /* 如果是MD5加密流程. */
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = StartWorkAttr.MD5;
                attr.UIBindKey = attr.KeyOfEn;
                attr.Name = "MD5";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.UIVisible = false;
                attr.MinLen = 0;
                attr.MaxLen = 40;
                attr.IDX = -100;
                attr.Insert();
            }

            if (this.NodePosType == NodePosType.Start)
            {

                if (Glo.IsEnablePRI && this.IsStartNode
                    && attr.IsExit(MapAttrAttr.KeyOfEn, StartWorkAttr.PRI, MapAttrAttr.FK_MapData, md.No) == false)
                {
                    /* 如果有优先级 */
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = StartWorkAttr.PRI;
                    attr.UIBindKey = attr.KeyOfEn;
                    attr.Name = "优先级";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.Enum;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = false;
                    attr.MinLen = 0;
                    attr.MaxLen = 200;
                    attr.IDX = -100;
                    attr.DefVal = "0";
                    attr.X = (float)174.76;
                    attr.Y = (float)56.19;
                    attr.Insert();
                }

                
                if (attr.IsExit(MapAttrAttr.KeyOfEn, StartWorkAttr.Title, MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = StartWorkAttr.Title;
                    attr.Name = "标题"; // "流程标题";
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = true;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = true;
                    attr.UIWidth = 251;

                    attr.MinLen = 0;
                    attr.MaxLen = 200;
                    attr.IDX = -100;
                    attr.X = (float)171.2;
                    attr.Y = (float)68.4;
                    attr.Insert();
                }

                //if (attr.IsExit(MapAttrAttr.KeyOfEn, "faqiren", MapAttrAttr.FK_MapData, md.No) == false)
                //{
                //    attr = new BP.Sys.MapAttr();
                //    attr.FK_MapData = md.No;
                //    attr.HisEditType = BP.En.EditType.Edit;
                //    attr.KeyOfEn = "faqiren";
                //    attr.Name = "发起人"; // "发起人";
                //    attr.MyDataType = BP.DA.DataType.AppString;
                //    attr.UIContralType = UIContralType.TB;
                //    attr.LGType = FieldTypeS.Normal;
                //    attr.UIVisible = true;
                //    attr.UIIsEnable = false;
                //    attr.UIIsLine = false;
                //    attr.MinLen = 0;
                //    attr.MaxLen = 200;
                //    attr.IDX = -100;
                //    attr.DefVal = "@WebUser.No";
                //    attr.X = (float)159.2;
                //    attr.Y = (float)102.8;
                //    attr.Insert();
                //}

                //if (attr.IsExit(MapAttrAttr.KeyOfEn, "faqishijian", MapAttrAttr.FK_MapData, md.No) == false)
                //{
                //    attr = new BP.Sys.MapAttr();
                //    attr.FK_MapData = md.No;
                //    attr.HisEditType = BP.En.EditType.Edit;
                //    attr.KeyOfEn = "faqishijian";
                //    attr.Name = "发起时间"; //"发起时间";
                //    attr.MyDataType = BP.DA.DataType.AppDateTime;
                //    attr.UIContralType = UIContralType.TB;
                //    attr.LGType = FieldTypeS.Normal;
                //    attr.UIVisible = true;
                //    attr.UIIsEnable = false;
                //    attr.DefVal = "@RDT";
                //    attr.Tag = "1";
                //    attr.X = (float)324;
                //    attr.Y = (float)102.8;
                //    attr.Insert();
                //}


                if (attr.IsExit(MapAttrAttr.KeyOfEn, "FK_NY", MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = "FK_NY";
                    attr.Name = "年月"; //"年月";
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.LGType = FieldTypeS.Normal;
                    //attr.UIBindKey = "BP.Pub.NYs";
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.MinLen = 0;
                    attr.MaxLen = 7;
                    attr.Insert();
                }


                if (attr.IsExit(MapAttrAttr.KeyOfEn, "MyNum", MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = "MyNum";
                    attr.Name = "个数"; // "个数";
                    attr.DefVal = "1";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.Insert();
                }
            }
            string msg = "";
            if (this.FocusField != "")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, this.FocusField, MapAttrAttr.FK_MapData, md.No) == false)
                {
                    msg += "@焦点字段 " + this.FocusField + " 被非法删除了.";
                    //this.FocusField = "";
                    //this.DirectUpdate();
                }
            }
            return msg;
        }
        /// <summary>
        /// 建立map
        /// </summary>
        public void CreateMap()
        {
            //创建节点表单.
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + this.NodeID;
            md.Delete();

            md.Name = this.Name;
            if (this.HisFlow.HisDataStoreModel == DataStoreModel.SpecTable)
                md.PTable = this.HisFlow.PTable;
            md.Insert();

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "OID";
            attr.Name = "WorkID";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.DefVal = "0";
            attr.HisEditType = BP.En.EditType.Readonly;
            attr.Insert();

            if (this.HisFlow.HisFlowSheetType == FlowSheetType.DocFlow)
            {
                this.AddDocAttr(md);
            }

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.KeyOfEn = "FID";
            attr.Name = "FID";
            attr.MyDataType = BP.DA.DataType.AppInt;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.DefVal = "0";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.RDT;
            attr.Name = "接受时间";  //"接受时间";
            attr.MyDataType = BP.DA.DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.Tag = "1";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.CDT;
            if (this.IsStartNode)
                attr.Name = "发起时间"; //"发起时间";
            else
                attr.Name = "完成时间"; //"完成时间";

            attr.MyDataType = BP.DA.DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.DefVal = "@RDT";
            attr.Tag = "1";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.Rec;
            if (this.IsStartNode == false)
                attr.Name = "记录人"; // "记录人";
            else
                attr.Name = "发起人"; //"发起人";

            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MaxLen = 20;
            attr.MinLen = 0;
            attr.DefVal = "@WebUser.No";
            attr.Insert();

            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = WorkAttr.Emps;
            attr.Name = "Emps";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MaxLen = 400;
            attr.MinLen = 0;
            attr.Insert();


            attr = new BP.Sys.MapAttr();
            attr.FK_MapData = md.No;
            attr.HisEditType = BP.En.EditType.UnDel;
            attr.KeyOfEn = StartWorkAttr.FK_Dept;
            attr.Name = "操作员部门"; //"操作员部门";
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.LGType = FieldTypeS.Normal;
         //   attr.UIBindKey = "BP.Port.Depts";
            attr.UIVisible = false;
            attr.UIIsEnable = false;
            attr.MinLen = 0;
            attr.MaxLen = 20;
            attr.Insert();

            if (this.NodePosType == NodePosType.Start)
            {
                //开始节点信息.
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.Edit;
             //   attr.edit
                attr.KeyOfEn = "Title";
                attr.Name = "标题"; // "流程标题";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.UIWidth = 251;

                attr.MinLen = 0;
                attr.MaxLen = 200;
                attr.IDX = -100;
                attr.X = (float)174.83;
                attr.Y = (float)54.4;
                attr.Insert();

                if (Glo.IsEnablePRI)
                {
                    /* 如果有优先级 */
                    attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = md.No;
                    attr.HisEditType = BP.En.EditType.UnDel;
                    attr.KeyOfEn = "PRI";
                    attr.UIBindKey = attr.KeyOfEn;
                    attr.Name = "优先级";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.Enum;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = false;
                    attr.MinLen = 0;
                    attr.MaxLen = 200;
                    attr.IDX = -100;
                    attr.DefVal = "2";
                    attr.X = (float)174.76;
                    attr.Y = (float)56.19;
                    attr.Insert();
                }

                //attr = new BP.Sys.MapAttr();
                //attr.FK_MapData = md.No;
                //attr.HisEditType = BP.En.EditType.Edit;
                //attr.KeyOfEn = "FaQiRen";
                //attr.Name = "发起人"; // "发起人";
                //attr.MyDataType = BP.DA.DataType.AppString;
                //attr.UIContralType = UIContralType.TB;
                //attr.LGType = FieldTypeS.Normal;
                //attr.UIVisible = true;
                //attr.UIIsEnable = false;
                //attr.UIIsLine = false;
                //attr.MinLen = 0;
                //attr.MaxLen = 200;
                //attr.IDX = -100;
                //attr.DefVal = "@WebUser.No";
                //attr.X = (float)175.56;
                //attr.Y = (float)92.43;
                //attr.Insert();

                //attr = new BP.Sys.MapAttr();
                //attr.FK_MapData = md.No;
                //attr.HisEditType = BP.En.EditType.Edit;
                //attr.KeyOfEn = "FaQiShiJian";
                //attr.Name = "发起时间"; //"发起时间";
                //attr.MyDataType = BP.DA.DataType.AppDateTime;
                //attr.UIContralType = UIContralType.TB;
                //attr.LGType = FieldTypeS.Normal;
                //attr.UIVisible = true;
                //attr.UIIsEnable = false;
                //attr.DefVal = "@RDT";
                //attr.Tag = "1";
                //attr.X = (float)384;
                //attr.Y = (float)92.43;
                //attr.Insert();


                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "FK_NY";
                attr.Name = "年月"; //"年月";
                attr.MyDataType = BP.DA.DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.LGType = FieldTypeS.Normal;
                //attr.UIBindKey = "BP.Pub.NYs";
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 7;
                attr.Insert();

                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "个数"; // "个数";
                attr.DefVal = "1";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.Insert();
               
            }
        }
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class Nodes : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Node();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public Nodes()
        {
        }
        /// <summary>
        /// 节点集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public Nodes(string fk_flow)
        {
            //   Nodes nds = new Nodes();
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
            //this.AddEntities(NodesCash.GetNodes(fk_flow));
            return;
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// RetrieveAll
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            Nodes nds = Cash.GetObj(this.ToString(), Depositary.Application) as Nodes;
            if (nds == null)
            {
                nds = new Nodes();
                QueryObject qo = new QueryObject(nds);
                qo.AddWhereInSQL(NodeAttr.NodeID, " SELECT Node FROM WF_Direction ");
                qo.addOr();
                qo.AddWhereInSQL(NodeAttr.NodeID, " SELECT ToNode FROM WF_Direction ");
                qo.DoQuery();

                Cash.AddObj(this.ToString(), Depositary.Application, nds);
                Cash.AddObj(this.GetNewEntity.ToString(), Depositary.Application, nds);
            }

            this.Clear();
            this.AddEntities(nds);
            return this.Count;
        }
        /// <summary>
        /// 开始节点
        /// </summary>
        public void RetrieveStartNode()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeAttr.NodePosType, (int)NodePosType.Start);
            qo.addAnd();
            qo.AddWhereInSQL(NodeAttr.NodeID, "SELECT FK_Node FROM WF_NodeStation WHERE FK_STATION IN (SELECT FK_STATION FROM Port_EmpSTATION WHERE FK_Emp='" + Web.WebUser.No + "')");

            qo.addOrderBy(NodeAttr.FK_Flow);
            qo.DoQuery();
        }
        #endregion
    }
}
