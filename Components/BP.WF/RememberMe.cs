
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 记忆我 属性
	/// </summary>
    public class RememberMeAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 当前节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 可执行人员
        /// </summary>
        public const string Objs = "Objs";
        /// <summary>
        /// 可执行人员
        /// </summary>
        public const string ObjsExt = "ObjsExt";
        /// <summary>
        /// 可执行人员数据量
        /// </summary>
        public const string NumOfObjs = "NumOfObjs";
        /// <summary>
        /// 工作人员（候选)
        /// </summary>
        public const string Emps = "Emps";
        /// <summary>
        /// 工作人员个数（候选)
        /// </summary>
        public const string NumOfEmps = "NumOfEmps";
        /// <summary>
        /// 工作人员（候选)
        /// </summary>
        public const string EmpsExt = "EmpsExt";
        #endregion
    }
	/// <summary>
	/// 记忆我
	/// </summary>
    public class RememberMe : EntityMyPK
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.FK_Emp, value);
                this.MyPK = this.FK_Node + "_" + Web.WebUser.No;
            }
        }
        public string Objs
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.Objs);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.Objs, value);
            }
        }
        public string ObjsExt
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.ObjsExt);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.ObjsExt, value);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(RememberMeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.FK_Node, value);
                this.MyPK = this.FK_Node + "_" + Web.WebUser.No;
            }
        }
        public int NumOfEmps
        {
            get
            {
                return this.Emps.Split('@').Length - 2;
            }
        }
        public int NumOfObjs
        {
            get
            {
                return this.Objs.Split('@').Length - 2;
            }
        }
        public string Emps
        {
            get
            {
                return this.GetValStringByKey(RememberMeAttr.Emps);
            }
            set
            {
                this.SetValByKey(RememberMeAttr.Emps, value);
            }
        }
        public string EmpsExt
        {
            get
            {

                string str = this.GetValStringByKey(RememberMeAttr.EmpsExt).Trim();
                if (str.Length == 0)
                    return str;

                if (str.Substring(str.Length - 1) == "、")
                    return str.Substring(0, str.Length - 1);
                else
                    return str;
            }
            set
            {

                this.SetValByKey(RememberMeAttr.EmpsExt, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// RememberMe
        /// </summary>
        public RememberMe()
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
                Map map = new Map("WF_RememberMe");
                map.EnDesc = "记忆我";
                map.EnType = EnType.Admin;

                map.AddMyPK();

                map.AddTBInt(RememberMeAttr.FK_Node, 0, "节点", false, false);
                map.AddTBString(RememberMeAttr.FK_Emp, "", "人员", true, false, 1, 30, 10);

                map.AddTBString(RememberMeAttr.Objs, "", "分配人员", true, false, 0, 4000, 10);
                map.AddTBString(RememberMeAttr.ObjsExt, "", "分配人员", true, false, 0, 4000, 10);

                //map.AddTBInt(RememberMeAttr.NumOfObjs, 0, "分配人员数", true, false);

                map.AddTBString(RememberMeAttr.Emps, "", "工作人员", true, false, 0, 4000, 10);
                map.AddTBString(RememberMeAttr.EmpsExt, "", "工作人员Ext", true, false, 0, 4000, 10);
                // map.AddTBInt(RememberMeAttr.NumOfEmps, 0, "执行人员数", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.FK_Emp = Web.WebUser.No;
            this.MyPK = this.FK_Node + "_" + this.FK_Emp;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
	/// 记忆我
	/// </summary>
	public class RememberMes: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new RememberMe();
			}
		}
		/// <summary>
		/// RememberMe
		/// </summary>
		public RememberMes(){} 		 
		#endregion
	}
	
}
