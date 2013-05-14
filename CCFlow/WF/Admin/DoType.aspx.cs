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
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_DoType : WebPage
    {
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "FlowCheck":
                    this.Title = "流程检查";
                    BP.WF.Flow fl = new BP.WF.Flow(this.RefNo);
                    this.Ucsys1.AddFieldSet("流程检查信息");
                    this.Ucsys1.Add(fl.DoCheck()); //  流程检查信息
                    this.Ucsys1.AddFieldSetEnd();
                    break;
                default:
                    this.Ucsys1.AddMsgOfInfo("错误标记", this.DoType);
                    break;
            }

        }
    }

}