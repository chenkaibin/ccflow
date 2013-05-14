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
namespace WorkNode
{
    public class BPLine : System.Windows.Controls.Label
    {
        #region 处理选中.
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                if (value == true)
                {
                    //Thickness d = new Thickness(0.5);
                    //this.BorderThickness = d;
                    //this.BorderBrush = new SolidColorBrush(Colors.Blue);
                    //if (this.MyLine.X1 == this.MyLine.X2)
                    //{
                    //    this.Width = this.MyLine.Width + 5;
                    //    this.Height = Math.Abs(this.MyLine.Y1 - this.MyLine.Y2);
                    //}
                    //else
                    //{
                    //    this.Width = Math.Abs(this.MyLine.X1 - this.MyLine.X2); ;
                    //    this.Height = this.MyLine.Width + 5;
                    //}
                }
                else
                {
                    //Thickness d1 = new Thickness(0);
                    //this.BorderThickness = d1;
                    // this.BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }
        public void SetUnSelectedState()
        {
            if (this.IsSelected)
                this.IsSelected = false;
            else
                this.IsSelected = true;
        }
        #endregion 处理选中.

        public string FK_MapData = null;
        /// <summary>
        /// BPLine
        /// </summary>
        public BPLine()
        {
            this.Name = "Line" + DateTime.Now.ToString("yyMMhhddhhss");
        }
        public string Color = null;
        public BPLine(string name, string color, double borderW,
            double x1, double y1, double x2, double y2)
        {
            this.Name = name;
            this.MyLine = new Line();
           // this.MyLine.
            this.MyLine.Name = "lo" + name;
            this.MyLine.X1 = x1;
            this.MyLine.Y1 = y1;
            this.MyLine.X2 = x2;
            this.MyLine.Y2 = y2;
            this.MyLine.StrokeThickness = borderW;
            this.MyLine.Cursor = Cursors.Hand;
            this.Color = color;
            this.MyLine.Stroke = new SolidColorBrush(Glo.ToColor(color));
            this.Content = this.MyLine;
        }
        public Line MyLine = null;
        public Point MoveFrom = new Point(0,0);
        public Point MoveTo = new Point(0,0);
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.MoveTo = e.GetPosition(this.Parent as UIElement);
            if (this.MoveFrom != this.MoveTo)
            {
                //double x = this.MoveTo.X - this.MoveFrom.X;
                //double y = this.MoveTo.Y - this.MoveFrom.Y;

                ////double x = this.MoveTo.X - this.MoveFrom.X;
                ////double y = this.MoveTo.Y - this.MoveFrom.Y;

                //this.MyLine.X1 = this.MyLine.X1 + x;
                //this.MyLine.Y1 = this.MyLine.Y1 + y;
                //this.MyLine.X2 = this.MyLine.X2 + x;
                //this.MyLine.Y2 = this.MyLine.Y2 + y;
            }
            base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.MoveFrom = e.GetPosition(this.Parent as UIElement);
            base.OnMouseLeftButtonDown(e);
        }

        #region 焦点事件
        protected override void OnGotFocus(RoutedEventArgs e)
        {
         //   this.MyLine.BorderBrush.Opacity = 4;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            // this.BorderBrush.Opacity = 0.5;
            base.OnLostFocus(e);
        }
        #endregion 焦点事件

        #region 移动事件
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            return;
            e.Handled = true;
            // 获取 textBox 对象的相对于 Canvas 的 x坐标 和 y坐标
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            switch (e.Key)
            {
                case Key.Up:
                    this.SetValue(Canvas.TopProperty, y - 1);
                    this.MyLine.SetValue(Canvas.TopProperty, y - 1);
                    break;
                case Key.Down:
                    this.SetValue(Canvas.TopProperty, y + 1);
                    this.MyLine.SetValue(Canvas.TopProperty, y + 1);
                    break;
                case Key.Left:
                    this.SetValue(Canvas.LeftProperty, x - 1);
                    this.MyLine.SetValue(Canvas.LeftProperty, x - 1);
                    break;
                case Key.Right:
                    this.SetValue(Canvas.LeftProperty, x + 1);
                    this.MyLine.SetValue(Canvas.LeftProperty, x + 1);
                    break;
                case Key.Delete:
                    Canvas c = this.Parent as Canvas;
                    c.Children.Remove(this);
                    break;
                case Key.C:
                    break;
                case Key.A:
                  
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        BPLine tb = new BPLine();
                        tb.Cursor = Cursors.Hand;
                        tb.SetValue(Canvas.LeftProperty, (double)this.GetValue(Canvas.LeftProperty) + 15);
                        tb.SetValue(Canvas.TopProperty, (double)this.GetValue(Canvas.TopProperty) + 15);
                        Canvas s1c = this.Parent as Canvas;
                        try
                        {
                            s1c.Children.Add(tb);
                        }
                        catch
                        {
                            s1c.Children.Remove(tb);
                        }
                    }
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        #endregion 移动事件
    }
}
