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


namespace WorkNode
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
        /// <summary>
        /// 设置选择的值
        /// </summary>
        /// <param name="val">要选择的值</param>
        public bool SetSelectVal(string val)
        {
            foreach (ListBoxItem item in this.Items)
            {
                item.IsSelected = false;
            }

            foreach (ListBoxItem item in this.Items)
            {
                if (item.Tag.ToString() == val)
                {
                    item.IsSelected = true;
                    this.SelectedItem = item;
                    this.SelectedValue = item;
                    return true;
                }
            }
            return false;
        }
        #endregion 处理选中.

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
        }
        #endregion bind Enum

        #region bind Enum
        /// <summary>
        /// enumID
        /// </summary>
        /// <param name="enumID"></param>
        public void BindEnum(string enumID)
        {
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
            this.UIBindKey = ensName;
            this.HisLGType = LGType.FK;
            FF.CCFlowAPISoapClient da = Glo.GetCCFlowAPISoapClientServiceInstance();
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
                FF.CCFlowAPISoapClient da = Glo.GetCCFlowAPISoapClientServiceInstance();
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
                MessageBox.Show(ex.Message);
            }
        }
        #endregion bing Enum
    }
}
