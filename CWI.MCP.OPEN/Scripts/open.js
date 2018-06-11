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
	if ((wh-55) > ch) {
		$(".page-content").height((wh-51 )+"px");
	}

	var bh = $("body").height();

	if (wh >= bh) {
		$("body").height(wh+"px");
	} else {
		if (!ch) {
			$("body").height((bh+100)+"px");
		}
	}
})();

(function(){
	var _COOKIE = {
		get:function(key){
		  
		    var arr, reg = new RegExp("(^| )" + key + "=([^;]*)(;|$)");
		    if (arr = document.cookie.match(reg))
		        return unescape(arr[2]);
		    else
		        return null;
		    
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

$.fn.extend({
	treeMenu: function(callback){
		this.on("click",function(e){
			var c = $(this).attr("class");
			if (c === "parent") {
				e.preventDefault();
				var o = $(this).attr("data-o");
				if (o) {
					$(this).attr("data-o","");
					$(this).next().addClass("hidden");
					callback.call(this,false);
				} else {
					$(this).attr("data-o",true);
					$(this).next().removeClass("hidden");
					callback.call(this,true);
				}
			}
		});
	}
});