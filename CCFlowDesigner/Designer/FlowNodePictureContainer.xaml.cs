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
using BP.Picture;
using Ccflow.Web.Component.Workflow;
using System.ComponentModel;
using BP;

namespace BP
{
    public delegate void ColseAnimationCompletedDelegate(object sender, EventArgs e);

    public partial class FlowNodePictureContainer : UserControl, INotifyPropertyChanged
    {
        public IContainer CurrentContainer { get; set; }

        public double ContainerWidth
        {
            set
            {
                gridContainer.Width = value;
            }
            get
            {
                return gridContainer.Width;
            }
        }
        public double ContainerHeight
        {
            set
            {

                gridContainer.Height = value;
            }
            get
            {
                return gridContainer.Height;
            }
        }

        public double PictureWidth
        {
            get
            {
                return ((IFlowNodePicture)currentPic).PictureWidth;
            }
            set
            {
                ((IFlowNodePicture)currentPic).PictureWidth = value;
            }
        }
        public double PictureHeight
        {
            get
            {
                return ((IFlowNodePicture)currentPic).PictureHeight;
            }
            set
            {
                ((IFlowNodePicture)currentPic).PictureHeight = value;
            }
        }

        public UserControl currentPic;
        public FlowNodePictureContainer()
        {
            InitializeComponent();
        }
        public new SolidColorBrush Background
        {
            set
            {
                ((IFlowNodePicture)currentPic).Background = value;
            }
        }
        FlowNodeType type;
        public FlowNodeType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;

               this.picOrdinaryNode.PictureVisibility = Visibility.Collapsed;
               this.picFenLiuNode.PictureVisibility = Visibility.Collapsed;
               this.picHeLiuNode.PictureVisibility = Visibility.Collapsed;
               this.picFenHeLiuNode.PictureVisibility = Visibility.Collapsed;
               this.picSubThreadNode.PictureVisibility = Visibility.Collapsed;
               this.picVitualNode.PictureVisibility = Visibility.Collapsed;

               switch (this.type)
               {
                   case FlowNodeType.Ordinary:
                       currentPic = picOrdinaryNode;
                       break;
                   case FlowNodeType.FL:
                       currentPic = picFenLiuNode;
                       break;
                   case FlowNodeType.HL:
                       currentPic = picHeLiuNode;
                       break;
                   case FlowNodeType.FHL:
                       currentPic = picFenHeLiuNode;
                       break;
                   case FlowNodeType.SubThread:
                       currentPic = picSubThreadNode;
                       break;
                   case FlowNodeType.VirtualStart:
                       picVitualNode.IsStartNode = true;
                       currentPic = picVitualNode;
                       break;
                   case FlowNodeType.VirtualEnd:
                       picVitualNode.IsStartNode = false;
                       currentPic = picVitualNode;
                       break;
                   default:
                       throw new Exception("errrrss");
               }
                ((IFlowNodePicture)currentPic).PictureVisibility = Visibility.Visible;
            }
        }

        public string NodeName
        {
            get { return txtNodeName.Text; }
            set
            {
                RaisePropertyChanging(new PropertyChangedEventArgs("NodeName"));
                txtNodeName.Text = value;
                tbNodeName.Text = value;
            }
        }
        public void SetSelectedColor()
        {
            ((IFlowNodePicture)currentPic).SetSelectedColor();
        }
        public void SetWarningColor()
        {
            ((IFlowNodePicture)currentPic).SetWarningColor();
        }
        public void SetBorderColor(Brush brush)
        {
            ((IFlowNodePicture)currentPic).SetBorderColor(brush);
        }
        public void ResetInitColor()
        {
            ((IFlowNodePicture)currentPic).ResetInitColor();
        }
        public Point GetPointOfIntersection(Point beginPoint, Point endPoint, DirectionMoveType type)
        {
            return new Point(0, 0);
        }
        public PointCollection ThisPointCollection
        {
            get
            {
                return ((IFlowNodePicture)currentPic).ThisPointCollection;
            }
        }
        MergePictureRepeatDirection _repeatDirection;
        public MergePictureRepeatDirection RepeatDirection
        {
            get
            {
                //if (Type == FlowNodeType.OR_MERGE
                //    || Type == FlowNodeType.AND_MERGE
                //    || Type == FlowNodeType.VOTE_MERGE)
                //{
                //    _repeatDirection=((SubThreadNode)currentPic).RepeatDirection;
                //}
                return _repeatDirection;
            }
            set
            {
                _repeatDirection = value;
                //if (Type == FlowNodeType.OR_MERGE
                //    ||  Type == FlowNodeType.AND_MERGE
                //    ||  Type == FlowNodeType.VOTE_MERGE)
                //{

                //    ((SubThreadNode)currentPic).RepeatDirection = _repeatDirection;
                //}
            }
        }

        private void tbNodeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtNodeName.Text = tbNodeName.Text;
            CurrentContainer.IsNeedSave = true;
        }

        private void tbNodeName_LostFocus(object sender, RoutedEventArgs e)
        {
            tbNodeName.Visibility = Visibility.Collapsed;
            txtNodeName.Visibility = Visibility.Visible;

        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        public event EventHandler<PropertyChangedEventArgs> PropertyChanging;
        protected virtual void RaisePropertyChanging(string prop)
        {
            RaisePropertyChanging(new PropertyChangedEventArgs(prop));
        }
        protected virtual void RaisePropertyChanging(PropertyChangedEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }

        #endregion
    }
}
