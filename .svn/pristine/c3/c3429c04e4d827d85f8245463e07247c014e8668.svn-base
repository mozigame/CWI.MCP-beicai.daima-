$(function () {
    $("#btnSubmit").bind("click",function() {
        if (CheckData()) {
            SubData();
        }
    });
});

function GetUserInfo() {
    var username = Km.cookie.get("username") == null ? "" : Km.cookie.get("username");
    var password = Km.cookie.get("userpwd") == null ? "" : Km.cookie.get("userpwd");

    if (username === $("#username").val()) {
        $("#userpwd").val(password);
    }
}

function CheckData() {
    var username = $('input[name="username"]');
    var userpwd = $('input[name="userpwd"]');

    if (username.val() == "") {
        $("#dvUsername").removeClass("hidden");
        username.focus();
        return false;
    } else {
        $("#dvUsername").addClass("hidden");
    }

    if (userpwd.val() == "") {
        $("#dvUserpwd").removeClass("hidden");
        userpwd.focus();
        return false;
    } else {
        $("#dvUserpwd").addClass("hidden");
    }
    return true;
}

function SubData() {
    var errorHandler = $('.alert');
    var param = $(".form-login").serialize();
    $.ajax({
        url: "/user/doLogin",
        type: "post",
        dataType: "json",
        data: param,
        success: function (result) {
            if (result.status == "1") {
                IsChecked($("#username").val(), $("#userpwd").val());
                location.href = '/UserAccount/Index';
            }
            else {
                errorHandler.text(result.data);
                errorHandler.removeClass("hidden");
            }
        }
    });
}

function IsChecked(username,userpwd) {
    if ($('#chkMemory').is(':checked')) {
        Km.cookie.set('username', username, 604800);
        Km.cookie.set('userpwd', userpwd, 604800);
    }
}

 
