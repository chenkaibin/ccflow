using System;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En; 
using BP.DA;
namespace BP.Rpt
{
	/// <summary>
	/// 三维实体
	/// </summary>
	public class Rpt3DCell : RptCell
	{
		#region 属性
		/// <summary>
		/// 3 纬属性
		/// </summary>
		public string PK3=null;
		#endregion 

		#region 构造方法
		/// <summary>
		/// 构造一个3维实体
		/// </summary>
		/// <param name="PK1">pk1</param>
		/// <param name="PK2">pk2</param>
		/// <param name="PK3">pk3</param>
		/// <param name="val">3个PK决定值</param>
		public Rpt3DCell(string PK1, string PK2, string PK3 ,Object _val)
		{
			this.PK1 = PK1;
			this.PK2 = PK2;
			this.PK3 = PK3;
			this.val = _val;
		}
		/// <summary>
		/// 构造一个3维实体
		/// </summary>
		/// <param name="PK1">pk1</param>
		/// <param name="PK2">pk2</param>
		/// <param name="PK3">pk3</param>
		/// <param name="url">要连接到的目标</param>
		/// <param name="val">3个PK决定值</param>
		/// <param name="_Target">目标</param>
		public Rpt3DCell(string PK1, string PK2, string PK3 ,Object _val, string url, string _Target )
		{
			this.PK1 = PK1;
			this.PK2 = PK2;
			this.PK3 = PK3;
			this.val = _val;
			this.Url= url;
			this.Target = _Target;
		}	
		#endregion
		 
	}
	/// <summary>
	/// 3纬报表集合
	/// </summary>
	public class Rpt3DCells : System.Collections.CollectionBase
	{
		#region sum it .
		private float _sum=-1;
		/// <summary>
		/// 总计
		/// </summary>
		public float Sum
		{
			get
			{
				if (_sum==-1)
				{
					_sum =0;
					foreach(Rpt3DCell cell in this)
					{
						_sum+=cell.valOfFloat;
					}
				}
				return _sum;
			}
		}
		/// <summary>
		/// 根据Pk1,取得他的sum.
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <returns>合计</returns>
		public float GetSumByPK1(string pk1)
		{
			float x= 0;
			foreach(RptCell cell in this)
			{
				if (cell.PK1==pk1)				 
					x+=float.Parse(cell.val.ToString());
			}
			return x;
		}
		/// <summary>
		/// 根据Pk1,pk2 取得他的sum.
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <returns>合计</returns>
		public float GetSumByPK1(string pk1,string pk2)
		{
			float x= 0;
			foreach(RptCell cell in this)
			{
				if (cell.PK1==pk1 && cell.PK2==pk2 )
					x+=float.Parse(cell.val.ToString());
			}
			return x;
		}
		#endregion
		
		#region 构造方法
		/// <summary>
		/// 构造方法
		/// </summary>
		public Rpt3DCells()
		{
		}
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列。
		/// 他的顺序必须是与纬度的顺序一致。 
		/// </summary>
		/// <param name="dt">DataTable</param>
		public Rpt3DCells(DataTable dt)
		{
			this.BindWithDataTable(dt);						
		}
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列。
		/// 他的顺序必须是与纬度的顺序一致。
		/// </summary>
		/// <param name="dt">3个列的 DataTable</param>
		/// <param name="url">连接</param>
		public Rpt3DCells(DataTable dt,string url)
		{
			this.BindWithDataTable(dt,url,"");						
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
				this.Add( new Rpt3DCell(dr[0].ToString(),dr[1].ToString(),dr[2].ToString(),dr[3] )) ; 
		}
		/// <summary>
		/// 根据一个Table 构造，这个Table 必须有3个列, 他的顺序是pk1, pk2,pk3.
		/// 他的顺序必须是与纬度的顺序一致。
		/// </summary>
		/// <param name="dt">3个列的 DataTable</param>
		/// <param name="url">连接</param>
		/// <param name="target">连接到 _self , _blank</param>
		public void BindWithDataTable(DataTable dt, string url, string target)
		{
			foreach(DataRow dr in dt.Rows)			
				this.Add( new Rpt3DCell(dr[0].ToString(),dr[1].ToString(),dr[2].ToString(),dr[3],url,target )) ; 
		}
		/// <summary>
		/// 加入一个新的元素
		/// </summary>
		/// <param name="myen">Cell实体</param>
		public virtual void Add(Rpt3DCell myen)
		{
			//判断这个实体是不是存在
			foreach(Rpt3DCell en in this)		
			{
				if (en.PK1 == myen.PK1 && en.PK2 == myen.PK2 && en.PK3 == myen.PK3)				
				{
					try
					{
						en.val =  myen.valOfFloat+ myen.valOfFloat ; 
					}
					catch
					{
					}
					return;
				}
			}
			// 加入这个实体。
			this.InnerList.Add(myen);
			return;
		}
		/// <summary>
		/// Rpt3DCell
		/// </summary>
		public Rpt3DCell this[int index]
		{
			get
			{	
				return (Rpt3DCell)this.InnerList[index];
			}
		}
		/// <summary>
		/// 通过3个值取出他的cell.
		/// 如果没有就New 一个返回。
		/// </summary>
		/// <param name="pk1">pk1</param>
		/// <param name="pk2">pk2</param>
		/// <param name="pk3">pk3</param>
		/// <returns>Rpt3DCell</returns>
		public Rpt3DCell GetCell(string pk1, string pk2, string pk3)
		{
			foreach(Rpt3DCell en in this)
				if (en.PK1 == pk1 && en.PK2 == pk2 && en.PK3 == pk3)
					return en;

			return new Rpt3DCell(pk1,pk2,pk3,0);
		}
		#endregion 
	}
}
