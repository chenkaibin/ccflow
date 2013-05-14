//===========================================================================
// 此文件是作为 ASP.NET 2.0 Web 项目转换的一部分修改的。
// 类名已更改，且类已修改为从文件“App_Code\Migrated\comm\uc\Stub_ucen_ascx_cs.cs”的抽象基类 
// 继承。
// 在运行时，此项允许您的 Web 应用程序中的其他类使用该抽象基类绑定和访问  
// 代码隐藏页。 
// 关联的内容页“comm\uc\ucen.ascx”也已修改，以引用新的类名。
// 有关此代码模式的更多信息，请参考 http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
namespace CCFlow.WF.UC
{
    using System;
    using System.IO;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Web.UI;
    using BP;
    using BP.En;
    using BP.Sys;
    using BP.Sys.Xml;
    using BP.DA;
    using BP.Web;
    using BP.Web.Controls;
    /// <summary>
    ///	UCEn 的摘要说明。
    /// </summary>
    public partial class UCEn : BP.Web.UC.UCBase3
    {
        #region add 2010-07-24 处理实体绑定的第二个算法

        #region add varable.
        public BP.Sys.GroupField currGF = new BP.Sys.GroupField();
        public MapDtls dtls;
        public MapFrames frames;
        public MapM2Ms m2ms;
        public FrmAttachments aths;
        private GroupFields gfs;
        public int rowIdx = 0;
        public bool isLeftNext = true;
        #endregion add varable.

        public void BindColumn2(Entity en, string enName)
        {
            this.ctrlUseSta = "";
            this.EnName = enName;
            this.HisEn = en;
            this.mapData = new MapData(enName);
            currGF = new GroupField();
            MapAttrs mattrs = this.mapData.MapAttrs;
            gfs = this.mapData.GroupFields;
            dtls = this.mapData.MapDtls;
            frames = this.mapData.MapFrames;
            m2ms = this.mapData.MapM2Ms;
            aths = this.mapData.FrmAttachments;
            mes = this.mapData.MapExts;

            #region 处理事件.
            if (this.IsPostBack == false)
            {
                fes = this.mapData.FrmEvents;
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
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

            //处理装载前填充.
            this.LoadData(mattrs, en);

            this.Add("<table width=100% >");
            foreach (BP.Sys.GroupField gf in gfs)
            {
                currGF = gf;
                this.AddTR();
                if (gfs.Count == 1)
                    this.AddTD("colspan=2 class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                else
                    this.AddTD("colspan=2 class=GroupField valign='top' align=left onclick=\"GroupBarClick('" + gf.Idx + "')\"  ", "<div style='text-align:left; float:left'>&nbsp;<img src='./Style/Min.gif' alert='Min' id='Img" + gf.Idx + "'  border=0 />&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");

                this.AddTREnd();
                int idx = -1;
                isLeftNext = true;
                rowIdx = 0;
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;

                    if (isLeftNext == true)
                        this.InsertObjects2Col(true, en.PKVal.ToString(), en.GetValStrByKey("FID"));

                    rowIdx++;
                    this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "'");

                    if (attr.UIIsEnable == false)
                    {
                        if (this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                            string url = meLink.Tag;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
                            if (url.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr item in attrs)
                                {
                                    url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                            this.AddTD("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                            this.AddTREnd();
                            continue;
                        }
                    }


                    #region 加入字段
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && attr.UIIsLine)
                    {
                        if (attr.UIIsEnable)
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'    width='100%' valign=top align=left>" + attr.Name + "<br>");
                        else
                            this.Add("<TD colspan=2 height='" + attr.UIHeight.ToString() + "px'   width='100%' valign=top class=TBReadonly>" + attr.Name + "<br>");

                        TB mytbLine = new TB();
                        if (attr.IsBigDoc)
                        {
                            mytbLine.TextMode = TextBoxMode.MultiLine;
                            mytbLine.Attributes["class"] = "TBDoc";
                        }

                        mytbLine.ID = "TB_" + attr.KeyOfEn;
                        if (attr.IsBigDoc)
                        {
                            //  mytbLine = 5;
                            // mytbLine.Columns = 30;
                        }

                        mytbLine.Attributes["style"] = "width:98%;height:100%;padding: 0px;margin: 0px;";
                        mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        mytbLine.Enabled = attr.UIIsEnable;

                        this.Add(mytbLine);
                        this.AddTDEnd();
                        this.AddTREnd();
                        rowIdx++;
                        continue;
                    }

                    TB tb = new TB();
                    tb.Attributes["width"] = "100%";
                    tb.Attributes["border"] = "1px";
                    tb.Columns = 40;
                    tb.ID = "TB_" + attr.KeyOfEn;
                    Control ctl = tb;

                    #region add contrals.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            tb.Enabled = attr.UIIsEnable;
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                    tb.ShowType = TBType.TB;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    break;
                                case BP.DA.DataType.AppDate:
                                    tb.ShowType = TBType.Date;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case BP.DA.DataType.AppDateTime:
                                    tb.ShowType = TBType.DateTime;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                    this.AddTD("colspan=2", cb);
                                    continue;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                case BP.DA.DataType.AppInt:
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    break;
                                case BP.DA.DataType.AppMoney:
                                case BP.DA.DataType.AppRate:
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = decimal.Parse(en.GetValStrByKey(attr.KeyOfEn)).ToString("0.00");
                                    break;
                                default:
                                    break;
                            }
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDateTime:
                                case BP.DA.DataType.AppDate:
                                    if (tb.Enabled)
                                        tb.Attributes["class"] = "TB";
                                    else
                                        tb.Attributes["class"] = "TBReadonly";
                                    break;
                                default:
                                    if (tb.Enabled)
                                        tb.Attributes["class"] = "TBNum";
                                    else
                                        tb.Attributes["class"] = "TBNumReadonly";
                                    break;
                            }
                            break;
                        case FieldTypeS.Enum:
                            DDL ddle = new DDL();
                            ddle.ID = "DDL_" + attr.KeyOfEn;
                            ddle.BindSysEnum(attr.KeyOfEn);
                            ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            ddle.Enabled = attr.UIIsEnable;
                            ctl = ddle;
                            break;
                        case FieldTypeS.FK:
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            try
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                ens.RetrieveAll();
                                ddl1.BindEntities(ens);
                                ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            }
                            catch
                            {
                            }
                            ddl1.Enabled = attr.UIIsEnable;
                            ctl = ddl1;
                            break;
                        default:
                            break;
                    }
                    #endregion add contrals.

                    string desc = attr.Name.Replace("：", "");
                    desc = desc.Replace(":", "");
                    desc = desc.Replace(" ", "");

                    if (desc.Length >= 5)
                    {
                        this.Add("<TD colspan=2 class=FDesc width='100%' >" + desc + "<br>");
                        this.Add(ctl);
                        this.AddTREnd();
                    }
                    else
                    {
                        this.AddTDDesc(desc);
                        this.AddTD("width='100%' class=TBReadonly", ctl);
                        this.AddTREnd();
                    }
                    #endregion 加入字段
                }
                //  this.InsertObjects(false);
            }
            this.AddTableEnd();
            this.AfterBindEn_DealMapExt(enName, mattrs, en);

            #region 处理iFrom 的自适应的问题。
            string js = "\t\n<script type='text/javascript' >";
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;
                js += "\t\n window.setInterval(\"ReinitIframe(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            }
            foreach (MapM2M m2m in m2ms)
            {
                if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                    js += "\t\n window.setInterval(\"ReinitIframe('F" + m2m.NoOfObj + "','TD" + m2m.NoOfObj + "')\", 200);";
            }
            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsAutoSize)
                    js += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);

            foreach (MapFrame fr in frames)
            {
                js += "\t\n window.setInterval(\"ReinitIframe(\"ReinitIframe('F" + fr.NoOfObj + "','TD" + fr.NoOfObj + "')\", 200);";
            }

            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            #region 处理iFrom Save。
            js = "\t\n<script type='text/javascript' >";
            js += "\t\n function SaveDtl(dtl) { ";
            js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData(); ";
            js += "\t\n } ";
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom Save。

            #region 处理iFrom Save M2M。
            js = "\t\n<script type='text/javascript' >";
            js += "\t\n function SaveM2M(dtl) { ";
            js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveM2M(); ";
            js += "\t\n } ";
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            //处理扩展.
        }


        public void InsertObjects2Col(bool isJudgeRowIdx, string pk, string fid)
        {
            #region 从表
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                if (dtl.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (dtl.RowIdx != rowIdx)
                        continue;
                }

                if (dtl.GroupID == 0 && rowIdx == 0)
                {
                    dtl.GroupID = currGF.OID;
                    dtl.RowIdx = 0;
                    dtl.Update();
                }
                else if (dtl.GroupID == currGF.OID)
                {
                }
                else
                {
                    continue;
                }
                dtl.IsUse = true;
                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                string src = "";
                try
                {
                    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&FID=" + this.HisEn.GetValStringByKey("FID") + "&IsWap=1&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }
                catch
                {
                    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&IsWap=1&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }
                this.Add("<TD colspan=2 class=FDesc ID='TD" + dtl.No + "'><a href='" + src + "'>" + dtl.Name + "</a></TD>");
                // this.Add("<iframe ID='F" + dtl.No + "' frameborder=0 Onblur=\"SaveDtl('" + dtl.No + "');\" style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' src='" + src + "' height='10px' scrolling=no  /></iframe>");
                //this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 从表

            #region 框架表
            foreach (MapFrame fram in frames)
            {
                if (fram.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (fram.RowIdx != rowIdx)
                        continue;
                }

                if (fram.GroupID == 0 && rowIdx == 0)
                {
                    fram.GroupID = currGF.OID;
                    fram.RowIdx = 0;
                    fram.Update();
                }
                else if (fram.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                fram.IsUse = true;
                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                string src = fram.URL;

                if (src.Contains("?"))
                    src += "&Table=" + fram.FK_MapData + "&WorkID=" + pk + "&FID=" + fid;
                else
                    src += "?Table=" + fram.FK_MapData + "&WorkID=" + pk + "&FID=" + fid;
                this.Add("<TD colspan=2 class=FDesc ID='TD" + fram.NoOfObj + "'><a href='" + src + "'>" + fram.Name + "</a></TD>");
                this.AddTREnd();
            }
            #endregion 从表

            #region 附件
            foreach (FrmAttachment ath in aths)
            {
                if (ath.IsUse)
                    continue;
                if (isJudgeRowIdx)
                {
                    if (ath.RowIdx != rowIdx)
                        continue;
                }

                if (ath.GroupID == 0 && rowIdx == 0)
                {
                    ath.GroupID = currGF.OID;
                    ath.RowIdx = 0;
                    ath.Update();
                }
                else if (ath.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                ath.IsUse = true;
                rowIdx++;

                string src = "./../CCForm/AttachmentUpload.aspx?IsWap=1&PKVal=" + this.HisEn.PKVal + "&NoOfObj=" + ath.NoOfObj + "&FK_MapData=" + EnsName + "&FK_FrmAttachment=" + ath.MyPK;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=2 class=FDesc ID='TD" + ath.NoOfObj + "'><a href='" + src + "'>" + ath.Name + "</a></TD>");
                this.AddTREnd();
            }
            #endregion 附件

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                if (m2m.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (m2m.RowIdx != rowIdx)
                        continue;
                }

                if (m2m.GroupID == 0 && rowIdx == 0)
                {
                    m2m.GroupID = currGF.OID;
                    m2m.RowIdx = 0;
                    m2m.Update();
                }
                else if (m2m.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                m2m.IsUse = true;
                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                    this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='50px' width='100%'  >");
                else
                    this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='" + m2m.H + "' width='" + m2m.W + "'  >");

                string src = "";
                if (m2m.HisM2MType == M2MType.M2M)
                    src = "/WF/CCForm/M2M.aspx?NoOfObj=" + m2m.NoOfObj;
                else
                    src = "/WF/CCForm/M2MM.aspx?NoOfObj=" + m2m.NoOfObj;

                string paras = this.RequestParas;

                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");

                src += "&r=q" + paras;

                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + m2m.FK_MapData;

                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                        if (m2m.IsEdit)
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        break;
                    case FrmShowWay.FrmSpecSize:
                        if (m2m.IsEdit)
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
            }
            #endregion 多对多的关系
        }
        public MapExts mes = null;
        public bool IsLoadData = false;
        private void LoadData(MapAttrs mattrs, Entity en)
        {
            this.LinkFields = "";
            if (mes.Count == 0)
                return;
            foreach (MapExt myitem in mes)
            {
                if (myitem.ExtType == MapExtXmlList.Link)
                    this.LinkFields += "," + myitem.AttrOfOper + ",";
            }

            if (this.IsLoadData == false)
                return;

            //if (this.IsReadonly )
            //    return;

            MapExt item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            if (item == null)
                return;

            DataTable dt = null;
            string sql = item.Tag;
            if (string.IsNullOrEmpty(sql) == false)
            {
                /* 如果有填充主表的sql  */
                #region 处理sql变量
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                foreach (MapAttr attr in mattrs)
                {
                    if (sql.Contains("@"))
                        sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    else
                        break;
                }
                #endregion 处理sql变量

                if (string.IsNullOrEmpty(sql) == false)
                {
                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
                return;

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                string[] sqls = item.Tag1.Split('*');
                foreach (string mysql in sqls)
                {
                    if (string.IsNullOrEmpty(mysql))
                        continue;

                    if (mysql.Contains(dtl.No + "=") == false)
                        continue;

                    #region 处理sql.
                    sql = mysql;
                    sql = sql.Replace(dtl.No + "=", "");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                    foreach (MapAttr attr in mattrs)
                    {
                        if (sql.Contains("@"))
                            sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                        else
                            break;
                    }
                    #endregion 处理sql.

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    if (sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);

                    GEDtls gedtls = new GEDtls(dtl.No);
                    gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);

                    dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                        gedtl.RefPK = en.PKVal.ToString();
                        gedtl.RDT = DataType.CurrentDataTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }
                }
            }
        }
        public string ctrlUseSta = "";
        public void BindColumn4(Entity en, string enName)
        {
            this.ctrlUseSta = "";
            this.EnName = enName;
            this.HisEn = en;
            this.mapData = new MapData(enName);
            currGF = new GroupField();
            MapAttrs mattrs = this.mapData.MapAttrs;
            gfs = this.mapData.GroupFields;
            dtls = this.mapData.MapDtls;
            frames = this.mapData.MapFrames;
            m2ms = this.mapData.MapM2Ms;
            aths = this.mapData.FrmAttachments;
            mes = this.mapData.MapExts;

            #region 处理事件.
            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
                    if (string.IsNullOrEmpty(msg) == false)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            //处理默认值.
            this.DealDefVal(mattrs);
            //处理装载前填充.
            this.LoadData(mattrs, en);
            this.Add("<table id=tabForm  align=center width='700px'>");
            string appPath = this.Page.Request.ApplicationPath;
            foreach (GroupField gf in gfs)
            {
                currGF = gf;
                this.AddTR();
                if (gfs.Count == 1)
                    this.AddTD("colspan=4 class=GroupField valign='top' align=left ", "<div style='text-align:left; float:left'>&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                else
                    this.AddTD("colspan=4 class=GroupField valign='top' align=left  onclick=\"GroupBarClick('" + gf.Idx + "')\"  ", "<div style='text-align:left; float:left'>&nbsp;<img src='/WF/Style/Min.gif' alert='Min' id='Img" + gf.Idx + "' border=0 />&nbsp;" + gf.Lab + "</div><div style='text-align:right; float:right'></div>");
                this.AddTREnd();

                bool isHaveH = false;
                int i = -1;
                int idx = -1;
                isLeftNext = true;
                rowIdx = 0;
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;

                    if (isLeftNext == true)
                        this.InsertObjects(true);

                    #region 加入字段
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && attr.UIIsLine)
                    {
                        if (isLeftNext == false)
                        {
                            this.AddTD();
                            this.AddTD();
                            this.AddTREnd();
                            rowIdx++;
                        }
                        rowIdx++;
                        this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "'");

                        int h = attr.UIHeight + 20;
                        if (attr.UIIsEnable)
                            this.Add("<TD height='" + h.ToString() + "px'  colspan=4 width='100%' valign=top align=left>");
                        else
                            this.Add("<TD height='" + h.ToString() + "px'  colspan=4 width='100%' valign=top class=TBReadonly>");

                        this.Add("<div style='font-size:12px;color:black;' >");
                        Label lab = new Label();
                        lab.ID = "Lab" + attr.KeyOfEn;
                        lab.Text = attr.Name;
                        this.Add(lab);
                        this.Add("</div>");
                        if (attr.TBModel == 2)
                        {
                            //富文本输出.
                            this.AddRichTextBox(en, attr);
                        }
                        else
                        {
                            TB mytbLine = new TB();
                            mytbLine.TextMode = TextBoxMode.MultiLine;
                            mytbLine.ID = "TB_" + attr.KeyOfEn;
                            mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn).Replace("\\n", "\n");

                            mytbLine.Enabled = attr.UIIsEnable;
                            if (mytbLine.Enabled == false)
                                mytbLine.Attributes.Add("readonly", "true");
                            else
                                mytbLine.Attributes["class"] = "TBDoc";

                            mytbLine.Attributes["style"] = "width:98%;height:" + attr.UIHeight + "px;padding: 0px;margin: 0px;";
                            this.Add(mytbLine);

                            if (mytbLine.Enabled)
                            {
                                string ctlID = mytbLine.ClientID;
                                Label mylab = this.GetLabelByID("Lab" + attr.KeyOfEn);
                                mylab.Text = "<a href=\"javascript:TBHelp('" + ctlID + "','" + appPath + "','" + enName + "','" + attr.KeyOfEn + "')\">" + attr.Name + "</a>";
                            }
                        }

                        this.AddTDEnd();
                        this.AddTREnd();
                        rowIdx++;
                        isLeftNext = true;
                        continue;
                    }

                    if (attr.IsBigDoc)
                    {
                        if (isLeftNext)
                        {
                            rowIdx++;
                            this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                        }

                        this.Add("<TD class=FDesc colspan=2 height='" + attr.UIHeight.ToString() + "px' >");
                        this.Add(attr.Name);
                        TB mytbLine = new TB();
                        mytbLine.ID = "TB_" + attr.KeyOfEn;
                        mytbLine.TextMode = TextBoxMode.MultiLine;
                        mytbLine.Attributes["class"] = "TBDoc";
                        mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        if (mytbLine.Enabled == false)
                        {
                            mytbLine.Attributes["class"] = "TBReadonly";
                            mytbLine.Attributes.Add("readonly", "true");
                        }
                        mytbLine.Attributes["style"] = "width:98%;height:100%;padding: 0px;margin: 0px;";
                        this.Add(mytbLine);
                        this.AddTDEnd();
                        if (isLeftNext == false)
                        {
                            this.AddTREnd();
                            rowIdx++;
                        }
                        isLeftNext = !isLeftNext;
                        continue;
                    }

                    //计算 colspanOfCtl .
                    int colspanOfCtl = 1;
                    if (attr.UIIsLine)
                        colspanOfCtl = 3;

                    if (attr.UIIsLine)
                    {
                        if (isLeftNext == false)
                        {
                            this.AddTD();
                            this.AddTD();
                            this.AddTREnd();
                            rowIdx++;
                        }
                        isLeftNext = true;
                    }

                    if (isLeftNext)
                    {
                        rowIdx++;
                        this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                    }

                    if (attr.UIIsEnable == false)
                    {
                        if (this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                        {
                            MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                            string url = meLink.Tag;
                            if (url.Contains("?") == false)
                                url = url + "?a3=2";
                            url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
                            if (url.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr item in attrs)
                                {
                                    url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                                    if (url.Contains("@") == false)
                                        break;
                                }
                            }
                            this.AddTD("colspan=" + colspanOfCtl, "<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                            continue;
                        }
                    }
                    
                    TB tb = new TB();
                    tb.ID = "TB_" + attr.KeyOfEn;
                    tb.Enabled = attr.UIIsEnable;
                    //add by dgq 2013-4-11 添加事件
                    if (attr.UIIsEnable == true)
                    {
                        //add by dgq 2013-4-9 添加修改事件
                        //tb.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                    }
                    #region add contrals.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                    this.AddTDDesc(attr.Name);
                                    if (attr.IsSigan)
                                    {
                                        string v = en.GetValStrByKey(attr.KeyOfEn);
                                        if (v.Length == 0)
                                            this.AddTD("<img src='/DataUser/Siganture/" + WebUser.No + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                                        else
                                            this.AddTD("<img src='/DataUser/Siganture/" + v + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                                    }
                                    else
                                    {
                                        tb.ShowType = TBType.TB;
                                        tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                        if (colspanOfCtl == 3)
                                            this.AddTD(" width='80%' colspan=" + colspanOfCtl, tb);
                                        else
                                            this.AddTD(" width='40%' colspan=" + colspanOfCtl, tb);
                                    }
                                    break;
                                case BP.DA.DataType.AppDate:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Date;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker();";

                                    this.AddTD("  width='40%' colspan=" + colspanOfCtl, tb);
                                    break;
                                case BP.DA.DataType.AppDateTime:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.DateTime;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                                    this.AddTD("  width='40%' colspan=" + colspanOfCtl, tb);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    this.AddTDDesc("");
                                    CheckBox cb = new CheckBox();
                                    cb.Text = attr.Name;
                                    cb.ID = "CB_" + attr.KeyOfEn;
                                    cb.Checked = attr.DefValOfBool;
                                    cb.Enabled = attr.UIIsEnable;
                                    cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                    if (attr.UIIsEnable)
                                    {
                                        //add by dgq 2013-4-9,添加内容修改后的事件
                                        //cb.Attributes["onmousedown"] = "Change('" + attr.FK_MapData + "')";
                                    }
                                    this.AddTD("  width='40%' colspan=" + colspanOfCtl, cb);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppFloat:
                                case BP.DA.DataType.AppInt:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Num;
                                    tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                    this.AddTD("  width='40%' colspan=" + colspanOfCtl, tb);
                                    break;
                                case BP.DA.DataType.AppMoney:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                    this.AddTD("width='40%' colspan=" + colspanOfCtl, tb);
                                    break;
                                case BP.DA.DataType.AppRate:
                                    this.AddTDDesc(attr.Name);
                                    tb.ShowType = TBType.Moneny;
                                    tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                    this.AddTD(" width='40%' colspan=" + colspanOfCtl, tb);
                                    break;
                                default:
                                    break;
                            }
                            // tb.Attributes["width"] = "100%";
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDateTime:
                                case BP.DA.DataType.AppDate:
                                    if (tb.Enabled)
                                    {
                                        // tb.Columns = attr.UIWidth;
                                        tb.MaxLength = attr.MaxLen;
                                        //tb.Attributes["class"] = "TB";
                                    }
                                    else
                                    {
                                        tb.Attributes["class"] = "TBReadonly";
                                    }
                                    break;
                                default:
                                    if (tb.Enabled)
                                        tb.Attributes["class"] = "TBNum";
                                    else
                                        tb.Attributes["class"] = "TBNumReadonly";
                                    break;
                            }
                            break;
                        case FieldTypeS.Enum:
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                this.AddTDDesc(attr.Name);
                                DDL ddle = new DDL();
                                ddle.ID = "DDL_" + attr.KeyOfEn;
                                ddle.BindSysEnum(attr.KeyOfEn);
                                ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                                ddle.Enabled = attr.UIIsEnable;
                                if (attr.UIIsEnable)
                                {
                                    //add by dgq 2013-4-9,添加内容修改后的事件
                                    //ddle.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                                }
                                this.AddTD("colspan=" + colspanOfCtl, ddle);
                            }
                            else
                            {
                                this.AddTDDesc(attr.Name);
                                this.Add("<TD class=TD colspan='" + colspanOfCtl + "'>");
                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum item in ses)
                                {
                                    RadioButton rb = new RadioButton();
                                    rb.ID = "RB_" + attr.KeyOfEn + "_" + item.IntKey;
                                    rb.Text = item.Lab;
                                    if (item.IntKey == en.GetValIntByKey(attr.KeyOfEn))
                                        rb.Checked = true;
                                    else
                                        rb.Checked = false;
                                    rb.GroupName = attr.KeyOfEn;
                                    this.Add(rb);
                                }
                                this.AddTDEnd();
                            }
                            break;
                        case FieldTypeS.FK:
                            this.AddTDDesc(attr.Name);
                            DDL ddl1 = new DDL();
                            ddl1.ID = "DDL_" + attr.KeyOfEn;
                            try
                            {
                                EntitiesNoName ens = attr.HisEntitiesNoName;
                                ens.RetrieveAll();
                                ddl1.BindEntities(ens);
                                ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            }
                            catch
                            {
                            }
                            ddl1.Enabled = attr.UIIsEnable;
                            if (attr.UIIsEnable)
                            {
                                //add by dgq 2013-4-9,添加内容修改后的事件
                                //ddl1.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                            }
                            this.AddTD("colspan=" + colspanOfCtl, ddl1);
                            break;
                        default:
                            break;
                    }
                    #endregion add contrals.

                    #endregion 加入字段

                    #region 尾后处理。
                    if (colspanOfCtl == 3)
                    {
                        isLeftNext = true;
                        this.AddTREnd();
                        continue;
                    }

                    if (isLeftNext == false)
                    {
                        isLeftNext = true;
                        this.AddTREnd();
                        continue;
                    }
                    isLeftNext = false;
                    #endregion add contrals.
                }
                // 最后处理补充上它。
                if (isLeftNext == false)
                {
                    this.AddTD();
                    this.AddTD();
                    this.AddTREnd();
                }
                this.InsertObjects(false);
            }

            this.AddTableEnd();


            #region 处理iFrom 的自适应的问题。
            string js = "\t\n<script type='text/javascript' >";
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                js += "\t\n window.setInterval(\"ReinitIframe('F" + dtl.No + "','TD" + dtl.No + "')\", 200);";
            }
            foreach (MapFrame fr in frames)
            {
                //  if (fr.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + fr.NoOfObj + "','TD" + fr.NoOfObj + "')\", 200);";
            }
            foreach (MapM2M m2m in m2ms)
            {
                //  if (m2m.ShowWay == FrmShowWay.FrmAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + m2m.NoOfObj + "','TD" + m2m.NoOfObj + "')\", 200);";
            }
            foreach (FrmAttachment ath in aths)
            {
                // if (ath.IsAutoSize)
                js += "\t\n window.setInterval(\"ReinitIframe('F" + ath.MyPK + "','TD" + ath.MyPK + "')\", 200);";
            }
            js += "\t\n</script>";
            this.Add(js);
            #endregion 处理iFrom 的自适应的问题。

            // 处理扩展。
            this.AfterBindEn_DealMapExt(enName, mattrs, en);
            if (this.IsReadonly == false)
            {
                #region 处理iFrom SaveDtlData。
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                //    js += "\t\n    GenerPageKVs(); //调用产生kvs ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveDtlData(); ";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom SaveDtlData。

                #region 处理iFrom  SaveM2M Save
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";
                js += "\t\n</script>";
                this.Add(js);
                #endregion 处理iFrom  SaveM2M Save。
            }
        }
        private void AfterBindEn_DealMapExt(string enName, MapAttrs mattrs, Entity en)
        {
            #region 处理事件.
            if (dtls.Count >= 1)
            {
                string scriptSaveDtl = "";
                scriptSaveDtl = "\t\n<script type='text/javascript' >";
                scriptSaveDtl += "\t\n function SaveDtlAll(){ ";

                foreach (MapDtl dtl in dtls)
                {
                    if (dtl.IsUpdate == true || dtl.IsInsert == true)
                    {
                        scriptSaveDtl += "\t\n try{  ";

                        if (dtl.HisDtlShowModel == DtlShowModel.Table)
                            scriptSaveDtl += "\t\n  SaveDtl('" + dtl.No + "'); ";

                        scriptSaveDtl += "\t\n } catch(e) { ";
                        scriptSaveDtl += "\t\n  alert(e.name  + e.message);  return false;";
                        scriptSaveDtl += "\t\n } ";
                    }
                }

                scriptSaveDtl += "\t\n  return true; } ";
                scriptSaveDtl += "\t\n</script>";

                this.Add(scriptSaveDtl);
            }
            else
            {
                string scriptSaveDtl = "";
                scriptSaveDtl = "\t\n<script type='text/javascript' >";
                scriptSaveDtl += "\t\n function SaveDtlAll() { ";
                scriptSaveDtl += "\t\n return true; } ";
                scriptSaveDtl += "\t\n</script>";
                this.Add(scriptSaveDtl);
            }


            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadAfter, en);
                    if (msg != null)
                        this.Alert(msg);
                }
                catch (Exception ex)
                {
                    this.Alert("载入之前错误:" + ex.Message);
                    return;
                }
            }
            #endregion 处理事件.

            #region 处理扩展设置
            if (mes.Count != 0)
            {
                #region load js.
                this.Page.RegisterClientScriptBlock("s4",
              "<script language='JavaScript' src='/WF/Scripts/jquery-1.4.1.min.js' ></script>");

                this.Page.RegisterClientScriptBlock("b7",
             "<script language='JavaScript' src='/WF/CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");

                this.Page.RegisterClientScriptBlock("y7",
            "<script language='JavaScript' src='/DataUser/JSLibData/" + enName + ".js' ></script>");

                this.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
                #endregion load js.

                #region 首先处理自动填充，下拉框数据。
                foreach (MapExt me in mes)
                {
                    switch (me.ExtType)
                    {
                        case MapExtXmlList.AutoFullDLL: // 自动填充下拉框.
                            DDL ddlFull = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            if (ddlFull == null)
                            {
                                me.Delete();
                                continue;
                            }

                            string valOld = ddlFull.SelectedItemStringVal;
                            string fullSQL = me.Doc.Replace("@WebUser.No", WebUser.No);
                            fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                            fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

                            if (fullSQL.Contains("@"))
                            {
                                Attrs attrs = en.EnMap.Attrs;
                                foreach (Attr attr in attrs)
                                {
                                    if (fullSQL.Contains("@") == false)
                                        break;
                                    fullSQL = fullSQL.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                                }
                            }
                            ddlFull.Items.Clear();
                            ddlFull.Bind(DBAccess.RunSQLReturnTable(fullSQL), "No", "Name");
                            ddlFull.SetSelectItem(en.GetValStrByKey(me.AttrOfOper));
                            break;
                        default:
                            break;
                    }
                }
                #endregion 首先处理自动填充，下拉框数据。

                #region 在处理其它。
                System.Data.DataTable dt = new DataTable();
                foreach (MapExt me in mes)
                {
                    switch (me.ExtType)
                    {
                        case MapExtXmlList.DDLFullCtrl: // 自动填充其他的控件..
                            DDL ddlOper = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            if (ddlOper == null)
                                continue;
                            ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";

                            if (me.Tag != "")
                            {
                                /* 处理下拉框的选择范围的问题 */
                                string[] strs = me.Tag.Split('$');
                                foreach (string str in strs)
                                {
                                    string[] myCtl = str.Split(':');
                                    string ctlID = myCtl[0];
                                    DDL ddlC1 = this.GetDDLByID("DDL_" + ctlID);
                                    if (ddlC1 == null)
                                    {
                                        //me.Tag = "";
                                        //me.Update();
                                        continue;
                                    }

                                    string sql = myCtl[1].Replace("~", "'");
                                    sql = sql.Replace("@WebUser.No", WebUser.No);
                                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                    sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                                    sql = sql.Replace("@Key", ddlOper.SelectedItemStringVal.Trim());
                                    if (sql.Contains("@"))
                                    {
                                        foreach (MapAttr attr in mattrs)
                                        {
                                            if (sql.Contains("@" + attr.KeyOfEn) == false)
                                                continue;
                                            sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));

                                            if (sql.Contains("@") == false)
                                                break;
                                        }
                                    }

                                    dt = DBAccess.RunSQLReturnTable(sql);
                                    string valC1 = ddlC1.SelectedItemStringVal;
                                    ddlC1.Items.Clear();
                                    foreach (DataRow dr in dt.Rows)
                                        ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                    ddlC1.SetSelectItem(valC1);
                                }
                            }

                            break;
                        case MapExtXmlList.ActiveDDL: /*自动初始化ddl的下拉框数据.*/
                            DDL ddlPerant = this.GetDDLByID("DDL_" + me.AttrOfOper);
                            DDL ddlChild = this.GetDDLByID("DDL_" + me.AttrsOfActive);
                            if (ddlPerant == null || ddlChild == null)
                                continue;
                            ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value,\'" + ddlChild.ClientID + "\', \'" + me.MyPK + "\')";
                            // 处理默认选择。
                            string val = ddlPerant.SelectedItemStringVal;
                            string valClient = en.GetValStrByKey(me.AttrsOfActive); // ddlChild.SelectedItemStringVal;

                            string fullSQL = me.Doc.Replace("@WebUser.No", WebUser.No);
                            fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                            fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                            fullSQL = fullSQL.Replace("@Key", val);

                            if (fullSQL.Contains("@"))
                            {
                                foreach (MapAttr attr in mattrs)
                                {
                                    if (fullSQL.Contains("@" + attr.KeyOfEn) == false)
                                        continue;

                                    fullSQL = fullSQL.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));

                                    if (fullSQL.Contains("@") == false)
                                        break;
                                }
                            }

                            dt = DBAccess.RunSQLReturnTable(fullSQL);
                            // ddlChild.Items.Clear();
                            foreach (DataRow dr in dt.Rows)
                                ddlChild.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));

                            ddlChild.SetSelectItem(valClient);
                            break;
                        case MapExtXmlList.AutoFullDLL: // 自动填充下拉框.
                            continue; //已经处理了。
                        case MapExtXmlList.TBFullCtrl: // 自动填充.
                            TextBox tbAuto = this.GetTextBoxByID("TB_" + me.AttrOfOper);
                            if (tbAuto == null)
                                continue;

                            // onpropertychange
                            // tbAuto.Attributes["onpropertychange"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["onkeydown"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["onkeyup"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            // tbAuto.Attributes["ondblclick"] = "ReturnValTBFullCtrl(this,'" + me.MyPK + "','sd');";

                            tbAuto.Attributes["ondblclick"] = "ReturnValTBFullCtrl(this,'" + me.MyPK + "');";
                            tbAuto.Attributes["onkeyup"] = "DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                            tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                            if (me.Tag != "")
                            {
                                /* 处理下拉框的选择范围的问题 */
                                string[] strs = me.Tag.Split('$');
                                foreach (string str in strs)
                                {
                                    string[] myCtl = str.Split(':');
                                    string ctlID = myCtl[0];
                                    DDL ddlC1 = this.GetDDLByID("DDL_" + ctlID);
                                    if (ddlC1 == null)
                                    {
                                        //me.Tag = "";
                                        //me.Update();
                                        continue;
                                    }

                                    string sql = myCtl[1].Replace("~", "'");
                                    sql = sql.Replace("@WebUser.No", WebUser.No);
                                    sql = sql.Replace("@WebUser.Name", WebUser.Name);
                                    sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                    sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                                    sql = sql.Replace("@Key", tbAuto.Text.Trim());
                                    if (sql.Contains("@"))
                                    {
                                        foreach (MapAttr attr in mattrs)
                                        {
                                            if (sql.Contains("@" + attr.KeyOfEn) == false)
                                                continue;
                                            sql = sql.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));

                                            if (sql.Contains("@") == false)
                                                break;
                                        }
                                    }

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
                            TextBox tbJS = this.GetTextBoxByID("TB_" + me.AttrOfOper);
                            if (tbJS != null)
                            {
                                tbJS.Attributes[me.Tag2] = me.Tag1 + "(this);";
                            }
                            else
                            {
                                DDL ddl = this.GetDDLByID("DDL_" + me.AttrOfOper);
                                if (ddl != null)
                                    ddl.Attributes[me.Tag2] = me.Tag1 + "(this);";
                            }
                            break;
                        case MapExtXmlList.PopVal: // 弹出窗.
                            TB tb = this.GetTBByID("TB_" + me.AttrOfOper);
                            if (tb == null)
                                continue;

                            if (me.PopValWorkModel == 0)
                                tb.Attributes["ondblclick"] = "ReturnVal(this,'" + me.Doc + "','sd');";
                            else
                                tb.Attributes["ondblclick"] = "ReturnValCCFormPopVal(this,'" + me.MyPK + "','" + en.PKVal + "');";
                            break;
                        default:
                            break;
                    }
                }
                #endregion 在处理其它。
            }
            #endregion 处理扩展设置

            #region 处理 JS 自动计算.
            string js = "\t\n <script type='text/javascript' >oid=" + en.PKVal + ";</script>";
            for (int i = 0; i < mattrs.Count; i++)
            {
                MapAttr attr = mattrs[i] as MapAttr;
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                switch (attr.HisAutoFull)
                {
                    case AutoFullWay.Way1_JS:
                        js += "\t\n <script type='text/javascript' >";
                        TB tb = null;
                        try
                        {
                            tb = this.GetTBByID("TB_" + attr.KeyOfEn);
                            if (tb == null)
                                continue;
                        }
                        catch
                        {
                            continue;
                        }

                        string left = "\n  document.forms[0]." + tb.ClientID + ".value = ";
                        string right = attr.AutoFullDoc;
                        foreach (MapAttr mattr in mattrs)
                        {
                            if (mattr.IsNum == false)
                                continue;

                            if (attr.AutoFullDoc.Contains("@" + mattr.KeyOfEn)
                                || attr.AutoFullDoc.Contains("@" + mattr.Name))
                            {
                            }
                            else
                            {
                                continue;
                            }

                            string tbID = "TB_" + mattr.KeyOfEn;
                            TB mytb = this.GetTBByID(tbID);
                            this.GetTBByID(tbID).Attributes["onkeyup"] = "javascript:Auto" + attr.KeyOfEn + "();";

                            right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                            right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + mytb.ClientID + ".value.replace( ',' ,  '' ) ) ");
                        }

                        js += "\t\n function Auto" + attr.KeyOfEn + "() { ";
                        js += left + right + ";";
                        js += " \t\n  document.forms[0]." + tb.ClientID + ".value= VirtyMoney(document.forms[0]." + tb.ClientID + ".value ) ;";
                        js += "\t\n } ";
                        js += "\t\n</script>";
                        continue;
                    default:
                        continue;
                }
            }
            this.Add(js);
            #endregion 处理 JS 自动计算.
        }
        public void InsertObjects(bool isJudgeRowIdx)
        {
            #region 从表
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false || this.ctrlUseSta.Contains(dtl.No))
                    continue;

                if (dtl.GroupID == 0)
                {
                    dtl.GroupID = currGF.OID;
                    dtl.RowIdx = 0;
                    dtl.Update();
                }

                if (isJudgeRowIdx)
                {
                    if (dtl.RowIdx != rowIdx)
                        continue;
                }

                if (dtl.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }

                // dtl.IsUse = true;

                this.ctrlUseSta += dtl.No;

                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=4 ID='TD" + dtl.No + "' height='50px' width='100%' style='align:left'>");
                string src = "";
                try
                {
                    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&FID=" + this.HisEn.GetValStringByKey("FID") + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }
                catch
                {
                    src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + this.HisEn.PKVal + "&IsWap=0&FK_Node=" + dtl.FK_MapData.Replace("ND", "");
                }

                if (this.IsReadonly || dtl.IsReadonly)
                    this.Add("<iframe ID='F" + dtl.No + "'  src='" + src + "&IsReadonly=1' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='30px' /></iframe>");
                else
                    this.Add("<iframe ID='F" + dtl.No + "'   Onblur=\"SaveDtl('" + dtl.No + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' /></iframe>");

                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 从表

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                if (this.ctrlUseSta.Contains("@" + m2m.MyPK))
                    continue;

                if (isJudgeRowIdx)
                {
                    if (m2m.RowIdx != rowIdx)
                        continue;
                }

                if (m2m.GroupID == 0 && rowIdx == 0)
                {
                    m2m.GroupID = currGF.OID;
                    m2m.RowIdx = 0;
                    m2m.Update();
                }
                else if (m2m.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }

                this.ctrlUseSta += "@" + m2m.MyPK;


                rowIdx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");

                string src = "/WF/CCForm/M2M.aspx?NoOfObj=" + m2m.NoOfObj;
                string paras = this.RequestParas;
                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");

                src += "&r=q" + paras;
                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + m2m.FK_MapData;
                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                        this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='20px' width='100%'  >");
                        if (m2m.HisM2MType == M2MType.M2M)
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "' src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=no /></iframe>");
                        break;
                    case FrmShowWay.FrmSpecSize:
                        this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='" + m2m.H + "' width='" + m2m.W + "'  >");
                        if (m2m.HisM2MType == M2MType.M2M)
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'    src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "' scrolling=auto /></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<TD colspan=4 ID='TD" + m2m.NoOfObj + "' height='20px' width='100%'  >");
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
            }
            #endregion 多对多的关系

            #region 框架
            foreach (MapFrame fram in frames)
            {
                if (this.ctrlUseSta.Contains("@" + fram.MyPK))
                    continue;

                if (isJudgeRowIdx)
                {
                    if (fram.RowIdx != rowIdx)
                        continue;
                }

                if (fram.GroupID == 0 && rowIdx == 0)
                {
                    fram.GroupID = currGF.OID;
                    fram.RowIdx = 0;
                    fram.Update();
                }
                else if (fram.GroupID == currGF.OID)
                {
                }
                else
                {
                    continue;
                }

                this.ctrlUseSta += "@" + fram.MyPK;
                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                if (fram.IsAutoSize)
                    this.Add("<TD colspan=4 ID='TD" + fram.NoOfObj + "' height='50px' width='100%'  >");
                else
                    this.Add("<TD colspan=4 ID='TD" + fram.NoOfObj + "' height='" + fram.H + "' width='" + fram.W + "'  >");

                string paras = this.RequestParas;
                if (paras.Contains("FID=") == false)
                    paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                if (paras.Contains("WorkID=") == false)
                    paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");

                string src = fram.URL;
                if (src.Contains("?"))
                    src += "&r=q" + paras;
                else
                    src += "?r=q" + paras;

                if (fram.IsAutoSize)
                {
                    this.Add("<iframe ID='F" + fram.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=auto /></iframe>");
                }
                else
                {
                    this.Add("<iframe ID='F" + fram.NoOfObj + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + fram.W + "' height='" + fram.H + "' scrolling=auto /></iframe>");
                }

                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 框架

            #region 附件
            foreach (BP.Sys.FrmAttachment ath in aths)
            {
                if (this.ctrlUseSta.Contains("@" + ath.MyPK))
                    continue;
                if (isJudgeRowIdx)
                {
                    if (ath.RowIdx != rowIdx)
                        continue;
                }

                if (ath.GroupID == 0 && rowIdx == 0)
                {
                    ath.GroupID = currGF.OID;
                    ath.RowIdx = 0;
                    ath.Update();
                }
                else if (ath.GroupID == currGF.OID)
                {
                }
                else
                {
                    continue;
                }
                this.ctrlUseSta += "@" + ath.MyPK;
                rowIdx++;
                // myidx++;
                this.AddTR(" ID='" + currGF.Idx + "_" + rowIdx + "' ");
                this.Add("<TD colspan=4 ID='TD" + ath.MyPK + "' height='50px' width='100%' style='align:left'>");
                string src = "";
                if (this.IsReadonly)
                    src = "./CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                else
                    src = "./CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal + "&Ath=" + ath.NoOfObj + "&FK_MapData=" + EnName + "&FK_FrmAttachment=" + ath.MyPK;


                if (ath.IsAutoSize)
                {
                    this.Add("<iframe ID='F" + ath.MyPK + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='100%' height='10px' scrolling=auto /></iframe>");
                }
                else
                {
                    this.Add("<iframe ID='F" + ath.MyPK + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ath.W + "' height='" + ath.H + "' scrolling=auto /></iframe>");
                }
                this.AddTDEnd();
                this.AddTREnd();
            }
            #endregion 附件
        }
        public void AddRichTextBox(Entity en, MapAttr attr)
        {
            /*说明这是富文本输出*/
            this.Page.RegisterClientScriptBlock("c51",
     "<script language='JavaScript' src='/WF/Comm/kindeditor/kindeditor-all.js'  charset='utf-8' ></script>");

            this.Page.RegisterClientScriptBlock("c2",
    "<script language='JavaScript' src='/WF/Comm/kindeditor/lang/zh_CN.js'  charset='utf-8' ></script>");

            this.Page.RegisterClientScriptBlock("c53",
  "<script language='JavaScript' src='/WF/Comm/kindeditor/plugins/code/prettify.js'  charset='utf-8' ></script>");


            this.Page.RegisterClientScriptBlock("s51",
    "<link href='/WF/Comm/kindeditor/plugins/code/prettify.css' rel='stylesheet' type='text/css' />");

            this.Page.RegisterClientScriptBlock("s52",
    "<link href='/WF/Comm/kindeditor/themes/default/default.css' rel='stylesheet' type='text/css' />");

            string strs = "\t\n <script>";
            strs += "\t\n  var editor1; ";
            strs += "\t\n KindEditor.ready(function(K) {";
            strs += "\t\n var fID='TB_" + attr.KeyOfEn + "'; ";
            strs += "\t\n var tbID='ContentPlaceHolder1_MyFlowUC1_MyFlow1_UCEn1_'+fID;";

            strs += "\t\n var ctrl =document.getElementById( tbID);";
            strs += "\t\n if (ctrl==null)";
            strs += "\t\n tbID = 'ContentPlaceHolder1_UCEn1_' + fID;";

            strs += "\t\n ctrl =document.getElementById( tbID);";
            strs += "\t\n if (ctrl == null) { ";
            strs += "\t\n tbID = 'ContentPlaceHolder1_WFRpt1_UCEn1_' + fID;";
            strs += "\t\n ctrl =document.getElementById( tbID);";
            strs += "\t\n  } ";

            strs += "\t\n if (ctrl == null) { ";
            strs += "\t\n     alert('没有找到要帮定的控件'); ";
            strs += "\t\n  } ";

            strs += "\t\n   editor1 = K.create('#'+tbID, {";
            strs += "\t\n cssPath : '/WF/Comm/kindeditor/plugins/code/prettify.css',";
            strs += "\t\n uploadJson : '/WF/Comm/kindeditor/asp.net/upload_json.ashx',";
            strs += "\t\n fileManagerJson : '/WF/Comm/kindeditor/asp.net/file_manager_json.ashx',";
            strs += "\t\n allowFileManager : true,";

            strs += "\t\n width : '100%',";
            //strs += "\t\n width : '" + attr.UIWidth + "px',";

            strs += "\t\n height : '" + attr.UIHeight + "px'";

            strs += "\t\n });";
            strs += "\t\n });";

            //strs += "\t\n KindEditor.show(function(K) {";
            //strs += "\t\n KindEditor.ready(function(K) {";

            strs += "\t\n </script>";
            this.Add(strs);

            TB tbd = new TB();
            tbd.TextMode = TextBoxMode.MultiLine;
            tbd.ID = "TB_" + attr.KeyOfEn;
            tbd.Text = en.GetValStrByKey(attr.KeyOfEn);
            tbd.TextMode = TextBoxMode.MultiLine;
            tbd.Attributes["style"] = "width:" + attr.UIWidth + "px;height:" + attr.UIHeight + "px;visibility:hidden;";
            this.Add(tbd);

        }
        #endregion

        #region 输出自由格式的表单.
        public string FK_MapData = null;
        FrmEvents fes = null;
        public string EnName = null;
        public string LinkFields = "";
        public MapData mapData = null;

        private void DealDefVal(MapAttrs mattrs)
        {
            if (this.IsReadonly)
                return;

            this.Page.RegisterClientScriptBlock("y7",
          "<script language='JavaScript' src='" + this.Request.ApplicationPath + "DataUser/JSLibData/" + this.EnName + "_Self.js' ></script>");

            this.Page.RegisterClientScriptBlock("yd7",
      "<script language='JavaScript' src='/DataUser/JSLibData/" + this.EnName + ".js' ></script>");

            foreach (MapAttr attr in mattrs)
            {
                if (attr.DefValReal.Contains("@") == false)
                    continue;
                this.HisEn.SetValByKey(attr.KeyOfEn, attr.DefVal);
            }
        }
        public void BindCCForm(Entity en, string enName, bool isReadonly)
        {
            this.ctrlUseSta = "";

            this.EnName = enName;
            this.mapData = new MapData(enName);
            string appPath = this.Request.ApplicationPath;

            mes = this.mapData.MapExts;
            this.IsReadonly = isReadonly;
            this.FK_MapData = enName;
            this.HisEn = en;
            this.EnName = enName;
            this.m2ms = this.mapData.MapM2Ms;
            this.dtls = this.mapData.MapDtls;
            this.mes = this.mapData.MapExts;

            #region 处理事件.
            fes = this.mapData.FrmEvents;
            if (this.IsPostBack == false)
            {
                try
                {
                    string msg = fes.DoEventNode(FrmEventList.FrmLoadBefore, en);
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

            MapAttrs mattrs = this.mapData.MapAttrs;
            this.DealDefVal(mattrs);

            //处理装载前填充.
            this.LoadData(mattrs, en);

            #region 输出Ele
            FrmEles eles = this.mapData.FrmEles;
            if (eles.Count >= 1)
            {
                string myjs = "\t\n<script type='text/javascript' >";
                myjs += "\t\n function BPPaint(ctrl,url,w,h,fk_FrmEle)";
                myjs += "\t\n {";
                myjs += "\t\n  var v= window.showModalDialog(url, 'ddf', 'dialogHeight: '+h+'px; dialogWidth: '+w+'px;center: yes; help: no'); ";
                myjs += "\t\n  if (v==null )  ";
                myjs += "\t\n     return ; ";

                ////  myjs += "\t\n     alert(document.getElementById('Ele'+fk_FrmEle ));";
                //  myjs += "\t\n  ctrl.src='dsdsd'; ";
                // myjs += "\t\n  alert('已经执行成功，谢谢使用')";
                //  myjs += "\t\n  ctrl.src=v; ";
                myjs += "\t\n  ctrl.src=v+'?temp='+new Date(); ";

                //   myjs += "\t\n  alert(ctrl.src)";
                //myjs += "\t\n  ctrl.setAttribute('src',v); ";
                //myjs += "\t\n  document.getElementById('Ele'+fk_FrmEle ).src=v; ";
                //myjs += "\t\n  document.getElementById('Ele'+fk_FrmEle ).setAttribute('src', v); ";
                myjs += "\t\n }";
                myjs += "\t\n</script>";
                this.Add(myjs);

                FrmEleDBs dbs = new FrmEleDBs(this.FK_MapData, en.PKVal.ToString());
                foreach (FrmEle ele in eles)
                {
                    float y = ele.Y;
                    this.Add("\t\n<DIV id=" + ele.MyPK + " style='position:absolute;left:" + ele.X + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                    switch (ele.EleType)
                    {
                        case FrmEle.HandSiganture:
                            FrmEleDB db = dbs.GetEntityByKey(FrmEleDBAttr.EleID, ele.EleID) as FrmEleDB;
                            string dbFile = appPath + "/DataUser/BPPaint/Def.png";
                            if (db != null)
                                dbFile = db.Tag1;

                            if (this.IsReadonly || ele.IsEnable == false)
                            {
                                this.Add("\t\n<img src='" + dbFile + "' onerror=\"this.src='" + appPath + "/DataUser/BPPaint/Def.png'\" style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            }
                            else
                            {
                                string url = appPath + "WF/CCForm/BPPaint.aspx?W=" + ele.HandSiganture_WinOpenW + "&H=" + ele.HandSiganture_WinOpenH + "&MyPK=" + ele.PKVal + "&PKVal=" + en.PKVal;
                                //  myjs = "javascript:window.showModalDialog('"+url+"', 'sdf', 'dialogHeight: " + ele.HandSiganture_WinOpenH + "px; dialogWidth: " + ele.HandSiganture_WinOpenW + "px;center: yes; help: no');";
                                myjs = "javascript:BPPaint(this,'" + url + "','" + ele.HandSiganture_WinOpenW + "','" + ele.HandSiganture_WinOpenH + "','" + ele.MyPK + "');";
                                //string myjs = "javascript:window.open('" + appPath + "WF/CCForm/BPPaint.aspx?PKVal=" + en.PKVal + "&MyPK=" + ele.MyPK + "&H=" + ele.HandSiganture_WinOpenH + "&W=" + ele.HandSiganture_WinOpenW + "', 'sdf', 'dialogHeight: " + ele.HandSiganture_WinOpenH + "px; dialogWidth: " + ele.HandSiganture_WinOpenW + "px;center: yes; help: no');";
                                this.Add("\t\n<img id='Ele" + ele.MyPK + "' onclick=\"" + myjs + "\" onerror=\"this.src='" + appPath + "/DataUser/BPPaint/Def.png'\" src='" + dbFile + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + ele.W + "px;height:" + ele.H + "px;' />");
                            }
                            break;
                        case FrmEle.iFrame:
                            string paras = this.RequestParas;
                            if (paras.Contains("FID=") == false)
                                paras += "&FID=" + this.HisEn.GetValStrByKey("FID");

                            if (paras.Contains("WorkID=") == false)
                                paras += "&WorkID=" + this.HisEn.GetValStrByKey("OID");

                            string src = ele.Tag1.Clone() as string; // url 
                            if (src.Contains("?"))
                                src += "&r=q" + paras;
                            else
                                src += "?r=q" + paras;

                            if (src.Contains("UserNo") == false)
                                src += "&UserNo=" + WebUser.No;
                            if (src.Contains("SID") == false)
                                src += "&SID=" + WebUser.SID;
                            if (src.Contains("@"))
                            {
                                foreach (Attr m in en.EnMap.Attrs)
                                {
                                    if (src.Contains("@") == false)
                                        break;
                                    src = src.Replace("@" + m.Key, en.GetValStrByKey(m.Key));
                                }
                            }
                            this.Add("<iframe ID='F" + ele.EleID + "'   src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + ele.W + "' height='" + ele.H + "' scrolling=auto /></iframe>");
                            break;
                        case FrmEle.EleSiganture:
                        default:
                            this.Add("未处理");
                            break;
                    }
                }
                this.Add("\t\n</DIV>");
            }
            #endregion 输出Ele

            #region 输出按钮
            FrmBtns btns = this.mapData.FrmBtns;
            foreach (FrmBtn btn in btns)
            {
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + btn.X + "px;top:" + btn.Y + "px;text-align:left;' >");
                this.Add("\t\n<span >");
                switch (btn.HisBtnEventType)
                {
                    case BtnEventType.Disable:
                        this.Add("<input type=button class=Btn value='" + btn.Text.Replace("&nbsp;", " ") + "' disabled='disabled'/>");
                        break;
                    case BtnEventType.RunExe:
                    case BtnEventType.RunJS:
                        this.Add("<input type=button class=Btn value=\"" + btn.Text.Replace("&nbsp;", " ") + "\" enable=true onclick=\"" + btn.EventContext.Replace("~", "'") + "\" />");
                        break;
                    default:
                        Button myBtn = new Button();
                        myBtn.Enabled = true;
                        myBtn.CssClass = "Btn";
                        myBtn.ID = btn.MyPK;
                        myBtn.Text = btn.Text.Replace("&nbsp;", " ");
                        myBtn.Click += new EventHandler(myBtn_Click);
                        this.Add(myBtn);
                        break;
                }
                this.Add("\t\n</span>");
                this.Add("\t\n</DIV>");
            }
            #endregion

            #region 输出竖线与标签 & 超连接 Img.
            FrmLabs labs = this.mapData.FrmLabs;
            foreach (FrmLab lab in labs)
            {
                Color col = ColorTranslator.FromHtml(lab.FontColor);
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + lab.X + "px;top:" + lab.Y + "px;text-align:left;' >");
                this.Add("\t\n<span style='color:" + lab.FontColorHtml + ";font-family: " + lab.FontName + ";font-size: " + lab.FontSize + "px;' >" + lab.TextHtml + "</span>");
                this.Add("\t\n</DIV>");
            }

            FrmLines lines = this.mapData.FrmLines;
            foreach (FrmLine line in lines)
            {
                if (line.X1 == line.X2)
                {
                    /* 一道竖线 */
                    float h = line.Y1 - line.Y2;
                    h = Math.Abs(h);
                    if (line.Y1 < line.Y2)
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + line.X1 + "px; top:" + line.Y1 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                    else
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + line.X2 + "px; top:" + line.Y2 + "px; width:" + line.BorderWidth + "px; height:" + h + "px;background-color:" + line.BorderColorHtml + "\" />");
                }
                else
                {
                    /* 一道横线 */
                    float w = line.X2 - line.X1;
                    w = Math.Abs(w);

                    if (line.X1 < line.X2)
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + line.X1 + "px; top:" + line.Y1 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                    else
                        this.Add("\t\n<img id='" + line.MyPK + "'  style=\"padding:0px;position:absolute; left:" + line.X2 + "px; top:" + line.Y2 + "px; width:" + w + "px; height:" + line.BorderWidth + "px;background-color:" + line.BorderColorHtml + "\" />");
                }
            }

            FrmLinks links = this.mapData.FrmLinks;
            foreach (FrmLink link in links)
            {
                string url = link.URL;
                if (url.Contains("@"))
                {
                    foreach (MapAttr attr in mattrs)
                    {
                        if (url.Contains("@") == false)
                            break;
                        url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                    }
                }
                this.Add("\t\n<DIV id=u2 style='position:absolute;left:" + link.X + "px;top:" + link.Y + "px;text-align:left;' >");
                this.Add("\t\n<span style='color:" + link.FontColorHtml + ";font-family: " + link.FontName + ";font-size: " + link.FontSize + "px;' > <a href=\"" + url + "\" target='" + link.Target + "'> " + link.Text + "</a></span>");
                this.Add("\t\n</DIV>");
            }

            FrmImgs imgs = this.mapData.FrmImgs;
            foreach (FrmImg img in imgs)
            {
                float y = img.Y;
                string imgSrc = appPath + "DataUser/ICON/" + BP.SystemConfig.CompanyID + "/LogBiger.png";
                if (img.ImgURL != null && img.ImgURL.Contains("DataUser"))
                {
                    imgSrc = img.ImgURL;
                }
                this.Add("\t\n<DIV id=" + img.MyPK + " style='position:absolute;left:" + img.X + "px;top:" + y + "px;text-align:left;vertical-align:top' >");
                if (string.IsNullOrEmpty(img.LinkURL) == false)
                {
                    this.Add("\t\n<a href='" + img.LinkURL + "' target=" + img.LinkTarget + " ><img src='" + imgSrc + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' /></a>");
                }
                else
                {
                    this.Add("\t\n<img src='" + imgSrc + "' style='padding: 0px;margin: 0px;border-width: 0px;width:" + img.W + "px;height:" + img.H + "px;' />");
                }
                this.Add("\t\n</DIV>");
                //style="position:absolute; left:170px; top:-20px; width:413px; height:478px"  position:absolute;left:" + img.X + "px;top:" + img.Y + "px;
            }
            #endregion 输出竖线与标签

            #region 输出数据控件.
            TB tb = new TB();
            //DDL ddl = new DDL();
            //CheckBox cb = new CheckBox();
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIVisible == false && attr.UIIsEnable)
                {
                    TB tbH = new TB();
                    tbH.Visible = false;
                    tbH.ID = "TB_" + attr.KeyOfEn;
                    tbH.Text = en.GetValStrByKey(attr.KeyOfEn);
                    this.Add(tbH);
                    continue;
                }

                if (attr.UIVisible == false)
                    continue;

                this.Add("<DIV id='F" + attr.KeyOfEn + "' style='position:absolute; left:" + attr.X + "px; top:" + attr.Y + "px; width:" + attr.UIWidth + "px; height:16px;text-align: left;word-break: keep-all;' >");
                this.Add("<span>");

                #region add contrals.
                if (attr.UIIsEnable == false && this.LinkFields.Contains("," + attr.KeyOfEn + ","))
                {
                    MapExt meLink = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.Link) as MapExt;
                    string url = meLink.Tag;
                    if (url.Contains("?") == false)
                        url = url + "?a3=2";
                    url = url + "&WebUserNo=" + WebUser.No + "&SID=" + WebUser.SID + "&EnName=" + enName;
                    if (url.Contains("@AppPath"))
                        url = url.Replace("@AppPath", "http://" + this.Request.Url.Host + this.Request.ApplicationPath);
                    if (url.Contains("@"))
                    {
                        Attrs attrs = en.EnMap.Attrs;
                        foreach (Attr item in attrs)
                        {
                            url = url.Replace("@" + attr.KeyOfEn, en.GetValStrByKey(attr.KeyOfEn));
                            if (url.Contains("@") == false)
                                break;
                        }
                    }
                    this.Add("<a href='" + url + "' target='" + meLink.Tag1 + "' >" + en.GetValByKey(attr.KeyOfEn) + "</a>");
                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;
                }
                if (attr.IsSigan)
                {
                    string v = en.GetValStrByKey(attr.KeyOfEn);
                    if (v.Length == 0)
                        this.Add("<img src='/DataUser/Siganture/" + WebUser.No + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                    else
                        this.Add("<img src='/DataUser/Siganture/" + v + ".jpg' border=0 onerror=\"this.src='/DataUser/Siganture/UnName.jpg'\"/>");
                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;
                }

                if (attr.MaxLen >= 3999 && attr.TBModel == 2)
                {
                    this.AddRichTextBox(en, attr);
                    this.Add("</span>");
                    this.Add("</DIV>");
                    continue;
                }

                if (attr.UIContralType == UIContralType.TB)
                {
                    tb = new TB();
                    tb.ID = "TB_" + attr.KeyOfEn;
                    if (attr.UIIsEnable == false  || isReadonly==true )
                    {
                        tb.Attributes.Add("readonly", "true");
                        tb.ReadOnly = true;
                    }
                    else
                    {
                        //add by dgq 2013-4-9 添加修改事件
                        tb.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                    }
                    tb.Attributes["tabindex"] = attr.IDX.ToString();
                }


                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                                if (attr.UIRows == 1)
                                {
                                    tb.Text = en.GetValStringByKey(attr.KeyOfEn);
                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 15px;padding: 0px;margin: 0px;";
                                    if (attr.UIIsEnable)
                                        tb.CssClass = "TB";
                                    else
                                        tb.CssClass = "TBReadonly";
                                    this.Add(tb);
                                }
                                else
                                {
                                    tb.TextMode = TextBoxMode.MultiLine;

                                    tb.Text = en.GetValStringByKey(attr.KeyOfEn);

                                    tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left;padding: 0px;margin: 0px;";
                                    tb.Rows = attr.UIRows;

                                    if (attr.UIIsEnable)
                                        tb.CssClass = "TBDoc";
                                    else
                                        tb.CssClass = "TBReadonly";

                                    this.Add(tb);
                                }
                                break;
                            case BP.DA.DataType.AppDate:
                                tb.ShowType = TBType.Date;
                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);
                                if (attr.UIIsEnable)
                                    tb.Attributes["onfocus"] = "WdatePicker();";

                                if (attr.UIIsEnable)
                                    tb.Attributes["class"] = "TB";
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppDateTime:
                                tb.ShowType = TBType.DateTime;
                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);

                                if (attr.UIIsEnable)
                                    tb.Attributes["class"] = "TBcalendar";
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                if (attr.UIIsEnable)
                                    tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppBoolean:
                                CheckBox cb = new CheckBox();
                                cb.Width = 350;
                                cb.Text = attr.Name;
                                cb.ID = "CB_" + attr.KeyOfEn;
                                cb.Checked = attr.DefValOfBool;
                                cb.Enabled = attr.UIIsEnable;
                                cb.Checked = en.GetValBooleanByKey(attr.KeyOfEn);
                                if (cb.Enabled == false || isReadonly == true)
                                    cb.Enabled = false;
                                else
                                {
                                    //add by dgq 2013-4-9,添加内容修改后的事件
                                    cb.Attributes["onmousedown"] = "Change('" + attr.FK_MapData + "')";
                                    cb.Enabled = true;
                                }
                                this.Add(cb);
                                break;
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppFloat:
                            case BP.DA.DataType.AppInt:
                                // tb.ShowType = TBType.Num;
                                tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;word-break: keep-all;";
                                tb.Text = en.GetValStrByKey(attr.KeyOfEn);

                                if (attr.UIIsEnable)
                                    tb.Attributes["class"] = "TBNum";
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppMoney:
                                if (attr.UIIsEnable)
                                    tb.Attributes["class"] = "TBNum";
                                else
                                    tb.Attributes["class"] = "TBReadonly";

                                //  tb.ShowType = TBType.Moneny;
                                tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;";
                                this.Add(tb);
                                break;
                            case BP.DA.DataType.AppRate:
                                if (attr.UIIsEnable)
                                    tb.Attributes["class"] = "TBNum";
                                else
                                    tb.Attributes["class"] = "TBReadonly";
                                tb.ShowType = TBType.Moneny;
                                tb.Text = en.GetValMoneyByKey(attr.KeyOfEn).ToString("0.00");
                                tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;";
                                this.Add(tb);
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum:
                        if (attr.UIContralType == UIContralType.DDL)
                        {
                            DDL ddle = new DDL();
                            ddle.ID = "DDL_" + attr.KeyOfEn;
                            ddle.BindSysEnum(attr.UIBindKey);
                            ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            ddle.Enabled = attr.UIIsEnable;
                            ddle.Attributes["tabindex"] = attr.IDX.ToString();
                            if (attr.UIIsEnable)
                            {
                                //add by dgq 2013-4-9,添加内容修改后的事件
                                ddle.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                            }
                            if (ddle.Enabled == true && isReadonly == true)
                                ddle.Enabled = false;
                            this.Add(ddle);
                        }
                        else
                        {
                            //BP.Sys.FrmRBs rbs = new FrmRBs();
                            //rbs.Retrieve(FrmRBAttr.FK_MapData, enName,
                            //    FrmRBAttr.KeyOfEn, attr.KeyOfEn);
                        }
                        break;
                    case FieldTypeS.FK:
                        DDL ddl1 = new DDL();
                        ddl1.ID = "DDL_" + attr.KeyOfEn;
                        ddl1.Attributes["tabindex"] = attr.IDX.ToString();
                        if (ddl1.Enabled)
                        {
                            EntitiesNoName ens = attr.HisEntitiesNoName;
                            ens.RetrieveAll();
                            ddl1.BindEntities(ens);
                            ddl1.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            //add by dgq 2013-4-9,添加内容修改后的事件
                            ddl1.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                        }
                        else
                        {
                            ddl1.Attributes["style"] = "width: " + attr.UIWidth + "px;height: 19px;";
                            if (ddl1.Enabled == true && isReadonly == true)
                                ddl1.Enabled = false;
                            ddl1.Attributes["Width"] = attr.UIWidth.ToString();
                            ddl1.Items.Add(new ListItem(en.GetValRefTextByKey(attr.KeyOfEn), en.GetValStrByKey(attr.KeyOfEn)));
                        }
                        ddl1.Enabled = attr.UIIsEnable;
                        this.Add(ddl1);
                        break;
                    default:
                        break;
                }
                #endregion add contrals.

                this.Add("</span>");
                this.Add("</DIV>");
            }

            #region  输出 rb.
            BP.Sys.FrmRBs myrbs = this.mapData.FrmRBs;
            MapAttr attrRB = new MapAttr();
            foreach (BP.Sys.FrmRB rb in myrbs)
            {
                this.Add("<DIV id='F" + rb.MyPK + "' style='position:absolute; left:" + rb.X + "px; top:" + rb.Y + "px; width:100%; height:16px;text-align: left;word-break: keep-all;' >");
                this.Add("<span style='word-break: keep-all;font-size:12px;'>");

                System.Web.UI.WebControls.RadioButton rbCtl = new RadioButton();
                rbCtl.ID = "RB_" + rb.KeyOfEn + "_" + rb.IntKey.ToString();
                rbCtl.GroupName = rb.KeyOfEn;
                rbCtl.Text = rb.Lab;
                this.Add(rbCtl);

                if (attrRB.KeyOfEn != rb.KeyOfEn)
                {
                    foreach (MapAttr ma in mattrs)
                    {
                        if (ma.KeyOfEn == rb.KeyOfEn)
                        {
                            attrRB = ma;
                            break;
                        }
                    }
                }
                if (isReadonly == true || attrRB.UIIsEnable == false)
                    rbCtl.Enabled = false;
                else
                {
                    //add by dgq 2013-4-9,添加内容修改后的事件
                    rbCtl.Attributes["onmousedown"] = "Change('" + attrRB.FK_MapData + "')";
                }
                this.Add("</span>");
                this.Add("</DIV>");
            }

            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    string id = "RB_" + attr.KeyOfEn + "_" + en.GetValStrByKey(attr.KeyOfEn);
                    RadioButton rb = this.GetRBLByID(id);
                    if (rb != null)
                        rb.Checked = true;
                }
            }
            #endregion  输出 rb.

            #endregion 输出数据控件.

            #region 输出明细.
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsView == false)
                    continue;

                float x = dtl.X;
                float y = dtl.Y;

                this.Add("<DIV id='Fd" + dtl.No + "' style='position:absolute; left:" + x + "px; top:" + y + "px; width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;' >");
                this.Add("<span>");

                string src = "";
                if (dtl.HisDtlShowModel == DtlShowModel.Table)
                {
                    if (isReadonly == true)
                        src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0");
                    else
                        src = "/WF/CCForm/Dtl.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0");
                }
                else
                {
                    if (isReadonly == true)
                        src = "/WF/CCForm/DtlCard.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=1&FID=" + en.GetValStrByKey("FID", "0");
                    else
                        src = "/WF/CCForm/DtlCard.aspx?EnsName=" + dtl.No + "&RefPKVal=" + en.PKVal + "&IsReadonly=0&FID=" + en.GetValStrByKey("FID", "0");
                }

                if (this.IsReadonly == true || dtl.IsReadonly)
                    this.Add("<iframe ID='F" + dtl.No + "'    src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                else
                    this.Add("<iframe ID='F" + dtl.No + "'  Onblur=\"SaveDtl('" + dtl.No + "');\"  src='" + src + "' frameborder=0  style='position:absolute;width:" + dtl.W + "px; height:" + dtl.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");

                this.Add("</span>");
                this.Add("</DIV>");
            }

            string js = "";
            if (this.IsReadonly == false)
            {
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function SaveDtl(dtl) { ";
                js += "\t\n   GenerPageKVs(); //调用产生kvs ";
                js += "\t\n   document.getElementById('F' + dtl ).contentWindow.SaveDtlData();";
                js += "\t\n } ";

                js += "\t\n function SaveM2M(dtl) { ";
                js += "\t\n   document.getElementById('F' + dtl ).contentWindow.SaveM2M();";
                js += "\t\n } ";

                //js += "\t\n function GenerParaVals() { ";
                //js += "\t\n  var strs='' ";
                //foreach (MapAttr attr in mattrs)
                //{
                //    if (attr.UIIsEnable == false || attr.UIVisible == false)
                //        continue;
                //    switch (attr.UIContralType)
                //    {
                //        case UIContralType.TB:
                //            js += "\t\n  strs+= '&" + attr.KeyOfEn + "=ReqTB('" + attr.KeyOfEn + "');";
                //            break;
                //        case UIContralType.DDL:
                //            js += "\t\n  strs+= '&" + attr.KeyOfEn + "=ReqDDL('" + attr.KeyOfEn + "');";
                //            break;
                //        default:
                //            break;
                //    }
                //}
                //js += "\t\n   return strs";
                //js += "\t\n } ";

                js += "\t\n</script>";
                this.Add(js);
            }
            #endregion 输出明细.

            #region 多对多的关系
            foreach (MapM2M m2m in m2ms)
            {
                this.Add("<DIV id='Fd" + m2m.NoOfObj + "' style='position:absolute; left:" + m2m.X + "px; top:" + m2m.Y + "px; width:" + m2m.W + "px; height:" + m2m.H + "px;text-align: left;' >");
                this.Add("<span>");

                string src = ".aspx?NoOfObj=" + m2m.NoOfObj;
                string paras = this.RequestParas;
                try
                {
                    if (paras.Contains("FID=") == false)
                        paras += "&FID=" + this.HisEn.GetValStrByKey("FID");
                }
                catch
                {
                }

                if (paras.Contains("OID=") == false)
                    paras += "&OID=" + this.HisEn.GetValStrByKey("OID");
                src += "&r=q" + paras;
                if (m2m.IsEdit)
                    src += "&IsEdit=1";
                else
                    src += "&IsEdit=0";

                if (src.Contains("FK_MapData") == false)
                    src += "&FK_MapData=" + enName;

                if (m2m.HisM2MType == M2MType.M2MM)
                    src = "/WF/CCForm/M2MM" + src;
                else
                    src = "/WF/CCForm/M2M" + src;

                switch (m2m.ShowWay)
                {
                    case FrmShowWay.FrmAutoSize:
                    case FrmShowWay.FrmSpecSize:
                        if (m2m.IsEdit)
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'   Onblur=\"SaveM2M('" + m2m.NoOfObj + "');\"  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "'   scrolling=auto/></iframe>");
                        else
                            this.Add("<iframe ID='F" + m2m.NoOfObj + "'  src='" + src + "' frameborder=0 style='padding:0px;border:0px;'  leftMargin='0'  topMargin='0' width='" + m2m.W + "' height='" + m2m.H + "'   scrolling=auto/></iframe>");
                        break;
                    case FrmShowWay.Hidden:
                        break;
                    case FrmShowWay.WinOpen:
                        this.Add("<a href=\"javascript:WinOpen('" + src + "&IsOpen=1','" + m2m.W + "','" + m2m.H + "');\"  />" + m2m.Name + "</a>");
                        break;
                    default:
                        break;
                }
                this.Add("</span>");
                this.Add("</DIV>");
            }
            #endregion 多对多的关系

            #region 输出附件
            FrmAttachments aths = this.mapData.FrmAttachments;
            FrmAttachmentDBs athDBs = null;
            if (aths.Count > 0)
                athDBs = new FrmAttachmentDBs(enName, en.PKVal.ToString());

            foreach (FrmAttachment ath in aths)
            {
                if (ath.UploadType == AttachmentUploadType.Single)
                {
                    /* 单个文件 */
                    FrmAttachmentDB athDB = athDBs.GetEntityByKey(FrmAttachmentDBAttr.FK_FrmAttachment, ath.MyPK) as FrmAttachmentDB;
                    float x = ath.X;
                    float y = ath.Y;
                    this.Add("<DIV id='Fa" + ath.MyPK + "' style='position:absolute; left:" + x + "px; top:" + y + "px; width:" + ath.W + "px;text-align: left;float:left' >");
                    //  this.Add("<span>");

                    Label lab = new Label();
                    lab.ID = "Lab" + ath.MyPK;
                    this.Add(lab);
                    if (athDB != null)
                        lab.Text = "<a href='" + this.Request.ApplicationPath + "DataUser/UploadFile/" + athDB.FilePathName + "' target=_blank ><img src='/WF/Img/FileType/" + athDB.FileExts + ".gif' border=0/>" + athDB.FileName + "</a>";

                    FileUpload fu = new FileUpload();
                    fu.ID = ath.MyPK;
                    fu.Attributes["Width"] = ath.W.ToString();
                    this.Add(fu);

                    Button mybtn = new Button();
                    mybtn.CssClass = "Btn";

                    if (ath.IsUpload && this.IsReadonly == false)
                    {
                        mybtn.ID = ath.MyPK;
                        mybtn.Text = "上传";
                        mybtn.CssClass = "bg";
                        mybtn.ID = "Btn_Upload_" + ath.MyPK + "_" + this.HisEn.PKVal;
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        this.Add(mybtn);
                    }

                    if (ath.IsDownload)
                    {
                        mybtn = new Button();
                        mybtn.Text = "下载";
                        mybtn.CssClass = "Btn";

                        mybtn.ID = "Btn_Download_" + ath.MyPK + "_" + this.HisEn.PKVal;
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        mybtn.CssClass = "bg";
                        if (athDB == null)
                            mybtn.Visible = false;
                        else
                            mybtn.Visible = true;
                        this.Add(mybtn);
                    }

                    if (ath.IsDelete && this.IsReadonly == false)
                    {
                        mybtn = new Button();
                        mybtn.CssClass = "Btn";
                        mybtn.Text = "删除";
                        mybtn.Attributes["onclick"] = " return confirm('您确定要执行删除吗？');";
                        mybtn.ID = "Btn_Delete_" + ath.MyPK + "_" + this.HisEn.PKVal;
                        mybtn.Click += new EventHandler(btnUpload_Click);
                        mybtn.CssClass = "bg";
                        if (athDB == null)
                            mybtn.Visible = false;
                        else
                            mybtn.Visible = true;
                        this.Add(mybtn);
                    }
                    this.Add("</DIV>");
                }

                if (ath.UploadType == AttachmentUploadType.Multi)
                {
                    this.Add("<DIV id='Fd" + ath.MyPK + "' style='position:absolute; left:" + ath.X + "px; top:" + ath.Y + "px; width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;' >");
                    this.Add("<span>");
                    string src = "";
                    if (this.IsReadonly)
                        src = this.Request.ApplicationPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal.ToString() + "&FK_FrmAttachment=" + ath.MyPK + "&IsReadonly=1";
                    else
                        src = this.Request.ApplicationPath + "WF/CCForm/AttachmentUpload.aspx?PKVal=" + this.HisEn.PKVal.ToString() + "&FK_FrmAttachment=" + ath.MyPK;

                    this.Add("<iframe ID='F" + ath.MyPK + "'    src='" + src + "' frameborder=0  style='position:absolute;width:" + ath.W + "px; height:" + ath.H + "px;text-align: left;'  leftMargin='0'  topMargin='0' scrolling=auto /></iframe>");
                    this.Add("</span>");
                    this.Add("</DIV>");
                }
            }
            #endregion 输出附件.

            #region 输出 img 附件
            FrmImgAths imgAths = this.mapData.FrmImgAths;
            if (imgAths.Count != 0 && this.IsReadonly == false)
            {
                js = "\t\n<script type='text/javascript' >";
                js += "\t\n function ImgAth(url,athMyPK)";
                js += "\t\n {";
                js += "\t\n  var v= window.showModalDialog(url, 'ddf', 'dialogHeight: 650px; dialogWidth: 950px;center: yes; help: no'); ";
                js += "\t\n  if (v==null )  ";
                js += "\t\n     return ;";
                js += "\t\n  document.getElementById('Img'+athMyPK ).setAttribute('src', './../../DataUser/ImgAth/Temp/'+v+'.png' ); ";
                js += "\t\n }";
                js += "\t\n</script>";
                this.Add(js);
            }

            foreach (FrmImgAth ath in imgAths)
            {
                this.Add("\t\n<DIV id=" + ath.MyPK + " style='position:absolute;left:" + ath.X + "px;top:" + ath.Y + "px;text-align:left;vertical-align:top' >");

                //update by dgq 2013-4-15 改为绝对路径
                //string url = "./CCForm/ImgAth.aspx?W=" + ath.W + "&H=" + ath.H + "&MyPK=" + en.PKVal + "&ImgAth=" + ath.MyPK;
                string url = "/WF/CCForm/ImgAth.aspx?W=" + ath.W + "&H=" + ath.H + "&MyPK=" + en.PKVal + "&ImgAth=" + ath.MyPK;
                if (isReadonly == false)
                    this.AddFieldSet("<a href=\"javascript:ImgAth('" + url + "','" + ath.MyPK + "');\" >编辑:" + ath.CtrlName + "</a>");

                this.Add("\t\n<img src='" + appPath + "/DataUser/ImgAth/Data/" + ath.MyPK + "_" + en.PKVal + ".png' onerror=\"this.src='./../Data/Img/LogH.PNG'\" name='Img" + ath.MyPK + "' id='Img" + ath.MyPK + "' style='padding: 0px;margin: 0px;border-width: 0px;' width=" + ath.W + " height=" + ath.H + " />");

                if (isReadonly == false)
                    this.AddFieldSetEnd();

                this.Add("\t\n</DIV>");
            }
            #endregion 输出附件.

            // 处理扩展.
            if (isReadonly == false)
                this.AfterBindEn_DealMapExt(enName, mattrs, en);
            return;
        }
        void btnUpload_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string[] ids = btn.ID.Split('_');
            string athPK = ids[2] + "_" + ids[3];
            string doType = ids[1];
            string athDBPK = athPK + "_" + this.HisEn.PKVal.ToString();
            FrmAttachment frmAth = new FrmAttachment(athPK);
            string pkVal = this.HisEn.PKVal.ToString();
            switch (doType)
            {
                case "Delete":
                    FrmAttachmentDB db = new FrmAttachmentDB();
                    db.MyPK = athDBPK;
                    db.Delete();
                    Button btnDel = this.GetButtonByID("Btn_Delete_" + athDBPK);
                    btnDel.Visible = false;

                    btnDel = this.GetButtonByID("Btn_Download_" + athDBPK);
                    btnDel.Visible = false;

                    Label lab1 = this.GetLabelByID("Lab" + frmAth.MyPK);
                    lab1.Text = "";
                    break;
                case "Upload":
                    FileUpload fu = this.FindControl(athPK) as FileUpload;
                    if (fu.HasFile == false || fu.FileName.Length <= 2)
                    {
                        this.Alert("请选择上传的文件.");
                        return;
                    }

                    //检查格式是否符合要求.
                    if (frmAth.Exts == "" || frmAth.Exts == "*.*")
                    {
                    }
                    else
                    {
                        string fileExt = fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                        fileExt = fileExt.ToLower();
                        if (frmAth.Exts.ToLower().Contains(fileExt) == false)
                        {
                            this.Alert("您上传的文件格式不符合要求,要求格式为:" + frmAth.Exts);
                            return;
                        }
                    }

                    if (System.IO.Directory.Exists(frmAth.SaveTo) == false)
                        System.IO.Directory.CreateDirectory(frmAth.SaveTo);

                    string saveTo = frmAth.SaveTo + "\\" + athDBPK + "." + fu.FileName.Substring(fu.FileName.LastIndexOf('.') + 1);
                    fu.SaveAs(saveTo);

                    FileInfo info = new FileInfo(saveTo);
                    FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                    dbUpload.MyPK = athDBPK;
                    dbUpload.FK_FrmAttachment = athPK;
                    dbUpload.RefPKVal = this.HisEn.PKVal.ToString();
                    if (this.EnName == null)
                        dbUpload.FK_MapData = this.HisEn.ToString();
                    else
                        dbUpload.FK_MapData = this.EnName;

                    dbUpload.FileExts = info.Extension;
                    dbUpload.FileFullName = saveTo;
                    dbUpload.FileName = fu.FileName;
                    dbUpload.FileSize = (float)info.Length;
                    dbUpload.Save();

                    Button myBtnDel = this.GetButtonByID("Btn_Delete_" + athDBPK);
                    myBtnDel.Visible = true;
                    myBtnDel = this.GetButtonByID("Btn_Download_" + athDBPK);
                    myBtnDel.Visible = true;

                    Label lab = this.GetLabelByID("Lab" + frmAth.MyPK);
                    lab.Text = "<a href='" + this.Request.ApplicationPath + "DataUser/UploadFile/" + dbUpload.FilePathName + "' target=_blank ><img src='/WF/Img/FileType/" + dbUpload.FileExts + ".gif' alt='" + dbUpload.FileName + "' ID='" + frmAth.NoOfObj + "'  border=0/>" + dbUpload.FileName + "</a>";
                    return;
                case "Download":
                    FrmAttachmentDB dbDown = new FrmAttachmentDB();
                    dbDown.MyPK = athDBPK;
                    dbDown.Retrieve();
                    PubClass.DownloadFile(dbDown.FileFullName, dbDown.FileName);
                    break;
                default:
                    break;
            }
        }
        void myBtn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            FrmBtn mybtn = new FrmBtn(btn.ID);
            string doc = mybtn.EventContext.Replace("~", "'");

            Attrs attrs = this.HisEn.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {
                doc = doc.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }
            doc = doc.Replace("@FK_Dept", WebUser.FK_Dept);
            doc = doc.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            doc = doc.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
            doc = doc.Replace("@WebUser.No", WebUser.No);
            doc = doc.Replace("@WebUser.Name", WebUser.Name);
            doc = doc.Replace("@MyPK", this.HisEn.PKVal.ToString());

            #region 处理两个变量.
            string alertMsgErr = mybtn.MsgErr;
            string alertMsgOK = mybtn.MsgOK;
            if (alertMsgOK.Contains("@"))
            {
                foreach (Attr attr in attrs)
                    alertMsgOK = alertMsgOK.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }

            if (alertMsgErr.Contains("@"))
            {
                foreach (Attr attr in attrs)
                    alertMsgErr = alertMsgErr.Replace("@" + attr.Key, this.HisEn.GetValStrByKey(attr.Key));
            }
            #endregion 处理两个变量.

            try
            {
                switch (mybtn.HisBtnEventType)
                {
                    case BtnEventType.RunSQL:
                        DBAccess.RunSQL(doc);
                        this.Alert(alertMsgOK);
                        return;
                    case BtnEventType.RunSP:
                        DBAccess.RunSP(doc);
                        this.Alert(alertMsgOK);
                        return;
                    case BtnEventType.RunURL:
                        doc = doc.Replace("@AppPath", System.Web.HttpContext.Current.Request.ApplicationPath);
                        string text = DataType.ReadURLContext(doc, 800, System.Text.Encoding.UTF8);
                        if (text != null && text.Substring(0, 7).Contains("Err"))
                            throw new Exception(text);
                        alertMsgOK += text;
                        this.Alert(alertMsgOK);
                        return;
                    default:
                        throw new Exception("没有处理的执行类型:" + mybtn.HisBtnEventType);
                }
            }
            catch (Exception ex)
            {
                this.Alert(alertMsgErr + ex.Message);
            }

            #region 处理按钮事件。
            #endregion
        }
        #endregion

        public static string GetRefstrs(string keys, Entity en, Entities hisens)
        {
            string refstrs = "";
            string path = System.Web.HttpContext.Current.Request.ApplicationPath;
            int i = 0;

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    //  string url = path + "/Comm/UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入他门的 方法
            RefMethods myreffuncs = en.EnMap.HisRefMethods;
            if (myreffuncs.Count > 0)
            {
                foreach (RefMethod func in myreffuncs)
                {
                    if (func.Visable == false)
                        continue;

                    // string url = path + "/Comm/RefMethod.aspx?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    string url = "/WF/Comm/RefMethod.aspx?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    if (func.Warning == null)
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(path) + "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                    else
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                }
            }
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            //  string path = this.Request.ApplicationPath;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    //string url = path + "/Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&Key=" + enDtl.RefKey + "&Val=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                    string url = path + "/Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \"  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            return refstrs;
        }
        public UCEn()
        {
        }
        public void AddContral()
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% ></td>"));
            this.Controls.Add(new LiteralControl("<td></TD>"));
        }
        public void AddContral(string desc, CheckBox cb)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td>"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        public void AddContral(string desc, CheckBox cb, int colspan)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td  colspan='" + colspan + "'>"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        //		public void AddContral(string desc, string val)
        public void AddContral(string desc, string val)
        {
            this.Add("<TD class='FDesc' > " + desc + "</TD>");
            this.Add("<TD>" + val + "</TD>");
        }
        public void AddContral(string desc, TB tb, string helpScript)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=500px;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            tb.Attributes["Width"] = "80%";

            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");
            this.Add("<td >" + helpScript);
            this.Add(tb);
            this.AddTDEnd();
        }
        public void AddContral(string desc, TB tb, string helpScript, int colspan)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");

            if (colspan < 3)
            {
                this.Add("<td  colspan=" + colspan + " width='30%' >" + helpScript);
            }
            else
            {
                this.Add("<td  colspan=" + colspan + " width='80%' >" + helpScript);
            }

            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        public void AddContral(string desc, TB tb, int colSpanOfCtl)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb, colSpanOfCtl);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            if (colSpanOfCtl < 3)
                this.Add("<td  colspan=" + colSpanOfCtl + " width='30%' >");
            else
                this.Add("<td  colspan=" + colSpanOfCtl + " width='80%' >");

            this.Add(tb);
            this.AddTDEnd();
        }
        /// <summary>
        /// 增加空件
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="tb"></param>
        public void AddContral(string desc, TB tb)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            //if (tb.ReadOnly == false)
            //    desc += "<font color=red><b>*</b></font>";

            tb.Attributes["style"] = "width=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            this.Add("<td  width='30%'>");
            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        //		public void AddContralDoc(string desc, TB tb)
        public void AddContralDoc(string desc, TB tb)
        {
            //if (desc.Length>
            this.Add("<td class='FDesc'  colspan='2' nowrap height='100px' width='50%' >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.Attributes["Class"] = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }
        public void AddContralDoc(string desc, TB tb, int colspanOfctl)
        {
            //if (desc.Length>
            this.Add("<td class='FDesc'  colspan='" + colspanOfctl + "' nowrap height='100px' width='50%' >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.Attributes["Class"] = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }
        //		public void AddContralDoc(string desc, int colspan, TB tb)
        public void AddContralDoc(string desc, int colspan, TB tb)
        {
            this.Add("<td class='FDesc'  colspan='" + colspan + "' nowrap width=1%  height='100px'  >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.EnsName = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }

        #region 方法
        public bool IsReadonly
        {
            get
            {
                string s = this.ViewState["IsReadonly"] as string;
                if (s == "1")
                    return true;
                return false;
            }
            set
            {
                if (value)
                    ViewState["IsReadonly"] = "1";
                else
                    ViewState["IsReadonly"] = "0";
            }
        }
        public bool IsShowDtl
        {
            get
            {
                return (bool)this.ViewState["IsShowDtl"];
            }
            set
            {
                ViewState["IsShowDtl"] = value;
            }
        }
        public void SetValByKey(string key, string val)
        {
            TB tb = new TB();
            tb.ID = "TB_" + key;
            tb.Text = val;
            tb.Visible = false;
            this.Controls.Add(tb);
        }
        public object GetValByKey(string key)
        {
            TB en = (TB)this.FindControl("TB_" + key);
            return en.Text;
        }
        public void BindAttrs(Attrs attrs)
        {
            //this.HisEn =en;
            bool isReadonly = false;
            this.IsReadonly = false;
            this.IsShowDtl = false;
            this.Controls.Clear();
            this.Attributes["visibility"] = "hidden";
            //this.Height=0;
            //this.Width=0;
            this.Controls.Clear();
            this.Add("<table width='100%' id='a1' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "MyNum")
                    continue;

                if (isLeft && isAddTR)
                {
                    this.AddTR();
                }

                isAddTR = true;
                val = attr.DefaultVal;
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.SetValByKey(attr.Key, val.ToString());
                        isAddTR = false;
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {

                            TB tb = new TB();
                            tb.LoadMapAttr(attr);
                            tb.ID = "TB_" + attr.Key;
                            tb.Attributes["Visible"] = "false";
                            this.Controls.Add(tb);
                            //this.AddContral(attr.Desc,area);
                            //this.SetValByKey(attr.Key, val.ToString() );
                            continue;
                        }
                        else
                        {
                            if (attr.UIHeight != 0)
                            {
                                TB area = new TB();
                                area.LoadMapAttr(attr);
                                area.ID = "TB_" + attr.Key;
                                area.Text = val.ToString();
                                area.Rows = 8;
                                area.TextMode = TextBoxMode.MultiLine;
                                if (isReadonly)
                                    //area.Enabled = false;
                                    area.ReadOnly = true;
                                this.AddContral(attr.Desc, area);
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.LoadMapAttr(attr);

                                tb.ID = "TB_" + attr.Key;
                                if (isReadonly)
                                    //tb.Enabled = false;
                                    tb.ReadOnly = true;
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        break;
                                }
                                tb.Attributes["width"] = "100%";
                                this.AddContral(attr.Desc, tb);
                            }
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    CheckBox cb = new CheckBox();
                    if (attr.DefaultVal.ToString() == "1")
                        cb.Checked = true;
                    else
                        cb.Checked = false;

                    if (isReadonly)
                        cb.Enabled = false;
                    else
                        cb.Enabled = attr.UIVisible;

                    cb.ID = "CB_" + attr.Key;
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* 如果是 DDLIsEnable 的, 就要找到. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* 如果是 enum 类型 */
                            int enumKey = 0;
                            try
                            {
                                enumKey = int.Parse(val.ToString());
                            }
                            catch
                            {
                                throw new Exception("默认值错误：" + attr.Key + " = " + val.ToString());
                            }

                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);


                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddContral(attr.Desc, ddl, true);
                            //this.Controls.Add(ddl);
                        }
                        else
                        {
                            /* 如果是 ens 类型 */
                            Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                            Entity en1 = ens.GetNewEntity;
                            en1.SetValByKey(attr.UIRefKeyValue, val.ToString());
                            string lab = "";
                            try
                            {
                                en1.Retrieve();
                                lab = en1.GetValStringByKey(attr.UIRefKeyText);
                            }
                            catch
                            {
                                if (SystemConfig.IsDebug == false)
                                {
                                    lab = "" + val.ToString();
                                }
                                else
                                {
                                    lab = "" + val.ToString();
                                    //lab="没有关联到值"+val.ToString()+"Class="+attr.UIBindKey+"EX="+ex.Message;
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false, this.Page.Request.ApplicationPath);
                            ddl.ID = "DDL_" + attr.Key;
                            this.AddContral(attr.Desc, ddl, true);
                            //this.Controls.Add(ddl);
                        }
                    }
                    else
                    {
                        /* 可以使用的情况. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        this.AddContral(attr.Desc, ddl1, true);
                        //	this.Controls.Add(ddl1);
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                    this.AddTREnd();

                isLeft = !isLeft;
            } // 结束循环.

            this.Add("</TABLE>");
        }
        //		public void BindReadonly(Entity en )
        public void BindReadonly(Entity en)
        {
            this.HisEn = en;
            //this.IsReadonly = isReadonly;
            //this.IsShowDtl = isShowDtl;
            this.Attributes["visibility"] = "hidden";
            this.Controls.Clear();
            this.AddTable(); //("<table   width='100%' id='AutoNumber1'  border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (isLeft && isAddTR)
                {
                    this.Add("<tr>");
                }
                isAddTR = true;
                val = en.GetValByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.AddContral(attr.Desc, val.ToString().ToString());
                        isAddTR = false;
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            this.SetValByKey(attr.Key, val.ToString());
                            continue;
                        }
                        else
                        {

                            if (attr.UIHeight != 0)
                            {
                                this.AddContral(attr.Desc, val.ToString());
                            }
                            else
                            {

                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        //this.AddContral(attr.Desc, val.ToString().ToString("0.00")  );
                                        break;
                                    default:
                                        this.AddContral(attr.Desc, val.ToString());
                                        break;
                                }
                            }
                        }

                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    if (en.GetValBooleanByKey(attr.Key))
                        this.AddContral(attr.Desc, "是");
                    else
                        this.AddContral(attr.Desc, "否");
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    this.AddContral(attr.Desc, val.ToString());
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                    this.AddTREnd();

                isLeft = !isLeft;
            } // 结束循环.

            this.Add("</TABLE>");



            if (en.IsExit(en.PK, en.PKVal) == false)
                return;

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
                refstrs += "<hr>";
            this.Add(refstrs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <param name="isReadonly"></param>
        /// <param name="isShowDtl"></param>
        //		public void Bind3Item(Entity en, bool isReadonly, bool isShowDtl)
        public void Bind3Item(Entity en, bool isReadonly, bool isShowDtl)
        {
            AttrDescs ads = new AttrDescs(en.ToString());
            this.HisEn = en;
            this.IsReadonly = isReadonly;
            this.IsShowDtl = isShowDtl;
            this.Controls.Clear();
            this.Attributes["visibility"] = "hidden";
            this.Controls.Clear();
            this.Add("<table   width='100%' id='AutoNumber1'  border='0' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {

                if (attr.Key == "MyNum")
                    continue;

                val = en.GetValByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;

                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.AddTR();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            this.SetValByKey(attr.Key, val.ToString());
                            continue;
                        }
                        else
                        {
                            if (attr.UIHeight != 0)
                            {
                                /* doc 文本类型。　*/
                                TB area = new TB();
                                area.LoadMapAttr(attr);
                                area.ID = "TB_" + attr.Key;
                                area.Text = val.ToString();
                                area.Rows = 8;
                                area.Columns = 30;
                                area.TextMode = TextBoxMode.MultiLine;
                                area.Attributes["height"] = "100px";
                                //area.Attributes["width"]="100px";
                                area.IsHelpKey = false;

                                area.Attributes.Add("class", "TextArea1");

                                if (isReadonly)
                                    area.Enabled = false;

                                this.AddTR();
                                this.Add("<TD colspan=3 class='FDesc' >" + attr.Desc + "</TD>");
                                this.AddTREnd();

                                this.AddTR();
                                this.Add("<TD colspan=3  height='250' >");
                                this.Add(area);
                                this.Add("</TD>");
                                this.AddTREnd();
                                continue;
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.ID = "TB_" + attr.Key;
                                tb.IsHelpKey = false;

                                if (isReadonly || attr.UIIsReadonly)
                                    tb.Enabled = false;
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        break;
                                }
                                tb.Attributes["width"] = "100%";
                                this.AddTR();
                                this.AddContral(attr.Desc, tb);

                                /*
                                AttrDesc ad = ads.GetEnByKey(AttrDescAttr.Attr,  attr.Key ) as AttrDesc;
                                if (ad!=null)
                                    this.AddContral(attr.Desc,tb);
                                else
                                {
                                    //this.AddContral(attr.Desc,tb);

                                    tb.Attributes["width"]="";

                                    //this.AddTR();
                                    this.Add("<TD class='FDesc' width='1%' >"+attr.Desc+"</TD>");
                                    this.Add("<TD  colspan=2 >");
                                    this.Add(tb);
                                    this.Add("</TD>");
                                    this.AddTREnd();
                                    continue;
                                }
                                */

                            }
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    CheckBox cb = new CheckBox();
                    cb.Checked = en.GetValBooleanByKey(attr.Key);

                    if (isReadonly || !attr.UIIsReadonly)
                        cb.Enabled = false;
                    else
                        cb.Enabled = attr.UIVisible;


                    cb.ID = "CB_" + attr.Key;
                    this.AddTR();
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* 如果是 DDLIsEnable 的, 就要找到. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* 如果是 enum 类型 */
                            int enumKey = int.Parse(val.ToString());
                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddTR();
                            this.AddContral(attr.Desc, ddl, false);
                            //this.Controls.Add(ddl);
                        }
                        else
                        {
                            /* 如果是 ens 类型 */
                            Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                            Entity en1 = ens.GetNewEntity;
                            en1.SetValByKey(attr.UIRefKeyValue, val.ToString());
                            string lab = "";
                            try
                            {
                                en1.Retrieve();
                                lab = en1.GetValStringByKey(attr.UIRefKeyText);
                            }
                            catch
                            {
                                if (SystemConfig.IsDebug == false)
                                {
                                    lab = "" + val.ToString();
                                }
                                else
                                {
                                    lab = "" + val.ToString();
                                    //lab="没有关联到值"+val.ToString()+"Class="+attr.UIBindKey+"EX="+ex.Message;
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false, this.Page.Request.ApplicationPath);
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddTR();
                            this.AddContral(attr.Desc, ddl, false);
                            //this.Controls.Add(ddl);
                        }
                    }
                    else
                    {
                        /* 可以使用的情况. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        //ddl1.SelfBindKey = ens.ToString();
                        //ddl1.SelfEnsRefKey = attr.UIRefKeyValue;
                        //ddl1.SelfEnsRefKeyText = attr.UIRefKeyText;

                        this.AddTR();
                        this.AddContral(attr.Desc, ddl1, true);
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {

                }

                AttrDesc ad1 = ads.GetEnByKey(AttrDescAttr.Attr, attr.Key) as AttrDesc;
                if (ad1 == null)
                    this.AddTD("class='Note'", "&nbsp;");
                else
                    this.AddTD("class='Note'", ad1.Desc);

                this.AddTREnd();
            } //结束循环.

            #region 查看是否包含 MyFile字段如果有就认为是附件。
            if (en.EnMap.Attrs.Contains("MyFileName"))
            {
                /* 如果包含这二个字段。*/
                string fileName = en.GetValStringByKey("MyFileName");
                string filePath = en.GetValStringByKey("MyFilePath");
                string fileExt = en.GetValStringByKey("MyFileExt");

                string url = "";
                if (fileExt != "")
                {
                    // 系统物理路径。
                    string path = this.Request.PhysicalApplicationPath.ToLower();
                    string path1 = filePath.ToLower();
                    path1 = path1.Replace(path, "");
                    url = "&nbsp;&nbsp;<a href='../" + path1 + "/" + en.PKVal + "." + fileExt + "' target=_blank ><img src='/WF/Img/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                }

                this.AddTR();
                this.AddTD("align=right nowrap=true class='FDesc'", "附件或图片:");
                HtmlInputFile file = new HtmlInputFile();
                file.ID = "file";
                file.Attributes.Add("style", "width:60%");
                this.Add("<TD colspan=2  class='FDesc' >");
                this.Add(file);
                this.Add(url);
                if (fileExt != "")
                {
                    Button btn1 = new Button();
                    btn1.CssClass = "Btn";
                    btn1.Text = "移除";
                    btn1.ID = "Btn_DelFile";
                    btn1.Attributes.Add("class", "Btn1");

                    btn1.Attributes["onclick"] += " return confirm('此操作要执行移除附件或图片，是否继续？');";
                    this.Add(btn1);
                }
                this.Add("</TD>");

                this.AddTREnd();
            }
            #endregion

            #region save button .
            this.AddTR();
            this.Add("<TD align=center colspan=3 >");


            Button btn = new Button();
            btn.CssClass = "Btn";

            if (en.HisUAC.IsInsert)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_New";
                btn.Text = "  新 建  ";
                btn.Attributes.Add("class", "Btn1");

                this.Add(btn);
                this.Add("&nbsp;");
            }

            if (en.HisUAC.IsUpdate)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Save";
                btn.Text = "  保  存  ";
                btn.Attributes.Add("class", "Btn1");

                this.Add(btn);
                this.Add("&nbsp;");
            }


            if (en.HisUAC.IsDelete)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Del";
                btn.Text = "  删  除  ";
                btn.Attributes.Add("class", "Btn1");

                btn.Attributes["onclick"] = " return confirm('您确定要执行删除吗？');";
                this.Add(btn);
                this.Add("&nbsp;");
            }

            this.Add("&nbsp;<input class='Btn' type=button onclick='javascript:window.close()' value='  关  闭  ' />");

            this.Add("</TD>");
            this.AddTREnd();
            #endregion

            this.AddTableEnd();

            if (isShowDtl == false)
                return;


            if (en.IsExit(en.PK, en.PKVal) == false)
                return;

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }
            this.Add("<HR>");

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
                refstrs += "<hr>";

            this.Add(refstrs);
        }
        private void btn_Click(object sender, EventArgs e)
        {
        }

        public Entity GetEnData(Entity en)
        {
            try
            {

                string s = null;
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.Key == "MyNum")
                    {
                        en.SetValByKey(attr.Key, 1);
                        continue;
                    }

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.UIVisible)
                            {
                                if (attr.UIHeight == 0)
                                {
                                    // 处理特殊字符.
                                    s = this.GetTBByID("TB_" + attr.Key).Text;
                                    en.SetValByKey(attr.Key, s);
                                    continue;
                                }
                                else
                                {
                                    if (this.IsExit("TB_" + attr.Key))
                                    {
                                        // 处理特殊字符.
                                        s = this.GetTBByID("TB_" + attr.Key).Text;
                                        en.SetValByKey(attr.Key, s);
                                        continue;
                                    }

                                    if (this.IsExit("TBH_" + attr.Key))
                                    {
                                        HtmlInputHidden input = (HtmlInputHidden)this.FindControl("TBH_" + attr.Key);
                                        en.SetValByKey(attr.Key, input.Value);
                                        continue;
                                    }

                                    if (this.IsExit("TBF_" + attr.Key))
                                    {
                                        //FredCK.FCKeditorV2.FCKeditor fck = (FredCK.FCKeditorV2.FCKeditor)this.FindControl("TB_" + attr.Key);
                                        //en.SetValByKey(attr.Key, fck.Value);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, this.GetValByKey(attr.Key));
                            }
                            break;
                        case UIContralType.DDL:
                            en.SetValByKey(attr.Key, this.GetDDLByKey("DDL_" + attr.Key).SelectedItem.Value);
                            break;
                        case UIContralType.CheckBok:
                            en.SetValByKey(attr.Key, this.GetCBByKey("CB_" + attr.Key).Checked);
                            break;
                        case UIContralType.RadioBtn:
                            if (attr.IsEnum)
                            {
                                SysEnums ses = new SysEnums(attr.UIBindKey);
                                foreach (SysEnum se in ses)
                                {
                                    string id = "RB_" + attr.Key + "_" + se.IntKey;
                                    RadioButton rb = this.GetRBLByID(id);
                                    if (rb != null && rb.Checked)
                                    {
                                        en.SetValByKey(attr.Key, se.IntKey);
                                        break;
                                    }
                                }
                            }
                            if (attr.MyFieldType == FieldType.FK)
                            {
                                Entities ens = BP.DA.ClassFactory.GetEns(attr.UIBindKey);
                                ens.RetrieveAll();
                                foreach (Entity enNoName in ens)
                                {
                                    RadioButton rb = this.GetRBLByID(attr.Key + "_" + enNoName.GetValStringByKey(attr.UIRefKeyValue));
                                    if (rb != null && rb.Checked)
                                    {
                                        en.SetValByKey(attr.Key, enNoName.GetValStrByKey(attr.UIRefKeyValue));
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetEnData error :" + ex.Message);
            }
            return en;
        }

        public DDL GetDDLByKey(string key)
        {
            return (DDL)this.FindControl(key);
        }
        //		public CheckBox GetCBByKey(string key)
        public CheckBox GetCBByKey(string key)
        {
            return (CheckBox)this.FindControl(key);
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.IsPostBack)
            {
                //	this.Bind(this.HisEn,this.IsReadonly,this.IsShowDtl) ;
            }
        }
        public Entity HisEn = null;
        public static string GetRefstrs1(string keys, Entity en, Entities hisens)
        {
            string refstrs = "";

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "-" + i + "</a>]";

                }
            }
            #endregion

            #region 加入他门的相关功能
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					refstrs+="[<a href='"+url+"' >"+en1.Name+"</a>]";
            //				}
            //			}
            #endregion

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            return refstrs;
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
        ///		设计器支持所需的方法 - 不要使用代码编辑器
        ///		修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

    }
}
