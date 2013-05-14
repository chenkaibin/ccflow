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
using BP.En;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
namespace WorkNode
{
    /// <summary>
    /// 类型
    /// </summary>
    public enum TBType
    {
        String,
        Int,
        Float,
        Money,
        DateTime,
        Date
    }
    public class BPTextBox : System.Windows.Controls.TextBox
    {
        #region 处理选中.
        public string NameOfReal = null;
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

        public double X=0;
        public double Y=0;
        /// <summary>
        /// 类型
        /// </summary>
        public TBType HisTBType = TBType.String;
        public string HisDataType
        {
            get
            {
                switch (this.HisTBType)
                {
                    case TBType.Float:
                        return DataType.AppFloat;
                    case TBType.Money:
                        return DataType.AppMoney;
                    case TBType.Int:
                        return DataType.AppInt;
                    case TBType.Date:
                        return DataType.AppDate;
                    case TBType.DateTime:
                        return DataType.AppDateTime;
                    case TBType.String:
                    default:
                        return DataType.AppString;
                }
            }
        }
        public string KeyName = null;
        /// <summary>
        /// BPTextBox
        /// </summary>
        public BPTextBox()
        {
            this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
        }
        public BPTextBox(TBType ty)
        {
            this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
            this.HisTBType = ty;
            this.InitType();
        }
        private string TBVal = null;
        public BPTextBox(TBType ty, string tbName)
        {
            this.NameOfReal = tbName;
            this.Name = tbName;
            // "TB" + DateTime.Now.ToString("yyMMddhhmmss");
            this.HisTBType = ty;
            this.InitType();
        }
        public void InitType()
        {
            Style style = new Style();
            this.Background = new SolidColorBrush(Colors.White);
             
            switch (this.HisTBType)
            {
                case TBType.Date:
                    this.Width = 100;
                    this.Height = 23;
                    break;
                case TBType.DateTime:
                    this.Width = 120;
                    this.Height = 23;
                    break;
                case TBType.String:
                    this.Width = 100;
                    this.Height = 23;
                    break;
                case TBType.Money:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0.00";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                case TBType.Int:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                case TBType.Float:
                    this.Width = 100;
                    this.Height = 23;
                    this.Text = "0";
                    this.TextAlignment = System.Windows.TextAlignment.Right;
                    break;
                default:
                    break;
            }
        }
        protected override void OnDrop(DragEventArgs e)
        {
            MessageBox.Show(e.ToString());
            base.OnDrop(e);
        }
    }
}
