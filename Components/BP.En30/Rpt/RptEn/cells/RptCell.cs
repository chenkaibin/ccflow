using System;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En; 
using BP.DA;
/*
 * Edit by peng 2004-09-24
 * RptCell：
 * 报表的基本元素，他是组成报表的基本部分。
 * 分为2纬，3纬报表。单元格。 
 * 
 * */

namespace BP.Rpt
{
	public class Rpt1DCell
	{
		
		public Rpt1DCell()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pk"></param>
		/// <param name="val"></param>
		public Rpt1DCell(string pk, Object val)
		{
			this.PK = pk ;
			this.val = val;
		}

		/// <summary>
		/// PK1
		/// </summary>
		public string PK=null;
		/// <summary>
		/// 值
		/// </summary>
		public Object val=null;

		public decimal valOfDecimal
		{
			get
			{
				try
				{
					return decimal.Round( decimal.Parse(val.ToString()), 4);
				}
				catch
				{
					return 0;
				}
			}
		}
		public float valOfFloat
		{
			get
			{
				try
				{
					return float.Parse(val.ToString());
				}
				catch
				{
					return 0;
				}
				 
			}
		}
	}
	public class Rpt1DCells:System.Collections.CollectionBase
	{
		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public Rpt1DCells()
		{
		}
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列。
		/// 他的顺序必须是与纬度的顺序一致。 
		/// </summary>
		/// <param name="dt">DataTable</param>
		public Rpt1DCells(DataTable dt)
		{
			this.BindWithDataTable(dt);						
		}
		 
		#endregion

		#region 公共方法
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列。
		/// 他的顺序必须是与纬度的顺序一致。
		/// </summary>
		/// <param name="dt">3个列的 DataTable</param>
		public void BindWithDataTable(DataTable dt)
		{
			foreach(DataRow dr in dt.Rows)			
				this.Add( new Rpt1DCell( dr[0].ToString(), dr[1] ) );
		}
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列。
		/// 他的顺序必须是与纬度的顺序一致。
		/// </summary>
		/// <param name="dt">3个列的 DataTable</param>
		public void BindWithDataTable(DataTable dt,string url)
		{
			foreach(DataRow dr in dt.Rows)			
				this.Add( new Rpt1DCell( dr[0].ToString(), dr[1] ) );
		}
		 
		/// <summary>
		/// 加入一个新的元素
		/// </summary>
		/// <param name="myen">Cell实体</param>
		public virtual void Add(Rpt1DCell myen)
		{
			//判断这个实体是不是存在
			foreach(Rpt1DCell en in this)		
			{
				if (en.PK == myen.PK)				 
					return ;
				 
			}
			// 加入这个实体。
			this.InnerList.Add(myen);
			return;
		}
		/// <summary>
		/// Rpt3DCell
		/// </summary>
		public Rpt1DCell this[int index]
		{
			get
			{	
				return (Rpt1DCell)this.InnerList[index];
			}
		}
		/// <summary>
		/// 通过3个值取出他的cell.
		/// 如果没有就New 一个返回。
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <returns>Rpt3DCell</returns>
		public Rpt1DCell GetCell(string pk1)
		{
			foreach(Rpt1DCell en in this)
				if (en.PK == pk1 )
					return en;

			return new Rpt1DCell(pk1,0);
		}
		public decimal GetRate(string pk1)
		{
			decimal sum=0;		 
			foreach(Rpt1DCell en in this)			 
				sum+= en.valOfDecimal;

			if (sum==0)
				return 0;

			decimal pkval= this.GetCell(pk1).valOfDecimal ;
			decimal rate= pkval/sum*100;
			return decimal.Round(rate,2);
		}
		#endregion		 
	}

	/// <summary>
	/// Cell 基类
	/// 单元格
	/// </summary>
	abstract public class RptCell
	{
		/// <summary>
		/// PK1
		/// </summary>
		public string PK1=null;
		/// <summary>
		/// PK2
		/// </summary>
		public string PK2=null;
		/// <summary>
		/// 值
		/// </summary>
		public Object val=null;
		public string valOfString
		{
			get
			{
				return val.ToString();
			}
		}
		public int valOfInt
		{
			get
			{
				try
				{
					return int.Parse(val.ToString()) ;
				}
				catch
				{
					return 0;
				}
			}
		}
		public decimal valOfDecimal
		{
			get
			{
				try
				{
					return decimal.Parse(val.ToString()) ;
				}
				catch
				{
					return 0;
				}
			}
		}
		public float valOfFloat
		{
			get
			{
				return float.Parse(val.ToString()) ;
			}
		}
		/// <summary>
		/// 连接的Url。
		/// </summary>
		public string Url="";
		/// <summary>
		/// URl 显示的提示。
		/// </summary>
		public string ToolTip="";
		/// <summary>
		/// 目标_top， _blank ， _parent
		/// </summary>
		public string Target="_self";
		 
	}
	/// <summary>
	///  2 维单元格
	///  实体
	/// </summary>
	public class RptPlanarCell :RptCell
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="PK1">PK1</param>
		/// <param name="PK2">PK2</param>
		/// <param name="val">Val</param>
		public RptPlanarCell(string _PK1, string _PK2  ,Object val)
		{
			this.PK1 =_PK1;
			this.PK2 =_PK2;
			this.val = val;
		}
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="PK1">PK1</param>
		/// <param name="PK2">PK2</param>
		/// <param name="val">Val</param>
		/// <param name="_Url">要连接的URL</param>
		public RptPlanarCell(string _PK1, string _PK2, Object _val, string _Url)
		{
			this.PK1 = _PK1;
			this.PK2 = _PK2;
			this.val = _val;
			this.Url = _Url;
		}	 
	}
	/// <summary>
	///  2 维单元格s
	///  实体集合
	/// </summary>
	public class RptPlanarCells : System.Collections.CollectionBase
	{
		#region 属性
		private float _SumOfFloat=-1;
		public float SumOfFloat
		{
			get
			{
				if (_SumOfFloat==-1)
				{
					_SumOfFloat = 0;
					foreach(RptPlanarCell cel in this)
						_SumOfFloat+=cel.valOfFloat;
				}
				return _SumOfFloat;
			}
		}
		#endregion

		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public RptPlanarCells()
		{
		}
		/// <summary>
		/// 根据Table构造一个集合。
		/// </summary>
		/// <param name="dt">这个DataTable要有3列，第1列PK1, 第2列PK2, 第3列Val</param>
		public RptPlanarCells(DataTable dt)
		{
			foreach(DataRow dr in dt.Rows)	
			{
				string pk1=dr[0].ToString();
				string pk2=dr[1].ToString();
				object obj=dr[2];

				this.Add( new RptPlanarCell( pk1,pk2,obj )) ;					 
			}
		}
		/// <summary>
		/// 根据Table构造一个集合。
		/// </summary>
		/// <param name="dt">这个DataTable要有3列，第1列PK1, 第2列PK2, 第3列Val</param>
		public RptPlanarCells(DataTable dt, string cellurl)
		{
			foreach(DataRow dr in dt.Rows)	
			{
				string pk1=dr[0].ToString();
				string pk2=dr[1].ToString();
				object obj=dr[2];

				if (cellurl==null)
				{
					this.Add( new RptPlanarCell( pk1,pk2,obj )) ;	
				}
				else
				{
					this.Add( new RptPlanarCell( pk1,pk2,obj, cellurl )) ;	
				}
			}
		}
		#endregion

		#region 公共方法
		/// <summary>
		/// 增加一个新的实体 with url
		/// </summary>
		/// <param name="pk1">PK1</param>
		/// <param name="pk2">PK2</param>
		/// <param name="val">两个PK决定值</param>
		/// <param name="url">连接</param>
		public void Add(string pk1,string pk2,object val,string url)
		{
			RptPlanarCell en = new RptPlanarCell(pk1,pk2,val,url) ;
			this.Add(en);
		}
		/// <summary>
		/// 增加一个新的实体
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <param name="val">两个PK决定值</param>
		public void Add(string pk1,string pk2,object val )
		{
			RptPlanarCell en = new RptPlanarCell(pk1,pk2,val);
			this.Add(en);
		}
		/// <summary>
		/// 加入一个新的元素
		/// </summary>
		/// <param name="myen">Cell实体</param>
		public virtual void Add(RptPlanarCell myen)
		{
			//判断这个实体是不是存在			 
			foreach(RptPlanarCell en in this)
			{
				if (en.PK1 == myen.PK1 && en.PK2 == myen.PK2)
				{				
					en.val =  myen.valOfFloat+myen.valOfFloat ; 					
					return;
				}
			}
			// 加入这个实体。
			this.InnerList.Add(myen);
			return;
		}
		/// <summary>
		/// 根据索引访问
		/// </summary>
		public RptPlanarCell this[int index]
		{
			get
			{	
				return (RptPlanarCell)this.InnerList[index];
			}
		}
		/// <summary>
		/// 得到一个单元格。
		/// </summary>
		/// <param name="pk1">PK1</param>
		/// <param name="pk2">PK2</param>
		/// <returns>RptPlanarCell</returns>
		public RptPlanarCell GetCell(string pk1, string pk2)
		{
			foreach(RptPlanarCell en in this)
				if (en.PK1 == pk1 && en.PK2 == pk2)
					return en;

			return new RptPlanarCell(pk1,pk2,0);
		}
		/// <summary>
		/// 得到val.
		/// </summary>
		/// <param name="pk1"></param>
		/// <param name="pk2"></param>
		/// <returns>val</returns>
		public decimal GetVal(string pk1,string pk2)
		{
			return this.GetCell(pk1,pk2).valOfDecimal ;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pk2"></param>
		/// <returns></returns>
		public decimal GetRateByPK2(string pk2)
		{
			decimal sum=this.SumOfDecimal;
			if (sum==0)
				return 0;
			else
				return this.GetSumByPK2(pk2)/sum;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pk2"></param>
		/// <returns></returns>
		public decimal GetRateByPK1(string pk2)
		{
			decimal sum=this.GetSumByPK2(pk2) ; // 一个纬度 == 2 的
			if (sum==0)
				return 0;
			else
				return this.GetSumByPK1(pk2)/ sum ;
		}
		/// <summary>
		/// 得到一个Rate。纵向
		/// </summary>
		/// <param name="pk1">PK1</param>
		/// <param name="pk2">PK2</param>
		/// <returns>RptPlanarCell</returns>
		public decimal GetRateVertical(string pk1, string pk2)
		{
			decimal sum=this.GetSumByPK1(pk1);
			if (sum==0)
				return 0;
			else
				return this.GetVal(pk1,pk2)/sum;
		}
		/// <summary>
		/// 横向
		/// </summary>
		/// <param name="pk1"></param>
		/// <param name="pk2"></param>
		/// <returns></returns>
		public decimal GetRateTransverse(string pk1, string pk2)
		{
			decimal sum=this.GetSumByPK2(pk2);
			if (sum==0)
				return 0;
			else
				return this.GetVal(pk1,pk2)/sum;
		}
		private decimal _Sum=-1;
		private int _SumOfInt=-1;

		/// <summary>
		/// 合计
		/// </summary>
		public decimal SumOfDecimal
		{
			get
			{
				if (_Sum==-1)
				{
					_Sum=0 ;
					foreach(RptPlanarCell en in this)
						_Sum+= en.valOfDecimal;
				}
				return _Sum;
			}
		} 
		public int SumOfInt
		{
			get
			{
				if (_SumOfInt==-1)
				{
					_SumOfInt=0 ;
					foreach(RptPlanarCell en in this)
						_SumOfInt+= en.valOfInt;
				}
				return _SumOfInt;
			}
		} 
		/// <summary>
		/// 得到Sum
		/// </summary>
		/// <param name="pk1">pk</param>
		/// <returns>sum</returns>
		public decimal GetSumByPK1(string pk1)
		{
			decimal dec=0 ;
			foreach(RptPlanarCell en in this)
				if (en.PK1 == pk1)
					dec+= en.valOfDecimal;
			return dec;
		}
		/// <summary>
		/// 得到Sum
		/// </summary>
		/// <param name="pk1">pk</param>
		/// <returns>sum</returns>
		public decimal GetSumByPK2(string pk2)
		{
			decimal dec=0 ;
			foreach(RptPlanarCell en in this)
				if (en.PK2 == pk2)
					dec+= en.valOfDecimal;
			return dec;
		}
		/// <summary>
		/// 得到一个单元格内的字符串
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <returns>html 标记</returns>
		public string GenerHtmlStrBy(string pk1, string pk2 )
		{
			RptPlanarCell en = this.GetCell(pk1,pk2) ;
			if (en.Url=="")
			{
				return en.val.ToString();
			}
			string url=en.Url+"&PK1="+pk1+"&PK2="+pk2;
			return "<A href='javascript:openit('"+url+"')'  >"+float.Parse( en.val.ToString() ).ToString("0")+"</A>";

			//return "<A href=\""+en.Url+"&PK1="+pk1+"&PK2="+pk2+"\" target='_blank'>"+en.val+"</A>";
		}
		/// <summary>
		/// 得到一个单元格内的字符串
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <returns>html 标记</returns>
		public string GenerHtmlStrBy(string d1EnsName, string pk1, string d2EnsName, string pk2 , AnalyseDataType adt)
		{
			RptPlanarCell en = this.GetCell(pk1,pk2) ;
			if (en.Url=="")
			{
				return en.val.ToString();
			}

			try
			{
				switch(adt)
				{
					case AnalyseDataType.AppFloat:
						return "<A href=\"javascript:openit('"+en.Url+"&"+d1EnsName+"="+pk1+"&"+d2EnsName+"="+pk2+"')\" >"+float.Parse(en.val.ToString())+"</A>";
					case AnalyseDataType.AppInt:
						return "<A href=\"javascript:openit('"+en.Url+"&"+d1EnsName+"="+pk1+"&"+d2EnsName+"="+pk2+"')\" >"+int.Parse(en.val.ToString())+"</A>";
					case AnalyseDataType.AppMoney:
						return "<A href=\"javascript:openit('"+en.Url+"&"+d1EnsName+"="+pk1+"&"+d2EnsName+"="+pk2+"')\" >"+float.Parse(en.val.ToString()).ToString("0.00")+"</A>";
					default:
						throw new Exception("adt 类型错误.");
				}
			}
			catch 
			{
				return "0";
			}
		}
		#endregion
	}
	
	

	
}
