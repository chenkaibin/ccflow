using System;
using BP.Port;
using System.Windows.Forms;
using BP.WF;
using BP.Comm;
using Microsoft.Office.Tools.Ribbon;

namespace CCFlowWord2007
{
    partial class Ribbon1 : RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Methods

        /// <summary>
        /// 加载XML，创建导航
        /// </summary>
        public void LoadXml()
        {
            BP.WF.Tabs tabs = new BP.WF.Tabs();
            tabs.RetrieveAll();

            BP.WF.GroupFuncs gs = new BP.WF.GroupFuncs();
            gs.RetrieveAll();

            BP.WF.Funcs fs = new BP.WF.Funcs();
            fs.RetrieveAll();

            this.SuspendLayout();
            this.Tabs.Clear();
            int i = 1;
            foreach (BP.WF.Tab tb in tabs)
            {
                i++;
                RibbonTab mytab = Factory.CreateRibbonTab();
                mytab.Label = tb.Name;
                mytab.Name = "t" + tb.No + i;
                if (i == 2)
                    mytab.ControlId.ControlIdType = RibbonControlIdType.Custom;
                else
                    mytab.ControlId.ControlIdType = RibbonControlIdType.Office;

                mytab.SuspendLayout();

                foreach (BP.WF.GroupFunc g in gs)
                {
                    if (g.FK_Tab != tb.No)
                        continue;

                    RibbonGroup group = Factory.CreateRibbonGroup();
                    group.Name = "s" + g.No;
                    group.Label = g.Name;
                    group.DialogLauncherClick += new RibbonControlEventHandler(Btn_Click);
                    group.SuspendLayout();

                    foreach (BP.WF.Func f in fs)
                    {
                        if (f.FK_Group != g.No)
                            continue;

                        switch (f.CtlType)
                        {
                            case "Btn":
                                RibbonButton btn = Factory.CreateRibbonButton();
                                btn.Name = "Btn_" + f.No;
                                btn.Label = f.Name;
                                btn.Tag = f;
                                try
                                {
                                    if (f.IsIcon)
                                    {
                                        btn.Image = System.Drawing.Image.FromFile(BP.WF.Glo.PathOfTInstall + "\\Img\\" + f.No + ".gif");
                                        btn.ShowImage = true;
                                    }
                                }
                                catch
                                {
                                }
                                btn.Click += new RibbonControlEventHandler(Btn_Click);
                                group.Items.Add(btn);
                                break;
                            default:
                                RibbonLabel lab = Factory.CreateRibbonLabel();
                                lab.Name = "Lab_" + f.No;
                                lab.Label = f.Name;
                                lab.Tag = f;
                                group.Items.Add(lab);
                                break;
                        }
                    }
                    group.ResumeLayout(false);
                    group.PerformLayout();
                    mytab.Groups.Add(group);
                } // End add to Group.

                mytab.ResumeLayout(false);
                mytab.PerformLayout();
                this.Tabs.Add(mytab);
            } // End add to Tab.

            this.ResumeLayout(false);
            this.RibbonType = "Microsoft.Word.Document";
            //  this.RibbonType = "Microsoft.PowerPoint.Presentation";
            this.Load += new RibbonUIEventHandler(Ribbon1_Load);
        }

        /// <summary>
        /// 执行btn本地事件
        /// </summary>
        /// <param name="func"></param>
        /// <param name="btn"></param>
        public void Do(BP.WF.Func func, RibbonButton btn)
        {
            switch (func.No)
            {
                case "LogOut":
                    if (MessageBox.Show("您确定要注销吗？", "执行确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        return;

                    WebUser.SignOut();

                    this.SetState();
                    break;
                case "Login":
                    FrmLogin fl = new FrmLogin();
                    fl.ShowDialog();
                    this.SetState();
                    break;
                case "ChUser":
                    FrmLogin fm = new FrmLogin();
                    fm.ShowDialog();
                    break;
                case "WKInfo":
                    string msg = "\t\n No=" + WebUser.No;
                    msg += "\t\n FK_Flow=" + WebUser.FK_Flow;
                    msg += "\t\n FK_Node=" + WebUser.FK_Node;
                    msg += "\t\n WorkID=" + WebUser.WorkID;
                    MessageBox.Show(msg);
                    break;
                case "About":
                    AboutBox ab = new AboutBox();
                    ab.ShowDialog();
                    break;
                case "Save":
                    this.DoSave();
                    break;
                case "SaveTo":
                default:
                    MessageBox.Show("功能未实现：" + func.No + " " + func.Name);
                    break;
            }
        }

        /// <summary>
        /// 获得指定名称的RibbonBtn
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public RibbonButton GetBtn(string name)
        {
            foreach (RibbonTab tab in this.Tabs)
            {
                foreach (RibbonGroup g in tab.Groups)
                {
                    RibbonButton btn;
                    for (int i = 0; i <= g.Items.Count; i++)
                    {
                        try
                        {
                            btn = g.Items[i] as RibbonButton;
                            if (btn == null)
                                continue;

                            if (btn.Name == name)
                                return btn;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            MessageBox.Show("@没有找到Name=" + name + " 的按钮");
            return null;
        }

        /// <summary>
        /// 执行保存公文
        /// </summary>
        public void DoSave()
        {
            Globals.ThisAddIn.DoSave();
        }
        #endregion

        #region btn Events

        void Btn_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                RibbonButton btn = (RibbonButton)sender;
                BP.WF.Func func = (BP.WF.Func)btn.Tag;
                switch (func.DoType)
                {
                    case "RunExe":
                        System.Diagnostics.Process.Start(func.Tag);
                        return;
                    case "RunIE":
                        switch (func.No)
                        {

                            case "Send": //要执行签发.
                                if (WebUser.FK_Flow == null)
                                {
                                    MessageBox.Show("您没有执行公文拟稿的过程不能签发。");
                                    return;
                                }
                                if (WebUser.WorkID == 0)
                                {
                                    Work wk = new Work();
                                    wk.FK_Dept = WebUser.FK_Dept;
                                    wk.Title = "公文拟稿-" + DateTime.Now.ToString("MM月dd日hh时mm分");
                                    wk.NodeID = WebUser.FK_Node;
                                    wk.RDT = DateTime.Now.ToString("yyyy-MM-dd");
                                    wk.Rec = WebUser.No;
                                    wk.Insert();
                                    WebUser.WorkID = wk.OID;
                                }
                                else
                                {
                                    /*判断是否已经处理了。*/
                                    WebUser.HisWork = new Work(WebUser.FK_Node, WebUser.WorkID);
                                    switch (WebUser.HisWork.HisNodeState)
                                    {
                                        case NodeState.Init:
                                        case NodeState.Back:
                                            break;
                                        default:
                                            MessageBox.Show("工作已经转入了下一个环节，您不能在处理了。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                            return;
                                    }
                                }
                                break;
                            case "Return":
                                break;
                            case "UnSend":
                                if (MessageBox.Show("您确定要撤消发送吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    return;
                                break;
                            case "Del":
                                if (MessageBox.Show("您确定要执行删除吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    return;
                                break;
                            default:
                                break;
                        }

                        FrmIE ie = new FrmIE();
                        ie.Width = func.Width;
                        ie.Height = func.Height;

                        string tag = func.Tag;
                        tag = tag.Replace("@Serv", BP.WF.Glo.WFServ);
                        ie.ShowUrl(tag + "&UserNo=" + WebUser.No + "&FK_Flow=" + BP.Port.WebUser.FK_Flow + "&FK_Node=" + WebUser.FK_Node + "&WorkID=" + WebUser.WorkID);
                        ie.Text = "您好：" + WebUser.No + "，" + WebUser.Name + "。  -  " + func.Name;
                        ie.ShowInTaskbar = false;
                        ie.HisRibbon1 = this;
                        ie.ShowDialog();
                        this.SetState();
                        return;
                    default:
                        try
                        {
                            this.Do(func, btn);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("执行" + func.Name + "出现错误。" + ex.Message);
                        }
                        break;
                }

                this.SetState();
            }
            catch (Exception ex)
            {
                SetState();
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 设置 btn 状态

        /// <summary>
        /// 设置按钮状态
        /// </summary>
        public void SetState()
        {
            //功能按钮
            this.Btn_Start.Enabled = false;
            this.Btn_Send.Enabled = false;
            this.Btn_Return.Enabled = false;
            this.Btn_Del.Enabled = false;
            this.Btn_FW.Enabled = false;
            this.Btn_UnSend.Enabled = false;    //Node中未有
            this.Btn_Rpt.Enabled = false;
            this.Btn_Save.Enabled = false;
            this.Btn_Attachment.Enabled = false;

            //流程按钮
            this.Btn_EmpWorks.Enabled = false;
            this.Btn_Runing.Enabled = false;
            this.Btn_View.Enabled = false;

            this.Btn_Login.Label = "登录";

            if (!string.IsNullOrEmpty(WebUser.No))
            {
                this.Btn_Start.Enabled = true;

                this.Btn_EmpWorks.Enabled = true;
                this.Btn_Runing.Enabled = true;
                this.Btn_View.Enabled = true;

                this.Btn_Login.Label = "更换用户";

                if (WebUser.CurrentNode != null)
                {
                    this.Btn_Del.Enabled = WebUser.CurrentNode.DelEnable.HasValue && WebUser.CurrentNode.DelEnable.Value;
                    this.Btn_Send.Enabled = WebUser.CurrentNode.SendEnable.HasValue &&
                                            WebUser.CurrentNode.SendEnable.Value;
                    this.Btn_Return.Enabled = WebUser.CurrentNode.ReturnRole != WorkFlow.ReturnRoleKind.UnEnable;
                    this.Btn_FW.Enabled = WebUser.CurrentNode.ShiftEnable.HasValue &&
                                          WebUser.CurrentNode.ShiftEnable.Value;
                    this.Btn_Rpt.Enabled = WebUser.CurrentNode.TrackEnable.HasValue &&
                                           WebUser.CurrentNode.TrackEnable.Value;
                    this.Btn_Save.Enabled = WebUser.CurrentNode.SaveEnable.HasValue &&
                                            WebUser.CurrentNode.SaveEnable.Value;
                    this.Btn_Attachment.Enabled = WebUser.CurrentNode.FJOpen != WorkFlow.AttachmentRoleKind.Close;

                    this.Btn_UnSend.Enabled = WebUser.HisWork != null && WebUser.HisWork.HisNodeState == NodeState.Complete;
                }
            }
        }

        #endregion

        #region RibbonBtn

        /// <summary>
        /// 流程轨迹
        /// </summary>
        public RibbonButton Btn_Rpt
        {
            get
            {
                return this.GetBtn("Btn_Rpt");
            }
        }

        /// <summary>
        /// 撤消发送
        /// </summary>
        public RibbonButton Btn_UnSend
        {
            get
            {
                return this.GetBtn("Btn_UnSend");
            }
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        public RibbonButton Btn_Del
        {
            get
            {
                return this.GetBtn("Btn_Del");
            }
        }

        /// <summary>
        /// 待办公文
        /// </summary>
        public RibbonButton Btn_EmpWorks
        {
            get
            {
                return this.GetBtn("Btn_EmpWorks");
            }
        }

        /// <summary>
        /// 在途公文
        /// </summary>
        public RibbonButton Btn_Runing
        {
            get
            {
                return this.GetBtn("Btn_Runing");
            }
        }

        /// <summary>
        /// 公文查询
        /// </summary>
        public RibbonButton Btn_View
        {
            get
            {
                return this.GetBtn("Btn_View");
            }
        }

        /// <summary>
        /// 附件
        /// </summary>
        public RibbonButton Btn_Attachment
        {
            get
            {
                return this.GetBtn("Btn_Ath");
            }
        }

        /// <summary>
        /// 登录/更换用户
        /// </summary>
        public RibbonButton Btn_Login
        {
            get
            {
                return this.GetBtn("Btn_Login");
            }
        }

        /// <summary>
        /// 注销
        /// </summary>
        public RibbonButton Btn_LogOut
        {
            get
            {
                return this.GetBtn("Btn_LogOut");
            }
        }

        /// <summary>
        /// 退回
        /// </summary>
        public RibbonButton Btn_Return
        {
            get
            {
                return this.GetBtn("Btn_Return");
            }
        }

        /// <summary>
        /// 移交
        /// </summary>
        public RibbonButton Btn_FW
        {
            get
            {
                return this.GetBtn("Btn_FW");
            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        public RibbonButton Btn_Send
        {
            get
            {
                return this.GetBtn("Btn_Send");
            }
        }

        /// <summary>
        /// 保存到网络
        /// </summary>
        public RibbonButton Btn_Save
        {
            get
            {
                return this.GetBtn("Btn_Save");
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public RibbonButton Btn_SendToMail
        {
            get
            {
                return this.GetBtn("Btn_SendToMail");
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        public RibbonButton Btn_SaveAs
        {
            get
            {
                return this.GetBtn("Btn_SaveAs");
            }
        }

        /// <summary>
        /// 另存为PDF
        /// </summary>
        public RibbonButton Btn_SaveAsPDF
        {
            get
            {
                return this.GetBtn("Btn_SaveAsPDF");
            }
        }

        /// <summary>
        /// 拟定公文
        /// </summary>
        public RibbonButton Btn_Start
        {
            get
            {
                return this.GetBtn("Btn_Start");
            }
        }

        /// <summary>
        /// 发送到U盘
        /// </summary>
        public RibbonButton Btn_SaveToU
        {
            get
            {
                return this.GetBtn("Btn_SaveToU");
            }
        }

        #endregion


        public Ribbon1()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            //if (BP.Port.WebUser.LoadProfile() == false)
            //{
            //    FrmLogin lg = new FrmLogin();
            //    DialogResult dl = lg.ShowDialog();
            //    if (dl != DialogResult.OK)
            //        return;
            //}

            try
            {
                this.LoadXml();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            BP.Port.WebUser.HisRib = this;
            //  CCFlowWord2007.Globals.ThisAddIn.HisRibbon1 = this;
            return;
        }

        #endregion

        //internal RibbonTab tab1;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon1 Ribbon1
        {
            get { return this.GetRibbon<Ribbon1>(); }
        }
    }
}
