using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;

namespace BP.WF.Ext
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowSheet : EntityNoName
    {
        /// <summary>
        /// 设计者编号
        /// </summary>
        public string DesignerNo
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerNo);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerNo, value);
            }
        }
        /// <summary>
        /// 设计者名称
        /// </summary>
        public string DesignerName
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.DesignerName);
            }
            set
            {
                this.SetValByKey(FlowAttr.DesignerName, value);
            }
        }

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No == "admin" || this.DesignerNo==WebUser.No)
                {
                    uac.IsUpdate = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowSheet()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowSheet(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("流程编号不存在");
            }
            else
            {
                this.Retrieve();
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "流程";
                map.CodeStruct = "3";

                map.AddTBStringPK(FlowAttr.No, null, "编号", true, true, 1, 10, 3);
                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别",
                    new FlowSorts(), true);
                map.AddTBString(FlowAttr.Name, null, "名称", true, false, 0, 50, 10, true);
                map.AddBoolean(FlowAttr.IsOK, true, "是否起用", true, true);

                map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "运行方式",
                    true, true, FlowAttr.FlowRunWay, "@0=手工启动@1=指定人员按时启动@2=数据集按时启动@3=触发式启动");

                map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 100, 10, true);
                map.AddBoolean(FlowAttr.IsCanStart, true,  "可以独立启动否？(独立启动的流程可以显示在发起流程列表里)" , true, true, true);
                map.AddBoolean(FlowAttr.IsMD5, false, "是否是数据加密流程(MD5数据加密防篡改)", true, true,true);

                map.AddTBStringDoc(FlowAttr.Note, null, "备注", 
                    true, false, true);
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);

                map.AddDDLSysEnum(FlowAttr.AppType, (int)FlowAppType.Normal,"流程应用类型",
                  true, true, "FlowAppType", "@0=正常的@1=工程类(具有项目组概念)");

                map.AddDDLSysEnum(FlowAttr.TimelineRole, (int)TimelineRole.ByNodeSet, "时效性规则",
                 true, true, FlowAttr.TimelineRole, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");

                // 数据存储.
                map.AddDDLSysEnum(FlowAttr.DataStoreModel, (int)DataStoreModel.ByCCFlow,
                    "流程数据存储模式", true, true, FlowAttr.DataStoreModel,
                   "@0=数据轨迹模式@1=数据合并模式");
                map.AddTBString(FlowAttr.PTable, null, "存储主表", true, true, 0, 30, 10);

                // add 2013-02-14 唯一确定此流程的标记
                map.AddTBString(FlowAttr.FlowCode, null, "流程标记", true, true, 0, 150, 10);

                map.AddTBString(FlowAttr.StartListUrl, null, "导航Url", true, false, 0, 500, 10, true);

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);

                // add 2013-03-24.
                map.AddTBString(FlowAttr.DesignerNo, null, "设计者编号", true, false, 0, 32, 10);
                map.AddTBString(FlowAttr.DesignerName, null, "设计者名称", true, false, 0, 100, 10);


                //查询条件.
                map.AddSearchAttr(BP.WF.FlowAttr.FK_FlowSort);

                RefMethod rm = new RefMethod();
                rm.Title =   "抄送节点"; // "抄送节点";
                rm.ToolTip = "当抄送方式设置为抄送节点时，此设置才有效。";
                rm.Icon = "/WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCCNode";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "检查报告"; // "设计检查报告";
                //rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "/WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设计报表"; // "报表运行";
                rm.Icon = "/WF/Img/Btn/View.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                //rm.Icon = "/WF/Img/Btn/Table.gif"; 
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "/WF/Img/Btn/Delete.gif";
                rm.Title = "删除数据"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.Warning = "您确定要执行删除流程数据吗?";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoDelData";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "/WF/Img/Btn/Delete.gif";
                rm.Title = "删除单个流程"; // this.ToE("DelFlowData", "删除数据"); // "删除数据";
                rm.ClassMethodName = this.ToString() + ".DoDelDataOne";
                rm.HisAttrs.AddTBInt("WorkID",0, "输入工作ID",true,false);
                rm.HisAttrs.AddTBString("sd", null, "删除备注", true, false,0,100,100);
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "/WF/Img/Btn/DTS.gif";
                rm.Title = "重新生成报表数据"; // "删除数据";
                rm.Warning = "您确定要执行吗? 注意:此方法耗费资源。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoReloadRptData";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "设置自动发起数据源";
                rm.Icon = "/WF/Img/Btn/DTS.gif";

                rm.ClassMethodName = this.ToString() + ".DoSetStartFlowDataSources()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手工启动定时任务";
                rm.Icon = "/WF/Img/Btn/DTS.gif";
                rm.Warning = "您确定要执行吗? 注意:对于数据量交大的数据因为web上执行时间的限时问题，会造成执行失败。";// "您确定要执行删除流程数据吗？";
                rm.ClassMethodName = this.ToString() + ".DoAutoStartIt()";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "流程数据管理";
                rm.Icon = "/WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoDataManger()";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重新生成流程标题";
                rm.Icon = "/WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoGenerTitle()";
                rm.Warning = "您确定要根据新的规则重新产生标题吗？";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "回滚流程";
                rm.Icon = "/WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoRebackFlowData()";
               
                rm.Warning = "您确定要回滚它吗？";

                rm.HisAttrs.AddTBInt("workid", 0, "请输入要会滚WorkID", true, false);
                rm.HisAttrs.AddTBInt("nodeid", 0, "会滚到的节点ID", true, false);
                rm.HisAttrs.AddTBString("note", null, "回滚原因", true, false,0,600,200);
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置自动发起"; // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif"; 
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("Event", "事件"); // "报表运行";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                ////rm.Icon = "/WF/Img/Btn/Table.gif";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowSheetDataOut", "数据转出定义");  //"数据转出定义";
                ////  rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// </summary>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note"></param>
        /// <returns></returns>
        public string DoRebackFlowData(Int64 workid, int backToNodeID, string note)
        {
            if (note.Length <= 2)
                return "请填写恢复已完成的流程原因.";

            Flow fl = new Flow(this.No);

            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            rpt.OID = workid;
            int i = rpt.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@错误，流程数据丢失。");
            if (backToNodeID == 0)
                backToNodeID = rpt.FlowEndNode;

            Emp empStarter = new Emp(rpt.FlowStarter);

            // 最后一个节点.
            Node endN = new Node(backToNodeID);
            GenerWorkFlow gwf = null;
            try
            {
                #region 创建流程引擎主表数据.
                gwf = new GenerWorkFlow();
                gwf.WorkID = workid;
                if (gwf.RetrieveFromDBSources() == 1)
                    throw new Exception("@当前工作ID为:" + workid + "的流程没有结束,不能采用此方法恢复。");

                gwf.FK_Flow = this.No;
                gwf.FlowName = this.Name;
                gwf.WorkID = workid;
                gwf.PWorkID = rpt.PWorkID;
                gwf.PFlowNo = rpt.PFlowNo;
                gwf.FK_Node = backToNodeID;
                gwf.NodeName = endN.Name;

                gwf.Starter = rpt.FlowStarter;
                gwf.StarterName = empStarter.Name;
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.Title = rpt.Title;
                gwf.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                gwf.FK_Dept = rpt.FK_Dept;

                Dept dept = new Dept(empStarter.FK_Dept);

                gwf.DeptName = dept.Name;
                gwf.PRI = 1;

                DateTime dttime = DateTime.Now;
                dttime = dttime.AddDays(3);

                gwf.SDTOfNode = dttime.ToString("yyyy-MM-dd");
                gwf.SDTOfFlow = dttime.ToString("yyyy-MM-dd");
                gwf.Insert(); /*插入流程引擎数据.*/

                #endregion 创建流程引擎主表数据

                int startNode = int.Parse(this.No + "01");
                string ndTrack = "ND" + int.Parse(this.No) + "Track";
               string actionType = (int)ActionType.Forward + "," + (int)ActionType.FlowOver + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL;
               // string actionType = " NDFrom=" + (int)ActionType.Forward + " OR NDFrom=" + (int)ActionType.FlowOver + " OR NDFrom=" + (int)ActionType.ForwardFL + " OR NDFrom=" + (int)ActionType.ForwardHL;
               string sql = "SELECT  * FROM " + ndTrack + " WHERE   ActionType IN (" + actionType + ")  and WorkID=" + workid + " ORDER BY RDT DESC, NDFrom ";
                System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@工作ID为:" + workid + "的数据不存在.");

                string starter = "";
                bool isMeetSpecNode = false;
                GenerWorkerList currWl = new GenerWorkerList();
                foreach (DataRow dr in dt.Rows)
                {
                    int ndFrom = int.Parse(dr["NDFrom"].ToString());
                    Node nd = new Node(ndFrom);

                    string ndFromT = dr["NDFromT"].ToString();

                    string EmpFrom = dr[TrackAttr.EmpFrom].ToString();
                    string EmpFromT = dr[TrackAttr.EmpFromT].ToString();

                    // 增加上 工作人员的信息.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = workid;
                    gwl.FK_Flow = this.No;

                    gwl.FK_Node = ndFrom;
                    gwl.FK_NodeText = ndFromT;

                    if (gwl.FK_Node == backToNodeID)
                    {
                        gwl.IsPass = false;
                        currWl = gwl;
                    }

                    gwl.FK_Emp = EmpFrom;
                    gwl.FK_EmpText = EmpFromT;
                    if (gwl.IsExits)
                        continue; /*有可能是反复退回的情况.*/

                    Emp emp = new Emp(gwl.FK_Emp);
                    gwl.FK_Dept1 = emp.FK_Dept;

                    gwl.RDT = dr["RDT"].ToString();
                    gwl.SDT = dr["RDT"].ToString();
                    gwl.DTOfWarning = gwf.SDTOfNode;
                    gwl.WarningDays = nd.WarningDays;
                    gwl.IsEnable = true;
                    gwl.WhoExeIt = nd.WhoExeIt;
                    gwl.Insert();
                }

                #region 加入退回信息, 让接受人能够看到退回原因.
                ReturnWork rw = new ReturnWork();
                rw.WorkID = workid;
                rw.ReturnNode = backToNodeID;
                rw.ReturnNodeName = endN.Name;
                rw.Returner = WebUser.No;
                rw.ReturnerName = WebUser.Name;

                rw.ReturnToNode = currWl.FK_Node;
                rw.ReturnToEmp = currWl.FK_Emp;
                rw.Note = note;
                rw.RDT = DataType.CurrentDataTime;
                rw.IsBackTracking = false;
                rw.MyPK = BP.DA.DBAccess.GenerGUID();
                #endregion   加入退回信息, 让接受人能够看到退回原因.

                //更新流程表的状态.
                rpt.FlowEnder = currWl.FK_Emp;
                rpt.WFState = WFState.ReturnSta; /*设置为退回的状态*/
                rpt.FlowEndNode = currWl.FK_Node;
                rpt.Update();

                // 向接受人发送一条消息.
                BP.WF.Dev2Interface.Port_SendMail(currWl.FK_Emp, "工作恢复:" + gwf.Title, "工作被:"+WebUser.No+" 恢复." +note,"ReBack"+workid, this.No, int.Parse(this.No+"01") ,workid,0);

                //写入该日志.
                WorkNode wn = new WorkNode(workid, currWl.FK_Node);
                wn.AddToTrack(ActionType.RebackOverFlow, currWl.FK_Emp, currWl.FK_EmpText, currWl.FK_Node, currWl.FK_NodeText, note);

                return "@已经还原成功,现在的流程已经复原到("+currWl.FK_NodeText+"). @当前工作处理人为(" + currWl.FK_Emp + " , " + currWl.FK_EmpText + ")  @请通知他处理工作.";
            }
            catch (Exception ex)
            {
                gwf.Delete();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Delete(GenerWorkerListAttr.WorkID, workid);

                string sqls = "";
                sqls += "@UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Complete + " WHERE OID=" + workid;
                DBAccess.RunSQLs(sqls);
                return "<font color=red>会滚期间出现错误</font><hr>" + ex.Message;
            }
        }
        /// <summary>
        /// 重新产生标题，根据新的规则.
        /// </summary>
        public string DoGenerTitle()
        {
            if (WebUser.No != "admin")
                return "非admin用户不能执行。";
            Flow fl = new Flow(this.No);
            Node nd = fl.HisStartNode;
            Works wks = nd.HisWorks;
            wks.RetrieveAllFromDBSource(WorkAttr.Rec);
            string table = nd.HisWork.EnMap.PhysicsTable;
            string tableRpt = "ND" + int.Parse(this.No) + "Rpt";
            Sys.MapData md = new Sys.MapData(tableRpt);
            foreach (Work wk in wks)
            {

                if (wk.Rec != WebUser.No)
                {
                    BP.Web.WebUser.Exit();
                    try
                    {
                        Emp emp = new Emp(wk.Rec);
                        BP.Web.WebUser.SignInOfGener(emp);
                    }
                    catch
                    {
                        continue;
                    }
                }
                string sql = "";
                string title = WorkNode.GenerTitle(fl, wk);
                Paras ps = new Paras();
                ps.Add("Title", title);
                ps.Add("OID", wk.OID);
                ps.SQL = "UPDATE " + table + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE " + md.PTable + " SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE OID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQL(ps);

                ps.SQL = "UPDATE WF_GenerFH SET Title=" + SystemConfig.AppCenterDBVarStr + "Title WHERE FID=" + SystemConfig.AppCenterDBVarStr + "OID";
                DBAccess.RunSQLs(sql);
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);
            return "全部生成成功,影响数据(" + wks.Count + ")条";
        }
        /// <summary>
        /// 流程数据管理
        /// </summary>
        /// <returns></returns>
        public string DoDataManger()
        {
            PubClass.WinOpen("/WF/Admin/FlowDB.aspx?s=d34&FK_Flow=" + this.No + "&ExtType=StartFlow&RefNo==", 700, 500);
            return null;
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoAutoStartIt();
        }

        public string DoDelDataOne(int workid, string sd)
        {
            return "删除成功 workid="+workid+"  理由:"+sd;
        }

        

        public string DoSetStartFlowDataSources()
        {
            string flowID=int.Parse(this.No).ToString()+"01";
            PubClass.WinOpen("/WF/MapDef/MapExt.aspx?s=d34&FK_MapData=ND" + flowID + "&ExtType=StartFlow&RefNo==", 700, 500);
            return null;
        }
        public string DoCCNode()
        {
            PubClass.WinOpen("/WF/Admin/CCNode.aspx?FK_Flow=" + this.No, 400, 500);
            return null;
        }
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoCheck();
        }

        public string DoReloadRptData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoReloadRptData();
        }

        public string DoDelData()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDelData();
        }

        /// <summary>
        /// 设计数据转出
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoExp();
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoDRpt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDRpt();
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoOpenRpt();
        }
        /// <summary>
        /// 更新之后的事情，也要更新缓存。
        /// </summary>
        protected override void afterUpdate()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            fl.Update();
            base.afterUpdate();
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowSheets : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowSheets() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowSheets(string fk_sort)
        {
            this.Retrieve(BP.WF.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowSheet();
            }
        }
        #endregion
    }
}

