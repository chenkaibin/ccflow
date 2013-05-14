using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using BP.WF;
using BP.En;

public partial class Demo_QingJia_S3_ZongJingLiShenpi : FlowPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.AddBtn(ButtonList.Btn_DelFlow, this.Btn_DelFlow);
        this.AddBtn(ButtonList.Btn_Forward, this.Btn_Forward);
        this.AddBtn(ButtonList.Btn_NewFlow, this.Btn_NewFlow);
        this.AddBtn(ButtonList.Btn_Return, this.Btn_Return);
        this.AddBtn(ButtonList.Btn_Save, this.Btn_Save);
        this.AddBtn(ButtonList.Btn_Send, this.Btn_Send);
        this.AddBtn(ButtonList.Btn_Track, this.Btn_Track);
        this.AddBtn(ButtonList.Btn_UnSend, this.Btn_UnSend);
        this.InitButtonState();
    }
    public Hashtable GenerWorkInfo()
    {
        Hashtable ht = new Hashtable();
        ht.Add("qingjiatian", float.Parse(this.TB_qingjiatian.Text));
        return ht;
    }
    protected void Btn_Return_Click(object sender, EventArgs e)
    {

    }
    protected void Btn_Send_Click(object sender, EventArgs e)
    {
        string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, this.GenerWorkInfo()).ToMsgOfHtml();
        msg = msg.Replace("@@", "@");
        msg = msg.Replace("@", "<BR>@");

        this.Alert("发送提示", msg);
        this.InitButtonState();
    }

    protected void Btn_UnSend_Click(object sender, EventArgs e)
    {
        /* 编写您的表单撤销业务逻辑 */
        string msg = BP.WF.Dev2Interface.Flow_DoUnSend(this.FK_Flow, this.WorkID);
        msg = msg.Replace("@@", "@");
        msg = msg.Replace("@", "<BR>@");
        this.Alert("撤销提示", msg);
        this.InitButtonState();
       
    }
}