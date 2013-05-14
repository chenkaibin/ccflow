using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CCFlow.WF.CCForm
{
    public partial class WF_ImgAth : BP.Web.WebPage
    {
        #region 属性.
        public int H
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["H"]);
                }
                catch
                {
                    return 120;
                }
            }
        }
        public int W
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["W"]);
                }
                catch
                {
                    return 100;
                }
            }
        }
        public string ImgAth
        {
            get
            {
                return this.Request.QueryString["ImgAth"];
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string url = "../../DataUser/ImgAth/Def.jpg";
                string newName = this.ImgAth + "_" + this.MyPK + ".png";
                string sourceFile = Server.MapPath("../../DataUser/ImgAth/Upload/" + newName);
                if (System.IO.File.Exists(sourceFile))
                    url = "../../DataUser/ImgAth/Upload/" + newName;
                else
                    url = "../../DataUser/ImgAth/Def.jpg";

                txtPhotoUrl.Text = url;
                Page.ClientScript.RegisterStartupScript(this.GetType(),
                "js", "<script>ImageCut('" + url + "','" + W + "','" + H + "' )</script>");
            }
        }
        //确定按钮
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string str = txtPhotoUrl.Text.ToString();
            string myName = this.ImgAth + "_" + this.MyPK;

            string type = str.Substring(str.LastIndexOf(".") + 1); //得到文件后缀名 

            CopyFile(str, "../../DataUser/ImgAth/Data/" + myName + ".png");

            //string temp = "Temp" + BP.Web.WebUser.No + "_" + DateTime.Now.ToString("yyMMddhhmmss");
            //CopyFile(str, "../Temp/" + temp + ".png");
            
            string temp = txtPhotoUrl.Text.Substring(str.LastIndexOf('/') + 1);
            temp = temp.Replace(".png", "");
            this.WinClose(temp);
        }

        //复制文件
        public void CopyFile(string SourceFile, string ObjectFile)
        {
            string sourceFile = Server.MapPath(SourceFile);
            string objectFile = Server.MapPath(ObjectFile);
            if (System.IO.File.Exists(sourceFile))
            {
                System.IO.File.Copy(sourceFile, objectFile, true);
            }
        }
        //取消按钮
        protected void btnCancle_Click(object sender, EventArgs e)
        {
        }
    }
}