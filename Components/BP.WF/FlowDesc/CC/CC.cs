using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF
{
    /// <summary>
    /// 抄送控制方式
    /// </summary>
    public enum CtrlWay
    {
        /// <summary>
        /// 按照岗位
        /// </summary>
        ByStation,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDept,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmp,
        /// <summary>
        /// 按SQL
        /// </summary>
        BySQL
    }
	/// <summary>
	/// 抄送属性
	/// </summary>
    public class CCAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string CCTitle = "CCTitle";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string CCDoc = "CCDoc";
        /// <summary>
        /// 抄送控制方式
        /// </summary>
        public const string CCCtrlWay = "CCCtrlWay";
        /// <summary>
        /// 抄送对象
        /// </summary>
        public const string CCSQL = "CCSQL";
        #endregion
    }
	/// <summary>
	/// 抄送
	/// </summary>
    public class CC : Entity
    {
        #region 属性
        /// <summary>
        /// 抄送
        /// </summary>
        /// <param name="rpt"></param>
        /// <returns></returns>
        public DataTable GenerCCers(Entity rpt)
        {
            string sql = "";
            switch (this.HisCtrlWay)
            {
                case CtrlWay.BySQL:
                    sql = this.CCSQL.Clone() as string;
                    sql = sql.Replace("@WebUser.No", Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", Web.WebUser.FK_Dept);
                    if (sql.Contains("@"))
                    {
                        foreach (Attr attr in rpt.EnMap.Attrs)
                        {
                            if (sql.Contains("@") == false)
                                break;
                            sql = sql.Replace("@" + attr.Key, rpt.GetValStrByKey(attr.Key));
                        }
                    }
                    break;
                case CtrlWay.ByEmp:
                    sql = "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_CCEmp WHERE FK_Node=" + this.NodeID + ")";
                    break;
                case CtrlWay.ByDept:
                    sql = "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept IN ( SELECT FK_Dept FROM WF_CCDept WHERE FK_Node=" + this.NodeID + "))";
                    break;
                case CtrlWay.ByStation:
                    sql = "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM Port_EmpStation WHERE FK_Station IN ( SELECT FK_Station FROM WF_CCStation WHERE FK_Node=" + this.NodeID + "))";
                    break;
                default:
                    throw new Exception("未处理的异常");
            }
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        ///节点ID
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
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 抄送标题
        /// </summary>
        public string CCTitle
        {
            get
            {
                string s= this.GetValStringByKey(CCAttr.CCTitle);
                if (string.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCTitle, value);
            }
        }
        /// <summary>
        /// 抄送内容
        /// </summary>
        public string CCDoc
        {
            get
            {
                string s = this.GetValStringByKey(CCAttr.CCDoc);
                if (string.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCDoc, value);
            }
        }
        /// <summary>
        /// 抄送对象
        /// </summary>
        public string CCSQL
        {
            get
            {
                string sql= this.GetValStringByKey(CCAttr.CCSQL);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("‘", "'");
                sql = sql.Replace("’", "'");
                sql = sql.Replace("''", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(CCAttr.CCSQL, value);
            }
        }
        /// <summary>
        /// 控制方式
        /// </summary>
        public CtrlWay HisCtrlWay
        {
            get
            {
                return (CtrlWay)this.GetValIntByKey(CCAttr.CCCtrlWay);
            }
            set
            {
                this.SetValByKey(CCAttr.CCCtrlWay, (int)value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// CC
        /// </summary>
        public CC()
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
                Map map = new Map("WF_Node");
                map.EnDesc = "抄送规则";
                map.EnType = EnType.Admin;
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10, true);
                map.AddTBIntPK(NodeAttr.NodeID, 0,"节点ID", true, true);

                map.AddDDLSysEnum(CCAttr.CCCtrlWay, 0, "控制方式",true, true,"CtrlWay");
                map.AddTBString(CCAttr.CCSQL, null, "SQL表达式", true, false, 0, 500, 10, true);
                map.AddTBString(CCAttr.CCTitle, null, "抄送标题", true, false, 0, 500, 10,true);
                map.AddTBStringDoc(CCAttr.CCDoc, null, "抄送内容(标题与内容支持变量)", true, false,true);

                map.AddSearchAttr(CCAttr.CCCtrlWay);

                // 相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.CCStations(), new BP.WF.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "抄送岗位");

                map.AttrsOfOneVSM.Add(new BP.WF.CCDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                DeptAttr.No,  "抄送部门" );

                map.AttrsOfOneVSM.Add(new BP.WF.CCEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "抄送人员");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 抄送s
	/// </summary>
	public class CCs: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CC();
			}
		}
		/// <summary>
        /// 抄送
		/// </summary>
		public CCs(){} 		 
		#endregion
	}
}
