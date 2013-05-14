using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Rpt
{
	/// <summary>
	/// 会计期间
	/// </summary>
	public class RptSort :SimpleNoNameFix
	{
		#region 实现基本的方方法
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Rpt_Sort";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
				return "报表类别";
			}
		}
		#endregion 

		#region 构造方法
		public RptSort()
		{
		}
		public RptSort(string _No ):base(_No)
		{
		}
		#endregion 
	}
	/// <summary>
	/// Sorts
	/// </summary>
	public class RptSorts :SimpleNoNameFixs
	{
		/// <summary>
		/// 会计期间集合
		/// </summary>
		public RptSorts()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new RptSort();
			}
		}
	}
}
