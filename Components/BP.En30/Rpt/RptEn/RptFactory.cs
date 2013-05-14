using System;
using BP.En ; 
using BP.DA;
using BP.Web ; 

namespace BP.Rpt
{
	/// <summary>
	/// RptFactory 的摘要说明。
	/// </summary>
	public class RptFactory
	{
		public RptFactory()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		/// <summary>
		/// 产生3维得Rpt3DEntity
		/// 用来统计个数
		/// </summary>
		/// <param name="en">实体</param>
		/// <param name="attrOfD1">纬度1属性</param>
		/// <param name="attrOfD2">纬度2属性</param>
		/// <param name="attrOfD3">纬度3属性</param>
		/// <param name="d1d2RefKey">纬度2，3关联属性（可以为空）</param>
		/// <param name="cellUrl">Url，（可以为空）</param>
		/// <returns>生成的实体。</returns>
		public static Rpt3DEntity Rpt3DEntity(Entity en, string attrOfD1, string attrOfD2, string attrOfD3, string d1d2RefKey, string cellUrl)
		{
			return new Rpt3DEntity();
		}
 
	}
}
