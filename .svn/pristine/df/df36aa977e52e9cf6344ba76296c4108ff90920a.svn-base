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
        $("#userpwd").attr('type', 'password').val(Aes.Ctr.decrypt(password, "av", 128));
    }
}

function CheckData() {
    var username = $('input[name="username"]');
    var userpwd = $('input[name="userpwd"]');

    if (username.val() == "") {
        $('.alert').html("帐号不能为空");
        $('.alert').removeClass("hidden");
        username.focus();
        return false;
    } else {
        $('.alert').addClass("hidden");
    }

    if (userpwd.val() == "") {
        $('.alert').html("密码不能为空");
        $('.alert').removeClass("hidden");
        userpwd.focus();
        return false;
    } else {
        $('.alert').addClass("hidden");
    }
    return true;
}

function SubData() {
    var errorHandler = $('.alert');
    var param = $(".form-login").serialize();
    $.ajax({
        url: "/user/dologin",
        type: "post",
        dataType: "json",
        data: param,
        success: function (result) {
            if (result.status == "1") {
                IsChecked($("#username").val(), $("#userpwd").val());
                location.href = "/apply/applymanagement";
            }
            else {
                errorHandler.text(result.data);
                errorHandler.removeClass("hidden");
            }
        }
    });
}

function IsChecked(username, userpwd) {
    if ($('#chkMemory').is(':checked')) {
        Km.cookie.set('username', username, 604800);
        Km.cookie.set('userpwd', Aes.Ctr.encrypt(userpwd, "av", 128), 604800);
    }
}

 
