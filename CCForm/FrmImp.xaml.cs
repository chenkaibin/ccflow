﻿using System;
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
using System.Windows.Interactivity;
using Silverlight;
using BP.En;
using BP.Sys;
using CCForm.FF;

namespace CCForm
{
    public partial class FrmImp : ChildWindow
    {
        public FrmImp()
        {
            InitializeComponent();
        }
        protected override void OnOpened()
        {
            this.TB_File.Text = "";
            string sql = "SELECT NodeID ,Name, Step FROM WF_Node WHERE FK_Flow='" + Glo.FK_Flow + "'";
            //  sql += "@SELECT No, Name FROM Sys_MapData WHERE FK_Flow='" + Glo.FK_Flow + "'";
            sql += "@SELECT No, Name FROM Sys_MapData WHERE FK_FrmSort IN (SELECT No from Sys_FrmSort )";
            //   sql += "@SELECT No, Name FROM Sys_MapData WHERE FK_Flow='" + Glo.FK_Flow + "'";

            CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableSAsync(sql);
            da.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(da_RunSQLReturnTableSCompleted);

            if (this.canvas1.Children.Count == 0)
            {
                da.FtpMethodAsync("GetDirs", "/Form.表单模版/", null, null);
                da.FtpMethodCompleted += new EventHandler<FtpMethodCompletedEventArgs>(da_FtpMethodCompleted);
            }
            base.OnOpened();
        }

        public MainPage HisMainPage = null;

        void da_FtpMethodCompleted(object sender, FtpMethodCompletedEventArgs e)
        {
            if (e.Result == null || e.Result.Contains("@") == false)
            {
                MessageBox.Show("无法连接ftp://templete.ccflow.org，无法获取共享表单模板资源。");
                return;
            }

            string[] strs = e.Result.Split('@');
            int idx = 0;
            foreach (string s in strs)
            {
                BPDir img = new BPDir();

                img.Cursor = Cursors.Hand;
                img.SetValue(Canvas.LeftProperty, (double)30);
                img.SetValue(Canvas.TopProperty, (double)30);

                img.Width = 32;
                img.Height = 32;

                MouseDragElementBehavior mdeImg = new MouseDragElementBehavior();
                Interaction.GetBehaviors(img).Add(mdeImg);
                this.canvas1.Children.Add(img);
                img.MouseLeftButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
                img.MouseRightButtonDown += new MouseButtonEventHandler(img_MouseLeftButtonDown);
            }
        }
        void img_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
        void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
        void da_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            this.listBox1.Items.Clear();
            this.listBox_FrmLab.Items.Clear();

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ListBoxItem lb = new ListBoxItem();
                lb.Content = "Step:" + dr["Step"] + " " + dr["Name"].ToString();
                lb.Tag = "ND" + dr["NodeID"];
                if (Glo.FK_MapData == lb.Tag.ToString())
                    continue;
                this.listBox1.Items.Add(lb);
            }

            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                ListBoxItem lb = new ListBoxItem();
                lb.Content = dr["Name"].ToString();
                lb.Tag = dr["No"];
                if (Glo.FK_MapData == lb.Tag.ToString())
                    continue;
                this.listBox_FrmLab.Items.Add(lb);
            }
        }
              
        OpenFileDialog dialog = new OpenFileDialog();
        LoadingWindow loadingWindow = new LoadingWindow();
        FileInfo file = null;
        private byte[] buffer;
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("导入会清除当前的设计内容，您确定要执行吗？",
                "警告", MessageBoxButton.OKCancel)
            == MessageBoxResult.Cancel)
                return;

            bool isClear = (bool)this.CB_IsClear.IsChecked;
            bool IsSetReadonly = (bool)this.CB_IsSetReadonly.IsChecked;
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    break;
                case 1: // 从本机上装文件。
                    if (buffer == null || buffer.Length <= 0 || file == null)
                    {
                        MessageBox.Show("请选择模板文件", "提示", MessageBoxButton.OK);
                        return;
                    }
                    loadingWindow.Title = "正在装载文件模版请稍后...";
                    loadingWindow.Show();
                    FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                    da.LoadFrmTempleteAsync(buffer, Glo.FK_MapData, true);
                    da.LoadFrmTempleteCompleted += new EventHandler<FF.LoadFrmTempleteCompletedEventArgs>(da_LoadFrmTempleteCompleted);
                    break;
                case 2: // 从节点表单.
                    loadingWindow.Title = "正在装载节点表单请稍后...";
                    loadingWindow.Show();
                    ListBoxItem lb = this.listBox1.SelectedItem as ListBoxItem;
                    CCFormSoapClient fda = Glo.GetCCFormSoapClientServiceInstance();
                    fda.CopyFrmAsync(lb.Tag.ToString(), Glo.FK_MapData, isClear, IsSetReadonly);
                    fda.CopyFrmCompleted += new EventHandler<CopyFrmCompletedEventArgs>(da_CopyFrmCompleted);
                    break;
                case 3: // 从流程表单.
                    loadingWindow.Title = "正在装载流程表单请稍后...";
                    loadingWindow.Show();
                    ListBoxItem lb44 = this.listBox_FrmLab.SelectedItem as ListBoxItem;
                    if (lb44 == null)
                        return;
                    CCFormSoapClient fdaa = Glo.GetCCFormSoapClientServiceInstance();
                    fdaa.CopyFrmAsync(lb44.Tag.ToString(), Glo.FK_MapData, isClear,true);
                    fdaa.CopyFrmCompleted += new EventHandler<CopyFrmCompletedEventArgs>(da_CopyFrmCompleted);
                    break;
                default:
                    break;
            }
        }
        void da_LoadFrmTempleteCompleted(object sender, FF.LoadFrmTempleteCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "错误", MessageBoxButton.OK);
                loadingWindow.DialogResult = false;
                return;
            }
            this.DialogResult = true;
            loadingWindow.DialogResult = false;
        }
        void da_CopyFrmCompleted(object sender, CopyFrmCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                this.DialogResult = true;
                loadingWindow.DialogResult = false;
            }
            else
            {
                MessageBox.Show(e.Result);
            }
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            var selectedItem = this.tabControl1.SelectedItem as TabItem;
            if (null == selectedItem)
                return;

            //调用服务上传
            try
            {
                loadingWindow.Show();
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.UploadFileAsync(buffer, "\\Temp\\" + Glo.FK_MapData + ".xml");
                da.UploadFileCompleted += new EventHandler<FF.UploadFileCompletedEventArgs>(da_UploadFileCompleted);
            }
            catch (Exception ex)
            {
                this.DialogResult = false;
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
            }
            this.DialogResult = true;
            loadingWindow.DialogResult = false;
        }

        void da_UploadFileCompleted(object sender, FF.UploadFileCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                loadingWindow.DialogResult = true;
                return;
            }

            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_Show_Click(object sender, RoutedEventArgs e)
        {
            dialog.Filter = "Xml Files (.xml)|*.xml|All Files (*.*)|*.*";
            if (dialog.ShowDialog().Value)
            {
                // 选择上传的文件
                file = dialog.File;
                Stream stream = file.OpenRead();
                stream.Position = 0;
                buffer = new byte[stream.Length + 1];

                //将文件读入字节数组
                stream.Read(buffer, 0, buffer.Length);
                this.TB_File.Text = dialog.File.Name;
            }
            else
            {
                MessageBox.Show("请选择文件！", "提示", MessageBoxButton.OK);
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void listBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void Btn_ImpIntenert_Click(object sender, RoutedEventArgs e)
        {
            FrmImpFromInternet im = new FrmImpFromInternet();
            im.Show();
        }

        private void Btn_ImpIntenert_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            FrmImpFromInternet im = new FrmImpFromInternet();
            im.HisMainPage = this.HisMainPage;
            im.Show();
        }
    }
}
