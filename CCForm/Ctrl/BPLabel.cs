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

namespace CCForm
{
    public class BPLabel : System.Windows.Controls.Label
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
                    Thickness d = new Thickness(0.5);
                    this.BorderThickness = d;
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Thickness d1 = new Thickness(0);
                    this.BorderThickness = d1;
                    this.BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }
        public void SetUnSelectedState()
        {
            if (this.IsSelected)
            {
                this.IsSelected = false;
            }
            else
            {
                this.IsSelected = true;
            }
        }
        #endregion 处理选中.

        /// <summary>
        /// BPLabel
        /// </summary>
        public BPLabel()
        {
            this.Name = "Lab" + DateTime.Now.ToString("yyMMddhhmmss");
            this.Content = "label...";
            this.Foreground = new SolidColorBrush(Colors.Black);
            this.FontStyle = FontStyles.Normal;
            this.AllowDrop = false;
        }

        #region 焦点事件
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            
            trackingMouseMove = false;
            base.OnLostFocus(e);
        }
        #endregion 焦点事件

        #region 移动事件
        public bool trackingMouseMove = false;
        Point mousePosition;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            mousePosition = e.GetPosition(null);
            trackingMouseMove = true;
            base.OnMouseLeftButtonDown(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            trackingMouseMove = false;
            base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {

            if (trackingMouseMove  )
            {
                double moveH = e.GetPosition(null).Y - mousePosition.Y;
                double moveW = e.GetPosition(null).X - mousePosition.X;
                double newTop = moveH + (double)this.GetValue(Canvas.TopProperty);
                double newLeft = moveW + (double)this.GetValue(Canvas.LeftProperty);
                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);
                mousePosition = e.GetPosition(null);
            }
            base.OnMouseMove(e);
        }

        #region 转向方法.
        public void ToLeft()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.LeftProperty, x - 1);
        }
        public void ToRight()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.LeftProperty, x + 1);
        }

        public void ToUp()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.TopProperty, y - 1);
        }
        public void ToDown()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.TopProperty, y + 1);
        }
        #endregion 转向方法.

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            // 获取 textBox 对象的相对于 Canvas 的 x坐标 和 y坐标
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);

            // KeyEventArgs.Key - 与事件相关的键盘的按键 [System.Windows.Input.Key枚举]
            switch (e.Key)
            {
                // 按 Up 键后 textBox 对象向 上 移动 1 个像素
                // Up 键所对应的 e.PlatformKeyCode == 38 
                // 当获得的 e.Key == Key.Unknown 时，可以使用 e.PlatformKeyCode 来确定用户所按的键
                case Key.Up:
                case Key.W:
                    this.ToUp();
                    break;
                case Key.Down:
                case Key.S:
                    this.ToDown();
                    break;
                case Key.Left:
                case Key.A:
                    this.ToLeft();
                    break;
                case Key.Right:
                case Key.D:
                    this.ToRight();
                    break;
                case Key.Delete:
                    if (this.Name.Contains("_blank_") == false)
                    {
                        if (MessageBox.Show("您确定要删除吗？",
                            "删除提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            return;
                    }
                    Canvas c = this.Parent as Canvas;
                    c.Children.Remove(this);
                    break;
                case Key.C:
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        BPLabel tb = new BPLabel();
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
