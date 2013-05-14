using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Silverlight;

namespace BP.SL
{
    public class PubClass
    {
        public static void DDL_BindDataTable( DataTable dt)
        {
        }

        public static Color ToColor(string colorName)
        {
            try
            {
                if (colorName.StartsWith("#"))
                    colorName = colorName.Replace("#", string.Empty);
                int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
                return new Color()
                {
                    A = Convert.ToByte((v >> 24) & 255),
                    R = Convert.ToByte((v >> 16) & 255),
                    G = Convert.ToByte((v >> 8) & 255),
                    B = Convert.ToByte((v >> 0) & 255)
                };
            }
            catch
            {
                return Colors.Black;
            }
        }

    }
}
