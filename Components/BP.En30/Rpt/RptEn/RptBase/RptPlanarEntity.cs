using System;
//using System.Drawing;
//// using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En ; 
using BP.DA;
using BP.Web ; 


namespace BP.Rpt
{
	/// <summary>
	/// 2交叉报表实体
	/// </summary>
	public class RptPlanarEntity : RptEntity
	{
		#region 属性。
		
		/// <summary>
		/// 个数
		/// </summary>
		public string DataProperty="个数";		
		/// <summary>
		/// 是否要显示Item的Url。
		/// </summary>
		private int _IsShowItem1Url=-1;
		/// <summary>
		/// 是不是需要显示Item1 的URL.
		/// </summary>
		public bool IsShowItem1Url
		{
			get
			{
				if (this._IsShowItem1Url==-1)
				{
					RefLinks ens = new RefLinks(this.SingleDimensionItem1.GetNewEntity.ToString()) ;
					if (ens.Count==0)
						_IsShowItem1Url=0;
					else
						_IsShowItem1Url=1;
				}

				if (_IsShowItem1Url==0)
					return false;
				else
					return true;		 
				
			}
		}
		/// <summary>
		/// 是否要显示Item2的Url。
		/// </summary>
		private int _IsShowItem2Url=-1;
		/// <summary>
		/// 是不是需要显示Item1 的URL.
		/// </summary>
		public bool IsShowItem2Url
		{
			get
			{
				if (this._IsShowItem2Url==-1)
				{
					RefLinks ens = new RefLinks(this.SingleDimensionItem2.GetNewEntity.ToString()) ;
					if (ens.Count==0)
						_IsShowItem2Url=0;
					else
						_IsShowItem2Url=1;
				}
				if (_IsShowItem2Url==0)
					return false;
				else
					return true;				
			}
		}
		#endregion 

		#region 构造
		/// <summary>
		/// 交叉报表实体
		/// </summary>
		public RptPlanarEntity()
		{
		}
		/// <summary>
		/// 交叉报表实体
		/// </summary>
		/// <param name="d1">一个集合</param>
		/// <param name="d2">另外一个集合</param>
		/// <param name="cells">单元格</param>
		public RptPlanarEntity(Entities d1,Entities d2, RptPlanarCells cells)
		{
			this.SingleDimensionItem1 = d1;
			this.SingleDimensionItem2 = d2;
			this.PlanarCells = cells;
		}
		/// <summary>
		/// 交叉报表实体
		/// </summary>
		/// <param name="d1">纬度1集合</param>
		/// <param name="d2">纬度2集合</param>
		/// <param name="dt">数据表</param>
		/// <param name="cellUrl">单元格连接</param>
		/// <param name="adt">数据类型</param>
		public RptPlanarEntity(Entities d1,Entities d2, 
			DataTable dt, string cellUrl, AnalyseDataType adt)
		{
			this.SingleDimensionItem1 = d1 ;
			this.SingleDimensionItem2 = d2 ;
			this.PlanarCells =  new RptPlanarCells(dt, cellUrl);
			this.HisADT = adt ;
		}
		public RptPlanarEntity(Entities d1,Entities d2, 
			DataTable dt)
		{
			this.SingleDimensionItem1 = d1 ;
			this.SingleDimensionItem2 = d2 ;
			this.PlanarCells =  new RptPlanarCells(dt);
		}
		#endregion
		 
		#region  属性 两个元素
		/// <summary>
		/// 一维实体1
		/// </summary>
		public Entities SingleDimensionItem1=null;
		/// <summary>
		/// 一维实体2
		/// </summary>
		public Entities SingleDimensionItem2=null;
		/// <summary>
		/// 单元s
		/// </summary>
		public RptPlanarCells PlanarCells=null;
		#endregion

		#region 方法
		/// <summary>
		/// 按照纬度删除不能对应上的纬度
		/// </summary>
		public void CutNotRefD1()
		{
			EntitiesNoName ens =(EntitiesNoName)this.SingleDimensionItem1.CreateInstance();
			foreach(EntityNoName en in this.SingleDimensionItem1)
			{
				foreach(RptPlanarCell cell in this.PlanarCells)
				{
					if (cell.PK1==en.No)
					{
						ens.AddEntity(en);
						break;
					}
				}
			}
			this.SingleDimensionItem1 =ens ; 
		}
		/// <summary>
		/// 按照纬度删除不能对应上的纬度
		/// </summary>
		public void CutNotRefD2()
		{
			EntitiesNoName ens =(EntitiesNoName)this.SingleDimensionItem2.CreateInstance();
			foreach(EntityNoName en in this.SingleDimensionItem2)
			{
				foreach(RptPlanarCell cell in this.PlanarCells)
				{
					if (cell.PK2==en.No)
					{
						ens.AddEntity(en);
						break;
					}
				}
			}
			this.SingleDimensionItem2 =ens ; 
		}
		/// <summary>
		/// 用到HTML的显示。把Val 根据URL的设置转换为Link.
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <returns>string</returns>
		public string GetCellContext(string pk1, string pk2)
		{
			RptPlanarCell cell = this.PlanarCells.GetCell(pk1,pk2);
			if (cell.Url=="")
				return cell.val.ToString();
			return "<a href='"+cell.Url+"?"+this.SingleDimensionItem1.GetNewEntity.ToString()+"="+cell.PK1+"&"+this.SingleDimensionItem2.GetNewEntity.ToString()+"="+cell.PK2+"' Target='"+CellUrlTarget+"' >"+cell.val+"</a>";
		}

		public string GetItem1Context(string pk1No,string pk1Name)
		{
			if (this.IsShowItem1Url)
				return "<a href='"+System.Web.HttpContext.Current.Request.ApplicationPath+"/Rpt/RptRefLink.aspx?EnsName="+this.SingleDimensionItem1.GetNewEntity.ToString()+"&val="+pk1No+"' > "+pk1Name+ " </a>";
			else
				return pk1Name;			 
		}
		public string GetItem2Context(string pk2No,string pk2Name)
		{
			if (this.IsShowItem2Url)
				return "<a href='"+System.Web.HttpContext.Current.Request.ApplicationPath+"/Rpt/RptRefLink.aspx?EnsName="+this.SingleDimensionItem2.GetNewEntity.ToString()+"&val="+pk2No+"' > "+pk2Name+ " </a>";
			else
				return pk2Name;
		}
		#endregion
	}
}
