using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    ///  属性
    /// </summary>
    public class GERptAttr
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 参与人员
        /// </summary>
        public const string FlowEmps = "FlowEmps";
        /// <summary>
        /// 流程ID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// Workid
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 发起年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 发起人ID
        /// </summary>
        public const string FlowStarter = "FlowStarter";
        /// <summary>
        /// 发起日期
        /// </summary>
        public const string FlowStartRDT = "FlowStartRDT";
        /// <summary>
        /// 发起人部门ID
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 数量
        /// </summary>
        public const string MyNum = "MyNum";
        /// <summary>
        /// 结束人
        /// </summary>
        public const string FlowEnder = "FlowEnder";
        /// <summary>
        /// 最后活动日期
        /// </summary>
        public const string FlowEnderRDT = "FlowEnderRDT";
        /// <summary>
        /// 跨度
        /// </summary>
        public const string FlowDaySpan = "FlowDaySpan";
        /// <summary>
        /// 结束节点
        /// </summary>
        public const string FlowEndNode = "FlowEndNode";
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// 父流程编号
        /// </summary>
        public const string PFlowNo = "PFlowNo";
    }
    /// <summary>
    /// 报表
    /// </summary>
    public class GERpt : BP.En.EntityOID
    {
        #region attrs
        public new Int64 OID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.OID);
            }
            set
            {
                this.SetValByKey(GERptAttr.OID, value);
            }
        }
        /// <summary>
        /// 流程时间跨度
        /// </summary>
        public int FlowDaySpan
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.FlowDaySpan);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowDaySpan, value);
            }
        }
        public int MyNum
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.MyNum);
            }
            set
            {
                this.SetValByKey(GERptAttr.MyNum, value);
            }
        }
        /// <summary>
        /// 主流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.FID);
            }
            set
            {
                this.SetValByKey(GERptAttr.FID, value);
            }
        }
       
        /// <summary>
        /// 流程参与人员
        /// </summary>
        public string FlowEmps
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEmps);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEmps, value);
            }
        }
        /// <summary>
        /// 流程发起人
        /// </summary>
        public string FlowStarter
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowStarter);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowStarter, value);
            }
        }
        /// <summary>
        /// 流程发起日期
        /// </summary>
        public string FlowStartRDT
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowStartRDT);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowStartRDT, value);
            }
        }
        /// <summary>
        /// 流程结束者
        /// </summary>
        public string FlowEnder
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEnder);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEnder, value);
            }
        }
        /// <summary>
        /// 流程结束时间
        /// </summary>
        public string FlowEnderRDT
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FlowEnderRDT);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEnderRDT, value);
            }
        }
        public string FlowEndNodeText
        {
            get
            {
                Node nd =new Node(this.FlowEndNode);
                return nd.Name;
            }
        }
        public int FlowEndNode
        {
            get
            {
                return this.GetValIntByKey(GERptAttr.FlowEndNode);
            }
            set
            {
                this.SetValByKey(GERptAttr.FlowEndNode, value);
            }
        }
        /// <summary>
        /// 流程标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.Title);
            }
            set
            {
                this.SetValByKey(GERptAttr.Title, value);
            }
        }
        /// <summary>
        /// 隶属年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(GERptAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 发起人部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(GERptAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 流程状态
        /// </summary>
        public WFState WFState
        {
            get
            {
                return (WFState)this.GetValIntByKey(GERptAttr.WFState);
            }
            set
            {
                this.SetValByKey(GERptAttr.WFState, (int)value);
            }
        }
        /// <summary>
        /// 父流程WorkID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(GERptAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(GERptAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// 父流程流程编号
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(GERptAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(GERptAttr.PFlowNo, value);
            }
        }
        #endregion attrs

        #region attrs - attrs
        private string _RptName = null;
        public string RptName
        {
            get
            {
                return _RptName;
            }
            set
            {
                this._RptName = value;

                this._SQLCash = null;
                this._enMap = null;
                this.Row = null;
            }
        }
        public override string ToString()
        {
            return RptName;
        }
        public override string PK
        {
            get
            {
                return "OID";
            }
        }
        public override string PKField
        {
            get
            {
                return "OID";
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this.RptName == null)
                {
                    BP.Port.Emp emp = new BP.Port.Emp();
                    return emp.EnMap;
                }

                if (this._enMap == null)
                    this._enMap = MapData.GenerHisMap(this.RptName);

                return this._enMap;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="rptName"></param>
        public GERpt(string rptName)
        {
            this.RptName = rptName;
        }
        public GERpt()
        {
        }
        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="rptName"></param>
        /// <param name="oid"></param>
        public GERpt(string rptName, Int64 oid)
        {
          
            this.RptName = rptName;
            this.OID = (int)oid;
            this.Retrieve();
        }
        #endregion attrs
    }
    /// <summary>
    /// 报表集合
    /// </summary>
    public class GERpts : BP.En.EntitiesOID
    {
        /// <summary>
        /// 报表集合
        /// </summary>
        public GERpts()
        {
        }

        public override Entity GetNewEntity
        {
            get
            {
                return new GERpt();
            }
        }
    }
}
