using System;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace BP.Report
{
	public class RptCellBase
	{
		private string _pk1=null;
		public string PK1
		{
			get{ return this._pk1;}
			set{ this._pk1 =value;}
		}


		private string _pk2=null;
		public string PK2
		{
			get{ return this._pk2;}
			set{ this._pk2 =value;}
		}
		
		
		private object  _value = 0;
		public  object  Value
		{
			get{ return this._value;}
			set
			{
				this._value =value;
			}
		}
		

		private string _toolTip="";
		public string ToolTip
		{
			get
			{
				return _toolTip;
			}
			set
			{
				this._toolTip = value;
			}
		}


		private string _url="";
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				this._url = value;
			}
		}


		private string _target="_self";
		public string Target
		{
			get
			{
				return _target;
			}
			set
			{
				this._target = value;
			}
		}


	}

	public class Rpt2DCell :RptCellBase
	{
		public Rpt2DCell(string pk1, string pk2, object val, string url ,string target)
		{
			this.PK1 = pk1;
			this.PK2 = pk2;
			this.Value = val;
			this.Url = url;
			this.Target =target; 
		}
	}
	public class Rpt3DCell : RptCellBase
	{

		private string _pk3=null;
		public string PK3
		{
			get{ return this._pk3;}
			set{ this._pk3 =value;}
		}
		

		public Rpt3DCell(string pk1, string pk2, string pk3 ,object val, string url ,string target)
		{
			this.PK1 = pk1;
			this.PK2 = pk2;
			this.PK3 = pk3;
			this.Value = val;
			this.Url = url;
			this.Target =target;
		}


	}


	public class Rpt2DCells : System.Collections.CollectionBase
	{
		#region 构造方法
		public Rpt2DCells()
		{
		}
		public Rpt2DCells(DataTable tb, string url ,string target)
		{
			BindData( tb ,url ,target);
		}
		#endregion

		#region 公共方法
		public  void BindData(DataTable tb, string url ,string target)
		{
			foreach(DataRow dr in tb.Rows)	
			{
				object val = 0;
				if( !dr[2].Equals(DBNull.Value))
					val = float.Parse( dr[2].ToString());
				this.Add( dr[0].ToString().Trim()
					,dr[1].ToString().Trim()
					,val
					,url 
					,target) ; 
			}
		}


		public bool ContainsCell( Rpt2DCell cell)
		{
			return ContainsCell( cell.PK1 ,cell.PK2 );
		}
		public bool ContainsCell(string pk1 ,string pk2 )
		{
			foreach(Rpt2DCell cell in this)		
			{
				if (cell.PK1 ==pk1 && cell.PK2 == pk2 )
				{
					return true;
				}
			}
			return false;
		}


		public bool Add(string pk1,string pk2,object val,string url ,string target)
		{
			if( this.ContainsCell( pk1 ,pk2))
				return false;
			else
			{
				Rpt2DCell en = new Rpt2DCell(pk1,pk2,val,url,target) ;
				this.InnerList.Add(en);
				return true;
			}
		}
		public bool Add(Rpt2DCell add)
		{
			if( this.ContainsCell( add))
				return false;
			else
			{
				this.InnerList.Add(add);
				return true;
			}
		}



		public Rpt2DCell this[int index]
		{
			get
			{	
				return this.InnerList[index] as Rpt2DCell;
			}
		}

		public Rpt2DCell Get2DCell(string pk1, string pk2)
		{
			foreach(Rpt2DCell en in this)
			{
				if (string.Compare(en.PK1,pk1)==0 &&string.Compare(en.PK2,pk2)==0)
					return en;
			}
			return null;
		}

		public string GenerHtmlStrBy(string pk1, string pk2 )
		{
			Rpt2DCell cell = this.Get2DCell(pk1,pk2) ;

			if( cell ==null)
				return "-";
			if (cell.Url=="")
				return cell.Value.ToString();
			else
				return "<A href='"+cell.Url+"?PK1='"+pk1
					+"&PK2="+pk2+" >"
					+cell.Value+"</A>";
		}
		#endregion 

	}
	
	
	public class Rpt3DCells : System.Collections.CollectionBase
	{
		#region 构造方法
		public Rpt3DCells()
		{
		}		 
		public Rpt3DCells(DataTable tb,string url ,string target)
		{
			this.BindData(tb,url,target);
		}
		#endregion

		#region 公共方法
		
		public  void BindData(DataTable tb, string url ,string target)
		{
			foreach(DataRow dr in tb.Rows)	
			{
				object val = 0;
				if( !dr[2].Equals(DBNull.Value))
					val = float.Parse( dr[2].ToString());
				this.Add( dr[0].ToString().Trim()
					,dr[1].ToString().Trim()
					,dr[2].ToString().Trim()
					,val
					,url 
					,target) ; 
			}
		}

		public bool ContainsCell( Rpt3DCell cell)
		{
			return ContainsCell( cell.PK1 ,cell.PK2 ,cell.PK3);
		}
		public bool ContainsCell(string pk1 ,string pk2 ,string pk3)
		{
			foreach(Rpt3DCell cell in this)
			{
				if (string.Compare(cell.PK1,pk1,true)==0 &&string.Compare(cell.PK2,pk2,true)==0 &&string.Compare(cell.PK3,pk3,true)==0)				
				{
					return true;
				}
			}
			return false;
		}


		public bool Add(string pk1, string pk2, string pk3 ,object val ,string url ,string target)
		{
			if( this.ContainsCell(pk1 ,pk2 ,pk3))
				return false;
			else
			{
				Rpt3DCell cell = new Rpt3DCell(pk1 ,pk2 ,pk3 ,val,url,target);
				this.InnerList.Add( cell);
				return true;
			}
		}
		public bool Add(Rpt3DCell add)
		{
			if( this.ContainsCell(add))
				return false;
			else
			{
				this.InnerList.Add(add);
				return true;
			}
		}
		public Rpt3DCell this[int index]
		{
			get
			{	
				return this.InnerList[index] as Rpt3DCell;
			}
		}
		public Rpt3DCell Get3DCell(string pk1, string pk2, string pk3)
		{
			foreach(Rpt3DCell en in this)
			{
				if (en.PK1 == pk1 && en.PK2 == pk2 && en.PK3 == pk3)
					return en;
			}
			return null;
		}
		#endregion 
	}
	

	
}
