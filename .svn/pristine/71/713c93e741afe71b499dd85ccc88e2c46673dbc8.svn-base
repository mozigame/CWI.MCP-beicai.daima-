/// <reference path="zepto.min.js" />
var common = {
    showErrorTip: function (msg) {
        /// <summary>show Error Msg</summary>
        /// <param name="msg" type="string">msg</param>
        var tooltips = $('.js_tooltips');
        if (tooltips.css('display') != 'none') {
            return;
        }
        // 如果有`animation`, `position: fixed`不生效
        $('.page.cell').removeClass('slideIn');
        tooltips.html(msg);
        tooltips.show();
        setTimeout(function () {
            tooltips.hide();
        }, 2000);
    },
    showMsg: function (options) {
        options = $.extend({ title: '警告', text: '警告内容' }, options);
        var $alert = $('.weui_dialog_alert');
        $alert.find('.weui_dialog_title').text(options.title);
        $alert.find('.weui_dialog_bd').text(options.text);
        $alert.on('touchend click', '.weui_btn_dialog', function (e) {
            $alert.hide();
            e.preventDefault();
        });
        $alert.show();
    },
    showSuccessMsg: function (options) {
        options = $.extend({ title: '操作成功', text: '操作内容' }, options);
        var $success = $('.weui_success_msg');
        $success.find('.weui_msg_title').text(options.title);
        $success.find('.weui_msg_desc').text(options.text);
        $success.on('touchend click', '.weui_btn', function (e) {
            $success.hide();
            e.preventDefault();
        });
        $success.show();
    },
    showLoadingToast: function (msg) {
        var $loadingToast = $('#loadingToast');
        $('#loadingToast p.weui_toast_content').text(msg);
        if ($loadingToast.css('display') != 'none') {
            return;
        }
        $loadingToast.show();
    },
    hideLoadingToast: function () {
        var $loadingToast = $('#loadingToast');
        if ($loadingToast.css('display') != 'block') {
            return;
        }
        $loadingToast.hide();
    },
    showToast: function (msg) {
        var $toast = $('#toast');
        if ($toast.css('display') != 'none') {
            return;
        }
        $('#toast p.weui_toast_content').text(msg);
        $toast.show();
        setTimeout(function () {
            $toast.hide();
        }, 2000);
    },
    //content 滚动
    scrollPage: function (url, params, getContent) {
        var pageIndex = 0;
        var pageCount = 10;
        $('.content').dropload({
            scrollArea: window,
            domUp: {
                domClass: 'dropload-up',
                domRefresh: '<div class="dropload-refresh">↓下拉刷新</div>',
                domUpdate: '<div class="dropload-update">↑释放更新</div>',
                domLoad: '<div class="dropload-load"><span class="loading"></span>加载中...</div>'
            },
            domDown: {
                domClass: 'dropload-down',
                domRefresh: '<div class="dropload-refresh">↑上拉加载更多</div>',
                domLoad: '<div class="dropload-load"><span class="loading"></span>加载中...</div>',
                domNoData: '<div class="dropload-noData">暂无数据</div>'
            },
            loadUpFn: function (me) {
                pageIndex = 1;
                this.loadPageData(me, true, url, getContent);
            },
            loadDownFn: function (me) {
                if (pageIndex >= pageCount) {
                    // 锁定
                    me.lock();
                    // 无数据
                    me.noData();
                }
                pageIndex++;
                this.loadPageData(me, false, url, getContent);
            },
            loadPageData: function (me, isReload, url, getContent) {
                var p = $.extend({ PageSize: 5, PageIndex: pageIndex }, params);
                $.getJSON(url, p, function (res) {
                    if (res.status == "1") {
                        pageCount = res.data.PageCount;
                        var divs = getContent(res);
                        if (isReload) {
                            $(".list").html(divs);
                        }
                        else {
                            $(".list").append(divs);
                        }
                        me.resetload();
                    } else {
                        common.showMsg(res.data);
                    }
                })
            },
            threshold: 50
        });
    },
    getFileExtension: function (fileName) {
        var extension = fileName.split('.').pop().toLowerCase();
        return extension;
    },
    getSrcByFileName: function (fileName) {
        var extension = this.getFileExtension(fileName);
        var fold = "/theme/images/";
        switch (extension) {
            case "txt":
                return fold + "txt.png";
                break;
            case "pdf":
                return fold + "pdf.png";
                break;
            case "doc":
            case "docx":
                return fold + "doc.png";
                break;
            case "xls":
            case "xlsx":
                return fold + "xls.png";
                break;
            case "ppt":
            case "pptx":
                return fold + "ppt.png";
                break;
            case "vsd":
            case "vsdx":
                return fold + "vsd.png";
                break;
            case "jpg":
            case "jpeg":
            case "bmp":
            case "png":
            case "gif":
            case "tif":
            case "tiff":
                return fold + "png.png";
                break;
            default:
                return "";
                break;
        }
    },
    getOrderStatusByCode: function (statusCode, isExpired) {
        var strOrderStatus = "";
        if (isExpired == 1) {
            strOrderStatus = "已过期";
        }
        else {
            switch (statusCode) {
                case 0:
                    strOrderStatus = "已取消";
                    break;
                case 1:
                    strOrderStatus = "未支付";
                    break;
                case 2:
                    strOrderStatus = "退款中";
                    break;
                case 3:
                    strOrderStatus = "已退款";
                    break;
                case 4:
                    strOrderStatus = "未打印";
                    break;
                case 5:
                    strOrderStatus = "已打印";
                    break;
                case 6:
                    strOrderStatus = "打印中";
                    break;
                case 7:
                    strOrderStatus = "已失效";
                    break;
                case 8:
                    strOrderStatus = "已过期";
                    break;
                case 9:
                    strOrderStatus = "打印失败";
                    break;
            }
        }
        return strOrderStatus;
    }
};

//模拟string.format("{0}","")
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({)" + i + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}
