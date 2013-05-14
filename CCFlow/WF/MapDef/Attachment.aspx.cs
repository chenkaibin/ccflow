using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_FrmAttachment : WebPage
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string UploadType
        {
            get
            {
                string s = this.Request.QueryString["UploadType"];
                if (s == null)
                    s = "1";
                return s;
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    s = "test";
                return s;
            }
        }
        public string Ath
        {
            get
            {
                return this.Request.QueryString["Ath"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            ath.MyPK = this.FK_MapData + "_" + this.Ath;
            if (this.Ath != null)
                ath.RetrieveFromDBSources();

            ath.FK_MapData = this.FK_MapData;
            ath.NoOfObj = this.Ath;
            ath.MyPK = this.FK_MapData + "_" + this.Ath;

            //this.Response.Write(this.Ath);
            //this.Response.Write("  -- "+this.FK_MapData);

            this.Title = "附件属性设置";

            this.Pub1.AddTable();
            //this.Pub1.AddCaptionLeft("附件");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("说明");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("编号");
            TextBox tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.NoOfObj;
            tb.Text = ath.NoOfObj;
            if (this.Ath != null)
                tb.Enabled = false;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("标示号只能英文字母数字或下滑线.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("名称");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Name;
            tb.Text = ath.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("附件的中文名称.");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("文件格式");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Exts;
            tb.Text = ath.Exts;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("实例:doc,docx,xls,多种格式用逗号分开.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("保存到");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.SaveTo;
            tb.Text = ath.SaveTo;
            tb.Columns = 60;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("类别");
            tb = new TextBox();
            tb.ID = "TB_" + FrmAttachmentAttr.Sort;
            tb.Text = ath.Sort;
            tb.Columns = 60;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=3", "帮助:类别可以为空,设置的格式为:类别名1,类别名2,类别名3");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsDownload;
            cb.Text = "是否可下载";
            cb.Checked = ath.IsDownload;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsDelete;
            cb.Text = "是否可删除";
            cb.Checked = ath.IsDelete;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsUpload;
            cb.Text = "是否可上传";
            cb.Checked = ath.IsUpload;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsNote;
            cb.Text = "是否增加备注列";
            cb.Checked = ath.IsNote;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();



            this.Pub1.AddTR();
            this.Pub1.AddTD("高度");
            BP.Web.Controls.TB mytb = new BP.Web.Controls.TB();
            mytb.ID = "TB_" + FrmAttachmentAttr.H;
            mytb.Text = ath.H.ToString();
            mytb.ShowType = BP.Web.Controls.TBType.Float;
            this.Pub1.AddTD("colspan=1", mytb);
            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("宽度");
            mytb = new BP.Web.Controls.TB();
            mytb.ID = "TB_" + FrmAttachmentAttr.W;
            mytb.Text = ath.W.ToString();
            mytb.ShowType = BP.Web.Controls.TBType.Float;
            mytb.Columns = 60;
            this.Pub1.AddTD("colspan=1", mytb);
            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("自动控制");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmAttachmentAttr.IsAutoSize;
            cb.Text = "自动控制高度与宽度(对傻瓜表单有效)";
            cb.Checked = ath.IsAutoSize;
            this.Pub1.AddTD("colspan=2", cb);
            this.Pub1.AddTREnd();

            GroupFields gfs = new GroupFields(ath.FK_MapData);

            this.Pub1.AddTR1();
            this.Pub1.AddTD("显示在分组");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_GroupField";
            ddl.BindEntities(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab, false, BP.Web.Controls.AddAllLocation.None);
            ddl.SetSelectItem(ath.GroupID);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "  Save  ";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.AddTD(btn);

            if (this.Ath != null)
            {
                btn = new Button();
                btn.ID = "Btn_Delete";
                btn.Text = "  Delete  ";
                btn.CssClass = "Btn";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Click);
                this.Pub1.AddTD(btn);
            }
            else
            {
                this.Pub1.AddTD();
            }
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            FrmAttachment ath = new FrmAttachment();
            Button btn = sender as Button;
            if (btn.ID == "Btn_Delete")
            {
                ath.MyPK = this.FK_MapData + "_" + this.Ath;
                ath.Delete();
                this.WinClose("删除成功.");
                return;
            }

            ath.MyPK = this.FK_MapData + "_" + this.Ath;
            if (this.Ath != null)
                ath.RetrieveFromDBSources();
            ath = this.Pub1.Copy(ath) as FrmAttachment;
            ath.FK_MapData = this.FK_MapData;
            ath.MyPK = this.FK_MapData + "_" + this.Ath;

            GroupFields gfs1 = new GroupFields(this.FK_MapData);
            if (gfs1.Count == 1)
            {
                GroupField gf = (GroupField)gfs1[0];
                ath.GroupID = gf.OID;
            }
            else
            {
                ath.GroupID = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemIntVal;
            }

            if (this.Ath == null)
            {
                ath.UploadType = (AttachmentUploadType)int.Parse(this.UploadType);
                ath.MyPK = this.FK_MapData + "_" + this.Ath;
                if (ath.IsExits == true)
                {
                    this.Alert("附件编号(" + ath.NoOfObj + ")已经存在。");
                    return;
                }
                ath.Insert();
            }
            else
            {
                ath.NoOfObj = this.Ath;
                ath.Update();
            }
            this.WinCloseWithMsg("保存成功");
        }
    }
}