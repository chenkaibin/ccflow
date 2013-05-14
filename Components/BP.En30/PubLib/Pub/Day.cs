using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Pub
{
	/// <summary>
	/// 天
	/// </summary>
	public class Day :SimpleNoNameFix
	{
		#region 实现基本的方方法
		 
		/// <summary>
		/// 物理表
		/// </summary>
		public override string  PhysicsTable
		{
			get
			{
				return "Pub_Day";
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		public override string  Desc
		{
			get
			{
                return this.ToE("Day", "天");  // "天";
			}
		}
		#endregion 

		#region 构造方法
		public Day()
        {
        }
        /// <summary>
        /// _No
        /// </summary>
        /// <param name="_No"></param>
		public Day(string _No ): base(_No)
        {
        }
		#endregion 
	}
	/// <summary>
	/// NDs
	/// </summary>
	public class Days :SimpleNoNameFixs
	{
		/// <summary>
		/// 天集合
		/// </summary>
		public Days()
		{
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Day();
			}
		}
	}
}
