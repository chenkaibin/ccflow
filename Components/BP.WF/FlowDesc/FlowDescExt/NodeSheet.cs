using System;
using System.Data;
using BP.DA;
using BP.En;
using System.Collections;
using BP.Port;

namespace BP.WF.Ext
{
    /// <summary>
    /// ������ÿ���ڵ����Ϣ.
    /// </summary>
    public class NodeSheet : Entity
    {
        /// <summary>
        /// ��ʱ������ʽ
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// ���ʹ���
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        public FJOpen HisFJOpen
        {
            get
            {
                return (FJOpen)this.GetValIntByKey(NodeAttr.FJOpen);
            }
            set
            {
                this.SetValByKey(NodeAttr.FJOpen, (int)value);
            }
        }
        /// <summary>
        /// ���ʹ���
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string OutTimeDeal
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, value);
            }
        }
        /// <summary>
        /// ��ʱ��������
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// ������sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// �Ƿ�����˻�
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        public bool AthEnable
        {
            get
            {
                if (HisFJOpen == FJOpen.None)
                    return false;
                return true;
            }
        }
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        protected override bool beforeUpdate()
        {
            Node nd = new Node(this.NodeID);
            nd.Update();
            return base.beforeUpdate();
        }

        #region ���Ի�ȫ�ֵ� Node
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                Flow fl = new Flow(this.FK_Flow);
                if (Web.WebUser.No == "admin" || fl.DesignerNo == BP.Web.WebUser.No)
                    uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// �ڵ�
        /// </summary>
        public NodeSheet() { }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node");
                map.EnDesc = "�ڵ�";
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                // ��������
                map.AddTBIntPK(NodeAttr.NodeID, 0, "�ڵ�ID", true, true);
                map.AddTBInt(NodeAttr.Step, 0, "����(�޼�������)", true, false);
                map.AddTBString(NodeAttr.FK_Flow, null, "���̱��", false, false, 3, 3, 10, false);

                map.AddTBString(NodeAttr.Name, null, "����", true, true, 0, 100, 10, true);
                map.AddBoolean(NodeAttr.IsTask, true, "�������乤����?", true, true, false);
                map.AddBoolean(NodeAttr.IsRM, true, "�Ƿ�����Ͷ��·���Զ����书��?", true, true, false);
                map.AddBoolean(NodeAttr.IsForceKill, false, "�Ƿ����ǿ��ɾ��������(�Ժ�������Ч)", true, true, true);
                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "�Ƿ��������߳̽�����Ա�ظ�(�����̵߳���Ч)?", true, true, true);
                map.AddBoolean(NodeAttr.IsBackTracking, false, "�Ƿ�������˻غ�ԭ·����(ֻ�������˻ع��ܲ���Ч)", true, true, true);

                // map.AddTBInt(NodeAttr.PassRate, 100, "ͨ����(���ں����ڵ���Ч)", true, true);
                map.AddTBDecimal(NodeAttr.PassRate, 0, "���ͨ����(�Ժ�������Ч)", true, false);

                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "����ģʽ",
                    true, true, NodeAttr.RunModel, "@0=��ͨ@1=����@2=����@3=�ֺ���@4=���߳�");
            
                //map.AddDDLSysEnum(NodeAttr.FLRole, 0, this.ToE("FLRole", "��������"), true, true, NodeAttr.FLRole,
                //    "@0=��������@1=������@2=����λ");

                map.AddTBString(NodeAttr.FocusField, null, "�����ֶ�", true, false, 0, 50, 10);

                map.AddDDLSysEnum(NodeAttr.DeliveryWay, 0, "���ʹ���", true, true);
                map.AddTBString(NodeAttr.DeliveryParas, null, "���ʹ���������", true, false, 0, 500, 10, true);

                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "˭ִ����",
              true, true, NodeAttr.WhoExeIt, "@0=����Աִ��@1=����ִ��@2=���ִ��");

                map.AddDDLSysEnum(NodeAttr.FormType, 0, "�ڵ��������", true, true);
                map.AddTBString(NodeAttr.FormUrl, null, "����URL", true, false, 0, 200, 10, true);
                map.AddTBString(NodeAttr.FocusField, null, "�����ֶ�", false, false, 0, 50, 10, false);

                map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "���ͺ�ת��",
                 true, true, NodeAttr.TurnToDeal, "@0=��ʾccflowĬ����Ϣ@1=��ʾָ����Ϣ@2=ת��ָ����url@3=��������ת��");

                map.AddTBString(NodeAttr.TurnToDealDoc, null, "ת��������", true, false, 0, 999, 10, true);
                map.AddTBString(NodeAttr.JumpSQL, null, "����ת�Ľڵ�", true, false, 0, 200, 10, false);
                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "�Ѷ���ִ",
              true, true, NodeAttr.ReadReceipts, "@0=����ִ@1=�Զ���ִ@2=����һ�ڵ�����ֶξ���@3=��SDK�����߲�������");

                //map.AddBoolean("IsSkipReturn", false, "�Ƿ���Կ缶����", true, true, true);

                map.AddTBDateTime("DTFrom", "�������ڴ�", true, true);
                map.AddTBDateTime("DTTo", "�������ڵ�", true, true);

                //���淽ʽ.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "���߳�����", true, true,
                    NodeAttr.SubThreadType, "@0=ͬ����@1=�����");
                map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "���淽ʽ", true, true);

                // �������̲߳��� 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "���߳�������ʽ", true, true, NodeAttr.SubFlowStartWay,"@0=������@2=ָ�����ֶ�����@3=����ϸ������");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "��������", true, false, 0, 100, 10, true);


                #region  ���ܰ�ť״̬
                map.AddTBString(BtnAttr.SendLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
             //   map.AddBoolean(BtnAttr.SendEnable, true, "�Ƿ�����", true, false);
                map.AddTBString(BtnAttr.SendJS, "", "��ťJS����", true, false, 0, 50, 10);

                map.AddTBString(BtnAttr.SaveLab, "����", "���水ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.ThreadLab, "���߳�", "���̰߳�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.JumpWayLab, "��ת", "��ת��ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "��ת����",
           true, true, NodeAttr.JumpWay);


                map.AddTBString(BtnAttr.ReturnLab, "�˻�", "�˻ذ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0,"�˻ع���",
           true, true, NodeAttr.ReturnRole);

                map.AddTBString(BtnAttr.CCLab, "����", "���Ͱ�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "���͹���",
           true, true, NodeAttr.CCRole);

                map.AddTBString(BtnAttr.ShiftLab, "�ƽ�", "�ƽ���ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, true, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.DelLab, "ɾ��", "ɾ����ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "ɾ������",
           true, true, BtnAttr.DelEnable);

               // map.AddBoolean(BtnAttr.DelEnable, true, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.EndFlowLab, "��������", "�������̰�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "�Ƿ�����", true, true);
               

                map.AddTBString(BtnAttr.PrintDocLab, "��ӡ����", "��ӡ���ݰ�ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintDocEnable, false, "�Ƿ�����", true, true);

                //map.AddTBString(BtnAttr.AthLab, "����", "������ť��ǩ", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "����Ȩ��"), true, true, 
                //    NodeAttr.FJOpen, "@0=�رո���@1=����Ա@2=����ID@3=����ID");

                map.AddTBString(BtnAttr.TrackLab, "�켣", "�켣��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.HungLab, "����", "����ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "�Ƿ�����", true, true);

                map.AddTBString(BtnAttr.SelectAccepterLab, "������", "�����˰�ť��ǩ", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "������ʽ",
          true, true, BtnAttr.SelectAccepterEnable);


                map.AddTBString(BtnAttr.SearchLab, "��ѯ", "��ѯ��ť��ǩ", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "�Ƿ�����", true, true);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "�Ƿ�����", true, true);
                #endregion  ���ܰ�ť״̬

                // ��������
                map.AddTBFloat(NodeAttr.WarningDays, 0,  "��������(0������)", true, false); // "��������(0������)"
                map.AddTBFloat(NodeAttr.DeductDays, 1,  "����(��)", true, false); //"����(��)"
                map.AddTBFloat(NodeAttr.DeductCent, 2,  "�۷�(ÿ����1���)", true, false); //"�۷�(ÿ����1���)"

                map.AddTBFloat(NodeAttr.MaxDeductCent, 0,  "��߿۷�", true, false);   //"��߿۷�"
                map.AddTBFloat(NodeAttr.SwinkCent, float.Parse("0.1"),  "�����÷�", true, false); //"�����÷�"
                map.AddDDLSysEnum(NodeAttr.OutTimeDeal, 0,  "��ʱ����",
                true, true, NodeAttr.OutTimeDeal,
                "@0=������@1=�Զ������˶�(���˶���ָ���ڵ�)@2=�Զ���תָ���ĵ�@3=�Զ�ת��ָ������Ա@4=��ָ������Ա����Ϣ@5=ɾ������@6=ִ��SQL");
                 
                map.AddTBString(NodeAttr.DoOutTime, null, "��������", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "ִ�г�ʱ����", true, false, 0, 100, 10, true);

        //        map.AddTBString(NodeAttr.FK_Flows, null, "flow", false, false, 0, 100, 10);

                map.AddDDLSysEnum(NodeAttr.CHWay, 0, "���˷�ʽ", true, true, NodeAttr.CHWay, "@0=������@1=��ʱЧ@2=��������");
                map.AddTBFloat(NodeAttr.Workload, 0, "������(��λ:����)", true, false);

                // �Ƿ��������˵㣿
                map.AddBoolean(NodeAttr.IsEval, false, "�Ƿ��������˵�", true, true, true);


                // ��ع��ܡ�
                map.AttrsOfOneVSM.Add(new BP.WF.NodeStations(), new BP.WF.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "�ڵ��λ");

                map.AttrsOfOneVSM.Add(new BP.WF.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                DeptAttr.No, "�ڵ㲿��");

                map.AttrsOfOneVSM.Add(new BP.WF.NodeEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "������Ա");

                map.AttrsOfOneVSM.Add(new BP.WF.NodeFlows(), new Flows(), NodeFlowAttr.FK_Node, NodeFlowAttr.FK_Flow, DeptAttr.Name, DeptAttr.No,
                    "�ɵ��õ�������");

                //map.AttrsOfOneVSM.Add(new BP.WF.NodeReturns(), new BP.WF.NodeSheets(), NodeEmpAttr.FK_Node, EmpDeptAttr.FK_Emp, DeptAttr.Name,
                //  DeptAttr.No, this.ToE("Accpter", "���˻صĽڵ�"));

                RefMethod rm = new RefMethod();
                rm.Title =  "���˻صĽڵ�"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�������ķ���"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoParas";
                rm.HisAttrs.AddTBInt("JE", 100, "������", true, false);
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "�Զ���������"; // "���͹���";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                //rm.Warning = "";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "��Ʊ���"; // "��Ʊ���";
                rm.ClassMethodName = this.ToString() + ".DoMapData";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "���ݴ�ӡ"; //"����&����";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "/WF/Img/FileType/doc.gif";
                map.AddRefMethod(rm);

                if (BP.SystemConfig.CustomerNo == "HCBD")
                {
                    /* Ϊ���ɰ�����õĸ��Ի�����. */
                    rm = new RefMethod();
                    rm.Title = "DXReport����";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = "/WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                //rm = new RefMethod();
                //rm.Title = this.ToE("DoFAppSet", "�����ⲿ����ӿ�"); // "�����ⲿ����ӿ�";
                //rm.ClassMethodName = this.ToString() + ".DoFAppSet";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =   "�¼�"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "������ʾ"; // this.ToE("DoAction", "�����¼��ӿ�"); // "�����¼��ӿ�";
                //rm.ClassMethodName = this.ToString() + ".DoShowSheets";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = this.ToE("DoCond", "�ڵ��������"); // "�ڵ��������";
                //rm.ClassMethodName = this.ToString() + ".DoCond";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "�����������"; // "�����������";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "��Ϣ����"; // "�����¼��ӿ�";
                rm.ClassMethodName = this.ToString() + ".DoListen";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title =  "���ͳɹ�ת������"; // "ת������";
                rm.ClassMethodName = this.ToString() + ".DoTurn";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "������ѡ����"; // "���Ի������˴���";
                rm.ClassMethodName = this.ToString() + ".DoAccepter";
                map.AddRefMethod(rm);
                 

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoParas(int je)
        {
            return "�Ѿ�ִ�з���"+je;
        }
        public string DoTurn()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoTurn();
        }
        /// <summary>
        /// ���͹���
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.CC&PK=" + this.NodeID , "���͹���", "Bill", 800, 500, 200, 300);
            return null;
        }
        /// <summary>
        /// ���Ի������˴���
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            PubClass.WinOpen("./RefFunc/UIEn.aspx?EnName=BP.WF.Selector&PK=" + this.NodeID, "���Ի������˴���", "acce", 800, 500, 200, 300);
            return null;
        }
        /// <summary>
        /// �˻ؽڵ�
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            PubClass.WinOpen("/WF/Admin/CanReturnNodes.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "���˻صĽڵ�", "Bill", 500, 300, 200, 300);
            return null;
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            PubClass.WinOpen("/WF/Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "DXReport����", "DXReport", 500, 300, 200, 300);
            return null;
        }
        public string DoListen()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoListen();
        }
        public string DoFeatureSet()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoFeatureSet();
        }
        public string DoShowSheets()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoShowSheets();
        }
        public string DoCond()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoCond();
        }
        //public string DoCondFL()
        //{
        //    BP.WF.Node nd = new BP.WF.Node(this.NodeID);
        //    return nd.DoCondFL();
        //}
        public string DoMapData()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoMapData();
        }
        public string DoAction()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoAction();
        }
        public string DoBill()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoBill();
        }
        public string DoFAppSet()
        {
            BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            return nd.DoFAppSet();
        }
        #endregion
    }
    /// <summary>
    /// �ڵ㼯��
    /// </summary>
    public class NodeSheets : EntitiesOID
    {
        #region ���췽��
        /// <summary>
        /// �ڵ㼯��
        /// </summary>
        public NodeSheets()
        {
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeSheet(); }
        }
    }
}