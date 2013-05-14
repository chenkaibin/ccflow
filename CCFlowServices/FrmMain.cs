using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Common;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using BP.WF;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.DA;
using BP;
using BP.Web;

namespace SMSServices
{
    public enum ScanSta
    {
        Working,
        Pause,
        Stop
    }
    public partial class FrmMain : Form
    {
        delegate void SetTextCallback(string text);

        public void Test_Insert_Model1()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            int i = 0;
            DateTime dtNow = DateTime.Now;
            while (i != 100000)
            {
                i++;
                string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";
                DBAccess.RunSQL(sql);
            }
            DateTime dtEnd = DateTime.Now;

            TimeSpan ts = dtEnd - dtNow;
            MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());
        }

        public void Test_Insert_Model2()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = SystemConfig.AppCenterDSN;
                conn.Open();
            }
            try
            {

                int i = 0;
                DateTime dtNow = DateTime.Now;
                while (i != 100000)
                {
                    i++;
                    string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                     cmd.ExecuteNonQuery();

                }
                DateTime dtEnd = DateTime.Now;

                TimeSpan ts = dtEnd - dtNow;
                MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());
                conn.Close();
            }
            catch (System.Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
        }

        public void Test_T_1()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            int i = 0;
            DateTime dtNow = DateTime.Now;

            DBAccess.DoTransactionBegin();
            while (i != 10)
            {
                i++;
                string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";
                DBAccess.RunSQL(sql);
            }
            DBAccess.DoTransactionCommit();

            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtNow;
            MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());

        }

        public void Test_T_2()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = SystemConfig.AppCenterDSN;
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION", conn);
            try
            {
                cmd.ExecuteNonQuery();

                int i = 0;
                DateTime dtNow = DateTime.Now;
                while (i != 10)
                {
                    i++;
                    string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";

                    cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                DateTime dtEnd = DateTime.Now;
                TimeSpan ts = dtEnd - dtNow;

                cmd.CommandText = "commit transaction";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                cmd.CommandText = "rollback transaction";
                cmd.ExecuteNonQuery();
                //DBAccess.DoTransactionRollback();
                conn.Close();
            }
        }
        public FrmMain()
        {
            //Test_T_1();
            //return;
            //Test_T_2();
            //return;
            ///* 两者相差在 20% 左右.*/
            //this.Test_Insert_Model1();
            //this.Test_Insert_Model2();
            //return;

            InitializeComponent();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.notifyIcon1.Visible = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            // this.toolStripStatusLabel1.Text = "服务停止状态...";
            this.toolStripStatusLabel1.Text = "服务暂停";
            this.textBox1.Text = "服务停止...";
            this.Btn_StartStop.Text = "启动";
        }

        Thread thread = null;
        private void Btn_StartStop_Click(object sender, EventArgs e)
        {

            #region 升级脚本.
            try
            {
                BP.DA.DBAccess.RunSQL("alter table GPM.dbo.RecordMsg alter column  SendUserID nvarchar(900)");
            }catch
            {
            }
            #endregion

            if (this.Btn_StartStop.Text == "启动")
            {
                if (this.thread == null)
                {
                    ThreadStart ts = new ThreadStart(RunIt);
                    thread = new Thread(ts);
                    thread.Start();
                    this.Btn_StartStop.Text = "暂停";
                }
                this.HisScanSta = ScanSta.Working;
                this.SetText("服务启动***********" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                this.Btn_StartStop.Text = "暂停";
                this.toolStripStatusLabel1.Text = "服务启动";
            }
            else
            {
                this.SetText("服务暂停***********" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                this.HisScanSta = ScanSta.Pause;
                this.Btn_StartStop.Text = "启动";
                this.toolStripStatusLabel1.Text = "服务暂停";
            }
        }
        public ScanSta HisScanSta = ScanSta.Pause;
        /// <summary>
        /// 执行自动启动流程任务 WF_Task 
        /// </summary>
        public void DoTask()
        {
            string sql = "SELECT * FROM WF_Task WHERE TaskSta=0 ORDER BY Starter";
            DataTable dt = null;
            try
            {
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            catch
            {
                Task ta = new Task();
                ta.CheckPhysicsTable();
                dt = DBAccess.RunSQLReturnTable(sql);
            }

            if (dt.Rows.Count == 0)
                return;

            #region 自动启动流程
            foreach (DataRow dr in dt.Rows)
            {
                string mypk = dr["MyPK"].ToString();
                string taskSta = dr["TaskSta"].ToString();
                string paras = dr["Paras"].ToString();
                string starter = dr["Starter"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();

                string startDT = dr[TaskAttr.StartDT].ToString();
                if (string.IsNullOrEmpty(startDT) == false)
                {
                    /*如果设置了发起时间,就检查当前时间是否与现在的时间匹配.*/
                    if (DateTime.Now.ToString("yyyy-MM-dd HH:mm").Contains(startDT) == false)
                        continue;
                }

                Flow fl = new Flow(fk_flow);
                this.SetText("开始执行(" + starter + ")发起(" + fl.Name + ")流程.");
                try
                {
                    string fTable = "ND" + int.Parse(fl.No + "01").ToString();
                    MapData md = new MapData(fTable);
                     sql = "";
                 //   sql = "SELECT * FROM " + md.PTable + " WHERE MainPK='" + mypk + "' AND WFState=1";
                    try
                    {
                        if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                            continue;
                    }
                    catch
                    {
                        this.SetText("开始节点表单表:" + fTable + "没有设置的默认字段MainPK. " + sql);
                        continue;
                    }

                    if (BP.Web.WebUser.No != starter)
                    {
                        BP.Web.WebUser.Exit();
                        BP.Port.Emp empadmin = new BP.Port.Emp(starter);
                        BP.Web.WebUser.SignInOfGener(empadmin);
                    }

                    Work wk = fl.NewWork();
                    string[] strs = paras.Split('@');
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        if (str.Contains("=") == false)
                            continue;

                        string[] kv = str.Split('=');
                        wk.SetValByKey(kv[0], kv[1]);
                    }

                    wk.SetValByKey("MainPK", mypk);
                    wk.Update();

                    WorkNode wn = new WorkNode(wk, fl.HisStartNode);
                    string msg = wn.NodeSend().ToMsgOfText();
                    msg = msg.Replace("'", "~");
                    DBAccess.RunSQL("UPDATE WF_Task SET TaskSta=1,Msg='" + msg + "' WHERE MyPK='" + mypk + "'");
                }
                catch (Exception ex)
                {
                    //如果发送错误。
                    this.SetText(ex.Message);
                    string msg = ex.Message;
                    try
                    {
                        DBAccess.RunSQL("UPDATE WF_Task SET TaskSta=2,Msg='" + msg + "' WHERE MyPK='" + mypk + "'");
                    }
                    catch
                    {
                        Task TK = new Task();
                        TK.CheckPhysicsTable();
                    }
                }
            }
            #endregion 自动启动流程
        }
        /// <summary>
        /// 执行线程.
        /// </summary>
        public void RunIt()
        {
            BP.WF.Flows fls = new BP.WF.Flows();
            fls.RetrieveAll();

            HisScanSta = ScanSta.Working;
            while (true)
            {
                System.Threading.Thread.Sleep(20000);
                while (this.HisScanSta == ScanSta.Pause)
                {
                    System.Threading.Thread.Sleep(3000);
                    if (this.checkBox1.Checked)
                        Console.Beep();
                }

                this.SetText("********************************");

                this.SetText("扫描触发式自动发起流程表......");
                this.DoTask();

                this.SetText("扫描定时发起流程....");
                this.DoAutuFlows(fls);

                this.SetText("扫描消息表....");
                this.DoSendMsg();

                //this.SetText("向CCIM里发送消息...");
                //this.DoSendMsgOfCCIM();

                if (DateTime.Now.Hour < 18 && DateTime.Now.Hour > 8)
                {
                    /* 在工作时间段才可以执行此调度。 */
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                    if (now.Contains(":13") || now.Contains(":33") || now.Contains(":53"))
                    {
                        this.SetText("检索自动节点任务....");
                        this.DoAutoNode();
                    }
                }

                System.Threading.Thread.Sleep(1000);
                switch (this.toolStripStatusLabel1.Text)
                {
                    case "服务启动":
                        this.toolStripStatusLabel1.Text = "服务启动..";
                        break;
                    case "服务启动..":
                        this.toolStripStatusLabel1.Text = "服务启动........";
                        break;
                    case "服务启动....":
                        this.toolStripStatusLabel1.Text = "服务启动.............";
                        break;
                    default:
                        this.toolStripStatusLabel1.Text = "服务启动";
                        break;
                }
            }
        }
        /// <summary>
        /// 自动执行节点
        /// </summary>
        private void DoAutoNode()
        {
            string sql = "SELECT * FROM WF_GenerWorkerList WHERE FK_Node IN (SELECT NODEID FROM WF_Node WHERE (WhoExeIt=1 OR  WhoExeIt=2) AND IsPass=0 AND IsEnable=1) ORDER BY FK_Emp";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                int fk_node = int.Parse(dr["FK_Node"].ToString());
                string fk_emp = dr["FK_Emp"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();

                try
                {
                    if (WebUser.No != fk_emp)
                    {
                        WebUser.Exit();
                        Emp emp = new Emp(fk_emp);
                        WebUser.SignInOfGener(emp);
                    }
                    string msg = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                    this.SetText("@处理:" + WebUser.No + ",WorkID=" + workid + ",正确处理:" + msg);
                }
                catch (Exception ex)
                {
                    this.SetText("@处理:" + WebUser.No + ",WorkID=" + workid + ",工作信息:" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        private void DoSendMsg()
        {
            int idx = 0;
            #region 发送消息
            BP.Sys.SMSs sms = new BP.Sys.SMSs();
            BP.En.QueryObject qo = new BP.En.QueryObject(sms);
            sms.Retrieve(BP.Sys.SMSAttr.MsgSta, (int)BP.Sys.MsgSta.UnRun);
            foreach (BP.Sys.SMS sm in sms)
            {
                if (this.HisScanSta == ScanSta.Stop)
                    return;

                while (this.HisScanSta == ScanSta.Pause)
                {
                    if (this.HisScanSta == ScanSta.Stop)
                        return;

                    System.Threading.Thread.Sleep(3000);

                    if (this.checkBox1.Checked)
                        Console.Beep();
                }

                if (sm.MsgAccepter.Length == 0)
                {
                    sm.HisMsgSta = BP.Sys.MsgSta.RunOK;
                    sm.Update();
                    continue;
                }
                try
                {
                    this.SetText("@执行：send email: " + sm.MsgAccepter);
                    this.SendMail(sm);

                    idx++;
                    this.SetText("已完成 , 第:" + idx + " 个.");
                    this.SetText("--------------------------------");

                    if (this.checkBox1.Checked)
                        Console.Beep();
                }
                catch (Exception ex)
                {
                    this.SetText("@错误：" + ex.Message);
                }
            }
            #endregion 发送消息
        }
        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="fls"></param>
        private void DoAutuFlows(BP.WF.Flows fls)
        {
            #region 自动启动流程
            foreach (BP.WF.Flow fl in fls)
            {
                if (fl.IsOK == false
                    || fl.HisFlowRunWay == BP.WF.FlowRunWay.HandWork)
                    continue;

                if (DateTime.Now.ToString("HH:mm") == fl.Tag)
                    continue;

                if (fl.RunObj == null || fl.RunObj == "")
                {
                    string msg = "您设置自动运行流程错误，没有设置流程内容，流程编号：" + fl.No;
                    this.SetText(msg);
                    continue;
                }

                #region 判断当前时间是否可以运行它。
                string nowStr = DateTime.Now.ToString("yyyy-MM-dd,HH:mm");
                string[] strs = fl.RunObj.Split('@'); //破开时间串。
                bool IsCanRun = false;
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (nowStr.Contains(str))
                        IsCanRun = true;
                }
                if (IsCanRun == false)
                    continue;

                // 设置时间.
                fl.Tag = DateTime.Now.ToString("HH:mm");
                #endregion 判断当前时间是否可以运行它。

                // 以此用户进入.
                switch (fl.HisFlowRunWay)
                {
                    case BP.WF.FlowRunWay.SpecEmp: //指定人员按时运行。
                        string RunObj = fl.RunObj;
                        string fk_emp = RunObj.Substring(0, RunObj.IndexOf('@'));

                        BP.Port.Emp emp = new BP.Port.Emp();
                        emp.No = fk_emp;
                        if (emp.RetrieveFromDBSources() == 0)
                        {
                            this.SetText("启动自动启动流程错误：发起人(" + fk_emp + ")不存在。");
                            continue;
                        }
                        BP.Web.WebUser.SignInOfGener(emp);
#warning 尚未实现。
                       //string info_send= BP.WF.Dev2Interface.Node_StartWork(fl.No,);
                       //this.SetText(info_send);
                        continue;
                    case BP.WF.FlowRunWay.DataModel: //按数据集合驱动的模式执行。
                        this.SetText("@开始执行数据驱动流程调度:" + fl.Name);
                        this.DTS_Flow(fl);
                        continue;
                    default:
                        break;
                }
            }
            if (BP.Web.WebUser.No != "admin")
            {
                BP.Port.Emp empadmin = new BP.Port.Emp("admin");
                BP.Web.WebUser.SignInOfGener(empadmin);
            }
            #endregion 发送消息
        }
        public void DTS_Flow(BP.WF.Flow fl)
        {
            #region 读取数据.
            BP.Sys.MapExt me = new MapExt();
            me.MyPK = "ND" + int.Parse(fl.No) + "01" + "_" + MapExtXmlList.StartFlow;
            int i = me.RetrieveFromDBSources();
            if (i == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }
            if (string.IsNullOrEmpty(me.Tag))
            {
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
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
            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Rows.Count == 0)
            {
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")此时无任务.");
                this.SetText("流程(" + fl.Name + ")此时无任务.");
                return;
            }

            this.SetText("@查询到(" + dtMain.Rows.Count + ")条任务.");

            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                this.SetText(errMsg);
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);
                return;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.
            string nodeTable = "ND" + int.Parse(fl.No) + "01";
            int idx = 0;
            foreach (DataRow dr in dtMain.Rows)
            {
                idx++;

                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + nodeTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                {
                    this.SetText("@" + fl.Name + ",第" + idx + "条,此任务在之前已经完成。");
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
                        this.SetText("@" + fl.Name + ",第" + idx + "条,设置的发起人员:" + emp.No + "不存在.");
                        BP.DA.Log.DefaultLogWriteLineInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + emp.No + "不存在。");
                        continue;
                    }
                    WebUser.SignInOfGener(emp);
                }

                #region  给值.
                //System.Collections.Hashtable ht = new Hashtable();

                Work wk = fl.NewWork();

                string err = "";
                #region 检查用户拼写的sql是否正确？
                foreach (DataColumn dc in dtMain.Columns)
                {
                    string f = dc.ColumnName.ToLower();
                    switch (f)
                    {
                        case "starter":
                        case "mainpk":
                        case "refmainpk":
                        case "tonode":
                            break;
                        default:
                            bool isHave = false;
                            foreach (Attr attr in wk.EnMap.Attrs)
                            {
                                if (attr.Key.ToLower() == f)
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                            {
                                err += " " + f + " ";
                            }
                            break;
                    }
                }
                if (string.IsNullOrEmpty(err) == false)
                    throw new Exception("您设置的字段:" + err + "不存在开始节点的表单中，设置的sql:" + me.Tag);

                #endregion 检查用户拼写的sql是否正确？

                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    // MapData md = new MapData(nodeTable);
                    MapDtls dtls = new MapDtls(nodeTable);
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


                int toNodeID = 0;
                try
                {
                    toNodeID = int.Parse(dr["ToNode"].ToString());
                }
                catch
                {
                    /*有可能在4.5以前的版本中没有tonode这个约定.*/
                }

                // 处理发送信息.
                //  Node nd =new Node();
                string msg = "";
                try
                {
                    if (toNodeID == 0)
                    {
                        WorkNode wn = new WorkNode(wk, fl.HisStartNode);
                        msg = wn.NodeSend().ToMsgOfText();
                    }

                    if (toNodeID == fl.StartNodeID)
                    {
                        /* 发起后让它停留在开始节点上，就是为开始节点创建一个待办。*/
                        Int64 workID = BP.WF.Dev2Interface.Node_CreateStartNodeWork(fl.No, null, null, WebUser.No, null);
                        if (workID != wk.OID)
                            throw new Exception("@异常信息:不应该不一致的workid.");
                        else
                            wk.Update();
                        msg = "已经为(" + WebUser.No + ") 创建了开始工作节点. ";
                    }

                    BP.DA.Log.DefaultLogWriteLineInfo(msg);
                    this.SetText("@" + fl.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "已完成.\r\n" + msg);
                }
                catch (Exception ex)
                {
                    this.SetText("@" + fl.Name + ",第" + idx + "条,发起人员:" + WebUser.No + "-" + WebUser.Name + "发起时出现错误.\r\n" + ex.Message);
                    BP.DA.Log.DefaultLogWriteLineWarning(ex.Message);
                }
            }
            #endregion 处理流程发起.
        }
        /// <summary>
        /// 发送邮件。
        /// </summary>
        /// <param name="sms"></param>
        public void SendMail(BP.Sys.SMS sms)
        {
            //如果向 ccim 写入消息。
            if (this.CB_IsWriteToCCIM.Checked)
            {
                try
                {
                    Glo.SendMessage(sms.MyPK, DateTime.Now.ToString(), sms.Title + "\t\n" + sms.Doc, sms.MsgAccepter);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误");
                    return;
                }
            }

            if (string.IsNullOrEmpty(sms.MsgAccepter))
            {
                BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(sms.SendToEmpID);
                if (sms.MsgType == 0)
                {
                    sms.MsgAccepter = emp.Email;
                }
                else
                {
                    sms.MsgAccepter = emp.Tel;
                }
            }

            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
            myEmail.From = new MailAddress("ccflow.cn@gmail.com", "ccflow", System.Text.Encoding.UTF8);

            myEmail.To.Add(sms.MsgAccepter);
            myEmail.Subject = sms.Title;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

            myEmail.Body = sms.Doc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件

            myEmail.Priority = MailPriority.High;//邮件优先级

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(SystemConfig.GetValByKey("SendEmailAddress", "ccflow.cn@gmail.com"),
                SystemConfig.GetValByKey("SendEmailPass", "ccflow123"));
            //上述写你的邮箱和密码
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 587); //使用的端口
            client.Host = SystemConfig.GetValByKey("SendEmailHost","smtp.gmail.com");
            client.EnableSsl = SystemConfig.GetValByKeyBoolen("SendEmailEnableSsl", true); 

            object userState = myEmail;
            try
            {
                client.SendAsync(myEmail, userState);
                sms.HisMsgSta = BP.Sys.MsgSta.RunOK;
                sms.Update();
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                throw ex;
            }
        }
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);

                try
                {
                    this.Invoke(d, new object[] { text });
                }
                catch
                {
                }
            }
            else
            {
                this.textBox1.Text += "\r\n" + text;
                this.textBox1.SelectionStart = this.textBox1.TextLength;
                this.textBox1.ScrollToCaret();
            }
        }

        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            if (thread != null)
            {
                this.HisScanSta = ScanSta.Stop;
                thread.Abort();
            }
            this.Close();
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            this.Hide();
            this.notifyIcon1.Visible = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BP.WF.Flow fl = new BP.WF.Flow("040");
            BP.WF.Dev2Interface.DTS_AutoStarterFlow(fl);
        }

        private void Btn_ToolBox_Click(object sender, EventArgs e)
        {
            CCFlowServices.ToolBox tb = new CCFlowServices.ToolBox();
            tb.Show();
        }

       
    }
}
