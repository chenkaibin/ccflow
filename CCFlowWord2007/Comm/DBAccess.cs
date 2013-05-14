using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CCFlowWord2007;
using CCFlowWord2007.ServiceReference1;


namespace BP.DA
{
    /// <summary>
    /// 数据库连接
    /// 2011.11.09 Liuxc
    /// </summary>
    public static class DBAccess
    {
        static DBAccess()
        {
            client = new DocFlowSoapClient();
        }

        private static DocFlowSoapClient client;

        #region Methods

        /// <summary>
        /// 获取网站配置项
        /// </summary>
        /// <param name="key">配置项名称</param>
        /// <returns></returns>
        public static string GetWebConfigByKey(string key)
        {
            return client.GetSettingByKey(key);
        }

        /// <summary>
        /// 运行SQL语句，返回int数字
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static int RunSQL(string sql)
        {
            return client.RunSQL(sql);
        }

        /// <summary>
        /// 运行SQL语句，返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static DataTable RunSQLReturnTable(string sql)
        {
            return client.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 运行SQL语句，返回string字符串
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public static string RunSQLReturnString(string sql)
        {
            return client.RunSQLReturnString(sql);
        }

        /// <summary>
        /// 获取一个ID
        /// </summary>
        /// <returns></returns>
        public static int GenerOID()
        {
            return client.GenerOID();
        }

        #endregion
    }
}
