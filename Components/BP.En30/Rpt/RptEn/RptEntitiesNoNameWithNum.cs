using System;
// using System.Drawing;
// using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En ; 
using BP.DA;
using BP.Web ; 

namespace BP.Rpt
{
	/// <summary>
	/// RptEntitiesNoNameMoney 的摘要说明。
	/// </summary>
	public class RptEntitiesNoNameWithNum : RptEntity
	{
		//public string LeftTitle="项目";
		/// <summary>
		/// 去掉项目=0 的纬度。
		/// </summary>
		public void CutIsZero()
		{
			EntitiesNoName ens =(EntitiesNoName)this.SingleDimensionItem1.CreateInstance();
			 
				foreach(EntityNoName en in this.SingleDimensionItem1)
				{
					Rpt1DCell cell = this.Rpt1DCells.GetCell(en.No);
					if (float.Parse(cell.val.ToString())!=0)
						ens.AddEntity(en);				;
				}
			 
			this.SingleDimensionItem1 =ens ; 

		}
		/// <summary>
		/// GetNameByNo
		/// </summary>
		/// <param name="No"></param>
		/// <returns></returns>
		public string GetTextByValue(string val)
		{
			foreach(Entity en in this.SingleDimensionItem1)
			{
				if (en.GetValStringByKey("No")== val )
					return en.GetValStringByKey("Name");
			}
			return "";
		}
		/// <summary>
		/// GetNameByNo
		/// </summary>
		/// <param name="No"></param>
		/// <returns></returns>
		public string GetNameByNo_del(string No)
		{
			foreach(EntityNoName en in this.SingleDimensionItem1)
			{
				if (en.No==No)
					return en.Name;
			}
			return "";
		}

		#region 属性
		/// <summary>
		/// SingleDimensionItem1
		/// </summary>
		public EntitiesNoName  SingleDimensionItem1=null;
		/// <summary>
		/// 单元格集合
		/// </summary>
		public Rpt1DCells Rpt1DCells=null;
		#endregion
		
		#region 构造函数
		public RptEntitiesNoNameWithNum()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
		/// 根据一个EntitiesNoName , 单元格 , 组合。
		/// </summary>
		/// <param name="ens"></param>
		/// <param name="cells"></param>
		public RptEntitiesNoNameWithNum(EntitiesNoName ens, Rpt1DCells cells)
		{
			this.SingleDimensionItem1=ens;
			this.Rpt1DCells =cells;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ens"></param>
		/// <param name="dt"></param>
		/// <param name="url"></param>
		public RptEntitiesNoNameWithNum(EntitiesNoName ens, DataTable dt, string url)
		{
			this.SingleDimensionItem1=ens;		 

			this.Rpt1DCells = new Rpt1DCells(dt);
		}
		#endregion
		//public RptEntitiesNoNameWithNum(
	}
}
