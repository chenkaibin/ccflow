using System;
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
using Silverlight;

namespace WorkNode
{
    public class BPDtl : System.Windows.Controls.HyperlinkButton
    {
        #region 处理选中.
        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                if (value == true)
                {
                    Thickness d = new Thickness(0.5);
                    this.BorderThickness = d;
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    Thickness d1 = new Thickness(0.5);
                    this.BorderThickness = d1;
                    this.BorderBrush = new SolidColorBrush(Colors.Black);
                }
            }
        }
        public void SetUnSelectedState()
        {
            if (this.IsSelected)
                this.IsSelected = false;
            else
                this.IsSelected = true;
        }
        #endregion 处理选中.

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        public BPDtl()
        {
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 400;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);
        }
        /// <summary>
        /// 初试化这个dtl表
        /// </summary>
        /// <param name="dtlTableID">要初试化的从表ID</param>
        /// <param name="ds">数据源</param>
        public BPDtl(string dtlTableID, DataSet ds)
        {
            this.Name = dtlTableID;
            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            this.Width = 400;
            this.Height = 200;
            this.BorderThickness = new Thickness(5);

            //取到dtl集合.
            DataTable dtDtl = ds.Tables["Sys_MapDtl"];
            foreach (DataRow drDtl in dtDtl.Rows)
            {
                if (dtlTableID != drDtl["No"].ToString())
                    continue;

                DataGrid dg = new DataGrid();
                dg.Name = "DG" + this.Name;
                DataGridTextColumn mycol = new DataGridTextColumn();
                mycol.Header = "IDX";
                dg.Columns.Add(mycol);

                //获得字段属性表.
                DataTable dtMapAttrs = ds.Tables["Sys_MapAttr"];
                foreach (DataRow dr in dtMapAttrs.Rows)
                {
                    // 因为这个表里也存放了主表，或者其它明细表的 字段类控件属性, 所以要过滤一下.
                    if (dr["FK_MapData"].ToString() != dtlTableID)
                        continue;

                    if (dr["UIVisible"].ToString() == "0")
                        continue;

                    // 在以下代码里实现
                    DataGridTextColumn txtColumn = new DataGridTextColumn();
                    txtColumn.Header = dr["Name"];
                    dg.Columns.Add(txtColumn);
                }
                dg.Width = this.Width;
                dg.Height = this.Height;
                this.Content = dg;
                this.MyDG = dg;

                //获得明细表的数据源, 把数据呈现给用户。
                DataTable dtDtlData = ds.Tables[dtlTableID];

            }
        }
        public DataGrid MyDG = null;
        public void UpdatePos()
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                MyDG.Width = this.Width;
                MyDG.Height = this.Height;
            }
        }
    }
}
