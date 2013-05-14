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
using CCFlow.WF.Comm.UC;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.UC
{
    public partial class MyFlow : BP.Web.UC.UCBase3
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

        #region Page load 事件
        public void DoDoType()
        {
            switch (this.DoType)
            {
                case "Runing":
                    ShowRuning();
                    return;
                case "Warting":
                    ShowWarting();
                    return;
                case "StopFlow":
                    StopFlow();
                    return;
                default:
                    break;
            }
        }
        public void StopFlow()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            this.UCEn1.AddTable("width='70%'");
            this.UCEn1.AddCaptionLeft("<a href='MyFlow" + this.PageSmall + ".aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "'><img src='" + this.Request.ApplicationPath + "WF/Img/Btn/Back.gif' border=0>返回</a> --- 结束[" + gwf.Title + "]流程");
            this.UCEn1.AddTR();
            this.UCEn1.AddTDTitle("请填写中止流程的原因");
            this.UCEn1.AddTREnd();

            TextBox tb = new TextBox();
            tb.Text = "";
            tb.ID = "TB_Doc";
            tb.Columns = 50;
            tb.Rows = 10;
            tb.TextMode = TextBoxMode.MultiLine;
            this.UCEn1.AddTR();
            this.UCEn1.AddTD(tb);
            this.UCEn1.AddTREnd();
            this.UCEn1.AddTR();
            Btn btn = new Btn();
            btn.ID = "Btn_OK";
            btn.Text = "  OK  ";
            // btn.OnClientClick = "return window.confim('您确定要执行吗？')";
            btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
            btn.Click += new EventHandler(ToolBar1_ButtonClick);
            this.UCEn1.AddTD(btn);
            this.UCEn1.AddTREnd();
            this.UCEn1.AddTableEnd();
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
            this.Pub1.AddTDTitle("期限");
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
            this.ToolBar1.AddLab("s", "在途工作");

            this.Pub1.AddTable("width='80%' align=left");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("nowarp=true", "序");
            this.Pub1.AddTDTitle("nowarp=true", "名称");
            this.Pub1.AddTDTitle("nowarp=true", "当前节点");
            this.Pub1.AddTDTitle("nowarp=true", "发起日期");
            this.Pub1.AddTDTitle("nowarp=true", "发起人");
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
                this.Pub1.Add("<a href=\"javascript:Do('您确认吗？','MyFlowInfo" + this.PageSmall + ".aspx?DoType=UnSend&FID=" + gwf.FID + "&WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "');\" ><img src='/WF/Img/Btn/delete.gif' border=0 />撤消</a>");
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

        #endregion

        #region 变量
        public Flow currFlow = null;
        public Work currWK = null;
        public BP.WF.Node currND = null;
        #endregion

        #region Page load 事件
        /// <summary>
        /// ss
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            #region 校验用户是否被禁用。
            if (Glo.IsEnableCheckUseSta == true)
            {
                if (Glo.CheckIsEnableWFEmp() == false)
                {
                    this.UCEn1.AddFieldSetGreen("错误");
                    this.UCEn1.AddB("<font color=red>您的帐号已经被禁用，如果有问题请与管理员联系。</font>");
                    this.UCEn1.AddFieldSetEnd();
                    BP.Web.WebUser.Exit();
                    return;
                }
            }

            if (this.DoType != null)
            {
                DoDoType();
                return;
            }
            #endregion 校验用户是否被禁用

            #region 判断是否有IsRead
            if (this.Request.QueryString["IsRead"] != null)
                BP.WF.Dev2Interface.Node_SetWorkRead(this.FK_Node, this.WorkID);
            #endregion

            #region 设置变量
            string appPath = this.Request.ApplicationPath;
            this.currFlow = new Flow(this.FK_Flow);
            this.currND = new BP.WF.Node(this.FK_Node);
            this.Page.Title = "第" + this.currND.Step + "步:" + this.currND.Name;
            #endregion 设置变量

            if (this.currND.HisFormType == FormType.SLForm)
            {
                if (this.WorkID == 0)
                {
                    WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                    currWK = currND.HisWork;
                    currWK.OID = this.WorkID;
                    currWK.Retrieve();
                    //this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }
                this.Response.Redirect("MyFlowSL.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No, true);
                return;
            }
            if (this.currND.HisFormType == FormType.SDKForm)
            {
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = currND.FormUrl;
                string urlExt = this.RequestParas;
                if (urlExt.Contains("WorkID") == false)
                    urlExt += "&WorkID=" + this.WorkID;

                if (urlExt.Contains("NodeID") == false)
                    urlExt += "&NodeID=" + currND.NodeID;

                if (urlExt.Contains("FK_Node") == false)
                    urlExt += "&FK_Node=" + currND.NodeID;

                if (urlExt.Contains("FID") == false)
                    urlExt += "&FID=" + currWK.FID;

                if (urlExt.Contains("UserNo") == false)
                    urlExt += "&UserNo=" + WebUser.No;

                if (urlExt.Contains("SID") == false)
                    urlExt += "&SID=" + WebUser.SID;

                if (url.Contains("?") == true)
                    url += "&" + urlExt;
                else
                    url += "?" + urlExt;

                url = url.Replace("?&", "?");
                this.Response.Redirect(url, true);
                return;
            }

            #region 判断是否有 workid
            if (this.WorkID == 0)
            {
                currWK = this.currFlow.NewWork();
                this.WorkID = currWK.OID;
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
                            foreach (BP.WF.ReturnWork rw in rws)
                            {
                                msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='/DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                                msgInfo += rw.NoteHtml;
                                //  msgInfo += "<br>";
                                msgInfo += "</fieldset>";
                            }
                            this.FlowMsg.AlertMsg_Info("流程退回提示", msgInfo);
                            gwf.WFState = WFState.Runing;
                            gwf.DirectUpdate();
                        }
                        break;
                    case WFState.Forward:
                        /* 判断移交过来的。 */
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
                        gwf.WFState = WFState.Runing;
                        gwf.DirectUpdate();
                        break;
                    default:
                        break;
                }
            }
            #endregion 判断是否有workid

            #region 判断权限
            if (this.IsPostBack == false)
            {
                if (currND.IsStartNode == false && Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Node, this.WorkID, WebUser.No) == false)
                {
                    this.ToolBar1.Clear();
                    this.ToolBar1.Add("&nbsp;");

                    this.FlowMsg.Clear();
                    this.FlowMsg.DivInfoBlockBegin(); //("<b>提示</b><hr>@当前的工作已经被处理，或者您没有执行此工作的权限。<br>@您可以执行如下操作。<ul><li><a href='Start.aspx'>发起新流程。</a></li><li><a href='Runing.aspx'>返回在途工作列表。</a></li></ul>");
                    this.FlowMsg.AddB("提示");
                    this.FlowMsg.AddHR();

                    this.FlowMsg.Add("@当前的工作已经被处理，或者您没有执行此工作的权限。");

                    this.FlowMsg.AddUL();
                    if (WebUser.IsWap)
                    {
                        this.FlowMsg.AddLi("<a href='Home.aspx'><img src='/WF/Img/Home.gif' border=0/>Home</a>");
                        this.FlowMsg.AddLi("<a href='Start" + this.PageSmall + ".aspx'><img src='/WF/Img/Start.gif' border=0/>发起流程</a>");
                        this.FlowMsg.AddLi("<a href='Runing" + this.PageSmall + ".aspx'><img src='/WF/Img/Runing.gif' border=0/>在途工作</a>");
                    }
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
                if (small != "")
                    this.ToolBar1.AddBR();

                #region 增加按钮
                BtnLab btnLab = new BtnLab(currND.NodeID);
                if (currND.IsEndNode)
                {
                    /*如果当前节点是结束节点.*/
                    if (btnLab.SendEnable)
                    {
                        /*如果启用了发送按钮.*/
                        this.ToolBar1.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                        this.Btn_Send.UseSubmitBehavior = false;
                        this.Btn_Send.OnClientClick = "this.disabled=true;SaveDtlAll();";

                        this.Btn_Send.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                    }
                }
                else
                {
                    if (btnLab.SendEnable)
                    {
                        /*如果启用了发送按钮.*/
                        if (btnLab.SelectAccepterEnable == 2)
                        {
                            /*如果启用了选择人窗口的模式是【选择既发送】.*/
                            this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.SendLab + "' enable=true onclick=\"KindEditerSync();window.open('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&type=1','选择收件人', 'height=500, width=400,scrollbars=yes'); \" />");
                            this.ToolBar1.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            Btn_Send.Style.Add("display", "none");
                            this.Btn_Send.UseSubmitBehavior = false;

                            if (this.currND.HisFormType == FormType.DisableIt)
                                this.Btn_Send.OnClientClick =  btnLab.SendJS + "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            else
                                this.Btn_Send.OnClientClick = btnLab.SendJS + "this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            //   this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            this.Btn_Send.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                        }
                        else
                        {
                            this.ToolBar1.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            this.Btn_Send.UseSubmitBehavior = false;
                            if (btnLab.SendJS.Trim().Length > 2)
                            {
                                this.Btn_Send.OnClientClick = btnLab.SendJS + ";this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            }
                            else
                            {
                                this.Btn_Send.UseSubmitBehavior = false;
                                if (this.currND.HisFormType == FormType.DisableIt)
                                    this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                                else
                                    this.Btn_Send.OnClientClick = "this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            }
                            this.Btn_Send.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                        }
                    }
                }

                if (btnLab.SaveEnable)
                {
                    this.ToolBar1.AddBtn(NamesOfBtn.Save, btnLab.SaveLab);
                    this.Btn_Save.UseSubmitBehavior = false;
                    this.Btn_Save.OnClientClick = "this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                    //  this.Btn_Save.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                    this.Btn_Save.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    string ur2 = appPath + "WF/HeLiuDtl.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "','dsdd'); \" />");
                }

                if (btnLab.PrintDocEnable)
                {
                    string urlr = appPath + "WF/WorkOpt/PrintDoc.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
                }


                if (btnLab.JumpWayEnable)
                {
                    /*如果没有焦点字段*/
                    string urlr = appPath + "WF/JumpWay" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
                }

                if (btnLab.ReturnEnable && this.currND.IsStartNode == false && this.currND.FocusField == "")
                {
                    /*如果没有焦点字段*/
                    string urlr = appPath + "WF/ReturnWork" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.ReturnLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
                }
                if (btnLab.ReturnEnable && this.currND.IsStartNode == false && this.currND.FocusField != "")
                {
                    /*如果有焦点字段*/
                    this.ToolBar1.AddBtn("Btn_ReturnWork", btnLab.ReturnLab);
                    this.Btn_ReturnWork.UseSubmitBehavior = false;
                    this.Btn_ReturnWork.OnClientClick = "this.disabled=true;";
                    this.Btn_ReturnWork.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
                if (btnLab.HungEnable)
                {
                    /*挂起*/
                    string urlr = appPath + "WF/WorkOpt/HungUp.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "','fd'); \" />");
                    //this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
                }

                if (btnLab.ShiftEnable && this.currND.IsStartNode == false)
                {
                    /*移交*/
                    this.ToolBar1.AddBtn("Btn_Shift", btnLab.ShiftLab);
                    this.Btn_Shift.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                if (btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto)
                {
                    /* 抄送 */
                    // this.ToolBar1.Add("<input type=button value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Msg/Write.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CC.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds'); \" />");
                }

                if (btnLab.DeleteEnable != 0)
                {
                    /*流程删除规则 */
                    switch (this.currND.HisDelWorkFlowRole)
                    {
                        case DelWorkFlowRole.None: /*不删除*/
                            break;
                        case DelWorkFlowRole.ByUser: //需要交互.
                        case DelWorkFlowRole.DeleteAndWriteToLog:
                        case DelWorkFlowRole.DeleteByFlag:
                            string urlrDel = appPath + "WF/DeleteWorkFlow" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;
                            this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />");
                            break;
                        case DelWorkFlowRole.DeleteReal: // 不需要交互，直接干净的删除.
                            this.ToolBar1.AddBtn("Btn_Delete", btnLab.DeleteLab);
                            this.Btn_Delete.OnClientClick = "return confirm('将要执行删除流程，您确认吗?')";
                            this.Btn_Delete.Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                            break;
                        default:
                            break;
                    }
                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false)
                {
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.EndFlowLab + "' enable=true onclick=\"To('MyFlow" + small + ".aspx?&DoType=StopFlow&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "'); \" />");
                    //this.ToolBar1.AddBtn("Btn_EndFlow", btnLab.EndFlowLab);
                    //this.ToolBar1.GetBtnByID("Btn_EndFlow").OnClientClick = "return confirm('" + this.ToE("AYS", "将要执行终止流程，您确认吗？") + "')";
                    //this.ToolBar1.GetBtnByID("Btn_EndFlow").Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                }

                //if (btnLab.RptEnable)
                //    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.RptLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WFRpt.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds0'); \" />");

                if (btnLab.TrackEnable)
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Chart.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");

                if (currND.HisFJOpen != FJOpen.None)
                {
                    if (this.WorkID == 0)
                        this.ToolBar1.Add("<input type=button class=Btn value='" + "附件" + "' onclick=\"javascript:alert('请保存后上传附件');\" enable=false  />");
                    else
                        this.ToolBar1.Add("<input type=button class=Btn value='" + "附件" + "' enable=true onclick=\"WinOpen('" + appPath + "WF/FileManager.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FJOpen=" + (int)currND.HisFJOpen + "&FID=" + this.FID + "','dds'); \" />");
                }

                //if (btnLab.OptEnable)
                //    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.OptLab + "' onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Home.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','opt'); \"  />");

                switch (btnLab.SelectAccepterEnable)
                {
                    case 1:
                        this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','dds'); \" />");
                        break;
                    case 2:
                        this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','dds'); \" />");
                        break;
                    default:
                        break;
                }

                if (btnLab.SearchEnable)
                    this.ToolBar1.Add("<input type=button class=Btn value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt&FK_Flow=" + this.FK_Flow + "','dsd0'); \" />");

                #endregion

                this.BindWork(currND, currWK);
                this.Session["Ect"] = null;
                //if (btnLab.SelectAccepterEnable == 2) /*在发送前打开.*/
                //    this.ToolBar1.Add("<input type=button value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&type=1','dds'); \" />");

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
            if (nd.HisFlow.IsMD5 && nd.IsStartNode == false && wk.IsPassCheckMD5() == false)
            {
                this.UCEn1.AddMsgOfWarning("错误", "<h2><font color=red>数据已经被非法篡改，请通知管理员解决问题。</font></h2>");
                this.ToolBar1.EnableAllBtn(false);
                this.ToolBar1.Clear();
                return;
            }

            if (this.IsPostBack == true)
                this.UCEn1.IsLoadData = false;
            else
                this.UCEn1.IsLoadData = true;

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

                        BP.WF.ReturnWork rw = new ReturnWork();
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
            MapAttrs mattrs = nd.MapData.MapAttrs;
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIIsEnable)
                    continue;

                if (attr.DefValReal.Contains("@") == false)
                    continue;

                wk.SetValByKey(attr.KeyOfEn, attr.DefVal);
            }
            #endregion 设置默认值。

            if (nd.HisFormType == FormType.DisableIt)
                wk.DirectUpdate();

            switch (nd.HisFormType)
            {
                case FormType.FreeForm:
                case FormType.DisableIt:
                case FormType.FixForm:
                    Frms frms = nd.HisFrms;
                    if (frms.Count == 0)
                    {
                        /* 仅仅只有节点表单的情况。 */
                        if (nd.HisFormType == FormType.FreeForm)
                        {
                            /* 自由表单 */
                            this.UCEn1.Add("<div id=divCCForm >");
                            this.UCEn1.BindCCForm(wk, "ND" + nd.NodeID, false); //, false, false, null);
                            if (wk.WorkEndInfo.Length > 2)
                                this.UCEn1.Add(wk.WorkEndInfo);
                            this.UCEn1.Add("</div>");
                        }

                        if (nd.HisFormType == FormType.FixForm)
                        {
                            /*傻瓜表单*/
                            this.UCEn1.BindColumn4(wk, "ND" + nd.NodeID); //, false, false, null);
                            if (wk.WorkEndInfo.Length > 2)
                                this.UCEn1.Add(wk.WorkEndInfo);
                        }
                    }
                    else
                    {
                        /* 节点表单与流程表单混合存在的情况。  */
                        //隐藏保存按钮
                        if (this.ToolBar1.IsExit(BP.Web.Controls.NamesOfBtn.Save) == true)
                            this.Btn_Save.Visible = false;

                        // 让其直接update，来接受外部传递过来的信息。
                        if (nd.HisFormType != FormType.DisableIt)
                            wk.DirectUpdate();

                        /*涉及到多个表单的情况...*/
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
                            fid = this.WorkID;

                        if (frms.Count == 1)
                        {
                            /* 仅仅只有一个流程表单的情况。 */
                            Frm frm = (Frm)frms[0];
                            FrmNode fn = frm.HisFrmNode;
                            string src = "";
                            #region update by dgq 一个表单也添加tab页
                            //src = "./CCForm/" + fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + "&FK_Node=" + nd.NodeID + "&WorkID=" + this.WorkID;
                            //this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                            //this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'  /></iframe>");
                            //this.UCEn1.Add("\t\n </DIV>");

                            //this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            //this.UCEn1.Add("\t\n function SaveDtlAll(){}");
                            //this.UCEn1.Add("\t\n</script>");
                            #endregion

                            //********************BEGIN****************************
                            #region 载入相关文件.
                            this.Page.RegisterClientScriptBlock("sg", "<link href='./Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");
                            this.Page.RegisterClientScriptBlock("s2g4", "<script language='JavaScript' src='./Style/Frm/jquery.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdf24j", "<script language='JavaScript' src='./Style/Frm/jquery.idTabs.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdsdf24j", "<script language='JavaScript' src='./Style/Frm/TabClick.js' ></script>");
                            #endregion 载入相关文件.
                            src = "./CCForm/" + fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + "&FK_Node=" + nd.NodeID + "&WorkID=" + this.WorkID;
                            this.UCEn1.Add("\t\n<div  id='usual2' class='usual' >");  //begain.
                            #region 输出标签.
                            this.UCEn1.Add("\t\n <ul  class='abc' style='background:red;border-color: #800000;border-width: 10px;' >");                            
                            this.UCEn1.Add("\t\n<li><a ID='HL" + frm.No + "' href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" + frm.Name + "</a></li>");                            
                            this.UCEn1.Add("\t\n </ul>");
                            #endregion 输出标签.

                            #region 输出表单 iframe 内容.
                            this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                            this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' Onblur=\"OnTabChange('" + frm.No + "',this);\" src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'   /></iframe>");
                            this.UCEn1.Add("\t\n </DIV>");
                            #endregion 输出表单 iframe 内容.
                            this.UCEn1.Add("\t\n</div>"); // end  usual2

                            // 设置选择的默认值.
                            this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frm.No + "\");");

                            #region SaveDtlAll
                            this.UCEn1.Add("\t\n function SaveDtlAll(){");
                            this.UCEn1.Add("\t\n   var tabText = document.getElementById('HL' + " + frm.No + ").innerText;");
                            this.UCEn1.Add("\t\n   var scope = document.getElementById('F' + " + frm.No + ");");
                            this.UCEn1.Add("\t\n   var lastChar = tabText.substring(tabText.length - 1, tabText.length);");
                            this.UCEn1.Add("\t\n   if (lastChar == \"*\") {");
                            this.UCEn1.Add("\t\n   var contentWidow = scope.contentWindow;");
                            this.UCEn1.Add("\t\n   contentWidow.SaveDtlData();");
                            this.UCEn1.Add("\t\n   }");
                            this.UCEn1.Add("\t\n}");
                            #endregion
                            this.UCEn1.Add("\t\n</script>");
                            //*********************END***************************
                            
                        }
                        else
                        {
                            /* 节点表单与流程表单混合存在。 */
                            #region 载入相关文件.
                            this.Page.RegisterClientScriptBlock("sg", "<link href='./Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");
                            this.Page.RegisterClientScriptBlock("s2g4", "<script language='JavaScript' src='./Style/Frm/jquery.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdf24j", "<script language='JavaScript' src='./Style/Frm/jquery.idTabs.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdsdf24j", "<script language='JavaScript' src='./Style/Frm/TabClick.js' ></script>");
                            #endregion 载入相关文件.

                            #region 参数.
                            string urlExtFrm = this.RequestParas;
                            if (urlExtFrm.Contains("WorkID") == false)
                                urlExtFrm += "&WorkID=" + this.WorkID;

                            if (urlExtFrm.Contains("NodeID") == false)
                                urlExtFrm += "&NodeID=" + nd.NodeID;

                            if (urlExtFrm.Contains("FK_Node") == false)
                                urlExtFrm += "&FK_Node=" + nd.NodeID;

                            if (urlExtFrm.Contains("UserNo") == false)
                                urlExtFrm += "&UserNo=" + WebUser.No;

                            if (urlExtFrm.Contains("SID") == false)
                                urlExtFrm += "&SID=" + WebUser.SID;

                            if (urlExtFrm.Contains("IsLoadData") == false)
                                urlExtFrm += "&IsLoadData=1";
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
                                src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + urlExtFrm;
                                this.UCEn1.Add("\t\n<li><a ID='HL" + frm.No + "' href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','/WF/CCForm/" + src + "');\" >" + frm.Name + "</a></li>");
                            }
                            this.UCEn1.Add("\t\n </ul>");
                            #endregion 输出标签.

                            #region 输出表单 iframe 内容.
                            foreach (Frm frm in frms)
                            {
                                FrmNode fn = frm.HisFrmNode;
                                this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                                this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' Onblur=\"OnTabChange('" + frm.No + "',this);\" src='loading.htm' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'   /></iframe>");
                                this.UCEn1.Add("\t\n </DIV>");
                            }
                            #endregion 输出表单 iframe 内容.

                            this.UCEn1.Add("\t\n</div>"); // end  usual2

                            // 设置选择的默认值.
                            this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frms[0].No + "\");");

                            this.UCEn1.Add("\t\n function SaveDtlAll(){}");
                            #region SaveDtlAll
                            this.UCEn1.Add("\t\n function SaveDtlAll(){");
                            this.UCEn1.Add("\t\n   var tabText = document.getElementById('HL' + currentTabId).innerText;");
                            this.UCEn1.Add("\t\n   var scope = document.getElementById('F' + currentTabId);");
                            this.UCEn1.Add("\t\n   var lastChar = tabText.substring(tabText.length - 1, tabText.length);");
                            this.UCEn1.Add("\t\n   if (lastChar == \"*\") {");
                            this.UCEn1.Add("\t\n   var contentWidow = scope.contentWindow;");
                            this.UCEn1.Add("\t\n   contentWidow.SaveDtlData();");
                            this.UCEn1.Add("\t\n   }");
                            this.UCEn1.Add("\t\n}");
                            #endregion
                            this.UCEn1.Add("\t\n</script>");
                        }
                    }
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

                    if (urlExt.Contains("UserNo") == false)
                        urlExt += "&UserNo=" + WebUser.No;

                    if (urlExt.Contains("SID") == false)
                        urlExt += "&SID=" + WebUser.SID;

                    if (url.Contains("?") == true)
                        url += "&" + urlExt;
                    else
                        url += "?" + urlExt;

                    url = url.Replace("?&", "?");

                    if (nd.HisFormType == FormType.SDKForm)
                    {
                        this.Response.Redirect(url, true);
                    }
                    else
                    {
                        //  this.UCEn1.AddIframeExt(url, nd.FrmAttr);
                        this.UCEn1.AddTable("width='93%'  id=ere ");
                        this.UCEn1.Add("<TR id=to >");
                        
                        //   this.UCEn1.Add("<TD ID='WorkPlace' Name='WorkPlace' height='900px' >");

                        this.UCEn1.Add("<TD ID='TDWorkPlace' height='700px' >");
                        this.UCEn1.AddIframeAutoSize(url, "FWorkPlace", "TDWorkPlace");
                        this.UCEn1.Add("</TD>");
                        this.UCEn1.Add("</TR>");

                        this.UCEn1.Add("<TR id=er >");
                        this.UCEn1.Add("<TD id=fd >");
                        this.UCEn1.Add(wk.WorkEndInfo);
                        this.UCEn1.Add("</TD>");
                        this.UCEn1.AddTREnd();
                        this.UCEn1.AddTableEnd();

                        // 设置选择的默认值.
                        this.UCEn1.Add("\t\n<script type='text/javascript'>");
                        this.UCEn1.Add("\t\n function SaveDtlAll(){}");
                        this.UCEn1.Add("\t\n</script>");
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
            string id = "";
            Btn btn = sender as Btn;
            if (btn != null)
                id = btn.ID;
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
                    string info = this.UCEn1.GetTBByID("TB_Doc").Text;
                    if (string.IsNullOrEmpty(info))
                    {
                        this.Alert("请输入强制终止流程的原因。");
                        return;
                    }
                    string infoEnd = BP.WF.Dev2Interface.Flow_DoFlowOverByCoercion(this.FK_Flow, this.WorkID, info);
                    this.ToMsg("结束流程提示:<hr>" + infoEnd, "info");
                    break;
                case NamesOfBtn.Delete:
                case "Btn_Del":
                    // 这是彻底删除的不需要交互。
                    string delMsg = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
                    this.ToMsg("删除流程提示<hr>" + delMsg, "info");
                    break;
                case NamesOfBtn.Save:
                    this.Send(true);
                    if (string.IsNullOrEmpty(this.Request.QueryString["WorkID"]))
                    {
                        //  this.Response.Redirect(this.PageID + ".aspx?FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FromNode=" + this.FromNode+"&FromWorkID="+this.FromWorkID, true);
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
                    this.WinOpen("WFRpt.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + WorkID, "工作报告", 800, 600);
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
            System.Web.HttpContext.Current.Session["RunDT"] = DateTime.Now;
            if (this.FK_Node == 0)
                throw new Exception("没有找到当前的节点");

            // 判断当前人员是否有执行该人员的权限。
            if (currND.IsStartNode == false && BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Node, this.WorkID, WebUser.No) == false)
                throw new Exception("您好：" + WebUser.No + "," + WebUser.Name + "：<br> 当前工作已经被其它人处理，您不能在执行保存或者发送!!!");

            Paras ps = new Paras();
            string dtStr = BP.SystemConfig.AppCenterDBVarStr;

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
                        foreach (MapAttr attr in mattrs)
                        {
                            if (attr.TBModel == 2)
                            {
                                /* 如果是福文本 */
                                currWK.SetValByKey(attr.KeyOfEn, this.Request.Form["ctl00$ContentPlaceHolder1$MyFlowUC1$MyFlow1$UCEn1$TB_" + attr.KeyOfEn]);
                            }

                            if (attr.UIIsEnable)
                                continue;
                            if (attr.DefValReal.Contains("@") == false)
                                continue;
                            currWK.SetValByKey(attr.KeyOfEn, attr.DefVal);
                        }
                        break;
                    case FormType.DisableIt:
                        currWK.Retrieve();
                        break;
                    default:
                        throw new Exception("@未涉及到的情况。");
                }
            }
            catch (Exception ex)
            {
                this.Btn_Send.Enabled = true;
                throw new Exception("@在保存前执行逻辑检查错误。" + ex.Message + " @StackTrace:" + ex.StackTrace);
            }

            #region 判断特殊的业务逻辑
            string dbStr = BP.SystemConfig.AppCenterDBVarStr;
            if (currND.IsStartNode)
            {
                if (this.currND.HisFlow.HisFlowAppType == FlowAppType.PRJ)
                {
                    /*对特殊的流程进行检查，检查是否有权限。*/
                    string prjNo = currWK.GetValStringByKey("PrjNo");
                    ps = new Paras();
                    ps.SQL = "SELECT * FROM WF_NodeStation WHERE FK_Station IN ( SELECT FK_Station FROM Prj_EmpPrjStation WHERE FK_Prj=" + dbStr + "FK_Prj AND FK_Emp=" + dbStr + "FK_Emp )  AND  FK_Node=" + dbStr + "FK_Node ";
                    ps.Add("FK_Prj", prjNo);
                    ps.AddFK_Emp();
                    ps.Add("FK_Node", this.FK_Node);

                    if (DBAccess.RunSQLReturnTable(ps).Rows.Count == 0)
                    {
                        string prjName = currWK.GetValStringByKey("PrjName");
                        ps = new Paras();
                        ps.SQL = "SELECT * FROM Prj_EmpPrj WHERE FK_Prj=" + dbStr + "FK_Prj AND FK_Emp=" + dbStr + "FK_Emp ";
                        ps.Add("FK_Prj", prjNo);
                        ps.AddFK_Emp();
                        //   ps.AddFK_Emp();

                        if (DBAccess.RunSQLReturnTable(ps).Rows.Count == 0)
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

            // 处理节点表单保存事件.
            currND.MapData.FrmEvents.DoEventNode(FrmEventList.SaveBefore, currWK);
            try
            {
                if (currND.IsStartNode)
                    currWK.FID = 0;

                if (currND.HisFlow.IsMD5)
                {
                    /*重新更新md5值.*/
                    currWK.SetValByKey("MD5", Glo.GenerMD5(currWK));
                }

                if (currND.IsStartNode && isSave)
                    currWK.SetValByKey(StartWorkAttr.Title, WorkNode.GenerTitle(currND.HisFlow, this.currWK));

                currWK.Update();
                /*如果是保存*/
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

            #region 2012-10-15  数据也要保存到Rpt表里.
            if (currND.SaveModel == SaveModel.NDAndRpt)
            {
                /* 如果保存模式是节点表与Node与Rpt表. */
                WorkNode wn = new WorkNode(currWK, currND);
                GERpt rptGe = currND.HisFlow.HisFlowData;
                rptGe.SetValByKey("OID", this.WorkID);
                wn.rptGe = rptGe;
                if (rptGe.RetrieveFromDBSources() == 0)
                {
                    rptGe.SetValByKey("OID", this.WorkID);
                    wn.DoCopyRptWork(currWK);

                    rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "," + WebUser.Name);
                    rptGe.SetValByKey(GERptAttr.FlowStarter, WebUser.No);
                    rptGe.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDataTime);
                    rptGe.SetValByKey(GERptAttr.WFState, 0);

                    rptGe.WFState = WFState.Draft;

                    rptGe.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                    rptGe.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                    rptGe.Insert();
                }
                else
                {
                    wn.DoCopyRptWork(currWK);
                    rptGe.Update();
                }
            }
            #endregion

            if (BP.WF.Glo.IsEnableDraft && currND.IsStartNode)
            {
                /*如果启用草稿, 并且是开始节点. */
                BP.WF.Dev2Interface.Node_SaveWork(this.FK_Flow,this.FK_Node,this.WorkID);
            }

            try
            {
                //处理表单保存后。
                currND.MapData.FrmEvents.DoEventNode(FrmEventList.SaveAfter, currWK);
            }
            catch (Exception ex)
            {
                //this.Response.Write(ex.Message);
                this.Alert(ex.Message.Replace("'", "‘"));
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

                // 2012-08-20 为上海修改.
                if (this.PageID.Contains("Small") == false)
                    msg += "@<a href='EmpWorks" + PageSmall + ".aspx?FK_Flow=" + this.FK_Flow + "' >进入待办流程列表</a>，<a href='Start" + PageSmall + ".aspx?FK_Flow=" + this.FK_Flow + "'>进入发起流程列表</a>。";
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
                    if (myurl.Contains("@"))
                        throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);

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
                            if (url.Contains("@"))
                                throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                            url += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&FromWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }

#warning 为上海修改了如果找不到路径就让它按系统的信息提示。

                    this.ToMsg(msg, "info");
                    //throw new Exception("您定义的转向条件不成立，没有出口。");
                    break;
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

    }
}