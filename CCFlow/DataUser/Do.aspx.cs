using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Web;
public partial class DataUser_Do : System.Web.UI.Page
{
    public string DoType
    {
        get
        {
            return this.Request.QueryString["DoType"];
        }
    }
    public string FK_Node
    {
        get
        {
            return this.Request.QueryString["FK_Node"];
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string s = "";
        foreach (string key   in this.Request.QueryString.AllKeys)
        {
            s += " , " + key + "=" + this.Request.QueryString[key];
        }
        this.Response.Write(s);
        Log.DefaultLogWriteLineError(s);
        return;

        try
        {
            switch (this.DoType)
            {
                case "OutError":
                    throw new Exception("您看到错误信息了吗？");
                case "OutOK":
                    /*在这是里处理您的业务过程。*/
                    return;
                default:
                    break;
            }
        }
        catch(Exception ex)
        {
            OutErrMsg(ex.Message);
        }
    }
    public void OutMsg(string msg)
    {
        this.Response.Write(msg);
    }
    public void OutErrMsg(string msg)
    {
        this.Response.Write("Error:"+msg);
    }
}