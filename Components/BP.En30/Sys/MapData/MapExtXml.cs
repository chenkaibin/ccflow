using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.XML;


namespace BP.Sys
{

    public class MapExtXmlList
    {
        /// <summary>
        /// 活动菜单
        /// </summary>
        public const string ActiveDDL = "ActiveDDL";
        /// <summary>
        /// 输入验证
        /// </summary>
        public const string InputCheck = "InputCheck";
        /// <summary>
        /// 文本框自动填充
        /// </summary>
        public const string TBFullCtrl = "TBFullCtrl";
        /// <summary>
        /// Pop返回值
        /// </summary>
        public const string PopVal = "PopVal";
        /// <summary>
        /// Func
        /// </summary>
        public const string Func = "Func";
        /// <summary>
        /// (动态的)填充下拉框
        /// </summary>
        public const string AutoFullDLL = "AutoFullDLL";
        /// <summary>
        /// 下拉框自动填充
        /// </summary>
        public const string DDLFullCtrl = "DDLFullCtrl";
        /// <summary>
        /// 表单装载填充
        /// </summary>
        public const string PageLoadFull = "PageLoadFull";
        /// <summary>
        /// 
        /// </summary>
        public const string StartFlow = "StartFlow";
        /// <summary>
        /// link.
        /// </summary>
        public const string Link = "Link";
    }
	public class MapExtXml:XmlEnNoName
	{
		#region 属性
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey("URL");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public MapExtXml()
		{
		}
        public MapExtXml(string no)
        {
            
        }
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new MapExtXmls();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class MapExtXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public MapExtXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new MapExtXml();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfXML + "MapExt.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "FieldExt";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
