using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Rpt;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;
using BP.Web.UC;
using BP.XML;
using BP.Sys.Xml;
using BP.Port;
using BP.Web.Comm;
using System.Collections.Specialized;

namespace CCFlow.WF.Rpt
{
    /// <summary>
    /// GroupEnsDtl 的摘要说明。
    /// </summary>
    public partial class UIContrastDtl : WebPage
    {
        #region 属性
        public string DTFrom
        {
            get
            {
                return this.Request.QueryString["DTFrom"];
            }
        }
        public string DTTo
        {
            get
            {
                return this.Request.QueryString["DTTo"];
            }
        }
        public string TBKey
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        public string FK_Flow = null;
        public string FK_Dept
        {
            get
            {
                return (string)ViewState["FK_Dept"];
            }
            set
            {
                string val = value;
                if (val == "all")
                    return;

                if (this.FK_Dept == null)
                {
                    ViewState["FK_Dept"] = value;
                    return;
                }

                if (this.FK_Dept.Length > val.Length)
                    return;

                ViewState["FK_Dept"] = value;
            }
        }
        #endregion 

        protected void Page_Load(object sender, System.EventArgs e)
        {
            #region 处理风格
            this.Page.RegisterClientScriptBlock("sds",
             "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            if (this.Request.QueryString["PageIdx"] == null)
                this.PageIdx = 1;
            else
                this.PageIdx = int.Parse(this.Request.QueryString["PageIdx"]);
            #endregion 处理风格

            this.BindData();
        }
        public void BindData()
        {
            string ensname = this.Request.QueryString["EnsName"];
            if (ensname == null)
                ensname = this.Request.QueryString["EnsName"];

            Entities ens =BP.DA.ClassFactory.GetEns(ensname);
            Entity en = ens.GetNewEntity;

            this.FK_Flow = ensname;
            this.FK_Flow = FK_Flow.Replace("Rpt", "");
            this.FK_Flow = FK_Flow.Replace("ND", "");
            this.FK_Flow = this.FK_Flow.PadLeft(3, '0');

            QueryObject qo = new QueryObject(ens);
            string[] strs = this.Request.RawUrl.Split('&');
            string[] strs1 = this.Request.RawUrl.Split('&');
            int i = 0;
            foreach (string str in strs)
            {
                if (str.IndexOf("EnsName") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }

                if (en.EnMap.Attrs.Contains(key) == false)
                    continue;

                qo.AddWhere(mykey[0], mykey[1]);
                qo.addAnd();
            }


            if (this.FK_Dept != null && (this.Request.QueryString["FK_Emp"] == null
                || this.Request.QueryString["FK_Emp"] == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        //if (this.Request.QueryString["ByLike"] != "1")
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }
            qo.AddHD();

            #region 加上日期时间段.
            if (en.EnMap.DTSearchWay != DTSearchWay.None)
            {
                string field = en.EnMap.DTSearchKey;
                qo.addAnd();
                qo.addLeftBracket();
                if (en.EnMap.DTSearchWay == DTSearchWay.ByDate)
                {
                    qo.AddWhere(field, " >= ", this.DTFrom + " 01:01");
                    qo.addAnd();
                    qo.AddWhere(field, " >= ", this.DTTo + " 23:59");
                }
                else
                {
                    qo.AddWhere(field, " >= ", this.DTFrom);
                    qo.addAnd();
                    qo.AddWhere(field, " >= ", this.DTTo);
                }
                qo.addRightBracket();
            }
            #endregion


            int num = qo.DoQuery();
            this.DataPanelDtl(ens, null);
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        public void DataPanelDtl(Entities ens, string ctrlId)
        {
         //   this.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            MapData md =new MapData(this.EnsName);

            #region 求出可显示的属性。
            Attrs selectedAttrs = new Attrs();
            string attrKeyShow = md.AttrsInTable;
            string[] strs = md.AttrsInTable.Split('@');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                string[] kv = str.Split('=');
                if (kv[0] == "MyNum")
                    continue;

                selectedAttrs.Add(myen.EnMap.GetAttrByKey(kv[0]));
            }
            #endregion 求出可显示的属性.

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft(myen.EnMap.EnDesc + " 记录：" + ens.Count + "条");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("标题");
            foreach (Attr attrT in selectedAttrs)
            {
                if (attrT.UIVisible == false)
                    continue;

                if (attrT.Key == "MyNum")
                    continue;
                this.Pub1.AddTDTitle(attrT.Desc);
            }

            bool isRefFunc = false;
            isRefFunc = true;
            int pageidx = this.PageIdx - 1;
            int idx =BP.SystemConfig.PageSize * pageidx;
            this.Pub1.AddTREnd();
            string style = WebUser.Style;
            foreach (Entity en in ens)
            {
                this.Pub1.AddTR();
                idx++;
                this.Pub1.AddTDIdx(idx);
                this.Pub1.Add("<TD class='TD'><a href=\"javascript:WinOpen('../WorkOpt/OneWork/Track.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") + "');\" ><img src='../Img/Track.png' border=0 />"+en.GetValStrByKey("Title")+"</a></TD>");
                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.UIVisible == false || attr.Key=="Title" || attr.Key == "MyNum")
                        continue;
                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        if (attr.UIBindKey == "BP.Pub.NYs")
                            this.Pub1.AddTD(en.GetValStrByKey(attr.Key));
                        else
                            this.Pub1.AddTD(en.GetValRefTextByKey(attr.Key));
                        continue;
                    }

                    if (attr.UIHeight != 0)
                    {
                        this.Pub1.AddTDDoc("...", "...");
                        continue;
                    }

                    string str = en.GetValStrByKey(attr.Key);
                    switch (attr.MyDataType)
                    {
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            if (str == "" || str == null)
                                str = "&nbsp;";
                            this.Pub1.AddTD(str);
                            break;
                        case DataType.AppString:
                            if (str == "" || str == null)
                                str = "&nbsp;";
                            if (attr.UIHeight != 0)
                                this.Pub1.AddTDDoc(str, str);
                            else
                                this.Pub1.AddTD(str);
                            break;
                        case DataType.AppBoolean:
                            if (str == "1")
                                this.Pub1.AddTD("是");
                            else
                                this.Pub1.AddTD("否");
                            break;
                        case DataType.AppFloat:
                        case DataType.AppInt:
                        case DataType.AppRate:
                        case DataType.AppDouble:
                            this.Pub1.AddTDNum(str);
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDNum(decimal.Parse(str).ToString("0.00"));
                            break;
                        default:
                            throw new Exception("no this case ...");
                    }
                }
                this.Pub1.AddTREnd();
            }
            #region  求合计代码写在这里。

            bool IsHJ = false;
            foreach (Attr attr in selectedAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText || attr.UIContralType == UIContralType.DDL )
                    continue;
                if (attr.Key == "OID" || attr.Key == "FID"
                    || attr.Key == "MID" || attr.Key.ToUpper() == "WORKID")
                    continue;
                switch (attr.MyDataType)
                {
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppMoney:
                        IsHJ = true;
                        break;
                    default:
                        break;
                }
            }
            if (IsHJ)
            {
                // 找出配置是不显示合计的列。
                this.Pub1.Add("<TR class='TRSum'>");
                this.Pub1.AddTD("class=Sum", "合计");
                this.Pub1.AddTD("class=Sum", "");
                foreach (Attr attr in selectedAttrs)
                {
                    if (attr.MyFieldType == FieldType.RefText
                        || attr.UIVisible == false
                        || attr.Key == "MyNum")
                        continue;

                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        this.Pub1.AddTD("class=Sum", "");
                        continue;
                    }

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        this.Pub1.AddTD("class=Sum", "");
                        continue;
                    }

                    if (attr.Key == "OID" || attr.Key == "FID"
                        || attr.Key == "MID" || attr.Key.ToUpper() == "WORKID")
                    {
                        this.Pub1.AddTD("class=Sum", "");
                        continue;
                    }

                    switch (attr.MyDataType)
                    {
                        case DataType.AppDouble:
                            this.Pub1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                            this.Pub1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            this.Pub1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDJE(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        default:
                            this.Pub1.AddTD("class=Sum", "");
                            break;
                    }
                }
                this.Pub1.AddTREnd();
            }
            #endregion

            this.Pub1.AddTableEnd();
        }
        private void DataPanelDtlAdd(Entity en, Attr attr, BP.Sys.Xml.PanelEnss cfgs, string url, string cardUrl, string focusField)
        {
            string cfgurl = "";
            if (attr.UIContralType == UIContralType.DDL)
            {
                this.Pub1.AddTD(en.GetValRefTextByKey(attr.Key));
                return;
            }

            if (attr.UIHeight != 0)
            {
                this.Pub1.AddTDDoc("...", "...");
                return;
            }

            string str = en.GetValStrByKey(attr.Key);

            if (focusField == attr.Key)
                str = "<a href=" + cardUrl + ">" + str + "</a>";

            switch (attr.MyDataType)
            {
                case DataType.AppDate:
                case DataType.AppDateTime:
                    if (str == "" || str == null)
                        str = "&nbsp;";
                    this.Pub1.AddTD(str);
                    break;
                case DataType.AppString:
                    if (str == "" || str == null)
                        str = "&nbsp;";

                    if (attr.UIHeight != 0)
                    {
                        this.Pub1.AddTDDoc(str, str);
                    }
                    else
                    {
                        if (attr.Key.IndexOf("ail") == -1)
                            this.Pub1.AddTD(str);
                        else
                            this.Pub1.AddTD("<a href=\"javascript:mailto:" + str + "\"' >" + str + "</a>");
                    }
                    break;
                case DataType.AppBoolean:
                    if (str == "1")
                        this.Pub1.AddTD("是");
                    else
                        this.Pub1.AddTD("否");
                    break;
                case DataType.AppFloat:
                case DataType.AppInt:
                case DataType.AppRate:
                case DataType.AppDouble:
                    foreach (BP.Sys.Xml.PanelEns pe in cfgs)
                    {
                        if (pe.Attr == attr.Key)
                        {
                            cfgurl = pe.URL;
                            Attrs attrs = en.EnMap.Attrs;
                            foreach (Attr attr1 in attrs)
                                cfgurl = cfgurl.Replace("@" + attr1.Key, en.GetValStringByKey(attr1.Key));
                            break;
                        }
                    }
                    if (cfgurl == "")
                    {
                        this.Pub1.AddTDNum(str);
                    }
                    else
                    {
                        cfgurl = cfgurl.Replace("@Keys", url);
                        this.Pub1.AddTDNum("<a href=\"javascript:WinOpen('" + cfgurl + "','dtl1');\" >" + str + "</a>");
                    }
                    break;
                case DataType.AppMoney:
                    cfgurl = "";
                    foreach (BP.Sys.Xml.PanelEns pe in cfgs)
                    {
                        if (pe.Attr == attr.Key)
                        {
                            cfgurl = pe.URL;
                            Attrs attrs = en.EnMap.Attrs;
                            foreach (Attr attr2 in attrs)
                                cfgurl = cfgurl.Replace("@" + attr2.Key, en.GetValStringByKey(attr2.Key));
                            break;
                        }
                    }
                    if (cfgurl == "")
                    {
                        this.Pub1.AddTDNum(decimal.Parse(str).ToString("0.00"));
                    }
                    else
                    {
                        cfgurl = cfgurl.Replace("@Keys", url);

                        this.Pub1.AddTDNum("<a href=\"javascript:WinOpen('" + cfgurl + "','dtl1');\" >" + decimal.Parse(str).ToString("0.00") + "</a>");
                    }
                    break;
                default:
                    throw new Exception("no this case ...");
            }
        }
    }
}
