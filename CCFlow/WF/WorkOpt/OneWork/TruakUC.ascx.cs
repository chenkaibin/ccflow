﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.WorkOpt.OneWork
{
    public partial class TruakUC : BP.Web.UC.UCBase3
    {
        #region 属性
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.FK_Flow + "01");
            }
        }
        public string FK_Flow
        {
            get
            {
                string flow = this.Request.QueryString["FK_Flow"];
                if (flow == null)
                {
                    throw new Exception("@没有获取它的流程编号。");
                    //BP.WF.CHOfFlow fl = new CHOfFlow(this.WorkID);
                    //return fl.FK_Flow;
                }
                else
                {
                    return flow;
                }
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["NodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion

        public void ViewWork()
        {
            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node, ReturnWorkAttr.WorkID, this.WorkID);

            ForwardWorks fws = new ForwardWorks();
            fws.Retrieve(ForwardWorkAttr.FK_Node, this.FK_Node, ForwardWorkAttr.WorkID, this.WorkID);

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            this.AddB(wk.EnDesc);
            this.ADDWork(wk, rws, fws, this.FK_Node);
        }
        public void BindTrack_ViewSpecialWork()
        {
            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node, ReturnWorkAttr.WorkID, this.WorkID);

            ForwardWorks fws = new ForwardWorks();
            fws.Retrieve(ForwardWorkAttr.FK_Node, this.FK_Node, ForwardWorkAttr.WorkID, this.WorkID);

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            this.AddB(wk.EnDesc);
            this.ADDWork(wk, rws, fws, this.FK_Node);
        }
        /// <summary>
        /// view work.
        /// </summary>
        public void BindTrack_ViewWork()
        {
            string appPath = this.Request.ApplicationPath;
            Track tk = new Track(this.FK_Flow, this.MyPK);
            Node nd = new Node(tk.NDFrom);
            Work wk = nd.HisWork;
            wk.OID = tk.WorkID;
            if (wk.RetrieveFromDBSources() == 0)
            {
                this.UCEn1.AddFieldSet("打开(" + nd.Name + ")错误");
                this.UCEn1.AddH1("当前的节点数据已经被删除！！！<br> 造成此问题出现的原因如下。");
                this.UCEn1.AddBR("1、当前节点数据被非法删除。");
                this.UCEn1.AddBR("2、节点数据是退回人与被退回人中间的节点，这部分节点数据查看不支持。");
                this.UCEn1.AddFieldSetEnd();
                return;
            }

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            if (gwf.RetrieveFromDBSources() == 0)
            {

            }
            else
            {
                if (gwf.FK_Node == wk.NodeID)
                {
                    this.UCEn1.AddFieldSet(wk.EnDesc);
                    this.UCEn1.AddH1("当工作(" + nd.Name + ")未完成，您不能查看它的工作日志。");
                    this.UCEn1.AddFieldSetEnd();
                    return;
                }
            }

            if (nd.HisFlow.IsMD5 && wk.IsPassCheckMD5() == false)
            {
                this.UCEn1.AddFieldSet("打开(" + nd.Name + ")错误");
                this.UCEn1.AddH1("当前的节点数据已经被篡改，请报告管理员。");
                this.UCEn1.AddFieldSetEnd();
                return;
            }

            this.UCEn1.IsReadonly = true;
            Frms frms = nd.HisFrms;
            if (frms.Count == 0)
            {
                if (nd.HisFormType == FormType.FreeForm)
                {
                    /* 自由表单 */
                    this.UCEn1.Add("<div id=divCCForm >");
                    this.UCEn1.BindCCForm(wk, "ND" + nd.NodeID, true); //, false, false, null);
                    if (wk.WorkEndInfo.Length > 2)
                        this.UCEn1.Add(wk.WorkEndInfo);
                    this.UCEn1.Add("</div>");
                }

                if (nd.HisFormType == FormType.FixForm)
                {
                    /*傻瓜表单*/
                    this.UCEn1.IsReadonly = true;
                    this.UCEn1.BindColumn4(wk, "ND" + nd.NodeID); //, false, false, null);
                    if (wk.WorkEndInfo.Length > 2)
                        this.UCEn1.Add(wk.WorkEndInfo);
                }

                BillTemplates bills = new BillTemplates();
                bills.Retrieve(BillTemplateAttr.NodeID, nd.NodeID);
                if (bills.Count >= 1)
                {
                    string title = "";
                    foreach (BillTemplate item in bills)
                        title += "<img src='/WF/Img/Btn/Word.gif' border=0/>" + item.Name + "</a>";

                    string urlr = appPath + "WF/WorkOpt/PrintDoc.aspx?FK_Node=" + nd.NodeID + "&FID=" + tk.FID + "&WorkID=" + tk.WorkID + "&FK_Flow=" + tk.FK_Flow;
                    this.UCEn1.Add("<p><a  href=\"javascript:WinOpen('" + urlr + "','dsdd');\"  />" + title + "</a></p>");
                    //this.UCEn1.Add("<a href='' target=_blank><img src='/WF/Img/Btn/Word.gif' border=0/>" + bt.Name + "</a>");
                }
            }
            else
            {
                /* 涉及到多个表单的情况...*/
                if (nd.HisFormType != FormType.DisableIt)
                {
                    Frm myfrm = new Frm();
                    myfrm.No = "ND" + nd.NodeID;
                    myfrm.Name = wk.EnDesc;
                    myfrm.HisFormType = nd.HisFormType;

                    FrmNode fnNode = new FrmNode();
                    fnNode.FK_Frm = myfrm.No;
                    fnNode.IsEdit = true;
                    fnNode.IsPrint = false;
                    switch (nd.HisFormType)
                    {
                        case FormType.FixForm:
                            fnNode.HisFrmType = FrmType.Column4Frm;
                            break;
                        case FormType.FreeForm:
                            fnNode.HisFrmType = FrmType.CCForm;
                            break;
                        case FormType.SelfForm:
                            fnNode.HisFrmType = FrmType.Url;
                            break;
                        default:
                            throw new Exception("出现了未判断的异常。");
                    }
                    myfrm.HisFrmNode = fnNode;
                    frms.AddEntity(myfrm, 0);
                }

                Int64 fid = this.FID;
                if (this.FID == 0)
                    fid = tk.WorkID;

                if (frms.Count == 1)
                {
                    /* 如果禁用了节点表单，并且只有一个表单的情况。*/
                    Frm frm = (Frm)frms[0];
                    FrmNode fn = frm.HisFrmNode;
                    string src = "";
                    src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=0&IsPrint=0&FK_Node=" + nd.NodeID + "&WorkID=" + tk.WorkID;
                    this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                    this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'  /></iframe>");
                    this.UCEn1.Add("\t\n </DIV>");
                }
                else
                {
                    #region 载入相关文件.
                    this.Page.RegisterClientScriptBlock("sg",
       "<link href='./Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");

                    this.Page.RegisterClientScriptBlock("s2g4",
             "<script language='JavaScript' src='./Style/Frm/jquery.min.js' ></script>");

                    this.Page.RegisterClientScriptBlock("sdf24j",
            "<script language='JavaScript' src='./Style/Frm/jquery.idTabs.min.js' ></script>");

                    this.Page.RegisterClientScriptBlock("sdsdf24j",
            "<script language='JavaScript' src='./Style/Frm/TabClick.js' ></script>");
                    #endregion 载入相关文件.

                    this.UCEn1.Clear();
                    this.UCEn1.Add("<div  style='clear:both' ></div>");
                    this.UCEn1.Add("\t\n<div  id='usual2' class='usual' >");  //begain.

                    #region 输出标签.
                    this.UCEn1.Add("\t\n <ul  class='abc' style='background:red;border-color: #800000;border-width: 10px;' >");
                    foreach (Frm frm in frms)
                    {
                        FrmNode fn = frm.HisFrmNode;
                        string src = "";
                        src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=0&IsPrint=0&FK_Node=" + nd.NodeID + "&WorkID=" + tk.WorkID;
                        this.UCEn1.Add("\t\n<li><a href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" + frm.Name + "</a></li>");
                    }
                    this.UCEn1.Add("\t\n </ul>");
                    #endregion 输出标签.

                    #region 输出表单 iframe 内容.
                    foreach (Frm frm in frms)
                    {
                        FrmNode fn = frm.HisFrmNode;
                        this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                        string src = "loading.htm";
                        this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'   /></iframe>");
                        this.UCEn1.Add("\t\n </DIV>");
                    }
                    #endregion 输出表单 iframe 内容.

                    this.UCEn1.Add("\t\n</div>"); // end  usual2

                    // 设置选择的默认值.
                    this.UCEn1.Add("\t\n<script type='text/javascript'>");
                    this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frms[0].No + "\");");
                    this.UCEn1.Add("\t\n</script>");
                }
            }
        }
        public void BindTrack()
        {
            this.Page.Title = "感谢您使用ccflow";
            if (this.DoType == "View")
            {
                this.BindTrack_ViewWork();
                return;
            }

            if (this.DoType == "ViewSpecialWork")
            {
                this.BindTrack_ViewSpecialWork();
                return;
            }

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("IDX");
            this.AddTDTitle("日期时间");
            this.AddTDTitle("从节点");
            this.AddTDTitle("人员");
            this.AddTDTitle("到节点");
            this.AddTDTitle("人员");
            this.AddTDTitle("活动");
            this.AddTDTitle("信息");
            this.AddTDTitle("表单");
            this.AddTDTitle("执行人");
            this.AddTREnd();

            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";

            string dbStr = BP.SystemConfig.AppCenterDBVarStr;
            Paras prs = new Paras();
            if (this.FID == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                prs.Add("WorkID11", this.WorkID);
                prs.Add("WorkID12", this.WorkID);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                prs.Add("FID11", this.FID);
                prs.Add("FID12", this.FID);
            }

            string sql = "";
            sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer FROM ND" + int.Parse(this.FK_Flow) + "Track " + sqlOfWhere1;
            prs.SQL = sql;

            DataTable dt = DBAccess.RunSQLReturnTable(prs);
            DataView dv = dt.DefaultView;
            dv.Sort = "RDT";

            // dv.RowFilter

            int idx = 1;
            foreach (DataRowView dr in dv)
            {
                this.AddTR();
                this.AddTDIdx(idx++);
                DateTime dtt = DataType.ParseSysDateTime2DateTime(dr[TrackAttr.RDT].ToString());
                this.AddTD(dtt.ToString("MM月dd日HH:mm"));

                this.AddTD(dr[TrackAttr.NDFromT].ToString());
                this.AddTD(dr[TrackAttr.EmpFromT].ToString());
                this.AddTD(dr[TrackAttr.NDToT].ToString());
                this.AddTD(dr[TrackAttr.EmpToT].ToString());

                ActionType at = (ActionType)int.Parse(dr[TrackAttr.ActionType].ToString());

                this.AddTD("<img src='./../../Img/Action/" + at.ToString() + ".png' class='ActionType' border=0/>" + BP.WF.Track.GetActionTypeT(at) );

                this.AddTD(DataType.ParseText2Html(dr[TrackAttr.Msg].ToString()));

                //this.AddTD(item.NDToT);
                //this.AddTD(item.EmpToT);
                //this.AddTD(item.HisActionTypeT);
                //this.AddTDDoc(item.MsgHtml);

                this.AddTD("<a href=\"javascript:WinOpen('" + this.Request.ApplicationPath + "WF/WFRpt.aspx?WorkID=" + dr[TrackAttr.WorkID].ToString() + "&FK_Flow=" + this.FK_Flow + "&DoType=View&MyPK=" + dr[TrackAttr.MyPK].ToString() + "','" + dr[TrackAttr.MyPK].ToString() + "');\">表单</a>");
                this.AddTD(dr[TrackAttr.Exer].ToString());
                this.AddTREnd();
            }
            this.AddTableEnd();

            if (this.CCID != null)
            {
                CCList cl = new CCList();
                cl.MyPK = this.CCID;
                cl.RetrieveFromDBSources();
                this.AddFieldSet(cl.Title);
                this.Add("抄送人:" + cl.Rec + ", 抄送日期:" + cl.RDT);
                this.AddHR();
                this.Add(cl.DocHtml);
                this.AddFieldSetEnd();

                if (cl.HisSta == CCSta.UnRead)
                {
                    cl.HisSta = CCSta.Read;
                    cl.Update();
                }
            }
        }
        public string CCID
        {
            get
            {
                return this.Request.QueryString["CCID"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindTrack();
        }

        #region 分流
        /// <summary>
        /// 分流支流
        /// </summary>
        /// <param name="fl"></param>
        public void BindBrach(Flow fl)
        {
            //  WorkFlow wf = new WorkFlow(fl, this.WorkID, this.FID);
            WorkNodes wns = new WorkNodes();
            wns.GenerByFID(fl, this.FID);

            this.AddH4("关于（" + fl.Name + "）工作报告");
            this.AddHR();

            Node nd = fl.HisStartNode;

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            this.Add("流程发起人：" + gwf.StarterName + "，发起日期：" + gwf.RDT + " ；流程状态：" + gwf.WFStateText);

            ReturnWorks rws = new ReturnWorks();
            rws.Retrieve(ReturnWorkAttr.WorkID, this.WorkID);

            ForwardWorks fws = new ForwardWorks();
            fws.Retrieve(ReturnWorkAttr.WorkID, this.WorkID);

            //  this.BindWorkNodes(wns, rws, fws);

            this.AddHR("流程报告结束");
        }
        #endregion 分流

        #region 合流
        /// <summary>
        /// 合流干流
        /// </summary>
        /// <param name="fl"></param>
        public void BindHeLiuRavie(Flow fl)
        {
        }
        /// <summary>
        /// 合流支流
        /// </summary>
        /// <param name="fl"></param>
        public void BindHeLiuBrach(Flow fl)
        {
        }
        #endregion 合流

        public void BindFHLWork(GenerFH hf)
        {
            this.AddH4(hf.Title);

            this.AddHR();
            this.AddFieldSet("当前节点基本信息");
            this.AddBR("接受时间：" + hf.RDT);
            this.AddBR("接受人：" + hf.ToEmpsMsg);
            this.AddFieldSetEndBR();

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FID, this.FID);

            this.AddFieldSet("分流人员信息");

            this.AddTable();
            this.AddTR();
            this.AddTDTitle("标题");
            this.AddTDTitle("发起人");
            this.AddTDTitle("发起日期");
            this.AddTDTitle("");
            this.AddTREnd();

            foreach (GenerWorkFlow gwf in gwfs)
            {
                if (gwf.WorkID == this.FID)
                    continue;

                this.AddTR();
                this.AddTD(gwf.Title);
                this.AddTD(gwf.Starter);
                this.AddTD(gwf.RDT);
                this.AddTD("<a href='" + this.Request.ApplicationPath + "WF/WFRpt.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=" + gwf.FID + "' target=_b" + gwf.WorkID + ">工作报告</a>");
                this.AddTREnd();
            }
            this.AddTableEndWithBR();
            this.AddFieldSetEnd();
        }

        protected void AddContral(string desc, string text)
        {
            this.Add("<td  class='FDesc' nowrap> " + desc + "</td>");
            this.Add("<td width='40%' class=TD>");
            if (text == "")
                text = "&nbsp;";
            this.Add(text);
            this.AddTDEnd();
        }
        /// <summary>
        /// 增加一个工作
        /// </summary>
        /// <param name="en"></param>
        /// <param name="rws"></param>
        /// <param name="fws"></param>
        /// <param name="nodeId"></param>
        public void ADDWork(Work en, ReturnWorks rws, ForwardWorks fws, int nodeId)
        {


            this.BindViewEn(en, "width=90%");
            foreach (ReturnWork rw in rws)
            {
                if (rw.ReturnToNode != nodeId)
                    continue;

                this.AddBR();
                this.AddMsgOfInfo("退回信息：", rw.NoteHtml);
            }

            foreach (ForwardWork fw in fws)
            {
                if (fw.FK_Node != nodeId)
                    continue;

                this.AddBR();
                this.AddMsgOfInfo("转发信息：", fw.NoteHtml);
            }


            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStrByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            #region 加入他的明细
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "WFRptDtl.aspx?RefPK=" + en.PKVal.ToString() + "&EnName=" + enDtl.Ens.GetNewEntity.ToString();
                    int i = 0;
                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a  href=\"javascript:WinOpen('" + url + "','u8');\" >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入一对多的实体编辑
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    string sql = "SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'";
                    int i = DBAccess.RunSQLReturnValInt(sql);

                    if (i == 0)
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "' target='_blank' >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region 加入他门的相关功能
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					//int i=DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM "+vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable+" WHERE "+vsM.AttrOfMInMM+"='"+en.PKVal+"'");
            //					refstrs+="[<a href='"+url+"' target='_blank' >"+en1.Name+"</a>]";
            //					//refstrs+="编辑: <a href=\"javascript:window.open(RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+ens.ToString()+"'> )\" > "+en1.Name+"</a>";
            //					//var newWindow= window.open( this.Request.ApplicationPath+'/Comm/'+'RefFuncLink.aspx?RefFuncOID='+OID+'&MainEnsName='+ CurrEnsName +CurrKeys,'chosecol', 'width=100,top=400,left=400,height=50,scrollbars=yes,resizable=yes,toolbar=false,location=false' );
            //					//refstrs+="编辑: <a href=\"javascript:EnsRefFunc('"+en1.OID.ToString()+"')\" > "+en1.Name+"</a>";
            //					//refstrs+="编辑:"+en1.Name+"javascript: EnsRefFunc('"+en1.OID.ToString()+"',)";
            //					//this.AddItem(en1.Name,"EnsRefFunc('"+en1.OID.ToString()+"')",en1.Icon);
            //				}
            //			}
            #endregion

            // 不知道为什么去掉。
            this.Add(refstrs);
        }

    }
}