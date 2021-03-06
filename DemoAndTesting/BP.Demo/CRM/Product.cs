using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// 产品
	/// </summary>
	public class ProductAttr: EntityNoNameAttr
	{
		#region 基本属性
		public const  string FK_SF="FK_SF";
        public const string Addr = "Addr";

		#endregion
	}
	/// <summary>
    /// 产品
	/// </summary>
    public class Product : EntityNoName
    {
        #region 基本属性

        #endregion

        #region 构造函数
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
        /// 产品
        /// </summary>		
        public Product() { }
        /// <summary>
        /// 产品
        /// </summary>
        /// <param name="no"></param>
        public Product(string no)
            : base(no)
        {

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

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.PhysicsTable = "Demo_Product";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsAllowRepeatNo = false;
                map.IsCheckNoLength = false;
                map.EnDesc = "产品";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(ProductAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(ProductAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(ProductAttr.Addr, null, "生产地址", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new Products(); }
        }
        #endregion
    }
	/// <summary>
	/// 产品
	/// </summary>
	public class Products : EntitiesNoName
	{
		#region 
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Product();
			}
		}	
		#endregion 

		#region 构造方法
		/// <summary>
		/// 产品s
		/// </summary>
		public Products(){}
		#endregion
	}
	
}
