using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCForm
{
    public partial class SelectTB : ChildWindow
    {
        public bool IsCheckGroup = false;
        public UIElementCollection eles = null;
        public SelectTB()
        {
            InitializeComponent();
            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
        }

        protected override void OnOpened()
        {
            this.IsCheckGroup = false;
            this.TB_KeyOfEn.Text = "";
            this.TB_Name.Text = "";
            this.CB_IsHid.IsChecked = false;
            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
            base.OnOpened();
        }

        void HisFrmSelectCheckGroup_Closed(object sender, EventArgs e)
        {
            if (this.HisFrmSelectCheckGroup.DialogResult == false)
                return;

            this.TB_Name.Text = this.HisFrmSelectCheckGroup.TB_GroupName.Text;
            this.TB_KeyOfEn.Text = this.HisFrmSelectCheckGroup.TB_GroupKey.Text;
            this.IsCheckGroup = true;
            this.DialogResult = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_KeyOfEn.Text)
                || string.IsNullOrEmpty(this.TB_Name.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "Note", MessageBoxButton.OK);
                return;
            }

            if (this.CB_IsHid.IsChecked == true)
            {
                if (MessageBox.Show("隐藏字段只能在工具箱的隐藏区域才能找到。",
                    "您确定吗?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

                string dtype = "0";
                if (this.RB_Boolen.IsChecked == true)
                    dtype = BP.En.DataType.AppBoolean;

                if (this.RB_Data.IsChecked == true)
                    dtype = BP.En.DataType.AppDate;

                if (this.RB_DataTime.IsChecked == true)
                    dtype = BP.En.DataType.AppDateTime;

                if (this.RB_Float.IsChecked == true)
                    dtype = BP.En.DataType.AppFloat;

                if (this.RB_Int.IsChecked == true)
                    dtype = BP.En.DataType.AppInt;

                if (this.RB_Money.IsChecked == true)
                    dtype = BP.En.DataType.AppMoney;

                if (this.RB_String.IsChecked == true)
                    dtype = BP.En.DataType.AppString;

                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.DoTypeAsync("NewHidF", Glo.FK_MapData, this.TB_KeyOfEn.Text, this.TB_Name.Text, dtype, null);
                da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
            }
            else
            {
                this.DialogResult = true;
            }
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                MessageBox.Show("执行成功.", "提示", MessageBoxButton.OK);
                this.DialogResult = false;
            }
            else
            {
                MessageBox.Show( e.Result, "执行错误", MessageBoxButton.OK);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.ParseStringToPinyinAsync(this.TB_Name.Text);
            ff.ParseStringToPinyinCompleted += new EventHandler<FF.ParseStringToPinyinCompletedEventArgs>(ff_ParseStringToPinyinCompleted);
        }
        void ff_ParseStringToPinyinCompleted(object sender, FF.ParseStringToPinyinCompletedEventArgs e)
        {
            if (e.Result != null)
                this.TB_KeyOfEn.Text = e.Result;
        }

        private void Btn_CreateCheckGroup_Click(object sender, RoutedEventArgs e)
        {
            HisFrmSelectCheckGroup = new SelectCheckGroup();
            HisFrmSelectCheckGroup.TB_GroupKey.Text = "";
            HisFrmSelectCheckGroup.TB_GroupName.Text = "";
            HisFrmSelectCheckGroup.Show();
            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
        }
        public SelectCheckGroup HisFrmSelectCheckGroup = new SelectCheckGroup();

    }
}

