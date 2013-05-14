using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.IO;
using BP;
using Ccflow.Web.Component.Workflow;
using WF;
using WF.WS;
using Liquid;
using System.ServiceModel;
using Silverlight;
using BP.Controls;
using System.Windows.Browser;
using WF.Designer;
using BP;
using BP.DA;

namespace BP
{
    public partial class Container : UserControl, IContainer
    {

        #region 变量
        /// <summary>
        /// 结点ID
        /// </summary>
        public int NodeID;
        /// <summary>
        /// 服务
        /// </summary>
        WSDesignerSoapClient _service = Glo.GetDesignerServiceInstance();//new BasicHttpBinding(), address);
        /// <summary>
        /// 页面编辑类型
        /// </summary>
        PageEditType editType = PageEditType.None;
        /// <summary>
        /// 选中结点
        /// </summary>
        FlowNode f = null;

        /// <summary>
        /// 新建结点编号
        /// </summary>
        int nextNewFlowNodeIndex = 0;
        /// <summary>
        /// 新建连接线编号 
        /// </summary>
        int nextNewDirectionIndex = 0;
        /// <summary>
        /// 新建标签编号
        /// </summary>
        int nextNewLabelIndex = 0;
        /// <summary>
        /// 结点集合
        /// </summary>
        public List<FlowNode> flowNodeCollections;
        /// <summary>
        /// 连接线集合
        /// </summary>
        public List<Direction> directionCollections;
        /// <summary>
        /// 标签集合
        /// </summary>
        public List<NodeLabel> lableCollections;
        /// <summary>
        /// 
        /// </summary>
        int nextMaxIndex = 0;
        Canvas _gridLinesContainer;
        Stack<string> _workFlowXmlNextStack;
        Stack<string> _workFlowXmlPreStack;
        Dictionary<string, FlowNode> dicFlowNode = new Dictionary<string, FlowNode>();
        string workflowXmlCurrent = @"";

        Point mousePosition;
        bool trackingMouseMove = false;
        System.Windows.Threading.DispatcherTimer _doubleClickTimer;
        Rectangle temproaryEllipse;
        NodeLabel l = null;

        List<System.Windows.Controls.Control> copyElementCollectionInMemory;
        /// <summary>
        /// 弹出窗口标题
        /// </summary>
        string WinTitle;

        Point pos;
        #endregion

        #region 属性
        /// <summary>
        /// 当前是否为添加回退连线状态
        /// </summary>
        public bool IsReturnTypeDir { get; set; }

        /// <summary>
        /// 当前是否有节点在编辑状态
        /// </summary>
        public bool IsSomeChildEditing { get; set; }
        /// <summary>
        /// 流程当前是否保存
        /// </summary>
        private bool isNeedSave;
        public bool IsNeedSave
        {
            get { return isNeedSave; }
            set
            {
                isNeedSave = value;
                //如果需要保存变化时，将流程名称加粗并在前面加一个星号，反之则恢复正常模式
                if (null != this.Designer)
                {
                    var currentItem = this.Designer.tbDesigner.SelectedItem as TabItemEx;
                    if (null != currentItem)
                    {
                        var txtTitle = ((Panel)(currentItem.Header)).Children[1] as TextBlock;
                        if (null != txtTitle)
                        {
                            if (isNeedSave && !txtTitle.Text.StartsWith("*"))
                            {
                                txtTitle.Text = "*" + txtTitle.Text;
                                txtTitle.FontWeight = FontWeights.Bold;
                            }
                            if (!isNeedSave)
                            {
                                txtTitle.Text = txtTitle.Text.Trim('*');
                                txtTitle.FontWeight = FontWeights.Normal;
                            }

                        }
                    }
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.FlowID;
            }
        }
        public string FlowID
        {
            get;
            set;
        }
        public string WorkID
        {
            get { return ""; }
        }
        public bool IsContainerRefresh { get; set; }
        public bool MouseIsInContainer { get; set; }
        public int NextNewFlowNodeIndex
        {
            get
            {
                int max = 0;
                foreach (UIElement c in cnsDesignerContainer.Children)
                {
                    var ele = c as IElement;
                    if (ele != null)
                    {
                        if (ele.IsDeleted)
                            continue;

                        if (ele.ElementType == WorkFlowElementType.FlowNode)
                        {
                            var n = ele as FlowNode;
                            if (n != null && !string.IsNullOrEmpty(n.NodeName))
                            {
                                int index = 0;
                                int.TryParse(n.NodeName.Substring(n.NodeName.Length - 1, 1), out index);
                                if (index > max)
                                {
                                    max = index;
                                }
                            }
                        }
                    }
                }
                return max + 1;
            }
        }
        public int NextNewDirectionIndex
        {
            get
            {
                return ++nextNewDirectionIndex;
            }
        }
        public int NextNewLabelIndex
        {
            get
            {
                int max = 0;
                foreach (UIElement c in cnsDesignerContainer.Children)
                {
                    var ele = c as IElement;
                    if (ele != null)
                    {
                        if (ele.IsDeleted)
                            continue;

                        if (ele.ElementType == WorkFlowElementType.Label)
                        {
                            NodeLabel l = ele as NodeLabel;
                            if (l != null && !string.IsNullOrEmpty(l.LabelName))
                            {
                                int index = 0;
                                int.TryParse(l.LabelName.Substring(l.LabelName.Length - 1, 1), out index);
                                if (index > max)
                                {
                                    max = index;
                                }
                            }
                        }
                    }
                }
                return max + 1;
            }
        }
        public Double ContainerWidth
        {
            get
            {
                return cnsDesignerContainer.Width;
            }
            set
            {
                cnsDesignerContainer.Width = value;
                //sliWidth.Value = value;
            }
        }

        public Double ContainerHeight
        {
            get
            {
                return cnsDesignerContainer.Height;
            }
            set
            {
                cnsDesignerContainer.Height = value;
                //sliHeight.Value = value;

            }
        }

        public Double ScrollViewerHorizontalOffset
        {
            get
            {
                return svContainer.HorizontalOffset;
            }
            set
            {
                svContainer.ScrollToHorizontalOffset(value);

            }
        }

        public Double ScrollViewerVerticalOffset
        {
            get
            {
                return svContainer.VerticalOffset;
            }
            set
            {
                svContainer.ScrollToVerticalOffset(value);
            }
        }

        public WSDesignerSoapClient _Service
        {
            get { return _service; }
            set { _service = value; }
        }

        public List<FlowNode> FlowNodeCollections
        {
            get
            {
                if (flowNodeCollections == null)
                {
                    flowNodeCollections = new List<FlowNode>();
                }
                return flowNodeCollections;
            }
        }

        public List<Direction> DirectionCollections
        {
            get
            {
                if (directionCollections == null)
                {
                    directionCollections = new List<Direction>();
                }
                return directionCollections;
            }
        }

        public List<NodeLabel> LableCollections
        {
            get
            {
                if (lableCollections == null)
                {
                    lableCollections = new List<NodeLabel>();
                }
                return lableCollections;
            }
        }

        /// <summary>
        /// 对象顺序
        /// </summary>
        public int NextMaxIndex
        {
            get
            {
                nextMaxIndex++;
                return nextMaxIndex;
            }
        }
        /// <summary>
        /// 左边界
        /// </summary>
        public double Left
        {
            get
            {
                return 230;
            }
        }
        /// <summary>
        /// 右边界
        /// </summary>
        public double Top
        {
            get
            {
                return 100;
            }

        }
        /// <summary>
        /// 新建结点名字
        /// </summary>
        public string NewNodeName
        {
            get;
            set;
        }
        public Stack<string> WorkFlowXmlNextStack
        {
            get
            {
                if (_workFlowXmlNextStack == null)
                    _workFlowXmlNextStack = new Stack<string>(50);
                return _workFlowXmlNextStack;
            }
        }

        public Stack<string> WorkFlowXmlPreStack
        {
            get
            {
                if (_workFlowXmlPreStack == null)
                    _workFlowXmlPreStack = new Stack<string>(50);
                return _workFlowXmlPreStack;
            }
        }
        int MoveStepLenght
        {
            get
            {
                if (CtrlKeyIsPress)
                    return 5;
                return 1;
            }
        }
        public Direction CurrentTemporaryDirection { get; set; }
        public List<System.Windows.Controls.Control> CurrentSelectedControlCollection
        {
            get
            {
                if (_currentSelectedControlCollection == null)
                    _currentSelectedControlCollection = new List<System.Windows.Controls.Control>();
                return _currentSelectedControlCollection;

            }
        }
        List<System.Windows.Controls.Control> _currentSelectedControlCollection;
        public bool CtrlKeyIsPress
        {
            get
            {
                return (Keyboard.Modifiers == ModifierKeys.Control);
                //return ctrlKeyIsPress;
            }
        }
        public bool IsMouseSelecting
        {
            get
            {
                return (temproaryEllipse != null);
            }
        }

        public List<System.Windows.Controls.Control> CopyElementCollectionInMemory
        {
            get
            {
                if (copyElementCollectionInMemory == null)
                    copyElementCollectionInMemory = new List<System.Windows.Controls.Control>();
                return copyElementCollectionInMemory;
            }
            set
            {
                copyElementCollectionInMemory = value;
            }
        }
        public PageEditType EditType
        {
            get
            {
                if (editType == PageEditType.None)
                {
                    editType = PageEditType.Add;
                }
                return editType;
            }
            set
            {
                editType = value;
            }
        }
        public MainPage Designer { get; set; }
        private Point GetPosition { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 设置设计器栏
        /// </summary>
        public void SetGridLines()
        {
            //if (!cbShowGridLines.IsChecked.HasValue || !cbShowGridLines.IsChecked.Value)
            //    return;
            GridLinesContainer.Children.Clear();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(255, 160, 160, 160);
            //  brush.Color = Color.FromArgb(255, 255, 255, 255);
            double thickness = 0.3;
            double top = 0;
            double left = 0;

            //  this.Parent.SetValue(canContainerCover DependencyProperty 
            //cnsDesignerContainer.Width = 500;
            //cnsDesignerContainer.Height = 9000;

            double width = cnsDesignerContainer.Width;
            double height = cnsDesignerContainer.Height;

            double stepLength = 40;

            double x, y;
            x = left + stepLength;
            y = top;

            while (x < width + left)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;

                line.Stroke = brush;
                line.StrokeThickness = thickness;
                line.Stretch = Stretch.Fill;
                GridLinesContainer.Children.Add(line);
                x += stepLength;
            }


            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;

                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                line.StrokeThickness = thickness;
                GridLinesContainer.Children.Add(line);
                y += stepLength;
            }
        }

        public bool Contains(UIElement uie)
        {
            return cnsDesignerContainer.Children.Contains(uie);
        }
        public CheckResult CheckSave()
        {
            var cr = new CheckResult();
            cr.IsPass = true;
            return cr;
        }
        public void NewFlow(string flowsort)
        {
            if (cnsDesignerContainer.Children.Count > 0)
            {
                if (HtmlPage.Window.Confirm("确定要保存吗？"))
                {
                }
                else
                {
                    return;
                }
            }
            clearContainer();

            if (flowsort != null)
            {
                _Service.DoAsync("NewFlow", flowsort, true);
            }
            else
            {
                _Service.DoAsync("NewFlow", null, true);
            }
            _Service.DoCompleted += _service_DoCompleted;
        }

        public void DrawFlows(string flowID)
        {
            SetGridLines();
            this.FlowID = flowID;

            WSDesignerSoapClient da = Glo.GetDesignerServiceInstance();
            string sqls = "";
            sqls += "SELECT NodeID,Name,X,Y,NodePosType,HisToNDs, RunModel FROM WF_Node WHERE FK_Flow='" + flowID + "'";
            //sqls += "@SELECT Node,ToNode FROM WF_Direction WHERE Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow='" + flowID + "')";
            sqls += "@SELECT Node,ToNode,DirType,IsCanBack,Dots FROM WF_Direction WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT * FROM WF_LabNote WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT Paras FROM WF_Flow WHERE No='" + flowID + "'";
            da.RunSQLReturnTableSAsync(sqls);
            da.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(da_RunSQLReturnTableSCompleted); 
        }
        void da_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            #region 画节点.
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                FlowNodeType fnt = (FlowNodeType)int.Parse(dr["RunModel"].ToString());

                FlowNode node = new FlowNode((IContainer)this, fnt);
                SolidColorBrush sc = new SolidColorBrush();
                sc.Color = Colors.White;

                NodePosType postype = (NodePosType)int.Parse(dr["NodePosType"].ToString());
                switch (postype)
                {
                    case NodePosType.Start:
                        sc.Color = Colors.Green;
                        break;
                    case NodePosType.End:
                        sc.Color = Colors.Red;
                        break;
                    default:
                        sc.Color = Colors.Black;
                        break;
                }

                node.sdPicture.txtNodeName.Foreground = sc;

                node.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
                node.FK_Flow = FlowID;
                node.NodeID = dr["NodeID"].ToString();
                node.NodeName = dr["Name"].ToString();
                double x = double.Parse(dr["X"]);
                double y = double.Parse(dr["Y"]);
                if (x < 50)
                    x = 50;

                if (y < 30)
                    y = 30;

                if (x > 1190)
                    x = 1190;

                if (y > 770)
                    y = 770;

                node.CenterPoint = new Point(x, y);

                AddFlowNode(node);
            }

            string paras = string.Empty;
            if (ds.Tables[3].Rows.Count > 0)
            {
                paras = ds.Tables[3].Rows[0]["Paras"].ToString();
            }
            if (string.IsNullOrEmpty(paras))
            {
                paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350"; ;
            }

            AtPara ap = new AtPara(paras);
            string startNodeX = ap.GetValStrByKey("StartNodeX");
            string startNodeY = ap.GetValStrByKey("StartNodeY");
            string endNodeX = ap.GetValStrByKey("EndNodeX");
            string endNodeY = ap.GetValStrByKey("EndNodeY");

            //FlowNodeType fnt = (FlowNodeType)int.Parse(dr["RunModel"].ToString());
            FlowNode node0 = new FlowNode((IContainer)this, FlowNodeType.VirtualStart);          
            SolidColorBrush scb = new SolidColorBrush();
            scb.Color = Colors.Black;
            node0.sdPicture.txtNodeName.Foreground = scb;
            node0.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            node0.FK_Flow = FlowID;
            node0.NodeID = "0";
            node0.NodeName = "开始";
            InsertFlowNode(node0, double.Parse(startNodeX), double.Parse(startNodeY));
            
            FlowNode node1 = new FlowNode((IContainer)this, FlowNodeType.VirtualEnd);
            SolidColorBrush scb1 = new SolidColorBrush();
            scb1.Color = Colors.Black;
            node1.sdPicture.txtNodeName.Foreground = scb1;
            node1.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            node1.FK_Flow = FlowID;
            node1.NodeID = "1";
            node1.NodeName = "结束";
            InsertFlowNode(node1, double.Parse(endNodeX), double.Parse(endNodeY));

            #endregion 画节点.

            #region 画线
            //foreach (FlowNode bfn in FlowNodeCollections)
            //{ 
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                string begin = dr["Node"].ToString();
                string to = dr["ToNode"].ToString();

                if (dicFlowNode.ContainsKey(begin) && dicFlowNode.ContainsKey(to))
                {
                    //if (bfn.NodeID == dr["Node"].ToString())
                    //{
                    //    foreach (FlowNode efn in FlowNodeCollections)
                    //    {
                    //        if (efn.NodeID == dr["ToNode"].ToString())
                    //        {
                    Direction d = new Direction((IContainer)this);
                    d.DirType = (DirType)Enum.Parse(typeof(DirType), dr["DirType"].ToString(), false);
                    d.IsCanBack = (int.Parse(dr["IsCanBack"]) == 0) ? false : true;

                    string dots1 = dr["Dots"] as string;
                    if (string.IsNullOrEmpty(dots1) == false)
                    {
                        string[] strs = dots1.Split('@');
                        IList<double> dots = new List<double>();
                        foreach (string str in strs)
                        {
                            if (str == null || str == "")
                                continue;
                            string[] mystr = str.Split(',');
                            if (mystr.Length == 2)
                            {
                                dots.Add(double.Parse(mystr[0]));
                                dots.Add(double.Parse(mystr[1]));
                            }
                        }

                        d.LineType = DirectionLineType.Polyline;
                        d.DirectionTurnPoint1.CenterPosition = new Point(dots[0], dots[1]);
                        d.DirectionTurnPoint2.CenterPosition = new Point(dots[2], dots[3]);
                    }

                    d.FlowID = FlowID;
                    d.BeginFlowNode = dicFlowNode[begin];
                    d.EndFlowNode = dicFlowNode[to];
                    AddDirection(d);
                }
            }

            Direction da = new Direction((IContainer)this);
            da.FlowID = FlowID;
            da.BeginFlowNode = node0;
            da.EndFlowNode = GetBeginFlowNode();
            da.DirType = DirType.Virtual;
            AddDirection(da);

            IList<FlowNode> endNodes = GetEndFlowNodes();
            foreach (FlowNode item in endNodes)
            {
                Direction db = new Direction((IContainer)this); 
                db.FlowID = FlowID;
                db.BeginFlowNode = item;
                db.EndFlowNode = node1;
                //db.DirType = DirType.Virtual;
                AddDirection(db);
            }
            #endregion 画线

            #region 画标签
            foreach (DataRow dr in ds.Tables[2].Rows)
            {
                NodeLabel r = new NodeLabel((IContainer)this);
                r.LabelName = dr["Name"].ToString();
                r.Position = new Point(double.Parse(dr["X"].ToString()), double.Parse(dr["Y"].ToString()));
                r.LableID = dr["MyPK"].ToString();
                AddLabel(r);
            }
            #endregion 画标签

            //画线中会将此设置为true.
            IsNeedSave = false;
        }

        /// <summary>
        /// 获取真实开始节点
        /// </summary>
        /// <returns></returns>
        private FlowNode GetBeginFlowNode()
        {
            FlowNode flowNode = null;

            if (FlowNodeCollections != null)
            {
                string beginId = (int.Parse(FlowID + "01")).ToString();

                if (dicFlowNode.ContainsKey(beginId))
                {
                    flowNode = dicFlowNode[beginId];
                }
                else//理论上不会到达
                {
                    foreach (FlowNode item in FlowNodeCollections)
                    {
                        //找一个作为开始节点
                        if (item != null
                            && (item.EndDirectionCollections == null || item.EndDirectionCollections.Count == 0)
                            && item.NodeID != "0" && item.NodeID != "1")
                        {
                            flowNode = item;
                            break;
                        }
                    }
                }
            }

            return flowNode;
        }

        /// <summary>
        /// 获取未连接到虚结束节点的实结束节点集合
        /// </summary>
        /// <returns></returns>
        private IList<FlowNode> GetEndFlowNodes()
        {
            IList<FlowNode> list = new List<FlowNode>();

            if (FlowNodeCollections != null)
            {
                foreach (FlowNode item in FlowNodeCollections)
                {
                    //找真实结束节点（不含虚节点）
                    if (item != null
                        && item.NodeID != "0" && item.NodeID != "1" && item.NodeID != (int.Parse(FlowID + "01")).ToString())
                    {
                        //作为首节点有出线，若均为回退线，仍为结束节点
                        if (item.BeginDirectionCollections != null)
                        {
                            bool isEnd = true;
                            foreach (Direction dir in item.BeginDirectionCollections)
                            {
                                if (dir.DirType != DirType.Backward)
                                {
                                    isEnd = false;
                                    break;
                                }
                            }

                            if (!isEnd)
                            {
                                continue;
                            }
                        }

                        list.Add(item);
                    }
                }
            }

            return list;
        }

        private void InsertFlowNode(FlowNode node, double x, double y)
        {
            if (x < 50)
                x = 50;

            if (y < 30)
                y = 30;

            if (x > 1190)
                x = 1190;

            if (y > 770)
                y = 770;

            node.CenterPoint = new Point(x, y);

            AddFlowNode(node);
        }

        public void DrawFlows()
        {
            DrawFlows(this.FlowID);
        }

        /// <summary>
        /// 设计报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnDesignerTable()
        {
            Glo.WinOpenByDoType("CH", "WFRpt", this.FlowID, null, null);
        }
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnCheck()
        {
            Glo.WinOpenByDoType("CH", "FlowCheck", FlowID, null, null);
        }
        /// <summary>
        /// 试运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnRun()
        {
            Glo.WinOpenByDoType("CH", "RunFlow", FlowID, null, null);
        }
        public void ShowFlowNodeSetting(FlowNode fn)
        {
            this.f = fn;
            fn.sdPicture.tbNodeName.Visibility = Visibility.Visible;
            fn.sdPicture.tbNodeName.Focus();
            fn.sdPicture.txtNodeName.Visibility = Visibility.Collapsed;
            //fn.sdPicture.tbNodeName.LostFocus += new RoutedEventHandler(tbNodeName_LostFocus);
            IsSomeChildEditing = true;
        }
        public void ShowDirectionSetting(Direction r)
        {
            this.WinTitle = "方向设置";
            Glo.WinOpenByDoType("CH", BP.UrlFlag.Dir, FlowID, r.BeginFlowNode.NodeID, r.EndFlowNode.NodeID);
        }
        public void LoadFromXmlString(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return;

            FlowNodeType FlowNodeType;
            MergePictureRepeatDirection repeatDirection = MergePictureRepeatDirection.None;
            string FlowID = "";
            int zIndex = 0;
            string NodeID = "";
            string NodeName = "";
            Point FlowNodePosition = new Point();
            double temd = 0;
            Byte[] b = System.Text.UTF8Encoding.UTF8.GetBytes(xml);
            XElement xele = XElement.Load(System.Xml.XmlReader.Create(new MemoryStream(b)));

            //txtWorkFlowName.Text = xele.Attribute(XName.Get("Name")).Value;

            FlowID = xele.Attribute(XName.Get("FlowID")).Value;

            var partNos = from item in xele.Descendants("FlowNode") select item;
            foreach (XElement node in partNos)
            {
                FlowNodeType = (FlowNodeType)Enum.Parse(typeof(FlowNodeType), node.Attribute(XName.Get("Type")).Value, true);
                repeatDirection = (MergePictureRepeatDirection)Enum.Parse(typeof(MergePictureRepeatDirection), node.Attribute(XName.Get("RepeatDirection")).Value, true);
                FlowID = node.Attribute(XName.Get("FlowID")).Value;
                NodeID = node.Attribute(XName.Get("NodeID")).Value;
                NodeName = node.Attribute(XName.Get("NodeName")).Value;

                double.TryParse(node.Attribute(XName.Get("PositionX")).Value, out temd);
                FlowNodePosition.X = temd;

                double.TryParse(node.Attribute(XName.Get("PositionY")).Value, out temd);
                FlowNodePosition.Y = temd;
                int.TryParse(node.Attribute(XName.Get("ZIndex")).Value, out zIndex);

                FlowNode a = new FlowNode((IContainer)this, FlowNodeType);
                a.SubFlow = node.Attribute(XName.Get("SubFlow")).Value;
                a.RepeatDirection = repeatDirection;
                a.CenterPoint = FlowNodePosition;
                a.NodeID = NodeID;
                a.NodeName = NodeName;
                a.ZIndex = zIndex;
                a.EditType = this.EditType;
                a.FK_Flow = FlowID;

                AddFlowNode(a);
            }

            string beginNodeID = "";
            string endNodeID = "";
            double beginPointX = 0;
            double beginPointY = 0;
            double endPointX = 0;
            double endPointY = 0;
            double turnPoint1X = 0;
            double turnPoint1Y = 0;
            double turnPoint2X = 0;
            double turnPoint2Y = 0;

            string ruleID = "";
            string ruleName = "";
            string beginFlowNodeFlowID = "";
            string endFlowNodeFlowID = "";
            double containerWidth = 0;
            double containerHeight = 0;
            DirectionLineType lineType = DirectionLineType.Line;
            double.TryParse(xele.Attribute(XName.Get("Width")).Value, out containerWidth);
            double.TryParse(xele.Attribute(XName.Get("Height")).Value, out containerHeight);

            ContainerWidth = containerWidth;
            ContainerHeight = containerHeight;


            FlowNode temFlowNode = null;
            partNos = from item in xele.Descendants("Direction") select item;
            foreach (XElement node in partNos)
            {
                lineType = (DirectionLineType)Enum.Parse(typeof(DirectionLineType), node.Attribute(XName.Get("LineType")).Value, true);

                FlowID = node.Attribute(XName.Get("FlowID")).Value;

                ruleID = node.Attribute(XName.Get("DirectionID")).Value;
                ruleName = node.Attribute(XName.Get("DirectionName")).Value;
                beginFlowNodeFlowID = node.Attribute(XName.Get("BeginFlowNodeFlowID")).Value;
                endFlowNodeFlowID = node.Attribute(XName.Get("EndFlowNodeFlowID")).Value;

                beginNodeID = node.Attribute(XName.Get("BeginNodeID")).Value;
                endNodeID = node.Attribute(XName.Get("EndNodeID")).Value;


                double.TryParse(node.Attribute(XName.Get("TurnPoint1X")).Value, out turnPoint1X);
                double.TryParse(node.Attribute(XName.Get("TurnPoint1Y")).Value, out turnPoint1Y);
                double.TryParse(node.Attribute(XName.Get("TurnPoint2X")).Value, out turnPoint2X);
                double.TryParse(node.Attribute(XName.Get("TurnPoint2Y")).Value, out turnPoint2Y);

                double.TryParse(node.Attribute(XName.Get("BeginPointX")).Value, out beginPointX);
                double.TryParse(node.Attribute(XName.Get("BeginPointY")).Value, out beginPointY);
                double.TryParse(node.Attribute(XName.Get("EndPointX")).Value, out endPointX);
                double.TryParse(node.Attribute(XName.Get("EndPointY")).Value, out endPointY);

                int.TryParse(node.Attribute(XName.Get("ZIndex")).Value, out zIndex);


                Direction r = new Direction(this, false, lineType);
                AddDirection(r);
                r.DirectionID = ruleID;
                r.DirectionName = ruleName;
                r.ZIndex = zIndex;
                r.EditType = this.EditType;
                r.FlowID = FlowID;
                r.LineType = lineType;
                if (turnPoint1X > 0 && turnPoint2X > 0)
                {
                    r.TurnPoint1HadMoved = true;
                    r.TurnPoint2HadMoved = true;
                    r.DirectionTurnPoint1.CenterPosition = new Point(turnPoint1X, turnPoint1Y);
                    r.DirectionTurnPoint2.CenterPosition = new Point(turnPoint2X, turnPoint2Y);
                }
                if (beginFlowNodeFlowID != "")
                {
                    temFlowNode = getFlowNode(beginFlowNodeFlowID);
                    if (temFlowNode != null)
                        temFlowNode.AddBeginDirection(r);
                    else
                        r.BeginPointPosition = new Point(beginPointX, beginPointY);

                }
                else
                {
                    r.BeginPointPosition = new Point(beginPointX, beginPointY);
                }
                temFlowNode = null;
                if (endFlowNodeFlowID != "")
                {
                    temFlowNode = getFlowNode(endFlowNodeFlowID);
                    if (temFlowNode != null)
                        temFlowNode.AddEndDirection(r);
                    else
                        r.EndPointPosition = new Point(endPointX, endPointY);

                }
                else
                {
                    r.EndPointPosition = new Point(endPointX, endPointY);
                }
            }


            partNos = from item in xele.Descendants("Label") select item;
            string labelName = "";
            double labelX = 0;
            double labelY = 0;
            foreach (XElement node in partNos)
            {
                labelName = node.Value;

                double.TryParse(node.Attribute(XName.Get("X")).Value, out labelX);
                double.TryParse(node.Attribute(XName.Get("Y")).Value, out labelY);

                NodeLabel l = new NodeLabel(this);
                l.LabelName = labelName;
                l.Position = new Point(labelX, labelY);
                AddLabel(l);
            }
        }

        /// <summary>
        /// 先设置模态对话框的属性，再打开
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="dotype"></param>
        /// <param name="fk_flow"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="title"></param>
        public void SetProper(string lang, string dotype, string fk_flow, string node1, string node2, string title)
        {
            this.WinTitle = title;
            // _Service.GetRelativeUrlAsync(lang, dotype, fk_flow, node1, node2, true);
            // _Service.GetRelativeUrlCompleted += _Service_GetRelativeUrlCompleted;
        }
        public string ToXmlString()
        {
            System.Text.StringBuilder xml = new System.Text.StringBuilder(@"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes"" ?>  ");
            xml.Append(Environment.NewLine);
            xml.Append(@"<WorkFlow");
            xml.Append(@" FlowID=""" + FlowID + @"""");
            xml.Append(@" ID=""" + Guid.NewGuid().ToString() + @"""");
            xml.Append(@" Name=""Test" + @"""");
            xml.Append(@" Description=""Test" + @"""");
            xml.Append(@" Width=""" + ContainerWidth.ToString() + @"""");
            xml.Append(@" Height=""" + ContainerHeight.ToString() + @""">");


            System.Text.StringBuilder FlowNodeXml = new System.Text.StringBuilder("    <FlowNodes>");
            System.Text.StringBuilder ruleXml = new System.Text.StringBuilder("    <Directions>");
            System.Text.StringBuilder labelXml = new System.Text.StringBuilder("    <Labels>");

            IElement ele;
            foreach (UIElement c in cnsDesignerContainer.Children)
            {
                ele = c as IElement;
                if (ele != null)
                {
                    if (ele.IsDeleted)
                        continue;
                    if (ele.ElementType == WorkFlowElementType.FlowNode)
                    {
                        FlowNodeXml.Append(Environment.NewLine);
                        FlowNodeXml.Append(ele.ToXmlString());

                    }
                    else if (ele.ElementType == WorkFlowElementType.Direction)
                    {
                        ruleXml.Append(Environment.NewLine);
                        ruleXml.Append(ele.ToXmlString());

                    }
                    else if (ele.ElementType == WorkFlowElementType.Label)
                    {
                        labelXml.Append(Environment.NewLine);
                        labelXml.Append(ele.ToXmlString());

                    }
                }

            }
            FlowNodeXml.Append(Environment.NewLine);
            FlowNodeXml.Append("    </FlowNodes>");
            ruleXml.Append(Environment.NewLine);
            ruleXml.Append("    </Directions>");
            labelXml.Append(Environment.NewLine);
            labelXml.Append("    </Labels>");
            xml.Append(Environment.NewLine);
            xml.Append(FlowNodeXml.ToString());
            xml.Append(Environment.NewLine);
            xml.Append(ruleXml.ToString());
            xml.Append(Environment.NewLine);
            xml.Append(labelXml.ToString());
            xml.Append(Environment.NewLine);
            xml.Append(@"</WorkFlow>");
            return xml.ToString();

        }

        public void AddDirection(Direction r)
        {
            if (!cnsDesignerContainer.Children.Contains(r))
            {
                cnsDesignerContainer.Children.Add(r);
                r.Container = this;
                r.DirectionChanged += new DirectionChangeDelegate(OnDirectionChanged);
            }
            if (!DirectionCollections.Contains(r))
            {
                DirectionCollections.Add(r);
            }
        }

        public void AddLabel(NodeLabel l)
        {
            l.IsSelectd = false;

            if (!cnsDesignerContainer.Children.Contains(l))
            {
                cnsDesignerContainer.Children.Add(l);
            }

            if (!LableCollections.Contains(l))
            {
                LableCollections.Add(l);
            }
        }

        public void RemoveDirection(Direction r)
        {
            if (cnsDesignerContainer.Children.Contains(r))
            {
                cnsDesignerContainer.Children.Remove(r);
            }
            if (DirectionCollections.Contains(r))
            {
                DirectionCollections.Remove(r);

            }
        }

        public void SelectDirection(Direction r, bool isSelectd)
        {
            SolidColorBrush brush = new SolidColorBrush();

            if (isSelectd)
            {
                brush.Color = Color.FromArgb(255, 255, 181, 0);
            }
            else
            {
                brush.Color = Color.FromArgb(255, 0, 128, 0);
            }

            r.begin.Fill = brush;
            r.endArrow.Stroke = brush;
            r.line.Stroke = brush;
            if (r.LineType == DirectionLineType.Polyline)
            {
                r.DirectionTurnPoint1.Fill = brush;
                r.DirectionTurnPoint2.Fill = brush;
            }
        }

        public void SelectFlowNode(FlowNode a, bool isSelectd)
        {
            if (isSelectd)
            {
                a.sdPicture.SetSelectedColor();
            }
            else
            {
                a.sdPicture.ResetInitColor();
                //SolidColorBrush brush = new SolidColorBrush(Colors.White);
                //a.sdPicture.Background = brush;
            }
        }

        void OnDirectionChanged(Direction a)
        {
            SaveChange(HistoryType.New);
        }

        public void OnFlowNodeChanged(FlowNode a)
        {
            SaveChange(HistoryType.New);
        }

        public void AddFlowNode(FlowNode a)
        {
            if (!cnsDesignerContainer.Children.Contains(a))
            {
                cnsDesignerContainer.Children.Add(a);
                a.Container = this;
                a.FlowNodeChanged += OnFlowNodeChanged;
                a.FK_Flow = FlowID;
            }
            if (!FlowNodeCollections.Contains(a))
            {
                FlowNodeCollections.Add(a);

                if (!dicFlowNode.ContainsKey(a.NodeID))
                {
                    dicFlowNode.Add(a.NodeID, a);
                }
            }
        }

        public void RemoveFlowNode(FlowNode a)
        {
            if (cnsDesignerContainer.Children.Contains(a))
                cnsDesignerContainer.Children.Remove(a);
            if (FlowNodeCollections.Contains(a))
            {
                FlowNodeCollections.Remove(a);
                _Service.DoAsync("DelNode", a.NodeID, true);
            }
        }

        public void RemoveLabel(NodeLabel l)
        {

            if (cnsDesignerContainer.Children.Contains(l))
                cnsDesignerContainer.Children.Remove(l);
            if (LableCollections.Contains(l))
            {
                LableCollections.Remove(l);
                _Service.DoAsync("DelLable", l.LableID, true);
            }
        }

        public void WinOpen(string url, string title)
        {
            Designer.IsRefresh = IsContainerRefresh;
            Designer.OpenWindow(url, title, 600, 800);

        }

        public void ShowContainerCover()
        {
            canContainerCover.Visibility = Visibility.Visible;

            sbContainerCoverDisplay.Begin();


        }

        public void CloseContainerCover()
        {
            sbContainerCoverClose.Completed += new EventHandler(sbContainerCoverClose_Completed);
            sbContainerCoverClose.Begin();
        }

        public void AddDirection(bool isReturnType)
        {
            var r = new Direction((IContainer)this);
            r.DirType = isReturnType ? DirType.Backward : DirType.Forward;
            r.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            r.DirectionName = "New Direction" + NextNewDirectionIndex.ToString();
            AddDirection(r);
            SaveChange(HistoryType.New);
        }

        public void AddLabel(int x, int y)
        {
            this.pos = new Point(x, y);
            _Service.DoNewLabelAsync(FlowID, x, y, "New Label" + NextNewLabelIndex.ToString(), null);
            _Service.DoNewLabelCompleted += _Service_DoNewLabelCompleted;
        }
        void _Service_DoNewLabelCompleted(object sender, DoNewLabelCompletedEventArgs e)
        {
            NodeLabel r = new NodeLabel((IContainer)this);
            r.LabelName = "NewLable" + NextNewLabelIndex.ToString();
            r.LableID = e.Result;
            r.SetValue(Canvas.LeftProperty, this.pos.X);
            r.SetValue(Canvas.TopProperty, this.pos.Y);

            AddLabel(r);

            SaveChange(HistoryType.New);
            isNeedSave = true;
            _Service.DoNewLabelCompleted -= _Service_DoNewLabelCompleted;
        }
        ToolbarButton HisCtl = null;
        public bool Save(ToolbarButton ctl)
        {
            if (ctl != null)
                this.HisCtl = ctl;

            if (this.HisCtl == null)
                this.HisCtl = new ToolbarButton();

            this.HisCtl.IsEnabled = false;
            // 如果没有要保存的内容，则返回促成成功。
            if (!isNeedSave)
            {
                this.HisCtl.IsEnabled = true;
                return true;
            }

            string nodes = "";
            string dirs = "";
            string labes = "";
            CheckResult cr = this.CheckSave();
            if (cr.IsPass)
            {
                IElement ele;
                foreach (UIElement c in cnsDesignerContainer.Children)
                {
                    ele = c as IElement;
                    if (ele != null)
                    {
                        if (ele.IsDeleted)
                            continue;
                        if (ele.ElementType == WorkFlowElementType.FlowNode)
                        {
                            FlowNode f = ele as FlowNode;
                            // 如果节点没有进线，并且不是惟一的开始节点，则设节点为结束节点。
                            //if (f.BeginDirectionCollections.Count == 0
                            //    && cnsDesignerContainer.Children.Count != 2)
                            //    f.Type = FlowNodeType.COMPLETION;
                            nodes += "~@Name=" + f.NodeName + "@X=" + (int)f.CenterPoint.X + "@Y=" + (int)f.CenterPoint.Y + "@NodeID=" + int.Parse(f.NodeID) + "@HisRunModel=" + (int)f.HisRunModel;
                        }
                        else if (ele.ElementType == WorkFlowElementType.Direction)
                        {
                            Direction d = ele as Direction;
                            if (d.EndFlowNode != null
                                && d.BeginFlowNode != null && d.BeginFlowNode.NodeID != "0")  //不为虚开始
                            {
                                //中间节与虚结束节点有连线时忽略不保存
                                if (d.EndFlowNode.NodeID == "1" && !CheckVirtualDir(d))
                                    continue;

                                dirs += "~@Node=" + int.Parse(d.BeginFlowNode.NodeID) + "@ToNode=" + int.Parse(d.EndFlowNode.NodeID)
                                    + "@DirType=" + (int)d.DirType + "@IsCanBack=" + (d.IsCanBack ? 1 : 0)
                                    + "@Dots=" + (d.LineType == DirectionLineType.Polyline
                                    ? ("#" + d.DirectionTurnPoint1.CenterPosition.X + "," + d.DirectionTurnPoint1.CenterPosition.Y
                                    + "#" + d.DirectionTurnPoint2.CenterPosition.X + "," + d.DirectionTurnPoint2.CenterPosition.Y) : string.Empty)
                                    + "@MyPK=" + (d.BeginFlowNode.NodeID + "_" + d.EndFlowNode.NodeID + "_" + (int)d.DirType);
                            }
                        }
                        else if (ele.ElementType == WorkFlowElementType.Label)
                        {
                            NodeLabel l = ele as NodeLabel;
                            //_Service.DoNewLabelAsync(FlowID, (int)l.Position.X, (int)l.Position.Y, l.LabelName, l.LableID);
                            labes += "~@FK_Flow=" + FlowID + "@X=" + (int)l.Position.X + "@Y=" + (int)l.Position.Y + "@MyPK=" + l.LableID + "@Label=" + l.LabelName;
                        }
                    }
                }  // 结束遍历。

                WSDesignerSoapClient ws = Glo.GetDesignerServiceInstance();
                ws.DoSaveFlowAsync(FlowID, nodes, dirs, labes);
                ws.DoSaveFlowCompleted += new EventHandler<DoSaveFlowCompletedEventArgs>(ws_DoSaveFlowCompleted);
                return true;
            }
            else
            {
                HtmlPage.Window.Alert(cr.Message);
                this.HisCtl.IsEnabled = true;
                return false;
            }
        }
        void ws_DoSaveFlowCompleted(object sender, DoSaveFlowCompletedEventArgs e)
        {
            this.HisCtl.IsEnabled = true;
            if (e.Result != null)
            {
                this.IsNeedSave = true;
                MessageBox.Show(e.Result, "保存流程错误", MessageBoxButton.OK);
                return;
            }
            //      MessageBox.Show("保存成功", "ccflow", MessageBoxButton.OK);
            this.IsNeedSave = false;
        }

        /// <summary>
        /// 保存时检查是否为有效的虚拟方向线
        /// 即其首节点是否为真实结束结点，排除中间节点情况
        /// </summary>
        /// <param name="d">当前虚拟方向线</param>
        /// <returns></returns>
        private bool CheckVirtualDir(Direction d)
        {
            FlowNode item = d.BeginFlowNode;
            bool isValid = false;

            //找真实结束节点（不含虚节点）
            if (item != null
                && item.NodeID != "0" && item.NodeID != "1" && item.NodeID != (int.Parse(FlowID + "01")).ToString())
            {
                isValid = true;
                //首节点切出线若均为回退线，仍为结束节点，否则为中间节点
                foreach (Direction dir in item.BeginDirectionCollections)
                {
                    if (dir == d)
                    {
                        continue;
                    }

                    if (dir.DirType != DirType.Backward)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        FlowNode getFlowNode(string FlowNodeFlowID)
        {
            for (int i = 0; i < FlowNodeCollections.Count; i++)
            {
                if (FlowNodeCollections[i].FK_Flow == FlowNodeFlowID)
                {
                    return FlowNodeCollections[i];
                }
            }
            return null;
        }

        public Canvas GridLinesContainer
        {
            get
            {
                if (_gridLinesContainer == null)
                {

                    Canvas temCan = new Canvas();
                    temCan.Name = "canGridLinesContainer";
                    cnsDesignerContainer.Children.Add(temCan);
                    _gridLinesContainer = temCan;

                }
                return _gridLinesContainer;
            }

        }

        public void clearContainer()
        {
            cnsDesignerContainer.Children.Clear();
            _gridLinesContainer = null;
            SetGridLines();
            flowNodeCollections = null;
            directionCollections = null;
        }

        public void ShowMessage(string message)
        {
            ShowContainerCover();
            MessageTitle.Text = message;
            MessageBody.Visibility = Visibility.Visible;
        }

        public void AddFlowNode()
        {
            NewNodeName = "New Node " + NextNewFlowNodeIndex.ToString();
            nextNewFlowNodeIndex--;
            _Service.DoNewNodeAsync(FlowID, 10, 10, NewNodeName, true);
            _Service.DoNewNodeCompleted += _Service_DoNewNodeCompleted;
        }

        void pushNextQueueToPreQueue()
        {
            if (WorkFlowXmlPreStack.Count > 0)
                WorkFlowXmlNextStack.Push(WorkFlowXmlPreStack.Pop());
            int cout = WorkFlowXmlNextStack.Count;

            for (int i = 0; i < cout; i++)
            {
                WorkFlowXmlPreStack.Push(WorkFlowXmlNextStack.Pop());
            }
        }

        public void SaveChange(HistoryType action)
        {
            if (action == HistoryType.New)
            {
                WorkFlowXmlPreStack.Push(workflowXmlCurrent);
                workflowXmlCurrent = ToXmlString();
                WorkFlowXmlNextStack.Clear();
            }
            if (action == HistoryType.Next)
            {
                if (WorkFlowXmlNextStack.Count > 0)
                {
                    WorkFlowXmlPreStack.Push(workflowXmlCurrent);
                    workflowXmlCurrent = WorkFlowXmlNextStack.Pop();
                    clearContainer();
                    ClearSelectFlowElement(null);
                }

                LoadFromXmlString(workflowXmlCurrent);
            }

            if (action == HistoryType.Previous)
            {
                if (WorkFlowXmlPreStack.Count > 0)
                {
                    WorkFlowXmlNextStack.Push(workflowXmlCurrent);
                    workflowXmlCurrent = WorkFlowXmlPreStack.Pop();
                    clearContainer();

                    LoadFromXmlString(workflowXmlCurrent);
                    ClearSelectFlowElement(null);
                }
            }
        }

        public void PreviousAction()
        {
            SaveChange(HistoryType.Previous);

        }

        public void NextAction()
        {
            SaveChange(HistoryType.Next);

        }

        public void AddSelectedControl(System.Windows.Controls.Control uc)
        {
            if (!CurrentSelectedControlCollection.Contains(uc))
                CurrentSelectedControlCollection.Add(uc);
        }

        public void RemoveSelectedControl(System.Windows.Controls.Control uc)
        {
            if (CurrentSelectedControlCollection.Contains(uc))
                CurrentSelectedControlCollection.Remove(uc);
        }

        public void SetWorkFlowElementSelected(System.Windows.Controls.Control uc, bool isSelected)
        {
            if (isSelected)
                AddSelectedControl(uc);
            else
                RemoveSelectedControl(uc);
            if (!CtrlKeyIsPress)
                ClearSelectFlowElement(uc);

        }

        public void ClearSelectFlowElement(System.Windows.Controls.Control uc)
        {

            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;

            int count = CurrentSelectedControlCollection.Count;
            for (int i = 0; i < count; i++)
            {

                ((IElement)CurrentSelectedControlCollection[i]).IsSelectd = false;
            }
            CurrentSelectedControlCollection.Clear();
            if (uc != null)
            {
                ((IElement)uc).IsSelectd = true;
                AddSelectedControl(uc);
            }
            MouseIsInContainer = true;


        }

        public void DeleteSeletedControl()
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;
            FlowNode a = null;
            Direction r = null;
            NodeLabel l = null;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    a.Delete();
                }
                if (CurrentSelectedControlCollection[i] is Direction)
                {
                    r = CurrentSelectedControlCollection[i] as Direction;
                    r.Delete();
                }
                if (CurrentSelectedControlCollection[i] is NodeLabel)
                {
                    l = CurrentSelectedControlCollection[i] as NodeLabel;
                    l.Delete();
                }
            }
            ClearSelectFlowElement(null);

        }

        public void AlignTop()
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;
            FlowNode a = null;
            double minY = 100000.0;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;

                    if (a.CenterPoint.Y < minY)
                        minY = a.CenterPoint.Y;
                }

            }
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    a.CenterPoint = new Point(a.CenterPoint.X, minY);
                }
            }
        }

        public void AlignBottom()
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;
            FlowNode a = null;
            double maxY = 0;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;

                    if (a.CenterPoint.Y > maxY)
                        maxY = a.CenterPoint.Y;
                }

            }
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    a.CenterPoint = new Point(a.CenterPoint.X, maxY);
                }
            }
        }

        public void AlignLeft()
        {

            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;
            FlowNode a = null;
            double minX = 100000.0;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;

                    if (a.CenterPoint.X < minX)
                        minX = a.CenterPoint.X;
                }
            }
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    a.CenterPoint = new Point(minX, a.CenterPoint.Y);
                }
            }

        }

        public void AlignRight()
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
                return;
            FlowNode a = null;
            double maxX = 0;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;

                    if (a.CenterPoint.X > maxX)
                        maxX = a.CenterPoint.X;
                }

            }
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    a.CenterPoint = new Point(maxX, a.CenterPoint.Y);
                }
            }
        }

        public void MoveUp()
        {
            MoveControlCollectionByDisplacement(0, -MoveStepLenght, null);
            SaveChange(HistoryType.New);
        }

        public void MoveLeft()
        {
            MoveControlCollectionByDisplacement(-MoveStepLenght, 0, null);
            SaveChange(HistoryType.New);

        }

        public void MoveDown()
        {
            MoveControlCollectionByDisplacement(0, MoveStepLenght, null);
            SaveChange(HistoryType.New);

        }

        public void MoveRight()
        {
            MoveControlCollectionByDisplacement(MoveStepLenght, 0, null);
            SaveChange(HistoryType.New);

        }

        public void MoveControlCollectionByDisplacement(double x, double y, UserControl uc)
        {
            if (CurrentSelectedControlCollection == null || CurrentSelectedControlCollection.Count == 0)
            {
                return;
            }

            // 如果光标所在的节点没有被选中，则不移动所有被选中的节点，光移动光标所有的节点即可。
            var element = uc as IElement;
            if (element != null && !element.IsSelectd)
            {
                return;
            }

            FlowNode selectedFlowNode = null;
            Direction selectedDirection = null;
            NodeLabel selectedLabel = null;
            if (uc is FlowNode)
            {
                selectedFlowNode = uc as FlowNode;
            }

            if (uc is Direction)
            {
                selectedDirection = uc as Direction;
            }
            if (uc is NodeLabel)
            {
                selectedLabel = uc as NodeLabel;
            }

            FlowNode a = null;
            Direction r = null;
            NodeLabel l = null;
            for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
            {
                if (CurrentSelectedControlCollection[i] is FlowNode)
                {
                    a = CurrentSelectedControlCollection[i] as FlowNode;
                    if (a == selectedFlowNode)
                        continue;
                    a.SetPositionByDisplacement(x, y);
                }
                if (CurrentSelectedControlCollection[i] is NodeLabel)
                {
                    l = CurrentSelectedControlCollection[i] as NodeLabel;
                    if (l == selectedLabel)
                        continue;
                    l.SetPositionByDisplacement(x, y);
                }
                if (CurrentSelectedControlCollection[i] is Direction)
                {
                    r = CurrentSelectedControlCollection[i] as Direction;
                    if (r == selectedDirection)
                        continue;
                    r.SetPositionByDisplacement(x, y);
                }
            }

            IsNeedSave = true;
        }



        public void PastMemoryToContainer()
        {
            if (CopyElementCollectionInMemory != null
                      && CopyElementCollectionInMemory.Count > 0)
            {
                FlowNode a = null;
                Direction r = null;
                NodeLabel l = null;

                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is Direction)
                    {
                        r = c as Direction;
                        AddDirection(r);
                        if (r.LineType == DirectionLineType.Line)
                        {
                            r.SetDirectionPosition(new Point(r.BeginPointPosition.X + 20, r.BeginPointPosition.Y + 20),
                                new Point(r.EndPointPosition.X + 20, r.EndPointPosition.Y + 20));
                        }
                        else
                        {
                            r.SetDirectionPosition(new Point(r.BeginPointPosition.X + 20, r.BeginPointPosition.Y + 20),
                                new Point(r.EndPointPosition.X + 20, r.EndPointPosition.Y + 20)
                               , new Point(r.DirectionTurnPoint1.CenterPosition.X + 20, r.DirectionTurnPoint1.CenterPosition.Y + 20)
                               , new Point(r.DirectionTurnPoint2.CenterPosition.X + 20, r.DirectionTurnPoint2.CenterPosition.Y + 20)
                               );
                        }
                    }
                }


                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is FlowNode)
                    {
                        a = c as FlowNode;
                        AddFlowNode(a);
                        a.CenterPoint = new Point(a.CenterPoint.X + 20, a.CenterPoint.Y + 20);
                        a.Move(a, null);

                    }

                }
                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is NodeLabel)
                    {
                        l = c as NodeLabel;
                        AddLabel(l);
                        l.Position = new Point(l.Position.X + 20, l.Position.Y + 20);
                    }
                }

                for (int i = 0; i < CurrentSelectedControlCollection.Count; i++)
                {
                    ((IElement)CurrentSelectedControlCollection[i]).IsSelectd = false;
                }
                CurrentSelectedControlCollection.Clear();

                for (int i = 0; i < CopyElementCollectionInMemory.Count; i++)
                {

                    ((IElement)CopyElementCollectionInMemory[i]).IsSelectd = true;
                    AddSelectedControl(CopyElementCollectionInMemory[i]);
                }
                CopySelectedControlToMemory(null);

                SaveChange(HistoryType.New);


            }
        }

        public void CopySelectedControlToMemory(System.Windows.Controls.Control currentControl)
        {
            copyElementCollectionInMemory = null;

            if (currentControl != null)
            {
                if (currentControl is FlowNode)
                {

                    CopyElementCollectionInMemory.Add(((FlowNode)currentControl).Clone());
                }
                if (currentControl is Direction)
                {

                    CopyElementCollectionInMemory.Add(((Direction)currentControl).Clone());
                }
                if (currentControl is NodeLabel)
                {

                    CopyElementCollectionInMemory.Add(((NodeLabel)currentControl).Clone());
                }
            }
            else
            {
                if (CurrentSelectedControlCollection != null
                    && CurrentSelectedControlCollection.Count > 0)
                {
                    FlowNode a = null;
                    Direction r = null;
                    NodeLabel l = null;
                    foreach (System.Windows.Controls.Control c in CurrentSelectedControlCollection)
                    {
                        if (c is FlowNode)
                        {
                            a = c as FlowNode;

                            CopyElementCollectionInMemory.Add(a.Clone());
                        }
                    }
                    foreach (System.Windows.Controls.Control c in CurrentSelectedControlCollection)
                    {
                        if (c is NodeLabel)
                        {
                            l = c as NodeLabel;

                            CopyElementCollectionInMemory.Add(l.Clone());
                        }
                    }
                    foreach (System.Windows.Controls.Control c in CurrentSelectedControlCollection)
                    {
                        if (c is Direction)
                        {
                            r = c as Direction;
                            r = r.Clone();
                            CopyElementCollectionInMemory.Add(r);

                            if (r.OriginDirection.BeginFlowNode != null)
                            {
                                FlowNode temA = null;
                                foreach (System.Windows.Controls.Control c1 in CopyElementCollectionInMemory)
                                {
                                    if (c1 is FlowNode)
                                    {
                                        temA = c1 as FlowNode;
                                        if (r.OriginDirection.BeginFlowNode == temA.OriginFlowNode)
                                        {
                                            r.BeginFlowNode = temA;
                                        }
                                    }
                                }
                            }
                            if (r.OriginDirection.EndFlowNode != null)
                            {
                                FlowNode temA = null;
                                foreach (System.Windows.Controls.Control c1 in CopyElementCollectionInMemory)
                                {
                                    if (c1 is FlowNode)
                                    {
                                        temA = c1 as FlowNode;
                                        if (r.OriginDirection.EndFlowNode == temA.OriginFlowNode)
                                        {
                                            r.EndFlowNode = temA;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (System.Windows.Controls.Control c in CurrentSelectedControlCollection)
                    {
                        if (c is FlowNode)
                        {
                            a = c as FlowNode;

                            a.OriginFlowNode = null;
                        }
                        if (c is Direction)
                        {
                            r = c as Direction;

                            r.OriginDirection = null;
                        }
                    }

                }
            }
        }
        public void UpdateSelectedControlToMemory(System.Windows.Controls.Control currentControl)
        {
            NodeLabel ll = (NodeLabel)currentControl;
            l = ll;
            ll.txtLabelName.Visibility = Visibility.Collapsed;
            ll.tbLabelName.Visibility = Visibility.Visible;
            ll.tbLabelName.Focus();
            ll.tbLabelName.LostFocus += new RoutedEventHandler(tbLabelName_LostFocus);
        }

        public void UpDateSelectedNode(System.Windows.Controls.Control currentControl)
        {
            FlowNode f = (FlowNode)currentControl;
            f.sdPicture.tbNodeName.Visibility = Visibility.Visible;
            f.sdPicture.txtNodeName.Visibility = Visibility.Collapsed;
        }

        public void NewFlow()
        {
            NewFlow(null);
        }
        #endregion

        #region 事件

        

        void _service_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            FlowID = e.Result;
            _Service.DoCompleted -= new EventHandler<DoCompletedEventArgs>(_service_DoCompleted);
            this.DrawFlows(FlowID);
        }

        

         

        void DoubleClick_Timer(object sender, EventArgs e)
        {
            _doubleClickTimer.Stop();
        }

        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseIsInContainer = true;

        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseIsInContainer = false;

        }

        private void cnsDesignerContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseIsInContainer)
            {
                if (menuFlowNode.Visibility == Visibility.Collapsed
                    && menuDirection.Visibility == Visibility.Collapsed
                    && menuLabel.Visibility == Visibility.Collapsed)
                {

                    menuContainer.ShowMenu();

                    double top = (double)(e.GetPosition(svContainer).Y);
                    double left = (double)(e.GetPosition(svContainer).X);
                    menuContainer.CenterPoint = new Point(left, top);
                }
            }
            e.Handled = true;
        }

        private void OnContextMenu(object sender, System.Windows.Browser.HtmlEventArgs e)
        {

            if (MouseIsInContainer)
            {
                e.PreventDefault();

                if (menuFlowNode.Visibility == Visibility.Collapsed
                    && menuDirection.Visibility == Visibility.Collapsed
                    && menuLabel.Visibility == Visibility.Collapsed)
                {
                    menuContainer.ShowMenu();

                    double top = (double)(e.ClientY - Top);
                    double left = (double)(e.ClientX - Left);
                    menuContainer.CenterPoint = new Point(left, top);
                }
            }
        }

        public void ShowFlowNodeContentMenu(FlowNode a, object sender, MouseButtonEventArgs e)
        {
            //虚节点不显示菜单
            if (a.NodeID != "0" && a.NodeID != "1")
            {
                menuFlowNode.RelatedFlowNode = a;
                menuContainer.Visibility = Visibility.Collapsed;
                menuDirection.Visibility = Visibility.Collapsed;
                menuLabel.Visibility = Visibility.Collapsed;
                double top = (double)(e.GetPosition(svContainer).Y);
                double left = (double)(e.GetPosition(svContainer).X);
                menuFlowNode.CenterPoint = new Point(left, top);
                menuFlowNode.ShowMenu();
            }
            e.Handled = true;
        }

        public void ShowLabelContentMenu(NodeLabel l, object sender, MouseButtonEventArgs e)
        {
            menuLabel.RelatedLabel = l;
            menuContainer.Visibility = Visibility.Collapsed;
            menuDirection.Visibility = Visibility.Collapsed;
            menuFlowNode.Visibility = Visibility.Collapsed;
            double top = (double)(e.GetPosition(svContainer).Y);
            double left = (double)(e.GetPosition(svContainer).X);
            menuLabel.CenterPoint = new Point(left, top);
            menuLabel.ShowMenu();
            e.Handled = true;
        }

        public void ShowDirectionContentMenu(Direction r, object sender, MouseButtonEventArgs e)
        {
            //虚连线不显示菜单
            if (r.DirType != DirType.Virtual)
            {
                menuDirection.RelatedDirection = r;
                menuContainer.Visibility = Visibility.Collapsed;
                menuLabel.Visibility = Visibility.Collapsed;
                menuFlowNode.Visibility = Visibility.Collapsed;
                double top = (double)(e.GetPosition(svContainer).Y);
                double left = (double)(e.GetPosition(svContainer).X);
                menuDirection.CenterPoint = new Point(left, top);
                menuDirection.ShowMenu();
            }
            
            e.Handled = true;
        }
        private void btnCloseMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBody.Visibility = Visibility.Collapsed;
            CloseContainerCover();
        }

        void _Service_DoNewNodeCompleted(object sender, DoNewNodeCompletedEventArgs e)
        {
            var a = new FlowNode((IContainer)this, FlowNodeType.Ordinary);
            a.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            a.NodeName = "新建节点" + NextNewFlowNodeIndex.ToString();
            a.NodeID = e.Result.ToString();
            a.FK_Flow = FlowID;
            a.CenterPoint = new Point(50, 30);
            AddFlowNode(a);
            SaveChange(HistoryType.New);
            _Service.DoNewNodeCompleted -= _Service_DoNewNodeCompleted;
        }

        void sbContainerCoverClose_Completed(object sender, EventArgs e)
        {
            canContainerCover.Visibility = Visibility.Collapsed;
        }

        void wfClient_UpdateWorkFlowXMLCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("保存成功.");
        }

        private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
                Point beginPoint = mousePosition;
                Point endPoint = e.GetPosition(element);

                if (temproaryEllipse == null)
                {
                    temproaryEllipse = new Rectangle();

                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromArgb(255, 234, 213, 2);
                    temproaryEllipse.Fill = brush;
                    temproaryEllipse.Opacity = 0.2;

                    brush = new SolidColorBrush();
                    brush.Color = Color.FromArgb(255, 0, 0, 0);
                    temproaryEllipse.Stroke = brush;
                    temproaryEllipse.StrokeMiterLimit = 2.0;

                    cnsDesignerContainer.Children.Add(temproaryEllipse);

                }

                if (endPoint.X >= beginPoint.X)
                {
                    if (endPoint.Y >= beginPoint.Y)
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                    }
                    else
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                    }

                }
                else
                {
                    if (endPoint.Y >= beginPoint.Y)
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                    }
                    else
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                    }

                }


                temproaryEllipse.Width = Math.Abs(endPoint.X - beginPoint.X);
                temproaryEllipse.Height = Math.Abs(endPoint.Y - beginPoint.Y);


            }
            else
            {
                if (CurrentTemporaryDirection != null)
                {
                    CurrentTemporaryDirection.CaptureMouse();
                    Point currentPoint = e.GetPosition(CurrentTemporaryDirection);
                    CurrentTemporaryDirection.EndPointPosition = currentPoint;

                    if (CurrentTemporaryDirection.BeginFlowNode != null)
                    {
                        CurrentTemporaryDirection.BeginPointPosition = CurrentTemporaryDirection.GetResetPoint(currentPoint, CurrentTemporaryDirection.BeginFlowNode.CenterPoint, CurrentTemporaryDirection.BeginFlowNode, DirectionMoveType.Begin);
                    }
                }
            }

        }

        private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            trackingMouseMove = false;
            if (CurrentTemporaryDirection != null)
            {
                CurrentTemporaryDirection.SimulateDirectionPointMouseLeftButtonUpEvent(DirectionMoveType.End, CurrentTemporaryDirection, e);
                if (CurrentTemporaryDirection.EndFlowNode == null)
                {
                    CurrentTemporaryDirection.BeginFlowNode.CanShowMenu = false;

                    CurrentTemporaryDirection.CanShowMenu = false;
                    // this.RemoveDirection(CurrentTemporaryDirection);
                    CurrentTemporaryDirection.Delete();

                }
                else
                {
                    CurrentTemporaryDirection.CanShowMenu = false;
                    CurrentTemporaryDirection.BeginFlowNode.CanShowMenu = false;
                    CurrentTemporaryDirection.EndFlowNode.CanShowMenu = true;
                    CurrentTemporaryDirection.DirType = this.IsReturnTypeDir ? DirType.Backward : DirType.Forward;
                    //连接到虚结束节点的线类型为2
                    if (CurrentTemporaryDirection.EndFlowNode.NodeID == "1")
                    {
                        CurrentTemporaryDirection.DirType = DirType.Virtual;
                        CurrentTemporaryDirection.LineType = DirectionLineType.Line;
                    }

                    //CurrentTemporaryDirection.IsReturnType = this.IsReturnTypeDir;
                    if (this.IsReturnTypeDir)
                    {
                        CurrentTemporaryDirection.LineType = DirectionLineType.Polyline;
                    }
                    CurrentTemporaryDirection.IsTemporaryDirection = false;
                    CurrentTemporaryDirection.IsSelectd = false;
                    RemoveSelectedControl(CurrentTemporaryDirection);
                    SaveChange(HistoryType.New);
                }
                CurrentTemporaryDirection.ReleaseMouseCapture();
                CurrentTemporaryDirection = null;

            }

            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(element);
            if (temproaryEllipse != null)
            {
                double width = temproaryEllipse.Width;
                double height = temproaryEllipse.Height;

                if (width > 10 && height > 10)
                {
                    Point p = new Point();
                    p.X = (double)temproaryEllipse.GetValue(Canvas.LeftProperty);
                    p.Y = (double)temproaryEllipse.GetValue(Canvas.TopProperty);

                    FlowNode a = null;
                    Direction r = null;
                    NodeLabel l = null;
                    foreach (UIElement uie in cnsDesignerContainer.Children)
                    {
                        if (uie is FlowNode)
                        {
                            a = uie as FlowNode;
                            if (p.X < a.CenterPoint.X && a.CenterPoint.X < p.X + width
                                && p.Y < a.CenterPoint.Y && a.CenterPoint.Y < p.Y + height)
                            {
                                AddSelectedControl(a);
                                a.IsSelectd = true;
                            }
                        }
                        if (uie is NodeLabel)
                        {
                            l = uie as NodeLabel;
                            if (p.X < l.Position.X && l.Position.X < p.X + width
                                && p.Y < l.Position.Y && l.Position.Y < p.Y + height)
                            {
                                AddSelectedControl(a);
                                l.IsSelectd = true;
                            }
                        }
                        if (uie is Direction)
                        {
                            r = uie as Direction;

                            Point ruleBeginPointPosition = r.BeginPointPosition;
                            Point ruleEndPointPosition = r.EndPointPosition;

                            if (p.X < ruleBeginPointPosition.X && ruleBeginPointPosition.X < p.X + width
                                && p.Y < ruleBeginPointPosition.Y && ruleBeginPointPosition.Y < p.Y + height
                                &&
                                p.X < ruleEndPointPosition.X && ruleEndPointPosition.X < p.X + width
                                && p.Y < ruleEndPointPosition.Y && ruleEndPointPosition.Y < p.Y + height
                                )
                            {
                                AddSelectedControl(r);
                                r.IsSelectd = true;
                            }
                        }
                    }
                }
                cnsDesignerContainer.Children.Remove(temproaryEllipse);
                temproaryEllipse = null;
            }
        }

        void tbLabelName_LostFocus(object sender, RoutedEventArgs e)
        {
            _Service.DoNewLabelAsync(FlowID, (int)l.Position.X, (int)l.Position.Y, l.LabelName, l.LableID);
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    // 如果有节点在编辑状态,则delete按钮只能删除字符,不能删除选中的节点
                    if (IsSomeChildEditing)
                    {
                        return;
                    }
                    if (CurrentSelectedControlCollection != null && CurrentSelectedControlCollection.Count > 0)
                    {
                        if (System.Windows.Browser.HtmlPage.Window.Confirm("确定要删除吗？"))
                        {
                            DeleteSeletedControl();
                            SaveChange(HistoryType.New);
                        }
                    }
                    break;
                case Key.Up:
                    MoveUp();
                    break;
                case Key.Down:
                    MoveDown();
                    break;
                case Key.Left:
                    MoveLeft();
                    break;
                case Key.Right:
                    MoveRight();
                    break;
                default:
                    break;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    switch (e.Key)
            //    {
            //        case Key.Z:

            //            SaveChange(HistoryType.Previous);
            //            break;
            //        case Key.Y:
            //            SaveChange(HistoryType.Next);
            //            break;
            //        case Key.C:

            //            CopySelectedControlToMemory(null);
            //            break;
            //        case Key.V:

            //            PastMemoryToContainer();
            //            break;
            //        case Key.A:
            //            FlowNode a = null;
            //            Direction r = null;
            //            NodeLabel l = null;
            //            foreach (UIElement uie in cnsDesignerContainer.Children)
            //            {

            //                if (uie is FlowNode)
            //                {
            //                    a = uie as FlowNode;
            //                    a.IsSelectd = true;
            //                    AddSelectedControl(a);
            //                }

            //                if (uie is Direction)
            //                {
            //                    r = uie as Direction;
            //                    r.IsSelectd = true;
            //                    AddSelectedControl(r);
            //                }
            //                if (uie is NodeLabel)
            //                {
            //                    l = uie as NodeLabel;
            //                    l.IsSelectd = true;
            //                    AddSelectedControl(l);
            //                }

            //            }
            //            break;
            //        case Key.S: 

            //            Save();
            //            break;

            //    }
            //}
        }

        #endregion

        #region 构造方法
        public Container()
        {
            InitializeComponent();

            MessageBody.Visibility = Visibility.Collapsed;

            menuFlowNode.Container = this;
            menuLabel.Container = this;
            menuDirection.Container = this;
            menuDirection.Visibility = Visibility.Collapsed;
            menuFlowNode.Visibility = Visibility.Collapsed;
            menuLabel.Visibility = Visibility.Collapsed;
            menuContainer.Visibility = Visibility.Collapsed;
            menuContainer.Container = this;

            SetGridLines();

            HtmlPage.Document.AttachEvent("oncontextmenu", OnContextMenu);

            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, SystemConst.DoubleClickTime);
            _doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);

            cnsDesignerContainer.Height = Application.Current.Host.Content.ActualHeight;
            cnsDesignerContainer.Width = Application.Current.Host.Content.ActualWidth;

        }

        public Container(string flowid)
            : this()
        {
            this.FlowID = flowid;
        }
        #endregion
    }
}
