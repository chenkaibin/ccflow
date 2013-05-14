
using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.En;

namespace BP.WF
{
	/// <summary>
	/// 普通工作
	/// </summary>
	public class GEWorkAttr :WorkAttr
	{
	}
    /// <summary>
    /// 普通工作
    /// </summary>
    public class GEWork : Work
    {
        #region 与_SQLCash 操作有关
        private SQLCash _SQLCash = null;
        public override SQLCash SQLCash
        {
            get
            {
                if (_SQLCash == null)
                {
                    _SQLCash = BP.DA.Cash.GetSQL(this.NodeID.ToString());
                    if (_SQLCash == null)
                    {
                        _SQLCash = new SQLCash(this);
                        BP.DA.Cash.SetSQL(this.NodeID.ToString(), _SQLCash);
                    }
                }
                return _SQLCash;
            }
            set
            {
                _SQLCash = value;
            }
        }
        #endregion

        #region 构造函数        
        /// <summary>
        /// 普通工作
        /// </summary>
        public GEWork()
        {
        }
        /// <summary>
        /// 普通工作
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public GEWork(int nodeid)
        {
            this.NodeID = nodeid;
            this.SQLCash = null;
        }
        /// <summary>
        /// 普通工作
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="_oid">OID</param>
        public GEWork(int nodeid, Int64 _oid)
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
                //   BP.Sys.MapData md = new BP.Sys.MapData();
                this._enMap = BP.Sys.MapData.GenerHisMap("ND" + this.NodeID.ToString());
                return this._enMap;
            }
        }
        /// <summary>
        /// GEWorks
        /// </summary>
        public override Entities GetNewEntities
        {
            get
            {
                if (this.NodeID == 0)
                    return new GEWorks();

                return new GEWorks(this.NodeID);
            }
        }
        #endregion
    }
    /// <summary>
    /// 普通工作s
    /// </summary>
    public class GEWorks : Works
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
                    return new GEWork();
                return new GEWork(this.NodeID);
            }
        }
        /// <summary>
        /// 普通工作ID
        /// </summary>
        public GEWorks()
        {
        }
        /// <summary>
        /// 普通工作ID
        /// </summary>
        /// <param name="nodeid"></param>
        public GEWorks(int nodeid)
        {
            this.NodeID = nodeid;
        }
        #endregion
    }
}
