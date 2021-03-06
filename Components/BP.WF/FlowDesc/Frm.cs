using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF
{
	/// <summary>
	/// Frm属性
	/// </summary>
    public class FrmAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FormType = "FormType";
        /// <summary>
        /// URL
        /// </summary>
        public const string URL = "URL";
        /// <summary>
        /// 是否可以更新
        /// </summary>
        public const string IsUpdate = "IsUpdate";
        /// <summary>
        /// PTable
        /// </summary>
        public const string PTable = "PTable";
        /// <summary>
        /// DBURL
        /// </summary>
        public const string DBURL = "DBURL";
    }
	/// <summary>
	/// Frm
	/// </summary>
    public class Frm : EntityNoName
    {
        #region 基本属性
        public FrmNode HisFrmNode = null;
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.PTable);
            }
            set
            {
                this.SetValByKey(FrmAttr.PTable, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FrmAttr.FK_Flow, value);
            }
        }
        public string URL
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.URL);
            }
            set
            {
                this.SetValByKey(FrmAttr.URL, value);
            }
        }
        public FormType HisFormType
        {
            get
            {
                return (FormType)this.GetValIntByKey(FrmAttr.FormType);
            }
            set
            {
                this.SetValByKey(FrmAttr.FormType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Frm
        /// </summary>
        public Frm()
        {
        }
        /// <summary>
        /// Frm
        /// </summary>
        /// <param name="no"></param>
        public Frm(string no)
            : base(no)
        {

        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                //Map map = new Map("Sys_MapData");

                Map map = new Map("Sys_MapData");

                map.EnDesc = "节点表单";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.CodeStruct = "4";
                map.IsAutoGenerNo = false;

                map.AddTBStringPK(FrmAttr.No, null, null, true, true, 1, 4, 4);
                map.AddTBString(FrmAttr.Name, null, null, true, false, 0, 50, 10);
                map.AddTBString(FrmAttr.FK_Flow, null, "流程表单属性:FK_Flow", true, false, 0, 50, 10);
                map.AddDDLSysEnum(FrmAttr.FormType, 0, "流程表单属性:表单类型", true, false, FrmAttr.FormType);
                map.AddTBString(FrmAttr.PTable, null, "PTable", true, false, 0, 50, 10);
                map.AddTBInt(FrmAttr.DBURL, 0, "DBURL", true, false);
                map.AddTBString(FrmAttr.URL, null, "流程表单属性:Url", true, false, 0, 50, 10);

                map.AddTBString(MapDataAttr.FK_FrmSort, "01", "表单类别", true, false, 0, 500, 20);

                map.AddTBInt(Sys.MapDataAttr.FrmW, 900, "表单宽度", true, false);
                map.AddTBInt(Sys.MapDataAttr.FrmH, 1200, "表单高度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        public int FrmW
        {
            get
            {
                return this.GetValIntByKey(Sys.MapDataAttr.FrmW);
            }
        }
        public int FrmH
        {
            get
            {
                return this.GetValIntByKey(Sys.MapDataAttr.FrmH);
            }
        }
        
        #endregion
    }
	/// <summary>
	/// Frm
	/// </summary>
    public class Frms : EntitiesNoName
    {
        /// <summary>
        /// Frm
        /// </summary>
        public Frms()
        {
        }
        /// <summary>
        /// Frm
        /// </summary>
        /// <param name="fk_flow"></param>
        public Frms(string fk_flow)
        {
            this.Retrieve(FrmAttr.FK_Flow, fk_flow);
        }
        public Frms(int fk_node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(FrmAttr.No, "SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + fk_node);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Frm();
            }
        }
    }
}
