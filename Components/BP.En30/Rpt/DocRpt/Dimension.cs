using System;
using System.Data;
using System.Collections;
using BP.DA;

namespace BP.Rpt.Doc
{
	/// <summary>
	/// 维度元素
	/// </summary>
	public class Dimension 
	{
		public Dimension()
		{
		}
		/// <summary>
		/// 构造维度元素
		/// </summary>
		/// <param name="no">编码</param>
		/// <param name="name">名称</param>
		/// <param name="fk">外键，没有则用""</param>
		public Dimension(string no ,string name ,string fk)
		{
			this.No = no;
			this.Name = name;
			this.FK = fk;
		}
		private string _no = "";
		/// <summary>
		/// 编码
		/// </summary>
		public string No
		{
			get{ return _no;}
			set{ _no =value;}
		}
		private string _name = "";
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get{ return _name;}
			set{ _name =value;}
		}
		private string _fk = "";
		/// <summary>
		/// 外键
		/// </summary>
		public string FK
		{
			get{ return (_fk==null)?"":_fk;}
			set{ _fk =value;}
		}
	}

	/// <summary>
	/// 维度元素集合，用于报表上某一维上的元素
	/// </summary>
	public class Dimensions :CollectionBase
	{
 
		#region 公共方法
		public  void BindDataCols3(DataView dv)
		{
			this.Clear();
			foreach(DataRowView dr in dv)	
			{
				this.Add( dr[0].ToString().Trim()
					,dr[1].ToString().Trim()
					,dr[2].ToString().Trim()
					,true) ; 
			}
		}
		public  void BindDataCols2(DataView dv)
		{
			this.Clear();
			foreach(DataRowView dr in dv)	
			{
				this.Add( dr[0].ToString().Trim()
					,dr[1].ToString().Trim()
					,""
					,true) ; 
			}
		}
		public  void BindDataEns(CollectionBase ens 
			,string noKey
			,string nameKey
			,string fkKey)
		{
			this.Clear();
			foreach(object en in ens)
			{
				if(fkKey==null||fkKey=="")
				{
					this.Add( ClassFactory.GetValueToStr(en,noKey)
						,ClassFactory.GetValueToStr(en,nameKey)
						,""
						,true) ; 
				}
				else
				{
					this.Add( ClassFactory.GetValueToStr(en,noKey)
						,ClassFactory.GetValueToStr(en,nameKey)
						,ClassFactory.GetValueToStr(en,fkKey)
						,true) ; 
				}
			}
		}


		public bool ContainsDim( Dimension dim,bool ignoreCase)
		{
			return ContainsDim( dim.No ,dim.FK ,ignoreCase);
		}
		public bool ContainsDim(string no ,string fk ,bool ignoreCase)
		{
			foreach(Dimension dim in this)
			{
				if (string.Compare(dim.No,no,ignoreCase)==0 &&string.Compare(dim.FK,fk,ignoreCase)==0)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 通过编码及外键获取维度元素
		/// </summary>
		/// <param name="no">编码</param>
		/// <param name="fk">外键，没有外键时为""</param>
		/// <param name="ignoreCase">是否忽略大小写</param>
		/// <returns>返回维度元素</returns>
		public Dimension GetDim(string no ,string fk ,bool ignoreCase)
		{
			foreach(Dimension dim in this)		
			{
				if (string.Compare(dim.No,no,ignoreCase)==0 &&string.Compare(dim.FK,fk,ignoreCase)==0)
				{
					return dim;
				}
			}
			return null;
		}


		/// <summary>
		/// 添加维度元素
		/// </summary>
		/// <param name="no">编码</param>
		/// <param name="name">名称</param>
		/// <param name="fk">外键，""表示空</param>
		/// <param name="ignoreCase">为true时表示编码不区分大小写</param>
		/// <returns>添加不成功表示已经存在相同的元素（以外键＋编码为主键）</returns>
		public bool Add(string no,string name,string fk ,bool ignoreCase)
		{
			if( this.ContainsDim( no ,fk ,ignoreCase))
				return false;
			else
			{
				Dimension dim = new Dimension(no ,name ,fk);
				this.InnerList.Add(dim);
				return true;
			}
		}
		/// <summary>
		/// 添加维度元素
		/// </summary>
		/// <param name="add">维度元素</param>
		/// <param name="ignoreCase">为true时表示编码不区分大小写</param>
		/// <returns>添加不成功表示已经存在相同的元素（以外键＋编码为主键）</returns>
		public bool Add(Dimension add ,bool ignoreCase)
		{
			if( this.ContainsDim(add , ignoreCase))
				return false;
			else
			{
				this.InnerList.Add(add);
				return true;
			}
		}


		public Dimension this[int index]
		{
			get
			{	
				return this.InnerList[index] as Dimension;
			}
		}


		public string GenerHtmlStrBy(string pk1, string pk2 )
		{
			return "";
		}


		/// <summary>
		/// 修改维度集合中的所有元素的名称，去掉名称中开始位置的某些字符，参考 string.TrimStart(chars)，如去掉“01月份”中的“0”
		/// </summary>
		/// <param name="chars">要去掉的字符</param>
		public void TrimStart(params char[] chars)
		{
			foreach(Dimension dim in this)
			{
				dim.Name = dim.Name.TrimStart(chars);
			}
		}

		/// <summary>
		/// 修改维度集合中的所有元素的名称，去掉名称中结束位置的某些字符，参考 string.TrimEnd(chars)
		/// </summary>
		/// <param name="chars">要去掉的字符</param>
		public void TrimEnd(params char[] chars)
		{
			foreach(Dimension dim in this)
			{
				dim.Name = dim.Name.TrimEnd(chars);
			}
		}
		/// <summary>
		/// 修改维度集合中的每一个元素的名称,去掉开始或结束位置的字符，参考 string.Trim()
		/// </summary>
		/// <param name="chars">要去掉的字符</param>
		public void Trim(params char[] chars)
		{
			foreach(Dimension dim in this)
			{
				dim.Name = dim.Name.Trim(chars);
			}
		}
		/// <summary>
		/// 填充指定年度的月份
		/// </summary>
		/// <param name="Year4">年号，必须为yyyy格式</param>
		public void FillMonth( string Year4)
		{
			this.Clear();
			for(int i=1; i<=12;i++)
			{
				string no = Year4 +"-"+ i.ToString().PadLeft(2,'0');
				this.Add(no,i.ToString()+"月份","",true);
			}
		}

		/// <summary>
		/// 填充指定年度月份的天数
		/// </summary>
		/// <param name="Year4">年号，必须为yyyy格式</param>
		public void FillDay( int year,int month)
		{
			this.Clear();
			int days = System.DateTime.DaysInMonth( year ,month);
			string yyyyMM = year.ToString() +"-"+ month.ToString().PadLeft(2,'0');
			for(int i=1;i<=days;i++)
			{
				string no = yyyyMM +"-"+ i.ToString().PadLeft(2,'0');
				string name = i.ToString()+"日";
				this.Add( no ,name ,"" ,true);
			}
		}


		/// <summary>
		/// 填充流程状态
		/// </summary>
		public void FillBreedState()
		{
			this.Clear();
			BP.Sys.SysEnums ses =new BP.Sys.SysEnums("WFState");
			foreach(BP.Sys.SysEnum se in ses)
			{
				this.Add(se.IntKey.ToString() ,se.Lab ,"",true);
			}
		}
		/// <summary>
		/// 填充季度
		/// </summary>
		public void FillQuarter()
		{
			this.Clear();
			BP.Sys.SysEnums ses =new BP.Sys.SysEnums("Quarter");
			foreach(BP.Sys.SysEnum se in ses)
			{
				this.Add(se.IntKey.ToString() ,se.Lab ,"",true);
			}
		}



		#endregion 
	}
}
