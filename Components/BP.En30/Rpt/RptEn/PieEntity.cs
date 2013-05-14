using System;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En ;
 
namespace BP.DA
{	 
	/// <summary>
	/// PieEntity 的摘要说明。
	/// </summary>
	public class PieEntity
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="No">编号</param>
		/// <param name="Name">名称</param>
		/// <param name="val">值</param>
		public PieEntity(string No, string Name,  float val)
		{
			this.No = No ;
			this.Name= Name;
			this.Val = val;
		}
		public string No="";
		public string Name="";
		public float Val= 0 ; 
	}
	/// <summary>
	/// 
	/// </summary>
	public class PieEntities:System.Collections.CollectionBase 
	{
		public string No="";
		public PieEntities()
		{
		}

		public PieEntities(DataTable dt)
		{
			foreach(DataRow dr in dt.Rows)
			{
				PieEntity en = new PieEntity(dr[0].ToString(),dr[1].ToString(), float.Parse( dr[2].ToString() ) ) ; 
				this.Add(en);
			}
		}

		public virtual void Add(PieEntity en)
		{
			this.InnerList.Add(en);
		}
		public PieEntity this[int index]
		{
			get
			{	
				return (PieEntity)this.InnerList[index];
			}
		}
		private string title="BP 图表";
		public string Title
		{
			get{return this.title;}
			set{this.title=value;}
		}
		/// <summary>
		/// 转换数据到Table
		/// Colmun 如下
		/// No
		/// Name
		/// Val
		/// </summary>
		/// <returns>DataTable</returns>
		public DataTable ToDataTable()
		{
			DataTable dt =new DataTable();
			dt.Columns.Add(new DataColumn("No",typeof(string)) ) ; 
			dt.Columns.Add(new DataColumn("Name",typeof(string)) ) ;
			dt.Columns.Add(new DataColumn("Val",typeof(float)) ) ;
			foreach(PieEntity en in this)
			{
				DataRow dr = dt.NewRow();
				dr["No"]=en.No;
				dr["Name"]=en.Name; 
				dr["Val"]=en.Val;
				dt.Rows.Add(dr);
			}
			return dt; 
		}
	}
	public class PieGroupEntities : System.Collections.CollectionBase 
	{
		public PieGroupEntities()
		{
		}
		/// <summary>
		/// 增一个 PieEntities
		/// </summary>
		/// <param name="en"></param>
		public void Add(PieEntities ens)
		{
			this.InnerList.Add(ens);
		}
		/// <summary>
		/// 取出里面的值根据索引
		/// </summary>
		public PieEntities this[int index]
		{			
			get
			{	
				return (PieEntities)this.InnerList[index];
			}
		}

		private string title="BP 图表";
		public string Title
		{
			get{return this.title;}
			set{this.title=value;}
		}
		/// <summary>
		/// 转换数据到Table
		/// Colmun 如下
		/// GroupNo
		/// GroupTitle
		/// No
		/// Name
		/// Val
		/// </summary>
		/// <returns>DataTable</returns>
		public DataTable ToDataTable()
		{
			DataTable dt =new DataTable();
			dt.Columns.Add(new DataColumn("GroupNo",typeof(string)) ) ; 
			dt.Columns.Add(new DataColumn("GroupTitle",typeof(string)) ) ; 
			dt.Columns.Add(new DataColumn("No",typeof(string)) ) ; 
			dt.Columns.Add(new DataColumn("Name",typeof(string)) ) ;
			dt.Columns.Add(new DataColumn("Val",typeof(float)) ) ;

			foreach(PieEntities ens in this)
			{							
				DataTable tempdt = ens.ToDataTable();
				foreach( DataRow dr1 in tempdt.Rows)
				{
					DataRow dr = dt.NewRow();
					dr["GroupNo"]=ens.No;
					dr["GroupTitle"]=ens.Title;
					dr["No"]=dr1["No"];
					dr["Name"]=dr1["Name"];
					dr["Val"]=dr1["Val"];
					dt.Rows.Add(dr);
				}
			}
			return dt;
 
		}
		
	}

	
}
