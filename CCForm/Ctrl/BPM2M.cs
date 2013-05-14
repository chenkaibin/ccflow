using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.Text;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
using System.Windows.Media.Imaging;
using Silverlight;

namespace CCForm
{
    public class BPM2M : System.Windows.Controls.Label
    {
        #region 处理选中.
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
                    Thickness d = new Thickness(0.5);
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

        public int M2MType = 0;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }
        public BPM2M()
        {

            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();

            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 500;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);

            DataGrid dg = new DataGrid();
            dg.Name = "DG" + this.Name;
            DataGridTextColumn mycol = new DataGridTextColumn();
            mycol.Header = "提示:aswd键改变位置，鼠标移动到边框，按住左键调整大小，双击或右键修改属性。";
            dg.Columns.Add(mycol);
            dg.Width = this.Width;
            dg.Height = this.Height;
            this.Content = dg;
            this.MyDG = dg;
            this.SizeChanged += new SizeChangedEventHandler(BPM2M_SizeChanged);
        }

        void BPM2M_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MyDG != null)
            {
                if (!Convert.IsDBNull(this.Width))
                    MyDG.Width = this.Width;
                if (!Convert.IsDBNull(this.Height))
                    MyDG.Height = this.Height;
            }
        }
        /// <summary>
        /// Dtl
        /// </summary>
        public BPM2M(string name)
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();

            this.Name = name;
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 400;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);
            this.LoadDtl();
            this.SizeChanged += new SizeChangedEventHandler(BPM2M_SizeChanged);
        }
        public void LoadDtl()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync("SELECT * FROM Sys_MapM2M WHERE NoOfObj='" + this.Name + "' AND FK_MapData='"+Glo.FK_MapData+"'");
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            if (ds.Tables[0].Rows.Count == 0)
            {
               //this.NewM2M();
                return;
            }

            //this.Content = "提示:aswd键改变位置，shift+方向键调整大小，双击或右键修改属性。";

            DataGrid dg = new DataGrid();
            dg.Name = "DG" + this.Name;
            DataGridTextColumn mycol = new DataGridTextColumn();
            mycol.Header = "提示:aswd键改变位置，shift+方向键调整大小，双击或右键修改属性。";
            dg.Columns.Add(mycol);

            dg.Width = this.Width;
            dg.Height = this.Height;
            this.Content = dg;
            this.MyDG = dg;
        }
        public void NewM2M(double x, double y)
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("NewM2M", Glo.FK_MapData, this.Name, x.ToString(), y.ToString(), null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_New_DoTypeCompleted);
        }

        void da_New_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "新建错误", MessageBoxButton.OK);
                return;
            }

            string url = Glo.BPMHost + "/WF/MapDef/MapM2M.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&FK_MapM2M=" + this.Name + Glo.TimeKey;
            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:450px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");

        }

        public DataGrid MyDG = null;
        public void UpdatePos()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                MyDG.Width = this.Width;
                MyDG.Height = this.Height;
            }
        }

        #region keyborard 事件
        public double X = 0;
        public double Y = 0;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Height > 5)
                            this.Height = this.Height - 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, y - 1);
                        this.Y = y - 1;
                    }
                    this.UpdatePos();
                    break;
                case Key.S:
                case Key.Down:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Height > 5)
                            this.Height = this.Height + 1;

                    }
                    else
                    {
                        this.SetValue(Canvas.TopProperty, y + 1);
                        this.Y = y + 1;

                    }
                    this.UpdatePos();
                    break;
                case Key.A:
                case Key.Left:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Width > 5)
                            this.Width = this.Width - 1;

                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x - 1);
                        this.X = x - 1;

                    }
                    this.UpdatePos();
                    break;
                case Key.D:
                case Key.Right:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        this.Width = this.Width + 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x + 1);
                        this.X = x + 1;
                    }
                    this.UpdatePos();
                    break;
                case Key.Delete:
                    DeleteIt();
                    Canvas c = this.Parent as Canvas;
                    c.Children.Remove(this);
                    break;
                case Key.C:
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除[" + this.Name + "]吗？如果确定它相关的字段与设置也将会被删除，以前产生的历史数据也会被删除。", "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("DelM2M", this.Name, null, null, null, null, null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "删除错误", MessageBoxButton.OK);
                return;
            }

            Canvas c = this.Parent as Canvas;
            if (c != null)
                c.Children.Remove(this);
        }
        #endregion 移动事件

        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
            //FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            //hidDA.RunSQLsAsync("UPDATE Sys_MapDtl SET IsView=0 WHERE No='" + Glo.FK_MapData + "'");
            //hidDA.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(hidDA_RunSQLsCompleted);
        }
        void hidDA_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        

    }
}
