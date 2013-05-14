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
namespace CCFlow.WF.UC
{
    public partial class UCReturnWork : BP.Web.UC.UCBase3
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int FID
        {
            get
            {
                
                    return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Int64 WorkID
        {
            get
            {

                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public DDL DDL1
        {
            get
            {
                return this.ToolBar1.GetDDLByID("DDL1");
            }
        }
        public TextBox TB1
        {
            get
            {
                return this.Pub1.GetTextBoxByID("TB_Doc");
            }
        }
        #endregion 属性

        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "工作退回";
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            this.ToolBar1.Add("<b>退回到:</b>");
            this.ToolBar1.AddDDL("DDL1");
            this.DDL1.Attributes["onchange"] = "OnChange(this);";
            this.ToolBar1.AddBtn("Btn_OK", "确定");
            this.ToolBar1.GetBtnByID("Btn_OK").Attributes["onclick"] = " return confirm('您确定要执行吗?');";
            this.ToolBar1.GetBtnByID("Btn_OK").Click += new EventHandler(ReturnWork_Click);
            this.ToolBar1.AddBtn("Btn_Cancel", "取消");
            this.ToolBar1.GetBtnByID("Btn_Cancel").Click += new EventHandler(ReturnWork_Click);
            string appPath = this.Request.ApplicationPath;
            if (nd.IsBackTracking)
            {
                /*如果允许原路退回*/
                CheckBox cb = new CheckBox();
                cb.ID = "CB_IsBackTracking";
                cb.Text = "退回后是否要原路返回?";
                this.ToolBar1.Add(cb);
            }

            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.ID = "TB_Doc";
            tb.Rows = 15;
            tb.Columns = 50;
            this.Pub1.Add(tb);
            if (this.IsPostBack == false)
            {
                try
                {
                    DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);
                    foreach (DataRow dr in dt.Rows)
                    {
                        this.DDL1.Items.Add(new ListItem(dr["RecName"] + "=>" + dr["Name"].ToString(), dr["No"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    this.Pub1.AddMsgOfWarning("提示:", "@:下列原因造成不能退回" + ex.Message);
                }

                try
                {
                    WorkNode wn = new WorkNode(this.WorkID, this.FK_Node);
                    WorkNode pwn = wn.GetPreviousWorkNode();
                    this.DDL1.SetSelectItem(pwn.HisNode.NodeID);
                    this.DDL1.Enabled = true;
                    Work wk = pwn.HisWork;
                    if (wn.HisNode.FocusField != "")
                    {
                        this.TB1.Text = wn.HisWork.GetValStrByKey(wn.HisNode.FocusField);
                    }
                    else
                    {
                        string info = this.DDL1.SelectedItem.Text;
                        string recName = info.Substring(0, info.IndexOf('='));
                        string nodeName = info.Substring(info.IndexOf('>') + 1);
                        this.TB1.Text = string.Format("{0}同志: \n  您处理的“{1}”工作有错误，需要您重新办理．\n\n此致!!!   \n\n  {2}", recName,
                            nodeName, wk.CDT, pwn.HisNode.Name, WebUser.Name + "\n  " + BP.DA.DataType.CurrentDataTime);
                    }
                }
                catch
                {
                }
            }
        }
        void ReturnWork_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.ID)
            {
                case "Btn_Cancel":
                    this.Response.Redirect("MyFlow" + Glo.FromPageType + ".aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                    return;
                default:
                    break;
            }

            try
            {
                string returnInfo = this.TB1.Text;
                int reNode = this.DDL1.SelectedItemIntVal;
                bool IsBackTracking = false;
                try
                {
                    IsBackTracking = this.ToolBar1.GetCBByID("CB_IsBackTracking").Checked;
                }
                catch
                {
                }

                //执行退回api.
                string rInfo = BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, this.FID,
                    this.FK_Node, reNode, returnInfo, IsBackTracking);
                this.ToMsg(rInfo, "info");
                return;
            }
            catch (Exception ex)
            {
                this.ToMsg(ex.Message, "info");
            }
        }
        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Response.Redirect("MyFlowInfo" + Glo.FromPageType + ".aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
    }

}