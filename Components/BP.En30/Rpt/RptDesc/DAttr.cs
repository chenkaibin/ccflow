
using System;
using System.Collections;
using BP.DA;
using System.Data;
using BP.Sys;
using BP.En;

namespace BP.Rpt
{
	public enum ChartType
	{
		/// <summary>
		/// 饼状图
		/// </summary>
		Pie,
		/// <summary>
		/// 柱状图
		/// </summary>
		Histogram,
		/// <summary>
		/// 折线图		
		/// </summary>
		Line
	}
	/// <summary>
	/// 工作方式
	/// </summary>
	public enum WorkWay
	{
		/// <summary>
		/// 自己定义
		/// </summary>
		Self,
		/// <summary>
		/// 求和
		/// </summary>
		Sum,
		/// <summary>
		/// 求平均数
		/// </summary>
		Avg,
		/// <summary>
		/// 求数量。
		/// </summary>
		Count
		
	}
	/// <summary>
	/// Entity 的摘要说明。
	/// </summary>	
	[Serializable]
	public class DAttr
	{
		#region 工作方式
		public string Tag=null;
		public Attr Attr=null;
		/// <summary>
		/// 工作方式
		/// </summary>
		public WorkWay WorkWay=WorkWay.Count;
		/// <summary>
		/// 如果是0是否把他去掉。
		/// </summary>
		public bool IsCutIfIsZero=false;
		#endregion

		#region 构造
		/// <summary>
		/// 实体
		/// </summary>
		public DAttr(Attr attr,WorkWay ww,string tag,bool IsCutIfIsZero)
		{
			this.Attr=attr;
			this.WorkWay=ww;
			this.IsCutIfIsZero= IsCutIfIsZero;
			this.Tag=tag;
		}
		#endregion

	}
	/// <summary>
	/// 
	/// </summary>
	public class DAttrs :System.Collections.CollectionBase
	{
		#region public 
		/// <summary>
		/// 增加一个属性
		/// </summary>
		/// <param name="attr">DAttr</param>
		public void Add(DAttr attr)
		{
			this.InnerList.Add(attr);
		}
		/// <summary>
		/// 根据一个key获得dattr.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public DAttr GetDAttrByKey(string key)
		{
			foreach(DAttr attr in this)
			{
				if (attr.Attr.Key==key)
					return attr;
			}
			throw new Exception("@不存在key=["+key+"]的属性。");
		}
		/// <summary>
		/// 他的attr集合
		/// </summary>
		public Attrs HisAttrs
		{
			get
			{
				Attrs attrs = new Attrs();
				foreach(DAttr attr in this)
				{
					attrs.Add(attr.Attr);
				}
				return attrs;
			}
		}
		#endregion

		#region 构造
		/// <summary>
		/// 构造方法
		/// </summary>
		public DAttrs()
		{
		} 
		#endregion

		#region 索引访问
		/// <summary>
		/// 根据索引访问集合内的元素Attr。
		/// </summary>
		public DAttr this[int index]
		{			
			get
			{	
				return (DAttr)this.InnerList[index];
			}
		}
		#endregion
	}
}
