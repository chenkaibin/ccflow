using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;
using BP.WF;

/// <summary>
///FlowPage 的摘要说明
/// </summary>
public class FlowPage : System.Web.UI.Page
{
    #region 属性
    public Int64 WorkID
    {
        get
        {
            return Int64.Parse(this.Request.QueryString["WorkID"]);
        }
    }
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    public int FK_Node
    {
        get
        {
            string str= this.Request.QueryString["FK_Node"];
            if (str == null || str=="")
                str = this.Request.QueryString["NodeID"];
            if (str == null || str == "")
                str = this.FK_Flow + "01";

            return int.Parse(str);
        }
    }
    public string TrackUrl
    {
        get
        {
            return this.Request.ApplicationPath + "WF/Track.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
        }
    }
    #endregion

    #region 空间
    public void AddBtn(string id, Button btn)
    {
        if (this.Btns == null)
            this.Btns = new System.Collections.Hashtable();

        if (this.Btns.ContainsKey(id))
            this.Btns.Remove(id);

        switch (id)
        {
            case ButtonList.Btn_UnSend:
            case ButtonList.Btn_NewFlow:
            case ButtonList.Btn_Return:
            case ButtonList.Btn_Track:
               // btn.Click = null;
                 //  btn.runat
                //btn.EnableViewState
                break;
            default:
                break;
        }
        this.Btns.Add(id, btn);
    }
    public System.Collections.Hashtable  Btns= null;
    public Button Btn_NewFlow
    {
        get
        {
            return this.Btns["Btn_NewFlow"] as Button;
        }
    }
    public Button Btn_Send
    {
        get
        {
            return this.Btns["Btn_Send"] as Button;
        }
    }
    public Button Btn_Save
    {
        get
        {
            return this.Btns["Btn_Save"] as Button;
        }
    }
    public Button Btn_Return
    {
        get
        {
            return this.Btns["Btn_Return"] as Button;
        }
    }
    public Button Btn_Forward
    {
        get
        {
            return this.Btns["Btn_Forward"] as Button;
        }
    }
    public Button Btn_DelFlow
    {
        get
        {
            return this.Btns["Btn_DelFlow"] as Button;
        }
    }
    public Button Btn_UnSend
    {
        get
        {
            return this.Btns["Btn_UnSend"] as Button;
        }
    }
    public Button Btn_Track
    {
        get
        {
            return this.Btns["Btn_Track"] as Button;
        }
    }
    #endregion

    /// <summary>
    /// 初始化按钮的状态
    /// </summary>
    public void InitButtonState()
    {
        string appPath = this.Request.ApplicationPath.Trim();
        string scrip = "";
        ButtonState bs = BP.WF.Dev2Interface.UI_GetButtonState(this.FK_Flow,this.FK_Node, this.WorkID);
        if (this.Btn_NewFlow != null)
        {
            this.Btn_NewFlow.Enabled = bs.Btn_NewFlow;
            if (bs.Btn_NewFlow == true)
                this.Btn_Return.Attributes["onclick"] = " return NewFlow('" + appPath + "','" + this.FK_Flow + "','" + this.WorkID + "');";
        }

        if (this.Btn_Send != null)
            this.Btn_Send.Enabled = bs.Btn_Send;

        if (this.Btn_Save != null)
            this.Btn_Save.Enabled = bs.Btn_Save;

        if (this.Btn_Return != null)
        {
            this.Btn_Return.Enabled = bs.Btn_Return;
            if (bs.Btn_Return == true)
                this.Btn_Return.Attributes["onclick"] = " return Return('" + appPath + "','" + this.FK_Flow + "','"+this.FK_Node+"','" + this.WorkID + "');";
        }

        if (this.Btn_Forward != null)
        {
            this.Btn_Forward.Enabled = bs.Btn_Forward;
            if (bs.Btn_Forward == true)
                this.Btn_Forward.Attributes["onclick"] = " return Forward('" + appPath + "','" + this.FK_Flow + "','" + this.WorkID + "');";
        }

        if (this.Btn_DelFlow != null)
        {
            this.Btn_DelFlow.Enabled = bs.Btn_DelFlow;
            if (bs.Btn_DelFlow == true)
                this.Btn_DelFlow.Attributes["onclick"] = " return confirm('您确定要删除吗？');";
        }

        if (this.Btn_UnSend != null)
        {
            this.Btn_UnSend.Enabled = bs.Btn_UnSend;
            if (bs.Btn_UnSend == true)
                this.Btn_UnSend.Attributes["onclick"] = " return confirm('您确定要撤销吗？');";
        }

        if (this.Btn_Track != null)
            this.Btn_Track.Enabled = bs.Btn_Track;

        if (this.Btn_Track != null)
        {
            this.Btn_Track.Enabled = bs.Btn_Track;
            if (bs.Btn_Track == true)
                this.Btn_Track.Attributes["onclick"] = " return Track('" + appPath + "','" + this.FK_Flow + "','" + this.WorkID + "');";
        }
    }
    /// <summary>
    /// 信息提示
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">消息</param>
    public void Alert(string title, string msg)
    {
        this.Response.Write("<fieldset style='width:50%' ><legend>"+title+"</legend> "+msg+"</fieldset>");
    }
    public string DoGenerJSOfReturn()
    {
        string script = "";
        script += "var url='" + this.Request.ApplicationPath + "WF/ReturnWork.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "';";
        script += "var newWindow = window.open(url, 'z', 'scroll:1;status:1;help:1;resizable:1;dialogWidth:680px;dialogHeight:420px');";
        script += "newWindow.focus();";
        script += "return false;";
        return script;
    }
    public void DoOpenReturn()
    {
        string url = this.Request.ApplicationPath + "WF/ReturnWork.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
        this.WinOpen(url);
    }
    public void WinOpen(string url)
    {
        BP.PubClass.WinOpen(url);
    }
}