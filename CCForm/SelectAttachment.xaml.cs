using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class SelectAttachment : ChildWindow
    {
        public SelectAttachment()
        {
            InitializeComponent();
        }
        public BPAttachment HisBPAttachment = null;
        public void BindIt(BPAttachment ment)
        {
            this.HisBPAttachment = ment;
            this.TB_No.Text = ment.Name;
            this.TB_Name.Text = ment.Label;
            this.TB_Exts.Text = ment.Exts;
            this.TB_SaveTo.Text = ment.SaveTo;
            this.CB_IsDelete.IsChecked = ment.IsDelete;
            this.CB_IsDownload.IsChecked = ment.IsDownload;
            this.CB_IsUpload.IsChecked = ment.IsUpload;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_Name.Text)
               || string.IsNullOrEmpty(this.TB_No.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "Note", MessageBoxButton.OK);
                return;
            }

            #region 属性.
            string mypk = this.TB_No.Text.Trim();
            string vals = "@EnName=BP.Sys.FrmAttachment@MyPK=" +Glo.FK_MapData +"_" + mypk + "@FK_MapData=" + Glo.FK_MapData + "@Name=" + this.TB_Name.Text + "@Exts=" + this.TB_Exts.Text + "@NoOfObj=" + mypk;

            vals += "@SaveTo=" + this.TB_SaveTo.Text.Trim();
            vals += "@X=" +this.HisBPAttachment.X;
            vals += "@Y=" + this.HisBPAttachment.Y;

            if (this.CB_IsDelete.IsChecked == true)
                vals += "@IsDelete=1";
            else
                vals += "@IsDelete=0";

            if (this.CB_IsDownload.IsChecked == true)
                vals += "@IsDownload=1";
            else
                vals += "@IsDownload=0";

            if (this.CB_IsUpload.IsChecked == true)
                vals += "@IsUpload=1";
            else
                vals += "@IsUpload=0";

            #endregion 属性.

            FF.CCFormSoapClient daSaveFile = Glo.GetCCFormSoapClientServiceInstance();
            daSaveFile.SaveEnAsync(vals);
            daSaveFile.SaveEnCompleted += new EventHandler<FF.SaveEnCompletedEventArgs>(daSaveFile_SaveEnCompleted);
        }
        void daSaveFile_SaveEnCompleted(object sender, FF.SaveEnCompletedEventArgs e)
        {
            if (e.Result.Contains("Err"))
            {
                MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                return;
            }

            if (this.HisBPAttachment == null)
                this.HisBPAttachment = new BPAttachment();

            this.HisBPAttachment.Label = this.TB_Name.Text;
            this.HisBPAttachment.Exts = this.TB_Exts.Text;
            this.HisBPAttachment.IsDelete = (bool)this.CB_IsDelete.IsChecked;
            this.HisBPAttachment.IsDownload = (bool)this.CB_IsDownload.IsChecked;
            this.HisBPAttachment.IsUpload = (bool)this.CB_IsUpload.IsChecked;
            this.HisBPAttachment.SaveTo = this.TB_SaveTo.Text;
            MessageBox.Show("保存成功.", "Save OK", MessageBoxButton.OK);
            this.DialogResult = true;
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
                this.TB_No.Text = e.Result;
        }

        private void TB_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TB_No_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void CB_IsDownload_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}

