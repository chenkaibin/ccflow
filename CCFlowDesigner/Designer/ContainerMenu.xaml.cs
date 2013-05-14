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
using BP.Controls;
using WF.WS;
using BP;
using BP.Frm;

namespace BP
{
    public partial class ContainerMenu : UserControl
    {
        private FlowNode flowNode = null;

        public ContainerMenu()
        {
            InitializeComponent();
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MuContentMenu.Hide();

        }
         
        IContainer _container;
        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }
        public Point CenterPoint
        {
            get
            {
                return new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty));
            }
            set
            {
                // 调整x,y 值 ，以防止菜单被遮盖住
                var x = value.X;
                var y = value.Y;
                var menuHeight = 420;
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
        public void ShowMenu()
        {
            this.Visibility = Visibility.Visible;
            MuContentMenu.Show();
        }
        private void  AddLine_Click()
        {
            Direction r = new Direction(_container);
            r.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
            r.DirectionName = "Line" + _container.NextNewDirectionIndex.ToString();
            _container.AddDirection(r);
            r.SetDirectionPosition(new Point(CenterPoint.X - 20, CenterPoint.Y - 20), new Point(CenterPoint.X + 30, CenterPoint.Y + 30),null,null);
            _container.SaveChange(HistoryType.New);
            _container.IsNeedSave = true;
        }
        private void menuAddLabel_Click()
        {
            _container.AddLabel((int)CenterPoint.X - 20, (int)CenterPoint.Y - 20);
           
        }

        void addAcitivty(FlowNodeType type)
        {
            flowNode = new FlowNode(_container, type);
            flowNode.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
            flowNode.NodeName = "新建节点" + _container.NextNewFlowNodeIndex.ToString();
            flowNode.CenterPoint = this.CenterPoint;

            _container._Service.DoNewNodeAsync(_container.FlowID, 10, 10, flowNode.NodeName, true);
            _container._Service.DoNewNodeCompleted += _Service_DoNewNodeCompleted;

        }

        void _Service_DoNewNodeCompleted(object sender, DoNewNodeCompletedEventArgs e)
        {
            flowNode.NodeID = e.Result.ToString();
            _container.AddFlowNode(flowNode);
            _container.SaveChange(HistoryType.New);
            _container.IsNeedSave = true;

            _container._Service.DoNewNodeCompleted -= _Service_DoNewNodeCompleted;

        }

        private void AddFlowNodeSubMenu_Click(object sender, RoutedEventArgs e)
        {
            FlowNodeType type =(FlowNodeType) Enum.Parse(typeof(FlowNodeType), ((HyperlinkButton)sender).Name, true);
            addAcitivty(type);
        }
         
  
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            MuContentMenu.Hide();
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
                case "Help":
                    BP.Glo.WinOpen("http://ccflow.org/Help.aspx?wd=设计器", "帮助", 900, 1200);
                    break;
                case "menuExp":
                    BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                    exp.Show();
                    break;
                case "menuImp":
                    BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                    imp.Show();
                    break;
                case "menuFullScreen":
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case "menuAddNode":
                    addAcitivty(FlowNodeType.Ordinary);
                    break;
                case "menuAddLine":
                    AddLine_Click();
                    break;
                case "menuAddLabel":
                    menuAddLabel_Click();
                    break;
                case "menuFlowPropertity":
                    BP.Glo.WinOpenByDoType("CH", BP.UrlFlag.FlowP, _container.FlowID, null, null);
                    //_container.SetProper("", "FlowP", _container.FlowID, "0", "0", "流程属性");
                    //_container.IsContainerRefresh = true;
                    break;
                case "menuRunFlow":
                    //_container.IsContainerRefresh = false;
                    //_container.SetProper("", "RunFlow", _container.FlowID, "0", "0", "运行");
                    BP.Glo.WinOpenByDoType("CH", BP.UrlFlag.RunFlow, _container.FlowID, null, null);
                    break;
                case "menuCheckFlow":
                    //_container.IsContainerRefresh = false;
                    //_container.SetProper("", "FlowCheck", _container.FlowID, "0", "0", "检查");
                    BP.Glo.WinOpenByDoType("CH", BP.UrlFlag.FlowCheck, _container.FlowID, null, null);
                    break;
                case "menuFlowDefination":
                    //_container.IsContainerRefresh = false;
                    //_container.SetProper("", "WFRpt", _container.FlowID, "0", "0", "流程报表定义");

                    BP.Glo.WinOpenByDoType("CH", BP.UrlFlag.WFRpt, _container.FlowID, null, null);
                    break;
                case "menuDelete": // 删除流程。
                    if (System.Windows.Browser.HtmlPage.Window.Confirm("您确定要删除吗？"))
                    {
                        _container.DeleteSeletedControl();
                        _container.SaveChange(HistoryType.New);
                        _container.IsNeedSave = true;
                    }
                    break;
                case "MenuDisplayHideGrid":
                    if (_container.GridLinesContainer.Children.Count > 0)
                    {
                        _container.GridLinesContainer.Children.Clear();
                    }
                    else
                    {
                        _container.SetGridLines();
                    }
                    break;
            }
            MuContentMenu.Hide();
        }
    }
}
