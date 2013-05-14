using System;
using System.Collections.Generic;
using System.Text;
using BP.Port;

namespace BP.WF
{
    public class Glo
    {
        public static FtpSupport.FtpConnection HisFtpConn
        {
            get
            {
                try
                {
                    return new FtpSupport.FtpConnection(Glo.FtpIP, Glo.FtpUser, Glo.FtpPass);
                }
                catch (Exception ex)
                {
                    throw new Exception("@有可能ftp密码与用户名错误，请检查。 @异常信息:" + ex.Message);
                }
            }
        }
        public static string FtpIP { private get; set; }
        public static string FtpUser { private get; set; }
        public static string FtpPass { private get; set; }
        /// <summary>
        /// 临时文件
        /// </summary>
        public static string PathOfTInstall = @"C:\\WF\\";
        /// <summary>
        /// 个人文件
        /// </summary>
        public static string Profile
        {
            get
            {
                return BP.WF.Glo.PathOfTInstall + "\\Profile.txt";
            }
        }
        public static string ProfileLogin
        {
            get
            {
                return BP.WF.Glo.PathOfTInstall + "\\Login.txt";
            }
        }
        /// <summary>
        /// 流程服务器的位置
        /// </summary>
        public static string WFServ = "http://127.0.0.1/Flow/";
        public static string DoStartFlow_del
        {
            get
            {
                return Glo.WFServ + "Port.aspx?FK_Flow=" + WebUser.FK_Flow + "&FK_Node=" + WebUser.FK_Node + "&WorkID=" + WebUser.WorkID + "&DoType=EmpWorks";
            }
        }
    }
}
