<%@ Page Language="C#" AutoEventWireup="true" Inherits="AppDemo_LoginMac" Codebehind="LoginMac.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<%@ Register src="UC/Login.ascx" tagname="Login" tagprefix="uc1" %>
<head id="Head1" runat="server">
 <%--<object id="locator" classid="CLSID:76A64158-CB41-11D1-8B02-00600806D9B6" VIEWASTEXT></object> 
     <object id="foo" classid="CLSID:75718C9A-F029-11d1-A1AC-00C04FB6C223"></object> --%>

    <title><%=BP.SystemConfig.SysName %></title>
    <style type="text/css">
        * { margin:0; padding:0;}
        html, body, form { width:100%; height:100%; font-family:"微软雅黑"; }
        body { background:#efefef;}
        .bg { width:1280px; height:100%; position:relative; background:url(Img/LoginBJ.jpg) no-repeat 0px 0px; margin:auto; border-left:1px solid #333; border-right:1px solid #333;}
        .login { position:absolute; left:590px; top:250px; height:200px; width:350px; overflow:hidden;}
    </style>
<%--     <script event="OnObjectReady(objObject,objAsyncContext)" for="foo"> 
         if(objObject.IPEnabled != null && objObject.IPEnabled != "undefined" && objObject.IPEnabled == true) { 
             if(objObject.MACAddress != null && objObject.MACAddress != "undefined" && objObject.DNSServerSearchOrder!=null) 
                 MACAddr = objObject.MACAddress; 
             if(objObject.IPEnabled && objObject.IPAddress(0) != null && objObject.IPAddress(0) != "undefined" && objObject.DNSServerSearchOrder!=null) 
                 IPAddr = objObject.IPAddress(0); 
             if(objObject.DNSHostName != null && objObject.DNSHostName != "undefined") 
                 sDNSName = objObject.DNSHostName; 
         } 
     </script>
       <script type="text/javascript">
           var MACAddr;
           var IPAddr;
           var DomainAddr;
           var sDNSName;
           function init() {
               var service = locator.ConnectServer();
               service.Security_.ImpersonationLevel = 3;
               service.InstancesOfAsync(foo, 'Win32_NetworkAdapterConfiguration'); 
           }
           function getMac(hidMac) {
               document.getElementById(hidMac).value = unescape(MACAddr); 
           }
         
     </script>--%>
     
</head>
<body>
    <form id="form1" runat="server">
    <div class="bg">
        <div class="login">
            <uc1:Login ID="Login1" runat="server"/> 
        </div>
    </div>
   
    </form>

</body>
</html>