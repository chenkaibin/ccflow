﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BP.En;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
namespace CCForm
{
    public class BPImg : System.Windows.Controls.TextBox
    {
        public string WinTarget = "_blank";
        public string WinURL = "";

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.Text = "按住鼠标左键拖动位置,鼠标拖动边框改变大小.";
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this.Text = "";
            base.OnMouseLeave(e);
        }

        #region 处理选中.
        public string Desc = null;
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
                    Thickness d = new Thickness(1);
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

        public double X = 0;
        public double Y = 0;
        public string KeyName = null;
        /// <summary>
        /// BPImg
        /// </summary>
        public BPImg()
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();

            this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
            this.IsReadOnly = true;

            this.Width = 200;
            this.Height = 120;

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCForm;component/Img/Logo/"+Glo.CompanyID+"/LogoBig.png", UriKind.Relative));
            ib.ImageSource = png;
            this.Background = ib;

            this.HisPng = png;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
        }
        public BitmapImage HisPng = null;

        #region 移动事件
        public bool IsCanReSize
        {
            get
            {
                return true;
            }
        }
        public bool IsCanDel
        {
            get
            {
                return true;
            }
        }
        public double MoveStep
        {
            get
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    return 1;
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    return 2;
                return 0;
            }
        }
        bool isCopy = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            this.X = (double)this.GetValue(Canvas.LeftProperty);
            this.Y = (double)this.GetValue(Canvas.TopProperty);
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    if (this.MoveStep != 0)
                    {
                        if (this.IsCanReSize == false)
                        {
                            MessageBox.Show("此控件不允许改变大小");
                            return;
                        }
                        if (this.Height > 18)
                            this.Height = this.Height - this.MoveStep;
                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, this.Y - 1);
                    }
                    break;
                case Key.Down:
                case Key.S:
                    if (this.MoveStep != 0)
                    {
                        if (this.IsCanReSize == false)
                        {
                            MessageBox.Show("此控件不允许改变大小");
                            return;
                        }
                        //   if (this.Height > 23)
                        this.Height = this.Height + this.MoveStep;
                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, this.Y + 1);
                    }
                    break;
                case Key.Left:
                case Key.A:
                    if (this.MoveStep != 0)
                    {
                        if (this.IsCanReSize == false)
                        {
                            MessageBox.Show("此控件不允许改变大小");
                            return;
                        }
                        if (this.Width > 8)
                            this.Width = this.Width - this.MoveStep;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, this.X - 1);
                    }
                    break;
                case Key.Right:
                case Key.D:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.IsCanReSize == false)
                        {
                            MessageBox.Show("此控件不允许改变大小");
                            return;
                        }
                        this.Width = this.Width + 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, this.X + 1);
                    }
                    break;
                case Key.Delete:
                    if (this.IsCanDel == false)
                    {
                        MessageBox.Show("该字段[" + this.Name + "]不可删除!", "提示", MessageBoxButton.OK);
                        return;
                    }
                    Canvas c = this.Parent as Canvas;
                    c.Children.Remove(this);
                    return;
                default:
                    break;
            }

            this.X = (double)this.GetValue(Canvas.LeftProperty);
            this.Y = (double)this.GetValue(Canvas.TopProperty);

            base.OnKeyDown(e);
        }
        bool trackingMouseMove = false;
        Point mousePosition;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //mousePosition = e.GetPosition(null);
            trackingMouseMove = true;
            //base.OnMouseLeftButtonDown(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //this.X = (double)this.GetValue(Canvas.LeftProperty);
            //this.Y = (double)this.GetValue(Canvas.TopProperty);
            trackingMouseMove = false;
            //base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //this.X = (double)this.GetValue(Canvas.LeftProperty);
            //this.Y = (double)this.GetValue(Canvas.TopProperty);
            trackingMouseMove = false;

            // base.OnMouseMove(e);
        }
        public void DoCopy()
        {
            return;
        }
        #endregion 移动事件
    }
}
