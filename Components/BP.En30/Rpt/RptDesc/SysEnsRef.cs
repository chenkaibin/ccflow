using System;
using System.Collections;
using BP.DA;
using BP.En;
//using BP.ZHZS.Base;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// 实体实体之间的关联
	/// </summary>
	public class EnsRefAttr 
	{
		/// <summary>
		/// 目录实体
		/// </summary>
		public const string CateEns="CateEns";
		/// <summary>
		/// 字实体
		/// </summary>
		public const string SubEns="SubEns";
		/// <summary>
		/// 关联到子实体类型
		/// </summary> 
		public const string RefSubEnKey="RefSubEnKey";	
	}
	 
	/// <summary>
	/// 实体实体之间的关联
	/// </summary>
	public class SysEnsRef:Entity 
	{
		#region 基本属性		 
		/// <summary>
		/// 目录实体
		/// </summary>
		public string CateEns
		{
			get
			{
				return this.GetValStringByKey(EnsRefAttr.CateEns ) ; 
			}
			set
			{
				this.SetValByKey(EnsRefAttr.CateEns,value) ; 
			}
		}
		/// <summary>
		/// 子实体
		/// </summary>
		public string SubEns
		{
			get
			{
				return this.GetValStringByKey(EnsRefAttr.SubEns ) ; 
			}
			set
			{
				this.SetValByKey(EnsRefAttr.SubEns,value) ; 
			}
		}
		/// <summary>
		/// 关联的key
		/// </summary>
		public string RefSubEnKey
		{
			get
			{
				return this.GetValStringByKey(EnsRefAttr.RefSubEnKey ) ; 
			}
			set
			{
				this.SetValByKey(EnsRefAttr.RefSubEnKey,value) ; 
			}
		}
		 
		 
		#endregion

		#region 构造方法
		/// <summary>
		/// 系统实体
		/// </summary>
		public SysEnsRef()
		{
		}		 
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				Map map = new Map("Sys_EnsRef");
				map.DepositaryOfEntity=Depositary.Application;
				map.DepositaryOfMap=Depositary.Application;
				map.EnType=EnType.Sys;
				map.EnDesc="实体信息";
				 
				map.EnType=EnType.Sys;
				map.AddTBStringPK(EnsRefAttr.CateEns,null,"目录实体",true,false,0,100,60);
				map.AddTBStringPK(EnsRefAttr.SubEns,null,"子实体",true,true,0,90,10);
				map.AddTBStringPK(EnsRefAttr.RefSubEnKey,null,"关联的属性",true,false,0,50,20);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion 

		#region 查询方法
		
		
		#endregion


	}
	/// <summary>
	/// 实体集合
	/// </summary>
	public class SysEnsRefs : Entities
	{		
		#region 构造
		public SysEnsRefs()
		{}
		/// <summary>
		/// 根据子实体找出
		/// </summary>
		/// <param name="subEns"></param>
		public SysEnsRefs(string subEns)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(EnsRefAttr.SubEns,subEns);
			qo.DoQuery();
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new SysEnsRef();
			}

		}
		#endregion

		#region 静态方法
		/// <summary>
		/// 取得目录实体与子实体关联的key
		/// 如果关联不上就返回null.
		/// </summary>
		/// <param name="cateEns">目录实体</param>
		/// <param name="subEns">子实体</param>
		/// <returns>关联的键</returns>
		public static string GetRefSubEnKey(string cateEns,string subEns)
		{
			//return "FK_Dept";
			SysEnsRefs ens = new SysEnsRefs();
			QueryObject qo =new QueryObject(ens);
			qo.AddWhere( EnsRefAttr.CateEns, cateEns);
			qo.addAnd();
			qo.AddWhere( EnsRefAttr.SubEns, subEns);
			
			int i = qo.DoQuery(); 
			if (i==0)
				return null;
			
			SysEnsRef en = (SysEnsRef)ens[0];
			return en.RefSubEnKey ;
			
			//return ens[0].GetValStringByKey(EnsRefAttr.RefSubEnKey);
		}
		#endregion

		#region 查询方法
		 
		#endregion
		
	}
}
