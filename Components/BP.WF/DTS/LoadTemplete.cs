using System;
using System.IO;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// Method ��ժҪ˵��
    /// </summary>
    public class LoadTemplete : Method
    {
        /// <summary>
        /// �����в����ķ���
        /// </summary>
        public LoadTemplete()
        {
            this.Title = "װ��������ʾģ��";
            this.Help = "Ϊ�˰�����λ������ѧϰ������ccflow, ���ṩһЩ����ģ�������ģ���Է���ѧϰ��";
            this.Help += "@��Щģ���λ��" + SystemConfig.PathOfData + "\\FlowDemo\\";
        }
        /// <summary>
        /// ����ִ�б���
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
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
        public override object Do()
        {
            string msg = "";
            FlowSorts sorts = new FlowSorts();
            sorts.ClearTable();

            DirectoryInfo dirInfo = new DirectoryInfo(SystemConfig.PathOfData + "\\FlowDemo\\Flow\\");
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                if (dir.FullName.Contains(".svn"))
                    continue;

                string[] fls = System.IO.Directory.GetFiles(dir.FullName);
                if (fls.Length == 0)
                    continue;

                FlowSort fs = new FlowSort();
                fs.No = dir.Name.Substring(0, 2);
                fs.Name = dir.Name.Substring(3);
                fs.Insert();
                foreach (string filePath in fls)
                {
                    msg += "@��ʼ��������ģ���ļ�:" + filePath;
                    Flow myflow = BP.WF.Flow.DoLoadFlowTemplate(fs.No, filePath, ImpFlowTempleteModel.AsTempleteFlowNo);
                    msg += "@����:" + myflow.Name + "װ�سɹ���";

                    System.IO.FileInfo info = new System.IO.FileInfo(filePath);
                    myflow.Name = info.Name.Replace(".xml", "");
                    if (myflow.Name.Substring(2, 1) == ".")
                        myflow.Name = myflow.Name.Substring(3);
                    myflow.DirectUpdate();
                }
            }

            // ���ȱ����ļ���
            FrmSorts fss = new FrmSorts();
            fss.ClearTable();

            string frmPath = SystemConfig.PathOfData + "\\FlowDemo\\Form\\";
            dirInfo = new DirectoryInfo(frmPath);
            dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo item in dirs)
            {
                if (item.FullName.Contains(".svn"))
                    continue;

                string[] fls = System.IO.Directory.GetFiles(item.FullName);
                if (fls.Length == 0)
                    continue;
                FrmSort fs = new FrmSort();
                fs.No = item.Name.Substring(0, 2);
                fs.Name = item.Name.Substring(3);
                fs.Insert();

                foreach (string f in fls)
                {
                    try
                    {
                        msg += "@��ʼ���ȱ���ģ���ļ�:" + f;
                        System.IO.FileInfo info = new System.IO.FileInfo(f);
                        if (info.Extension != ".xml")
                            continue;

                        DataSet ds = new DataSet();
                        ds.ReadXml(f);

                        MapData md = MapData.ImpMapData(ds,false);
                        md.FK_FrmSort = fs.No;
                        md.Update();
                    }
                    catch (Exception ex)
                    {
                        msg += "@����ʧ��" + ex.Message;
                    }
                }
            }

            BP.DA.Log.DefaultLogWriteLineInfo(msg);
            return msg;
        }
    }
}