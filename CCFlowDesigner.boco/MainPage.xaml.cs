using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using WF;
using WF.WS;
using Silverlight;
using BP.Controls;
using Liquid;
using WF.Designer;
using System.IO;
using BP;
using WF.WSFtp;
using BP.Frm;

namespace BP
{
    /// <summary>
    /// 设计器主页面
    /// </summary>
    public partial class MainPage : UserControl
    {
        #region 变量

        private TreeNode firstNodeByFlow = new TreeNode();
        private System.Windows.Threading.DispatcherTimer _doubleClickTimer;
        private string FlowTempleteUrl = "";
        private string title; // 子窗体标题       
        private WSDesignerSoapClient _service = Glo.GetDesignerServiceInstance();

        // 最后的流程类型，用于重新绑定流程树后，再打开最后操作的流程类别
        private string CurrFK_FlowSort = "01";

        private List<ToolbarButton> ToolBarButtonList = new List<ToolbarButton>();
        private const string ToolBarEnableIsFlowSensitived = "EnableIsFlowSensitived";

       // private FrmShareFlow frmShareFlow;
        CYFtpSoapClient ftpClient = null;

        #endregion

        #region 属性

        public static readonly string CustomerId = "CCFlow";


        public WSDesignerSoapClient _Service
        {
            get { return _service; }
            set { _service = value; }
        }


        private Container SelectedContainer
        {
            get
            {
                var c = (Container)tbDesigner.SelectedContent;
                return c;
            }
        }

        public bool IsRefresh { get; set; }

        #endregion

        #region Enum
        /// <summary>
        /// 窗口打开方式 
        /// </summary>
        public enum WindowModelEnum
        {
            Dialog,
            Window
        }
        #endregion

        #region Constructs
        private AdminLogin adminLogin = new AdminLogin();
        /// <summary>
        /// 构造方法
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            ftpClient = Glo.GetFtpServiceInstance();
            adminLogin.Closed += new EventHandler(frmlogin_Closed);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("UserNo")
            && System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("SID"))
            {
                string userNo = System.Windows.Browser.HtmlPage.Document.QueryString["UserNo"];
                string passWord = System.Windows.Browser.HtmlPage.Document.QueryString["SID"];

                var ws = Glo.GetDesignerServiceInstance();
                ws.RunSQLReturnTableAsync("SELECT SID FROM Port_Emp WHERE No='" + userNo + "'");
                ws.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(ws_RunSQLReturnTableCompleted);
            }
            else
            {
                adminLogin.Show();
            }
        }

        void ws_RunSQLReturnTableCompleted(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                try
                {
                    ds.FromXml(e.Result);
                }
                catch (Exception ex)
                {
                    throw new Exception("@系统出现异常" + ex.StackTrace);
                }

                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0)
                    throw new Exception("@用户名或者密码错误.");

                string sid = System.Windows.Browser.HtmlPage.Document.QueryString["SID"];
                string s = dt.Rows[0][0].ToString();
                if (s.Equals(sid) == false)
                {
                    throw new Exception("@用户名或者密码错误.");
                }
                else
                {
                    frmlogin_Closed(null, null);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                adminLogin.Show();
            }
        }
        void frmlogin_Closed(object sender, EventArgs e)
        {
            if (sender != null)
            {
                if (adminLogin.DialogResult != true)
                    return;
            }

            try
            {
                var ws = Glo.GetDesignerServiceInstance();
                // 图片.
                ws.DoAsync("GetSettings", "CompanyID", true);
                ws.DoCompleted += ws_GetCustomerIdCompleted;

                // 流程树。
                this.BindFlowAndFlowSort();

                // 绑定formTree.
                this.BindFormTree();

                //装 toolbar.
                this.LoadToolbar();

                // InitDesignerXml
                ws = Glo.GetDesignerServiceInstance();
                ws.DoTypeAsync("InitDesignerXml", null, null, null, null, null);
                ws.DoTypeCompleted += ws_DoTypeCompleted;
                Application.Current.Host.Content.Resized += new EventHandler(Content_Resized);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 方法
        void ws_GetCustomerIdCompleted(object sender, DoCompletedEventArgs e)
        {
            try
            {
                var id = e.Result;
                if (id == null || id == "")
                    id = "CCFlow";
                imageLogo.Source = new BitmapImage(new Uri(string.Format("./Images/Logo/{0}/LogoW.jpg", id), UriKind.Relative));
                var brush = new ImageBrush(); //定义图片画刷
                brush.ImageSource = new BitmapImage(new Uri(string.Format("./Images/Logo/{0}/Welcome.jpg", id), UriKind.Relative));
                tbDesigner.Background = brush;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ws_DoTypeCompleted(object sender, DoTypeCompletedEventArgs e)
        {
            switch (Glo.TempVar)
            {
                case "DeleteFrmSort":
                case "DeleteFrm":
                    if (e.Result != null)
                        MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                    else
                        this.BindFormTree();
                    return;
                default:
                    break;
            }

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            this.TvwSysMenu.Nodes.Clear();
            DataTable dt = ds.Tables[1];
            TreeNode liP = new TreeNode();
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr["No"];
                string name = dr["Name"];
                string lab = dr["CH"];
                string w = dr["W"];
                string h = dr["H"];
                string url = dr["Url"];
                string icon = dr["Icon"];

                TreeNode li = new TreeNode();
                if (string.IsNullOrEmpty(icon) == false)
                    li.Icon = icon;

                li.Title = lab;
                li.Tag = dr;
                if (no.Length == 2)
                {
                    liP = li;
                    this.TvwSysMenu.Nodes.Add(li);
                }
                else
                {
                    li.MouseLeftButtonDown += TreeViewSysMenu_li_MouseLeftButtonDown;
                    liP.Nodes.Add(li);
                }
            }

            TvwSysMenu.ExpandAll();

            this.TvwFlowDataEnum.Nodes.Clear();
            dt = ds.Tables[0];
            liP = new TreeNode();
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr["No"];
                string name = dr["Name"];
                string lab = dr["CH"];
                string w = dr["W"];
                string h = dr["H"];
                string url = dr["Url"];
                string icon = dr["Icon"];
                TreeNode li = new TreeNode();
                if (string.IsNullOrEmpty(icon) == false)
                    li.Icon = icon;

                li.Title = lab;
                li.Tag = dr;
                if (no.Length == 2)
                {
                    liP = li;
                    this.TvwFlowDataEnum.Nodes.Add(li);
                }
                else
                {
                    li.MouseLeftButtonDown += TreeViewSysMenu_li_MouseLeftButtonDown;
                    liP.Nodes.Add(li);
                }
            }
            TvwFlowDataEnum.ExpandAll();
        }
        private void TreeViewSysMenu_li_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            TreeNode tn = sender as TreeNode;
            if (tn == null)
                return;
            DataRow dr = tn.Tag as DataRow;
            string w = dr["W"];
            string h = dr["H"];
            string url = dr["Url"];

            Glo.WinOpen(Glo.BPMHost + url, "name", int.Parse(h), int.Parse(w));
        }

        private void TreeViewSysMenu_li_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            return;
        }


        #region Flow CRUD related
        /// <summary>
        /// 打开工作流
        /// </summary>
        /// <param name="flowSortId"></param>
        /// <param name="flowId"></param>
        /// <param name="flowName"></param>
        private void OpenFlow(string flowSortId, string flowId, string flowName)
        {
            OpenFlow(flowId, flowName);
        }
        /// <summary>
        /// 打开工作流
        /// </summary>
        /// <param name="flowid">工作流ID</param>
        /// <param name="title">工作流名称</param>
        private void OpenFlow(string flowid, string title)
        {
            foreach (TabItem t in tbDesigner.Items)
            {
                var ct = t.Content as Container;
                if (ct != null && ct.FlowID == flowid)
                {
                    tbDesigner.SelectedItem = t;
                    return;
                }
            }
            var c = new Container();
            c.Designer = this;
            c.FlowID = flowid;
            c.DrawFlows();

            var ti = new TabItemEx();
            ti.Content = c;
            Button btnClose = new Button();
            btnClose.Opacity = 0.2;
            btnClose.Content = "╳";
            btnClose.MinHeight = 15;
            btnClose.MinWidth = 17;
            btnClose.Click += new RoutedEventHandler(btnClose_Click);
            btnClose.ClickMode = ClickMode.Release;
            TextBlock tbx = new TextBlock();
            tbx.Text = title;
            tbx.Width = title.Length;

            Canvas cs = new Canvas();
            cs.Children.Add(btnClose);
            cs.Children.Add(tbx);
            cs.Height = 20;
            ti.Title = title;
            ti.Width = tbx.ActualWidth + btnClose.ActualWidth + 30;
            ti.Header = cs;
            btnClose.DataContext = ti;
            btnClose.SetValue(Canvas.LeftProperty, ti.Width - btnClose.ActualWidth - 20);
            btnClose.SetValue(Canvas.TopProperty, 0.0);

            tbx.SetValue(Canvas.TopProperty, btnClose.ActualHeight);
            tbx.SetValue(Canvas.LeftProperty, 0.0);
            ti.SetValue(TabControl.WidthProperty, tbx.ActualWidth + btnClose.ActualWidth + 40);

            ti.VerticalAlignment = VerticalAlignment.Top;
            ti.VerticalContentAlignment = VerticalAlignment.Top;

            tbDesigner.Items.Add(ti);
            tbDesigner.SelectedItem = ti;

            setToolBarButtonEnableStatus(true);
        }
        /// <summary>
        /// 删除工作流
        /// </summary>
        public void DeleteFlow(string flowid)
        {
            if (HtmlPage.Window.Confirm("您确定要删除编号为【"+flowid+"】的工作流吗？"))
            {
                _Service.DoAsync("DelFlow", flowid, true);
                _Service.DoCompleted += Server_DoCompletedToRefreshSortTree;
            }
        }
        void Server_DoCompletedToRefreshSortTree(object sender, DoCompletedEventArgs e)
        {
            _Service.DoCompleted -= Server_DoCompletedToRefreshSortTree;
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "Err", MessageBoxButton.OK);
                return;
            }
            foreach (TabItem t in tbDesigner.Items)
            {
                var ct = t.Content as Container;
                if (ct != null && ct.FlowID == TvwFlow.Selected.Name)
                {
                    tbDesigner.Items.Remove(t);
                    break;
                }
            }
            BindFlowAndFlowSort();
        }
        /// <summary>
        /// 删除工作流类别
        /// </summary>
        /// <param name="flowsortid">工作流类别ID</param>
        public void DeleteFlowSort(string flowsortid)
        {
            if (HtmlPage.Window.Confirm("您确认要删除吗？"))
            {
                _Service.DoAsync("DelFlowSort", flowsortid, true);
                _Service.DoCompleted += Server_DoCompletedToRefreshSortTree;
            }
        }
        /// <summary>
        /// 新建工作流
        /// </summary>
        public void NewFlow(string flowSortId, string flowName, int dataSaveModel, string pTable,string flowCode)
        {
            _Service.DoAsync("NewFlow", flowSortId + "," + flowName + "," + dataSaveModel + "," + pTable +","+ flowCode, true);
            loadingWin(true);
            _Service.DoCompleted += _service_DoCompleted;
        }
        #endregion

        #region Window or Dialog open related

        public void OpenDialog(string url, string title, int h, int w)
        {
            OpenWindowOrDialog(url, title, string.Format("dialogHeight:{0}px;dialogWidth:{1}px", h, w), WindowModelEnum.Dialog);
        }

        public void OpenWindow(string url, string title, int h, int w)
        {

            OpenWindowOrDialog(url, title, string.Format("height={0},width={1}", h, w), WindowModelEnum.Window);
        }

        public void OpenDialog(string url, string title)
        {
            OpenWindowOrDialog(url, title, "dialogHeight:600px;dialogWidth:800px", WindowModelEnum.Dialog);
        }

        /// <summary>
        /// 弹出网页窗口
        /// </summary>
        /// <param name="url">网页地址</param>
        private void OpenWindowOrDialog(string url, string title, string property, WindowModelEnum windowModel)
        {
            if (url.Contains("http") == false)
                url = Glo.BPMHost + url;

            if (WindowModelEnum.Dialog == windowModel)
            {
                HtmlPage.Window.Eval(
                    string.Format(
                        "window.showModalDialog('{0}',window,'dialogHeight:600px;dialogWidth:800px;help:no;scroll:auto;resizable:yes;status:no;');",
                        url));

            }
            else
            {
                HtmlPage.Window.Eval(
                    string.Format(
                        "window.open('{0}','{1}','{2};help=no,resizable=yes,status=no,scrollbars=1');", url,
                        title, property));

            }
        }
        #endregion

        private void releaseToFtp()
        {
            if (SelectedContainer == null)
                return;

            var result = SnapshotCapturer.SaveScreenToString();
            _service = Glo.GetDesignerServiceInstance();
            var sortId = SelectedContainer.FlowID;
            _Service.DoAsync("ReleaseToFTP", SelectedContainer.FlowID + "," + result, true);
            _Service.DoCompleted += _service_ReleaseToFTPCompleted;
        }
        private void _service_ReleaseToFTPCompleted(object sender, DoCompletedEventArgs e)
        {
            _Service.DoCompleted -= _service_GetFlowsCompleted;
        }
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoubleClick_Timer(object sender, EventArgs e)
        {
            _doubleClickTimer.Stop();
        }
        /// <summary>
        /// 绑定工作流树
        /// </summary>
        private void BindFlowAndFlowSort()
        {
            _service = Glo.GetDesignerServiceInstance();
            _Service.DoAsync("GetFlows", string.Empty, true);
            _Service.DoCompleted += _service_GetFlowsCompleted;
        }
        public void BindFormTree()
        {
            string sqls = "";
            sqls += "@SELECT No,Name FROM Sys_FrmSort";
            sqls += "@SELECT No,Name,FK_FrmSort FROM Sys_MapData WHERE FK_FrmSort IN (SELECT No FROM Sys_FrmSort)";
            var da = Glo.GetDesignerServiceInstance();
            da.RunSQLReturnTableSAsync(sqls);
            da.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(da_RunSQLReturnTableSCompleted);
        }
        void da_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            TreeNode firstNode = new TreeNode();
            this.FromTree.Clear();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var node = new TreeNode();
                node.Title = dr["Name"].ToString();
                node.ID = dr["No"].ToString();
                node.IsSort = true;
                node.Icon = "../Images/MenuItem/FlowSheet.png";
                firstNode.Nodes.Add(node);

                if (node.ID == this.CurrFK_FrmSort)
                    node.IsExpanded = true;
                this.FromTree.Nodes.Add(node);
            }

            foreach (DataRow d in ds.Tables[1].Rows)
            {
                var node = new TreeNode();
                node.Title = d["Name"].ToString();
                node.ID = d["FK_FrmSort"].ToString();
                node.Name = d["No"].ToString();
                node.IsSort = false;
                foreach (TreeNode ne in firstNode.Nodes)
                {
                    try
                    {
                        if (ne.ID == d["FK_FrmSort"].ToString())
                        {
                            ne.Nodes.Add(node);
                            ne.IsSort = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            // 完成绑定后，展开最后的FlowSort
            foreach (TreeNode node in FromTree.Nodes)
            {
                if (node.ID == this.CurrFK_FlowSort)
                {
                    node.IsExpanded = true;
                    node.Expand();
                    this.CurrFK_FlowSort = string.Empty;
                }
            }
        }
        /// <summary>
        /// 获取工作流类型事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _service_GetFlowsCompleted(object sender, DoCompletedEventArgs e)
        {
            DataSet ds = null;
            try
            {
                firstNodeByFlow = new TreeNode();
                TvwFlow.Clear();
                ds = new DataSet();
                ds.FromXml(e.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载流程树时发生了错误,请检查数据库和Web.Config。错误具体信息为:\n" + e.Result + "\t\n@异常信息:" + ex.Message,
                    "错误", MessageBoxButton.OK);
                return;
            }

            #region bing Flow.
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var node = new TreeNode();
                node.Title = dr["Name"].ToString();
                node.ID = dr["No"].ToString();
                node.IsSort = true;
                node.Icon = "../Images/MenuItem/FlowSort.png";
                firstNodeByFlow.Nodes.Add(node);
                TvwFlow.Nodes.Add(node);
            }
            foreach (DataRow d in ds.Tables[1].Rows)
            {
                var node = new TreeNode();
                node.Title = d["Name"].ToString();
                node.ID = d["FK_FlowSort"].ToString();
                node.Name = d["No"].ToString();
                node.IsSort = false;
                if (SelectedContainer != null)
                {
                    if (SelectedContainer.FlowID == node.Name)
                    {
                        var te = this.tbDesigner.SelectedItem as TabItemEx;
                        te.Title = node.Title;
                        Canvas cs = te.Header as Canvas;
                        TextBlock tbx = cs.Children[1] as TextBlock;
                        tbx.Text = node.Title;
                    }
                }
                foreach (TreeNode ne in firstNodeByFlow.Nodes)
                {
                    try
                    {
                        if (ne.ID == d["FK_FlowSort"].ToString())
                        {
                            ne.Nodes.Add(node);
                            ne.IsSort = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            // 完成绑定后，展开最后的FlowSort
            foreach (TreeNode node in TvwFlow.Nodes)
            {
                if (node.ID == this.CurrFK_FlowSort)
                {
                    node.IsExpanded = true;
                    node.Expand();
                    this.CurrFK_FlowSort = string.Empty;
                }
            }
            #endregion bing Flow.
            _Service.DoCompleted -= _service_GetFlowsCompleted;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MuFlowTree_ItemSelected(object sender, MenuEventArgs e)
        {
            if (e.Tag == null)
                return;
            switch (e.Tag.ToString())
            {
                case "menuExp":
                    BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                    exp.Show();
                    break;
                case "Help":
                    Glo.WinOpen("http://ccflow.org/Help.aspx?wd=设计器", "帮助", 900, 1200);
                    break;
                case "OpenFlow":
                    OpenFlow(TvwFlow.Selected.ID, TvwFlow.Selected.Name, TvwFlow.Selected.Title);
                    break;
                case "NewFlow_Blank":
                    NewFlowHandler(0);
                    break;
                case "NewFlow_Disk":
                    NewFlowHandler(1);
                    break;
                case "NewFlow_CCF":
                    NewFlowHandler(2);
                    break;
                case "NewFlowSort":
                    var newFlowSort = new NewFlowSort(this);
                    newFlowSort.DisplayType = NewFlowSort.DisplayTypeEnum.Add;
                    newFlowSort.ServiceDoCompletedEvent += AddEditFlowSortDoCompletedEventHandler;
                    newFlowSort.Show();
                    break;
                case "Delete":
                    var deleteFlowNode = TvwFlow.Selected as TreeNode;
                    if (null == deleteFlowNode)
                    {
                        break;
                    }
                    if (!deleteFlowNode.IsSort)
                    {
                        this.CurrFK_FlowSort = TvwFlow.Selected.ID;
                        DeleteFlow(TvwFlow.Selected.Name);

                        foreach (TabItem t in tbDesigner.Items)
                        {
                            var ct = t.Content as Container;
                            if (null != ct && ct.FlowID == TvwFlow.Selected.Name)
                            {
                                tbDesigner.Items.Remove(t);
                                break;
                            }
                        }
                    }
                    else
                    {
                        DeleteFlowSort(deleteFlowNode.ID);
                    }
                    break;
                case "Share":
                    {
                        ShareFlowTemplate();
                        break;
                    }
                case "Refresh":
                    this.BindFlowAndFlowSort();
                    break;
                case "Edit":
                    this.CurrFK_FlowSort = TvwFlow.Selected.ID;
                    var editFlowSort = new NewFlowSort(this);
                    editFlowSort.InitControl(TvwFlow.Selected.ID, TvwFlow.Selected.EditedTitle);
                    editFlowSort.DisplayType = NewFlowSort.DisplayTypeEnum.Edit;
                    editFlowSort.ServiceDoCompletedEvent += AddEditFlowSortDoCompletedEventHandler;
                    editFlowSort.Show();
                    break;
            }
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }

        #region 共享模板

        private string flowShareRoot = string.Empty;
        private string bitmap_str;
        private string bitmap_part;
        private int bitmap_part_length = 3999999;
        private string temDes = string.Empty;//模板描述

        /// <summary>
        /// 分享模板
        /// </summary>
        private void ShareFlowTemplate()
        {
            temDes = string.Empty;

            if (SelectedContainer == null || SelectedContainer.FlowID != TvwFlow.Selected.Name)
            {
                MessageBox.Show("请先打开流程图");
                return;
            }

            if (string.IsNullOrEmpty(flowShareRoot))
            {
                ftpClient.GetFlowShareRootCompleted += GetFlowShareRoot_Completed;
                ftpClient.GetFlowShareRootAsync();
            }
            else
            {
                GetFlowShareRoot_Completed(null, null);
            }
        }

        private void GetFlowShareRoot_Completed(object sender, GetFlowShareRootCompletedEventArgs e)
        {
            //ftpClient.GetFlowShareRootCompleted -= GetFlowShareRoot_Completed;
            //if (string.IsNullOrEmpty(flowShareRoot) && e != null)
            //{
            //    flowShareRoot = e.Result;
            //}

            //if (SessionManager.Session.ContainsKey("user"))
            //{
            //    FrmTemNote temNote = new FrmTemNote();
            //    temNote.Closed += (sender2, e2) =>
            //    {
            //        temDes = temNote.TemDescription;
            //        ShareFlow_Start(flowShareRoot);
            //    };
            //    temNote.Show();
            //}
            //else
            //{
            //    FrmLoginTemLib loginTemLib = new FrmLoginTemLib();
            //    loginTemLib.Closed += (sender1, e1) =>
            //    {
            //        if (loginTemLib.LoginSuccess)
            //        {
            //            FrmTemNote temNote = new FrmTemNote();
            //            temNote.Closed += (sender2, e2) =>
            //            {
            //                temDes = temNote.TemDescription;
            //                ShareFlow_Start(flowShareRoot);
            //            };
            //            temNote.Show();
            //        }
            //    };
            //    loginTemLib.Show();
            //}
        }

        /// <summary>
        /// 上传工作流
        /// </summary>
        /// <param name="path"></param>
        private void ShareFlow_Start(string path)
        {
            //loadingWin(true);
            //ElementToPNG eleToPng = new ElementToPNG();

            //bitmap_str = StringHandler.ToString(eleToPng.GetPNGStream(tbDesigner.SelectedContent as UIElement));
            //if (bitmap_str.Length > bitmap_part_length)
            //{
            //    bitmap_part = bitmap_str.Substring(0, bitmap_part_length);
            //    bitmap_str = bitmap_str.Substring(bitmap_part_length);
            //}
            //else
            //{
            //    bitmap_part = bitmap_str;
            //    bitmap_str = "";
            //}
            //ftpClient.SaveFlowBitmapCompleted += SaveFlowBitmap_Completed;

            //ftpClient.SaveFlowBitmapAsync(SelectedContainer.FlowID, bitmap_part, false);
        }

        /// <summary>
        /// 保存流程模板图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFlowBitmap_Completed(object sender, SaveFlowBitmapCompletedEventArgs e)
        {
            if (!string.IsNullOrEmpty(bitmap_str))
            {
                if (bitmap_str.Length > bitmap_part_length)
                {
                    bitmap_part = bitmap_str.Substring(0, bitmap_part_length);
                    bitmap_str = bitmap_str.Substring(bitmap_part_length);
                }
                else
                {
                    bitmap_part = bitmap_str;
                    bitmap_str = "";
                }

                ftpClient.SaveFlowBitmapAsync(SelectedContainer.FlowID, bitmap_part, true);
            }
            else//保存流程模板图片完成,开始上传模板
            {
                ftpClient.SaveFlowBitmapCompleted -= SaveFlowBitmap_Completed;

                ftpClient.UploadFlowCompleted += UploadFlow_Completed;
                ftpClient.UploadFlowAsync(SelectedContainer.FlowID, flowShareRoot, temDes);
            }
        }

        /// <summary>
        /// 上传流程模板完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UploadFlow_Completed(object sender, UploadFlowCompletedEventArgs e)
        {
            ftpClient.UploadFlowCompleted -= UploadFlow_Completed;
            loadingWin(false);

            MessageBox.Show("模板共享成功！");
        }

        #endregion

        public string CurrFK_FrmSort = "01";
        private void MuFrmTree_ItemSelected(object sender, MenuEventArgs e)
        {
            var selectedNode = this.FromTree.Selected as TreeNode;
            if (selectedNode == null)
                return;
            if (e.Tag == null)
                return;
            this.CurrFK_FrmSort = selectedNode.ID;
            Glo.TempVar = e.Tag.ToString();
            switch (e.Tag.ToString())
            {
                case "Frm_EditForm": //表单属性
                    BP.Frm.Frm frm = new BP.Frm.Frm();
                    frm.BindFrm(selectedNode.Name);
                    frm.HisMainPage = this;
                    frm.Show();
                    break;
                case "Frm_NewForm": //新建表单
                    BP.Frm.Frm frm1 = new BP.Frm.Frm();
                    frm1.BindNew();
                    frm1.HisMainPage = this;
                    frm1.Show();
                    break;
                case "Frm_FormDesignerFix": //设计傻瓜表单
                    Glo.WinOpenByDoType("CH", UrlFlag.FormFixModel, selectedNode.Name, null, null);
                    break;
                case "Frm_FormDesignerFree": //设计free表单
                    Glo.WinOpenByDoType("CH", UrlFlag.FormFreeModel, selectedNode.Name, null, null);
                    break;
                case "Help":
                    Glo.WinOpen("http://ccflow.org/Help.aspx?wd=设计器", "帮助", 900, 1200);
                    break;
                case "Frm_NewFormSort": //新建表单类别
                    BP.Frm.FrmSortEdit frmSortEdit = new BP.Frm.FrmSortEdit();
                    frmSortEdit.No = "";
                    frmSortEdit.TB_Name.Text = "New Form Sort";
                    frmSortEdit.HisMainPage = this;
                    frmSortEdit.Show();
                    break;
                case "Frm_EditSort": //编辑
                    BP.Frm.FrmSortEdit frmSortEdit1 = new BP.Frm.FrmSortEdit();
                    frmSortEdit1.No = selectedNode.ID;
                    frmSortEdit1.TB_Name.Text = selectedNode.Title;
                    frmSortEdit1.HisMainPage = this;
                    frmSortEdit1.Show();
                    break;
                case "Frm_Delete": //删除
                    var deleteFlowNode = this.FromTree.Selected as TreeNode;
                    if (null == deleteFlowNode)
                        break;

                    if (MessageBox.Show("您确认要删除吗？", "ccflow", MessageBoxButton.OKCancel)
                        == MessageBoxResult.Cancel)
                        return;

                    if (deleteFlowNode.IsSort == true)
                    {
                        var ws = Glo.GetDesignerServiceInstance();
                        Glo.TempVar = "DeleteFrmSort";
                        ws.DoTypeAsync(Glo.TempVar, deleteFlowNode.ID, null, null, null, null);
                        ws.DoTypeCompleted += new EventHandler<DoTypeCompletedEventArgs>(ws_DoTypeCompleted);
                    }
                    else
                    {
                        var ws = Glo.GetDesignerServiceInstance();
                        Glo.TempVar = "DeleteFrm";
                        ws.DoTypeAsync(Glo.TempVar, deleteFlowNode.Name, null, null, null, null);
                        ws.DoTypeCompleted += new EventHandler<DoTypeCompletedEventArgs>(ws_DoTypeCompleted);
                        //DeleteFrmSort(deleteFlowNode.ID);
                    }
                    break;
                case "Frm_Refresh": //刷新
                    this.BindFormTree();
                    break;
                case "Frm_Imp":  //Imp
                    BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                    imp.Show();
                    break;
                default:
                    MessageBox.Show("功能未完成:" + Glo.TempVar);
                    break;
            }
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        /// <summary>
        /// 添加流程类别关闭时要执行的动作，一般来说是刷新窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddEditFlowSortDoCompletedEventHandler(object sender, EventArgs e)
        {
            var add = (NewFlowSort)sender;
            if (add.DialogResult == true)
            {
                this.BindFlowAndFlowSort();
            }
        }
        void EditFrmSortDoCompletedEventHandler(object sender, EventArgs e)
        {
            switch (Glo.TempVar)
            {
                case "Frm_EditForm":
                case "Frm_NewForm":
                    var frm = (BP.Frm.Frm)sender;
                    if (frm.DialogResult == true)
                    {
                        this.BindFormTree();
                    }
                    break;
                case "Frm_NewFormSort":
                case "Frm_EditFormSort":
                    var add = (BP.Frm.FrmSortEdit)sender;
                    if (add.DialogResult == true)
                    {
                        this.BindFormTree();
                    }
                    break;
                default:
                    MessageBox.Show("未判断的标记:" + Glo.TempVar);
                    break;
            }
        }
        /// <summary>
        /// 在工作流树空白处按下鼠标左键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsFlowTree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        /// <summary>
        /// 工作流树被选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvwFlow_SelectionChanged(object sender, TreeEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        /// <summary>
        /// 右击工作流树事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvwFlow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Point position = e.GetPosition(TvwFlow);
            TreeNode node = TvwFlow.GetNodeAtPoint(position) as TreeNode;
            if (node != null)
            {
                Point MuFlowTreePos = TvwFlow.TransformToVisual(TvwFlow).Transform(position);
                node.IsSelected = true;
                TvwFlow.ClearSelected();
                TvwFlow.SetSelected(node);

                // 调整x,y 值 ，以防止菜单被遮盖住
                var x = MuFlowTreePos.X;
                var y = MuFlowTreePos.Y;
                var menuHeight = 250;
                var menuWidth = 170;
                if (x + menuWidth > 220)
                {
                    x = x - (x + menuWidth - 220);
                }
                if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
                {
                    y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
                }
                MuFlowTree.SetValue(Canvas.LeftProperty, x);
                MuFlowTree.SetValue(Canvas.TopProperty, y);
                MuFlowTree.Show();
                if (node.isRoot)
                {
                    MuFlowTree.Get("OpenFlow").IsEnabled = false;
                    //MuFlowTree.Get("NewFlow").IsEnabled = false;
                    MuFlowTree.Get("Delete").IsEnabled = false;
                    MuFlowTree.Get("Edit").IsEnabled = false;
                }
                else
                {
                    MuFlowTree.Get("OpenFlow").IsEnabled = true;
                    //  MuFlowTree.Get("NewFlow").IsEnabled = true;
                    MuFlowTree.Get("Delete").IsEnabled = true;
                    MuFlowTree.Get("Edit").IsEnabled = true;
                }
                if (node.IsSort)
                {
                    // 只有节点的兄弟数大于1时允许删除
                    if (node.ParentNode.Nodes.Count > 1)
                    {
                        MuFlowTree.Get("Delete").IsEnabled = true;
                    }
                    else
                    {
                        MuFlowTree.Get("Delete").IsEnabled = false;
                    }
                    MuFlowTree.Get("OpenFlow").IsEnabled = false;
                    MuFlowTree.Get("Edit").IsEnabled = true;
                }
                else
                {
                    MuFlowTree.Get("Edit").IsEnabled = false;
                    MuFlowTree.Get("OpenFlow").IsEnabled = true;
                }
            }
            e.Handled = true;
        }
        /// <summary>
        /// 双击工作流树事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvwFlow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_doubleClickTimer.IsEnabled)
            {
                _doubleClickTimer.Stop();
                // 如果双击的是流程类型结点就返回，什么也不做.
                if (null == TvwFlow.Selected || ((TreeNode)TvwFlow.Selected).IsSort)
                    return;

                OpenFlow(TvwFlow.Selected.Name, TvwFlow.Selected.Title);
            }
            else
            {
                _doubleClickTimer.Start();
            }
        }

        #region CCForm_MouseLeftButtonDown

        /// <summary>
        /// 在工作流树空白处按下鼠标左键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CCFormTree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        /// <summary>
        /// 工作流树被选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CCForm_SelectionChanged(object sender, TreeEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        private void CCForm_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();

            if (_doubleClickTimer.IsEnabled)
            {
                _doubleClickTimer.Stop();

                if (null == this.FromTree.Selected
                    || ((TreeNode)FromTree.Selected).IsSort)
                    return;

                TreeNode tn = FromTree.Selected as TreeNode;
                this.CurrFK_FrmSort = tn.ID;

                Glo.WinOpenByDoType("CH", UrlFlag.FormFreeModel, tn.Name, null, null);

                //BP.Frm.Frm frm = new BP.Frm.Frm();
                //frm.BindFrm(tn.Name);
                //frm.HisMainPage = this;
                //frm.Show();
            }
            else
            {
                _doubleClickTimer.Start();
            }
        }
        private void CCForm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Point position = e.GetPosition(this.FromTree);
            TreeNode node = this.FromTree.GetNodeAtPoint(position) as TreeNode;
            if (node != null)
            {
                Point MuFlowTreePos = this.FromTree.TransformToVisual(this.FromTree).Transform(position);
                node.IsSelected = true;
                this.FromTree.ClearSelected();
                this.FromTree.SetSelected(node);

                // 调整x,y 值 ，以防止菜单被遮盖住
                var x = MuFlowTreePos.X;
                var y = MuFlowTreePos.Y;
                var menuHeight = 250;
                var menuWidth = 170;
                if (x + menuWidth > 220)
                {
                    x = x - (x + menuWidth - 220);
                }
                if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
                {
                    y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
                }
                this.MuFormTree.SetValue(Canvas.LeftProperty, x);
                this.MuFormTree.SetValue(Canvas.TopProperty, y);
                this.MuFormTree.Show();
                if (node.isRoot)
                {
                    this.MuFormTree.Get("Frm_EditForm").IsEnabled = false;
                    this.MuFormTree.Get("Frm_Delete").IsEnabled = false;
                    this.MuFormTree.Get("Frm_EditSort").IsEnabled = false;
                }
                else
                {
                    MuFormTree.Get("Frm_EditForm").IsEnabled = true;
                    MuFormTree.Get("Frm_Delete").IsEnabled = true;
                    MuFormTree.Get("Frm_EditForm").IsEnabled = true;
                }

                if (node.IsSort)
                {
                    // 只有节点的兄弟数大于1时允许删除
                    if (node.ParentNode.Nodes.Count > 1)
                    {
                        MuFormTree.Get("Frm_Delete").IsEnabled = true;
                    }
                    else
                    {
                        MuFormTree.Get("Frm_Delete").IsEnabled = false;
                    }
                    MuFormTree.Get("Frm_EditForm").IsEnabled = false;
                    //  MuFormTree.Get("Edit").IsEnabled = true;
                }
                else
                {
                    MuFormTree.Get("Frm_EditForm").IsEnabled = true;
                }
            }
            e.Handled = true;
        }
        #endregion

        /// <summary>
        /// 报表设计事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDesignerTable_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer != null)
            {
                SelectedContainer.btnDesignerTable();
            }
        }

        public void NewFlowHandler(int tabIdx)
        {
            this.CurrFK_FlowSort = TvwFlow.Selected.ID;
            var fu = new FrmNewFlow();
            fu.CurrentDesinger = this;
            fu.tabControl.TabIndex = tabIdx;

            if (TvwFlow.Selected != null && ((TreeNode)TvwFlow.Selected).IsSort)
            {
                fu.CurrentFlowSortName = TvwFlow.Selected.Title;
            }
            fu.FlowTempleteLoadCompeletedEventHandler += (eSender, eArgs) =>
            {
                try
                {
                    var result = eArgs.Result.Split(',');
                    // 返回值的格式为FlowSortID,FlowId,FlowName  
                    if (3 != result.Length)
                    {
                        MessageBox.Show(eArgs.Result, "错误1", MessageBoxButton.OK);
                        return;
                    }

                    this.BindFlowAndFlowSort();
                    OpenFlow(result[0], result[1], result[2]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误2", MessageBoxButton.OK);
                }
            };
            fu.Show();
        }

        /// <summary>
        /// 关闭选项卡事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedContainer != null)
            {
                if (SelectedContainer.IsNeedSave)
                {
                    if (HtmlPage.Window.Confirm("确定要保存吗？"))
                    {
                        if (SelectedContainer.Save(null))
                        {
                            this.tbDesigner.Items.Remove(this.tbDesigner.SelectedItem);
                        }
                    }
                    else
                    {
                        tbDesigner.Items.Remove(this.tbDesigner.SelectedItem);
                    }
                }
                else
                {
                    tbDesigner.Items.Remove(this.tbDesigner.SelectedItem);

                }
            }

            if (tbDesigner.Items.Count == 0)
            {
                setToolBarButtonEnableStatus(false);
            }
        }

        /// <summary>
        /// 执行新建工作流事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _service_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            loadingWin(false);
            if (e.Result.IndexOf(";") < 0)
            {
                MessageBox.Show(e.Result, "错误3", MessageBoxButton.OK);
                return;
            }
            string[] flow = e.Result.Split(';');
            //  FlowID = flow[0];
            _Service.DoCompleted -= _service_DoCompleted;
            this.BindFlowAndFlowSort();
            OpenFlow(flow[0], flow[1]);
        }
        /// <summary>
        /// Asp.net网页关闭时要执行的事件
        /// </summary>
        [ScriptableMember]
        public void OnAspNetPageClosed(string type, string para)
        {
            switch (type)
            {
                case "FlowNodeProperty":
                    var paras = para.Split(','); //id,namevalue

                    foreach (TabItem t in tbDesigner.Items)
                    {
                        Container ct = t.Content as Container;

                        foreach (var flowNode in ct.FlowNodeCollections)
                        {
                            if (paras[0] == flowNode.NodeID && paras[0] != flowNode.NodeName)
                            {
                                ct.IsNeedSave = true;
                                flowNode.NodeName = paras[1];
                                break;
                            }
                        }
                    }

                    break;

                case "FlowProperty":
                    paras = para.Split(','); //id,namevalue
                    var oldValue = string.Empty;
                    int index = 0;

                    foreach (TabItem t in tbDesigner.Items)
                    {
                        Container ct = t.Content as Container;
                        oldValue = (t as TabItemEx).Title;
                        if (paras[0] == ct.FlowID && paras[1] != oldValue)
                        {
                            var tabItem = tbDesigner.Items[index] as TabItemEx;
                            Button btnClose = new Button();
                            btnClose.Opacity = 0.2;
                            btnClose.Content = "╳";
                            btnClose.MinHeight = 15;
                            btnClose.MinWidth = 17;
                            btnClose.Click += new RoutedEventHandler(btnClose_Click);
                            btnClose.ClickMode = ClickMode.Release;
                            TextBlock tbx = new TextBlock();
                            tbx.Text = paras[1];
                            tbx.Width = paras[1].Length;
                            Canvas cs = new Canvas();
                            cs.Children.Add(btnClose);
                            cs.Children.Add(tbx);
                            cs.Height = 20;
                            tabItem.Title = paras[1];
                            tabItem.Width = tbx.ActualWidth + btnClose.ActualWidth + 30;
                            tabItem.Header = cs;
                            btnClose.DataContext = tabItem;
                            btnClose.SetValue(Canvas.LeftProperty, tabItem.Width - btnClose.ActualWidth - 20);
                            btnClose.SetValue(Canvas.TopProperty, 0.0);
                            tbx.SetValue(Canvas.TopProperty, btnClose.ActualHeight);
                            tbx.SetValue(Canvas.LeftProperty, 0.0);
                            tabItem.SetValue(TabControl.WidthProperty, tbx.ActualWidth + btnClose.ActualWidth + 40);

                            tabItem.VerticalAlignment = VerticalAlignment.Top;
                            tabItem.VerticalContentAlignment = VerticalAlignment.Top;
                            break;

                        }
                        index++;
                    }

                    this.BindFlowAndFlowSort();
                    break;

            }
        }

        private void Content_FullScreenChanged(object sender, EventArgs e)
        {
            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, SystemConst.DoubleClickTime);
            _doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);
            try
            {
                LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
                TbcFDS.Height = LayoutRoot.Height - 75;

                TvwFlow.Height = Application.Current.Host.Content.ActualHeight - 35 - 100;
                TvwFlowDataEnum.Height = TvwSysMenu.Height = TvwFlow.Height;

                tbDesigner.Height = Application.Current.Host.Content.ActualHeight - 35;
                tbDesigner.Width = Application.Current.Host.Content.ActualWidth - 227;
            }
            catch
            {
            }
        }
        private void Content_Resized(object sender, EventArgs e)
        {
            LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
            if (LayoutRoot.Height < 100)
            {
                return;
            }
            TbcFDS.Height = LayoutRoot.Height - 75;
            TvwFlow.Height = Application.Current.Host.Content.ActualHeight - 35 - 100;
            tbDesigner.Height = Application.Current.Host.Content.ActualHeight - 35;
            tbDesigner.Width = Application.Current.Host.Content.ActualWidth - 227;
        }

        private void TbcFDS_MouseLeave(object sender, MouseEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();

        }

        #region Toolbar related

        private void LoadToolbar()
        {
            var ens = new List<ToolbarItem>();
            ens = ToolbarItem.Instance.GetLists();
            foreach (ToolbarItem en in ens)
            {
                var btn = new ToolbarButton();
                btn.Name = "Btn_" + en.No;
                btn.IsEnabled = en.IsEnable;
                if (!en.IsEnable)
                {
                    btn.Tag = ToolBarEnableIsFlowSensitived;
                }
                btn.Click += new RoutedEventHandler(ToolBar_Click);

                var mysp = new StackPanel();
                mysp.Orientation = Orientation.Horizontal;
                mysp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                mysp.Name = "sp" + en.No;

                var img = new Image();
                var png = new BitmapImage(new Uri("/Images/" + en.No + ".png", UriKind.Relative));
                img.Source = png;
                mysp.Children.Add(img);

                var tb = new TextBlock();
                tb.Name = "tbT" + en.No;
                tb.Text = en.Name;
                tb.FontSize = 12;
                mysp.Children.Add(tb);

                btn.Content = mysp;
                this.toolbar1.AddBtn(btn);
                ToolBarButtonList.Add(btn);
            }
        }
        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as ToolbarButton;
            if (null == control)
                return;

            switch (control.Name)
            {
                case "Btn_ToolBarFrmLab":
                    Glo.WinOpenByDoType("CH", "FrmLib", "", "0", null);
                    //string  url = "/WF/Admin/XAP/DoPort.aspx?DoType=FrmLib&FK_Node=0&Lang=CH";
                    //Glo.WinOpen(Glo.BPMHost + url, "执行", 800, 760);
                    return;
                //BP.Frm.FrmLib lib = new BP.Frm.FrmLib();
                //lib.Show();
                case "Btn_ToolBarLogin":
                    Glo.WinOpen("/WF/Login.aspx", "登陆", 800, 900);
                    return;
                case "Btn_ToolBarNewNode": // 添加节点.
                    if (SelectedContainer != null)
                    {
                        SelectedContainer.AddFlowNode();
                        SelectedContainer.IsNeedSave = true;
                    }
                    break;
                case "Btn_ToolBarNewLine": // 添加线事件.
                    if (MessageBox.Show("创建连接线，您可按下一个节点的中间，然后拖动到另外一个节点中间后松开手就完成了。现在试一下？",
                        "您知道吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        SelectedContainer.IsReturnTypeDir = false;
                        return;
                    }

                    if (SelectedContainer != null)
                    {
                        SelectedContainer.AddDirection(false);
                        SelectedContainer.IsNeedSave = true;
                    }
                    break;
                case "Btn_ToolBarNewReturnLine": // 添加回退线事件.
                    if (MessageBox.Show("创建回退连接线，您可按下一个节点的中间，然后拖动到另外一个节点中间后松开手就完成了。现在试一下？",
                        "您知道吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        SelectedContainer.IsReturnTypeDir = true;
                        return;
                    }

                    if (SelectedContainer != null)
                    {
                        SelectedContainer.AddDirection(true);
                        SelectedContainer.IsNeedSave = true;
                    }
                    break;
                case "Btn_ToolBarNewLabel": // 添加标签。
                    if (null != SelectedContainer)
                    {
                        SelectedContainer.AddLabel(10, 10);
                        SelectedContainer.IsNeedSave = true;
                    }
                    break;
                case "Btn_ToolBarSave":
                    if (SelectedContainer != null)
                    {
                        SelectedContainer.Save(control);
                    }
                    break;
                case "Btn_ToolBarDesignReport":
                    btnDesignerTable_Click(sender, e);
                    break;
                case "Btn_ToolBarCheck":
                    if (SelectedContainer != null)
                        SelectedContainer.btnCheck();
                    break;
                case "Btn_ToolBarRun":
                    if (SelectedContainer != null)
                        SelectedContainer.btnRun();
                    break;
                case "Btn_ToolBarEditFlow":
                    Glo.WinOpenByDoType("CH", "FlowP", SelectedContainer.FlowID, null, null);
                    break;
                case "Btn_ToolBarDeleteFlow":
                    DeleteFlow(SelectedContainer.FlowID);
                    break;
                case "Btn_ToolBarHelp":
                    Glo.WinOpen("http://ccflow.org/Help.aspx", "Help", 1000, 1200);
                    break;
                case "Btn_ToolBarGenerateModel":
                    Glo.WinOpen("/WF/Admin/XAP/DoPort.aspx?DoType=ExpFlowTemplete&FK_Flow=" + SelectedContainer.FlowID + "&Lang=CH", "Help", 50, 50);
                    break;
                case "Btn_ToolBarShareModel":
                    {
                        if (SessionManager.Session.ContainsKey("user"))
                        {
                            ShowTemplateLib();
                        }
                        else
                        {
                            //FrmLoginTemLib loginTemLib = new FrmLoginTemLib();
                            //loginTemLib.Closed += (sender1, e1) =>
                            //{
                            //    if (loginTemLib.LoginSuccess)
                            //    {
                            //        ShowTemplateLib();
                            //    }
                            //};
                            //loginTemLib.Show();
                        }
                        break;
                    }
                case "Btn_ToolBarLoadModel":
                    NewFlowHandler(0);
                    break;
                case "Btn_ToolBarReleaseToFTP":
                    releaseToFtp();
                    break;
            }
        }


        /// <summary>
        /// 显示模板库
        /// </summary>
        private void ShowTemplateLib()
        {
            //frmShareFlow = new FrmShareFlow();
            //frmShareFlow.FlowID = SelectedContainer.FlowID;
            //frmShareFlow.fileView.FlowTempleteLoadCompeleted += (sender1, eArgs) =>
            //{
            //    try
            //    {
            //        var result = eArgs.Result.Split(',');
            //        // 返回值的格式为FlowSortID,FlowId,FlowName  
            //        if (3 != result.Length)
            //        {
            //            MessageBox.Show(eArgs.Result, "错误", MessageBoxButton.OK);
            //            return;
            //        }

            //        this.BindFlowAndFlowSort();
            //        OpenFlow(result[0], result[1], result[2]);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
            //    }
            //};
            //frmShareFlow.Show();
        }

        private void setToolBarButtonEnableStatus(bool isEnable)
        {
            foreach (var toolbarButton in ToolBarButtonList)
            {
                if (toolbarButton.Tag != null && toolbarButton.Tag.ToString() == ToolBarEnableIsFlowSensitived)
                {
                    toolbarButton.IsEnabled = isEnable;
                }
            }
        }
        #endregion

        /// <summary>
        /// 当前流程变化时，确保只有当前流程才显示关闭按钮，这样可以避免“当前流程是A，然后点击B的关闭按钮
        /// 表示用户想关闭B，但关闭的是A”的情况。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDesigner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem t in tbDesigner.Items)
            {
                if (t.IsSelected)
                {
                    var canvas = t.Header as Canvas;
                    if (canvas != null)
                    {
                        canvas.Children[0].Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    var canvas = t.Header as Canvas;
                    if (canvas != null)
                    {
                        canvas.Children[0].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        #region UserControl Related
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MuFlowTree.Hide();
            MuFormTree.Hide();
        }
        /// <summary>
        ///  diable the default silverlight rightmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HtmlPage.RegisterScriptableObject("Designer", this);
            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, SystemConst.DoubleClickTime);
            _doubleClickTimer.Tick += DoubleClick_Timer;
            try
            {
                Content_Resized(null, null);
            }
            catch
            {
            }
            Application.Current.Host.Content.FullScreenChanged += Content_FullScreenChanged;
        }
        /// <summary>
        /// 键盘按键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
        }
        /// <summary>
        /// 键盘按键释放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
        }
        #endregion

        #endregion

        #region 进度指示
        LoadingWindow cwLoading = new LoadingWindow();
        /// <summary>
        /// 显示Loadingp窗体
        /// </summary>
        private void loadingWin(bool showIt)
        {
            if (!showIt)
            {
                cwLoading.Close();
                return;
            }
            cwLoading.Show();
        }
        #endregion

    }
}