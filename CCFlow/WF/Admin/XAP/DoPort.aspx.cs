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
using BP.Web;
using BP.DA;
using BP.En;
namespace CCFlow.WF.Admin.XAP
{

    public partial class DoPort : WebPage
    {
        #region 属性。
        public string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public string PK
        {
            get
            {
                string s = this.Request.QueryString["PK"];
                if (s.Contains("ND0") == true)
                    s = s.Replace("ND00", "ND");
                if (s.Contains("ND0") == true)
                    s = s.Replace("ND00", "ND");
                return s;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string PassKey
        {
            get
            {
                return this.Request.QueryString["PassKey"];
            }
        }
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.Browser.Cookies == false)
            {
                this.Response.Write("您的浏览器不支持cookies功能，无法使用改系统。");
                return;
            }

            if (this.PassKey != BP.SystemConfig.AppSettings["PassKey"])
                return;

            //if (this.Lang == null || this.Lang == "")
            //    throw new Exception("语言编号错误。");

         //   BP.SystemConfig.DoClearCash();
            BP.Port.Emp emp = new BP.Port.Emp("admin");
            BP.Web.WebUser.SignInOfGenerLang(emp, "CH");

            string fk_flow = this.Request.QueryString["FK_Flow"];
            string fk_Node = this.Request.QueryString["FK_Node"];

            string FK_MapData = this.Request.QueryString["FK_MapData"];
            if (string.IsNullOrEmpty(FK_MapData))
                FK_MapData = this.Request.QueryString["PK"];

            switch (this.DoType)
            {
                case "DownFormTemplete":
                    BP.Sys.MapData md = new BP.Sys.MapData(FK_MapData);
                    DataSet ds = md.GenerHisDataSet();
                    string file = BP.SystemConfig.PathOfTemp + md.No + ".xml";
                    ds.WriteXml(file);
                    System.IO.FileInfo f = new System.IO.FileInfo(file);
                    BP.PubClass.DownloadFile(f.FullName, md.Name + ".xml");
                    this.Pub1.AddFieldSet("下载提示");

                    string url = "../../../Temp/" + md.No + ".xml";
                    this.Pub1.AddH2("ccflow 已经完成模板的生成了，正在执行下载如果您的浏览器没有反应请<a href='" + url + "' >点这里进行下载</a>。");
                    this.Pub1.Add("如果该xml文件是在ie里直接打开的，请把鼠标放在连接上右键目标另存为，保存该模板。");
                    this.Pub1.AddFieldSetEnd();
                    return;
                case "Ens": // 实体编辑.
                    this.Response.Redirect("/WF/Comm/Batch.aspx?EnsName=" + this.EnsName, true);
                    break;
                case "En": // 单个实体编辑. 
                    switch (this.EnName)
                    {
                        case "BP.WF.Flow":
                            Flow fl = new Flow(this.PK);
                            if (fl.HisFlowSheetType == FlowSheetType.DocFlow)
                                this.Response.Redirect("/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Ext.FlowDocs&No=" + this.PK, true);
                            else
                                this.Response.Redirect("/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Ext.FlowSheets&No=" + this.PK, true);
                            break;
                        case "BP.WF.Node":
                            Node nd = new Node(this.PK);
                            this.Response.Redirect("/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Ext.NodeSheets&PK=" + this.PK, true);
                            break;
                        default:
                            this.Response.Redirect("/WF/Comm/RefFunc/UIEn.aspx?EnsName=" + this.EnsName + "&No=" + this.PK, true);
                            break;
                    }
                    break;
                case "FrmLib": //"表单库"
                    this.Response.Redirect("../FlowFrms.aspx?ShowType=FrmLab&FK_Flow=" + fk_flow + "&FK_Node=" + fk_Node + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "FlowFrms": //"流程表单"
                    this.Response.Redirect("../FlowFrms.aspx?ShowType=FlowFrms&FK_Flow=" + fk_flow + "&FK_Node=" + fk_Node + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "StaDef": // 节点岗位.
                    this.Response.Redirect("/WF/Comm/RefFunc/Dot2Dot.aspx?EnName=BP.WF.Ext.NodeSheet&AttrKey=BP.WF.NodeStations&PK=" + this.PK + "&NodeID=" + this.PK + "&RunModel=0&FLRole=0&FJOpen=0&r=" + this.PK, true);
                    break;
                case "WFRpt": // 报表设计.r
                    this.Response.Redirect("../../MapDef/Rpt/Home.aspx?FK_MapData=ND" + int.Parse(this.PK) + "Rpt&FK_Flow=" + this.PK, true);
                    break;
                case "MapDef": //表单定义.
                    int nodeid = int.Parse(this.PK.Replace("ND", ""));
                    Node nd1 = new Node();
                    nd1.NodeID = nodeid;
                    nd1.RetrieveFromDBSources();
                    if (nd1.HisFormType == FormType.FreeForm)
                        this.Response.Redirect("../../MapDef/CCForm/Frm.aspx?FK_MapData=" + this.PK + "&FK_Flow=" + nd1.FK_Flow, true);
                    else
                        this.Response.Redirect("../../MapDef/MapDef.aspx?PK=" + this.PK + "&FK_Flow=" + nd1.FK_Flow, true);
                    break;
                case "MapDefFixModel": // 表单定义.
                case "FormFixModel":
                    this.Response.Redirect("../../MapDef/MapDef.aspx?FK_MapData=" + FK_MapData + "&FK_Flow=" + this.FK_Flow, true);
                    break;
                case "MapDefFreeModel": // 表单定义.
                case "FormFreeModel":
                    this.Response.Redirect("../../MapDef/CCForm/Frm.aspx?FK_MapData=" + FK_MapData + "&FK_Flow=" + this.FK_Flow, true);
                    break;
                case "MapDefFree": //表单定义.
                    int nodeidFree = int.Parse(this.PK.Replace("ND", ""));
                    Node ndFree = new Node(nodeidFree);
                    this.Response.Redirect("../MapDef/CCForm/Frm.aspx?FK_MapData=" + this.PK + "&FK_Flow=" + ndFree.FK_Flow, true);
                    break;
                case "MapDefF4": //表单定义.
                    int nodeidF4 = int.Parse(this.PK.Replace("ND", ""));
                    Node ndF4 = new Node(nodeidF4);
                    this.Response.Redirect("../../MapDef/MapDef.aspx?PK=" + this.PK + "&FK_Flow=" + ndF4.FK_Flow, true);
                    break;
                case "Dir": // 方向。
                    this.Response.Redirect("../Admin/Cond.aspx?CondType=" + this.Request.QueryString["CondType"] + "&FK_Flow=" + this.Request.QueryString["FK_Flow"] + "&FK_MainNode=" + this.Request.QueryString["FK_MainNode"] + "&FK_Node=" + this.Request.QueryString["FK_Node"] + "&FK_Attr=" + this.Request.QueryString["FK_Attr"] + "&DirType=" + this.Request.QueryString["DirType"] + "&ToNodeID=" + this.Request.QueryString["ToNodeID"], true);
                    break;
                case "RunFlow": //运行流程
                    this.Response.Redirect("../Admin/StartFlow.aspx?FK_Flow=" + fk_flow + "&Lang=" + BP.Web.WebUser.SysLang, true);
                    break;
                case "FlowCheck": // 流程设计
                    this.Response.Redirect("../Admin/DoType.aspx?RefNo=" + this.Request.QueryString["RefNo"] + "&DoType=" + this.DoType, true);
                    break;
                case "ExpFlowTemplete": //流程设计.
                    Flow flT = new Flow(this.Request.QueryString["FK_Flow"]);
                    string fileXml = flT.GenerFlowXmlTemplete();
                    BP.PubClass.DownloadFile(fileXml + flT.Name + ".xml", flT.Name + ".xml");
                    BP.PubClass.WinClose();
                    break;
                default:
                    throw new Exception("Error:" + this.DoType);
            }
        }
    }

}