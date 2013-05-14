using System;
//
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

namespace BP.Pub
{
    
	public class DayScop:EntityNoName
	{
		#region 基本属性
        public int F
        {
            get
            {
                return this.GetValIntByKey("F");
            }
        }
        public int T
        {
            get
            {
                return this.GetValIntByKey("T");
            }
        }
		#endregion 

		#region 构造函数
		public DayScop(){}
		#endregion

		#region 重写map
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Pub_DayScop");
                map.EnDesc = "日期范围";
                map.EnType = EnType.Admin;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK("No", null, "编号", true, false, 0, 50, 20);
                map.AddTBString("Name", null, "名称", true, false, 0, 50, 20);

                map.AddTBInt("F", 0, "F", false, false);
                map.AddTBInt("T", 0, "T", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
		#endregion
	}
	public class DayScops:EntitiesNoName
	{
		#region 构造函数
        public DayScops() { }
		#endregion

		#region 方法
        public override Entity GetNewEntity
        {
            get
            {
                return new DayScop();
            }
        }
		#endregion
	}
}
