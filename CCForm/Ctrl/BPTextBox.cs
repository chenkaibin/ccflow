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
namespace CCForm
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

        public double X = 0;
        public double Y = 0;
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
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();
            this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
        }
        public BPTextBox(TBType ty)
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();
            this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
            this.HisTBType = ty;
            this.InitType();
        }
        private string TBVal = null;
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            this.Text = this.Name;
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (this.TBVal == null)
                this.Text = "";
            else
                this.Text = this.TBVal;
            base.OnMouseLeave(e);
        }
        public BPTextBox(TBType ty, string tbName)
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();
            this.NameOfReal = tbName;
            this.Name = tbName;
            this.HisTBType = ty;
            this.InitType();
        }
        public void InitType()
        {
            Style style = new Style();

            this.IsReadOnly = true;
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

        #region 移动事件
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
            //Canvas c = this.Parent as Canvas;
            //Point point = e.GetPosition(c);

            //this.X = point.X;// (double)this.GetValue(Canvas.LeftProperty);
            //this.Y = point.Y;//(double)this.GetValue(Canvas.TopProperty);         
            trackingMouseMove = false;
            //base.OnMouseLeftButtonUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Canvas c = this.Parent as Canvas;
            //Point point = e.GetPosition(c);

            //this.X = (double)this.GetValue(Canvas.LeftProperty);
            //this.Y = (double)this.GetValue(Canvas.TopProperty);
            trackingMouseMove = false;

            //base.OnMouseMove(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.V)
                {
                    base.OnKeyUp(e);
                    return;
                }
                this.DoCopy();

            }
            base.OnKeyUp(e);
        }
        public bool IsCanReSize
        {
            get
            {
                if (this.HisTBType == TBType.DateTime)
                    return false;

                if (this.HisTBType == TBType.Date)
                    return false;

                return true;
            }
        }
        public bool IsCanDel
        {
            get
            {
                switch (this.Name)
                {
                    case "Title":
                        return false;
                    default:
                        return true;
                }
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
                    this.DeleteIt();
                    return;
                default:
                    break;
            }

            this.X = (double)this.GetValue(Canvas.LeftProperty);
            this.Y = (double)this.GetValue(Canvas.TopProperty);

            base.OnKeyDown(e);
        }
        public bool IsDelete = false;
        public void DeleteIt()
        {
            if (this.IsCanDel == false)
            {
                MessageBox.Show("该字段[" + this.Name + "]不可删除!", "提示", MessageBoxButton.OK);
                return;
            }

            if (this.Name != null)
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                string sqls = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND KeyOfEn='" + this.Name + "'";
                da.RunSQLsAsync(sqls);
            }

            IsDelete = true;
            Canvas c = this.Parent as Canvas;
            c.Children.Remove(this);
        }
        public void HidIt()
        {
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLsAsync("UPDATE Sys_MapAttr SET UIVisible=0 WHERE KeyOfEn='" + this.Name + "' AND FK_MapData='" + Glo.FK_MapData + "'");
            hidDA.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(hidDA_RunSQLsCompleted);
        }
        void hidDA_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void DoCopy()
        {
            return;

            BPTextBox tb = new BPTextBox(this.HisTBType);
            tb.Cursor = Cursors.Hand;
            tb.HisTBType = this.HisTBType;
            tb.InitType();
            tb.Text = this.Text;
            tb.Width = this.Width;
            tb.Height = this.Height;

            tb.SetValue(Canvas.LeftProperty, (double)this.GetValue(Canvas.LeftProperty) + 15);
            tb.SetValue(Canvas.TopProperty, (double)this.GetValue(Canvas.TopProperty) + 15);
            Canvas s1c = this.Parent as Canvas;
            try
            {
                s1c.Children.Add(tb);
            }
            catch
            {
                s1c.Children.Remove(tb);
            }
        }
        #endregion 移动事件
    }
}
