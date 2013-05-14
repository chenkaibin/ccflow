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

namespace BP.Picture
{
    public partial class VirtualNode : UserControl, IFlowNodePicture
    {
        public VirtualNode()
        {
            InitializeComponent();
        }
        public new double Opacity
        {
            set { picRect.Opacity = value; }
            get { return picRect.Opacity; }
        }
        public  double PictureWidth
        {
            set { picRect.Width = value; }
            get { return picRect.Width; }
        }
        public  double PictureHeight
        {
            set { picRect.Height = value; }
            get { return picRect.Height; }
        }
        public new Brush Background
        {
            set { picRect.Fill = value; }
            get { return picRect.Fill; }
        }
        public  Visibility PictureVisibility
        {
            set
            {

                this.Visibility = value;
            }
            get
            {

                return this.Visibility;
            }
        }

        public bool IsStartNode { get; set; }

        public void ResetInitColor()
        {
            SolidColorBrush brush = new SolidColorBrush();
            //brush.Color = Colors.White;
            picRect.Fill = IsStartNode ? SystemConst.ColorConst.BeginNodeColor : SystemConst.ColorConst.EndNodeColor;
            //SolidColorBrush brush1 = new SolidColorBrush();
            //brush1.Color = IsStartNode ? Colors.Green : Colors.Red;
            //picRectInside.Fill = brush1;
        }

        public void SetWarningColor()
        {
            picRect.Fill = SystemConst.ColorConst.WarningColor; 
        }
        public void SetSelectedColor()
        {
            picRect.Fill = SystemConst.ColorConst.SelectedColor;
        }
        public void SetBorderColor(Brush brush)
        {
            picRect.Stroke = brush;
        }
        public PointCollection ThisPointCollection
        {
            get { return null; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
