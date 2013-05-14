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
    public static class Drag
    {
        // Global variables used to keep track of the 
        // mouse position and whether the object is captured
        // by the mouse.
        static bool isMouseCaptured;
        static double mouseVerticalPosition;
        static double mouseHorizontalPosition;
        static DateTime _lastTime;
        public static void BindDrag(this UIElement element)
        {
            element.MouseLeftButtonDown += Handle_MouseDown;
            element.MouseMove += Handle_MouseMove;
            element.MouseLeftButtonUp += Handle_MouseUp;

            //element.MouseLeftButtonUp += new MouseButtonEventHandler(button1_MouseLeftButtonUp);
            //this.button1.MouseLeftButtonDown += button1_MouseLeftButtonDown;
            //this.MouseLeftButtonUp += new MouseButtonEventHandler(GridStyle_MouseLeftButtonUp);
            //this.MouseMove += new MouseEventHandler(GridStyle_MouseMove);

        }
        public static void UnBindDrag(this UIElement element)
        {
            element.MouseLeftButtonDown -= Handle_MouseDown;
            element.MouseMove -= Handle_MouseMove;
            element.MouseLeftButtonUp -= Handle_MouseUp;
        }

        public static void Handle_MouseDown(object sender, MouseEventArgs args)
        {
            isMouseCaptured = false;
            FrameworkElement e = sender as FrameworkElement;
            Point p = args.GetPosition(e);
            if (p.X > 5 && p.X < e.ActualWidth - 5 && p.Y > 5 && p.Y < e.ActualHeight - 5)
            {
                UIElement _UIElement = sender as UIElement;
                mouseVerticalPosition = args.GetPosition(null).Y;
                mouseHorizontalPosition = args.GetPosition(null).X;
                isMouseCaptured = true;
                _UIElement.CaptureMouse();
            }
            if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300)
            {
                isMouseCaptured = false;
            }
            _lastTime = DateTime.Now;
        }

        public static void Handle_MouseMove(object sender, MouseEventArgs args)
        {
            if (isMouseCaptured)
            {
                UIElement _UIElement = sender as UIElement;
                // Calculate the current position of the object.
                double deltaV = args.GetPosition(null).Y - mouseVerticalPosition;
                double deltaH = args.GetPosition(null).X - mouseHorizontalPosition;
                double newTop = deltaV + (double)_UIElement.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)_UIElement.GetValue(Canvas.LeftProperty);

                // Set new position of object.
                _UIElement.SetValue(Canvas.TopProperty, newTop);
                _UIElement.SetValue(Canvas.LeftProperty, newLeft);

                // Update position global variables.
                mouseVerticalPosition = args.GetPosition(null).Y;
                mouseHorizontalPosition = args.GetPosition(null).X;
            }
        }

        public static void Handle_MouseUp(object sender, MouseEventArgs args)
        {
            UIElement _UIElement = sender as UIElement;
            isMouseCaptured = false;
            _UIElement.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
        }
    }
}
