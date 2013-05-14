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

namespace BP
{
    public class SystemConst
    {
        public class ColorConst
        {
            public static Brush WarningColor
            {
                get
                {
                    var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 0, 0) };
                    return brush;
                }
            }
            public static Brush SelectedColor
            {
                get
                {
                    var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 181, 0) };
                    return brush;
                }
            }

            public static Brush NodeFontColor
            {
                get
                {
                    var brush = new SolidColorBrush { Color = Colors.White };
                    return brush;
                }
            }

            public static Brush BeginNodeColor
            {
                get
                {
                    var brush = new SolidColorBrush { Color = ColorConverter.GetColorFromHex("#FFE010C0")};
                    return brush;
                }
            }

            public static Brush EndNodeColor
            {
                get
                {
                    var brush = new SolidColorBrush { Color = Colors.Green };
                    return brush;
                }
            }

            public class TrackingColor
            {
                public static Brush SelectedColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 181, 0) };
                        return brush;
                    }
                }

                public static Brush NodeBackColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = ColorConverter.GetColorFromHex("#FF0090FF") };
                        return brush;
                    }
                }

                public static Brush DirectionColor
                {
                    get
                    {
                        return NodeBackColor;
                        //var brush = new SolidColorBrush { Color = ColorConverter.GetColorFromHex("#FF0090FF") };
                        //return brush;
                    }
                }

                public static Brush LabelBackColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Color.FromArgb(255, 255, 181, 0) };
                        return brush;
                    }
                }
            }

            public class UnTrackingColor
            {
                public static Brush SelectedColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Colors.Gray };
                        return brush;
                    }
                }

                public static SolidColorBrush NodeBorderColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Colors.DarkGray };
                        return brush;
                    }
                }

                public static SolidColorBrush NodeBackColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Colors.DarkGray };
                        return brush;
                    }
                }

                public static Brush DirectionColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = Colors.DarkGray };
                        return brush;
                    }
                }

                public static Brush LabelBackColor
                {
                    get
                    {
                        var brush = new SolidColorBrush { Color = ColorConverter.GetColorFromHex("#FFAFBFDF") };
                        return brush;
                    }
                }
            }
        }
        public static double DirectionThickness
        {
            get
            {
                return 2.0;
            }
        }

        /// <summary>
        /// 双击的时间间隔
        /// </summary>
        public static int DoubleClickTime = 400;
    }

    public class ColorConverter
    {
        public static string ToHexColor(Color color)
        {
            string A = Convert.ToString(color.A, 16);
            if (A == "0")
                A = "00";
            string R = Convert.ToString(color.R, 16);
            if (R == "0")
                R = "00";
            string G = Convert.ToString(color.G, 16);
            if (G == "0")
                G = "00";
            string B = Convert.ToString(color.B, 16);
            if (B == "0")
                B = "00";
            string HexColor = "#" + A + R + G + B;

            return HexColor;
        }

        public static Color GetColorFromHex(string hexValue)
        {
            //value = #ab364f
            int a = Convert.ToInt32("0x" + hexValue.Substring(1, 2), 16);
            int r = Convert.ToInt32("0x" + hexValue.Substring(3, 2), 16);
            int g = Convert.ToInt32("0x" + hexValue.Substring(5, 2), 16);
            int b = Convert.ToInt32("0x" + hexValue.Substring(7, 2), 16);
            Color color = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);

            return color;
        }
    }
}
