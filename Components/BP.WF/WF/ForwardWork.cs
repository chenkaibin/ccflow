using System;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Port; 
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 转发记录
	/// </summary>
    public class ForwardWorkAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 转发原因
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// 转发人
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 转发人名成
        /// </summary>
        public const string FK_EmpName = "FK_EmpName";
        /// <summary>
        /// 转发时间
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 是否读取？
        /// </summary>
        public const string IsRead = "IsRead";
        /// <summary>
        /// 转发给
        /// </summary>
        public const string ToEmp = "ToEmp";
        /// <summary>
        /// 转发给人员名称
        /// </summary>
        public const string ToEmpName = "ToEmpName";
        #endregion
    }
	/// <summary>
	/// 转发记录
	/// </summary>
	public class ForwardWork : EntityMyPK
	{		
		#region 基本属性
		/// <summary>
		/// 工作ID
		/// </summary>
        public Int64 WorkID
		{
			get
			{
				return this.GetValInt64ByKey(ForwardWorkAttr.WorkID);
			}
			set
			{
				SetValByKey(ForwardWorkAttr.WorkID,value);
			}
		}
		/// <summary>
		/// 工作节点
		/// </summary>
		public int FK_Node
		{
			get
			{
				return this.GetValIntByKey(ForwardWorkAttr.FK_Node);
			}
			set
			{
				SetValByKey(ForwardWorkAttr.FK_Node,value);
			}
		}
        /// <summary>
        /// 是否读取？
        /// </summary>
        public bool IsRead
        {
            get
            {
                return this.GetValBooleanByKey(ForwardWorkAttr.IsRead);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.IsRead, value);
            }
        }
        /// <summary>
        /// ToEmpName
        /// </summary>
        public string ToEmpName
        {
            get
            {
                return this.GetValStringByKey(ForwardWorkAttr.ToEmpName);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.ToEmpName, value);
            }
        }
        /// <summary>
        /// 转发人名称.
        /// </summary>
        public string FK_EmpName
        {
            get
            {
                return this.GetValStringByKey(ForwardWorkAttr.FK_EmpName);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.FK_EmpName, value);
            }
        }
        /// <summary>
        /// 转发时间
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(ForwardWorkAttr.RDT);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.RDT, value);
            }
        }
        /// <summary>
        /// 转发意见
        /// </summary>
		public string Note
		{
			get
			{
				return this.GetValStringByKey(ForwardWorkAttr.Note);
			}
			set
			{
				SetValByKey(ForwardWorkAttr.Note,value);
			}
		}
        /// <summary>
        /// 转发意见html格式
        /// </summary>
        public string NoteHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(ForwardWorkAttr.Note);
            }
        }
        /// <summary>
        /// 转发人
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(ForwardWorkAttr.FK_Emp);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 转发给
        /// </summary>
        public string ToEmp
        {
            get
            {
                return this.GetValStringByKey(ForwardWorkAttr.ToEmp);
            }
            set
            {
                SetValByKey(ForwardWorkAttr.ToEmp, value);
            }
        }
		#endregion

		#region 构造函数
		/// <summary>
		/// 转发记录
		/// </summary>
		public ForwardWork()
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

                Map map = new Map("WF_ForwardWork");
                map.EnDesc = "转发记录";
                map.EnType = EnType.App;
                map.AddMyPK();

                map.AddTBInt(ForwardWorkAttr.WorkID, 0, "工作ID", true, true);
                map.AddTBInt(ForwardWorkAttr.FK_Node, 0, "FK_Node", true, true);
                map.AddTBString(ForwardWorkAttr.FK_Emp, null, "转发人", true, true, 0, 40, 10);
                map.AddTBString(ForwardWorkAttr.FK_EmpName, null, "转发人名称", true, true, 0, 40, 10);

                map.AddTBString(ForwardWorkAttr.ToEmp, null, "转发给", true, true, 0, 40, 10);
                map.AddTBString(ForwardWorkAttr.ToEmpName, null, "转发给名称", true, true, 0, 40, 10);

                map.AddTBDateTime(ForwardWorkAttr.RDT, null, "转发时间", true, true);
                map.AddTBString(ForwardWorkAttr.Note, null, "转发原因", true, true, 0, 2000, 10);

                map.AddTBInt(ForwardWorkAttr.IsRead, 0, "是否读取？", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 插入之前
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerOIDByGUID().ToString();
            this.RDT = DataType.CurrentDataTime;
            return base.beforeInsert();
        }
		#endregion
	}
	/// <summary>
	/// 转发记录s 
	/// </summary>
	public class ForwardWorks : Entities
	{	 
		#region 构造
		/// <summary>
		/// 转发记录s
		/// </summary>
		public ForwardWorks()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new ForwardWork();
			}
		}
		#endregion
	}
	
}
