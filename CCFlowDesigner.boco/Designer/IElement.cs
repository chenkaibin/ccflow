using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silverlight;
using WF.WS;
using BP;

namespace BP
{
    /// <summary>
    /// 节点位置
    /// </summary>
    public enum NodePosType
    {
        Start,
        Mid,
        End
    }
    public enum MergePictureRepeatDirection
    {
        Vertical = 0,
        Horizontal,
        None
    }
    /// <summary>
    /// 节点类型
    /// </summary>
    public enum FlowNodeType_del
    {
        INITIAL,
        INTERACTION,
        COMPLETION,
        //AND_MERGE, ccflow 没有这些类型。
        //AND_BRANCH,
        STATIONODE,
        AUTOMATION,
        DUMMY,
        //OR_BRANCH,
        //OR_MERGE,
        SUBPROCESS,
        VOTE_MERGE,
    }
    public enum FlowNodeType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL,
        /// <summary>
        /// 子线程.
        /// </summary>
        SubThread,
        /// <summary>
        /// 虚节点
        /// </summary>
        VirtualStart,
        VirtualEnd
        //INITIAL,
        //INTERACTION,
        //COMPLETION,
        ////AND_MERGE, ccflow 没有这些类型。
        ////AND_BRANCH,
        //STATIONODE,
        //AUTOMATION,
        //DUMMY,
        ////OR_BRANCH,
        ////OR_MERGE,
        //SUBPROCESS,
        //VOTE_MERGE,
    }
    /// <summary>
    /// 运行模式
    /// </summary>
    public enum RunModel
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 0,
        /// <summary>
        /// 合流
        /// </summary>
        HL = 1,
        /// <summary>
        /// 分流
        /// </summary>
        FL = 2,
        /// <summary>
        /// 分合流
        /// </summary>
        FHL,
        /// <summary>
        /// 子线程.
        /// </summary>
        SubThread
    }
    /// <summary>
    /// 节点位置类型
    /// </summary>
    public enum FlowNodePosType
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        Start,
        /// <summary>
        /// 中间点
        /// </summary>
        Mid,
        /// <summary>
        /// 结束点
        /// </summary>
        End
    }
    public enum WorkFlowElementType
    {
        FlowNode = 0,
        Direction,
        Label
    }

    public enum PageEditType
    {
        Add = 0,
        Modify,
        None
    }

    public enum DirectionLineType
    {
        Line = 0,
        Polyline
    }

    public enum HistoryType
    {
        New,
        Next,
        Previous
    } ;

    public class CheckResult
    {
        private bool isPass = true;

        public bool IsPass
        {
            get { return isPass; }
            set { isPass = value; }
        }

        private string message = "";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class UserStation
    {
        private bool isPass = true;

        public bool IsPass
        {
            get { return isPass; }
            set { isPass = value; }
        }

        private bool isSel = true;

        public bool IsSel
        {
            get { return isSel; }
            set { isSel = value; }
        }

        private string message = "";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public interface IElement
    {
        CheckResult CheckSave();
        UserStation Station();
        string ToXmlString();
        void LoadFromXmlString(string xmlString);
        void ShowMessage(string message);
        WorkFlowElementType ElementType { get; }
        PageEditType EditType { get; set; }
        bool IsSelectd { get; set; }
        IContainer Container { get; set; }
        void Delete();
        void UpperZIndex();
        bool IsDeleted { get; }
        void Zoom(double zoomDeep);
        void Edit();
        
    }
}