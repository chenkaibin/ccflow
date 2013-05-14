using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Port
{
    /// <summary>
    /// ��λ
    /// </summary>
    public class UnitAttr : EntityNoNameAttr
    {
        /// <summary>
        /// ��־
        /// </summary>
        public const string Flag = "Flag";
        /// <summary>
        /// ���ڵ�ı��
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
    ///  ��λ
	/// </summary>
	public class Unit :EntityNoName
    {
        #region ����
        /// <summary>
        /// ���ڵ���
        /// </summary>
        public string Flag
        {
            get
            {
                return this.GetValStringByKey(UnitAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(UnitAttr.ParentNo, value);
            }
        }
        #endregion

     
		#region ���췽��
		/// <summary>
		/// ��λ
		/// </summary>
		public Unit()
        {
        }
        /// <summary>
        /// ��λ
        /// </summary>
        /// <param name="_No"></param>
        public Unit(string _No) : base(_No) { }
		#endregion 

		/// <summary>
		/// ��λMap
		/// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_Unit");
                map.EnDesc = "��λ";
                map.CodeStruct = "2";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.IsAllowRepeatNo = false;

                map.AddTBStringPK(UnitAttr.No, null, "���", true, false, 1, 50, 50);
                map.AddTBString(UnitAttr.Name, null, "����", true, false, 1, 50, 20);
                map.AddTBString(UnitAttr.ParentNo, null, "���ڵ���", true, false, 1, 50, 20);

                //map.AddTBString(UnitAttr.Flag, null, "���", true, false, 1, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
	}
	/// <summary>
    /// ��λ
	/// </summary>
    public class Units : EntitiesNoName
	{
		/// <summary>
		/// ��λs
		/// </summary>
        public Units() { }
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
                return new Unit();
			}
		}
        public override int RetrieveAll()
        {
            QueryObject qo11 = new QueryObject(this);
            qo11.AddWhere(DeptAttr.No, " like ", Web.WebUser.FK_Unit + "%");
            return qo11.DoQuery();
        }
	}
}
