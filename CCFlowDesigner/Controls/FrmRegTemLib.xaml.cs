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
using System.Text.RegularExpressions;

using WF.WSFtp;

namespace BP.Controls
{
    public partial class FrmRegTemLib : ChildWindow
    {
        CYFtpSoapClient ftpClient;

        public FrmRegTemLib()
        {
            InitializeComponent();

            ftpClient = BP.Glo.GetFtpServiceInstance();
        }

        private void RegUser()
        {
            ftpClient.RegUserCompleted += RegUser_Completed;
            ftpClient.RegUserAsync(txbName.Text.Trim(), txbPassword.Text.Trim()
                , txbEmail.Text.Trim());
        }

        private void RegUser_Completed(object sender, RegUserCompletedEventArgs e)
        {
            ftpClient.RegUserCompleted -= RegUser_Completed;
            if (e.Result.Trim() == "success")
            {
                MessageBox.Show("注册成功！");
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show(e.Result);
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            #region 验证

            if (string.IsNullOrEmpty(txbName.Text))
            {
                MessageBox.Show("用户名不能为空");
                return;
            }
            if (txbName.Text.Length <= 3)
            {
                MessageBox.Show("用户名长度不能小于3");
                return;
            }
            if (string.IsNullOrEmpty(txbPassword.Text)
            || string.IsNullOrEmpty(txbRePassword.Text))
            {
                MessageBox.Show("密码不能为空");
                return;
            }
            if (txbPassword.Text.Trim().Length < 6)
            {
                MessageBox.Show("密码长度不能小于6");
                return;
            }

            if (txbPassword.Text.Trim() != txbRePassword.Text.Trim())
            {
                MessageBox.Show("输入的密码不一致");
                return;
            }

            if (string.IsNullOrEmpty(txbEmail.Text))
            {
                MessageBox.Show("Email不能为空");
                return;
            }

            #endregion

            RegUser();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

