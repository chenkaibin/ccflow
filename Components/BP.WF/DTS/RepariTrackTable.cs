using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.Sys;
using BP.En;
namespace BP.WF
{
    /// <summary>
    /// �޸��ڵ��map ��ժҪ˵��
    /// </summary>
    public class RepariTrackTable : Method
    {
        /// <summary>
        /// �����в����ķ���
        /// </summary>
        public RepariTrackTable()
        {
            this.Title = "�޸��������̵�NDxxTrack��.";
            this.Help = "�޸��������̵�,û�д����Ļ��Զ�����.";
         
        }
        /// <summary>
        /// ����ִ�б���
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "��ȷ��Ҫִ����";
            //HisAttrs.AddTBString("P1", null, "ԭ����", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "������", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "ȷ��", true, false, 0, 10, 10);
        }
        /// <summary>
        /// ��ǰ�Ĳ���Ա�Ƿ����ִ���������
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// ִ��
        /// </summary>
        /// <returns>����ִ�н��</returns>
        public override object Do()
        {
            Flows fls = new Flows();
            fls.RetrieveAll();
            foreach (Flow fl in fls)
            {
                try
                {
                    string ptable = "ND" + int.Parse(fl.No) + "Track";
                    DBAccess.RunSQL("DROP TABLE " + ptable);
                }
                catch
                {
                }
                Track.CreateTrackTable(fl.No);
            }
            return "ȫ��ִ�гɹ�.";
        }
    }
}
