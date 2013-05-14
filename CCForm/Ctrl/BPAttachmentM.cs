using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.Text;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
using System.Windows.Media.Imaging;
using Silverlight;

namespace CCForm
{
    public class BPAttachmentM : System.Windows.Controls.TextBox
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

        public string Label = null;
        public string SaveTo = null;
        public bool IsDelete = false;
        public bool IsDownload = false;
        public bool IsUpload = false;

        public BPAttachmentM()
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 500;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);
        }
         
        public void UpdatePos()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
            }
        }

        #region keyborard 事件
        public double X = 0;
        public double Y = 0;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Height > 5)
                            this.Height = this.Height - 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, y - 1);
                        this.Y = y - 1;
                    }
                    this.UpdatePos();
                    break;
                case Key.S:
                case Key.Down:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Height > 5)
                            this.Height = this.Height + 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, y + 1);
                        this.Y = y + 1;
                    }
                    this.UpdatePos();
                    break;
                case Key.A:
                case Key.Left:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Width > 5)
                            this.Width = this.Width - 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x - 1);
                        this.X = x - 1;

                    }
                    this.UpdatePos();
                    break;
                case Key.D:
                case Key.Right:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        this.Width = this.Width + 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x + 1);
                        this.X = x + 1;
                    }
                    this.UpdatePos();
                    break;
                case Key.Delete:
                    this.DeleteIt();
                    Canvas c = this.Parent as Canvas;
                    c.Children.Remove(this);
                    break;
                case Key.C:
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除[" + this.Name + "]吗？如果确定以前产生的历史数据也会被删除。",
                "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLsAsync("DELETE FROM Sys_FrmAttachment WHERE NoOfObj='" + this.Name + "' AND FK_MapData='" + Glo.FK_MapData + "'");
            da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(da_RunSQLsCompleted);
        }
        void da_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            if (e.Result != 1)
                return;
            Canvas c = this.Parent as Canvas;
            if (c != null)
                c.Children.Remove(this);
        }
        #endregion 移动事件
        bool trackingMouseMove = false;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            trackingMouseMove = true;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            trackingMouseMove = false;
        }
        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
