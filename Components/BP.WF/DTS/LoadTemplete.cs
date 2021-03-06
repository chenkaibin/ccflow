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
    /// Method 的摘要说明
    /// </summary>
    public class LoadTemplete : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public LoadTemplete()
        {
            this.Title = "装载流程演示模板";
            this.Help = "为了帮助各位爱好者学习与掌握ccflow, 特提供一些流程模板与表单模板以方便学习。";
            this.Help += "@这些模板的位于" + SystemConfig.PathOfData + "\\FlowDemo\\";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
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
                    msg += "@开始调度流程模板文件:" + filePath;
                    Flow myflow = BP.WF.Flow.DoLoadFlowTemplate(fs.No, filePath, ImpFlowTempleteModel.AsTempleteFlowNo);
                    msg += "@流程:" + myflow.Name + "装载成功。";

                    System.IO.FileInfo info = new System.IO.FileInfo(filePath);
                    myflow.Name = info.Name.Replace(".xml", "");
                    if (myflow.Name.Substring(2, 1) == ".")
                        myflow.Name = myflow.Name.Substring(3);
                    myflow.DirectUpdate();
                }
            }

            // 调度表单文件。
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
                        msg += "@开始调度表单模板文件:" + f;
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
                        msg += "@调度失败" + ex.Message;
                    }
                }
            }

            BP.DA.Log.DefaultLogWriteLineInfo(msg);
            return msg;
        }
    }
}
