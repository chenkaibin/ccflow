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
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.UC
{
    public partial class AllotTask : BP.Web.UC.UCBase3
    {
        /// <summary>
        /// WorkID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        /// <summary>
        /// FID
        /// </summary>
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
        /// <summary>
        /// IsFHL
        /// </summary>
        public bool IsFHL
        {
            get
            {
                if (this.WorkID == this.FID)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// NodeID
        /// </summary>
        public int NodeID
        {
            get
            {
                return int.Parse(this.Request.QueryString["NodeID"]);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// dd
        /// </summary>
        public void BindLB()
        {
            // 当前用的员工权限。 
            this.Clear();

            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeID, true);
            string sql = "SELECT * FROM WF_RememberMe WHERE FK_Emp='" + WebUser.No + "' AND FK_Node=" + this.NodeID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (WebUser.IsWap)
                this.AddFieldSet("<a href='./WAP/Home.aspx' ><img src='/WF/Img/Home.gif' border=0/>主页</a> - 工作分配");
            else
                this.AddFieldSet("工作分配");

            if (dt.Rows.Count == 0)
                throw new Exception("@系统错误..." + sql);

            string[] objs = dt.Rows[0]["Objs"].ToString().Split('@');
            string[] emps = dt.Rows[0]["Emps"].ToString().Split('@');

            string ids = "";
            this.AddUL();
            foreach (string fk_emp in emps)
            {
                if (fk_emp == null || fk_emp == "")
                    continue;

                Emp emp = new Emp(fk_emp);
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + fk_emp;
                ids += "," + cb.ID;

                if (Glo.IsShowUserNoOnly)
                    cb.Text = emp.No;
                else
                    cb.Text = emp.No + "  , " + emp.Name;

                GenerWorkerList wl = wls.GetEntityByKey(GenerWorkerListAttr.FK_Emp, fk_emp) as GenerWorkerList;
                if (wl == null)
                    cb.Checked = false;
                else
                {
                    cb.Checked = wl.IsEnable;
                }
                this.Add("<li>");
                this.Add(cb);
                this.Add("</li>");
                //this.AddBR();
            }
            this.AddULEnd();

            this.AddHR();
            Btn btn = new Btn();
            btn.ID = "Btn_Do";
            btn.Text = "  确定  ";
            btn.Click += new EventHandler(BPToolBar1_ButtonClick);
            this.Add(btn);

            CheckBox cbx = new CheckBox();
            cbx.ID = "seleall";
            cbx.Text = "选择全部";
            cbx.Checked = true;
            cbx.Attributes["onclick"] = "SetSelected(this,'" + ids + "')";
            this.Add(cbx);
            //this.Add("<input type=button value='取消' onclick='window.close();'  />");
            this.Add("<br><br>帮助:系统会记住本次的工作指定，下次您在发送时间它会自动把工作投递给您本次指定的人。");
            this.AddFieldSetEnd();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "工作分配";
            if (this.IsPostBack == false)
            {
                string fk_emp = this.Request.QueryString["FK_Emp"];
                if (fk_emp != null)
                {
                    Emp emp = new Emp(fk_emp);
                    BP.Web.WebUser.SignInOfGenerLang(emp, null);
                }
            }
            this.BindLB();
            //    this.GetBtnByID("Btn_Do").Click += new EventHandler(BPToolBar1_ButtonClick);
        }
        private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            this.Confirm();
            return;
        }
        public void Confirm()
        {
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.NodeID, true);
            string sql = "SELECT * FROM WF_RememberMe WHERE FK_Emp='" + WebUser.No + "' AND FK_Node=" + this.NodeID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count == 0)
                throw new Exception("@system error.....");

            try
            {
                string[] objs = dt.Rows[0]["Objs"].ToString().Split('@');
                string[] empStrs = dt.Rows[0]["Emps"].ToString().Split('@');

                ArrayList al = new ArrayList();
                GenerWorkerLists wlSeles = new GenerWorkerLists();

                foreach (string fk_emp in empStrs)
                {
                    if (fk_emp == null || fk_emp == "")
                        continue;

                    CheckBox cb = this.GetCBByID("CB_" + fk_emp);
                    if (cb == null)
                    {
                        BP.DA.Log.DebugWriteWarning("不应该查询不到:" + fk_emp);
                        continue;
                    }

                    if (cb.Checked == false)
                        continue;

                    GenerWorkerList wl = wls.GetEntityByKey(GenerWorkerListAttr.FK_Emp, fk_emp) as GenerWorkerList;
                    al.Add(cb.ID.Substring(3));
                    wlSeles.AddEntity(wl);
                }

                if (al.Count == 0)
                {
                    this.Alert( "当前工作中你没有分配给任何人，此工作将不能被其他人所执行！");
                    return;
                }

                if (this.IsFHL)
                {
                    /* 处理分合流 */
                    this.DealWithFHLFlow(al, wlSeles);
                }
                else
                {
                    this.DealWithPanelFlow(al, wlSeles);
                }

                // 保存记忆功能。
                RememberMe rm = new RememberMe();
                rm.FK_Emp = BP.Web.WebUser.No;
                rm.FK_Node = NodeID;
                rm.Objs = "@";
                rm.ObjsExt = "";

                foreach (GenerWorkerList mywl in wlSeles)
                {
                    rm.Objs += mywl.FK_Emp + "@";
                    rm.ObjsExt += mywl.FK_EmpText + "&nbsp;&nbsp;";
                }

                rm.Emps = "@";
                rm.EmpsExt = "";

                foreach (GenerWorkerList wl in wls)
                {
                    rm.Emps += wl.FK_Emp + "@";

                    if (rm.Objs.IndexOf(wl.FK_Emp) != -1)
                        rm.EmpsExt += "<font color=green>(" + wl.FK_Emp + ")" + wl.FK_EmpText + "</font>&nbsp;&nbsp;";
                    else
                        rm.EmpsExt += "<strike>(" + wl.FK_Emp + ")" + wl.FK_EmpText + "</strike>&nbsp;&nbsp;";
                }

                rm.FK_Emp = BP.Web.WebUser.No;
                rm.Update();


                if (WebUser.IsWap)
                {
                    this.Clear();
                    this.AddFieldSet("提示信息");
                    this.Add("<br>&nbsp;&nbsp;任务分配成功，特别提示：当下一次流程发送时系统会按照您设置的路径进行智能投递。");
                    this.AddUL();
                    this.AddLi("<a href='./WAP/Home.aspx' ><img src='/WF/Img/Home.gif' border=0/>主页</a>");
                    this.AddLi("<a href='./WAP/Start.aspx' ><img src='/WF/Img/Start.gif' border=0/>发起</a>");
                    this.AddLi("<a href='./WAP/Runing.aspx' ><img src='/WF/Img/Runing.gif' border=0/>待办</a>");
                    this.AddULEnd();
                    this.AddFieldSetEnd();
                }
                else
                {
                    this.WinCloseWithMsg("任务分配成功。");
                }
            }
            catch (Exception ex)
            {
                this.Response.Write(ex.Message);
                Log.DebugWriteWarning(ex.Message);
                this.Alert("任务分配出错：" + ex.Message);
            }
        }
        public void DealWithFHLFlow(ArrayList al, GenerWorkerLists wlSeles)
        {
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.FID, this.FID);

            DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=0  WHERE FID=" + this.FID);

            string emps = "";
            string myemp = "";
            foreach (Object obj in al)
            {
                emps += obj.ToString() + ",";
                myemp = obj.ToString();
                DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=1  WHERE FID=" + this.FID + " AND FK_Emp='" + obj + "'");
            }

            //BP.WF.Node nd = new BP.WF.Node(NodeID);
            //Work wk = nd.HisWork;
            //wk.OID = this.WorkID;
            //wk.Retrieve();
            //wk.Emps = emps;
            //wk.Update();
        }

        public void DealWithPanelFlow(ArrayList al, GenerWorkerLists wlSeles)
        {
            // 删除当前非配的工作。
            // 已经非配或者自动分配的任务。
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            int NodeID = gwf.FK_Node;
            Int64 workId = this.WorkID;
            //GenerWorkerLists wls = new GenerWorkerLists(this.WorkID,NodeID);
            DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=0  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + NodeID);
            //  string vals = "";
            string emps = "";
            string myemp = "";
            foreach (Object obj in al)
            {
                emps += obj.ToString() + ",";
                myemp = obj.ToString();
                DBAccess.RunSQL("UPDATE  WF_GenerWorkerlist SET IsEnable=1  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + NodeID + " AND fk_emp='" + obj + "'");
            }

            BP.WF.Node nd = new BP.WF.Node(NodeID);
            Work wk = nd.HisWork;

            wk.OID = this.WorkID;
            wk.Retrieve();

            wk.Emps = emps;
            wk.Update();
        }
    }
}