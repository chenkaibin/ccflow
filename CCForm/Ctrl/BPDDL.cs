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

namespace CCForm
{
    public class BPDDL : System.Windows.Controls.ComboBox
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

        //protected override void OnDropDownOpened(EventArgs e)
        //{
        //    Glo.currEle = this;
        //  //  Glo.IsMouseDown = true;
        //    base.OnDropDownOpened(e);
        //}
       
        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    Glo.currEle = this;
        //  //  Glo.IsMouseDown = true;
        //    base.OnGotFocus(e);
        //}
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    Glo.currEle = this;
        //    Glo.IsMouseDown = true;
        //    base.OnMouseLeftButtonDown(e);
        //}
        //protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        //{
        //    Glo.IsMouseDown = false;
        //    base.OnMouseLeftButtonUp(e);
        //}

        #region bind Enum
        public string UIBindKey = "";
        public string HisLGType = LGType.Enum;
        public string HisDataType
        {
            get
            {
                if (this.HisLGType == LGType.Enum)
                    return DataType.AppInt;
                else
                    return DataType.AppString;
            }
        }
        public string KeyName = null;
        /// <summary>
        /// BPDDL
        /// </summary>
        public BPDDL()
        {
            this.Name = "DDL" + DateTime.Now.ToString("yyMMddhhmmss");
            this.AllowDrop = true;
        }
        #endregion bind Enum

        #region bind Enum
        /// <summary>
        /// enumID
        /// </summary>
        /// <param name="enumID"></param>
        public void BindEnum(string enumID)
        {
            this.AllowDrop = true;

            this.UIBindKey = enumID;
            this.HisLGType = LGType.Enum;
            string sql = "SELECT IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + enumID + "'";
            this.BindSQL(sql);
        }
        /// <summary>
        /// ensName
        /// </summary>
        /// <param name="ensName"></param>
        public void BindEns(string ensName)
        {
            this.AllowDrop = true;
            this.UIBindKey = ensName;
            this.HisLGType = LGType.FK;
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RequestSFTableAsync(ensName);
            da.RequestSFTableCompleted += new EventHandler<FF.RequestSFTableCompletedEventArgs>(da_RequestSFTableCompleted);
        }
        void da_RequestSFTableCompleted(object sender, FF.RequestSFTableCompletedEventArgs e)
        {
            try
            {
                Silverlight.DataSet ds = new Silverlight.DataSet();
                ds.FromXml(e.Result);
                this.Items.Clear();
                foreach (Silverlight.DataRow dr in ds.Tables[0].Rows)
                {
                    ListBoxItem li = new ListBoxItem();
                    li.Tag = dr["No"];
                    li.Content = dr["Name"];
                    this.Items.Add(li);
                }
                if (this.Items.Count != 0)
                    this.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\t\n数据:" + e.Error.Message);
            }
        }
        #endregion bing Enum

        #region bind table
        public void BindSQL(string sql)
        {
            try
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.RunSQLReturnTableAsync(sql);
                da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            try
            {
                Silverlight.DataSet ds = new Silverlight.DataSet();
                ds.FromXml(e.Result);

                this.Items.Clear();
                foreach (Silverlight.DataRow dr in ds.Tables[0].Rows)
                {
                    this.Items.Add(dr["No"] + ":" + dr["Name"]);
                }

                if (this.Items.Count != 0)
                    this.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion bing Enum

        #region 移动事件
        bool isCopy = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            // 获取 textBox 对象的相对于 Canvas 的 x坐标 和 y坐标
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            

            // KeyEventArgs.Key - 与事件相关的键盘的按键 [System.Windows.Input.Key枚举]
            switch (e.Key)
            {
                // 按 Up 键后 textBox 对象向 上 移动 1 个像素
                // Up 键所对应的 e.PlatformKeyCode == 38 
                // 当获得的 e.Key == Key.Unknown 时，可以使用 e.PlatformKeyCode 来确定用户所按的键
               
                case Key.Up:
                case Key.W:
                    this.SetValue(Canvas.TopProperty, y - 1);
                    break;
                case Key.Down:
                case Key.S:
                    this.SetValue(Canvas.TopProperty, y + 1);
                    break;

                case Key.Left:
                case Key.A:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                        if (this.Width > 10)
                            this.Width = this.Width - 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x - 1);
                    }
                    break;
                case Key.Right:
                case Key.D:
                    if (Keyboard.Modifiers == ModifierKeys.Shift)
                    {
                            this.Width = this.Width + 1;
                    }
                    else
                    {
                        this.SetValue(Canvas.LeftProperty, x + 1);
                    }
                    break;
                case Key.Delete:

                    this.DeleteIt();

                    break;
                case Key.C:
                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        MessageBox.Show("此类型控件不支持复制。","提示",MessageBoxButton.OK);
                        //***********下拉框复制暂时取消*************//
                        //BPDDL ctl = new BPDDL();
                        //ctl.Cursor = Cursors.Hand;
                        //ctl.SetValue(Canvas.LeftProperty, (double)this.GetValue(Canvas.LeftProperty) + 15);
                        //ctl.SetValue(Canvas.TopProperty, (double)this.GetValue(Canvas.TopProperty) + 15);
                        //ctl.Items.Add("ComboBox");
                        //ctl.SelectedIndex = 0;
                        //Canvas s1c = this.Parent as Canvas;
                        //try
                        //{
                        //    s1c.Children.Add(ctl);
                        //}
                        //catch
                        //{
                        //    s1c.Children.Remove(ctl);
                        //}
                    }
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }
        #endregion 移动事件

        public bool IsDelete = false;
        public void DeleteIt()
        {
            if (this.Name != null)
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                string sqls = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND KeyOfEn='" + this.Name + "'";
                da.RunSQLsAsync(sqls);
            }
            Canvas c = this.Parent as Canvas;
            c.Children.Remove(this);
            IsDelete = true;
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
    }
}
