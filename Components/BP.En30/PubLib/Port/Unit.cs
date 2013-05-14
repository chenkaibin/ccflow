using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Port
{
    /// <summary>
    /// 单位
    /// </summary>
    public class UnitAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 标志
        /// </summary>
        public const string Flag = "Flag";
        /// <summary>
        /// 父节点的编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
    ///  单位
	/// </summary>
	public class Unit :EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string Flag
        {
            get
            {
                return this.GetValStringByKey(UnitAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(UnitAttr.ParentNo, value);
            }
        }
        #endregion

     
		#region 构造方法
		/// <summary>
		/// 单位
		/// </summary>
		public Unit()
        {
        }
        /// <summary>
        /// 单位
        /// </summary>
        /// <param name="_No"></param>
        public Unit(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// 单位Map
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_Unit");
                map.EnDesc = "单位";
                map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.IsAllowRepeatNo = false;

                map.AddTBStringPK(UnitAttr.No, null, "编号", true, false, 1, 50, 50);
                map.AddTBString(UnitAttr.Name, null, "名称", true, false, 1, 50, 20);
                map.AddTBString(UnitAttr.ParentNo, null, "父节点编号", true, false, 1, 50, 20);

                //map.AddTBString(UnitAttr.Flag, null, "标记", true, false, 1, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// 单位
	/// </summary>
    public class Units : EntitiesNoName
	{
		/// <summary>
		/// 单位s
		/// </summary>
        public Units() { }
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new Unit();
			}
		}
        public override int RetrieveAll()
        {
            QueryObject qo11 = new QueryObject(this);
            qo11.AddWhere(DeptAttr.No, " like ", Web.WebUser.FK_Unit + "%");
            return qo11.DoQuery();
        }
	}
}
