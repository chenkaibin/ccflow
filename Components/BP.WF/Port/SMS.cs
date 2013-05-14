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
	/// ��Ϣ״̬
	/// </summary>
    public enum MsgSta
    {
        /// <summary>
        /// δ��ʼ
        /// </summary>
        UnRun,
        /// <summary>
        /// �ɹ�
        /// </summary>
        RunOK,
        /// <summary>
        /// ʧ��
        /// </summary>
        RunError
    }
	/// <summary>
	/// ��Ϣ����
	/// </summary>
	public class SMSAttr:EntityMyPKAttr
	{
        /// <summary>
        /// ��Ϣ��ǣ��д˱�ǵĲ��ڷ��ͣ�
        /// </summary>
        public const string MsgFlag = "MsgFlag";
        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public const string MsgType = "MsgType";
		/// <summary>
		/// ״̬ 0 δ���ͣ� 1 ���ͳɹ���2����ʧ��.
		/// </summary>
		public const string MsgSta="MsgSta";
        /// <summary>
        /// ��Ϣ��������
        /// </summary>
        public const string MsgAccepter = "MsgAccepter";
        /// <summary>
        /// ����
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// ����
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// ������
        /// </summary>
        public const string Sender = "Sender";
        /// <summary>
        /// ���͸�
        /// </summary>
        public const string SendToEmpID = "SendToEmpID";
        /// <summary>
        /// ��������
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// ��������
        /// </summary>
        public const string SendDT = "SendDT";
	}
	/// <summary>
	/// ��Ϣ
	/// </summary> 
    public class SMS : EntityMyPK
    {
        #region �·��� 2013 
        /// <summary>
        /// �����ֻ���Ϣ
        /// </summary>
        /// <param name="tel">�ֻ���</param>
        /// <param name="doc">�ֻ�����</param>
        public static void SendSMS(string tel, string doc)
        {
            // �����������Ϣ����.
            if (BP.WF.Glo.IsEnableSysMessage == false)
                return;

            SMS sms = new SMS();
            sms.MyPK = DBAccess.GenerGUID();
            sms.HisMsgSta = MsgSta.UnRun;
            sms.MsgAccepter = tel;
            sms.Title = doc;
            sms.Sender = BP.Web.WebUser.No;
            sms.RDT = BP.DA.DataType.CurrentDataTime;

            sms.MsgType = 1;  // 0 �ʼ� 1�����š�

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
        /// �����ʼ���Ϣ
        /// </summary>
        /// <param name="email">�ʼ���ַ</param>
        /// <param name="mailTitle">�ʼ�����</param>
        /// <param name="mailDoc">��������</param>
        /// <param name="msgFlag">��־</param>
        public static void SendEmail(string email, string mailTitle, string mailDoc, string msgFlag)
        {
            // �����������Ϣ����.
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
            
            sms.MsgFlag = msgFlag; // ��Ϣ��־.
            sms.Insert();
        }
        #endregion �·���

        #region  ����
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
        /// ��Ϣ����
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
        /// ��Ϣ���(�������������ⷢ���ظ�)
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
        /// ������
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
        /// ��¼����
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
        /// ��������
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
        /// ����
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
        /// �ʼ�����
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
        /// �����豸
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

        #region ���캯��
        /// <summary>
        /// UI�����ϵķ��ʿ���
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
        /// ��Ϣ
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
                map.EnDesc = "��Ϣ";

                map.AddMyPK();

                map.AddTBString(SMSAttr.Sender, null, "������", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.SendToEmpID, null, "���͸�EmpID", false, true, 0, 200, 20);
                map.AddTBDateTime(SMSAttr.RDT, "д��ʱ��", true, false);
                
                map.AddTBInt(SMSAttr.MsgSta, (int)MsgSta.UnRun, "��Ϣ״̬", true, true);
                map.AddTBInt(SMSAttr.MsgType, 0, "��Ϣ����(0�ʼ�1�ֻ�2����)", true, true);

                map.AddTBString(SMSAttr.MsgFlag, null, "��Ϣ���(���ڷ�ֹ�����ظ�)", false, true, 0, 200, 20);

                map.AddTBString(SMSAttr.MsgAccepter, null, "�����豸", false, true, 0, 200, 20);
                map.AddTBString(SMSAttr.Title, null, "����", false, true, 0, 3000, 20);
                map.AddTBStringDoc(SMSAttr.Doc, null, "����", false, true);
                map.AddTBDateTime(SMSAttr.SendDT,null, "����ʱ��", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        /// <summary>
        /// �����ʼ�
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
            myEmail.SubjectEncoding = System.Text.Encoding.UTF8;//�ʼ��������

            myEmail.Body = mailDoc;
            myEmail.BodyEncoding = System.Text.Encoding.UTF8;//�ʼ����ݱ���
            myEmail.IsBodyHtml = true;//�Ƿ���HTML�ʼ�

            myEmail.Priority = MailPriority.High;//�ʼ����ȼ�

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(SystemConfig.GetValByKey("SendEmailAddress", "ccflow.cn@gmail.com"),
                SystemConfig.GetValByKey("SendEmailPass", "ccflow123"));
            //����д������������
            client.Port = SystemConfig.GetValByKeyInt("SendEmailPort", 587); //ʹ�õĶ˿�
            client.Host = SystemConfig.GetValByKey("SendEmailHost", "smtp.gmail.com");

            // ����ssl����. 
            if (SystemConfig.GetValByKeyInt("SendEmailEnableSsl", 1) == 1)
                client.EnableSsl = true;  //����ssl����.
            else
                client.EnableSsl = false; //����ssl����.


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
	/// ��Ϣs
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
 