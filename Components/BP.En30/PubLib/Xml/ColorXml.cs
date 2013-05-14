using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;


namespace BP.Pub.Xml
{
	/// <summary>
    /// 颜色属性
	/// </summary>
	public class ColorXmlAttr
	{
		/// <summary>
		/// 编号
		/// </summary>
		public const string No="No";
		/// <summary>
		/// 名称
		/// </summary>
		public const string Name="Name";
	}
	/// <summary>
	/// 颜色
	/// </summary>
	public class ColorXml:XmlEn
	{
		#region 属性
        /// <summary>
        /// 编号
        /// </summary>
		public string No
		{
			get
			{
				return this.GetValStringByKey(ColorXmlAttr.No);
			}
		}
        /// <summary>
        /// 名称
        /// </summary>
		public string Name
		{
			get
			{
				return this.GetValStringByKey(ColorXmlAttr.Name);
			}
		}
		#endregion

		#region 构造
        /// <summary>
        /// 颜色
        /// </summary>
		public ColorXml()
		{
		}
        /// <summary>
        /// 颜色
        /// </summary>
        /// <param name="no">编号</param>
		public ColorXml(string no)
		{
            this.RetrieveByPK(ColorXmlAttr.No, no);
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new ColorXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 颜色s
	/// </summary>
	public class ColorXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 颜色s
		/// </summary>
		public ColorXmls(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ColorXml();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Sys\\Color.xml";
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
