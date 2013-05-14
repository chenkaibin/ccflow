
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.En;

namespace BP.WF
{
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum CheckState
    {
        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 2,
        /// <summary>
        /// 同意
        /// </summary>
        Agree = 1,
        /// <summary>
        /// 不同意
        /// </summary>
        Dissent = 0
    }
	/// <summary>
	/// 开始工作节点
	/// </summary>
	public class GEStartWorkAttr :WorkAttr
	{
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 流程状态
        /// </summary>
        public const string WFState = "WFState";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 部门text
        /// </summary>
        public const string FK_DeptText = "FK_DeptText";
	}
    /// <summary>
    /// 开始工作节点
    /// </summary>
    public class GEStartWork : StartWork
    {
        #region 构造函数
        /// <summary>
        /// 开始工作节点
        /// </summary>
        public GEStartWork()
        {
        }
        /// <summary>
        /// 开始工作节点
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEStartWork(int nodeid)
        {
            this.NodeID = nodeid;
            this.SQLCash=null;

        }
        /// <summary>
        /// 开始工作节点
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEStartWork(int nodeid, Int64 _oid)
        {
            this.NodeID = nodeid;
            this.OID = _oid;
            this.SQLCash = null;
        }
        #endregion

        #region Map
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                //if (this._enMap != null)
                //    return this._enMap;
                //BP.Sys.MapData md = new BP.Sys.MapData();
                this._enMap = BP.Sys.MapData.GenerHisMap("ND" + this.NodeID.ToString());
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get
            {
                if (this.NodeID == 0)
                    return new GEStartWorks();
                return new GEStartWorks(this.NodeID);
            }
        }
        #endregion
    }
    /// <summary>
    /// 开始工作节点s
    /// </summary>
    public class GEStartWorks : Works
    {
        #region 重载基类方法
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID = 0;
        #endregion

        #region 方法
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                if (this.NodeID == 0)
                    return new GEStartWork();
                return new GEStartWork(this.NodeID);
            }
        }
        /// <summary>
        /// 开始工作节点ID
        /// </summary>
        public GEStartWorks()
        {

        }
        /// <summary>
        /// 开始工作节点ID
        /// </summary>
        /// <param name="nodeid"></param>
        public GEStartWorks(int nodeid)
        {
            this.NodeID = nodeid;
        }
        #endregion
    }
}
