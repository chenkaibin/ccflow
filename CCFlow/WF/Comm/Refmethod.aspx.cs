using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;

namespace BP.Web.Comm
{
	/// <summary>
	/// RefFuncLink 的摘要说明。
	/// </summary>
	public partial class UIRefMethod : WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.BPToolBar1.ButtonClick += new EventHandler(BPToolBar1_ButtonClick);

            string ensName = this.Request.QueryString["EnsName"];
            int index = int.Parse(this.Request.QueryString["Index"]);
            Entities ens = DA.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];

            //			Attrs attrs =en.EnMap.Attrs;
            //			foreach(Attr attr in attrs)
            //			{
            //				en.SetValByKey(attr.Key, this.Request.QueryString[attr.Key]);
            //			}

            //			string pk=this.Request.QueryString["PK"];
            //			if (pk==null)
            //				pk=this.Request.QueryString[en.PK];

            //rm.PK=pk;

            if (rm.HisAttrs == null || rm.HisAttrs.Count == 0)
            {
                string pk = this.RefEnKey;
                if (pk == null)
                    pk = this.Request.QueryString[en.PK];

                en.PKVal = pk;
                en.Retrieve();

                rm.HisEn = en;
                object obj = rm.Do(null);
                if (obj == null)
                {
                    this.WinClose();
                    return;
                }

                string info = obj.ToString();
                info = info.Replace("@", "<br>@");
                if (info.Contains("<"))
                    this.ToMsgPage(info);
                else
                    this.WinCloseWithMsg(info);
                return;
            }
            this.Bind(rm);

            if (this.IsPostBack == false)
            {
                this.BPToolBar1.AddBtn(NamesOfBtn.Do, rm.Title);
                this.BPToolBar1.AddSpt("sd");
                this.BPToolBar1.AddBtn(NamesOfBtn.Close);
              //  this.BPToolBar1.AddBtn(NamesOfBtn.Help);
            }

           // this.Label1.Text = this.GenerCaption(this.HisEn.EnMap.EnDesc + "" + this.HisEn.EnMap.TitleExt);

            this.Label1.Text = this.GenerCaption(en.EnMap.EnDesc + "=>" + rm.GetIcon(this.Request.ApplicationPath) + rm.Title);
        }
        public void Bind(RefMethod rm)
        {

            this.UCEn1.BindAttrs(rm.HisAttrs);
            //检查是否有选择项目。
        }

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

        private void BPToolBar1_ButtonClick(object sender, EventArgs e)
        {
            ToolbarBtn btn = (ToolbarBtn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.Do:
                    string ensName = this.Request.QueryString["EnsName"];
                    int index = int.Parse(this.Request.QueryString["Index"]);
                    Entities ens = DA.ClassFactory.GetEns(ensName);
                    Entity en = ens.GetNewEntity;
                    en.PKVal = this.Request.QueryString[en.PK];
                    en.Retrieve();

                    BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];
                    rm.HisEn = en;
                    int mynum = 0;
                    foreach (Attr attr in rm.HisAttrs)
                    {
                        if (attr.MyFieldType == FieldType.RefText)
                            continue;
                        mynum++;
                    }
                    //	object[] objs =new object[rm.HisAttrs.Count];
                    object[] objs = new object[mynum];

                    int idx = 0;
                    foreach (Attr attr in rm.HisAttrs)
                    {
                        if (attr.MyFieldType == FieldType.RefText)
                            continue;

                        switch (attr.UIContralType)
                        {
                            case UIContralType.TB:
                                switch (attr.MyDataType)
                                {
                                    case BP.DA.DataType.AppString:
                                    case BP.DA.DataType.AppDate:
                                    case BP.DA.DataType.AppDateTime:
                                        string str1 = this.UCEn1.GetTBByID("TB_" + attr.Key).Text;
                                        objs[idx] = str1;
                                        //attr.DefaultVal=str1;
                                        break;
                                    case BP.DA.DataType.AppInt:
                                        int myInt = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                        objs[idx] = myInt;
                                        //attr.DefaultVal=myInt;
                                        break;
                                    case BP.DA.DataType.AppFloat:
                                        float myFloat = float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                        objs[idx] = myFloat;
                                        //attr.DefaultVal=myFloat;
                                        break;
                                    case BP.DA.DataType.AppDouble:
                                    case BP.DA.DataType.AppMoney:
                                    case BP.DA.DataType.AppRate:
                                        decimal myDoub = decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                        objs[idx] = myDoub;
                                        //attr.DefaultVal=myDoub;
                                        break;
                                    case BP.DA.DataType.AppBoolean:
                                        int myBool = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                        if (myBool == 0)
                                        {
                                            objs[idx] = false;
                                            attr.DefaultVal = false;
                                        }
                                        else
                                        {
                                            objs[idx] = true;
                                            attr.DefaultVal = true;
                                        }
                                        break;
                                    default:
                                        throw new Exception("没有判断的数据类型．");

                                }
                                break;
                            case UIContralType.DDL:
                                try
                                {
                                    string str = this.UCEn1.GetDDLByKey("DDL_" + attr.Key).SelectedItemStringVal;
                                    objs[idx] = str;
                                    attr.DefaultVal = str;
                                }
                                catch (Exception ex)
                                {
                                    // this.ToErrorPage("获取：[" + attr.Desc + "] 期间出现错误，可能是该下拉框中没有选择项目，错误技术信息：" + ex.Message);
                                    objs[idx] = null;
                                    // attr.DefaultVal = "";
                                }
                                break;
                            case UIContralType.CheckBok:
                                if (this.UCEn1.GetCBByKey("CB_" + attr.Key).Checked)
                                    objs[idx] = "1";
                                else
                                    objs[idx] = "0";
                                attr.DefaultVal = objs[idx].ToString();
                                break;
                            default:
                                break;
                        }
                        idx++;
                    }

                    //					string pk=this.Request.QueryString["PK"];
                    //					if (pk==null)
                    //						pk=this.Request.QueryString[ en.PK ];
                    //					rm.PK=pk;

                    try
                    {
                        object obj = rm.Do(objs);
                        if (obj != null)
                        {
                            this.ToMsgPage(obj.ToString());
                        }
                        this.WinClose();
                    }
                    catch (Exception ex)
                    {
                        string msg = "";
                        foreach (object obj in objs)
                            msg += "@" + obj.ToString();

                        throw new Exception("@执行[" + ensName + "]期间出现错误：" + ex.Message + "[参数为：]" + msg);
                    }
                    return;
                case NamesOfBtn.Help:
                    this.Helper();
                    break;
                case NamesOfBtn.Close:
                    this.WinClose();
                    break;
                case NamesOfBtn.Cancel:
                    this.WinClose();
                    break;
                default:
                    throw new Exception("error id" + btn.ID);
            }
        }
	}
}
