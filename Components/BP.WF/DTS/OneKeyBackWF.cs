using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class OneKeyBackWF : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public OneKeyBackWF()
        {
            this.Title = "一键备份流程与表单。";
            this.Help = "把流程与表单都生成xml文档备份到C:\\CCFlowTemplete下面。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
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
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string path = "C:\\CCFlowTemplete" + DateTime.Now.ToString("yy年MM月dd日HH时mm分ss秒");
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                FlowSort fs = new FlowSort(fl.FK_FlowSort);
                string pathDir = path + "\\Flow\\" + fs.No + "." + fs.Name;
                if (System.IO.Directory.Exists(pathDir) == false)
                    System.IO.Directory.CreateDirectory(pathDir);

              //  fl.GenerFlowXmlTemplete(pathDir + "\\" + fl.Name + ".xml");
                fl.GenerFlowXmlTemplete(pathDir);
            }

            MapDatas mds = new MapDatas();
            mds.RetrieveAllFromDBSource();
            foreach (MapData md in mds)
            {
                if (md.FK_FrmSort.Length < 2)
                    continue;

                FrmSort fs = new FrmSort(md.FK_FrmSort);
                string pathDir = path + "\\Form\\" + fs.No + "." + fs.Name;
                if (System.IO.Directory.Exists(pathDir) == false)
                    System.IO.Directory.CreateDirectory(pathDir);
                DataSet ds = md.GenerHisDataSet();
                ds.WriteXml(pathDir + "\\" + md.Name + ".xml");
            }
            return "执行成功,存放路径:" + path;
        }
    }
}
