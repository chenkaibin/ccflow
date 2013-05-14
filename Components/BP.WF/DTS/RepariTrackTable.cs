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
    /// 修复节点表单map 的摘要说明
    /// </summary>
    public class RepariTrackTable : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public RepariTrackTable()
        {
            this.Title = "修复所有流程的NDxxTrack表.";
            this.Help = "修复所有流程的,没有创建的会自动加入.";
         
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
            return "全部执行成功.";
        }
    }
}
