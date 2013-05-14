using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.DA;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    ///  ����
    /// </summary>
    public class TrackAttr:EntityMyPKAttr
    {
        /// <summary>
        /// ��¼����
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// �������
        /// </summary>
        public const string CDT = "CDT";
        /// <summary>
        /// FID
        /// </summary>
        public const string FID = "FID";
        /// <summary>
        /// WorkID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// �����
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// ���������
        /// </summary>
        public const string ActionTypeText = "ActionTypeText";
        /// <summary>
        /// ʱ����
        /// </summary>
        public const string WorkTimeSpan = "WorkTimeSpan";
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public const string NodeData = "NodeData";
        /// <summary>
        /// �켣�ֶ�
        /// </summary>
        public const string TrackFields = "TrackFields";
        /// <summary>
        /// ��ע
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// �ӽڵ�
        /// </summary>
        public const string NDFrom = "NDFrom";
        /// <summary>
        /// ���ڵ�
        /// </summary>
        public const string NDTo = "NDTo";
        /// <summary>
        /// ����Ա
        /// </summary>
        public const string EmpFrom = "EmpFrom";
        /// <summary>
        /// ����Ա
        /// </summary>
        public const string EmpTo = "EmpTo";
        /// <summary>
        /// ���
        /// </summary>
        public const string Msg = "Msg";
        /// <summary>
        /// EmpFromT
        /// </summary>
        public const string EmpFromT = "EmpFromT";
        /// <summary>
        /// NDFromT
        /// </summary>
        public const string NDFromT = "NDFromT";
        /// <summary>
        /// NDToT
        /// </summary>
        public const string NDToT = "NDToT";
        /// <summary>
        /// EmpToT
        /// </summary>
        public const string EmpToT = "EmpToT";
        /// <summary>
        /// ʵ��ִ����Ա
        /// </summary>
        public const string Exer = "Exer";
    }
    /// <summary>
    /// �켣
    /// </summary>
    public class Track : BP.En.Entity
    {
        public override string PK
        {
            get
            {
                return "MyPK";
            }
        }

        public override string PKField
        {
            get
            {
                return "MyPK";
            }
        }

        #region attrs
        /// <summary>
        /// �ڵ��
        /// </summary>
        public int NDFrom
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.NDFrom);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDFrom, value);
            }
        }
        /// <summary>
        /// �ڵ㵽
        /// </summary>
        public int NDTo
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.NDTo);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDTo, value);
            }
        }
        /// <summary>
        /// ����Ա
        /// </summary>
        public string EmpFrom
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpFrom);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpFrom, value);
            }
        }
        /// <summary>
        /// ����Ա
        /// </summary>
        public string EmpTo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpTo);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpTo, value);
            }
        }
        /// <summary>
        /// ��¼����
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RDT);
            }
            set
            {
                this.SetValByKey(TrackAttr.RDT, value);
            }
        }
        /// <summary>
        /// fid
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64ByKey(TrackAttr.FID);
            }
            set
            {
                this.SetValByKey(TrackAttr.FID, value);
            }
        }
        /// <summary>
        /// Workid
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(TrackAttr.WorkID);
            }
            set
            {
                this.SetValByKey(TrackAttr.WorkID, value);
            }
        }
        /// <summary>
        /// �����
        /// </summary>
        public ActionType HisActionType
        {
            get
            {
                return (ActionType)this.GetValIntByKey(TrackAttr.ActionType);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionType, (int)value);
            }
        }
        public static string GetActionTypeT(ActionType at)
        {
            switch (at)
            {
                case ActionType.Forward:
                    return "ǰ��";
                case ActionType.Return:
                    return "�˻�";
                case ActionType.Shift:
                    return "�ƽ�";
                case ActionType.UnShift:
                    return "�����ƽ�";
                case ActionType.Start:
                    return "����";
                case ActionType.UnSend:
                    return "��������";
                case ActionType.ForwardFL:
                    return " -ǰ��(������)";
                case ActionType.ForwardHL:
                    return " -������㷢��";
                case ActionType.FlowOver:
                    return "���̽���";
                case ActionType.CallSubFlow:
                    return "������������";
                case ActionType.StartSubFlow:
                    return "�����̷���";
                case ActionType.SubFlowForward:
                    return "������ǰ��";
                case ActionType.RebackOverFlow:
                    return "�ָ�����ɵ�����";
                case ActionType.FlowOverByCoercion:
                    return "ǿ�ƽ�������";
                case ActionType.HungUp:
                    return "����";
                case ActionType.UnHungUp:
                    return "ȡ������";
                case ActionType.Press:
                    return "�߰�";
                case ActionType.CC:
                    return "����";
                default:
                    return "δ֪";
            }
        }
        public string ActionTypeText
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.ActionTypeText);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionTypeText, value);
            }
        }
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string NodeData
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NodeData);
            }
            set
            {
                this.SetValByKey(TrackAttr.NodeData, value);
            }
        }
        public string Exer
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Exer);
            }
            set
            {
                this.SetValByKey(TrackAttr.Exer, value);
            }
        }
        /// <summary>
        /// ������
        /// </summary>
        public string Msg
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Msg);
            }
            set
            {
                this.SetValByKey(TrackAttr.Msg, value);
            }
        }
        public string MsgHtml
        {
            get
            {
                return this.GetValHtmlStringByKey(TrackAttr.Msg);
            }
        }
        public string EmpToT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpToT);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpToT, value);
            }
        }
        public string EmpFromT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpFromT);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpFromT, value);
            }
        }

        public string NDFromT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NDFromT);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDFromT, value);
            }
        }
        public string NDToT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.NDToT);
            }
            set
            {
                this.SetValByKey(TrackAttr.NDToT, value);
            }
        }
        #endregion attrs

        #region ����
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();

                #region ��������
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //Ҫ���ӵ�����Դ����ʾҪ���ӵ����Ǹ�ϵͳ���ݿ⣩��
                map.PhysicsTable = "WF_Track"; // Ҫ�����
                map.EnDesc = "�켣��";
                map.EnType = EnType.App;
                #endregion

                #region �ֶ�

                //����һ���Զ���������.
               map.AddTBIntPK(TrackAttr.MyPK, 0, "MyPK", true, false);

                map.AddTBInt(TrackAttr.ActionType, 0, "����", true, false);
                map.AddTBString(TrackAttr.ActionTypeText, null, "����(����)", true, false, 0, 30, 100);

                map.AddTBInt(TrackAttr.FID, 0, "����ID", true, false);
                map.AddTBInt(TrackAttr.WorkID, 0, "����ID", true, false);


                map.AddTBInt(TrackAttr.NDFrom, 0, "�ӽڵ�", true, false);
                map.AddTBString(TrackAttr.NDFromT, null, "�ӽڵ�(����)", true, false, 0, 100, 100);

                map.AddTBInt(TrackAttr.NDTo, 0, "���ڵ�", true, false);
                map.AddTBString(TrackAttr.NDToT, null, "���ڵ�(����)", true, false, 0, 100, 100);

                map.AddTBString(TrackAttr.EmpFrom, null, "����Ա", true, false, 0, 20, 100);
                map.AddTBString(TrackAttr.EmpFromT, null, "����Ա(����)", true, false, 0, 30, 100);

                map.AddTBString(TrackAttr.EmpTo, null, "����Ա", true, false, 0, 20, 100);
                map.AddTBString(TrackAttr.EmpToT, null, "����Ա(����)", true, false, 0, 30, 100);

                map.AddTBString(TrackAttr.RDT, null, "����", true, false, 0, 20, 100);

                map.AddTBFloat(TrackAttr.WorkTimeSpan, 0, "ʱ����(��)", true, false);
                map.AddTBStringDoc(TrackAttr.Msg, null, "��Ϣ", true, false);
                map.AddTBStringDoc(TrackAttr.NodeData, null, "�ڵ�����(��־��Ϣ)", true, false);

                map.AddTBString(TrackAttr.Exer, null, "ִ����", true, false, 0, 20, 100);
                #endregion �ֶ�

                this._enMap = map;
                return this._enMap;
            }
        }
        ///// <summary>
        ///// �켣
        ///// </summary>
        ///// <param name="rptName"></param>
        //public Track(string mypk)
        //{
        //    this.MyPK = mypk;
        //    if (this.RetrieveFromDBSources() == 0)
        //    {
        //        TrackTemp t = new TrackTemp(this.MyPK);
        //        this.Row = t.Row;
        //    }
        //}

        public string FK_Flow = null;
        /// <summary>
        /// �켣
        /// </summary>
        public Track()
        {
        }
        /// <summary>
        /// �켣
        /// </summary>
        /// <param name="flowNo">���̱��</param>
        /// <param name="mypk">����</param>
        public Track(string flowNo,string mypk)
        {
            string sql = "SELECT * FROM ND" + int.Parse(flowNo) + "Track WHERE MyPK='"+mypk+"'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@��־���ݶ�ʧ.."+sql);
            this.Row.LoadDataTable(dt, dt.Rows[0]);
        }
        /// <summary>
        /// ����track.
        /// </summary>
        /// <param name="fk_flow">���̱��</param>
        public static void CreateTrackTable(string fk_flow)
        {
            string ptable = "ND" + int.Parse(fk_flow) + "Track";
            if (BP.DA.DBAccess.IsExitsObject(ptable))
                return;

            try
            {
                /*���������ָ���ı�,�ʹ�����.*/
                BP.DA.DBAccess.RunSQL("DROP TABLE WF_Track");
            }
            catch
            {
            }

            Track tk = new Track();
            tk.CheckPhysicsTable();

            string sqlRename = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sqlRename = "EXEC SP_RENAME WF_Track, " + ptable;
                    break;
                case DBType.Informix:
                    sqlRename = "RENAME TABLE WF_Track TO " + ptable;
                    break;
                case DBType.Oracle:
                    sqlRename = "ALTER TABLE WF_Track rename to " + ptable;
                    break;
                case DBType.MySQL:
                    sqlRename = "ALTER TABLE WF_Track rename to " + ptable;
                    break;
                default:
                    throw new Exception("δ�漰��������.");
            }
            DBAccess.RunSQL(sqlRename);
        }
      
        /// <summary>
        /// ������Ȩ��
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (BP.Web.WebUser.IsAuthorize)
                this.Exer = BP.Web.WebUser.AuthorizerEmpID + "," + BP.Web.WebUser.Auth;
            else
                this.Exer = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name;

            string ptable = "ND" + int.Parse(this.FK_Flow) + "Track";

            string dbstr = SystemConfig.AppCenterDBVarStr;

            string sql = "INSERT INTO " + ptable;
            sql += "(";
            sql += "" + TrackAttr.MyPK + ",";
            sql += "" + TrackAttr.ActionType + ",";
            sql += "" + TrackAttr.ActionTypeText + ",";
            sql += "" + TrackAttr.FID + ",";
            sql += "" + TrackAttr.WorkID + ",";
            sql += "" + TrackAttr.NDFrom + ",";
            sql += "" + TrackAttr.NDFromT + ",";
            sql += "" + TrackAttr.NDTo + ",";
            sql += "" + TrackAttr.NDToT + ",";

            sql += "" + TrackAttr.EmpFrom + ",";
            sql += "" + TrackAttr.EmpFromT + ",";
            sql += "" + TrackAttr.EmpTo + ",";
            sql += "" + TrackAttr.EmpToT + ",";
            sql += "" + TrackAttr.RDT + ",";
            sql += "" + TrackAttr.WorkTimeSpan + ",";
            sql += "" + TrackAttr.Msg + ",";
            sql += "" + TrackAttr.NodeData + ",";
            sql += "" + TrackAttr.Exer + "";
            sql += ") VALUES (";
            sql += dbstr + TrackAttr.MyPK + ",";
            sql += dbstr + TrackAttr.ActionType + ",";
            sql += dbstr + TrackAttr.ActionTypeText + ",";
            sql += dbstr + TrackAttr.FID + ",";
            sql += dbstr + TrackAttr.WorkID + ",";
            sql += dbstr + TrackAttr.NDFrom + ",";
            sql += dbstr + TrackAttr.NDFromT + ",";
            sql += dbstr + TrackAttr.NDTo + ",";
            sql += dbstr + TrackAttr.NDToT + ",";
            sql += dbstr + TrackAttr.EmpFrom + ",";
            sql += dbstr + TrackAttr.EmpFromT + ",";
            sql += dbstr + TrackAttr.EmpTo + ",";
            sql += dbstr + TrackAttr.EmpToT + ",";
            sql += dbstr + TrackAttr.RDT + ",";
            sql += dbstr + TrackAttr.WorkTimeSpan + ",";
            sql += dbstr + TrackAttr.Msg + ",";
            sql += dbstr + TrackAttr.NodeData + ",";
            sql += dbstr + TrackAttr.Exer + "";
            sql += ")";

            this.ActionTypeText = Track.GetActionTypeT(this.HisActionType);
            this.SetValByKey(TrackAttr.MyPK, DBAccess.GenerOIDByGUID());

            #region ִ�б���
            try
            {
                Paras ps = SqlBuilder.GenerParas(this, null);
                ps.SQL = sql;
               // ps.Remove("MyPK");

                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        this.RunSQL(ps);
                        break;
                    case DBType.Access:
                        this.RunSQL(ps);
                        break;
                    case DBType.MySQL:
                    case DBType.Informix:
                    default:
                        ps.SQL = ps.SQL.Replace("[", "").Replace("]", "");
                        this.RunSQL(ps); // ����sql.
                        //  this.RunSQL(sql.Replace("[", "").Replace("]", ""), SqlBuilder.GenerParas(this, null));
                        break;
                }
            }
            catch (Exception ex)
            {
                // д����־.
                Log.DefaultLogWriteLineError(ex.Message);

                //����track.
                Track.CreateTrackTable(this.FK_Flow);
                throw ex;
            }
            #endregion ִ�б���

            //�Ͳ�������ִ��insert�ˡ�
            return false;
        }
        #endregion attrs
    }
    /// <summary>
    /// �켣����
    /// </summary>
    public class Tracks : BP.En.Entities
    {
        /// <summary>
        /// �켣����
        /// </summary>
        public Tracks()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new Track();
            }
        }
    }
}
