<%@ Page Language="C#" AutoEventWireup="true" Inherits="AppDemo_Left" Codebehind="Left.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>导航</title>
    <link href="css/basic.css" rel="stylesheet" type="text/css" />
    <link href="css/base.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript">
        function myrefresh() {
            window.location.reload();
        }
        setTimeout('myrefresh()', 100000); //指定1秒刷新一次 
   </script> 
</head>
<body>
    <form id="form1" runat="server">
    <div class="JS_zhut">
        <div class="JS_zhut_n">
            <div class="left fl">
	            <div class="title"><span class="fl ml20">系统导航</span></div>
                <ul id="Ul1" class="menu" runat="server">
              <%--  <%
                    BP.WF.XML.ToolBars ens = new BP.WF.XML.ToolBars();
                    ens.RetrieveAll();
                    foreach (BP.WF.XML.ToolBar en in ens)
                    {
                        %>
                    <li runat="server" id="Li4" style=" background:url(Img/Menu/.jpg) -1px 0px"><a  href="../WF/StartSmall.aspx" target="main"><%=en.Name%> </a></li>
                  <% }  %>--%>
                    <li runat="server" id="Li2s" style=" background:url(Img/Menu/Start.jpg) -1px 0px"><a  href="../WF/StartSmall.aspx" target="main">发起</a></li>
                    <li runat="server" id="get" style=" background:url(Img/Menu/EmpWorks.jpg) -1px 0px"><a  href="../WF/EmpWorksSmall.aspx" target="main">待办 (<%= EmpWorks%>)</a></li>
                    <li runat="server" id="Li3" style=" background:url(Img/Menu/CC.jpg) -1px 0px"><a  href="../WF/CCSmall.aspx" target="main">抄送 (<%= CCNum%>)</a></li>
                    <li runat="server" id="Li5" style=" background:url(Img/Menu/HungUp.jpg) -1px 0px"><a  href="../WF/HungUpListSmall.aspx" target="main">挂起 (<%= HungUpNum%>)</a></li>
                    <li runat="server" id="Li1" style=" background:url('Img/Menu/Ruing.jpg') -1px 0px"><a  href="../WF/RuningSmall.aspx" target="main">在途</a></li>
                    <li runat="server" id="over" style=" background:url(Img/Menu/Search.jpg) -1px 0px"><a  href="../WF/FlowSearchSmall.aspx" target="main">查询 </a></li>
                    <li runat="server" id="Li4" style=" background:url(Img/Menu/SearchKey.jpg) -1px 0px"><a  href="../WF/KeySearch.aspx" target="main">关键字查询 </a></li>
                    <%--<li runat="server" id="cld" style=" background:url(Img/Menu/Rili.jpg) -1px 0px"><a  href="../WF/CalendarSmall.aspx" target="main">日历</a></li>--%>
                    <li runat="server" id="Li2" style=" background:url(Img/Menu/GetTask.jpg) -1px 0px"><a  href="../WF/GetTaskSmall.aspx" target="main">取回审批</a></li>
                    <li runat="server" id="Li6" style=" background:url(Img/Menu/Emps.jpg) -1px 0px"><a  href="../WF/EmpsSmall.aspx" target="main">成员 </a></li>
                    <li runat="server" id="end" style=" background:url(Img/Menu/Setting.jpg) -1px 0px"><a  href="../WF/ToolsSmall.aspx" target="main">设置 </a></li>
--%>
                </ul>
            </div>
        </div>
    </div>
    </form>
</body>
</html>

