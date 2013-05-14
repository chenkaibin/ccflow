using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ccflow.Web.Component.Workflow;
using WF.WS;
using Silverlight;
using WF.Designer;
using BP.Picture;
using System.ComponentModel;
using BP;

namespace BP
{
    public delegate void MoveDelegate(FlowNode a, MouseEventArgs e);

    public delegate void DeleteDelegate(FlowNode a);

    public delegate void FlowNodeChangeDelegate(FlowNode a);

    public partial class FlowNode : UserControl, IElement
    {
        #region Variables
        double origPictureWidth = 0;
        double origPictureHeight = 0;
        Point origPosition;
        bool positionIsChange = true;
        System.Windows.Threading.DispatcherTimer _doubleClickTimer;
        #endregion

        #region Properties

        public List<Direction> BeginDirectionCollections = new List<Direction>();

        public List<Direction> EndDirectionCollections = new List<Direction>();

        public double PictureWidth
        {
            get
            {
                return sdPicture.PictureWidth;
            }
        }

        public double PictureHeight
        {
            get
            {
                return sdPicture.PictureHeight;
            }
        }

        bool isPassCheck
        {
            set
            {
                if (value)
                {

                    sdPicture.ResetInitColor();
                }
                else
                {
                    sdPicture.SetWarningColor();
                }
            }
        }

        ErrorTip _errorTipControl;
        ErrorTip errorTipControl
        {
            get
            {
                if (_errorTipControl == null)
                {
                    _errorTipControl = new ErrorTip();
                    _errorTipControl.ParentElement = this;
                    container.Children.Add(_errorTipControl);

                }
                _errorTipControl.SetValue(Canvas.ZIndexProperty, 1);

                var top = -this.PictureHeight / 2;
                var left = this.PictureWidth;

                _errorTipControl.SetValue(Canvas.TopProperty, top);
                _errorTipControl.SetValue(Canvas.LeftProperty, left);
                return _errorTipControl;
            }
        }

        FlowNode _stationTipControl;
        /// <summary>
        /// 状态提示节点
        /// </summary>
        FlowNode stationTipControl
        {
            get
            {
                if (_stationTipControl == null)
                {
                    _stationTipControl = new FlowNode(_container, FlowNodeType.Ordinary);
                    _stationTipControl.Container = this.Container;
                  // _stationTipControl.sdPicture.txtNodeName.Text = "";
                    // container.Children.Add(_stationTipControl);
                    _container.AddFlowNode(_stationTipControl);

                }
                _stationTipControl.SetValue(Canvas.ZIndexProperty, 1);
                _stationTipControl.SetValue(Canvas.TopProperty, -(this.PictureHeight / 2) - 40);
                _stationTipControl.SetValue(Canvas.LeftProperty, this.PictureWidth - 60);
                _stationTipControl.CenterPoint = new Point(this.CenterPoint.X + this.PictureWidth + 40, this.CenterPoint.Y);
                return _stationTipControl;
            }
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
                sdPicture.CurrentContainer = value;
            }
        }
        public NodePosType HisPosType = NodePosType.Mid;
        FlowNodeType type = FlowNodeType.Ordinary;
        public FlowNodeType HisRunModel
        {
            get
            {
                return type;
            }
            set
            {
                bool isChanged = false;
                if (type != value)
                    isChanged = true;

                type = value;
                eiCenterEllipse.Visibility = Visibility.Visible;
                sdPicture.Type = type;
                sdPicture.ResetInitColor();
                if (isChanged)
                {
                    Move(this, null);
                }
            }
        }
        string fk_Flow;
        public string FK_Flow
        {
            get
            {
                if (string.IsNullOrEmpty(fk_Flow))
                {
                    fk_Flow = Guid.NewGuid().ToString();
                }
                return fk_Flow;
            }
            set
            {
                fk_Flow = value;
            }
        }

        string nodeID;
        public string NodeID
        {
            get
            {

                return nodeID;
            }
            set
            {
                nodeID = value;
            }
        }
        public string NodeName
        {
            get
            {
                return sdPicture.NodeName;
            }
            set
            {
                sdPicture.NodeName = value;
                sdPicture.PropertyChanged += sdPicture_PropertyChanged;
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

        string _subFlow;
        public string SubFlow
        {
            get
            {
                return _subFlow;
            }
            set
            {
                _subFlow = value;
            }
        }
        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }
        }
        FlowNodeComponent flowNodeData;
        public FlowNodeComponent FlowNodeData
        {
            get
            {
                if (flowNodeData == null)
                {
                    if (EditType == PageEditType.Add)
                    {
                        flowNodeData = new FlowNodeComponent();
                        flowNodeData.NodeID = this.NodeID;
                        flowNodeData.FK_Flow = this.FK_Flow;
                        flowNodeData.NodeName = sdPicture.NodeName;
                        flowNodeData.FlowNodeType = HisRunModel.ToString();
                        flowNodeData.RepeatDirection = RepeatDirection.ToString();
                        flowNodeData.SubFlow = SubFlow;


                    }
                    else if (EditType == PageEditType.Modify)
                    {
                        FlowNodeData = getFlowNodeComponentFromServer(this.NodeID);

                    }
                }
                return FlowNodeData;
            }
            set
            {
                flowNodeData = value;
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

        public Point CenterPoint
        {
            get
            {


                return new Point((double)this.GetValue(Canvas.LeftProperty) + this.Width / 2, (double)this.GetValue(Canvas.TopProperty) + this.Height / 2);

            }
            set
            {


                this.SetValue(Canvas.LeftProperty, value.X - this.Width / 2);
                this.SetValue(Canvas.TopProperty, value.Y - this.Height / 2);
                Move(this, null);


            }
        }

        public Point Position
        {
            get
            {
                Point position;

                position = new Point();
                position.Y = (double)this.GetValue(Canvas.TopProperty);
                position.X = (double)this.GetValue(Canvas.LeftProperty);


                return position;
            }
            set
            {

                this.SetValue(Canvas.TopProperty, value.Y);
                this.SetValue(Canvas.LeftProperty, value.X);
                Move(this, null);
            }
        }
        public WorkFlowElementType ElementType
        {
            get
            {
                return WorkFlowElementType.FlowNode;
            }
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

        FlowNode originFlowNode;
        public FlowNode OriginFlowNode
        {
            get
            {
                return originFlowNode;
            }
            set
            {
                originFlowNode = null;
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
                _container.SelectFlowNode(this, isSelectd);

                if (isSelectd)
                {
                    //sdPicture.SetSelectedColor();
                    if (!_container.CurrentSelectedControlCollection.Contains(this))
                        _container.AddSelectedControl(this);
                }
                else
                {
                    //sdPicture.ResetInitColor();
                }
            }
        }

        /// <summary>
        /// 轨迹节点
        /// </summary>
        private bool isTrackingNode = false;
        public bool IsTrackingNode
        {
            get { return isTrackingNode; }
            set
            {
                isTrackingNode = value;
#warning 从这里把路过的节点的边框设置颜色。
                //this.sdPicture.SetBorderColor(SystemConst.ColorConst.TrackingColor.NodeBackColor);
                //this.sdPicture.Background = SystemConst.ColorConst.TrackingColor.NodeBackColor as SolidColorBrush;
                IsSelectd = false;
            }
        }

        public bool PointIsInside(Point p)
        {
            bool isInside = false;


            double thisWidth = sdPicture.PictureWidth;
            double thisHeight = sdPicture.PictureHeight;

            double thisX = CenterPoint.X - thisWidth / 2;
            double thisY = CenterPoint.Y - thisHeight / 2;

            if (thisX < p.X && p.X < thisX + thisWidth
                && thisY < p.Y && p.Y < thisY + thisHeight)
            {
                isInside = true;
            }


            return isInside;
        } 
        #endregion

        #region Delegete and Event

        public event MoveDelegate FlowNodeMove;
        public event DeleteDelegate DeleteFlowNode;
        public event FlowNodeChangeDelegate FlowNodeChanged;

        #endregion

        #region Constructs
        public FlowNode(IContainer container, FlowNodeType at)
            : this()
        {

            _container = container;
            editType = PageEditType.Add;

            this.HisRunModel = at;

            System.Windows.Browser.HtmlPage.Document.AttachEvent("oncontextmenu", OnContextMenu);
            this.Name = FK_Flow;

            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, SystemConst.DoubleClickTime);
            _doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);
            sbDisplay.Begin();
        }

        public FlowNode()
        {
            InitializeComponent();
        } 
        #endregion

        #region Methods
        public Point GetPointOfIntersection(Point beginPoint, Point endPoint, DirectionMoveType type)
        {
            double endPointRadius = 4;
            double beginPointRadius = 4;
            Point p = new Point();
            //if (Type == FlowNodeType.INTERACTION
            //     || Type == FlowNodeType.AND_MERGE
            //    || Type == FlowNodeType.OR_MERGE
            //    || Type == FlowNodeType.VOTE_MERGE
            //    )
            //{

            #region


            if (Math.Abs(endPoint.X - beginPoint.X) <= PictureWidth / 2
                && Math.Abs(endPoint.Y - beginPoint.Y) <= PictureHeight / 2)
            {
                p = endPoint;
            }
            else
            {
                //起始点坐标和终点坐标之间的夹角（相对于Y轴坐标系）
                double angle = Math.Abs(Math.Atan((endPoint.X - beginPoint.X) / (endPoint.Y - beginPoint.Y)) * 180.0 / Math.PI);
                //节点的长和宽之间的夹角（相对于Y轴坐标系）
                double angel2 = Math.Abs(Math.Atan(PictureWidth / PictureHeight) * 180.0 / Math.PI);
                //半径
                double radio = PictureHeight < PictureWidth ? PictureHeight / 2 : PictureWidth / 2;

                if (angle <= angel2)//起始点坐标在终点坐标的上方,或者下方
                {
                    if (endPoint.Y < beginPoint.Y)//在上方
                    {
                        if (endPoint.X < beginPoint.X)
                            p.X = endPoint.X + Math.Tan(Math.PI * angle / 180.0) * radio;
                        else
                            p.X = endPoint.X - Math.Tan(Math.PI * angle / 180.0) * radio;

                        p.Y = endPoint.Y + PictureHeight / 2;
                    }
                    else//在下方
                    {
                        if (endPoint.X < beginPoint.X)
                            p.X = endPoint.X + Math.Tan(Math.PI * angle / 180.0) * radio;
                        else
                            p.X = endPoint.X - Math.Tan(Math.PI * angle / 180.0) * radio;

                        p.Y = endPoint.Y - PictureHeight / 2;
                    }

                }

                else//左方或者右方
                {
                    if (endPoint.X < beginPoint.X)//在右方
                    {
                        p.X = endPoint.X + PictureWidth / 2;
                        if (endPoint.Y < beginPoint.Y)
                            p.Y = endPoint.Y + Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                        else
                            p.Y = endPoint.Y - Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                    }
                    else//在左方
                    {
                        p.X = endPoint.X - PictureWidth / 2;
                        if (endPoint.Y < beginPoint.Y)
                            p.Y = endPoint.Y + Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                        else
                            p.Y = endPoint.Y - Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;

                    }
                }
            }

            if (type == DirectionMoveType.End)
            {
                p.X -= endPointRadius;
                p.Y -= endPointRadius;
            }
            if (type == DirectionMoveType.Begin)
            {
                p.X -= beginPointRadius;
                p.Y -= beginPointRadius;
            }

            #endregion
            //  }

            //else if (Type == FlowNodeType.INITIAL
            //  || Type == FlowNodeType.COMPLETION
            //  || Type == FlowNodeType.AUTOMATION
            //   || Type == FlowNodeType.DUMMY
            //    || Type == FlowNodeType.SUBPROCESS || Type == FlowNodeType.STATIONODE)
            //{
            //    #region
            //    if (Math.Abs(endPoint.X - beginPoint.X) <= PictureWidth / 2
            //        && Math.Abs(endPoint.Y - beginPoint.Y) <= PictureHeight / 2)
            //    {
            //        p = endPoint;
            //    }
            //    else
            //    {
            //        double radial = (PictureWidth < PictureHeight ? PictureWidth : PictureHeight) / 2;
            //        double bc = Math.Sqrt((endPoint.X - beginPoint.X) * (endPoint.X - beginPoint.X) + (endPoint.Y - beginPoint.Y) * (endPoint.Y - beginPoint.Y));
            //        p.X = endPoint.X - (endPoint.X - beginPoint.X) * radial / bc;
            //        p.Y = endPoint.Y - (endPoint.Y - beginPoint.Y) * radial / bc;


            //    }
            //    if (type == DirectionMoveType.End)
            //    {
            //        p.X -= endPointRadius;
            //        p.Y -= endPointRadius;
            //    }
            //    if (type == DirectionMoveType.Begin)
            //    {
            //        p.X -= beginPointRadius;
            //        p.Y -= beginPointRadius;
            //    }
            //    #endregion
            //}

            //else if (Type == FlowNodeType.AND_BRANCH
            //     || Type == FlowNodeType.OR_BRANCH
            //   )
            //{
            //    if (Math.Abs(endPoint.X - beginPoint.X) <= PictureWidth / 2
            //        && Math.Abs(endPoint.Y - beginPoint.Y) <= PictureHeight / 2)
            //    {
            //        p = endPoint;

            //        if (type == DirectionMoveType.End)
            //        {
            //            p.X -= endPointRadius;
            //            p.Y -= endPointRadius;
            //        }
            //        if (type == DirectionMoveType.Begin)
            //        {
            //            p.X -= beginPointRadius;
            //            p.Y -= beginPointRadius;
            //        }
            //    }
            //    else
            //    {

            //        //double angle = Math.Abs(Math.Atan((endPoint.X - beginPoint.X) / (endPoint.Y - beginPoint.Y)) * 180.0 / Math.PI);
            //        //if (angle < 45)
            //        //{
            //        //    if (endPoint.Y < beginPoint.Y)
            //        //    {
            //        //        p = ThisPointCollection[2];
            //        //    }
            //        //    else
            //        //    {
            //        //        p = ThisPointCollection[0];

            //        //    }

            //        //}

            //        //else
            //        //{
            //        //    if (endPoint.X < beginPoint.X)
            //        //    {
            //        //        p = ThisPointCollection[1];

            //        //    }
            //        //    else
            //        //    {
            //        //        p = ThisPointCollection[3];

            //        //    }
            //        //}


            //        double x = 0, y = 0;
            //        double tan = Math.Abs((endPoint.Y - beginPoint.Y) / (beginPoint.X - endPoint.X));

            //        if (endPoint.X <= beginPoint.X && endPoint.Y >= beginPoint.Y)//右上
            //        {
            //            y = (endPoint.Y + (ThisPointCollection[0].Y + (double)GetValue(Canvas.TopProperty)) * tan) / (1 + tan);

            //            x = (endPoint.Y - y) / tan + endPoint.X;
            //        }
            //        else if (this.CenterPoint.X <= beginPoint.X && this.CenterPoint.Y <= beginPoint.Y)//右下
            //        {
            //            y = (endPoint.Y + (ThisPointCollection[2].Y + (double)GetValue(Canvas.TopProperty)) * tan) / (1 + tan);
            //            x = (y - endPoint.Y) / tan + endPoint.X;
            //        }
            //        else if (this.CenterPoint.X >= beginPoint.X && this.CenterPoint.Y >= beginPoint.Y)//左上
            //        {
            //            y = (endPoint.Y + (ThisPointCollection[0].Y + (double)GetValue(Canvas.TopProperty)) * tan) / (1 + tan);
            //            x = (y - endPoint.Y) / tan + endPoint.X;

            //        }
            //        else if (this.CenterPoint.X >= beginPoint.X && this.CenterPoint.Y <= beginPoint.Y)//左下
            //        {
            //            y = (endPoint.Y + (ThisPointCollection[2].Y + (double)GetValue(Canvas.TopProperty)) * tan) / (1 + tan);
            //            x = (endPoint.Y - y) / tan + endPoint.X;
            //        }
            //        p.Y = y;
            //        p.X = x;
            //        if (type == DirectionMoveType.End)
            //        {
            //            p.X += -endPointRadius;
            //            p.Y += -endPointRadius;
            //        }
            //        if (type == DirectionMoveType.Begin)
            //        {
            //            p.X += -beginPointRadius;
            //            p.Y += -beginPointRadius;
            //        }
            //    }
            //}

            return p;
        }

        public CheckResult CheckSave()
        {
            CheckResult cr = new CheckResult();
            cr.IsPass = true;


            //if (BeginDirectionCollections != null
            //    && BeginDirectionCollections.Count > 0)
            //{
            //    cr.IsPass = false;//不能有后继节点
            //    cr.Message += string.Format(Text.Message_NotHaveFollowUpFlowNode, NodeName);
            //}
            //if (EndDirectionCollections == null
            //    || EndDirectionCollections.Count == 0)
            //{
            //    cr.IsPass = false;//必须至少有一个前驱节点
            //    cr.Message += string.Format(Text.Message_MustHaveAtLeastOnePreFlowNode, NodeName);
            //}

            isPassCheck = cr.IsPass;
            if (!cr.IsPass)
            {
                errorTipControl.Visibility = Visibility.Visible;
                errorTipControl.ErrorMessage = cr.Message.TrimEnd("\r\n".ToCharArray());
            }
            else
            {
                if (_errorTipControl != null)
                {
                    _errorTipControl.Visibility = Visibility.Collapsed;
                    container.Children.Remove(_errorTipControl);
                    _errorTipControl = null;
                }
            }
            return cr;
        }

        public MergePictureRepeatDirection RepeatDirection
        {
            get
            {

                return sdPicture.RepeatDirection;
            }
            set
            {

                bool isChanged = false;
                if (sdPicture.RepeatDirection != value)
                {

                    isChanged = true;

                }
                sdPicture.RepeatDirection = value;

                if (isChanged)
                {
                    Move(this, null);
                }
            }
        }

        public void SetFlowNodeData(FlowNodeComponent FlowNodeData)
        {
            bool isChanged = false;

            if (FlowNodeData.NodeName != FlowNodeData.NodeName
                || FlowNodeData.FlowNodeType != FlowNodeData.FlowNodeType
                || FlowNodeData.RepeatDirection != FlowNodeData.RepeatDirection)
            {
                isChanged = true;

            }

            setUIValueByFlowNodeData(FlowNodeData);
            if (isChanged)
            {
                if (FlowNodeChanged != null)
                    FlowNodeChanged(this);
            }
            IsSelectd = IsSelectd;

        }

        void setUIValueByFlowNodeData(FlowNodeComponent FlowNodeData)
        {
            sdPicture.NodeName = FlowNodeData.NodeName;
            FlowNodeType type = (FlowNodeType)Enum.Parse(typeof(FlowNodeType), FlowNodeData.FlowNodeType, true);
            MergePictureRepeatDirection repeatDirection = (MergePictureRepeatDirection)Enum.Parse(typeof(MergePictureRepeatDirection), FlowNodeData.RepeatDirection, true);
            HisRunModel = type;
            RepeatDirection = repeatDirection;
            SubFlow = FlowNodeData.SubFlow;
        }

        public PointCollection ThisPointCollection
        {
            get
            {
                return sdPicture.ThisPointCollection;
            }
        }

        FlowNodeComponent getFlowNodeComponentFromServer(string NodeID)
        {
            FlowNodeComponent ac = new FlowNodeComponent();
            ac = new FlowNodeComponent();
            ac.NodeID = this.NodeID;
            ac.FK_Flow = this.FK_Flow;
            ac.NodeName = sdPicture.NodeName;
            ac.FlowNodeType = HisRunModel.ToString();
            ac.SubFlow = this.SubFlow;
            return ac;
        }

        public void UpperZIndex()
        {
            ZIndex = _container.NextMaxIndex;
        }

        public string ToXmlString()
        {
            var xml = new System.Text.StringBuilder();

            xml.Append(@"       <FlowNode ");
            xml.Append(@" FK_Flow=""" + FK_Flow + @"""");
            xml.Append(@" NodeID=""" + NodeID + @"""");
            xml.Append(@" NodeName=""" + NodeName + @"""");
            xml.Append(@" Type=""" + HisRunModel.ToString() + @"""");
            xml.Append(@" SubFlow=""" + (HisRunModel == FlowNodeType.Ordinary ? SubFlow : @"") + @"""");
            xml.Append(@" PositionX=""" + CenterPoint.X + @"""");
            xml.Append(@" PositionY=""" + CenterPoint.Y + @"""");
            xml.Append(@" RepeatDirection=""" + RepeatDirection.ToString() + @"""");
            xml.Append(@" ZIndex=""" + ZIndex + @""">");

            xml.Append(Environment.NewLine);
            xml.Append("        </FlowNode>");
            return xml.ToString();
        }

        public void LoadFromXmlString(string xmlString)
        {
        }

        #region Add or Remove Direction
        public void AddBeginDirection(Direction r)
        {
            if (!BeginDirectionCollections.Contains(r))
            {
                BeginDirectionCollections.Add(r);
                r.BeginFlowNode = this;
                Move(this, null);

                // 添加节点后，将结束节点变为交互节点
                //if (FlowNodeType.COMPLETION == this.Type)
                //{
                //    this.Type = FlowNodeType.INTERACTION;
                //}
            }

        }

        public void RemoveBeginDirection(Direction r)
        {
            if (BeginDirectionCollections.Contains(r))
            {
                BeginDirectionCollections.Remove(r);
                r.RemoveBeginFlowNode(this);

                //移除连线后，如果如果节点不是开始节点，并且没有开始节点，则变为结束节点
                if (0 == BeginDirectionCollections.Count && this.HisPosType != NodePosType.End)
                {
                  //  this.HisPosType = NodePosType.End;
                    //this.Type = FlowNodeType.COMPLETION;
                }
            }
        }

        public void AddEndDirection(Direction r)
        {
            if (!EndDirectionCollections.Contains(r))
            {
                EndDirectionCollections.Add(r);
                r.EndFlowNode = this;
                Move(this, null);

            }

        }

        public void RemoveEndDirection(Direction r)
        {
            if (EndDirectionCollections.Contains(r))
            {
                EndDirectionCollections.Remove(r);
                r.RemoveEndFlowNode(this);
            }
        }
        #endregion

        bool isDeleted = false;
        public void Delete()
        {
            if (!isDeleted)
            {
                isDeleted = true;
                canShowMenu = false;
                sbClose.Completed += new EventHandler(sbClose_Completed);
                sbClose.Begin();
                foreach (Direction d in _container.DirectionCollections)
                {
                    if (d.BeginFlowNode == this || d.EndFlowNode == this)
                    {
                        d.Delete();
                    }
                }
                _container._Service.DoAsync("DelNode", this.NodeID, true);
            }
        }

        public void Move(FlowNode a, MouseEventArgs e)
        {
            if (FlowNodeMove != null)
                FlowNodeMove(a, e);
        }

        public void Zoom(double zoomDeep)
        {
            if (origPictureWidth == 0)
            {
                origPictureWidth = sdPicture.PictureWidth;
                origPictureHeight = sdPicture.PictureHeight;
            }
            if (positionIsChange)
            {
                origPosition = this.Position;
                positionIsChange = false;
            }

            sdPicture.PictureHeight = origPictureHeight * zoomDeep;
            sdPicture.PictureWidth = origPictureWidth * zoomDeep;
            this.Position = new Point(origPosition.X * zoomDeep, origPosition.Y * zoomDeep);
        }

        public void Edit()
        {
            _container.ShowFlowNodeSetting(this);
        }

        public FlowNode Clone()
        {
            FlowNode clone = new FlowNode(this._container, this.HisRunModel);
            clone.originFlowNode = this;
            clone.FlowNodeData = new FlowNodeComponent();
            clone.FlowNodeData.NodeName = this.FlowNodeData.NodeName;
            clone.FlowNodeData.FlowNodeType = this.FlowNodeData.FlowNodeType;
            clone.setUIValueByFlowNodeData(clone.FlowNodeData);
            // clone.CenterPoint = this.CenterPoint;
            clone.CenterPoint = this.CenterPoint;
            clone.ZIndex = this.ZIndex;
            //_container.AddFlowNode(clone);
            return clone;
        }

        void FlowNodeChange()
        {
            if (FlowNodeChanged != null)
            {
                FlowNodeChanged(this);
            }
        }

        public void ShowMessage(string message)
        {
            _container.ShowMessage(message);
        }

        private void OnContextMenu(object sender, System.Windows.Browser.HtmlEventArgs e)
        {
            //if (_container.MouseIsInContainer)
            //{
            //    e.PreventDefault();
            //    if (canShowMenu && !IsDeleted)
            //    {
            //        _container.ShowFlowNodeContentMenu(this, sender, e);
            //    }
            //}
        }

        void sdPicture_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        void sbClose_Completed(object sender, EventArgs e)
        {
            if (isDeleted)
            {
                if (this.BeginDirectionCollections != null)
                {
                    foreach (Direction r in this.BeginDirectionCollections)
                    {
                        r.RemoveBeginFlowNode(this);
                    }
                }
                if (this.EndDirectionCollections != null)
                {
                    foreach (Direction r in this.EndDirectionCollections)
                    {
                        r.RemoveEndFlowNode(this);
                    }
                }
                if (DeleteFlowNode != null)
                    DeleteFlowNode(this);

                _container.RemoveFlowNode(this);

                //if (FlowNodeChanged != null)
                //    FlowNodeChanged(this);
            }
        }

        #region Mouse Move and Click Related

        void DoubleClick_Timer(object sender, EventArgs e)
        {
            _doubleClickTimer.Stop();
        }

        bool trackingMouseMove = false;

        Point mousePosition;
        bool hadActualMove = false;
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {

                FrameworkElement element = sender as FrameworkElement;
                FlowNodePictureContainer fn = sender as FlowNodePictureContainer;
                try
                {
                    OrdinaryNode sn = fn.currentPic as OrdinaryNode;

                    if (sn != null && sn.picRect.Cursor == Cursors.SizeNWSE)
                    {
                        sn.picRect.Width += 1;
                        sn.picRect.Height += 1;
                    }
                    else
                    {
                        element.Cursor = Cursors.Hand;

                        if (e.GetPosition(null) == mousePosition)
                            return;
                        hadActualMove = true;
                        double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                        double deltaH = e.GetPosition(null).X - mousePosition.X;
                        double newTop = deltaV + Position.Y;
                        double newLeft = deltaH + Position.X;

                        double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                        double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);
                        if ((CenterPoint.X - sdPicture.PictureWidth / 2 < 2 && deltaH < 0)
                            || (CenterPoint.Y - sdPicture.PictureHeight / 2 < 2 && deltaV < 0)
                            )
                        {
                            //超过流程容器的范围
                        }
                        else
                        {
                            positionIsChange = true;
                            this.SetValue(Canvas.TopProperty, newTop);
                            this.SetValue(Canvas.LeftProperty, newLeft);

                            Move(this, e);
                            mousePosition = e.GetPosition(null);
                            _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
                            _container.IsNeedSave = true;
                        }

                    }
                }
                catch
                {
                }
            }
        }
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            hadActualMove = false;
            if (_doubleClickTimer != null)
                if (_doubleClickTimer.IsEnabled)
                {
                    _doubleClickTimer.Stop();
                    _container.ShowFlowNodeSetting(this);
                }
                else
                {
                    _doubleClickTimer.Start();
                    this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);

                    FrameworkElement element = sender as FrameworkElement;
                    FlowNodePictureContainer fn = sender as FlowNodePictureContainer;
                    OrdinaryNode sn = fn.currentPic as OrdinaryNode;
                    mousePosition = e.GetPosition(null);
                    trackingMouseMove = true;
                    if (null != element)
                    {
                        element.CaptureMouse();

                        if (sn != null && sn.picRect.Cursor == Cursors.SizeNWSE)
                        {

                        }
                        else
                        {
                            element.Cursor = Cursors.Hand;
                        }
                    }
                }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection != null)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

            canShowMenu = true;
            if (!hadActualMove && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SetWorkFlowElementSelected(this, IsSelectd);
            }
            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();

            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;

            if (hadActualMove)
            {
                FlowNodeChange();
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            canShowMenu = true;

            //ToolTip tt = new ToolTip();
            //ttFlowNodeTip.Content = FlowNodeData.NodeName + "\r\n" + FlowNodeData.FlowNodeType;
            return;

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            canShowMenu = false;

        }

        public void SetPositionByDisplacement(double x, double y)
        {
            Point p = new Point();
            p.X = (double)this.GetValue(Canvas.LeftProperty);
            p.Y = (double)this.GetValue(Canvas.TopProperty);

            this.SetValue(Canvas.TopProperty, p.Y + y);
            this.SetValue(Canvas.LeftProperty, p.X + x);
            Move(this, null);

        }

        private void CenterEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID")
                || System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
            {
                return;
            }
            if (isDeleted)
            {
                return;
            }
            e.Handled = true;
            if (_doubleClickTimer.IsEnabled)
            {
                _doubleClickTimer.Stop();
                _container.ShowFlowNodeSetting(this);

            }
            else
            {
                _doubleClickTimer.Start();

                if (!_container.CurrentSelectedControlCollection.Contains(this))
                {
                    if (_container.CurrentTemporaryDirection == null)
                    {
                        //if (this.Type != FlowNodeType.COMPLETION)
                        //{
                        _container.CurrentTemporaryDirection = new Direction(_container, true);
                        _container.AddDirection(_container.CurrentTemporaryDirection);
                        _container.CurrentTemporaryDirection.BeginFlowNode = this;
                        _container.CurrentTemporaryDirection.BeginPointPosition = this.CenterPoint;
                        _container.CurrentTemporaryDirection.EndPointPosition = _container.CurrentTemporaryDirection.BeginPointPosition;
                        _container.CurrentTemporaryDirection.ZIndex = _container.NextMaxIndex;
                        _container.IsNeedSave = true;
                        // _container.CurrentTemporaryDirection.DirectionName = Text.NewDirection;
                        //}
                    }
                }
                else
                {

                    FrameworkElement element = sender as FrameworkElement;
                    mousePosition = e.GetPosition(null);
                    trackingMouseMove = true;
                    if (null != element)
                    {
                        element.CaptureMouse();
                        element.Cursor = Cursors.Hand;
                    }

                }
            }
        }

        private void CenterEllipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID")
                || System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
            {
                return;

            }
            if (isDeleted)
            {
                return;
            }

            UserControl_MouseMove(sender, e);

        }

        private void CenterEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDeleted)
                return;
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection != null)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;
            UserControl_MouseLeftButtonUp(sender, e);
            if (_container.CurrentTemporaryDirection != null)
            {
                if (_container.CurrentTemporaryDirection.BeginFlowNode != null
                    && _container.CurrentTemporaryDirection.BeginFlowNode == this)
                {
                    this.RemoveBeginDirection(_container.CurrentTemporaryDirection);
                    _container.RemoveDirection(_container.CurrentTemporaryDirection);
                    _container.CurrentTemporaryDirection = null;

                }
            }

        }

        private void eiCenterEllipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_container.MouseIsInContainer)
            {
                e.GetPosition(null);

                if (canShowMenu && !IsDeleted)
                {
                    _container.ShowFlowNodeContentMenu(this, sender, e);
                }
            }
        }
        #endregion

        #region IElement 成员

        public UserStation Station()
        {
            UserStation us = new UserStation();
            us.IsPass = true;
            return us;
        }
        public void Worklist_del(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return;

            bool ishave = false;
            string empName = "";
            string sdt = "";
            int rowIndex = 0;
            foreach (DataRow dr in dataSet.Tables[1].Rows)
            {
                if (this.NodeID == dr["NodeID"].ToString())
                {
                    ishave = true;
                    empName += dr["EmpName"].ToString() + ";";

                    // 第一个点应该是 xxx在xxx时间发起，而非xxx在什么时间接受.
                    if (rowIndex == 0)
                        sdt = DateTime.Parse(dr["RDT"].ToString()).ToString("MM月dd号HH时mm分") + "发起";
                    else
                        sdt = DateTime.Parse(dr["RDT"].ToString()).ToString("MM月dd号HH时mm分") + "接收";
                }
                rowIndex++;
            }
            if (ishave == false)
                return;

            var dir = new Direction(_container)
            {
                BeginFlowNode = this,
                EndFlowNode = stationTipControl,
                IsTemporaryDirection = true,
               // FK_Flow = this.FK_Flow,
                Container = _container
            };

            _container.AddDirection(dir);
            stationTipControl.Visibility = Visibility.Visible;
            stationTipControl.StationMessage = empName.TrimEnd(';') + "\n" + sdt;
            _stationTipControl = null;

          //  stationTipControl.Width = stationTipControl.Width + 40;
          //  stationTipControl.Height = stationTipControl.Height -20;
            //   _container.ContainerHeight = _container.ContainerHeight / 3;
        }
        public string StationMessage
        {
            set
            {

           //     sdPicture.picSubThreadNode.tbMessage.Text = value;
            }
        }

        #endregion 
        #endregion
    }
}
