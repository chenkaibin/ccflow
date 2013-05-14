using System;
using System.Collections; 
using BP.En;
//using BP.ZHZS.Base;

namespace BP.DA
{
	/// <summary>
	/// DSTaxpayer 的摘要说明。
	/// </summary>
	public class DBSimpleNoName :EntityNoName
	{ 

		#region 构造方法
		public DBSimpleNoName(){}
		/// <summary>
		/// 税务编号
		/// </summary>
		/// <param name="_No"></param>
		public DBSimpleNoName(string _No ): base(_No){}
		#endregion 

		public override Map EnMap
		{
            get
            {
                if (this._enMap != null) return this._enMap;
                Map map = new Map("Sys_DBSimpleNoName");
                map.EnDesc = "Sys_DBSimpleNoName";
                map.CodeStruct = "3";
                map.IsAllowRepeatNo = false;
                map.AddTBStringPK(SimpleNoNameAttr.No, null, "编号", true, true, 2, 8, 4);
                map.AddTBString(SimpleNoNameAttr.Name, null, "名称", true, false, 2, 50, 50);
                this._enMap = map;
                return this._enMap;
            }
		}
        public override Entities GetNewEntities
        {
            get { return new DBSimpleNoNames(); }
        }
	}
	/// <summary>
	/// 纳税人集合
	/// </summary>
	public class DBSimpleNoNames :SimpleNoNames
	{
		public DBSimpleNoNames(){}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new DBSimpleNoName();
			}
			 
		}
		public void AddByNoName(string no , string name)
		{
			DBSimpleNoName en = new DBSimpleNoName();
			en.No = no;
			en.Name = name;
			
			this.AddEntity(en);
		}
	}
}
