using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web.Controls;
using BP.Web;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_CCRole : System.Web.UI.Page
    {
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            this.Pub1.AddTable();
            this.Pub1.AddCaption("抄送规则");

            this.Pub1.AddTR();
            this.Pub1.AddTD("标题");
            TB tb = new TB();
            tb.Text = ""; // nd.HisCCRole;
            tb.ID = "TB_Title";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTableEnd();
        }
    }
}