
using System;
using System.Collections;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.En;

namespace BP.Rpt
{
	/// <summary>
	/// 分析数据类型
	/// </summary>
	public enum AnalyseDataType
	{
		/// <summary>
		/// 整形
		/// </summary>
		AppInt,
		/// <summary>
		/// 货币
		/// </summary>
		AppMoney,
		/// <summary>
		/// 浮点
		/// </summary>
		AppFloat
	}
	/// <summary>
	/// 分析目标
	/// </summary>
	public class AnalyseObj
	{
		/// <summary>
		/// 分析目标
		/// </summary>
		/// <param name="dp">描述</param>
		/// <param name="oc">操作列</param>
		/// <param name="adt">分析的数据类型</param>
		public AnalyseObj(string dp, string oc, AnalyseDataType adt)
		{
			this.DataProperty=dp;
			this.OperationColumn=oc;
			this.HisADT = adt;
		}
		/// <summary>
		/// 属性
		/// </summary>
		public string DataProperty="个数";
		/// <summary>
		/// 操作
		/// </summary>
		public string OperationColumn="COUNT(*)";
		/// <summary>
		/// 分析的数据类型
		/// </summary>
		public AnalyseDataType HisADT=AnalyseDataType.AppInt;
	}
	/// <summary>
	/// 分析目标s
	/// </summary>
	public class AnalyseObjs:CollectionBase
	{
		/// <summary>
		/// 分析目标s
		/// </summary>
		public AnalyseObjs(){}
		/// <summary>
		/// 分析目标
		/// </summary>
		public AnalyseObj this[int index]
		{
			get
			{
				return (AnalyseObj)this.InnerList[index];
			}
		}
		/// <summary>
		/// 增加一个分析对象
		/// </summary>
		/// <param name="DataProperty">数据属性</param>
		/// <param name="OperationColumn">操作列</param>
		/// <param name="adp">数据类型</param>
		/// <returns>返回增加的位置</returns>
		public virtual int AddAnalyseObj(string  DataProperty, string OperationColumn, AnalyseDataType adp)
		{
			AnalyseObj ao = new AnalyseObj(DataProperty,OperationColumn,adp);
			return this.InnerList.Add(ao);
		}
		/// <summary>
		/// 得到一个分析对象，根据要操作的列
		/// </summary>
		/// <param name="oc">要操作的列</param>
		/// <returns>分析对象</returns>
		public AnalyseObj GetAnalyseObjByOperationColumn(string oc)
		{

			foreach(AnalyseObj ao in this)
			{
				if (ao.OperationColumn==oc)
					return ao;
			}
			throw new Exception("没有找到OperationColumn="+oc+"的分析对象。");
		}
	}
	/// <summary>
	/// Entity 的摘要说明。
	/// </summary>	
	[Serializable]
	abstract public class Rpt3D
	{
		#region 方法
		/// <summary>
		/// 得到纬度的实体集合，通过属性。
		/// </summary>
		/// <param name="attrOfD">纬度属性</param>
		/// <returns>EntitiesNoName</returns>
		protected Entities GetEntitiesByAttrKey(string attrOfD)
		{
			Attr attr = this.HisEns.GetNewEntity.EnMap.GetAttrByKey(attrOfD);
			if (attr.MyFieldType==FieldType.PKEnum || attr.MyFieldType==FieldType.Enum)
			{
				/*如果是Enum 类型．*/
				SysEnums sysEnums = new SysEnums(attr.UIBindKey);
				return sysEnums;
			}
			else 
			{
                Entities ens = attr.HisFKEns; // ClassFactory.GetEns(attr.UIBindKey);
				ens.RetrieveAll();
				return ens;
				//	return ens.ToEntitiesNoName(attr.UIRefKeyValue,attr.UIRefKeyText);
			}

		}
		protected EntitiesNoName GetEntitiesNoNameByAttrKey_del(string attrOfD)
		{
			Attr attr = this.HisEns.GetNewEntity.EnMap.GetAttrByKey(attrOfD);

			
			if (attr.MyFieldType==FieldType.PKEnum || attr.MyFieldType==FieldType.Enum)
			{
				/*如果是Enum 类型．*/
				SysEnums sysEnums = new SysEnums(attr.UIBindKey);
				return sysEnums.ToEntitiesNoName();
			}
			else 
			{
                Entities ens = attr.HisFKEns; 
				ens.RetrieveAll();

				return ens.ToEntitiesNoName(attr.UIRefKeyValue,attr.UIRefKeyText);
			}

		}
		/// <summary>
		/// 纬度实体集合
		/// </summary>
		public Entities GetDEns(string attrKey)
		{
			return GetEntitiesByAttrKey(attrKey);
			/*
			if (attrKey=="BP.Port.Depts")
			{
				BP.Port.Depts Depts=(BP.Port.Depts)ens;
				Depts.RetrieveAllNoXJ();
				return Depts;
			}
			return ens;
			*/

		}
		/// <summary>
		/// 纬度2的实体集合
		/// </summary>
		public Entities DEns2
		{
			get
			{
				//return null;
				return GetEntitiesByAttrKey(this.AttrOfD2) ;
			}
		}
		/// <summary>
		/// 纬度3的实体集合
		/// </summary>
		public Entities DEns3
		{
			get
			{
				//return null;

				return GetEntitiesByAttrKey(this.AttrOfD3) ;
			}
		}
		#endregion

		#region 查询属性
		/// <summary>
		/// 单个属性的查询属性集合
		/// </summary>
		private AttrsOfSearch _HisAttrsOfSearch=null;
		/// <summary>
		/// 单个属性的查询属性集合。
		/// </summary>
		public AttrsOfSearch HisAttrsOfSearch
		{
			get
			{
				if (_HisAttrsOfSearch==null)
				{
					_HisAttrsOfSearch = new AttrsOfSearch();
				}
				return  _HisAttrsOfSearch;
			}
		}
		/// <summary>
		/// 外键查询属性
		/// </summary>
		private Attrs _HisFKSearchAttrs=null;
		/// <summary>
		/// 外键查询属性。
		/// </summary>
		public Attrs HisFKSearchAttrs
		{
			get
			{
				if (_HisFKSearchAttrs==null)
				{
					_HisFKSearchAttrs = new Attrs();
				}
				return  _HisFKSearchAttrs;
			}
			set
			{
				_HisFKSearchAttrs=value;
			}
		}
		/// <summary>
		/// 增加外键查询属性
		/// </summary>
		/// <param name="key">查询属性key</param>
		public void AddFKSearchAttrs(string key)
		{
			this.HisFKSearchAttrs.Add(this.HisEn.EnMap.GetAttrByKey(key),false,false);
		}
		#endregion

		#region 基本属性
		/// <summary>
		/// 数据类型.
		/// </summary>
		public int DataType=BP.DA.DataType.AppInt;
		
		public bool _IsShowSum=false;
		/// <summary>
		/// 是否可以显示合计(default true)
		/// </summary>
		public bool IsShowSum
		{
			get
			{
				if ( IsShowRate)
					return true;
				else
					return _IsShowSum;
			}
			set
			{
				_IsShowSum =value;
			}
		}
		/// <summary>
		/// 显示比率(default true)
		/// </summary>
		public bool IsShowRate=true;

		/// <summary>
		/// Entity
		/// </summary>
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
		/// <summary>
		/// 操作对象
		/// </summary>
		private Entities _HisEns=null;
		/// <summary>
		/// 操作对象
		/// </summary>
		public Entities HisEns
		{
			get
			{
				return _HisEns;
			}
			set
			{
				_HisEns=value;
			}
		}
		/// <summary>
		/// 1纬度属性
		/// </summary>
		public string AttrOfD1=null;
		 
		/// <summary>
		/// 2纬度属性
		/// </summary>
		public  string AttrOfD2=null;
		 
		/// <summary>
		/// 3纬度属性
		/// </summary>
		public string AttrOfD3 =null;
		
		/// <summary>
		/// 2纬度3纬度关联属性。
		/// </summary>
		private string _D2D3RefKey="";
		/// <summary>
		/// 3纬度属性
		/// </summary>
		public string D2D3RefKey
		{
			get
			{
				if (_D2D3RefKey=="")
				{
					Attr d2=this.HisEn.EnMap.GetAttrByKey(this.AttrOfD2) ;
					Attr d3=this.HisEn.EnMap.GetAttrByKey(this.AttrOfD3) ;

					_D2D3RefKey=BP.Sys.SysEnsRefs.GetRefSubEnKey(d2.UIBindKey,d3.UIBindKey) ;


					
				}
				return _D2D3RefKey;
			}
		}
		/// <summary>
		/// 标题
		/// </summary>
		public string Title="3纬报表";
		/// <summary>
		/// 是否能显示关键字查询。
		/// </summary>
		public bool IsShowSearchKey=false;		 
		#endregion

		#region 纬度1的基本属性。
		private Attrs _DAttrs=null;
		/// <summary>
		/// 纬度的属性，这些纬度可以供用户选择。
		/// </summary>
		public  Attrs  DAttrs
		{
			get
			{
				if (_DAttrs==null)				 
					_DAttrs =  new Attrs() ;
				return _DAttrs;
			}
		}
		/// <summary>
		/// 增加一个纬度属性。
		/// </summary>
		/// <param name="attrKey">属性</param>
		public void AddDAttrByKey(string attrKey)
		{
			DAttrs.Add(this.HisEn.EnMap.GetAttrByKey(attrKey),false,false);
		}
		#endregion

		#region 分析目标属性
		/// <summary>
		/// 分析目标s
		/// </summary>
		protected AnalyseObjs _HisAnalyseObjs=null;
		/// <summary>
		/// 分析目标s
		/// </summary>
		public AnalyseObjs HisAnalyseObjs
		{
			get
			{
				if (this._HisAnalyseObjs==null)
				{
					_HisAnalyseObjs= new AnalyseObjs();
					_HisAnalyseObjs.AddAnalyseObj("个数","COUNT(*)", BP.Rpt.AnalyseDataType.AppInt );
				}
				return _HisAnalyseObjs;
			}
		}
		

		
		/// <summary>
		/// 数据性质。
		/// </summary>
		public string DataProperty="个数";		
		/// <summary>
		/// 计算列
		/// （默认 COUNT(*) ）.
		/// AVG(filed).
		/// </summary>
		public string OperationColumn="COUNT(*)";
		#endregion

		#region 构造
		/// <summary>
		/// 实体
		/// </summary>
		public Rpt3D()
		{
			 
		}	 
		#endregion

	}
	/// <summary>
	/// EnObj 的摘要说明。
	/// </summary>
	abstract public class Rpt3Ds :CollectionBase
	{
		#region 构造
		/// <summary>
		/// 构造方法
		/// </summary>
		public Rpt3Ds()
		{
			 
		}
//		/// <summary>
//		/// Rpt3Ds
//		/// </summary>
//		public Rpt3Ds this[int index]
//		{
//			return null ;
//			 
//
//		}
		#endregion
	}
}
