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
using System.IO;
using BP;
using WF.WS;
using Silverlight;
using System.Collections;
namespace BP.Controls
{
    public partial class FrmNewFlow : ChildWindow
    {
        #region Private Variables
        private LoadingWindow loadingWindow = new LoadingWindow();
        OpenFileDialog dialog = new OpenFileDialog();
        private byte[] buffer;
        FileInfo file; 
        #endregion

        #region Events
        public event EventHandler<FlowTemplete_LoadCompletedEventArgs> FlowTempleteLoadCompeletedEventHandler;
        
        #endregion

        #region Properties
        /// <summary>
        /// 当前设计器的实例
        /// </summary>
        public MainPage CurrentDesinger { get; set; }
        /// <summary>
        /// 默认流程类别
        /// </summary>
        public String CurrentFlowSortName { get; set; }
        #endregion

        #region Constructs
        public FrmNewFlow()
        {
            InitializeComponent();

            WSDesignerSoapClient da = Glo.GetDesignerServiceInstance();
            da.RunSQLReturnTableAsync("SELECT no,name FROM WF_FlowSort ");
            da.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);


            DataTable dt=new DataTable();
            dt.Columns.Add(new DataColumn("No"));
            dt.Columns.Add(new DataColumn("Name"));

            DataRow dr = dt.NewRow();
            dr[0] = "0";
            dr[1] = "数据轨迹模式";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "1";
            dr[1] = "数据合并模式";
            dt.Rows.Add(dr);
            IList list = dt.GetBindableData(new Connector());

            DDL_DataStoreModel.ItemsSource = list;
            DDL_DataStoreModel.DisplayMemberPath = dt.Columns[1].ColumnName;
            DDL_DataStoreModel.SelectedIndex = 0;
        }
        void da_RunSQLReturnTableCompleted(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];

            // 得到默认的流程类别
            int defaultFlowSort = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][1] == CurrentFlowSortName)
                {
                    defaultFlowSort = i;
                }
            }

            IList list = dt.GetBindableData(new Connector());
            if (list.Count > 0)
            {
                cbxFlowSortImport.ItemsSource = list;
                cbxFlowSortImport.DisplayMemberPath = dt.Columns[1].ColumnName;
                cbxFlowSortImport.SelectedIndex = defaultFlowSort;

                DDL_FlowSort.ItemsSource = list;
                DDL_FlowSort.DisplayMemberPath = dt.Columns[1].ColumnName;
                DDL_FlowSort.SelectedIndex = defaultFlowSort;
            }
        }
        #endregion

        #region Private Methods
		private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = tabControl.SelectedItem as TabItem;
            if (null == selectedItem)
                return;

            if (selectedItem == tabStandardNew)
            {
                if (string.IsNullOrWhiteSpace(TB_FlowName.Text))
                {
                    MessageBox.Show("请输入流程名称", "提示", MessageBoxButton.OK);
                    return;
                }
                if (null != CurrentDesinger)
                {
                    var flowSortID = (DDL_FlowSort.SelectedItem as BindableObject).GetValue("NO");
                    var DataStoreModel = this.DDL_DataStoreModel.SelectedIndex;
                    var ptable = this.TB_PTable.Text;
                    var flowCode = this.TB_FlowCode.Text;

                    CurrentDesinger.NewFlow(flowSortID, TB_FlowName.Text, DataStoreModel, ptable, flowCode);
                    this.DialogResult = true;
                }
            }

            if (selectedItem == tabImportNew)
            {
                if (buffer == null || buffer.Length <= 0 || file == null || cbxFlowSortImport.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择模板文件", "提示", MessageBoxButton.OK);
                    return;
                }

                //调用服务上传
                try
                {
                    loadingWindow.Show();

                    WSDesignerSoapClient daUpload = Glo.GetDesignerServiceInstance();
                    daUpload.UploadfileAsync(buffer, file.Name);
                    daUpload.UploadfileCompleted += _Service_UploadfileCompleted;

                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    this.DialogResult = false;
                    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnUpLoad_Click(object sender, RoutedEventArgs e)
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

                tbxFileName.Text = dialog.File.Name;
            }
            else
            {
                MessageBox.Show("请选择文件！", "提示", MessageBoxButton.OK);
            }
        }
        void _Service_UploadfileCompleted(object sender, UploadfileCompletedEventArgs e)
        {
            if (e.Result.Contains("Error:"))
            {
                loadingWindow.Close();
                MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                return;
            }

            WSDesignerSoapClient daLoadIt = Glo.GetDesignerServiceInstance();
            daLoadIt.FlowTemplete_LoadAsync((cbxFlowSortImport.SelectedItem as BindableObject).GetValue("NO"),
                e.Result, true);
            daLoadIt.FlowTemplete_LoadCompleted += _service_FlowTemplete_LoadCompleted;
        }

        void _service_FlowTemplete_LoadCompleted(object sender, FlowTemplete_LoadCompletedEventArgs e)
        {
            loadingWindow.Close();

            if (null != FlowTempleteLoadCompeletedEventHandler)
            {
                FlowTempleteLoadCompeletedEventHandler(sender, e);
            }
        }
 
	    #endregion    
    }
}

