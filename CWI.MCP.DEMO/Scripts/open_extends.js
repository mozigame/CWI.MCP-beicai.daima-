/**
 * 
 */

function indexof(items,item){
	var index = -1;
	if (typeof Array.prototype.indexOf !== "function") {
		for (var i = 0; i < items.length; i++) {
			if(items[i] === item) index = i;
		}
	} else {
		index = items.indexOf(item);
	}
	return index;
}
/**
 * 表单验证
 * @param  {[type]} ){		var _this         [description]
 * @return {[type]}          [description]
 */
$.fn.extend({
	validateForm: function(){
		var _this = this;
		var elems = this[0].elements;
		var callback = arguments.length>0 && typeof arguments[0] === "function" ? arguments[0] : false;

		function f(l,t,v){
			switch (t) {
				case "len":
					v = v.split(",");
					l.conf.len = {
						max:v[1],
						min:v[0]
					}
				break;
				case "type":
					if (v === "1" || v === "2") {
						l.conf.type = v;
					}
				break;
				case "pass":
					var el = $(v);
					l.conf.pass = {
						"for": el
					}
				break;
				case "code":
					v = v.split(",");
					l.conf.code = {
						len: v[0],
						url: typeof v[1] === "undefined" ? false: v[1]
					}
				break;
				case "smscode":
					v = v.split(",");
					var s = v[0].split("|");
					var el = [];
					for (var i=0; i<s.length; i++) {
						el[el.length] = $(s[i]);
					}
					l.conf.smscode = {
						"for": el,
						"time":v[1],
						url: typeof v[2] === "undefined" ? false: v[2]
					}
					l.conf.submit = false;
				break;
				case "check":
					l.conf.check = v;
				break;
				case "depend":
					v = v.split(",");
					var s = v[0].split("|");
					var el = [];
					for (var i=0; i<s.length; i++) {
						el[el.length] = $(s[i]);
					}
					l.conf.depend = {
						dep:el,
						mess:v[1]
					}
				break;
			}
		}

		var Validate = {
			elems:[],
			signal:[],
			len:function(v,c){
				var r = "长度应在"+c.len.min+"到"+c.len.max;
				if (v != "" && (c.len.min <= v.length &&  v.length <= c.len.max)) {
					r = false;
				}
				return r;
			},
			type:function(v,c){
				var n = {1:"数字",2:"字母"}
				var r = "内容应全为"+n[c.type];
				// 1 = number 2 = 字母
				if (c.type === "1") {
					var reg = /\w*/;
					if (reg.test(v)) {
						r = false;
					}
				} else if(c.type === "2"){
					var reg = /\d*/;
					if (reg.test(v)) {
						r = false;
					}
				}
				return r;
			},
			mail:function(v,c){
				var r = "邮箱为空或者格式不正确!";
				var reg = /[a-z|A-Z|0-9]{1,32}@[a-z|A-Z|0-9]*\.[a-z]{1,128}/;
				if (reg.test(v)) {
					r = false;
				}
				return r;
			},
			phone:function(v,c){
				var r = "手机号码为空或者不正确!";
				var reg = /^1[3-8]\d{9}$/;
				if (reg.test(v)) {
					r = false;
				}
				return r;
			},
			pass:function(v,c){
				var r = "两次密码不符!";
				var rv = c.pass["for"].val();
				if (v === rv) {
					r = false;
				}
				return r;
			},
			code:function(v,c){
				// len,url
				// 如果url存在即异步验证
				// len 长度
				var r = "验证码为空或者不正确!";
				if (v.length === Number(c.code.len)) {
					r = false;
				}
				
				var _obj = this;

				if (!r && c.code.url) {
					var el = arguments[2];
					var name = el.name;
					$.ajax(c.code.url+"?"+name+"="+v,{
						async:false,
						type:"GET",
						dataType:"json",
						success:function(result){
							if (!result.status) {
								r = "验证码错误,请重新获取!";
							} else {
								r = false;
							}
						}
					});
				}
				return r;
			},
			smscode:function(v,c){
				var r = false,m,els = c.smscode["for"],s = false,el,
					_this = arguments[2],
					_r;

				if ($(_this).attr("data-on")) {return s;}
				for (var e=0; e<els.length; e++) {
					el = els[e][0];
					for (var i = 0; i < el.conf.method.length; i++) {
						m = el.conf.method[i];
						if (_r = this[m](el.value,el.conf,el)) {
							r = _r;
							$(_this).parent().append('<div class="text-danger">'+r+'</div>');
						};
					}
				}

				if (!r) {
					var param = {};
						param[els[1][0].name] = els[1][0].value;
					$.post(c.smscode.url,param,function(result){
						if (result.status) {
							$(_this).attr("data-on","1");
							var i = c.smscode.time;
							_this.innerHTML = i+"秒后重新获取";
							var intval = setInterval(function(){
								i--;
								_this.innerHTML = i+"秒后重新获取";
								if (i === 0) {
									clearInterval(intval);
									_this.innerHTML = "获取验证码";
									_this.setAttribute("data-on","");
								};
							},1000);
						} else {
							$(_this).parent().append('<div class="text-danger">'+result.data+'</div>');
						}
					},"json");
				}
				return s;
			},
			check:function(v,c){
				var el = arguments[2];
				var r = c.check;
				if (el.checked) {
					r = false;
				}
				return r;
			},
			depend:function(v,c){
				var r = false;
				var el = arguments[2];
				for (var i = 0; i < c.depend.dep.length; i++) {
					if (!c.depend.dep[i].val() && v) {
						r = c.depend.mess;
					} else if(c.depend.dep[i].val() && !v) {
						r = c.depend.mess;
					}
				}
				if (v && r) {
					r = c.depend.mess;
				}
				if (!v && !r) {
					r = false;
				}
				return r;
			},
			submit:function(){
				var v = true,m,k,x=0,r=true;
				for (var i = 0; i<this.elems.length; i++ ){
					if (this.elems[i].conf.verify || this.elems[i].conf.submit === false) continue;

					$(this.elems[i]).nextAll().remove();
					for (k in this.elems[i].conf.method) {
						m = this.elems[i].conf.method[k];
						var result = this[m](this.elems[i].value,this.elems[i].conf,this.elems[i]);
						
						if(result){
							v = r = false;
							$(this.elems[i]).parent().append('<div class="text-danger">'+result+'</div>');
						}
					}
					if (r) {
						$(this.elems[i]).parent().removeClass("has-error");
						$(this.elems[i]).parent().addClass("has-success");
					} else {
						$(this.elems[i]).parent().addClass("has-error");
						r = true;
					}

				}
				
				return !v;
			},
			init:function(){
				var obj = this;
				var a;
				var preg = /data-verify/;
				var s = false;
				var t;
				var p;
				var blur = ["checkbox","radio","submit"];

				for (var k = 0;k < elems.length; k++) {
					elems[k].conf = {method:[]};
					a = elems[k].attributes;
					for (var i = 0; i<a.length; i++){
						if (preg.test(a[i].nodeName)) {
							s = true;
							p = preg.exec(a[i].nodeName);		
							t = a[i].nodeName.slice(p[0].length+1);
							elems[k].conf.method[elems[k].conf.method.length] = t;
							elems[k].conf.verify = false;
							
							if (t !== "submit") {
								this.elems[this.elems.length] = elems[k];
							}
							
							f(elems[k], t, a[i].value);
						}
					}
					if (s) {
						if (elems[k].nodeName.toLowerCase() === "input" && (indexof(blur,elems[k].type) === -1)) {
							$(elems[k]).on("blur",v);
						} else if(elems[k].type === "submit") {
							$(elems[k]).on("click",function(e){
								if (obj.submit()) {
									e.preventDefault();
								} else {
									if (typeof callback === "function") {
										e.preventDefault();
										callback(_this,e);
									}
								}
							});
						} else if(elems[k].nodeName.toLowerCase() === "button"){
							$(elems[k]).on("click",v);
						} else if(elems[k].nodeName.toLowerCase() === "input" && (elems[k].type === "checkbox" || elems[k].type === "radio")) {

						}
					}
					s = false;
				}

				function v(e){
					var m;
					var r = true;
					$(this).nextAll().remove();
					for (var k in this.conf.method) {
						m = this.conf.method[k];
						var result = obj[m](this.value,this.conf,this);
						if (result) {
							r = false;

							$(this).parent().append('<div class="text-danger">'+result+'</div>');
						};
					}

					if (r) {

						$(this).parent().removeClass("has-error");
						if (this.value !== "") {
							$(this).parent().addClass("has-success");
						}
						this.conf.verify = true;
					} else {

						$(this).parent().addClass("has-error");
					}
				}
			}
		}
		Validate.init();
	}
});

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