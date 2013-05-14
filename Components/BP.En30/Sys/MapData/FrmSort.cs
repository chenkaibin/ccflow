using System;
using System.IO;
using System.Collections;
using BP.DA;
using BP.En;
//using BP.ZHZS.Base;

namespace BP.Sys
{
	/// <summary>
	/// 表单类别
	/// </summary>
    public class FrmSortAttr
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 图标
        /// </summary>
        public const string Icon = "Icon";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
	/// <summary>
    /// 表单类别
	/// </summary>
    public class FrmSort : EntityNoName
    {
        #region 实现基本属性
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(FrmSortAttr.Icon);
            }
            set
            {
                this.SetValByKey(FrmSortAttr.Icon, value);
            }
        }
        #endregion

        #region 构造方法
        public FrmSort() { }
        /// <summary>
        /// 文件管理者
        /// </summary>
        /// <param name="_No"></param>
        public FrmSort(string _No)
        {
            this.No = _No;
            try
            {
                this.Retrieve();
            }
            catch
            {
                this.Name = "未知类型";
                this.Insert();
            }
        }
        /// <summary>
        /// map 
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_FrmSort");
                map.EnType = EnType.Sys;
                map.CodeStruct = "2";
                map.IsAllowRepeatNo = false;
                map.IsAutoGenerNo = true;
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.EnDesc = "表单类别";
                map.AddTBStringPK(FrmSortAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(FrmSortAttr.Name, null, "名称", true, false, 0, 50, 20);
                //   map.AddTBString(FrmSortAttr.Icon, "图标", "图标", true, false, 0, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// 表单类别s 
	/// </summary>
	public class FrmSorts :EntitiesNoName
	{
		/// <summary>
		/// 文件管理者s
		/// </summary>
		public FrmSorts(){}		 
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new FrmSort();
			}
		}
        public override int RetrieveAll()
        {
            int i= base.RetrieveAll();
            if (i == 0)
            {
                FrmSort fs = new FrmSort();
                fs.No = "01";
                fs.Name = "默认类别";
                fs.Insert();
                return base.RetrieveAll();
            }
            else
            {
                return i;
            }
        }
	}
}
