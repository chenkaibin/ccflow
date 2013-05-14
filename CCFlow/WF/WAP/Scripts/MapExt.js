function DDLAnsc(e, ddlChild, fk_mapExt) {
    var json_data = { "Key": e, "FK_MapExt": fk_mapExt };
    $.ajax({
        type: "get",
        url: "/WF/CCForm/HanderMapExt.ashx",
        data: json_data,
        beforeSend: function (XMLHttpRequest) {
            //ShowLoading();
        },
        success: function (data, textStatus) {
            $("#" + ddlChild).empty();
            var dataObj = eval("(" + data + ")"); //转换为json对象 
            $.each(dataObj.Head, function (idx, item) {
                $("#" + ddlChild).append("<option value='" + item.No + "'>" + item.Name + "</option");
            });
        },
        complete: function (XMLHttpRequest, textStatus) {
            //HideLoading();
        },
        error: function () {
            //请求出错处理
        }
    });
}

function FullDtl(key, fk_mapExt) {

    var json_data = { "Key": key, "FK_MapExt": fk_mapExt, "DoType": "ReqDtlFullList" };
    $.ajax({
        type: "get",
        url: "/WF/CCForm/HanderMapExt.ashx",
        data: json_data,
        beforeSend: function (XMLHttpRequest) {
            //ShowLoading();
        },
        success: function (data, textStatus) {
            var dataObj = eval("(" + data + ")"); //转换为json对象.

            for (var i in dataObj.Head) {
                if (typeof (i) == "function")
                    continue;

                for (var k in dataObj.Head[i]) {
                    var fullDtl = dataObj.Head[i][k];
                    //  alert('您确定要填充从表吗?，里面的数据将要被删除。' + key + ' ID= ' + fullDtl);
                    var frm = document.getElementById('F' + fullDtl);
                    var src = frm.src;
                    var idx = src.indexOf("&Key");
                    if (idx == -1)
                        src = src + '&Key=' + key + '&FK_MapExt=' + fk_mapExt;
                    else
                        src = src.substring(0, idx) + '&Key=' + key + '&FK_MapExt=' + fk_mapExt;
                    frm.src = src;
                }
            }
        },
        complete: function (XMLHttpRequest, textStatus) {
            //HideLoading();
        },
        error: function () {
            //请求出错处理
        }
    });
}

function FullCtrlDDL(key, ctrlIdBefore, fk_mapExt) {
    var json_data = { "Key": key, "FK_MapExt": fk_mapExt, "DoType": "ReqDDLFullList" };
    $.ajax({
        type: "get",
        url: "/WF/CCForm/HanderMapExt.ashx",
        data: json_data,
        beforeSend: function (XMLHttpRequest) {
            //ShowLoading();
        },
        success: function (data, textStatus) {
            var dataObj = eval("(" + data + ")"); //转换为json对象 
            var beforeID = ctrlIdBefore.substring(0, ctrlIdBefore.indexOf('TB_'));
            var endId = ctrlIdBefore.substring(ctrlIdBefore.lastIndexOf('_'));

            for (var i in dataObj.Head) {
                if (typeof (i) == "function")
                    continue;

                for (var k in dataObj.Head[i]) {
                    var fullDDLID = dataObj.Head[i][k];

                    FullCtrlDDLDB(key, fullDDLID, beforeID, endId, fk_mapExt);
                }
            }
        },
        complete: function (XMLHttpRequest, textStatus) {
            //HideLoading();
        },
        error: function () {
            //请求出错处理
        }
    });
}
function FullCtrlDDLDB(e, ddlID, ctrlIdBefore, endID, fk_mapExt) {
    var json_data = { "Key": e, "FK_MapExt": fk_mapExt, "DoType": "ReqDDLFullListDB", "MyDDL": ddlID };

    $.ajax({
        type: "get",
        url: "/WF/CCForm/HanderMapExt.ashx",
        data: json_data,
        beforeSend: function (XMLHttpRequest) {
            //ShowLoading();
        },
        success: function (data, textStatus) {

            endID = endID.replace('_', '');
            if (endID != parseInt(endID)) {
                endID = "";
            } else {
                endID = "_" + endID;
            }

            var id = ctrlIdBefore + "DDL_" + ddlID + "" + endID;

            $("#" + id).empty();

            var dataObj = eval("(" + data + ")"); //转换为json对象 
            // alert(data);
            $.each(dataObj.Head, function (idx, item) {
                $("#" + id).append("<option value='" + item.No + "'>" + item.Name + "</option");
            });
        },
        complete: function (XMLHttpRequest, textStatus) {
            //HideLoading();
        },
        error: function () {
            //请求出错处理
        }
    });
}

function FullCtrl(e, ctrlIdBefore, fk_mapExt) {
    var json_data = { "Key": e, "FK_MapExt": fk_mapExt, "DoType": "ReqCtrl" };
    $.ajax({
        type: "get",
        url: "/WF/CCForm/HanderMapExt.ashx",
        data: json_data,
        beforeSend: function (XMLHttpRequest) {
            //ShowLoading();
        },
        success: function (data, textStatus) {
            var dataObj = eval("(" + data + ")"); //转换为json对象 
            var beforeID = ctrlIdBefore.substring(0, ctrlIdBefore.indexOf('TB_'));
            var endId = ctrlIdBefore.substring(ctrlIdBefore.lastIndexOf('_'));
            for (var i in dataObj.Head) {
                if (typeof (i) == "function")
                    continue;

                for (var k in dataObj.Head[i]) {
                    if (k == 'No' || k == 'Name')
                        continue;

                 //   alert(k);

                    $("#" + beforeID + 'TB_' + k).val(dataObj.Head[i][k]);
                    $("#" + beforeID + 'TB_' + k + endId).val(dataObj.Head[i][k]);
                }
            }
        },
        complete: function (XMLHttpRequest, textStatus) {
            //HideLoading();
        },
        error: function () {
            //请求出错处理
        }
    });
}

function openDiv(e, tbID) {
    //alert(document.getElementById("divinfo").style.display);
    if (document.getElementById("divinfo").style.display == "none") {
        var txtObject = document.getElementById(tbID);
        var orgObject = document.getElementById("divinfo");

        var rect = getoffset(txtObject);
        orgObject.style.top = rect[0] + 22;
        orgObject.style.left = rect[1];
        orgObject.style.display = "block";
        txtObject.focus();
    }
}
function getoffset(e) {
    var t = e.offsetTop;
    var l = e.offsetLeft;
    while (e = e.offsetParent) {
        t += e.offsetTop;
        l += e.offsetLeft;
    }
    var rec = new Array(1);
    rec[0] = t;
    rec[1] = l;
    return rec
}
// ********************** 根据关键字动态查询. ******************************** //
var oldValue = "";
var highlightindex = -1;
function DoAnscToFillDiv(sender, e, tbid, fk_mapExt) {

    openDiv(sender, tbid);
    var myEvent = event || window.event;
    var myKeyCode = myEvent.keyCode;
    //获得ID为divinfo里面的DIV对象   
    var autoNodes = $("#divinfo").children("div");
    if (myKeyCode == 38) {
        if (highlightindex != -1) {
            autoNodes.eq(highlightindex).css("background-color", "Silver");
            autoNodes.eq(highlightindex).css("color", "Black");
            if (highlightindex == 0) {
                highlightindex = autoNodes.length - 1;
            }
            else {
                highlightindex--;
            }
        }
        else {
            highlightindex = autoNodes.length - 1;
        }
        autoNodes.eq(highlightindex).css("background-color", "blue");
        autoNodes.eq(highlightindex).css("color", "white");

    }
    else if (myKeyCode == 40) {
        if (highlightindex != -1) {
            autoNodes.eq(highlightindex).css("background-color", "Silver");
            autoNodes.eq(highlightindex).css("color", "black");
            highlightindex++;
        }
        else {
            highlightindex++;
        }

        if (highlightindex == autoNodes.length) {
            autoNodes.eq(autoNodes.length).css("background-color", "Silver");
            autoNodes.eq(autoNodes.length).css("color", "black");
            highlightindex = 0;
        }
        autoNodes.eq(highlightindex).css("background-color", "blue");
        autoNodes.eq(highlightindex).css("color", "white");
    }
    else if (myKeyCode == 13) {
        if (highlightindex != -1) {

            //获得选中的那个的文本值
            var textInputText = autoNodes.eq(highlightindex).text();
            var strs = textInputText.split('|');
            autoNodes.eq(highlightindex).css("background-color", "Silver");
            $("#" + tbid).val(strs[0]);
            $("#divinfo").hide();
            oldValue = strs[0];

            //执行填充其它的控件.
            FullCtrl(oldValue, tbid, fk_mapExt);

            //执行个性化填充下拉框.
            FullCtrlDDL(oldValue, tbid, fk_mapExt);

            //执行填充从表.
            FullDtl(oldValue, fk_mapExt);

            highlightindex = -1;
        }
    }
    else {
        if (e != oldValue) {
            $("#divinfo").empty();
            var json_data = { "Key": e, "FK_MapExt": fk_mapExt };
            $.ajax({
                type: "get",
                url: "/WF/CCForm/HanderMapExt.ashx",
                data: json_data,
                beforeSend: function (XMLHttpRequest, fk_mapExt) {
                    //ShowLoading();
                },
                success: function (data, textStatus) {

                    /* 如何解决与文本框的宽度与下拉框的一样宽。*/

                    //alert($("#" + tbid));

                    if (data != "") {
                        highlightindex = -1;
                        dataObj = eval("(" + data + ")"); // 转换为json对象 
                        $.each(dataObj.Head, function (idx, item) {
                            $("#divinfo").append("<div style='" + itemStyle + "' name='" + idx + "' onmouseover='MyOver(this)' onmouseout='MyOut(this)' onclick=\"ItemClick('" + sender.id + "','" + item.No + "','" + tbid + "','" + fk_mapExt + "');\" value='" + item.No + "'>" + item.No + '|' + item.Name + "</div>");
                        });
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    //    alert('HideLoading');
                    //HideLoading();
                },
                error: function () {
                    //     alert('ssss');
                    //请求出错处理
                }
            });
            oldValue = e;
        }
    }
}

var itemStyle = 'padding:2px;color: #000000;background-color:Silver;width:100%;border-style: none double double none;border-width: 1px;';
var itemStyleOfS = 'padding:2px;color: #000000;background-color:green;width:100%';

function ItemClick(sender, val, tbid, fk_mapExt) {

    $("#divinfo").empty();
    $("#divinfo").css("display", "none");
    highlightindex = -1;
    oldValue = val;

    $("#" + tbid).val(oldValue);

    //执行填充其它的控件.
    FullCtrl(oldValue, tbid, fk_mapExt);

    //执行个性化填充下拉框.
    FullCtrlDDL(oldValue, tbid, fk_mapExt);

    //执行填充从表.
    FullDtl(oldValue, fk_mapExt);

}

function MyOver(sender) {
    if (highlightindex != -1) {
        $("#divinfo").children("div").eq(highlightindex).css("background-color", "Silver");
    }

    highlightindex = $(sender).attr("name");
    $(sender).css("background-color", "blue");
    $(sender).css("color", "white");
}

function MyOut(sender) {
    $(sender).css("background-color", "Silver");
    $(sender).css("color", "black");
}

function hiddiv() {
    $("#divinfo").css("display", "none");
}
