using System;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;

namespace BP.WF.Ext
{
    /// <summary>
    /// 流程
    /// </summary>
    public class FlowDoc : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public FlowDoc()
        {
        }
        /// <summary>
        /// 流程
        /// </summary>
        /// <param name="_No">编号</param>
        public FlowDoc(string _No)
        {
            this.No = _No;
            if (SystemConfig.IsDebug)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("流程编号不存在");
            }
            else
            {
                this.Retrieve();
            }
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

                Map map = new Map("WF_Flow");

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "流程";
                map.CodeStruct = "3";

                map.AddTBStringPK(BP.WF.FlowAttr.No, null, "编号", true, true, 1, 10, 3);
                map.AddTBString(BP.WF.FlowAttr.Name, null, "名称", true, false, 0, 50, 10);

                //map.AddDDLEntities(BP.WF.FlowAttr.FK_FlowSort, "01", "流程类别",
                //new FlowSorts(), true);
                //map.AddTBInt(BP.WF.FlowAttr.FlowSort, (int)BP.WF.FlowType.Panel, "流程类型", false, false);
                // @0=业务流程@1=公文流程.
                // map.AddTBInt(BP.WF.FlowAttr.FlowSheetType, (int)FlowSheetType.SheetFlow, "表单类型", false, false);


               

                map.AddBoolean(BP.WF.FlowAttr.IsOK, true, "是否起用", true, true);


               // map.AddBoolean(BP.WF.FlowAttr.CCType, false, "是否抄送参与人", true, true);

      //          map.AddDDLSysEnum(BP.WF.FlowAttr.CCType, (int)CCType.None, "抄送类型", true, true, BP.WF.FlowAttr.CCType, 
      //              "@0=不抄送@1=按人员@2=按岗位@3=按节点@4=按部门@5=按照部门与岗位");
      //          map.AddDDLSysEnum(BP.WF.FlowAttr.CCWay, (int)CCWay.ByMsg, "抄送方式", true, true, BP.WF.FlowAttr.CCWay,
      //"@0=调用本系统及时信息@1=通过Email(在web.config中配置)@2=调用手机接口@3=调用数据库函数");

                map.AddDDLSysEnum(FlowAttr.FlowRunWay, (int)FlowRunWay.HandWork, "运行方式", true, true, FlowAttr.FlowRunWay,
                    "@0=手工启动@1=指定人员按时启动@2=数据集按时启动@3=触发式启动");
                map.AddTBString(FlowAttr.RunObj, null, "运行内容", true, false, 0, 100, 10);
               
                map.AddTBString(BP.WF.FlowAttr.Note, null, "备注", true, false, 0, 100, 10,true);
                
                map.AddTBString(FlowAttr.StartListUrl, null, "导航Url", true, false, 0, 500, 10, true);

                // map.AddBoolean(BP.WF.FlowAttr.CCType, false, "流程完成后抄送参与人员", true, true);
                // map.AddTBString(BP.WF.FlowAttr.CCStas, null, "要抄送的岗位", false, false, 0, 2000, 10);
                // map.AddTBDecimal(BP.WF.FlowAttr.AvgDay, 0, "平均运行用天", false, false);

                RefMethod rm = new RefMethod();
                rm.Title = "设计检查报告"; // "设计检查报告";
                rm.ToolTip = "检查流程设计的问题。";
                rm.Icon = "/WF/Img/Btn/Confirm.gif";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "视图定义"; //"视图定义";
                rm.Icon = "/WF/Img/Btn/View.gif";
                rm.ClassMethodName = this.ToString() + ".DoDRpt";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "报表运行"; // "报表运行";
                rm.ClassMethodName = this.ToString() + ".DoOpenRpt()";
                //rm.Icon = "/WF/Img/Btn/Table.gif";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据转出定义";  //"数据转出定义";
                //  rm.Icon = "/WF/Img/Btn/Table.gif";
                rm.ToolTip = "在流程完成时间，流程数据转储存到其它系统中应用。";

                rm.ClassMethodName = this.ToString() + ".DoExp";
                map.AddRefMethod(rm);

                //map.AttrsOfOneVSM.Add(new FlowStations(), new Stations(), FlowStationAttr.FK_Flow,
                //    FlowStationAttr.FK_Station, DeptAttr.Name, DeptAttr.No, "抄送岗位");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region  公共方法
        /// <summary>
        /// 执行检查
        /// </summary>
        /// <returns></returns>
        public string DoCheck()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoCheck();
        }
        /// <summary>
        /// 设计数据转出
        /// </summary>
        /// <returns></returns>
        public string DoExp()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoExp();
        }
        /// <summary>
        /// 定义报表
        /// </summary>
        /// <returns></returns>
        public string DoDRpt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoDRpt();
        }
        /// <summary>
        /// 运行报表
        /// </summary>
        /// <returns></returns>
        public string DoOpenRpt()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            return fl.DoOpenRpt();
        }
        /// <summary>
        /// 更新之后的事情
        /// </summary>
        protected override void afterUpdate()
        {
            Flow fl = new Flow();
            fl.No = this.No;
            fl.RetrieveFromDBSources();
            fl.Update();
            base.afterUpdate();
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class FlowDocs : EntitiesNoName
    {
        #region 查询
        /// <summary>
        /// 查询出来全部的在生存期间内的流程
        /// </summary>
        /// <param name="FlowSort">流程类别</param>
        /// <param name="IsCountInLifeCycle">是不是计算在生存期间内 true 查询出来全部的 </param>
        public void Retrieve(string FlowSort)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(BP.WF.FlowAttr.FK_FlowSort, FlowSort);
            qo.addOrderBy(BP.WF.FlowAttr.No);
            qo.DoQuery();
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public FlowDocs() { }
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="fk_sort"></param>
        public FlowDocs(string fk_sort)
        {
            this.Retrieve(BP.WF.FlowAttr.FK_FlowSort, fk_sort);
        }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowDoc();
            }
        }
        #endregion
    }
}

