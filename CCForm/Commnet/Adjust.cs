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
    /// <summary>
    /// 鼠标拖拽调整尺寸
    /// </summary>
    public class Adjust
    {
        FrameworkElement _Control;
        Point _PrePoint;
        double _ButtonWidth = 0.0;
        Direction _Edge;
        bool IsDrag = false;
        bool IsEdge = false;

        public void Bind(FrameworkElement _FrameworkElement)
        {
            _Control = _FrameworkElement;
            _Control.MouseMove += new MouseEventHandler(_Control_MouseMove);
            _Control.MouseLeftButtonDown += new MouseButtonEventHandler(_Control_MouseLeftButtonDown);
            _Control.MouseLeftButtonUp += new MouseButtonEventHandler(_Control_MouseLeftButtonUp);
        }

        private void _Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;
            _Control.ReleaseMouseCapture();
        }

        private void _Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsDrag && IsEdge)
            {
                Point pp = e.GetPosition(null);
                double height = pp.Y - _PrePoint.Y;
                double width = pp.X - _PrePoint.X;
                move(_Edge, height, width);
                _PrePoint = pp;
            }
            else
            {
                Point p1 = e.GetPosition(null);
                Point p2 = e.GetPosition(_Control);
                double x1 = _Control.ActualWidth;
                double y1 = _Control.ActualHeight;
                if (p2.X > -5 && p2.X < 5)
                {
                    if (p2.Y > -5 && p2.Y < 5)
                    {
                        _Control.Cursor = Cursors.SizeNWSE;
                        _Edge = Direction.WN;
                        IsEdge = true;
                    }
                    else if ((p2.Y - y1 > -5 && p2.Y - y1 < 5))
                    {
                        _Control.Cursor = Cursors.SizeNESW;
                        _Edge = Direction.WS;
                        IsEdge = true;
                    }
                    else
                    {
                        _Control.Cursor = Cursors.SizeWE;
                        _Edge = Direction.W;
                        IsEdge = true;
                    }
                }
                else if ((p2.X - x1 > -5 && p2.X - x1 < 5))
                {
                    if (p2.Y > -5 && p2.Y < 5)
                    {
                        _Control.Cursor = Cursors.SizeNESW;
                        _Edge = Direction.EN;
                        IsEdge = true;
                    }
                    else if ((p2.Y - y1 > -5 && p2.Y - y1 < 5))
                    {
                        _Control.Cursor = Cursors.SizeNWSE;
                        _Edge = Direction.ES;
                        IsEdge = true;
                    }
                    else
                    {
                        _Control.Cursor = Cursors.SizeWE;
                        _Edge = Direction.E;
                        IsEdge = true;
                    }
                }
                else if (p2.Y > -5 && p2.Y < 5)
                {
                    _Control.Cursor = Cursors.SizeNS;
                    _Edge = Direction.N;
                    IsEdge = true;
                }
                else if ((p2.Y - y1 > -5 && p2.Y - y1 < 5))
                {
                    _Control.Cursor = Cursors.SizeNS;
                    _Edge = Direction.S;
                    IsEdge = true;
                }
                else
                {
                    _Control.Cursor = Cursors.Arrow;
                    _Edge = Direction.None;
                    IsEdge = false;
                }
            }
        }

        private void _Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEdge == true)
            {
                IsDrag = true;
            }
            _Control.CaptureMouse();
            _ButtonWidth = _Control.ActualWidth;
            _PrePoint = e.GetPosition(null);
        }

        private void move(Direction edge, double height, double width)
        {
            switch (edge)
            {
                case Direction.E:
                    if (width + _Control.Width > 10)
                    {
                        _Control.Width += width;
                    }
                    break;
                case Direction.EN:
                    if (width + _Control.Width > 10)
                    {
                        _Control.Width += width;
                    }
                    if (_Control.Height - height > 10)
                    {
                        height = -height;
                        _Control.Height += height;
                        _Control.SetValue(Canvas.TopProperty, Convert.ToDouble(_Control.GetValue(Canvas.TopProperty)) - height);
                    }
                    break;
                case Direction.ES:
                    if (_Control.Height + height > 10)
                    {
                        _Control.Height += height;
                    }
                    if (_Control.Width + width > 10)
                    {
                        _Control.Width += width;
                    }
                    break;
                case Direction.N:
                    if (_Control.Height - height > 10)
                    {
                        height = -height;
                        _Control.Height += height;
                        _Control.SetValue(Canvas.TopProperty, Convert.ToDouble(_Control.GetValue(Canvas.TopProperty)) - height);
                    }
                    break;
                case Direction.S:
                    if (_Control.Height + height > 10)
                    {
                        _Control.Height += height;
                    }
                    break;
                case Direction.W:
                    if (_Control.Width - width > 10)
                    {
                        width = -width;
                        _Control.Width += width;
                        _Control.SetValue(Canvas.LeftProperty, Convert.ToDouble(_Control.GetValue(Canvas.LeftProperty)) - width);
                    }
                    break;
                case Direction.WN:
                    if (_Control.Width - width > 10)
                    {
                        width = -width;
                        _Control.Width += width;
                        _Control.SetValue(Canvas.LeftProperty, Convert.ToDouble(_Control.GetValue(Canvas.LeftProperty)) - width);
                    }
                    if (_Control.Height - height > 10)
                    {
                        height = -height;
                        _Control.Height += height;
                        _Control.SetValue(Canvas.TopProperty, Convert.ToDouble(_Control.GetValue(Canvas.TopProperty)) - height);
                    }
                    break;
                case Direction.WS:
                    if (_Control.Width - width > 10)
                    {
                        width = -width;
                        _Control.Width += width;
                        _Control.SetValue(Canvas.LeftProperty, Convert.ToDouble(_Control.GetValue(Canvas.LeftProperty)) - width);
                    }
                    if (height + _Control.Height > 10)
                    {
                        _Control.Height += height;
                    }
                    break;
                default:
                    break;
            }
        }

    }
    /// <summary>
    /// 标识用户鼠标当前处于控件的位置
    /// </summary>
    public enum Direction
    {
        N,
        S,
        W,
        E,
        WS,
        WN,
        EN,
        ES,
        None
    }
}
