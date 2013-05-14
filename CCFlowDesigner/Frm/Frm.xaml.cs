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
using BP.DA;
using Silverlight;

namespace BP.Frm
{
    public partial class Frm : ChildWindow
    {
        public MainPage HisMainPage = null;
        public AppType HisAppType = AppType.Application;
        public bool IsNew = false;
        public Frm()
        {
            InitializeComponent();
            this.MouseRightButtonDown += (sender, e) =>
            {
                e.Handled = true;
            };
        }
        public void BindFrm(string fk_mapdata)
        {
            this.IsNew = false;
            string sqls = "SELECT * FROM Sys_FrmSort ";
            sqls += "@SELECT * FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
            this.TB_No.IsEnabled = false;
            WSDesignerSoapClient daBindFrm = Glo.GetDesignerServiceInstance();
            daBindFrm.RunSQLReturnTableSAsync(sqls);
            daBindFrm.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(daBindFrm_RunSQLReturnTableSCompleted);
        }
        void daBindFrm_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            this.Btn_Del.IsEnabled = false;
            this.Btn_Save.IsEnabled = false;

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtSort = ds.Tables[0];
            if (dtSort.Rows.Count == 0)
            {
                DataRow dr = dtSort.NewRow();
                dr[0] = "01";
                dr[1] = "默认类别";
                dtSort.Rows.Add(dr);
            }
            if (this.IsNew)
            {
                Glo.Ctrl_DDL_BindDataTable(this.DDL_FrmSort, dtSort, "01");
                this.HisAppType = AppType.Application;
                this.Btn_Del.IsEnabled = true;
                this.Btn_Save.IsEnabled = true;
                return;
            }

            DataTable dtMapdata = ds.Tables[1];
            if (dtMapdata.Rows.Count == 0)
            {
                MessageBox.Show("数据已被删除，请刷新列表。");
                this.DialogResult = false;
                return;
            }

            this.TB_No.Text = dtMapdata.Rows[0]["No"];
            this.TB_Name.Text = dtMapdata.Rows[0]["Name"];
            this.TB_PTable.Text = dtMapdata.Rows[0]["PTable"];
            string tag = dtMapdata.Rows[0]["Tag"];
            if (tag != null)
                this.TB_URL.Text = tag;

            this.TB_Designer.Text = dtMapdata.Rows[0]["Designer"];
            this.TB_DesignerUnit.Text = dtMapdata.Rows[0]["DesignerUnit"];
            this.TB_DesignerContact.Text = dtMapdata.Rows[0]["DesignerContact"];

            // 应用类型。
            this.HisAppType = (AppType)int.Parse(dtMapdata.Rows[0]["AppType"]);

            Glo.Ctrl_DDL_SetSelectVal(this.DDL_DBUrl, dtMapdata.Rows[0]["DBURL"]);
            Glo.Ctrl_DDL_SetSelectVal(this.DDL_FrmType, dtMapdata.Rows[0]["FrmType"]);
            Glo.Ctrl_DDL_BindDataTable(this.DDL_FrmSort, dtSort, dtMapdata.Rows[0]["FK_FrmSort"]);

            if (this.HisAppType == AppType.Application)
            {
                this.Btn_Del.IsEnabled = true;
                this.Btn_Save.IsEnabled = true;
            }
        }
        public void BindNew()
        {
            this.Title = "新建表单";
            this.IsNew = true;
            this.TB_No.IsEnabled = true;
            string sqls = "SELECT No,Name FROM Sys_FrmSort";
            WSDesignerSoapClient daNew = Glo.GetDesignerServiceInstance();
            daNew.RunSQLReturnTableSAsync(sqls);
            daNew.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(daNew_RunSQLReturnTableSCompleted);
            this.HisAppType = AppType.Application;
        }
        void daNew_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtSort = ds.Tables[0];
            Glo.Ctrl_DDL_BindDataTable(this.DDL_FrmSort, dtSort,"01");
        }
        
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("您确定要删除吗？", "ccflow提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;
            WSDesignerSoapClient delMap = Glo.GetDesignerServiceInstance();
            delMap.DoTypeAsync("DelFrm", this.TB_No.Text,null,null,null,null);
            delMap.DoTypeCompleted += new EventHandler<DoTypeCompletedEventArgs>(delMap_DoTypeCompleted);
        }
        void delMap_DoTypeCompleted(object sender, DoTypeCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                this.DialogResult = true;
                this.HisMainPage.BindFormTree();
                return;
            }
            MessageBox.Show(e.Result, "错误", MessageBoxButton.OK);
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Btn_Save.IsEnabled = false;
            if (this.IsNew)
            {
                WSDesignerSoapClient daCheckID = Glo.GetDesignerServiceInstance();
                daCheckID.RunSQLReturnValIntAsync("SELECT count(*) FROM Sys_MapData WHERE No='" + this.TB_No.Text.Trim() + "'");
                daCheckID.RunSQLReturnValIntCompleted += new EventHandler<RunSQLReturnValIntCompletedEventArgs>(daCheckID_RunSQLReturnValIntCompleted);
            }
            else
            {
                this.SaveEn();
            }
        }
        void daCheckID_RunSQLReturnValIntCompleted(object sender, RunSQLReturnValIntCompletedEventArgs e)
        {
            if (e.Result > 0)
            {
                this.Btn_Save.IsEnabled = true;
                MessageBox.Show("已经存在编号为(" + this.TB_No.Text + ")的表单", "保存失败", MessageBoxButton.OK);
                return;
            }
            else
            {
                this.SaveEn();
            }
        }
        public void SaveEn()
        {
            string error = "";
            if (string.IsNullOrEmpty(this.TB_No.Text.Trim()))
                error += "编号不能为空.";

            if (string.IsNullOrEmpty(this.TB_Name.Text.Trim()))
                error += "名称不能为空.";

            string strs = "";
            strs += "@EnName=BP.Sys.MapData@PKVal=" + this.TB_No.Text;
            strs += "@No=" + this.TB_No.Text;
            strs += "@Name=" + this.TB_Name.Text;
            strs += "@PTable=" + this.TB_PTable.Text;
            strs += "@Tag=" + this.TB_URL.Text;

            ListBoxItem lb = this.DDL_FrmSort.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@FK_FrmSort=" + lb.Tag.ToString();

            lb = this.DDL_FrmType.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@FrmType=" + lb.Tag.ToString();

            lb = this.DDL_DBUrl.SelectedItem as ListBoxItem;
            if (lb != null)
                strs += "@DBURL=" + lb.Tag.ToString();

            strs += "@AppType=" + (int)this.HisAppType;
            strs += "@Designer=" + this.TB_Designer.Text;
            strs += "@DesignerContact=" + this.TB_DesignerContact.Text;
            strs += "@DesignerUnit=" + this.TB_DesignerUnit.Text;

            WSDesignerSoapClient daSaveEn = Glo.GetDesignerServiceInstance();
            daSaveEn.SaveEnAsync(strs);
            daSaveEn.SaveEnCompleted += new EventHandler<SaveEnCompletedEventArgs>(daSaveEn_SaveEnCompleted);
        }
        void daSaveEn_SaveEnCompleted(object sender, SaveEnCompletedEventArgs e)
        {
            this.Btn_Save.IsEnabled = true;
            if (e.Result.Contains("Err"))
            {
                MessageBox.Show(e.Result, "error", MessageBoxButton.OK);
            }
            else
            {
                this.HisMainPage.BindFormTree();
                this.DialogResult = true;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.HisMainPage.BindFormTree();
            this.DialogResult = false;
        }
        private void DDL_FrmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetState();
        }
        public void SetState()
        {
            try
            {
                this.TB_PTable.IsEnabled = true;
                this.DDL_DBUrl.IsEnabled = true;
                this.TB_URL.IsEnabled = true;
                if (this.DDL_FrmType.SelectedIndex == 2)
                {
                    /*自定义表单*/
                    this.TB_PTable.IsEnabled = false;
                    this.DDL_DBUrl.IsEnabled = false;
                    this.TB_URL.IsEnabled = true;
                }
                else
                {
                    this.TB_PTable.IsEnabled = true;
                    this.DDL_DBUrl.IsEnabled = true;
                    this.TB_URL.IsEnabled = false;
                }
            }
            catch
            {
            }
        }
        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            string s = this.TB_Name.Text;
            var daPinYin = Glo.GetDesignerServiceInstance();
            daPinYin.ParseStringToPinyinAsync(s);
            daPinYin.ParseStringToPinyinCompleted += new EventHandler<ParseStringToPinyinCompletedEventArgs>(daPinYin_ParseStringToPinyinCompleted);
        }

        void daPinYin_ParseStringToPinyinCompleted(object sender, ParseStringToPinyinCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_No.Text.Trim()) == true)
            {
                this.TB_No.Text = e.Result;
            }

            if (string.IsNullOrEmpty(this.TB_PTable.Text.Trim()) == true)
            {
                this.TB_PTable.Text = e.Result;
            }
        }
    }
}

