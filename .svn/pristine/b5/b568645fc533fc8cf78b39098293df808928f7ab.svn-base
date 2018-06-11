$(".sidebar-on").on("click",function(e){
	if ($(this).attr("data-on")) {
		$(this).attr("data-on","");
		$(this).parent().removeClass("page-sidebar-hidden");
		$(this).children().removeClass("icon-menu-right").addClass("icon-menu-left");
		$(".page-content").removeClass("page-content-stretch");
	} else {
		$(this).attr("data-on","1");
		$(this).parent().addClass("page-sidebar-hidden");
		$(this).children().removeClass("icon-menu-left").addClass("icon-menu-right");
		$(".page-content").addClass("page-content-stretch");
	}
});

(function(){
	var wh = $(window).height();
	var ch = $(".page-content").height();
	if ((wh-50) > ch) {
		$(".page-content").height((wh-50)+"px");
	}

	var bh = $("body").height();

	if (wh >= bh) {
		$("body").height(wh+"px");
	} else {
	    if (!ch) {
	        $("body").height((bh + 100) + "px");
	    }
	}
})();

(function(){
	var _COOKIE = {
		get:function(key){
			var r="";
			var cookies = document.cookie.split(";");
			var data = {};
			for (var i = 0; i < cookies.length; i++) {
				var arr = cookies[i].split("=");
					arr[0] = arr[0].replace(/^\s*/,"");
				data[arr[0]] = arr[1];
			}
			if (data[key] != undefined) {
				r = data[key];
			}
			return r;
		},
		set:function(key,value){
			var expires = 0;
			var d = new Date();
			if (arguments.length===3) {
				expires = arguments[2];
				d = new Date((Date.now()/1000+expires)*1000);
			}
			var cookie_str = key+"="+value;
				cookie_str+= expires ? ";expires="+d.toUTCString(): "";
			document.cookie = cookie_str;
		}
	}

	window.Km = {
		cookie:_COOKIE
	}
})(window);
