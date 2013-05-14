using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_SetFlowSort : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                FlowSort fs = new FlowSort();
                fs.Name = Tbx_FlowSortName.Text;
                fs.No = fs.GenerNewNo;
                fs.Insert();
                LbMessage.Text = "添加成功！";
                this.WinClose();
            }
            catch
            {
                LbMessage.Text = "保存失败！";
            }

        }
    }
}