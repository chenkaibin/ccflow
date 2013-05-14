using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Port;
using BP.WF;

namespace CCFlow.WF.WorkOpt
{
public partial class WF_WorkOpt_PrintDoc : BP.Web.WebPage
{
    #region 属性
    public int FK_Node
    {
        get
        {
            return int.Parse(this.Request.QueryString["FK_Node"]);
        }
    }
    public Int64 WorkID
    {
        get
        {
            return int.Parse(this.Request.QueryString["WorkID"]);
        }
    }
    public string FK_Bill
    {
        get
        {
            return  this.Request.QueryString["FK_Bill"];
        }
    }
    #endregion end 属性

    protected void Page_Load(object sender, EventArgs e)
    {
        BillTemplates templetes = new BillTemplates();
        templetes.Retrieve(BillTemplateAttr.NodeID, this.FK_Node);
        if (templetes.Count == 0)
        {
            this.WinCloseWithMsg("当前节点上没有绑定单据模板。");
            return;
        }

        if (templetes.Count == 1)
        {
            PrintDoc(templetes[0] as BillTemplate);
            return;
        }

        this.Pub1.AddTable();
        this.Pub1.AddCaptionLeft("请选择要打印的单据");
        this.Pub1.AddTR();
        this.Pub1.AddTDTitle("单据编号");
        this.Pub1.AddTDTitle("单据名称");
        this.Pub1.AddTDTitle("打印");
        this.Pub1.AddTREnd();

        foreach (BillTemplate en in templetes)
        {
            this.Pub1.AddTR();
            this.Pub1.AddTD(en.No);
            this.Pub1.AddTD(en.Name);
            this.Pub1.AddTD("<a href='PrintDoc.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Bill=" + en.No + "' >打印</a>");
            this.Pub1.AddTREnd();
        }
        this.Pub1.AddTableEnd();


        if (this.FK_Bill != null)
        {
            BillTemplate templete = new BillTemplate(this.FK_Bill);
            this.PrintDoc(templete);
        }
    }

    public void PrintDoc(BillTemplate en)
    {
        Node nd = new Node(this.FK_Node);
        Work wk = nd.HisWork;
        wk.OID = this.WorkID;
        wk.Retrieve();
        string msg = "";
        string file = DataType.CurrentYear + "_" + WebUser.FK_Dept + "_" + en.No + "_" + this.WorkID + ".doc";
        BP.Rpt.RTF.RTFEngine rtf = new BP.Rpt.RTF.RTFEngine();
//        Works works;
        string[] paths;
        string path;
        try
        {
            #region 生成单据
            rtf.HisEns.Clear();
            rtf.EnsDataDtls.Clear();
            rtf.AddEn(wk);
            rtf.ensStrs += ".ND" + wk.NodeID;
            ArrayList al = wk.GetDtlsDatasOfArrayList();
            foreach (Entities ens in al)
                rtf.AddDtlEns(ens);

            BP.Sys.GEEntity ge = new BP.Sys.GEEntity("ND" + int.Parse(nd.FK_Flow) + "Rpt");
            ge.Copy(wk);
            rtf.HisGEEntity = ge;

            paths = file.Split('_');
            path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";

            path = BP.WF.Glo.FlowFileBill + DataType.CurrentYear + "\\" + WebUser.FK_Dept + "\\" + en.No + "\\";
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);
            // rtf.ensStrs = ".ND";
            rtf.MakeDoc(en.Url + ".rtf",
                path, file, en.ReplaceVal, false);
            #endregion

            #region 转化成pdf.
            if (en.HisBillFileType == BillFileType.PDF)
            {
                string rtfPath = path + file;
                string pdfPath = rtfPath.Replace(".doc", ".pdf");
                try
                {
                    Glo.Rtf2PDF(rtfPath, pdfPath);

                    file = file.Replace(".doc", ".pdf");
                    System.IO.File.Delete(rtfPath);

                    file = file.Replace(".doc", ".pdf");
                    //System.IO.File.Delete(rtfPath);
                }
                catch (Exception ex)
                {
                    msg += ex.Message;
                }
            }
            #endregion

            string url = this.Request.ApplicationPath + "/DataUser/Bill/" + DataType.CurrentYear + "/" + WebUser.FK_Dept + "/" + en.No + "/" + file;
            this.Response.Redirect(url, false);
            //         BP.PubClass.OpenWordDocV2( path+file, en.Name);
        }
        catch (Exception ex)
        {
            BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
            dir.Do();
            path = BP.WF.Glo.FlowFileBill + DataType.CurrentYear + "\\" + WebUser.FK_Dept + "\\" + en.No + "\\";
            string msgErr = "@生成单据失败，请让管理员检查目录设置 [" + BP.WF.Glo.FlowFileBill + "]。@Err：" + ex.Message + " @File=" + file + " @Path:" + path;
            throw new Exception(msgErr + "@其它信息:" + ex.Message);
        }
    }
}

}