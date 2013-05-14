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
using WorkNode.FF;

namespace WorkNode
{
    public class DBType
    {
        public const string  MSSQL="MSSQL";
        public const string  Oracle="Oracle";
        public const string MySQL = "MySQL";
        public const string DB2 = "DB2";
    }
    public class Glo
    {
        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static FF.CCFlowAPISoapClient GetCCFlowAPISoapClientServiceInstance()
        {
            var basicBinding = new BasicHttpBinding() { MaxBufferSize = 2147483647, MaxReceivedMessageSize = 2147483647, Name = "CCFlowAPISoapClient" };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            var endPoint = new EndpointAddress(Glo.BPMHost + "/WF/WorkOpt/CCFlowAPI.asmx");
            var ctor =
                typeof(CCFlowAPISoapClient).GetConstructor(new Type[] { typeof(Binding), typeof(EndpointAddress) });
            return (CCFlowAPISoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
        public static string FK_MapData
        {
            get
            {
                return "ND" + Glo.FK_Node;
            }
        }
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
        public static string FK_Flow
        {
            get
            {
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow") == false)
                    throw new Exception("@丢失FK_Flow参数.");
                return System.Windows.Browser.HtmlPage.Document.QueryString["FK_Flow"];
            }
        }
        private static string _UserNo = null;
        public static string UserNo
        {
            get
            {
                if (_UserNo != null)
                    return _UserNo;

                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("UserNo") == false)
                    throw new Exception("@丢失UserNo参数.");
                _UserNo = System.Windows.Browser.HtmlPage.Document.QueryString["UserNo"];
                return _UserNo;
            }
            set
            {
                _UserNo = value;
            }
        }
        private static int _FK_Node = 0;
        public static int FK_Node
        {
            get
            {
                if (_FK_Node == 0)
                {
                    if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Node") == false)
                        throw new Exception("@丢失FK_Node参数.");
                    _FK_Node = int.Parse(System.Windows.Browser.HtmlPage.Document.QueryString["FK_Node"]);
                }
                return _FK_Node;
            }
            set
            {
                _FK_Node = value;
            }
        }
        private static Int64 _WorkID = 0;
        public static Int64 WorkID
        {
            get
            {
                if (_WorkID == 0)
                {
                    if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID") == false)
                        throw new Exception("@丢失WorkID参数.");

                    _WorkID = int.Parse(System.Windows.Browser.HtmlPage.Document.QueryString["WorkID"]);
                }
                return _WorkID;
            }
            set
            {
                _WorkID = value;
            }
        }
        private static Int64 _FID = 0;
        public static Int64 FID
        {
            get
            {
                if (_FID == 0)
                {
                    if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FID") == false)
                        throw new Exception("@丢失FID参数.");

                    _FID = int.Parse(System.Windows.Browser.HtmlPage.Document.QueryString["FID"]);
                }
                return _FID;
            }
            set
            {
                _FID = value;
            }
        }
        public static object TempVal = null;
        public static MapData HisMapData = null;
    }
}

