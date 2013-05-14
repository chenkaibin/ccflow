
using System;
using System.Collections;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.En;

namespace BP.Rpt
{
	/// <summary>
	/// Entity 的摘要说明。
	/// 次类报表实体用于对一个特定的列计算。
	/// </summary>	
	[Serializable]
	abstract public class Rpt2DNum
	{
		#region 方法
		/// <summary>
		/// 得到纬度的实体集合，通过属性。
		/// </summary>
		/// <param name="attrOfD">纬度属性</param>
		/// <returns>EntitiesNoName</returns>
		protected EntitiesNoName GetEntitiesNoNameByAttrKey(string attrOfD)
		{
			Attr attr = this.HisEns.GetNewEntity.EnMap.GetAttrByKey(attrOfD) ;
			EntitiesNoName ens = (EntitiesNoName)attr.HisFKEns ;
			ens.RetrieveAll();
			return ens;
		}
		/// <summary>
		/// 纬度1的实体集合
		/// </summary>
		public EntitiesNoName GetDEns(string attrKey)
		{
			return GetEntitiesNoNameByAttrKey( attrKey ) ;
		}  
		#endregion

		#region 查询属性
		public bool IsShowSearchKey=false;	 
		/// <summary>
		/// 单个实体查询属性。
		/// </summary>
		private AttrsOfSearch _HisAttrsOfSearch=null;
		public AttrsOfSearch  HisAttrsOfSearch
		{
			get
			{
				if (_HisAttrsOfSearch==null)
					_HisAttrsOfSearch=new AttrsOfSearch();
				return _HisAttrsOfSearch ;
			}
		}		 
		/// <summary>
		/// 外键查询属性。
		/// </summary>
		private Attrs _HisFKSearchAttrs;
		public Attrs HisFKSearchAttrs
		{
			get
			{
				if (_HisFKSearchAttrs==null)
					_HisFKSearchAttrs=new Attrs();
				return _HisFKSearchAttrs;
			}
		}
		public void AddFKSearchAttrs(string fk)
		{
			HisFKSearchAttrs.Add(this.HisEn.EnMap.GetAttrByKey(fk),false, false); 
		}
		#endregion

		#region abstract 基本属性
		private Entity _HisEn=null;
		public Entity HisEn
		{
			get
			{
				if (this._HisEn==null)
				{
					this._HisEn = this.HisEns.GetNewEntity;

				}
				return this._HisEn ;
			}
		}
		private Entities _HisEns=null;
		public Entities HisEns
		{
			get
			{
				if (_HisEns==null)
					throw new Exception("@没有指定要操作的实体属性。");
				return _HisEns;
			}
			set
			{
				this._HisEns=value;
			}
		}		 
		/// <summary>
		/// 标题
		/// </summary>
		public  string Title="@xyer报表";
		public  string LeftTitle="项目";
		#endregion

		#region 纬度1的基本属性。
		private Attrs _AttrsOfD1=null;
		/// <summary>
		/// 第1纬度的属性。
		/// </summary>
		public  Attrs  AttrsOfD1
		{
			get
			{
				if (_AttrsOfD1==null)
				{
					_AttrsOfD1 =  new Attrs() ;
				}
				return _AttrsOfD1;
			}
		}
		/// <summary>
		/// 增加
		/// </summary>
		/// <param name="attrKey">属性</param>
		public void AddAttrOfD1ByKey(string attrKey)
		{
			AttrsOfD1.Add(this.HisEn.EnMap.GetAttrByKey(attrKey),false,false);
		}
		#endregion

		#region 纬度 数值 的属性。
		//public WorkWay WorkWay=WorkWay.Sum;
		/// <summary>
		/// 要计算的数值属性。
		/// </summary>
		private DAttrs _DAttrs=null;
		/// <summary>
		/// 第1纬度的属性。
		/// </summary>
		public  DAttrs  DAttrs
		{
			get
			{
				if (_DAttrs==null)
				{
					_DAttrs =  new DAttrs();					
				}
				return _DAttrs;
			}
		}
		/// <summary>
		/// 增加
		/// </summary>
		/// <param name="attrKey">属性</param>
		public void AddDAttr(string attrKey, WorkWay ww,bool IsCutIfIsZero)
		{
			DAttr attr = new DAttr(this.HisEn.EnMap.GetAttrByKey(attrKey), ww,null, IsCutIfIsZero);
			this.DAttrs.Add(attr);
		}
		/// <summary>
		/// 增加
		/// </summary>
		/// <param name="attrKey">属性</param>
		/// <param name="ww"></param>
		/// <param name="tag"></param>
		public void AddDAttrSelf(string attrKey, string tag, bool IsCutIfIsZero)
		{
			DAttr attr = new DAttr(this.HisEn.EnMap.GetAttrByKey(attrKey),WorkWay.Self,tag,IsCutIfIsZero);
			this.DAttrs.Add(attr);
		}
		#endregion

		#region 构造
		/// <summary>
		/// 实体
		/// </summary>
		public Rpt2DNum()
		{
		}	 
		#endregion

	}
	 
}
