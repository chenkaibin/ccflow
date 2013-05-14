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
using BP.En;

namespace CCForm
{
    public class BPDatePicker : System.Windows.Controls.DatePicker
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
                    Thickness d1 = new Thickness(0.5);
                    this.BorderThickness = d1;
                    this.BorderBrush = new SolidColorBrush(Colors.Black);
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

        public string KeyName = null;
        public string HisDateType = DataType.AppDate;
        /// <summary>
        /// BPDatePicker
        /// </summary>
        public BPDatePicker()
        {
            this.Name = "DP" + DateTime.Now.ToString("yyMMddhhmmss");
            this.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.DataContext = DateTime.Now.ToString("yyyy-MM-dd");
            this.Width = this.Width - 10;
        }

        #region 焦点事件
        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    this.BorderBrush.Opacity = 4;
        //    base.OnGotFocus(e);
        //}
        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    this.BorderBrush.Opacity = 0.5;
        //    base.OnLostFocus(e);
        //}
        #endregion 焦点事件

        #region 移动事件
        //bool trackingMouseMove = false;
        //Point mousePosition;
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    mousePosition = e.GetPosition(null);
        //    trackingMouseMove = true;
        //    base.OnMouseLeftButtonDown(e);
        //}
        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    trackingMouseMove = false;
        //    base.OnMouseLeftButtonUp(e);
        //}
        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    //FrameworkElement element = sender as FrameworkElement;
        //    if (trackingMouseMove)
        //    {
        //        double moveH = e.GetPosition(null).Y - mousePosition.Y;
        //        double moveW = e.GetPosition(null).X - mousePosition.X;
        //        double newTop = moveH + (double)this.GetValue(Canvas.TopProperty);
        //        double newLeft = moveW + (double)this.GetValue(Canvas.LeftProperty);
        //        this.SetValue(Canvas.TopProperty, newTop);
        //        this.SetValue(Canvas.LeftProperty, newLeft);
        //        mousePosition = e.GetPosition(null);
        //    }
        //    base.OnMouseMove(e);
        //}
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
                    this.SetValue(Canvas.TopProperty, y - 1);
                    break;
                // 按 Down 键后 textBox 对象向 下 移动 1 个像素
                // Down 键所对应的 e.PlatformKeyCode == 40
                case Key.Down:
                   // MessageBox.Show("sss");
                    this.SetValue(Canvas.TopProperty, y + 1);
                    break;
                // 按 Left 键后 textBox 对象向 左 移动 1 个像素
                // Left 键所对应的 e.PlatformKeyCode == 37
                case Key.Left:
                    this.SetValue(Canvas.LeftProperty, x - 1);
                    break;

                // 按 Right 键后 textBox 对象向 右 移动 1 个像素
                // Right 键所对应的 e.PlatformKeyCode == 39 
                case Key.Right:
                    this.SetValue(Canvas.LeftProperty, x + 1);
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
                        BPDatePicker tb = new BPDatePicker();
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
