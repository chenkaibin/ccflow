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
    public partial class SelectM2M : ChildWindow
    {
        public double X = 0;
        public double Y = 0;
        public int IsM2M = 0;
        public SelectM2M()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.DoTypeAsync("NewM2M",Glo.FK_MapData,this.TB_No.Text, this.IsM2M.ToString(),this.IsM2M.ToString(), this.X.ToString(),this.Y.ToString());
            ff.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(ff_DoTypeCompleted);
        }

        void ff_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result,"提示", MessageBoxButton.OK);
                return;
            }

            Glo.TempVal = this.TB_No.Text;
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
    }
}

