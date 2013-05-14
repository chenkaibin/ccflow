using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace CCForm
{
    public partial class FrmImg : ChildWindow
    {
        public FrmImg()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.HisImg.WinURL = this.TB_Url.Text;
            this.HisImg.WinTarget = this.TB_WinName.Text;

            if (FileName != null)
            {
                ImageBrush ib = new ImageBrush();
                BitmapImage png = new BitmapImage(new Uri(Glo.BPMHost + "/DataUser/ImgAth/Upload/" + FileName, UriKind.RelativeOrAbsolute));
                ib.ImageSource = png;
                this.HisImg.Background = ib;
                this.HisImg.HisPng = png;
            }

            BPImg img = Glo.currEle as BPImg;
            img.WinURL = this.TB_Url.Text;
            img.WinTarget = this.TB_WinName.Text;
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public BPImg HisImg = null;
        public string FileName { get; set; }
        public void BindIt(BPImg img)
        {
            HisImg = img;
            this.TB_Url.Text = img.WinURL;
            Glo.BindComboBoxWinOpenTarget(this.DDL_WinName, img.WinTarget);
            this.Show();
        }
        private void DDL_WinName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem it = (ComboBoxItem)this.DDL_WinName.SelectedItem;
            if (it == null)
                return;
            this.TB_WinName.Text = it.Tag.ToString();
            if (this.TB_WinName.Text == "def")
                this.TB_WinName.Text = "";
        }
        //上传图片
        private void Btn_B_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "PNG 图片 (*.png)|*.png|JPG 图片 (*.jpg)|*.jpg";

            bool? result = dlg.ShowDialog();

            if (result != null && result == true)
            {
                string name = dlg.File.Name;
                string extension = name.Substring(name.LastIndexOf('.'), name.Length - name.LastIndexOf('.')); //取得扩展名（包括“.”） 
                FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + extension; // 根据当前时间重命名 
                uploadImage(FileName, dlg.File.OpenRead());
                TB_File.Text = name;
            }
        }

        private void uploadImage(string fileName, Stream data)
        {
            Uri uri = new Uri(string.Format(Glo.BPMHost + "/WF/MapDef/CCForm/DataHandler.ashx?filename={0}", fileName), UriKind.RelativeOrAbsolute);

            WebClient client = new WebClient();
            client.OpenWriteCompleted += delegate(object s, OpenWriteCompletedEventArgs e)
            {
                uploadData(data, e.Result);
                e.Result.Close();
                data.Close();
            };
            client.OpenWriteAsync(uri);
        }
        private void uploadData(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytes;

            while ((bytes = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytes);
            }
        }
    }
}

