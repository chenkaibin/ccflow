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
using System.Windows.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
using System.Windows.Media.Imaging;
using Silverlight;
using BP.En;
using BP.Sys;
namespace CCForm
{
    public partial class MainPage : UserControl
    {
        #region 处理移动的变量
        private Point origPoint;  /* 鼠标点击的位置   */
        private Rectangle rect;   /*  选择的区域  */
        private Image CustomCursorImage; /* 选择的矩形区域 */

        private bool isMultipleSelected;
        private List<FrameworkElement> selectedElements = new List<FrameworkElement>();
        private bool isMouseCaptured;
        #endregion

        #region zhoupeng add 全局变量
        LoadingWindow loadingWindow = new LoadingWindow();
        public SelectM2M winSelectM2M = new SelectM2M();
        public FlowFrm winFlowFrm = new FlowFrm();
        public FrmLink winFrmLink = new FrmLink();
        public FrmLab winFrmLab = new FrmLab();
        public SelectTB winSelectTB = new SelectTB();
        public SelectDDLTable winSelectDDL = new SelectDDLTable();
        public SelectRB winSelectRB = new SelectRB();
        public FrmImp winFrmImp = new FrmImp();
        public FrmBtn winFrmBtn = new FrmBtn();

        public FrmOp winFrmOp = new FrmOp();
        public FrmImg winFrmImg = new FrmImg();
        public FrmEle winFrmEle = new FrmEle();

        public NodeFrms winNodeFrms = new NodeFrms();
        public SelectAttachment winSelectAttachment = new SelectAttachment();
        public FrmAttachmentM winFrmAttachmentM = new FrmAttachmentM();

        public bool IsRB = false;
        #endregion 全局变量

        #region 全局变量
        bool be = false;//在绿点上判断当前鼠标的状态是否按下.
        bool bl = false;//判断LABEL当前鼠标的状态是否按下
        bool btxt = false;
        string selectType = ToolBox.Mouse; // 当前工具选择类型 hand line1 line2 label txt cannel

        BPM2M currM2M;   //当前currM2M
        BPImgAth currImgAth; //当前 currDtl
        BPDtl currDtl; //当前 currDtl
        BPCheckBox currCB; //当前 label
        BPRadioBtn currRB; //当前 label
        Label currCheckBoxLabel; //当前 label
        BPLabel currLab; //当前 label
        BPBtn currBtn; //当前 BPBtn
        BPLink currLink;  //当前 linke
        BPLine currLine;  //当前 Line
        BPImg currImg;   //当前Img
        BPEle currEle;   //当前Ele
        BPAttachment currAth;   //当前Ath
        BPAttachmentM currAthM;   //当前currAthM
        BPTextBox currTB;    //当前textbox
        BPDatePicker currDP;    //当前textbox
        BPDDL currDDL;   //当前标签
        Ellipse e1;//选中线后出现的绿点1
        Ellipse e2;//选中线后出现的绿点2
        Ellipse eCurrent;//选中的绿点
        Grid g = new Grid();//遮罩图层
        private DateTime _lastTime;//获取双击label的时间间隔
        #endregion

        public void SetGridLines(bool isFresh)
        {
            #region 判断是否存在.
            int mynum = this.canvasMain.Children.Count;
            string ids = "";
            for (int i = 0; i < mynum; i++)
            {
                Line mylin = this.canvasMain.Children[i] as Line;
                if (mylin == null)
                    continue;

                if (mylin.Name == null)
                    continue;

                if (mylin.Name.Contains("GLine"))
                {
                    ids += "@" + mylin.Name;
                }
            }

            if (ids != "")
            {
                string[] myids = ids.Split('@');
                foreach (string id in myids)
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    Line mylin = this.canvasMain.FindName(id) as Line;
                    if (mylin == null)
                        continue;
                    this.canvasMain.Children.Remove(mylin);
                }
                if (!isFresh)
                    return;
            }
            #endregion 判断是否存在.

            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(255, 160, 160, 160);
            //  brush.Color = Color.FromArgb(255, 255, 255, 255);
            double thickness = 0.3;
            double top = 0;
            double left = 0;
            double width = canvasMain.Width;
            double height = canvasMain.Height;
            double stepLength = 40;
            double x, y;
            x = left + stepLength;
            y = top;

            while (x < width + left)
            {
                Line line = new Line();
                line.Name = "GLine" + x + "_" + y;
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;

                line.Stroke = brush;
                line.StrokeThickness = thickness;
                line.Stretch = Stretch.Fill;
                canvasMain.Children.Add(line);
                x += stepLength;
            }

            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.Name = "GLine" + x + "_" + y;
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;

                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                line.StrokeThickness = thickness;
                canvasMain.Children.Add(line);
                y += stepLength;
            }
        }
        void WindowDilag_Closed(object sender, EventArgs e)
        {
            this.SetSelectedTool(ToolBox.Mouse);
            ChildWindow c = sender as ChildWindow;
            if (c.DialogResult == false)
            {
                if (c.Name == "winFrmLab")
                    this.currLab.trackingMouseMove = false;

                if (c.Name == "winFrmLink")
                    this.currLink.trackingMouseMove = false;
                return;
            }

            if (c.Name == "winFrmImg")
            {
                //this.currLab.Content = this.winFrmLab.TB_Text.Text.Replace("@", "\n");
                //int size = this.winFrmLab.DDL_FrontSize.SelectedIndex + 6;
                // this.currImg.FontSize = ""; // double.Parse(size.ToString());
                return;
            }

            if (c.Name == "winSelectM2M")
            {
                BPM2M m2m = new BPM2M();
                m2m.Name = Glo.TempVal.ToString();
                m2m.Cursor = Cursors.Hand;
                m2m.SetValue(Canvas.LeftProperty, Glo.X);
                m2m.SetValue(Canvas.TopProperty, Glo.Y);
                if (this.canvasMain.FindName(m2m.Name) != null)
                {
                    MessageBox.Show("已经存在对象：" + m2m.Name);
                    this.SetSelectedTool(ToolBox.Mouse);
                    return;
                }

                //MouseDragElementBehavior mymdeE = new MouseDragElementBehavior();
                //Interaction.GetBehaviors(m2m).Add(mymdeE);
                m2m.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                m2m.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                this.SetSelectedTool(ToolBox.Mouse);

                this.canvasMain.Children.Add(m2m);
                Glo.currEle = m2m;
                this.currM2M = m2m;
                string url = "";
                url = Glo.BPMHost + "/WF/MapDef/MapM2M.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&NoOfObj=" + m2m.Name + Glo.TimeKey;
                HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:600px;dialogWidth:650px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                return;
            }

            if (c.Name == "winFrmLab")
            {
                this.currLab.Content = this.winFrmLab.TB_Text.Text.Replace("@", "\n");
                int size = this.winFrmLab.DDL_FrontSize.SelectedIndex + 6;
                this.currLab.FontSize = double.Parse(size.ToString());
                this.currLab.trackingMouseMove = false;
                return;
            }

            if (c.Name == "winFrmLink")
            {
                // this.winFrmLink.BindIt(this.currLink);
                this.currLink.Content = this.winFrmLink.TB_Text.Text.Replace("@", "\n");
                this.currLink.WinTarget = this.winFrmLink.TB_WinName.Text;
                this.currLink.URL = this.winFrmLink.TB_URL.Text;
                int size = this.winFrmLink.DDL_FrontSize.SelectedIndex + 6;
                this.currLink.FontSize = double.Parse(size.ToString());
                this.currLink.trackingMouseMove = false;
                return;
            }

            if (c.Name == "winFlowFrm")
            {
                Glo.FK_MapData = this.winFlowFrm.TB_No.Text;
                this.BindTreeView();
                return;
            }

            if (c.Name == "winNodeFrms")
            {
                this.BindTreeView();
                return;
            }

            if (c.Name == "winFrmImp")
            {
                this.BindFrm();
                return;
            }

            if (c.Name == "winFrmOP")
            {
                this.canvasMain.Width = double.Parse(this.winFrmOp.TB_FrmW.Text);
                this.canvasMain.Height = double.Parse(this.winFrmOp.TB_FrmH.Text);
                Glo.HisMapData.FrmH = this.canvasMain.Height;
                Glo.HisMapData.FrmW = this.canvasMain.Width;
                this.scrollViewer1.Width = Glo.HisMapData.FrmW;
                //重新画线
                this.SetGridLines(true);
                return;
            }


            if (c.Name == "winSelectTB")
            {
                #region 增加审核分组.
                if (this.winSelectTB.IsCheckGroup == true)
                {
                    if (Glo.X > 300)
                        Glo.X = 300;

                    /*如果是增加的审核分组.*/
                    string gName = this.winSelectTB.TB_Name.Text;
                    string gKey = this.winSelectTB.TB_KeyOfEn.Text;
                    this.SetSelectedTool(ToolBox.Mouse);
                    FF.CCFormSoapClient daCreateCheckGroup = Glo.GetCCFormSoapClientServiceInstance();
                    daCreateCheckGroup.DoTypeAsync("CreateCheckGroup", gKey, gName, Glo.FK_MapData, null, null, null);
                    daCreateCheckGroup.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(daCreateCheckGroup_DoTypeCompleted);
                    return;
                }
                #endregion

                TBType tp = TBType.String;
                if (winSelectTB.RB_String.IsChecked == true)
                    tp = TBType.String;

                if (winSelectTB.RB_Money.IsChecked == true)
                    tp = TBType.Money;
                if (winSelectTB.RB_Int.IsChecked == true)
                    tp = TBType.Int;
                if (winSelectTB.RB_Float.IsChecked == true)
                    tp = TBType.Float;

                if (winSelectTB.RB_DataTime.IsChecked == true)
                    tp = TBType.DateTime;

                if (winSelectTB.RB_Data.IsChecked == true)
                    tp = TBType.Date;

                if (winSelectTB.RB_Boolen.IsChecked == true)
                {
                    /* 如果是boolen 类型. */
                    BPCheckBox cb = new BPCheckBox();
                    cb.Name = this.winSelectTB.TB_KeyOfEn.Text;
                    cb.Content = this.winSelectTB.TB_Name.Text;
                    cb.KeyName = cb.Content.ToString();

                    Label cbLab = new Label();
                    cbLab.Name = "CBLab" + cb.Name;
                    cbLab.Content = this.winSelectTB.TB_Name.Text;
                    cbLab.Tag = cb.Name;
                    cb.Content = cbLab;
                    cb.SetValue(Canvas.LeftProperty, Glo.X);
                    cb.SetValue(Canvas.TopProperty, Glo.Y);

                    MouseDragElementBehavior mdeCB = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(cb).Add(mdeCB);
                    try
                    {
                        this.canvasMain.Children.Add(cb);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("可能是同名的字段(" + cb.Name + ")已经存在. 异常信息:\t\n" + ex.Message, "错误", MessageBoxButton.OK);
                        return;
                    }

                    cbLab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    cb.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    return;
                }

                BPTextBox mytb = new BPTextBox(tp,
                    this.winSelectTB.TB_KeyOfEn.Text);

                mytb.KeyName = this.winSelectTB.TB_Name.Text;

                // mytb.Name = this.winSelectTB.TB_Name;

                mytb.Cursor = Cursors.Hand;
                mytb.SetValue(Canvas.LeftProperty, Glo.X);
                mytb.SetValue(Canvas.TopProperty, Glo.Y);
                try
                {
                    this.canvasMain.Children.Add(mytb);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("可能是同名的字段(" + mytb.Name + ")已经存在. 异常信息:\t\n" + ex.Message, "错误", MessageBoxButton.OK);
                    return;
                }

                //MouseDragElementBehavior mymdeEdpTB = new MouseDragElementBehavior();
                //Interaction.GetBehaviors(mytb).Add(mymdeEdpTB);

                mytb.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                mytb.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                // 检查是否生成 标签.
                if (this.winSelectTB.CB_IsGenerLabel.IsChecked == true)
                {
                    /*要生成标签*/
                    BPLabel lab = new BPLabel();
                    lab.Content = this.winSelectTB.TB_Name.Text;
                    //lab.Width = 100;
                    //lab.Height = 23;
                    lab.Cursor = Cursors.Hand;
                    lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                    lab.SetValue(Canvas.TopProperty, Glo.Y);
                    this.canvasMain.Children.Add(lab);
                    MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(lab).Add(DragBehavior);
                    lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                }
            }

            if (c.Name == "winFrmBtn")
            {
                BPBtn btn = this.winFrmBtn.HisBtn;
                if (this.canvasMain.Children.Contains(btn))
                {
                    BPBtn mybtn = (BPBtn)this.canvasMain.FindName(btn.Name);
                    mybtn = btn;
                }
                else
                {
                    btn.Cursor = Cursors.Hand;
                    btn.SetValue(Canvas.LeftProperty, Glo.X);
                    btn.SetValue(Canvas.TopProperty, Glo.Y);
                    this.canvasMain.Children.Add(btn);
                    MouseDragElementBehavior mymdeE = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(btn).Add(mymdeE);
                    btn.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    btn.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                }
                this.SetSelectedTool(ToolBox.Mouse);
            }

            if (c.Name == "winFrmEle")
            {
                BPEle ele = this.winFrmEle.HisEle;
                BPEle myEle = this.canvasMain.FindName(ele.Name) as BPEle;
                if (myEle != null)
                {
                    myEle = ele;
                }
                else
                {

                    ele.Cursor = Cursors.Hand;
                    ele.SetValue(Canvas.LeftProperty, Glo.X);
                    ele.SetValue(Canvas.TopProperty, Glo.Y);
                    this.canvasMain.Children.Add(ele);
                    MouseDragElementBehavior mymdeE = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(ele).Add(mymdeE);
                    ele.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    ele.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                }
                this.SetSelectedTool(ToolBox.Mouse);
            }

            if (c.Name == "winFrmAttachmentM")
            {
                BPAttachmentM atth = this.winFrmAttachmentM.HisBPAttachment;
                if (this.canvasMain.Children.Contains(atth) == true)
                    return;

                atth.Label = this.winFrmAttachmentM.TB_Name.Text;
                atth.SaveTo = this.winFrmAttachmentM.TB_SaveTo.Text;
                atth.Cursor = Cursors.Hand;
                atth.SetValue(Canvas.LeftProperty, Glo.X);
                atth.SetValue(Canvas.TopProperty, Glo.Y);

                try
                {
                    this.canvasMain.Children.Add(atth);
                }
                catch (Exception ex)
                {

                    MessageBox.Show("增加附件控件时出现异常系统要求重新加载。异常信息:\t\n" + ex.Message);
                    this.BindFrm();
                    return;
                }

                MouseDragElementBehavior mymdeE = new MouseDragElementBehavior();
                Interaction.GetBehaviors(atth).Add(mymdeE);

                atth.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                atth.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                this.SetSelectedTool(ToolBox.Mouse);
            }

            if (c.Name == "winSelectAttachment")
            {
                BPAttachment atth = this.winSelectAttachment.HisBPAttachment;
                if (this.canvasMain.Children.Contains(atth) == true)
                    return;

                BPAttachment atthMy = new BPAttachment(this.winSelectAttachment.TB_No.Text, this.winSelectAttachment.TB_Name.Text,
                   this.winSelectAttachment.TB_Exts.Text, 70, this.winSelectAttachment.TB_SaveTo.Text);
                atthMy.SetValue(Canvas.LeftProperty, Glo.X);
                atthMy.SetValue(Canvas.TopProperty, Glo.Y);

                //atthMy.Label = dr["Name"] as string;
                //atthMy.Exts = dr["Exts"] as string;
                //atthMy.SaveTo = dr["SaveTo"] as string;

                atthMy.X = Glo.X;
                atthMy.Y = Glo.Y;

                atthMy.IsUpload = (bool)this.winSelectAttachment.CB_IsUpload.IsChecked;
                atthMy.IsDelete = (bool)this.winSelectAttachment.CB_IsDelete.IsChecked;
                atthMy.IsDownload = (bool)this.winSelectAttachment.CB_IsDownload.IsChecked;

                MouseDragElementBehavior mde = new MouseDragElementBehavior();
                Interaction.GetBehaviors(atthMy).Add(mde);
                this.canvasMain.Children.Add(atthMy);

                atthMy.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                atthMy.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);

                /*要生成标签*/
                BPLabel lab = new BPLabel();
                lab.Content = this.winSelectAttachment.TB_Name.Text;
                lab.Name = "lb" + DateTime.Now.ToString("yyyyMMddhhmmss");
                lab.Cursor = Cursors.Hand;
                lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                lab.SetValue(Canvas.TopProperty, Glo.Y);
                this.canvasMain.Children.Add(lab);
                MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                Interaction.GetBehaviors(lab).Add(DragBehavior);
                lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                this.SetSelectedTool(ToolBox.Mouse);

                // this.Save_Click(null, null);
                //  this.BindFrm();
            }


            if (c.Name == "winSelectDDL")
            {
                if (this.winSelectDDL.listBox1.SelectedIndex < 0)
                    return;

                ListBoxItem mylbi = this.winSelectDDL.listBox1.SelectedItem as ListBoxItem;
                string enKey = mylbi.Content.ToString();
                enKey = enKey.Substring(0, enKey.IndexOf(':'));

                BPDDL myddl = new BPDDL();
                myddl.KeyName = this.winSelectDDL.TB_KeyOfName.Text;
                myddl.Name = this.winSelectDDL.TB_KeyOfEn.Text;
                myddl.Width = 100;
                myddl.Height = 23;
                myddl.Cursor = Cursors.Hand;
                myddl.SetValue(Canvas.LeftProperty, Glo.X);
                myddl.SetValue(Canvas.TopProperty, Glo.Y);
                myddl.BindEns(enKey);
                this.canvasMain.Children.Add(myddl);
                //MouseDragElementBehavior mymdeE = new MouseDragElementBehavior();
                //Interaction.GetBehaviors(myddl).Add(mymdeE);

                myddl.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                myddl.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                // 检查是否生成 标签.
                if (this.winSelectDDL.CB_IsGenerLab.IsChecked == true)
                {
                    /*要生成标签*/
                    BPLabel lab = new BPLabel();
                    lab.Content = this.winSelectDDL.TB_KeyOfName.Text;
                    //lab.Width = 100;
                    //lab.Height = 23;
                    lab.Cursor = Cursors.Hand;
                    lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                    lab.SetValue(Canvas.TopProperty, Glo.Y);
                    this.canvasMain.Children.Add(lab);
                    MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(lab).Add(DragBehavior);
                    lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                }
            }
            if (c.Name == "winSelectRB")
            {
                if (this.winSelectRB.listBox1.SelectedIndex < 0)
                    return;

                ListBoxItem lbi = this.winSelectRB.listBox1.SelectedItem as ListBoxItem;
                string enumKey = lbi.Content.ToString();
                enumKey = enumKey.Substring(0, enumKey.IndexOf(':'));

                string cfgKeys = lbi.Tag as string;
                string[] strs = cfgKeys.Split('@');
                if (IsRB)
                {
                    int addX = 0;
                    int addY = 0;
                    string gName = this.winSelectRB.TB_KeyOfEn.Text; // "RB" + DateTime.Now.ToString("yyMMddhhmmss");
                    foreach (string str in strs)
                    {
                        if (string.IsNullOrEmpty(str))
                            continue;

                        string[] mykey = str.Split('=');
                        BPRadioBtn rb = new BPRadioBtn();
                        //rb.Name = Glo.FK_MapData + "_" + this.winSelectRB.TB_KeyOfName.Text + "_" + mykey[0];
                        rb.KeyName = this.winSelectRB.TB_KeyOfName.Text;
                        rb.Content = mykey[1];
                        rb.Tag = mykey[0];
                        rb.Name = Glo.FK_MapData + "_" + gName + "_" + mykey[0];
                        rb.UIBindKey = enumKey;
                        rb.SetValue(Canvas.LeftProperty, Glo.X + addX);
                        rb.SetValue(Canvas.TopProperty, Glo.Y + addY);
                        rb.Cursor = Cursors.Hand;
                        rb.GroupName = gName;
                        this.canvasMain.Children.Add(rb);
                        addY += 16;

                        rb.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                        rb.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    }

                    // 检查是否生成 标签.
                    if (this.winSelectRB.CB_IsGenerLab.IsChecked == true)
                    {
                        /*要生成标签*/
                        BPLabel lab = new BPLabel();
                        lab.Content = this.winSelectRB.TB_KeyOfName.Text;
                        //lab.Width = 100;
                        //lab.Height = 23;
                        lab.Cursor = Cursors.Hand;
                        lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                        lab.SetValue(Canvas.TopProperty, Glo.Y);
                        this.canvasMain.Children.Add(lab);
                        MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                        Interaction.GetBehaviors(lab).Add(DragBehavior);
                        lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                        lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                        this.SetSelectedTool(ToolBox.Mouse);
                    }
                }
                else
                {
                    /* 如果是ddl.*/
                    BPDDL myddlEnum = new BPDDL();
                    myddlEnum.Name = this.winSelectRB.TB_KeyOfEn.Text;
                    myddlEnum.KeyName = this.winSelectRB.TB_KeyOfName.Text;
                    myddlEnum.Width = 100;
                    myddlEnum.Height = 23;
                    myddlEnum.Cursor = Cursors.Hand;
                    myddlEnum.SetValue(Canvas.LeftProperty, Glo.X);
                    myddlEnum.SetValue(Canvas.TopProperty, Glo.Y);
                    myddlEnum.BindEnum(enumKey);
                    MouseDragElementBehavior mymdeEE = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(myddlEnum).Add(mymdeEE);
                    this.canvasMain.Children.Add(myddlEnum);

                    myddlEnum.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    myddlEnum.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                    // 检查是否生成 标签.
                    if (this.winSelectRB.CB_IsGenerLab.IsChecked == true)
                    {
                        /*要生成标签*/
                        BPLabel lab = new BPLabel();
                        lab.Content = this.winSelectRB.TB_KeyOfName.Text;
                        //lab.Width = 100;
                        //lab.Height = 23;
                        lab.Cursor = Cursors.Hand;
                        lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                        lab.SetValue(Canvas.TopProperty, Glo.Y);
                        this.canvasMain.Children.Add(lab);
                        MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                        Interaction.GetBehaviors(lab).Add(DragBehavior);
                        lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                        lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                        this.SetSelectedTool(ToolBox.Mouse);
                    }
                }
            }
        }

        void daCreateCheckGroup_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result);
                return;
            }

            if (Glo.X > 300)
                Glo.X = 300;

            /*如果是增加的审核分组.*/
            string gName = this.winSelectTB.TB_Name.Text;
            string gKey = this.winSelectTB.TB_KeyOfEn.Text;


            BPTextBox tbNote = new BPTextBox(TBType.String, gKey + "_Note");
            tbNote.KeyName = "审核意见";
            tbNote.Cursor = Cursors.Hand;
            tbNote.SetValue(Canvas.LeftProperty, Glo.X - 10);
            tbNote.SetValue(Canvas.TopProperty, Glo.Y);
            tbNote.Width = 550;
            tbNote.Height = 70;
            try
            {
                this.canvasMain.Children.Add(tbNote);
            }
            catch (Exception ex)
            {
                MessageBox.Show("可能是同名的字段(" + tbNote.Name + ")已经存在. 异常信息:\t\n" + ex.Message, "错误", MessageBoxButton.OK);
                return;
            }

            MouseDragElementBehavior mymdeNote = new MouseDragElementBehavior();
            Interaction.GetBehaviors(tbNote).Add(mymdeNote);
            tbNote.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            tbNote.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);


            BPTextBox tbChecker = new BPTextBox(TBType.String, gKey + "_Checker");
            tbChecker.KeyName = "审核人";
            tbChecker.Cursor = Cursors.Hand;
            tbChecker.SetValue(Canvas.LeftProperty, Glo.X + 80);
            tbChecker.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.canvasMain.Children.Add(tbChecker);

            MouseDragElementBehavior mymdeEdp = new MouseDragElementBehavior();
            Interaction.GetBehaviors(tbChecker).Add(mymdeEdp);
            tbChecker.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            tbChecker.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

            BPTextBox tbRDT = new BPTextBox(TBType.DateTime, gKey + "_RDT");
            tbRDT.KeyName = "审核时间";
            tbRDT.Cursor = Cursors.Hand;
            tbRDT.SetValue(Canvas.LeftProperty, Glo.X + 320);
            tbRDT.SetValue(Canvas.TopProperty, Glo.Y + 75);
            this.canvasMain.Children.Add(tbRDT);
            MouseDragElementBehavior mymdeRDT = new MouseDragElementBehavior();
            Interaction.GetBehaviors(tbRDT).Add(mymdeRDT);
            tbRDT.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            tbRDT.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);


            /*要生成标签*/
            BPLabel abCheckNote = new BPLabel();
            abCheckNote.Content = "审核意见";
            abCheckNote.Name = "Lab" + gKey + "Note";
            abCheckNote.Cursor = Cursors.Hand;
            abCheckNote.SetValue(Canvas.LeftProperty, Glo.X - 20);
            abCheckNote.SetValue(Canvas.TopProperty, Glo.Y);
            this.canvasMain.Children.Add(abCheckNote);
            MouseDragElementBehavior DragBehaviorNote = new MouseDragElementBehavior();
            Interaction.GetBehaviors(abCheckNote).Add(DragBehaviorNote);
            abCheckNote.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            abCheckNote.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);


            BPLabel labChecker = new BPLabel();
            labChecker.Content = "审核人";
            labChecker.Name = "Lab" + gKey + "Checker";
            labChecker.Cursor = Cursors.Hand;
            labChecker.SetValue(Canvas.LeftProperty, Glo.X + 40);
            labChecker.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.canvasMain.Children.Add(labChecker);
            MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
            Interaction.GetBehaviors(labChecker).Add(DragBehavior);
            labChecker.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            labChecker.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

            BPLabel abCheckRDT = new BPLabel();
            abCheckRDT.Content = "日期";
            abCheckRDT.Name = "Lab" + gKey + "RDT";
            abCheckRDT.Cursor = Cursors.Hand;
            abCheckRDT.SetValue(Canvas.LeftProperty, Glo.X + 290);
            abCheckRDT.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.canvasMain.Children.Add(abCheckRDT);
            MouseDragElementBehavior DragBehaviorRDT = new MouseDragElementBehavior();
            Interaction.GetBehaviors(abCheckRDT).Add(DragBehaviorRDT);
            abCheckRDT.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
            abCheckRDT.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
        }
        private void DoubleClick_Timer(object sender, EventArgs e)
        {
            _doubleClickTimer.Stop();
        }
        public MainPage()
        {
            InitializeComponent();

            #region 获取参数
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.CfgKeyAsync("CompanyID");
            da.CfgKeyCompleted += new EventHandler<FF.CfgKeyCompletedEventArgs>(da_CfgKeyCompleted);

            FF.CCFormSoapClient daAppCenter = Glo.GetCCFormSoapClientServiceInstance();
            daAppCenter.CfgKeyAsync("AppCenterDBType");
            daAppCenter.CfgKeyCompleted += new EventHandler<FF.CfgKeyCompletedEventArgs>(daAppCenter_CfgKeyCompleted);
            #endregion 获取参数

            _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
            _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            _doubleClickTimer.Tick += new EventHandler(DoubleClick_Timer);

            #region chinwin.
            winFrmImg.Name = "winFrmImg";
            winFrmLab.Name = "winFrmLab";
            winSelectM2M.Name = "winSelectM2M";
            winFrmLink.Name = "winFrmLink";
            winSelectTB.Name = "winSelectTB";
            winSelectDDL.Name = "winSelectDDL";
            winSelectRB.Name = "winSelectRB";
            winFrmImp.Name = "winFrmImp";
            winFrmEle.Name = "winFrmEle";
            winSelectAttachment.Name = "winSelectAttachment";
            winFrmOp.Name = "winFrmOP";
            winFrmBtn.Name = "winFrmBtn";
            winFlowFrm.Name = "winFlowFrm";
            winNodeFrms.Name = "winNodeFrms";
            winFrmAttachmentM.Name = "winFrmAttachmentM";

            winFrmImg.Closed += new EventHandler(WindowDilag_Closed);
            winFrmLab.Closed += new EventHandler(WindowDilag_Closed);
            winFrmLink.Closed += new EventHandler(WindowDilag_Closed);
            winNodeFrms.Closed += new EventHandler(WindowDilag_Closed);
            winSelectTB.Closed += new EventHandler(WindowDilag_Closed);
            winSelectDDL.Closed += new EventHandler(WindowDilag_Closed);
            winSelectRB.Closed += new EventHandler(WindowDilag_Closed);
            winFrmImp.Closed += new EventHandler(WindowDilag_Closed);
            winFrmEle.Closed += new EventHandler(WindowDilag_Closed);
            winFrmBtn.Closed += new EventHandler(WindowDilag_Closed);
            winSelectAttachment.Closed += new EventHandler(WindowDilag_Closed);
            winFrmOp.Closed += new EventHandler(WindowDilag_Closed);
            winFlowFrm.Closed += new EventHandler(WindowDilag_Closed);
            winFrmAttachmentM.Closed += new EventHandler(WindowDilag_Closed);
            winSelectM2M.Closed += new EventHandler(WindowDilag_Closed);
            #endregion chinwin.

            #region 构造
            this.SetSelectedTool(ToolBox.Mouse);
            e1 = new Ellipse();
            e1.Tag = "e1";
            e1.Cursor = Cursors.Hand;
            e1.MouseLeftButtonDown += new MouseButtonEventHandler(e_MouseLeftButtonDown);
            e1.Width = 9;
            e1.Height = 9;
            e1.Fill = new SolidColorBrush(Colors.Green);

            e2 = new Ellipse();
            e2.Tag = "e2";
            e2.Cursor = Cursors.Hand;
            e2.MouseLeftButtonDown += new MouseButtonEventHandler(e_MouseLeftButtonDown);
            e2.Width = 9;
            e2.Height = 9;
            e2.Fill = new SolidColorBrush(Colors.Green);
            #endregion

            #region 生成toolbar .
            List<Func> ens = new List<Func>();
            ens = Func.instance.GetToolList();
            foreach (Func en in ens)
            {
                Toolbar.ToolbarButton btn = new Toolbar.ToolbarButton();
                btn.Name = "Btn_" + en.No;
                btn.Click += new RoutedEventHandler(ToolBar_Click);

                StackPanel mysp = new StackPanel();
                mysp.Orientation = Orientation.Horizontal;
                mysp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                mysp.Name = "sp" + en.No;

                Image img = new Image();
                BitmapImage png = new BitmapImage(new Uri("/CCForm;component/Img/" + en.No + ".png", UriKind.Relative));
                img.Source = png;
                img.Width = 13;
                img.Height = 13;
                mysp.Children.Add(img);

                TextBlock tb = new TextBlock();
                tb.Name = "tbT" + en.No;
                tb.Text = en.Name + " ";
                tb.FontSize = 13;
                mysp.Children.Add(tb);
                btn.Content = mysp;
                this.toolbar1.AddBtn(btn);
            }

            List<EleFunc> ensEle = new List<EleFunc>();
            ensEle = EleFunc.instance.getToolList();
            foreach (EleFunc en in ensEle)
            {
                Toolbar.ToolbarBtn btn = new Toolbar.ToolbarBtn();
                btn.Name = "Btn_" + en.No;
                btn.Click += new RoutedEventHandler(ToolBar_Click);

                StackPanel mysp = new StackPanel();
                Image img = new Image();
                BitmapImage png = new BitmapImage(new Uri("/CCForm;component/Img/" + en.No + ".png", UriKind.Relative));
                img.Source = png;
                mysp.Children.Add(img);


                TextBlock tb = new TextBlock();
                tb.Name = "tbT" + en.No;
                tb.Text = en.Name + " ";
                tb.FontSize = 15;
                mysp.Children.Add(tb);
                btn.Content = mysp;
                this.toolbar1.AddBtn(btn);
            }
            this.ListBox1.ItemsSource = ToolBox.instance.GetToolBoxList();
            #endregion

            #region 其它.
            this.canvasMain.KeyDown += new KeyEventHandler(canvasMain_KeyDown);
            this.BindTreeView();
            #endregion 其它.
        }

        void daAppCenter_CfgKeyCompleted(object sender, FF.CfgKeyCompletedEventArgs e)
        {
            Glo.AppCenterDBType = e.Result;
        }
        void da_CfgKeyCompleted(object sender, FF.CfgKeyCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Result))
                Glo.CompanyID = "CCFlow";
            else
                Glo.CompanyID = e.Result;
        }
        void canvasMain_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (this.canvasMain.Children.Contains(muFrm) == false)
                this.canvasMain.Children.Add(muFrm);
            return;

            muFrm.Visibility = System.Windows.Visibility.Visible;
            muFrm.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
            muFrm.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
            //   this.muFrm.Visibility = System.Windows.Visibility.Visible;
        }
        public void BindTreeView()
        {
            string sqls = "SELECT NodeID, Name,Step FROM WF_Node WHERE FK_Flow='" + Glo.FK_Flow + "'";
            sqls += "@SELECT * FROM WF_FrmNode WHERE FK_Flow='" + Glo.FK_Flow + "' AND FK_Frm IN (SELECT No FROM Sys_MapData ) ORDER BY FK_Node, Idx";
            sqls += "@SELECT * FROM Sys_MapData ";
            sqls += "@SELECT * FROM Sys_MapDtl WHERE  FK_MapData IN( SELECT No FROM Sys_MapData WHERE FK_Flow='" + Glo.FK_Flow + "') AND DtlShowModel=1";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableSAsync(sqls);
            da.RunSQLReturnTableSCompleted += new EventHandler<FF.RunSQLReturnTableSCompletedEventArgs>(BindTreeView_RunSQLReturnTableCompleted);
        }
        void BindTreeView_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableSCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            this.treeView_Node.Items.Clear();
            this.treeView_Flow.Items.Clear();

            DataTable dtNode = ds.Tables[0];
            DataTable dtFrmNode = ds.Tables[1];
            DataTable dtFrm = ds.Tables[2];
            DataTable dtDtl = ds.Tables[3];

            foreach (DataRow dr in dtNode.Rows)
            {
                TreeViewItem li = new TreeViewItem();
                string nodeID = dr[0].ToString();
                string name = dr["Name"].ToString();
                string step = dr["Step"].ToString();
                li.Name = "ND" + nodeID;
                li.Header = "编号:" + nodeID + ",步骤:" + step + "名称:" + name;

                li.MouseRightButtonDown += new MouseButtonEventHandler(li_MouseRightButtonDown);
                li.MouseLeftButtonDown += new MouseButtonEventHandler(BindTreeView_li_MouseLeftButtonDown);

                if (Glo.FK_Node == int.Parse(nodeID))
                    li.IsExpanded = true;

                if (li.Name.ToString() == Glo.FK_MapData)
                    li.IsSelected = true;

                foreach (DataRow drFrmNode in dtFrmNode.Rows)
                {
                    string fk_node = drFrmNode["FK_Node"] as string;
                    if (nodeID != fk_node)
                        continue;

                    string fk_frm = drFrmNode["FK_Frm"] as string;
                    foreach (DataRow drFrm in dtFrm.Rows)
                    {
                        string noFrm = drFrm["No"] as string;
                        if (fk_frm != noFrm)
                            continue;

                        string formType = drFrm["FormType"] as string;
                        string nameFrm = drFrm["Name"] as string;
                        string url = drFrm["URL"] as string;

                        TreeViewItem liItem = new TreeViewItem();
                        liItem.Header = noFrm + "," + nameFrm;
                        liItem.Name = fk_node + "_" + noFrm;
                        liItem.Tag = "@FK_Node=" + fk_node + "@No=" + noFrm + "@Name=" + drFrm["Name"] + "@PTable=" + drFrm["PTable"] + "@URL=" + url + "@FormType=" + drFrm["formType"];
                        // liItem.MouseLeftButtonDown += new MouseButtonEventHandler(liItem_MouseLeftButtonDown);

                        liItem.MouseRightButtonDown += new MouseButtonEventHandler(li_MouseRightButtonDown);
                        liItem.MouseLeftButtonDown += new MouseButtonEventHandler(BindTreeView_li_MouseLeftButtonDown);
                        li.Items.Add(liItem);

                        if (liItem.Name == Glo.FK_MapData)
                            liItem.IsSelected = true;

                        foreach (DataRow drDtl in dtDtl.Rows)
                        {
                            string fk_frmDtl = drDtl["FK_MapData"] as string;
                            if (fk_frmDtl != noFrm)
                                continue;

                            string noDtlFrm = drDtl["No"] as string;
                            string nameDtlFrm = drDtl["Name"] as string;

                            TreeViewItem liItemDtl = new TreeViewItem();
                            liItemDtl.Header = fk_frmDtl + "," + nameDtlFrm;
                            liItemDtl.Name = fk_node + "_" + fk_frm + "_" + noDtlFrm;
                            liItemDtl.Tag = "@FK_Node=" + fk_node + "@No=" + fk_frmDtl + "@Name=" + drDtl["Name"] + "@PTable=" + drDtl["PTable"] + "@IsUpdate=" + drDtl["IsUpdate"];
                            liItemDtl.MouseRightButtonDown += new MouseButtonEventHandler(li_MouseRightButtonDown);
                            liItemDtl.MouseLeftButtonDown += new MouseButtonEventHandler(BindTreeView_li_MouseLeftButtonDown);
                            liItem.Items.Add(liItemDtl);

                            if (liItemDtl.Name == Glo.FK_MapData)
                                liItemDtl.IsSelected = true;
                        }
                    }
                }
                this.treeView_Node.Items.Add(li);

            }

            //绑定流程表单.
            foreach (DataRow drFrm in dtFrm.Rows)
            {
                TreeViewItem li = new TreeViewItem();
                string no = drFrm["No"] as string;
                string name = drFrm["Name"] as string;
                li.Header = "编号:" + no + "名称:" + name;
                li.Tag = no;
                this.treeView_Flow.Items.Add(li);
            }
            this.BindFrm();
        }
        void muItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsmuElePanel = true;
            this.CloseMenum();

            Liquid.MenuItem item = sender as Liquid.MenuItem;
            TreeViewItem li = this.treeView_Node.SelectedItem as TreeViewItem;
            if (li == null)
                return;

            switch (item.Name)
            {
                case "FrmBill":
                    MessageBox.Show("设置方式，请详看操作说明书的配置章节，可视化的设计在开发过程中。");
                    break;
                case "FrmRef":
                    this.BindTreeView();
                    break;
                case "FrmAction":
                    FrmEvent frm = new FrmEvent();
                    frm.Show();
                    return;
                case "RefFrm":
                    if (li.Tag != null)
                    {
                        BP.WF.FrmNode myfn = new BP.WF.FrmNode(li.Tag.ToString());
                        this.winNodeFrms.FK_Node = myfn.FK_Node;
                    }
                    else
                    {
                        this.winNodeFrms.FK_Node = int.Parse(li.Name.Replace("ND", ""));
                    }
                    this.winNodeFrms.listBox1.Items.Clear();
                    this.winNodeFrms.Show();
                    break;
                case "NewFrm":
                    // BP.WF.FrmNode fn = new BP.WF.FrmNode(li.Tag.ToString());

                    this.winFlowFrm.TB_No.Text = "";
                    this.winFlowFrm.TB_No.IsEnabled = true;
                    this.winFlowFrm.TB_Name.Text = "";
                    this.winFlowFrm.TB_PTable.Text = "";
                    this.winFlowFrm.TB_URL.Text = "";
                    this.winFlowFrm.NodeID = int.Parse(li.Name.Replace("ND", ""));
                    this.winFlowFrm.Show();
                    break;
                case "DeleteFrm":
                    if (li.Tag == null)
                        return;

                    if (Glo.IsDtlFrm == true)
                    {
                        if (MessageBox.Show("明细表单不能被删除.",
                       "提示", MessageBoxButton.OK)
                       == MessageBoxResult.No)
                            return;
                        return;
                    }

                    if (MessageBox.Show("您确实要删除吗？如果删除所有相关联的节点表单都会被删除!!!",
                        "删除提示", MessageBoxButton.OKCancel)
                        == MessageBoxResult.No)
                        return;

                    string[] strs = li.Name.Split('_');
                    string fk_frm = strs[1];
                    Glo.FK_MapData = "ND" + strs[0];
                    Glo.FK_Node = int.Parse(strs[0]);
                    this.DoTypeName = "DeleteFrm";
                    this.DoType(this.DoTypeName, fk_frm, null, null, null, null);
                    break;
                case "EditFrm":
                    if (li.Tag == null)
                        return;

                    if (Glo.IsDtlFrm)
                        return;

                    BP.WF.FrmNode fn = new BP.WF.FrmNode(li.Tag.ToString());
                    this.winFlowFrm.TB_No.Text = fn.No;
                    this.winFlowFrm.TB_Name.Text = fn.Name;
                    this.winFlowFrm.DDL_FrmType.SelectedIndex = fn.FormType;
                    this.winFlowFrm.TB_URL.Text = fn.URL;
                    this.winFlowFrm.TB_PTable.Text = fn.PTable;
                    this.winFlowFrm.CB_IsReadonly.IsChecked = fn.IsReadonly;
                    this.winFlowFrm.NodeID = fn.FK_Node;
                    this.winFlowFrm.Show();
                    break;
                case "DeFrm": // 设计表单.
                    Glo.IsDtlFrm = false;
                    this.IsmuElePanel = true;
                    if (li.Tag == null)
                    {
                        Glo.FK_MapData = li.Name as string;
                        if (Glo.FK_MapData.Contains("ND"))
                            Glo.FK_Node = int.Parse(Glo.FK_MapData.Replace("ND", ""));
                    }
                    else
                    {
                        string[] str = li.Name.Split('_');
                        Glo.FK_Node = int.Parse(str[0]);
                        if (str.Length == 3)
                        {
                            Glo.IsDtlFrm = true;
                            Glo.FK_MapData = str[2];
                        }
                        else
                        {
                            Glo.FK_MapData = str[1];
                            Glo.IsDtlFrm = true;
                        }
                    }
                    this.BindFrm();
                    break;
                case "FrmUp":   // 上移.
                case "FrmDown": // 上移.
                    if (li.Name.ToString().Contains("ND"))
                        return;

                    string[] strs1 = li.Name.Split('_');
                    Glo.FK_MapData = strs1[1];
                    Glo.FK_Node = int.Parse(strs1[0]);
                    this.DoType(item.Name, Glo.FK_Node.ToString(), Glo.FK_MapData, null, null, null);
                    break;
                default:
                    break;
            }
        }
        private string DoTypeName = null;
        public void DoType(string doType, string v1, string v2, string v3, string v4, string v5)
        {
            this.DoTypeName = doType;
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync(doType, v1, v2, v3, v4, v5);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            switch (this.DoTypeName)
            {
                case "FrmUp":
                case "FrmDown":
                case "DeleteFrm":
                    this.BindTreeView();
                    break;
                default:
                    if (e.Result != null)
                        MessageBox.Show(e.Result, "执行信息", MessageBoxButton.OK);
                    break;
            }
        }
        void li_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Point position = e.GetPosition(this.treeView_Node);
            Point muNodeFrmPos = treeView_Node.TransformToVisual(this.treeView_Node).Transform(position);
            TreeViewItem node = sender as TreeViewItem;

            var x = position.X;
            var y = position.Y;
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

            this.CloseMenum();
            muNodeFrm.SetValue(Canvas.LeftProperty, x);
            muNodeFrm.SetValue(Canvas.TopProperty, y);
            muNodeFrm.Visibility = System.Windows.Visibility.Visible;
        }
        private void CloseMenum()
        {
            muFrm.Visibility = System.Windows.Visibility.Collapsed;
            muNodeFrm.Visibility = System.Windows.Visibility.Collapsed;
            muElePanel.Visibility = System.Windows.Visibility.Collapsed;
            muFlowFrm.Visibility = System.Windows.Visibility.Collapsed;
        }
        private System.Windows.Threading.DispatcherTimer _doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
        void BindTreeView_li_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseMenum();

            if (_doubleClickTimer.IsEnabled || IsmuElePanel == true)
            {
                _doubleClickTimer.Stop();
                IsmuElePanel = false;
                TreeViewItem li = this.treeView_Node.SelectedItem as TreeViewItem;
                if (li == null)
                    return;

                if (li.Tag == null)
                {
                    Glo.FK_MapData = li.Name.ToString();
                    Glo.FK_Node = int.Parse(li.Name.Replace("ND", ""));
                }
                else
                {
                    string[] strs = li.Name.Split('_');
                    Glo.FK_Node = int.Parse(strs[0]);
                    if (strs.Length == 3)
                    {
                        Glo.IsDtlFrm = true;
                        Glo.FK_MapData = strs[2];
                    }
                    else
                    {
                        Glo.IsDtlFrm = false;
                        Glo.FK_MapData = strs[1];
                    }
                }
                this.BindFrm();
            }
            else
            {
                _doubleClickTimer.Start();
            }
        }
        void canvasMain_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }
        public DataSet FrmDS = null;
        public void BindFrm()
        {
            loadingWindow.Show();
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.GenerFrmAsync(Glo.FK_MapData, 0);
            da.GenerFrmCompleted += new EventHandler<FF.GenerFrmCompletedEventArgs>(da_BindFrmCompleted);
        }
        void da_BindFrmCompleted(object sender, FF.GenerFrmCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Length < 30)
                    throw new Exception(e.Result);
                BindFrm(e.Result);
            }
            catch (Exception ex)
            {
                throw new Exception("在绑定控件时出现错误,技术信息:Table=" + Glo.TempVal + " Ex=" + ex.Message);
            }
        }
        void BindFrm(string xmlStrs)
        {
            this.canvasMain.Children.Clear();
            this.FrmDS = new DataSet();
            try
            {
                if (xmlStrs.Length < 200)
                    throw new Exception(xmlStrs);

                this.FrmDS.FromXml(xmlStrs);
                loadingWindow.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Err", MessageBoxButton.OK);
                loadingWindow.DialogResult = true;
                return;
            }

            string table = "";
            try
            {
                foreach (DataTable dt in this.FrmDS.Tables)
                {
                    Glo.TempVal = dt.TableName;
                    table = dt.TableName;
                    switch (dt.TableName)
                    {
                        case "Sys_FrmEle":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPEle bpele = new BPEle();
                                bpele.Name = dr["MyPK"].ToString();

                                if (string.IsNullOrEmpty(dr["EleType"]))
                                    continue;

                                if (string.IsNullOrEmpty(dr["EleID"]))
                                    continue;

                                if (string.IsNullOrEmpty(dr["EleName"]))
                                    continue;

                                bpele.EleType = dr["EleType"].ToString();
                                bpele.EleName = dr["EleName"].ToString();
                                bpele.EleID = dr["EleID"].ToString();

                                bpele.Cursor = Cursors.Hand;
                                bpele.SetValue(Canvas.LeftProperty, double.Parse(dr["X"].ToString()));
                                bpele.SetValue(Canvas.TopProperty, double.Parse(dr["Y"].ToString()));

                                bpele.Width = double.Parse(dr["W"].ToString());
                                bpele.Height = double.Parse(dr["H"].ToString());

                                MouseDragElementBehavior mdeImg = new MouseDragElementBehavior();
                                Interaction.GetBehaviors(bpele).Add(mdeImg);
                                this.canvasMain.Children.Add(bpele);

                                bpele.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                bpele.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_MapData":
                            if (dt.Rows.Count == 0)
                                continue;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["No"] != Glo.FK_MapData)
                                    continue;

                                Glo.HisMapData = new MapData();
                                Glo.HisMapData.FrmH = double.Parse(dt.Rows[0]["FrmH"]);
                                Glo.HisMapData.FrmW = double.Parse(dt.Rows[0]["FrmW"]);
                                Glo.HisMapData.No = (string)dt.Rows[0]["No"];
                                Glo.HisMapData.Name = (string)dt.Rows[0]["Name"];
                                Glo.IsDtlFrm = false;

                                this.canvasMain.Width = Glo.HisMapData.FrmW;
                                this.canvasMain.Height = Glo.HisMapData.FrmH;
                                this.scrollViewer1.Width = Glo.HisMapData.FrmW;
                            }
                            break;
                        case "Sys_FrmBtn":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPBtn btn = new BPBtn();
                                btn.Name = dr["MyPK"];
                                btn.Content = dr["Text"].Replace("&nbsp;", " ");
                                btn.HisBtnType = (BtnType)int.Parse(dr["BtnType"]);
                                btn.HisEventType = (EventType)int.Parse(dr["EventType"]);

                                if (dr["EventContext"] != null)
                                    btn.EventContext = dr["EventContext"].Replace("~", "'");

                                if (dr["MsgErr"] != null)
                                    btn.MsgErr = dr["MsgErr"].Replace("~", "'");

                                if (dr["MsgOK"] != null)
                                    btn.MsgOK = dr["MsgOK"].Replace("~", "'");

                                btn.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                btn.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));
                                this.canvasMain.Children.Add(btn);
                                btn.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                btn.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmLine":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                string color = dr["BorderColor"];
                                if (string.IsNullOrEmpty(color))
                                    color = "Black";

                                BPLine myline = new BPLine(dr["MyPK"], color, double.Parse(dr["BorderWidth"]),
                                    double.Parse(dr["X1"]), double.Parse(dr["Y1"]), double.Parse(dr["X2"]),
                                    double.Parse(dr["Y2"]));

                                myline.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                myline.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));

                                this.canvasMain.Children.Add(myline);
                                myline.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                myline.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmLab":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPLabel lab = new BPLabel();
                                lab.Name = dr["MyPK"];
                                string text = dr["Text"].Replace("&nbsp;", " ");
                                text = text.Replace("@", "\n");
                                lab.Content = text;
                                lab.FontSize = double.Parse(dr["FontSize"]);
                                lab.Cursor = Cursors.Hand;
                                lab.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                lab.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));

                                if (dr["IsBold"] == "1")
                                    lab.FontWeight = FontWeights.Bold;
                                else
                                    lab.FontWeight = FontWeights.Normal;


                                string color = dr["FontColor"];
                                lab.Foreground = new SolidColorBrush(Glo.ToColor(color));
                                this.canvasMain.Children.Add(lab);
                                MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                                Interaction.GetBehaviors(lab).Add(DragBehavior);
                                lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmLink":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPLink link = new BPLink();
                                link.Name = dr["MyPK"];
                                link.Content = dr["Text"];
                                link.URL = dr["URL"];

                                link.WinTarget = dr["Target"];

                                link.FontSize = double.Parse(dr["FontSize"]);
                                link.Cursor = Cursors.Hand;
                                link.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                link.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));

                                string color = dr["FontColor"];
                                if (string.IsNullOrEmpty(color))
                                    color = "Black";

                                link.Foreground = new SolidColorBrush(Glo.ToColor(color));

                                this.canvasMain.Children.Add(link);

                                link.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                link.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                                MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                                Interaction.GetBehaviors(link).Add(DragBehavior);
                            }
                            continue;
                        case "Sys_FrmImg":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPImg img = new BPImg();
                                img.Name = dr["MyPK"];
                                img.Cursor = Cursors.Hand;
                                img.SetValue(Canvas.LeftProperty, double.Parse(dr["X"].ToString()));
                                img.SetValue(Canvas.TopProperty, double.Parse(dr["Y"].ToString()));

                                img.Width = double.Parse(dr["W"].ToString());
                                img.Height = double.Parse(dr["H"].ToString());

                                //判断图片路径是否修改
                                if (dr["ImgURL"] != null && dr["ImgURL"].Contains("DataUser"))
                                {
                                    ImageBrush ib = new ImageBrush();
                                    BitmapImage png = new BitmapImage(new Uri(Glo.BPMHost + dr["ImgURL"], UriKind.RelativeOrAbsolute));
                                    ib.ImageSource = png;
                                    img.Background = ib;
                                    img.HisPng = png;
                                }
                                img.WinURL = dr["LinkURL"];
                                img.WinTarget = dr["LinkTarget"];
                                //MouseDragElementBehavior mdeImg = new MouseDragElementBehavior();
                                //Interaction.GetBehaviors(img).Add(mdeImg);
                                this.canvasMain.Children.Add(img);

                                img.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                img.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmImgAth":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPImgAth ath = new BPImgAth();
                                ath.Name = dr["MyPK"];
                                ath.Cursor = Cursors.Hand;
                                ath.SetValue(Canvas.LeftProperty, double.Parse(dr["X"].ToString()));
                                ath.SetValue(Canvas.TopProperty, double.Parse(dr["Y"].ToString()));

                                ath.Height = double.Parse(dr["H"].ToString());
                                ath.Width = double.Parse(dr["W"].ToString());

                                //MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                //Interaction.GetBehaviors(ath).Add(mde);
                                this.canvasMain.Children.Add(ath);

                                ath.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                ath.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmRB":
                            DataTable dtRB = this.FrmDS.Tables["Sys_FrmRB"];
                            foreach (DataRow dr in dtRB.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPRadioBtn btn = new BPRadioBtn();
                                btn.Name = dr["MyPK"];
                                btn.GroupName = dr["KeyOfEn"];
                                btn.Content = dr["Lab"];
                                btn.UIBindKey = dr["EnumKey"];
                                btn.Tag = dr["IntKey"];
                                btn.SetValue(Canvas.LeftProperty, double.Parse(dr["X"].ToString()));
                                btn.SetValue(Canvas.TopProperty, double.Parse(dr["Y"].ToString()));

                                MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                Interaction.GetBehaviors(btn).Add(mde);
                                this.canvasMain.Children.Add(btn);

                                btn.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                btn.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_MapAttr":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["UIVisible"] == "0")
                                    continue;

                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                string myPk = dr["MyPK"];
                                string FK_MapData = dr["FK_MapData"];
                                string keyOfEn = dr["KeyOfEn"];
                                string name = dr["Name"];
                                string defVal = dr["DefVal"];
                                string UIContralType = dr["UIContralType"];
                                string MyDataType = dr["MyDataType"];
                                string lgType = dr["LGType"];
                                double X = double.Parse(dr["X"]);
                                double Y = double.Parse(dr["Y"]);
                                if (X == 0)
                                    X = 100;
                                if (Y == 0)
                                    Y = 100;

                                string UIBindKey = dr["UIBindKey"];
                                switch (UIContralType)
                                {
                                    case CtrlType.TextBox:
                                        TBType tp = TBType.String;
                                        switch (MyDataType)
                                        {
                                            case DataType.AppInt:
                                                tp = TBType.Int;
                                                break;
                                            case DataType.AppFloat:
                                            case DataType.AppDouble:
                                                tp = TBType.Float;
                                                break;
                                            case DataType.AppMoney:
                                                tp = TBType.Money;
                                                break;
                                            case DataType.AppString:
                                                tp = TBType.String;
                                                break;
                                            case DataType.AppDateTime:
                                                tp = TBType.DateTime;
                                                break;
                                            case DataType.AppDate:
                                                tp = TBType.Date;
                                                break;
                                            default:
                                                break;
                                        }

                                        BPTextBox tb = new BPTextBox(tp);
                                        tb.NameOfReal = keyOfEn;
                                        tb.Name = keyOfEn;
                                        tb.SetValue(Canvas.LeftProperty, X);
                                        tb.SetValue(Canvas.TopProperty, Y);
                                        tb.X = X;
                                        tb.Y = Y;

                                        tb.Width = double.Parse(dr["UIWidth"]);
                                        tb.Height = double.Parse(dr["UIHeight"]);
                                        if (this.canvasMain.FindName(tb.Name) != null)
                                        {
                                            MessageBox.Show("已经存在" + tb.Name);
                                            continue;
                                        }

                                        this.canvasMain.Children.Add(tb);
                                        //MouseDragElementBehavior mymdee = new MouseDragElementBehavior();
                                        //Interaction.GetBehaviors(tb).Add(mymdee);
                                        tb.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                        tb.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                                        //   }
                                        break;
                                    case CtrlType.DDL:
                                        BPDDL ddl = new BPDDL();
                                        ddl.Name = keyOfEn;
                                        ddl.HisLGType = lgType;
                                        ddl.Width = double.Parse(dr["UIWidth"]);
                                        ddl.UIBindKey = UIBindKey;
                                        ddl.HisLGType = lgType;
                                        if (lgType == LGType.Enum)
                                        {
                                            ddl.BindEnum(UIBindKey);
                                        }
                                        else
                                        {
                                            ddl.BindEns(UIBindKey);
                                        }

                                        ddl.SetValue(Canvas.LeftProperty, X);
                                        ddl.SetValue(Canvas.TopProperty, Y);
                                        this.canvasMain.Children.Add(ddl);

                                        ddl.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                                        ddl.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                        break;
                                    case CtrlType.CheckBox:
                                        BPCheckBox cb = new BPCheckBox();
                                        cb.Name = keyOfEn;
                                        cb.Content = name;

                                        Label cbLab = new Label();
                                        cbLab.Name = "CBLab" + cb.Name;
                                        cbLab.Content = name;
                                        cbLab.Tag = keyOfEn;
                                        cb.Content = cbLab;

                                        if (defVal == "1")
                                            cb.IsChecked = true;
                                        else
                                            cb.IsChecked = false;

                                        cb.SetValue(Canvas.LeftProperty, X);
                                        cb.SetValue(Canvas.TopProperty, Y);

                                        MouseDragElementBehavior mdeCB = new MouseDragElementBehavior();
                                        Interaction.GetBehaviors(cb).Add(mdeCB);
                                        this.canvasMain.Children.Add(cb);

                                        cbLab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                        cb.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                                        break;
                                    case CtrlType.RB:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            continue;
                        case "Sys_MapM2M":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPM2M m2m = new BPM2M(dr["NoOfObj"]);
                                m2m.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                m2m.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));

                                m2m.Width = double.Parse(dr["W"]);
                                m2m.Height = double.Parse(dr["H"]);

                                //MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                //Interaction.GetBehaviors(m2m).Add(mde);
                                this.canvasMain.Children.Add(m2m);

                                m2m.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                m2m.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_MapDtl":
                            foreach (DataRow dr in dt.Rows)
                            {
                                BPDtl dtl = new BPDtl(dr["No"]);
                                dtl.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                dtl.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));
                                dtl.Width = double.Parse(dr["W"]);
                                dtl.Height = double.Parse(dr["H"]);

                                //MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                //Interaction.GetBehaviors(dtl).Add(mde);
                                this.canvasMain.Children.Add(dtl);
                                dtl.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                dtl.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                            }
                            continue;
                        case "Sys_FrmAttachment":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                string uploadTypeInt = dr["UploadType"].ToString();
                                if (uploadTypeInt == null)
                                    uploadTypeInt = "0";

                                AttachmentUploadType uploadType = (AttachmentUploadType)int.Parse(uploadTypeInt);
                                if (uploadType == AttachmentUploadType.Single)
                                {

                                    BPAttachment ath = new BPAttachment(dr["NoOfObj"],
                                        dr["Name"], dr["Exts"],
                                        double.Parse(dr["W"]), dr["SaveTo"].ToString());

                                    ath.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                    ath.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));

                                    ath.Label = dr["Name"] as string;
                                    ath.Exts = dr["Exts"] as string;
                                    ath.SaveTo = dr["SaveTo"] as string;

                                    ath.X = double.Parse(dr["X"]);
                                    ath.Y = double.Parse(dr["Y"]);

                                    if (dr["IsUpload"] == "1")
                                        ath.IsUpload = true;
                                    else
                                        ath.IsUpload = false;

                                    if (dr["IsDelete"] == "1")
                                        ath.IsDelete = true;
                                    else
                                        ath.IsDelete = false;

                                    if (dr["IsDownload"] == "1")
                                        ath.IsDownload = true;
                                    else
                                        ath.IsDownload = false;

                                    MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                    Interaction.GetBehaviors(ath).Add(mde);
                                    this.canvasMain.Children.Add(ath);

                                    ath.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                                    ath.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                    continue;
                                }

                                if (uploadType == AttachmentUploadType.Multi)
                                {
                                    BPAttachmentM athM = new BPAttachmentM();
                                    athM.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                    athM.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));
                                    athM.Name = dr["NoOfObj"];
                                    athM.Width = double.Parse(dr["W"]);
                                    athM.Height = double.Parse(dr["H"]);
                                    athM.X = double.Parse(dr["X"]);
                                    athM.Y = double.Parse(dr["Y"]);
                                    athM.SaveTo = dr["SaveTo"];
                                    athM.Text = dr["Name"];
                                    athM.Label = dr["Name"];

                                    //MouseDragElementBehavior mde = new MouseDragElementBehavior();
                                    //Interaction.GetBehaviors(athM).Add(mde);
                                    try
                                    {
                                        this.canvasMain.Children.Add(athM);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("控件ID:" + athM.Name + "重复. Error Info:" + ex.Message);
                                    }
                                    athM.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                                    athM.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                                    continue;
                                }
                            }
                            continue;
                        default:
                            break;
                    }
                }
                loadingWindow.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("err:" + table, ex.Message, MessageBoxButton.OK);
            }
            this.SetGridLines(true);
        }

        //鼠标单击主面板事件
        private void canvasMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseMenum();

            //是否拖拽了?
            this.IsDrog = false;
            origPoint = e.GetPosition(canvasMain);

            Glo.IsMouseDown = true;
            delPoint();
            switch (selectType)
            {
                case ToolBox.Mouse:
                    return;
                case ToolBox.Selected:
                    if (e.OriginalSource is Canvas)
                    {
                        RemoveRect();
                        isMultipleSelected = true;
                        isMouseCaptured = false;
                        rect = new Rectangle();
                        origPoint = e.GetPosition(canvasMain);
                        rect.BindDrag();
                        rect.MouseMove += rect_MouseMove;
                        rect.MouseEnter += new MouseEventHandler(rect_MouseEnter);
                        rect.MouseLeave += new MouseEventHandler(rect_MouseLeave);
                        rect.SetValue(Canvas.LeftProperty, origPoint.X);
                        rect.SetValue(Canvas.TopProperty, origPoint.Y);
                        rect.Fill = new SolidColorBrush(Colors.LightGray);
                        rect.Stroke = new SolidColorBrush(Color.FromArgb(255, 36, 93, 219));
                        rect.StrokeThickness = 1;
                        rect.Opacity = 0.3;
                        //移动时出现的图标
                        CustomCursorImage = new Image();
                        CustomCursorImage.Source = new BitmapImage(new Uri("Img/arrow-move.png", UriKind.Relative));
                        CustomCursorImage.Visibility = System.Windows.Visibility.Collapsed;
                        CustomCursorImage.Width = 22;
                        CustomCursorImage.Height = 22;

                        canvasMain.Children.Add(CustomCursorImage);
                        canvasMain.Children.Add(rect);
                        canvasMain.MouseMove += canvasMain_MouseMove;
                    }
                    return;
                case ToolBox.Line:  // 线.
                    try
                    {
                        this.UnSelectAll();
                        Glo.IsMouseDown = true;
                        this.be = false;
                        currLine = new BPLine("Line" + DateTime.Now.ToString("yyMMhhddhhss"), "Black", 2,
                            e.GetPosition(this.canvasMain).X,
                            e.GetPosition(this.canvasMain).Y, e.GetPosition(this.canvasMain).X,
                            e.GetPosition(this.canvasMain).Y);

                        this.selectedElements.Add(currLine);

                        //currLine.Name = ;
                        //currLine.Cursor = Cursors.Hand;
                        //currLine.StrokeThickness = 1;
                        //currLine.Stroke = new SolidColorBrush(Colors.Black);
                        //currline.MyLine.X1 = currline.MyLine.X2 = e.GetPosition(this.canvasMain).X;
                        //currLine.Y1 = currLine.Y2 = e.GetPosition(this.canvasMain).Y;
                        //currLine.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                        //currLine.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);

                        this.canvasMain.Children.Add(currLine);
                        currLine.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                        currLine.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                        this.SetSelectedTool(ToolBox.Line);
                        Glo.currEle = currLine;
                    }
                    catch
                    {
                        currLine = null;
                        Glo.currEle = null;
                        this.SetSelectedTool(ToolBox.Line);
                    }
                    break;
                case ToolBox.Label: /* 标签。 */
                    BPLabel lab = new BPLabel();
                    lab.Width = 100;
                    lab.Height = 23;
                    lab.Cursor = Cursors.Hand;
                    lab.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    lab.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
                    this.canvasMain.Children.Add(lab);
                    MouseDragElementBehavior DragBehavior = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(lab).Add(DragBehavior);
                    this.SetSelectedTool(ToolBox.Mouse);

                    lab.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    lab.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    break;
                case ToolBox.Link: /* Link。 */
                    BPLink link = new BPLink();
                    link.Width = 100;
                    link.Height = 23;
                    link.Cursor = Cursors.Hand;
                    link.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    link.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
                    //Label lab1 = new Label();
                    //lab1.Name = "LinkLab" + link.Name;
                    //lab1.Content = link.Content;
                    // link.Content = lab1;
                    this.canvasMain.Children.Add(link);
                    MouseDragElementBehavior mylinkE = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(link).Add(mylinkE);

                    link.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    link.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.Attachment:  // 附件
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;

                    BPAttachment bpAth = new BPAttachment();
                    bpAth.X = Glo.X;
                    bpAth.Y = Glo.Y;
                    this.winSelectAttachment.BindIt(bpAth);
                    this.winSelectAttachment.Show();

                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.AttachmentM:  // 多附件
                    int numAthM = 1;
                    foreach (UIElement ctl in this.canvasMain.Children)
                    {
                        BPAttachmentM dtl = ctl as BPAttachmentM;
                        if (dtl == null)
                            continue;
                        numAthM++;
                    }

                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;

                    BPAttachmentM myAthM = new BPAttachmentM();
                    myAthM.X = e.GetPosition(this.canvasMain).X;
                    myAthM.Y = e.GetPosition(this.canvasMain).Y;
                    myAthM.Name = "";
                    myAthM.Label = "";
                    myAthM.SaveTo = @"D:\ccflow\trunk\CCFlow\DataUser\UploadFile\";
                    myAthM.IsDelete = true;
                    myAthM.IsDownload = true;
                    myAthM.IsUpload = true;
                    myAthM.Width = 400;
                    myAthM.Height = 100;


                    this.winFrmAttachmentM.BindIt(myAthM);
                    this.winFrmAttachmentM.Show();

                    //  if (this.winFrmAttachmentM.DialogResult == false)
                    //    return;
                    //  myAthM = this.winFrmAttachmentM.HisBPAttachment;
                    //myAthM.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    //myAthM.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
                    //myAthM.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    //myAthM.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    //this.SetSelectedTool(ToolBox.Mouse);
                    //this.canvasMain.Children.Add(myAthM);
                    //Glo.currEle = myAthM;
                    //this.currBPAttachmentM = myAthM;
                    break;
                case ToolBox.TextBox:  // 文本框
                    this.winSelectTB.Show();
                    this.winSelectTB.RB_String.IsChecked = true;

                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.DateCtl:  // 文本框。
                    this.winSelectTB.RB_Data.IsChecked = true;
                    this.winSelectTB.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.Btn:
                    BPBtn btn = new BPBtn();
                    this.winFrmBtn.HisBtn = btn;
                    this.winFrmBtn.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.CheckBox:
                    this.winSelectTB.RB_Boolen.IsChecked = true;
                    this.winSelectTB.CB_IsGenerLabel.IsChecked = false;
                    this.winSelectTB.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.DDLTable:  // DDL。
                    this.winSelectDDL.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.DDLEnum:  // DDL。
                    this.winSelectRB.Show();
                    this.IsRB = false;
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.RBS:  // 复选框。
                    this.winSelectRB.Show();
                    this.IsRB = true;
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.Img:
                    int numImg = 1;
                    foreach (UIElement ctl in this.canvasMain.Children)
                    {
                        BPImg dtl = ctl as BPImg;
                        if (dtl == null)
                            continue;
                        numImg++;
                    }

                    BPImg bpImg = new BPImg();
                    bpImg.Name = Glo.FK_MapData + "Img" + numImg;
                    bpImg.IsSelected = true;
                    bpImg.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    bpImg.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);

                    this.canvasMain.Children.Add(bpImg);

                    //MouseDragElementBehavior mdeAth = new MouseDragElementBehavior();
                    //Interaction.GetBehaviors(bpImg).Add(mdeAth);

                    bpImg.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    bpImg.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                    Glo.currEle = bpImg;
                    this.currImg = bpImg;
                    break;
                case ToolBox.FrmEle:
                    this.winFrmEle.SetBlank();
                    this.winFrmEle.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.Dtl:
                    int num = 1;
                    foreach (UIElement ctl in this.canvasMain.Children)
                    {
                        BPDtl dtl = ctl as BPDtl;
                        if (dtl == null)
                            continue;
                        num++;
                    }

                    BPDtl newDtl = new BPDtl();
                    newDtl.Name = Glo.FK_MapData + "Dtl" + num;
                    newDtl.NewDtl();
                    newDtl.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    newDtl.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
                    this.canvasMain.Children.Add(newDtl);

                    newDtl.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    newDtl.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                    Glo.currEle = newDtl;
                    this.currDtl = newDtl;
                    break;
                case ToolBox.M2M:
                    this.winSelectM2M.IsM2M = 0;
                    this.winSelectM2M.Show();
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.M2MM:
                    Glo.X = e.GetPosition(this.canvasMain).X;
                    Glo.Y = e.GetPosition(this.canvasMain).Y;
                    this.winSelectM2M.IsM2M = 1;
                    this.winSelectM2M.X = Glo.X;
                    this.winSelectM2M.Y = Glo.Y;
                    this.winSelectM2M.Show();
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                case ToolBox.ImgAth:
                    int num1 = 1;
                    foreach (UIElement ctl in this.canvasMain.Children)
                    {
                        BPImgAth dtl = ctl as BPImgAth;
                        if (dtl == null)
                            continue;
                        num1++;
                    }

                    BPImgAth ath = new BPImgAth();
                    ath.Name = Glo.FK_MapData + "ath" + num1;
                    ath.IsSelected = true;
                    ath.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
                    ath.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);

                    this.canvasMain.Children.Add(ath);

                    //MouseDragElementBehavior mdeImgAth = new MouseDragElementBehavior();
                    //Interaction.GetBehaviors(ath).Add(mdeImgAth);

                    ath.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    ath.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    this.SetSelectedTool(ToolBox.Mouse);
                    Glo.currEle = ath;
                    this.currImgAth = ath;
                    break;
                default:
                    MessageBox.Show("功能未完成:" + selectType, "请期待", MessageBoxButton.OK);
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
            }
        }

        void link_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bl = true;
            BPLink link = sender as BPLink;
            this.currLink = link;
            if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300)
            {
                this.winFrmLink.BindIt(this.currLink);
            }
            else
            {
                //  this.linkWin.Visibility = Visibility.Collapsed;
                //   this.gVisable.Visibility = Visibility.Collapsed;
            }
            _lastTime = DateTime.Now;
        }
        public bool IsmuElePanel = false;
        /// <summary>
        /// 点画布上的元素时发生, 根据元素的类型处理响应的操作。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UIElement_Click(object sender, MouseButtonEventArgs e)
        {
            Glo.currEle = sender as UIElement;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                bool isHave = false;
                foreach (UIElement item in this.selectedElements)
                {
                    if (item == Glo.currEle)
                    {
                        isHave = true;
                        break;
                    }
                }
                if (isHave == false)
                {
                    FrameworkElement item = sender as FrameworkElement;
                    if (selectedElements.Contains(item) == false)
                        selectedElements.Add(item);
                }
            }
            else
            {
                this.UnSelectAll();
                selectedElements.Clear();
                FrameworkElement itemE = sender as FrameworkElement;
                this.selectedElements.Add(itemE);
            }

            #region 处理 Btn
            BPBtn btn = sender as BPBtn;
            if (btn != null)
            {
                btn.SetUnSelectedState();
                this.currBtn = btn;
                this.currBtn.IsSelected = true;
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    this.winFrmBtn.HisBtn = btn;
                    this.winFrmBtn.Show();
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 Btn

            #region 处理 Line
            BPLine line = sender as BPLine;
            if (line != null)
            {
                line.SetUnSelectedState();
                this.currLine = line;
                this.line_MouseLeftButtonDown(line, e);
                return;
            }
            #endregion 处理 Line

            #region 处理标签
            BPLabel lab = sender as BPLabel;
            if (lab != null)
            {
                lab.SetUnSelectedState();
                this.currLab = lab;
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    this.winFrmLab.BindIt(this.currLab);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理标签

            #region 处理　Link
            BPLink link = sender as BPLink;
            if (link != null)
            {
                this.currLink = link;
                this.currLink.IsSelected = false;
                currLink.SetUnSelectedState();
                //    if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 500 || IsmuElePanel == true)
                if (IsmuElePanel == true
                    || (DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 500)
                {
                    IsmuElePanel = false;
                    this.winFrmLink.BindIt(this.currLink);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理标签

            #region 处理　textbox .
            string host = Glo.BPMHost + "/WF/MapDef/Do.aspx?DoType=CCForm";
            BPTextBox tb = sender as BPTextBox;
            if (tb != null)
            {
                this.currTB = tb;
                this.currTB.SetUnSelectedState();
                this.currTB.IsSelected = true;
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 400 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    if (tb.NameOfReal == null)
                    {
                        /*说明这是一个复制的文本框，要让用户修改标签与字段名称 */
                        //this.textWin.SetValue(Canvas.TopProperty, labWin.GetValue(Canvas.TopProperty));
                        //this.textWin.SetValue(Canvas.LeftProperty, labWin.GetValue(Canvas.LeftProperty));

                        //this.textWin_Name.Text = "";
                        //this.textWin_Key.Text = "";
                        //this.textWin.Visibility = Visibility.Visible;
                        return;
                    }

                    /* Edit 事件.   */
                    string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + tb.Name + "&DataType=" + tb.HisDataType + "&GroupField=0&LGType=" + LGType.Normal + "&KeyOfEn=" + tb.Name + "&UIContralType=" + CtrlType.TextBox + "&KeyName=" + tb.KeyName + Glo.TimeKey;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:500px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 textbox .

            #region 处理　bpcheckbox .
            BPCheckBox cb = sender as BPCheckBox;
            if (cb != null)
            {
                this.currCB = cb;
                this.currCB.SetUnSelectedState();

                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件.   */
                    string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + cb.Name + "&DataType=" + DataType.AppBoolean + "&GroupField=0&LGType=" + LGType.Normal + "&KeyOfEn=" + cb.Name + "&UIContralType=" + CtrlType.CheckBox + "&KeyName=" + cb.KeyName;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:300px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 textbox .

            #region 处理　BPDatePicker .
            BPDatePicker dp = sender as BPDatePicker;
            if (dp != null)
            {
                this.currDP = dp;
                this.currDP.SetUnSelectedState();

                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件.   */
                    string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + dp.Name + "&DataType=" + dp.HisDateType + "&GroupField=0&LGType=" + LGType.Normal + "&KeyOfEn=" + dp.Name + "&UIContralType=" + CtrlType.TextBox + "&KeyName=" + dp.KeyName;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:300px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 textbox .

            #region 处理　BPDDL .
            BPDDL ddl = sender as BPDDL;
            if (ddl != null)
            {
                this.currDDL = ddl;
                this.currDDL.IsSelected = true;
                this.currDDL.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件. */
                    string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + this.currDDL.Name + "&DataType=" + ddl.HisDataType + "&GroupField=0&LGType=" + ddl.HisLGType + "&KeyOfEn=" + ddl.Name + "&UIBindKey=" + ddl.UIBindKey + "&UIContralType=" + CtrlType.DDL + "&KeyName=" + this.currDDL.KeyName;
                    //MessageBox.Show(url);
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:300px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 BPDDL .

            #region 处理　RadioBtn .
            BPRadioBtn rb = sender as BPRadioBtn;
            if (rb != null)
            {
                this.currRB = rb;
                this.currRB.IsSelected = true;
                this.currRB.SetUnSelectedState();

                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件. */
                    string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + this.currRB.GroupName + "&DataType=" + DataType.AppInt + "&GroupField=0&LGType=" + LGType.Enum + "&KeyOfEn=" + currRB.GroupName + "&UIBindKey=" + currRB.UIBindKey + "&UIContralType=" + CtrlType.RB;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:300px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 RadioBtn .

            #region 处理　BPDtl .
            BPDtl dtl = sender as BPDtl;
            if (dtl != null)
            {
                this.currDtl = dtl;
                this.currDtl.IsSelected = true;
                this.currDtl.SetUnSelectedState();

                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件. */
                    string url = Glo.BPMHost + "/WF/MapDef/MapDefDtlFreeFrm.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&FK_MapDtl=" + dtl.Name;
                    HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
                    // Glo.WinOpenModalDialog(url, 400, 500);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 BPDtl .

            #region 处理　BPM2M .
            BPM2M m2m = sender as BPM2M;
            if (m2m != null)
            {
                this.currM2M = m2m;
                this.currM2M.IsSelected = true;
                this.currM2M.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    /* Edit 事件. */
                    string url = "";
                    url = Glo.BPMHost + "/WF/MapDef/MapM2M.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&NoOfObj=" + m2m.Name + Glo.TimeKey;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:600px;dialogWidth:650px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 BPDtl .

            #region 处理 BPAttachment
            BPAttachment ath = sender as BPAttachment;
            if (ath != null)
            {
                this.currAth = ath;
                Glo.currEle = ath;
                this.currAth.IsSelected = true;
                this.currAth.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    //this.winSelectAttachment.BindIt(ath);
                    string url = Glo.BPMHost + "/WF/MapDef/Attachment.aspx?FK_MapData=" + Glo.FK_MapData + "&Ath=" + ath.Name + Glo.TimeKey;
                    //    HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
                    Glo.WinOpenModalDialog(url, 400, 500);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 BPAttachment

            #region 处理 BPAttachmentM
            BPAttachmentM athm = sender as BPAttachmentM;
            if (athm != null)
            {
                this.currAthM = athm;
                Glo.currEle = athm;
                this.currAthM.IsSelected = true;
                this.currAthM.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    //this.winFrmAttachmentM.BindIt(athm);
                    //this.winFrmAttachmentM.Show();

                    string url = Glo.BPMHost + "/WF/MapDef/Attachment.aspx?FK_MapData=" + Glo.FK_MapData + "&Ath=" + athm.Name + Glo.TimeKey;
                    // HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
                    Glo.WinOpenModalDialog(url, 400, 500);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 BPAttachment

            #region 处理 Img
            BPImg img = sender as BPImg;
            if (img != null)
            {
                this.currImg = img;
                Glo.currEle = img;

                this.currImg.IsSelected = true;
                this.currImg.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    this.winFrmImg.BindIt(img);
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 Img

            #region 处理 Ele
            BPEle ele = sender as BPEle;
            if (ele != null)
            {
                this.currEle = ele;
                Glo.currEle = ele;

                this.currEle.IsSelected = true;
                this.currEle.SetUnSelectedState();
                if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300 || IsmuElePanel == true)
                {
                    IsmuElePanel = false;
                    this.winFrmEle.BindData(ele.Name);
                    this.winFrmEle.Show();
                }
                _lastTime = DateTime.Now;
                return;
            }
            #endregion 处理 Img
        }
        public void UnSelectAll()
        {
            this.selectedElements.Clear();
            foreach (UIElement en in this.canvasMain.Children)
            {
                BPLabel lab = en as BPLabel;
                if (lab != null)
                {
                    lab.IsSelected = false;
                }
                BPRadioBtn btn = en as BPRadioBtn;
                if (btn != null)
                {
                    btn.IsSelected = false;
                }

                BPTextBox tb = en as BPTextBox;
                if (tb != null)
                {
                    tb.IsSelected = false;
                }
                BPLink link = en as BPLink;
                if (link != null)
                {
                    link.IsSelected = false;
                }

                BPDtl dtl = en as BPDtl;
                if (dtl != null)
                {
                    dtl.IsSelected = false;
                }

                BPDDL ddl = en as BPDDL;
                if (ddl != null)
                {
                    ddl.IsSelected = false;
                }

                BPDatePicker dp = en as BPDatePicker;
                if (dp != null)
                {
                    dp.IsSelected = false;
                }

                BPCheckBox cb = en as BPCheckBox;
                if (cb != null)
                {
                    cb.IsSelected = false;
                }
                BPAttachment ath = en as BPAttachment;
                if (ath != null)
                {
                    ath.IsSelected = false;
                }

                BPImg img = en as BPImg;
                if (img != null)
                {
                    img.IsSelected = false;
                }
            }
        }
        /// <summary>
        /// 显示菜单。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UIElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (this.canvasMain.Children.Contains(muElePanel) == false)
                this.canvasMain.Children.Add(muElePanel);

            this.muElePanel.Visibility = System.Windows.Visibility.Visible;
            muElePanel.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X);
            muElePanel.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y);
            Glo.currEle = sender as UIElement;
        }
        void e_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            be = true;
            eCurrent = sender as Ellipse;
            eCurrent.Fill = new SolidColorBrush(Colors.Red);
        }
        void line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BPLine line = sender as BPLine;
            this.currLine = line;
            Glo.currEle = line;
            if (selectType == ToolBox.Mouse)
            {
                e.Handled = true;
                if (!canvasMain.Children.Contains(e1) && !canvasMain.Children.Contains(e2))
                {
                    e1.SetValue(Canvas.LeftProperty, line.MyLine.X1 - 4);
                    e1.SetValue(Canvas.TopProperty, line.MyLine.Y1 - 4);

                    e2.SetValue(Canvas.LeftProperty, line.MyLine.X2 - 4);
                    e2.SetValue(Canvas.TopProperty, line.MyLine.Y2 - 4);

                    this.canvasMain.Children.Add(e1);
                    this.canvasMain.Children.Add(e2);

                    //Interaction.GetBehaviors(e1).Add(myLineMDE);
                    //Interaction.GetBehaviors(e2).Add(myLineMDE);
                }
                else
                {
                    e1.SetValue(Canvas.LeftProperty, line.MyLine.X1 - 4);
                    e1.SetValue(Canvas.TopProperty, line.MyLine.Y1 - 4);

                    e2.SetValue(Canvas.LeftProperty, line.MyLine.X2 - 4);
                    e2.SetValue(Canvas.TopProperty, line.MyLine.Y2 - 4);
                }
            }

            if ((DateTime.Now.Subtract(_lastTime).TotalMilliseconds) < 300)
            {
                ///* 双击事件 */
                //this.lineWin.Visibility = Visibility.Visible;
                //this.gVisable.Visibility = Visibility.Visible;
                //this.lineWin.SetValue(Canvas.TopProperty, line.GetValue(Canvas.TopProperty));
                //this.lineWin.SetValue(Canvas.LeftProperty, line.GetValue(Canvas.LeftProperty));
            }
            else
            {
                e1.SetValue(Canvas.LeftProperty, line.MyLine.X1 - 4);
                e1.SetValue(Canvas.TopProperty, line.MyLine.Y1 - 4);

                e2.SetValue(Canvas.LeftProperty, line.MyLine.X2 - 4);
                e2.SetValue(Canvas.TopProperty, line.MyLine.Y2 - 4);
            }
            _lastTime = DateTime.Now;
        }
        /// <summary>
        /// 执行复制.
        /// </summary>
        private void DoCopy()
        {
            if (this.IsCopy == false)
                return;

            List<FrameworkElement> copyEles = new List<FrameworkElement>();
            string timeKey = DateTime.Now.ToString("yyMMddhhmmss");
            int idx = 0;
            string name = "";
            foreach (FrameworkElement item in this.selectedElements)
            {
                idx++;
                BPLine line = item as BPLine;
                if (line != null)
                {
                    name = "Line" + timeKey + "_" + idx.ToString();

                    BPLine lineN = new BPLine(name, line.Color, line.MyLine.StrokeThickness,
                        line.MyLine.X1 + 10, line.MyLine.Y1 + 10, line.MyLine.X2 + 10, line.MyLine.Y2 + 10);

                    this.canvasMain.Children.Add(lineN);
                    lineN.IsSelected = true;
                    copyEles.Add(lineN);

                    MouseDragElementBehavior mdeLine = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(lineN).Add(mdeLine);
                    lineN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    lineN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                }

                BPLabel lab = item as BPLabel;
                if (lab != null)
                {
                    name = "Lab" + timeKey + "_" + idx.ToString();
                    BPLabel labN = new BPLabel();
                    labN.Name = name;
                    labN.Content = lab.Content;
                    labN.SetValue(Canvas.LeftProperty, (double)lab.GetValue(Canvas.LeftProperty) + 16);
                    labN.SetValue(Canvas.TopProperty, (double)lab.GetValue(Canvas.TopProperty) + 16);
                    labN.Foreground = lab.Foreground;
                    labN.FontSize = lab.FontSize;
                    labN.FontWeight = lab.FontWeight;
                    this.canvasMain.Children.Add(labN);

                    MouseDragElementBehavior mdeLine = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(labN).Add(mdeLine);
                    labN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    labN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                    labN.IsSelected = true;
                    copyEles.Add(labN);
                }

                BPLink link = item as BPLink;
                if (link != null)
                {
                    name = "Link" + timeKey + "_" + idx.ToString();
                    BPLink labN = new BPLink();
                    labN.Name = name;
                    labN.Content = link.Content;
                    labN.SetValue(Canvas.LeftProperty, (double)link.GetValue(Canvas.LeftProperty) + 16);
                    labN.SetValue(Canvas.TopProperty, (double)link.GetValue(Canvas.TopProperty) + 16);
                    labN.Foreground = link.Foreground;
                    labN.FontSize = link.FontSize;
                    labN.FontWeight = link.FontWeight;
                    this.canvasMain.Children.Add(labN);

                    MouseDragElementBehavior mdeLine = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(labN).Add(mdeLine);
                    labN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    labN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    labN.IsSelected = true;
                    copyEles.Add(labN);
                }


                BPTextBox tb = item as BPTextBox;
                if (tb != null && tb == null)//是条件永不成立，textbox复制暂时取消。
                {
                    name = "TB" + timeKey + "_" + idx.ToString();
                    BPTextBox tbN = new BPTextBox();
                    tbN.Name = name;
                    tbN.SetValue(Canvas.LeftProperty, (double)tb.GetValue(Canvas.LeftProperty) + 16);
                    tbN.SetValue(Canvas.TopProperty, (double)tb.GetValue(Canvas.TopProperty) + 16);
                    tbN.Width = tb.Width;
                    tbN.Height = tb.Height;
                    // tbN.HisDataType = tb.HisDataType;
                    tbN.HisTBType = tb.HisTBType;
                    tbN.TextAlignment = tb.TextAlignment;
                    tbN.Text = tb.Text;
                    tbN.Background = new SolidColorBrush(Colors.Orange);
                    tbN.IsReadOnly = true;
                    this.canvasMain.Children.Add(tbN);

                    MouseDragElementBehavior mdeLine = new MouseDragElementBehavior();
                    Interaction.GetBehaviors(tbN).Add(mdeLine);
                    tbN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    tbN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);
                    tbN.IsSelected = true;
                    copyEles.Add(tbN);
                }

                BPImg img = item as BPImg;
                if (img != null)
                {
                    MessageBox.Show("装饰图片复制功能未完成.");
                    return;

                    //  name = "Img" + timeKey + "_" + idx.ToString();
                    //  BPImg labN = new BPImg();
                    //  labN.Name = name;
                    ////  labN.Content = lab.Content;
                    //  labN.SetValue(Canvas.LeftProperty, (double)lab.GetValue(Canvas.LeftProperty) + 16);
                    //  labN.SetValue(Canvas.TopProperty, (double)lab.GetValue(Canvas.TopProperty) + 16);
                    //  labN.Foreground = lab.Foreground;
                    //  this.canvasMain.Children.Add(labN);

                    //  MouseDragElementBehavior mdeLine = new MouseDragElementBehavior();
                    //  Interaction.GetBehaviors(labN).Add(mdeLine);
                    //  labN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                    //  labN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                    //  labN.IsSelected = true;
                    //  copyEles.Add(labN);
                }
            }
            this.UnSelectAll();
            this.selectedElements = copyEles;
            foreach (FrameworkElement item in this.selectedElements)
            {
                BPLine line = item as BPLine;
                if (line != null)
                    line.IsSelected = true;

                BPLabel lab = item as BPLabel;
                if (lab != null)
                    lab.IsSelected = true;
            }
            this.IsCopy = true;
        }
        private bool IsDrog = false;
        //鼠标松开主面板事件
        private void canvasMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Glo.IsMouseDown = false;

            #region 如果是复制.
            if (Keyboard.Modifiers == ModifierKeys.Control && IsDrog == true)
            {
                this.DoCopy();
                return;
            }
            #endregion 如果是复制.

            #region 矩形选择
            if (isMultipleSelected)
            {
                this.selectedElements.Clear();
                SelectUIElement();
                canvasMain.MouseMove -= canvasMain_MouseMove;
                isMultipleSelected = false;
            }
            else if (!(e.OriginalSource is Rectangle))
            {
                RemoveRect();
            }
            #endregion


            Glo.IsMouseDown = false;
            be = false;
            bl = false;
            btxt = false;
            if (this.selectType != ToolBox.Line && this.selectType != ToolBox.Selected)
                this.SetSelectedTool(ToolBox.Mouse);

            // 当前选择的点.
            if (eCurrent != null)
            {
                eCurrent.Fill = new SolidColorBrush(Colors.Green);
            }
        }
        //鼠标在主面板上移动事件
        private void canvasMain_MouseMove(object sender, MouseEventArgs e)
        {
            //   this.canvasMain_MouseRightButtonDown(null, null);

            #region 如果是复制.
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                IsDrog = true;
            }
            #endregion 如果是复制.

            #region 画线
            if (Glo.IsMouseDown && this.currLine != null && this.selectType == ToolBox.Line)
            {
                currLine.MyLine.X2 = e.GetPosition(this.canvasMain).X;
                currLine.MyLine.Y2 = e.GetPosition(this.canvasMain).Y;
                double x = currLine.MyLine.X1 - currLine.MyLine.X2;
                double y = currLine.MyLine.Y1 - currLine.MyLine.Y2;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    /*是横线 */
                    currLine.MyLine.Y2 = currLine.MyLine.Y1;
                }
                else
                {
                    currLine.MyLine.X2 = currLine.MyLine.X1;
                }
                return;
            }
            #endregion 画线

            #region 改变线的长度
            if (selectType == ToolBox.Mouse && be == true)
            {
                if (eCurrent.Tag.ToString() == "e1")
                {
                    double x = e.GetPosition(this.canvasMain).X - currLine.MyLine.X2;
                    double y = e.GetPosition(this.canvasMain).Y - currLine.MyLine.Y2;
                    if (Math.Abs(x) > Math.Abs(y))
                    {
                        currLine.MyLine.X1 = e.GetPosition(this.canvasMain).X;
                        currLine.MyLine.Y1 = currLine.MyLine.Y2;
                        eCurrent.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X - 4);
                        eCurrent.SetValue(Canvas.TopProperty, currLine.MyLine.Y2 - 4);
                    }
                    else
                    {
                        currLine.MyLine.X1 = currLine.MyLine.X2;
                        currLine.MyLine.Y1 = e.GetPosition(this.canvasMain).Y;
                        eCurrent.SetValue(Canvas.LeftProperty, currLine.MyLine.X2 - 4);
                        eCurrent.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y - 4);
                    }
                }
                else //if (eCurrent.Tag.ToString() == "e2")
                {
                    double x = e.GetPosition(this.canvasMain).X - currLine.MyLine.X1;
                    double y = e.GetPosition(this.canvasMain).Y - currLine.MyLine.Y1;
                    if (Math.Abs(x) > Math.Abs(y))
                    {
                        currLine.MyLine.X2 = e.GetPosition(this.canvasMain).X;
                        currLine.MyLine.Y2 = currLine.MyLine.Y1;
                        eCurrent.SetValue(Canvas.LeftProperty, e.GetPosition(this.canvasMain).X - 4);
                        eCurrent.SetValue(Canvas.TopProperty, currLine.MyLine.Y1 - 4);
                    }
                    else
                    {
                        currLine.MyLine.X2 = currLine.MyLine.X1;
                        currLine.MyLine.Y2 = e.GetPosition(this.canvasMain).Y;
                        eCurrent.SetValue(Canvas.LeftProperty, currLine.MyLine.X1 - 4);
                        eCurrent.SetValue(Canvas.TopProperty, e.GetPosition(this.canvasMain).Y - 4);
                    }
                }
                return;
            }
            #endregion

            #region 处理 矩形选择.
            if (isMultipleSelected && selectType == ToolBox.Selected) /*  更新rect 的大小  */
            {
                Point curPoint = e.GetPosition(canvasMain);
                if (curPoint.X > origPoint.X)
                {
                    rect.Width = curPoint.X - origPoint.X;
                }
                if (curPoint.X < origPoint.X)
                {
                    rect.SetValue(Canvas.LeftProperty, curPoint.X);
                    rect.Width = origPoint.X - curPoint.X;
                }
                if (curPoint.Y > origPoint.Y)
                {
                    rect.Height = curPoint.Y - origPoint.Y;
                }
                if (curPoint.Y < origPoint.Y)
                {
                    rect.SetValue(Canvas.TopProperty, curPoint.Y);
                    rect.Height = origPoint.Y - curPoint.Y;
                }
            }
            #endregion 处理 矩形选择.

            //if (Glo.IsMouseDown)
            //{
            //    double deltaV = e.GetPosition(canvasMain).Y - origPoint.Y;
            //    double deltaH = e.GetPosition(canvasMain).X - origPoint.X;
            //    foreach (var se in selectedElements)
            //    {
            //        double cLeft = deltaH + (double)se.GetValue(Canvas.LeftProperty);
            //        double cTop = deltaV + (double)se.GetValue(Canvas.TopProperty);
            //        se.SetValue(Canvas.LeftProperty, cLeft);
            //        se.SetValue(Canvas.TopProperty, cTop);
            //    }
            //    origPoint.X = e.GetPosition(canvasMain).X;
            //    origPoint.Y = e.GetPosition(canvasMain).Y;
            //}
        }
        /// <summary>
        /// 工具选择触发事件.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toolbox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseMenum();

            string id = "";
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                id = tb.Name.Replace("Btn_", "");
                if (id == "")
                    id = tb.Tag.ToString().Replace("Btn_", "");
            }

            Image img = sender as Image;
            if (img != null)
            {
                id = img.Tag.ToString().Replace("Btn_", "");
            }

            selectType = id;
            switch (id)
            {
                case ToolBox.RBS:
                    break;
                case ToolBox.HiddenField:
                    this.SetSelectedTool(ToolBox.Mouse);
                    FrmHiddenField fm = new FrmHiddenField();
                    fm.Show();
                    return;
                default:
                    break;
            }
            this.SetSelectedTool(id);
        }
        /// <summary>
        /// 设置选择的ToolBox.
        /// </summary>
        /// <param name="id"></param>
        public void SetSelectedTool(string id)
        {
            this.selectType = id;
            if (this.selectType == ToolBox.Line)
            {
                this.canvasMain.Cursor = Cursors.Eraser;
            }
            else
            {
#warning 如何解决鼠标的样式改变？？？
                // System.Windows.Input.Cursor cc = new System.Windows.Input.Cursor();
                //  cc.
                this.canvasMain.Cursor = Cursors.Arrow;
            }
        }
        private void HidCurrSelectUI()
        {
            if (MessageBox.Show("您确定要隐藏选择的元素吗？", "执行确认", MessageBoxButton.OKCancel)
                == MessageBoxResult.No)
                return;

            BPRadioBtn rb = Glo.currEle as BPRadioBtn;
            if (rb != null)
            {
                rb.DoDeleteIt();
                return;
            }

            if (this.canvasMain.Children.Contains(Glo.currEle))
            {
                BPTextBox tb = Glo.currEle as BPTextBox;
                if (tb != null)
                {
                    tb.HidIt();
                    return;
                }

                BPDDL ddl = Glo.currEle as BPDDL;
                if (ddl != null)
                {
                    ddl.HidIt();
                    return;
                }

                BPDtl dtl = Glo.currEle as BPDtl;
                if (dtl != null)
                {
                    dtl.DeleteIt();
                    return;
                }

                BPM2M m2m = Glo.currEle as BPM2M;
                if (m2m != null)
                {
                    m2m.DeleteIt();
                    return;
                }

                MessageBox.Show("您选择的元素不支持隐藏", "执行错误", MessageBoxButton.OK);
                return;
                //this.canvasMain.Children.Remove(Glo.currEle);
            }
            Glo.currEle = null;
        }
        private void DeleteCurrSelectUI()
        {
            BPRadioBtn rb = Glo.currEle as BPRadioBtn;
            if (rb != null)
            {
                rb.DoDeleteIt();
                return;
            }

            if (this.canvasMain.Children.Contains(Glo.currEle))
            {
                BPTextBox tb = Glo.currEle as BPTextBox;
                if (tb != null)
                {
                    tb.DeleteIt();
                    return;
                }

                BPCheckBox cb = Glo.currEle as BPCheckBox;
                if (cb != null)
                {
                    cb.DeleteIt();
                    return;
                }

                BPDDL ddl = Glo.currEle as BPDDL;
                if (ddl != null)
                {
                    ddl.DeleteIt();
                    return;
                }

                BPDtl dtl = Glo.currEle as BPDtl;
                if (dtl != null)
                {
                    dtl.DeleteIt();
                    return;
                }

                BPM2M m2m = Glo.currEle as BPM2M;
                if (m2m != null)
                {
                    m2m.DeleteIt();
                    return;
                }

                this.canvasMain.Children.Remove(Glo.currEle);
            }
            Glo.currEle = null;
        }
        public void DoFunc(string funcID)
        {

        }
        private void UI_muElePanel_Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseMenum();
            Liquid.MenuItem tb = sender as Liquid.MenuItem;
            switch (tb.Name)
            {
                case "FrmTempleteShareIt": //共享模板.
                    FrmShare fss = new FrmShare();
                    fss.Show();
                    break;
                case "AdvAction": //事件.
                    FrmEvent fa = new FrmEvent();
                    fa.Show();
                    break;
                case "AdvUAC": //访问控制.
                    FrmUAC fuac = new FrmUAC();
                    fuac.Show();
                    break;
                case "GradeLine":
                case "GradeLine_Ext":
                    this.SetGridLines(false);
                    break;
                case "FullScreen": //全屏
                case "FullScreen_Ext": //全屏
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case "FrmTempleteShare":
                    FrmImpFromInternet impFrmI = new FrmImpFromInternet();
                    impFrmI.HisMainPage = this;
                    impFrmI.Show();
                    break;
                case "FrmTempleteExp": //导出表单模版.
                case "FrmTempleteExp_Ext": //导出表单模版.
                    //Glo.WinOpen(Glo.BPMHost + "/WF/MapDef/Handler.ashx?DoType=DownTempFrm&FK_MapData=" + Glo.FK_MapData);
                    Glo.WinOpen(Glo.BPMHost + "/WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + Glo.FK_MapData + Glo.TimeKey,
                        100, 100);
                    return;
                case "FrmTempleteImp": //导入表单模版
                case "FrmTempleteImp_Ext": //导入表单模版
                    winFrmImp.HisMainPage = this;
                    winFrmImp.Show();
                    break;
                case "eleDel":
                    this.DeleteCurrSelectUI();
                    break;
                case "eleCopyTo": //复制到其它表单
                    FrmCopyEleTo copyIt = new FrmCopyEleTo();
                    copyIt.Tag = sender;
                    copyIt.Show();
                    break;
                case "eleHid":
                    this.HidCurrSelectUI();
                    break;
                case "eleCancel":
                    break;
                case "eleDtlFrm":
                    BPDtl dtlFrm = Glo.currEle as BPDtl;
                    if (dtlFrm != null)
                    {
                        this.currDtl = dtlFrm;
                        //  string url = Glo.BPMHost + "/WF/MapDef/MapDef.aspx?DoType=Edit&FK_MapData=" + dtlFrm.Name;
                        string url = Glo.BPMHost + "/WF/MapDef/CCForm/Frm.aspx?FK_MapData=" + dtlFrm.Name + Glo.TimeKey + Glo.TimeKey;
                        HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("当前选中的元素不是明细表", "提示", MessageBoxButton.OK);
                    }
                    break;
                case "eleTabIdx":
                case "eleTabIdx_Ext":
                    string url1 = Glo.BPMHost + "/WF/MapDef/TabIdx.aspx?FK_MapData=" + Glo.FK_MapData + Glo.TimeKey;
                    HtmlPage.Window.Eval("window.showModalDialog('" + url1 + "',window,'dialogHeight:500px;dialogWidth:700px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                    return;
                case "eleEdit":
                    Line line = Glo.currEle as Line;
                    if (line != null)
                    {
                        this.IsmuElePanel = true;
                        line_MouseLeftButtonDown(line, e);
                        return;
                    }
                    BPLabel lab = Glo.currEle as BPLabel;
                    if (lab != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(lab, e);
                    }

                    BPBtn btn = Glo.currEle as BPBtn;
                    if (btn != null)
                    {
                        this.IsmuElePanel = true;
                        this.winFrmBtn.HisBtn = btn;
                        this.winFrmBtn.Show();
                        return;
                        //UIElement_Click(lab, e);
                    }

                    BPLink link = Glo.currEle as BPLink;
                    if (link != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(link, e);
                    }

                    BPTextBox textb = Glo.currEle as BPTextBox;
                    if (textb != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(textb, e);
                    }

                    BPDDL ddl = Glo.currEle as BPDDL;
                    if (ddl != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(ddl, e);
                    }

                    BPCheckBox cb = Glo.currEle as BPCheckBox;
                    if (cb != null)
                    {
                        this.currCB = cb;
                        this.IsmuElePanel = true;
                        UIElement_Click(cb, e);
                    }

                    BPRadioBtn rb = Glo.currEle as BPRadioBtn;
                    if (rb != null)
                    {
                        this.currRB = rb;
                        this.IsmuElePanel = true;
                        UIElement_Click(rb, e);
                    }

                    BPDtl dtl = Glo.currEle as BPDtl;
                    if (dtl != null)
                    {
                        this.currDtl = dtl;
                        this.IsmuElePanel = true;
                        UIElement_Click(dtl, e);
                    }

                    BPM2M m2m = Glo.currEle as BPM2M;
                    if (m2m != null)
                    {
                        this.currM2M = m2m;
                        this.IsmuElePanel = true;
                        UIElement_Click(m2m, e);
                    }

                    BPImg bpImg = Glo.currEle as BPImg;
                    if (bpImg != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(bpImg, e);
                    }

                    BPEle bpEle = Glo.currEle as BPEle;
                    if (bpEle != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(bpEle, e);
                    }

                    BPImgAth bpImgAth = Glo.currEle as BPImgAth;
                    if (bpImgAth != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(bpImgAth, e);
                    }

                    BPAttachment ath = Glo.currEle as BPAttachment;
                    if (ath != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(ath, e);
                    }

                    BPAttachmentM athM = Glo.currEle as BPAttachmentM;
                    if (athM != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(athM, e);
                    }


                    Image img = Glo.currEle as Image;
                    if (img != null)
                    {
                        this.IsmuElePanel = true;
                        UIElement_Click(img, e);
                    }
                    break;
                default:
                    MessageBox.Show(tb.Text + " 功能未完成.", "敬请期待", MessageBoxButton.OK);
                    break;
            }
        }

        #region 私有方法
        //删除主面板上线上的点
        private void delPoint()
        {
            if (canvasMain.Children.Contains(e1))
                this.canvasMain.Children.Remove(e1);

            if (canvasMain.Children.Contains(e2))
                this.canvasMain.Children.Remove(e2);
        }
        #endregion

        /// <summary>
        /// 地图截图名称
        /// </summary>
        string mapImageName;
        /// <summary>
        /// 地图截图文件的文件流
        /// </summary>
        Stream mapImageStream;
        FileInfo fileInfoOfMapImage;
        public bool IsCopy = false;
        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            this.CloseMenum();
            string id = "";
            Button btn = sender as Button;
            if (btn == null)
            {
                Toolbar.ToolbarButton mybtn = sender as Toolbar.ToolbarButton;
                id = mybtn.Name;
            }
            else
            {
                id = btn.Name;
            }
            switch (id)
            {
                case "Btn_" + EleFunc.SelectAll:
                    #region  处理
                    this.selectedElements.Clear();
                    foreach (FrameworkElement item in this.canvasMain.Children)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                            lab.IsSelected = true;

                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            line.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPLink link = item as BPLink;
                        if (link != null)
                        {
                            link.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPRadioBtn rb = item as BPRadioBtn;
                        if (rb != null)
                        {
                            rb.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPTextBox tb = item as BPTextBox;
                        if (tb != null)
                        {
                            tb.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPDDL ddl = item as BPDDL;
                        if (ddl != null)
                        {
                            ddl.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPDtl dtl = item as BPDtl;
                        if (dtl != null)
                        {
                            dtl.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPM2M m2m = item as BPM2M;
                        if (m2m != null)
                        {
                            m2m.IsSelected = true;
                            this.selectedElements.Add(item);
                        }

                        BPImg img = item as BPImg;
                        if (img != null)
                        {
                            img.IsSelected = true;
                            this.selectedElements.Add(item);
                        }
                    }
                    this.IsCopy = true;
                    #endregion  处理 selectall
                    break;
                case "Btn_" + EleFunc.CopyEle:
                    this.IsCopy = true;
                    break;
                case "Btn_" + EleFunc.Paste:
                    this.DoCopy();
                    break;
                case "Btn_Property":
                    this.winFrmOp.Show();
                    break;
                case "Btn_Alignment_Down"://底端对齐
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }

                    double maxY = 0;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        double dY = Canvas.GetTop(item) + item.ActualHeight;
                        if (maxY < dY)
                        {
                            maxY = dY;
                        }
                    }

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.TopProperty, maxY - item.ActualHeight);
                    }
                    break;
                case "Btn_Alignment_Top":
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double minY = 1000;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        //MatrixTransform transform = item.TransformToVisual(this.canvasMain) as MatrixTransform;
                        //double y = transform.Matrix.OffsetY;
                        double dY = Canvas.GetTop(item);
                        if (minY > dY)
                            minY = dY;
                    }
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.TopProperty, minY);
                    }
                    break;
                case "Btn_Alignment_Left":
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }

                    double minX = 1000;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.canvasMain) as MatrixTransform;
                        double x = transform.Matrix.OffsetX;
                        if (x <= 0)
                            continue;

                        if (minX > x)
                            minX = x;
                    }

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.LeftProperty, minX);
                    }
                    break;
                case "Btn_Alignment_Right":
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double maxX = 0;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.canvasMain) as MatrixTransform;
                        double x = transform.Matrix.OffsetX + item.ActualWidth;
                        if (maxX < x)
                        {
                            maxX = x;
                        }
                    }
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.canvasMain) as MatrixTransform;
                        item.SetValue(Canvas.LeftProperty, maxX - item.ActualWidth);
                    }
                    break;
                case "Btn_Alignment_Center":
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double miX = 1000, maX = 0; /* 求最大， 最小*/
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.canvasMain) as MatrixTransform;
                        double x = transform.Matrix.OffsetX + item.ActualWidth;
                        if (maX < x)
                            maX = x;
                        if (miX > transform.Matrix.OffsetX)
                            miX = x;
                    }
                    double miudeLine = (maX - miX) / 2 + miX;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.LeftProperty, miudeLine - item.ActualWidth / 2);
                    }
                    break;
                case "Btn_Save":
                    Save_Click(sender, e);
                    break;
                case "Btn_Copy":
                    this.winFrmImp.Show();
                    break;
                case "Btn_View":
                    //if (5 == 5)
                    //{
                    //    string url1 = null;
                    //    if (Glo.IsDtlFrm == false)
                    //        url1 = Glo.BPMHost + "/WF/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node;
                    //    else
                    //        url1 = Glo.BPMHost + "/WF/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&RefPKVal=0&OID=0";
                    //    HtmlPage.Window.Eval("window.open('" + url1 + "','_blank','scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes,width=" + Glo.HisMapData.FrmW + ",height=" + Glo.HisMapData.FrmH + "')");
                    //}
                    //else
                    //{

                    string url1 = null;
                    if (Glo.IsDtlFrm == false)
                        url1 = Glo.BPMHost + "/WF/CCForm/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node + Glo.TimeKey;
                    else
                        url1 = Glo.BPMHost + "/WF/CCForm/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&RefPKVal=0&OID=0" + Glo.TimeKey;
                    HtmlPage.Window.Eval("window.open('" + url1 + "','_blank','scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes,width=" + Glo.HisMapData.FrmW + ",height=" + Glo.HisMapData.FrmH + "')");

                    //}

                    break;
                case "Btn_Glo":
                    MessageBox.Show(Glo.currEle.ToString());
                    break;
                case "Btn_Impdddd": // del.
                    this.UI_muElePanel_Btn_MouseLeftButtonDown(null, null);
                    OpenFileDialog myOpenFileDialog = new OpenFileDialog();
                    myOpenFileDialog.Filter = "驰骋工作流程表单模板(*.xml)|*.xml|All Files (*.*)|*.*";  //SL目前只支持jpg和png格式图像的显示
                    myOpenFileDialog.Multiselect = false;//只允许选择一个图片
                    if (myOpenFileDialog.ShowDialog() == false)
                        return;

                    mapImageName = myOpenFileDialog.File.Name.ToString();
                    //获得图片的流信息，并与image控件绑定
                    FileInfo aFileInfo = myOpenFileDialog.File;
                    fileInfoOfMapImage = myOpenFileDialog.File;

                    if (aFileInfo == null)
                        return;
                    mapImageStream = aFileInfo.OpenRead();
                    //上传图片
                    mapImageStream.Position = 0;
                    byte[] buffer = new byte[mapImageStream.Length + 1];
                    mapImageStream.Read(buffer, 0, buffer.Length);
                    String fileName = fileInfoOfMapImage.Name;
                    FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                    da.UploadFileAsync(buffer, "\\Temp\\s.xml");
                    da.UploadFileCompleted += new EventHandler<FF.UploadFileCompletedEventArgs>(da_UploadFileCompleted);
                    break;
                case "Btn_Exp":
                    //Glo.WinOpen(Glo.BPMHost + "/WF/MapDef/Handler.ashx?DoType=DownTempFrm&FK_MapData=" + Glo.FK_MapData);
                    Glo.WinOpen(Glo.BPMHost + "/WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + Glo.FK_MapData + Glo.TimeKey,
                        100, 100);
                    if (MessageBox.Show("已经开始执行导出了，如果您的浏览器不能正常被下载，请点确定按钮直接下载模版。", "您看到导出的文件了吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        Glo.WinOpen(Glo.BPMHost + "/DataUser/Temp/" + Glo.FK_MapData + ".xml", 100, 200);
                    return;
                case "Btn_Imp":
                    winFrmImp.HisMainPage = this;
                    winFrmImp.Show();
                    break;
                case "Btn_Delete":
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("您没有选择删除的对象，提示:按下ctrl可实现多选。", "批量删除提示", MessageBoxButton.OK);
                        return;
                    }
                    string alter = "共有 (" + this.selectedElements.Count + ") 个对象被选择，您确定要删除它们吗？\n\n提示:按下ctrl可实现多选。";
                    if (MessageBox.Show(alter, "批量删除提示", MessageBoxButton.OK) == MessageBoxResult.Cancel)
                        return;

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPRadioBtn rb = item as BPRadioBtn;
                        if (rb != null && rb.IsSelected)
                            this.canvasMain.Children.Remove(item);

                        BPLine line = item as BPLine;
                        if (line != null && line.IsSelected)
                            this.canvasMain.Children.Remove(item);

                        BPImg img = item as BPImg;
                        if (img != null && img.IsSelected)
                            this.canvasMain.Children.Remove(item);

                        BPAttachment ath = item as BPAttachment;
                        if (ath != null && ath.IsSelected)
                            this.canvasMain.Children.Remove(item);

                        BPCheckBox cb = item as BPCheckBox;
                        if (cb != null && cb.IsSelected)
                            this.canvasMain.Children.Remove(item);

                        BPDatePicker dp = item as BPDatePicker;
                        if (dp != null && dp.IsSelected)
                            this.canvasMain.Children.Remove(dp);

                        BPDDL ddl = item as BPDDL;
                        if (ddl != null && ddl.IsSelected)
                            this.canvasMain.Children.Remove(ddl);

                        BPDtl dtl = item as BPDtl;
                        if (dtl != null && dtl.IsSelected)
                            dtl.DeleteIt();

                        BPLabel lab = item as BPLabel;
                        if (lab != null && lab.IsSelected)
                            this.canvasMain.Children.Remove(lab);

                        BPLink link = item as BPLink;
                        if (link != null && link.IsSelected)
                            this.canvasMain.Children.Remove(link);

                        BPTextBox tb = item as BPTextBox;
                        if (tb != null && tb.IsSelected)
                            tb.DeleteIt();

                        BPBtn mybtn = item as BPBtn;
                        if (mybtn != null && mybtn.IsSelected)
                            this.canvasMain.Children.Remove(mybtn);
                    }

                    if (Glo.currEle != null)
                    {
                        if (this.canvasMain.Children.Contains(Glo.currEle))
                        {
                            this.canvasMain.Children.Remove(Glo.currEle);
                            Glo.currEle = null;
                            this.delPoint();
                        }
                    }
                    this.selectedElements.Clear();
                    break;
                case "Btn_FontSizeAdd":
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null && lab.IsSelected)
                        {
                            lab.FontSize = lab.FontSize + 1;
                        }

                        BPLink link = item as BPLink;
                        if (link != null && link.IsSelected)
                        {
                            link.FontSize = link.FontSize + 1;
                        }

                        BPLine line = item as BPLine;
                        if (line != null && line.IsSelected)
                        {
                            line.MyLine.StrokeThickness = line.MyLine.StrokeThickness + 1;
                        }
                    }
                    break;
                case "Btn_FontSizeCut":
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null && lab.IsSelected)
                        {
                            if (lab.FontSize < 8)
                                continue;

                            lab.FontSize = Math.Abs(lab.FontSize - 1);
                        }

                        BPLink link = item as BPLink;
                        if (link != null && link.IsSelected)
                        {
                            if (link.FontSize < 8)
                                continue;

                            link.FontSize = Math.Abs(link.FontSize - 1);
                        }

                        BPLine line = item as BPLine;
                        if (line != null && line.IsSelected)
                        {
                            if (line.MyLine.StrokeThickness < 0.5)
                                continue;
                            line.MyLine.StrokeThickness = Math.Abs(line.MyLine.StrokeThickness - 1);
                        }
                    }
                    break;
                case "Btn_Colorpicker":
                    ColorPickerWin cw = new ColorPickerWin();
                    cw._selectedColor += new ColorPickerWin.SelectedColor(SelectedColor);
                    cw.Show();
                    break;
                case "Btn_Bold":
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            if (lab.FontWeight == FontWeights.Bold)
                                lab.FontWeight = FontWeights.Normal;
                            else
                                lab.FontWeight = FontWeights.Bold;
                        }

                        BPLink link = item as BPLink;
                        if (link != null)
                        {
                            if (link.FontWeight == FontWeights.Bold)
                                link.FontWeight = FontWeights.Normal;
                            else
                                link.FontWeight = FontWeights.Bold;
                        }
                    }
                    break;
                default:
                    MessageBox.Show(sender.ToString() + " ID=" + id + " 功能未实现.");
                    break;
            }
        }

        void SelectedColor(Color strColor)
        {
            foreach (FrameworkElement item in this.selectedElements)
            {
                BPLabel lab = item as BPLabel;
                if (lab != null)
                    lab.Foreground = new SolidColorBrush(strColor);

                BPLink link = item as BPLink;
                if (link != null)
                    link.Foreground = new SolidColorBrush(strColor);

                BPLine line = item as BPLine;
                if (line != null)
                    line.MyLine.Stroke = new SolidColorBrush(strColor);
            }
        }
        void da_UploadFileCompleted(object sender, FF.UploadFileCompletedEventArgs e)
        {
            this.BindFrm(e.Result);
            MessageBox.Show("模板装载成功..");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.SetSelectedTool(ToolBox.Mouse);

            #region mapData
            DataTable mapDataDT = new DataTable();
            mapDataDT.TableName = "Sys_MapData";
            mapDataDT.Columns.Add(new DataColumn("No", typeof(string)));
            mapDataDT.Columns.Add(new DataColumn("FrmW", typeof(double)));
            mapDataDT.Columns.Add(new DataColumn("FrmH", typeof(double)));

            DataRow drMapDR = mapDataDT.NewRow();
            drMapDR["No"] = Glo.FK_MapData;
            drMapDR["FrmW"] = Glo.HisMapData.FrmW.ToString("0.00");
            drMapDR["FrmH"] = Glo.HisMapData.FrmH.ToString("0.00");
            mapDataDT.Rows.Add(drMapDR);
            #endregion mapData

            #region line
            DataTable lineDT = new DataTable();
            lineDT.TableName = "Sys_FrmLine";
            lineDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            lineDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));

            lineDT.Columns.Add(new DataColumn("X", typeof(double)));
            lineDT.Columns.Add(new DataColumn("Y", typeof(double)));

            lineDT.Columns.Add(new DataColumn("X1", typeof(double)));
            lineDT.Columns.Add(new DataColumn("Y1", typeof(double)));

            lineDT.Columns.Add(new DataColumn("X2", typeof(double)));
            lineDT.Columns.Add(new DataColumn("Y2", typeof(double)));

            lineDT.Columns.Add(new DataColumn("BorderWidth", typeof(string)));
            lineDT.Columns.Add(new DataColumn("BorderColor", typeof(string)));
            // lineDT.Columns.Add(new DataColumn("BorderStyle", typeof(string)));
            #endregion line

            #region btn
            DataTable btnDT = new DataTable();
            btnDT.TableName = "Sys_FrmBtn";
            btnDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            btnDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            btnDT.Columns.Add(new DataColumn("Text", typeof(string)));
            btnDT.Columns.Add(new DataColumn("X", typeof(double)));
            btnDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion line

            #region label
            DataTable labDT = new DataTable();
            labDT.TableName = "Sys_FrmLab";
            labDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            labDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            labDT.Columns.Add(new DataColumn("X", typeof(double)));
            labDT.Columns.Add(new DataColumn("Y", typeof(double)));
            labDT.Columns.Add(new DataColumn("Text", typeof(string)));

            labDT.Columns.Add(new DataColumn("FontColor", typeof(string)));
            labDT.Columns.Add(new DataColumn("FontName", typeof(string)));
            labDT.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            labDT.Columns.Add(new DataColumn("FontSize", typeof(int)));
            labDT.Columns.Add(new DataColumn("IsBold", typeof(int)));
            labDT.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion label

            #region Link
            DataTable linkDT = new DataTable();
            linkDT.TableName = "Sys_FrmLink";
            linkDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            linkDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            linkDT.Columns.Add(new DataColumn("X", typeof(double)));
            linkDT.Columns.Add(new DataColumn("Y", typeof(double)));
            linkDT.Columns.Add(new DataColumn("Text", typeof(string)));
            linkDT.Columns.Add(new DataColumn("Target", typeof(string)));
            linkDT.Columns.Add(new DataColumn("URL", typeof(string)));

            linkDT.Columns.Add(new DataColumn("FontColor", typeof(string)));
            linkDT.Columns.Add(new DataColumn("FontName", typeof(string)));
            linkDT.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            linkDT.Columns.Add(new DataColumn("FontSize", typeof(int)));

            linkDT.Columns.Add(new DataColumn("IsBold", typeof(int)));
            linkDT.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion Link

            #region img
            DataTable imgDT = new DataTable();
            imgDT.TableName = "Sys_FrmImg";
            imgDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            imgDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            imgDT.Columns.Add(new DataColumn("X", typeof(double)));
            imgDT.Columns.Add(new DataColumn("Y", typeof(double)));
            imgDT.Columns.Add(new DataColumn("W", typeof(double)));
            imgDT.Columns.Add(new DataColumn("H", typeof(double)));
            imgDT.Columns.Add(new DataColumn("ImgURL", typeof(string)));
            imgDT.Columns.Add(new DataColumn("LinkURL", typeof(string)));
            imgDT.Columns.Add(new DataColumn("LinkTarget", typeof(string)));
            #endregion img

            #region eleDT
            DataTable eleDT = new DataTable();
            eleDT.TableName = "Sys_FrmEle";
            eleDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            eleDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));

            //eleDT.Columns.Add(new DataColumn("EleType", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleID", typeof(string)));
            //eleDT.Columns.Add(new DataColumn("EleName", typeof(string)));

            eleDT.Columns.Add(new DataColumn("X", typeof(double)));
            eleDT.Columns.Add(new DataColumn("Y", typeof(double)));
            eleDT.Columns.Add(new DataColumn("W", typeof(double)));
            eleDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion eleDT

            #region Sys_FrmImgAth
            DataTable imgAthDT = new DataTable();
            imgAthDT.TableName = "Sys_FrmImgAth";
            imgAthDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("Name", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("X", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("Y", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("W", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion Sys_FrmImgAth

            #region mapAttrDT
            DataTable mapAttrDT = new DataTable();
            mapAttrDT.TableName = "Sys_MapAttr";
            mapAttrDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("KeyOfEn", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIContralType", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MyDataType", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("LGType", typeof(string)));

            mapAttrDT.Columns.Add(new DataColumn("UIWidth", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("UIHeight", typeof(double)));

            mapAttrDT.Columns.Add(new DataColumn("UIBindKey", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKey", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKeyText", typeof(string)));
            //   mapAttrDT.Columns.Add(new DataColumn("UIVisible", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("X", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion mapAttrDT

            #region frmRBDT
            DataTable frmRBDT = new DataTable();
            frmRBDT.TableName = "Sys_FrmRB";
            frmRBDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            frmRBDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            frmRBDT.Columns.Add(new DataColumn("KeyOfEn", typeof(string)));
            frmRBDT.Columns.Add(new DataColumn("EnumKey", typeof(string)));
            frmRBDT.Columns.Add(new DataColumn("IntKey", typeof(int)));
            frmRBDT.Columns.Add(new DataColumn("Lab", typeof(string)));
            frmRBDT.Columns.Add(new DataColumn("X", typeof(double)));
            frmRBDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion frmRBDT

            #region Dtl
            DataTable dtlDT = new DataTable();

            dtlDT.TableName = "Sys_MapDtl";
            dtlDT.Columns.Add(new DataColumn("No", typeof(string)));
            dtlDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));

            dtlDT.Columns.Add(new DataColumn("X", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("Y", typeof(double)));

            dtlDT.Columns.Add(new DataColumn("H", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("W", typeof(double)));
            #endregion Dtl

            #region m2mDT
            DataTable m2mDT = new DataTable();
            m2mDT.TableName = "Sys_MapM2M";
            m2mDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("NoOfObj", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));

            m2mDT.Columns.Add(new DataColumn("X", typeof(double)));
            m2mDT.Columns.Add(new DataColumn("Y", typeof(double)));

            m2mDT.Columns.Add(new DataColumn("H", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("W", typeof(string)));
            #endregion m2mDT

            #region athDT
            DataTable athDT = new DataTable();
            athDT.TableName = "Sys_FrmAttachment";
            athDT.Columns.Add(new DataColumn("MyPK", typeof(string)));
            athDT.Columns.Add(new DataColumn("FK_MapData", typeof(string)));
            athDT.Columns.Add(new DataColumn("NoOfObj", typeof(string)));
            athDT.Columns.Add(new DataColumn("Name", typeof(string)));
            athDT.Columns.Add(new DataColumn("Exts", typeof(string)));
            athDT.Columns.Add(new DataColumn("SaveTo", typeof(string)));
            athDT.Columns.Add(new DataColumn("UploadType", typeof(int)));
            athDT.Columns.Add(new DataColumn("X", typeof(double)));
            athDT.Columns.Add(new DataColumn("Y", typeof(double)));
            athDT.Columns.Add(new DataColumn("W", typeof(double)));
            athDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion athDT

            #region 把数据给值.
            foreach (UIElement ctl in this.canvasMain.Children)
            {
                double dX = Canvas.GetLeft(ctl);
                double dY = Canvas.GetTop(ctl);

                #region line.
                BPLine line = ctl as BPLine;
                if (line != null)
                {
                    DataRow drline = lineDT.NewRow();
                    drline["MyPK"] = line.Name;
                    drline["FK_MapData"] = Glo.FK_MapData;

                    drline["X"] = dX.ToString("0.00");
                    drline["Y"] = dY.ToString("0.00");

                    drline["X1"] = line.MyLine.X1.ToString("0.00");
                    drline["X2"] = line.MyLine.X2.ToString("0.00");
                    drline["Y1"] = line.MyLine.Y1.ToString("0.00");
                    drline["Y2"] = line.MyLine.Y2.ToString("0.00");
                    drline["BorderWidth"] = line.MyLine.StrokeThickness.ToString("0.00");

                    //MatrixTransform transform = ctl.TransformToVisual(this.canvasMain) as MatrixTransform;
                    //double x = transform.Matrix.OffsetX;
                    //double y = transform.Matrix.OffsetY;

                    SolidColorBrush d = (SolidColorBrush)line.MyLine.Stroke;
                    drline["BorderColor"] = Glo.PreaseColorToName(d.Color.ToString());
                    lineDT.Rows.Add(drline);
                    continue;
                }
                #endregion line.

                #region lab.
                BPLabel lab = ctl as BPLabel;
                if (lab != null)
                {
                    DataRow drLab = labDT.NewRow();
                    drLab["MyPK"] = lab.Name;
                    drLab["Text"] = lab.Content.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                    drLab["FK_MapData"] = Glo.FK_MapData;

                    drLab["X"] = dX.ToString("0.00");
                    drLab["Y"] = dY.ToString("0.00");

                    // drLab["FontColor"] = lab.GetValue( lapp ).ToString();
#warning 如何获取字体颜色 ? .

                    SolidColorBrush d = (SolidColorBrush)lab.Foreground;
                    drLab["FontColor"] = d.Color.ToString();
                    // Glo.PreaseColorToName(d.Color.ToString());
                    drLab["FontName"] = lab.FontFamily.ToString();
                    drLab["FontStyle"] = lab.FontStyle.ToString();
                    drLab["FontSize"] = lab.FontSize.ToString();

                    if (lab.FontWeight == FontWeights.Normal)
                        drLab["IsBold"] = "0";
                    else
                        drLab["IsBold"] = "1";

                    if (lab.FontStyle.ToString() == "Italic")
                        drLab["IsItalic"] = "1";
                    else
                        drLab["IsItalic"] = "0";

                    // labWin_CBWight
                    //   drLab["UIWidth"] = lab.Width.ToString();
                    labDT.Rows.Add(drLab);
                    continue;
                }
                #endregion lab.

                #region btn.
                BPBtn btn = ctl as BPBtn;
                if (btn != null)
                {
                    DataRow drBtn = btnDT.NewRow();
                    drBtn["MyPK"] = btn.Name;
                    drBtn["Text"] = btn.Content.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                    drBtn["FK_MapData"] = Glo.FK_MapData;

                    drBtn["X"] = dX.ToString("0.00");
                    drBtn["Y"] = dY.ToString("0.00");

                    btnDT.Rows.Add(drBtn);
                    continue;
                }
                #endregion lab.

                #region BPLink.
                BPLink link = ctl as BPLink;
                if (link != null)
                {
                    DataRow drLink = linkDT.NewRow();
                    drLink["MyPK"] = link.Name;

                    drLink["Text"] = link.Content.ToString();
                    drLink["FK_MapData"] = Glo.FK_MapData;

                    drLink["X"] = dX.ToString("0.00");
                    drLink["Y"] = dY.ToString("0.00");

                    SolidColorBrush d = (SolidColorBrush)link.Foreground;
                    drLink["FontColor"] = Glo.PreaseColorToName(d.Color.ToString());
                    drLink["FontName"] = link.FontFamily.ToString();
                    drLink["FontStyle"] = link.FontStyle.ToString();
                    drLink["FontSize"] = link.FontSize.ToString();
                    drLink["URL"] = link.URL;
                    drLink["Target"] = link.WinTarget;

                    if (link.FontWeight == FontWeights.Normal)
                        drLink["IsBold"] = "0";
                    else
                        drLink["IsBold"] = "1";

                    if (link.FontStyle.ToString() == "Italic")
                        drLink["IsItalic"] = "1";
                    else
                        drLink["IsItalic"] = "0";

                    linkDT.Rows.Add(drLink);
                    continue;
                }
                #endregion BPLink.

                #region img.
                BPImg img = ctl as BPImg;
                if (img != null)
                {
                    DataRow drImg = imgDT.NewRow();
                    drImg["MyPK"] = img.Name;
                    drImg["FK_MapData"] = Glo.FK_MapData;

                    MatrixTransform transform = ctl.TransformToVisual(this.canvasMain)
                        as MatrixTransform;
                    double x = transform.Matrix.OffsetX;
                    double y = transform.Matrix.OffsetY;

                    if (x <= 0)
                        x = 0;
                    if (y == 0)
                        y = 0;
                    if (y.ToString() == "NaN")
                    {
                        x = Canvas.GetLeft(ctl);
                        y = Canvas.GetTop(ctl);
                    }

                    drImg["X"] = x.ToString("0.00"); // Canvas.GetLeft(ctl).ToString("0.00");
                    drImg["Y"] = y.ToString("0.00"); // Canvas.GetTop(ctl).ToString("0.00");

                    drImg["W"] = img.Width.ToString("0.00");
                    drImg["H"] = img.Height.ToString("0.00");

                    BitmapImage png = img.HisPng;


                    drImg["ImgURL"] = png.UriSource.ToString().Contains("DataUser") ? png.UriSource.ToString().Replace(Glo.BPMHost, "") : png.UriSource.ToString();
                    drImg["LinkURL"] = img.WinURL;
                    drImg["LinkTarget"] = img.WinTarget;

                    imgDT.Rows.Add(drImg);
                    continue;
                }
                #endregion lab.

                #region ele.
                BPEle ele = ctl as BPEle;
                if (ele != null)
                {
                    DataRow drImg = eleDT.NewRow();
                    drImg["MyPK"] = ele.Name;
                    drImg["FK_MapData"] = Glo.FK_MapData;

                    //drImg["EleType"] = ele.EleType;
                    //drImg["EleName"] = ele.EleName;
                    //drImg["EleID"] = ele.EleID;

                    //eleDT.Columns.Add(new DataColumn("EleType", typeof(string)));
                    //eleDT.Columns.Add(new DataColumn("EleID", typeof(string)));
                    //eleDT.Columns.Add(new DataColumn("EleName", typeof(string)));

                    MatrixTransform transform = ctl.TransformToVisual(this.canvasMain)
                        as MatrixTransform;
                    double x = transform.Matrix.OffsetX;
                    double y = transform.Matrix.OffsetY;

                    if (x <= 0)
                        x = 0;
                    if (y == 0)
                        y = 0;
                    if (y.ToString() == "NaN")
                    {
                        x = Canvas.GetLeft(ctl);
                        y = Canvas.GetTop(ctl);
                    }

                    drImg["X"] = x.ToString("0.00"); // Canvas.GetLeft(ctl).ToString("0.00");
                    drImg["Y"] = y.ToString("0.00"); // Canvas.GetTop(ctl).ToString("0.00");

                    drImg["W"] = ele.Width.ToString("0.00");
                    drImg["H"] = ele.Height.ToString("0.00");

                    eleDT.Rows.Add(drImg);
                    continue;
                }
                #endregion lab.

                #region TextBox
                BPTextBox tb = ctl as BPTextBox;
                if (tb != null)
                {
                    if (tb.IsDelete == true)
                        continue;

                    DataRow mapAttrDR = mapAttrDT.NewRow();
                    mapAttrDR["MyPK"] = Glo.FK_MapData + "_" + tb.Name;
                    mapAttrDR["FK_MapData"] = Glo.FK_MapData;
                    mapAttrDR["KeyOfEn"] = tb.Name;

                    mapAttrDR["UIContralType"] = CtrlType.TextBox;
                    mapAttrDR["MyDataType"] = tb.HisDataType;

                    mapAttrDR["UIWidth"] = tb.Width.ToString("0.00");
                    mapAttrDR["UIHeight"] = tb.Height.ToString("0.00");
                    mapAttrDR["LGType"] = LGType.Normal;


                    MatrixTransform transform = ctl.TransformToVisual(this.canvasMain) as MatrixTransform;
                    double x = transform.Matrix.OffsetX;
                    double y = transform.Matrix.OffsetY;

                    if (y.ToString() == "NaN")
                    {
                        x = Canvas.GetLeft(ctl);
                        y = Canvas.GetTop(ctl);
                    }

                    mapAttrDR["X"] = x.ToString("0.00"); // Canvas.GetLeft(ctl).ToString("0.00");
                    mapAttrDR["Y"] = y.ToString("0.00"); // Canvas.GetTop(ctl).ToString("0.00");
                    // mapAttrDR["UIVisible"] = "1";
                    mapAttrDT.Rows.Add(mapAttrDR);
                    continue;
                }
                #endregion TextBox

                #region datapicker
                BPDatePicker dp = ctl as BPDatePicker;
                if (dp != null)
                {
                    DataRow mapAttrDR = mapAttrDT.NewRow();
                    mapAttrDR["MyPK"] = Glo.FK_MapData + "_" + dp.Name;
                    mapAttrDR["FK_MapData"] = Glo.FK_MapData;
                    mapAttrDR["KeyOfEn"] = dp.Name;

                    mapAttrDR["UIContralType"] = CtrlType.TextBox;
                    mapAttrDR["MyDataType"] = dp.HisDateType;
                    mapAttrDR["LGType"] = LGType.Normal;

                    mapAttrDR["X"] = dX.ToString("0.00");
                    mapAttrDR["Y"] = dY.ToString("0.00");

                    // mapAttrDR["UIVisible"] = "1";
                    mapAttrDR["UIWidth"] = "50";
                    mapAttrDR["UIHeight"] = "23";

                    mapAttrDT.Rows.Add(mapAttrDR);
                    continue;
                }
                #endregion TextBox

                #region BPDDL
                BPDDL ddl = ctl as BPDDL;
                if (ddl != null)
                {
                    if (ddl.IsDelete == true)
                        continue;

                    DataRow mapAttrDR = mapAttrDT.NewRow();
                    mapAttrDR["MyPK"] = Glo.FK_MapData + "_" + ddl.Name;
                    mapAttrDR["FK_MapData"] = Glo.FK_MapData;
                    mapAttrDR["KeyOfEn"] = ddl.Name;

                    mapAttrDR["UIContralType"] = CtrlType.DDL;
                    mapAttrDR["MyDataType"] = ddl.HisDataType;
                    mapAttrDR["LGType"] = ddl.HisLGType;

                    //mapAttrDR["UIWidth"] = "0";
                    //mapAttrDR["UIHeight"] = "0";

                    mapAttrDR["UIWidth"] = ddl.Width.ToString("0.00");
                    mapAttrDR["UIHeight"] = "23"; // ddl.Height.ToString("0.00");


                    mapAttrDR["X"] = dX.ToString("0.00");
                    mapAttrDR["Y"] = dY.ToString("0.00");

                    mapAttrDR["UIBindKey"] = ddl.UIBindKey;
                    mapAttrDR["UIRefKey"] = "No";
                    mapAttrDR["UIRefKeyText"] = "Name";
                    //     mapAttrDR["UIVisible"] = "1";
                    mapAttrDT.Rows.Add(mapAttrDR);
                    continue;
                }
                #endregion BPDDL

                #region CheckBox
                BPCheckBox cb = ctl as BPCheckBox;
                if (cb != null)
                {
                    if (cb.IsDelete == true)
                        continue;

                    DataRow mapAttrDR = mapAttrDT.NewRow();
                    mapAttrDR["MyPK"] = Glo.FK_MapData + "_" + cb.Name;
                    mapAttrDR["FK_MapData"] = Glo.FK_MapData;
                    mapAttrDR["KeyOfEn"] = cb.Name;
                    mapAttrDR["UIContralType"] = CtrlType.CheckBox;
                    mapAttrDR["MyDataType"] = DataType.AppBoolean;
                    mapAttrDR["LGType"] = LGType.Normal;
                    mapAttrDR["X"] = dX.ToString("0.00");
                    mapAttrDR["Y"] = dY.ToString("0.00");
                    //   mapAttrDR["UIVisible"] = "1";
                    mapAttrDR["UIWidth"] = "100"; // cb.Width.ToString("0.00");
                    mapAttrDR["UIHeight"] = "23"; // cb.Height.ToString("0.00");

                    //mapAttrDR["UIBindKey"] = "";
                    //mapAttrDR["UIRefKey"] = "";
                    //mapAttrDR["UIRefKeyText"] = "";
                    mapAttrDT.Rows.Add(mapAttrDR);
                    continue;
                }
                #endregion CheckBox

                #region BPRadioBtn
                BPRadioBtn rb = ctl as BPRadioBtn;
                if (rb != null)
                {
                    DataRow mapAttrRB = frmRBDT.NewRow();
                    mapAttrRB["MyPK"] = rb.Name;
                    mapAttrRB["FK_MapData"] = Glo.FK_MapData;
                    mapAttrRB["KeyOfEn"] = rb.GroupName;
                    mapAttrRB["IntKey"] = rb.Tag as string;
                    mapAttrRB["Lab"] = rb.Content as string;
                    mapAttrRB["EnumKey"] = rb.UIBindKey;
                    mapAttrRB["X"] = dX.ToString("0.00");
                    mapAttrRB["Y"] = dY.ToString("0.00");
                    frmRBDT.Rows.Add(mapAttrRB);
                    continue;
                }
                #endregion BPRadioBtn

                #region BPDtl
                BPDtl dtlCtl = ctl as BPDtl;
                if (dtlCtl != null)
                {
                    DataRow mapDtl = dtlDT.NewRow();
                    mapDtl["No"] = dtlCtl.Name;
                    mapDtl["FK_MapData"] = Glo.FK_MapData;

                    MatrixTransform transform = dtlCtl.TransformToVisual(this.canvasMain) as MatrixTransform;

                    mapDtl["X"] = transform.Matrix.OffsetX.ToString("0.00");
                    mapDtl["Y"] = transform.Matrix.OffsetY.ToString("0.00");

                    //mapDtl["X"] = dX.ToString("0.00");
                    //mapDtl["Y"] = dY.ToString("0.00");

                    mapDtl["W"] = dtlCtl.Width.ToString("0.00");
                    mapDtl["H"] = dtlCtl.Height.ToString("0.00");
                    dtlDT.Rows.Add(mapDtl);
                    continue;
                }
                #endregion BPDtl

                #region BPM2M
                BPM2M m2mCtl = ctl as BPM2M;
                if (m2mCtl != null)
                {
                    DataRow rowM2M = m2mDT.NewRow();
                    rowM2M["NoOfObj"] = m2mCtl.Name;
                    rowM2M["FK_MapData"] = Glo.FK_MapData;
                    rowM2M["MyPK"] = Glo.FK_MapData + "_" + m2mCtl.Name;

                    MatrixTransform transform = m2mCtl.TransformToVisual(this.canvasMain) as MatrixTransform;

                    rowM2M["X"] = transform.Matrix.OffsetX.ToString("0.00");
                    rowM2M["Y"] = transform.Matrix.OffsetY.ToString("0.00");

                    rowM2M["W"] = m2mCtl.Width.ToString("0.00");
                    rowM2M["H"] = m2mCtl.Height.ToString("0.00");

                    m2mDT.Rows.Add(rowM2M);
                    continue;
                }
                #endregion BPM2M

                #region BPAttachment
                BPAttachment athCtl = ctl as BPAttachment;
                if (athCtl != null)
                {
                    DataRow mapAth = athDT.NewRow();
                    mapAth["MyPK"] = Glo.FK_MapData + "_" + athCtl.Name;
                    mapAth["FK_MapData"] = Glo.FK_MapData;
                    mapAth["NoOfObj"] = athCtl.Name;
                    mapAth["Name"] = athCtl.Label;
                    mapAth["Exts"] = athCtl.Exts;
                    mapAth["SaveTo"] = athCtl.SaveTo;
                    mapAth["UploadType"] = "0";

                    MatrixTransform transform = athCtl.TransformToVisual(this.canvasMain)
                        as MatrixTransform;

                    mapAth["X"] = transform.Matrix.OffsetX.ToString("0.00");
                    mapAth["Y"] = transform.Matrix.OffsetY.ToString("0.00");
                    mapAth["W"] = athCtl.HisTB.Width.ToString("0.00");

                    athDT.Rows.Add(mapAth);
                    continue;
                }
                #endregion BPAttachment

                #region BPAttachmentM
                BPAttachmentM athM = ctl as BPAttachmentM;
                if (athM != null)
                {
                    DataRow mapAth = athDT.NewRow();
                    mapAth["MyPK"] = Glo.FK_MapData + "_" + athM.Name;
                    mapAth["FK_MapData"] = Glo.FK_MapData;
                    mapAth["NoOfObj"] = athM.Name;
                    mapAth["Name"] = athM.Label;
                    mapAth["SaveTo"] = athM.SaveTo;
                    mapAth["UploadType"] = "1";

                    MatrixTransform transform = athM.TransformToVisual(this.canvasMain) as MatrixTransform;
                    mapAth["X"] = transform.Matrix.OffsetX.ToString("0.00");
                    mapAth["Y"] = transform.Matrix.OffsetY.ToString("0.00");

                    mapAth["W"] = athM.Width.ToString("0.00");
                    mapAth["H"] = athM.Height.ToString("0.00");
                    athDT.Rows.Add(mapAth);
                    continue;
                }
                #endregion BPAttachmentM

                #region BPImgAth
                BPImgAth imgAth = ctl as BPImgAth;
                if (imgAth != null)
                {
                    DataRow mapAth = imgAthDT.NewRow();
                    mapAth["MyPK"] = imgAth.Name;
                    mapAth["FK_MapData"] = Glo.FK_MapData;
                    mapAth["Name"] = imgAth.Desc;

                    MatrixTransform transform = imgAth.TransformToVisual(this.canvasMain)
                        as MatrixTransform;

                    mapAth["X"] = transform.Matrix.OffsetX.ToString("0.00");
                    mapAth["Y"] = transform.Matrix.OffsetY.ToString("0.00");

                    mapAth["W"] = imgAth.Width.ToString("0.00");
                    mapAth["H"] = imgAth.Height.ToString("0.00");
                    imgAthDT.Rows.Add(mapAth);
                    continue;
                }
                #endregion BPAttachment
            }
            #endregion 把数据给值.

            #region 处理 RB 枚举值
            string keys = "";
            foreach (DataRow dr in frmRBDT.Rows)
            {
                string keyOfEn = dr["KeyOfEn"];
                if (keys.Contains("@" + keyOfEn + "@"))
                {
                    continue;
                }
                else
                {
                    keys += "@" + keyOfEn + "@";
                }

                string enumKey = dr["EnumKey"];
                DataRow mapAttrDR = mapAttrDT.NewRow();
                mapAttrDR["MyPK"] = Glo.FK_MapData + "_" + keyOfEn;
                mapAttrDR["FK_MapData"] = Glo.FK_MapData;
                mapAttrDR["KeyOfEn"] = keyOfEn;

                mapAttrDR["UIContralType"] = CtrlType.RB;
                mapAttrDR["MyDataType"] = DataType.AppInt;
                mapAttrDR["LGType"] = LGType.Enum;
                //  mapAttrDR["IntKey"] = dr["IntKey"];

                mapAttrDR["X"] = "0";
                mapAttrDR["Y"] = "0";

                mapAttrDR["UIBindKey"] = enumKey;
                mapAttrDR["UIRefKey"] = "No";
                mapAttrDR["UIRefKeyText"] = "Name";
                //      mapAttrDR["UIVisible"] = "1";
                mapAttrDR["UIWidth"] = "30";
                mapAttrDR["UIHeight"] = "23";
                mapAttrDT.Rows.Add(mapAttrDR);
            }
            #endregion 处理 RB 枚举值

            #region 增加到里面
            DataSet myds = new DataSet();
            myds.Tables.Add(labDT);
            myds.Tables.Add(linkDT);
            myds.Tables.Add(imgDT);
            myds.Tables.Add(eleDT);
            myds.Tables.Add(btnDT);
            myds.Tables.Add(imgAthDT);
            myds.Tables.Add(mapAttrDT);
            myds.Tables.Add(frmRBDT);
            myds.Tables.Add(lineDT);
            myds.Tables.Add(dtlDT);
            myds.Tables.Add(athDT);
            myds.Tables.Add(mapDataDT);
            myds.Tables.Add(m2mDT);
            #endregion 增加到里面

            #region 求出已经删除的集合.
            string sqls = "";
            foreach (DataTable ysdt in this.FrmDS.Tables)
            {
                DataTable newDt = myds.Tables[ysdt.TableName];
                if (newDt == null)
                    continue;

                #region 求pK
                string pk = "";
                foreach (DataColumn dc in ysdt.Columns)
                {
                    switch (dc.ColumnName.ToLower())
                    {
                        case "mypk":
                            pk = "MyPK";
                            break;
                        case "no":
                            pk = "No";
                            break;
                        case "oid":
                            pk = "OID";
                            break;
                        default:
                            break;
                    }
                }
                #endregion 求pK

                foreach (DataRow dr in ysdt.Rows)
                {
                    if (ysdt.TableName == "Sys_MapData")
                        continue;

                    if (dr["FK_MapData"] != Glo.FK_MapData)
                        continue;

                    string pkVal = dr[pk].ToString();
                    bool isHave = false;
                    if (ysdt.TableName == "Sys_MapAttr")
                    {
                        /* 如果是判断的字段控件 .. */
                        isHave = false;
                        foreach (DataRow newDr in newDt.Rows)
                        {
                            if (dr["FK_MapData"] != Glo.FK_MapData || dr["UIVisible"] == "0")
                            {
                                isHave = true;
                                break;
                            }
                            if (newDr[pk].ToString() == pkVal)
                            {
                                isHave = true;
                                break;
                            }
                        }

                        if (isHave == false)
                        {
                            if (dr["UIVisible"] == "0" || dr["EditType"] != "0")
                                isHave = true;
                        }
                    }
                    else
                    {
                        foreach (DataRow newDr in newDt.Rows)
                        {
                            if (newDr[pk].ToString() == pkVal)
                            {
                                isHave = true;
                                break;
                            }
                        }
                    }
                    if (isHave == false)
                        sqls += "@DELETE FROM " + ysdt.TableName + " WHERE " + pk + "='" + pkVal + "'";
                }
            }
            #endregion

            #region save label.
            string len = Glo.LEN_Function;
            //  if (Glo.ap
            foreach (UIElement ctl in this.canvasMain.Children)
            {
                BPCheckBox cb = ctl as BPCheckBox;
                if (cb != null)
                {
                    if (cb.KeyName == null)
                        continue;

                    if (string.IsNullOrEmpty(cb.KeyName))
                        continue;

                    Label mylab = cb.Content as Label;
                    sqls += "@UPDATE Sys_MapAttr SET Name='" + mylab.Content + "'  WHERE MyPK='" + Glo.FK_MapData + "_" + cb.Name + "' AND " + len + "(Name)=0";
                    continue;
                }

                BPTextBox tb = ctl as BPTextBox;
                if (tb != null)
                {
                    if (string.IsNullOrEmpty(tb.KeyName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + tb.KeyName + "' WHERE MyPK='" + Glo.FK_MapData + "_" + tb.Name + "' AND ( " + len + "(Name)=0 OR KeyOfEn=Name )";
                    continue;
                }

                BPDDL ddl = ctl as BPDDL;
                if (ddl != null)
                {
                    if (string.IsNullOrEmpty(ddl.KeyName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + ddl.KeyName + "' WHERE MyPK='" + Glo.FK_MapData + "_" + ddl.Name + "' AND " + len + "(Name)=0";
                    continue;
                }

                BPRadioBtn rb = ctl as BPRadioBtn;
                if (rb != null)
                {
                    if (string.IsNullOrEmpty(rb.KeyName))
                        continue;

                    if (sqls.Contains("_" + rb.GroupName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + rb.KeyName + "' WHERE MyPK='" + Glo.FK_MapData + "_" + rb.GroupName + "' AND " + len + "(Name)=0";
                    continue;
                }
            }
            #endregion save  label.

            sqls += "@UPDATE Sys_MapAttr SET UIVisible=1 WHERE FK_MapData='" + Glo.FK_MapData + "' AND UIVisible is null";
            this.loadingWindow.Title = "正在保存数据...";
            this.loadingWindow.Show();

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveFrmAsync(Glo.FK_MapData, myds.ToXml(true, false), sqls, null);

            da.SaveFrmCompleted += new EventHandler<FF.SaveFrmCompletedEventArgs>(da_SaveFrmCompleted);
        }
        void da_SaveFrmCompleted(object sender, FF.SaveFrmCompletedEventArgs e)
        {
            if (e != null && e.Result != null)
            {
                MessageBox.Show(e.Result, "保存错误", MessageBoxButton.OK);
                return;
            }

            this.BindFrm();

            if (Keyboard.Modifiers == ModifierKeys.Windows)
            {
                string url1 = null;
                if (Glo.IsDtlFrm == false)
                    url1 = Glo.BPMHost + "/WF/CCForm/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node + Glo.TimeKey;
                else
                    url1 = Glo.BPMHost + "/WF/CCForm/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&RefPKVal=0&OID=0" + Glo.TimeKey;
                HtmlPage.Window.Eval("window.open('" + url1 + "','_blank','scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes,width=" + Glo.HisMapData.FrmW + ",height=" + Glo.HisMapData.FrmH + "')");
            }
            else
            {
                //  MessageBox.Show("ccform 保存成功.", "保存提示", MessageBoxButton.OK);
            }
        }
        public void RunSQL(string sql)
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLsAsync(sql);
            da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(da_RunSQLsCompleted);
        }
        void da_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            switch (this.DoTypeName)
            {
                case "DeleteFrm":
                    this.BindTreeView();
                    MessageBox.Show("删除成功!!!");
                    break;
                default:
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.delPoint();
            switch (e.Key)
            {
                case Key.W: // 向上
                case Key.Up: // 向上
                    foreach (FrameworkElement item in selectedElements)
                    {
                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            if (line.MyLine.X1 == line.MyLine.X2)
                            {
                                line.MyLine.Y1 = line.MyLine.Y1 - 1;
                                line.MyLine.Y2 = line.MyLine.Y2 - 1;
                            }
                            else
                            {
                                line.MyLine.Y1 = line.MyLine.Y1 - 1;
                                line.MyLine.Y2 = line.MyLine.Y2 - 1;
                            }
                            continue;
                        }

                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            lab.ToUp();
                            continue;
                        }

                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            BPTextBox textBox = item as BPTextBox;
                            if (textBox != null)
                            {
                                if (textBox.Height > 18)
                                    textBox.Height = textBox.Height - 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.TopProperty, Canvas.GetTop(item) - 1);
                        }
                    }
                    break;
                case Key.S: // 向下
                case Key.Down: // 向下
                    if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Windows)
                    {
                        this.Save_Click(null, null);
                        return;
                    }
                    foreach (FrameworkElement item in selectedElements)
                    {
                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            if (line.MyLine.X1 == line.MyLine.X2)
                            {
                                line.MyLine.Y1 = line.MyLine.Y1 + 1;
                                line.MyLine.Y2 = line.MyLine.Y2 + 1;
                            }
                            else
                            {
                                line.MyLine.Y1 = line.MyLine.Y1 + 1;
                                line.MyLine.Y2 = line.MyLine.Y2 + 1;
                            }
                            continue;
                        }

                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            lab.ToDown();
                            continue;
                        }

                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            BPTextBox textBox = item as BPTextBox;
                            if (textBox != null)
                            {
                                textBox.Height = textBox.Height + 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.TopProperty, Canvas.GetTop(item) + 1);
                        }
                    }
                    break;
                case Key.D: // 向右
                case Key.Right: // 向右
                    foreach (FrameworkElement item in selectedElements)
                    {
                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            if (line.MyLine.X1 == line.MyLine.X2)
                            {
                                line.MyLine.X1 = line.MyLine.X1 + 1;
                                line.MyLine.X2 = line.MyLine.X2 + 1;
                            }
                            else
                            {
                                line.MyLine.X1 = line.MyLine.X1 + 1;
                                line.MyLine.X2 = line.MyLine.X2 + 1;
                            }
                            continue;
                        }

                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            lab.ToRight();
                            continue;
                        }
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            BPTextBox textBox = item as BPTextBox;
                            if (textBox != null)
                            {
                                textBox.Width = textBox.Width + 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.LeftProperty, Canvas.GetLeft(item) + 1);
                        }
                    }
                    break;
                case Key.A: // 向左.
                case Key.Left: // 向左
                    foreach (FrameworkElement item in selectedElements)
                    {
                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            if (line.MyLine.X1 == line.MyLine.X2)
                            {
                                line.MyLine.X1 = line.MyLine.X1 - 1;
                                line.MyLine.X2 = line.MyLine.X2 - 1;
                            }
                            else
                            {
                                line.MyLine.X1 = line.MyLine.X1 - 1;
                                line.MyLine.X2 = line.MyLine.X2 - 1;
                            }
                            continue;
                        }

                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            lab.ToLeft();
                            continue;
                        }
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            BPTextBox textBox = item as BPTextBox;
                            if (textBox != null)
                            {
                                textBox.Width = textBox.Width - 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.LeftProperty, Canvas.GetLeft(item) - 1);
                        }
                    }
                    break;
                case Key.Delete: //删除.
                    string sql = "";
                    foreach (UIElement item in selectedElements)
                    {
                        if (this.canvasMain.Children.Contains(item) == true)
                        {
                            BPTextBox tb = item as BPTextBox;
                            if (tb != null)
                            {
                                if (tb.Name == "Title" && Glo.FK_Node.ToString().Contains("101"))
                                {
                                    MessageBox.Show("@开始节点的标题字段不允许删除。");
                                    continue;
                                }
                                else
                                {
                                    tb.DeleteIt();
                                    continue;
                                }
                            }

                            BPDtl dtl = item as BPDtl;
                            if (dtl != null)
                            {
                                dtl.DeleteIt();
                                continue;
                            }

                            this.canvasMain.Children.Remove(item);
                        }
                    }
                    this.selectedElements.Clear();
                    break;
                case Key.V: //复制.
                    this.DoCopy();
                    break;
                case Key.C: //复制.
                    this.IsCopy = true;
                    break;
                case Key.Escape: //复制.
                    this.UnSelectAll();
                    this.SetSelectedTool(ToolBox.Mouse);
                    break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }

        #region  矩形选择框处理
        bool flag = true;
        double left = 0.0;
        double top = 0.0;
        void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            rect.Cursor = Cursors.None;
            CustomCursorImage.Visibility = System.Windows.Visibility.Visible;
            Point point = e.GetPosition(this.canvasMain);
            CustomCursorImage.SetValue(Canvas.LeftProperty, point.X);
            CustomCursorImage.SetValue(Canvas.TopProperty, point.Y > 15 ? point.Y - 10 : point.Y);
        }
        void rect_MouseLeave(object sender, MouseEventArgs e)
        {
            CustomCursorImage.Visibility = System.Windows.Visibility.Collapsed;
        }
        void rect_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                left = Convert.ToDouble(rect.GetValue(Canvas.LeftProperty));
                top = Convert.ToDouble(rect.GetValue(Canvas.TopProperty));
                flag = false;
            }
            foreach (var item in this.selectedElements)
            {
                item.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) + Convert.ToDouble(rect.GetValue(Canvas.LeftProperty)) - left);
                item.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) + Convert.ToDouble(rect.GetValue(Canvas.TopProperty)) - top);
                BPLine line = item as BPLine;
                if (line != null)
                {
                    line.MyLine.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) + Convert.ToDouble(rect.GetValue(Canvas.LeftProperty)) - left);
                    line.MyLine.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) + Convert.ToDouble(rect.GetValue(Canvas.TopProperty)) - top);
                }
            }
            left = Convert.ToDouble(rect.GetValue(Canvas.LeftProperty));
            top = Convert.ToDouble(rect.GetValue(Canvas.TopProperty));

            Point point = e.GetPosition(this.canvasMain);
            CustomCursorImage.SetValue(Canvas.LeftProperty, point.X);
            CustomCursorImage.SetValue(Canvas.TopProperty, point.Y > 15 ? point.Y - 10 : point.Y);
        }

        private void SelectUIElement()
        {
            double BeginX = Convert.ToDouble(rect.GetValue(Canvas.LeftProperty));
            double BeginY = Convert.ToDouble(rect.GetValue(Canvas.TopProperty));
            double EndX = BeginX + rect.ActualWidth;
            double EndY = BeginY + rect.ActualHeight;

            double finalX1 = double.MaxValue;
            double finalX2 = double.MinValue;
            double finalY1 = double.MaxValue;
            double finalY2 = double.MinValue;

            double X1 = 0.0;
            double Y1 = 0.0;
            double X2 = 0.0;
            double Y2 = 0.0;
            foreach (UIElement ue in canvasMain.Children)
            {
                if (ue is FrameworkElement && !object.ReferenceEquals(ue, rect))
                {
                    FrameworkElement c = ue as FrameworkElement;
                    BPLine line = c as BPLine;
                    if (line != null)
                    {
                        X1 = line.MyLine.X1;
                        Y1 = line.MyLine.Y1;
                        X2 = line.MyLine.X2;
                        Y2 = line.MyLine.Y2;
                    }
                    else
                    {
                        X1 = Convert.ToDouble(c.GetValue(Canvas.LeftProperty));
                        Y1 = Convert.ToDouble(c.GetValue(Canvas.TopProperty));
                        X2 = X1 + c.ActualWidth;
                        Y2 = Y1 + c.ActualHeight;

                        if (BeginX < X2 && EndX > X1 && BeginY < Y2 && EndY > Y1)
                        {
                            this.selectedElements.Add(c);
                            if (finalX1 > X1) finalX1 = X1;
                            if (finalX2 < X2) finalX2 = X2;
                            if (finalY1 > Y1) finalY1 = Y1;
                            if (finalY2 < Y2) finalY2 = Y2;
                        }
                    }
                    //if (BeginX < X2 && EndX > X1 && BeginY < Y2 && EndY > Y1)
                    //{
                    //    this.selectedElements.Add(c);
                    //    if (finalX1 > X1) finalX1 = X1;
                    //    if (finalX2 < X2) finalX2 = X2;
                    //    if (finalY1 > Y1) finalY1 = Y1;
                    //    if (finalY2 < Y2) finalY2 = Y2;
                    //}

                }
            }
            //判断有没有线
            //bool isHaveLine = false;
            //foreach (FrameworkElement item in this.selectedElements)
            //{
            //    BPLine line = item as BPLine;
            //    if (line != null)
            //    {
            //        isHaveLine = true;
            //    }
            //}
            //如果没有线进行框选区域
            if (this.selectedElements.Count > 0)
            {
                rect.SetValue(Canvas.LeftProperty, finalX1);
                rect.SetValue(Canvas.TopProperty, finalY1);
                rect.Width = Math.Abs(finalX2 - finalX1);
                rect.Height = Math.Abs(finalY2 - finalY1);
            }
            else if (this.selectedElements.Count == 0)
            {
                RemoveRect();
            }
        }

        void RemoveRect()
        {
            if (rect != null)
            {
                isMultipleSelected = false;
                rect.MouseMove -= rect_MouseMove;
                canvasMain.Children.Remove(rect);
                this.selectedElements.Clear();
                flag = true;
                rect = null;
            }
            //移除鼠标
            if (CustomCursorImage != null)
            {
                canvasMain.Children.Remove(CustomCursorImage);
                CustomCursorImage = null;
            }
        }
        #endregion

        #region 关于撤销与恢复的处理.
        public void DoRecStep(string doType, UIElement obj, double x1, double y1, double x2, double y2)
        {
            Glo.CurrOpStep = Glo.CurrOpStep + 1;
            FuncStep en = new FuncStep();
            en.DoType = doType;
            en.Ele = obj;
            en.X1 = x1;
            en.X2 = x2;
            en.Y1 = y1;
            en.Y2 = y2;
            Glo.FuncSteps.Add(en);
        }
        public void DoRecStep(string doType, UIElement obj)
        {
            DoRecStep(doType, obj, 0, 0, 0, 0);
        }
        #endregion 关于撤销与恢复的处理.


        private void zz_NodeCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.CloseMenum();
        }
    }
}
