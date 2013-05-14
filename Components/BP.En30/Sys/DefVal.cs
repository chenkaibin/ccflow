using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
	 /// <summary>
	 /// 属性
	 /// </summary>
	public class DefValAttr
	{
		/// <summary>
		/// 属性Key
		/// </summary>
		public const string AttrKey="AttrKey";
        /// <summary>
        /// 描述
        /// </summary>
        public const string AttrDesc = "AttrDesc";
		/// <summary>
		/// 工作人员ID
		/// </summary>
		public const string FK_Emp="FK_Emp";
		/// <summary>
		/// 默认值
		/// </summary>
		public const string Val="Val";
		/// <summary>
		/// EnsName
		/// </summary>
		public const string EnsName="EnsName";
        /// <summary>
        /// 描述
        /// </summary>
        public const string EnsDesc = "EnsDesc";
	}
	/// <summary>
	/// 默认值
	/// </summary>
	public class DefVal: EntityOID
	{
		#region 基本属性
        /// <summary>
        /// 类名
        /// </summary>
		public string EnsName
		{
			get
			{
				return this.GetValStringByKey(DefValAttr.EnsName ) ; 
			}
			set
			{
				this.SetValByKey(DefValAttr.EnsName,value) ; 
			}
		}
        /// <summary>
        /// 描述
        /// </summary>
        public string EnsDesc
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.EnsDesc);
            }
            set
            {
                this.SetValByKey(DefValAttr.EnsDesc, value);
            }
        }
		/// <summary>
		/// 默认值
		/// </summary>
		public string Val
		{
			get
			{
				return this.GetValStringByKey(DefValAttr.Val ) ; 
			}
			set
			{
				this.SetValByKey(DefValAttr.Val,value) ; 
			}
		}
		/// <summary>
		/// 操作员ID
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(DefValAttr.FK_Emp ) ; 
			}
			set
			{
				this.SetValByKey(DefValAttr.FK_Emp,value) ; 
			}
		}
		/// <summary>
		/// 属性
		/// </summary>
		public string AttrKey
		{
			get
			{
				return this.GetValStringByKey(DefValAttr.AttrKey ) ; 
			}
			set
			{
				this.SetValByKey(DefValAttr.AttrKey,value) ; 
			}
		}
        /// <summary>
        /// 属性描述
        /// </summary>
        public string AttrDesc
        {
            get
            {
                return this.GetValStringByKey(DefValAttr.AttrDesc);
            }
            set
            {
                this.SetValByKey(DefValAttr.AttrDesc, value);
            }
        }
		#endregion

		#region 构造方法
       
		/// <summary>
		/// 默认值
		/// </summary>
		public DefVal()
		{
		}
		/// <summary>
		/// map
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null) return this._enMap;
                Map map = new Map("Sys_DefVal");
                map.EnType = EnType.Sys;
                map.EnDesc = "默认值";
                map.DepositaryOfEntity = Depositary.None;

                map.AddTBIntPKOID();
                map.AddTBString(DefValAttr.EnsName, null, "类名称", false, true, 0, 100, 10);
                map.AddTBString(DefValAttr.EnsDesc, null, "类描述", false, true, 0, 100, 10);

                map.AddTBString(DefValAttr.AttrKey, null, "属性", false, true, 0, 100, 10);
                map.AddTBString(DefValAttr.AttrDesc, null, "属性描述", false, true, 0, 100, 10);

                map.AddTBString(DefValAttr.FK_Emp, null, "人员", false, true, 0, 100, 10);
                map.AddTBString(DefValAttr.Val, null, "值", true, false, 0, 1000, 10);
                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion 
	}
	/// <summary>
	/// 默认值s
	/// </summary>
	public class DefVals : EntitiesOID
	{
		/// <summary>
		/// 查询.
		/// </summary>
		/// <param name="EnsName"></param>
		/// <param name="key"></param>
		/// <param name="FK_Emp"></param>
        public void Retrieve(string EnsName, string key, int FK_Emp)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DefValAttr.AttrKey, key);
            qo.addAnd();
            qo.AddWhere(DefValAttr.EnsName, EnsName);
            qo.addAnd();
            qo.AddWhere(DefValAttr.FK_Emp, FK_Emp);
            qo.DoQuery();
        }
		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="EnsName"></param>
		/// <param name="key"></param>
        public void Retrieve(string EnsName, string key)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(DefValAttr.AttrKey, key);
            qo.addAnd();
            qo.AddWhere(DefValAttr.EnsName, EnsName);
            qo.DoQuery();
        }
		/// <summary>
		/// 默认值s
		/// </summary>
		public DefVals()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DefVal();
            }
        }
	}
}
