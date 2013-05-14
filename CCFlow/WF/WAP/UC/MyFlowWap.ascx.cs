using System;
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
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.WAP.UC
{
    public partial class MyFlowWap : BP.Web.UC.UCBase3
    {
        #region 控件
        public string _PageSamll = null;
        public string PageSmall
        {
            get
            {
                if (_PageSamll == null)
                {
                    if (this.PageID.ToLower().Contains("small"))
                        _PageSamll = "Small";
                    else
                        _PageSamll = "";
                }
                return _PageSamll;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        protected Btn Btn_Send
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.Send);
            }
        }
        protected Btn Btn_Delete
        {
            get
            {
                return this.ToolBar1.GetBtnByID(NamesOfBtn.Delete);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        protected Btn Btn_Save
        {
            get
            {
                Btn btn = this.ToolBar1.GetBtnByID(NamesOfBtn.Save);
                if (btn == null)
                    btn = new Btn();
                return btn;
            }
        }
        protected Btn Btn_ReturnWork
        {
            get
            {
                Btn btn = this.ToolBar1.GetBtnByID("Btn_ReturnWork");
                if (btn == null)
                    btn = new Btn();
                return btn;
            }
        }
        protected Btn Btn_Shift
        {
            get
            {
                return this.ToolBar1.GetBtnByID(BP.Web.Controls.NamesOfBtn.Shift);
            }
        }
        #endregion

        #region  运行变量
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrEmpty(s))
                    throw new Exception("@流程编号参数错误...");
                return s;
            }
        }
        public string FromNode
        {
            get
            {
                return this.Request.QueryString["FromNode"];
            }
        }
        /// <summary>
        /// 当前的工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                if (ViewState["WorkID"] == null)
                {
                    if (this.Request.QueryString["WorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                else
                    return Int64.Parse(ViewState["WorkID"].ToString());
            }
            set
            {
                ViewState["WorkID"] = value;
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public int FK_Node
        {
            get
            {
                string fk_nodeReq = this.Request.QueryString["FK_Node"];
                if (string.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.Request.QueryString["NodeID"];

                if (string.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.Request.QueryString["WorkID"] != null)
                    {
                        string sql = "SELECT FK_Node from  WF_GenerWorkFlow where WorkID=" + this.WorkID;

                        _FK_Node = DBAccess.RunSQLReturnValInt(sql);

                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
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
        public int FromWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FromWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion

        #region 动态控件
        /// <summary>
        /// 节点名称
        /// </summary>
        public Label Lab_NodeName
        {
            get
            {
                return this.ToolBar1.GetLabelByID("Lab_NodeName");
            }
        }
        #endregion

        #region Page load 事件
        public void DoDoType()
        {
            switch (this.DoType)
            {
                case "Runing":
                    ShowRuning();
                    return;
                case "History":
                    ShowHistory();
                    return;
                case "Warting":
                    ShowWarting();
                    return;
                default:
                    break;
            }
        }
        public void ShowWarting()
        {
            this.ToolBar1.AddLab("s", "待办工作");
            string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "'  AND FK_Flow='" + this.FK_Flow + "' ORDER BY WorkID ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return;

            int i = 0;
            bool is1 = false;
            DateTime cdt = DateTime.Now;
            string color = "";
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("标题");
            this.Pub1.AddTDTitle("发起");
            this.Pub1.AddTDTitle("发起日期");
            this.Pub1.AddTDTitle("接受日期");
            this.Pub1.AddTDTitle(  "期限");
            this.Pub1.AddTREnd();

            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string sdt = dr["SDT"] as string;
                DateTime mysdt = DataType.ParseSysDate2DateTime(sdt);
                if (cdt >= mysdt)
                {
                    this.Pub1.AddTRRed(); // ("onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"\" ");
                }
                else
                {
                    is1 = this.Pub1.AddTR(is1); // ("onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"\" ");
                }
                i++;
                this.Pub1.AddTD(i);
                this.Pub1.AddTD(dr["NodeName"].ToString());
                this.Pub1.AddTD("<a href=\"MyFlow" + this.PageSmall + ".aspx?FK_Flow=" + dr["FK_Flow"] + "&WorkID=" + dr["WorkID"] + "&FID=" + dr["FID"] + "\" >" + dr["Title"].ToString());
                this.Pub1.AddTD(dr["Starter"].ToString());
                this.Pub1.AddTD(dr["RDT"].ToString());
                this.Pub1.AddTD(dr["ADT"].ToString());
                this.Pub1.AddTD(dr["SDT"].ToString());
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
        public void ShowRuning()
        {
            this.ToolBar1.AddLab("s",   "在途工作");

            this.Pub1.AddTable("width='80%' align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("nowarp=true", "序");
            this.Pub1.AddTDTitle("nowarp=true", "名称");
            this.Pub1.AddTDTitle("nowarp=true",  "当前节点");
            this.Pub1.AddTDTitle("nowarp=true", "发起日期");
            this.Pub1.AddTDTitle("nowarp=true",  "发起人");
            this.Pub1.AddTDTitle("nowarp=true", "操作");
            this.Pub1.AddTREnd();

            string sql = "  SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B  WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + BP.Web.WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND b.FK_Flow='" + this.FK_Flow + "'";
            //this.Response.Write(sql);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            int i = 0;
            bool is1 = false;
            string FromPageType = Glo.FromPageType;
            foreach (GenerWorkFlow gwf in gwfs)
            {
                i++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(i);
                this.Pub1.AddTDA("MyFlow" + this.PageSmall + ".aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow, gwf.Title);
                this.Pub1.AddTD(gwf.NodeName);
                this.Pub1.AddTD(gwf.RDT);
                this.Pub1.AddTD(gwf.StarterName);

                this.Pub1.AddTDBegin();
                this.Pub1.Add("<a href=\"javascript:Do('您确认吗？','MyFlowInfo" + this.PageSmall + ".aspx?DoType=UnSend&FID=" + gwf.FID + "&WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "');\" ><img src='/WF/Img/Btn/delete.gif' border=0 />" + "撤消" + "</a>");
                this.Pub1.Add("-<a href=\"javascript:WinOpen('./../WF/WFRpt.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=0')\" ><img src='/WF/Img/Btn/rpt.gif' border=0 />报告</a>");
                this.Pub1.Add("-<a href=\"javascript:WinOpen('./../WF/Chart.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=0')\" ><img src='/WF/Img/Track.png' border=0 />轨迹</a>");
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTRSum();
            this.Pub1.AddTD("colspan=6", "&nbsp;");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public void ShowHistory()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            this.ToolBar1.AddLab("s",  "历史工作:" + nd.Name);
            Works wks = nd.HisWorks;
            QueryObject qo = new QueryObject(wks);
            qo.AddWhere(WorkAttr.Rec, WebUser.No);
            qo.addAnd();
            qo.AddWhereInSQL(WorkAttr.OID,
                "SELECT OID FROM  ND" + int.Parse(nd.FK_Flow) + "Rpt WHERE WFState=" + (int)WFState.Complete);
           
            qo.DoQuery();
            this.Pub1.BindWorkDtl(nd, wks);
        }
        #endregion

        #region 变量
        public Flow currFlow = null;
        public Work currWK = null;
        public BP.WF.Node currND = null;
        #endregion

        #region Page load 事件
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.DoType != null)
            {
                DoDoType();
                return;
            }

            string appPath = this.Request.ApplicationPath;
            this.currFlow = new Flow(this.FK_Flow);
            this.currND = new BP.WF.Node(this.FK_Node);

            #region 判断是否有 workid
            if (this.WorkID == 0)
            {
                currWK = this.currFlow.NewWork();
                this.WorkID = currWK.OID;
                //   this.Response.Redirect("MyFlow" + this.PageSmall + ".aspx?WorkID=" + currWK.OID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + currWK.HisNode.NodeID, true);
                // return;
            }
            else
            {
                currWK = this.currFlow.GenerWork(this.WorkID, this.currND);
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                gwf.RetrieveFromDBSources();

                string msg = "";
                switch (gwf.WFState)
                {
                    case WFState.ReturnSta:
                        /* 如果工作节点退回了*/
                        ReturnWorks rws = new ReturnWorks();
                        rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node,
                            ReturnWorkAttr.WorkID, this.WorkID,
                            ReturnWorkAttr.RDT);
                        if (rws.Count != 0)
                        {
                            string msgInfo = "";
                            foreach (ReturnWork rw in rws)
                            {
                                msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                                msgInfo += rw.NoteHtml;
                                //  msgInfo += "<br>";
                                msgInfo += "</fieldset>";
                            }
                            this.FlowMsg.AlertMsg_Info("流程退回提示", msgInfo);
                          //  currWK.Update("WFState", (int)WFState.Runing);
                        }
                        break;
                    case WFState.Forward:
                        /* 如果不是退回来的，就判断是否是移交过来的。 */
                        ForwardWorks fws = new ForwardWorks();
                        BP.En.QueryObject qo = new QueryObject(fws);
                        qo.AddWhere(ForwardWorkAttr.WorkID, this.WorkID);
                        qo.addAnd();
                        qo.AddWhere(ForwardWorkAttr.FK_Node, this.FK_Node);
                        qo.addOrderBy(ForwardWorkAttr.RDT);
                        qo.DoQuery();

                        if (fws.Count >= 1)
                        {
                            this.FlowMsg.AddFieldSet("移交历史信息");
                            foreach (ForwardWork fw in fws)
                            {
                                msg = "@移交人[" + fw.FK_Emp + "," + fw.FK_EmpName + "]。@接受人：" + fw.ToEmp + "," + fw.ToEmpName + "。<br>移交原因@" + fw.NoteHtml;
                                if (fw.FK_Emp == WebUser.No)
                                    msg = "<b>" + msg + "</b>";

                                msg = msg.Replace("@", "<br>@");
                                this.FlowMsg.Add(msg + "<hr>");
                            }
                            this.FlowMsg.AddFieldSetEnd();
                        }
                      //  currWK.Update(StartWorkAttr.WFState, (int)WFState.Runing);
                        break;
                    default:
                        break;
                }
            }
            #endregion 判断是否有workid

            #region 判断权限
            if (this.IsPostBack == false)
            {
                if (currND.IsStartNode == false && BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Node,this.WorkID, WebUser.No) == false)
                {
                    this.ToolBar1.Clear();
                    this.ToolBar1.Add("&nbsp;");

                    this.FlowMsg.Clear();
                    this.FlowMsg.DivInfoBlockBegin(); //("<b>提示</b><hr>@当前的工作已经被处理，或者您没有执行此工作的权限。<br>@您可以执行如下操作。<ul><li><a href='Start.aspx'>发起新流程。</a></li><li><a href='Runing.aspx'>返回在途工作列表。</a></li></ul>");
                    this.FlowMsg.AddB( "提示");
                    this.FlowMsg.AddHR();

                    this.FlowMsg.Add(  "@当前的工作已经被处理，或者您没有执行此工作的权限。<br>@您可以执行如下操作。");

                    this.FlowMsg.AddUL();
                    if (WebUser.IsWap)
                        this.FlowMsg.AddLi("<a href='Home.aspx'><img src='/WF/Img/Home.gif' border=0/>返回主页</a>");
                    this.FlowMsg.AddLi("<a href='Start" + this.PageSmall + ".aspx'><img src='/WF/Img/Start.gif' border=0/>发起流程</a>");
                    this.FlowMsg.AddLi("<a href='Runing" + this.PageSmall + ".aspx'><img src='/WF/Img/Runing.gif' border=0/>在途工作</a>");
                    this.FlowMsg.AddULEnd();
                    this.FlowMsg.DivInfoBlockEnd();
                    return;
                }
            }
            this.LoadPop_del();
            #endregion 判断权限

            try
            {
                string small = this.PageID;
                small = small.Replace("MyFlow", "");

                #region 增加按钮
                this.ToolBar1.Add("<a href='Home.aspx'><img src='/WF/Img/Home.gif' border=0/>Home</a>");
                BtnLab btnLab = new BtnLab(currND.NodeID);
                if (btnLab.SendEnable)
                {
                    this.ToolBar1.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                    this.Btn_Send.UseSubmitBehavior = false;
                    this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                    this.Btn_Send.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.SaveEnable)
                {
                    this.ToolBar1.AddBtn(NamesOfBtn.Save, btnLab.SaveLab);
                    this.Btn_Save.UseSubmitBehavior = false;
                    this.Btn_Save.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                    this.Btn_Save.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.PrintDocEnable)
                {
                    string urlr = "./WorkOpt/PrintDoc.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
                }

                if (btnLab.ReturnEnable && this.currND.IsStartNode == false && this.currND.FocusField == "")
                {
                    /*如果没有焦点字段*/
                    string urlr = "ReturnWork" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.ReturnLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
                }

                if (btnLab.ReturnEnable && this.currND.IsStartNode == false && this.currND.FocusField != "")
                {
                    /*如果有焦点字段*/
                    this.ToolBar1.AddBtn("Btn_ReturnWork", btnLab.ReturnLab);
                    this.Btn_ReturnWork.UseSubmitBehavior = false;
                    this.Btn_ReturnWork.OnClientClick = "this.disabled=true;";
                    this.Btn_ReturnWork.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }


                if (btnLab.ShiftEnable)
                {
                    this.ToolBar1.AddBtn("Btn_Shift", btnLab.ShiftLab);
                    this.Btn_Shift.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.CCRole == CCRole.HandCC)
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Msg/Write.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");

                if (btnLab.DeleteEnable !=0 )
                {
                    this.ToolBar1.AddBtn("Btn_Delete", btnLab.DeleteLab);
                    this.Btn_Delete.OnClientClick = "return confirm('将要执行删除流程，您确认吗？')";
                    this.Btn_Delete.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false)
                {
                    this.ToolBar1.AddBtn("Btn_EndFlow", btnLab.EndFlowLab);
                    this.ToolBar1.GetBtnByID("Btn_EndFlow").OnClientClick = "return confirm('将要执行终止流程，您确认吗？')";
                    this.ToolBar1.GetBtnByID("Btn_EndFlow").Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                //if (btnLab.RptEnable)
                //    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.RptLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WFRpt.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds0'); \" />");

                if (btnLab.TrackEnable)
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Chart.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds'); \" />");

                if (currND.HisFJOpen != FJOpen.None)
                {
                    if (this.WorkID == 0)
                        this.ToolBar1.Add("<input class=Btn type=button value='" + "附件" + "' onclick=\"javascript:alert('请保存后上传附件');\" enable=false  />");
                    else
                        this.ToolBar1.Add("<input class=Btn type=button value='" + "附件" + "' enable=true onclick=\"WinOpen('" + appPath + "WF/FileManager.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FJOpen=" + (int)currND.HisFJOpen + "&FID=" + this.FID + "','dds'); \" />");
                }

                //if (btnLab.OptEnable)
                //    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.OptLab + "' onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Home.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','dds'); \"  />");

                if (btnLab.SearchEnable)
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt&FK_Flow=" + this.FK_Flow + "','dsd0'); \" />");
                #endregion

                this.BindWork(currND, currWK);
                this.Session["Ect"] = null;

                if (btnLab.SelectAccepterEnable == 1)
                    this.ToolBar1.Add("<input class=Btn type=button value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','dds'); \" />");
            }
            catch (Exception ex)
            {
                #region 解决开始节点数据库字段变化修复数据库问题 。
                string rowUrl = this.Request.RawUrl;
                if (rowUrl.IndexOf("rowUrl") > 1)
                {
                }
                else
                {
                    this.Response.Redirect(rowUrl + "&rowUrl=1", true);
                    return;
                }
                #endregion

                this.FlowMsg.DivInfoBlock(ex.Message);
                string Ect = this.Session["Ect"] as string;
                if (Ect == null)
                    Ect = "0";
                if (int.Parse(Ect) < 2)
                {
                    this.Session["Ect"] = int.Parse(Ect) + 1;
                    return;
                }
                return;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// BindWork
        /// </summary>
        public void BindWork(BP.WF.Node nd, Work wk)
        {
            switch (nd.HisNodeWorkType)
            {
                case NodeWorkType.StartWorkFL:
                case NodeWorkType.WorkFHL:
                case NodeWorkType.WorkFL:
                case NodeWorkType.WorkHL:
                    if (this.FID != 0 && this.FID != this.WorkID)
                    {
                        /* 这种情况是分流节点向退回到了分河流。*/
                        this.UCEn1.AddFieldSet("分流节点退回信息");

                        ReturnWork rw = new ReturnWork();
                        rw.Retrieve(ReturnWorkAttr.WorkID, this.WorkID, ReturnWorkAttr.ReturnToNode, nd.NodeID);
                        this.UCEn1.Add(rw.NoteHtml);
                        this.UCEn1.AddHR();
                        //this.UCEn1.addb
                        TextBox tb = new TextBox();
                        tb.ID = "TB_Doc";
                        tb.TextMode = TextBoxMode.MultiLine;
                        tb.Rows = 7;
                        tb.Columns = 50;
                        this.UCEn1.Add(tb);

                        this.UCEn1.AddBR();
                        Btn btn = new Btn();
                        btn.ID = "Btn_Reject";
                        btn.Text = "驳回工作";
                        btn.Click += new EventHandler(ToolBar1_ButtonClick);
                        this.UCEn1.Add(btn);

                        btn = new Btn();
                        btn.ID = "Btn_KillSubFlow";
                        btn.Text = "终止工作";
                        btn.Click += new EventHandler(ToolBar1_ButtonClick);
                        this.UCEn1.Add(btn);
                        this.UCEn1.AddFieldSetEnd(); // ("分流节点退回信息");

                        //this.ToolBar1.Controls.Clear();//.Clear();
                        //this.Response.Write("<script language='JavaScript'> DoSubFlowReturn('" + this.FID + "','" + wk.OID + "','" + nd.NodeID + "');</script>");
                        //this.Response.Write("<javascript ></javascript>");
                        return;
                    }
                    break;
                default:
                    break;
            }
            if (nd.IsStartNode)
            {
                /*判断是否来与子流程.*/
                if (string.IsNullOrEmpty(this.Request.QueryString["FromNode"]) == false)
                {
                    if (this.FromWorkID == 0)
                        throw new Exception("流程设计错误，调起子流程时，没有接受到FromWorkID参数。");

                    ///* 如果来自于主流程 */
                    //int FromNode = int.Parse(this.Request.QueryString["FromNode"]);
                    //BP.WF.Node FromNode_nd = new BP.WF.Node(FromNode);
                    //Work fromWk = FromNode_nd.HisWork;
                    //fromWk.OID = this.FromWorkID;
                    //fromWk.RetrieveFromDBSources();
                    //wk.Copy(fromWk);
                    //   wk.FID = this.FID;
                }
            }

            // 处理传递过来的参数。
            foreach (string k in this.Request.QueryString.AllKeys)
            {
                wk.SetValByKey(k, this.Request.QueryString[k]);
            }

            #region 设置默认值
            MapAttrs mattrs = currND.MapData.MapAttrs;
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIIsEnable)
                    continue;

                if (attr.DefValReal.Contains("@") == false)
                    continue;

                wk.SetValByKey(attr.KeyOfEn, attr.DefVal);
            }
            #endregion 设置默认值。

            #region 判断是否合流节点。。
            if (nd.HisNodeWorkType == NodeWorkType.WorkHL || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
            {
                GenerWorkerLists wls = new GenerWorkerLists();
                QueryObject qo = new QueryObject(wls);
                qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsPass, 1);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.FK_Node,
                    nd.FromNodes[0].GetValByKey(NodeAttr.NodeID));

                int i = qo.DoQuery();
                if (i == 1)
                {
                    qo.clear();
                    qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
                    qo.addAnd();
                    qo.AddWhere(GenerWorkerListAttr.IsPass, 1);
                    qo.DoQuery();
                }

                if (1 == 2)
                {
                    this.Pub2.AddFieldSet("分流信息");
                    this.Pub2.AddTable("border=0"); // ("<table border=0 >");
                    this.Pub2.AddTR();
                    this.Pub2.AddTDTitle("节点");
                    this.Pub2.AddTDTitle("处理人");
                    this.Pub2.AddTDTitle("名称");

                    this.Pub2.AddTDTitle("部门");
                    this.Pub2.AddTDTitle("状态");
                    this.Pub2.AddTDTitle("应完成日期");
                    this.Pub2.AddTDTitle("实际完成日期");
                    this.Pub2.AddTDTitle("");
                    this.Pub2.AddTREnd();

                    bool isHaveRuing = false;
                    bool is1 = false;
                    foreach (GenerWorkerList wl in wls)
                    {
                        is1 = this.Pub2.AddTR(is1);
                        this.Pub2.AddTD(wl.FK_NodeText);
                        this.Pub2.AddTD(wl.FK_Emp);

                        this.Pub2.AddTD(wl.FK_EmpText);
                        this.Pub2.AddTD(wl.FK_DeptT);

                        if (wl.IsPass)
                        {
                            this.Pub2.AddTD("已完成");
                            this.Pub2.AddTD(wl.SDT);
                            this.Pub2.AddTD(wl.RDT);
                        }
                        else
                        {
                            this.Pub2.AddTD("未完成-<a href=\"javascript:WinOpen('');\"><img src='/WF/Img/sms.gif' border=0/>催办</a>");
                            this.Pub2.AddTD(wl.SDT);
                            this.Pub2.AddTD();
                        }

                        if (wl.IsPass == false)
                        {
                            isHaveRuing = true;
                            this.Pub2.AddTD("<a href=\"javascript:DoDelSubFlow('" + wl.FK_Flow + "','" + wl.WorkID + "')\"><img src='/WF/Img/Btn/Delete.gif' border=0/>终止</a>");
                        }
                        else
                        {
                            this.Pub2.AddTD("<a href=\"javascript:WinOpen('FHLFlow.aspx?WorkID=" + wl.WorkID + "&FID=" + wl.FID + "&FK_Flow=" + nd.FK_Flow + "&FK_Node=" + this.FK_Node + "')\">打开</a>");
                        }
                        this.Pub2.AddTREnd();
                    }
                    if (isHaveRuing)
                    {
                        if (nd.IsForceKill == false)
                            this.Btn_Send.Enabled = false;
                    }
                    this.Pub2.AddTableEnd();
                    this.Pub2.AddFieldSetEnd(); //.AddFieldSet("分流信息");
                }
            }
            #endregion 判断是否合流节点。

            switch (nd.HisFormType)
            {
                case FormType.FreeForm:
                case FormType.DisableIt:
                case FormType.FixForm:
                    this.UCEn1.BindColumn2(wk, "ND" + nd.NodeID); //, false, false, null);
                    this.OutJSAuto(wk);
                    return;
                case FormType.SelfForm:
                case FormType.SDKForm:
                    wk.Save();
                    if (this.WorkID == 0)
                    {
                        this.WorkID = wk.OID;
                    }
                    string url = nd.FormUrl;
                    string urlExt = this.RequestParas;
                    if (urlExt.Contains("WorkID") == false)
                        urlExt += "&WorkID=" + this.WorkID;

                    if (urlExt.Contains("NodeID") == false)
                        urlExt += "&NodeID=" + nd.NodeID;

                    if (urlExt.Contains("FK_Node") == false)
                        urlExt += "&FK_Node=" + nd.NodeID;

                    if (urlExt.Contains("FID") == false)
                        urlExt += "&FID=" + wk.FID;

                    urlExt += "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;

                    if (url.Contains("?") == true)
                        url += "&" + urlExt;
                    else
                        url += "?" + urlExt;

                    if (nd.HisFormType == FormType.SDKForm)
                    {
                        this.Response.Redirect(url, true);
                    }
                    else
                    {
                        //  this.UCEn1.AddIframeExt(url, nd.FrmAttr);
                        this.UCEn1.AddIframeAutoSize(url, "D" + this.FK_Flow, "WorkPlace");
                        this.UCEn1.Add(wk.WorkEndInfo);
                    }
                    return;
                default:
                    throw new Exception("@没有涉及到的扩充。");
            }
        }
        public void OutJSAuto(Entity en)
        {
            if (en.EnMap.IsHaveJS == false)
                return;

            Attrs attrs = en.EnMap.Attrs;
            string js = "";
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                if (attr.IsNum == false)
                    continue;

                string tbID = "TB_" + attr.Key;
                TB tb = this.UCEn1.GetTBByID(tbID);
                if (tb == null)
                    continue;

                tb.Attributes["OnKeyPress"] = "javascript:C();";
                tb.Attributes["onkeyup"] = "javascript:C();";

                if (attr.MyDataType == DataType.AppInt)
                    tb.Attributes["OnKeyDown"] = "javascript:return VirtyInt(this);";
                else
                    tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";

                //   tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";

                if (attr.MyDataType == DataType.AppMoney)
                    tb.Attributes["onblur"] = "this.value=VirtyMoney(this.value);";

                if (attr.AutoFullWay == AutoFullWay.Way1_JS)
                {
                    js += attr.Key + "," + attr.AutoFullDoc + "~";
                    tb.Enabled = true;
                }
            }

            string[] strs = js.Split('~');

            ArrayList al = new ArrayList();
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                string key = str.Substring(0, str.IndexOf(','));
                string exp = str.Substring(str.IndexOf(',') + 1);

                string left = "\n  document.forms[0].UCEn1_TB_" + key + ".value = ";
                foreach (Attr attr in attrs)
                {
                    exp = exp.Replace("@" + attr.Key, "  parseFloat( document.forms[0].UCEn1_TB_" + attr.Key + ".value.replace( ',' ,  '' ) ) ");
                    exp = exp.Replace("@" + attr.Desc, " parseFloat( document.forms[0].UCEn1_TB_" + attr.Key + ".value.replace( ',' ,  '' ) ) ");
                }
                al.Add(left + exp);
            }

            string body = "";
            foreach (string s in al)
            {
                body += s;
            }
            this.Response.Write("<script language='JavaScript'> function  C(){ " + body + " }  \n </script>");
        }
        /// <summary>
        /// 显示 关联表单
        /// </summary>
        /// <param name="nd"></param>
        public void ShowSheets(BP.WF.Node nd, Work currWk)
        {
            if (nd.HisFJOpen != FJOpen.None)
            {
                string url = "FileManager.aspx?WorkID=" + this.WorkID + "&FID=" + currWk.FID
                    + "&FJOpen=" + (int)nd.HisFJOpen + "&FK_Node=" + nd.NodeID;
                this.Pub1.Add("<iframe leftMargin='0' topMargin='0' src='" + url + "' width='100%' height='200px' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
            }

            //this.Pub1.AddIframe("FileManager.aspx?WorkID="+this.WorkID+"&FID=0&FJOpen=1&FK_Node="+nd.NodeID);

            if (this.Pub1.Controls.Count > 20)
                return;

            // 显示设置的步骤.
            string[] strs = nd.ShowSheets.Split('@');

            if (strs.Length >= 1)
                this.Pub1.AddHR();

            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                int FK_Node = int.Parse(str);
                BP.WF.Node mynd;
                try
                {
                    mynd = new BP.WF.Node(FK_Node);
                }
                catch
                {
                    nd.ShowSheets = nd.ShowSheets.Replace("@" + FK_Node, "");
                    nd.Update();
                    continue;
                }

                Work nwk = mynd.HisWork;
                nwk.OID = this.WorkID;


                if (nwk.RetrieveFromDBSources() == 0)
                    continue;

                // this.Pub1.AddB("== " + mynd.Name + " ==<hr width=90%>");

                this.Pub1.AddFieldSet("历史步骤:" + mynd.Name);
                // this.Pub1.DivInfoBlockBegin();
                this.Pub1.ADDWork(nwk, new ReturnWorks(), new ForwardWorks(), nd.NodeID);

                this.Pub1.AddFieldSetEnd(); // (mynd.Name);

                //this.Pub1.DivInfoBlockEnd(); // (mynd.Name);
            }
        }
        #endregion

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        #region toolbar 2
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            //try
            //{
            Btn btn = (Btn)sender;
            switch (btn.ID)
            {

                case "Btn_Reject":
                case "Btn_KillSubFlow":
                    try
                    {
                        WorkFlow wkf = new WorkFlow(this.FK_Flow, this.WorkID);
                        if (btn.ID == "Btn_KillSubFlow")
                        {
                            this.ToMsg("删除流程信息:<hr>" + wkf.DoDeleteWorkFlowByReal(true), "info");
                        }
                        else
                        {
                            string msg = wkf.DoReject(this.FID, this.FK_Node, this.UCEn1.GetTextBoxByID("TB_Doc").Text);
                            this.ToMsg(msg, "info");
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        this.ToMsg(ex.Message, "info");
                        return;
                    }
                case "Btn_EndFlow": //结束流程。
                    WorkFlow mywf = null;
                    if (this.FID == 0)
                        mywf = new WorkFlow(this.FK_Flow, this.WorkID);
                    else
                        mywf = new WorkFlow(this.FK_Flow, this.FID);

                    this.ToMsg("结束流程提示:<hr>" + mywf.DoFlowOver(ActionType.FlowOverByCoercion, "强制删除"), "info");
                    break;
                case NamesOfBtn.Delete:
                case "Btn_Del":
                    WorkFlow wf = null;
                    if (this.FID == 0)
                        wf = new WorkFlow(this.FK_Flow, this.WorkID);
                    else
                        wf = new WorkFlow(this.FK_Flow, this.FID);

                    this.ToMsg("删除流程提示<hr>" + wf.DoDeleteWorkFlowByReal(true), "info");
                    // this.ToMsgPage("操作提示<hr>您已经成功删除了该流程。");
                    break;
                case NamesOfBtn.Save:
                    this.Send(true);
                    if (string.IsNullOrEmpty(this.Request.QueryString["WorkID"]))
                    {
                        this.Response.Redirect(this.PageID + ".aspx?FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FromNode=" + this.FromNode + "&FromWorkID=" + this.FromWorkID, true);
                        return;
                    }
                    break;
                case "Btn_ReturnWork":
                    this.BtnReturnWork();
                    break;
                case BP.Web.Controls.NamesOfBtn.Shift:
                    this.DoShift();
                    break;
                case "Btn_WorkerList":
                    if (WorkID == 0)
                        throw new Exception("没有指定当前的工作,不能查看工作者列表.");
                    break;
                case "Btn_PrintWorkRpt":
                    if (WorkID == 0)
                        throw new Exception("没有指定当前的工作,不能打印工作报告.");
                    this.WinOpen("WFRpt.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + WorkID,  "工作报告", 800, 600);
                    //this.BtnEdit();
                    break;
                case NamesOfBtn.Send:
                    this.Send(false);
                    break;
                default:
                    break;
            }
            //}
            //catch (Exception ex)
            //{
            //    this.FlowMsg.AlertMsg_Warning("信息提示", ex.Message);
            //}
        }
        #region 按钮事件
        /// <summary>
        /// 保存工作
        /// </summary>
        /// <param name="isDraft">是不是做为草稿保存</param> 
        private void Send(bool isSave)
        {
            // 判断当前人员是否有执行该人员的权限。
            string sql = "SELECT FK_Emp FROM WF_GenerWorkerlist WHERE FK_Node='" + this.FK_Node + "' AND WorkID=" + this.WorkID + " AND FK_Emp='" + WebUser.No + "' AND IsEnable=1 AND IsPass=0";

            if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 1 && currND.IsStartNode == false)
            {
                this.ToMsg("保存或发送错误", "您好：" + WebUser.No + "," + WebUser.Name + "：<br> 当前工作已经被其它人处理，您不能在执行保存或者发送!!!");
                return;
            }

            System.Web.HttpContext.Current.Session["RunDT"] = DateTime.Now;
            if (this.FK_Node == 0)
                throw new Exception( "没有找到当前的节点");

            try
            {
                switch (currND.HisFormType)
                {
                    case FormType.SelfForm:
                        break;
                    case FormType.FixForm:
                    case FormType.FreeForm:
                        currWK = (Work)this.UCEn1.Copy(this.currWK);
                        // 设置默认值
                        MapAttrs mattrs = currND.MapData.MapAttrs;
                        //MapAttrs mattrs = new MapAttrs("ND" + this.FK_Node);
                        foreach (MapAttr attr in mattrs)
                        {
                            if (attr.UIIsEnable)
                                continue;

                            if (attr.DefValReal.Contains("@") == false)
                                continue;

                            currWK.SetValByKey(attr.KeyOfEn, attr.DefVal);
                        }
                        break;
                    case FormType.DisableIt:
                        break;
                    default:
                        throw new Exception("@未涉及到的情况。");
                }
            }
            catch (Exception ex)
            {
                this.Btn_Send.Enabled = true;
                throw new Exception("@在保存前执行逻辑检查错误。" + ex.Message);
            }

            #region 判断特殊的业务逻辑
            if (currND.IsStartNode)
            {
                if (this.currND.HisFlow.HisFlowAppType == FlowAppType.PRJ)
                {
                    /*对特殊的流程进行检查，检查是否有权限。*/
                    string prjNo = currWK.GetValStringByKey("PrjNo");
                    if (DBAccess.RunSQLReturnTable("SELECT * FROM WF_NodeStation WHERE FK_Station IN ( SELECT FK_Station FROM Prj_EmpPrjStation WHERE FK_Prj='" + prjNo + "' AND FK_Emp='" + WebUser.No + "' )  AND  FK_Node=" + this.FK_Node).Rows.Count == 0)
                    {
                        string prjName = currWK.GetValStringByKey("PrjName");
                        if (DBAccess.RunSQLReturnTable("SELECT * FROM Prj_EmpPrj WHERE FK_Prj='" + prjNo + "' AND FK_Emp='" + WebUser.No + "'").Rows.Count == 0)
                            throw new Exception("您不是(" + prjNo + "," + prjName + ")成员，您不能发起改流程。");
                        else
                            throw new Exception("您属于这个项目(" + prjNo + "," + prjName + ")，但是在此项目下您没有发起改流程的岗位。");
                    }
                }
            }
            #endregion 判断特殊的业务逻辑。

            currWK.Rec = WebUser.No;
            currWK.SetValByKey("FK_Dept", WebUser.FK_Dept);
            currWK.SetValByKey("FK_NY", BP.DA.DataType.CurrentYearMonth);
            try
            {
                if (currND.IsStartNode)
                    currWK.FID = 0;

                currWK.Update(); /* 如果是保存 */
            }
            catch (Exception ex)
            {
                try
                {
                    currWK.CheckPhysicsTable();
                }
                catch (Exception ex1)
                {
                    throw new Exception("@保存错误:" + ex.Message + "@检查物理表错误：" + ex1.Message);
                }
                this.Btn_Send.Enabled = true;
                this.Pub1.AlertMsg_Warning("错误", ex.Message + "@有可能此错误被系统自动修复,请您从新保存一次.");
                return;
            }
            string msg = "";
            // 调用工作流程，处理节点信息采集后保存后的工作。
            if (isSave)
            {
                if (string.IsNullOrEmpty(this.Request.QueryString["WorkID"]))
                    return;

                currWK.RetrieveFromDBSources();
                this.UCEn1.ResetEnVal(currWK);
                return;
            }

            WorkNode firstwn = new WorkNode(this.currWK, this.currND);
            try
            {
                msg = firstwn.NodeSend().ToMsgOfHtml();
            }
            catch (Exception exSend)
            {
                this.FlowMsg.AddFieldSetGreen("错误");
                this.FlowMsg.Add(exSend.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                this.FlowMsg.AddFieldSetEnd();
                return;
            }

            this.Btn_Send.Enabled = false;
            /*处理转向问题.*/
            switch (firstwn.HisNode.HisTurnToDeal)
            {
                case TurnToDeal.SpecUrl:
                    string myurl = firstwn.HisNode.TurnToDealDoc.Clone().ToString();
                    if (myurl.Contains("&") == false)
                        myurl += "?1=1";
                    Attrs myattrs = firstwn.HisWork.EnMap.Attrs;
                    Work hisWK = firstwn.HisWork;
                    foreach (Attr attr in myattrs)
                    {
                        if (myurl.Contains("@") == false)
                            break;
                        myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                    }
                    myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&FromWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                    this.Response.Redirect(myurl, true);
                    return;
                case TurnToDeal.TurnToByCond:
                    TurnTos tts = new TurnTos(this.FK_Flow);
                    if (tts.Count == 0)
                        throw new Exception("@您没有设置节点完成后的转向条件。");
                    foreach (TurnTo tt in tts)
                    {
                        tt.HisWork = firstwn.HisWork;
                        if (tt.IsPassed == true)
                        {
                            string url = tt.TurnToURL.Clone().ToString();
                            if (url.Contains("&") == false)
                                url += "?1=1";
                            Attrs attrs = firstwn.HisWork.EnMap.Attrs;
                            Work hisWK1 = firstwn.HisWork;
                            foreach (Attr attr in attrs)
                            {
                                if (url.Contains("@") == false)
                                    break;
                                url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                            }
                            url += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&FromWorkID" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }
                    throw new Exception("您定义的转向条件不成立，没有出口。");
                default:
                    this.ToMsg(msg, "info");
                    break;
            }
            return;
        }
        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;

            Glo.SessionMsg = msg;
            this.Response.Redirect("MyFlowInfo" + Glo.FromPageType + ".aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);

            //if (this.PageID.Contains("Single") == true)
            //    this.Response.Redirect("MyFlowInfoSmallSingle.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else if (this.PageID.Contains("Small"))
            //    this.Response.Redirect("MyFlowInfoSmall.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else
            //    this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
        public void BtnReturnWork()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            try
            {
                string msg = this.UCEn1.GetTextBoxByID("TB_" + nd.FocusField).Text;
                wk.Update(nd.FocusField, msg);

                string small = this.PageID;
                small = small.Replace("MyFlow", "");
                this.Response.Redirect("ReturnWork" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + wk.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
            return;
        }
        public void DoShift()
        {


            GenerWorkFlow gwf = new GenerWorkFlow();
            if (gwf.Retrieve(GenerWorkFlowAttr.WorkID, this.WorkID) == 0)
            {
                this.Alert("工作还没有发出，您不能移交。");
                return;
            }

            BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
            if (nd.FocusField != "")
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.Retrieve();
                string msg = this.UCEn1.GetTextBoxByID("TB_" + nd.FocusField).Text;
                wk.Update(nd.FocusField, msg);
            }

            string url = "Forward" + Glo.FromPageType + ".aspx?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
            this.Response.Redirect(url, true);
        }
        #endregion

        #endregion

        #region 处理工作.
        /// <summary>
        /// 新建一个工作
        /// </summary>

        #endregion
    }
}