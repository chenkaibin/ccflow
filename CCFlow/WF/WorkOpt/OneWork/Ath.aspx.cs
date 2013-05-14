using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.DA;

namespace CCFlow.WF.OneWork
{
public partial class WF_WorkOpt_OneWork_Ath : BP.Web.WebPage
{
    #region attr
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    public int OID
    {
        get
        {
            return int.Parse(this.Request.QueryString["WorkID"]);
        }
    }
    public int FID
    {
        get
        {
            return int.Parse(this.Request.QueryString["FID"]);
        }
    }
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        #region 处理风格
        this.Page.RegisterClientScriptBlock("s",
         "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");
        #endregion 处理风格

        string flowID = int.Parse(this.FK_Flow).ToString();
        string sql = "SELECT * FROM Sys_FrmAttachmentDB WHERE FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE FK_MapData LIKE 'ND" + flowID + "%' AND IsUpload=1) AND RefPKVal='" + this.OID + "' ORDER BY RDT";
        DataTable dt = DBAccess.RunSQLReturnTable(sql);
        if (dt.Rows.Count > 0)
        { 
            this.Pub1.AddTable();
        //    this.Pub1.AddCaptionLeft("流程附件");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("附件编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("大小(kb)");
            this.Pub1.AddTDTitle("上传人");
            this.Pub1.AddTDTitle("上传日期");
            this.Pub1.AddTREnd();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(i);
                this.Pub1.AddTD(dr["FK_FrmAttachment"].ToString());
                this.Pub1.AddTD("<a href='" + this.Request.ApplicationPath + "WF/CCForm/AttachmentUpload.aspx?DoType=Down&MyPK=" + dr["MyPK"] + "' target=_sd ><img src='/WF/Img/FileType/" + dr["FileExts"] + ".gif' onerror=\"this.src='/WF/Img/FileType/Undefined.gif'\" border=0/>" + dr["FileName"].ToString() + "</a>");
                this.Pub1.AddTD(dr["FileSize"].ToString());
                this.Pub1.AddTD(dr["RecName"].ToString());
                this.Pub1.AddTD(dr["RDT"].ToString());
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        Bills bills = new Bills();
        bills.Retrieve(BillAttr.WorkID, this.OID);
        if (bills.Count > 0)
        {
            this.Pub1.AddTable();
        //    this.Pub1.AddCaptionLeft("单据");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("打印人");
            this.Pub1.AddTDTitle("日期");
            this.Pub1.AddTDTitle("功能");
            this.Pub1.AddTREnd();
            int idx = 0;
            foreach (Bill bill in bills)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);

                this.Pub1.AddTD(bill.FK_BillTypeT);

                this.Pub1.AddTD(bill.FK_NodeT);
                this.Pub1.AddTD(bill.FK_EmpT);
                this.Pub1.AddTD(bill.RDT);
                this.Pub1.AddTD("<a href='" + this.Request.ApplicationPath + "WF/Rpt/Bill.aspx?MyPK=" + bill.MyPK + "&DoType=Print' >打印</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        int num = bills.Count + dt.Rows.Count;
        if (num == 0)
        {
            this.Pub1.AddMsgGreen("提示", "<h2>当前流程没有数据，或者该流程没有附件或者单据。</h2>");
        }
    }
}
}