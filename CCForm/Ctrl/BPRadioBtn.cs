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
    public class BPRadioBtn : System.Windows.Controls.RadioButton
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
        public string UIBindKey = null;
        protected override void OnClick()
        {
            base.OnClick();
        }
       
        public string FK_MapData = null;
        /// <summary>
        /// BPRadioButton
        /// </summary>
        public BPRadioBtn()
        {
            this.Name = "RB" + DateTime.Now.ToString("yyMMddhhmmss");
        }

        #region 焦点事件
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.BorderBrush.Opacity = 4;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.BorderBrush.Opacity = 0.5;
            base.OnLostFocus(e);
        }
        #endregion 焦点事件

        #region 移动事件
        bool trackingMouseMove = false;
        Point mousePosition;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            #region 把相同分组的数据
            Canvas c = this.Parent as Canvas;
            foreach (UIElement ctl in c.Children)
            {
                BPRadioBtn rb = null;
                try
                {
                    rb = ctl as BPRadioBtn;
                    if (rb == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                if (rb.GroupName == this.GroupName)
                    rb.Foreground = new SolidColorBrush(Colors.Blue);
                else
                    rb.Foreground = new SolidColorBrush(Colors.Black);
            }
            #endregion 把相同分组的按钮设置成一组颜色.

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
            //FrameworkElement element = sender as FrameworkElement;
            if (trackingMouseMove)
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
        public bool IsDelete = false;
        public void DoDeleteIt()
        {
            Canvas c = this.Parent as Canvas;
            string ids = "";
            foreach (UIElement uiCtl in c.Children)
            {
                Control ctl = uiCtl as Control;
                if (ctl == null)
                    continue;

                if (ctl.Name.Contains(this.GroupName))
                    ids += "@" + ctl.Name;
            }

            string[] strs = ids.Split('@');
            foreach (string str in strs)
            {
                foreach (UIElement uiCtl in c.Children)
                {
                    Control ctl = uiCtl as Control;
                    if (ctl == null)
                        continue;

                    if (ctl.Name == str)
                    {
                        c.Children.Remove(ctl);
                        BPRadioBtn eleBtn = ctl as BPRadioBtn;
                        eleBtn.IsDelete = true;
                        break;
                    }
                }
            }
            c.Children.Remove(this);
            Glo.currEle = null;
            this.IsDelete = true;
        }
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
                case Key.Delete:
                    this.DoDeleteIt();
                    break;
                case Key.Up:
                case Key.W:
                    this.SetValue(Canvas.TopProperty, y - 1);
                    break;

                // 按 Down 键后 textBox 对象向 下 移动 1 个像素
                // Down 键所对应的 e.PlatformKeyCode == 40
                case Key.Down:
                case Key.S:
                    this.SetValue(Canvas.TopProperty, y + 1);
                    break;
                case Key.Left:
                case Key.D:
                    this.SetValue(Canvas.LeftProperty, x + 1);
                    break;
                // 按 Right 键后 textBox 对象向 右 移动 1 个像素
                // Right 键所对应的 e.PlatformKeyCode == 39 
                case Key.Right:
                case Key.A:
                    this.SetValue(Canvas.LeftProperty, x - 1);
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        #endregion 移动事件
    }
}
