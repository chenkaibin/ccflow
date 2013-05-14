using System;

namespace BP.Report
{
	public class UCHtmlRptBase  : System.Web.UI.UserControl
	{
		protected string HTMLContent
		{
			get
			{
				return this.Session["HTMLContent"] as string;
			}
			set
			{
				this.Session["HTMLContent"] = value;
			}
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{ 
			writer.Write( HTMLContent );
			//HTMLContent = "";
		}

	}
}
