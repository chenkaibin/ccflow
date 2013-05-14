using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;


namespace BP.Sys.Xml
{
	/// <summary>
	/// 属性
	/// </summary>
	public class PanelEnsAttr
	{
		/// <summary>
		/// 过错行为
		/// </summary>
		public const string Attr="Attr";
		/// <summary>
		/// 表达式
		/// </summary>
		public const string URL="URL";
		/// <summary>
		/// 描述
		/// </summary>
		public const string For="For";
	}
	/// <summary>
	/// PanelEns 的摘要说明。
	/// 考核过错行为的数据元素
	/// 1，它是 PanelEns 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
	public class PanelEns:XmlEn
	{

		#region 属性
		public string Attr
		{
			get
			{
				return this.GetValStringByKey(PanelEnsAttr.Attr);
			}
		}
		public string For
		{
			get
			{
				return this.GetValStringByKey(PanelEnsAttr.For);
			}
		}
		public string URL
		{
			get
			{
				return this.GetValStringByKey(PanelEnsAttr.URL);
			}
		}
		#endregion

		#region 构造
		public PanelEns()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new PanelEnss();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class PanelEnss:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public PanelEnss(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new PanelEns();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Ens\\PanelEns.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Item";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null;
			}
		}
		#endregion
		 
	}
}
