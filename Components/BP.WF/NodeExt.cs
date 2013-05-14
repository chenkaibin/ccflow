
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port; 
using BP.Port; 
using BP.En;

 
namespace BP.WF
{
	/// <summary>
	/// 节点
	/// </summary>
    public class NodeExtAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 节点
        /// </summary>
        public const string PerCentFK = "PerCentFK";
        public const string CentOfTo = "CentOfTo";
        /// <summary>
        /// 省局
        /// </summary>
        public const string FK_XZCL = "FK_XZCL";
    }
	/// <summary>
	/// 节点
	/// </summary>
    public class NodeExt : EntityNoName
    {
        #region 基本属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeExt() { }
        /// <summary>
        /// strubg
        /// </summary>
        public NodeExt(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        public NodeExt(int no)
        {
            this.No = no.ToString();
            this.Retrieve();
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

                Map map = new Map("WF_NodeExt");
                map.EnDesc = "节点";
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(SimpleNoNameFixAttr.No, null, "编号", true, false, 4, 4, 100);
                map.AddTBString(SimpleNoNameFixAttr.Name, null, "名称", true, false, 2, 60, 500);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 节点s
	/// </summary>
	public class NodeExts : EntitiesNoName
	{	
		#region 构造方法
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{			 
				return new NodeExt();
			}
		}
		/// <summary>
		/// 节点s 
		/// </summary>
		public NodeExts(){}
		#endregion
	}
	
}
