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
using System.ServiceModel;
using WF.WS;
using BP;

namespace BP.Frm
{
    public partial class FrmSortEdit : ChildWindow
    {
        protected override void OnClosed(EventArgs e)
        {
            System.Threading.Thread.Sleep(10);
            this.HisMainPage.BindFormTree();
            base.OnClosed(e);
        }
        public string No = "";
        public MainPage HisMainPage = null;
        public FrmSortEdit()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.No == "")
            {
                var doit = BP.Glo.GetDesignerServiceInstance();
                doit.DoAsync("NewFrmSort", this.TB_Name.Text, true);
                doit.DoCompleted += new EventHandler<DoCompletedEventArgs>(doit_DoCompleted);
            }
            else
            {
                string strs = "";
                strs += "@EnName=BP.Sys.FrmSort@PKVal=" + this.No;
                strs += "@Name=" + this.TB_Name.Text;
                var da = BP.Glo.GetDesignerServiceInstance();
                da.SaveEnAsync(strs);
                da.SaveEnCompleted += new EventHandler<SaveEnCompletedEventArgs>(da_SaveEnCompleted);
            }
        }

       
        void doit_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                return;
            }
            this.DialogResult = true;
        }
        void da_SaveEnCompleted(object sender, SaveEnCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (e.Result.Length != 2)
                {
                    MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                    return;
                }
            }
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult =false;
        }
    }
}

