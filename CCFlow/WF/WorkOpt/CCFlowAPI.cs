using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.Port;
using Silverlight.DataSetConnector;
using System.Drawing.Imaging;
using System.Drawing;
using System.Configuration;

/// <summary>
///ccflowAPI 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class CCFlowAPI : CCForm {

    public CCFlowAPI()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }
     
    /// <summary>
    /// 获取当前操作员可以发起的流程集合
    /// </summary>
    /// <param name="userNo">人员编号</param>
    /// <returns>可以发起的xml</returns>
    [WebMethod(EnableSession = true)]
    public string DB_GenerCanStartFlowsOfDataTable(string userNo)
    {
        System.Data.DataSet ds = new System.Data.DataSet();
        ds.Tables.Add(BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(userNo));
        return Connector.ToXml(ds);
        //return DataSetToXml(ds);
    }
    public string DataSetToXml(DataSet ds)
    {
        string strs = "";
        strs += "<DataSet>";
        foreach (DataTable dt in ds.Tables)
        {
            strs += "\t\n<" + dt.TableName + ">";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "\t\n< ";
                foreach (DataColumn dc in dt.Columns)
                {
                    strs += dc.ColumnName + "='" + dr[dc.ColumnName] + "' ";
                }
                strs += "/>";
            }
            strs += "\t\n</" + dt.TableName + ">";
        }
        strs += "\t\n</DataSet>";
        return strs;
    }
    /// <summary>
    /// 待办提示
    /// </summary>
    /// <param name="userNo"></param>
    /// <returns></returns>
    [WebMethod]
    public string AlertString(string userNo)
    {
        return "@EmpWorks=12@CC=34";
    }
    /// <summary>
    /// 用户登录
    /// 0,密码用户名错误
    /// 1,成功.
    /// 2,服务器错误.
    /// </summary>
    /// <param name="userNo"></param>
    /// <param name="pass"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public int Port_Login(string userNo, string pass)
    {
        try
        {
            Emp emp = new Emp(userNo);
            if (emp.Pass.Equals(pass) == false)
                return 0;

            BP.WF.Dev2Interface.Port_Login(userNo, "");
            return 1;
        }
        catch(Exception ex)
        {
            Log.DefaultLogWriteLineError(ex.Message);
            return 2;
        }
    }
    /// <summary>
    /// 获取一条待办工作
    /// </summary>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="userNo">操作员编号</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string GenerWorkNode(string fk_flow, int fk_node, Int64 workID, Int64 fid, string userNo)
    {
        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            MapData md = new MapData();
            md.No = "ND" + fk_node;
            if (md.RetrieveFromDBSources() == 0)
                throw new Exception("装载错误，该表单ID=" + md.No + "丢失，请修复一次流程重新加载一次.");

            DataSet myds = md.GenerHisDataSet();

            // 节点数据.
            Node nd = new Node(fk_node);
            myds.Tables.Add(nd.ToDataTableField("WF_Node"));

            //节点标签数据.
            BtnLab btnLab = new BtnLab(fk_node);
            myds.Tables.Add(btnLab.ToDataTableField("WF_BtnLab"));

            // 流程数据.
            Flow fl = new Flow(fk_flow);
            myds.Tables.Add(fl.ToDataTableField("WF_Flow"));

            //.工作数据放里面去, 放进去前执行一次装载前填充事件.
            BP.WF.Work wk = nd.HisWork;
            wk.OID = workID;
            wk.RetrieveFromDBSources();
            wk.ResetDefaultVal();
            // 执行一次装载前填充.
            string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, wk);
            if (string.IsNullOrEmpty(msg) == false)
                return msg;
            myds.Tables.Add(wk.ToDataTableField("Main"));

            #region 获取明细表数据，并把它加入dataset里.
            if (md.MapDtls.Count > 0)
            {
                foreach (MapDtl dtl in md.MapDtls)
                {
                    GEDtls dtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
                    try
                    {
                        qo = new QueryObject(dtl);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                qo.addAnd();
                                qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, workID);
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, workID);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                    DataTable dtDtl = qo.DoQueryToTable();
                    dtDtl.TableName = dtl.No; //修改明细表的名称.
                    myds.Tables.Add(dtDtl); //加入这个明细表.
                }
            }
            #endregion

            //把流程信息表发送过去.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            myds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            if (gwf.WFState == WFState.Forward)
            {
                //如果是转发.
                BP.WF.ForwardWorks fws = new ForwardWorks();
                fws.Retrieve(ForwardWorkAttr.WorkID, workID, ForwardWorkAttr.FK_Node, fk_node);
                myds.Tables.Add(fws.ToDataTableField("WF_ForwardWork"));
            }

            if (gwf.WFState == WFState.ReturnSta)
            {
                //如果是退回.
                ReturnWorks rts = new ReturnWorks();
                rts.Retrieve(ReturnWorkAttr.WorkID, workID, ReturnWorkAttr.ReturnToNode, fk_node);
                myds.Tables.Add(rts.ToDataTableField("WF_ForwardWork"));
            }

            if (gwf.WFState == WFState.HungUp)
            {
                //如果是挂起.
                HungUps hups = new HungUps();
                hups.Retrieve(HungUpAttr.WorkID, workID, HungUpAttr.FK_Node, fk_node);
                myds.Tables.Add(hups.ToDataTableField("WF_HungUp"));
            }

            //放入track信息.
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND"+int.Parse(fk_flow)+"Track WHERE WorkID=" + BP.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            dt.TableName = "Track";
            myds.Tables.Add(dt);

            //写入数据.
            myds.WriteXml("c:\\sss.xml");

            //转化成它所能识别的格式.
            return Silverlight.DataSetConnector.Connector.ToXml(myds);
        }
        catch (Exception ex)
        {
            Log.DebugWriteError(ex.StackTrace);
            return "@生成工作FK_Flow=" + fk_flow + ",FK_Node=" + fk_node + ",WorkID=" + workID + ",FID=" + fid + "错误,错误信息:" + ex.Message;
        }
    }
    /// <summary>
    /// 获取一条待办工作
    /// </summary>
    /// <param name="fk_flow">工作编号</param>
    /// <param name="fk_node">节点编号</param>
    /// <param name="workID">工作ID</param>
    /// <param name="userNo">操作员编号</param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_SaveWork(string fk_flow, int fk_node, Int64 workID, string userNo, string dsXml)
    {
        try
        {
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            DataSet ds = Silverlight.DataSetConnector.Connector.FromXml(dsXml);
            Hashtable htMain = new Hashtable();
            DataTable dtMain = ds.Tables["Main"]; //获得约定的主表数据.
            foreach (DataRow dr in dtMain.Rows)
            {
                htMain.Add(dr[0].ToString(), dr[1].ToString());
            }

            return BP.WF.Dev2Interface.Node_SaveWork(fk_flow, fk_node, workID, htMain, null);
        }
        catch (Exception ex)
        {
            return "@保存工作出现错误:" + ex.Message;
        }
    }
    /// <summary>
    /// 执行发送
    /// </summary>
    /// <param name="fk_flow"></param>
    /// <param name="fk_node"></param>
    /// <param name="workID"></param>
    /// <param name="userNo"></param>
    /// <param name="dsXml"></param>
    /// <returns></returns>
    [WebMethod(EnableSession = true)]
    public string Node_SendWork(string fk_flow, int fk_node, Int64 workID, string userNo, string dsXml)
    {
        try
        {
            BP.WF.Dev2Interface.Port_Login(userNo);

            StringReader sr = new StringReader(dsXml);
            DataSet ds = new DataSet();
            ds.ReadXml(sr);

            Hashtable htMain = new Hashtable();
            DataTable dtMain = ds.Tables["Main"];
            foreach (DataRow dr in dtMain.Rows)
            {
                htMain.Add(dr[0].ToString(), dr[1].ToString());
            }

            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID, htMain, ds);
            return objs.ToMsgOfText();
        }
        catch (Exception ex)
        {
            return "@发送工作出现错误:" + ex.Message;
        }
    }
}
