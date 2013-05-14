using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 按日期查询方式
    /// </summary>
    public enum DTSearchWay
    {
        /// <summary>
        /// 不设置
        /// </summary>
        None,
        /// <summary>
        /// 按日期
        /// </summary>
        ByDate,
        /// <summary>
        /// 按日期时间
        /// </summary>
        ByDateTime
    }
    /// <summary>
    /// 应用类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 流程表单
        /// </summary>
        Application = 0,
        /// <summary>
        /// 节点表单
        /// </summary>
        Node = 1
    }
    public enum FrmFrom
    {
        Flow,
        Node,
        Dtl
    }
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum FrmType
    {
        /// <summary>
        /// 自由表单
        /// </summary>
        CCForm=0,
        /// <summary>
        /// 傻瓜表单
        /// </summary>
        Column4Frm = 1,
        /// <summary>
        /// URL 表单(自定义)
        /// </summary>
        Url=2
    }
	/// <summary>
	/// 映射基础
	/// </summary>
    public class MapDataAttr : EntityNoNameAttr
    {
        public const string PTable = "PTable";
        public const string Dtls = "Dtls";
        public const string EnPK = "EnPK";
        public const string SearchKeys = "SearchKeys";
        //public const string CellsFrom = "CellsFrom";
        public const string FrmW = "FrmW";
        public const string FrmH = "FrmH";
        /// <summary>
        /// 来源
        /// </summary>
        public const string FrmFrom = "FrmFrom";
        /// <summary>
        /// DBURL
        /// </summary>
        public const string DBURL = "DBURL";
        /// <summary>
        /// 设计者
        /// </summary>
        public const string Designer = "Designer";
        /// <summary>
        /// 设计者单位
        /// </summary>
        public const string DesignerUnit = "DesignerUnit";
        /// <summary>
        /// 设计者联系方式
        /// </summary>
        public const string DesignerContact = "DesignerContact";
        /// <summary>
        /// 表单类别
        /// </summary>
        public const string FK_FrmSort = "FK_FrmSort";
        /// <summary>
        /// 在表格中显示的列
        /// </summary>
        public const string AttrsInTable = "AttrsInTable";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string AppType = "AppType";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// Tag
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 时间查询方式
        /// </summary>
        public const string DTSearchWay = "DTSearchWay";
        /// <summary>
        /// 时间查询字段
        /// </summary>
        public const string DTSearchKey = "DTSearchKey";
        /// <summary>
        /// 关键字查询
        /// </summary>
        public const string IsSearchKey = "IsSearchKey";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
	/// <summary>
	/// 映射基础
	/// </summary>
    public class MapData : EntityNoName
    {
        #region 外键属性
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrames MapFrames
        {
            get
            {
                MapFrames obj = this.GetRefObject("MapFrames") as MapFrames;
                if (obj == null)
                {
                    obj = new MapFrames(this.No);
                    this.SetRefObject("MapFrames", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public GroupFields GroupFields
        {
            get
            {
                GroupFields obj = this.GetRefObject("GroupFields") as GroupFields;
                if (obj == null)
                {
                    obj = new GroupFields(this.No);
                    this.SetRefObject("GroupFields", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 逻辑扩展
        /// </summary>
        public MapExts MapExts
        {
            get
            {
                MapExts obj = this.GetRefObject("MapExts") as MapExts;
                if (obj == null)
                {
                    obj = new MapExts(this.No);
                    this.SetRefObject("MapExts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                FrmEvents obj = this.GetRefObject("FrmEvents") as FrmEvents;
                if (obj == null)
                {
                    obj = new FrmEvents(this.No);
                    this.SetRefObject("FrmEvents", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 一对多
        /// </summary>
        public MapM2Ms MapM2Ms
        {
            get
            {
                MapM2Ms obj = this.GetRefObject("MapM2Ms") as MapM2Ms;
                if (obj == null)
                {
                    obj = new MapM2Ms(this.No);
                    this.SetRefObject("MapM2Ms", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 从表
        /// </summary>
        public MapDtls MapDtls
        {
            get
            {
                MapDtls obj = this.GetRefObject("MapDtls") as MapDtls;
                if (obj == null)
                {
                    obj = new MapDtls(this.No);
                    this.SetRefObject("MapDtls", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public FrmLinks FrmLinks
        {
            get
            {
                FrmLinks obj = this.GetRefObject("FrmLinks") as FrmLinks;
                if (obj == null)
                {
                    obj = new FrmLinks(this.No);
                    this.SetRefObject("FrmLinks", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtns FrmBtns
        {
            get
            {
                FrmBtns obj = this.GetRefObject("FrmLinks") as FrmBtns;
                if (obj == null)
                {
                    obj = new FrmBtns(this.No);
                    this.SetRefObject("FrmBtns", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 元素
        /// </summary>
        public FrmEles FrmEles
        {
            get
            {
                FrmEles obj = this.GetRefObject("FrmEles") as FrmEles;
                if (obj == null)
                {
                    obj = new FrmEles(this.No);
                    this.SetRefObject("FrmEles", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 线
        /// </summary>
        public FrmLines FrmLines
        {
            get
            {
                FrmLines obj = this.GetRefObject("FrmLines") as FrmLines;
                if (obj == null)
                {
                    obj = new FrmLines(this.No);
                    this.SetRefObject("FrmLines", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public FrmLabs FrmLabs
        {
            get
            {
                FrmLabs obj = this.GetRefObject("FrmLabs") as FrmLabs;
                if (obj == null)
                {
                    obj = new FrmLabs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public FrmImgs FrmImgs
        {
            get
            {
                FrmImgs obj = this.GetRefObject("FrmLabs") as FrmImgs;
                if (obj == null)
                {
                    obj = new FrmImgs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachments FrmAttachments
        {
            get
            {
                FrmAttachments obj = this.GetRefObject("FrmAttachments") as FrmAttachments;
                if (obj == null)
                {
                    obj = new FrmAttachments(this.No);
                    this.SetRefObject("FrmAttachments", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAths FrmImgAths
        {
            get
            {
                FrmImgAths obj = this.GetRefObject("FrmImgAths") as FrmImgAths;
                if (obj == null)
                {
                    obj = new FrmImgAths(this.No);
                    this.SetRefObject("FrmImgAths", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 单选按钮
        /// </summary>
        public FrmRBs FrmRBs
        {
            get
            {
                FrmRBs obj = this.GetRefObject("FrmRBs") as FrmRBs;
                if (obj == null)
                {
                    obj = new FrmRBs(this.No);
                    this.SetRefObject("FrmRBs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public MapAttrs MapAttrs
        {
            get
            {
                MapAttrs obj = this.GetRefObject("MapAttrs") as MapAttrs;
                if (obj == null)
                {
                    obj = new MapAttrs(this.No);
                    this.SetRefObject("MapAttrs", obj);
                }
                return obj;
            }
        }
        #endregion

        public static Boolean IsEditDtlModel
        {
            get
            {
                string s = BP.Web.WebUser.GetSessionByKey("IsEditDtlModel", "0");
                if (s == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                BP.Web.WebUser.SetSessionByKey("IsEditDtlModel", "1");
            }
        }
        /// <summary>
        /// 关键字查询
        /// </summary>
        public bool IsSearchKey
        {
            get
            {
                return this.GetValBooleanByKey(MapDataAttr.IsSearchKey);
            }
            set
            {
                this.SetValByKey(MapDataAttr.IsSearchKey, value);
            }
        }

        #region 属性
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        public DBUrlType HisDBUrl
        {
            get
            {
                return (DBUrlType)this.GetValIntByKey(MapDataAttr.DBURL);
            }
        }
        public int HisFrmTypeInt
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, value);
            }
        }
        public FrmType HisFrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, (int)value);
            }
        }
        public AppType HisAppType
        {
            get
            {
                return (AppType)this.GetValIntByKey(MapDataAttr.AppType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.AppType, (int)value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Note);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Note, value);
            }
        }
        /// <summary>
        /// 类别，可以为空.
        /// </summary>
        public string FK_FrmSort
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FrmSort);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FrmSort, value);
            }
        }
        /// <summary>
        /// 从表集合.
        /// </summary>
        public string Dtls
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Dtls);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Dtls, value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string EnPK
        {
            get
            {
                string s= this.GetValStrByKey(MapDataAttr.EnPK);
                if (string.IsNullOrEmpty(s))
                    return "OID";
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.EnPK, value);
            }
        }
        /// <summary>
        /// 查询键
        /// </summary>
        public string SearchKeys
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.SearchKeys);
            }
            set
            {
                this.SetValByKey(MapDataAttr.SearchKeys, value);
            }
        }
        /// <summary>
        /// 时间查询字段
        /// </summary>
        public string DTSearchKey
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.DTSearchKey);
            }
            set
            {
                this.SetValByKey(MapDataAttr.DTSearchKey, value);
            }
        }
        /// <summary>
        /// 时间查询方式
        /// </summary>
        public DTSearchWay HisDTSearchWay
        {
            get
            {
                return (DTSearchWay)this.GetValIntByKey(MapDataAttr.DTSearchWay);
            }
            set
            {
                this.SetValByKey(MapDataAttr.DTSearchWay, (int)value);
            }
        }
        /// <summary>
        /// 在表格中显示的列
        /// </summary>
        public string AttrsInTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.AttrsInTable);
                if (string.IsNullOrEmpty(s))
                    s = "@FK_Dept=发起人部门@FlowStarter=发起人@WFState=状态@Title=标题@FlowStartRDT=发起时间@FlowEmps=参与人@FlowDaySpan=时间跨度@FlowEnder=结束人@FlowEnderRDT=流程结束时间@FK_NY=年月@FlowEndNode=结束节点";
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.AttrsInTable, value);
            }
        }
        public Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    _HisEns = BP.DA.ClassFactory.GetEns(this.No);
                }
                return _HisEns;
            }
        }
        public Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        /// <summary>
        /// 在列表中显示的属性
        /// </summary>
        private Attrs _AttrsInTableEns = null;
        /// <summary>
        /// 在列表中显示的属性
        /// </summary>
        public Attrs AttrsInTableEns
        {
            get
            {
                if (_AttrsInTableEns == null)
                {
                    _AttrsInTableEns = new Attrs();
                    string attrKeyShow = this.AttrsInTable;
                    string[] strs = this.AttrsInTable.Split('@');
                    Map mp = this.HisEn.EnMap;
                    foreach (string str in strs)
                    {
                        if (str == null || str == "")
                            continue;

                        string[] kv = str.Split('=');
                        if (kv[0] == "MyNum")
                            continue;

                        _AttrsInTableEns.Add(mp.GetAttrByKey(kv[0]));
                    }
                }
                return _AttrsInTableEns;
            }
        }
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmW);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmW, value);
            }
        }
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmH);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmH, value);
            }
        }
        #endregion

        #region 构造方法
        public Map GenerHisMap()
        {
            MapAttrs mapAttrs =this.MapAttrs;
            if (mapAttrs.Count == 0)
            {
                this.RepairMap();
                mapAttrs = this.MapAttrs; 
            }

            Map map = new Map(this.PTable);
            DBUrl u = new DBUrl(this.HisDBUrl);
            map.EnDBUrl = u;
            map.EnDesc = this.Name;
            map.EnType = EnType.App;
            map.DepositaryOfEntity = Depositary.None;
            map.DepositaryOfMap = Depositary.Application;
            map.IsShowSearchKey = this.IsSearchKey;

            // 按日期查询.
            map.DTSearchWay = this.HisDTSearchWay;
            map.DTSearchKey = this.DTSearchKey;

            Attrs attrs = new Attrs();
            foreach (MapAttr mapAttr in mapAttrs)
                map.AddAttr(mapAttr.HisAttr);

            if (this.SearchKeys.Contains("@") == true)
            {
                string[] strs = this.SearchKeys.Split('@');
                foreach (string s in strs)
                {
                    if (s == "" || s == null)
                        continue;
                    try
                    {
                        map.AddSearchAttr(s);
                    }
                    catch
                    {
                    }
                }
            }

            // 产生从表。
            MapDtls dtls = this.MapDtls; // new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                BP.Sys.GEDtls dtls1 = new GEDtls(dtl.No);
                map.AddDtl(dtls1, "RefPK");
            }
            return map;
        }
        private GEEntity _HisEn = null;
        public GEEntity HisGEEn
        {
            get
            {
                if (this._HisEn == null)
                    _HisEn = new GEEntity(this.No);
                return _HisEn;
            }
        }
        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public GEEntity GenerGEEntityByDataSet(DataSet ds)
        {
            // New 它的实例.
            GEEntity en = this.HisGEEn;

            // 它的table.
            DataTable dt = ds.Tables[this.No];
            
            //装载数据.
            en.Row.LoadDataTable(dt, dt.Rows[0]);

            // dtls.
            MapDtls dtls = this.MapDtls;
            foreach (MapDtl item in dtls)
            {
                DataTable dtDtls = ds.Tables[item.No];
                GEDtls dtlsEn = new GEDtls(item.No);
                foreach (DataRow dr  in dtDtls.Rows)
                {
                    // 产生它的Entity data.
                    GEDtl dtl = (GEDtl)dtlsEn.GetNewEntity;
                    dtl.Row.LoadDataTable(dtDtls, dr);

                    //加入这个集合.
                    dtlsEn.AddEntity(dtl);
                }

                //加入到他的集合里.
                en.Dtls.Add(dtDtls);
            }
            return en;
        }
        /// <summary>
        /// 生成map.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static Map GenerHisMap(string no)
        {
            if (SystemConfig.IsDebug)
            {
                MapData md = new MapData();
                md.No = no;
                md.Retrieve();
                return md.GenerHisMap();
            }
            else
            {
                Map map = BP.DA.Cash.GetMap(no);
                if (map == null)
                {
                    MapData md = new MapData();
                    md.No = no;
                    md.Retrieve();
                    map = md.GenerHisMap();
                    BP.DA.Cash.SetMap(no, map);
                }
                return map;
            }
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        public MapData()
        {
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapData(string no):base(no)
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_MapData");
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "映射基础";
                map.EnType = EnType.Sys;
                map.CodeStruct = "4";

                map.AddTBStringPK(MapDataAttr.No, null, "编号", true, false, 1, 20, 20);
                map.AddTBString(MapDataAttr.Name, null, "描述", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.EnPK, null, "实体主键", true, false, 0, 10, 20);
                map.AddTBString(MapDataAttr.SearchKeys, null, "查询键", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.PTable, null, "物理表", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Dtls, null, "从表", true, false, 0, 500, 20);

                map.AddTBInt(MapDataAttr.FrmW, 900, "FrmW", true, true);
                map.AddTBInt(MapDataAttr.FrmH, 1200, "FrmH", true, true);

                //数据源.
                map.AddTBInt(MapDataAttr.DBURL, 0, "DBURL", true, false);

                //Tag
                map.AddTBString(MapDataAttr.Tag, null, "Tag", true, false, 0, 500, 20);

                //FrmType  @自由表单，@傻瓜表单，@自定义表单.
                map.AddTBInt(MapDataAttr.FrmType, 0, "表单类型", true, false);


                // 可以为空这个字段。
                map.AddTBString(MapDataAttr.FK_FrmSort, null, "表单类别", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.AttrsInTable, null, "在表格中显示的列", true, false, 0, 3800, 20);
                // enumAppType
                map.AddTBInt(MapDataAttr.AppType, 1, "应用类型", true, false);

                //时间查询:用于报表查询.
                map.AddTBInt(MapDataAttr.IsSearchKey, 0, "是否需要关键字查询", true, false);
                map.AddTBInt(MapDataAttr.DTSearchWay, 0, "时间查询方式", true, false);
                map.AddTBString(MapDataAttr.DTSearchKey, null, "查询字段", true, false, 0, 50, 20);

                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);

                //增加参数字段.
                map.AddTBAtParas(1000);

                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, false, 0, 128, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public MapAttrs HisShowColsAttrs
        {
            get
            {
                MapAttrs mattrs =  new MapAttrs(this.No);
                MapAttrs attrs = new MapAttrs();

                string[] strs = this.AttrsInTable.Split('@');
                foreach (string str in strs)
                {
                    if (str == null || str == "")
                        continue;
                    string[] kv = str.Split('=');
                    MapAttr myattr = mattrs.GetEntityByKey(MapAttrAttr.KeyOfEn, kv[0]) as MapAttr;
                    if (myattr == null)
                        continue;
                    attrs.AddEntity(myattr);
                }
                return attrs;
            }
        }
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static MapData ImpMapData(DataSet ds)
        {
            return ImpMapData(ds, true);
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="isSetReadony"></param>
        /// <returns></returns>
        public static MapData ImpMapData( DataSet ds, bool isSetReadony)
        {
            string errMsg = "";
            if (ds.Tables.Contains("WF_Node") == true)
                errMsg += "@此模板文件为流程模板。";
            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";
            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";
            if (errMsg != "")
                throw new Exception(errMsg);

            DataTable dt = ds.Tables["Sys_MapData"];
            string fk_mapData = dt.Rows[0]["No"].ToString();
            MapData md = new MapData();
            md.No = fk_mapData;
            if (md.IsExits)
                throw new Exception("已经存在(" + fk_mapData + ")的数据。");

            //导入.
            return ImpMapData(fk_mapData, ds, isSetReadony);
        }
        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ds">表单数据</param>
        /// <param name="isSetReadonly">是否设置只读？</param>
        /// <returns></returns>
        public static MapData ImpMapData(string fk_mapdata, DataSet ds, bool isSetReadonly)
        {
            #region 检查导入的数据是否完整.
            string errMsg = "";
            //if (ds.Tables[0].TableName != "Sys_MapData")
            //    errMsg += "@非表单模板。";

            if (ds.Tables.Contains("WF_Node") == true)
                errMsg += "@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";

            DataTable dtCheck = ds.Tables["Sys_MapAttr"];
            bool isHave = false;
            foreach (DataRow dr in dtCheck.Rows)
            {
                if (dr["KeyOfEn"].ToString() == "OID")
                {
                    isHave = true;
                    break;
                }
            }

            if (isHave == false)
                errMsg += "@缺少列:OID";

            if (errMsg != "")
                throw new Exception("以下错误不可导入，可能的原因是非表单模板文件:" + errMsg);
            #endregion

            // 定义在最后执行的sql.
            string endDoSQL = "";

            //检查是否存在OID字段.
            MapData mdOld = new MapData();
            mdOld.No = fk_mapdata;
            mdOld.Delete();

            // 求出dataset的map.
            string oldMapID = "";
            DataTable dtMap = ds.Tables["Sys_MapData"];
            foreach (DataRow dr in dtMap.Rows)
            {
                if (dr["No"].ToString().Contains("Dtl"))
                    continue;
                oldMapID = dr["No"].ToString();
            }

            string timeKey = DateTime.Now.ToString("MMddHHmmss");
            // string timeKey = fk_mapdata;
            foreach (DataTable dt in ds.Tables)
            {
                int idx = 0;
                switch (dt.TableName)
                {
                    case "Sys_MapDtl":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapDtl dtl = new MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                dtl.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly)
                            {
                                //dtl.IsReadonly = true;

                                dtl.IsInsert = false;
                                dtl.IsUpdate = false;
                                dtl.IsDelete = false;
                            }

                            dtl.Insert();
                        }
                        break;
                    case "Sys_MapData":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapData md = new MapData();

                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                md.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                                //md.SetValByKey(dc.ColumnName, val);
                            }
                            if (string.IsNullOrEmpty(md.PTable.Trim()))
                                md.PTable = md.No;

                            md.DirectInsert();
                        }
                        break;
                    case "Sys_FrmBtn":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmBtn en = new FrmBtn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly == true)
                                en.IsEnable = false;


                            en.MyPK = "Btn_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLine":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLine en = new FrmLine();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "LE_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLab":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLab en = new FrmLab();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //  en.FK_MapData = fk_mapdata; 删除此行解决从表lab的问题。
                            en.MyPK = "LB_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLink":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLink en = new FrmLink();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "LK_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmEle":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEle en = new FrmEle();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly == true)
                                en.IsEnable = false;

                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImg":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Img_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImgAth":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImgAth en = new FrmImgAth();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "ImgA_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmRB":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                           
                            try
                            {
                                en.Save();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_FrmAttachment":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Ath_" + idx + "_" + fk_mapdata;
                            if (isSetReadonly == true)
                            {
                                en.IsDelete = false;
                                en.IsUpload = false;
                            }

                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_MapM2M":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapM2M en = new MapM2M();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //   en.NoOfObj = "M2M_" + idx + "_" + fk_mapdata;
                            if (isSetReadonly == true)
                            {
                                en.IsDelete = false;
                                en.IsInsert = false;
                            }
                            en.Insert();
                        }
                        break;
                    case "Sys_MapFrame":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapFrame en = new MapFrame();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.NoOfObj = "Fra_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapExt en = new MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Ext_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_MapAttr":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapAttr en = new MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            if (isSetReadonly == true)
                            {
                                if (en.DefValReal != null 
                                    && (en.DefValReal.Contains("@WebUser.") 
                                    || en.DefValReal.Contains("@RDT")))
                                    en.DefValReal = "";

                                switch (en.UIContralType)
                                {
                                    case UIContralType.DDL:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.TB:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.RadioBtn:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.CheckBok:
                                        en.UIIsEnable = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            en.MyPK = en.FK_MapData + "_" + en.KeyOfEn;
                            en.DirectInsert();
                        }
                        break;
                    case "Sys_GroupField":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            GroupField en = new GroupField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            int beforeID = en.OID;
                            en.OID = 0;
                            en.Insert();
                            endDoSQL += "@UPDATE Sys_MapAttr SET GroupID=" + en.OID + " WHERE FK_MapData='" + fk_mapdata + "' AND GroupID=" + beforeID;
                        }
                        break;
                    case "Sys_Enum":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                            if (se.IsExits)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain":
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
                    default:
                        break;
                }
            }
            //执行最后结束的sql.
            DBAccess.RunSQLs(endDoSQL);

            MapData mdNew = new MapData(fk_mapdata);
            mdNew.RepairMap();
          //  mdNew.FK_FrmSort = fk_sort;
            mdNew.Update();
            return mdNew;
        }
        public void RepairMap()
        {
            GroupFields gfs = new GroupFields(this.No);
            if (gfs.Count == 0)
            {
                GroupField gf = new GroupField();
                gf.EnName = this.No;
                gf.Lab =this.Name;
                gf.Insert();
                string sqls = "";
                sqls += "@UPDATE Sys_MapDtl SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapFrame SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapM2M SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                DBAccess.RunSQLs(sqls);
            }
            else
            {
                GroupField gfFirst = gfs[0] as GroupField;
                string sqls = "";
                sqls += "@UPDATE Sys_MapDtl SET GroupID=" + gfFirst.OID + "        WHERE  No   IN (SELECT No   FROM Sys_MapDtl        WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "'))AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gfFirst.OID + "       WHERE  MyPK IN (SELECT MyPK FROM Sys_MapAttr       WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "'))AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_MapFrame SET GroupID=" + gfFirst.OID + "      WHERE  MyPK IN (SELECT MyPK FROM Sys_MapFrame      WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "'))AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_MapM2M SET GroupID=" + gfFirst.OID + "        WHERE  MyPK IN (SELECT MyPK FROM Sys_MapM2M        WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "'))AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gfFirst.OID + " WHERE  MyPK IN (SELECT MyPK FROM Sys_FrmAttachment WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "'))AND FK_MapData='" + this.No + "' ";
                DBAccess.RunSQLs(sqls);
            }

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();

            if (this.EnPK == "OID")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = "OID";
                    attr.Name = "OID";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }
            if (this.EnPK == "No" || this.EnPK == "MyPK")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, this.EnPK, MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = this.EnPK;
                    attr.Name = this.EnPK;
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }

            if (attr.IsExit(MapAttrAttr.KeyOfEn, "RDT", MapAttrAttr.FK_MapData, this.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "RDT";
                attr.Name = "更新时间";

                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }
        }
        protected override bool beforeInsert()
        {
            this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeDelete()
        {
            #region 删除物理表。
            try
            {
                BP.DA.DBAccess.RunSQL("DROP TABLE " + this.PTable);
            }
            catch
            {
            }
            Sys.MapDtls dtls = new BP.Sys.MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                try
                {
                    DBAccess.RunSQL("DROP TABLE " + dtl.PTable);
                }
                catch
                {
                }
                dtl.Delete();
            }
            #endregion

            string sql = "";
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData ='" + this.No + "'";
            DataTable Sys_MapDtl = DBAccess.RunSQLReturnTable(sql);
            string ids = "'" + this.No + "'";
            foreach (DataRow dr in Sys_MapDtl.Rows)
                ids += ",'" + dr["No"] + "'";

            string where = " FK_MapData IN (" + ids + ")";

            #region 删除相关的数据。
            sql += "@DELETE FROM Sys_MapDtl WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmLine WHERE " + where;
            sql += "@DELETE FROM Sys_FrmEle WHERE " + where;
            sql += "@DELETE FROM Sys_FrmEvent WHERE " + where;
            sql += "@DELETE FROM Sys_FrmBtn WHERE " + where;
            sql += "@DELETE FROM Sys_FrmLab WHERE " + where;
            sql += "@DELETE FROM Sys_FrmLink WHERE " + where;
            sql += "@DELETE FROM Sys_FrmImg WHERE " + where;
            sql += "@DELETE FROM Sys_FrmImgAth WHERE " + where;
            sql += "@DELETE FROM Sys_FrmRB WHERE " + where;
            sql += "@DELETE FROM Sys_FrmAttachment WHERE " + where;
            sql += "@DELETE FROM Sys_MapM2M WHERE " + where;
            sql += "@DELETE FROM Sys_MapFrame WHERE " + where;
            sql += "@DELETE FROM Sys_MapExt WHERE " + where;
            sql += "@DELETE FROM Sys_MapAttr WHERE " + where;
            sql += "@DELETE FROM Sys_GroupField WHERE EnName IN (" + ids + ")";
            sql += "@DELETE FROM Sys_MapData WHERE No IN (" + ids + ")";
            sql += "@DELETE FROM Sys_M2M WHERE " + where;
            DBAccess.RunSQLs(sql);
            #endregion 删除相关的数据。

            return base.beforeDelete();
        }
        public System.Data.DataSet GenerHisDataSet()
        {
            DataSet ds = new DataSet();
            string sql = "";

            // Sys_MapDtl.
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData ='" + this.No + "'";
            DataTable Sys_MapDtl = DBAccess.RunSQLReturnTable(sql);
            Sys_MapDtl.TableName = "Sys_MapDtl";
            ds.Tables.Add(Sys_MapDtl);
            string ids = "'" + this.No + "'";
            foreach (DataRow dr in Sys_MapDtl.Rows)
            {
                ids += ",'" + dr["No"] + "'";
            }
            string where = " FK_MapData IN (" + ids + ")";

            // Sys_MapData.
            sql = "SELECT * FROM Sys_MapData WHERE No IN (" + ids + ")";
            DataTable Sys_MapData = DBAccess.RunSQLReturnTable(sql);
            Sys_MapData.TableName = "Sys_MapData";
            ds.Tables.Add(Sys_MapData);

            // line.
            sql = "SELECT * FROM Sys_FrmLine WHERE " + where;
            DataTable dtLine = DBAccess.RunSQLReturnTable(sql);
            dtLine.TableName = "Sys_FrmLine";
            ds.Tables.Add(dtLine);

            // ele.
            sql = "SELECT * FROM Sys_FrmEle WHERE " + where;
            DataTable dtFrmEle = DBAccess.RunSQLReturnTable(sql);
            dtFrmEle.TableName = "Sys_FrmEle";
            ds.Tables.Add(dtFrmEle);

            // link.
            sql = "SELECT * FROM Sys_FrmLink WHERE " + where;
            DataTable dtLink = DBAccess.RunSQLReturnTable(sql);
            dtLink.TableName = "Sys_FrmLink";
            ds.Tables.Add(dtLink);

            // btn.
            sql = "SELECT * FROM Sys_FrmBtn WHERE " + where;
            DataTable dtBtn = DBAccess.RunSQLReturnTable(sql);
            dtBtn.TableName = "Sys_FrmBtn";
            ds.Tables.Add(dtBtn);

            // Sys_FrmImg.
            sql = "SELECT * FROM Sys_FrmImg WHERE " + where;
            DataTable dtFrmImg = DBAccess.RunSQLReturnTable(sql);
            dtFrmImg.TableName = "Sys_FrmImg";
            ds.Tables.Add(dtFrmImg);

            // Sys_FrmLab.
            sql = "SELECT * FROM Sys_FrmLab WHERE " + where;
            DataTable Sys_FrmLab = DBAccess.RunSQLReturnTable(sql);
            Sys_FrmLab.TableName = "Sys_FrmLab";
            ds.Tables.Add(Sys_FrmLab);

            // Sys_FrmLab.
            sql = "SELECT * FROM Sys_FrmRB WHERE " + where;
            DataTable Sys_FrmRB = DBAccess.RunSQLReturnTable(sql);
            Sys_FrmRB.TableName = "Sys_FrmRB";
            ds.Tables.Add(Sys_FrmRB);

            // Sys_MapAttr.
            sql = "SELECT * FROM Sys_MapAttr WHERE " + where + " AND KeyOfEn NOT IN('WFState') ORDER BY FK_MapData,IDX ";
            DataTable Sys_MapAttr = DBAccess.RunSQLReturnTable(sql);
            Sys_MapAttr.TableName = "Sys_MapAttr";
            if (Sys_MapAttr.Rows.Count == 0)
            {
                BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.KeyOfEn = "OID";
                attr.Name = "OID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }
            ds.Tables.Add(Sys_MapAttr);

            // Sys_MapM2M.
            sql = "SELECT * FROM Sys_MapM2M WHERE " + where;
            DataTable Sys_MapM2M = DBAccess.RunSQLReturnTable(sql);
            Sys_MapM2M.TableName = "Sys_MapM2M";
            ds.Tables.Add(Sys_MapM2M);

            // Sys_FrmAttachment.
            sql = "SELECT * FROM Sys_FrmAttachment WHERE " + where;
            DataTable Sys_FrmAttachment = DBAccess.RunSQLReturnTable(sql);
            Sys_FrmAttachment.TableName = "Sys_FrmAttachment";
            ds.Tables.Add(Sys_FrmAttachment);

            // Sys_FrmImgAth.
            sql = "SELECT * FROM Sys_FrmImgAth WHERE " + where;
            DataTable Sys_FrmImgAth = DBAccess.RunSQLReturnTable(sql);
            Sys_FrmImgAth.TableName = "Sys_FrmImgAth";
            ds.Tables.Add(Sys_FrmImgAth);

            // Sys_MapExt.
            sql = "SELECT * FROM Sys_MapExt WHERE " + where;
            DataTable Sys_MapExt = DBAccess.RunSQLReturnTable(sql);
            Sys_MapExt.TableName = "Sys_MapExt";
            ds.Tables.Add(Sys_MapExt);

            // Sys_GroupField.
            sql = "SELECT * FROM Sys_GroupField WHERE  EnName IN (" + ids + ")";
            DataTable Sys_GroupField = DBAccess.RunSQLReturnTable(sql);
            Sys_GroupField.TableName = "Sys_GroupField";
            ds.Tables.Add(Sys_GroupField);

           
            // Sys_Enum
            sql = "SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + ids + ") )";
            DataTable Sys_Enum = DBAccess.RunSQLReturnTable(sql);
            Sys_Enum.TableName = "Sys_Enum";
            ds.Tables.Add(Sys_Enum);
            return ds;
        }
        /// <summary>
        /// 生成自动的ｊｓ程序。
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <param name="tbPer"></param>
        /// <returns></returns>
        public static string GenerAutoFull(string pk, MapAttrs attrs, MapAttr attr, string tbPer)
        {
            string left = "\n document.forms[0]." + tbPer + "_TB" + attr.KeyOfEn + "_" + pk + ".value = ";
            string right = attr.AutoFullDoc;
            foreach (MapAttr mattr in attrs)
            {
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tbPer + "_TB_" + mattr.KeyOfEn + "_" + pk + ".value) ");
            }
            return " alert( document.forms[0]." + tbPer + "_TB" + attr.KeyOfEn + "_" + pk + ".value ) ; \t\n " + left + right;
        }
    }
	/// <summary>
	/// 映射基础s
	/// </summary>
    public class MapDatas : EntitiesMyPK
	{		
		#region 构造
        /// <summary>
        /// 映射基础s
        /// </summary>
		public MapDatas()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new MapData();
			}
		}
		#endregion
	}
}
