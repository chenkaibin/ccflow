using System;
using System.IO;
using System.Collections;
using BP.DA;
using BP.En;
//using BP.ZHZS.Base;

namespace BP.Sys
{
	/// <summary>
	/// 文件类型
	/// </summary>
    public class SysFileTypeAttr
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
    }
	/// <summary>
	/// SysFileType
	/// </summary>
    public class SysFileType : EntityNoName
    {
        #region 实现基本属性
        public string Icon
        {
            get
            {
                return this.GetValStringByKey(SysFileTypeAttr.Icon);
            }
            set
            {
                this.SetValByKey(SysFileTypeAttr.Icon, value);
            }
        }
        #endregion

        #region 构造方法
        public SysFileType() { }
        /// <summary>
        /// 文件管理者
        /// </summary>
        /// <param name="_No"></param>
        public SysFileType(string _No)
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
                Map map = new Map("Sys_FileType");
                map.EnType = EnType.Sys;
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.EnDesc = "文件类型";
                map.AddTBStringPK(SysFileTypeAttr.No, null, "编号", true, false, 1, 10, 10);
                map.AddTBString(SysFileTypeAttr.Name, null, "名称", true, false, 0, 50, 20);
                map.AddTBString(SysFileTypeAttr.Icon, "图标", "图标", true, false, 0, 50, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 文件管理者 
	/// </summary>
	public class SysFileTypes :Entities
	{
		/// <summary>
		/// 文件管理者s
		/// </summary>
		public SysFileTypes(){}		 
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new SysFileType();
			}
		}
	}
}
