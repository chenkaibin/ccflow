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
	/// 3D报表样式
	/// </summary>
	public enum Rpt3DStyle
	{
		/// <summary>
		/// 在左边显示D1,D2,顶部显示D3
		/// </summary>
		LeftD1D2,

		/// <summary>
		/// 在左边显示D1,顶部显示D2,D3
		/// </summary>
		LeftD1,

		/// <summary>
		/// 在左边显示D2,D3,顶部显示D1
		/// </summary>
		LeftD2D3,

		/// <summary>
		/// 在左边显示D3,顶部显示D1,D2
		/// </summary>
		LeftD3
	}
	/// <summary>
	/// 3D报表实体，有关说明请参考RptBase
	/// </summary>
	public class  Rpt3DEntity:RptBase
	{
		public Rpt3DEntity()
		{
		}
		private Rpt3DStyle _style = Rpt3DStyle.LeftD1D2;
		/// <summary>
		/// 报表样式
		/// </summary>
		public Rpt3DStyle Style 
		{
			get{ return _style;}
			set{ _style =value;}
		}

		
		#region Get HTML
		/// <summary>
		/// 获取Html代码
		/// </summary>
		public override string GetHtmlCode()
		{
			switch( this.Style )
			{
				case Rpt3DStyle.LeftD1D2:
					return GetHtmlCode_LeftD1D2();

				case Rpt3DStyle.LeftD2D3:
					return GetHtmlCode_LeftD2D3();

				case Rpt3DStyle.LeftD3:
					return GetHtmlCode_LeftD3();

				case Rpt3DStyle.LeftD1:
					return GetHtmlCode_LeftD1();

				default:
					throw new Exception("设置Style发生异常！");
			}
		}
		public string GetHtmlCode_LeftD2D3()
		{
			this.SB.Remove(0,this.SB.Length);

			this.RWTitle(this.Title );
			//this.RWBR();
			this.RW("<div align='center'>") ; 
			this.RW("<table "+ this.RptTableStyle +" >");

			this.RW("<TR>");
			this.RW("<TH bgcolor='#C0C0C0'  colspan='2' nowrap align='center'>分组</TH>");
			foreach(Dimension en1 in this.D1Ens ) 
			{
				this.RW("<TH bgcolor='#C0C0C0' rowspan='2' nowrap>"+  this.GetD1Content(en1)  +"</TH>" ) ;
			}
			this.RW("</TR>");

			this.RW("<TR>");
			this.RWTH( this.TitleD2 ) ;
			this.RWTH( this.TitleD3 ) ;
			this.RW("</TR>");

			#region 生成内容

			foreach(Dimension en2 in this.D2Ens  ) 
			{
				Dimensions ds3 = this.GetD3EnsBy2FK( en2.No);
				if (ds3.Count == 0)
					continue;

				this.RW("<TR>");

				this.RWTD( this.GetD2Content(en2) , ds3.Count );
				bool isFirst=true;
				foreach(Dimension en3 in ds3)
				{
					this.RWTD( this.GetD3Content(en3) ) ;
					foreach(Dimension en1 in this.D1Ens )
					{
						this.RWTDCenter( this.Get3DCellContent(en1.No,en1.Name ,en2.No,en2.Name ,en3.No,en3.Name)) ;
					}
					if (isFirst)
					{
						this.RW("</TR>");
						isFirst=false;
					}
					else
					{
						this.RW("<TR>");
					}
				}
				this.RW("</TR>");
			}
			 
			#endregion
 
			this.RW("</table>");
			this.RWCenter(this.Author+"&nbsp;&nbsp;&nbsp;&nbsp;"+this.ReportDate);
			this.RW("</div>");

			return this.SB.ToString();
		}
		public string GetHtmlCode_LeftD1D2()
		{
			this.SB.Remove(0,this.SB.Length);
			
			this.RWTitle(this.Title );
			//this.RWBR();
			this.RW("<div align='center'>") ; 
			this.RW("<table "+ this.RptTableStyle +" >");

			this.RW("<TR>");
			this.RW("<TH bgcolor='#C0C0C0'  colspan='2' nowrap align='center'>分组</TH>");
			foreach(Dimension en3 in this.D3Ens ) 
			{
				this.RW("<TH bgcolor='#C0C0C0' rowspan='2' nowrap>"+  this.GetD3Content(en3)  +"</TH>" ) ;
			}
			this.RW("</TR>");
			this.RW("<TR>");
			this.RWTH( this.TitleD1 ) ;
			this.RWTH( this.TitleD2 ) ;
			this.RW("</TR>");

			#region 生成内容

			foreach(Dimension en1 in this.D1Ens  ) 
			{	
				Dimensions ds2 = this.GetD2EnsBy1FK( en1.No);
				if (ds2.Count == 0)
					continue;

				this.RW("<TR>");
				this.RWTD( this.GetD1Content(en1) , ds2.Count );
				bool isFirst=true;
				foreach(Dimension en2 in ds2)
				{
					this.RWTD( this.GetD2Content(en2,en1) ) ;
					foreach(Dimension en3 in this.D3Ens )
					{
						this.RWTDCenter( this.Get3DCellContent(en1.No,en1.Name ,en2.No,en2.Name ,en3.No,en3.Name)) ;
					}
					if (isFirst)
					{
						this.RW("</TR>");
						isFirst=false;
					}
					else
					{
						this.RW("<TR>");
					}
				}
				this.RW("</TR>");
			}
			 
			#endregion
 
			this.RW("</table>");
			this.RWCenter(this.Author+"&nbsp;&nbsp;&nbsp;&nbsp;"+this.ReportDate);
			this.RW("</div>");

			return this.SB.ToString();
		}
		

		public string GetHtmlCode_LeftD3()
		{
			this.SB.Remove(0,this.SB.Length);

			this.RWTitle(this.Title );
			//this.RWBR();
			this.RW("<div align='center'>") ; 
			this.RW("<table "+ this.RptTableStyle +" >");

			this.RW("<TR>");
			this.RW("<TH bgcolor='#C0C0C0'  rowspan='2' nowrap align='center'>"+ this.TitleD3 +"</TH>");
			string tr2 = "";
			string[] tr3__ = new string[this.D3Ens.Count];
			foreach(Dimension en1 in this.D1Ens ) 
			{
				Dimensions ds2 = this.GetD2EnsBy1FK( en1.No);
				if (ds2.Count == 0)
					continue;
				this.RW("<TH bgcolor='#C0C0C0' colspan='" +ds2.Count+ "' nowrap>"+  this.GetD1Content(en1)  +"</TH>" ) ;
				foreach(Dimension en2 in ds2 ) 
				{
					tr2 +="<td nowrap align=center>" + this.GetD2Content(en2,en1) + "</td>";
					for(int r=0;r<this.D3Ens.Count;r++) 
					{
						string cell = this.Get3DCellContent(en1.No,en1.Name ,en2.No,en2.Name ,this.D3Ens[r].No,this.D3Ens[r].Name);
						tr3__[r] +="<td nowrap align=center>" + cell + "</td>";
					}
				}
			}
			this.RW("</TR>");

			this.RW("<TR>");
			this.RW( tr2 );
			this.RW("</TR>");

			for(int r=0;r<this.D3Ens.Count;r++) 
			{
				this.RW("<TR>");
				this.RW("<td nowrap align=center>" + this.GetD3Content(this.D3Ens[r]) + "</td>");
				this.RW( tr3__[r] );
				this.RW("</TR>");
			}

			this.RW("</table>");
			this.RWCenter(this.Author+"&nbsp;&nbsp;&nbsp;&nbsp;"+this.ReportDate);
			this.RW("</div>");

			return this.SB.ToString();
		}
		
		public string GetHtmlCode_LeftD1()
		{
			this.SB.Remove(0,this.SB.Length);

			this.RWTitle(this.Title );
			//this.RWBR();
			this.RW("<div align='center'>") ; 
			this.RW("<table "+ this.RptTableStyle +" >");

			this.RW("<TR>");
			this.RW("<TH bgcolor='#C0C0C0'  rowspan='2' nowrap align='center'>"+ this.TitleD1 +"</TH>");
			string tr2 = "";
			string[] tr3__ = new string[this.D1Ens.Count];
			foreach(Dimension en2 in this.D2Ens ) 
			{
				Dimensions ens3 = this.GetD3EnsBy2FK( en2.No);
				if (ens3.Count == 0)
					continue;
				this.RW("<TH bgcolor='#C0C0C0' colspan='" +ens3.Count+ "' nowrap>"+  this.GetD2Content(en2)  +"</TH>" ) ;

				foreach(Dimension en3 in ens3 ) 
				{
					tr2 +="<td nowrap align=center>" + this.GetD3Content(en3) + "</td>";

					for(int r=0;r<this.D1Ens.Count;r++) 
					{
						string cell = this.Get3DCellContent( this.D1Ens[r].No,this.D1Ens[r].Name ,en2.No,en2.Name ,en3.No,en3.Name);
						tr3__[r] +="<td nowrap align=center>" + cell + "</td>";
					}
				}

			}
			this.RW("</TR>");

			this.RW("<TR>");
			this.RW( tr2 );
			this.RW("</TR>");

			for(int r=0;r<this.D1Ens.Count;r++) 
			{
				this.RW("<TR>");
				this.RW("<td nowrap align=center>" + this.GetD1Content(this.D1Ens[r]) + "</td>");
				this.RW( tr3__[r] );
				this.RW("</TR>");
			}

			this.RW("</table>");
			this.RWCenter(this.Author+"&nbsp;&nbsp;&nbsp;&nbsp;"+this.ReportDate);
			this.RW("</div>");

			return this.SB.ToString();
		}
		
		
		#endregion Get HTML
		
		private string _titleD3="D3标题";
		/// <summary>
		/// 维度3的标题
		/// </summary>
		public string TitleD3
		{
			get{ return _titleD3; }
			set{ _titleD3 =value; }
		}

		
		#region 维度扩展

		private Dimensions _d3Ens=null;
		public  Dimensions D3Ens
		{
			get{ return _d3Ens;}
		}
		private string _d3EnsName ="EnsName";
		public  string D3EnsName 
		{
			get{ return _d3EnsName;}
		}
		private string _d3AddHref ="";
		public  string D3AddHref 
		{
			get{ return _d3AddHref;}
			set{ _d3AddHref =value;}
		}
		private string _d3URL ="";// System.Web.HttpContext.Current.Request.ApplicationPath+"/Rpt/DimensionRefLink.aspx";
		public  string D3URL 
		{
			get{ return _d3URL;}
			set{ _d3URL =value;}
		}
		private string _d3Target ="_d3Target";
		/// <summary>
		/// 维度2的链接的目标窗口
		/// </summary>
		public  string D3Target 
		{
			get{ return _d3Target;}
			set{ _d3Target =value;}
		}


		private bool _d3RefD2 =false;
		public  bool D3RefD2
		{
			get{ return _d3RefD2;}
			set{ _d3RefD2 =value;}
		}
		/// <summary>
		/// 设置维度3
		/// </summary>
		/// <param name="titleD3">维度3的标题</param>
		/// <param name="d3Ens">维度3的元素集合，为null时会自动创建空的集合</param>
		/// <param name="d3RefD2">维度3是否关联于维度2</param>
		/// <param name="d3EnsName">维度3的类名称</param>
		public void BindD3(string titleD3 ,Dimensions d3Ens ,bool d3RefD2 ,string d3EnsName)
		{
			this._titleD3 = titleD3;
			
			if(d3Ens==null)
				this._d3Ens = new Dimensions();
			else
				this._d3Ens = d3Ens;

			_d3RefD2      = d3RefD2;
			_d3EnsName  = d3EnsName;
		}

		#endregion

		#region 获取维度数据

		/// <summary>
		/// 获取维度3的HTML内容
		/// </summary>
		/// <param name="en3">维度3的元素</param>
		/// <returns>HTML代码</returns>
		public string GetD3Content(Dimension en3)
		{
			return GetD3Content( en3 ,null);
		}
		/// <summary>
		/// 获取维度2的HTML内容，内容可能包括维度2
		/// </summary>
		/// <param name="en3">维度3的元素</param>
		/// <param name="en2">维度2的元素，en2不为空时，内容同时包装的了维度2</param>
		/// <returns>HTML代码</returns>
		public string GetD3Content(Dimension en3 ,Dimension en2)
		{
			if (this.D3URL.Trim()!="")
			{
				string href = "<a href='"+this.D3URL;
				href +="?EnsName=" + this.D3EnsName+ "&Val=" + en3.No;
				if( en2!=null)
					href +="&RefD2EnsName="+ this.D2EnsName + "&RefD2Val=" +en2.No;
				href += this.D3AddHref +"' ";
				href +=" Title='";
				if( en2!=null)
					href += en2.Name+"["+en2.No+"]"+",";
				href +=en2.Name+"["+en3.No+"]" +"' ";
				href +=" Target='"+ this.D3Target +"'>"+ en3.Name + "</a>";
				return href;
			}
			else
				return en3.Name;
		}


		#endregion


		#region 绑定及获取单元格数据

		private Rpt3DCells _dataCells3D = new Rpt3DCells();
		/// <summary>
		/// 获取3D数据源
		/// </summary>
		public  Rpt3DCells DataCells3D 
		{
			get{ return _dataCells3D; }
		}

		/// <summary>
		/// 绑定数据源
		/// </summary>
		/// <param name="d3Cells">数据源，必须是Rpt3DCells才行，否则会出错</param>
		/// <param name="url">统计数值上的链接</param>
		/// <param name="target">目标窗口</param>
		public override void BindDataCells( object d3Cells ,string url ,string target)
		{
			this.CellUrl    = url;
			this.CellTarget = target;
			this._dataCells3D = (Rpt3DCells)d3Cells;
		}

		/// <summary>
		/// 填充单元格数据[只使用前4列填充,不包含维度名称]
		/// </summary>
		/// <param name="dvWith4Cols">包含有4列的数据视图</param>
		/// <param name="url">单元格数据链接</param>
		/// <param name="target">链接目标窗口</param>
		public void BindDataByDV4( DataView dvWith4Cols ,string url ,string target)
		{
			if( dvWith4Cols.Table.Columns.Count<4)
				throw new Exception("数据源没有足够的列！");

			this.DataCells3D.Clear();
			this.CellUrl    = url;
			this.CellTarget = target;
			foreach(DataRowView row in dvWith4Cols)
			{
				this.DataCells3D.Add( row[0].ToString().Trim()
					,row[1].ToString().Trim()
					,row[2].ToString().Trim()
					,row[3]
					,url
					,target);
			}
		}
		/// <summary>
		/// 填充格数据[使用前7列填充,格式为"编号,名称,编号,名称,编号,名称,合计数"]
		/// 可自动填充维度
		/// </summary>
		/// <param name="dvWith7Cols">包含7列数据的视图</param>
		/// <param name="fill3DData">是否填充维度上的数据[要求事先设定维度信息]</param>
		/// <param name="url">单元格数据链接</param>
		/// <param name="target">链接目标窗口</param>
		public void BindDataByDV7( DataView dvWith7Cols ,bool fill3DData ,string url ,string target )
		{
			if( dvWith7Cols.Table.Columns.Count<7)
				throw new Exception("数据源没有足够的列！");

			this.DataCells3D.Clear();
			this.CellUrl    = url;
			this.CellTarget = target;
			if( fill3DData )//填充维度数据
			{
				#region 填充 单元格数据 ,维度数据
				//置空
				this.D1Ens.Clear();
				this.D2Ens.Clear();
				this.D3Ens.Clear();
				this.DataCells3D.Clear();

				foreach(DataRowView row in dvWith7Cols)
				{
					#region 填充单元格数据
					this.DataCells3D.Add( row[0].ToString().Trim()
						,row[2].ToString().Trim()
						,row[4].ToString().Trim()
						,row[6]
						,url
						,target);
					#endregion 填充单元格数据

					#region 填充维度1
					this.D1Ens.Add(row[0].ToString().Trim()
						,row[1].ToString().Trim()
						,""
						,true);

					#endregion 填充维度1
				
					#region 填充维度2
				
					if( this.D2RefD1)//维度2关联于维度1
					{
						this.D2Ens.Add(row[2].ToString().Trim()
							,row[3].ToString().Trim()
							,row[0].ToString().Trim()
							,true);
					}
					else
					{
						this.D2Ens.Add(row[2].ToString().Trim()
							,row[3].ToString().Trim()
							,""
							,true);
					}

					
					#endregion 填充维度2
			
					#region 填充维度3
					
					if( this.D3RefD2)//维度3关联于维度2
					{
						this.D3Ens.Add(row[4].ToString().Trim()
							,row[5].ToString().Trim()
							,row[2].ToString().Trim()
							,true);
					}
					else
					{
						this.D3Ens.Add(row[4].ToString().Trim()
							,row[5].ToString().Trim()
							,""
							,true);
					}
					
					#endregion 填充维度3
				}

				#endregion 填充维度数据
			}
			else
			{
				#region 填充单元格数据
				foreach(DataRowView row in dvWith7Cols)
				{
					this.DataCells3D.Add( row[0].ToString().Trim()
						,row[2].ToString().Trim()
						,row[4].ToString().Trim()
						,row[6]
						,this.CellUrl,this.CellTarget);
				}
				#endregion 填充单元格数据
			}
		}

		/// <summary>
		/// 对维度1进行合计
		/// 为了不破坏显示格式，本算法必须设定 this.Style == Rpt3DStyle.LeftD1D2 才计算
		/// </summary>
		/// <param name="sumTitle">合计列上的标题</param>
		public void AddSumD1(string sumTitleOnD1,string sumTitleOnD2,string sumTitleOnD3)
		{
			#region 增加合计
			if(this.Style == Rpt3DStyle.LeftD1D2)
			{
				this.D1Ens.Add("sum",sumTitleOnD1,"",true);
				this.D2Ens.Add("sum",sumTitleOnD1,"sum",true);
				foreach(Dimension en3 in this.D3Ens )
				{
					float sum = 0;
					foreach(Dimension en1 in this.D1Ens  ) 
					{
						Dimensions ds2 = this.GetD2EnsBy1FK( en1.No);
						foreach(Dimension en2 in ds2)
						{
							Rpt3DCell cell = this.DataCells3D.Get3DCell(en1.No,en2.No,en3.No);
							if (cell==null||cell.Value==null||cell.Value.Equals(DBNull.Value))
							{}
							else if(float.Parse( cell.Value.ToString())==0)
							{}
							else
							{
								sum += float.Parse( cell.Value.ToString());
							}
						}
					}
					this.DataCells3D.Add( "sum","sum",en3.No,sum,this.CellUrl,this.CellTarget);
				}
			}
			#endregion 增加合计
		}
		/// <summary>
		/// 对维度2进行合计
		/// </summary>
		/// <param name="sumTitle">合计列上的标题</param>
		public void AddSumD2(string sumTitle)
		{
			#region 增加合计
			foreach(Dimension en1 in this.D1Ens  ) 
			{
				foreach(Dimension en3 in this.D3Ens )
				{
					float sum = 0;
					Dimensions ds2 = this.GetD2EnsBy1FK( en1.No);
					if(ds2.Count<2)
						continue;
					foreach(Dimension en2 in ds2)
					{
						Rpt3DCell cell = this.DataCells3D.Get3DCell(en1.No,en2.No,en3.No);
						if (cell==null||cell.Value==null||cell.Value.Equals(DBNull.Value))
						{}
						else if(float.Parse( cell.Value.ToString())==0)
						{}
						else
						{
							sum += float.Parse( cell.Value.ToString());
						}
					}
					if(this.D2RefD1)
						this.D2Ens.Add("sum",sumTitle,en1.No,true);
					else
						this.D2Ens.Add("sum",sumTitle,"",true);
					this.DataCells3D.Add( en1.No,"sum",en3.No,sum,this.CellUrl,this.CellTarget);
				}
			}
			#endregion 增加合计
		}		
		/// <summary>
		/// 对维度3进行合计
		/// </summary>
		/// <param name="sumTitle">合计列上的标题</param>
		public void AddSumD3(string sumTitle)
		{
			#region 增加合计
			foreach(Dimension en1 in this.D1Ens  ) 
			{
				Dimensions ds2 = this.GetD2EnsBy1FK( en1.No);
				foreach(Dimension en2 in ds2)
				{
					float sum = 0;
					Dimensions ds3 = this.GetD3EnsBy2FK( en2.No);
					if(ds3.Count<2)
						continue;
					foreach(Dimension en3 in ds3 )
					{
						Rpt3DCell cell = this.DataCells3D.Get3DCell(en1.No,en2.No,en3.No);
						if (cell==null||cell.Value==null||cell.Value.Equals(DBNull.Value))
						{}
						else if(float.Parse( cell.Value.ToString())==0)
						{}
						else
						{
							sum += float.Parse( cell.Value.ToString());
						}
					}
					if(this.D3RefD2)
						this.D3Ens.Add("sum",sumTitle,en2.No,true);
					else
						this.D3Ens.Add("sum",sumTitle,"",true);
					this.DataCells3D.Add( en1.No,en2.No,"sum",sum,this.CellUrl,this.CellTarget);
				}
			}
			#endregion 增加合计
		}

		public Dimensions GetD3EnsBy2FK(string en2No)
		{
			if(!D3RefD2)
			{
				return this.D3Ens ;
			}
			else
			{
				Dimensions refds = new Dimensions();
				foreach(Dimension en3 in this.D3Ens)
				{
					if (en3 != null && en3.FK==en2No)
					{
						refds.Add( en3 ,true);
					}
				}
				return refds;
			}
		}
		
		
		public object GetCellValue(string pk1 ,string pk2 ,string pk3)
		{
			Rpt3DCell cell = this.DataCells3D.Get3DCell(pk1 ,pk2 ,pk3);
			if( cell==null)
				return null;
			else
				return cell.Value;
		}
		public float GetCellValueToFloat(string pk1 ,string pk2 ,string pk3)
		{
			object val = GetCellValue(pk1 ,pk2 ,pk3);
			if( val==null||val.Equals(DBNull.Value)||val.ToString().Trim()=="")
				return 0;
			else
			{
				return float.Parse( val.ToString());
			}
		}


		public string   Get3DCellContent(string pk1,string name1, string pk2,string name2, string pk3,string name3)
		{
			Rpt3DCell cell = this.DataCells3D.Get3DCell(pk1,pk2,pk3);
			if (cell==null||cell.Value==null||cell.Value.Equals(DBNull.Value))
				return "-";
			else if(cell.Url=="" 
				|| float.Parse( cell.Value.ToString())==0
				|| pk1=="sum"||pk2=="sum"||pk3=="sum"
				)
				return cell.Value.ToString();
			else
				return "<a href='"+cell.Url+"?"
					+  this.D1EnsName +"="+cell.PK1
					+"&"+ this.D2EnsName +"="+cell.PK2
					+"&"+ this.D3EnsName +"="+cell.PK3
					+ CellAddHref
					+"' title='"+name1+","+name2+","+name3+"'"
					+" Target='"+ cell.Target +"'>"
					+cell.Value+"</a>";
		}


		#endregion 
	
	}
}