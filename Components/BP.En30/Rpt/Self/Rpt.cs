using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;


namespace BP.Rpt
{
	/// <summary>
	/// 报表
	/// </summary>
	public class List :EntityNoName
	{
		public string SearchAttrs
		{
			get
			{
				return this.GetValStringByKey("SearchAttrs");
			}
			set
			{
				this.SetValByKey("SearchAttrs",value);
			}
		}

		#region 实现基本的方法
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				
				Map map = new Map("Rpt_List");
				map.EnDesc="报表";
				map.EnType= EnType.Sys;
				map.AddTBStringPK("No",null,"编号",true,false,1,100,100);
				map.AddTBString("Name",null,"名称",true,false,1,100,200);
				map.AddTBString("SearchAttrs",null,"查询条件",true,false,0,100,200);
				map.AddDDLEntities("FK_Sort",null,"类别", new BP.Rpt.RptSorts(),false);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion 

		#region 构造方法
		/// <summary>
		/// 报表
		/// </summary> 
		public List()
		{
		}
		/// <summary>
		/// 报表
		/// </summary>
		/// <param name="_No">报表编号</param> 
		public List(string ensName )
		{
			this.No=ensName;
			if (this.Retrieve("No",ensName)==0)
			{
				this.Name= DA.ClassFactory.GetEns(ensName).GetNewEntity.EnDesc;
					this.Insert();
			}
		}
		#endregion 
	}
	/// <summary>
	/// 报表
	/// </summary>
	public class Lists :EntitiesNoName
	{
		/// <summary>
		/// 报表s
		/// </summary>
		public Lists(){}
		/// <summary>
		/// 报表
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new List();
			}
		}
	}
}
