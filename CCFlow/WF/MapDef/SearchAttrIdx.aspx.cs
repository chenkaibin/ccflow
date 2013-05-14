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
using BP.En;
using BP.Port;
using BP.DA;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_SearchAttrIdx : WebPage
    {
        public new string RefNo
        {
            get
            {
                return this.Request.QueryString["RefNo"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title =   "定义查询条件"; // "查询条件定义";

            MapData md = new MapData(this.RefNo);
            this.Ucsys1.AddTable();
            this.Ucsys1.AddCaptionLeft(md.Name + " 定义查询条件" );
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("colspan=3", "ID");
            this.Ucsys1.AddTDTitle("名称");
            this.Ucsys1.AddTDTitle();
            this.Ucsys1.AddTREnd();

            MapAttrs attrs = new MapAttrs(this.RefNo);

            int i = 0;
            foreach (MapAttr attr in attrs)
            {
                if (attr.LGType == FieldTypeS.Normal)
                    continue;

                i++;
                this.Ucsys1.AddTR();
                this.Ucsys1.AddTDIdx(i);
                this.Ucsys1.AddTD("<img src='/WF/Img/Btn/Up.gif' />");
                this.Ucsys1.AddTD("<img src='/WF/Img/Btn/Down.gif' />");


                this.Ucsys1.AddTD(attr.Name);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.KeyOfEn;
                if (md.SearchKeys.Contains("@" + attr.KeyOfEn))
                    cb.Checked = true;
                else
                    cb.Checked = false;
                this.Ucsys1.AddTD(cb);
                this.Ucsys1.AddTREnd();
            }

            this.Ucsys1.AddTRSum();
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.Ucsys1.AddTD("colspan=5 align=center", btn);
            this.Ucsys1.AddTREnd();
            this.Ucsys1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            MapData md = new MapData(this.RefNo);
            MapAttrs attrs = new MapAttrs(this.RefNo);

            string s = "";
            foreach (MapAttr attr in attrs)
            {
                if (attr.LGType == FieldTypeS.Normal)
                    continue;

                CheckBox cb = this.Ucsys1.GetCBByID("CB_" + attr.KeyOfEn);
                if (cb.Checked == false)
                    continue;

                s += "@" + attr.KeyOfEn;
            }

            md.SearchKeys = s;
            md.Update();
            this.Alert("保存成功。");
        }
    }

}