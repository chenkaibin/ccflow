using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
	/// <summary>
	/// 用户自定义表
	/// </summary>
    public class SFTableAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 字段
        /// </summary>
        public const string FK_Val = "FK_Val";
        /// <summary>
        /// 描述
        /// </summary>
        public const string TableDesc = "TableDesc";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
    }
	/// <summary>
	/// 用户自定义表
	/// </summary>
    public class SFTable : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 是否是类
        /// </summary>
        public bool IsClass
        {
            get
            {
                if (this.No.Contains("."))
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 值
        /// </summary>
        public string FK_Val
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.FK_Val);
            }
            set
            {
                this.SetValByKey(SFTableAttr.FK_Val, value);
            }
        }
        public string TableDesc
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.TableDesc);
            }
            set
            {
                this.SetValByKey(SFTableAttr.TableDesc, value);
            }
        }
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(SFTableAttr.DefVal);
            }
            set
            {
                this.SetValByKey(SFTableAttr.DefVal, value);
            }
        }
        public EntitiesNoName HisEns
        {
            get
            {
                if (this.IsClass)
                {
                    EntitiesNoName ens = (EntitiesNoName)BP.DA.ClassFactory.GetEns(this.No);
                    ens.RetrieveAll();
                    return ens;
                }

                BP.En.GENoNames ges = new GENoNames(this.No, this.Name);
                ges.RetrieveAll();
                return ges;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTable()
        {

        }
        public SFTable(string mypk)
        {
            this.No = mypk;
            try
            {
                this.Retrieve();
            }
            catch (Exception ex)
            {
                switch (this.No)
                {
                    case "BP.Pub.NYs":
                        this.Name = "年月";
                      //  this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_NY";
                     //   this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.YFs":
                        this.Name = "月";
                      //  this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_YF";
                       // this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.Days":
                        this.Name = "天";
                     //   this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_Day";
                        //this.IsEdit = true;
                        this.Insert();
                        break;
                    case "BP.Pub.NDs":
                        this.Name = "年";
                     //   this.HisSFTableType = SFTableType.ClsLab;
                        this.FK_Val = "FK_ND";
                       // this.IsEdit = true;
                        this.Insert();
                        break;
                    default:
                        throw new Exception(ex.Message);
                }
            }
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
                Map map = new Map("Sys_SFTable");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "用户自定义表";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(SFTableAttr.No, null, "编号", true, false, 1, 20, 20);
                map.AddTBString(SFTableAttr.Name, null, "表名称", true, false, 0, 30, 20);
                map.AddTBString(SFTableAttr.FK_Val, null, "字段（显示在物理表）", true, false, 0, 50, 20);
           //     map.AddDDLSysEnum(SFTableAttr.SFTableType, 0, "表类型", true, false, SFTableAttr.SFTableType, "@0=用户定义@1=类库@2=系统表");
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 50, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值(特殊)", true, false, 0, 200, 20);
             //   map.AddBoolean(SFTableAttr.IsEdit, true, "是否可编辑", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeDelete()
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.No);
            if (attrs.Count != 0)
            {
                string err = "";
                foreach (MapAttr item in attrs)
                    err += " @ " + item.MyPK + " " + item.Name ;
                throw new Exception("@如下实体字段在引用:"+err);
            }
            return base.beforeDelete();
        }
    }
	/// <summary>
	/// 用户自定义表s
	/// </summary>
    public class SFTables : EntitiesNoName
	{		
		#region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
		public SFTables()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new SFTable();
			}
		}
		#endregion
	}
}
