using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Port;
using BP.En;
using BP.Web;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Win32;  

namespace BP.WF
{
    /// <summary>
    /// 流程
    /// 记录了流程的信息．
    /// 流程的编号，名称，建立时间．
    /// </summary>
    public class Flow : EntityNoName
    {
        #region 业务处理
        /// <summary>
        /// 
        /// </summary>
        public TimelineRole HisTimelineRole
        {
            get
            {
                return (TimelineRole)this.GetValIntByKey(FlowAttr.TimelineRole);
            }
        }
        /// <summary>
        /// 轨迹字段
        /// </summary>
        public string AppType
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.AppType);
            }
            set
            {
                this.SetValByKey(FlowAttr.AppType, value);
            }
        }
        /// <summary>
        /// 流程编码
        /// </summary>
        public string FlowCode
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.FlowCode);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowCode, value);
            }
        }
        /// <summary>
        /// 创建新工作
        /// </summary>
        /// <returns></returns>
        public StartWork NewWork()
        {
            return NewWork(WebUser.No);
        }
        /// <summary>
        /// 创建新工作.
        /// </summary>
        /// <param name="empNo">人员编号</param>
        /// <returns></returns>
        public StartWork NewWork(string empNo)
        {
            Emp emp = new Emp(empNo);
            return NewWork(emp);

        }
        /// <summary>
        /// 产生一个新工作
        /// </summary>
        /// <returns></returns>
        public StartWork NewWork(Emp emp)
        {
            //开始节点.
            BP.WF.Node nd = new BP.WF.Node(this.StartNodeID);

            //从草稿里看看是否有新工作？
            StartWork wk = (StartWork)nd.HisWork;
            string dbstr=SystemConfig.AppCenterDBVarStr;
            
            Paras ps = new Paras();
            /*如果要启用草稿,就创建一个新的WorkID.*/
            try
            {
                //从报表里查询该数据是否存在？
                ps.SQL = "SELECT OID,FlowEndNode FROM " + this.PTable + " WHERE FlowStarter=" + dbstr + "FlowStarter AND WFState=" + dbstr + "WFState ";
                ps.Add(GERptAttr.FlowStarter, emp.No);
                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 1)
                {
                    wk.OID = Int64.Parse(dt.Rows[0][0].ToString());
                    int nodeID = int.Parse(dt.Rows[0][1].ToString());
                    if (nodeID != this.StartNodeID)
                    {
                        string error = "@这里出现了blank的状态下流程运行到其它的节点上去了的情况。";
                        Log.DefaultLogWriteLineError(error);
                        throw new Exception(error);
                    }
                }

                if (wk.OID == 0)
                {
                    /*说明没有空白,就创建一个空白..*/
                    wk.ResetDefaultVal();
                    wk.Rec = WebUser.No;

                    wk.SetValByKey("RecText", emp.Name);
                    wk.SetValByKey("Emps", emp.No);

                    wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);

                    wk.OID = DBAccess.GenerOID("WorkID"); /*这里产生WorkID ,这是唯一产生WorkID的地方.*/

                    //把尽量可能的流程字段放入，否则会出现冲掉流程字段属性.
                    wk.SetValByKey(GERptAttr.FK_NY, BP.DA.DataType.CurrentYearMonth);
                    wk.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
                    wk.DirectInsert();

                    if (this.PTable == wk.EnMap.PhysicsTable)
                    {

                        /*如果开始节点表与流程报表相等.*/
                        GERpt rpt = this.HisFlowData;
                        rpt.OID = wk.OID;
                        rpt.RetrieveFromDBSources();
                        rpt.FID = 0;
                        rpt.FlowStartRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.MyNum = 0;
                        rpt.Title = "未生成";
                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;
                        rpt.FK_NY = DataType.CurrentYearMonth;
                        rpt.FlowEmps = "@" + emp.No + "," + emp.Name;
                        rpt.FlowEnderRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.FK_Dept = emp.FK_Dept;
                        rpt.FlowEnder = emp.No;
                        rpt.FlowEndNode =this.StartNodeID;
                        rpt.DirectUpdate();
                    }
                    else
                    {
                        GERpt rpt = this.HisFlowData;
                        rpt.OID = wk.OID;
                        rpt.FID = 0;
                        rpt.FlowStartRDT = BP.DA.DataType.CurrentDataTime;
                        rpt.FlowEnderRDT = BP.DA.DataType.CurrentDataTime;

                        rpt.MyNum = 0;
                        rpt.Title = "未生成";

                        rpt.WFState = WFState.Blank;
                        rpt.FlowStarter = emp.No;

                        rpt.FlowEndNode = this.StartNodeID;
                        rpt.FlowEmps = "@" + emp.No + "," + emp.Name;
                        rpt.FlowEndNode = this.StartNodeID;
                        rpt.FK_NY = DataType.CurrentYearMonth;
                        rpt.FK_Dept = emp.FK_Dept;

                        rpt.FlowEnder = emp.No;
                        rpt.FlowEndNode = this.StartNodeID;
                        rpt.InsertAsOID(wk.OID);
                    }
                }
            }
            catch (Exception ex)
            {
                wk.CheckPhysicsTable();
                throw ex;
            }

            if (SystemConfig.IsBSsystem)
            {
                // 记录这个id ,不让其它在复制时间被修改。
                Int64 newOID = wk.OID;
                // 处理传递过来的参数。
                int i = 0;
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    i++;
                    wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }

                if (i >= 3)
                {
                    wk.OID = newOID;
                    wk.DirectUpdate();
                }

                #region 处理删除草稿的需求。
                if ( System.Web.HttpContext.Current.Request.QueryString["IsDeleteDraft"] == "1" )
                {
                    /*是否要删除Draft */
                    Int64 oid = wk.OID;
                    //if (num != 0)
                    //{
                    wk.ResetDefaultVal();
                    try
                    {
                        wk.DirectUpdate();
                    }
                    catch
                    {
                    }

                    MapDtls dtls = wk.HisMapDtls;
                    foreach (MapDtl dtl in dtls)
                        DBAccess.RunSQL("DELETE " + dtl.PTable + " WHERE RefPK=" + oid);

                    //删除附件数据。
                    DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + wk.NodeID + "' AND RefPKVal='" + wk.OID + "'");
                    wk.OID = newOID;
                }
                #endregion 处理删除草稿的需求。

                #region 处理开始节点.
                if (System.Web.HttpContext.Current.Request.QueryString["FromTableName"] != null)
                {
                    string tableName = System.Web.HttpContext.Current.Request.QueryString["FromTableName"];
                    string tablePK = System.Web.HttpContext.Current.Request.QueryString["FromTablePK"];
                    string tablePKVal = System.Web.HttpContext.Current.Request.QueryString["FromTablePKVal"];

                    DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM " + tableName + " WHERE " + tablePK + "='" + tablePKVal + "'");
                    if (dt.Rows.Count == 0)
                        throw new Exception("@利用table传递数据错误，没有找到指定的行数据，无法为用户填充数据。");

                    string innerKeys = ",OID,RDT,CDT,FID,WFState,";
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (innerKeys.Contains("," + dc.ColumnName + ","))
                            continue;

                        wk.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName].ToString());
                    }
                }
                #endregion 处理删除草稿的需求。

                #region 处理流程之间的数据传递1。
                if (System.Web.HttpContext.Current.Request.QueryString["FromWorkID"] != null)
                {
                    /* 如果是从另外的一个流程上传递过来的，就考虑另外的流程数据。*/
                    int fromNode = int.Parse(System.Web.HttpContext.Current.Request.QueryString["FromNode"]);
                    Int64 fromWorkID = Int64.Parse(System.Web.HttpContext.Current.Request.QueryString["FromWorkID"]);
                    BP.WF.Node fromNd = new BP.WF.Node(fromNode);
                    Work wkFrom = fromNd.HisWork;
                    wkFrom.OID = fromWorkID;
                    wkFrom.RetrieveFromDBSources();
                    wk.Copy(wkFrom);
                    i = 0;
                    foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                    {
                        i++;
                        wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                    }
                    wk.OID = newOID;

                    // 在执行copy后，有可能这两个字段会被冲掉。
                    wk.SetValByKey("FromWorkID", fromWorkID);
                    wk.SetValByKey("FromNode", fromNode);
                    wk.Update();

                    //复制明细。
                    MapDtls dtls = wk.HisMapDtls;
                    if (dtls.Count > 0)
                    {
                        MapDtls dtlsFrom = wkFrom.HisMapDtls;
                        int idx = 0;

                        if (dtlsFrom.Count == dtls.Count)
                        {
                            foreach (MapDtl dtl in dtls)
                            {
                                if (dtl.IsCopyNDData == false)
                                    continue;

                                //new 一个实例.
                                GEDtl dtlData = new GEDtl(dtl.No);
                                MapDtl dtlFrom = dtlsFrom[idx] as MapDtl;

                                GEDtls dtlsFromData = new GEDtls(dtlFrom.No);
                                dtlsFromData.Retrieve(GEDtlAttr.RefPK, fromWorkID);
                                foreach (GEDtl geDtlFromData in dtlsFromData)
                                {
                                    dtlData.Copy(geDtlFromData);
                                    dtlData.RefPK = wk.OID.ToString();
                                    dtlData.SaveAsOID(geDtlFromData.OID);
                                }
                            }
                        }
                    }

                    //复制附件数据。
                    if (wk.HisFrmAttachments.Count > 0)
                    {
                        if (wkFrom.HisFrmAttachments.Count > 0)
                        {
                            //删除数据。
                            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + wk.NodeID + "' AND RefPKVal='" + wk.OID + "'");
                            FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + fromNode, fromWorkID.ToString());
                            int idx = 0;
                            foreach (FrmAttachmentDB athDB in athDBs)
                            {
                                idx++;
                                FrmAttachmentDB dbNew = new FrmAttachmentDB();
                                dbNew.Copy(athDB);
                                dbNew.RefPKVal = wk.OID.ToString();
                                dbNew.FK_FrmAttachment = dbNew.FK_FrmAttachment.Replace("ND" + fromNode, "ND" + wk.HisNode.NodeID);

                                dbNew.MyPK = dbNew.FK_FrmAttachment + "_" + dbNew.RefPKVal + "_" + idx;
                                dbNew.FK_MapData = "ND" + wk.HisNode.NodeID;
                                dbNew.Insert();
                            }
                        }
                    }
                }
                #endregion 处理流程之间的数据传递。

                #region 处理流程之间的数据传递2。
                if (System.Web.HttpContext.Current.Request.QueryString["JumpToNode"] != null)
                {
                    wk.Rec = WebUser.No;
                    wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
                    wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
                    wk.FK_Dept = emp.FK_Dept;
                    wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
                    wk.SetValByKey("FK_DeptText", emp.FK_DeptText);
                    wk.FID = 0;
                    wk.SetValByKey("RecText", emp.Name);

                    int jumpNodeID = int.Parse(System.Web.HttpContext.Current.Request.QueryString["JumpToNode"]);
                    Node jumpNode = new Node(jumpNodeID);

                    string jumpToEmp = System.Web.HttpContext.Current.Request.QueryString["JumpToEmp"];
                    if (string.IsNullOrEmpty(jumpToEmp))
                        jumpToEmp = emp.No;

                    WorkNode wn = new WorkNode(wk, nd);
                    wn.NodeSend(jumpNode, jumpToEmp);

                    WorkFlow wf = new WorkFlow(this, wk.OID, wk.FID);
                    return wf.GetCurrentWorkNode().HisWork as StartWork ;
                }
                #endregion 处理流程之间的数据传递。
            }

            wk.Rec = emp.No;
            wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey("FK_NY", DataType.CurrentYearMonth);
            wk.FK_Dept = emp.FK_Dept;
            wk.SetValByKey("FK_DeptName", emp.FK_DeptText);
            wk.SetValByKey("FK_DeptText", emp.FK_DeptText);
            wk.FID = 0;
            wk.SetValByKey("RecText", emp.Name);

            if (wk.IsExits == false)
                wk.DirectInsert();

            return wk;
        }

        /// <summary>
        /// 创建工作
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="fk_node"></param>
        /// <returns></returns>
        public Work GenerWork(Int64 workid, Node nd)
        {
            Work wk = nd.HisWork;
           // wk.ResetDefaultVal();
            wk.OID = workid;
            if (wk.RetrieveFromDBSources() == 0)
            {
                /*
                 * 2012-10-15 偶然发现一次工作丢失情况, WF_GenerWorkerlist WF_GenerWorkFlow 都有这笔数据，没有查明丢失原因。 zp
                 * 用如下代码自动修复，但是会遇到数据copy不完全的问题。
                 * */
#warning 2011-10-15 偶然发现一次工作丢失情况.

                string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
                GERpt rpt = new GERpt(fk_mapData);
                rpt.OID = int.Parse(workid.ToString());
                if (rpt.RetrieveFromDBSources() >= 1)
                {
                    /*  查询到报表数据.  */
                    wk.Copy(rpt);
                    wk.Rec = WebUser.No;
                    wk.InsertAsOID(workid);
                }
                else
                {
                    /*  没有查询到报表数据.  */
#warning 这里不应该出现的异常信息.

                    string msg = "@不应该出现的异常.";
                    msg += "@在为节点NodeID=" + nd.NodeID + " workid:" + workid + " 获取数据时.";
                    msg += "获取它的Rpt表数据时，不应该查询不到。";
                    msg += "GERpt 信息: table:" + rpt.EnMap.PhysicsTable + "   OID=" + rpt.OID;

                    string sql = "SELECT count(*) FROM " + rpt.EnMap.PhysicsTable + " WHERE OID=" + workid;
                    int num = DBAccess.RunSQLReturnValInt(sql);

                    msg += " @SQL:" + sql;
                    msg += " ReturnNum:" + num;
                    if (num == 0)
                    {
                        msg += "已经用sql可以查询出来，但是不应该用类查询不出来.";
                    }
                    else
                    {
                        /*如果可以用sql 查询出来.*/
                        num = rpt.RetrieveFromDBSources();
                        msg += "@从rpt.RetrieveFromDBSources = " + num;
                    }

                    Log.DefaultLogWriteLineError(msg);

                    MapData md = new MapData("ND" + int.Parse(nd.FK_Flow) + "01");
                    sql = "SELECT * FROM " + md.PTable + " WHERE OID=" + workid;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 1)
                    {
                        rpt.Copy(dt.Rows[0]);
                        try
                        {
                            rpt.FlowStarter = dt.Rows[0][StartWorkAttr.Rec].ToString();
                            rpt.FlowStartRDT = dt.Rows[0][StartWorkAttr.RDT].ToString();
                            rpt.FK_Dept = dt.Rows[0][StartWorkAttr.FK_Dept].ToString();
                        }
                        catch
                        {
                        }
                        rpt.OID = int.Parse(workid.ToString());
                        try
                        {
                            rpt.InsertAsOID(rpt.OID);
                        }
                        catch (Exception ex)
                        {
                            Log.DefaultLogWriteLineError("@不应该出插入不进去 rpt:" + rpt.EnMap.PhysicsTable + " workid=" + workid);
                            rpt.RetrieveFromDBSources();
                        }
                    }
                    else
                    {
                        Log.DefaultLogWriteLineError("@没有找到开始节点的数据, NodeID:" + nd.NodeID + " workid:" + workid);
                        throw new Exception("@没有找到开始节点的数据, NodeID:" + nd.NodeID + " workid:" + workid + " sql:" + sql);
                    }

#warning 不应该出现的工作丢失.
                    Log.DefaultLogWriteLineError("@工作[" + nd.NodeID + " : " + wk.EnDesc + "], 报表数据WorkID=" + workid + " 丢失, 没有从NDxxxRpt里找到记录,请联系管理员。");

                    wk.Copy(rpt);
                    wk.Rec = WebUser.No;
                    wk.ResetDefaultVal();
                    wk.Insert();
                }
            }

            // 设置当前的人员把记录人。
            wk.Rec = WebUser.No;
            wk.RecText = WebUser.Name;
            wk.Rec = WebUser.No;
            wk.SetValByKey(WorkAttr.RDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(WorkAttr.CDT, BP.DA.DataType.CurrentDataTime);
            wk.SetValByKey(GERptAttr.WFState, WFState.Runing);
            wk.SetValByKey("FK_Dept", WebUser.FK_Dept);
            wk.SetValByKey("FK_DeptName", WebUser.FK_DeptName);
            wk.SetValByKey("FK_DeptText", WebUser.FK_DeptName);
            wk.FID = 0;
            wk.SetValByKey("RecText", WebUser.Name);
            return wk;
        }
        #endregion 业务处理

        /// <summary>
        /// 自动发起
        /// </summary>
        /// <returns></returns>
        public string DoAutoStartIt()
        {
            switch (this.HisFlowRunWay)
            {
                case BP.WF.FlowRunWay.SpecEmp: //指定人员按时运行。
                    string RunObj = this.RunObj;
                    string FK_Emp = RunObj.Substring(0, RunObj.IndexOf('@'));
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = FK_Emp;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        return "启动自动启动流程错误：发起人(" + FK_Emp + ")不存在。";
                    }

                    BP.Web.WebUser.SignInOfGener(emp);
                    string info_send= BP.WF.Dev2Interface.Node_StartWork(this.No,null,null,0,null,0,null).ToMsgOfText();
                    if (WebUser.No != "admin")
                    {
                        emp = new BP.Port.Emp();
                        emp.No = "admin";
                        emp.Retrieve();
                        BP.Web.WebUser.SignInOfGener(emp);
                        return info_send;
                    }
                    return info_send;
                case BP.WF.FlowRunWay.DataModel: //按数据集合驱动的模式执行。
                    break;
                default:
                    return "@该流程您没有设置为自动启动的流程类型。";
            }

            string msg = "";
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = "ND" + int.Parse(this.No) + "01_" + MapExtXmlList.StartFlow;
            int i = me.RetrieveFromDBSources();
            if (i == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }
            if (string.IsNullOrEmpty(me.Tag))
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return "没有为流程(" + this.Name + ")的开始节点设置发起数据,请参考说明书解决.";
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = BP.DA.DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                return "流程(" + this.Name + ")此时无任务,查询语句:"+me.Tag.Replace("'","”");
            }

            msg+="@查询到(" + dtMain.Rows.Count + ")条任务.";

            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                return "流程(" + this.Name + ")的开始节点设置发起数据,不完整." + errMsg;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.
          
               string fk_mapdata = "ND" + int.Parse(this.No) + "01";

            MapData md = new MapData(fk_mapdata);
            int idx = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                idx++;

                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + md.PTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                {
                    msg+= "@" + this.Name + ",第" + idx + "条,此任务在之前已经完成。";
                    continue; /*说明已经调度过了*/
                }

                string starter = dr["Starter"].ToString();
                if (WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.No = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        msg+="@" + this.Name + ",第" + idx + "条,设置的发起人员:" + emp.No + "不存在.";
                       msg+="@数据驱动方式发起流程(" + this.Name + ")设置的发起人员:" + emp.No + "不存在。";
                        continue;
                    }
                    WebUser.SignInOfGener(emp);
                }

                #region  给值.
                Work wk = this.NewWork();
                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    // MapData md = new MapData(nodeTable);
                    MapDtls dtls = md.MapDtls; // new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != mainPK)
                                    continue;

                                dtlEn = dtl.HisGEDtl;
                                foreach (DataColumn dc in dt.Columns)
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());

                                dtlEn.RefPK = wk.OID.ToString();
                                dtlEn.OID = 0;
                                dtlEn.Insert();
                            }
                        }
                    }
                }
                #endregion  给值.

                // 处理发送信息.
                Node nd = this.HisStartNode;
                try
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    string infoSend = wn.NodeSend().ToMsgOfHtml();
                    BP.DA.Log.DefaultLogWriteLineInfo(msg);
                    msg+= "@" + this.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "已完成.\r\n" + infoSend;
                    //this.SetText("@第（" + idx + "）条任务，" + WebUser.No + " - " + WebUser.Name + "已经完成。\r\n" + msg);
                }
                catch (Exception ex)
                {
                    msg+= "@" + this.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message;
                }
                  msg+= "<hr>";
            }
            return msg;
            #endregion 处理流程发起.
        }
        public string Tag = null;
        /// <summary>
        /// 运行类型
        /// </summary>
        public FlowRunWay HisFlowRunWay
        {
            get
            {
                return (FlowRunWay)this.GetValIntByKey(FlowAttr.FlowRunWay);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowRunWay, (int)value);
            }
        }
        /// <summary>
        /// 运行对象
        /// </summary>
        public string RunObj
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.RunObj);
            }
            set
            {
                this.SetValByKey(FlowAttr.RunObj, value);
            }
        }
        public string StartListUrl
        {
            get
            {
                return this.GetValStrByKey(FlowAttr.StartListUrl);
            }
            set
            {
                this.SetValByKey(FlowAttr.StartListUrl, value);
            }
        }
        /// <summary>
        /// 流程表单类型
        /// </summary>
        public FlowSheetType HisFlowSheetType
        {
            get
            {
                return (FlowSheetType)this.GetValIntByKey(FlowAttr.FlowSheetType);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowSheetType, (int)value);
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 修补流程数据视图
        /// </summary>
        /// <returns></returns>
        public static string RepareV_FlowData_View()
        {
            string err = "";
            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            if (fls.Count == 1)
                return null;

            if (fls.Count == 0)
                return null;

            string sql = "";
            sql = "CREATE VIEW V_FlowData (FK_FlowSort,FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,MyNum) ";
            //     sql += "\t\n /*  WorkFlow Data " + DateTime.Now.ToString("yyyy-MM-dd") + " */ ";
            sql += " AS ";
            foreach (Flow fl in fls)
            {
                string mysql = "\t\n SELECT '" + fl.FK_FlowSort + "' AS FK_FlowSort,'" + fl.No + "' AS FK_Flow,OID,FID,Title,WFState,CDT,FlowStarter,FlowStartRDT,FK_Dept,FK_NY,FlowDaySpan,FlowEmps,FlowEnder,FlowEnderRDT,FlowEndNode,1 as MyNum FROM "+fl.PTable;
                try
                {
                    DBAccess.RunSQLReturnTable(mysql);
                }
                catch (Exception ex)
                {
                    continue;
                    try
                    {
                        fl.DoCheck();
                        DBAccess.RunSQLReturnTable(mysql);
                    }
                    catch (Exception ex1)
                    {
                        err += ex1.Message;
                        continue;
                    }
                }
                sql += mysql;
                sql += "\t\n UNION ";
            }
            if (sql.Contains("SELECT") == false)
                return null;
            sql = sql.Substring(0, sql.Length - 6);
            if (sql.Length > 20)
            {
                try
                {
                    DBAccess.RunSQL("DROP VIEW V_FlowData");
                }
                catch
                {
                    try
                    {
                        DBAccess.RunSQL("DROP table V_FlowData");
                    }
                    catch
                    {
                    }
                }

                try
                {
                    DBAccess.RunSQL(sql);
                }
                catch
                {
                }
            }
            return null;
        }
        /// <summary>
        /// 校验流程
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            try
            {
                DBAccess.RunSQL("UPDATE WF_Node SET FlowName = (SELECT Name FROM WF_Flow WHERE NO=WF_Node.FK_Flow)");
                DBAccess.RunSQL("DELETE WF_Direction WHERE Node=ToNode");

                this.NumOfBill = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM WF_BillTemplate WHERE NodeID IN (select NodeID from WF_Flow WHERE no='" + this.No + "')");
                this.NumOfDtl = DBAccess.RunSQLReturnValInt("SELECT count(*) FROM Sys_MapDtl WHERE FK_MapData='ND" + int.Parse(this.No) + "Rpt'");
                this.DirectUpdate();

                string msg = "<font color=blue>关于《" + this.Name + " 》 流程检查信息</font><hr>";
                Nodes nds = new Nodes(this.No);
                Emps emps = new Emps();

                string rpt = "<html>\r\n<title>" + this.Name + "-流程诊断报告</title>";
                rpt += "\r\n<body>\r\n<table width='70%' align=center border=1>\r\n<tr>\r\n<td>";
                rpt += "\r\n<div aligen=center><h1><b>" + this.Name + " - 流程设计文档</b></h1></div>";
                rpt += "<b>流程设计:</b><br>";
                rpt += "&nbsp;&nbsp;" + this.Note;
                rpt += "\r\n<br><b>信息:</b><br>";

                BillTemplates bks = new BillTemplates(this.No);

                rpt += "&nbsp;&nbsp;步骤:" + nds.Count + "，单据:" + bks.Count + "个。";

                #region 对节点进行检查
                foreach (Node nd in nds)
                {
                    //设置它的位置类型.
                    nd.SetValByKey(NodeAttr.NodePosType, (int)nd.GetHisNodePosType());

                    #region 修复数节点表单数据库.
                    try
                    {
                        nd.RepareMap();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(nd.Name + " - " + ex.Message);
                    }

                    DBAccess.RunSQL("UPDATE Sys_MapData SET Name='" + nd.Name + "' WHERE No='ND" + nd.NodeID + "'");
                    try
                    {
                        nd.HisWork.CheckPhysicsTable();
                    }
                    catch (Exception ex)
                    {
                        rpt += "@" + nd.Name + " , 节点类型NodeWorkTypeText=" + nd.NodeWorkTypeText + "出现错误.@err=" + ex.Message;
                    }

                    BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nd.NodeID);
                    foreach (BP.Sys.MapAttr attr in attrs)
                    {
                        rpt += attr.KeyOfEn + " " + attr.Name + "、";
                    }

                    // 从表检查。
                    Sys.MapDtls dtls = new BP.Sys.MapDtls("ND" + nd.NodeID);
                    foreach (Sys.MapDtl dtl in dtls)
                    {
                        dtl.HisGEDtl.CheckPhysicsTable();

                        rpt += "<br>明细：" + dtl.Name;
                        BP.Sys.MapAttrs attrs1 = new BP.Sys.MapAttrs(dtl.No);
                        foreach (BP.Sys.MapAttr attr in attrs1)
                        {
                            rpt += attr.KeyOfEn + " " + attr.Name + "、";
                        }
                    }
                    rpt += "<br>岗位：";
                    #endregion 修复数节点表单数据库.


                    #region 对访问规则进行检查
                    switch (nd.HisDeliveryWay)
                    {
                        case DeliveryWay.ByStation:
                            if (nd.NodeStations.Count == 0)
                                rpt += "<font color=red>您设置了该节点的访问规则是按岗位，但是您没有为节点绑定岗位。</font>";
                            break;
                        case DeliveryWay.ByDept:
                            if (nd.NodeDepts.Count == 0)
                                rpt += "<font color=red>您设置了该节点的访问规则是按部门，但是您没有为节点绑定部门。</font>";
                            break;
                        case DeliveryWay.ByBindEmp:
                            if (nd.NodeEmps.Count == 0)
                                rpt += "<font color=red>您设置了该节点的访问规则是按人员，但是您没有为节点绑定人员。</font>";
                            break;
                        case DeliveryWay.BySpecNodeEmp: /*按指定的岗位计算.*/
                        case DeliveryWay.BySpecNodeEmpStation: /*按指定的岗位计算.*/
                            if (nd.DeliveryParas.Trim().Length == 0)
                            {
                                rpt += "<font color=red>您设置了该节点的访问规则是按指定的岗位计算，但是您没有设置节点编号.</font>";
                            }
                            else
                            {
                                if (DataType.IsNumStr(nd.DeliveryParas) == false)
                                {
                                    rpt += "<font color=red>您没有设置指定岗位的节点编号，目前设置的为{" + nd.DeliveryParas + "}</font>";
                                }
                            }
                            break;
                        case DeliveryWay.BySQL:
                            if (nd.DeliveryParas.Trim().Length == 0)
                            {
                                rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询，但是您没有在节点属性里设置查询sql，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册 。</font>";
                            }
                            else
                            {
                                try
                                {
                                    DataTable testDB = DBAccess.RunSQLReturnTable(nd.DeliveryParas);
                                    if (testDB.Columns.Contains("No") == false || testDB.Columns.Contains("Name") == false)
                                    {
                                        rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询，设置的sql不符合规则，此sql的要求是查询必须包含No,Name两个列，sql表达式里支持@+字段变量，详细参考开发手册 。</font>";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    rpt += "<font color=red>您设置了该节点的访问规则是按SQL查询,执行此语句错误." + ex.Message + "</font>";
                                }
                            }
                            break;
                        case DeliveryWay.ByPreviousNodeFormEmpsField:
                            if (attrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, nd.DeliveryParas) == false)
                            {
                                /*检查节点字段是否有FK_Emp字段*/
                                rpt += "<font color=red>您设置了该节点的访问规则是按指定节点表单人员，但是您没有在节点表单中增加FK_Emp字段，详细参考开发手册 。</font>";
                            }
                            break;
                        case DeliveryWay.BySelected: /* 由上一步发送人员选择 */
                            if (nd.IsStartNode)
                            {
                                rpt += "<font color=red>开始节点不能设置指定的选择人员访问规则。</font>";
                                break;
                            }
                            if (attrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, "FK_Emp") == false)
                            {
                                /*检查节点字段是否有FK_Emp字段*/
                                rpt += "<font color=red>您设置了该节点的访问规则是按指定节点表单人员，但是您没有在节点表单中增加FK_Emp字段，详细参考开发手册 。</font>";
                            }
                            break;
                        case DeliveryWay.ByPreviousOper: /* 由上一步发送人员选择 */
                        case DeliveryWay.ByPreviousOperSkip: /* 由上一步发送人员选择 */
                            if (nd.IsStartNode)
                            {
                                rpt += "<font color=red>节点访问规则设置错误:开始节点。</font>";
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion

                    // 岗位是否设置正确。
                    msg += "<b>《" + nd.Name + "》：</b>节点类型：" + nd.HisNodeWorkType + "<hr>";
                    if (nd.NodeStations.Count == 0)
                    {
                        string infoAccept = "";
                        if (nd.HisDeliveryWay == DeliveryWay.BySelected)
                        {
                            if (nd.HisToNodes.Count == 0)
                                infoAccept = "<font color=red>@可执行的人员是用户在表单里的,人员选择器中选择出来的.但是当前节点有多个分支，流程设计错误。</font>";
                            else
                                infoAccept = "@可执行的人员是用户在表单里的,人员选择器中选择出来的.";
                        }

                        if (nd.NodeEmps.Count != 0)
                        {
                            infoAccept = "@节点上指定了如下可执行的如下人员.";
                            foreach (NodeEmp ne in nd.NodeEmps)
                                infoAccept += "@" + ne.FK_Emp + "," + ne.FK_EmpT;
                        }

                        if (nd.HisWork.EnMap.Attrs.Contains("FK_Emp") == false)
                            infoAccept = "可执行的人员是用户在表单里选择出来的。";

                        if (infoAccept == "")
                            msg += "<font color=red>错误：" + nd.Name + ",没有设置岗位。</font>";
                        else
                            msg += infoAccept;
                    }
                    else
                    {
                        msg += "岗位：";
                        foreach (NodeStation st in nd.NodeStations)
                        {
                            msg += st.FK_StationT + "、";
                            rpt += st.FK_StationT + "、";
                        }

                        emps.RetrieveInSQL("select FK_Emp from Port_Empstation WHERE fk_station in (select fk_station from WF_NodeStation WHERE FK_Node=" + nd.NodeID + " )");
                        if (emps.Count == 0)
                        {
                            msg += "<font color=red>岗位人员设置错误：虽然您设置了岗位，但是岗位上没有相关人员执行，流程到此步骤不能正常运行，请在用户维护里维护岗位。</font>";
                        }
                        else
                        {
                            msg += "本节点可执行的工作人员如下";
                            foreach (Emp emp in emps)
                            {
                                msg += emp.No + emp.Name + "、";
                            }
                        }
                    }

                    msg += "<br>";
                    //对单据进行检查。
                    BillTemplates Bills = nd.BillTemplates;
                    if (Bills.Count == 0)
                    {
                        msg += "";
                    }
                    else
                    {
                        msg += "单据";
                        rpt += "<br>单据：";
                        foreach (BillTemplate Bill in Bills)
                        {
                            msg += Bill.Name + "、";
                            rpt += Bill.Name + "、";
                        }
                    }
                    msg += "<br>";
                    msg += "<br>";


                    // 节点完成条件的定义.
                    Conds conds = new Conds(CondType.Node, nd.NodeID, 1, null);
                    if (conds.Count == 0)
                    {
                        //msg += " 。";
                    }
                    else
                    {
                        msg += " 方向条件:";
                        rpt += "<br>方向条件：";
                        foreach (Cond cond in conds)
                        {
                            Node ndOfCond = new Node();
                            ndOfCond.NodeID = ndOfCond.NodeID;
                            if (ndOfCond.RetrieveFromDBSources() == 0)
                            {
                                //msg += "<font color=red>设定的方向条件的节点已经被删除了，所以系统自动删除了这个条件。</font>";
                                //cond.Delete();
                                continue;
                            }

                            try
                            {
                                if (cond.AttrKey.Length < 2)
                                    continue;
                                if (ndOfCond.HisWork.EnMap.Attrs.Contains(cond.AttrKey) == false)
                                    throw new Exception("属性:" + cond.AttrKey + " , " + cond.AttrName + " 不存在。");
                            }
                            catch (Exception ex)
                            {
                                msg += "<font color=red>" + ex.Message + "</font>";
                                ndOfCond.Delete();
                            }
                            msg += cond.AttrKey + cond.AttrName + cond.OperatorValue + "、";
                            rpt += cond.AttrKey + cond.AttrName + cond.OperatorValue + "、";
                        }
                    }
                    msg += "<br>";

                    #region 检查是否有方向条件
                    //if (nd.HisToNodes.Count >= 2
                    //    && (nd.HisRunModel == RunModel.Ordinary || nd.HisRunModel == RunModel.SubThread))
                    //{
                    //    foreach (Node cND in nd.HisToNodes)
                    //    {
                    //        string sql = "SELECT COUNT(*) FROM WF_Cond WHERE NodeID='" + nd.NodeID + "' AND ToNodeID='" + cND.NodeID + "'";
                    //        if (DBAccess.RunSQLReturnValInt(sql) == 0)
                    //        {
                    //            msg += "<br><font color=red>@从节点{" + nd.Name + "}到节点{" + cND.Name + "},没有设置方向条件，请在方向线上点右键设置方向条件。</font>";
                    //        }
                    //    }
                    //}
                    #endregion 检查是否有方向条件

                }
                #endregion

                #region 检查焦点字段设置是否还有效
                foreach (Node nd in nds)
                {
                    if (nd.FocusField.Trim() == "")
                        continue;

                    if (nd.HisWork.EnMap.Attrs.Contains(nd.FocusField) == false)
                        msg += "<font color=red><br>@焦点字段（" + nd.FocusField + "）在节点(step:" + nd.Step + " 名称:" + nd.Name + ")属性里的设置已无效，表单里不存在该字段。</font>";

                    if (this.IsMD5)
                    {
                        if (nd.HisWork.EnMap.Attrs.Contains(WorkAttr.MD5) == false)
                            nd.RepareMap();
                    }
                }
                #endregion

                string sqls = "UPDATE WF_Node SET IsCCFlow=0";
                sqls += "@UPDATE WF_Node  SET IsCCFlow=1 WHERE NodeID IN (SELECT NODEID FROM WF_Cond a WHERE a.NodeID= NodeID AND CondType=1 )";
                BP.DA.DBAccess.RunSQLs(sqls);

                msg += "<br><a href='/DataUser/FlowDesc/" + this.Name + ".htm' target=_s>设计报告</a>";
                msg += "<hr><b> </b> &nbsp; <br>" + DataType.CurrentDataTime + "<br><br><br>";
                rpt += "\r\n</td></tr>\r\n</table>\r\n</body>\r\n</html>";

                string path = BP.SystemConfig.PathOfDataUser + "\\FlowDesc\\" + this.Name + "\\";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);

                try
                {
                    DataType.SaveAsFile(path + this.Name + "_设计报告.htm", rpt);
                }
                catch
                {
                }

                msg += "<hr>开始检查基础数据是否完整<hr>";
                emps = new Emps();
                emps.RetrieveAllFromDBSource();

                CheckRpt();

                DBAccess.RunSQL("DELETE FROM WF_NodeEmp WHERE FK_Emp  not in (select no from port_emp)");
                DBAccess.RunSQL("DELETE FROM WF_Emp WHERE NO not in (select no from port_emp )");
                try
                {
                    DBAccess.RunSQL("UPDATE WF_Emp set Name=(select name from port_emp where port_emp.no=wf_emp.no),FK_Dept=(select fk_dept from port_emp where port_emp.no=wf_emp.no)");
                }
                catch
                {

                }

                Node.CheckFlow(this);
                return msg;
            }
            catch (Exception ex)
            {
                throw new Exception("@检查流程错误:" + ex.Message+" @"+ex.StackTrace);
            }
        }

        #region 产生数据模板。
        public void GenerHisHtmlDocRpt()
        {
            string path = SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\" + this.No + "." + this.Name + "\\";
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            Nodes nds = this.HisNodes;

            string msg = "";

            #region 产生表单列表
            //产生一个列表.
            msg = "<html>";
            msg += "\r\n<title>" + this.Name + "表单模板</title>";
            msg += "\r\n<meta http-equiv=Content-Type content='text/html; charset=utf-8'>";
            msg += "\r\n<body>";
            msg += "\r\n<h1>" + this.Name + "</h1>";

            msg += "\r\n<h3>驰骋工作流引擎,工作流程管理系统自动生成，更多请到<a href='http://doc.ccFlow.org' target=_blank >流程模板交流网，免费下载。</a>联系:QQ:793719823,Tel:18660153393</h3>";
            msg += "\r\n<hr>";

            msg += "\r\n<h3><a href='" + this.Name + ".xml' target=_blank>ccflow可识别的(" + this.Name + ")流程模板(xml格式)</a> </h3>";

            //  <a href='" + this.Name + "_设计报告.htm' target=_blank>" + this.Name + "设计报告</a> - 
            // DataType.SaveAsFile(path + this.Name + "_设计报告.htm", rpt);

            msg += "\r\n<table border=1>";
            foreach (Node nd in nds)
            {
                msg += "\r\n<tr>";
                msg += "\r\n<TD><a href='" + nd.FlowName + "_" + nd.Name + ".doc' target=_blank >" + nd.Name + "</a></TD>";
                msg += "\r\n<TD><a href='" + nd.FlowName + "_" + nd.Name + ".pdf' target=_blank >" + nd.Name + "</a></TD>";
                msg += "\r\n<TD><a href='" + nd.FlowName + "_" + nd.Name + ".htm' target=_blank >" + nd.Name + "</a></TD>";
                msg += "\r\n</tr>";
            }
            msg += "\r\n</table>";

            msg += "\r\n<img src='./../" + this.No + ".gif' alt='" + this.Name + "流程图' border=0 />";

            msg += "\r\n<hr>";
            msg += "\r\n<h3>特别说明：本流程所有表单模板有ccflow及ccflow的客户设计者辛苦劳动所得，所有转载、引用、使用请注明出处，并且征得ccflow同意。</h3>";

            msg += "\r\n<ul>";
            msg += "\r\n<li><a href='http://ccFlow.org' target=_blank >驰骋工作流引擎官方网站 http://ccFlow.org </a></li>";
            msg += "\r\n<li><a href='http://doc.ccFlow.org' target=_blank >标准流程模板网 http://doc.ccFlow.org </a></li>";
            msg += "\r\n</ul>";

            msg += "\r\n</body>";
            msg += "\r\n</html>";

            DataType.SaveAsFile(path + "index.htm", msg);
            #endregion 产生表单列表

            foreach (Node nd in nds)
            {
                Work wk = nd.HisWork;
                this.GenerWorkTempleteDoc(nd.HisWork, "ND" + nd.NodeID, nd); // 生成word, pdf..
                msg = "<html>";
                msg += "\r\n<title>" + this.Name + ",表单模板</Title>";
                msg += "\r\n<meta http-equiv=Content-Type content='text/html; charset=utf-8'>";
                msg += "\r\n<body>";

                msg += "\r\n<h1>" + this.Name + " - " + nd.Name + "</h1>";

                msg += "\r\n<h3>返回：<a href='Index.htm' >" + this.Name + "</a>，此模板有ccflow自动生成，更多的单据模板 <a href='http://doc.ccFlow.org' target=_blank >驰骋流程模板网免费下载</a>。</h3>";
                msg += "\r\n<hr>";

                msg += this.GenerWorkTempleteHtml(nd.HisWork, "ND" + nd.NodeID);

                msg += "\r\n<hr>";
                msg += "\r\n<h3>特别说明：本流程所有表单模板有ccflow及ccflow的客户设计者辛苦劳动所得，所有转载、引用、使用请注明出处，并且征得ccflow同意。</h3>";
                msg += "\r\n<ul>";
                msg += "\r\n<li><a href='http://ccFlow.org' target=_blank >驰骋工作流引擎官方网站 http://ccFlow.org </a></li>";
                msg += "\r\n<li><a href='http://doc.ccFlow.org' target=_blank >标准流程模板网 http://doc.ccFlow.org </a></li>";
                msg += "\r\n</ul>";
                msg += "\r\n</body>";
                msg += "\r\n</html>";
                DataType.SaveAsFile(path + this.Name + "_" + nd.Name + ".htm", msg);
            }
        }
        /// <summary>
        /// 生成流程模板
        /// </summary>
        /// <returns></returns>
        public string GenerFlowXmlTemplete()
        {
            string path = SystemConfig.PathOfDataUser + @"\FlowDesc\" + this.No + "." + this.Name + "\\";
            return this.GenerFlowXmlTemplete(path);
        }
        /// <summary>
        /// 生成流程模板
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GenerFlowXmlTemplete(string path)
        {
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            // 把所有的数据都存储在这里。
            DataSet ds = new DataSet();

            // 流程信息。
            string sql = "SELECT * FROM WF_Flow WHERE No='" + this.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Flow";
            ds.Tables.Add(dt);

            // 节点信息
            sql = "SELECT * FROM WF_Node WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Node";
            ds.Tables.Add(dt);

            // 文书信息
            BillTemplates tmps = new BillTemplates(this.No);
            string pks = "";
            foreach (BillTemplate tmp in tmps)
            {
                try
                {
                    System.IO.File.Copy(SystemConfig.PathOfDataUser + @"\CyclostyleFile\" + tmp.No + ".rtf", path + "\\" + tmp.No + ".rtf", true);
                }
                catch
                {
                    pks += "@" + tmp.PKVal;
                    tmp.Delete();
                }
            }
            tmps.Remove(pks);
            ds.Tables.Add(tmps.ToDataTableField("WF_BillTemplate"));

            // 条件信息
            sql = "SELECT * FROM WF_Cond WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Cond";
            ds.Tables.Add(dt);

            // 转向规则.
            sql = "SELECT * FROM WF_TurnTo WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_TurnTo";
            ds.Tables.Add(dt);

            // 节点与表单绑定.
            sql = "SELECT * FROM WF_FrmNode WHERE FK_Flow='" + this.No + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_FrmNode";
            ds.Tables.Add(dt);

            // 方向
            string sqlin = "SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "'";
            sql = "SELECT * from WF_Direction where Node IN (" + sqlin + ") OR ToNode In (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_Direction";
            ds.Tables.Add(dt);

            //// 应用设置 FAppSet
            //sql = "SELECT * FROM WF_FAppSet WHERE FK_Flow='" + this.No + "'";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "WF_FAppSet";
            //ds.Tables.Add(dt);

            // 流程标签.
            LabNotes labs = new LabNotes(this.No);
            ds.Tables.Add(labs.ToDataTableField("WF_LabNote"));

            // 消息监听.
            Listens lts = new Listens(this.No);
            ds.Tables.Add(lts.ToDataTableField("WF_Listen"));

            // 可退回的节点。
            sql = "SELECT * FROM WF_NodeReturn WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeReturn";
            ds.Tables.Add(dt);


            // 节点与部门。
            sql = "SELECT * FROM WF_NodeDept WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeDept";
            ds.Tables.Add(dt);


            // 节点与岗位权限。
            sql = "SELECT * FROM WF_NodeStation WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeStation";
            ds.Tables.Add(dt);

            // 节点与人员。
            sql = "SELECT * FROM WF_NodeEmp WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_NodeEmp";
            ds.Tables.Add(dt);


            // 抄送人员。
            sql = "SELECT * FROM WF_CCEmp WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCEmp";
            ds.Tables.Add(dt);

            // 抄送部门。
            sql = "SELECT * FROM WF_CCDept WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCDept";
            ds.Tables.Add(dt);

            // 抄送部门。
            sql = "SELECT * FROM WF_CCStation WHERE FK_Node IN (" + sqlin + ")";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "WF_CCStation";
            ds.Tables.Add(dt);

            //// 流程报表。
            //WFRpts rpts = new WFRpts(this.No);
            //// rpts.SaveToXml(path + "WFRpts.xml");
            //ds.Tables.Add(rpts.ToDataTableField("WF_Rpt"));

            //// 流程报表属性
            //RptAttrs rptAttrs = new RptAttrs();
            //rptAttrs.RetrieveAll();
            //ds.Tables.Add(rptAttrs.ToDataTableField("RptAttrs"));

            //// 流程报表访问权限。
            //RptStations rptStations = new RptStations(this.No);
            //rptStations.RetrieveAll();
            ////  rptStations.SaveToXml(path + "RptStations.xml");
            //ds.Tables.Add(rptStations.ToDataTableField("RptStations"));

            //// 流程报表人员访问权限。
            //RptEmps rptEmps = new RptEmps(this.No);
            //rptEmps.RetrieveAll();

            // rptEmps.SaveToXml(path + "RptEmps.xml");
            // ds.Tables.Add(rptEmps.ToDataTableField("RptEmps"));

            int flowID = int.Parse(this.No);
            sql = "SELECT * FROM Sys_MapData WHERE No LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);


            // Sys_MapAttr.
            sql = "SELECT * FROM Sys_MapAttr WHERE  FK_MapData LIKE 'ND" + flowID + "%' ORDER BY FK_MapData,Idx";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            ds.Tables.Add(dt);

            // Sys_EnumMain
            sql = "SELECT * FROM Sys_EnumMain WHERE No IN (SELECT KeyOfEn from Sys_MapAttr WHERE FK_MapData LIKE 'ND" + flowID + "%' )";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_EnumMain";
            ds.Tables.Add(dt);

            // Sys_Enum
            sql = "SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT No FROM Sys_EnumMain WHERE No IN (SELECT KeyOfEn from Sys_MapAttr WHERE FK_MapData LIKE 'ND" + flowID + "%' ) )";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_Enum";
            ds.Tables.Add(dt);

            // Sys_MapDtl
            sql = "SELECT * FROM Sys_MapDtl WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapDtl";
            ds.Tables.Add(dt);

            // Sys_MapExt
            sql = "SELECT * FROM Sys_MapExt WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapExt";
            ds.Tables.Add(dt);

            // Sys_GroupField
            sql = "SELECT * FROM Sys_GroupField WHERE EnName LIKE 'ND" + flowID + "%' ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_GroupField";
            ds.Tables.Add(dt);

            // Sys_MapFrame
            sql = "SELECT * FROM Sys_MapFrame WHERE FK_MapData LIKE 'ND" + flowID + "%' ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapFrame";
            ds.Tables.Add(dt);

            // Sys_MapM2M
            sql = "SELECT * FROM Sys_MapM2M WHERE FK_MapData LIKE 'ND" + flowID + "%' ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapM2M";
            ds.Tables.Add(dt);

            // Sys_FrmLine.
            sql = "SELECT * FROM Sys_FrmLine WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLine";
            ds.Tables.Add(dt);

            // Sys_FrmLab.
            sql = "SELECT * FROM Sys_FrmLab WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLab";
            ds.Tables.Add(dt);

            // Sys_FrmEle.
            sql = "SELECT * FROM Sys_FrmEle WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmEle";
            ds.Tables.Add(dt);

            // Sys_FrmLink.
            sql = "SELECT * FROM Sys_FrmLink WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmLink";
            ds.Tables.Add(dt);

            // Sys_FrmRB.
            sql = "SELECT * FROM Sys_FrmRB WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmRB";
            ds.Tables.Add(dt);

            // Sys_FrmImgAth.
            sql = "SELECT * FROM Sys_FrmImgAth WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmImgAth";
            ds.Tables.Add(dt);

            // Sys_FrmImg.
            sql = "SELECT * FROM Sys_FrmImg WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmImg";
            ds.Tables.Add(dt);

            // Sys_FrmAttachment.
            sql = "SELECT * FROM Sys_FrmAttachment WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmAttachment";
            ds.Tables.Add(dt);

            // Sys_FrmEvent.
            sql = "SELECT * FROM Sys_FrmEvent WHERE  FK_MapData LIKE 'ND" + flowID + "%'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FrmEvent";
            ds.Tables.Add(dt);

            ds.WriteXml(path + "\\" + this.Name + ".xml");
            return path;
        }
        /// <summary>
        /// 生成 word 文件
        /// </summary>
        private void GenerWorkTempleteDoc(Entity en, string enName, Node nd)
        {
            string tempFilePath = SystemConfig.PathOfWorkDir + @"\VisualFlow\DataUser\FlowDesc\" + this.No + "." + this.Name + "\\";
            if (System.IO.Directory.Exists(tempFilePath) == false)
                System.IO.Directory.CreateDirectory(tempFilePath);
            tempFilePath = tempFilePath + nd.FlowName + "_" + nd.Name + ".doc";
            if (File.Exists(tempFilePath) == true)
                return;

            BP.WF.Glo.KillProcess("WINWORD.EXE");
            try
            {
                RegistryKey delKey = Registry.LocalMachine.OpenSubKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Shared Tools\Text Converters\Import\",
                    true);
                delKey.DeleteValue("MSWord6.wpc");
                delKey.Close();
            }
            catch
            {
            }

            GroupField currGF = new GroupField();
            MapAttrs mattrs = new MapAttrs(enName);
            GroupFields gfs = new GroupFields(enName);
            MapDtls dtls = new MapDtls(enName);
            foreach (MapDtl dtl in dtls)
                dtl.IsUse = false;

            // 计算出来单元格的行数。
            int rowNum = 0;
            foreach (GroupField gf in gfs)
            {
                rowNum++;
                bool isLeft = true;
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.GroupID != gf.OID)
                        continue;

                    if (attr.UIIsLine)
                    {
                        rowNum++;
                        isLeft = true;
                        continue;
                    }

                    if (isLeft == false)
                        rowNum++;
                    isLeft = !isLeft;
                }
            }

            rowNum = rowNum + 2 + dtls.Count;


            // 创建Word文档
            string CheckedInfo = "";
            string message = "";
            Object Nothing = System.Reflection.Missing.Value;
            object filename = tempFilePath;

            Word.Application WordApp = new Word.ApplicationClass();
            Word.Document WordDoc = WordApp.Documents.Add(ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing);

            try
            {
                WordApp.ActiveWindow.View.Type = Word.WdViewType.wdOutlineView;
                WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryHeader;

                #region 增加页眉
                // 添加页眉 插入图片
                string pict = SystemConfig.PathOfDataUser + "log.jpg"; // 图片所在路径
                if (System.IO.File.Exists(pict))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(pict);
                    object LinkToFile = false;
                    object SaveWithDocument = true;
                    object Anchor = WordDoc.Application.Selection.Range;

                    WordDoc.Application.ActiveDocument.InlineShapes.AddPicture(pict, ref  LinkToFile,
                        ref  SaveWithDocument, ref  Anchor);
                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Width = img.Width; // 图片宽度
                    //    WordDoc.Application.ActiveDocument.InlineShapes[1].Height = img.Height; // 图片高度
                }
                WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("[驰骋业务流程管理系统 http://ccFlow.org ] - [" + nd.FlowName + "-" + nd.Name + "模板]");
                WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight; // 设置右对齐
                WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekMainDocument; // 跳出页眉设置
                WordApp.Selection.ParagraphFormat.LineSpacing = 15f; // 设置文档的行间距
                #endregion

                // 移动焦点并换行
                object count = 14;
                object WdLine = Word.WdUnits.wdLine; // 换一行;
                WordApp.Selection.MoveDown(ref  WdLine, ref  count, ref  Nothing); // 移动焦点
                WordApp.Selection.TypeParagraph(); // 插入段落

                // 文档中创建表格
                Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, rowNum, 4, ref  Nothing, ref  Nothing);

                // 设置表格样式
                newTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleThickThinLargeGap;
                newTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                newTable.Columns[1].Width = 100f;
                newTable.Columns[2].Width = 100f;
                newTable.Columns[3].Width = 100f;
                newTable.Columns[4].Width = 100f;

                // 填充表格内容
                newTable.Cell(1, 1).Range.Text = nd.Name + "表单";
                newTable.Cell(1, 1).Range.Bold = 2; // 设置单元格中字体为粗体

                // 合并单元格
                newTable.Cell(1, 1).Merge(newTable.Cell(1, 4));
                WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter; // 垂直居中
                WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter; // 水平居中

                int groupIdx = 1;
                foreach (GroupField gf in gfs)
                {
                    groupIdx++;
                    // 填充表格内容
                    newTable.Cell(groupIdx, 1).Range.Text = gf.Lab;
                    newTable.Cell(groupIdx, 1).Range.Font.Color = Word.WdColor.wdColorDarkBlue; // 设置单元格内字体颜色
                    newTable.Cell(groupIdx, 1).Shading.BackgroundPatternColor = Word.WdColor.wdColorGray25;
                    // 合并单元格
                    newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
                    WordApp.Selection.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    groupIdx++;

                    bool isLeft = true;
                    bool isColumns2 = false;
                    int currColumnIndex = 0;
                    foreach (MapAttr attr in mattrs)
                    {
                        if (attr.UIVisible == false)
                            continue;

                        if (attr.GroupID != gf.OID)
                            continue;

                        if (newTable.Rows.Count < groupIdx)
                            continue;

                        #region 增加从表
                        foreach (MapDtl dtl in dtls)
                        {
                            if (dtl.IsUse)
                                continue;

                            if (dtl.RowIdx != groupIdx - 3)
                                continue;


                            if (gf.OID != dtl.GroupID)
                                continue;

                            newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
                            newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));

                            Attrs dtlAttrs = dtl.GenerMap().Attrs;
                            string strs = "";
                            int colNum = 0;
                            foreach (Attr attrDtl in dtlAttrs)
                            {
                                if (attrDtl.UIVisible == false)
                                    continue;
                                strs += "|" + attrDtl.Desc;
                                colNum++;
                            }


                            newTable.Cell(groupIdx, 1).Select();
                            WordApp.Selection.Delete(ref Nothing, ref Nothing);
                            Word.Table newTableDtl = WordDoc.Tables.Add(WordApp.Selection.Range, dtl.RowsOfList, colNum,
                                ref Nothing, ref Nothing);



                            newTableDtl.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            newTableDtl.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                            int colIdx = 1;
                            foreach (Attr attrDtl in dtlAttrs)
                            {
                                if (attrDtl.UIVisible == false)
                                    continue;
                                newTableDtl.Cell(1, colIdx).Range.Text = attrDtl.Desc;
                                colIdx++;
                            }

                            for (int i = 2; i <= dtl.RowsOfList; i++)
                            {
                                newTableDtl.Cell(i, 1).Range.Text = "";
                            }
                            groupIdx++;
                        }
                        #endregion 增加从表


                        if (attr.UIIsLine)
                        {
                            currColumnIndex = 0;
                            isLeft = true;
                            if (attr.IsBigDoc)
                            {
                                newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
                                newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 4));
                                newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n";
                            }
                            else
                            {
                                newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 4));
                                newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
                            }
                            groupIdx++;
                            continue;
                        }
                        else
                        {
                            if (attr.IsBigDoc)
                            {
                                if (currColumnIndex == 2)
                                {
                                    currColumnIndex = 0;
                                }

                                newTable.Rows[groupIdx].SetHeight(100f, Word.WdRowHeightRule.wdRowHeightAtLeast);
                                if (currColumnIndex == 0)
                                {
                                    newTable.Cell(groupIdx, 1).Merge(newTable.Cell(groupIdx, 2));
                                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name + ":\r\n";
                                    currColumnIndex = 3;
                                    continue;
                                }
                                else if (currColumnIndex == 3)
                                {
                                    newTable.Cell(groupIdx, 2).Merge(newTable.Cell(groupIdx, 3));
                                    newTable.Cell(groupIdx, 2).Range.Text = attr.Name + ":\r\n";
                                    currColumnIndex = 0;
                                    groupIdx++;
                                    continue;
                                }
                                else
                                {
                                    continue;
                                    //throw new Exception("sdsdsdsd");
                                }
                            }
                            else
                            {
                                if (currColumnIndex == 0)
                                {
                                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
                                    currColumnIndex++;
                                    continue;
                                }
                                else if (currColumnIndex == 1)
                                {
                                    newTable.Cell(groupIdx, 3).Range.Text = attr.Name;
                                    currColumnIndex++;
                                    groupIdx++;
                                    continue;
                                }
                                else
                                {
                                    newTable.Cell(groupIdx, 1).Range.Text = attr.Name;
                                    currColumnIndex = 0;
                                    continue;
                                }
                            }
                        }
                    }
                }  //结束循环

                #region 添加页脚
                WordApp.ActiveWindow.View.SeekView = Word.WdSeekView.wdSeekPrimaryFooter;
                WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("模板由ccflow5自动生成，严谨转载。欢迎使用开源,免费的工作流程管理系统 http://ccFlow.org.");
                WordApp.Selection.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                #endregion


                // 文件保存
                WordDoc.SaveAs(ref  filename, ref  Nothing, ref  Nothing, ref  Nothing,
                    ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
                    ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing, ref  Nothing,
                    ref  Nothing, ref  Nothing, ref  Nothing);

                WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
                WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
                try
                {
                    string docFile = filename.ToString();
                    string pdfFile = docFile.Replace(".doc", ".pdf");
                    Glo.Rtf2PDF(docFile, pdfFile);
                }
                catch (Exception ex)
                {
                    Log.DebugWriteInfo("@生成pdf失败." + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.DebugWriteError("@生成word出现错误:" + nd.NodeID + ",@Err:" + ex.Message);
                // throw ex;
                // WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
                WordDoc.Close(ref  Nothing, ref  Nothing, ref  Nothing);
                WordApp.Quit(ref  Nothing, ref  Nothing, ref  Nothing);
            }
        }
        private string GenerWorkTempleteHtml(Entity en, string enName)
        {
            string msg = "";
            msg += "\r\n<table border=1 width='80%' >";
            GroupField currGF = new GroupField();
            MapAttrs mattrs = new MapAttrs(enName);
            GroupFields gfs = new GroupFields(enName);
            MapDtls dtls = new MapDtls(enName);
            foreach (MapDtl dtl in dtls)
                dtl.IsUse = false;
            foreach (GroupField gf in gfs)
            {
                currGF = gf;
                msg += "<TR>";
                msg += "<TD colspan=4 valign='top' align=left bgcolor=Silver ><div style='text-align:left; float:left'>&nbsp;<b>" + gf.Lab + "</b></div><div style='text-align:right; float:right'></div></TD>";
                msg += "</TR>";

                bool isHaveH = false;
                int i = -1;
                int idx = -1;
                bool isLeftNext = true;
                int rowIdx = 0;
                foreach (MapAttr attr in mattrs)
                {

                    if (attr.GroupID != gf.OID)
                    {
                        if (gf.Idx == 0 && attr.GroupID == 0)
                        {

                        }
                        else
                            continue;
                    }

                    if (attr.HisAttr.IsRefAttr || attr.UIVisible == false)
                        continue;

                    if (isLeftNext == true)
                        this.InsertObjects(dtls, gfs, true, rowIdx, currGF);

                    #region 加入字段
                    // 显示的顺序号.
                    idx++;
                    if (attr.IsBigDoc && attr.UIIsLine)
                    {
                        if (isLeftNext == false)
                        {
                            msg += "\r\n<TD>&nbsp;</TD>";
                            msg += "\r\n<TD>&nbsp;</TD>";
                            msg += "\r\n</TR>";
                            rowIdx++;
                        }
                        rowIdx++;
                        msg += "\r\n<TR>";
                        if (attr.UIIsEnable)
                            msg += "\r\n<TD  colspan=4 width='100%' valign=top align=left>" + attr.Name;
                        else
                            msg += "\r\n<TD  colspan=4 width='100%' valign=top class=TBReadonly>" + attr.Name;

                        msg += "";
                        msg += "\r\n</TD>";
                        msg += "\r\n</TR>";

                        rowIdx++;
                        isLeftNext = true;
                        continue;
                    }

                    if (attr.IsBigDoc)
                    {
                        if (isLeftNext)
                        {
                            rowIdx++;
                            msg += "<TR>";
                        }

                        msg += "<TD colspan=2>" + attr.Name;
                        //TB mytbLine = new TB();
                        //mytbLine.ID = "TB_" + attr.KeyOfEn;
                        //mytbLine.TextMode = TextBoxMode.MultiLine;
                        //mytbLine.Rows = 8;
                        //mytbLine.Columns = 30;
                        //mytbLine.Attributes["style"] = "width:100%;padding: 0px;margin: 0px;";
                        //mytbLine.Text = en.GetValStrByKey(attr.KeyOfEn);
                        //mytbLine.Enabled = attr.UIIsEnable;
                        //if (mytbLine.Enabled == false)
                        //    mytbLine.Attributes["class"] = "TBReadonly";

                        msg += "\r\n";
                        msg += "\r\n</TD>";

                        if (isLeftNext == false)
                        {
                            msg += "\r\n</TR>";
                            rowIdx++;
                        }

                        isLeftNext = !isLeftNext;
                        continue;
                    }

                    //计算 colspanOfCtl .
                    int colspanOfCtl = 1;
                    if (attr.UIIsLine)
                        colspanOfCtl = 3;

                    if (attr.UIIsLine)
                    {
                        if (isLeftNext == false)
                        {
                            msg += "\r\n<TD>&nbsp;</TD>";
                            msg += "\r\n<TD>&nbsp;</TD>";
                            msg += "\r\n</TR>";
                            rowIdx++;
                        }
                        isLeftNext = true;
                    }

                    if (isLeftNext)
                    {
                        rowIdx++;
                        msg += "\r\n<TR>";
                    }

                    #region add contrals.
                    switch (attr.LGType)
                    {
                        case FieldTypeS.Normal:
                            switch (attr.MyDataType)
                            {
                                case DataType.AppString:
                                case DataType.AppDate:
                                case DataType.AppDateTime:
                                    msg += "<TD>" + attr.Name + "</TD>";
                                    msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                                    break;
                                case DataType.AppBoolean:
                                    msg += "<TD></TD>";
                                    msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                                    break;
                                case DataType.AppDouble:
                                case DataType.AppFloat:
                                case DataType.AppInt:
                                    msg += "<TD>" + attr.Name + "</TD>";
                                    msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                                    break;
                                case DataType.AppMoney:
                                case DataType.AppRate:
                                    msg += "<TD>" + attr.Name + "</TD>";
                                    msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case FieldTypeS.Enum:
                            msg += "<TD>" + attr.Name + "</TD>";
                            msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                            break;

                            //this.AddTDDesc(attr.Name);
                            //DDL ddle = new DDL();
                            //ddle.ID = "DDL_" + attr.KeyOfEn;
                            //ddle.BindSysEnum(attr.KeyOfEn);
                            //ddle.SetSelectItem(en.GetValStrByKey(attr.KeyOfEn));
                            //ddle.Enabled = attr.UIIsEnable;
                            //this.AddTD("colspan=" + colspanOfCtl, ddle);
                            break;
                        case FieldTypeS.FK:
                            msg += "<TD>" + attr.Name + "</TD>";
                            msg += "<TD colspan=" + colspanOfCtl + ">&nbsp;</TD>";
                            break;
                        default:
                            break;
                    }
                    #endregion add contrals.

                    #endregion 加入字段

                    #region 尾后处理。

                    if (colspanOfCtl == 3)
                    {
                        isLeftNext = true;
                        msg += "\r\n</TR>";
                        continue;
                    }

                    if (isLeftNext == false)
                    {
                        isLeftNext = true;
                        msg += "\r\n</TR>";
                        continue;
                    }
                    isLeftNext = false;
                    #endregion add contrals.

                }
                // 最后处理补充上它。
                if (isLeftNext == false)
                {
                    msg += "\r\n<TD>&nbsp;</TD>";
                    msg += "\r\n<TD>&nbsp;</TD>";
                    msg += "\r\n</TR>";
                }
                this.InsertObjects(dtls, gfs, false, rowIdx, currGF);
            }
            msg += "\r\n</Table>";
            msg += "\r\n</div>";
            return msg;
        }
        private void InsertObjects(MapDtls dtls, GroupFields gfs, bool isJudgeRowIdx, int rowIdx, GroupField currGF)
        {
            foreach (MapDtl dtl in dtls)
            {
                if (dtl.IsUse)
                    continue;

                if (isJudgeRowIdx)
                {
                    if (dtl.RowIdx != rowIdx)
                        continue;
                }

                if (dtl.GroupID == 0 && rowIdx == 0)
                {
                    dtl.GroupID = currGF.OID;
                    dtl.RowIdx = 0;
                    dtl.Update();
                }
                else if (dtl.GroupID == currGF.OID)
                {

                }
                else
                {
                    continue;
                }
                dtl.IsUse = true;
                rowIdx++;
            }
        }
        #endregion 产生数据模板。
        /// <summary>
        /// 重新设置Rpt表
        /// </summary>
        public void CheckRptOfReset()
        {
            string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
            string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "'";
            DBAccess.RunSQL(sql);

            sql = "DELETE FROM Sys_MapData WHERE No='" + fk_mapData + "'";
            DBAccess.RunSQL(sql);
            this.CheckRpt();
        }
        /// <summary>
        ///  重新设置Rpt表
        /// </summary>
        public void CheckRpt()
        {
            try
            {
                //创建 track 表.
                Track.CreateTrackTable(this.No);

                Nodes nds = this.HisNodes;

                CheckRpt(nds);

                CheckRptDtl(nds);

                this.CheckRptView(nds);
            }
            catch(Exception ex)
            {
                throw new Exception("@执行检查出现错误:"+ex.Message+" @"+ex.StackTrace);
            }

        }
        public string DoReloadRptData()
        {
            this.CheckRpt();

            // 检查报表数据是否丢失。

            if (this.HisDataStoreModel != DataStoreModel.ByCCFlow)
                return "@流程" + this.No + this.Name + "的数据存储非轨迹模式不能重新生成.";

            DBAccess.RunSQL("DELETE FROM " + this.PTable);

            string sql = "SELECT OID FROM ND" + int.Parse(this.No) + "01 WHERE  OID NOT IN (SELECT OID FROM  "+this.PTable+" ) ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            this.CheckRptData(this.HisNodes, dt);

            return "@共有:" + dt.Rows.Count + "条(" + this.Name + ")数据被装载成功。";
        }
        /// <summary>
        /// 检查与修复报表数据
        /// </summary>
        /// <param name="nds"></param>
        /// <param name="dt"></param>
        private string CheckRptData(Nodes nds, DataTable dt)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(this.No) + "Rpt");
            string err = "";
            foreach (DataRow dr in dt.Rows)
            {
                rpt.ResetDefaultVal();
                int oid = int.Parse(dr[0].ToString());
                rpt.SetValByKey("OID", oid);
                Work startWork = null;
                Work endWK = null;
                string flowEmps = "";
                foreach (Node nd in nds)
                {
                    try
                    {
                        Work wk = nd.HisWork;
                        wk.OID = oid;
                        if (wk.RetrieveFromDBSources() == 0)
                            continue;

                        rpt.Copy(wk);
                        if (nd.NodeID == int.Parse(this.No + "01"))
                            startWork = wk;

                        try
                        {
                            if (flowEmps.Contains("@" + wk.Rec + ","))
                                continue;

                            flowEmps += "@" + wk.Rec + "," + wk.RecOfEmp.Name;
                        }
                        catch
                        {
                        }
                        endWK = wk;
                    }
                    catch (Exception ex)
                    {
                        err += ex.Message;
                    }
                }

                if (startWork == null || endWK == null)
                    continue;

                rpt.SetValByKey("OID", oid);
                rpt.FK_NY = startWork.GetValStrByKey("RDT").Substring(0, 7);
                rpt.FK_Dept = startWork.GetValStrByKey("FK_Dept");
                if (string.IsNullOrEmpty(rpt.FK_Dept))
                {
                    string fk_dept = DBAccess.RunSQLReturnString("SELECT FK_Dept FROM Port_Emp WHERE No='" + startWork.Rec + "'");
                    rpt.FK_Dept = fk_dept;

                    startWork.SetValByKey("FK_Dept", fk_dept);
                    startWork.Update();
                }
                rpt.Title = startWork.GetValStrByKey("Title");
                string wfState = DBAccess.RunSQLReturnStringIsNull("SELECT WFState FROM WF_GenerWorkFlow WHERE WorkID=" + oid, "1");
                rpt.WFState = (WFState) int.Parse(wfState);
                rpt.FlowStarter = startWork.Rec;
                rpt.FlowStartRDT = startWork.RDT;
                rpt.FID = startWork.GetValIntByKey("FID");
                rpt.FlowEmps = flowEmps;
                rpt.FlowEnder = endWK.Rec;
                rpt.FlowEnderRDT = endWK.RDT;
                rpt.FlowEndNode = endWK.NodeID;
                rpt.MyNum = 1;

                //修复标题字段。
                WorkNode wn = new WorkNode(startWork, this.HisStartNode);
                rpt.Title = WorkNode.GenerTitle(this, startWork);
                try
                {
                    TimeSpan ts = endWK.RDT_DateTime - startWork.RDT_DateTime;
                    rpt.FlowDaySpan = ts.Days;
                }
                catch
                {
                }
                rpt.InsertAsOID(rpt.OID);
            } // 结束循环。
            return err;
        }
        /// <summary>
        /// 生成明细报表信息
        /// </summary>
        /// <param name="nds"></param>
        private void CheckRptDtl(Nodes nds)
        {
            MapDtls dtlsDtl = new MapDtls();
            dtlsDtl.Retrieve(MapDtlAttr.FK_MapData, "ND" + int.Parse(this.No) + "Rpt");
            foreach (MapDtl dtl in dtlsDtl)
            {
                dtl.Delete();
            }

            //  dtlsDtl.Delete(MapDtlAttr.FK_MapData, "ND" + int.Parse(this.No) + "Rpt");
            foreach (Node nd in nds)
            {
                if (nd.IsEndNode == false)
                    continue;

                // 取出来从表.
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                if (dtls.Count == 0)
                    continue;

                string rpt = "ND" + int.Parse(this.No) + "Rpt";
                int i = 0;
                foreach (MapDtl dtl in dtls)
                {
                    i++;
                    string rptDtlNo = "ND" + int.Parse(this.No) + "RptDtl" + i.ToString();
                    MapDtl rtpDtl = new MapDtl();
                    rtpDtl.No = rptDtlNo;
                    if (rtpDtl.RetrieveFromDBSources() == 0)
                    {
                        rtpDtl.Copy(dtl);
                        rtpDtl.No = rptDtlNo;
                        rtpDtl.FK_MapData = rpt;
                        rtpDtl.PTable = rptDtlNo;
                        rtpDtl.GroupID = -1;
                        rtpDtl.Insert();
                    }

                    

                    MapAttrs attrsRptDtl = new MapAttrs(rptDtlNo);
                    MapAttrs attrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in attrs)
                    {
                        if (attrsRptDtl.Contains(MapAttrAttr.KeyOfEn, attr.KeyOfEn) == true)
                            continue;

                        MapAttr attrN = new MapAttr();
                        attrN.Copy(attr);
                        attrN.FK_MapData = rptDtlNo;
                        switch (attr.KeyOfEn)
                        {
                            case "FK_NY":
                                attrN.UIVisible = true;
                                attrN.IDX = 100;
                                attrN.UIWidth = 60;
                                break;
                            case "RDT":
                                attrN.UIVisible = true;
                                attrN.IDX = 100;
                                attrN.UIWidth = 60;
                                break;
                            case "Rec":
                                attrN.UIVisible = true;
                                attrN.IDX = 100;
                                attrN.UIWidth = 60;
                                break;
                            default:
                                break;
                        }

                        attrN.Save();
                    }

                    GEDtl geDtl = new GEDtl(rptDtlNo);
                    geDtl.CheckPhysicsTable();
                }
            }
        }
        /// <summary>
        /// 产生所有节点视图
        /// </summary>
        /// <param name="nds"></param>
        private void CheckRptView(Nodes nds)
        {
            if (this.HisDataStoreModel == DataStoreModel.SpecTable)
                return;
            string viewName = "V" + this.No;
            string sql = "CREATE VIEW " + viewName + " ";
            sql += "/* WorkFlow:" + this.Name + " Date:" + DateTime.Now.ToString("yyyy-MM-dd") + " */ ";
            sql += "\r\n (MyPK,FK_Node,OID,FID,RDT,FK_NY,CDT,Rec,Emps,FK_Dept,MyNum) AS ";
            bool is1 = false;
            foreach (Node nd in nds)
            {
                nd.HisWork.CheckPhysicsTable();
                if (is1 == false)
                    is1 = true;
                else
                    sql += "\r\n UNION ";

                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.Informix:
                        sql += "\r\n SELECT '" + nd.NodeID + "' || '_'|| OID||'_'|| FID  AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT,SUBSTR(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                    case DBType.MySQL:
                        sql += "\r\n SELECT '" + nd.NodeID + "'+'_'+CHAR(OID)  +'_'+CHAR(FID)  AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT," + BP.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                    default:
                        sql += "\r\n SELECT '" + nd.NodeID + "'+'_'+CAST(OID AS varchar(10)) +'_'+CAST(FID AS VARCHAR(10)) AS MyPK, '" + nd.NodeID + "' AS FK_Node,OID,FID,RDT," + BP.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,7) AS FK_NY,CDT,Rec,Emps,FK_Dept, 1 AS MyNum FROM ND" + nd.NodeID + " ";
                        break;
                }
            }
            if (SystemConfig.AppCenterDBType != DBType.Informix)
                sql += "\r\n GO ";

            try
            {
                if (DBAccess.IsExitsObject(viewName) == true)
                    DBAccess.RunSQL("DROP VIEW " + viewName);
            }
            catch
            {
            }

            try
            {
                DBAccess.RunSQL(sql);
            }
            catch
            {
            }
        }
        private void CheckRpt(Nodes nds)
        {
            string fk_mapData = "ND" + int.Parse(this.No) + "Rpt";
            string flowId = int.Parse(this.No).ToString();

            #region 插入字段。
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.MSSQL:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.Informix:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.MySQL:
                    sql = "SELECT  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " CHAR(NodeID) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                default:
                    sql = "SELECT distinct  KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
            }

            if (SystemConfig.AppCenterDBType == DBType.MySQL)
            {
#warning 没有处理好mysql的删除。
                //DataTable delDt = DBAccess.RunSQLReturnTable(sql);
                //foreach (DataRow dr in delDt.Rows)
                //{
                //    string sql2 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='ND" + flowId + "Rpt' ";
                //    DBAccess.RunSQL(sql2); // 删除不存在的字段.
                //}
            }
            else
            {
                string sql2 = "DELETE FROM Sys_MapAttr WHERE KeyOfEn NOT IN (" + sql + ") AND FK_MapData='ND" + flowId + "Rpt' ";
                DBAccess.RunSQL(sql2); // 删除不存在的字段.
            }


            // 补充上没有字段。
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                case DBType.MySQL:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " CHAR(NodeID) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
                default:
                    sql = "SELECT MyPK, KeyOfEn FROM Sys_MapAttr WHERE FK_MapData IN ( SELECT 'ND' " + SystemConfig.AppCenterDBAddStringStr + " cast(NodeID as varchar(20)) FROM WF_Node WHERE FK_Flow='" + this.No + "')";
                    break;
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='ND" + flowId + "Rpt'";
            DataTable dtExits = DBAccess.RunSQLReturnTable(sql);
            string pks = "@";
            foreach (DataRow dr in dtExits.Rows)
                pks += dr[0] + "@";

            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                if (pks.Contains("@" + dr["KeyOfEn"].ToString() + "@"))
                    continue;
                pks += dr["KeyOfEn"].ToString() + "@";
                BP.Sys.MapAttr ma = new BP.Sys.MapAttr(mypk);
                ma.MyPK = "ND" + flowId + "Rpt_" + ma.KeyOfEn;
                ma.FK_MapData = "ND" + flowId + "Rpt";
                ma.UIIsEnable = false;

                if (ma.DefValReal.Contains("@"))
                {
                    /*如果是一个有变量的参数.*/
                    ma.DefVal = "";
                }

                try
                {
                    ma.Insert();
                }
                catch
                {
                }
            }

            MapAttrs attrs = new MapAttrs(fk_mapData);

            // 创建mapData.
            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + flowId + "Rpt";
            if (md.RetrieveFromDBSources() == 0)
            {
                md.Name = this.Name;
                md.PTable = this.PTable;
                md.Insert();
            }
            else
            {
                md.Name = this.Name;
                md.PTable = this.PTable;
                md.Update();
            }
            #endregion 插入字段。

            #region 补充上流程字段。
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case StartWorkAttr.FK_Dept:
                        attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.UIIsEnable = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case "FK_NY":
                        attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        attr.GroupID = groupID;
                        attr.Update();
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.WFState) == false)
            {
                /* 流程状态 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.WFState;
                attr.Name = "流程状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIBindKey = GERptAttr.WFState;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.Enum;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.IDX = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEmps) == false)
            {
                /* 参与人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEmps; // "FlowEmps";
                attr.Name = "参与人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.IDX = -100;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStarter;
                attr.Name = "发起人"; //  
                attr.MyDataType = DataType.AppString;

                attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;

                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 20;
                attr.IDX = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStartRDT; // "FlowStartRDT";
                attr.Name = "发起时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.IDX = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnder;
                attr.Name = "结束人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 20;
                attr.IDX = -1;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnderRDT; // "FlowStartRDT";
                attr.Name = "结束时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.IDX = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowEndNode) == false)
            {
                /* 结束节点 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEndNode;
                attr.Name = "结束节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.IDX = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.FlowDaySpan) == false)
            {
                /* FlowDaySpan */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowDaySpan; // "FlowStartRDT";
                attr.Name = "跨度(天)";
                attr.MyDataType = DataType.AppMoney;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = false;
                attr.IDX = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PWorkID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PWorkID;
                attr.Name = "父流程WorkID";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.IDX = -101;
                attr.Insert();
            }

            if (attrs.Contains(md.No + "_" + GERptAttr.PFlowNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PFlowNo;
                attr.Name = "父流程流程编号"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 3;
                attr.IDX = -100;
                attr.Insert();
            }


            if (attrs.Contains(md.No + "_MyNum") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = md.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "MyNum";
                attr.Name = "条";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "1";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.IDX = -101;
                attr.Insert();
            }
            #endregion 补充上流程字段。

            #region 为流程字段设置分组。
            try
            {
                string flowInfo = "流程信息";
                GroupField flowGF = new GroupField();
                int num = flowGF.Retrieve(GroupFieldAttr.EnName, fk_mapData, GroupFieldAttr.Lab, "流程信息");
                if (num == 0)
                {
                    flowGF = new GroupField();
                    flowGF.Lab = flowInfo;
                    flowGF.EnName = fk_mapData;
                    flowGF.Idx = -1;
                    flowGF.Insert();
                }
                sql = "UPDATE Sys_MapAttr SET GroupID=" + flowGF.OID + " WHERE  FK_MapData='" + fk_mapData + "'  AND KeyOfEn IN('" + GERptAttr.PFlowNo + "','" + GERptAttr.PWorkID + "','" + GERptAttr.MyNum + "','" + GERptAttr.FK_Dept + "','" + GERptAttr.FK_NY + "','" + GERptAttr.FlowDaySpan + "','" + GERptAttr.FlowEmps + "','" + GERptAttr.FlowEnder + "','" + GERptAttr.FlowEnderRDT + "','" + GERptAttr.FlowEndNode + "','" + GERptAttr.FlowStarter + "','" + GERptAttr.FlowStartRDT + "','" + GERptAttr.WFState + "')";
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
            }
            #endregion 为流程字段设置分组

            GERpt sw = this.HisFlowData;
            sw.CheckPhysicsTable();

            DBAccess.RunSQL("DELETE FROM Sys_GroupField WHERE EnName='" + fk_mapData + "' AND OID NOT IN (SELECT GroupID FROM Sys_MapAttr WHERE FK_MapData = '" + fk_mapData + "')");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='活动时间' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='CDT'");
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Name='参与者' WHERE FK_MapData='ND" + flowId + "Rpt' AND KeyOfEn='Emps'");
        }

        #region 基本属性
        /// <summary>
        /// 是否是MD5加密流程
        /// </summary>
        public bool IsMD5
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsMD5);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsMD5, value);
            }
        }
        /// <summary>
        /// 是否可以发起?
        /// </summary>
        public bool IsCanStart
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsCanStart);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsCanStart, value);
            }
        }
        /// <summary>
        /// 是否可用？
        /// </summary>
        public bool IsOK
        {
            get
            {
                return this.GetValBooleanByKey(FlowAttr.IsOK);
            }
            set
            {
                this.SetValByKey(FlowAttr.IsOK, value);
            }
        }
        /// <summary>
        /// 是否有单据
        /// </summary>
        public int NumOfBill
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.NumOfBill);
            }
            set
            {
                this.SetValByKey(FlowAttr.NumOfBill, value);
            }
        }
        /// <summary>
        /// 标题生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.TitleRole);
            }
            set
            {
                this.SetValByKey(FlowAttr.TitleRole, value);
            }
        }

        /// <summary>
        /// 明细表
        /// </summary>
        public int NumOfDtl
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.NumOfDtl);
            }
            set
            {
                this.SetValByKey(FlowAttr.NumOfDtl, value);
            }
        }
        public decimal AvgDay
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.AvgDay);
            }
            set
            {
                this.SetValByKey(FlowAttr.AvgDay, value);
            }
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.No + "01");
                //return this.GetValIntByKey(FlowAttr.StartNodeID);
            }
        }
        /// <summary>
        /// add 2013-01-01.
        /// 业务主表(默认为NDxxRpt)
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStringByKey(FlowAttr.PTable);
                if (string.IsNullOrEmpty(s))
                    s = "ND" + int.Parse(this.No) + "Rpt";
                return s;
            }
            set
            {
                this.SetValByKey(FlowAttr.PTable, value);
            }
        }
        /// <summary>
        /// 流程类别
        /// </summary>
        public string FK_FlowSort
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.FK_FlowSort);
            }
            set
            {
                this.SetValByKey(FlowAttr.FK_FlowSort, value);
            }
        }
        /// <summary>
        /// 参数
        /// </summary>
        public string Paras
        {
            get
            {
                return this.GetValStringByKey(FlowAttr.Paras);
            }
            set
            {
                this.SetValByKey(FlowAttr.Paras, value);
            }
        }
        /// <summary>
        /// 流程类别名称
        /// </summary>
        public string FK_FlowSortText
        {
            get
            {
                return this.GetValRefTextByKey(FlowAttr.FK_FlowSort);
            }
        }
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
        #endregion

        #region 计算属性
        /// <summary>
        /// 流程类型(大的类型)
        /// </summary>
        public int FlowType_del
        {
            get
            {
                return this.GetValIntByKey(FlowAttr.FlowType);
            }
        }
        /// <summary>
        /// 是否自动流程
        /// </summary>
        public bool IsAutoWorkFlow_del
        {
            get
            {
                return false;
                /*
                if (this.FlowType==1)
                    return true;
                else
                    return false;
                    */
            }
        }
        public string Note
        {
            get
            {
                string s = this.GetValStringByKey("Note");
                if (s.Length == 0)
                {
                    return "无";
                }
                return s;
            }
        }
        public string NoteHtml
        {
            get
            {
                if (this.Note == "无" || this.Note == "")
                    return  "流程设计人员没有编写此流程的帮助信息，请打开设计器-》打开此流程-》设计画布上点击右键-》流程属性-》填写流程帮助信息。";
                else
                    return this.Note;
            }
        }
        /// <summary>
        /// 是否多线程自动流程
        /// </summary>
        public bool IsMutiLineWorkFlow_del
        {
            get
            {
                return false;
                /*
                if (this.FlowType==2 || this.FlowType==1 )
                    return true;
                else
                    return false;
                    */
            }
        }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 应用类型
        /// </summary>
        public FlowAppType HisFlowAppType
        {
            get
            {
                return (FlowAppType)this.GetValIntByKey(FlowAttr.AppType);
            }
            set
            {
                this.SetValByKey(FlowAttr.AppType, (int)value);
            }
        }
        /// <summary>
        /// 流程类型
        /// </summary>
        public FlowType HisFlowType
        {
            get
            {
                return (FlowType)this.GetValIntByKey(FlowAttr.FlowType);
            }
            set
            {
                this.SetValByKey(FlowAttr.FlowType, (int)value);
            }
        }
        
        /// <summary>
        /// 数据存储模式
        /// </summary>
        public DataStoreModel HisDataStoreModel
        {
            get
            {
                return (DataStoreModel)this.GetValIntByKey(FlowAttr.DataStoreModel);
            }
            set
            {
                this.SetValByKey(FlowAttr.DataStoreModel, (int)value);
            }
        }
        /// <summary>
        /// 节点
        /// </summary>
        public Nodes _HisNodes = null;
        /// <summary>
        /// 他的节点集合.
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                if (this._HisNodes == null)
                    _HisNodes = new Nodes(this.No);
                return _HisNodes;
            }
            set
            {
                _HisNodes = value;
            }
        }
        /// <summary>
        /// 他的 Start 节点
        /// </summary>
        public Node HisStartNode
        {
            get
            {

                foreach (Node nd in this.HisNodes)
                {
                    if (nd.IsStartNode)
                        return nd;
                }
                throw new Exception("@没有找到他的开始节点,工作流程[" + this.Name + "]定义错误.");
            }
        }
        /// <summary>
        /// 他的事务类别
        /// </summary>
        public FlowSort HisFlowSort
        {
            get
            {
                return new FlowSort(this.FK_FlowSort);
            }
        }
        /// <summary>
        /// flow data 数据
        /// </summary>
        public BP.WF.GERpt HisFlowData
        {
            get
            {
                try
                {
                    BP.WF.GERpt wk = new BP.WF.GERpt("ND" + int.Parse(this.No) + "Rpt");
                    return wk;
                }
                catch
                {
                    this.DoCheck();
                    BP.WF.GERpt wk1 = new BP.WF.GERpt("ND" + int.Parse(this.No) + "Rpt");
                    return wk1;
                }
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程
        /// </summary>
        public Flow()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public Flow(string _No)
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

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "流程";
                map.CodeStruct = "3";

                map.AddTBStringPK(FlowAttr.No, null, null, true, true, 1, 10, 3);
                map.AddTBString(FlowAttr.Name, null, null, true, false, 0, 500, 10);

                map.AddDDLEntities(FlowAttr.FK_FlowSort, "01", "流程类别", new FlowSorts(), false);
                map.AddTBInt(FlowAttr.FlowType, (int)FlowType.Panel, "流程类型", false, false);


                // @0=业务流程@1=公文流程.
                map.AddTBInt(FlowAttr.FlowSheetType, (int)FlowSheetType.SheetFlow, "表单类型", false, false);

                //map.AddDDLSysEnum(FlowAttr.DocType, (int)DocType.OfficialDoc, "公文类型(对公文有效)", false, false,
                //    FlowAttr.DocType, "@0=正式公文@1=便函");
                //map.AddDDLSysEnum(FlowAttr.XWType, (int)XWType.Down, "行文类型(对公文有效)", false, false, FlowAttr.XWType,
                //    "@0=上行文@1=平行文@2=下行文");

                map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "运行方式", false,
                    false, FlowAttr.FlowRunWay,
                    "@0=手工启动@1=指定人员按时启动@2=数据集按时启动@3=触发式启动");
                map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 3000, 10);

                map.AddTBString(FlowAttr.Note, null, "备注", true, false, 0, 100, 10);
                map.AddTBString(FlowAttr.RunSQL, null, "流程结束执行后执行的SQL", true, false, 0, 2000, 10);
                 


                map.AddTBInt(FlowAttr.NumOfBill, 0, "是否有单据", false, false);
                map.AddTBInt(FlowAttr.NumOfDtl, 0, "NumOfDtl", false, false);
                map.AddBoolean(FlowAttr.IsOK, true, "是否起用", true, true);
                map.AddBoolean(FlowAttr.IsCanStart, true, "可以独立启动否？", true, true, true);
                map.AddTBDecimal(FlowAttr.AvgDay, 0, "平均运行用天", false, false);
                map.AddTBString(FlowAttr.StartListUrl, null, "导航Url", true, false, 0, 500, 10, true);
                map.AddTBInt(FlowAttr.AppType, 0, "应用类型", false, false);
                map.AddTBInt(FlowAttr.IsMD5, 0, "IsMD5", false, false);
                map.AddTBInt(FlowAttr.Idx, 0, "显示顺序号(在发起列表中)",true, false);

                map.AddTBInt(FlowAttr.TimelineRole, 0, "时效性规则", true, false);
                map.AddTBString(FlowAttr.Paras, null, "参数", false, false, 0, 400, 10);

                // add 2013-01-01. 
                map.AddTBString(FlowAttr.PTable, null, "流程数据存储主表", true, false, 0, 30, 10);

                // add 2013-01-01.
                map.AddTBInt(FlowAttr.DataStoreModel, 0, "数据存储模式", true, false);

                // add 2013-02-05.
                map.AddTBString(FlowAttr.TitleRole, null, "标题生成规则", true, false, 0, 150, 10, true);

                // add 2013-02-14 
                map.AddTBString(FlowAttr.FlowCode, null, "流程标记", true, false, 0, 150, 10);
              

                //map.AddDDLSysEnum(FlowAttr.TimelineRole, (int)TimelineRole.ByNodeSet, "时效性规则",
                // true, true, FlowAttr.TimelineRole, "@0=按节点(由节点属性来定义)@1=按发起人(开始节点SysSDTOfFlow字段计算)");


                map.AddSearchAttr(FlowAttr.FK_FlowSort);
                map.AddSearchAttr(FlowAttr.FlowRunWay);

                RefMethod rm = new RefMethod();
                rm.Title = "设计检查报告"; // "设计检查报告";
                rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "/WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                map.AddRefMethod(rm);

                //   rm = new RefMethod();
                //rm.Title = this.ToE("ViewDef", "视图定义"); //"视图定义";
                //rm.Icon = "/WF/Img/Btn/View.gif";
                //rm.ClassMethodName = this.ToString() + ".DoDRpt";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "报表运行"; // "报表运行";
                rm.Icon = "/WF/Img/Btn/View.gif";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                //rm.Icon = "/WF/Img/Btn/Table.gif";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("FlowDataOut", "数据转出定义");  //"数据转出定义";
                ////  rm.Icon = "/WF/Img/Btn/Table.gif";
                //rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";
                //rm.ClassMethodName = this.ToString() + ".DoExp";
                //map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "删除数据";
                rm.Warning = "您确定要执行删除流程数据吗？";
                rm.ToolTip = "清除历史流程数据。";
                rm.ClassMethodName = this.ToString() + ".DoExp";
                map.AddRefMethod(rm);

                //map.AttrsOfOneVSM.Add(new FlowStations(), new Stations(), FlowStationAttr.FK_Flow,
                //    FlowStationAttr.FK_Station, DeptAttr.Name, DeptAttr.No, "抄送岗位");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            DBAccess.RunSQL("UPDATE WF_Node SET FlowName='" + this.Name + "' WHERE FK_Flow='" + this.No + "'");
            DBAccess.RunSQL("UPDATE Sys_MapData SET  Name='" + this.Name + "' WHERE No='"+this.PTable+"'");
            return base.beforeUpdateInsertAction();
        }

        #region  公共方法
        /// <summary>
        /// 设计数据转出
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            this.DoCheck();
            PubClass.WinOpen("/WF/Admin/Exp.aspx?CondType=0&FK_Flow=" + this.No, "单据", "cdsn", 800, 500, 210, 300);
            return null;
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoDRpt()
        {
            this.DoCheck();
            PubClass.WinOpen("/WF/Admin/WFRpt.aspx?CondType=0&FK_Flow=" + this.No, "单据", "cdsn", 800, 500, 210, 300);
            return null;
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            BP.PubClass.WinOpen("/WF/MapDef/Rpt/Home.aspx?FK_Flow=" + this.No + "&DoType=Edit&FK_MapData=ND" + int.Parse(this.No) + "Rpt", 700, 400);
            return null;
        }
        public string DoDelData()
        {
            string sql = "  where FK_Node in (SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "')";
            string sql1 = " where NodeID in (SELECT NodeID FROM WF_Node WHERE fk_flow='" + this.No + "')";

           // DA.DBAccess.RunSQL("DELETE FROM WF_CHOfFlow WHERE FK_Flow='" + this.No + "'");

            DA.DBAccess.RunSQL("DELETE FROM WF_Bill WHERE FK_Flow='" + this.No + "'");
            DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'");
            DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'");

            string sqlIn = " WHERE ReturnNode IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            DA.DBAccess.RunSQL("DELETE FROM WF_ReturnWork " + sqlIn);
            DA.DBAccess.RunSQL("DELETE FROM WF_GenerFH WHERE FK_Flow='" + this.No + "'");
            DA.DBAccess.RunSQL("DELETE FROM WF_SelectAccper " + sql);

           // DA.DBAccess.RunSQL("DELETE FROM WF_FileManager " + sql);
            DA.DBAccess.RunSQL("DELETE FROM WF_RememberMe " + sql);

            try
            {
                DA.DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.No) + "Track ");
            }
            catch
            {
            }

            if (DBAccess.IsExitsObject( this.PTable))
                DBAccess.RunSQL("DELETE FROM "+this.PTable);

            //DA.DBAccess.RunSQL("DELETE FROM WF_WorkList WHERE FK_Flow='" + this.No + "'");
            //DA.DBAccess.RunSQL("DELETE FROM Sys_MapExt WHERE FK_MapData LIKE 'ND"+int.Parse(this.No)+"%'" );

            //删除节点数据。
            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                try
                {
                    Work wk = nd.HisWork;
                    DA.DBAccess.RunSQL("DELETE FROM " + wk.EnMap.PhysicsTable);
                }
                catch
                {
                }

                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    try
                    {
                        DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                    }
                    catch
                    {
                    }
                }
            }
            MapDtls mydtls = new MapDtls("ND" + int.Parse(this.No) + "Rpt");
            foreach (MapDtl dtl in mydtls)
            {
                try
                {
                    DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable);
                }
                catch
                {
                }
            }
            return "删除成功...";
        }
         
        /// <summary>
        /// 装载流程模板
        /// </summary>
        /// <param name="fk_flowSort">流程类别</param>
        /// <param name="path">流程名称</param>
        /// <returns></returns>
        public static Flow DoLoadFlowTemplate(string fk_flowSort, string path, ImpFlowTempleteModel model)
        {
            FileInfo info = new FileInfo(path);
            DataSet ds = new DataSet();
            ds.ReadXml(path);

            if (ds.Tables.Contains("WF_Flow") == false)
                throw new Exception("导入错误，非流程模版文件。");

            DataTable dtFlow = ds.Tables["WF_Flow"];
            Flow fl = new Flow();
            string oldFlowNo = dtFlow.Rows[0]["No"].ToString();
            int oldFlowID = int.Parse(oldFlowNo);
            string timeKey = DateTime.Now.ToString("yyMMddhhmmss");

            //判断流程标示.
            if (dtFlow.Columns.Contains("FlowCode") == true)
            {
                string flowCode = dtFlow.Rows[0]["FlowCode"].ToString();
                if (string.IsNullOrEmpty(flowCode) == false)
                {
                    if (fl.IsExit(FlowAttr.FlowCode, flowCode))
                        throw new Exception("@该流程标示:" + flowCode + "已经存在于系统中,您不能导入.");
                }
            }
        

            switch (model)
            {
                case ImpFlowTempleteModel.AsNewFlow: /*做为一个新流程. */
                    fl.No = fl.GenerNewNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    break;
                case ImpFlowTempleteModel.AsTempleteFlowNo: /*用流程模版中的编号*/
                    fl.No = oldFlowNo;
                    if (fl.IsExits)
                    {
                        throw new Exception("导入错误:流程模版中的编号(" + oldFlowNo + ")在系统中已经存在,流程名称为:"+ dtFlow.Rows[0]["Name"].ToString() );
                    }
                    else
                    {
                        fl.No = oldFlowNo;
                        fl.DoDelData();
                        fl.DoDelete(); /*删除可能存在的垃圾.*/
                    }
                    break;
                case ImpFlowTempleteModel.AsTempleteFlowNoOvrewaiteWhenExit: /*用流程模版中的编号，如果有并覆盖它.*/
                    fl.No = oldFlowNo;
                    fl.DoDelData();
                    fl.DoDelete(); /*删除可能存在的垃圾.*/
                    break;
                default:
                    throw new Exception("@没有判断");
            }

            // string timeKey = fl.No;
            int idx = 0;
            string infoErr = "";
            string infoTable = "";
            int flowID = int.Parse(fl.No);

            #region 处理流程表数据
            foreach (DataColumn dc in dtFlow.Columns)
            {
                string val = dtFlow.Rows[0][dc.ColumnName] as string;
                switch (dc.ColumnName.ToLower())
                {
                    case "no":
                    case "fk_flowsort":
                        continue;
                    case "name":
                        val = "CopyOf" + val + "_" + DateTime.Now.ToString("MM月dd日HH时mm分");
                        break;
                    default:
                        break;
                }
                fl.SetValByKey(dc.ColumnName, val);
            }
            fl.FK_FlowSort = fk_flowSort;
            fl.Insert();
            #endregion 处理流程表数据

            #region 处理OID 插入重复的问题。 Sys_GroupField ,  Sys_MapAttr.
            DataTable mydtGF = ds.Tables["Sys_GroupField"];
            DataTable myDTAttr = ds.Tables["Sys_MapAttr"];
            DataTable myDTAth = ds.Tables["Sys_FrmAttachment"];
            DataTable myDTDtl = ds.Tables["Sys_MapDtl"];
            DataTable myDFrm = ds.Tables["Sys_MapFrame"];
            DataTable myDM2M = ds.Tables["Sys_MapM2M"];
            foreach (DataRow dr in mydtGF.Rows)
            {
                Sys.GroupField gf = new Sys.GroupField();
                foreach (DataColumn dc in mydtGF.Columns)
                {
                    string val = dr[dc.ColumnName] as string;
                    gf.SetValByKey(dc.ColumnName, val);
                }
                int oldID = gf.OID;
                gf.OID = DBAccess.GenerOID();
                dr["OID"] = gf.OID;

                // 属性。
                if (myDTAttr != null && myDTAttr.Columns.Contains("GroupID"))
                {
                    foreach (DataRow dr1 in myDTAttr.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString() == oldID.ToString())
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDTAth != null && myDTAth.Columns.Contains("GroupID"))
                {
                    // 附件。
                    foreach (DataRow dr1 in myDTAth.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString() == oldID.ToString())
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDTDtl != null && myDTDtl.Columns.Contains("GroupID"))
                {
                    // 从表。
                    foreach (DataRow dr1 in myDTDtl.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString() == oldID.ToString())
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDFrm != null && myDFrm.Columns.Contains("GroupID"))
                {
                    // frm.
                    foreach (DataRow dr1 in myDFrm.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString() == oldID.ToString())
                            dr1["GroupID"] = gf.OID;
                    }
                }

                if (myDM2M != null && myDM2M.Columns.Contains("GroupID"))
                {
                    // m2m.
                    foreach (DataRow dr1 in myDM2M.Rows)
                    {
                        if (dr1["GroupID"] == null)
                            dr1["GroupID"] = 0;

                        if (dr1["GroupID"].ToString() == oldID.ToString())
                            dr1["GroupID"] = gf.OID;
                    }
                }
            }
            #endregion 处理OID 插入重复的问题。 Sys_GroupField ， Sys_MapAttr.

            int timeKeyIdx = 0;
            foreach (DataTable dt in ds.Tables)
            {
                timeKeyIdx++;
                timeKey = timeKey + timeKeyIdx.ToString();

                infoTable = "@导入:" + dt.TableName + " 出现异常。";
                switch (dt.TableName)
                {
                    case "WF_Flow": //模版文件。
                        continue;
                    case "WF_BillTemplate":
                        continue; /*因为省掉了 打印模板的处理。*/
                        foreach (DataRow dr in dt.Rows)
                        {
                            BillTemplate bt = new BillTemplate();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_flow":
                                        val = flowID.ToString();
                                        break;
                                    case "nodeid":
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                bt.SetValByKey(dc.ColumnName, val);
                            }
                            int i = 0;
                            string no = bt.No;
                            while (bt.IsExits)
                            {
                                bt.No = no + i.ToString();
                                i++;
                            }

                            try
                            {
                                File.Copy(info.DirectoryName + "\\" + no + ".rtf", BP.SystemConfig.PathOfWebApp + @"\DataUser\CyclostyleFile\" + bt.No + ".rtf", true);
                            }
                            catch (Exception ex)
                            {
                                // infoErr += "@恢复单据模板时出现错误：" + ex.Message + ",有可能是您在复制流程模板时没有复制同目录下的单据模板文件。";
                            }
                            bt.Insert();
                        }
                        break;
                    case "WF_FrmNode": //Conds.xml。
                        DBAccess.RunSQL("DELETE WF_FrmNode WHERE FK_Flow='" + fl.No + "'");
                        foreach (DataRow dr in dt.Rows)
                        {
                            FrmNode fn = new FrmNode();
                            fn.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        break;
                                }
                                fn.SetValByKey(dc.ColumnName, val);
                            }
                            // 开始插入。
                            fn.MyPK = fn.FK_Frm + "_" + fn.FK_Node;
                            fn.Insert();
                        }
                        break;
                    case "WF_Cond": //Conds.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Cond cd = new Cond();
                            cd.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "tonodeid":
                                    case "fk_node":
                                    case "nodeid":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                        val = fl.No;
                                        break;
                                    default:
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            cd.FK_Flow = fl.No;

                            //  return this.FK_MainNode + "_" + this.ToNodeID + "_" + this.HisCondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
                            // ，开始插入。 
                            if (cd.MyPK.Contains("Stas"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Stas.ToString();
                            }
                            else if (cd.MyPK.Contains("Dept"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Depts.ToString();
                            }
                            else if (cd.MyPK.Contains("Paras"))
                            {
                                cd.MyPK = cd.FK_Node + "_" + cd.ToNodeID + "_" + cd.HisCondType.ToString() + "_" + ConnDataFrom.Paras.ToString();
                            }
                            else
                            {
                                cd.MyPK = DA.DBAccess.GenerOID().ToString() + DateTime.Now.ToString("yyMMddHHmmss");
                            }
                            cd.DirectInsert();
                        }
                        break;
                    case "WF_NodeReturn"://可退回的节点。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeReturn cd = new NodeReturn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                    case "returnn":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                cd.SetValByKey(dc.ColumnName, val);
                            }

                            //开始插入。
                            try
                            {
                                cd.Insert();
                            }
                            catch
                            {
                                cd.Update();
                            }
                        }
                        break;
                    case "WF_Direction": //方向。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Direction dir = new Direction();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "node":
                                    case "tonode":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                dir.SetValByKey(dc.ColumnName, val);
                            }
                            dir.FK_Flow = fl.No;
                            dir.Insert();
                        }
                        break;
                    case "WF_TurnTo": //转向规则.
                        foreach (DataRow dr in dt.Rows)
                        {
                            TurnTo fs = new TurnTo();

                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                fs.SetValByKey(dc.ColumnName, val);
                            }
                            fs.FK_Flow = fl.No;
                            fs.Save();
                        }
                        break;
                    case "WF_FAppSet": //FAppSets.xml。
                        continue;
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    FAppSet fs = new FAppSet();
                    //    fs.FK_Flow = fl.No;
                    //    foreach (DataColumn dc in dt.Columns)
                    //    {
                    //        string val = dr[dc.ColumnName] as string;
                    //        if (val == null)
                    //            continue;

                    //        switch (dc.ColumnName.ToLower())
                    //        {
                    //            case "fk_node":
                    //                if (val.Length == 3)
                    //                    val = flowID + val.Substring(1);
                    //                else if (val.Length == 4)
                    //                    val = flowID + val.Substring(2);
                    //                else if (val.Length == 5)
                    //                    val = flowID + val.Substring(3);
                    //                break;
                    //            default:
                    //                break;
                    //        }
                    //        fs.SetValByKey(dc.ColumnName, val);
                    //    }
                    //    fs.OID = 0;
                    //    fs.Insert();
                    //}
                    case "WF_LabNote": //LabNotes.xml。
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            LabNote ln = new LabNote();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                ln.SetValByKey(dc.ColumnName, val);
                            }
                            idx++;
                            ln.FK_Flow = fl.No;
                            ln.MyPK = ln.FK_Flow + "_" + ln.X + "_" + ln.Y + "_" + idx;
                            ln.DirectInsert();
                        }
                        break;
                    case "WF_NodeDept": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeDept dir = new NodeDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                dir.SetValByKey(dc.ColumnName, val);
                            }
                            dir.Insert();
                        }

                        break;
                    case "WF_Node": //LabNotes.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            BP.WF.Ext.NodeSheet nd = new BP.WF.Ext.NodeSheet();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands":
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }

                            nd.FK_Flow = fl.No;
                            nd.FlowName = fl.Name;
                            nd.DirectInsert();

                            //删除mapdata.
                            DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData='ND" + nd.NodeID + "'");
                        }
                        foreach (DataRow dr in dt.Rows)
                        {
                            Node nd = new Node();
                            nd.NodeID = int.Parse(dr[NodeAttr.NodeID].ToString());
                            nd.RetrieveFromDBSources();
                            nd.FK_Flow = fl.No;
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "nodeid":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "fk_flow":
                                    case "fk_flowsort":
                                        continue;
                                    case "showsheets":
                                    case "histonds":
                                    case "groupstands":
                                        string key = "@" + flowID;
                                        val = val.Replace(key, "");
                                        break;
                                    default:
                                        break;
                                }
                                nd.SetValByKey(dc.ColumnName, val);
                            }
                            nd.FK_Flow = fl.No;
                            nd.FlowName = fl.Name;
                            nd.DirectUpdate();
                        }
                        break;
                    case "WF_NodeStation": //FAppSets.xml。
                        DBAccess.RunSQL("DELETE WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + fl.No + "')");
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeStation ns = new NodeStation();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ns.SetValByKey(dc.ColumnName, val);
                            }
                            ns.Insert();
                        }
                        break;
                    case "WF_Listen": // 信息侦听。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Listen li = new Listen();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "oid":
                                        continue;
                                        break;
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    case "nodes":
                                        string[] nds = val.Split('@');
                                        string valExt = "";
                                        foreach (string nd in nds)
                                        {
                                            if (nd == "" || nd == null)
                                                continue;
                                            string ndExt = nd.Clone() as string;
                                            if (ndExt.Length == 3)
                                                ndExt = flowID + ndExt.Substring(1);
                                            else if (val.Length == 4)
                                                ndExt = flowID + ndExt.Substring(2);
                                            else if (val.Length == 5)
                                                ndExt = flowID + ndExt.Substring(3);
                                            ndExt = "@" + ndExt;
                                            valExt += ndExt;
                                        }
                                        val = valExt;
                                        break;
                                    default:
                                        break;
                                }
                                li.SetValByKey(dc.ColumnName, val);
                            }
                            li.Insert();
                        }
                        break;
                    case "Sys_Enum": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        break;
                                    default:
                                        break;
                                }
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                            if (se.IsExits)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnumMain sem = new Sys.SysEnumMain();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                sem.SetValByKey(dc.ColumnName, val);
                            }
                            if (sem.IsExits)
                                continue;
                            sem.Insert();
                        }
                        break;
                    case "Sys_MapAttr": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapAttr ma = new Sys.MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_mapdata":
                                    case "keyofen":
                                    case "autofulldoc":
                                        if (val == null)
                                            continue;

                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                ma.SetValByKey(dc.ColumnName, val);
                            }
                            bool b = ma.IsExit(Sys.MapAttrAttr.FK_MapData, ma.FK_MapData,
                                Sys.MapAttrAttr.KeyOfEn, ma.KeyOfEn);

                            ma.MyPK = ma.FK_MapData + "_" + ma.KeyOfEn;
                            if (b == true)
                                ma.DirectUpdate();
                            else
                                ma.DirectInsert();
                        }
                        break;
                    case "Sys_MapData": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapData md = new Sys.MapData();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + int.Parse(fl.No));
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
                        }
                        break;
                    case "Sys_MapDtl": //RptEmps.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapDtl md = new Sys.MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.MapExt md = new Sys.MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                md.SetValByKey(dc.ColumnName, val);
                            }
                            md.Save();
                        }
                        break;
                    case "Sys_FrmLine":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLine en = new FrmLine();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }

                            en.MyPK = "LIE" + timeKey + "_" + idx;
                            if (en.IsExitGenerPK())
                                continue;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmEle":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEle en = new FrmEle();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImg":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.MyPK = "Img" + timeKey + "_" + idx;

                            if (en.IsExitGenerPK())
                                continue;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLab":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLab en = new FrmLab();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.MyPK = "Lab" + timeKey + "_" + idx;

                            if (en.IsExitGenerPK())
                                continue;

                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLink":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLink en = new FrmLink();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.MyPK = "LK" + timeKey + "_" + idx;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmAttachment":
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.MyPK = en.FK_MapData + "_" + en.NoOfObj;
                            en.Save();
                        }
                        break;
                    case "Sys_FrmEvent": //事件.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEvent en = new FrmEvent();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Save();
                        }
                        break;
                    case "Sys_MapM2M": //Sys_MapM2M.
                        idx = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapM2M en = new MapM2M();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Save();
                        }
                        break;
                    case "Sys_FrmRB": //Sys_FrmRB.
                        idx = 0;

                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                en.SetValByKey(dc.ColumnName, val);
                            }
                            en.Save();
                        }
                        break;
                    case "WF_NodeEmp": //FAppSets.xml。
                        foreach (DataRow dr in dt.Rows)
                        {
                            NodeEmp ne = new NodeEmp();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "Sys_GroupField": // 
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.GroupField gf = new Sys.GroupField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "enname":
                                    case "keyofen":
                                        val = val.Replace("ND" + oldFlowID, "ND" + flowID);
                                        break;
                                    default:
                                        break;
                                }
                                gf.SetValByKey(dc.ColumnName, val);
                            }
                            //  int oid = DBAccess.GenerOID();
                            //  DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + gf.EnName + "' AND GroupID=" + gf.OID);
                            gf.InsertAsOID(gf.OID);
                        }
                        break;
                    case "WF_CCDept": // 抄送.
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCDept ne = new CCDept();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "WF_CCEmp": // 抄送.
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCEmp ne = new CCEmp();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    case "WF_CCStation": // 抄送.
                        foreach (DataRow dr in dt.Rows)
                        {
                            CCStation ne = new CCStation();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;

                                switch (dc.ColumnName.ToLower())
                                {
                                    case "fk_node":
                                        if (val.Length == 3)
                                            val = flowID + val.Substring(1);
                                        else if (val.Length == 4)
                                            val = flowID + val.Substring(2);
                                        else if (val.Length == 5)
                                            val = flowID + val.Substring(3);
                                        break;
                                    default:
                                        break;
                                }
                                ne.SetValByKey(dc.ColumnName, val);
                            }
                            ne.Insert();
                        }
                        break;
                    default:
                        // infoErr += "Error:" + dt.TableName;
                        break;
                    //    throw new Exception("@unhandle named " + dt.TableName);
                }
            }

            #region 处理数据完整性。
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE NodeID NOT IN (SELECT NodeID FROM WF_Node)");
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE ToNodeID NOT IN (SELECT NodeID FROM WF_Node)");
            DBAccess.RunSQL("DELETE FROM WF_Cond WHERE FK_Node NOT IN (SELECT NodeID FROM WF_Node) AND FK_Node > 0");
            #endregion

            if (infoErr == "")
            {
                //// 删除空白行.
                //BP.DTS.DeleteBlankGroupField en = new BP.DTS.DeleteBlankGroupField();
                //en.Do();

                infoTable = "";
                    fl.DoCheck();
                 
                return fl; // "完全成功。";
            }

            infoErr = "@执行期间出现如下非致命的错误：\t\r" + infoErr + "@ " + infoTable;
            throw new Exception(infoErr);
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        fl.DoDelete();
            //        throw new Exception("@" + infoErr + " @table=" + infoTable + "@" + ex.Message);
            //    }
            //    catch (Exception ex1)
            //    {
            //        throw new Exception("@删除已经产生的错误的流程数据期间错误:" + ex1.Message );
            //    }
            //}
        }
        public Node DoNewNode(int x, int y)
        {
            Node nd = new Node();
            int idx = this.HisNodes.Count;
            while (true)
            {
                string strID = this.No + idx.ToString().PadLeft(2, '0');
                nd.NodeID = int.Parse(strID);
                if (nd.IsExits == false)
                    break;
                idx++;
            }

            nd.HisNodeWorkType = NodeWorkType.Work;
            nd.Name = "节点" + idx;
            nd.HisNodePosType = NodePosType.Mid;
            nd.FK_Flow = this.No;
            nd.FlowName = this.Name;
            nd.X = x;
            nd.Y = y;
            nd.Step = idx;
            nd.Insert();

            nd.CreateMap();
            return nd;
        }
      
        /// <summary>
        /// 执行新建
        /// </summary>
        /// <param name="flowSort">类别</param>
        /// <param name="flowName">流程名称</param>
        /// <param name="model">数据存储模式</param>
        /// <param name="pTable">数据存储物理表</param>
        /// <param name="flowCode">流程标记</param>
        public void DoNewFlow(string flowSort,string flowName, DataStoreModel model, string pTable,string flowCode)
        {
            this.Name = flowName;
            if (string.IsNullOrWhiteSpace(this.Name))
                this.Name = "新建流程" + this.No; //新建流程.

            this.No = this.GenerNewNoByKey(FlowAttr.No);
            this.HisDataStoreModel = model;
            this.PTable = pTable;
            this.FK_FlowSort = flowSort;
            this.FlowCode = flowCode;

            if (string.IsNullOrEmpty(flowCode)==false)
            {
                if (this.IsExit(FlowAttr.FlowCode, flowCode))
                    throw new Exception("@该流程标示:"+flowCode+"已经存在于系统中.");
            }

            /*给初始值*/
            //this.Paras = "@StartNodeX=10@StartNodeY=15@EndNodeX=40@EndNodeY=10";
            this.Paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350";
            this.Save();

            #region 删除有可能存在的历史数据.
            Flow fl = new Flow(this.No);
            fl.DoDelData();
            fl.DoDelete();

            //设计者，设计人员?
            fl.DesignerNo = WebUser.No;
            fl.DesignerName = WebUser.Name;
            this.Save();
            #endregion 删除有可能存在的历史数据.

            Node nd = new Node();
            nd.NodeID = int.Parse(this.No + "01");
            nd.Name = "开始节点";//  "开始节点"; 
            nd.Step = 1;
            nd.FK_Flow = this.No;
            nd.FlowName = this.Name;
            nd.HisNodePosType = NodePosType.Start;
            nd.HisNodeWorkType = NodeWorkType.StartWork;
            nd.X = 200;
            nd.Y = 150;
            nd.Insert();

            nd.CreateMap();
            nd.HisWork.CheckPhysicsTable();

            nd = new Node();
            nd.NodeID = int.Parse(this.No + "99");
            nd.Name =  "结束节点"; // "结束节点";
            nd.Step = 99;
            nd.FK_Flow = this.No;
            nd.FlowName = this.Name;
            nd.HisNodePosType = NodePosType.Mid;
            nd.HisNodeWorkType = NodeWorkType.Work;
            nd.X = 200;
            nd.Y = 250;
            nd.Insert();
            nd.CreateMap();
            nd.HisWork.CheckPhysicsTable();

            BP.Sys.MapData md = new BP.Sys.MapData();
            md.No = "ND" + int.Parse(this.No) + "Rpt";
            md.Name = this.Name;
            md.Save();

            #region 生成CCForm 的装饰.
            FrmImg img = new FrmImg();
            img.MyPK = "Img" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No;
            img.FK_MapData = "ND" + int.Parse(this.No + "01");
            img.X = (float)577.26;
            img.Y = (float)3.45;

            img.W = (float)137;
            img.H = (float)40;

            img.ImgURL = "/ccform;component/Img/LogoBig.png";
            img.LinkURL = "http://ccflow.org";
            img.LinkTarget = "_blank";
            img.Insert();

            FrmLab lab = new FrmLab();
            //lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No;
            //lab.Text = this.ToE("Title", "流程标题");
            //lab.FK_MapData = "ND" + int.Parse(this.No + "01");
            //lab.X = (float)106.5;
            //lab.Y = (float)59.22;
            //lab.FontSize = 11;
            //lab.FontColor = "black";
            //lab.FontName = "Portable User Interface";
            //lab.FontStyle = "Normal";
            //lab.FontWeight = "normal";
            //lab.Insert();

            lab = new FrmLab();
            lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 1;
            lab.Text = "优先级";
            lab.FK_MapData = "ND" + int.Parse(this.No + "01");
            lab.X = (float)109.05;
            lab.Y = (float)58.1;
            lab.FontSize = 11;
            lab.FontColor = "black";
            lab.FontName = "Portable User Interface";
            lab.FontStyle = "Normal";
            lab.FontWeight = "normal";
            lab.Insert();


            lab = new FrmLab();
            lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 2;
            lab.Text = "发起人";
            lab.FK_MapData = "ND" + int.Parse(this.No + "01");
            lab.X = (float)106.48;
            lab.Y = (float)96.08;
            lab.FontSize = 11;
            lab.FontColor = "black";
            lab.FontName = "Portable User Interface";
            lab.FontStyle = "Normal";
            lab.FontWeight = "normal";
            lab.Insert();

            lab = new FrmLab();
            lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 3;
            lab.Text = "发起时间";
            lab.FK_MapData = "ND" + int.Parse(this.No + "01");
            lab.X = (float)307.64;
            lab.Y = (float)95.17;

            lab.FontSize = 11;
            lab.FontColor = "black";
            lab.FontName = "Portable User Interface";
            lab.FontStyle = "Normal";
            lab.FontWeight = "normal";
            lab.Insert();

            lab = new FrmLab();
            lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 4;
            lab.Text = "新建节点(请修改标题)";
            lab.FK_MapData = "ND" + int.Parse(this.No + "01");

            lab.X = (float)294.67;
            lab.Y = (float)8.27;

            lab.FontSize = 23;
            lab.FontColor = "Blue";
            lab.FontName = "Portable User Interface";
            lab.FontStyle = "Normal";
            lab.FontWeight = "normal";
            lab.Insert();

            lab = new FrmLab();
            lab.MyPK = "Lab" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No + 5;
            lab.Text = "说明:以上内容是ccflow自动产生的，您可以修改/删除它。@为了更方便您的设计您可以到http://ccflow.org官网下载表单模板.";
            lab.Text += "@因为当前技术问题与silverlight开发工具使用特别说明如下:@";
            lab.Text += "@1,改变控件位置: ";
            lab.Text += "@  所有的控件都支持 wasd, 做为方向键用来移动控件的位置， 部分控件支持方向键. ";
            lab.Text += "@2, 增加textbox, 从表, dropdownlistbox, 的宽度 shift+ -> 方向键增加宽度 shift + <- 减小宽度.";
            lab.Text += "@3, 保存 windows键 + s.  删除 delete.  复制 ctrl+c   粘帖: ctrl+v.";
            lab.Text += "@4, 支持全选，批量移动， 批量放大缩小字体., 批量改变线的宽度.";
            lab.Text += "@5, 改变线的长度： 选择线，点绿色的圆点，拖拉它。.";
            lab.Text += "@6, 放大或者缩小　label 的字体 , 选择一个多个label , 按 A+ 或者　A－　按钮.";
            lab.Text += "@7, 改变线或者标签的颜色， 选择操作对象，点工具栏上的调色板.";

            lab.X = (float)168.24;
            lab.Y = (float)163.7;
            lab.FK_MapData = "ND" + int.Parse(this.No + "01");
            lab.FontSize = 11;
            lab.FontColor = "Red";
            lab.FontName = "Portable User Interface";
            lab.FontStyle = "Normal";
            lab.FontWeight = "normal";
            lab.Insert();

            string key = "L" + DateTime.Now.ToString("yyMMddhhmmss") + WebUser.No;
            FrmLine line = new FrmLine();
            line.MyPK = key + "_1";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)281.82;
            line.Y1 = (float)81.82;
            line.X2 = (float)281.82;
            line.Y2 = (float)121.82;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_2";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)360;
            line.Y1 = (float)80.91;
            line.X2 = (float)360;
            line.Y2 = (float)120.91;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_3";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)158.82;
            line.Y1 = (float)41.82;
            line.X2 = (float)158.82;
            line.Y2 = (float)482.73;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_4";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)81.55;
            line.Y1 = (float)80;
            line.X2 = (float)718.82;
            line.Y2 = (float)80;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();


            line.MyPK = key + "_5";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)81.82;
            line.Y1 = (float)40;
            line.X2 = (float)81.82;
            line.Y2 = (float)480.91;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_6";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)81.82;
            line.Y1 = (float)481.82;
            line.X2 = (float)720;
            line.Y2 = (float)481.82;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_7";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)83.36;
            line.Y1 = (float)40.91;
            line.X2 = (float)717.91;
            line.Y2 = (float)40.91;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_8";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)83.36;
            line.Y1 = (float)120.91;
            line.X2 = (float)717.91;
            line.Y2 = (float)120.91;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();

            line.MyPK = key + "_9";
            line.FK_MapData = "ND" + int.Parse(this.No + "01");
            line.X1 = (float)719.09;
            line.Y1 = (float)40;
            line.X2 = (float)719.09;
            line.Y2 = (float)482.73;
            line.BorderWidth = (float)2;
            line.BorderColor = "Black";
            line.Insert();
            #endregion

            this.CheckRpt();
            Flow.RepareV_FlowData_View();
        }
        protected override bool beforeUpdate()
        {
            Node.CheckFlow(this);
            return base.beforeUpdate();
        }
        public void DoDelete()
        {
            string sql = "";

            //sql = " DELETE FROM WF_chofflow WHERE FK_Flow='" + this.No + "'";

            sql += "@ DELETE  FROM WF_GenerWorkerlist WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_GenerWorkFlow WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE FROM  WF_Cond WHERE FK_Flow='" + this.No + "'";

            // 删除岗位节点。
            sql += "@ DELETE  FROM  WF_NodeStation WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除方向。
            //sql += "@ DELETE FROM WF_Direction  WHERE Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //sql += "@ DELETE FROM WF_Direction  WHERE ToNode IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            sql += "@ DELETE FROM  WF_Direction WHERE FK_Flow='" + this.No + "'";


            //删除它。
            sql += "@ DELETE FROM WF_NodeEmp  WHERE   FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            //删除侦听.
            sql += "@ DELETE FROM WF_Listen WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除d2d数据.
            //  sql += "@GO DELETE WF_M2M WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";
            //// 删除配置.
            //sql += "@ DELETE FROM WF_FAppSet WHERE NodeID IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除配置.
            sql += "@ DELETE FROM WF_FlowEmp WHERE FK_Flow='" + this.No + "' ";


            //// 外部程序设置
            //sql += "@ DELETE FROM WF_FAppSet WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            // 删除单据
            sql += "@ DELETE FROM WF_BillTemplate WHERE  NodeID in (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + this.No + "')";

            Nodes nds = new Nodes(this.No);
            foreach (Node nd in nds)
            {
                nd.Delete();
            }

            sql += "@ DELETE  FROM WF_Node WHERE FK_Flow='" + this.No + "'";
            sql += "@ DELETE  FROM  WF_LabNote WHERE FK_Flow='" + this.No + "'";

            //删除分组信息
            sql += "@ DELETE FROM Sys_GroupField WHERE EnName NOT IN(SELECT NO FROM Sys_MapData)";

            #region 删除流程报表。
            MapData md = new MapData();
            md.No = "ND" + int.Parse(this.No) + "Rpt";
            md.Delete();

            //删除视图.
            try
            {
                BP.DA.DBAccess.RunSQL("DROP VIEW V_" + this.No);
            }
            catch
            {
            }
            #endregion 删除流程报表。

            // 执行录制的sql scripts.
            BP.DA.DBAccess.RunSQLs(sql);
            this.Delete();
            Flow.RepareV_FlowData_View();
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class Flows : EntitiesNoName
    {
        #region 查询
        public static void GenerHtmlRpts()
        {
            Flows fls = new Flows();
            fls.RetrieveAll();

            foreach (Flow fl in fls)
            {
                fl.DoCheck();
                fl.GenerHisHtmlDocRpt();
                fl.GenerFlowXmlTemplete();
            }

            // 生成索引界面
            string path = SystemConfig.PathOfWorkDir + @"\VisualFlow\DataUser\FlowDesc\";
            string msg = "";
            msg += "<html>";
            msg += "\r\n<title>.net工作流程引擎设计，流程模板</title>";

            msg += "\r\n<body>";

            msg += "\r\n<h1>驰骋流程模板网</h1> <br><a href=index.htm >返回首页</a> - <a href='http://ccFlow.org' >访问驰骋工作流程管理系统，工作流引擎官方网站</a> 流程系统建设请联系:QQ:793719823,Tel:18660153393<hr>";

            foreach (Flow fl in fls)
            {
                msg += "\r\n <h3><b><a href='./" + fl.No + "/index.htm' target=_blank>" + fl.Name + "</a></b> - <a href='" + fl.No + ".gif' target=_blank  >" + fl.Name + "流程图</a></h3>";

                msg += "\r\n<UL>";
                Nodes nds = fl.HisNodes;
                foreach (Node nd in nds)
                {
                    msg += "\r\n<li><a href='./" + fl.No + "/" + nd.NodeID + "_" + nd.FlowName + "_" + nd.Name + "表单.doc' target=_blank>步骤" + nd.Step + ", - " + nd.Name + "模板</a> -<a href='./" + fl.No + "/" + nd.NodeID + "_" + nd.Name + "_表单模板.htm' target=_blank>Html版</a></li>";
                }
                msg += "\r\n</UL>";
            }
            msg += "\r\n</body>";
            msg += "\r\n</html>";

            try
            {
                string pathDef = SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\" + SystemConfig.CustomerNo + "_index.htm";
                DataType.WriteFile(pathDef, msg);

                pathDef = SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\index.htm";
                DataType.WriteFile(pathDef, msg);
                System.Diagnostics.Process.Start(SystemConfig.PathOfWorkDir + "\\VisualFlow\\DataUser\\FlowDesc\\");
            }
            catch
            {
            }
        }
        #endregion 查询

        #region 查询
        /// <summary>
        /// 查出来全部的自动流程
        /// </summary>
        public void RetrieveIsAutoWorkFlow()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowAttr.FlowType, 1);
            qo.addOrderBy(FlowAttr.No);
            qo.DoQuery();
        }
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string flowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowAttr.FK_FlowSort, flowSort);
            qo.addOrderBy(FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flows() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public Flows(string fk_sort)
        {
            this.Retrieve(FlowAttr.FK_FlowSort, fk_sort);
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
                return new Flow();
            }
        }
        #endregion
    }
}

