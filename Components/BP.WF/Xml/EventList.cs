using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 事件
    /// </summary>
    public class EventList : XmlEn
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
        public string EventType
        {
            get
            {
                return this.GetValStringByKey("EventType");
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 事件
        /// </summary>
        public EventList()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EventLists();
            }
        }
        #endregion
    }
    /// <summary>
    /// 事件s
    /// </summary>
    public class EventLists : XmlEns
    {
        #region 构造
        /// <summary>
        /// 事件s
        /// </summary>
        public EventLists() { }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new EventList();
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
