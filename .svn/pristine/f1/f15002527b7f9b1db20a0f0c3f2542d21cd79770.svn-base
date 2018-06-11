"use strict";
/**
 * Version: 1.0
 * Edition: Alpha
 * 通用表单验证及后端验证
 */
$.fn.extend({
	validateForm: function(config) {
		var _$ = this,
			options = {
				"extends": "",
				submit: "",
				render: "",
				allow_submit: "",
				submit_verify_type: false
			};
		if (typeof config === "function") {
			options.submit = config;
		}

		if (typeof config === "object") {
			for (var key in config) {
				options[key] = config[key];
			}
		}

		/**
		 * 初始化验证类
		 */
		function Validate() {
			this.elems = [];
			// 标记为兄弟对象调用验证方法,需要返回值.
			this.brother_call = false;
			this.submit_call = false;
			this.verify = true;
			this.submit_single_verify = true;
			this.submit_verify = false;
			this.init = function() {
				var elems = _$[0].elements,
					attrs, preg = /data-verify/,
					el, aname, on;
				this.submit_verify = options.submit_verify_type;

				for (var ei = 0; ei < elems.length; ei++) {
					on = false;
					el = elems[ei];
					var conf = {};
					attrs = el.attributes;
					FormObserver.make(el);
					for (var ai = 0; ai < attrs.length; ai++) {
						if (preg.test(attrs[ai].nodeName)) {
							on = true;
							aname = attrs[ai].nodeName.replace("data-verify-", "");
							el.addSubscriber(validate[aname]);
							conf = Parse[aname](el, conf, aname, attrs[ai].value);
						}
					}
					if (on) {
						var error_elem = document.createElement("p");
						conf.error_elem = error_elem;
						if (typeof conf.verify === "undefined") {
							conf.verify = false;
						}
						el.conf = conf;
						this.elems.push(el);
						// 添加事件监听
						var name = el.nodeName.toLowerCase();
						var type = el.type.toLowerCase();
						if (name === "input" || name === "textarea") {
							if (type === "submit") {
								$(el).on("click", Handle.signal);
							} else if (type === "file") {
								$(el).on("change", Handle.signal);
							} else if (type === "checkbox" || type === "radio") {
								$(el).on("click", Handle.signal);
							} else {
								$(el).on("blur", Handle.signal);
							}
						} else if (name === "button") {
							$(el).on("click", Handle.signal);
						} else if (name === "select") {
							$(el).on("change", Handle.signal);
						}
					}
				}
			}
			this.render = function(conf) {
				if (!conf.verify) {
					this.verify = conf.verify;
				}
				if (typeof options.render === "function") {
					options.render(conf);
				} else {
					var r = conf.verify,
						mess = conf.mess,
						el = conf.elem;
					$(el).nextAll().remove();
					if (r) {
						Rander.decorate({
							border: {
								borderColor: "green"
							},
							color: {
								color: "green"
							}
						});
					} else {
						Rander.decorate({
							border: {
								borderColor: "red"
							},
							color: {
								color: "red"
							}
						});
						if (mess instanceof Array) {
							for (var i = 0; i < mess.length; i++) {
								$(el).parent().append("<p style='color:red;'>" + mess[i] + "</p>");
							}
						} else {
							$(el).parent().append("<p style='color:red;'>" + mess + "</p>");
						}
					}
				}
			}
			this.submit = function(e) {
				if (validate.submit_verify) {
					return false;
				}
				validate.verify = true;
				validate.submit_call = true;
				for (var i = 0; i < validate.elems.length; i++) {
					validate.elems[i].publish();
					if (validate.submit_single_verify && !validate.verify) {
						validate.submit_call = false;
						return false;
					}
				}
				validate.submit_call = false;
				if (validate.verify) {
					if (typeof options.submit === "function") {
						e.preventDefault();
						options.submit(this);
					}
				} else {
					e.preventDefault();
				}
			}
		}

		/**
		 * 基本验证
		 */
		function ValidateBase() {
			/**
			 * 验证长度
			 * @param  {[type]} el [description]
			 * @param  {[type]} v  [description]
			 * @param  {[type]} c  [description]
			 * @return {[type]}    [description]
			 */
			this.len = function(el, v, c) {
					var r = false,
						mess;

					if (typeof c.len.max !== "undefined") {
						if (v.length <= c.len.max && v.length >= c.len.min) {
							r = true;
						} else {
							mess = "长度应大于等于" + c.len.min + ",且小于等于" + c.len.max;
						}
					} else {
						if (v.length == c.len.min) {
							r = true;
						} else {
							mess = "长度应为" + c.len.min;
						}
					}
					if (typeof c.len.mess !== "undefined") {
						mess = c.len.mess;
					}

					if (this.brother_call) {
						if (r) {
							r = false;
						} else {
							r = {
								mess: mess
							};
						}

					} else {
						Rander.setRander($(el)).setDecorate(["border", "color"]);
						this.render({
							verify_type: "len",
							verify: r,
							mess: mess,
							elem: el,
							error_elem: c.error_elem
						});
					}
					return r;
				}
				/**
				 * 验证必填
				 * @param  {[type]} el [description]
				 * @param  {[type]} v  [description]
				 * @param  {[type]} c  [description]
				 * @return {[type]}    [description]
				 */
			this.require = function(el, v, c) {
					var r = false,
						mess;
					if (v) {
						r = true;
					} else {
						mess = "此项为必填项";
					}
					if (typeof c.require.mess !== "undefined") {
						mess = c.require.mess;
					}
					Rander.setRander($(el)).setDecorate(["border", "color"]);
					this.render({
						verify_type: "require",
						verify: r,
						mess: mess,
						elem: el,
						error_elem: c.error_elem
					});
					return r;
				}
				/**
				 * 验证数据类型 字符串 OR 数字
				 * @param  {[type]} el [description]
				 * @param  {[type]} v  [description]
				 * @param  {[type]} c  [description]
				 * @return {[type]}    [description]
				 */
			this.type = function(el, v, c) {
				var r = false,
					mess, n = {
						1: "数字",
						2: "字母"
					};
				if (c.type.type === "1") {
					var reg = /[0-9]+\d$/;
					if (reg.test(v)) {
						r = true;
					}
				} else if (c.type.type === "2") {
					var reg = /^[a-zA-Z]+[a-zA-Z]$/;
					if (reg.test(v)) {
						r = true;
					}
				}
				mess = "此项必须为" + n[c.type.type];
				if (typeof c.type.mess !== "undefined") {
					mess = c.type.mess;
				}
				Rander.setRander($(el)).setDecorate(["border", "color"]);
				this.render({
					verify_type: "type",
					verify: r,
					mess: mess,
					elem: el,
					error_elem: c.error_elem
				});
				return r;
			}

			/**
			 * 验证是否选中
			 * @param  {[type]} el [description]
			 * @param  {[type]} v  [description]
			 * @param  {[type]} c  [description]
			 * @return {[type]}    [description]
			 */
			this.check = function(el, v, c) {
				var r = false,
					mess;
				if (el.checked) {
					r = true;
				} else {
					mess = "请选择此项";
				}
				if (typeof c.check.mess !== "undefined") {
					mess = c.check.mess;
				}
				Rander.setRander($(el)).setDecorate(["border", "color"]);
				this.render({
					verify_type: "check",
					verify: r,
					mess: mess,
					elem: el,
					error_elem: c.error_elem
				});
				return r;
			}
		}

		/**
		 * 特殊验证
		 */
		function ValidateSpec() {
			/**
			 * 验证邮箱
			 * @param  {[type]} el [description]
			 * @param  {[type]} v  [description]
			 * @param  {[type]} c  [description]
			 * @return {[type]}    [description]
			 */
			this.mail = function(el, v, c) {
				var r = false,
					mess = "邮箱格式不正确",
					preg, p;
				p = "[a-z|A-Z|0-9]";
				if (c.mail.prefix) {
					p += "{1," + c.mail.prefix + "}";
				} else {
					p += "{1,32}";
				}
				p += "@";
				if (c.mail.suffix) {
					p += "(" + c.mail.suffix + ")";
				} else {
					p += "[a-z|A-Z|0-9]*\\.[a-z]{1,128}";
				}
				preg = new RegExp(p);
				if (preg.test(v)) {
					r = true;
				}
				if (c.mail.mess) {
					mess = c.mail.mess;
				}
				Rander.setRander($(el)).setDecorate(["border", "color"]);
				this.render({
					verify_type: "mail",
					verify: r,
					mess: mess,
					elem: el,
					error_elem: c.error_elem
				});
				return r;
			}

			/**
			 * 验证确认密码
			 * @param  {[type]} el [description]
			 * @param  {[type]} v  [description]
			 * @param  {[type]} c  [description]
			 * @return {[type]}    [description]
			 */
			this.pass = function(el, v, c) {
					var r = false,
						mess = "两次密码不符";

					var rv = c.pass.depend.val();
					if (v === rv) {
						r = true;
					}

					if (typeof c.pass.mess !== "undefined") {
						mess = c.pass.mess;
					}

					Rander.setRander($(el)).setDecorate(["border", "color"]);
					this.render({
						verify_type: "pass",
						verify: r,
						mess: mess,
						elem: el,
						error_elem: c.error_elem
					});
					return r;
				}
				/**
				 * 验证手机号码是否正确
				 * @param  {[type]} el [description]
				 * @param  {[type]} v  [description]
				 * @param  {[type]} c  [description]
				 * @return {[type]}    [description]
				 */
			this.phone = function(el, v, c) {
					var r = false,
						mess;
					var reg = /^1[3-8]\d{9}$/;
					if (reg.test(v)) {
						r = true;
					}
					mess = "手机号码格式不正确";
					if (typeof c.phone.mess !== "undefined") {
						mess = c.phone.mess;
					}
					if (this.brother_call) {
						if (r) {
							r = false;
						} else {
							r = {
								mess: mess
							}
						}

					} else {
						Rander.setRander($(el)).setDecorate(["border", "color"]);
						this.render({
							verify_type: "phone",
							verify: r,
							mess: mess,
							elem: el,
							error_elem: c.error_elem
						});
					}
					return r;

				}
				/**
				 * 验证图形验证码
				 * @return {[type]} [description]
				 */
			this.code = function(el, v, c) {
					var r = false,
						mess;
					if (v.length == c.code.len) {
						r = true;
					}
					mess = "格式不正确";
					if (typeof c.code.mess !== "undefined") {
						mess = c.code.mess;
					}
					if (this.brother_call) {
						if (r) {
							r = false;
						} else {
							r = {
								mess: mess
							}
						}

					} else {
						if (r && (typeof options.code === "function")) {
							options.code();
						}
						Rander.setRander($(el)).setDecorate(["border", "color"]);
						this.render({
							verify_type: "code",
							verify: r,
							mess: mess,
							elem: el,
							error_elem: c.error_elem
						});
					}
					return r;
				}
				/**
				 * 验证短信验证码
				 * @param  {[type]} el [description]
				 * @param  {[type]} v  [description]
				 * @param  {[type]} c  [description]
				 * @return {[type]}    [description]
				 */
			this.smscode = function(el, v, c) {
					if (this.submit_call) {
						return;
					}

					var depends = c.smscode.depend,
						r = true, _r,
						mess = [];
					for (var i = 0; i < depends.length; i++) {
						if (true) {
							_r = depends[i][0].publish();
							if (!_r) {
								r = _r;
							}
						} else {
							this.brother_call = true;
							_r = depends[i][0].publish();
							if (_r) {
								r = false;
								mess.push(_r.mess);
							}
							this.brother_call = false;
						}
					}

					if (r) {
						if (typeof options.smscode === "function") {
							options.smscode({
								phoneNumber: c.smscode.phone.val(),
								name: c.smscode.phone.attr("name"),
								elem: el
							});
						}
					}

					Rander.setRander($(el));
					this.render({
						verify_type: "smscode",
						verify: r,
						mess: mess,
						elem: el,
						error_elem: c.error_elem
					});
					return r;
				}
				// 依赖验证
				// 功能设计: 主对象和依赖对象必须都填写才能
			this.depend = function(el, v, c) {
				var elem, r = false,
					mess, _this = this;
				for (var i = 0; i < c.depend.elems.length; i++) {
					elem = c.depend.elems[i];
					if (elem.val() || v) {
						if (elem.val() && v) {
							r = true;
						} else {
							r = false;
						}
						if (!r && !elem.val()) {
							// 第一种主失去焦点检查依赖关系
							elem.on("blur", function() {
								_this.depend(el, v, c);
							});
						}
					} else {
						r = true;
					}

					// 第二种提交表单时检查依赖关系
				}
				if (typeof c.depend.mess !== "undefined") {
					mess = c.depend.mess;
				}
				Rander.setRander($(el));
				this.render({
					verify_type: "depend",
					verify: r,
					mess: mess,
					elem: el,
					error_elem: c.error_elem
				});
				return r;
			},
			this.url = function(el, v, c) {
				if (v !== "") {
					var r = false,
						mess = "请填写正确的URL或IP地址,如:http://www.baidu.com",
						preg  = /^(http|https):\/\/(\w+\.\w+\.\w+|\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/;
					if (preg.test(v)) {
						r = true;
					}

					if (typeof c.url.mess !== "undefined") {
						mess = c.url.mess;
					}

					Rander.setRander($(el)).setDecorate(["color","border"]);
				} else {
					r = true;
					Rander.setRander($(el));
				}
				this.render({
					verify_type: "url",
					verify: r,
					mess: mess,
					elem: el,
					error_elem: c.error_elem
				});
			}
		}

		ValidateBase.prototype = new ValidateSpec();
		Validate.prototype = new ValidateBase();
		Validate.prototype.constructor = Validate;

		/**
		 * 集合
		 * @type {Object}
		 */
		var FormObserver = {
			addSubscriber: function(callback) {
				this.subscribers[this.subscribers.length] = callback;
			},
			removeSubscriber: function(callback) {},
			publish: function() {
				var r = false;
				for (var i = 0; i < this.subscribers.length; i++) {
					if (typeof this.subscribers[i] === "function") {
						r = this.subscribers[i].call(validate, this, this.value, this.conf);
						if (r === false || r !== true) {
							break;
						}
					}
				}
				this.conf.verify = r;
				return r;
			},
			make: function(obj) {
				for (var key in this) {
					obj[key] = this[key];
					obj.subscribers = [];
				}
			}
		}

		/**
		 * 处理事件
		 * @type {Object}
		 */
		var Handle = {
			signal: function(e) {
				this.publish();
				if (options.submit_verify_type && (typeof options.allow_submit === "function")) {
					validate.submit_verify = true;
					Handle.check_submit();
				}
			},
			check_submit: function() {
				var r = true;
				for (var i = 0; i < validate.elems.length; i++) {
					if (!validate.elems[i].conf.verify) {
						r = validate.elems[i].conf.verify;
					}
				}
				if (r) {
					validate.submit_verify = false;
					options.allow_submit();
				}
			}
		}

		/**
		 * 解析属性字符串
		 * @type {Object}
		 */
		var Parse = {
			len: function(el, conf, aname, avalue) {
				if (avalue) {
					var v = avalue.split(",");
					var len = v[0].split("|");
					if (len.length === 1) {
						conf.len = {
							min: len[0]
						}
					} else if (len.length === 2) {
						conf.len = {
							min: len[0],
							max: len[1]
						}
					}

					if (typeof v[1] !== "undefined") {
						conf.len.mess = v[1];
					}
				} else {
					throw new Error("Len: 需要至少一个参数来做验证!");
				}
				return conf;
			},
			require: function(el, conf, aname, avalue) {
				// 不需要解析参数
				conf.require = {};
				if (avalue) {
					conf.require.mess = avalue;
				}
				return conf;
			},
			type: function(el, conf, aname, avalue) {
				if (avalue) {
					var avalue = avalue.split(",");
					conf.type = {};
					if (avalue[0] === "1" || avalue[0] === "2") {
						conf.type.type = avalue[0];
					}

					if (typeof avalue[1] !== "undefined") {
						conf.type.mess = avalue[1];
					}
				} else {
					throw new Error("Type: 需要一个参数来做验证!");
				}
				return conf;
			},
			mail: function(el, conf, aname, avalue) {
				// 不需要解析参数
				var conf = conf;
				conf.mail = {};
				if (avalue) {
					var avalue = avalue.split(",");
					conf.mail = {
						prefix: avalue[0],
						suffix: avalue[1],
						mess: avalue[2]
					}
				}
				return conf;
			},
			pass: function(el, conf, aname, avalue) {
				var conf = conf;
				conf.pass = {};
				if (avalue) {
					var avalue = avalue.split(",");
					conf.pass = {
						depend: $(avalue[0])
					}
					if (avalue.length === 2) {
						conf.pass.mess = avalue[1];
					}
				} else {
					throw new Error("Pass: 二次密码需要一个匹配对象!");
				}
				return conf;
			},
			phone: function(el, conf, aname, avalue) {
				// 不需要解析参数
				var conf = conf;
				conf.phone = {};
				if (avalue) {
					conf.phone.mess = avalue;
				}
				return conf;
			},
			check: function(el, conf, aname, avalue) {
				var conf = conf;
				conf.check = {};
				if (avalue) {
					conf.check = {
						mess: avalue
					}
				}
				return conf;
			},
			code: function(el, conf, aname, avalue) {
				var conf = conf;
				conf.code = {};
				if (avalue) {
					var v = avalue.split(",");
					conf.code.len = v[0];
					if (v.length === 2) {
						conf.code.mess = v[1];
					}
				} else {
					throw new Error("Len: 需要至少一个参数来做验证!");
				}
				return conf;
			},
			smscode: function(el, conf, aname, avalue) {
				if (avalue) {
					var conf = conf;
					conf.smscode = {};
					conf.smscode.submit = true;
					var avalue = avalue.split(",");
					var depend = avalue[0].split("|");
					var els = [];
					for (var i = 0; i < depend.length; i++) {
						els.push($(depend[i]));
					}
					conf.smscode.depend = els;
					conf.smscode.phone = $(avalue[1]);
					if (avalue.length === 3) {
						conf.smscode.mess = avalue[2];
					}
				} else {
					throw new Error("SmsCode: 至少需要2个对象!");
				}
				return conf;
			},
			depend: function(el, conf, aname, avalue) {
				var conf = conf;
				conf.depend = {};
				conf.verify = true;
				if (avalue) {
					var avalue = avalue.split(",");
					var depends = avalue[0].split("|");
					var els = [];
					for (var i = 0; i < depends.length; i++) {
						els.push($(depends[i]));
					}
					conf.depend = {
						elems: els
					}
					if (avalue.length === 2) {
						conf.depend.mess = avalue[1];
					}
				} else {
					throw new Error("Depend: 需要至少一个依赖对象!");
				}
				return conf;
			},
			url: function(el, conf, aname, avalue) {
				var conf = conf;
				conf.url = {};
				// 域名 ip地址
				return conf;
			}
		}

		/**
		 * 装饰
		 * @type {Object}
		 */
		var Rander = {};
		Rander.setRander = function(elem) {
			this.elem = elem;
			this.decoras = [];
			return this;
		}

		Rander.decorate = function(conf) {
			for (var i = 0; i < this.decoras.length; i++) {
				this.elem.css(conf[this.decoras[i]]);
			}
		}

		Rander.setDecorate = function(deco) {
			for (var i = 0; i < deco.length; i++) {
				this.decoras.push(deco[i]);
			}

		}

		var validate = new Validate();
		validate.init();
		_$.on("submit", validate.submit);
	}
});