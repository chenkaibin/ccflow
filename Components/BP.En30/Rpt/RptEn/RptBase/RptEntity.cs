using System;
////using System.Drawing;
// using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using BP.En ; 
using BP.DA;
using BP.Web ; 

/*
 * Edit by peng: 2004-09-23
 * 
 * 报表的实体：
 * 
 * 
 * */

namespace BP.Rpt
{	
	/// <summary>
	/// 报表实体的基本属性：
	/// 他的2纬3位报表都是从此基础上继承下来的。
	/// </summary>
	abstract public class RptEntity
	{
		/// <summary>
		/// 对应的key1.
		/// </summary>
		public string Key1="Key1";
		/// <summary>
		/// key2
		/// </summary>
		public string Key2="Key2";
        /// <summary>
        /// Key3
        /// </summary>
		public string Key3="Key3";


		/// <summary>
		/// 分析对象的数据类型
		/// </summary>
		public AnalyseDataType HisADT=AnalyseDataType.AppInt;
		/// <summary>
		/// 是否显示tooltips
		/// </summary>
		public bool IsShowTooltips=false;
		/// <summary>
		/// 报表实体
		/// </summary>
		public RptEntity(){}
		/// <summary>
		/// 连接属性
		/// </summary>
		public string CellUrlTarget="_self";

		#region  基本属性
		/// <summary>
		/// 交叉报表标题
		/// </summary>
		public string Title="";

		/// <summary>
		/// 项目
		/// </summary>
		public string LeftTitle="项目";
		/// <summary>
		/// 作者
		/// </summary>
		public string Author=WebUser.Name ;
		/// <summary>
		/// 报表日期
		/// </summary>
		public string ReportDate=DateTime.Now.ToString("yyyy-MM-dd") ; 
		#endregion
	}
	
	
}
