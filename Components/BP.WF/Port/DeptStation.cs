using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.WF.Port
{
	/// <summary>
	/// 部门岗位
	/// </summary>
	public class DeptStationAttr  
	{
		#region 基本属性
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const  string FK_Dept="FK_Dept";
		/// <summary>
		/// 工作岗位
		/// </summary>
		public const  string FK_Station="FK_Station";		 
		#endregion	
	}
	/// <summary>
    /// 部门岗位 的摘要说明。
	/// </summary>
    public class DeptStation : Entity
    {
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

        #region 基本属性
        /// <summary>
        /// 工作人员ID
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptStationAttr.FK_Dept);
            }
            set
            {
                SetValByKey(DeptStationAttr.FK_Dept, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(DeptStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///工作岗位
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(DeptStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(DeptStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region 扩展属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 工作部门岗位
        /// </summary> 
        public DeptStation() { }
        /// <summary>
        /// 工作人员工作岗位对应
        /// </summary>
        /// <param name="_empoid">工作人员ID</param>
        /// <param name="wsNo">工作岗位编号</param> 	
        public DeptStation(string _empoid, string wsNo)
        {
            this.FK_Dept = _empoid;
            this.FK_Station = wsNo;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("Port_DeptStation");
                map.EnDesc = "部门岗位";
                map.EnType = EnType.Dot2Dot;   
                map.AddDDLEntitiesPK(DeptStationAttr.FK_Dept, null, "部门", new Depts(), true);
                map.AddDDLEntitiesPK(DeptStationAttr.FK_Station, null, "岗位", new Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 部门岗位 
	/// </summary>
    public class DeptStations : Entities
    {
        #region 构造
        /// <summary>
        /// 工作部门岗位
        /// </summary>
        public DeptStations()
        {
        }
        /// <summary>
        /// 工作人员与工作岗位集合
        /// </summary>
        public DeptStations(string stationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DeptStationAttr.FK_Station, stationNo);
            qo.DoQuery();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptStation();
            }
        }
        #endregion
    }
}
