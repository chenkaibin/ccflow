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

namespace BP.Picture
{
    public interface IFlowNodePicture
    {
        double Opacity
        {
            set;
            get;
        }
        double PictureWidth
        {
            get;
            set;
        }
        double PictureHeight
        {
            get;
            set;
        }
        Brush Background
        {
            set;
            get;
        }
        Visibility PictureVisibility
        {
            set;
            get;
        }
        void ResetInitColor();
        void SetWarningColor();
        void SetSelectedColor();
        void SetBorderColor(Brush brush);
        PointCollection ThisPointCollection { get; }
    }
}
