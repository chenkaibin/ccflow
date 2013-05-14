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
using Ccflow.Web.Component.Workflow;
using Silverlight;
using BP;

namespace BP
{
    public enum DirectionMoveType { Begin = 0, Line, End }

    public enum DirType
    {
        //前进线
        Forward = 0,
        //回退线
        Backward = 1,
        //虚拟线
        Virtual = 2
    }

    public delegate void DirectionChangeDelegate(Direction r);

    public partial class Direction  : UserControl, IElement
    {
        Point origBeginPoint ;
        Point origEndPoint; 
        Point origTurnPoint1Point;
        Point origTurnPoint2Point; 
        bool positionIsChange = true;

        /// <summary>
        /// 轨迹线
        /// </summary>
        private bool isTrackingLine = false;
        public bool IsTrackingLine
        {
            get { return isTrackingLine; }
            set
            {
                isTrackingLine = value;
                IsSelectd = false;
                //SolidColorBrush brush = SystemConst.ColorConst.TrackingColor.DirectionColor as SolidColorBrush;
                //begin.Fill = brush;
                //endArrow.Stroke = brush;
                //line.Stroke = brush;
                //if (LineType == DirectionLineType.Polyline)
                //{
                //    ruleTurnPoint1.Fill = brush;
                //    ruleTurnPoint2.Fill = brush;
                //}

#warning 从这里把路过的节点的边框设置成红色。
                BeginFlowNode.IsTrackingNode = true;
                EndFlowNode.IsTrackingNode = true;
                //BeginFlowNode.sdPicture.SetBorderColor(brush);
                //EndFlowNode.sdPicture.SetBorderColor(brush);
                //SolidColorBrush brush1 = new SolidColorBrush(Colors.White);
                //BeginFlowNode.sdPicture.Background = brush1;
                //EndFlowNode.sdPicture.Background = brush1;
                ////(EndFlowNode.sdPicture.currentPic as BP.Picture.OrdinaryNode).picRect.Stroke = brush;
                ////(BeginFlowNode.sdPicture.currentPic as BP.Picture.OrdinaryNode).picRect.Stroke = brush;             
            }
        }

        /// <summary>
        /// 是否为回退线
        /// </summary>
        //private bool isReturnType = false;
        //public bool IsReturnType
        //{
        //    get { return isReturnType; }
        //    set
        //    {
        //        isReturnType = value;
        //        if (value)
        //        {
        //            line.StrokeDashArray = new DoubleCollection() { 3, 1 };
        //            //d.line.StrokeDashCap = PenLineCap.Flat;
        //            //d.line.StrokeDashOffset = 1;
        //            //d.line.StrokeThickness = 1;
        //        }
        //        else
        //        {
        //            line.StrokeDashArray = null;
        //        }
        //    }
        //}

        /// <summary>
        /// 是否为回退线
        /// </summary>
        private DirType dirType = DirType.Forward;
        public DirType DirType
        {
            get { return dirType; }
            set
            {
                dirType = value;
                if (value == BP.DirType.Backward)
                {
                    //IsReturnType = true;
                    line.StrokeDashArray = new DoubleCollection() { 3, 1 };
                    //d.line.StrokeDashCap = PenLineCap.Flat;
                    //d.line.StrokeDashOffset = 1;
                    //d.line.StrokeThickness = 1;
                }
                else
                {
                    line.StrokeDashArray = null;
                }
            }
        }

        /// <summary>
        /// 是否可原路返回
        /// </summary>
        public bool IsCanBack { get; set; }

        public void Zoom(double zoomDeep)
        {
            if (positionIsChange)
            {
                origBeginPoint = BeginPointPosition;
                origEndPoint = EndPointPosition; 
                origTurnPoint1Point = DirectionTurnPoint1.CenterPosition;
                origTurnPoint2Point = DirectionTurnPoint2.CenterPosition;
                positionIsChange = false;
            }
            if(BeginFlowNode == null)
                 BeginPointPosition = new Point(origBeginPoint.X * zoomDeep, origBeginPoint.Y * zoomDeep);
            if(EndFlowNode == null)
                EndPointPosition = new Point(origEndPoint.X * zoomDeep, origEndPoint.Y * zoomDeep);
            if (LineType == DirectionLineType.Polyline)
            {
                DirectionTurnPoint1.CenterPosition = new Point(origTurnPoint1Point.X * zoomDeep, origTurnPoint1Point.Y * zoomDeep); 
                onDirectionTurn1PointMove(DirectionTurnPoint1.CenterPosition); 
                DirectionTurnPoint2.CenterPosition = new Point(origTurnPoint2Point.X * zoomDeep, origTurnPoint2Point.Y * zoomDeep); 
                onDirectionTurn2PointMove(DirectionTurnPoint2.CenterPosition);  
            } 
        }

        bool isPassCheck
        {
            set
            {
                Color color = Colors.Transparent;

                if (value)
                {
                    color = Color.FromArgb(255, 0, 128, 0);
                }
                else
                {
                    color = Color.FromArgb(255, 255, 0, 0);
                } 
                
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = color;
                begin.Fill = brush;
                endArrow.Stroke = brush;
                line.Stroke = brush;

                if (LineType == DirectionLineType.Polyline)
                {
                    ruleTurnPoint1.Fill = brush;
                    ruleTurnPoint2.Fill = brush;
                }
            }
        }

        void setDirectionNameControlPosition()
        {
            double top = 0;
            double left = 0;
            if (this.LineType == DirectionLineType.Line)
            {
                left = (BeginPointPosition.X + EndPointPosition.X) / 2;
                top = (BeginPointPosition.Y + EndPointPosition.Y) / 2;
            }
            else
            {
                left = (line.Points[1].X + line.Points[2].X) / 2;
                top = (line.Points[1].Y + line.Points[2].Y) / 2;
            }
            tbDirectionName.SetValue(Canvas.TopProperty, top - 15);
            tbDirectionName.SetValue(Canvas.LeftProperty, left - 10);
            _container.IsNeedSave = true;
        }

        public CheckResult CheckSave()
        {
            CheckResult cr = new CheckResult();
            cr.IsPass = true;

            if (BeginFlowNode == null && EndFlowNode == null)
            {
                cr.Message += "eee";
                //Text.Message_MustBeLinkToBeginAndEndFlowNode;
                cr.IsPass = false;
            }
            else
            {
                if (BeginFlowNode == null)
                {
                    cr.Message += "eddee";
                    //  cr.Message += Text.Message_MustBeLinkToBeginFlowNode;
                    cr.IsPass = false;
                }
                if (EndFlowNode == null)
                {
                    //cr.Message += Text.Message_MustBeLinkToEndFlowNode;
                    cr.IsPass = false;
                }
            }
            isPassCheck = cr.IsPass;
            if (!cr.IsPass)
            {
                errorTipControl.Visibility = Visibility.Visible;
                errorTipControl.ErrorMessage = cr.Message;
            }
            else
            {
                if (_errorTipControl != null)
                {
                    _errorTipControl.Visibility = Visibility.Collapsed;
                    cnDirectionContainer.Children.Remove(_errorTipControl);
                    _errorTipControl = null;
                }
            }
            return cr;
        }
        public void UpperZIndex()
        {
            ZIndex = _container.NextMaxIndex;
        }
        ErrorTip _errorTipControl;
        ErrorTip errorTipControl
        {
            get
            {
                if (_errorTipControl == null)
                {
                    _errorTipControl = new ErrorTip();
                    cnDirectionContainer.Children.Add(_errorTipControl);
                    _errorTipControl.ParentElement = this;
                    _errorTipControl.SetValue(Canvas.ZIndexProperty, 1);



                }
                if (LineType == DirectionLineType.Line)
                {
                    _errorTipControl.SetValue(Canvas.TopProperty, (EndPointPosition.Y + BeginPointPosition.Y) / 2 - 80);
                    _errorTipControl.SetValue(Canvas.LeftProperty, (EndPointPosition.X + BeginPointPosition.X) / 2);
                }
                else
                {
                    _errorTipControl.SetValue(Canvas.TopProperty, line.Points[1].Y - 80);
                    _errorTipControl.SetValue(Canvas.LeftProperty, line.Points[1].X);
                }
                return _errorTipControl;
            }
        }
        public Direction Clone()
        {
            Direction clone = new Direction(this._container);
            clone.originDirection = this;
            clone.DirectionData = new DirectionComponent();
            clone.DirectionData.LineType = this.DirectionData.LineType;
            clone.DirectionData.DirectionCondition = this.DirectionData.DirectionCondition;
            clone.DirectionData.DirectionName = this.DirectionData.DirectionName;
            clone.LineType = this.LineType;
            clone.setUIValueByDirectionData(clone.DirectionData);
            clone.BeginPointPosition = this.BeginPointPosition;
            clone.EndPointPosition = this.EndPointPosition;
            clone.ZIndex = this.ZIndex;
            if (LineType == DirectionLineType.Polyline)
            {
                clone.DirectionTurnPoint1.CenterPosition = this.DirectionTurnPoint1.CenterPosition;
                clone.DirectionTurnPoint2.CenterPosition = this.DirectionTurnPoint2.CenterPosition;
            }
            return clone;
        }

        DirectionLineType lineType = DirectionLineType.Line;
        public DirectionLineType LineType
        {
            get
            {
                return lineType;
            }
            set
            {
                bool isChange = false;
                if (lineType != value)
                {
                    isChange = true;
                }
                lineType = value;
                if (isChange)
                {
                    if (LineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(BeginPointPosition, EndPointPosition);
                    }
                    else
                    {
                        setTurnPointInitPosition();  
                        SetDirectionPosition(BeginPointPosition, EndPointPosition, DirectionTurnPoint1.CenterPosition, DirectionTurnPoint2.CenterPosition);
                    }
                }
            }
        }
        public void SetDirectionData(DirectionComponent ruleData)
        {
            bool isChanged = false;
            if (DirectionData.DirectionCondition != ruleData.DirectionCondition
               || DirectionData.DirectionName != ruleData.DirectionName
                || DirectionData.LineType != ruleData.LineType)
            {
                isChanged = true;
            }

            DirectionData = ruleData;
            setUIValueByDirectionData(ruleData);
            if (isChanged)
            {
                if (DirectionChanged != null)
                    DirectionChanged(this);
            }
        }

        void setUIValueByDirectionData(DirectionComponent ruleData)
        {
            LineType = (DirectionLineType)Enum.Parse(typeof(DirectionLineType), ruleData.LineType, true);
            tbDirectionName.Text = ruleData.DirectionName;
        }

        DirectionComponent getDirectionComponentFromServer(string ruleID)
        {
            DirectionComponent rc = new DirectionComponent();
            rc.DirectionID = this.DirectionID;
            rc.FlowID = this.FlowID;
            rc.DirectionCondition = "";
            rc.DirectionName = tbDirectionName.Text;
            rc.LineType = Enum.GetName(typeof(DirectionLineType), LineType);
            return rc;
        }
        DirectionComponent ruleData;
        public DirectionComponent DirectionData
        {
            get
            {
                if (ruleData == null)
                {
                    if (EditType == PageEditType.Add)
                    {
                        ruleData = new DirectionComponent();
                        ruleData.DirectionID = this.DirectionID;
                        ruleData.FlowID = this.FlowID;
                        ruleData.DirectionCondition = "";
                        ruleData.DirectionName = tbDirectionName.Text;
                        ruleData.LineType = LineType.ToString();

                    }
                    else if (EditType == PageEditType.Modify)
                    {
                        ruleData = getDirectionComponentFromServer(this.DirectionID);

                    }
                }
                return ruleData;
            }
            set
            {
                ruleData = value;
            }
        }


        PageEditType editType = PageEditType.None;
        public PageEditType EditType
        {
            get
            {
                return editType;
            }
            set
            {
                editType = value;
            }
        }

        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(Canvas.ZIndexProperty);

            }
            set
            {
                this.SetValue(Canvas.ZIndexProperty, value);
            }

        }
        public event DirectionChangeDelegate DirectionChanged;

        public WorkFlowElementType ElementType
        {
            get
            {
                return WorkFlowElementType.Direction;
            }
        }
        public string ToXmlString()
        {
            return "";
        }
        public void LoadFromXmlString(string xmlString)
        {
        }
        public bool CanShowMenu
        {
            get
            {
                return canShowMenu;
            }
            set
            {
                canShowMenu = value;
            }
        }
        bool canShowMenu = false;
        public delegate void DeleteDelegate(Direction r);
        public event DeleteDelegate DeleteDirection;


        bool isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }
        }

        public void Delete()
        {

            

            if (!isDeleted)
            {
                isDeleted = true;
                if (this.IsTemporaryDirection)
                {
                    sbBeginClose_Completed(null, null);
                }
                else
                {
                    sbBeginClose.Completed += new EventHandler(sbBeginClose_Completed);
                    sbBeginClose.Begin();
                }
            }

            try
            {
                _container._Service.DoDropLineAsync(int.Parse(this.BeginFlowNode.NodeID), int.Parse(this.EndFlowNode.NodeID));
            }
            catch { }
        }

        void sbBeginClose_Completed(object sender, EventArgs e)
        {
            if (this.EndFlowNode != null)
                this.EndFlowNode.RemoveEndDirection(this);
            if (this.BeginFlowNode != null)
                this.BeginFlowNode.RemoveBeginDirection(this);
            if (DeleteDirection != null)
                DeleteDirection(this);
            _container.RemoveDirection(this);

            //if (DirectionChanged != null)
            //    DirectionChanged(this);
        }

        public DirectionMoveType MoveType;

        FlowNode beginFlowNode;
        FlowNode endFlowNode;
        public FlowNode BeginFlowNode
        {
            get
            {
                return beginFlowNode;
            }
            set
            {

                beginFlowNode = value;
                if (beginFlowNode != null)
                {
                    beginFlowNode.AddBeginDirection(this);
                    beginFlowNode.FlowNodeMove += new MoveDelegate(OnFlowNodeMove);
                    OnFlowNodeMove(beginFlowNode, null);
                }
            }
        }
        public FlowNode EndFlowNode
        {
            get { return endFlowNode; }
            set
            {
                endFlowNode = value;

                if (endFlowNode != null)
                {
                    endFlowNode.AddEndDirection(this);
                    endFlowNode.FlowNodeMove += new MoveDelegate(OnFlowNodeMove);
                    OnFlowNodeMove(endFlowNode, null);
                }

            }
        }

        public Point GetPointPosition(DirectionMoveType MoveType)
        {
            Point p = new Point();
            if (MoveType == DirectionMoveType.Begin)
            {
                p.X = (double)begin.GetValue(Canvas.LeftProperty);
                p.Y = (double)begin.GetValue(Canvas.TopProperty);

            }
            else if (MoveType == DirectionMoveType.End)
            {
                p.X = (double)end.GetValue(Canvas.LeftProperty);
                p.Y = (double)end.GetValue(Canvas.TopProperty);

            }
            return p;
        }
        public Point GetCurrentMovedPointPosition()
        {

            return GetPointPosition(MoveType);
        }
        IContainer _container;
        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }
        System.Windows.Threading.DispatcherTimer _doubleClickTimer;
        public Direction(IContainer container)
            : this(container, false)
        {
        }
        public Direction(IContainer container, bool isTemporary):this(container,isTemporary, DirectionLineType.Line)
        {

        }
        public Direction(IContainer container, bool isTemporary,DirectionLineType lineType)
        {

            InitializeComponent(); 
            this.IsTemporaryDirection = isTemporary;
            LineType = lineType;
            editType = PageEditType.Add;
            _container = container;
            this.Name = FlowID;

            //spContentMenu.Visibility = Visibility.Collapsed;
            System.Windows.Browser.HtmlPage.Document.AttachEvent("oncontextmenu", OnContextMenu);

            beginPointRadius = begin.Width / 2;
            begin.Height = begin.Width;

            endPointRadius = endEllipse.Width / 2;
            endEllipse.Height = endEllipse.Width;

            endArrow.SetValue(Canvas.TopProperty, endPointRadius);
            endArrow.SetValue(Canvas.LeftProperty, endPointRadius);

            if (LineType == DirectionLineType.Line)
            {
                SetDirectionPosition(new Point(0, 0), new Point(50, 50));
            }
            else
            {
                SetDirectionPosition(new Point(0, 0), new Point(50, 50), DirectionTurnPoint1.CenterPosition, DirectionTurnPoint2.CenterPosition);

            }


            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, SystemConst.DoubleClickTime);
            _doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);
            if (!this.IsTemporaryDirection)
            {
                sbBeginDisplay.Begin();
            }

        }
        void DoubleClick_Timer(object sender, EventArgs e)
        {
            _doubleClickTimer.Stop();
        }
        public Point BeginPointPosition
        {
            get
            {
                return GetPointPosition(DirectionMoveType.Begin);
            }
            set
            {
                if (value != null && !double.IsNaN(value.X) && !double.IsNaN(value.Y))
                {
                    begin.SetValue(Canvas.TopProperty, value.Y);
                    begin.SetValue(Canvas.LeftProperty, value.X);
                    if (LineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(BeginPointPosition, EndPointPosition);
                    }
                    else
                    {
                        SetDirectionPosition(BeginPointPosition, EndPointPosition, DirectionTurnPoint1.CenterPosition, DirectionTurnPoint2.CenterPosition);

                    }
                }

            }
        }
        Direction originDirection;
        public Direction OriginDirection
        {
            get
            {
                return originDirection;
            }
            set
            {
                originDirection = value;
            }

        }


        public Point EndPointPosition
        {
            get
            {
                return GetPointPosition(DirectionMoveType.End);

            }
            set
            {
                if (value != null && !double.IsNaN(value.X) && !double.IsNaN(value.Y))
                {
                    end.SetValue(Canvas.TopProperty, value.Y);
                    end.SetValue(Canvas.LeftProperty, value.X);
                    if (LineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(BeginPointPosition, EndPointPosition);
                    }
                    else
                    {
                        SetDirectionPosition(BeginPointPosition, EndPointPosition, DirectionTurnPoint1.CenterPosition, DirectionTurnPoint2.CenterPosition);

                    }
                }
            }
        }
        double beginPointRadius = 5;
        double endPointRadius = 5;
        private void OnContextMenu(object sender, System.Windows.Browser.HtmlEventArgs e)
        {

            //if (_container.MouseIsInContainer)
            //{
            //    e.PreventDefault();

            //    if (canShowMenu && !IsDeleted)
            //    {
            //        _container.ShowDirectionContentMenu(this, sender, e);

            //    }

            //}


        }
        string flowID;
        public string FlowID
        {
            get
            {
                if (string.IsNullOrEmpty(flowID))
                {
                    flowID = Guid.NewGuid().ToString();
                }
                return flowID;
            }
            set
            {
               flowID = value;
            }

        }
        string ruleID;
        public string DirectionID
        {
            get
            {

                ruleID = Guid.NewGuid().ToString();
                return ruleID;
            }
            set
            {
                ruleID = value;
            }

        }
        string ruleName;
        public string DirectionName
        {
            get
            {

                return ruleName;
            }
            set
            {
                ruleName = value;
                tbDirectionName.Text = value;
                DirectionData.DirectionName = value;
            }

        }

        public void RemoveBeginFlowNode(FlowNode a)
        {
            if (BeginFlowNode == a)
                BeginFlowNode = null;
            //需要删除事件代理 
        }

        public Point GetResetPoint(Point beginPoint, Point endPoint, FlowNode a, DirectionMoveType type)
        {
            Point p = a.GetPointOfIntersection(beginPoint, endPoint, type);
            return p;
        }

        void OnFlowNodeMove(FlowNode a, MouseEventArgs e)
        {

            if (a != EndFlowNode && a != BeginFlowNode)
                return;

            double newTop = (double)a.GetValue(Canvas.TopProperty);
            double newLeft = (double)a.GetValue(Canvas.LeftProperty);
            newTop = newTop + a.Height / 2;
            newLeft = newLeft + a.Width / 2;
            Point beginPoint = new Point();
            Point endPoint = new Point();


            beginPoint = BeginPointPosition;
            endPoint = EndPointPosition;
            if (EndFlowNode == a)
            {

                endPoint.X = (double)(newLeft - endPointRadius);
                endPoint.Y = (double)(newTop - endPointRadius);

                if (LineType == DirectionLineType.Line)
                {

                    endPoint = GetResetPoint(beginPoint, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);
                }
                else
                {
                    endPoint = GetResetPoint(DirectionTurnPoint2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);

                }

                if (BeginFlowNode != null)
                {
                    if (LineType == DirectionLineType.Line)
                    {
                        beginPoint = GetResetPoint(EndFlowNode.CenterPoint, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMoveType.Begin);
                    }
                    else
                    {
                        beginPoint = GetResetPoint(DirectionTurnPoint1.CenterPosition, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMoveType.Begin);

                    }

                }


            }
            else if (BeginFlowNode == a)
            {

                beginPoint.X = (double)(newLeft - beginPointRadius);
                beginPoint.Y = (double)(newTop - beginPointRadius);

                if (LineType == DirectionLineType.Line)
                {

                    beginPoint = GetResetPoint(endPoint, BeginFlowNode.CenterPoint, a, DirectionMoveType.Begin);
                }
                else
                {
                    beginPoint = GetResetPoint(DirectionTurnPoint1.CenterPosition, BeginFlowNode.CenterPoint, a, DirectionMoveType.Begin);

                }
                if (EndFlowNode != null)
                {
                    if (LineType == DirectionLineType.Line)
                    {
                        endPoint = GetResetPoint(BeginFlowNode.CenterPoint, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);
                    }
                    else
                    {
                        endPoint = GetResetPoint(DirectionTurnPoint2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);

                    }

                }
            }

            if (LineType == DirectionLineType.Line)
            {
                SetDirectionPosition(beginPoint, endPoint);

            }
            else
            {
                SetDirectionPosition(beginPoint, endPoint, DirectionTurnPoint1.CenterPosition, DirectionTurnPoint2.CenterPosition);

            }
        }

        public void RemoveEndFlowNode(FlowNode a)
        {
            if (EndFlowNode == a)
                EndFlowNode = null;
            //需要删除事件代理 
        }

        bool trackingLineMouseMove = false;
        Point mousePosition;
        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pointHadActualMove = false;
            trackingPointMouseMove = false;
            this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
            FrameworkElement element = sender as FrameworkElement;

            if (element.Name == "begin")
                MoveType = DirectionMoveType.Begin;
            if (element.Name == "end")
                MoveType = DirectionMoveType.End;
            if (element.Name == "line")
                MoveType = DirectionMoveType.Line;

            mousePosition = e.GetPosition(null);
            if (null != element)
            {

                trackingPointMouseMove = true;
                element.CaptureMouse();
                element.Cursor = Cursors.Hand;

            }
            e.Handled = true;


        }
        public void ShowMessage(string message)
        {

            _container.ShowMessage(message);
        }
        void ruleChange()
        {
            if (DirectionChanged != null)
                DirectionChanged(this);
        }

        void setLinkToFlowNode(DirectionMoveType movetype)
        {
            double centerX = 0;
            double centerY = 0;

            if (movetype == DirectionMoveType.Begin)
            {
                centerX = BeginPointPosition.X + beginPointRadius;
                centerY = BeginPointPosition.Y + beginPointRadius;

            }
            if (movetype == DirectionMoveType.End)
            {
                centerX = EndPointPosition.X + endPointRadius;
                centerY = EndPointPosition.Y + endPointRadius;

            }

            FlowNode act = null;
            bool isLinked = false;
            for (int i = 0; i < _container.FlowNodeCollections.Count; i++)
            {

                if (isLinked)
                    break;
                act = _container.FlowNodeCollections[i];

                if (act.PointIsInside(new Point(centerX, centerY)))
                {
                    if (movetype == DirectionMoveType.Begin)
                    {
                        #region 检查
                        //if (act.Type == FlowNodeType.COMPLETION)
                        //{
                        //    act.Type = FlowNodeType.INTERACTION;
                        //    //ShowMessage(Text.Message_EndFlowNodeCanNotHaveFollowUpActivitiy);
                        //    //isLinked = false;
                        //    //break;
                        //}
                        //else if ((act.Type == FlowNodeType.AND_MERGE
                        //    || act.Type == FlowNodeType.OR_MERGE
                        //    || act.Type == FlowNodeType.VOTE_MERGE)
                        //    && act.BeginDirectionCollections != null)
                        //{

                        //    int count = act.BeginDirectionCollections.Count;
                        //    if (act.BeginDirectionCollections.Contains(this))
                        //        count--;
                        //    if (count > 0)
                        //    {
                        //        //ShowMessage(Text.Message_SubThreadNodeSheetnlyHaveAFollowUpFlowNode);
                        //        //isLinked = false;
                        //        //break;
                        //    }
                        //}
                        #endregion
                        if (this.EndFlowNode == act)
                        {
                            //if (!IsTemporaryDirection)
//                                ShowMessage(Text.Message_BeginAndEndFlowNodeCanNotBeTheSame);
                            //ShowMessage("错误");
                        }
                        else
                        {
                            if (this.EndFlowNode == null)
                            {
                                act.AddBeginDirection(this);
                                if (this.IsTemporaryDirection)
                                    this.DirectionName = "NewDirection" + _container.NextNewDirectionIndex.ToString();
                                isLinked = true;
                            }
                            else
                            {
                                bool isExists = false;
                                if (act.BeginDirectionCollections != null)
                                {
                                    for (int j = 0; j < act.BeginDirectionCollections.Count; j++)
                                    {
                                        if (act.BeginDirectionCollections[j].EndFlowNode == this.EndFlowNode
                                           && act.BeginDirectionCollections[j].BeginFlowNode != this.BeginFlowNode
                                            )
                                        {
                                            isExists = true;
                                            break;
                                        }
                                    }
                                }
                                if (isExists)
                                {
                                    ShowMessage("方向已存在");
                                }
                                else
                                {
                                    act.AddBeginDirection(this);
                                    if (this.IsTemporaryDirection)
                                        this.DirectionName = "New Direction" + _container.NextNewDirectionIndex.ToString();

                                    isLinked = true;
                                }
                            }
                        }
                    }
                    if (movetype == DirectionMoveType.End)
                    {
                        #region 检查

                        if (this.IsTemporaryDirection)
                        {
                            if (this.BeginFlowNode == act)
                            {
                                isLinked = false;
                                break;
                            }
                        }

                        if (act.HisPosType == NodePosType.Start )
                        {
                            //开始节点不能有前驱节点
                          //  ShowMessage(Text.Message_BeginActivitiesCanNotHavePreFlowNode);
                            isLinked = false;
                            break;
                        }
                        //else if ((act.Type == FlowNodeType.AND_BRANCH
                        //   || act.Type == FlowNodeType.OR_BRANCH)
                        //   )
                        //{
                        //    if (act.EndDirectionCollections != null
                        //    && act.EndDirectionCollections.Count > 0)
                        //    {
                        //        int count = act.EndDirectionCollections.Count;
                        //        if (act.EndDirectionCollections.Contains(this))
                        //            count--;
                        //        if (count > 0)
                        //        {
                        //            act.AddEndDirection(this);
                        //            if (this.IsTemporaryDirection)
                        //                this.DirectionName = Text.NewDirection + _container.NextNewDirectionIndex.ToString();

                        //            isLinked = true;
                        //            //分支节点有且只能有一个前驱节点
                        //            //ShowMessage(Text.Message_HeLiuNodeSheetnlyHaveOnePreFlowNode);
                        //            //isLinked = false;
                        //            break;
                        //        }
                        //    }
                        //}
                        if (this.IsTemporaryDirection)
                        {
                            if (this.BeginFlowNode != null)
                            {
                                //if (this.BeginFlowNode.Type == FlowNodeType.COMPLETION)
                                //{
                                //    this.beginFlowNode.Type = FlowNodeType.INTERACTION;
                                //    //ShowMessage(Text.Message_EndFlowNodeCanNotHaveFollowUpActivitiy);
                                //    //isLinked = false;
                                //    //break;
                                //}
                                //if ((this.BeginFlowNode.Type == FlowNodeType.AND_MERGE
                                //    || this.BeginFlowNode.Type == FlowNodeType.OR_MERGE
                                //    || this.BeginFlowNode.Type == FlowNodeType.VOTE_MERGE)
                                //&& this.BeginFlowNode.BeginDirectionCollections != null)
                                //{
                                //    int count = BeginFlowNode.BeginDirectionCollections.Count;
                                //    if (this.BeginFlowNode.BeginDirectionCollections.Contains(this))
                                //        count--;
                                //    if (count > 0)
                                //    {
                                //        ////汇聚节点只能有一个后继节点
                                //        //ShowMessage(Text.Message_SubThreadNodeSheetnlyHaveAFollowUpFlowNode);
                                //        //isLinked = false;
                                //        //break;
                                //    }
                                //}
                            }

                        }


                        #endregion
                        if (this.BeginFlowNode == act)
                        {
                            //if (!IsTemporaryDirection)
                            //    ShowMessage(Text.Message_BeginAndEndFlowNodeCanNotBeTheSame);
                        }
                        else
                        {
                            if (this.BeginFlowNode == null)
                            {
                                act.AddEndDirection(this);
                                if (this.IsTemporaryDirection)
                                    this.DirectionName = "NewDirection" + _container.NextNewDirectionIndex.ToString();

                                isLinked = true;
                            }
                            else
                            {
                                bool isExists = false;
                                if (act.EndDirectionCollections != null)
                                {
                                    for (int j = 0; j < act.EndDirectionCollections.Count; j++)
                                    {
                                        if (act.EndDirectionCollections[j].BeginFlowNode == this.BeginFlowNode

                                           && act.EndDirectionCollections[j].EndFlowNode != this.EndFlowNode
                                            )
                                        {
                                            isExists = true;
                                            break;
                                        }
                                    }
                                }
                                if (isExists)
                                {
                                   // ShowMessage(Text.Message_TheSameDirectionThatAlreadyExist);
                                }
                                else
                                {
                                    act.AddEndDirection(this);
                                    if (this.IsTemporaryDirection)
                                       // this.DirectionName = Text.NewDirection + _container.NextNewDirectionIndex.ToString();

                                    isLinked = true;
                                }


                            }
                        }

                    }
                }
            }


        }
        public void SimulateDirectionPointMouseLeftButtonUpEvent(DirectionMoveType moveType, object sender, MouseButtonEventArgs e)
        {
            MoveType = moveType;
            Point_MouseLeftButtonUp(sender, e);
        }
        bool hadActualMove = false;
        public bool isTemplateDirection = false;
        public bool IsTemporaryDirection
        {
            get
            {
                return isTemplateDirection;
            }
            set
            {
                isTemplateDirection = value;
                if (value)
                {
                    DoubleCollection d = new DoubleCollection();
                    d.Add(1);
                    line.StrokeDashArray = d;
                }
                else
                {
                    if (DirType == BP.DirType.Backward)
                    {
                        line.StrokeDashArray = new DoubleCollection() { 3, 1 };
                        //line.StrokeDashCap = PenLineCap.Flat;
                        //line.StrokeDashOffset = 1;
                    }
                    else
                    {
                        line.StrokeDashArray = null;
                    }
                }
            }
        }

        bool pointHadActualMove = false;
        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection != null)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;

            trackingPointMouseMove = false;

            if (!_container.CurrentSelectedControlCollection.Contains(this))
            {
                FrameworkElement element = sender as FrameworkElement;
                element.ReleaseMouseCapture();

                mousePosition.X = mousePosition.Y = 0;
                element.Cursor = null;

                Point centerPoint = new Point();

                FlowNode a = null;

                if (MoveType == DirectionMoveType.Begin)
                {
                    centerPoint.X = BeginPointPosition.X + beginPointRadius;
                    centerPoint.Y = BeginPointPosition.Y + beginPointRadius;
                    if (BeginFlowNode != null)
                        a = BeginFlowNode;
                }
                if (MoveType == DirectionMoveType.End)
                {
                    centerPoint.X = EndPointPosition.X + endPointRadius;
                    centerPoint.Y = EndPointPosition.Y + endPointRadius;
                    if (EndFlowNode != null)
                        a = EndFlowNode;
                }


                if (a != null)
                {


                    //移去原来的关联
                    if (!a.PointIsInside(centerPoint))
                    {
                        if (MoveType == DirectionMoveType.Begin
                             && BeginFlowNode != null)
                        {
                            BeginFlowNode = null;
                            a.RemoveBeginDirection(this);

                        }
                        if (MoveType == DirectionMoveType.End
                           && EndFlowNode != null)
                        {
                            EndFlowNode = null;
                            a.RemoveEndDirection(this);

                        }
                    }
                }

                setLinkToFlowNode(MoveType);


                if (pointHadActualMove)
                    ruleChange();
            }
            if (!pointHadActualMove && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SetWorkFlowElementSelected(this, IsSelectd);
            }
        }
        public void SetDirectionPosition(Point beginPoint, Point endPoint)
        {
            SetDirectionPosition(beginPoint, endPoint,null,null);
        }
        public void SetDirectionPosition(Point beginPoint, Point endPoint, Point? turnPoint1, Point? turnPoint2)
        {
            if (double.IsNaN(beginPoint.X)
                || double.IsNaN(beginPoint.Y)
                || double.IsNaN(endPoint.X)
                || double.IsNaN(endPoint.Y)
                )
                return;


            begin.SetValue(Canvas.LeftProperty, beginPoint.X);
            begin.SetValue(Canvas.TopProperty, beginPoint.Y);

            end.SetValue(Canvas.LeftProperty, endPoint.X);
            end.SetValue(Canvas.TopProperty, endPoint.Y);



            Point p1 = new Point(beginPoint.X + beginPointRadius, beginPoint.Y + beginPointRadius);
            Point p4 = new Point(endPoint.X + endPointRadius, endPoint.Y + endPointRadius);

            Point p2 = new Point();
            Point p3 = new Point();

            if (LineType == DirectionLineType.Line)
            {
                p2 = p1;
                p3 = p1;

                if (ruleTurnPoint1 != null)
                    ruleTurnPoint1.Visibility = Visibility.Collapsed;

                if (ruleTurnPoint2 != null)
                    ruleTurnPoint2.Visibility = Visibility.Collapsed;
            }
            else
            {

                DirectionTurnPoint1.Visibility = Visibility.Visible;
                DirectionTurnPoint2.Visibility = Visibility.Visible;
                if (turnPoint1 != null && turnPoint2 != null)
                {

                    DirectionTurnPoint1.CenterPosition = turnPoint1.Value;
                    DirectionTurnPoint2.CenterPosition = turnPoint2.Value;
                    p2 = DirectionTurnPoint1.CenterPosition;
                    p3 = DirectionTurnPoint2.CenterPosition;
                }
                else
                {  

                    if (!trackingLineMouseMove)
                    {
                        if (TurnPoint1HadMoved)
                        {
                            p2 = DirectionTurnPoint1.CenterPosition;
                        } if (TurnPoint2HadMoved)
                        {
                            p3 = DirectionTurnPoint2.CenterPosition;
                        }
                    }


                    DirectionTurnPoint1.CenterPosition = p2;
                    DirectionTurnPoint2.CenterPosition = p3;
                }

            }

            line.Points.Clear();
            line.Points.Add(p1);
            line.Points.Add(p2);
            line.Points.Add(p3);
            line.Points.Add(p4);


            endArrow.SetAngleByPoint(p3, p4);
            setDirectionNameControlPosition();
        }

        void ruleTurnPoint1_DirectionTurnPointMove(object sender, MouseEventArgs e, Point newPoint)
        {
            // line.Points.Clear();
            positionIsChange = true;
            onDirectionTurn1PointMove(newPoint);
        }

        void onDirectionTurn1PointMove(Point newPoint)
        {
            TurnPoint1HadMoved = true;
            line.Points[1] = newPoint;
            if (BeginFlowNode != null)
            {
                this.BeginPointPosition = this.GetResetPoint(DirectionTurnPoint1.CenterPosition, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMoveType.Begin);

            }
            setDirectionNameControlPosition();
        }

        void ruleTurnPoint2_DirectionTurnPointMove(object sender, MouseEventArgs e, Point newPoint)
        {
            // line.Points.Clear(); 
            positionIsChange = true;

            onDirectionTurn2PointMove(newPoint);
        }
        void onDirectionTurn2PointMove(Point newPoint)
        {
            line.Points[2] = newPoint;
            TurnPoint2HadMoved = true;

            if (EndFlowNode != null)
            {
                this.EndPointPosition = this.GetResetPoint(DirectionTurnPoint2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);

            }
            endArrow.SetAngleByPoint(line.Points[2], line.Points[3]);

            setDirectionNameControlPosition();
        }

        DirectionTurnPoint ruleTurnPoint2;
        public DirectionTurnPoint DirectionTurnPoint2
        {
            get
            {
                if (ruleTurnPoint2 == null)
                {
                    ruleTurnPoint2 = new DirectionTurnPoint();
                    // ruleTurnPoint2.Visibility = Visibility.Collapsed;

                    cnDirectionContainer.Children.Add(ruleTurnPoint2);
                    ruleTurnPoint2.SetValue(Canvas.ZIndexProperty, 200);

                    ruleTurnPoint2.DirectionTurnPointMove += new DirectionTurnPoint.DirectionTurnPointMoveDelegate(ruleTurnPoint2_DirectionTurnPointMove);
                    ruleTurnPoint2.OnDoubleClick += new DirectionTurnPoint.DoubleClickDelegate(ruleTurnPoint2_OnDoubleClick);
                     

                }
                return ruleTurnPoint2;
            }
        }
        
        DirectionTurnPoint ruleTurnPoint1;
        public DirectionTurnPoint DirectionTurnPoint1
        {
            get
            {
                if (ruleTurnPoint1 == null)
                {
                    ruleTurnPoint1 = new DirectionTurnPoint();
                    // ruleTurnPoint1.Visibility = Visibility.Collapsed;
                    cnDirectionContainer.Children.Add(ruleTurnPoint1);
                    ruleTurnPoint1.SetValue(Canvas.ZIndexProperty, 200);

                    ruleTurnPoint1.DirectionTurnPointMove += new DirectionTurnPoint.DirectionTurnPointMoveDelegate(ruleTurnPoint1_DirectionTurnPointMove);
                    ruleTurnPoint1.OnDoubleClick += new DirectionTurnPoint.DoubleClickDelegate(ruleTurnPoint1_OnDoubleClick);
                }
                return ruleTurnPoint1;
            }
        }

        void ruleTurnPoint1_OnDoubleClick(object sender, EventArgs e)
        {
            Point p = new Point();
            p.X = (BeginPointPosition.X + ruleTurnPoint2.CenterPosition.X) / 2;
            p.Y = (BeginPointPosition.Y + ruleTurnPoint2.CenterPosition.Y) / 2;
            SetDirectionPosition(BeginPointPosition, EndPointPosition,p, ruleTurnPoint2.CenterPosition);
        }
        void ruleTurnPoint2_OnDoubleClick(object sender, EventArgs e)
        {
            Point p = new Point();
            p.X = (EndPointPosition.X + ruleTurnPoint1.CenterPosition.X) / 2;
            p.Y = (EndPointPosition.Y + ruleTurnPoint1.CenterPosition.Y) / 2;
            SetDirectionPosition(BeginPointPosition, EndPointPosition, ruleTurnPoint1.CenterPosition,p);
        }
        void setTurnPointInitPosition()
        {
            Point p2 = new Point();
            Point p3 = new Point();
            Point p1 = new Point(BeginPointPosition.X + beginPointRadius, BeginPointPosition.Y + beginPointRadius);
            Point p4 = new Point(EndPointPosition.X + endPointRadius, EndPointPosition.Y + endPointRadius);
           
            p2.X = (p1.X + p4.X) / 2;
            p2.Y = p1.Y;

            p3.X = p2.X;
            p3.Y = p4.Y;

            DirectionTurnPoint1.CenterPosition = p2;
            DirectionTurnPoint2.CenterPosition = p3;
            IsSelectd = IsSelectd;
        }

        public bool TurnPoint1HadMoved = false;
        public bool TurnPoint2HadMoved = false;
        public void SetPositionByDisplacement(double x, double y)
        {
            if (BeginFlowNode != null && EndFlowNode != null
                            && !_container.CurrentSelectedControlCollection.Contains(BeginFlowNode)
                            && !_container.CurrentSelectedControlCollection.Contains(EndFlowNode)
                            )
            {
            }
            else if (BeginFlowNode != null && EndFlowNode == null
                           && !_container.CurrentSelectedControlCollection.Contains(BeginFlowNode)
                            )
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMoveType.End);

            }
            else if (BeginFlowNode == null && EndFlowNode != null
                   && !_container.CurrentSelectedControlCollection.Contains(EndFlowNode)
                    )
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMoveType.Begin);

            }
            else
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMoveType.Line);

            }
        }
        public void SetDirectionPositionByDisplacement(double x, double y, DirectionMoveType moveType)
        {
            Point beginPoint = BeginPointPosition;
            Point endPoint = EndPointPosition;


            if (moveType == DirectionMoveType.Begin || moveType == DirectionMoveType.Line)
            {
                beginPoint.X += x;
                beginPoint.Y += y;
            }

            if (moveType == DirectionMoveType.End || moveType == DirectionMoveType.Line)
            {
                endPoint.X += x;
                endPoint.Y += y;
            }

            if (LineType == DirectionLineType.Line)
            {
                SetDirectionPosition(beginPoint, endPoint);

            }
            else
            {
                SetDirectionPosition(beginPoint, endPoint, 
                    new Point(DirectionTurnPoint1.CenterPosition.X + x , DirectionTurnPoint1.CenterPosition.Y + y ), 
                    new Point(DirectionTurnPoint2.CenterPosition.X + x , DirectionTurnPoint2.CenterPosition.Y + y ));

            }

        }

        bool isSelectd = false;
        public bool IsSelectd
        {
            get
            {
                return isSelectd;
            }
            set
            {
                isSelectd = value;
                _container.SelectDirection(this, value);
                ////if ((this.line.Stroke as SolidColorBrush).Color.ToString() == Colors.Red.ToString())
                ////    return;

                //SolidColorBrush brush = new SolidColorBrush();

                //if (IsTrackingLine)
                //{
                //    if (isSelectd)
                //    {
                //        brush.Color = Color.FromArgb(255, 255, 0, 255);
                //        if (!_container.CurrentSelectedControlCollection.Contains(this))
                //            _container.AddSelectedControl(this);
                //    }
                //    else
                //    {
                //        brush.Color = Colors.Red;
                //    }
                //}
                //else
                //{
                    if (isSelectd)
                    {
                        //brush.Color = Color.FromArgb(255, 255, 181, 0);
                        if (!_container.CurrentSelectedControlCollection.Contains(this))
                            _container.AddSelectedControl(this);
                    }
                    else
                    {
                        //brush.Color = Color.FromArgb(255, 0, 128, 0);
                    }

                //}

                //begin.Fill = brush;
                //endArrow.Stroke = brush;
                //line.Stroke = brush;
                //if (LineType == DirectionLineType.Polyline)
                //{
                //    ruleTurnPoint1.Fill = brush;
                //    ruleTurnPoint2.Fill = brush;
                //}
            }
        }

        bool trackingPointMouseMove = false;
        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingPointMouseMove)
            {
                if (e.GetPosition(null) == mousePosition)
                    return;
                if (_container.CurrentSelectedControlCollection.Contains(this))
                {
                    if (MoveType == DirectionMoveType.Begin && BeginFlowNode != null
                        && !_container.CurrentSelectedControlCollection.Contains(this.BeginFlowNode))
                    {
                        pointHadActualMove = false;
                        mousePosition = e.GetPosition(null);
                        return;
                    }
                    if (MoveType == DirectionMoveType.End && EndFlowNode != null
                        && !_container.CurrentSelectedControlCollection.Contains(this.EndFlowNode))
                    {
                        pointHadActualMove = false;
                        mousePosition = e.GetPosition(null);
                        return;
                    }
                }

                FrameworkElement element = sender as FrameworkElement;
                Point currentPoint = e.GetPosition(this);

                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;

                double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);

                if (currentPoint.X > containerWidth
                   || currentPoint.Y > containerHeight
                    || currentPoint.X < 0
                    || currentPoint.Y < 0
                    )
                {
                    //超过流程容器的范围


                }
                else
                {

                    positionIsChange = true;
                    if (_container.CurrentSelectedControlCollection.Contains(this))
                    {
                        SetPositionByDisplacement(deltaH, deltaV); 

                    }
                    else
                    {

                        if (MoveType == DirectionMoveType.Begin)
                        {
                            this.BeginPointPosition = currentPoint;

                            if (EndFlowNode != null)
                            {
                                if (LineType == DirectionLineType.Line)
                                {
                                    this.EndPointPosition = this.GetResetPoint(currentPoint, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);
                                }
                                else
                                {
                                    this.EndPointPosition = this.GetResetPoint(DirectionTurnPoint2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMoveType.End);

                                }
                            }


                        }
                        else if (MoveType == DirectionMoveType.End)
                        {
                            this.EndPointPosition = currentPoint;

                            if (BeginFlowNode != null)
                            {
                                if (LineType == DirectionLineType.Line)
                                {
                                    this.BeginPointPosition = this.GetResetPoint(currentPoint, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMoveType.Begin);
                                }
                                else
                                {
                                    this.BeginPointPosition = this.GetResetPoint(DirectionTurnPoint1.CenterPosition, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMoveType.Begin);

                                }
                            }
                        }
                    }

                }
                pointHadActualMove = true;
                _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
                mousePosition = e.GetPosition(null);
            }

        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            hadActualMove = false;
            trackingLineMouseMove = false;
            if (_doubleClickTimer.IsEnabled)
            {
                _doubleClickTimer.Stop();
                //spContentMenu.Visibility = Visibility.Collapsed;
                if (this.IsTrackingLine && this.DirType == BP.DirType.Backward 
                    || !this.IsTrackingLine && this.DirType == BP.DirType.Forward)
                {
                    _container.ShowDirectionSetting(this);
                }
            }
            else
            {
                _doubleClickTimer.Start();
                this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
                FrameworkElement element = sender as FrameworkElement;
                mousePosition = e.GetPosition(null);
                if (null != element)
                {
                    trackingLineMouseMove = true;
                    element.CaptureMouse();
                    element.Cursor = Cursors.Hand;

                }
            }
            e.Handled = true;

        }

        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection !=null )
            {
                e.Handled = false;

            }
            else
                e.Handled = true;

            if (!hadActualMove && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SetWorkFlowElementSelected(this, IsSelectd);
            }
            trackingLineMouseMove = false;
            FrameworkElement element = sender as FrameworkElement;
            element.ReleaseMouseCapture();

            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;

            setLinkToFlowNode(DirectionMoveType.Begin);
            setLinkToFlowNode(DirectionMoveType.End);

            if (hadActualMove)
                ruleChange();
        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {


            if (trackingLineMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;


                if (BeginFlowNode != null && EndFlowNode != null
                    && !_container.CurrentSelectedControlCollection.Contains(BeginFlowNode)
                    && !_container.CurrentSelectedControlCollection.Contains(EndFlowNode)
                    )
                    return;
                if (mousePosition == e.GetPosition(null))
                    return;
                hadActualMove = true;

                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;

                double newLeft = e.GetPosition((FrameworkElement)this.Parent).X;
                double newTop = e.GetPosition((FrameworkElement)this.Parent).Y;

                double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);

                if (containerWidth < newLeft || containerWidth < newTop
                    || newLeft < 0 || newTop < 0
                    )
                {
                    //超过流程容器的范围

                }

                else
                {
                    positionIsChange = true;

                    SetPositionByDisplacement(deltaH, deltaV); 
                    _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);  
                }
                mousePosition = e.GetPosition(null);
            }
        } 

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            canShowMenu = true;
            //ttDirectionTip.Content = DirectionData.DirectionName + "\r\n" + DirectionData.DirectionCondition;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            canShowMenu = false;
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (_container.MouseIsInContainer)
            {
                e.GetPosition(null);

                if (canShowMenu && !IsDeleted)
                {
                    _container.ShowDirectionContentMenu(this, sender, e);
                }
            }
        }
        public void Edit() 
        {
            _container.ShowDirectionSetting(this);
        }
        #region IElement 成员

        public UserStation Station()
        {
            return null;
        }
       
        /// <summary>
        /// 轨迹数据
        /// </summary>
        /// <param name="trackDataSet"></param>
        public bool MarkRed(DataSet trackDataSet)
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Red;
            if (trackDataSet == null || trackDataSet.Tables.Count == 0)
                return false;

            DataTable dt = trackDataSet.Tables["WF_Track"];
            foreach (DataRow dr in dt.Rows)
            {
                string begin = dr["NDFrom"].ToString();
                string to = dr["NDTo"].ToString();

                if (this.BeginFlowNode.NodeID == begin && this.EndFlowNode.NodeID == to)
                {
                    brush = new SolidColorBrush();
                    brush.Color = Colors.Red;
                    this.begin.Fill = brush;
                    this.endArrow.Stroke = brush;
                    this.line.Stroke = brush;

                    this.BeginFlowNode.BorderBrush = brush;
                    this.EndFlowNode.BorderBrush = brush;
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
