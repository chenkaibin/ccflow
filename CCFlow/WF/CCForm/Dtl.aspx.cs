using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.DA;
using BP.En;
using BP.WF.XML;
using BP.Sys;  
using BP.Web.Controls;
namespace CCFlow.WF.CCForm
{
    public partial class Comm_Dtl : WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                if (string.IsNullOrEmpty(this.Request.QueryString["FK_Node"]))
                    return 0;

                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string FK_MapExt
        {
            get
            {
                return this.Request.QueryString["FK_MapExt"];
            }
        }
        public new string Key
        {
            get
            {
                return this.Request.QueryString["Key"];
            }
        }
        public new string EnsName
        {
            get
            {
                string str = this.Request.QueryString["EnsName"];
                if (str == null)
                    return "ND299Dtl";
                return str;
            }
        }
        /// <summary>
        /// 主表FK_MapData
        /// </summary>
        public string MainEnsName
        {
            get
            {
                string str = this.Request.QueryString["MainEnsName"];
                if (str == null)
                    return "ND299";
                return str;
            }
        }
        public int BlankNum
        {
            get
            {
                try
                {
                    return int.Parse(ViewState["BlankNum"].ToString());
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                ViewState["BlankNum"] = value;
            }
        }
        public new string RefPK
        {
            get
            {
                string str = this.Request.QueryString["RefPK"];
                return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.Request.QueryString["RefPKVal"];
                if (str == null)
                    return "1";
                return str;
            }
        }
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                if (str == null)
                    return 1;
                return Int64.Parse(str);
            }
        }
        #endregion 属性

        public int DtlCount
        {
            get
            {
                return int.Parse(ViewState["DtlCount"].ToString());
            }
            set
            {
                ViewState["DtlCount"] = value;
            }
        }
        public int IsReadonly
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["IsReadonly"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
             "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            MapDtl mdtl = new MapDtl(this.EnsName);
            if (this.IsReadonly == 1)
            {
                mdtl._IsReadonly = 1;
                this.Button1.Enabled = false;
            }

            if (mdtl.HisDtlShowModel == DtlShowModel.Card)
            {
                this.Response.Redirect("DtlFrm.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node + "&MainEnsName=" + this.MainEnsName, true);
                return;
            }
            this.Bind(mdtl);
        }
        public int addRowNum
        {
            get
            {
                try
                {
                    int i = int.Parse(this.Request.QueryString["addRowNum"]);
                    if (this.Request.QueryString["IsCut"] == null)
                        return i;
                    else
                        return i;
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int IsWap
        {
            get
            {
                if (this.Request.QueryString["IsWap"] == "1")
                    return 1;
                return 0;
            }
        }
        public bool IsEnable_del
        {
            get
            {
                string s = this.ViewState["R"] as string;
                if (s == null || s == "0")
                    return false;
                return true;
            }
            set
            {
                if (value)
                    this.ViewState["R"] = "1";
                else
                    this.ViewState["R"] = "0";
            }
        }
        public GEEntity _MainEn = null;
        public GEEntity MainEn
        {
            get
            {
                if (_MainEn == null)
                    _MainEn = new GEEntity(this.FK_MapData, this.RefPKVal);
                return _MainEn;
            }
        }
        public MapAttrs _MainMapAttrs = null;
        public MapAttrs MainMapAttrs
        {
            get
            {
                if (_MainMapAttrs == null)
                    _MainMapAttrs = new MapAttrs(this.FK_MapData);
                return _MainMapAttrs;
            }
        }
        public string FK_MapData = null;
        public void Bind(MapDtl mdtl)
        {
            if (this.Request.QueryString["IsTest"] != null)
                BP.DA.Cash.SetMap(this.EnsName, null);

            GEDtls dtls = new GEDtls(this.EnsName);
            this.FK_MapData = mdtl.FK_MapData;

            GEEntity mainEn = null;

            #region 生成标题
            MapAttrs attrs = new MapAttrs(this.EnsName);
            MapAttrs attrs2 = new MapAttrs();
            this.Pub1.Add("<Table border=1 style='padding:0px' >");
            this.Pub1.Add(mdtl.MTR);
            if (mdtl.IsShowTitle)
            {
                this.Pub1.AddTR();
                if (this.IsWap == 1)
                {
                    BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
                    this.Pub1.AddTD("<a href='/WF/WAP/MyFlow.aspx?WorkID=" + this.RefPKVal + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow + "' /><img src='/WF/Wap/Img/Back.png' border=0/></a>");
                }
                else
                {
                    if (mdtl.IsReadonly == false)
                        this.Pub1.Add("<TD class='FDesc'><img src='/WF/Img/Btn/Table.gif' onclick=\"return DtlOpt('" + this.RefPKVal + "','" + this.EnsName + "');\" border=0/></TD>");
                    else
                        this.Pub1.AddTDTitle();
                }

                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.IsPK)
                        continue;
                    //for lijian 增加了 @符号是一个换行符. 
                    this.Pub1.AddTDTitle(attr.Name.Replace("@", "<br>"));// ("<TD class='FDesc' nowarp=true ><label>" + attr.Name + "</label></TD>");
                }

                if (mdtl.IsEnableAthM)
                    this.Pub1.AddTDTitle("");

                if (mdtl.IsEnableM2M)
                    this.Pub1.AddTDTitle("");

                if (mdtl.IsEnableM2MM)
                    this.Pub1.AddTDTitle("");

                if (mdtl.IsDelete && this.IsReadonly == 0)
                    this.Pub1.Add("<TD class='FDesc' nowarp=true ><img src='/WF/Img/Btn/Save.gif' border=0 onclick='SaveDtlData();' ></TD>");

                this.Pub1.AddTREnd();
            }
            #endregion 生成标题

            QueryObject qo = null;
            try
            {
                qo = new QueryObject(dtls);
                switch (mdtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                        break;
                    case DtlOpenType.ForFID: // 按流程ID来控制.
                        qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                        break;
                }
            }
            catch
            {
                dtls.GetNewEntity.CheckPhysicsTable();
            }

            #region 生成翻页
            this.Pub2.Clear();
            try
            {
                int count = qo.GetCount();
                this.DtlCount = count;
                this.Pub2.Clear();
                this.Pub2.BindPageIdx(count, mdtl.RowsOfList, this.PageIdx, "Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&IsWap=" + this.IsWap + "&IsReadonly=" + this.IsReadonly + "&MainEnsName=" + this.MainEnsName);
                int num = qo.DoQuery("OID", mdtl.RowsOfList, this.PageIdx, false);
                mdtl.RowsOfList = mdtl.RowsOfList + this.addRowNum;
                if (mdtl.IsInsert && this.IsReadonly == 0)
                {
                    int dtlCount = dtls.Count;
                    for (int i = 0; i < mdtl.RowsOfList - dtlCount; i++)
                    {
                        BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                        dt.ResetDefaultVal();
                        dt.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                        dt.OID = i;
                        dtls.AddEntity(dt);
                    }

                    if (num == mdtl.RowsOfList)
                    {
                        BP.Sys.GEDtl dt1 = new GEDtl(this.EnsName);
                        dt1.ResetDefaultVal();
                        dt1.SetValByKey(GEDtlAttr.RefPK, this.RefPKVal);
                        dt1.OID = mdtl.RowsOfList + 1;
                        dtls.AddEntity(dt1);
                    }
                }
            }
            catch
            {
                dtls.GetNewEntity.CheckPhysicsTable();
            }
            #endregion 生成翻页

            DDL ddl = new DDL();
            CheckBox cb = new CheckBox();

            #region 生成数据
            int idx = 1;
            string ids = ",";
            int dtlsNum = dtls.Count;
            MapExts mes = new MapExts(this.EnsName);

            // 需要自动填充的下拉框IDs. 这些下拉框不需要自动填充数据。
            string autoFullDataDDLIDs = ",";
            string LinkFields = ",";
            foreach (MapExt me in mes)
            {
                switch (me.ExtType)
                {
                    case MapExtXmlList.ActiveDDL:
                        autoFullDataDDLIDs += me.AttrsOfActive + ",";
                        break;
                    case MapExtXmlList.AutoFullDLL:
                        autoFullDataDDLIDs += me.AttrOfOper + ",";
                        break;
                    case MapExtXmlList.Link:
                        LinkFields += me.AttrOfOper + ",";
                        break;
                    default:
                        break;
                }
            }

            foreach (BP.Sys.GEDtl dtl in dtls)
            {
                #region 处理
                if (ids.Contains("," + dtl.OID + ","))
                    continue;

                ids += dtl.OID + ",";
                this.Pub1.AddTR();

                if (dtlsNum == idx && mdtl.IsShowIdx && mdtl.IsInsert && this.IsReadonly == 0)
                {
                    DDL myAdd = new DDL();
                    myAdd.AutoPostBack = true;
                    myAdd.Items.Add(new ListItem("+", "+"));
                    for (int i = 1; i < 10; i++)
                    {
                        myAdd.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    myAdd.SelectedIndexChanged += new EventHandler(myAdd_SelectedIndexChanged);
                    this.Pub1.AddTD(myAdd);
                }
                else
                {
                    if (mdtl.IsShowIdx)
                    {
                        this.Pub1.AddTDIdx(idx++);
                    }
                }
                #endregion 处理

                #region 增加rows
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false || attr.KeyOfEn == "OID")
                        continue;

                    string val = dtl.GetValByKey(attr.KeyOfEn).ToString();
                    if (attr.UIIsEnable == false && LinkFields.Contains("," + attr.KeyOfEn + ","))
                    {
                        if (string.IsNullOrEmpty(val))
                            val = "...";
                        MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link,
                            MapExtAttr.AttrOfOper, attr.KeyOfEn) as MapExt;

                        string url = meLink.Tag.Clone() as string;
                        if (url.Contains("?") == false)
                            url = url + "?a3=2";

                        url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + mdtl.No;
                        if (url.Contains("@AppPath"))
                            url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
                        if (url.Contains("@"))
                        {
                            if (attrs2.Count == 0)
                                attrs2 = new MapAttrs(mdtl.No);
                            foreach (MapAttr item in attrs2)
                            {
                                url = url.Replace("@" + item.KeyOfEn, dtl.GetValStrByKey(item.KeyOfEn));
                                if (url.Contains("@") == false)
                                    break;
                            }
                            if (url.Contains("@"))
                            {
                                /*可能是主表也要有参数*/
                                if (mainEn == null)
                                    mainEn = this.MainEn;
                                foreach (Attr attrM in mainEn.EnMap.Attrs)
                                {
                                    url = url.Replace("@" + attrM.Key, mainEn.GetValStrByKey(attrM.Key));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                        }
                        this.Pub1.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + val + "</a>");
                        continue;
                    }

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            TextBox tb = new TextBox();
                            tb.ID = "TB_" + attr.KeyOfEn + "_" + dtl.OID;
                            //  tb.Enabled = attr.UIIsEnable;
                            if (attr.UIIsEnable == false)
                            {
                                tb.Attributes.Add("readonly", "true");
                                tb.CssClass = "TBReadonly";
                            }
                            else
                            {
                                tb.Attributes["onfocus"] = "isChange=true;";
                            }
                            switch (attr.MyDataType)
                            {
                                case DataType.AppString:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                    this.Pub1.AddTD("width='2px'", tb);
                                    tb.Text = val;
                                    if (attr.UIIsEnable == false)
                                    {
                                        tb.Attributes.Add("readonly", "true");
                                        tb.CssClass = "TBReadonly";
                                    }

                                    if (attr.UIHeight > 25)
                                    {
                                        tb.TextMode = TextBoxMode.MultiLine;
                                        tb.Attributes["Height"] = attr.UIHeight + "px";
                                        tb.Rows = attr.UIHeight / 25;
                                    }
                                    break;
                                case DataType.AppDate:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                    tb.Text = val;
                                    if (attr.UIIsEnable)
                                    {
                                        tb.Attributes["onfocus"] = "WdatePicker();isChange=true;";
                                    }
                                    else
                                    {
                                        tb.ReadOnly = true;
                                    }
                                    this.Pub1.AddTD("width='2px'", tb);
                                    break;
                                case DataType.AppDateTime:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                    tb.Text = val;
                                    if (attr.UIIsEnable)
                                    {
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});isChange=true;";
                                    }
                                    else
                                    {
                                        tb.ReadOnly = true;
                                    }
                                    this.Pub1.AddTD("width='2px'", tb);
                                    break;
                                case DataType.AppInt:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                    if (attr.UIIsEnable == false)
                                    {
                                        tb.Attributes["class"] = "TBNumReadonly";
                                        tb.ReadOnly = true;
                                    }
                                    try
                                    {
                                        tb.Text = val;
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Alert(ex.Message + " val =" + val);
                                        tb.Text = "0";
                                    }
                                    this.Pub1.AddTD(tb);
                                    break;
                                case DataType.AppMoney:
                                case DataType.AppRate:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;border-width:0px;";
                                    if (attr.UIIsEnable == false)
                                    {
                                        tb.Attributes["class"] = "TBNumReadonly";
                                        tb.ReadOnly = true;
                                    }

                                    try
                                    {
                                        tb.Text = decimal.Parse(val).ToString("0.00");
                                    }
                                    catch (Exception ex)
                                    {
                                        this.Alert(ex.Message + " val =" + val);
                                        tb.Text = "0.00";
                                    }
                                    this.Pub1.AddTD(tb);
                                    break;
                                default:
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
                                    tb.Text = val;
                                    this.Pub1.AddTD(tb);
                                    break;
                            }

                            if (attr.IsNum && attr.LGType == FieldTypeS.Normal)
                            {
                                if (tb.Enabled)
                                {
                                    // OnKeyPress="javascript:return VirtyNum(this);"
                                    if (attr.MyDataType == DataType.AppInt)
                                        tb.Attributes["OnKeyDown"] = "javascript:return VirtyInt(this);";
                                    else
                                        tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";

                                    tb.Attributes["onkeyup"] += "javascript:C" + dtl.OID + "();C" + attr.KeyOfEn + "();";
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
                                }
                                else
                                {
                                    tb.Attributes["onpropertychange"] += "C" + attr.KeyOfEn + "();";
                                    tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border-width:0px;";
                                }
                            }
                            break;
                        case UIContralType.DDL:
                            switch (attr.LGType)
                            {
                                case FieldTypeS.Enum:
                                    DDL myddl = new DDL();
                                    myddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                    myddl.Attributes["onchange"] = "isChange= true;";
                                    if (attr.UIIsEnable)
                                    {
                                        try
                                        {
                                            myddl.BindSysEnum(attr.KeyOfEn);
                                            myddl.SetSelectItem(val);
                                        }
                                        catch (Exception ex)
                                        {
                                            BP.PubClass.Alert(ex.Message);
                                        }
                                    }
                                    else
                                    {
                                        myddl.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn), dtl.GetValStrByKey(attr.KeyOfEn)));
                                    }
                                    myddl.Enabled = attr.UIIsEnable;
                                    this.Pub1.AddTDCenter(myddl);
                                    break;
                                case FieldTypeS.FK:
                                    DDL ddl1 = new DDL();
                                    ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                                    ddl1.Attributes["onchange"] = "isChange=true;";
                                    ddl1.Attributes["onfocus"] = "isChange=true;";
                                    if (attr.UIIsEnable)
                                    {
                                        //   ddl1.Attributes["onchange"] = "isChange=true;";
                                        EntitiesNoName ens = attr.HisEntitiesNoName;
                                        ens.RetrieveAll();
                                        ddl1.BindEntities(ens);
                                        if (ddl1.SetSelectItem(val) == false)
                                            ddl1.Items.Insert(0, new ListItem("请选择", val));
                                    }
                                    else
                                    {
                                        ddl1.Items.Add(new ListItem(dtl.GetValRefTextByKey(attr.KeyOfEn), dtl.GetValStrByKey(attr.KeyOfEn)));
                                    }
                                    ddl1.Enabled = attr.UIIsEnable;
                                    this.Pub1.AddTDCenter(ddl1);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case UIContralType.CheckBok:
                            cb = new CheckBox();
                            cb.ID = "CB_" + attr.KeyOfEn + "_" + dtl.OID;
                            cb.Text = attr.Name;
                            if (val == "1")
                                cb.Checked = true;
                            else
                                cb.Checked = false;
                            //  cb.Attributes["onchecked"] = "alert('ss'); isChange= true; ";
                            cb.Attributes["onclick"] = "isChange= true;";
                            this.Pub1.AddTD(cb);
                            break;
                        default:
                            break;
                    }
                }


                if (mdtl.IsEnableAthM)
                {
                    if (dtl.OID >= 100)
                        this.Pub1.AddTD("<a href=\"javascript:window.showModalDialog('AttachmentUpload.aspx?IsBTitle=1&PKVal=" + dtl.OID + "&Ath=AthM&FK_MapData=" + mdtl.No + "&FK_FrmAttachment=" + mdtl.No + "_AthM','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='/WF/Img/AttachmentM.png' border=0 width='16px' /></a>");
                    else
                        this.Pub1.AddTD("");
                }

                if (mdtl.IsEnableM2M)
                {
                    if (dtl.OID >= 100)
                        this.Pub1.AddTD("<a href=\"javascript:window.showModalDialog('M2M.aspx?IsOpen=1&NoOfObj=M2M&OID=" + dtl.OID + "&FK_MapData=" + mdtl.No + "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='/WF/Img/M2M.png' border=0 width='16px' /></a>");
                    else
                        this.Pub1.AddTD("");
                }

                if (mdtl.IsEnableM2MM)
                {
                    if (dtl.OID >= 100)
                        this.Pub1.AddTD("<a href=\"javascript:window.showModalDialog('M2MM.aspx?IsOpen=1&NoOfObj=M2MM&OID=" + dtl.OID + "&FK_MapData=" + mdtl.No + "','m2m','dialogHeight: 500px; dialogWidth: 600px;center: yes; help: no')\"><img src='/WF/Img/M2M.png' border=0 width='16px' /></a>");
                    else
                        this.Pub1.AddTD("");
                }

                if (mdtl.IsDelete && this.IsReadonly == 0 && dtl.OID >= 100)
                {
                    this.Pub1.Add("<TD border=0><img src='/WF/Img/Btn/Delete.gif' onclick=\"javascript:Del('" + dtl.OID + "','" + this.EnsName + "','" + this.RefPKVal + "','" + this.PageIdx + "')\" /></TD>");
                }
                else if (mdtl.IsDelete)
                {
                    if (this.IsReadonly == 0)
                        this.Pub1.Add("<TD class=TD border=0>&nbsp;</TD>");
                }
                this.Pub1.AddTREnd();
                #endregion 增加rows
            }

            #region 拓展属性
            if (this.IsReadonly == 0 && mes.Count != 0)
            {
                this.Page.RegisterClientScriptBlock("s81",
              "<script language='JavaScript' src='/WF/Scripts/jquery-1.4.1.min.js' ></script>");

                this.Page.RegisterClientScriptBlock("b81",
             "<script language='JavaScript' src='MapExt.js' defer='defer' type='text/javascript' ></script>");

                this.Pub1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");

                this.Page.RegisterClientScriptBlock("dCd",
    "<script language='JavaScript' src='/DataUser/JSLibData/" + mdtl.No + ".js' ></script>");

                foreach (BP.Sys.GEDtl mydtl in dtls)
                {
                    //ddl.ID = "DDL_" + attr.KeyOfEn + "_" + dtl.OID;
                    foreach (MapExt me in mes)
                    {
                        switch (me.ExtType)
                        {
                            case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                DDL ddlOper = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (ddlOper == null)
                                    continue;
                                ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                                break;
                            case MapExtXmlList.ActiveDDL:
                                DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                string val, valC;
                                DataTable dt;
                                if (ddlPerant == null)
                                    continue;
#warning 此处需要优化
                                string ddlC = "Pub1_DDL_" + me.AttrsOfActive + "_" + mydtl.OID;
                                ddlPerant.Attributes["onchange"] = " isChange=true; DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";
                                DDL ddlChild = this.Pub1.GetDDLByID("DDL_" + me.AttrsOfActive + "_" + mydtl.OID);
                                val = ddlPerant.SelectedItemStringVal;
                                if (ddlChild.Items.Count == 0)
                                    valC = mydtl.GetValStrByKey(me.AttrsOfActive);
                                else
                                    valC = ddlChild.SelectedItemStringVal;

                                dt = DBAccess.RunSQLReturnTable(me.Doc.Replace("@Key", val));

                                ddlChild.Bind(dt, "No", "Name");
                                if (ddlChild.SetSelectItem(valC) == false)
                                {
                                    ddlChild.Items.Insert(0, new ListItem("请选择" + valC, valC));
                                    ddlChild.SelectedIndex = 0;
                                }
                                ddlChild.Attributes["onchange"] = " isChange=true;";
                                break;
                            case MapExtXmlList.AutoFullDLL: //自动填充下拉框的范围.
                                DDL ddlFull = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (ddlFull == null)
                                    continue;

                                string valOld = mydtl.GetValStrByKey(me.AttrOfOper);
                                //string valOld =ddlFull.SelectedItemStringVal;

                                string fullSQL = me.Doc.Replace("@WebUser.No", WebUser.No);
                                fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                                fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                fullSQL = fullSQL.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

                                if (fullSQL.Contains("@"))
                                {
                                    Attrs attrsFull = mydtl.EnMap.Attrs;
                                    foreach (Attr attr in attrsFull)
                                    {
                                        if (fullSQL.Contains("@") == false)
                                            break;
                                        fullSQL = fullSQL.Replace("@" + attr.Key, mydtl.GetValStrByKey(attr.Key));
                                    }
                                }

                                if (fullSQL.Contains("@"))
                                {
                                    /*从主表中取数据*/
                                    Attrs attrsFull = this.MainEn.EnMap.Attrs;
                                    foreach (Attr attr in attrsFull)
                                    {
                                        if (fullSQL.Contains("@") == false)
                                            break;

                                        if (fullSQL.Contains("@" + attr.Key) == false)
                                            continue;

                                        fullSQL = fullSQL.Replace("@" + attr.Key, this.MainEn.GetValStrByKey(attr.Key));
                                    }
                                }

                                ddlFull.Items.Clear();
                                ddlFull.Bind(DBAccess.RunSQLReturnTable(fullSQL), "No", "Name");
                                if (ddlFull.SetSelectItem(valOld) == false)
                                {
                                    ddlFull.Items.Insert(0, new ListItem("请选择" + valOld, valOld));
                                    ddlFull.SelectedIndex = 0;
                                }
                                ddlFull.Attributes["onchange"] = " isChange=true;";
                                break;
                            case MapExtXmlList.TBFullCtrl: // 自动填充.
                                TextBox tbAuto = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (tbAuto == null)
                                    continue;
                                tbAuto.Attributes["onkeyup"] = " isChange=true; DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                                tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                                if (me.Tag != "")
                                {
                                    /* 处理下拉框的选择范围的问题 */
                                    string[] strs = me.Tag.Split('$');
                                    foreach (string str in strs)
                                    {
                                        string[] myCtl = str.Split(':');
                                        string ctlID = myCtl[0];
                                        DDL ddlC1 = this.Pub1.GetDDLByID("DDL_" + ctlID + "_" + mydtl.OID);
                                        if (ddlC1 == null)
                                        {
                                            //me.Tag = "";
                                            // me.Update();
                                            continue;
                                        }

                                        string sql = myCtl[1].Replace("~", "'");
                                        sql = sql.Replace("@WebUser.No", WebUser.No);
                                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                        sql = sql.Replace("@Key", tbAuto.Text.Trim());
                                        dt = DBAccess.RunSQLReturnTable(sql);
                                        string valC1 = ddlC1.SelectedItemStringVal;
                                        ddlC1.Items.Clear();
                                        foreach (DataRow dr in dt.Rows)
                                            ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                        ddlC1.SetSelectItem(valC1);
                                    }
                                }
                                break;
                            case MapExtXmlList.InputCheck:
                                TextBox tbCheck = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                if (tbCheck != null)
                                    tbCheck.Attributes[me.Tag2] += " rowPK=" + mydtl.OID + "; " + me.Tag1 + "(this);";
                                break;
                            case MapExtXmlList.PopVal: //弹出窗.
                                TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.Link: // 超链接.

                                //TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                //tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion 拓展属性

            #region 生成合计
            if (mdtl.IsShowSum && dtls.Count > 1)
            {
                this.Pub1.AddTRSum();
                if (mdtl.IsShowIdx)
                    this.Pub1.AddTD();
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;
                    if (attr.IsNum && attr.LGType == FieldTypeS.Normal)
                    {
                        TextBox tb = new TextBox();
                        tb.ID = "TB_" + attr.KeyOfEn;
                        tb.Text = attr.DefVal;
                        // tb.ShowType = attr.HisTBType;
                        tb.ReadOnly = true;
                        tb.Font.Bold = true;
                        tb.BackColor = System.Drawing.Color.FromName("infobackground");
                        switch (attr.MyDataType)
                        {
                            case DataType.AppRate:
                            case DataType.AppMoney:
                                tb.Text = dtls.GetSumDecimalByKey(attr.KeyOfEn).ToString("0.00");
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            case DataType.AppInt:
                                tb.Text = dtls.GetSumIntByKey(attr.KeyOfEn).ToString();
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            case DataType.AppFloat:
                                tb.Text = dtls.GetSumFloatByKey(attr.KeyOfEn).ToString();
                                tb.Attributes["style"] = "width:" + attr.UIWidth + "px;text-align:right;border:none";
                                break;
                            default:
                                break;
                        }
                        this.Pub1.AddTD("align=right", tb);
                    }
                    else
                    {
                        this.Pub1.AddTD();
                    }
                }
                if (mdtl.IsEnableAthM)
                    this.Pub1.AddTD();

                if (mdtl.IsEnableM2M)
                    this.Pub1.AddTD();

                if (mdtl.IsEnableM2MM)
                    this.Pub1.AddTD();

                if (mdtl.IsReadonly == false)
                    this.Pub1.AddTD();

                this.Pub1.AddTREnd();
            }
            #endregion 生成合计
            #endregion 生成数据

            this.Pub1.AddTableEnd();

            #region 生成 自动计算行
            if (this.IsReadonly == 0)
            {
                // 输出自动计算公式
                this.Response.Write("\n<script language='JavaScript'>");
                foreach (GEDtl dtl in dtls)
                {
                    string top = "\n function C" + dtl.OID + "() { \n ";
                    string script = "";
                    foreach (MapAttr attr in attrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.IsNum == false)
                            continue;

                        if (attr.LGType != FieldTypeS.Normal)
                            continue;

                        if (attr.HisAutoFull == AutoFullWay.Way1_JS)
                        {
                            script += this.GenerAutoFull(dtl.OID.ToString(), attrs, attr);
                        }
                    }
                    string end = " \n  } ";
                    this.Response.Write(top + script + end);
                }
                this.Response.Write("\n</script>");

                // 输出合计算计公式
                foreach (MapAttr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.LGType != FieldTypeS.Normal)
                        continue;

                    if (attr.IsNum == false)
                        continue;

                    string top = "\n<script language='JavaScript'> function C" + attr.KeyOfEn + "() { \n ";
                    string end = "\n  isChange =true ;  } </script>";
                    this.Response.Write(top + this.GenerSum(attr, dtls) + " ; \t\n" + end);
                }
            }
            #endregion
        }
        bool isAddDDLSelectIdxChange = false;
        void myAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            string val = ddl.SelectedItemStringVal;
            string url = "";
            isAddDDLSelectIdxChange = true;
            this.Save();

            if (val.Contains("+"))
                url = "Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&AddRowNum=" + ddl.SelectedItemStringVal.Replace("+", "").Replace("-", "") + "&IsCut=0&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node;
            else
                url = "Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&AddRowNum=" + ddl.SelectedItemStringVal.Replace("+", "").Replace("-", "") + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node;

            this.Response.Redirect(url, true);
        }
        public void Save()
        {
            MapDtl mdtl = new MapDtl(this.EnsName);
            GEDtls dtls = new GEDtls(this.EnsName);
            FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.
            GEEntity mainEn = mdtl.GenerGEMainEntity(this.RefPKVal);

            #region 从表保存前处理事件.
            if (fes.Count > 0)
            {
                try
                {
                    string msg = fes.DoEventNode(EventListDtlList.DtlSaveEnd, mainEn);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 从表保存前处理事件.


            QueryObject qo = new QueryObject(dtls);
            switch (mdtl.DtlOpenType)
            {
                case DtlOpenType.ForEmp:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    qo.addAnd();
                    qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                    break;
                case DtlOpenType.ForWorkID:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    break;
                case DtlOpenType.ForFID:
                    qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                    break;
            }

            int num = qo.DoQuery("OID", mdtl.RowsOfList, this.PageIdx, false);
            int dtlCount = dtls.Count;

            mdtl.RowsOfList = mdtl.RowsOfList + this.addRowNum;
            for (int i = 0; i < mdtl.RowsOfList - dtlCount; i++)
            {
                BP.Sys.GEDtl dt = new GEDtl(this.EnsName);
                dt.ResetDefaultVal();
                dt.OID = i;
                dtls.AddEntity(dt);
            }

            if (num == mdtl.RowsOfList)
            {
                BP.Sys.GEDtl dt1 = new GEDtl(this.EnsName);
                dt1.ResetDefaultVal();
                dt1.OID = mdtl.RowsOfList + 1;
                dtls.AddEntity(dt1);
            }


            Map map = dtls.GetNewEntity.EnMap;
            bool isTurnPage = false;
            string err = "";
            int idx = 0;

            // 判断是否有事件.
            bool isHaveBefore = false;
            bool isHaveEnd = false;
            FrmEvent fe_Before = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveBefore) as FrmEvent;
            if (fe_Before == null)
                isHaveBefore = false;
            else
                isHaveBefore = true;

            FrmEvent fe_End = fes.GetEntityByKey(FrmEventAttr.FK_Event, EventListDtlList.DtlItemSaveAfter) as FrmEvent;
            if (fe_End == null)
                isHaveEnd = false;
            else
                isHaveEnd = true;

            //...................................
            foreach (GEDtl dtl in dtls)
            {
                idx++;
                try
                {
                    this.Pub1.Copy(dtl, dtl.OID.ToString(), map);
                    if (dtl.OID < mdtl.RowsOfList + 2)
                    {
                        int myOID = dtl.OID;
                        dtl.OID = 0;
                        if (dtl.IsBlank)
                            continue;

                        dtl.OID = myOID;
                        if (dtl.OID == mdtl.RowsOfList + 1)
                            isTurnPage = true;

                        dtl.RefPK = this.RefPKVal;
                        dtl.FID = this.FID;
                        if (isHaveBefore)
                        {
                            try
                            {
                                err += fes.DoEventNode(EventListDtlList.DtlItemSaveBefore, dtl);
                            }
                            catch (Exception ex)
                            {
                                err += ex.Message;
                                continue;
                            }
                        }
                        dtl.InsertAsOID(DBAccess.GenerOID("Dtl"));
                    }
                    else
                    {
                        dtl.FID = this.FID;
                        if (isHaveBefore)
                        {
                            try
                            {
                                err += fes.DoEventNode(EventListDtlList.DtlItemSaveBefore, dtl);
                            }
                            catch (Exception ex)
                            {
                                err += ex.Message;
                                continue;
                            }
                        }
                        dtl.Update();
                    }

                    if (isHaveEnd)
                    {
                        /* 如果有保存后的事件。*/
                        try
                        {
                            fes.DoEventNode(EventListDtlList.DtlItemSaveAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            err += ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    dtl.CheckPhysicsTable();
                    err += "Row: " + idx + " Error \r\n" + ex.Message;
                }
            }

            if (err != "")
            {
                BP.DA.Log.DefaultLogWriteLineInfo(err);
                this.Alert(err);
                return;
            }

            if (isAddDDLSelectIdxChange == true)
                return;

            #region 从表保存后处理事件。
            if (fes.Count > 0)
            {
                try
                {
                    string msg = fes.DoEventNode(EventListDtlList.DtlSaveEnd, mainEn);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            if (isTurnPage)
            {
                int pageNum = 0;
                int count = this.DtlCount + 1;
                decimal pageCountD = decimal.Parse(count.ToString()) / decimal.Parse(mdtl.RowsOfList.ToString()); // 页面个数。
                string[] strs = pageCountD.ToString("0.0000").Split('.');
                if (int.Parse(strs[1]) > 0)
                    pageNum = int.Parse(strs[0]) + 1;
                else
                    pageNum = int.Parse(strs[0]);
                this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + pageNum + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node, true);
            }
            else
            {
                this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal + "&PageIdx=" + this.PageIdx + "&IsWap=" + this.IsWap + "&FK_Node=" + this.FK_Node, true);
            }
        }
        public void ExpExcel()
        {
            BP.Sys.MapDtl mdtl = new MapDtl(this.EnsName);
            this.Title = mdtl.Name;
            GEDtls dtls = new GEDtls(this.EnsName);
            QueryObject qo = new QueryObject(dtls);
            switch (mdtl.DtlOpenType)
            {
                case DtlOpenType.ForEmp:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    //qo.addAnd();
                    //qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                    break;
                case DtlOpenType.ForWorkID:
                    qo.AddWhere(GEDtlAttr.RefPK, this.RefPKVal);
                    break;
                case DtlOpenType.ForFID:
                    qo.AddWhere(GEDtlAttr.FID, this.RefPKVal);
                    break;
            }
            qo.DoQuery();

            // this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //DataTable dt = dtls.ToDataTableDesc();
            // this.GenerExcel(dtls.ToDataTableDesc(), mdtl.Name + ".xls");

            this.GenerExcel_pri_Text(dtls.ToDataTableDesc(), mdtl.Name + "@" + WebUser.No + "@" + DataType.CurrentData + ".xls");

            //this.ExportDGToExcelV2(dtls, this.Title + ".xls");
            //dtls.GetNewEntity.CheckPhysicsTable();
            //this.Response.Redirect("Dtl.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
        }
        void BPToolBar1_ButtonClick(object sender, EventArgs e)
        {
            ToolbarBtn btn = sender as ToolbarBtn;
            switch (btn.ID)
            {
                case NamesOfBtn.New:
                case NamesOfBtn.Save:
                case NamesOfBtn.SaveAndNew:
                    this.Save();
                    break;
                case NamesOfBtn.SaveAndClose:
                    this.Save();
                    this.WinClose();
                    break;
                case NamesOfBtn.Delete:
                    GEDtls dtls = new GEDtls(this.EnsName);
                    QueryObject qo = new QueryObject(dtls);
                    qo.DoQuery("OID", BP.SystemConfig.PageSize, this.PageIdx, false);
                    foreach (GEDtl dtl in dtls)
                    {
                        CheckBox cb = this.Pub1.GetCBByID("CB_" + dtl.PKVal);
                        if (cb == null)
                            continue;

                        if (cb.Checked)
                            dtl.Delete();
                    }
                    this.Pub1.Clear();
                    MapDtl md = new MapDtl(this.EnsName);
                    this.Bind(md);
                    break;
                case NamesOfBtn.Excel:
                    this.ExpExcel();
                    break;
                default:
                    BP.PubClass.Alert("@当前版本不支持此功能。");
                    break;
            }
        }
        /// <summary>
        /// 生成列的计算
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public string GenerAutoFull(string pk, MapAttrs attrs, MapAttr attr)
        {
            string left = "\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn + "_" + pk).ClientID + ".value = ";
            string right = attr.AutoFullDoc;
            foreach (MapAttr mattr in attrs)
            {
                //if (mattr.IsNum == false)
                //    continue;

                //if (mattr.LGType != FieldTypeS.Normal)
                //    continue;

                string tbID = "TB_" + mattr.KeyOfEn + "_" + pk;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) ) ");
            }
            string s = left + right;
            s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn + "_" + pk).ClientID + ".value ) ;";
            return s += " C" + attr.KeyOfEn + "();";
        }
        public string GenerSum(MapAttr mattr, GEDtls dtls)
        {
            if (dtls.Count <= 1)
                return "";

            string ClientID = "";

            try
            {
                ClientID = this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID;
            }
            catch
            {
                return "";
            }

            string left = "\n  document.forms[0]." + ClientID + ".value = ";
            string right = "";
            int i = 0;
            foreach (GEDtl dtl in dtls)
            {
                string tbID = "TB_" + mattr.KeyOfEn + "_" + dtl.OID;
                TextBox tb = this.Pub1.GetTextBoxByID(tbID);
                if (tb == null)
                    continue;

                if (i == 0)
                    right += " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
                else
                    right += " +parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
                i++;
            }
            string s = left + right + " ;";
            switch (mattr.MyDataType)
            {
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppRate:
                    return s += "\t\n  document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value= VirtyMoney(document.forms[0]." + this.Pub1.GetTextBoxByID("TB_" + mattr.KeyOfEn).ClientID + ".value ) ;";
                default:
                    return s;
            }
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            this.Save();
        }
    }

}