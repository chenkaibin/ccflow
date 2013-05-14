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
    public partial class SubThreadNode : UserControl, IFlowNodePicture
    {
        public SubThreadNode()
        {
            InitializeComponent();
        }
        public new double Opacity
        {
            set { picRect.Opacity = value; }
            get { return picRect.Opacity; }
        }
        double? _pictureWidth;
        //public  double  PictureWidth
        //{ 
        //    get {
        //        if (_pictureWidth == null)
        //        {
        //            _pictureWidth = Math.Abs(ThisPointCollection[1].X - ThisPointCollection[3].X);
        //        }
        //        return _pictureWidth.Value;
        //    }
        //    set
        //    {

        //        double width = value/2;
        //        double containerWidth = 100;
        //        double containerHeight = 80;
        //        PathGeometry pg = new PathGeometry();
        //        PathFigure pf = new PathFigure();
        //        pg.Figures.Add(pf);

        //        LineSegment ps = new LineSegment();
        //        ps.Point = new Point(containerWidth, containerHeight-width);
        //        pf.Segments.Add(ps);
        //        pf.StartPoint = ps.Point;


        //        ps = new LineSegment();
        //        ps.Point = new Point(containerWidth + width, containerHeight);
        //        pf.Segments.Add(ps);

        //        ps = new LineSegment();
        //        ps.Point = new Point(containerWidth , containerHeight+width);
        //        pf.Segments.Add(ps);

        //        ps = new LineSegment();
        //        ps.Point = new Point(containerWidth - width, containerHeight );
        //        pf.Segments.Add(ps);

        //        ps = new LineSegment();
        //        ps.Point = new Point(containerWidth, containerHeight - width);
        //        pf.Segments.Add(ps);
               
        //        picRect.SetValue(Path.DataProperty, pg);
        //    }
        //} 
        //public  double PictureHeight
        //{ 
        //    get {

        //        return PictureWidth;
        //    }
        //    set
        //    {
        //        PictureWidth = value;
        //    }
        //}
        public double PictureWidth
        {
            set { picRect.Width = value - 4; }
            get { return picRect.Width + 4; }
        }
        public double PictureHeight
        {
            set { picRect.Height = value - 4; }
            get { return picRect.Height + 4; }
        }

        PointCollection _thisPointCollection;
        public PointCollection ThisPointCollection
        {
            get
            {
                return null; 
                //if (true)//(_thisPointCollection == null)
                //{
                //    _thisPointCollection = new PointCollection();


                //    PathGeometry pg = (PathGeometry)picRect.GetValue(Path.DataProperty); 

                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[0]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[1]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[2]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[3]).Point);
                //}

                //return _thisPointCollection;
            }
        }
        public new Brush Background
        {
            set { picRect.Fill = value; 
            }
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
        public void ResetInitColor()
        {
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.White;
            picRect.Fill = brush;
            brush = new SolidColorBrush();
            brush.Color = Colors.Green;
            picRect.Stroke = brush;
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
    }
}
