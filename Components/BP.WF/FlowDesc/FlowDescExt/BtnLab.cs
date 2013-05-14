using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF
{
    /// <summary>
    /// Btn
    /// </summary>
    public class BtnLab : Entity
    {
        /// <summary>
        /// but
        /// </summary>
        public static string Btns
        {
            get
            {
                return "Send,Save,Thread,Return,CC,Shift,Del,Rpt,Ath,Track,Opt,EndFLow";
            }
        }
        /// <summary>
        /// PK
        /// </summary>
        public override string PK
        {
            get
            {
                return NodeAttr.NodeID;
            }
        }

        #region 基本属性
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.NodeID);
            }
            set
            {
                this.SetValByKey(BtnAttr.NodeID, value);
            }
        }
        public string SearchLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SearchLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchLab, value);
            }
        }
        public bool SearchEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SearchEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.SearchEnable, value);
            }
        }

        public string ShiftLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ShiftLab);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftLab, value);
            }
        }
        public bool ShiftEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ShiftEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.ShiftEnable, value);
            }
        }
        public string SelectAccepterLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SelectAccepterLab);
            }
        }
        public int SelectAccepterEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.SelectAccepterEnable);
            }
        }
        public string SaveLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SaveLab);
            }
        }
        public bool SaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.SaveEnable);
            }
        }
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public string ThreadLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ThreadLab);
            }
        }
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public bool ThreadEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ThreadEnable);
            }
        }
        /// <summary>
        /// 跳转标签
        /// </summary>
        public string JumpWayLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.JumpWayLab);
            }
        }
        public JumpWay JumpWayEnum
        {
            get
            {
                return (JumpWay)this.GetValIntByKey(NodeAttr.JumpWay);
            }
        }
        public bool JumpWayEnable
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.JumpWay);
            }
        }
        public string ReturnLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.ReturnLab);
            }
        }
        public bool ReturnEnable
        {
            get
            {
                return  this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        public string HungLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.HungLab);
            }
        }
        public bool HungEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.HungEnable);
            }
        }
        public string PrintDocLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.PrintDocLab);
            }
        }
        public bool PrintDocEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.PrintDocEnable);
            }
        }
        public string SendLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendLab);
            }
        }
        public bool SendEnable
        {
            get
            {
                return true;
            }
        }
        public string SendJS
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.SendJS).Replace("~","'");
            }
        }
        public string TrackLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.TrackLab);
            }
        }
        public bool TrackEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.TrackEnable);
            }
        }
        public string CCLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.CCLab);
            }
        }
        public CCRole CCRole
        {
            get
            {
                return (CCRole)this.GetValIntByKey(BtnAttr.CCRole);
            }
        }
        public string DeleteLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.DelLab);
            }
        }
        public int DeleteEnable
        {
            get
            {
                return this.GetValIntByKey(BtnAttr.DelEnable);
            }
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        public string EndFlowLab
        {
            get
            {
                return this.GetValStringByKey(BtnAttr.EndFlowLab);
            }
        }
        /// <summary>
        /// 是否启用结束流程
        /// </summary>
        public bool EndFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.EndFlowEnable);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLab() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeid"></param>
        public BtnLab(int nodeid)
        {
            this.NodeID = nodeid;
            this.RetrieveFromDBSources();
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

                Map map = new Map("WF_Node");
                map.EnDesc = "节点标签";

                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBIntPK(BtnAttr.NodeID, 0, "NodeID", true, false);

                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 50, 10);
                
             //map.AddBoolean(BtnAttr.SendEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(NodeAttr.JumpWay, false, "是否启用", true, true);


                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);


                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);


                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ReturnRole, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.CCRole, 0, "抄送规则",true, true, BtnAttr.CCRole);

            //  map.AddBoolean(BtnAttr, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.DelLab, "删除流程", "删除流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.DelEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.FJOpen, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "方式",
          true, true, BtnAttr.SelectAccepterEnable);

               // map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.OptLab, "选项", "选项按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.OptEnable, true, "是否启用", true, true);

                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, true, "是否启用", true, true);

                //map.AddTBString(BtnAttr.URL, null, "URL", true, false, 0, 50, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// Btn
    /// </summary>
    public class BtnLabs : Entities
    {
        /// <summary>
        /// Btn
        /// </summary>
        public BtnLabs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BtnLab();
            }
        }
    }
}
