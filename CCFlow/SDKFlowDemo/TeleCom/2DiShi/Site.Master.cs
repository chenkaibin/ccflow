﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.TeleCom._2DiShi
{
    public partial class Site : System.Web.UI.MasterPage
    {
        #region 流程引擎传来的变量.
        /// <summary>
        /// 工作ID，在建立草稿时已经产生了.
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        /// <summary>
        ///  流程编号.
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion 流程引擎传来的变量

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Node,this.WorkID, BP.Web.WebUser.No);
                return;
            }
            catch (Exception ex)
            {
                this.Response.Write("<font color=red>" + ex.Message + "</font>");
            }
        }
    }
}