using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using WF.WS;
using BP;

namespace BP
{
    public interface IContainer
    {
        #region Properties

        string FlowID { get; set; }
        string FK_Flow { get; }
        string WorkID { get; }

        bool CtrlKeyIsPress { get; }
        double ContainerWidth { get; set; }
        double ContainerHeight { get; set; }
        double ScrollViewerHorizontalOffset { get; set; }
        double ScrollViewerVerticalOffset { get; set; }

        int NextNewFlowNodeIndex { get; }
        int NextNewDirectionIndex { get; }
        int NextNewLabelIndex { get; }
        int NextMaxIndex { get; }

        bool IsNeedSave { get; set; }
        bool IsSomeChildEditing { get; set; }
        bool IsContainerRefresh { get; set; }
        bool MouseIsInContainer { get; set; }
        bool IsMouseSelecting { get; }
        bool IsReturnTypeDir { get; set; }
       
        List<FlowNode> FlowNodeCollections { get; }
        List<Direction> DirectionCollections { get; }
        List<NodeLabel> LableCollections { get; }
        Direction CurrentTemporaryDirection { get; set; }
        List<System.Windows.Controls.Control> CurrentSelectedControlCollection { get; }
        PageEditType EditType { get; set; }
        //List<System.Windows.Controls.Control> CopyElementCollectionInMemory { get; set; }
        //Stack<string> WorkFlowXmlPreStack { get; }
        //Stack<string> WorkFlowXmlNextStack { get; }

        WSDesignerSoapClient _Service { get; set; }
        Canvas GridLinesContainer { get; }
        #endregion

        #region Methods
        void AddFlowNode(FlowNode a);
        void RemoveFlowNode(FlowNode a);
        void RemoveLabel(NodeLabel l);
        void AddDirection(Direction r);
        void AddLabel(NodeLabel l);
        void AddLabel(int x, int y);
        void RemoveDirection(Direction r);
        void SelectDirection(Direction r, bool isSelected);
        void SelectFlowNode(FlowNode a, bool isSelected);

        string ToXmlString();
        void LoadFromXmlString(string xmlString);

        void ShowMessage(string message);
        void ShowFlowNodeSetting(FlowNode ac);
        void ShowDirectionSetting(Direction rc);
        void ShowFlowNodeContentMenu(FlowNode a, object sender, MouseButtonEventArgs e);
        void ShowLabelContentMenu(NodeLabel l, object sender, MouseButtonEventArgs e);
        void ShowDirectionContentMenu(Direction r, object sender, MouseButtonEventArgs e);

        void AddSelectedControl(System.Windows.Controls.Control uc);
        void RemoveSelectedControl(System.Windows.Controls.Control uc);
        void SetWorkFlowElementSelected(System.Windows.Controls.Control uc, bool isSelect);
        void MoveControlCollectionByDisplacement(double x, double y, UserControl uc);
        void ClearSelectFlowElement(System.Windows.Controls.Control uc);
        void CopySelectedControlToMemory(System.Windows.Controls.Control currentControl);
        void UpdateSelectedControlToMemory(System.Windows.Controls.Control currentControl);
        void UpDateSelectedNode(System.Windows.Controls.Control currentControl);
        void DeleteSeletedControl();
        void PastMemoryToContainer();
        void PreviousAction();
        void NextAction();

        bool Contains(UIElement uiel);
        CheckResult CheckSave();
        void SaveChange(HistoryType action);
        void SetProper(string lang, string dotype, string fk_flow, string node1, string node2, string title);
        void SetGridLines(); 
        #endregion
    }
}