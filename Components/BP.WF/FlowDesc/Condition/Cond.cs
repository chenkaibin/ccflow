using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.WF
{
    /// <summary>
    /// �����������Դ
    /// </summary>
    public enum ConnDataFrom
    {
        /// <summary>
        /// ��������
        /// </summary>
        Form,
        /// <summary>
        /// ��λ����
        /// </summary>
        Stas,
        /// <summary>
        /// Depts
        /// </summary>
        Depts,
        /// <summary>
        /// ��sql����.
        /// </summary>
        SQL,
        /// <summary>
        /// ������
        /// </summary>
        Paras
    }
  
    /// <summary>
    /// ��������
    /// </summary>
    public class CondAttr
    {
        /// <summary>
        /// ������Դ
        /// </summary>
        public const string DataFrom = "DataFrom";
        /// <summary>
        /// ����Key
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// ����Key
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// ����
        /// </summary>
        public const string AttrName = "AttrName";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Attr = "FK_Attr";
        /// <summary>
        /// �������
        /// </summary>
        public const string FK_Operator = "FK_Operator";
        /// <summary>
        /// �����ֵ
        /// </summary>
        public const string OperatorValue = "OperatorValue";
        /// <summary>
        /// ����ֵ
        /// </summary>
        public const string OperatorValueT = "OperatorValueT";
        /// <summary>
        /// Node
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// ��������
        /// </summary>
        public const string CondType = "CondType";
        /// <summary>
        /// ����
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// �Է���������Ч
        /// </summary>
        public const string ToNodeID = "ToNodeID";
        /// <summary>
        /// �жϷ�ʽ
        /// </summary>
        public const string ConnJudgeWay = "ConnJudgeWay";
        /// <summary>
        /// MyPOID
        /// </summary>
        public const string MyPOID = "MyPOID";
        /// <summary>
        /// PRI
        /// </summary>
        public const string PRI = "PRI";
    }
    /// <summary>
    /// ��������
    /// </summary>
    public enum CondType
    {
        /// <summary>
        /// �ڵ��������
        /// </summary>
        Node,
        /// <summary>
        /// ��������
        /// </summary>
        Flow,
        /// <summary>
        /// ��������
        /// </summary>
        Dir
    }
    /// <summary>
    /// ����
    /// </summary>
    public class Cond : EntityMyPK
    {
        public Work en = null;
        /// <summary>
        /// ������Դ
        /// </summary>
        public ConnDataFrom HisDataFrom
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(CondAttr.DataFrom);
            }
            set
            {
                this.SetValByKey(CondAttr.DataFrom, (int)value);
            }
        }
        /// <summary>
        /// ���̱��
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// ��������(������������λ���������������������߲���)
        /// </summary>
        public CondType HisCondType
        {
            get
            {
                return (CondType)this.GetValIntByKey(CondAttr.CondType);
            }
            set
            {
                this.SetValByKey(CondAttr.CondType, (int)value);
            }
        }
        /// <summary>
        /// Ҫ����Ľڵ�
        /// </summary>
        public Node HisNode
        {
            get
            {
                return new Node(this.NodeID);
            }
        }
        /// <summary>
        /// ���ȼ�
        /// </summary>
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(CondAttr.PRI);
            }
            set
            {
                this.SetValByKey(CondAttr.PRI, value);
            }
        }
        /// <summary>
        /// MyPOID
        /// </summary>
        public int MyPOID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.MyPOID);
            }
            set
            {
                this.SetValByKey(CondAttr.MyPOID, value);
            }
        }
        /// <summary>
        /// �ڵ�ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.NodeID);
            }
            set
            {
                this.SetValByKey(CondAttr.NodeID, value);
            }
        }
        /// <summary>
        /// �ڵ�ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                int i = this.GetValIntByKey(CondAttr.FK_Node);
                if (i == 0)
                    return this.NodeID;
                return i;
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string FK_NodeT
        {
            get
            {
                Node nd = new Node(this.FK_Node);
                return nd.Name;
            }
        }
        /// <summary>
        /// �Է���������Ч
        /// </summary>
        public int ToNodeID
        {
            get
            {
                return this.GetValIntByKey(CondAttr.ToNodeID);
            }
            set
            {
                this.SetValByKey(CondAttr.ToNodeID, value);
            }
        }

        /// <summary>
        /// �ڸ��������֮ǰҪ���ò�����
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {
            this.RunSQL("UPDATE WF_Node SET IsCCNode=0,IsCCFlow=0");
            this.RunSQL("UPDATE WF_Node SET IsCCNode=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond WHERE CondType=" + (int)CondType.Node + ")");
            this.RunSQL("UPDATE WF_Node SET IsCCFlow=1 WHERE NodeID IN (SELECT NodeID FROM WF_Cond WHERE CondType=" + (int)CondType.Flow + ")");

            this.MyPOID = BP.DA.DBAccess.GenerOID();
            return base.beforeUpdateInsertAction();
        }

        #region ʵ�ֻ����ķ�����
        /// <summary>
        /// ����
        /// </summary>
        public string FK_Attr
        {
            get
            {
                return this.GetValStringByKey(CondAttr.FK_Attr);
            }
            set
            {
                if (value == null)
                    throw new Exception("FK_Attr��������Ϊnull");

                value = value.Trim();

                this.SetValByKey(CondAttr.FK_Attr, value);

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr(value);
                this.SetValByKey(CondAttr.AttrKey, attr.KeyOfEn);
                this.SetValByKey(CondAttr.AttrName, attr.Name);
            }
        }
        /// <summary>
        /// Ҫ�����ʵ������
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrKey);
            }
        }
        /// <summary>
        /// ��������
        /// </summary>
        public string AttrName
        {
            get
            {
                return this.GetValStringByKey(CondAttr.AttrName);
            }
        }
        /// <summary>
        /// ������ֵ
        /// </summary>
        public string OperatorValueT
        {
            get
            {
                return this.GetValStringByKey(CondAttr.OperatorValueT);
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValueT, value);
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        public string FK_Operator
        {
            get
            {
                string s = this.GetValStringByKey(CondAttr.FK_Operator);
                if (s == null || s == "")
                    return "=";
                return s;
            }
            set
            {
                this.SetValByKey(CondAttr.FK_Operator, value);
            }
        }
        /// <summary>
        /// ����ֵ
        /// </summary>
        public object OperatorValue
        {
            get
            {
                return this.GetValStringByKey(CondAttr.OperatorValue);
            }
            set
            {
                this.SetValByKey(CondAttr.OperatorValue, value as string);
            }
        }
        /// <summary>
        /// ����ֵStr
        /// </summary>
        public string OperatorValueStr
        {
            get
            {
                string sql= this.GetValStringByKey(CondAttr.OperatorValue);
                sql = sql.Replace("~", "'");
                return sql;
            }
        }
        /// <summary>
        /// ����ֵint
        /// </summary>
        public int OperatorValueInt
        {
            get
            {
                return this.GetValIntByKey(CondAttr.OperatorValue);
            }
        }
        /// <summary>
        /// ����ֵboolen
        /// </summary>
        public bool OperatorValueBool
        {
            get
            {
                return this.GetValBooleanByKey(CondAttr.OperatorValue);
            }
        }
        /// <summary>
        /// workid
        /// </summary>
        private Int64 _WorkID = 0;
        public Int64 WorkID
        {
            get
            {
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string MsgOfCond = "";
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="fk_node">�ڵ�ID</param>
        public void DoUp(int fk_node)
        {
            int condtypeInt = (int)this.HisCondType;
            this.DoOrderUp(CondAttr.FK_Node, fk_node.ToString(),CondAttr.CondType, condtypeInt.ToString(), CondAttr.PRI);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="fk_node">�ڵ�ID</param>
        public void DoDown(int fk_node)
        {
            int condtypeInt = (int)this.HisCondType;
            this.DoOrderDown(CondAttr.FK_Node, fk_node.ToString(), CondAttr.CondType, condtypeInt.ToString(), CondAttr.PRI);
        }
        #endregion

        #region ���췽��
        /// <summary>
        /// ����
        /// </summary>
        public Cond() { }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="mypk"></param>
        public Cond(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��������ܲ���ͨ��
        /// </summary>
        public virtual bool IsPassed
        {
            get
            {
                Node nd = new Node(this.FK_Node);
                if (this.en == null)
                {
                    Work en = nd.HisWork;
                    try
                    {
                        en.SetValByKey("OID", this.WorkID);
                        en.Retrieve();
                        this.en = en;
                    }
                    catch (Exception ex)
                    {
                        //this.Delete();
                        return false;
                        //throw new Exception("@��ȡ���ж�����ʵ��[" + nd.EnDesc + "], ���ִ���:" + ex.Message + "@����ԭ���Ƕ������̵��ж��������ִ���,��������ѡ����ж������������ǵ�ǰ�����ڵ����һ���������,ȡ������ʵ���ʵ��.");
                    }
                }

                if (this.HisDataFrom == ConnDataFrom.Stas)
                {
                    string strs = this.OperatorValue.ToString();
                    BP.Port.EmpStations sts = new BP.Port.EmpStations();
                    sts.Retrieve("FK_Emp",en.Rec);
                    string strs1 = "";
                    foreach (BP.Port.EmpStation st in sts)
                    {
                        if (strs.Contains("@" + st.FK_Station+"@"))
                        {
                            this.MsgOfCond = "@�Ը�λ�жϷ�������Ϊtrue����λ����" + strs + "������Ա(" + en.Rec + ")��λ:" + st.FK_Station + st.FK_StationT;
                            return true;
                        }
                        strs1 += st.FK_Station + "-" + st.FK_StationT;
                    }

                    this.MsgOfCond = "@�Ը�λ�жϷ�������Ϊfalse����λ����" + strs + "������Ա(" + en.Rec + ")��λ:" + strs1;
                    return false;
                }

                if (this.HisDataFrom == ConnDataFrom.Depts)
                {
                    string strs = this.OperatorValue.ToString();
                    BP.Port.EmpDepts sts = new BP.Port.EmpDepts();
                    sts.Retrieve("FK_Emp", en.Rec);
                    string strs1 = "";
                    foreach (BP.Port.EmpDept st in sts)
                    {
                        if (strs.Contains("@" + st.FK_Dept + "@"))
                        {
                            this.MsgOfCond = "@�Ը�λ�жϷ�������Ϊtrue�����ż���" + strs + "������Ա(" + en.Rec + ")����:" + st.FK_Dept + st.FK_DeptT;
                            return true;
                        }
                        strs1 += st.FK_Dept + "-" + st.FK_DeptT;
                    }

                    this.MsgOfCond = "@�Բ����жϷ�������Ϊfalse�����ż���" + strs + "������Ա(" +  en.Rec+ ")����:" + strs1;
                    return false;
                }

                if (this.HisDataFrom == ConnDataFrom.SQL)
                {
                    //this.MsgOfCond = "@�Ա���ֵ�жϷ���ֵ " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") ������:(" + this.FK_Operator + ") �ж�ֵ:(" + this.OperatorValue.ToString() + ")";
                    string sql = this.OperatorValueStr;
                    sql = sql.Replace("~", "'");
                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                    if (sql.Contains("@") == true)
                    {
                        /* ������� @ */
                        foreach (Attr attr in this.en.EnMap.Attrs)
                        {
                            sql = sql.Replace("@" + attr.Key, en.GetValStrByKey(attr.Key));
                        }
                    }

                    int result = DBAccess.RunSQLReturnValInt(sql, -1);
                    if (result == 0)
                        return false;

                    if (result >= 1)
                        return true;

                    throw new Exception("@�����õ�sql����ֵ��������ccflow��Ҫ�󣬱�����0����ڵ���1��");
                }

                if (this.HisDataFrom == ConnDataFrom.Paras)
                {
                    //this.MsgOfCond = "@�Ա���ֵ�жϷ���ֵ " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") ������:(" + this.FK_Operator + ") �ж�ֵ:(" + this.OperatorValue.ToString() + ")";
                    string exp = this.OperatorValueStr;
                    string[] strs = exp.Trim().Split(' ');

                    string key = strs[0].Trim();
                    string oper = strs[1].Trim();
                    string val = strs[2].Trim();
                    val = val.Replace("'", "");
                    val = val.Replace("%", "");
                    val = val.Replace("~", "");

                    BP.En.Row row = this.en.Row;
                    foreach (string item in row.Keys)
                    {
                        if (key != item.Trim())
                            continue;

                        string valPara = row[key].ToString();
                        if (oper == "=")
                        {
                            if (valPara == val)
                                return true;
                            else
                                return false;
                        }

                        if (oper.ToUpper() == "LIKE")
                        {
                            if (valPara.Contains(val))
                                return true;
                            else
                                return false;
                        }

                        if (oper == ">")
                        {
                            if (float.Parse(valPara) > float.Parse(val))
                                return true;
                            else
                                return false;
                        }
                        if (oper == ">=")
                        {
                            if (float.Parse(valPara) >= float.Parse(val))
                                return true;
                            else
                                return false;
                        }
                        if (oper == "<")
                        {
                            if (float.Parse(valPara) < float.Parse(val))
                                return true;
                            else
                                return false;
                        }
                        if (oper == "<=")
                        {
                            if (float.Parse(valPara) <= float.Parse(val))
                                return true;
                            else
                                return false;
                        }

                        if (oper == "!=")
                        {
                            if (float.Parse(valPara) != float.Parse(val))
                                return true;
                            else
                                return false;
                        }
                        throw new Exception("@������ʽ����:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
                    }

                    throw new Exception("@�ж�����ʱ����,û���ҵ���Ӧ�ı���ʽ:" + exp + " Key=" + key + " oper=" + oper + " Val=" + val);
                }

                try
                {
                    if (en.EnMap.Attrs.Contains(this.AttrKey) == false)
                        throw new Exception("�ж�����������ִ���ʵ�壺" + nd.EnDesc + " ����" + this.AttrKey + "�Ѿ�������.");

                    this.MsgOfCond = "@�Ա���ֵ�жϷ���ֵ " + en.EnDesc + "." + this.AttrKey + " (" + en.GetValStringByKey(this.AttrKey) + ") ������:(" + this.FK_Operator + ") �ж�ֵ:(" + this.OperatorValue.ToString() + ")";

                    switch (this.FK_Operator.Trim().ToLower())
                    {
                        case "<>":
                            if (en.GetValStringByKey(this.AttrKey) != this.OperatorValue.ToString())
                                return true;
                            else
                                return false;
                        case "=":  // ����� = 
                            if (en.GetValStringByKey(this.AttrKey) == this.OperatorValue.ToString())
                                return true;
                            else
                                return false;
                        case ">":
                            if (en.GetValDoubleByKey(this.AttrKey) > Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        case ">=":
                            if (en.GetValDoubleByKey(this.AttrKey) >= Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        case "<":
                            if (en.GetValDoubleByKey(this.AttrKey) < Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        case "<=":
                            if (en.GetValDoubleByKey(this.AttrKey) <= Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        case "!=":
                            if (en.GetValDoubleByKey(this.AttrKey) != Double.Parse(this.OperatorValue.ToString()))
                                return true;
                            else
                                return false;
                        case "like":
                            if (en.GetValStringByKey(this.AttrKey).IndexOf(this.OperatorValue.ToString()) == -1)
                                return false;
                            else
                                return true;
                        default:
                            throw new Exception("@û���ҵ���������(" + this.FK_Operator.Trim().ToLower() + ").");
                    }
                }
                catch (Exception ex)
                {
                    Node nd23 = new Node(this.NodeID);
                    throw new Exception("@�ж�����:Node=[" + this.NodeID + "," + nd23.EnDesc + "], ���ִ���@" + ex.Message + "���п����������˷Ƿ��������жϷ�ʽ��");
                }
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_Cond");
                map.EnDesc = "��������";

                map.AddMyPK();
                map.AddTBInt(CondAttr.CondType, 0, "��������", true, true);
                //map.AddDDLSysEnum(CondAttr.CondType, 0, "��������", true, false, CondAttr.CondType,"@0=�ڵ��������@1=�����������@2=��������");

                map.AddTBInt(CondAttr.DataFrom, 0, "����������Դ0����,1��λ(�Է���������Ч)", true, true);
                map.AddTBString(CondAttr.FK_Flow, null, "����", true, true, 0, 60, 20);
                
                map.AddTBInt(CondAttr.NodeID, 0, "�������¼�MainNode", true, true);

                map.AddTBInt(CondAttr.FK_Node, 0, "�ڵ�ID", true, true);
                map.AddTBString(CondAttr.FK_Attr, null, "����", true, true, 0, 80, 20);
                map.AddTBString(CondAttr.AttrKey, null, "���Լ�", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.AttrName, null, "��������", true, true, 0, 500, 20);
                map.AddTBString(CondAttr.FK_Operator, "=", "�������", true, true, 0, 60, 20);
                map.AddTBString(CondAttr.OperatorValue, "", "Ҫ�����ֵ", true, true, 0, 4000, 20);
                map.AddTBString(CondAttr.OperatorValueT, "", "Ҫ�����ֵT", true, true, 0, 4000, 20);
                map.AddTBInt(CondAttr.ToNodeID, 0, "ToNodeID���Է���������Ч��", true, true);
                map.AddDDLSysEnum(CondAttr.ConnJudgeWay, 0, "������ϵ", true, false, CondAttr.ConnJudgeWay, "@0=or@1=and");
                map.AddTBInt(CondAttr.MyPOID, 0, "MyPOID", true, true);
                map.AddTBInt(CondAttr.PRI, 0, "�������ȼ�", true, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ����s
    /// </summary>
    public class Conds : Entities
    {
        #region ����
        /// <summary>
        /// ���Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get { return new Cond(); }
        }
        /// <summary>
        /// ������������������ǲ��Ƕ�����.
        /// </summary>
        public bool IsAllPassed
        {
            get
            {
                if (this.Count == 0)
                    throw new Exception("@û��Ҫ�жϵļ���.");

                foreach (Cond en in this)
                {
                    if (en.IsPassed == false)
                        return false;
                }
                return true;
            }
        }
        /// <summary>
        /// �Ƿ�ͨ�� (ͳһ�ھ����ǰ�and��ʽ�ж�û��or�Ĺ�ϵ��)
        /// </summary>
        public bool IsPass
        {
            get
            {
                // �ж�  and. �Ĺ�ϵ��
                foreach (Cond en in this)
                {
                    if (en.IsPassed == false)
                        return false;
                }
                return true;
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        public string MsgOfDesc
        {
            get
            {
                string msg = "";
                foreach (Cond c in this)
                {
                    msg += "@" + c.MsgOfCond;
                }
                return msg;
            }
        }
        /// <summary>
        /// �ǲ������е�һ��passed. 
        /// </summary>
        public bool IsOneOfCondPassed
        {
            get
            {
                foreach (Cond en in this)
                {
                    if (en.IsPassed == true)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// ȡ������һ�������������. 
        /// </summary>
        public Cond GetOneOfCondPassed
        {
            get
            {
                foreach (Cond en in this)
                {
                    if (en.IsPassed == true)
                        return en;
                }
                throw new Exception("@û�����������");
            }
        }
        public int NodeID = 0;
        #endregion

        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public Conds()
        {
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="fk_flow">���̱��</param>
        public Conds(string fk_flow)
        {
            this.Retrieve(CondAttr.FK_Flow, fk_flow);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="ct">����</param>
        /// <param name="nodeID">�ڵ�</param>
        public Conds(CondType ct, int nodeID, Int64 workid,Work enData)
        {
            this.NodeID = nodeID;
            this.Retrieve(CondAttr.NodeID, nodeID, CondAttr.CondType, (int)ct, CondAttr.PRI);
            foreach (Cond en in this)
            {
                en.WorkID = workid;
                en.en = enData;
            }
        }

        public string ConditionDesc
        {
            get
            {
                return "";
            }
        }
        #endregion
    }
}