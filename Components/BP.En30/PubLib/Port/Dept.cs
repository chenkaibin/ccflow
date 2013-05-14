using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.Port
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 父节点的编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 单位编号
        /// </summary>
        public const string FK_Unit = "FK_Unit";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 单位编号
        /// </summary>
        public string FK_Unit
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.FK_Unit);
            }
            set
            {
                this.SetValByKey(DeptAttr.FK_Unit, value);
            }
        }
        public int Grade
        {
            get
            {
                return 1;
            }
        }
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.EnType = EnType.Admin;

                map.EnDesc = "部门"; //  实体的描述.
                map.DepositaryOfEntity = Depositary.Application; //实体map的存放位置.
                map.DepositaryOfMap = Depositary.Application;    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, false, 1, 50, 20);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点No", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.FK_Unit, null, "所在单位", true, false, 0, 50, 30);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    ///得到集合
    /// </summary>
    public class Depts : EntitiesNoName
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {

        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }
    }
}
