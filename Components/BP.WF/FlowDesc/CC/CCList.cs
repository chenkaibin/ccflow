using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.WF
{
	/// <summary>
	/// ���� ����
	/// </summary>
    public class CCListAttr : EntityMyPKAttr
    {
        #region ��������
        /// <summary>
        /// ����
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// ��������
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// �ڵ�
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        public const string FlowName = "FlowName";
        public const string NodeName = "NodeName";
        /// <summary>
        /// �Ƿ��ȡ
        /// </summary>
        public const string Sta = "Sta";
        public const string WorkID = "WorkID";
        public const string FID = "FID";


        /// <summary>
        /// ���͸�
        /// </summary>
        public const string CCTo = "CCTo";
        /// <summary>
        /// ��¼��
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// RDT
        /// </summary>
        public const string RDT = "RDT";

        /// <summary>
        /// ������ID
        /// </summary>
        public const string PWorkID = "PWorkID";
        /// <summary>
        /// �����̱��
        /// </summary>
        public const string PFlowNo = "PFlowNo";
        #endregion
    }
    public enum CCSta
    {
        /// <summary>
        /// δ��
        /// </summary>
        UnRead,
        /// <summary>
        /// �Ѷ�ȡ
        /// </summary>
        Read,
        /// <summary>
        /// ��ɾ��
        /// </summary>
        Del
    }
	/// <summary>
	/// ����
	/// </summary>
    public class CCList : EntityMyPK
    {
        #region ����
        /// <summary>
        /// ״̬
        /// </summary>
        public CCSta HisSta
        {
            get
            {
                return (CCSta)this.GetValIntByKey(CCListAttr.Sta);
            }
            set
            {
                this.SetValByKey(CCListAttr.Sta, (int)value);
            }
        }
        /// <summary>
        /// UI�����ϵķ��ʿ���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                if (Web.WebUser.No != "admin")
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        public string CCTo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.CCTo);
            }
            set
            {
                this.SetValByKey(CCListAttr.CCTo, value);
            }
        }
        /// <summary>
        /// �ڵ���
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(CCListAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(CCListAttr.FK_Node, value);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.WorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.WorkID, value);
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.FID);
            }
            set
            {
                this.SetValByKey(CCListAttr.FID, value);
            }
        }
        /// <summary>
        /// �����̹���ID
        /// </summary>
        public Int64 PWorkID
        {
            get
            {
                return this.GetValInt64ByKey(CCListAttr.PWorkID);
            }
            set
            {
                this.SetValByKey(CCListAttr.PWorkID, value);
            }
        }
        /// <summary>
        /// �����̱��
        /// </summary>
        public string PFlowNo
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.PFlowNo);
            }
            set
            {
                this.SetValByKey(CCListAttr.PFlowNo, value);
            }
        }
        /// <summary>
        /// ���̱��
        /// </summary>
        public string FK_FlowT
        {
            get
            {
                return this.GetValRefTextByKey(CCListAttr.FK_Flow);
            }
        }
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FlowName);
            }
            set
            {
                this.SetValByKey(CCListAttr.FlowName, value);
            }
        }
        public string NodeName
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.NodeName);
            }
            set
            {
                this.SetValByKey(CCListAttr.NodeName, value);
            }
        }
        /// <summary>
        /// ���ͱ���
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Title);
            }
            set
            {
                this.SetValByKey(CCListAttr.Title, value);
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string Doc
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Doc);
            }
            set
            {
                this.SetValByKey(CCListAttr.Doc, value);
            }
        }
        public string DocHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(CCListAttr.Doc);
            }
        }
        /// <summary>
        /// ���Ͷ���
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CCListAttr.FK_Flow, value);
            }
        }
        public string Rec
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.Rec);
            }
            set
            {
                this.SetValByKey(CCListAttr.Rec, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(CCListAttr.RDT);
            }
            set
            {
                this.SetValByKey(CCListAttr.RDT, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// CCList
        /// </summary>
        public CCList()
        {
        }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_CCList");
                map.EnDesc = "�����б�";
                map.EnType = EnType.Admin;
                map.AddMyPK();
                map.AddTBString(CCListAttr.FK_Flow, null, "���̱��", true, true, 0, 500, 10, true);
                map.AddTBString(CCListAttr.FlowName, null, "��������", true, true, 0, 500, 10, true);

                map.AddTBInt(CCListAttr.FK_Node, 0, "�ڵ�", true, true);
                map.AddTBString(CCListAttr.NodeName, null, "�ڵ�����", true, true, 0, 500, 10, true);
                map.AddTBInt(CCListAttr.WorkID, 0, "����ID", true, true);
                map.AddTBInt(CCListAttr.FID, 0, "FID", true, true);

                map.AddTBString(CCListAttr.Title, null, "����", true, true, 0, 500, 10, true);
                map.AddTBStringDoc();

                map.AddTBString(CCListAttr.Rec, null, "��¼��", true, true, 0, 50, 10, true);
                map.AddTBString(CCListAttr.RDT, null, "��¼����", true, true, 0, 500, 10, true);
                map.AddTBInt(CCListAttr.Sta, 0, "״̬", true, true);
                map.AddTBString(CCListAttr.CCTo, null, "���͸�", true, false, 0, 50, 10, true);

                map.AddTBString(CCListAttr.PFlowNo, null, "�����̱��", true, true, 0, 100, 10, true);
                map.AddTBInt(CCListAttr.PWorkID, 0, "������WorkID", true, true);
                 
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// ����
	/// </summary>
	public class CCLists: EntitiesMyPK
	{
		#region ����
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CCList();
			}
		}
		/// <summary>
        /// ����
		/// </summary>
		public CCLists(){} 		 
		#endregion
	}
}
