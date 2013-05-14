using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using Silverlight;
using BP.Sys;
using CCForm.FF;

namespace CCForm
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public class DBType
    {
        /// <summary>
        /// MSSQL
        /// </summary>
        public const string  MSSQL="MSSQL";
        /// <summary>
        /// Oracle
        /// </summary>
        public const string  Oracle="Oracle";
        /// <summary>
        /// MySQL
        /// </summary>
        public const string MySQL = "MySQL";
        /// <summary>
        /// Infomix
        /// </summary>
        public const string Infomix = "Infomix";
    }
    /// <summary>
    /// 全局类
    /// </summary>
    public class Glo
    {
        /// <summary>
        /// 全局变量x
        /// </summary>
        public static double X = 0;
        /// <summary>
        /// 全局变量Y
        /// </summary>
        public static double Y = 0;

        private static string _Len_Function = null;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string LEN_Function
        {
            get
            {
                if (_Len_Function == null)
                {
                    switch (Glo.AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            _Len_Function = "LEN";
                            break;
                        default:
                            _Len_Function = "LENGTH";
                            break;
                    }
                }
                return _Len_Function;
            }
        }
        public static string TimeKey
        {
            get
            {
                return "&T=" + DateTime.Now.ToString("yyMMddHHmmss");
            }
        }

        #region 操作步骤的恢复
        public static int CurrOpStep = 0;
        private static List<FuncStep> _FuncSteps = null;
        public static List<FuncStep> FuncSteps
        {
            get
            {
                if (_FuncSteps == null)
                    _FuncSteps = FuncStep.instance.GetEns();
                return _FuncSteps;
            }
        }
        #endregion 操作步骤的恢复

        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static FF.CCFormSoapClient GetCCFormSoapClientServiceInstance()
        {
            var basicBinding = new BasicHttpBinding() { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647, Name = "CCFormSoapClient" };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            var endPoint = new EndpointAddress(Glo.BPMHost + "/WF/MapDef/CCForm/CCForm.asmx");
            var ctor =
                typeof(CCFormSoapClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            return (CCFormSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
        public static bool trackingMouseMove = false;
        public static UIElement currEle = null;
        public static bool IsMouseDown = false;
        public static bool IsDtlFrm = false;
        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// 比如:http://demo.ccflow.org:8888
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;
                var location = (HtmlPage.Window.GetProperty("location")) as ScriptObject;
                _BPMHost = "http://" + location.GetProperty("host");
                return _BPMHost;
            }
        }

        public static string CompanyID = "CCFlow";
        public static string AppCenterDBType = "MSSQL";
        public static void BindComboBoxFrontName(ComboBox cb, string selectVal)
        {
            cb.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "宋体";
            cbi.Tag = "宋体";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "仿宋";
            cbi.Tag = "仿宋";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "粗体";
            cbi.Tag = "粗体";
            cb.Items.Add(cbi);

            foreach (ComboBoxItem item in cb.Items)
            {
                if (item.Tag.ToString() == selectVal)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            }

            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void SetComboBoxSelected(ComboBox cb, string val)
        {
            foreach (ComboBoxItem item in cb.Items)
            {
                item.IsSelected = false;
            }
            foreach (ComboBoxItem item in cb.Items)
            {
                if (item.Tag.ToString() == val)
                    item.IsSelected = true;
            }
            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxWinOpenTarget(ComboBox cb, string taget)
        {
            cb.Items.Clear();

            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "新窗口";
            cbi.Tag = "_blank";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;

            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "父窗口";
            cbi.Tag = "_parent";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "本窗口";
            cbi.Tag = "_self";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "自定义";
            cbi.Tag = "def";
            if ("@_blank,_self,_parent,".Contains(taget) == false)
                cbi.IsSelected = true;
            cb.Items.Add(cbi);
            SetComboBoxSelected(cb, taget);
        }
        public static void BindComboBoxFontSize(ComboBox cb, double selectDB)
        {
            cb.Items.Clear();
            for (int i = 6; i < 25; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = i.ToString();
                if (i.ToString() == selectDB.ToString())
                    cbi.IsSelected = true;
                cb.Items.Add(cbi);
            }
            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxLineBorder(ComboBox cb, double selectDB)
        {
            cb.Items.Clear();
            for (int i = 1; i < 15; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = i.ToString();
                if (i.ToString() == selectDB.ToString())
                    cbi.IsSelected = true;
                else
                    cbi.IsSelected = false;
                cb.Items.Add(cbi);
            }

            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        private static string[] _Colors = null;
        public static string[] ColorsStrs
        {
            get
            {
                if (_Colors == null)
                {
                    string cls = "@Black=#FF000000@Red=#FFFF0000@Blue=#FF0000FF@Green=#FF008000";
                    _Colors = cls.Split('@');
                }
                return _Colors;
            }
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

        public static Color PreaseColor_Del(string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    return Colors.Red;
                case "Yellow":
                    return Colors.Yellow;
                case "Blue":
                    return Colors.Blue;
                case "Brown":
                    return Colors.Brown;
                case "Cyan":
                    return Colors.Cyan;
                case "DarkGray":
                    return Colors.DarkGray;
                case "Gray":
                    return Colors.Gray;
                case "Orange":
                    return Colors.Orange;
                case "Green":
                    return Colors.Green;
                default:
                    return Colors.Black;
            }
        }
        public static string PreaseColorToName(string coloVal)
        {
            foreach (string c in Glo.ColorsStrs)
            {
                if (string.IsNullOrEmpty(c))
                    continue;

                string[] kvs = c.Split('=');
                if (kvs[1] == coloVal)
                    return kvs[0];
            }
            return coloVal;
        }
        /// <summary>
        /// 字体类型
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="selectDB"></param>
        public static void BindComboBoxFontStyle(ComboBox cb, string selectDB)
        {
            cb.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "italic";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "normal";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "oblique";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "inherit";
            cb.Items.Add(cbi);

            foreach (ComboBoxItem li in cb.Items)
            {
                if (li.Content.ToString() == selectDB)
                {
                    li.IsSelected = true;
                    break;
                }
            }
            if (cb.SelectedIndex < 0)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxColors_Del(ComboBox cb, string selectDB)
        {
            cb.Items.Clear();
            foreach (string str in Glo.ColorsStrs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string[] cls = str.Split('=');
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = cls[0];
                cbi.Tag = cls[1];

                if (cbi.Content.ToString() == selectDB || cbi.Tag.ToString() == selectDB)
                    cbi.IsSelected = true;
                cb.Items.Add(cbi);
            }
            if (cb.SelectedIndex < 0)
                cb.SelectedIndex = 0;
        }
        public static void WinOpen(string url)
        {
            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:600px;dialogWidth:800px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
        }
        public static void WinOpenModalDialog(string url, int h, int w)
        {
            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:" + h + "px;dialogWidth:" + w + "px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
        }
        public static void WinOpen(string url, int h, int w)
        {
            string p = "dialogHeight:" + h + "px;dialogWidth:" + w + "px";
            HtmlPage.Window.Eval(string.Format("window.open('{0}','{1}','{2};scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes;');", url,
                      "Title", p));
        }
        public static void IE_ShowAddFGuide()
        {
            Glo.WinOpen(Glo.BPMHost + "/WF/MapDef/Do.aspx?DoType=AddF&MyPK=" + Glo.FK_MapData);
        }
        public static string FK_Flow
        {
            get
            {
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow") == false)
                    return "004";
                return System.Windows.Browser.HtmlPage.Document.QueryString["FK_Flow"];
            }
        }
        private static int _FK_Node = 0;
        public static int FK_Node
        {
            get
            {
                if (_FK_Node != 0)
                    return _FK_Node;
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Node") == false)
                    _FK_Node = 401;
                else
                    _FK_Node = int.Parse(System.Windows.Browser.HtmlPage.Document.QueryString["FK_Node"]);
                return _FK_Node;
            }
            set
            {
                _FK_Node = value;
            }
        }
        private static string _FK_MapData = null;
        public static string FK_MapData
        {
            get
            {
                if (_FK_MapData != null)
                    return _FK_MapData;

                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_MapData") == false)
                    return "ND401";
                return System.Windows.Browser.HtmlPage.Document.QueryString["FK_MapData"];
            }
            set
            {
                _FK_MapData = value;
                // MapData HisMapData = new MapData(value);
            }
        }
        public static MapData HisMapData = null;
        public static object TempVal = null;
    }
}

