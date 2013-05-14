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
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_MapDtl : WebPage
    {
        #region 属性
        public new string DoType
        {
            get
            {
                string v = this.Request.QueryString["DoType"];
                if (v == null || v == "")
                    v = "New";
                return v;
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_MapDtl
        {
            get
            {
                return this.Request.QueryString["FK_MapDtl"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            this.Title = md.Name + " - 设计明细";
            switch (this.DoType)
            {
                case "Edit":
                    MapDtl dtl = new MapDtl();
                    if (this.FK_MapDtl == null)
                    {
                        dtl.No = this.FK_MapData + "Dtl";
                    }
                    else
                    {
                        dtl.No = this.FK_MapDtl;
                        dtl.Retrieve();
                    }
                    BindEdit(md, dtl);
                    break;
                default:
                case "New":
                    int num = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM Sys_MapDtl WHERE FK_MapData='" + this.FK_MapData + "'") + 1;
                    MapDtl dtl1 = new MapDtl();
                    dtl1.Name = "从表" + num;
                    dtl1.No = this.FK_MapData + "Dtl" + num;
                    dtl1.PTable = this.FK_MapData + "Dtl" + num;
                    BindEdit(md, dtl1);
                    break;
            }
        }
        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                switch (this.DoType)
                {
                    case "New":
                    default:
                        MapDtl dtlN = new MapDtl();
                        dtlN = (MapDtl)this.Pub1.Copy(dtlN);
                        if (this.DoType == "New")
                        {
                            if (dtlN.IsExits)
                            {
                                this.Alert(  "已存在编号：" + dtlN.No);
                                return;
                            }
                        }
                        dtlN.FK_MapData = this.FK_MapData;
                        dtlN.GroupID = 0;
                        dtlN.RowIdx = 0;
                        GroupFields gfs1 = new GroupFields(this.FK_MapData);
                        if (gfs1.Count == 1)
                        {
                            GroupField gf = (GroupField)gfs1[0];
                            dtlN.GroupID = gf.OID;
                        }
                        else
                        {
                            dtlN.GroupID = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemIntVal;
                        }
                        dtlN.Insert();
                        if (btn.ID.Contains("AndClose"))
                        {
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect("MapDtl.aspx?DoType=Edit&FK_MapDtl=" + dtlN.No + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                    case "Edit":
                        MapDtl dtl = new MapDtl(this.FK_MapDtl);
                        dtl = (MapDtl)this.Pub1.Copy(dtl);
                        if (this.DoType == "New")
                        {
                            if (dtl.IsExits)
                            {
                                this.Alert( "已存在编号："+ dtl.No);
                                return;
                            }
                        }
                        dtl.FK_MapData = this.FK_MapData;
                        GroupFields gfs = new GroupFields(dtl.FK_MapData);
                        if (gfs.Count > 1)
                            dtl.GroupID = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemIntVal;

                        if (this.DoType == "New")
                            dtl.Insert();
                        else
                            dtl.Update();

                        if (btn.ID.Contains("AndC"))
                        {
                            this.WinClose();
                            return;
                        }
                        this.Response.Redirect("MapDtl.aspx?DoType=Edit&FK_MapDtl=" + dtl.No + "&FK_MapData=" + this.FK_MapData, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                MapDtl dtl = new MapDtl();
                dtl.No = this.FK_MapDtl;
                dtl.Delete();
                this.WinClose();
                //this.Response.Redirect("MapDtl.aspx?DoType=DtlList&FK_MapData=" + this.FK_MapData, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }

        void btn_MapAth_Click(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.MyPK = this.FK_MapDtl + "_AthM";
            if (ath.RetrieveFromDBSources() == 0)
            {
                ath.FK_MapData = this.FK_MapDtl;
                ath.NoOfObj = "AthM";
                ath.Name = "我的从表附件";
                ath.UploadType = AttachmentUploadType.Multi;
                ath.Insert();
            }
            this.Response.Redirect("Attachment.aspx?DoType=Edit&FK_MapData=" + this.FK_MapDtl + "&UploadType=1&Ath=AthM", true);
        }
        void btn_MapExt_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("MapExt.aspx?DoType=New&FK_MapData=" + this.FK_MapDtl, true);
        }

        void btn_New_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("MapDtl.aspx?DoType=New&FK_MapData=" + this.FK_MapData, true);
        }
        void btn_Go_Click(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            dtl.IntMapAttrs();
            this.Response.Redirect("MapDtlDe.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&FK_MapDtl=" + this.FK_MapDtl, true);
        }

        public void BindEdit(MapData md, MapDtl dtl)
        {
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("从表属性");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            int idx = 1;
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD(  "表英文名称");
            TB tb = new TB();
            tb.ID = "TB_No";
            tb.Text = dtl.No;
            if (this.DoType == "Edit")
                tb.Enabled = false;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            //this.Pub1.AddTD("英文名称全局唯一");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD( "表中文名称" );
            tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = dtl.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("XX 从表");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD( "物理表名" );
            tb = new TB();
            tb.ID = "TB_PTable";
            tb.Text = dtl.PTable;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            //this.Pub1.AddTD("存储数据的物理表名称");
            //  this.Pub1.AddTD("存储数据的物理表名称");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD( "操作权限");
            DDL ddl = new DDL();
            ddl.BindSysEnum(MapDtlAttr.DtlOpenType, (int)dtl.DtlOpenType);
            ddl.ID = "DDL_DtlOpenType";
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD();
            // this.Pub1.AddTD("用于从表的权限控制");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            CheckBox cb = new CheckBox();
            cb.ID = "CB_IsView";
            cb.Text = "是否可见";
            cb.Checked = dtl.IsView;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsUpdate";
            cb.Text = "是否可以修改行"; // "是否可以修改行";
            cb.Checked = dtl.IsUpdate;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsInsert";
            cb.Text =  "是否可以新增行" ; // "是否可以新增行";
            cb.Checked = dtl.IsInsert;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsDelete";
            cb.Text = "是否可以删除行"; // "是否可以删除行";
            cb.Checked = dtl.IsDelete;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsShowIdx";
            cb.Text = "是否显示序号列"; //"是否显示序号列";
            cb.Checked = dtl.IsShowIdx;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsShowSum";
            cb.Text = "是否合计行";// "是否合计行";
            cb.Checked = dtl.IsShowSum;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsShowTitle";
            cb.Text = "是否显示标头";// "是否显示标头";
            cb.Checked = dtl.IsShowTitle;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsExp";
            cb.Text =  "是否可以导出？";// "是否可以导出";
            cb.Checked = dtl.IsShowTitle;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsImp";
            cb.Text = "是否可以导入？" ;// "是否可以导出";
            cb.Checked = dtl.IsShowTitle;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsCopyNDData";
            cb.Text =  "是允许从上一个节点Copy数据" ;
            cb.Checked = dtl.IsCopyNDData;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsHLDtl";
            cb.Text = "是否是合流汇总从表(当前节点是合流节点有效)";
            cb.Checked = dtl.IsHLDtl;
            this.Pub1.AddTD("colspan=2", cb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsEnableAthM";
            cb.Text = "是否启用多附件";
            cb.Checked = dtl.IsEnableAthM;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableM2M;
            cb.Text = "是否启用一对多";
            cb.Checked = dtl.IsEnableM2M;
            this.Pub1.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_" + MapDtlAttr.IsEnableM2MM;
            cb.Text = "是否启用一对多多";
            cb.Checked = dtl.IsEnableM2MM;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            cb = new CheckBox();
            cb.ID = "CB_IsEnablePass";
            cb.Text = "是否起用审核字段？";// "是否合计行";
            cb.Checked = dtl.IsEnablePass;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();


            //cb = new CheckBox();
            //cb.ID = "CB_IsEnableAth";
            //cb.Text = "是否启用单附件"; 
            //cb.Checked = dtl.IsEnablePass;
            //this.Pub1.AddTD(cb);
            //this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD(  "初始化行数");
            tb = new TB();
            tb.ID = "TB_RowsOfList";
            tb.Attributes["class"] = "TBNum";
            tb.TextExtInt = dtl.RowsOfList;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("显示格式");
            ddl = new DDL();
            ddl.ID = "DDL_DtlShowModel";
            ddl.BindSysEnum(MapDtlAttr.DtlShowModel, (int)dtl.HisDtlShowModel);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("越出处理");
            ddl = new DDL();
            ddl.ID = "DDL_WhenOverSize";
            ddl.BindSysEnum(MapDtlAttr.WhenOverSize, (int)dtl.HisWhenOverSize);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            GroupFields gfs = new GroupFields(md.No);
            if (gfs.Count > 1)
            {
                this.Pub1.AddTR1();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("显示在分组");
                ddl = new DDL();
                ddl.ID = "DDL_GroupField";
                ddl.BindEntities(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab, false, AddAllLocation.None);
                ddl.SetSelectItem(dtl.GroupID);
                this.Pub1.AddTD("colspan=2", ddl);
                this.Pub1.AddTREnd();
            }
            if (gfs.Count > 1)
                this.Pub1.AddTR();
            else
                this.Pub1.AddTR1();

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            this.Pub1.AddTDBegin("colspan=3 align=center");

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = " 保存并关闭 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            if (this.FK_MapDtl != null)
            {
                //btn = new Button();
                //btn.ID = "Btn_D";
                //btn.Text = this.ToE("DesignSheet", "设计表单"); // "设计表单";
                //btn.Click += new EventHandler(btn_Go_Click);
                //this.Pub1.Add(btn);

                btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";
                btn.Text = "删除"; // "删除";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Del_Click);
                this.Pub1.Add(btn);


                btn = new Button();
                btn.ID = "Btn_New";
                btn.CssClass = "Btn";
                btn.Text = "新建"; // "删除";
                btn.Click += new EventHandler(btn_New_Click);
                this.Pub1.Add(btn);

                btn = new Button();
                btn.ID = "Btn_MapExt";
                btn.CssClass = "Btn";
                btn.Text =  "扩展设置"; // "删除";

                btn.Click += new EventHandler(btn_MapExt_Click);
                this.Pub1.Add(btn);

                if (dtl.IsEnableAthM)
                {
                    btn = new Button();
                    btn.CssClass = "Btn";
                    btn.ID = "Btn_IsEnableAthM";
                    btn.Text = "附件属性"; // "删除";
                    btn.Click += new EventHandler(btn_MapAth_Click);
                    this.Pub1.Add(btn);
                }

                // btn = new Button();
                // btn.ID = "Btn_DtlTR";
                // btn.Text = "多表头";
                // btn.Attributes["onclick"] = "javascript:WinOpen('')";
                //// btn.Click += new EventHandler(btn_DtlTR_Click);
                // this.Pub1.Add(btn);
            }
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

    }

}