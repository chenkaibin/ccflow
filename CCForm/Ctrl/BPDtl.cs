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
    public class BPDtl : System.Windows.Controls.Label
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        public BPDtl()
        {
            Adjust adjust = new Adjust();
            adjust.Bind(this);
            this.BindDrag();
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 400;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);
            this.SizeChanged += new SizeChangedEventHandler(BPDtl_SizeChanged);
        }
        /// <summary>
        /// Dtl
        /// </summary>
        public BPDtl(string name)
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
            this.SizeChanged += new SizeChangedEventHandler(BPDtl_SizeChanged);
        }

        void BPDtl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (MyDG != null)
            {
                if (!Convert.IsDBNull(this.Width))
                    MyDG.Width = this.Width;
                if (!Convert.IsDBNull(this.Height))
                    MyDG.Height = this.Height;
            }
        }
        public void LoadDtl()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync("SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.Name + "'");
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            if (ds.Tables[0].Rows.Count == 0)
            {
                // this.NewDtl();
                return;
            }

            DataGrid dg = new DataGrid();
            dg.Name = "DG" + this.Name;
            DataGridTextColumn mycol = new DataGridTextColumn();
            mycol.Header = "IDX";
            dg.Columns.Add(mycol);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["UIVisible"].ToString() == "0")
                    continue;
                DataGridTextColumn txtColumn = new DataGridTextColumn();
                txtColumn.Header = dr["Name"];
                dg.Columns.Add(txtColumn);
            }
            dg.Width = this.Width;
            dg.Height = this.Height;
            this.Content = dg;
            this.MyDG = dg;
        }
        public void NewDtl()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("NewDtl", this.Name, Glo.FK_MapData, null, null, null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_New_DoTypeCompleted);
        }
        void da_New_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "提示", MessageBoxButton.OK);
                return;
            }

            this.LoadDtl();

            string url = Glo.BPMHost + "/WF/MapDef/MapDefDtlFreeFrm.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&FK_MapDtl=" + this.Name + Glo.TimeKey;
            Glo.WinOpen(url, 700, 1100);
            //   HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
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
        //public double X = 0;
        //public double Y = 0;
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
                    if (this.Name.Contains("_blank_") == false)
                    {
                        if (MessageBox.Show("您确定要删除吗？",
                            "删除提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            return;
                    }

                    this.DeleteIt();
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
            if (MessageBox.Show("您确定要删除明细表[" + this.Name + "]吗？如果确定它相关的字段与设置也将会被删除，物理表也将被删除。", "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("DelDtl", this.Name, null, null, null, null, null);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "删除错误", MessageBoxButton.OK);
                return;
            }

            try
            {
                Canvas c = this.Parent as Canvas;
                c.Children.Remove(this);
            }
            catch
            {
            }
        }
        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLsAsync("UPDATE Sys_MapDtl SET IsView=0 WHERE No='" + Glo.FK_MapData + "'");
            hidDA.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(hidDA_RunSQLsCompleted);
        }
        void hidDA_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
        #endregion 移动事件


        public double X
        {
            get
            {
                return Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
            }
            set
            {
                this.SetValue(Canvas.LeftProperty, value);
            }
        }
        public double Y
        {
            get
            {
                return Convert.ToDouble(this.GetValue(Canvas.TopProperty));
            }
            set
            {
                this.SetValue(Canvas.TopProperty, value);
            }
        }
    }
}
