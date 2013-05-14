using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP;
using BP.WF;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Rpt;
using BP.Sys.Xml;

namespace CCFlow.WF.Rpt
{
    public partial class WF_Rpt_G : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                {
                    throw new Exception("丢失FK_Flow参数.");
                    s = "021";
                }
                s = s.Replace("ND", "");
                s = s.Replace("Rpt", "");
                return s;
            }
        }
        public new string EnsName
        {
            get
            {
                return "ND" + int.Parse(this.FK_Flow) + "Rpt";
            }
        }
        public Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    if (this.EnsName != null)
                    {
                        if (this._HisEns == null)
                            _HisEns = BP.DA.ClassFactory.GetEns(this.EnsName);
                    }
                }
                return _HisEns;
            }
        }
        /// <summary>
        /// key
        /// </summary>
        public new string Key
        {
            get
            {
                try
                {
                    return this.ToolBar1.GetTBByID("TB_Key").Text;
                }
                catch
                {
                    return null;
                }
            }
        }
        public UserRegedit ur = null;
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsFY
        {
            get
            {
                string str = this.Request.QueryString["IsFY"];
                if (str == null || str == "0")
                    return false;
                return true;
            }
        }
        public string NumKey
        {
            get
            {
                string str = this.Request.QueryString["NumKey"];
                if (str == null)
                    return ViewState["NumKey"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["NumKey"] = value;
            }
        }
        public string OrderBy
        {
            get
            {
                string str = this.Request.QueryString["OrderBy"];
                if (str == null)
                    return ViewState["OrderBy"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["OrderBy"] = value;
            }
        }
        public string DoType
        {
            get
            {
                string s = this.Request.QueryString["DoType"];
                if (s == null)
                    s = "My";
                return s;
            }
        }
        public string OrderWay
        {
            get
            {
                string str = this.Request.QueryString["OrderWay"];
                if (str == null)
                    return ViewState["OrderWay"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["OrderWay"] = value;
            }
        }
        public bool IsReadonly
        {
            get
            {
                string i = this.Request.QueryString["IsReadonly"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }
        public bool IsShowSum
        {
            get
            {
                string i = this.Request.QueryString["IsShowSum"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }
        public bool IsContainsNDYF
        {
            get
            {
                if (this.ViewState["IsContinueNDYF"].ToString().ToUpper() == "TRUE")
                    return true;
                else
                    return false;
            }
        }
        public string CfgVal
        {
            get
            {
                return this.ViewState["CfgVal"].ToString();
            }
            set
            {
                this.ViewState["CfgVal"] = value;
            }
        }
        public MapData HisMD = null;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HisMD = new MapData(this.EnsName);
            AttrSearchs searchs = null;

            #region 处理查询设的默认.
            if (this.DoType == "My")
            {
                #region 处理查询权限
                Entity en = this.HisEns.GetNewEntity;
                Map map = en.EnMap;
                this.ToolBar1.InitByMapV2(map, 1, this.EnsName);
                //      this.ToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Export);
                searchs = map.SearchAttrs;
                string defVal = "";
                System.Data.DataTable dt = null;
                foreach (AttrSearch attr in searchs)
                {
                    DDL mydll = this.ToolBar1.GetDDLByKey("DDL_" + attr.Key);
                    if (mydll == null)
                        continue;
                    defVal = mydll.SelectedItemStringVal;
                    mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + this.EnsName + "','" + attr.Key + "')";
                    switch (attr.Key)
                    {
                        case "FK_NY":
                            dt = DBAccess.RunSQLReturnTable("SELECT DISTINCT FK_NY FROM " + this.EnsName + " WHERE FK_NY!='' ORDER BY FK_NY");
                            mydll.Items.Clear();
                            mydll.Items.Add(new ListItem("=>月份", "all"));
                            foreach (DataRow dr in dt.Rows)
                            {
                                mydll.Items.Add(new ListItem(dr[0].ToString(), dr[0].ToString()));
                            }
                            mydll.SetSelectItem(defVal);
                            break;
                        case "FlowStarter":
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM WF_Emp WHERE  FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + this.EnsName + " WHERE FlowStarter!='')");
                            mydll.Items.Clear();
                            mydll.Items.Add(new ListItem("=>发起人", "all"));
                            foreach (DataRow dr in dt.Rows)
                            {
                                mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                            }
                            mydll.SetSelectItem(defVal);
                            mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'ND" + int.Parse(this.FK_Flow) + "Rpt','" + attr.Key + "')";
                            break;
                        case "FK_Dept":
                            if (WebUser.No != "admin")
                            {
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "')");
                                if (dt.Rows.Count == 0)
                                {
                                    this.UCSys1.AddMsgOfWarning("提示", "<h2>系统管理员没有给您设置查询权限。</h2>");
                                    this.ToolBar1.Controls.Clear();
                                    return;
                                }
                                mydll.Items.Clear();
                                foreach (DataRow dr in dt.Rows)
                                    mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                            }

                            if (mydll.Items.Count >= 2)
                            {
                                ListItem liMvals = new ListItem("*多项组合..", "mvals");
                                liMvals.Attributes.CssStyle.Add("style", "color:green");
                                liMvals.Attributes.Add("color", "green");
                                liMvals.Attributes.Add("style", "color:green");
                            }
                            mydll.SetSelectItem(defVal);
                            break;
                        default:
                            break;
                    }
                }
                #endregion 处理查询权限
            }
            else
            {
                #region 处理查询权限
                Entity en = this.HisEns.GetNewEntity;
                Map map = en.EnMap;
                this.ToolBar1.InitByMapV2(map, 1, this.EnsName);
                //   this.ToolBar1.AddBtn(BP.Web.Controls.NamesOfBtn.Export);
                searchs = map.SearchAttrs;
                string defVal = "";
                System.Data.DataTable dt = null;
                foreach (AttrSearch attr in searchs)
                {
                    DDL mydll = this.ToolBar1.GetDDLByKey("DDL_" + attr.Key);
                    if (mydll == null)
                        continue;
                    defVal = mydll.SelectedItemStringVal;
                    mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + this.EnsName + "','" + attr.Key + "')";
                    switch (attr.Key)
                    {
                        case "FK_NY":
                            dt = DBAccess.RunSQLReturnTable("SELECT DISTINCT FK_NY FROM " + this.EnsName + " WHERE FK_NY!='' ORDER BY FK_NY");
                            mydll.Items.Clear();
                            mydll.Items.Add(new ListItem("=>月份", "all"));
                            foreach (DataRow dr in dt.Rows)
                            {
                                mydll.Items.Add(new ListItem(dr[0].ToString(), dr[0].ToString()));
                            }
                            mydll.SetSelectItem(defVal);
                            break;
                        case "FlowStarter":
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM WF_Emp WHERE  FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + this.EnsName + " WHERE FlowStarter!='')");
                            mydll.Items.Clear();
                            mydll.Items.Add(new ListItem("=>发起人", "all"));
                            foreach (DataRow dr in dt.Rows)
                            {
                                mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                            }
                            mydll.SetSelectItem(defVal);
                            mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'ND" + int.Parse(this.FK_Flow) + "Rpt','" + attr.Key + "')";
                            break;
                        case "FK_Dept":
                            if (WebUser.No != "admin")
                            {
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Flow='" + this.FK_Flow + "' AND FK_Emp='" + WebUser.No + "')");
                                if (dt.Rows.Count == 0)
                                {
                                    this.UCSys1.AddMsgOfWarning("提示", "<h2>系统管理员没有给您设置查询权限。</h2>");
                                    this.ToolBar1.Controls.Clear();
                                    return;
                                }
                                mydll.Items.Clear();
                                foreach (DataRow dr in dt.Rows)
                                    mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                            }

                            if (mydll.Items.Count >= 2)
                            {
                                ListItem liMvals = new ListItem("*多项组合..", "mvals");
                                liMvals.Attributes.CssStyle.Add("style", "color:green");
                                liMvals.Attributes.Add("color", "green");
                                liMvals.Attributes.Add("style", "color:green");
                            }
                            mydll.SetSelectItem(defVal);
                            break;
                        default:
                            break;
                    }
                }
                #endregion 处理查询权限

                this.ToolBar1.GetBtnByID("Btn_Search").Click += new System.EventHandler(this.ToolBar1_ButtonClick);
                //  this.ToolBar1.GetBtnByID(BP.Web.Controls.NamesOfBtn.Export).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            }
            #endregion 处理查询设的默认。

            this.CB_IsShowPict.Text = "显示图形";

            #region 权限问题
            UAC uac = new UAC();
            try
            {
                uac = this.HisEn.HisUAC;
            }
            catch
            {
                uac.IsView = true;
            }

            if (uac.IsView == false)
                throw new Exception("您没有查看[" + this.HisEn.EnDesc + "]数据的权限.");

            if (this.IsReadonly)
            {
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
            }
            #endregion 权限问题

            this.ur = new UserRegedit(WebUser.No, this.EnsName + "_Group");

            #region 设置tool bar 1 的contral
            if (uac.IsView == false)
                throw new Exception("@对不起，您没有查看的权限！");

            if (this.OrderBy != null)
            {
                if (this.OrderBy != null)
                    ur.OrderBy = this.OrderBy;

                if (this.OrderWay == "Up")
                    ur.OrderWay = "DESC";
                else
                    ur.OrderWay = "";

                if (this.NumKey == null)
                    this.NumKey = ur.NumKey;
            }

            this.OrderBy = ur.OrderBy;
            this.OrderWay = ur.OrderWay;
            this.CfgVal = ur.Vals;

            if (this.HisMD.AttrsInTableEns.Contains("FK_NY") && this.HisMD.AttrsInTableEns.Contains("FK_ND"))
            {
                this.ViewState["IsContinueNDYF"] = "TRUE";
            }
            else
            {
                this.ViewState["IsContinueNDYF"] = "FALSE";
            }

            if (this.IsPostBack == false)
            {
                string reAttrs = this.Request.QueryString["Attrs"];
                this.CheckBoxList1.Items.Clear();
                foreach (Attr attr in this.HisMD.AttrsInTableEns)
                {
                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        ListItem li = new ListItem(attr.Desc, attr.Key);
                        if (reAttrs != null)
                        {
                            if (reAttrs.IndexOf(attr.Key) != -1)
                            {
                                li.Selected = true;
                            }
                        }

                        // 根据状态 设置信息.
                        if (this.CfgVal.IndexOf(attr.Key) != -1)
                            li.Selected = true;
                        this.CheckBoxList1.Items.Add(li);
                    }
                }

                if (this.CheckBoxList1.Items.Count == 0)
                    throw new Exception(this.HisMD.Name +" 没有外键条件，不适合做分组查询" ); //没有外键条件，不适合做分组查询。

                if (this.CheckBoxList1.Items.Count == 1)
                    this.CheckBoxList1.Enabled = false;
            }
            #endregion

            #region 设置选择的 默认值
            //    searchs = this.HisMD.HisEn.EnMap.SearchAttrs;
            foreach (AttrSearch attr in searchs)
            {
                string mykey = this.Request.QueryString[attr.HisAttr.Key];
                if (mykey == "" || mykey == null)
                    continue;
                else
                    this.ToolBar1.GetDDLByKey("DDL_" + attr.HisAttr.Key).SetSelectItem(mykey, attr.HisAttr);

                #region 处理权限问题.
                DDL mydll = this.ToolBar1.GetDDLByKey("DDL_" + attr.Key);
                if (mydll == null)
                    continue;
                string defVal = mydll.SelectedItemStringVal;
                DataTable dt = null;
                mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'ND" + int.Parse(this.FK_Flow) + "Rpt','" + attr.Key + "')";
                switch (attr.Key)
                {
                    case "FK_NY":
                        dt = DBAccess.RunSQLReturnTable("SELECT DISTINCT FK_NY FROM " + this.EnsName + " WHERE FK_NY!='' ORDER BY FK_NY");
                        mydll.Items.Clear();
                        mydll.Items.Add(new ListItem("=>月份", "all"));
                        foreach (DataRow dr in dt.Rows)
                        {
                            mydll.Items.Add(new ListItem(dr[0].ToString(), dr[0].ToString()));
                        }
                        mydll.SetSelectItem(defVal);
                        break;
                    case "FlowStarter":
                        dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM WF_Emp WHERE  FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + this.EnsName + " WHERE FlowStarter!='')");
                        mydll.Items.Clear();
                        mydll.Items.Add(new ListItem("=>发起人", "all"));
                        foreach (DataRow dr in dt.Rows)
                        {
                            mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        }
                        mydll.SetSelectItem(defVal);
                        mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'ND" + int.Parse(this.FK_Flow) + "Rpt','" + attr.Key + "')";
                        break;
                    case "FK_Dept":
                        if (WebUser.No != "admin")
                        {
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "')");
                            if (dt.Rows.Count == 0)
                            {
                                BP.WF.Port.DeptFlowSearch dfs = new BP.WF.Port.DeptFlowSearch();
                                dfs.FK_Dept = WebUser.FK_Dept;
                                dfs.FK_Emp = WebUser.No;
                                dfs.FK_Flow = this.FK_Flow;
                                dfs.MyPK = WebUser.FK_Dept + "_" + WebUser.No + "_" + this.FK_Flow;
                                dfs.Insert();
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + this.FK_Flow + "')");
                            }
                            mydll.Items.Clear();
                            foreach (DataRow dr in dt.Rows)
                                mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        }

                        if (mydll.Items.Count >= 2)
                        {
                            ListItem liMvals = new ListItem("*多项组合..", "mvals");
                            liMvals.Attributes.CssStyle.Add("style", "color:green");
                            liMvals.Attributes.Add("color", "green");
                            liMvals.Attributes.Add("style", "color:green");
                        }
                        mydll.SetSelectItem(defVal);
                        break;
                    default:
                        break;
                }
                #endregion 处理权限问题.

            }
            #endregion

            this.ToolBar1.AddSpt("spt1");
            this.ToolBar1.AddBtn(NamesOfBtn.Excel, "导出到Excel");

            #region 增加排序
            if (this.IsPostBack == false)
                this.CB_IsShowPict.Checked = ur.IsPic;
            #endregion

            this.BindNums();
            if (this.IsPostBack == false)
                this.BingDG();

            this.ToolBar1.GetBtnByID("Btn_Search").Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.ToolBar1.GetBtnByID("Btn_Excel").Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.CB_IsShowPict.CheckedChanged += new EventHandler(State_Changed);
            this.CheckBoxList1.SelectedIndexChanged += new EventHandler(State_Changed);
        }

        public void BindNums()
        {
            this.UCSys2.Clear();
            // 查询出来关于它的活动列配置。
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            Attrs attrs = this.HisMD.AttrsInTableEns;
            attrs.AddTBInt("MyNum", 1, "流程数量", true, true);

            this.UCSys2.Add("<table border=0 cellPadding=0 BorderStyle=None Width=100%  >");
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                if (attr.UIVisible == false)
                    continue;

                if (attr.IsNum == false)
                    continue;

                if (attr.Key == "OID" || attr.Key == "WorkID" || attr.Key == "MID")
                    continue;

                bool isHave = false;
                // 有没有配置抵消它的属性。
                foreach (ActiveAttr aa in aas)
                {
                    if (aa.AttrKey != attr.Key)
                        continue;

                    CheckBox cb1 = new CheckBox();
                    cb1.ID = "CB_" + attr.Key;
                    cb1.Text = attr.Desc;
                    cb1.AutoPostBack = true;

                    if (this.CfgVal.IndexOf("@" + attr.Key) == -1)
                        cb1.Checked = false; /* 如果不包含 key .*/
                    else
                        cb1.Checked = true;

                    cb1.CheckedChanged += new EventHandler(State_Changed);

                    this.UCSys2.Add("<TD >");
                    this.UCSys2.Add(cb1);
                    this.UCSys2.Add("</TD>");
                    isHave = true;
                }
                if (isHave)
                    continue;

                this.UCSys2.AddTR();
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.Key;
                cb.Text = attr.Desc;
                cb.AutoPostBack = true;
                cb.CheckedChanged += new EventHandler(State_Changed);

                if (this.CfgVal.IndexOf("@" + attr.Key) == -1)
                    cb.Checked = false; /* 如果不包含 key .*/
                else
                    cb.Checked = true;

                this.UCSys2.Add("<TD style='font-size:12px;text-align:left'>");
                this.UCSys2.Add(cb);
                this.UCSys2.Add("</TD>");

                DDL ddl = new DDL();
                ddl.ID = "DDL_" + attr.Key;
                ddl.Items.Add(new ListItem("求和", "SUM"));
                ddl.Items.Add(new ListItem("求平均", "AVG"));
                if (this.IsContainsNDYF)
                    ddl.Items.Add(new ListItem("求累计", "AMOUNT"));

                //ddl.Items.Add(new ListItem(this.ToE("ForMax", "求最大"), "MAX"));
                //ddl.Items.Add(new ListItem(this.ToE("ForMin", "求最小"), "MIN"));
                //ddl.Items.Add(new ListItem(this.ToE("ForBZC", "求标准差"), "BZC"));
                //ddl.Items.Add(new ListItem(this.ToE("ForLSXS", "求离散系数"), "LSXS"));

                if (this.CfgVal.IndexOf("@" + attr.Key + "=AVG") != -1)
                {
                    ddl.SelectedIndex = 1;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=SUM") != -1)
                {
                    ddl.SelectedIndex = 0;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=AMOUNT") != -1)
                {
                    ddl.SelectedIndex = 2;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=MAX") != -1)
                {
                    ddl.SelectedIndex = 3;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=MIN") != -1)
                {
                    ddl.SelectedIndex = 4;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=BZC") != -1)
                {
                    ddl.SelectedIndex = 5;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=LSXS") != -1)
                {
                    ddl.SelectedIndex = 6;
                }

                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(State_Changed);

                this.UCSys2.Add("<TD style='font-size:12px;text-align:left'>");
                this.UCSys2.Add(ddl);
                this.UCSys2.AddTDEnd();
                this.UCSys2.AddTREnd();

                if (this.NumKey == "" || this.NumKey == null)
                {
                    this.NumKey = attr.Key;
                    this.UCSys2.GetCBByID("CB_" + attr.Key).Checked = true;
                }
            }
            this.UCSys2.AddTableEnd();

            //			//this.DDL_GroupField.Items.Add(new ListItem("个数","COUNT(*)"));
            //			this.DDL_GroupWay.Items.Add(new ListItem("求和","0"));
            //			this.DDL_GroupWay.Items.Add(new ListItem("求平均","1"));
            //
            //			this.DDL_Order.Items.Add(new ListItem("降序","0"));
            //			this.DDL_Order.Items.Add(new ListItem("升序","1"));
            //
            //
            //			//this.DDL_GroupField.Items.Add(new ListItem("个数","COUNT(*)"));
            //			this.DDL_GroupWay.Items.Add(new ListItem("求和","0"));
            //			this.DDL_GroupWay.Items.Add(new ListItem("求平均","1"));
            //
            //			this.DDL_Order.Items.Add(new ListItem("降序","0"));
            //			this.DDL_Order.Items.Add(new ListItem("升序","1"));
        }

        #region 方法
        /// <summary>
        /// 处理什么都没有选择。
        /// </summary>
        public void DealChoseNone()
        {
            System.Web.UI.ControlCollection ctls = this.UCSys2.Controls;
            bool isCheck = false;
            foreach (Control ct in ctls)
            {
                if (ct.ID == null)
                    continue;

                if (ct.ID.IndexOf("CB_") == -1)
                    continue;

                string key = ct.ID.Substring("CB_".Length);
                CheckBox cb = this.UCSys2.GetCBByID("CB_" + key);
                if (cb.Checked == false)
                    continue;
                isCheck = true;
            }

            if (isCheck == false)
            {
                foreach (Control ct in ctls)
                {
                    if (ct.ID == null)
                        continue;

                    if (ct.ID.IndexOf("CB_") == -1)
                        continue;

                    string key = ct.ID.Substring("CB_".Length);
                    CheckBox cb = this.UCSys2.GetCBByID("CB_" + key);
                    cb.Checked = true;
                }
            }

            isCheck = false;
            foreach (ListItem li in this.CheckBoxList1.Items)
            {
                if (li.Selected)
                    isCheck = true;
            }

            if (isCheck == false)
            {
                foreach (ListItem li in this.CheckBoxList1.Items)
                {
                    li.Selected = true;
                    break;
                }
            }
        }
        public DataTable BingDG()
        {
            this.DealChoseNone();

            Entities ens = this.HisMD.HisEns;
            Entity en = ens.GetNewEntity;

            // 查询出来关于它的活动列配置.
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.EnsName);

            Paras myps = new Paras();
            Attrs attrs = this.HisMD.AttrsInTableEns;

            // 找到 分组的数据. 
            string groupKey = "";
            Attrs AttrsOfNum = new Attrs();
            System.Web.UI.ControlCollection ctls = this.UCSys2.Controls;
            string StateNumKey = "StateNumKey@"; // 为保存操作状态的需要。
            string Condition = ""; //处理特殊字段的条件问题。
            foreach (Control ct in ctls)
            {
                if (ct.ID == null)
                    continue;

                if (ct.ID.IndexOf("CB_") == -1)
                    continue;

                string key = ct.ID.Substring("CB_".Length);
                CheckBox cb = this.UCSys2.GetCBByID("CB_" + key);
                if (cb.Checked == false)
                    continue;

                AttrsOfNum.Add(attrs.GetAttrByKey(key));

                DDL ddl = this.UCSys2.GetDDLByID("DDL_" + key);
                if (ddl == null)
                {
                    ActiveAttr aa = (ActiveAttr)aas.GetEnByKey(ActiveAttrAttr.AttrKey, key);
                    if (aa == null)
                        continue;

                    Condition += aa.Condition;
                    groupKey += " round (" + aa.Exp + ", 4) AS " + key + ",";
                    StateNumKey += key + "=Checked@"; // 记录状态
                    //groupKey+=" round ( SUM("+key+"), 4) "+key+",";
                    //StateNumKey+=key+"=SUM@"; // 记录状态
                    continue;
                }

                switch (ddl.SelectedItemStringVal)
                {
                    case "SUM":
                        groupKey += " round ( SUM(" + key + "), 4) " + key + ",";
                        StateNumKey += key + "=SUM@"; // 记录状态
                        break;
                    case "AVG":
                        groupKey += " round (AVG(" + key + "), 4)  " + key + ",";
                        StateNumKey += key + "=AVG@"; // 记录状态
                        break;
                    case "AMOUNT":
                        groupKey += " round ( SUM(" + key + "), 4) " + key + ",";
                        StateNumKey += key + "=AMOUNT@"; // 记录状态
                        break;
                    default:
                        throw new Exception("没有判断的情况.");
                }
            }

            bool isHaveLJ = false; // 是否有累计字段。
            if (StateNumKey.IndexOf("AMOUNT@") != -1)
                isHaveLJ = true;

            if (groupKey == "")
            {
                this.UCSys1.AddMsgOfWarning("警告",
                    "<img src='/WF/Img/Pub/warning.gif' /><b><font color=red>您没有选择分析的数据</font></b>"); //您没有选择分析的数据。
                return null;
            }

            /* 如果包含累计数据，那它一定需要一个月份字段。业务逻辑错误。*/
            groupKey = groupKey.Substring(0, groupKey.Length - 1);
            BP.DA.Paras ps = new Paras();
            // 生成 sql.
            string selectSQL = "SELECT ";
            string groupBy = " GROUP BY ";
            Attrs AttrsOfGroup = new Attrs();
            string StateGroupKey = "StateGroupKey=@"; // 为保存操作状态的需要。
            foreach (ListItem li in this.CheckBoxList1.Items)
            {
                if (li.Value == "FK_NY")
                {
                    /* 如果是年月 分组， 并且如果内部有 累计属性，就强制选择。*/
                    if (isHaveLJ)
                        li.Selected = true;
                }

                if (li.Selected)
                {
                    selectSQL += li.Value + ",";
                    groupBy += li.Value + ",";

                    // 加入组里面。
                    AttrsOfGroup.Add(attrs.GetAttrByKey(li.Value), false, false);
                    StateGroupKey += li.Value + "@";
                }
            }
            groupBy = groupBy.Substring(0, groupBy.Length - 1);

            #region 生成Where  _OLD .   通过这个过程产生两个 where.
            // 找到 WHERE 数据。
            string where = " WHERE ";
            string whereOfLJ = " WHERE "; // 累计的where.
            string url = "";
            foreach (Control item in this.ToolBar1.Controls)
            {
                if (item.ID == null)
                    continue;
                if (item.ID.IndexOf("DDL_") == -1)
                    continue;
                if (item.ID.IndexOf("DDL_Form_") == 0 || item.ID.IndexOf("DDL_To_") == 0)
                    continue;

                string key = item.ID.Substring("DDL_".Length);
                DDL ddl = (DDL)item;
                if (ddl.SelectedItemStringVal == "all")
                    continue;

                string val = ddl.SelectedItemStringVal;
                if (val == null)
                    continue;

                if (val == "mvals")
                {
                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.EnsName + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                            {
                                val = WebUser.FK_Dept;
                                ddl.SelectedIndex = 0;
                            }

                            if (key == "FK_Unit")
                            {
                                val = WebUser.FK_Unit;
                                ddl.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);
                        where += " " + key + " IN (" + instr + ")  AND ";
                        continue;
                    }
                }

                if (key == "FK_Dept")
                {
                    if (val.Length == 8)
                    {
                        where += " FK_Dept =" + SystemConfig.AppCenterDBVarStr + "V_Dept    AND ";
                    }
                    else
                    {
                        switch (SystemConfig.AppCenterDBType)
                        {
                            case DBType.Oracle:
                            case DBType.Informix:
                                where += " FK_Dept LIKE '%'||:V_Dept||'%'   AND ";
                                break;
                            case DBType.MSSQL:
                            default:
                                where += " FK_Dept LIKE  " + SystemConfig.AppCenterDBVarStr + "V_Dept+'%'   AND ";
                                //  WHERE += " FK_Dept LIKE '@V_Dept%'   AND ";
                                break;
                        }
                    }
                    myps.Add("V_Dept", val);
                }
                else
                {
                    where += " " + key + " =" + SystemConfig.AppCenterDBVarStr + key + "   AND ";
                    if (key != "FK_NY")
                        whereOfLJ += " " + key + " =" + SystemConfig.AppCenterDBVarStr + key + "   AND ";

                    myps.Add(key, val);
                }
            }
            #endregion

            #region 加上 where like 条件
            if (en.EnMap.IsShowSearchKey == true && this.ToolBar1.GetTBByID("TB_Key").Text.Trim().Length > 1)
            {
                string key = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();
                if (key.Length > 1)
                {
                    string whereLike = "";
                    bool isAddOr = false;
                    foreach (Attr likeKey in attrs)
                    {
                        if (likeKey.IsNum)
                            continue;
                        if (likeKey.IsRefAttr)
                            continue;

                        switch (likeKey.Field)
                        {
                            case "MyFileExt":
                            case "MyFilePath":
                            case "WebPath":
                                continue;
                            default:
                                break;
                        }


                        if (isAddOr == false)
                        {
                            isAddOr = true;
                            whereLike += "      " + likeKey.Field + " LIKE '%" + key + "%' ";
                        }
                        else
                        {
                            whereLike += "   OR   " + likeKey.Field + " LIKE '%" + key + "%'";
                        }
                    }
                    whereLike += "          ";
                    where += "(" + whereLike + ")";
                }
            }
            #endregion

            #region 加上日期时间段.
            if (en.EnMap.DTSearchWay != DTSearchWay.None)
            {
                string dtFrom = this.ToolBar1.GetTBByID("TB_S_From").Text.Trim();
                string dtTo = this.ToolBar1.GetTBByID("TB_S_To").Text.Trim();
                string field = en.EnMap.DTSearchKey;
                string addAnd = "";
                if (en.EnMap.IsShowSearchKey && this.ToolBar1.GetTBByID("TB_Key").Text.Trim().Length > 0)
                    addAnd = " AND ";

                if (en.EnMap.DTSearchWay == DTSearchWay.ByDate)
                {
                    where += addAnd + "( " + field + ">='" + dtFrom + " 01:01' AND " + field + "<='" + dtTo + " 23:59')     ";
                }
                else
                {
                    where += addAnd + "(";
                    where += field + " >='" + dtFrom + "' AND " + field + "<='" + dtTo + "'";
                    where += ")";
                }
            }
            #endregion

            where = where.Replace("AND  AND", " AND ");


            if (where == " WHERE ")
            {
                where = "" + Condition.Replace("and", "");
                whereOfLJ = "" + Condition.Replace("and", "");
            }
            else
            {
                where = where.Substring(0, where.Length - " AND ".Length) + Condition;
                whereOfLJ = whereOfLJ.Substring(0, whereOfLJ.Length - " AND ".Length) + Condition;
            }

            string orderByReq = this.Request.QueryString["OrderBy"];
            string orderby = "";
            if (orderByReq != null)
            {
                //this.Alert(orderByReq + "  " + this.OrderWay);
                //this.ResponseWriteBlueMsg(selectSQL);
            }

            if (orderByReq != null && this.OrderBy != null && (selectSQL.Contains(orderByReq) || groupKey.Contains(orderByReq)))
            {
                orderby = " ORDER BY " + this.OrderBy;
                if (this.OrderWay != "Up")
                    orderby += " DESC ";
            }

            // 组装成需要的 sql 
            string sql = "";
            sql = selectSQL + groupKey + " FROM " + this.HisMD.PTable + where + groupBy + orderby;

            // 物理表。
            // this.ResponseWriteBlueMsg(sql);
            myps.SQL = sql;
            DataTable dt2 = DBAccess.RunSQLReturnTable(myps);
            // this.Response.Write(sql);

            DataTable dt1 = dt2.Clone();
            dt1.Columns.Add("IDX", typeof(int));

            #region 对他进行分页面
            int myIdx = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                myIdx++;
                DataRow mydr = dt1.NewRow();
                mydr["IDX"] = myIdx;
                foreach (DataColumn dc in dt2.Columns)
                {
                    mydr[dc.ColumnName] = dr[dc.ColumnName];
                }
                dt1.Rows.Add(mydr);
            }
            #endregion

            #region 处理 Int 类型的分组列。
            DataTable dt = dt1.Clone();
            dt.Rows.Clear();
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns[attr.Key].DataType = typeof(string);
            }
            foreach (DataRow dr in dt1.Rows)
            {
                dt.ImportRow(dr);
            }
            #endregion

            // 处理这个物理表 , 如果有累计字段, 就扩展它的列。
            if (isHaveLJ)
            {
                // 首先扩充列.
                foreach (Attr attr in AttrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppInt:
                            dt.Columns.Add(attr.Key + "Amount", typeof(int));
                            break;
                        default:
                            dt.Columns.Add(attr.Key + "Amount", typeof(decimal));
                            break;
                    }
                }

                // 添加累计汇总数据.
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (Attr attr in AttrsOfNum)
                    {
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                            continue;

                        //形成查询sql.
                        if (whereOfLJ.Length > 10)
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + this.HisMD.PTable + whereOfLJ + " AND ";
                        else
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + this.HisMD.PTable + " WHERE ";

                        foreach (Attr attr1 in AttrsOfGroup)
                        {
                            switch (attr1.Key)
                            {
                                case "FK_NY":
                                    sql += " FK_NY <= '" + dr["FK_NY"] + "' AND FK_ND='" + dr["FK_NY"].ToString().Substring(0, 4) + "' AND ";
                                    break;
                                case "FK_Dept":
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                                case "FK_SJ":
                                case "FK_XJ":
                                    sql += attr1.Key + " LIKE '" + dr[attr1.Key] + "%' AND ";
                                    break;
                                default:
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                            }
                        }

                        sql = sql.Substring(0, sql.Length - "AND ".Length);
                        if (attr.MyDataType == DataType.AppInt)
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValInt(sql, 0);
                        else
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    }
                }
            }
            // 生成表头。
            //            this.UCSys1.AddTable("width='30%'");

            this.UCSys1.Clear();
            //        this.UCSys1.AddTable("style='align:left' ");
            this.UCSys1.Add("<table class='MyTable' >");

            #region 增加分组条件
            if (StateNumKey.IndexOf("=AMOUNT") != -1)
            {
                /* 如果包含累计 */

                // 增加分组条件。
                this.UCSys1.AddTR();  // 开始第一列。
                this.UCSys1.Add("<td rowspan=2 class='Title'>ID</td>");
                foreach (Attr attr in AttrsOfGroup)
                {
                    this.UCSys1.Add("<td rowspan=2 class='GroupTitle'>" + attr.Desc + "</td>");
                }
                // 增加数据列
                foreach (Attr attr in AttrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1)
                    {
                        /*  如果本数据列 包含累计 */
                        this.UCSys1.Add("<td  colspan=2 class='GroupTitle' >" + attr.Desc + "</td>");
                    }
                    else
                    {
                        this.UCSys1.Add("<td  rowspan=2 class='GroupTitle' >" + attr.Desc + "</td>");
                    }
                }
                this.UCSys1.AddTREnd();  // end 开始第一列

                this.UCSys1.AddTR();
                foreach (Attr attr in AttrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                        continue;

                    this.UCSys1.Add("<td class='GroupTitle'>本月</td>"); //本月 this.ToE("OrderCondErr")
                    this.UCSys1.Add("<td class='GroupTitle'>累计</td>"); //累计
                }
                this.UCSys1.AddTR();
            }
            else  /* 没有合计的情况 */
            {
                this.UCSys1.AddTR();
                this.UCSys1.AddTDGroupTitle("IDX");

                // 分组条件
                foreach (Attr attr in AttrsOfGroup)
                {
                    if (this.OrderBy == attr.Key)
                    {
                        switch (this.OrderWay)
                        {
                            case "Down":
                                this.UCSys1.AddTDGroupTitle("<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&OrderBy=" + attr.Key + "&OrderWay=Up' >" + attr.Desc + "<img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                break;
                            case "Up":
                            default:
                                this.UCSys1.AddTDGroupTitle("<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&OrderBy=" + attr.Key + "&OrderWay=Down' >" + attr.Desc + "<img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                break;
                        }
                    }
                    else
                    {
                        this.UCSys1.AddTDGroupTitle("<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&OrderBy=" + attr.Key + "&OrderWay=Down' >" + attr.Desc + "</a>");
                    }
                }

                // 分组数据
                foreach (Attr attr in AttrsOfNum)
                {
                    string lab = "";
                    if (StateNumKey.Contains(attr.Key + "=SUM"))
                    {
                        lab = "(合计)" + attr.Desc;
                    }
                    else
                    {
                        lab = "(平均)" + attr.Desc;
                    }

                    if (this.OrderBy == attr.Key)
                    {
                        switch (this.OrderWay)
                        {
                            case "Down":
                                if (this.NumKey == attr.Key)
                                    this.UCSys1.AddTDGroupTitle(lab + "<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&OrderWay=Up'><img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                else
                                    this.UCSys1.AddTDGroupTitle("<a href=\"Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='Group.aspx?EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&OrderWay=Up&FK_Flow=" + this.FK_Flow + "'><img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                break;
                            case "Up":
                            default:
                                if (this.NumKey == attr.Key)
                                    this.UCSys1.AddTDGroupTitle(lab + "<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&OrderBy=" + attr.Key + "&NumKey=" + attr.Key + "&OrderWay=Down'><img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                else
                                    this.UCSys1.AddTDGroupTitle("<a href=\"Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='Group.aspx?EnsName=" + this.EnsName + "&OrderBy=" + attr.Key + "&NumKey=" + attr.Key + "&OrderWay=Down&FK_Flow=" + this.FK_Flow + "'><img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                break;
                        }
                    }
                    else
                    {
                        if (this.NumKey == attr.Key)
                            this.UCSys1.AddTDGroupTitle(lab + "<a href='Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "' ><img src='/WF/Img/ArrDownUp.gif' border=0/></a>");
                        else
                            this.UCSys1.AddTDGroupTitle("<a href=\"Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='Group.aspx?EnsName=" + this.EnsName + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&FK_Flow=" + this.FK_Flow + "' ><img src='/WF/Img/ArrDownUp.gif' border=0/></a>");
                    }
                }
                this.UCSys1.AddTDGroupTitle("挖掘");
                this.UCSys1.AddTREnd();
            }
            #endregion 生成表头

            #region 生成要查询条件
            string YSurl = "GroupDtl.aspx?EnsName=" + this.EnsName;
            string keys = "";

            if (this.ToolBar1.FindControl("TB_S_From") != null)
            {
                YSurl += "&DTFrom=" + this.ToolBar1.GetTBByID("TB_S_From").Text;
                YSurl += "&DTTo=" + this.ToolBar1.GetTBByID("TB_S_To").Text;
            }
            if (this.ToolBar1.FindControl("TB_Key") != null)
            {
                YSurl += "&Key=" + this.ToolBar1.GetTBByID("TB_Key").Text;
            }



            // 分组的信息中是否包含部门？
            bool IsHaveFK_Dept = false;
            foreach (Attr attr in AttrsOfGroup)
            {
                if (attr.Key == "FK_Dept")
                {
                    IsHaveFK_Dept = true;
                    break;
                }
            }
            foreach (AttrSearch a23 in en.EnMap.SearchAttrs)
            {
                Attr attrS = a23.HisAttr;
                if (attrS.MyFieldType == FieldType.RefText)
                    continue;

                if (IsHaveFK_Dept && attrS.Key == "FK_Dept")
                    continue;

                DDL ddl = this.ToolBar1.GetDDLByKey("DDL_" + attrS.Key);
                if (ddl == null)
                {
                    throw new Exception(attrS.Key);
                }

                string val = this.ToolBar1.GetDDLByKey("DDL_" + attrS.Key).SelectedItemStringVal;
                if (val == "all")
                    continue;
                keys += "&" + attrS.Key + "=" + val;
            }
            YSurl = YSurl + keys;
            #endregion

            #region 生成外键
            // 为表扩充外键
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns.Add(attr.Key + "T", typeof(string));
            }
            foreach (Attr attr in AttrsOfGroup)
            {
                if (attr.IsEnum)
                {
                    /* 说明它是枚举类型 */
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int val = 0;
                        try
                        {
                            val = int.Parse(dr[attr.Key].ToString());
                        }
                        catch
                        {
                            dr[attr.Key + "T"] = " ";
                            continue;
                        }

                        foreach (SysEnum se in ses)
                        {
                            if (se.IntKey == val)
                                dr[attr.Key + "T"] = se.Lab;
                        }
                    }
                    continue;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    string val = dr[attr.Key].ToString();

                    if (attr.UIBindKey.Contains(".") == false)
                    {
                        try
                        {
                            dr[attr.Key + "T"] = DBAccess.RunSQLReturnStringIsNull("SELECT Name FROM " + attr.UIBindKey + " WHERE No='" + val + "'", val);
                        }
                        catch
                        {
                            dr[attr.Key + "T"] = val;
                        }
                        continue;
                    }

                    Entity myen = attr.HisFKEn;
                    myen.SetValByKey(attr.UIRefKeyValue, val);
                    try
                    {
                        myen.Retrieve();
                        dr[attr.Key + "T"] = myen.GetValStrByKey(attr.UIRefKeyText);
                    }
                    catch
                    {
                        if (val == null || val.Length <= 1)
                        {
                            dr[attr.Key + "T"] = val;
                        }
                        else
                        {
                            dr[attr.Key + "T"] = val;
                        }
                    }
                }
            }
            #endregion

            #region 生成表体
            int i = 0;
            bool is1 = false;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                url = YSurl.Clone() as string;
                string keyActive = "";
                // 产生url .
                foreach (Attr attr in AttrsOfGroup)
                {
                    url += "&" + attr.Key + "=" + dr[attr.Key].ToString();
                    //keyActive+="&"+attr.Key+"="+dr[attr.Key].ToString() ; 
                }

                is1 = this.UCSys1.AddTR(is1);
                this.UCSys1.AddTDIdx(int.Parse(dr["IDX"].ToString()));
                // 分组条件
                foreach (Attr attr in AttrsOfGroup)
                {
                    this.UCSys1.AddTD(dr[attr.Key + "T"].ToString());
                }

                // 分组数据
                foreach (Attr attr in AttrsOfNum)
                {
                    decimal obj = 0;
                    try
                    {
                        obj = decimal.Parse(dr[attr.Key].ToString());
                    }
                    catch (Exception ex)
                    {
                        // throw new Exception(dr[attr.Key].ToString() +"@SQL="+ sql +"@"+ex.Message +"@Attr="+attr.Key );
                    }

                    switch (attr.MyDataType)
                    {
                        case DataType.AppMoney:
                        case DataType.AppRate:
                            if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                            {
                                this.UCSys1.AddTDJE(obj);
                                this.UCSys1.AddTDJE(decimal.Parse(dr[attr.Key + "Amount"].ToString()));
                            }
                            else
                            {
                                this.UCSys1.AddTDJE(obj);
                            }
                            break;
                        default:
                            if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                            {
                                this.UCSys1.AddTDNum(obj);
                                this.UCSys1.AddTDNum(decimal.Parse(dr[attr.Key + "Amount"].ToString()));
                            }
                            else
                            {
                                this.UCSys1.AddTDNum(obj);
                            }
                            break;
                    }
                }
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + url + "','s','900', '600')\" >详细</a>");
                this.UCSys1.AddTREnd();
            }

            #region  加入合计信息。
            this.UCSys1.AddTR("class='TRSum'");
            this.UCSys1.AddTD("汇总");
            foreach (Attr attr in AttrsOfGroup)
            {
                this.UCSys1.AddTD();
            }

            //不显示合计列。
            string NoShowSum = SystemConfig.GetConfigXmlEns("NoShowSum", this.EnsName);
            if (NoShowSum == null)
                NoShowSum = "";

            Attrs AttrsOfNum1 = AttrsOfNum.Clone();
            decimal d = 0;
            foreach (Attr attr in AttrsOfNum)
            {
                if (NoShowSum.Contains("@" + attr.Key + "@"))
                {
                    bool isHave = false;
                    foreach (ActiveAttr aa in aas)
                    {
                        if (aa.AttrKey != attr.Key)
                            continue;

                        isHave = true;
                        /* 如果它是一个计算列 */
                        string exp = aa.ExpApp;
                        if (exp == null || exp == "")
                        {
                            this.UCSys1.AddTD();
                            break;
                        }
                        foreach (Attr myattr in AttrsOfNum1)
                        {
                            if (exp.IndexOf("@" + myattr.Key + "@") != -1)
                            {
                                d = 0;
                                foreach (DataRow dr1 in dt.Rows)
                                {
                                    try
                                    {
                                        d += decimal.Parse(dr1[myattr.Key].ToString());
                                    }
                                    catch
                                    {
                                    }
                                }

                                exp = exp.Replace("@" + myattr.Key + "@", d.ToString());
                            }
                        }
                        this.UCSys1.AddTDNum(DataType.ParseExpToDecimal(exp));
                    }

                    if (isHave == false)
                        this.UCSys1.AddTD();
                    else
                    {

                    }
                    continue;
                }

                switch (attr.MyDataType)
                {
                    case DataType.AppMoney:
                    case DataType.AppRate:
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key].ToString());
                            this.UCSys1.AddTDJE(d);

                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key + "Amount"].ToString());
                            this.UCSys1.AddTDJE(d);
                        }
                        else
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                            {
                                try
                                {
                                    d += decimal.Parse(dr1[attr.Key].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (StateNumKey.IndexOf(attr.Key + "=AVG") < 1)
                            {
                                this.UCSys1.AddTDJE(d);
                            }
                            else
                            {
                                if (dt.Rows.Count == 0)
                                    this.UCSys1.AddTD();
                                else
                                    this.UCSys1.AddTDJE(d / dt.Rows.Count);
                            }
                        }
                        break;
                    default:
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key].ToString());
                            this.UCSys1.AddTDNum(d);

                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key + "Amount"].ToString());
                            this.UCSys1.AddTDNum(d);
                        }
                        else
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                            {
                                try
                                {
                                    d += decimal.Parse(dr1[attr.Key].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (StateNumKey.IndexOf(attr.Key + "=AVG") < 1)
                            {
                                this.UCSys1.AddTDNum(d);
                            }
                            else
                            {
                                if (dt.Rows.Count == 0)
                                    this.UCSys1.AddTD();
                                else
                                    this.UCSys1.AddTDJE(d / dt.Rows.Count);
                            }
                        }
                        break;
                }
            }
            this.UCSys1.AddTD();
            this.UCSys1.AddTREnd();
            #endregion

            this.UCSys1.AddTableEnd();
            #endregion 生成表体

            #region 生成图形
            //this.BPTabStrip1.Visible = true;
            ///this.CB_IsShowPict.Checked;
            //if (AttrsOfGroup.Count==1)
            if (this.CB_IsShowPict.Checked)
            {
                /* 如果是 1 纬 */
                string colOfGroupField = "";
                string colOfGroupName = "";
                string colOfNumField = "";
                string colOfNumName = "";
                string title = "";
                int chartHeight = int.Parse(this.TB_H.Text);
                int chartWidth = int.Parse(this.TB_W.Text);

                if (isHaveLJ)
                {
                    /*  如果有累计, 就按照累计字段分析。*/
                    colOfGroupField = AttrsOfGroup[0].Key;
                    colOfGroupName = AttrsOfGroup[0].Desc;

                    colOfNumName = AttrsOfNum[0].Desc;
                    if (dt.Columns.Contains(AttrsOfNum[0].Key + "AMOUNT"))
                        colOfNumField = AttrsOfNum[0].Key + "AMOUNT";
                    else
                        colOfNumField = AttrsOfNum[0].Key;
                }
                else
                {
                    colOfGroupField = AttrsOfGroup[0].Key;
                    colOfGroupName = AttrsOfGroup[0].Desc;

                    if (NumKey == null)
                    {
                        colOfNumName = AttrsOfNum[0].Desc;
                        colOfNumField = AttrsOfNum[0].Key;
                    }
                    else
                    {
                        //  colOfNumField = AttrsOfNum[0].Key;
                        colOfNumName = attrs.GetAttrByKey(NumKey).Desc; // this.UCSys1.get;
                        colOfNumField = NumKey;
                    }
                }

                string colOfNumName1 = "";
                if (StateNumKey.Contains(this.NumKey + "=SUM"))
                    colOfNumName1 = "(合计)" + colOfNumName;
                else
                    colOfNumName1 = "(平均)" + colOfNumName;

                if (dt.Columns.Contains(colOfNumField) == false)
                {
                    foreach (Attr item in AttrsOfNum)
                    {
                        if (dt.Columns.Contains(item.Key))
                        {
                            colOfNumField = item.Key;
                            break;
                        }
                    }
                }


                try
                {
                    url = this.Request.ApplicationPath + "/Temp/" + CCFlow.WF.Comm.UC.UCSys.GenerChart(dt,
                      colOfGroupField + "T", colOfGroupName,
                      colOfNumField, colOfNumName1
                      , "", chartHeight, chartWidth, ChartType.Histogram);

                    this.UCSys3.AddBR("<img src='" + url + "' />");

                    url = this.Request.ApplicationPath + "/Temp/" + CCFlow.WF.Comm.UC.UCSys.GenerChart(dt,
                       colOfGroupField + "T", colOfGroupName,
                       colOfNumField, colOfNumName1
                       , "", chartHeight, chartWidth, ChartType.Pie);

                    this.UCSys3.AddBR("<img src='" + url + "' />");

                    url = this.Request.ApplicationPath + "/Temp/" + BP.Web.UC.UCGraphics.GenerChart(dt,
                       colOfGroupField + "T", colOfGroupName,
                       colOfNumField, colOfNumName1
                       , "", chartHeight, chartWidth, ChartType.Line);
                    this.UCSys3.AddBR("<img src='" + url + "' />");
                }
                catch (Exception ex)
                {
                    this.ResponseWriteRedMsg("@产生图片文件出现错误:" + ex.Message);
                }

                //this.BPTabStrip1.Items[0].Text = this.ToE("TableGrade", "表格");
                //// this.BPTabStrip1.Items[0].Text = this.ToE("TableGrade", "表格-<a href=\"javascript:WinOpen('./Rpt/Adv.aspx')\" >高级</a>");
                //this.BPTabStrip1.Items[2].Text = this.ToE("Histogram", colOfNumName + "-柱状图");
                //this.BPTabStrip1.Items[4].Text = this.ToE("Pie", colOfNumName + "-饼图");
                //this.BPTabStrip1.Items[6].Text = this.ToE("Line", colOfNumName + "-折线图");
            }
            #endregion

            #region 保存状态
            //if (this.IsPostBack)
            //{
            //    this.ResponseWriteBlueMsg("hi");
            // 保存状态。

            ur.Vals = StateGroupKey + StateNumKey;
            ur.CfgKey = this.EnsName + "_Group";
            ur.FK_Emp = WebUser.NoOfSessionID;
            ur.OrderBy = this.OrderBy;
            ur.OrderWay = this.OrderWay;
            ur.IsPic = this.CB_IsShowPict.Checked;
            ur.GenerSQL = myps.SQL;
            ur.NumKey = this.NumKey;
            ur.Paras = "";
            foreach (Para para in myps)
            {
                ur.Paras += "@" + para.ParaName + "=" + para.val;
            }
            ur.Save();

            this.SetValueByKey("Vals", ur.Vals);
            this.SetValueByKey("CfgKey", ur.CfgKey);
            this.SetValueByKey("OrderBy", ur.OrderBy);
            this.SetValueByKey("OrderWay", ur.OrderWay);
            this.SetValueByKey("IsPic", ur.IsPic);
            this.SetValueByKey("SQL", ur.GenerSQL);
            this.SetValueByKey("NumKey", ur.NumKey);
            this.CfgVal = ur.Vals;
            #endregion

            return dt1;
        }

        //public string Vals = null;
        //public string CfgKey = null;
        //public string OrderBy = null;
        //public string OrderWay = null;
        //public bool IsPic = false;
        //public bool NumKey = false;
        //public bool Paras = false;
        //public bool SQL = false;


        public DataTable DealTable(DataTable dt)
        {
            DataTable dtCopy = new DataTable();

            #region 把他们转换为 string 类型。
            foreach (DataColumn dc in dt.Columns)
                dtCopy.Columns.Add(dc.ColumnName, typeof(string));

            foreach (DataRow dr in dt.Rows)
                dtCopy.ImportRow(dr);
            #endregion

            Entity en = this.HisMD.HisEn;
            Map map = en.EnMap;
            Attrs attrs = this.HisMD.AttrsInTableEns;
            foreach (DataColumn dc in dt.Columns)
            {
                bool isLJ = false;
                Attr attr = null;
                try
                {
                    attr = map.GetAttrByKey(dc.ColumnName);
                    isLJ = false;
                }
                catch
                {
                    try
                    {
                        attr = map.GetAttrByKey(dc.ColumnName + "AMOUNT");
                        isLJ = true;
                    }
                    catch
                    {
                    }
                }

                if (attr == null)
                    continue;

                if (attr.UIBindKey == null || attr.UIBindKey == "")
                {
                    if (isLJ)
                        dtCopy.Columns[attr.Key.ToUpper() + "AMOUNT"].ColumnName = "累计";
                    else
                        dtCopy.Columns[attr.Key.ToUpper()].ColumnName = attr.Desc;
                    continue;
                }

                // 设置标签 
                if (attr.UIBindKey.IndexOf(".") != -1)
                {
                    //  Entity en1 = BP.DA.ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    Entity en1 = attr.HisFKEn;
                    string pk = en1.PK;
                    foreach (DataRow dr in dtCopy.Rows)
                    {
                        if (dr[attr.Key] == DBNull.Value)
                            continue;

                        string val = (string)dr[attr.Key];
                        if (val == null || val == "")
                            continue;

                        en1.SetValByKey(pk, dr[attr.Key]);
                        int i = en1.RetrieveFromDBSources();
                        if (i == 0)
                            continue;

                        dr[attr.Key] = en1.GetValStrByKey(attr.UIRefKeyValue) + en1.GetValStrByKey(attr.UIRefKeyText);
                    }
                }
                else if (attr.UIBindKey.Length >= 2)
                {
                    foreach (DataRow mydr in dtCopy.Rows)
                    {
                        if (mydr[attr.Key] == DBNull.Value)
                            continue;

                        int intVal = int.Parse(mydr[attr.Key].ToString());
                        SysEnum se = new SysEnum(attr.UIBindKey, intVal);
                        mydr[attr.Key] = se.Lab;
                    }
                }
                dtCopy.Columns[attr.Key.ToUpper()].ColumnName = attr.Desc;
            }

            try
            {
                dtCopy.Columns["MYNUM"].ColumnName = "个数";
            }
            catch
            {
            }
            return dtCopy;
        }

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
        #endregion

        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            Btn btn = (Btn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.Help:
                    //  this.Helper();
                    break;
                case NamesOfBtn.Excel:
                    DataTable dt = this.BingDG();
                    this.ExportDGToExcel(this.DealTable(dt), this.HisEns.GetNewEntity.EnDesc);
                    return;
                default:
                    this.ToolBar1.SaveSearchState(this.EnsName, this.Key);
                    this.BingDG();
                    return;
            }
        }
        void State_Changed(object sender, EventArgs e)
        {
            this.BingDG();
            //this.SaveState();
        }
        void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BingDG();
            //  this.SaveState();
        }
    }

}