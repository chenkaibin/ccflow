using System;
using System.Data ; 
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;

namespace BP.DA
{
	/// <summary>
	/// SysConfig 的摘要说明。
	/// </summary>
	public class AppConfig
	{
		/// <summary>
		///取得配置NestedNamesSection内的相应key的内容
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static NameValueCollection GetConfig(string key)
		{
            try
            {
                //  System.Configuration.GetSection
                //Hashtable ht = (Hashtable)ConfigurationSettings.GetConfig("NestedNamesSection");
                Hashtable ht = (Hashtable)System.Configuration.ConfigurationManager.GetSection("NestedNamesSection");
                return (NameValueCollection)ht[key];
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}
		
		 
	}
}
