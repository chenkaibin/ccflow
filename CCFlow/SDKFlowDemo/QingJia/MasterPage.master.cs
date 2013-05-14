using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Demo_QingJiaTiao_MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Label1.Text = " 节点信息参数:";
        this.Label1.Text += "<BR>流程编号:FK_Flow=" + this.Request.QueryString["FK_Flow"];
        this.Label1.Text += "<BR>节点ID:NodeID=" + this.Request.QueryString["NodeID"];
        this.Label1.Text += "<BR>工作ID:WorkID=" + this.Request.QueryString["WorkID"];
        this.Label1.Text += "<BR>当前操作员:WebUser.No=" + BP.Web.WebUser.No;
        this.Label1.Text += "<BR>当前操作员:WebUser.Name=" + BP.Web.WebUser.Name;
        //this.Label1.Text += "<BR>WorkID=" + this.Request.QueryString["WorkID"];
        //this.Label1.Text += "<BR>WorkID=" + this.Request.QueryString["WorkID"];
    }
}
