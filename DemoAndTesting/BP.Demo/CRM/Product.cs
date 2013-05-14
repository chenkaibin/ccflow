using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
	/// <summary>
	/// ��Ʒ
	/// </summary>
	public class ProductAttr: EntityNoNameAttr
	{
		#region ��������
		public const  string FK_SF="FK_SF";
        public const string Addr = "Addr";

		#endregion
	}
	/// <summary>
    /// ��Ʒ
	/// </summary>
    public class Product : EntityNoName
    {
        #region ��������

        #endregion

        #region ���캯��
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// ��Ʒ
        /// </summary>		
        public Product() { }
        /// <summary>
        /// ��Ʒ
        /// </summary>
        /// <param name="no"></param>
        public Product(string no)
            : base(no)
        {

        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map();

                #region ��������
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.PhysicsTable = "Demo_Product";
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsAllowRepeatNo = false;
                map.IsCheckNoLength = false;
                map.EnDesc = "��Ʒ";
                map.EnType = EnType.App;
                map.CodeStruct = "4";
                #endregion

                #region �ֶ�
                map.AddTBStringPK(ProductAttr.No, null, "���", true, false, 0, 50, 50);
                map.AddTBString(ProductAttr.Name, null, "����", true, false, 0, 50, 200);
                map.AddTBString(ProductAttr.Addr, null, "������ַ", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new Products(); }
        }
        #endregion
    }
	/// <summary>
	/// ��Ʒ
	/// </summary>
	public class Products : EntitiesNoName
	{
		#region 
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Product();
			}
		}	
		#endregion 

		#region ���췽��
		/// <summary>
		/// ��Ʒs
		/// </summary>
		public Products(){}
		#endregion
	}
	
}