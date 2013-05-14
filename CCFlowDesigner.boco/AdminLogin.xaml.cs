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

namespace WF
{
    public partial class AdminLogin : ChildWindow
    {
        public AdminLogin()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //WSTest.WSDesignerSoapClient wsc = new WSTest.WSDesignerSoapClient();
            //wsc.DoTypeAsync("AdminLogin", "admin", "pub", null, null, null);
            //wsc.DoTypeCompleted += new EventHandler<WSTest.DoTypeCompletedEventArgs>(wsc_DoTypeCompleted);
            //return;

            string user = this.textBox1.Text.Trim().ToLower();
            //if (user != "admin")
            //{
            //    MessageBox.Show("非管理员不能登陆", "Error", MessageBoxButton.OK);
            //    return;
            //}

            string pass = this.passwordBox1.Password.Trim().ToLower();
            var da = BP.Glo.GetDesignerServiceInstance();
            da.DoTypeAsync("AdminLogin", user, pass, null, null, null);
            da.DoTypeCompleted += new EventHandler<WS.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }
        void da_DoTypeCompleted(object sender, WS.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                return;
            }
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        //void wsc_DoTypeCompleted(object sender, WSTest.DoTypeCompletedEventArgs e)
        //{
        //    if (e.Result != null)
        //    {
        //        MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
        //        return;
        //    }
        //    this.DialogResult = true;
        //}
    }
}

