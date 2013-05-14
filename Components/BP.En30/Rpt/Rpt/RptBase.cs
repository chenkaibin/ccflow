using System;
using System.Reflection;
using System.Text;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;


namespace BP.Report
{
	/// <summary>
	/// 报表实体的基类，报表至少是2维的，所以基类中有维度1及维度2
	/// 维度增加时，继承基类然后添加维度，添加维度相关方法及输出HTML代码的方法即可
	/// </summary>
	public class RptBase
	{
		public RptBase()
		{
			SB = new StringBuilder();
		}
		private string _rptTableStyle =" border=1 cellspacing=0 style='font-size: 9.5pt' cellpadding=0 bordercolordark='#FFFFFF' bordercolorlight='#000000'";
		/// <summary>
		/// 表格样式
		/// </summary>
		public  string RptTableStyle
		{
			get{ return _rptTableStyle;}
			set{ _rptTableStyle =value;}
		}

		#region  基本属性 ，标题等

		private string _title="BP报表[大标题]";
		/// <summary>
		/// 大标题
		/// </summary>
		public  string Title
		{
			get{ return _title; }
			set{ _title =value; }
		}
		
		private string _titleD1="D1标题";
		/// <summary>
		/// 维度1的标题
		/// </summary>
		public string TitleD1
		{
			get{ return _titleD1; }
			set{ _titleD1 =value; }
		}

		private string _titleD2="D2标题";
		/// <summary>
		/// 维度2的标题
		/// </summary>
		public string TitleD2
		{
			get{ return _titleD2; }
			set{ _titleD2 =value; }
		}
		
		private string _titleVal0="V0数";//"V0标题";
		/// <summary>
		/// 统计数值标题
		/// </summary>
		public string TitleVal0
		{
			get{ return _titleVal0; }
			set{ _titleVal0 =value; }
		}

		private string _author = "";//BP.Web.WebUser.Name ;
		/// <summary>
		/// 制作者
		/// </summary>
		public string Author
		{
			get{ return _author; }
			set{ _author =value; }
		}
		
		private string _reportDate = DateTime.Now.ToString("yyyy年MM月dd日") ; 
		/// <summary>
		/// 制作日期
		/// </summary>
		public string ReportDate
		{
			get{ return _reportDate; }
			set{ _reportDate =value; }
		}
		#endregion


		#region  维度及数据绑定

		private Dimensions _d1Ens=null;
		/// <summary>
		/// 维度1的元素集合
		/// </summary>
		public  Dimensions D1Ens
		{
			get{ return _d1Ens;}
		}
		private string _d1EnsName ="EnsName";
		/// <summary>
		/// 维度1的类名称，在向别的页面传参数时用到
		/// </summary>
		public  string D1EnsName 
		{
			get{ return _d1EnsName;}
		}
		private string _d1AddHref ="";
		/// <summary>
		/// 维度1链接向别的页面传参数时的附加内容，主要为了传递外部条件
		/// </summary>
		public  string D1AddHref 
		{
			get{ return _d1AddHref;}
			set{ _d1AddHref =value;}
		}
		private string _d1URL ="";// System.Web.HttpContext.Current.Request.ApplicationPath+"/Rpt/DimensionRefLink.aspx";
		/// <summary>
		/// 维度1的链接
		/// </summary>
		public  string D1URL 
		{
			get{ return _d1URL;}
			set
			{
				_d1URL =value;
			}
		}
		private string _d1Target ="_d1Target";
		/// <summary>
		/// 维度1的链接的目标窗口
		/// </summary>
		public  string D1Target 
		{
			get{ return _d1Target;}
			set{ _d1Target =value;}
		}

		private Dimensions _d2Ens=null;
		/// <summary>
		/// 维度2的元素集合
		/// </summary>
		public  Dimensions D2Ens
		{
			get{ return _d2Ens;}
		}
		private bool _d2RefD1 =false;
		/// <summary>
		/// 维度2是否关联于维度1
		/// </summary>
		public  bool D2RefD1 
		{
			get{ return _d2RefD1;}
			set{ _d2RefD1 =value;}
		}
		private string _d2EnsName ="EnsName";
		/// <summary>
		/// 维度2的类名称，在向别的页面传参数时用到
		/// </summary>
		public  string D2EnsName 
		{
			get{ return _d2EnsName;}
		}
		private string _d2AddHref ="";
		/// <summary>
		/// 维度2链接向别的页面传参数时的附加内容，主要为了传递外部条件
		/// </summary>
		public  string D2AddHref 
		{
			get{ return _d2AddHref;}
			set{ _d2AddHref =value;}
		}
		private string _d2URL = "";//System.Web.HttpContext.Current.Request.ApplicationPath+"/Rpt/DimensionRefLink.aspx";
		/// <summary>
		/// 维度2的链接
		/// </summary>
		public  string D2URL 
		{
			get{ return _d2URL;}
			set{ _d2URL =value;}
		}
		private string _d2Target ="_d2Target";
		/// <summary>
		/// 维度2的链接的目标窗口
		/// </summary>
		public  string D2Target 
		{
			get{ return _d2Target;}
			set{ _d2Target =value;}
		}
		
		/// <summary>
		/// 设置维度1
		/// </summary>
		/// <param name="titleD1">维度1的标题</param>
		/// <param name="d1Ens">维度1的元素集合，为null时会自动创建空的集合</param>
		/// <param name="d1EnsName">维度1的类名称</param>
		public void BindD1(string titleD1 ,Dimensions d1Ens ,string d1EnsName)
		{
			this._titleD1     = titleD1;
			if(d1Ens==null)
				this._d1Ens = new Dimensions();
			else
				this._d1Ens = d1Ens;
			this._d1EnsName = d1EnsName;
		}
		/// <summary>
		/// 在维度1的元素集合中添加数据元素
		/// </summary>
		/// <param name="d1">维度元素</param>
		/// <returns>是否添加成功，不成功则表示已经存在键值相同的元素</returns>
		public bool D1AddDim(Dimension d1)
		{
			return this.D1Ens.Add( d1 ,true);
		}
		
		/// <summary>
		/// 设置维度2
		/// </summary>
		/// <param name="titleD2">维度2的标题</param>
		/// <param name="d2Ens">维度2的元素集合，为null时会自动创建空的集合</param>
		/// <param name="d2RefD1">维度2是否关联于维度1</param>
		/// <param name="d2EnsName">维度2的类名称</param>
		public void BindD2(string titleD2 ,Dimensions d2Ens ,bool d2RefD1 ,string d2EnsName)
		{
			this._titleD2  = titleD2;
			
			if(d2Ens==null)
				this._d2Ens = new Dimensions();
			else
				this._d2Ens = d2Ens;

			this._d2RefD1 = d2RefD1;
			this._d2EnsName = d2EnsName;
		}
		/// <summary>
		/// 在维度2的元素集合中添加数据元素
		/// </summary>
		/// <param name="d2">维度元素</param>
		/// <returns>是否添加成功，不成功则表示已经存在键值相同的元素</returns>
		public bool D2AddDim(Dimension d2)
		{
			return this.D2Ens.Add( d2 ,true);
		}


		#endregion
	
		#region 获取维度数据

		/// <summary>
		/// 获取维度1的HTML内容
		/// </summary>
		/// <param name="en1">维度元素</param>
		/// <returns>包装了en1的HTML内容</returns>
		public string GetD1Content(Dimension en1)
		{
			if (this.D1URL.Trim()!="" && en1.No!="sum")
				return "<a href='"+this.D1URL
					+"?EnsName="+ this.D1EnsName+"&Val="+en1.No
					+ this.D1AddHref +"'"
					+" Title='"+ en1.Name+"["+en1.No+"]'"
					+" Target='"+ this.D1Target +"'>"+ en1.Name +"</a>";
			else
				return en1.Name;
		}


		/// <summary>
		/// 获取维度2的HTML内容
		/// </summary>
		/// <param name="en2">维度2的元素</param>
		/// <returns>HTML代码</returns>
		public string GetD2Content(Dimension en2 )
		{
			return GetD2Content( en2,null);
		}
		/// <summary>
		/// 获取维度2的HTML内容，内容可能包括维度1
		/// </summary>
		/// <param name="en2">维度2的元素</param>
		/// <param name="en1">维度1的元素，en1不为空时，内容同时包装的了维度1</param>
		/// <returns>HTML代码</returns>
		public string GetD2Content(Dimension en2 ,Dimension en1)
		{
			if (this.D2URL.Trim()!="" && en2.No!="sum")
			{
				string href = "<a href='"+this.D2URL;
				href +="?EnsName=" + this.D2EnsName+ "&Val=" + en2.No;
				if( en1!=null)
					href +="&RefD1EnsName="+ this.D1EnsName + "&RefD1Val=" +en1.No;
				href += this.D2AddHref +"' ";
				href +=" Title='";
				if( en1!=null)
					href += en1.Name+"["+en1.No+"]"+",";
				href +=en2.Name+"["+en2.No+"]" +"'";
				href +=" Target='"+ this.D2Target +"'>"+ en2.Name + "</a>";
				return href;
			}
			else
				return en2.Name;
		}

		/// <summary>
		/// 通过某维度1元素的键值，从维度2集合中获取集合，当维度2不关联于维度1时，返回整个维度2集合
		/// </summary>
		/// <param name="en1No">键值，是维度1元素的编码</param>
		/// <returns></returns>
		public Dimensions GetD2EnsBy1FK(string en1No)
		{
			if(!this.D2RefD1)//非关联时，返回整个集合
			{
				return this.D2Ens ;
			}
			else//是关联时，比较键值成功后加入返回集合
			{
				#region
				Dimensions refds = new Dimensions();
				foreach(Dimension en2 in this.D2Ens)
				{
					if (en2 != null && en2.FK==en1No)
					{
						refds.Add( en2 ,true);
					}
				}
				#endregion
				return refds;
			}
		}

		
		#endregion


		#region 绑定及获取单元格数据

		private string _celUrl="";
		/// <summary>
		/// 数据格的链接
		/// </summary>
		public string CellUrl
		{
			get
			{
				return _celUrl;
			}
			set
			{
				this._celUrl = value;
			}
		}
		private string _cellTarget="_self";
		/// <summary>
		/// 数据格的链接的目标窗口
		/// </summary>
		public string CellTarget
		{
			get
			{
				return _cellTarget;
			}
			set
			{
				this._cellTarget = value;
			}
		}

		private string _cellAddHref="";
		/// <summary>
		/// 数据格链接向别的页面传参数时的附加内容，主要为了传递外部条件
		/// </summary>
		public  string CellAddHref
		{
			get{return _cellAddHref;}
			set{_cellAddHref =value;}
		}
		public virtual void BindDataCells( object cells ,string url ,string target)
		{
		}
		#endregion 


		#region 包装为HTML代码
		
		/// <summary>
		/// 输出字符
		/// </summary>
		/// <param name="str"></param>
		public void RW(string str)
		{
			this.WriterHtmlText( str);
		}
		/// <summary>
		/// 输出单元格
		/// </summary>
		/// <param name="str"></param>  
		public void RWTD(string str)
		{
			this.WriterHtmlText( "<TD nowrap >"+str+"</TD>");
		}
		/// <summary>
		/// 输出单元格
		/// </summary>
		/// <param name="str"></param>  
		public void RWTDCenter(string str)
		{
			this.WriterHtmlText( "<TD nowrap align=center>"+str+"</TD>");
		}
		/// <summary>
		/// 输出单元格
		/// </summary>
		/// <param name="str"></param>  
		public void RWTD(string str, int rowspan)
		{
			this.WriterHtmlText("<TD rowspan='"+rowspan.ToString()+"' >"+str+"</TD>");
		}
		/// <summary>
		/// 输出标题单元格
		/// </summary>
		/// <param name="str"></param>
		public void RWTH(string str)
		{
			this.WriterHtmlText("<TH nowrap bgcolor='#C0C0C0' >"+str+"</TH>");
		}
		/// <summary>
		/// 输出到中间
		/// </summary>
		/// <param name="str"></param>
		public void RWCenter(string str)
		{
			this.WriterHtmlText("<p align='center'> "+str+"</P>");
		}
		/// <summary>
		/// 输出到中间并且 Blod
		/// </summary>
		/// <param name="str"></param>
		public void RWCenterAndB(string str)
		{
			this.WriterHtmlText("<p align='center'> <b>"+str+"</b></P>");
		}
		/// <summary>
		/// 输出并且换行
		/// </summary>
		/// <param name="str"></param>
		public void RWLine(string str)
		{
			this.WriterHtmlText("<br>"+str);
		}
		/// <summary>
		/// 输出BR
		/// </summary>
		public void RWBR()
		{
			this.WriterHtmlText("<br>");
		}
		/// <summary>
		/// 输出空格
		/// </summary>
		/// <param name="i">输出空格个数</param>
		public void RWBlank(int i )
		{	      
			for( int ii = 0 ; ii< i; ii++ )
			{
				this.RWBlank();
			}
		}
		public void RWBlank()
		{
			this.WriterHtmlText("&nbsp;");
		}

		public void RWAlignAndB(string str ,string align)
		{
			this.WriterHtmlText("<p align='"+ align +"'> <b>"+str+"</b></P>");
		}
		public void RWTitle(string str )
		{
			this.WriterHtmlText("<div align='center'><b><font face='宋体' size='4'>"+str+"</font></b></div>");
		}
		public void RWRedError(string str,string align)
		{
			this.WriterHtmlText("<p align='"+ align +"'> <b><font face='宋体' color='red'>"+str+"</font></b></P>");
		}

		public void RWLineBold0(string str)
		{
			this.RW("<div align='center'>") ;
			this.WriterHtmlText("<b>"+str+"</b>");
			this.RW("</div>") ; 
		}
		public void WriterHtmlText( string text)
		{
			this.SB.Append( text );
		}

		protected readonly StringBuilder SB;

		#endregion

		#region Get HTML 
		/// <summary>
		/// 获取Html代码
		/// </summary>
		public virtual string GetHtmlCode()
		{
			return this.SB.ToString();
		}

		#endregion 
	}


}
