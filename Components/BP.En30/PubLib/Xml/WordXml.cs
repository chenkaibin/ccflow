using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;

namespace BP.Pub.Xml
{
	/// <summary>
    /// 字体属性
	/// </summary>
    public class WordXmlAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
    }
	/// <summary>
	/// 字体
	/// </summary>
	public class WordXml:XmlEn
	{
		#region 属性
        /// <summary>
        /// 编号
        /// </summary>
		public string No
		{
			get
			{
				return this.GetValStringByKey(WordXmlAttr.No);
			}
		}
        /// <summary>
        /// 名称
        /// </summary>
		public string Name
		{
			get
			{
				return this.GetValStringByKey(WordXmlAttr.Name);
			}
		}
		#endregion

		#region 构造
        /// <summary>
        /// 字体
        /// </summary>
		public WordXml()
		{
		}
        /// <summary>
        /// 字体
        /// </summary>
        /// <param name="no">编号</param>
		public WordXml(string no)
		{
            this.RetrieveByPK(WordXmlAttr.No, no);
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new WordXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 字体s
	/// </summary>
    public class WordXmls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 字体s
        /// </summary>
        public WordXmls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new WordXml();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\Sys\\Word.xml";
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
