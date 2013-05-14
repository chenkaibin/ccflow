using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Data;
using BP.En;
using BP.DA;


//
namespace BP.Report
{
	/// <summary>
	/// 单据打印处理 
	/// </summary>
	public class RrfReport
	{
		#region 构造函数
		public RrfReport()
		{
			BaseDir =  AppDomain.CurrentDomain.BaseDirectory;
            CyclostyleDir = "DataUser/CyclostyleFile";
			DirectoryInfo dir = Directory.CreateDirectory( BaseDir + CyclostyleDir);
			TempDir       = "Temp";
			dir = Directory.CreateDirectory( BaseDir + TempDir);

			_encoder = System.Text.Encoding.GetEncoding("GB2312") ;
		}
		#endregion


		#region 属性－路径

		/// <summary>
		/// 根目录
		/// </summary>
		public readonly string BaseDir ;
		/// <summary>
		/// 模板目录
		/// </summary>
		public readonly string CyclostyleDir ;
		/// <summary>
		/// 临时文件目录
		/// </summary>
		public readonly string TempDir ="Temp";

		private  string _cyclostyleFilePath = "" ;
		/// <summary>
		/// 模板路径
		/// </summary>
		public  string CyclostyleFilePath 
		{
			get
			{
				return _cyclostyleFilePath;
			}
		}
		private  string _tempFilePath = "" ;
		/// <summary>
		/// 临时文件路径
		/// </summary>
		public  string TempFilePath 
		{
			get
			{
				return _tempFilePath;
			}
		}
		
		private string _fileName ="";
		/// <summary>
		/// 不包含路径的文件名
		/// </summary>
		public string FileName
		{
			get
			{
				return _fileName;
			}
		}
		
		#endregion 属性－路径

		/// <summary>
		/// 产生一个报表
		/// </summary>
		/// <param name="ens">要产生报表的集合</param>
		/// <param name="cyclostyfilename">参数文件名称。</param>
		public static void GenerRpt(Entities ens, string cyclostyfilename)
		{
			BP.WF.WebRtfReport rpt = new BP.WF.WebRtfReport();
			rpt.LoadCyclostyle( cyclostyfilename , DA.DBAccess.GenerOID()+".doc" );
			rpt.BindEnInEns( ens );
			rpt.Close();
			string url=   System.Web.HttpContext.Current.Request.ApplicationPath+"/"+ rpt.TempDir +"/"+ rpt.FileName ; 
			PubClass.WinOpen(url);
			return;
			//PubClass.OpenWordDoc(url,rpt.FileName);
		}
		/// <summary>
		/// 产生一个报表
		/// </summary>
		/// <param name="ens">要产生报表的集合</param>
		/// <param name="cyclostyfilename">参数文件名称。</param>
		public static void GenerRpt(Entity en, string cyclostyfilename)
		{
			BP.WF.WebRtfReport rpt = new BP.WF.WebRtfReport();
			rpt.LoadCyclostyle( cyclostyfilename , DA.DBAccess.GenerOID()+".Doc" );
			rpt.BindEn( en ); 
			rpt.Close();
			string url=   System.Web.HttpContext.Current.Request.ApplicationPath+"/"+ rpt.TempDir +"/"+ rpt.FileName ; 
			
			PubClass.WinOpen(url);
			return;
		//	PubClass.OpenWordDoc(url,rpt.FileName);
		}

		#region 文档加载保存关闭
		/// <summary>
		/// 加载Rtf模板
		/// </summary>
		/// <param name="cyclostyfilename">模板文件名</param>
		/// <param name="saveFileName">临时文件名</param>
		public void LoadCyclostyle( string cyclostyfilename ,string saveFileName)
		{
			this._cyclostyleFilePath = BaseDir+CyclostyleDir+"/" + cyclostyfilename.TrimStart('\\','/').ToUpper().Replace(".RTF","") +".RTF";
			this._fileName           = saveFileName.TrimStart('\\','/').ToUpper().Replace(".RTF","") +".RTF";
			this._tempFilePath       = BaseDir+TempDir+"/"       +  _fileName;
			//System.IO.File.Copy( this.CyclostyleFilePath , this.TempFilePath );
			try
			{
				ReadDocParamTable();
			}
			catch(Exception ex)
			{
				throw new Exception("参数表填写有误！ "+ ex.Message + _cyclostyleFilePath );
			}
		}
		public DocParams DocParamTable = null;
		
		/// <summary>
		/// 装载参数表
		/// </summary>
		private void ReadDocParamTable()
		{
			string xlmpath = this.CyclostyleFilePath.ToUpper().Replace(".RTF" ,".XML");
			DataSet ds = new DataSet("ds");
			ds.ReadXml( xlmpath);
			DataTable tb = ds.Tables[0];
			this.DocParamTable = new DocParams( tb ); //根据datatable 生成参数集合。

			this.ListDT = new Hashtable(); // 明细类名表
			this.ListEnsNames = new Hashtable(); // 类名列表
			foreach(DocParam par in this.DocParamTable)
			{
				int pos = par.Field.IndexOf('.');
				if( pos!=-1)
				{
					string clas = par.Field.Substring(0,pos);
					if(ListEnsNames.ContainsKey( clas)==false)
					{
						ListEnsNames.Add( clas ,clas); // 加入类名列表。
					}
					if(par.Key.TrimStart('<').Substring(0,2)=="DT")
					{
						/*  dtl */
						if(this.ContainsDT( clas)==false)  // 加入明细类名表
						{
							this.ListDT.Add( clas ,0);
						}
					}
				}
			}
			ds.Dispose();
		}
		public  Hashtable ListEnsNames;
		public  Hashtable ListDT;
		public  bool ContainsClass(string enName)
		{
			return this.ListEnsNames.ContainsKey( enName);
		}
		public  bool ContainsDT(string ensName)
		{
			return this.ListDT.ContainsKey( ensName);
		}
		/// <summary>
		/// 取得参数
		/// </summary>
		/// <param name="enName">类名称</param>
		/// <returns>参数集合</returns>
		public DocParams GetDocParams(string enName)
		{
			DocParams pars = new DocParams();
			foreach(DocParam par in this.DocParamTable)
			{
				if(par.Field.IndexOf(enName)!=-1)
				{
					pars.Add( par);
				}
			}
			return pars;
		}
		/// <summary>
		/// 生成文档。
		/// </summary>
		public  void MakeDocument()
		{
			System.IO.File.Copy( this.CyclostyleFilePath , this.TempFilePath ); //复制一个样本到临时目录里面。

			StreamReader read = null;
			StreamWriter wr = null;
			try
			{
				read= new StreamReader( this.TempFilePath ,Encoding.ASCII); // 文件流
				string str = read.ReadToEnd();  //读取完毕。
				read.Close(); // 关闭。
				wr = new StreamWriter( this.TempFilePath ,false,Encoding.ASCII);

				str= this.RepairLine(   str ); // 修复线。

				#region 从表
				IDictionaryEnumerator dic= this.ListDT.GetEnumerator();
				 

				while(dic.MoveNext())
				{
					string ensName = dic.Key.ToString(); //DT
					DocParams pars = this.GetDocParams( ensName);
					int rowCount = (int)dic.Value;
					if(pars.Count>0)
					{
						string rowKey  = pars[0].Key.TrimStart('<').Substring(0,4); //find dt row model
						int pos_rowKey = str.IndexOf( rowKey);
						int row_start=-1 ,row_end=-1 ;
						if(pos_rowKey!=-1)
						{
							row_start = str.Substring(0,pos_rowKey).LastIndexOf("\\row");
							row_end = str.Substring(pos_rowKey).IndexOf("\\row");
						}
						if(row_start!=-1 && row_end!=-1 )
						{
							string row = str.Substring(row_start ,(pos_rowKey-row_start)+row_end);
							str = str.Replace( row,"");
							for(int ir=rowCount;ir>0;ir--)
							{
								string tmp = row;
								pars = this.GetDocParams(ensName+ir);
								foreach(DocParam par in pars)
								{
									string key ;
									if(par.Key[par.Key.Length-1]=='>')
										key= par.Key.Remove(par.Key.Length-4,3);
									else
										key= par.Key.Remove(par.Key.Length-3,3);
									if(key.Length>3) //安全起见
										tmp = tmp.Replace( key , par.Value );
								}
								str = str.Insert(row_start,tmp);
							}
						}
					}
				}
				#endregion 从表

				#region 主表
				foreach( DocParam par in this.DocParamTable )
				{
					str = str.Replace( par.Key , par.Value );
				}
				#endregion

				wr.Write( str ); 
			}
			catch( Exception ex)
			{
				if( read!=null)
					read.Close();
				if( wr!=null)
					wr.Close();
				throw new Exception( "生成文档失败："+ex.Message); 
			}
			read.Close();
			wr.Close();
		}
		
		
		public  void Close()
		{
			this.MakeDocument();
			this.ListEnsNames.Clear();
			this.ListDT.Clear();
			this.DocParamTable.Clear();
		}

		#endregion 文档加载保存关闭

		#region 关键方法
		private System.Text.Encoding _encoder;
		/// <summary>
		/// 把string 转换rpt 可以识别的
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string GetCode(string str)
		{
			if( str =="")
				return str;
			string rtn = "";
			byte[] rr = _encoder.GetBytes( str );
			foreach( byte b in rr )
			{
				if( b>122)
					rtn += "\\'" + b.ToString("x");
				else
					rtn += (char)b;
			}
			return rtn ;
		}
		/// <summary>
		/// 修复线
		/// </summary>
		/// <param name="line"></param>
		public string RepairLine(string line)//str
		{
			int start = line.IndexOf("<");
			int end   = line.IndexOf(">");
			while(start!=-1 && end>start )
			{
				#region 处理 <parm> 中的内容
				string pro_old = "";
				pro_old = line.Substring(start,end-start );

				string str_new = pro_old.Replace("{","");
				str_new = str_new.Replace("}","");
				if(str_new.Length>0)
				{
					int _xie_start = 0;   //反斜杠开始搜索位置
					int _xie=-1,_space=-1;//反斜杠位置，空格位置
					_xie = str_new.IndexOf( '\\',_xie_start);
					while(_xie!=-1)
					{
						#region 处理 \…… 内容
						if(str_new.Length>(_xie+1) && str_new[_xie+1]=='\'')
						{
							_xie_start = _xie+1;
							_xie = str_new.IndexOf( '\\' ,_xie_start);
							continue;
						}
						_space = str_new.IndexOf(" ",_xie);
						if(_space!=-1)
						{
							str_new = str_new.Replace(str_new.Substring(_xie,_space-_xie),"");
						}
						_xie = str_new.IndexOf( '\\',_xie_start);
						#endregion 处理 \…… 内容
					}
					str_new = str_new.Replace(" ","");
				}
				line = line.Replace(pro_old ,str_new);
				start = end -(pro_old.Length-str_new.Length)+1;
				end=-1;
				if( line.Length>start)
				{
					end   = line.IndexOf(">" ,start);
					start = line.IndexOf("<" ,start);
				}
				#endregion 处理 <parm> 中的内容
			}
			return line;

		}


		#endregion 


		#region 参数替换
		/// <summary>
		///  SetValueByField
		/// </summary>
		/// <param name="field">字段名</param>
		/// <param name="val">要替换的值</param>
		public void SetValueByField( string field , string val)
		{
			DocParam par = this.DocParamTable.GetDocParamByField( field);

			if(par!=null )
			{
				if( par.NeedConvert ) 
					par.Value = this.GetCode( val );
				else
					par.Value =  val ;
			}
		}
		/// <summary>
		/// SetValueByField
		/// </summary>
		/// <param name="field"></param>
		/// <param name="val"></param>
		/// <param name="convertCode"></param>
		public void SetValueByField( string field , string val , bool convertCode)
		{
			DocParam par = this.DocParamTable.GetDocParamByField( field);
			if(par!=null )
			{
				if( convertCode ) 
					par.Value = this.GetCode( val );
				else
					par.Value =  val ;
			}
		}

		#endregion 参数替换

	
	}

}
