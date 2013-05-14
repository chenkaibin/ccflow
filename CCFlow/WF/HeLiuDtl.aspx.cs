using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.DA;
using BP.En;

namespace CCFlow.WF
{
    public partial class WF_WorkOpt_HeLiuDtl : System.Web.UI.Page
    {
        public int WorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["OID"]);
                }
            }
        }

        public int FK_Node
        {
            get
            {
                return DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            if (nd.HisNodeWorkType == NodeWorkType.WorkHL || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
            {
                GenerWorkerLists wls = new GenerWorkerLists();
                QueryObject qo = new QueryObject(wls);
                qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
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
                    qo.DoQuery();
                }


                this.Pub1.AddFieldSet("分流信息");
                this.Pub1.AddTable("border=0"); // ("<table border=0 >");
                this.Pub1.AddTR();
                this.Pub1.AddTDTitle("节点");
                this.Pub1.AddTDTitle("处理人");
                this.Pub1.AddTDTitle("名称");

                this.Pub1.AddTDTitle("部门");
                this.Pub1.AddTDTitle("状态");
                this.Pub1.AddTDTitle("应完成日期");
                this.Pub1.AddTDTitle("实际完成日期");
                this.Pub1.AddTDTitle("");
                this.Pub1.AddTREnd();

                bool isHaveRuing = false;
                bool is1 = false;
                foreach (GenerWorkerList wl in wls)
                {
                    is1 = this.Pub1.AddTR(is1);
                    this.Pub1.AddTD(wl.FK_NodeText);
                    this.Pub1.AddTD(wl.FK_Emp);

                    this.Pub1.AddTD(wl.FK_EmpText);
                    this.Pub1.AddTD(wl.FK_DeptT);

                    if (wl.IsPass)
                    {
                        this.Pub1.AddTD("已完成");
                        this.Pub1.AddTD(wl.SDT);
                        this.Pub1.AddTD(wl.RDT);
                    }
                    else
                    {
                        this.Pub1.AddTD("未完成");
                        this.Pub1.AddTD(wl.SDT);
                        this.Pub1.AddTD();
                    }

                    if (wl.IsPass == false)
                    {
                        isHaveRuing = true;
                        if (nd.IsForceKill)
                            this.Pub1.AddTD("<a href=\"javascript:DoDelSubFlow('" + wl.FK_Flow + "','" + wl.WorkID + "')\"><img src='/WF/Img/Btn/Delete.gif' border=0/>终止</a>");
                        else
                            this.Pub1.AddTD();
                    }
                    else
                    {
                        this.Pub1.AddTD("<a href=\"javascript:WinOpen('FHLFlow.aspx?WorkID=" + wl.WorkID + "&FID=" + wl.FID + "&FK_Flow=" + nd.FK_Flow + "&FK_Node=" + this.FK_Node + "')\">打开</a>");
                    }
                    this.Pub1.AddTREnd();
                }

                if (isHaveRuing)
                {
                    //if (nd.IsForceKill == false)
                    //   // this.Btn_Send.Enabled = false;
                }
                this.Pub1.AddTableEnd();
                this.Pub1.AddFieldSetEnd(); //.AddFieldSet("分流信息");
            }
        }
    }
}