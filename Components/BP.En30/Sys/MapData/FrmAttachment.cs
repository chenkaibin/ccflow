using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 附件上传类型
    /// </summary>
    public enum AttachmentUploadType
    {
        /// <summary>
        /// 单个的
        /// </summary>
        Single,
        /// <summary>
        /// 多个的
        /// </summary>
        Multi,
        /// <summary>
        /// 指定的
        /// </summary>
        Specifically
    }
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachmentAttr : EntityMyPKAttr
    {
        /// <summary>
        /// Name
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 要求上传的格式
        /// </summary>
        public const string Exts = "Exts";
        /// <summary>
        /// 附件编号
        /// </summary>
        public const string NoOfObj = "NoOfObj";
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public const string IsUpload = "IsUpload";
        /// <summary>
        /// 是否增加
        /// </summary>
        public const string IsNote = "IsNote";
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public const string IsDelete = "IsDelete";
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public const string IsDownload = "IsDownload";
        /// <summary>
        /// 保存到
        /// </summary>
        public const string SaveTo = "SaveTo";
        /// <summary>
        /// 类别
        /// </summary>
        public const string Sort = "Sort";
        /// <summary>
        /// 上传类型
        /// </summary>
        public const string UploadType = "UploadType";
        /// <summary>
        /// RowIdx
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public const string IsAutoSize = "IsAutoSize";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachment : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 上传类型
        /// </summary>
        public AttachmentUploadType UploadType
        {
            get
            {
                return (AttachmentUploadType)this.GetValIntByKey(FrmAttachmentAttr.UploadType);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.UploadType, (int)value);
            }
        }
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public bool IsUpload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsUpload, value);
            }
        }
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public bool IsDownload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsDownload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsDownload, value);
            }
        }
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsDelete, value);
            }
        }
        /// <summary>
        /// 自动控制大小
        /// </summary>
        public bool IsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// 备注列
        /// </summary>
        public bool IsNote
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsNote, value);
            }
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Name);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Name, value);
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Sort, value);
            }
        }
        /// <summary>
        /// 要求的格式
        /// </summary>
        public string Exts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Exts);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Exts, value);
            }
        }
        public string SaveTo
        {
            get
            {
                string s = this.GetValStringByKey(FrmAttachmentAttr.SaveTo);
                if (s == "" || s == null)
                    s = SystemConfig.PathOfDataUser + @"\UploadFile\" + this.FK_MapData + "\\";
                return s;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.SaveTo, value);
            }
        }
        /// <summary>
        /// 附件编号
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.NoOfObj);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.NoOfObj, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.X);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.W);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.W, value);
            }
        }
        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.H);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.H, value);
            }
        }
        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.RowIdx, value);
            }
        }
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.GroupID);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.GroupID, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmAttachmentAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_MapData, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachment()
        {
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="mypk"></param>
        public FrmAttachment(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
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
                Map map = new Map("Sys_FrmAttachment");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "附件";
                map.EnType = EnType.Sys;
                map.AddMyPK();

                map.AddTBString(FrmAttachmentAttr.FK_MapData, null,"FK_MapData", true, false, 1, 30, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "附件编号", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentAttr.Name, null,"名称", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentAttr.Exts, null, "要求上传的格式", true, false, 0, 50, 20);
                map.AddTBString(FrmAttachmentAttr.SaveTo, null, "保存到", true, false, 0, 150, 20);
                map.AddTBString(FrmAttachmentAttr.Sort, null, "类别(可为空)", true, false, 0, 500, 20);

                map.AddTBFloat(FrmAttachmentAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmAttachmentAttr.Y, 5, "Y", false, false);
                map.AddTBFloat(FrmAttachmentAttr.W, 40, "TBWidth", false, false);
                map.AddTBFloat(FrmAttachmentAttr.H, 150, "H", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "是否可以上传", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsDelete, true, "是否可以删除", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "是否可以下载", false, false);

                map.AddBoolean(FrmAttachmentAttr.IsAutoSize, true, "自动控制大小", false, false);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "是否增加备注", false, false);

                map.AddTBInt(FrmAttachmentAttr.UploadType, 0, "上传类型0单个1多个2指定", false, false);

                map.AddTBInt(FrmAttachmentAttr.RowIdx, 0, "RowIdx", false, false);
                map.AddTBInt(FrmAttachmentAttr.GroupID, 0, "GroupID", false, false);
                map.AddTBString(FrmAttachmentAttr.GUID, null, "GUID", true, false, 0, 128, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public bool IsUse = false;
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.NoOfObj;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 附件s
    /// </summary>
    public class FrmAttachments : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件s
        /// </summary>
        public FrmAttachments()
        {
        }
        /// <summary>
        /// 附件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachments(string fk_mapdata)
        {
            this.Retrieve(FrmAttachmentAttr.FK_MapData, fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachment();
            }
        }
        #endregion
    }
}
