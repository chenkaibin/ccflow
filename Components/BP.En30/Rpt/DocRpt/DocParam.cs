using System;
using System.Data;
using System.Collections;

namespace BP.Rpt.Doc
{
	/// <summary>
	/// 单据打印参数
	/// </summary>
	public class DocParam
	{
		public DocParam()
		{
		}
		public DocParam( string key ,string field , string val ,string convert)
		{
			if( key=="")
				throw new Exception("Key不允许为空！");
			if( field=="")
				throw new Exception("Field不允许为空！");
			this.Key = "<"+ key.ToUpper() +">";
			//this.Key = key.ToUpper();
			this.Field = field;
			this.Value = val;
			if( convert !="0" )
				this.NeedConvert =true;
			else
				this.NeedConvert =false;
		}
		private string key = "null";
		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				if( value=="")
					throw new Exception("Key不允许为空！");
				key = value;
			}
		}

		private string field = "null";
		public string Field
		{
			get
			{
				return field;
			}
			set
			{
				if( value=="")
					throw new Exception("Field不允许为空！");
				field = value;
			}
		}

		private string val ="";
		public string Value
		{
			get
			{
				return val;
			}
			set
			{
				val = value;
			}
		}
		
		private bool  needConvert = true;
		public  bool  NeedConvert
		{
			get
			{
				return needConvert;
			}
			set
			{
				needConvert = value;
			}
		}
	}

	/// <summary>
	/// 单据打印参数集合
	/// </summary>
	public class DocParams : System.Collections.CollectionBase
	{
		public DocParams()
		{
		}
		public DocParams( DataTable tb )
		{
			if( tb.Columns.Count<2 )
				throw new Exception( "构造DocParams要求2列以上的数据！" );
			
			if( tb.Columns.Count==2 )
				foreach( DataRow row in tb.Rows )
				{
					DocParam par = new DocParam( row[0].ToString().Trim() ,row[1].ToString().Trim() ,"" ,"1");
					this.Add( par );
				}
			else if( tb.Columns.Count==3 )
				foreach( DataRow row in tb.Rows )
				{
					DocParam par = new DocParam( row[0].ToString().Trim() ,row[1].ToString().Trim() ,row[2].ToString().Trim() ,"1");
					this.Add( par );
				}
			else //if( tb.Columns.Count==4 )
				foreach( DataRow row in tb.Rows )
				{
					DocParam par = new DocParam( row[0].ToString().Trim() ,row[1].ToString().Trim() ,row[2].ToString().Trim() ,row[3].ToString().Trim() );
					this.Add( par );
				}
		}
		
		public void Add( DocParam par)
		{
			if( this.GetDocParamByKey( par.Key )!=null)
				throw new Exception( "不允许加入重复的["+par.Key+"]，请检查xml配置文件．");
			else if( this.GetDocParamByKey( par.Field )!=null)
				throw new Exception( "不允许加入重复的["+ par.Field+"]，请检查xml配置文件．");

			this.InnerList.Add( par );
		}
		public void Remove(DocParam par)
		{
			this.InnerList.Remove( par);
		}
		public void RemoveByKey(string key)
		{
			DocParam par = this.GetDocParamByKey( key);
			if( par!=null)
				Remove( par);
		}
		public void RemoveByField(string field)
		{
			DocParam par = this.GetDocParamByField( field);
			if( par!=null)
				Remove( par);
		}
		
		
		public DocParam GetDocParamByKey(string key)
		{
			foreach( DocParam par in this)
			{
				if( string.Compare( par.Key,key,true)==0 )
				{
					return par;
				}
			}
			return null;
		}
		public DocParam GetDocParamByField(string field)
		{
			foreach( DocParam par in this)
			{
				if( string.Compare( par.Field,field,true)==0)
				{
					return par;
				}
			}
			return null;
		}
		public DocParam this[int index]
		{
			get
			{
				return this.InnerList[ index] as DocParam;
			}
		}

	}

}
