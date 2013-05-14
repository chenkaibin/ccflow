using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AppDemo_Left : System.Web.UI.Page
{
    /// <summary>
    /// 返回待办件数量
    /// </summary>
    /// <returns></returns>
    public int EmpWorks
    {
        get
        {
            string sql = "SELECT COUNT(*) AS Num FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND WFState=2";
            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
    }
    /// <summary>
    /// 返回抄送件数量
    /// </summary>
    /// <returns></returns>
    public int CCNum
    {
        get
        {
            string sql = "SELECT COUNT(*) AS Num FROM WF_CCList WHERE CCTo='" + BP.Web.WebUser.No + "' AND Sta=0";
            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
    }
    /// <summary>
    /// 返回挂起流程数量
    /// </summary>
    public int HungUpNum
    {
        get
        {
            string sql = "SELECT COUNT(*) AS Num FROM WF_HungUp WHERE Rec='" + BP.Web.WebUser.No + "'";
            return BP.DA.DBAccess.RunSQLReturnValInt(sql);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}