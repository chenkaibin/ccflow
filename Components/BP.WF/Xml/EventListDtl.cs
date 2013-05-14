using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 事件列表
    /// </summary>
    public class EventListDtlList
    {
        public const string DtlSaveBefore = "DtlSaveBefore";
        public const string DtlSaveEnd = "DtlSaveEnd";
        public const string DtlItemSaveBefore = "DtlItemSaveBefore";
        public const string DtlItemSaveAfter = "DtlItemSaveAfter";
        public const string DtlItemDelBefore = "DtlItemDelBefore";
        public const string DtlItemDelAfter = "DtlItemDelAfter";
    }

    /// <summary>
    /// 从表事件
    /// </summary>
    public class EventListDtl : XmlEn
    {
        #region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public string EventDesc
        {
            get
            {
                return this.GetValStringByKey("EventDesc");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 从表事件
        /// </summary>
        public EventListDtl()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EventListDtls();
            }
        }
        #endregion
    }
    /// <summary>
    /// 从表事件s
    /// </summary>
    public class EventListDtls : XmlEns
    {
        #region 构造
        /// <summary>
        /// 从表事件s
        /// </summary>
        public EventListDtls() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EventListDtl();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\EventList.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "ItemDtl";
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
