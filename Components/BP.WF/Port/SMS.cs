using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Port;
using System.Net.Mail;

namespace BP.Sys
{
	/// <summary>
	/// 消息状态
	/// </summary>
    public enum MsgSta
    {
        /// <summary>
        /// 未开始
        /// </summary>
        UnRun,
        /// <summary>
        /// 成功
        /// </summary>
        RunOK,
        /// <summary>
        /// 失败
        /// </summary>
        RunError
    }
	/// <summary>
	/// 消息属性
	/// </summary>
	public class SMSAttr:EntityMyPKAttr
	{
        /// <summary>
        /// 消息标记（有此标记的不在发送）
        /// </summary>
        public const string MsgFlag = "MsgFlag";
        /// <summary>
        /// 信息类型
        /// </summary>
        public const string MsgType = "MsgType";
		/// <summary>
		/// 状态 0 未发送， 1 发送成功，2发送失败.
		/// </summary>
		public const string MsgSta="MsgSta";
        /// <summary>
        /// 信息接受载体
        /// </summary>
        public const string MsgAccepter = "MsgAccepter";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// 内容
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// 发送给
        /// </summary>
        public const string SendToEmpID = "SendToEmpID";
        /// <summary>
        /// 插入日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 发送日期
        /// </summary>
        public const string SendDT = "SendDT";
	}
	/// <summary>
	/// 消息
	/// </summary> 
    public class SMS : EntityMyPK
    {
        #region 新方法 2013 
        /// <summary>
        /// 发送手机消息
        /// </summary>
        /// <param name="tel">手机号</param>
        /// <param name="doc">手机内容</param>
        public static void SendSMS(string tel, string doc)
        {
            // 如果不启用消息机制.
            if (BP.WF.Glo.IsEnableSysMessage == false)
                return;

            SMS sms = new SMS();
            sms.MyPK = DBAccess.GenerGUID();
            sms.HisMsgSta = MsgSta.UnRun;
            sms.MsgAccepter = tel;
            sms.Title = doc;
            sms.Sender = BP.Web.WebUser.No;
            sms.RDT = BP.DA.DataType.CurrentDataTime;

            sms.MsgType = 1;  // 0 邮件 1，短信。

            try
            {
                sms.Insert();
            }
            catch
            {
                sms.CheckPhysicsTable();
                sms.Insert();
            }
        }
        /// <summary>
        /// 发送邮件消息
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="mailTitle">邮件标题</param>
        /// <param name="mailDoc">发送内容</param>
        /// <param name="msgFlag">标志</param>
        public static void SendEmail(string email, string mailTitle, string mailDoc, string msgFlag)
        {
            // 如果不启用消息机制.
            if (BP.WF.Glo.IsEnableSysMessage == false)
                return;

            SMS sms = new SMS();
            sms.CheckPhysicsTable();

            sms.MyPK = DBAccess.GenerGUID();
            sms.HisMsgSta = MsgSta.UnRun;

            sms.MsgAccepter = email;
            sms.Title = mailTitle;
            sms.Doc = mailDoc;

            sms.Sender = BP.Web.WebUser.No;
            sms.RDT = BP.DA.DataType.CurrentDataTime;
            
            sms.MsgFlag = msgFlag; // 消息标志.
            sms.Insert();
        }
        #endregion 新方法

        #region  属性
        public MsgSta HisMsgSta
        {
            get
            {
                return (MsgSta)this.GetValIntByKey(SMSAttr.MsgSta);
            }
            set
            {
                this.SetValByKey(SMSAttr.MsgSta, (int)value);
            }
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MsgType
        {
            get
            {
                return this.GetValIntByKey(SMSAttr.MsgType);
            }
            set
            {
                SetValByKey(SMSAttr.MsgType, value);
            }
        }
        public string SendToEmpID
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.SendToEmpID);
            }
            set
            {
                SetValByKey(SMSAttr.SendToEmpID, value);
            }
        }
        
        /// <summary>
        /// 消息标记(可以用它来避免发送重复)
        /// </summary>
        public string MsgFlag
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.MsgFlag);
            }
            set
            {
                SetValByKey(SMSAttr.MsgFlag, value);
            }
        }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.Sender);
            }
            set
            {
                SetValByKey(SMSAttr.Sender, value);
            }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.RDT);
            }
            set
            {
                SetValByKey(SMSAttr.RDT, value);
            }
        }
        /// <summary>
        /// 发送日期
        /// </summary>
        public string SendDT
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.SendDT);
            }
            set
            {
                SetValByKey(SMSAttr.SendDT, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.Title);
            }
            set
            {
                SetValByKey(SMSAttr.Title, value);
            }
        }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Doc
        {
            get
            {
                string doc = this.GetValStringByKey(SMSAttr.Doc);
                if (string.IsNullOrEmpty(doc))
                    return this.Title;
                return doc.Replace('~', '\'');
            }
            set
            {
                SetValByKey(SMSAttr.Doc, value);
            }
        }
        /// <summary>
        /// 接受设备
        /// </summary>
        public string MsgAccepter
        {
            get
            {
                return this.GetValStringByKey(SMSAttr.MsgAccepter);
            }
            set
            {
                SetValByKey(SMSAttr.MsgAccepter, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        /// 消息
        /// </summary>
        public SMS()
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_SMS");
                map.EnDesc = "消息";

                map.AddMyPK();

                map.AddTBString(SMSAttr.Sender, null, "发送人", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.SendToEmpID, null, "发送给EmpID", false, true, 0, 200, 20);
                map.AddTBDateTime(SMSAttr.RDT, "写入时间", true, false);
                
                map.AddTBInt(SMSAttr.MsgSta, (int)MsgSta.UnRun, "消息状态", true, true);
                map.AddTBInt(SMSAttr.MsgType, 0, "消息类型(0邮件1手机2其它)", true, true);

                map.AddTBString(SMSAttr.MsgFlag, null, "消息标记(用于防止发送重复)", false, true, 0, 200, 20);

                map.AddTBString(SMSAttr.MsgAccepter, null, "接受设备", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.Title, null, "标题", false, true, 0, 3000, 20);
                map.AddTBStringDoc(SMSAttr.Doc, null, "内容", false, true);
                map.AddTBDateTime(SMSAttr.SendDT,null, "发送时间", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailDoc"></param>
        /// <returns></returns>
        public static bool SendEmailNow(string mail, string mailTitle, string mailDoc)
        {
            System.Net.Mail.MailMessage myEmail = new System.Net.Mail.MailMessage();
            myEmail.From = new System.Net.Mail.MailAddress("ccflow.cn@gmail.com", "ccflow", System.Text.Encoding.UTF8);

            myEmail.To.Add(mail);
            myEmail.Subject = mailTitle;
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码

            myEmail.Body = mailDoc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码
            myEmail.IsBodyHtml = true;//是否是HTML邮件

            myEmail.Priority = MailPriority.High;//邮件优先级

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(SystemConfig.GetValByKey("SendEmailAddress", "ccflow.cn@gmail.com"),
                SystemConfig.GetValByKey("SendEmailPass", "ccflow123"));
            //上述写你的邮箱和密码
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 587); //使用的端口
            client.Host = SystemConfig.GetValByKey("SendEmailHost", "smtp.gmail.com");

            // 经过ssl加密. 
            if (SystemConfig.GetValByKeyInt("SendEmailEnableSsl", 1) == 1)
                client.EnableSsl = true;  //经过ssl加密.
            else
                client.EnableSsl = false; //经过ssl加密.


            try
            {
                object userState = myEmail;
                client.SendAsync(myEmail, userState);
                return true;
            }
            catch
            {
                return false;
            }
        }
         
    }
	/// <summary>
	/// 消息s
	/// </summary> 
    public class SMSs : Entities
    {
        public override Entity GetNewEntity
        {
            get
            {
                return new SMS();
            }
        }
        public SMSs()
        {
        }
    }
}
 