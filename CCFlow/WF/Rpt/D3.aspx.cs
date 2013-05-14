using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
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
    public partial class WF_Rpt_D3 : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                {
                    // throw new Exception("丢失FK_Flow参数.");
                    s = "001";
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
                return "";
                //return this.ViewState["CfgVal"].ToString();
            }
            set
            {
                this.ViewState["CfgVal"] = value;
            }
        }
        public MapData HisMD = null;
        public MapAttrs HisMAs = null;
        #endregion

        #region 控件
        public CheckBox CB_IsImg
        {
            get
            {
                return this.Left.GetCBByID("CB_IsImg");
            }
        }
        /// <summary>
        /// 过滤为空的数据.
        /// </summary>
        public CheckBox CB_IsNull
        {
            get
            {
                return this.Left.GetCBByID("CB_IsNull");
            }
        }

        public CheckBox CB_IsRate
        {
            get
            {
                return this.Left.GetCBByID("CB_IsRate");
            }
        }
        public DDL DDL_Num
        {
            get
            {
                return this.Left.GetDDLByID("DDL_Num");
            }
        }
        public DDL DDL_FXWay
        {
            get
            {
                return this.Left.GetDDLByID("DDL_FXWay");
            }
        }
        public DDL DDL_D1
        {
            get
            {
                return this.Left.GetDDLByID("DDL_D1");
            }
        }
        public DDL DDL_D1_Order
        {
            get
            {
                return this.Left.GetDDLByID("DDL_D1_Order");
            }
        }

        public DDL DDL_D2
        {
            get
            {
                return this.Left.GetDDLByID("DDL_D2");
            }
        }
        public DDL DDL_D2_Order
        {
            get
            {
                return this.Left.GetDDLByID("DDL_D2_Order");
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.HisMD = new MapData(this.EnsName);
            this.HisMAs = new MapAttrs(this.EnsName);
            this.ur = new UserRegedit(WebUser.No, this.EnsName + "_D3");

            this.InitToolbar();
            this.InitLeft();

            if (this.IsPostBack == false)
                this.BindDG();
        }

        public void InitToolbar()
        {
            AttrSearchs searchs = null;

            #region 处理查询设的默认.
            if (this.DoType == "My")
            {
                #region 处理查询权限
                Entity en = this.HisEns.GetNewEntity;
                Map map = en.EnMap;
                this.ToolBar1.InitByMapV2(map, 1, this.EnsName);
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
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM WF_Emp WHERE  FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + this.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + this.EnsName + " WHERE FlowStarter!='')");
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
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + this.FK_Flow + "')");
                                if (dt.Rows.Count == 0)
                                {
                                    this.Right.AddMsgOfWarning("提示", "<h2>系统管理员没有给您设置查询权限。</h2>");
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
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM WF_Emp WHERE  FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + this.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + this.EnsName + " WHERE FlowStarter!='')");
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
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + this.FK_Flow + "')");
                                if (dt.Rows.Count == 0)
                                {
                                    this.Right.AddMsgOfWarning("提示", "<h2>系统管理员没有给您设置查询权限。</h2>");
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
        }


        public void InitLeft()
        {
            this.Left.AddTable();
            string paras = this.ur.Paras;

            #region 选项
            this.Left.AddTR();
            this.Left.AddTDTitle("colspan=2", "选项");
            this.Left.AddTREnd();

            DDL ddl = new DDL();
            ddl.ID = "DDL_Num";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);

            ListItem li = null;
            this.Left.AddTR();
            this.Left.AddTD("分析数据");
            Attrs attrs = this.HisMD.AttrsInTableEns;
            attrs.AddTBInt("MyNum", 1, "流程数量", true, true);
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                if (attr.UIVisible == false)
                    continue;

                if (attr.IsNum == false)
                    continue;

                if (attr.Key == "OID" || attr.Key == "WorkID" || attr.Key == "FID" || attr.Key == "MID")
                    continue;

                li = new ListItem(attr.Desc, attr.Key);
                if (paras.Contains("@Num=" + attr.Key))
                    li.Selected = true;
                ddl.Items.Add(li);
            }
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();

            this.Left.AddTR();
            this.Left.AddTD("分析方式");
            ddl = new DDL();
            ddl.ID = "DDL_FXWay";
            ddl.AutoPostBack = true;

            li = new ListItem("求和", "SUM");
            if (paras.Contains("@FXWay=SUM"))
                li.Selected = true;
            ddl.Items.Add(li);

            li = new ListItem("求平均", "AVG");
            if (paras.Contains("@FXWay=AVG"))
                li.Selected = true;
            ddl.Items.Add(li);
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();


            this.Left.AddTR();
            CheckBox cb = new CheckBox();
            cb.AutoPostBack = true;
            cb.ID = "CB_IsRate";
            cb.Text = "显示百分比";
            if (paras.Contains("@IsRate=1"))
                cb.Checked = true;
            else
                cb.Checked = false;

            cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
            this.Left.AddTD(cb);

            cb = new CheckBox();
            cb.ID = "CB_IsImg";
            cb.AutoPostBack = true;
            if (paras.Contains("@IsImg=1"))
                cb.Checked = true;
            else
                cb.Checked = false;

            cb.Text = "显示图形";
            cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
            this.Left.AddTD(cb);
            this.Left.AddTREnd();


            AtPara ps = new AtPara(paras);

            this.Left.AddTR();
            this.Left.AddTDBegin();
            TextBox tb = new TextBox();
            tb.Width = 40;
            tb.ID = "TB_W";
            tb.BorderWidth = 1;
            tb.BorderStyle = BorderStyle.Outset;
            tb.Text = ps.GetValStrByKey("W");
            if (string.IsNullOrEmpty(tb.Text))
                tb.Text = "500";

            this.Left.Add("宽:");
            this.Left.Add(tb);
            this.Left.AddTDEnd();

            this.Left.AddTDBegin();
            this.Left.Add("高:");
            tb = new TextBox();
            tb.Width = 40;
            tb.ID = "TB_H";
            tb.BorderWidth = 1;
            tb.BorderStyle = BorderStyle.Outset;
            tb.Text = ps.GetValStrByKey("H");
            if (string.IsNullOrEmpty(tb.Text))
                tb.Text = "300";

            this.Left.Add(tb);
            this.Left.AddTDEnd();
            this.Left.AddTREnd();


            this.Left.AddTR();
            cb = new CheckBox();
            cb.AutoPostBack = true;
            cb.ID = "CB_IsNull";
            cb.Text = "过滤为null值的数据";
            if (paras.Contains("@IsNull=1"))
                cb.Checked = true;
            else
                cb.Checked = false;
            cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
            this.Left.AddTD("colspan=2", cb);
            this.Left.AddTREnd();

            #endregion 选项

            #region 横纬度
            this.Left.AddTR();
            this.Left.AddTDTitle("colspan=2", "横纬度");
            this.Left.AddTREnd();

            ddl = new DDL();
            ddl.ID = "DDL_D1";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.Left.AddTR();
            this.Left.AddTD("纬度项目");
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType == UIContralType.DDL)
                {
                    li = new ListItem(attr.Desc, attr.Key);

                    if (paras.Contains("@D1=" + attr.Key))
                        li.Selected = true;

                    ddl.Items.Add(new ListItem(attr.Desc, attr.Key));
                }
            }
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();

            ddl = new DDL();
            ddl.ID = "DDL_D1_Order";
            ddl.AutoPostBack = true;
            ddl.Items.Add(new ListItem("升序", "Up"));
            ddl.Items.Add(new ListItem("降序", "Desc"));
            if (paras.Contains("@D1_Order=Up"))
                li.Selected = true;
            else
                li.Selected = false;

            this.Left.AddTR();
            this.Left.AddTD("排序方式");
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();
            #endregion 横纬度

            #region 纵纬度
            this.Left.AddTR();
            this.Left.AddTDTitle("colspan=2", "纵纬度");
            this.Left.AddTREnd();

            this.Left.AddTR();
            this.Left.AddTD("数据项目");
            ddl = new DDL();
            ddl.ID = "DDL_D2";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType == UIContralType.DDL)
                {
                    li = new ListItem(attr.Desc, attr.Key);
                    if (paras.Contains("@D2=" + attr.Key))
                        li.Selected = true;
                    ddl.Items.Add(new ListItem(attr.Desc, attr.Key));
                }
            }
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();

            ddl = new DDL();
            ddl.ID = "DDL_D2_Order";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            ddl.Items.Add(new ListItem("升序", ""));
            ddl.Items.Add(new ListItem("降序", "DESC"));
            if (paras.Contains("@D2_Order=Up"))
                li.Selected = true;
            else
                li.Selected = false;
            this.Left.AddTR();
            this.Left.AddTD("排序方式");
            this.Left.AddTD(ddl);
            this.Left.AddTREnd();
            #endregion 纵纬度

            this.Left.AddTableEnd();

            //#region 纵纬度1
            //this.Left.AddTR();
            //this.Left.AddTDTitle("colspan=2", "纵纬度2");
            //this.Left.AddTREnd();

            //this.Left.AddTR();
            //this.Left.AddTD("数据项目");
            //ddl = new DDL();
            //ddl.ID = "DDL_D3";
            //foreach (MapAttr attr in this.HisMAs)
            //{
            //    if (attr.UIBindKey == "")
            //        continue;
            //    ddl.Items.Add(new ListItem(attr.Name, attr.KeyOfEn));
            //}
            //this.Left.AddTD(ddl);
            //this.Left.AddTREnd();

            //ddl = new DDL();
            //ddl.ID = "DDL_D3_Order";
            //ddl.Items.Add(new ListItem("升序", ""));
            //ddl.Items.Add(new ListItem("降序", "DESC"));
            //this.Left.AddTR();
            //this.Left.AddTD("排序方式");
            //this.Left.AddTD(ddl);
            //this.Left.AddTREnd();
            //#endregion 纵纬度2

        }
        public DataTable BindDG()
        {
            if (this.DDL_D1.Items.Count <= 1 || this.DDL_Num.Items.Count == 0)
            {
                this.Right.Clear();
                this.ToolBar1.Visible = false;
                this.Right.AddMsgGreen("提示:", "<h2>没有足够的纬度或者没有数据分析项目。</h2>");
                return null;
            }

            if (this.DDL_D1.SelectedItemStringVal == this.DDL_D2.SelectedItemStringVal)
            {
                if (this.DDL_D1.SelectedIndex == 0)
                    this.DDL_D2.SelectedIndex = 1;
                else
                    this.DDL_D2.SelectedIndex = 0;
            }

            Attrs attrs = this.HisMD.AttrsInTableEns;
            SysEnums sesD1 = null;
            Entities ensD1 = null;
            SysEnums sesD2 = null;
            Entities ensD2 = null;
            Map map = this.HisMD.HisEn.EnMap;

            #region 生成两个纬度报表
            Attr attrD1 = attrs.GetAttrByKey(this.DDL_D1.SelectedItemStringVal);
            if (attrD1.IsEnum)
                sesD1 = new SysEnums(attrD1.UIBindKey);
            else
                ensD1 = attrD1.HisFKEns;

            Attr attrD2 = attrs.GetAttrByKey(this.DDL_D2.SelectedItemStringVal);
            if (attrD2.IsEnum)
                sesD2 = new SysEnums(attrD2.UIBindKey);
            else
                ensD2 = attrD2.HisFKEns;
            #endregion


            #region 生成执行的原始sql
            string Condition = ""; //处理特殊字段的条件问题。
            Paras myps = new BP.DA.Paras();
            string sql = "SELECT " + attrD1.Key + "," + attrD2.Key + ", " + this.DDL_FXWay.SelectedItemStringVal + "(" + this.DDL_Num.SelectedItemStringVal + ") FROM " + map.PhysicsTable;
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
                                //  val = WebUser.FK_Unit;
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
            try
            {
                string key = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();
                if (key.Length > 1)
                {
                    string whereLike = "";

                    bool isAddAnd = false;
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


                        if (isAddAnd == false)
                        {
                            isAddAnd = true;
                            whereLike += "      " + likeKey.Field + " LIKE '%" + key + "%' ";
                        }
                        else
                        {
                            whereLike += "   AND   " + likeKey.Field + " LIKE '%" + key + "%'";
                        }
                    }
                    whereLike += "          ";
                    where += whereLike;
                }
            }
            catch
            {
            }
            #endregion

            #region 加上日期时间段.
            if (map.DTSearchWay != DTSearchWay.None)
            {
                string dtFrom = this.ToolBar1.GetTBByID("TB_S_From").Text.Trim();
                string dtTo = this.ToolBar1.GetTBByID("TB_S_To").Text.Trim();
                string field = map.DTSearchKey;
                if (map.DTSearchWay == DTSearchWay.ByDate)
                {
                    where += "( " + field + ">='" + dtFrom + " 01:01' AND " + field + "<='" + dtTo + " 23:59')     ";
                }
                else
                {
                    where += "(";
                    where += field + " >='" + dtFrom + "' AND " + field + "<='" + dtTo + "'";
                    where += ")";
                }
            }
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
            #endregion

            sql += where + " GROUP BY  " + attrD1.Key + "," + attrD2.Key;
            myps.SQL = sql;
            DataTable dt = DBAccess.RunSQLReturnTable(myps);

            string leftMsg = this.DDL_FXWay.SelectedItem.Text + ":" + this.DDL_Num.SelectedItem.Text;

            #region 生成标题
            this.Right.AddTable("border=0");
            this.Right.AddTR();
            this.Right.AddTDGroupTitle(leftMsg);
            if (sesD1 != null)
            {
                foreach (SysEnum se in sesD1)
                {
                    this.Right.AddTDGroupTitle(se.Lab);
                }
            }
            if (ensD1 != null)
            {
                foreach (Entity en in ensD1)
                {
                    this.Right.AddTDGroupTitle(en.GetValStrByKey("Name"));
                }
            }
            this.Right.AddTREnd();
            #endregion .生成标题.

            #region 生成单元格
            if (sesD2 != null)
            {
                foreach (SysEnum se in sesD2)
                {
                    this.Right.AddTR();
                    this.Right.AddTDGroupTitle(se.Lab);
                    if (sesD1 != null)
                    {
                        foreach (SysEnum seD1 in sesD1)
                        {
                            this.Right.AddTD("onclick='' ", this.GetIt(dt, seD1.IntKey.ToString(), se.IntKey.ToString()));
                        }
                    }
                    if (ensD1 != null)
                    {
                        foreach (Entity enD1 in ensD1)
                        {
                            this.Right.AddTD(this.GetIt(dt, enD1.GetValStrByKey("No"), se.IntKey.ToString()));
                        }
                    }
                    this.Right.AddTREnd();
                }
            }

            if (ensD2 != null)
            {
                foreach (Entity en in ensD2)
                {
                    this.Right.AddTR();
                    this.Right.AddTDGroupTitle(en.GetValStrByKey("Name"));

                    if (sesD1 != null)
                    {
                        foreach (SysEnum seD1 in sesD1)
                        {
                            this.Right.AddTD(this.GetIt(dt, seD1.IntKey.ToString(), en.GetValStrByKey("No")));
                        }
                    }
                    if (ensD1 != null)
                    {
                        foreach (Entity enD1 in ensD1)
                        {
                            this.Right.AddTD(this.GetIt(dt, enD1.GetValStrByKey("No"), en.GetValStrByKey("No")));
                        }
                    }
                    this.Right.AddTREnd();
                }
            }
            this.Right.AddTableEnd();
            #endregion .生成单元格.

            #region 保存状态
            string paras = "";
            if (this.CB_IsImg.Checked)
                paras = "@IsImg=1";
            else
                paras = "@IsImg=0";

            if (this.CB_IsRate.Checked)
                paras += "@IsRate=1";
            else
                paras += "@IsRate=0";

            if (this.CB_IsNull.Checked)
                paras += "@IsNull=1";
            else
                paras += "@IsNull=0";

            paras += "@Num=" + this.DDL_Num.SelectedItemStringVal;
            paras += "@FXWay=" + this.DDL_FXWay.SelectedItemStringVal;
            paras += "@D1=" + this.DDL_D1.SelectedItemStringVal;
            paras += "@D1_Order=" + this.DDL_D1_Order.SelectedItemStringVal;
            paras += "@D2=" + this.DDL_D2.SelectedItemStringVal;
            paras += "@D2_Order=" + this.DDL_D2_Order.SelectedItemStringVal;
            paras += "@W=" + this.Left.GetTextBoxByID("TB_W").Text;
            paras += "@H=" + this.Left.GetTextBoxByID("TB_H").Text;

            ur.CfgKey = this.EnsName + "_D3";
            ur.MyPK = WebUser.No + "_" + ur.CfgKey;
            ur.FK_Emp = WebUser.NoOfSessionID;
            ur.Paras = paras;
            ur.Save();
            #endregion

            return null;
        }
        public decimal GetIt(DataTable dt, string d1, string d2)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() == d1 && dr[1].ToString() == d2)
                    return decimal.Parse(dr[2].ToString());
            }
            return 0;
        }

        void cb_CheckedChanged(object sender, EventArgs e)
        {
            this.BindDG();
        }
        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindDG();
        }
        void ToolBar1_ButtonClick(object sender, EventArgs e)
        {
            Btn btn = (Btn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.Help:
                    //  this.Helper();
                    break;
                case NamesOfBtn.Excel:
                    DataTable dt = this.BindDG();
                    // this.ExportDGToExcel(this.DealTable(dt), this.HisEns.GetNewEntity.EnDesc);
                    return;
                default:
                    this.ToolBar1.SaveSearchState(this.EnsName, this.Key);
                    this.BindDG();
                    return;
            }
        }
    }
}