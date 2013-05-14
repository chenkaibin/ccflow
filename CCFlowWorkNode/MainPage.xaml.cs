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
using BP.WF;
namespace WorkNode
{
    public partial class MainPage : UserControl
    {
        #region zhoupeng add 全局变量
        LoadingWindow loadingWindow = new LoadingWindow();
        #endregion 全局变量

        public void SetGridLines()
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
        /// <summary>
        /// 入口点
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            #region 获取参数
            FF.CCFlowAPISoapClient da = Glo.GetCCFlowAPISoapClientServiceInstance();
            da.GenerWorkNodeAsync(Glo.FK_Flow, Glo.FK_Node, Glo.WorkID, Glo.UserNo);
            da.GenerWorkNodeCompleted += new EventHandler<FF.GenerWorkNodeCompletedEventArgs>(BindFrm);
            #endregion 获取参数
        }
        /// <summary>
        /// 绑定表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BindFrm(object sender, FF.GenerWorkNodeCompletedEventArgs e)
        {
            #region 初始化数据.
            this.canvasMain.Children.Clear();
            this.FrmDS = new DataSet();
            try
            {
                if (e.Result.Length < 200)
                    throw new Exception(e.Result);
                this.FrmDS.FromXml(e.Result);
                loadingWindow.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Err", MessageBoxButton.OK);
                loadingWindow.DialogResult = true;
                return;
            }
            #endregion 初始化数据.

            this.InitToolbar();

            string table = "";
            try
            {
                this.dtMapAttrs = this.FrmDS.Tables["Sys_MapAttr"];
                foreach (DataTable dt in this.FrmDS.Tables)
                {
                    Glo.TempVal = dt.TableName;
                    table = dt.TableName;
                    switch (dt.TableName)
                    {
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
                                bool isEnable = false;
                                if (dr["UIIsEnable"].ToString() == "1")
                                    isEnable = true;

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
                                       
                                        tb.Text = this.GetValByKey(keyOfEn); //给控件赋值.
                                        tb.Width = double.Parse(dr["UIWidth"]);

                                        if (tb.Height > 24)
                                            tb.TextWrapping = TextWrapping.Wrap;

                                        tb.Height = double.Parse(dr["UIHeight"]);

                                        if (isEnable)
                                            tb.IsEnabled = true;
                                        else
                                            tb.IsEnabled = false;
                                        this.canvasMain.Children.Add(tb);
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
                                            DataTable dtEnum = this.FrmDS.Tables["Sys_Enum"];
                                            foreach (DataRow drEnum in dtEnum.Rows)
                                            {
                                                if (drEnum["EnumKey"].ToString() != UIBindKey)
                                                    continue;

                                                ListBoxItem li = new ListBoxItem();
                                                li.Tag = drEnum["IntKey"].ToString();
                                                li.Content = drEnum["Lab"].ToString();
                                                ddl.Items.Add(li);
                                            }
                                            if (ddl.Items.Count == 0)
                                                throw new Exception("@没有从Sys_Enum中找到编号为(" + UIBindKey + ")的枚举值。");
                                        }
                                        else
                                        {
                                            ddl.BindEns(UIBindKey);
                                        }

                                        ddl.SetValue(Canvas.LeftProperty, X);
                                        ddl.SetValue(Canvas.TopProperty, Y);
                                        
                                        //给控件赋值.
                                        ddl.SetSelectVal(this.GetValByKey(keyOfEn));

                                        this.canvasMain.Children.Add(ddl);
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

                                        cb.SetValue(Canvas.LeftProperty, X);
                                        cb.SetValue(Canvas.TopProperty, Y);

                                        if (this.GetValByKey(keyOfEn) == "1")
                                            cb.IsChecked = true;
                                        else
                                            cb.IsChecked = false;
                                        this.canvasMain.Children.Add(cb);
                                        break;
                                    case CtrlType.RB:
                                        break;
                                    default:
                                        break;
                                }
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
                                this.canvasMain.Children.Add(btn);
                            }
                            continue;
                        case "Sys_MapDtl":
                            foreach (DataRow dr in dt.Rows)
                            {
                                BPDtl dtl = new BPDtl(dr["No"], this.FrmDS);
                                dtl.SetValue(Canvas.LeftProperty, double.Parse(dr["X"]));
                                dtl.SetValue(Canvas.TopProperty, double.Parse(dr["Y"]));
                                dtl.Width = double.Parse(dr["W"]);
                                dtl.Height = double.Parse(dr["H"]);
                                this.canvasMain.Children.Add(dtl);
                            }
                            continue;
                        case "Sys_FrmEle":
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["FK_MapData"] != Glo.FK_MapData)
                                    continue;

                                BPEle img = new BPEle();
                                img.Name = dr["MyPK"].ToString();
                                img.EleType = dr["EleType"].ToString();
                                img.EleName = dr["EleName"].ToString();
                                img.EleID = dr["EleID"].ToString();

                                img.Cursor = Cursors.Hand;
                                img.SetValue(Canvas.LeftProperty, double.Parse(dr["X"].ToString()));
                                img.SetValue(Canvas.TopProperty, double.Parse(dr["Y"].ToString()));

                                img.Width = double.Parse(dr["W"].ToString());
                                img.Height = double.Parse(dr["H"].ToString());
                                this.canvasMain.Children.Add(img);
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
                                // Glo.IsDtlFrm = false;
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
                                this.canvasMain.Children.Add(img);
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
                                this.canvasMain.Children.Add(ath);
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
                                this.canvasMain.Children.Add(m2m);
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

                                    this.canvasMain.Children.Add(ath);
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
                                    this.canvasMain.Children.Add(athM);
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
                MessageBox.Show("err:" + table, ex.Message +" "+ex.StackTrace, 
                    MessageBoxButton.OK);
            }
            this.SetGridLines();
        }
        /// <summary>
        /// 生成toolbar
        /// </summary>
        public void InitToolbar()
        {
            DataTable dt = this.FrmDS.Tables["WF_BtnLab"];

            #region 生成toolbar .
            /*发送*/
            List<Func> ens = new List<Func>();

            Func enAdd = new Func();
            enAdd.No = BtnAttr.SendLab;
            enAdd.Name = dt.Rows[0][BtnAttr.SendLab];
            ens.Add(enAdd);

            int i=0;
            /*保存*/
            if (dt.Rows[0][BtnAttr.SaveEnable].ToString() != "0")
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.SaveLab;
                enAdd.Name = dt.Rows[0][BtnAttr.SaveLab];
                ens.Add(enAdd);
            }

            /*退回*/
            if (dt.Rows[0][BtnAttr.ReturnRole].ToString() != "0" || i==0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.ReturnLab;
                enAdd.Name = dt.Rows[0][BtnAttr.ReturnLab];
                ens.Add(enAdd);
            }

            /*跳转*/
            if (dt.Rows[0][NodeAttr.JumpWay].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.JumpWayLab;
                enAdd.Name = dt.Rows[0][BtnAttr.JumpWayLab];
                ens.Add(enAdd);
            }

            /*抄送*/
            if (dt.Rows[0][NodeAttr.CCRole].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.CCLab;
                enAdd.Name = dt.Rows[0][BtnAttr.CCLab];
                ens.Add(enAdd);
            }

            /*移交*/
            if (dt.Rows[0][BtnAttr.ShiftEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.ShiftLab;
                enAdd.Name = dt.Rows[0][BtnAttr.ShiftLab];
                ens.Add(enAdd);
            }

            /*删除*/
            if (dt.Rows[0][BtnAttr.DelEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.DelLab;
                enAdd.Name = dt.Rows[0][BtnAttr.DelLab];
                ens.Add(enAdd);
            }

            /*结束*/
            if (dt.Rows[0][BtnAttr.EndFlowEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.EndFlowLab;
                enAdd.Name = dt.Rows[0][BtnAttr.EndFlowLab];
                ens.Add(enAdd);
            }

            /*打印单据*/
            if (dt.Rows[0][BtnAttr.PrintDocEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.PrintDocLab;
                enAdd.Name = dt.Rows[0][BtnAttr.PrintDocLab];
                ens.Add(enAdd);
            }

            /*轨迹*/
            if (dt.Rows[0][BtnAttr.TrackEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.TrackLab;
                enAdd.Name = dt.Rows[0][BtnAttr.TrackLab];
                ens.Add(enAdd);
            }

            /*接受人*/
            if (dt.Rows[0][BtnAttr.SelectAccepterEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.SelectAccepterLab;
                enAdd.Name = dt.Rows[0][BtnAttr.SelectAccepterLab];
                ens.Add(enAdd);
            }

            /*选项*/
            //if (dt.Rows[0][BtnAttr.OptEnable].ToString() != "0" || i == 0)
            //{
            //    enAdd = new Func();
            //    enAdd.No = BtnAttr.OptLab;
            //    enAdd.Name = dt.Rows[0][BtnAttr.OptLab];
            //    ens.Add(enAdd);
            //}

            /*查询*/
            if (dt.Rows[0][BtnAttr.SearchEnable].ToString() != "0" || i == 0)
            {
                enAdd = new Func();
                enAdd.No = BtnAttr.SearchLab;
                enAdd.Name = dt.Rows[0][BtnAttr.SearchLab];
                ens.Add(enAdd);
            }


            //把按钮增加到tool bar .
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
                BitmapImage png = new BitmapImage(new Uri("/WorkNode;component/Img/" + Func.Save + ".png", UriKind.Relative));
                img.Source = png;
                img.Width = 13 ;
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
            #endregion
        }
        void daAppCenter_CfgKeyCompleted(object sender, FF.CfgKeyCompletedEventArgs e)
        {
            Glo.AppCenterDBType = e.Result;
        }
        public DataSet FrmDS = null;
        private DataTable dtND = null;
        private DataTable dtMapAttrs = null;
        public string GetValByKey(string key)
        {
            if (dtND == null)
                dtND = this.FrmDS.Tables["Main"];
            return dtND.Rows[0][key].ToString();
        }
        
        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            #region 获取id
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
            #endregion 获取id

            id = id.Replace("Btn_", "");
            switch (id)
            {
                case BP.WF.BtnAttr.SendLab: // 发送.
                    Send();
                    break;
                case BP.WF.BtnAttr.SaveLab: // 保存.
                    SaveFrm();
                    return;
                case BP.WF.BtnAttr.ReturnLab: //退回.
                    ReturnWork rw = new ReturnWork();
                    rw.Show();
                    return;
                case BP.WF.BtnAttr.CCLab: //抄送.
                    CC cc = new CC();
                    cc.Show();
                    return;
                case BP.WF.BtnAttr.PrintDocLab: //打印单据.
                    PrintDoc doc = new PrintDoc();
                    doc.Show();
                    return;
                case BP.WF.BtnAttr.SelectAccepterLab: //选择接受人.
                    SelectAccepter sa = new SelectAccepter();
                    sa.Show();
                    return;
                case BP.WF.BtnAttr.ShiftLab: //移交.
                    ShiftWork sw = new ShiftWork();
                    sw.Show();
                    return;
                case BP.WF.BtnAttr.HungLab: //挂起.
                    HungUp hw = new HungUp();
                    hw.Show();
                    return;
                case BP.WF.BtnAttr.TrackLab: //轨迹.
                    Track tc = new Track();
                    tc.Show();
                    return;
                case BP.WF.BtnAttr.JumpWayLab: //跳转.
                    Jump j = new Jump();
                    j.Show();
                    return;
                case BP.WF.BtnAttr.SearchLab: //查询.
                    Search s = new Search();
                    s.Show();
                    return;
                case BP.WF.BtnAttr.EndFlowLab: //结束流程.
                    if (MessageBox.Show("您确定要结束流程吗？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        MessageBox.Show("结束功能未实现。");
                    }
                    return;
                case BP.WF.BtnAttr.DelLab: //删除.
                    if (MessageBox.Show("您确定要删除流程吗？", "确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        MessageBox.Show("删除功能未实现。");
                    }
                    return;
                default:
                    MessageBox.Show(sender.ToString() + " ID=" + id + " 功能未实现.");
                    break;
            }
        }
        /// <summary>
        /// 获取页面的数据
        /// </summary>
        /// <returns></returns>
        public DataSet GenerFrmDataSet()
        {
            #region  创建数据表.
            DataTable dtMain = new DataTable();
            dtMain.TableName = "Main";
            dtMain.Columns.Add(new DataColumn("KeyOfEn", typeof(string)));
            dtMain.Columns.Add(new DataColumn("Val", typeof(string)));
            #endregion 创建数据表.

            #region 获取主表的值.
            foreach (DataRow dr in dtMapAttrs.Rows)
            {
                if (dr["UIVisible"] == "0")
                    continue;

                if (dr["FK_MapData"] != Glo.FK_MapData)
                    continue;
                DataRow drNew = dtMain.NewRow();

                string keyOfEn = dr["KeyOfEn"];

                UIElement ctl = this.canvasMain.FindName(keyOfEn) as UIElement;
                BPTextBox tb = ctl as BPTextBox;
                if (tb != null)
                {
                    drNew[0] = keyOfEn;
                    drNew[1] = tb.Text;
                    dtMain.Rows.Add(drNew);
                    continue;
                }

                BPDDL ddl = ctl as BPDDL;
                if (ddl != null)
                {
                    ListBoxItem li = ddl.SelectedItem as ListBoxItem;
                    if (li == null)
                        throw new Exception("@没有找到选择的item.");
                    drNew[0] = keyOfEn;
                    drNew[1] = li.Tag.ToString();
                    dtMain.Rows.Add(drNew);
                    continue;
                }

                BPCheckBox cb = ctl as BPCheckBox;
                if (cb != null)
                {
                    drNew[0] = keyOfEn;
                    if (cb.IsChecked == true)
                        drNew[1] = "1";
                    else
                        drNew[1] = "0";
                    dtMain.Rows.Add(drNew);
                    continue;
                }
            }
            #endregion 获取从表的值.

            DataSet dsNodeData = new DataSet();
            dsNodeData.Tables.Add(dtMain);

#warning 这里仅仅处理了主表的保存，多从表，多m2m，附件，图片。都没有处理。
            return dsNodeData;
        }
        /// <summary>
        /// 发送
        /// </summary>
        private void Send()
        {
            this.loadingWindow.Title = "正在保存并发送...";
            this.loadingWindow.Show();
            FF.CCFlowAPISoapClient sendWorkNode = Glo.GetCCFlowAPISoapClientServiceInstance();
            sendWorkNode.Node_SendWorkAsync(Glo.FK_Flow, Glo.FK_Node, Glo.WorkID, Glo.UserNo,
                this.GenerFrmDataSet().ToXml(true, false));
            sendWorkNode.Node_SendWorkCompleted += new EventHandler<FF.Node_SendWorkCompletedEventArgs>(sendWorkNode_Node_SendWorkCompleted);
        }
        void sendWorkNode_Node_SendWorkCompleted(object sender, FF.Node_SendWorkCompletedEventArgs e)
        {
            /*发送成功后
            * 1, 提示发送信息
            * 2，
            */

            this.loadingWindow.DialogResult = true;
            MessageBox.Show(e.Result);
            //string url1 = null;
            //if (Glo.IsDtlFrm == false)
            //    url1 = Glo.BPMHost + "/WF/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node;
            //else
            //    url1 = Glo.BPMHost + "/WF/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&RefPKVal=0&OID=0";
            //HtmlPage.Window.Eval("window.open('" + url1 + "','_blank','scrollbars=yes,resizable=yes,toolbar=false,location=false,center=yes,center: yes,width=" + Glo.HisMapData.FrmW + ",height=" + Glo.HisMapData.FrmH + "')");
        }
        /// <summary>
        /// 保存表单数据.
        /// </summary>
        private void SaveFrm()
        {
            this.loadingWindow.Title = "正在保存数据...";
            this.loadingWindow.Show();

            FF.CCFlowAPISoapClient saveWorkNode = Glo.GetCCFlowAPISoapClientServiceInstance();
            saveWorkNode.Node_SaveWorkAsync(Glo.FK_Flow, Glo.FK_Node, Glo.WorkID, Glo.UserNo,
                this.GenerFrmDataSet().ToXml(true, false));
            saveWorkNode.Node_SaveWorkCompleted += new EventHandler<FF.Node_SaveWorkCompletedEventArgs>(saveWorkNode_Node_SaveWorkCompleted);
        }
        /// <summary>
        /// 处理保存返回的结果.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void saveWorkNode_Node_SaveWorkCompleted(object sender, FF.Node_SaveWorkCompletedEventArgs e)
        {
            /*保存成功后
           * 1, 可能返回异常
           * 2，
           */
            this.loadingWindow.DialogResult = true;
        }
    }
}
