using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using BP.WF;
using BP.En;

public partial class Demo_QingJiaTiao_S1_TianxieShenqingDan : FlowPage
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
        ht.Add("Title", this.TB_Title.Text);
        ht.Add("qingjiatian", float.Parse(this.TB_qingjiatian.Text));
        return ht;
    }
    protected void Btn_Send_Click(object sender, EventArgs e)
    {
       

        // 书写您的表单保存逻辑。
        // 处理ccflow的业务逻辑，仅把关键字段传递给ccflow的节点表单中去，用户判断方向。
        Hashtable ht = new Hashtable();
        ht.Add("Title", this.TB_Title.Text);
        ht.Add("qingjiatian", float.Parse(this.TB_qingjiatian.Text));

        string msg = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow,
            this.WorkID, ht).ToMsgOfHtml();

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
        this.Alert("撤销提示",msg);
        this.InitButtonState();
    }
    protected void Btn_Save_Click(object sender, EventArgs e)
    {
        Hashtable ht = new Hashtable();
        ht.Add("Title", this.TB_Title.Text);
        ht.Add("qingjiatian", float.Parse(this.TB_qingjiatian.Text));

        /* 编写您的表单保存业务逻辑 */
        string msg = BP.WF.Dev2Interface.Node_SaveWork(this.FK_Flow,this.FK_Node,
            this.WorkID, ht,null);
        this.Alert("保存提示信息", msg);
    }

    protected void Btn_Return_Click(object sender, EventArgs e)
    {
        // 退回前的业务处理
        this.DoOpenReturn();

        // 退回后的业务处理
    }
}