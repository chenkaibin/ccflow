using System;
using System.Text;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
using BP.Web;

namespace BP.Sys
{
    public enum EventDoType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable,
        /// <summary>
        /// 执行存储过程
        /// </summary>
        SP,
        /// <summary>
        /// 运行SQL
        /// </summary>
        SQL,
        /// <summary>
        /// 自定义URL
        /// </summary>
        URLOfSelf,
        /// <summary>
        /// 系统的url.
        /// </summary>
        URLOfSystem,
        /// <summary>
        /// 自定义WS
        /// </summary>
        WSOfSelf,
        /// <summary>
        /// 系统的WS
        /// </summary>
        WSOfSystem,
        /// <summary>
        /// EXE
        /// </summary>
        EXE,
        /// <summary>
        /// JS
        /// </summary>
        Javascript
    }
    public class FrmEventList
    {
        /// <summary>
        /// 载入前
        /// </summary>
        public const string FrmLoadBefore = "FrmLoadBefore";
        /// <summary>
        /// 载入后
        /// </summary>
        public const string FrmLoadAfter = "FrmLoadAfter";
        /// <summary>
        /// 保存前
        /// </summary>
        public const string SaveBefore = "SaveBefore";
        /// <summary>
        /// 保存后
        /// </summary>
        public const string SaveAfter = "SaveAfter";
    }
    /// <summary>
    /// 事件标记列表
    /// </summary>
    public class EventListOfNode : FrmEventList
    {
        #region 节点事件
        /// <summary>
        /// 节点发送前
        /// </summary>
        public const string SendWhen = "SendWhen";
        /// <summary>
        /// 节点发送成功
        /// </summary>
        public const string SendSuccess = "SendSuccess";
        /// <summary>
        /// 节点发送失败
        /// </summary>
        public const string SendError = "SendError";
        /// <summary>
        /// 当节点退回前
        /// </summary>
        public const string ReturnBefore = "ReturnBefore";
        /// <summary>
        /// 当节点退后
        /// </summary>
        public const string ReturnAfter = "ReturnAfter";
        /// <summary>
        /// 当节点撤销发送前
        /// </summary>
        public const string UndoneBefore = "UndoneBefore";
        /// <summary>
        /// 当节点撤销发送后
        /// </summary>
        public const string UndoneAfter = "UndoneAfter";
        #endregion 节点事件

        #region 流程事件
        /// <summary>
        /// 流程完成时
        /// </summary>
        public const string WhenFlowOver = "WhenFlowOver";
        /// <summary>
        /// 流程删除前
        /// </summary>
        public const string BeforeFlowDel = "BeforeFlowDel";
        /// <summary>
        /// 流程删除后
        /// </summary>
        public const string AfterFlowDel = "AfterFlowDel";
        #endregion 流程事件
    }
	/// <summary>
	/// 事件属性
	/// </summary>
    public class FrmEventAttr
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string FK_Event = "FK_Event";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 执行类型
        /// </summary>
        public const string DoType = "DoType";
        /// <summary>
        /// 执行内容
        /// </summary>
        public const string DoDoc = "DoDoc";
        /// <summary>
        /// 标签
        /// </summary>
        public const string MsgOK = "MsgOK";
        /// <summary>
        /// 执行错误提示
        /// </summary>
        public const string MsgError = "MsgError";
    }
	/// <summary>
	/// 事件
	/// 节点的节点保存事件有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
    public class FrmEvent : EntityMyPK
    {
        #region 基本属性
        public override En.UAC HisUAC
        {
            get
            {
                UAC uac = new En.UAC();
                uac.IsAdjunct = false;
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_MapData, value);
            }
        }
        public string DoDoc
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.DoDoc).Replace("~", "'");
            }
            set
            {
                string doc = value.Replace("'", "~");
                this.SetValByKey(FrmEventAttr.DoDoc, doc);
            }
        }
        /// <summary>
        /// 执行成功提示
        /// </summary>
        public string MsgOK(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgOK);
            if (val.Trim() == "")
                return "";

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }
        public string MsgOKString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgOK);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgOK, value);
            }
        }
        public string MsgErrorString
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.MsgError);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.MsgError, value);
            }
        }
        /// <summary>
        /// 错误或异常提示
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public string MsgError(Entity en)
        {
            string val = this.GetValStringByKey(FrmEventAttr.MsgError);
            if (val.Trim() == "")
                return null;

            if (val.IndexOf('@') == -1)
                return val;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                val = val.Replace("@" + attr.Key, en.GetValStringByKey(attr.Key));
            }
            return val;
        }

        public string FK_Event
        {
            get
            {
                return this.GetValStringByKey(FrmEventAttr.FK_Event);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.FK_Event, value);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public EventDoType HisDoType
        {
            get
            {
                return (EventDoType)this.GetValIntByKey(FrmEventAttr.DoType);
            }
            set
            {
                this.SetValByKey(FrmEventAttr.DoType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvent()
        {
        }
        public FrmEvent(string mypk)
        {
            this.MyPK = mypk;
            this.RetrieveFromDBSources();
        }
        public FrmEvent(string fk_mapdata, string fk_Event)
        {
            this.FK_Event = fk_Event;
            this.FK_MapData = fk_mapdata;
            this.MyPK = this.FK_MapData + "_" + this.FK_Event;
            this.RetrieveFromDBSources();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FrmEvent");
                map.EnDesc = "事件";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.AddMyPK();

                map.AddTBString(FrmEventAttr.FK_Event, null, "事件名称", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.FK_MapData, null, "FK_MapData", true, true, 0, 400, 10);

                map.AddTBInt(FrmEventAttr.DoType, 0, "事件类型", true, true);
                map.AddTBString(FrmEventAttr.DoDoc, null, "执行内容", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.MsgOK, null, "成功执行提示", true, true, 0, 400, 10);
                map.AddTBString(FrmEventAttr.MsgError, null, "异常信息提示", true, true, 0, 400, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.FK_Event;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
	/// 事件
	/// </summary>
    public class FrmEvents : EntitiesOID
    {
        public string DoEventNode(string dotype, Entity en)
        {
           return DoEventNode(dotype,en,null);
        }
        public string DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;
            return _DoEventNode(dotype, en, atPara);
        }

        /// <summary>
        /// 执行事件，事件标记是 EventList.
        /// </summary>
        /// <param name="dotype">执行类型</param>
        /// <param name="en">数据实体</param>
        /// <param name="atPara">特殊的参数格式@key=value 方式.</param>
        /// <returns></returns>
        private string _DoEventNode(string dotype, Entity en, string atPara)
        {
            if (this.Count == 0)
                return null;

            FrmEvent nev = this.GetEntityByKey(FrmEventAttr.FK_Event, dotype) as FrmEvent;
            if (nev == null || nev.HisDoType == EventDoType.Disable)
                return null;

             
            string doc = nev.DoDoc.Trim();
            if (doc == null || doc == "")
                return null;

            #region 处理执行内容
            Attrs attrs = en.EnMap.Attrs;
            string MsgOK = "";
            string MsgErr = "";
            foreach (Attr attr in attrs)
            {
                if (doc.Contains("@" + attr.Key) == false)
                    continue;
                if (attr.MyDataType == DataType.AppString
                    || attr.MyDataType == DataType.AppDateTime
                    || attr.MyDataType == DataType.AppDate)
                    doc = doc.Replace("@" + attr.Key, "'" + en.GetValStrByKey(attr.Key) + "'");
                else
                    doc = doc.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
            }

            doc = doc.Replace("~", "'");
            doc = doc.Replace("@WebUser.No", BP.Web.WebUser.No);
            doc = doc.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            doc = doc.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
            doc = doc.Replace("@FK_Node", nev.FK_MapData.Replace("ND", ""));
            doc = doc.Replace("@FK_MapData", nev.FK_MapData);

            if (System.Web.HttpContext.Current != null)
            {
                /*如果是 bs 系统, 有可能参数来自于url ,就用url的参数替换它们 .*/
                string url = System.Web.HttpContext.Current.Request.RawUrl;
                if (url.IndexOf('?') != -1)
                    url = url.Substring(url.IndexOf('?'));

                string[] paras = url.Split('&');
                foreach (string s in paras)
                {
                    if (doc.Contains("@" + s) == false)
                        continue;

                    string[] mys = s.Split('=');
                    if (doc.Contains("@" + mys[0]) == false)
                        continue;
                    doc = doc.Replace("@" + mys[0], mys[1]);
                }
            }

            if (nev.HisDoType == EventDoType.URLOfSelf)
            {
                if (doc.Contains("?") == false)
                    doc += "?1=2";

                doc += "&UserNo=" + WebUser.No;
                doc += "&SID=" + WebUser.SID;
                doc += "&FK_Dept=" + WebUser.FK_Dept;
                doc += "&FK_Unit=" + WebUser.FK_Unit;
                doc += "&OID=" + en.PKVal;

                if (SystemConfig.IsBSsystem)
                {
                    /*是bs系统，并且是url参数执行类型.*/
                    string url = System.Web.HttpContext.Current.Request.RawUrl;
                    if (url.IndexOf('?') != -1)
                        url = url.Substring(url.IndexOf('?'));
                    string[] paras = url.Split('&');
                    foreach (string s in paras)
                    {
                        if (doc.Contains(s))
                            continue;
                        doc += "&" + s;
                    }
                    doc = doc.Replace("&?", "&");
                }
                if (SystemConfig.IsBSsystem == false)
                {
                    /*非bs模式下调用,比如在cs模式下调用它,它就取不到参数. */
                }

                if (doc.Contains("http") == false)
                {
                    /*如果没有绝对路径 */
                    if (SystemConfig.IsBSsystem)
                    {
                        /*在cs模式下自动获取*/
                        string host = System.Web.HttpContext.Current.Request.Url.Host;
                        if (doc.Contains("@AppPath"))
                            doc = doc.Replace("@AppPath", "http://" + host + System.Web.HttpContext.Current.Request.ApplicationPath);
                        else
                            doc = "http://" +  System.Web.HttpContext.Current.Request.Url.Authority + doc;
                    }

                    if (SystemConfig.IsBSsystem == false)
                    {
                        /*在cs模式下它的baseurl 从web.config中获取.*/
                        string cfgBaseUrl = SystemConfig.AppSettings["BaseUrl"];
                        if (string.IsNullOrEmpty(cfgBaseUrl))
                        {
                            string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                            Log.DefaultLogWriteLineError(err);
                            throw new Exception(err);
                        }
                        doc = cfgBaseUrl + doc;
                    }
                }

                //增加上系统约定的参数.
                doc += "&EntityName=" + en.ToString() + "&EntityPK=" + en.PK + "&EntityPKVal=" + en.PKVal + "&FK_Event=" + nev.MyPK;
            }
            #endregion 处理执行内容

            if (atPara != null)
            {
                AtPara ap = new AtPara(atPara);
                foreach (string s in ap.HisHT.Keys)
                    doc = doc.Replace("@" + s, ap.GetValStrByKey(s));
            }

            if (dotype == FrmEventList.FrmLoadBefore)
                en.Retrieve(); /*如果不执行，就会造成实体的数据与查询的数据不一致.*/

            switch (nev.HisDoType)
            {
                case EventDoType.SP:
                    try
                    {
                        Paras ps = new Paras();
                        DBAccess.RunSP(doc, ps);
                        return nev.MsgOK(en);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                case EventDoType.SQL:
                    try
                    {
                        // 允许执行带有GO的sql.
                        DBAccess.RunSQLs(doc);
                        return nev.MsgOK(en);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                case EventDoType.URLOfSelf:
                    string myURL = doc.Clone() as string;
                    if (myURL.Contains("http") == false)
                    {
                        if (SystemConfig.IsBSsystem)
                        {
                            string host = System.Web.HttpContext.Current.Request.Url.Host;
                            if (myURL.Contains("@AppPath"))
                                myURL = myURL.Replace("@AppPath", "http://" + host + System.Web.HttpContext.Current.Request.ApplicationPath);
                            else
                                myURL = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + myURL;
                        }
                        else
                        {
                            string cfgBaseUrl = SystemConfig.AppSettings["BaseUrl"];
                            if (string.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                                Log.DefaultLogWriteLineError(err);
                                throw new Exception(err);
                            }
                            myURL = cfgBaseUrl + myURL;
                        }
                    }

                    try
                    {
                        Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
                        string text = DataType.ReadURLContext(myURL, 8000, encode);
                        if (text == null)
                            throw new Exception("@流程设计错误，执行的URL错误:" + myURL+", 返回为null, 请检查设置是否正确。");

                        if (text != null && text.Substring(0, 7).Contains("Err"))
                            throw new Exception(text);

                        if (text == null || text.Trim() == "")
                            return null;
                        return text;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@" + nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                case EventDoType.URLOfSystem:
                    string hos1t = System.Web.HttpContext.Current.Request.Url.Host;
                    string url = "http://" + hos1t + System.Web.HttpContext.Current.Request.ApplicationPath + "/DataUser/AppCoder/FrmEventHandle.aspx";
                    url += "?FK_MapData=" + en.ClassID + "&WebUseNo=" + WebUser.No + "&EventType=" + nev.FK_Event;
                    foreach (Attr attr in attrs)
                    {
                        if (attr.UIIsDoc || attr.IsRefAttr || attr.UIIsReadonly )
                            continue;
                        url += "&" + attr.Key + "=" + en.GetValStrByKey(attr.Key);
                    }

                    try
                    {
                        string text = DataType.ReadURLContext(url, 800, System.Text.Encoding.UTF8);
                        if (text != null && text.Substring(0, 7).Contains("Err"))
                            throw new Exception(text);

                        if (text == null || text.Trim() == "")
                            return null;
                        return text;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@" + nev.MsgError(en) + " Error:" + ex.Message);
                    }
                    break;
                default:
                    throw new Exception("@no such way.");
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents() { }
        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="FK_MapData">FK_MapData</param>
        public FrmEvents(string fk_MapData)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmEventAttr.FK_MapData, fk_MapData);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmEvent();
            }
        }
    }
}
