using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;


namespace BP
{
	/// <summary>
	/// SystemConfigOfTax 的摘要说明。
	/// </summary>
	public class SystemConfigOfTax : SystemConfig
	{		

		#region 关于流程的配置
		/// <summary>
		/// 计算机判断是否汇总
		/// </summary>
		public static string WFSQLOfPCIsCollect
		{
			get{ return AppSettings["WFSQLOfPCIsCollect"]; }
		}
		
		#endregion
 
		/// <summary>
		/// 单位名称
		/// </summary>
		public static string UnitName
		{
			get{ return AppSettings["UnitName"]; }
		}
		/// <summary>
		/// 单位No
		/// </summary>
		public static string UnitNo
		{
			get{ return AppSettings["UnitNo"]; }
		}
		public static string UnitNoInHisSystem
		{
			get{ return AppSettings["UnitNoInHisSystem"]; }
		}
		public static decimal QZD_YYS
		{
			get
			{ 
				string sql="SELECT VAL FROM SYS_Config WHERE PARAKEY='QZD_CGH'";
				return DA.DBAccess.RunSQLReturnValDecimal(sql,3000,2);
			}
		}
		/// <summary>
		/// 文化建设事业费的税目。
		/// </summary>
		public static string SMsOfWHJS
		{
			get
			{ 
				string sql="SELECT VAL FROM SYS_Config WHERE PARAKEY='SMsOfWHJS'";
				//return DA.DBAccess.RunSQLReturnString(sql,"@05610300@05620300@05630300@");
				return DA.DBAccess.RunSQLReturnString(sql);

			}
		}
		/// <summary>
		/// 年度营业税起征点
		/// </summary>
		public static decimal QZD_CGH
		{
			get
			{ 
				string sql="SELECT VAL FROM SYS_Config WHERE PARAKEY='QZD_CGH'";
				return DA.DBAccess.RunSQLReturnValDecimal(sql,3000,2);
			}
		}
        public static string QZD_GGH_TD_HYs
        {
            get
            {
                return null;

                //string sql = "SELECT VAL FROM SYS_Config WHERE PARAKEY='QZD_GGH_HYs'";
                //string s = DA.DBAccess.RunSQLReturnString(sql, null);
                //if (s == null)
                //    throw new Exception("不存在QZD_GGH_HYs ");

                //return s.Replace(" ", "");
            }
        }
		/// <summary>
		/// 年度营业税起征点
		/// </summary>
		public static decimal QZD_GGH
		{
			get
			{
				string sql="SELECT VAL FROM SYS_Config WHERE PARAKEY='QZD_GGH'";
				return DA.DBAccess.RunSQLReturnValDecimal(sql,5000,2);
			}
		}
        /// <summary>
        /// 特定工管户起征点
        /// </summary>
        public static decimal QZD_GGH_TD
        {
            get
            {
                string sql = "SELECT VAL FROM SYS_Config WHERE PARAKEY='QZD_GGH_TD'";
                return DA.DBAccess.RunSQLReturnValDecimal(sql, 3000, 2);
            }
        }
		/// <summary>
		/// 是否联机工作
		/// </summary>
		public static bool IsWorkOn
		{
			get
			{
				if ( AppSettings["IsWorkOn"]=="0")
					return false;
				else
					return true;
			}
		}
        public static bool IsAutoGenerCHOfNode
		{
            get
            {
                if (AppSettings["IsAutoGenerCHOfNode"] == null
                    || AppSettings["IsAutoGenerCHOfNode"] == "0")
                    return false;
                else
                    return true;
            }
		}
		public static bool IsAutoDTSWork
		{
			get
			{
				if ( AppSettings["IsAutoDTSWork"]=="0")
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// 部门编号(部门编号.)
		/// </summary>
		public static string FK_Dept
		{
			get
			{ 
				if (AppSettings["FK_Dept"]!=null)
					return AppSettings["FK_Dept"];
				else
					return "zsjg";
			}
		}
	}
}
