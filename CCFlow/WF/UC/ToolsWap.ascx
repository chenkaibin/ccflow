<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.ToolWap" Codebehind="ToolsWap.ascx.cs" %>
    <script language="JavaScript" src="/WF/Comm/JS/Calendar/WdatePicker.js" defer="defer" ></script>
    <script type="text/javascript">
        function SetSelected(cb, ids) {
            //alert(ids);
            var arrmp = ids.split(',');
            var arrObj = document.all;
            var isCheck = false;
            if (cb.checked)
                isCheck = true;
            else
                isCheck = false;
            for (var i = 0; i < arrObj.length; i++) {
                if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                    for (var idx = 0; idx <= arrmp.length; idx++) {
                        if (arrmp[idx] == '')
                            continue;
                        var cid = arrObj[i].name + ',';
                        var ctmp = arrmp[idx] + ',';
                        if (cid.indexOf(ctmp) > 1) {
                            arrObj[i].checked = isCheck;
                            //                    alert(arrObj[i].name + ' is checked ');
                            //                    alert(cid + ctmp);
                        }
                    }
                }
            }
        }
    </script>

