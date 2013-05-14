using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.Web;
using BP.DA;
using BP.Sys;
using BP;

public partial class WF_MapDef_Rpt_Home : BP.Web.WebPage
{
    public string FK_Flow
    {
        get
        {
            string s= this.Request.QueryString["FK_Flow"];
            if (s == null)
                s ="007";
            return s;
        }
    }
    public int Idx
    {
        get
        {
            string s = this.Request.QueryString["Idx"];
            if (s == null)
                s = "0";
            return int.Parse(s);
        }
    }
    public string FK_MapData
    {
        get
        {
            string s = this.Request.QueryString["FK_MapData"];
            if (s == null)
                s = "ND7Rpt";
            return s;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "报表设计";
        Flow fl = new Flow(this.FK_Flow);
        this.BindLeft(fl);

        switch (this.Request.QueryString["ActionType"])
        {
            case null:
                break;
            case "Reset":
                 MapData mdReset = new MapData();
                 mdReset.No = this.FK_MapData;
                 mdReset.RetrieveFromDBSources();
                 mdReset.SearchKeys = "@FK_Dept@WFState@FK_NY@";
                 mdReset.AttrsInTable = "";
                 mdReset.DirectUpdate();
                 this.Pub2.AddFieldSet("提示","<h2>重设置成功，<a href='./../../Rpt/Search.aspx?FK_Flow="+this.FK_Flow+"&DoType=Dept' target=_v >点这里预览</a>。</h2>");
                 break;
            case "Left":
                if (this.Idx == 0)
                    break;
                MapData md = new MapData();
                md.No = this.FK_MapData;
                md.RetrieveFromDBSources();
                int i = -1;
                string attrs = "";
                string p = "";
                string[] strs = md.AttrsInTable.Split('@');
                foreach (string str in strs)
                {
                    if (str == null || str == "")
                        continue;
                    string[] kv = str.Split('=');
                    string key = kv[0];
                    string val = kv[1];

                    i++;
                    if (this.Idx - 1 == i)
                    {
                        p = "@" + key + "=" + val;
                        continue;
                    }

                    if (this.Idx == i)
                    {
                        attrs += "@" + key + "=" + val + p;
                        continue;
                    }

                    attrs += "@" + key + "=" + val;
                }
                md.AttrsInTable = attrs;
                md.DirectUpdate();
                break;
            case "Right":
                MapData mdR = new MapData();
                mdR.No = this.FK_MapData;
                mdR.RetrieveFromDBSources();
                int iR = -1;
                string attrsR = "";
                string pR = "";
                string[] strsR = mdR.AttrsInTable.Split('@');
                foreach (string str in strsR)
                {
                    if (str == null || str == "")
                        continue;
                    string[] kv = str.Split('=');
                    string key = kv[0];
                    string val = kv[1];

                    iR++;

                    if (this.Idx == iR)
                    {
                        pR = "@" + key + "=" + val;
                        continue;
                    }

                    if (this.Idx + 1 == iR)
                    {
                        attrsR += "@" + key + "=" + val + pR;
                        continue;
                    }
                    attrsR += "@" + key + "=" + val;
                }
                mdR.AttrsInTable = attrsR;
                mdR.DirectUpdate();
                break;
            default:
                break;
        }

        switch (this.DoType)
        {
            case "ColumnsOrder":
                ColumnsOrder();
                break;
            case "SelectColumns":
                SelectColumns();
                break;
            case "SearchCond":
                this.SearchCond();
                break;
            default:
                this.BindHome();
                break;
        }
    }
    #region 显示顺序
    public void ColumnsOrder()
    {
        MapData md = new MapData();
        md.No = this.FK_MapData;
        md.RetrieveFromDBSources();

        MapAttrs attrs=new MapAttrs(this.FK_MapData);
        MapAttrs attrsOfSearch = new MapAttrs();
        string[] strs = md.AttrsInTable.Split('@');
        foreach (string str in strs)
        {
            if (str == null || str == "")
                continue;
            string[] kv = str.Split('=');

            MapAttr myattr = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, kv[0]) as MapAttr;
            if (myattr == null)
                continue;
            attrsOfSearch.AddEntity(myattr);
        }

        this.Pub2.AddTable("align=left");
        this.Pub2.AddCaptionLeft("列表字段显示顺序- 移动箭头改变顺序");

        this.Pub2.AddTR();
        int idx = -1;
        foreach (MapAttr attr in attrsOfSearch)
        {
            idx++;
            this.Pub2.Add("<TD class=Title>");
            if (idx != 0)
                this.Pub2.Add("<a href=\"javascript:DoLeft('" + FK_Flow + "','" + FK_MapData + "','" + idx + "')\" ><img src='/WF/Img/Arr/Arrowhead_Previous_S.gif' ></a>");

            this.Pub2.Add(attr.Name);
            if (idx != strs.Length-2)
                this.Pub2.Add("<a href=\"javascript:DoRight('" + FK_Flow + "','" + FK_MapData + "','" + idx + "')\" ><img src='/WF/Img/Arr/Arrowhead_Next_S.gif' ></a>");

            this.Pub2.Add("</TD>");
        }
        this.Pub2.AddTREnd();

       // AtPara ap=new AtPara(

        for (int i = 0; i < 12; i++)
        {
            this.Pub2.AddTR();
            foreach (MapAttr attr in attrsOfSearch)
                this.Pub2.AddTD();
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTableEnd();
    }
    void btn_ColumnsOrder_Click(object sender, EventArgs e)
    {
        MapData md = new MapData(this.FK_MapData);
        MapAttrs mattrs = new MapAttrs(md.No);
        string keys = "";
        foreach (MapAttr mattr in mattrs)
        {
            if (mattr.UIContralType != UIContralType.DDL)
                continue;

            CheckBox cb = this.Pub2.GetCBByID("CB_F_" + mattr.KeyOfEn);
            if (cb.Checked)
                keys += "@" + mattr.KeyOfEn;
        }
        md.SearchKeys = keys;
        md.Update();
        Cash.Map_Cash.Remove(this.FK_MapData);
        this.Alert("保存成功.");
    }
    #endregion

    #region 查询列表字段筛选
    public void SelectColumns()
    {
        this.Pub2.AddH2("请选择要显示的字段,然后点保存按钮,系统的查询列表就以您设计的体现.");
        this.Pub2.AddHR();

        this.Pub2.AddTable("width=90% align=left");
        GroupFields gfs = new GroupFields(this.FK_Flow);
        MapAttrs mattrs = new MapAttrs(this.FK_MapData);
        MapData md = new MapData(this.FK_MapData);
        bool isBr = false;
        string attrInTable = md.AttrsInTable;
        foreach (GroupField gf in gfs)
        {
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle(gf.Lab);
            this.Pub2.AddTREnd();

            this.Pub2.AddTR();
            this.Pub2.AddTDBigDocBegain();

            this.Pub2.AddTable("width=100% border=0");
              isBr = false;
            foreach (MapAttr attr in mattrs)
            {
                if (attr.GroupID != gf.OID)
                    continue;

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.KeyOfEn;
                cb.Text = attr.Name+"("+attr.KeyOfEn+")";
                cb.Checked = attrInTable.Contains("@" + attr.KeyOfEn + "=");
                 
                if (isBr == false)
                    this.Pub2.AddTR();
                this.Pub2.AddTD(cb);
                if (isBr)
                    this.Pub2.AddTREnd();

                isBr = !isBr;
            }
            this.Pub2.AddTableEnd();


            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();
        }
        this.Pub2.AddTR();
        this.Pub2.AddTDTitle("未分组");
        this.Pub2.AddTREnd();
        this.Pub2.AddTR();
        this.Pub2.AddTDBigDocBegain();

        this.Pub2.AddTable("width=100% border=0");
          isBr = false;
        foreach (MapAttr attr in mattrs)
        {
            if (gfs.Contains(attr.GroupID))
                continue;

            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + attr.KeyOfEn;
            cb.Text = attr.Name + "(" + attr.KeyOfEn + ")";

            cb.Checked = attrInTable.Contains("@" + attr.KeyOfEn + "=");

            if (isBr == false)
                this.Pub2.AddTR();
            this.Pub2.AddTD(cb);
            if (isBr)
                this.Pub2.AddTREnd();

            isBr = !isBr;
        }
        this.Pub2.AddTableEnd();

        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();


        this.Pub2.AddTRSum();
        this.Pub2.AddTDBegin();

        Button btn = new Button();
        btn.Text = " Save ";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_SelectColumns_Click);
        this.Pub2.Add(btn);

        this.Pub2.AddTDEnd();
        this.Pub2.AddTREnd();
        this.Pub2.AddTableEnd();
    }

    void btn_SelectColumns_Click(object sender, EventArgs e)
    {
        MapAttrs mattrs = new MapAttrs(this.FK_MapData);
        MapData md = new MapData(this.FK_MapData);
        string keys = "";
        foreach (MapAttr attr in mattrs)
        {
            CheckBox cb = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn);
            if (cb == null)
                continue;
            if (cb.Checked == false)
                continue;
            keys += "@" + attr.KeyOfEn + "=" + attr.Name;
        }
        md.AttrsInTable = keys;
        md.DirectUpdate();
        this.Response.Redirect("Home.aspx?FK_MapData=" + this.FK_MapData + "&FK_Flow=" + this.FK_Flow + "&DoType=ColumnsOrder", true);
    }
    #endregion

    #region 查询条件定义
    public void SearchCond()
    {
        MapData md = new MapData(this.FK_MapData);
        MapAttrs attrs = new MapAttrs(this.FK_MapData);
        MapAttrs attrsOfSearch = md.HisShowColsAttrs;

        #region 查询条件定义
        this.Pub2.AddH2("查询条件定义");

        this.Pub2.AddFieldSet("是否增加关键字查询");
        this.Pub2.Add("关键字查询是接受用户输入一个关键字，在整个表的列中用like 查询(外键、枚举、数值类型的除外)");
        this.Pub2.AddBR();
        CheckBox mycb = new CheckBox();
        mycb.ID = "CB_IsSearchKey";
        mycb.Text = "是否增加关键字查询";
        mycb.Checked = md.IsSearchKey;
        this.Pub2.Add(mycb);
        this.Pub2.AddFieldSetEnd();

        this.Pub2.AddFieldSet("外键与枚举类型");
        this.Pub2.Add("外键现枚举类型的数据才能进行下拉框查询，请选择要给查询。");
        this.Pub2.AddBR();

        foreach (MapAttr mattr in attrsOfSearch)
        {
            if (mattr.UIContralType != UIContralType.DDL)
                continue;

            CheckBox cb = new CheckBox();
            cb.ID = "CB_F_" + mattr.KeyOfEn;
            if (md.SearchKeys.Contains("@" + mattr.KeyOfEn))
                cb.Checked = true;

            cb.Text = mattr.Name + "(" + mattr.KeyOfEn + ")";
            this.Pub2.Add(cb);
            this.Pub2.AddBR();
        }
        this.Pub2.AddFieldSetEnd();

        bool isHave = false;
        foreach (MapAttr mattr in attrsOfSearch)
        {
            if (mattr.MyDataType== DataType.AppDate || mattr.MyDataType == DataType.AppDateTime)
            {
                isHave = true;
                break;
            }
        }

        if (isHave)
        {
            this.Pub2.AddFieldSet("时间段");
            this.Pub2.Add("对于数据进行按时间段的查询：比如对流程的发起时间进行发起时间从，到进行查询。");
            this.Pub2.AddBR();

            this.Pub2.Add("选择方式:");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_DTSearchWay";
            ddl.BindSysEnum("DTSearchWay");
            ddl.SetSelectItem((int)md.HisDTSearchWay);
            this.Pub2.Add(ddl);

            this.Pub2.Add("&nbsp;字段:");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_DTSearchKey";
            foreach (MapAttr mattr in attrsOfSearch)
            {
                if (mattr.MyDataType == DataType.AppDate || mattr.MyDataType == DataType.AppDateTime)
                {
                    if (mattr.UIVisible == false)
                        continue;
                    ddl.Items.Add(new ListItem(mattr.KeyOfEn + "  " + mattr.Name, mattr.KeyOfEn));
                }
            }
            ddl.SetSelectItem(md.DTSearchKey);
            this.Pub2.Add(ddl);
            this.Pub2.AddFieldSetEnd();
        }

        this.Pub2.AddHR();
        Button btn = new Button();
        btn.Text = "保存";
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_SearchCond_Click);
        this.Pub2.Add(btn);
        #endregion
    }
    void btn_SearchCond_Click(object sender, EventArgs e)
    {
        MapData md = new MapData(this.FK_MapData);
        MapAttrs mattrs = md.HisShowColsAttrs;
        string keys = "";
        foreach (MapAttr mattr in mattrs)
        {
            if (mattr.UIContralType != UIContralType.DDL)
                continue;
            CheckBox cb = this.Pub2.GetCBByID("CB_F_" + mattr.KeyOfEn);
            if (cb.Checked)
                keys += "@" + mattr.KeyOfEn;
        }

        md.SearchKeys = keys;
        md.IsSearchKey = this.Pub2.GetCBByID("CB_IsSearchKey").Checked;

        if (this.Pub2.IsExit("DDL_DTSearchWay") )
        {
            BP.Web.Controls.DDL ddl = this.Pub2.GetDDLByID("DDL_DTSearchWay");
            md.HisDTSearchWay = (DTSearchWay)ddl.SelectedItemIntVal;

            ddl = this.Pub2.GetDDLByID("DDL_DTSearchKey");
            md.DTSearchKey = ddl.SelectedItemStringVal;
        }
        md.Update();
        Cash.Map_Cash.Remove(this.FK_MapData);
        this.Alert("保存成功.");
    }
#endregion

    public void BindHome()
    {
        this.Pub2.AddH2("欢迎使用ccflow报表设计器.");
        this.Pub2.AddHR();

        this.Pub2.AddFieldSet("什么是流程数据？");
        this.Pub2.AddUL();
        this.Pub2.AddLi("流程数据查询");
        this.Pub2.AddLi("流程数据统计分析");
        this.Pub2.AddLi("流程数对比分析");
        this.Pub2.AddULEnd();
        this.Pub2.AddFieldSetEnd();

        this.Pub2.AddFieldSet("设计者必读？");
        string info = "";
        info += "<b>关于流程数据表:</b><br>";
        info += "流程数据是一个流程上所有节点表单字段合集组成的物理表，是以NDxxxRpt命名的，流程发起后就向这个物理表中增加一条数据。";
        info += "<br><b>如何进行权限控制:</b><br>";
        info += "数据权限是以查询与分析的部门条件进行控制的，一个操作员能够查询那些部门的数据是管理员在系统中维护的，存放在WF_DeptFlowSearch物理表中。";
        this.Pub2.Add(info);
        this.Pub2.AddULEnd();
        this.Pub2.AddFieldSetEnd();
    }
    public void BindLeft(Flow fl)
    {
        //this.Pub1.AddH2(fl.Name + " - 查询设计");

        this.Pub1.Add("<a href='http://ccflow.org' target=_b><img src='/DataUser/ICON/" + SystemConfig.CompanyID + "/LogBiger.png' border=0/></a>");
        this.Pub1.AddHR();
        this.Pub1.AddUL();
        this.Pub1.AddLi("<a href=\"Home.aspx?FK_Flow=" + this.FK_Flow + "&FK_MapData=" + this.FK_MapData + "\"><b>帮助</b></a>");
        this.Pub1.Add("流程报表设计器的基础使用方法。<br><br>");

        this.Pub1.AddLi("<a href=\"Home.aspx?DoType=SelectColumns&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + this.FK_MapData + "\"><b>查询列表字段筛选</b></a>");
        this.Pub1.Add("增加或移除查询结果集合中的列内容。<br><br>");

        this.Pub1.AddLi("<a href=\"Home.aspx?DoType=ColumnsOrder&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + this.FK_MapData + "\"><b>列表字段显示顺序</b></a>");
        this.Pub1.Add("设置查询结果集合中的列的位置。<br><br>");

        this.Pub1.AddLi("<a href=\"Home.aspx?DoType=SearchCond&FK_Flow=" + this.FK_Flow + "&FK_MapData=" + this.FK_MapData + "\"><b>查询条件设计</b></a>");
        this.Pub1.Add("为查询功能设计查询条件。<br><br>");

        this.Pub1.AddLi("<a href=\"javascript:DoReSet('" + this.FK_Flow + "','" + this.FK_MapData + "');\"><b>重设查询列表默认值</b></a>");
        this.Pub1.Add("按ccflow的默认值显示给用户。<br><br>");


        this.Pub1.AddLi("<a href=\"javascript:DoRightSet('" + this.FK_Flow + "','" + this.FK_MapData + "');\"><b>查询权限设置</b></a>");
        this.Pub1.Add("查询权限，用于设置那些人可以查询那些数据。<br><br>");

        //this.Pub1.AddLi("<a href=\"javascript:Card('" + this.FK_Flow + "','" + this.FK_MapData + "');\"><b>显示卡片</b></a>");
        //this.Pub1.Add("对卡片显示出来的流程详细信息内容进行设计。<br><br>");

        this.Pub1.AddLi("<a href='./../../Rpt/Search.aspx?FK_Flow=" + this.FK_Flow + "&DoType=Dept' target=_sar ><b>查询预览</b></a> -  <a href='./../../Rpt/Group.aspx?FK_Flow=" + this.FK_Flow + "&DoType=Dept' target=_sar ><b>统计预览</b></a>");
        this.Pub1.Add("预览查询与统计的设计结果。<br><br>");

        this.Pub1.AddULEnd();
    }
}