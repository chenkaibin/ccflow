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
using Liquid;
using System.Windows.Browser;
using MenuItem = Liquid.MenuItem;
using BP;

namespace BP
{
    public partial class FlowNodeMenu : UserControl
    {
        private IContainer _container;
        public FlowNodeMenu()
        {
            InitializeComponent();
        }

        public IContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        private FlowNode relatedFlowNode;

        public FlowNode RelatedFlowNode
        {
            get { return relatedFlowNode; }
            set { relatedFlowNode = value; }
        }


        public void ApplyCulture()
        {
        }

        public Point CenterPoint
        {
            get { return new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty)); }
            set
            {
                // 调整x,y 值 ，以防止菜单被遮盖住.
                var x = value.X;
                var y = value.Y;
                var menuHeight = 250;
                var menuWidth = 170;
                var hostWidth = Application.Current.Host.Content.ActualWidth - 250;
                var hostHeight = Application.Current.Host.Content.ActualHeight;
                if (x + menuWidth > hostWidth)
                {
                    x = x - (x + menuWidth - hostWidth);
                }
                if (y + menuHeight > hostHeight)
                {
                    y = y - (y + menuHeight - hostHeight);
                }
                this.SetValue(Canvas.TopProperty, y);
                this.SetValue(Canvas.LeftProperty, x);
            }
        }


        private bool isMultiControlSelect = false;

        public void ShowMenu()
        {
            isMultiControlSelect = false;

            if (_container.CurrentSelectedControlCollection != null
                && _container.CurrentSelectedControlCollection.Count > 0
                )
            {
                if (!_container.CurrentSelectedControlCollection.Contains(relatedFlowNode))
                {
                    _container.ClearSelectFlowElement(null);
                    isMultiControlSelect = false;
                }
                else
                {
                    isMultiControlSelect = true;
                }
            }
            else
            {
                isMultiControlSelect = false;
            }

            this.Visibility = Visibility.Visible;
            MuContentMenu.Show();

            try
            {
                if (NodePosType.Start == RelatedFlowNode.HisPosType)
                {
                    MuContentMenu.SetEnabledStatus("menuDeleteNode", false);
                }
                else
                {
                    MuContentMenu.SetEnabledStatus("menuDeleteNode", true);
                }
                setMenuItemStyleByType(RelatedFlowNode.HisRunModel);
            }
            catch
            {

            }
        }
        private void setMenuItemStyleByType(FlowNodeType type)
        {
            var menuType = (MuContentMenu.Items[8] as MenuItem).Content as Menu;

            if (null == menuType)
            {
                return;
            }
            foreach (var item in menuType.Items)
            {
                var subMenu = item as MenuItem;
                if (null != subMenu)
                {
                    subMenu.FontWeight = FontWeights.Normal;
                    subMenu.CheckBoxVisibility = System.Windows.Visibility.Collapsed;
                    subMenu.IsChecked = false;
                }
            }

            MenuItem mItem = null;

            switch (type)
            {
                case FlowNodeType.Ordinary:
                    mItem = (menuType.Items[0] as MenuItem);
                    break;
                case FlowNodeType.FL:
                    mItem = (menuType.Items[1] as MenuItem);
                    break;
                case FlowNodeType.HL:
                    mItem = (menuType.Items[2] as MenuItem);
                    break;
                case FlowNodeType.FHL:
                    mItem = (menuType.Items[3] as MenuItem);
                    break;
                case FlowNodeType.SubThread:
                    mItem = (menuType.Items[4] as MenuItem);
                    break;
            }

            mItem.FontWeight = FontWeights.ExtraBold;
            mItem.CheckBoxVisibility = System.Windows.Visibility.Visible;
            mItem.IsChecked = true;
        }

        private void deleteFlowNode()
        {
            if (relatedFlowNode != null)
            {
                if (HtmlPage.Window.Confirm("您确认要删除吗？"))
                {
                    this.Visibility = Visibility.Collapsed;
                    IElement iel;
                    foreach (System.Windows.Controls.Control c in _container.CurrentSelectedControlCollection)
                    {
                        iel = c as IElement;
                        if (iel != null)
                        {
                            iel.Delete();
                        }
                    }
                    relatedFlowNode.Delete();
                    _container.SaveChange(HistoryType.New);
                    _container.IsNeedSave = true;
                }
            }
        }
        private void showFlowNodeSetting()
        {
            this.Visibility = Visibility.Collapsed;
            _container.ShowFlowNodeSetting(relatedFlowNode);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private void Menu_ItemSelected(object sender, MenuEventArgs e)
        {
            if (e.Tag == null)
                return;
            switch (e.Tag.ToString())
            {
                case "MenuDisplayHideGrid": //显示/隐藏网格
                    if (_container.GridLinesContainer.Children.Count > 0)
                        _container.GridLinesContainer.Children.Clear();
                    else
                        _container.SetGridLines();
                    break;
                case "menuExp": // 导出/共享:流程模板
                    BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                    exp.Show();
                    break;
                case "menuImp": //  导入/查找:流程模板
                    BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                    imp.Show();
                    break;
                case "Help":
                    Glo.WinOpen("http://ccflow.org/Help.aspx?wd=设计器", "帮助", 900, 1200);
                    break;
                case "menuModifyName":
                    showFlowNodeSetting();
                    break;
                case "menuDeleteNode":
                    deleteFlowNode();
                    break;
                case "menuDesignNodeFixModel":
                    Glo.WinOpenByDoType("CH", "MapDefFixModel", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuDesignNodeFreeModel":
                    Glo.WinOpenByDoType("CH", "MapDefFreeModel", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuDesignFlowFrm": // 表单库
                    Glo.WinOpenByDoType("CH", "FrmLib", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuDesignBindFlowFrm": //流程表单
                    Glo.WinOpenByDoType("CH", "FlowFrms", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuJobStation": // 节点工作岗位。
                    Glo.WinOpenByDoType("CH", "StaDef", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuNodeProperty":
                    Glo.WinOpenByDoType("CH", "NodeP", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuFlowProperty":
                    Glo.WinOpenByDoType("CH", "FlowP", _container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuNodeTypePT":
                    RelatedFlowNode.HisRunModel = FlowNodeType.Ordinary;
                    break;
                case "menuNodeTypeFL":
                    RelatedFlowNode.HisRunModel = FlowNodeType.FL;
                    break;
                case "menuNodeTypeHL":
                    RelatedFlowNode.HisRunModel = FlowNodeType.HL;
                    break;
                case "menuNodeTypeFHL":
                    RelatedFlowNode.HisRunModel = FlowNodeType.FHL;
                    break;
                case "menuNodeTypeZLC":
                    RelatedFlowNode.HisRunModel = FlowNodeType.SubThread;
                    break;
            }
            MuContentMenu.Hide();
        }
    }
}
