/*
SQLyog Ultimate v11.3 (64 bit)
MySQL - 5.5.28 : Database - wyd
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`wyd` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `wyd`;

/*Table structure for table `mcp_admin` */

DROP TABLE IF EXISTS `mcp_admin`;

CREATE TABLE `mcp_admin` (
  `admin_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '管理员ID',
  `admin_account` varchar(32) NOT NULL COMMENT '管理员帐号',
  `admin_pwd` varchar(32) NOT NULL COMMENT '管理员密码',
  `issupper` int(11) NOT NULL DEFAULT '0' COMMENT '是否为超级管理员:1-是,0-否',
  `admin_name` varchar(64) NOT NULL COMMENT '管理员姓名',
  `mobile` varchar(16) NOT NULL COMMENT '手机号码',
  `email` varchar(256) NOT NULL COMMENT '邮箱地址',
  `last_login_ip` varchar(32) NOT NULL COMMENT '最后一次登录IP',
  `last_login_date` datetime NOT NULL COMMENT '最后一次登录时间',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态码:1-启用,2-停用,3-删除',
  `created_by` varchar(36) DEFAULT NULL COMMENT '创建人ID',
  `created_on` datetime NOT NULL COMMENT '创建日期',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '最后修改人ID',
  `modified_on` datetime DEFAULT NULL COMMENT '最后变更日期',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`admin_id`),
  UNIQUE KEY `udx_admin_account` (`admin_account`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='运营管理员';

/*Table structure for table `mcp_app_token` */

DROP TABLE IF EXISTS `mcp_app_token`;

CREATE TABLE `mcp_app_token` (
  `app_id` varchar(32) NOT NULL COMMENT '应用ID',
  `app_key` varchar(16) NOT NULL COMMENT '应用Key',
  `access_token` varchar(32) NOT NULL COMMENT '登陆令牌',
  `expires_in` decimal(11,4) NOT NULL DEFAULT '0.0000' COMMENT '令牌有效期（单位：秒）',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`app_id`,`app_key`),
  UNIQUE KEY `udx_access_token` (`access_token`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='应用访问令牌';

/*Table structure for table `mcp_application` */

DROP TABLE IF EXISTS `mcp_application`;

CREATE TABLE `mcp_application` (
  `app_id` varchar(32) NOT NULL COMMENT '应用ID',
  `app_key` varchar(16) NOT NULL COMMENT '应用Key',
  `app_type` int(11) NOT NULL COMMENT '应用类型：1-APP,2-WX,3-Web,4-Winform,5-Other',
  `app_name` varchar(128) NOT NULL COMMENT '应用名称',
  `sign_key` varchar(128) NOT NULL COMMENT '签名密钥',
  `auth_callback_url` varchar(256) NOT NULL COMMENT '授权认证回调地址',
  `update_callback_url` varchar(256) NOT NULL COMMENT '更新状态回调地址',
  `logo_path` varchar(256) NOT NULL COMMENT 'logoUrl地址',
  `business_license_path` varchar(256) NOT NULL COMMENT '营业执照地址',
  `developer_id` varchar(32) NOT NULL COMMENT '开发者ID',
  `status_code` int(11) NOT NULL DEFAULT '0' COMMENT '应用状态：0-新建,1-运营,2-停用,3-删除',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `created_by` varchar(36) NOT NULL COMMENT '创建人',
  `modified_on` datetime DEFAULT NULL COMMENT '更新时间',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '更新人',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`app_id`),
  UNIQUE KEY `udx_app_key` (`app_key`),
  UNIQUE KEY `udx_app_name` (`app_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='应用平台';

/*Table structure for table `mcp_developer` */

DROP TABLE IF EXISTS `mcp_developer`;

CREATE TABLE `mcp_developer` (
  `developer_id` varchar(32) NOT NULL COMMENT '开发者ID',
  `mobile` varchar(16) NOT NULL COMMENT '手机号码',
  `email` varchar(128) NOT NULL COMMENT '邮箱地址',
  `user_pwd` varchar(32) NOT NULL COMMENT '开发者密码',
  `is_actived` int(11) NOT NULL DEFAULT '0' COMMENT '是否激活:0-未激活,1-已激活',
  `actived_datetime` datetime NOT NULL COMMENT '激活日期',
  `last_login_ip` varchar(32) NOT NULL COMMENT '最后一次登录IP',
  `last_login_date` datetime NOT NULL COMMENT '最后一次登录时间',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态码:1-启用,2-停用,3-删除',
  `created_on` datetime NOT NULL COMMENT '创建日期',
  `modified_on` datetime DEFAULT NULL COMMENT '最后变更日期',
  `memo` varchar(128) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`developer_id`),
  UNIQUE KEY `udx_mobile` (`mobile`),
  UNIQUE KEY `udx_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='开发者账号';

/*Table structure for table `mcp_equipment` */

DROP TABLE IF EXISTS `mcp_equipment`;

CREATE TABLE `mcp_equipment` (
  `equipment_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '设备ID',
  `equipment_type` int(11) NOT NULL DEFAULT '1' COMMENT '1:58mm热敏小票,2:80mm热敏小票,3:针式快递面单',
  `equipment_code` varchar(32) NOT NULL COMMENT '设备编码',
  `check_code` varchar(32) NOT NULL COMMENT '校验码',
  `is_bind` int(11) NOT NULL DEFAULT '0' COMMENT '是否关联:0-未关联,1-已关联;',
  `created_by` varchar(36) NOT NULL COMMENT '创建人',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '更新人',
  `modified_on` datetime DEFAULT NULL COMMENT '更新时间',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`equipment_id`),
  UNIQUE KEY `udx_equipment_mac` (`equipment_code`),
  UNIQUE KEY `udx_equipment_no` (`check_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='设备';

/*Table structure for table `mcp_express` */

DROP TABLE IF EXISTS `mcp_express`;

CREATE TABLE `mcp_express` (
  `express_id` varchar(8) NOT NULL COMMENT '快递商编号',
  `express_name` varchar(128) NOT NULL COMMENT '快递商名称',
  `default_template_id` varchar(8) NOT NULL COMMENT '默认模板ID',
  `sort_index` decimal(11,2) NOT NULL DEFAULT '0.00' COMMENT '排序值',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态:1-启用,2-停用,3-删除',
  `created_by` varchar(36) NOT NULL COMMENT '创建人',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '更新人',
  `modified_on` datetime DEFAULT NULL COMMENT '更新时间',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`express_id`),
  UNIQUE KEY `udx_express_name` (`express_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='快递商';

/*Table structure for table `mcp_express_params` */

DROP TABLE IF EXISTS `mcp_express_params`;

CREATE TABLE `mcp_express_params` (
  `param_code` varchar(32) NOT NULL COMMENT '参数代码',
  `param_name` varchar(512) NOT NULL COMMENT '参数名称',
  `param_type` int(11) NOT NULL DEFAULT '1' COMMENT '参数类型:1-字符型,2-数字型,3-布尔型',
  `param_source` varchar(32) NOT NULL COMMENT '参数来源模板ID',
  PRIMARY KEY (`param_code`),
  KEY `udx_param_name` (`param_name`(255)),
  KEY `udx_param_code` (`param_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `mcp_express_template` */

DROP TABLE IF EXISTS `mcp_express_template`;

CREATE TABLE `mcp_express_template` (
  `express_id` varchar(8) NOT NULL COMMENT '快递商ID',
  `template_id` varchar(8) NOT NULL COMMENT '模板ID',
  `template_name` varchar(128) NOT NULL COMMENT '模板名称',
  `template_ver` varchar(64) NOT NULL COMMENT '模板版本',
  `template_example_path` varchar(256) NOT NULL COMMENT '模板样例',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态:1-启用,2-停用,3-删除',
  `create_by` varchar(36) NOT NULL COMMENT '创建人',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '更新人',
  `modified_on` datetime DEFAULT NULL COMMENT '更新时间',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`template_id`),
  UNIQUE KEY `udx_template_name` (`template_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `mcp_express_template_param` */

DROP TABLE IF EXISTS `mcp_express_template_param`;

CREATE TABLE `mcp_express_template_param` (
  `template_id` varchar(8) NOT NULL COMMENT '模板ID',
  `param_index` int(11) NOT NULL COMMENT '参数序号',
  `param_code` varchar(32) NOT NULL COMMENT '参数代码',
  `param_desc` varchar(64) NOT NULL COMMENT '参数描述',
  `param_max_len` int(11) NOT NULL DEFAULT '0' COMMENT '参数值最大字符数:0-不受限制',
  `param_type` int(11) NOT NULL DEFAULT '1' COMMENT '参数类型:1-字符型,2-数字型,3-布尔型',
  `loc_x` decimal(11,2) NOT NULL DEFAULT '0.00' COMMENT '参数X轴位置',
  `loc_y` decimal(11,2) NOT NULL DEFAULT '0.00' COMMENT '参数Y轴位置',
  `is_need` int(11) NOT NULL DEFAULT '0' COMMENT '是否必填项:0-非必填,1-必填',
  PRIMARY KEY (`template_id`,`param_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `mcp_merchant_printer` */

DROP TABLE IF EXISTS `mcp_merchant_printer`;

CREATE TABLE `mcp_merchant_printer` (
  `merchant_printer_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '商户打印机映射ID',
  `app_id` varchar(32) NOT NULL COMMENT '应用ID',
  `merchant_code` varchar(36) NOT NULL COMMENT '商户编码，app内唯一',
  `printer_code` varchar(32) NOT NULL COMMENT '打印机唯一标识',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`merchant_printer_id`),
  UNIQUE KEY `udx_app_merchant_printer` (`app_id`,`merchant_code`,`printer_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='商户打印机映射表';

/*Table structure for table `mcp_order` */

DROP TABLE IF EXISTS `mcp_order`;

CREATE TABLE `mcp_order` (
  `order_id` varchar(36) NOT NULL COMMENT '订单ID',
  `app_id` varchar(32) NOT NULL COMMENT '应用ID',
  `bill_no` varchar(36) NOT NULL COMMENT '票据APP内唯一标识',
  `bill_type` int(11) NOT NULL DEFAULT '1' COMMENT '票据类型:1-小票;2-快递面单',
  `order_content` varchar(4096) NOT NULL COMMENT '订单内容(票据类型:1-url,2-处理后的参数)',
  `order_date` datetime NOT NULL COMMENT '订单时间',
  `order_key` varchar(32) NOT NULL COMMENT '订单Key',
  `copies` int(11) NOT NULL DEFAULT '1' COMMENT '打印份数,订单类型为2时恒为1',
  `callback_status` int(11) NOT NULL DEFAULT '0' COMMENT '回调状态:0-未回调,1-回调成功,2-回调失败',
  `modified_on` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`order_id`),
  UNIQUE KEY `udx_app_bill` (`app_id`,`bill_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='打印订单';

/*Table structure for table `mcp_order_printer` */

DROP TABLE IF EXISTS `mcp_order_printer`;

CREATE TABLE `mcp_order_printer` (
  `order_printer_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT 'ID',
  `order_id` varchar(36) NOT NULL COMMENT '订单ID',
  `printer_code` varchar(32) NOT NULL COMMENT '打印机唯一标识',
  `order_status` int(11) NOT NULL DEFAULT '0' COMMENT '订单状态:0-未打印,1-已打印,2-打印失败',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `modified_on` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`order_printer_id`),
  UNIQUE KEY `udx_order_printer` (`order_id`,`printer_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='订单打印机';

/*Table structure for table `mcp_printer` */

DROP TABLE IF EXISTS `mcp_printer`;

CREATE TABLE `mcp_printer` (
  `printer_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '打印机ID',
  `printer_code` varchar(32) NOT NULL COMMENT '打印机唯一标识',
  `offsets_left` decimal(11,2) NOT NULL DEFAULT '0.00' COMMENT '打印机左纠偏量，单位毫米，范围(-20至20)',
  `offsets_top` decimal(11,2) NOT NULL DEFAULT '0.00' COMMENT '打印机上纠偏量，单位毫米，范围(-20至20)',
  `connection_id` bigint(20) NOT NULL COMMENT '连接ID',
  `ip_port` varchar(32) NOT NULL COMMENT '打印机IP地址及端口',
  `status_code` int(11) NOT NULL DEFAULT '2' COMMENT '打印机状态:1-启用,2-停用,3-删除',
  `created_by` varchar(36) DEFAULT NULL COMMENT '创建人',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '更新人',
  `modified_on` datetime DEFAULT NULL COMMENT '更新时间',
  `memo` varchar(512) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`printer_id`),
  UNIQUE KEY `udx_printer_mac` (`printer_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC COMMENT='商家打印机';

/*Table structure for table `mcp_printer_status` */

DROP TABLE IF EXISTS `mcp_printer_status`;

CREATE TABLE `mcp_printer_status` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '记录ID',
  `printer_code` varchar(32) NOT NULL COMMENT '打印机唯标识',
  `status_code` int(11) NOT NULL COMMENT '状态编码：1-正常,2-缺纸,3-故障,99-未知异常',
  `begin_time` datetime NOT NULL COMMENT '异常开始时间',
  `end_time` datetime NOT NULL COMMENT '异常结束时间',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='打印机状态';

/*Table structure for table `mcp_sys_blacklist` */

DROP TABLE IF EXISTS `mcp_sys_blacklist`;

CREATE TABLE `mcp_sys_blacklist` (
  `black_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '黑名单ID',
  `app_sign` varchar(32) NOT NULL COMMENT 'APP应用标识',
  `terminal_code` varchar(64) NOT NULL COMMENT '设备标示',
  `counter` tinyint(4) DEFAULT '0' COMMENT '计数',
  `lock_expire_date` datetime DEFAULT NULL COMMENT '锁定日期',
  `last_check_time` datetime DEFAULT NULL COMMENT '最后核查时间',
  `remark` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`black_id`),
  UNIQUE KEY `idx_terminal_code` (`terminal_code`,`app_sign`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Table structure for table `mcp_sys_email` */

DROP TABLE IF EXISTS `mcp_sys_email`;

CREATE TABLE `mcp_sys_email` (
  `sys_mail_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '系统邮件ID',
  `app_sign` varchar(32) NOT NULL COMMENT 'APP应用标识',
  `sender_name` varchar(256) DEFAULT NULL COMMENT '发件人名称',
  `sender_email` varchar(256) DEFAULT NULL COMMENT '发件人邮箱',
  `receiver_name` varchar(256) DEFAULT NULL COMMENT '收件人名称',
  `receiver_email` varchar(256) DEFAULT NULL COMMENT '收件人邮箱',
  `title` varchar(512) DEFAULT NULL COMMENT '邮件标题',
  `content` text COMMENT '邮件内容',
  `priority` int(11) DEFAULT '2' COMMENT '优先级: 1-高,2-中,3-低',
  `mail_type` int(11) DEFAULT NULL COMMENT '邮件类型: 1-绑定邮箱,2-解除绑定,3-找回密码',
  `created_by` varchar(36) NOT NULL COMMENT '创建人ID',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态码:1-有效,2-无效',
  `remark` varchar(128) NOT NULL COMMENT '备注',
  PRIMARY KEY (`sys_mail_id`),
  KEY `idx_receiver_email` (`receiver_email`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统邮件';

/*Table structure for table `mcp_sys_sms` */

DROP TABLE IF EXISTS `mcp_sys_sms`;

CREATE TABLE `mcp_sys_sms` (
  `sys_sms_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '系统短信ID',
  `app_sign` varchar(32) NOT NULL COMMENT 'APP应用标识',
  `sender_no` varchar(32) NOT NULL COMMENT '发送方号码',
  `receiver_no` varchar(256) NOT NULL COMMENT '接收方号码',
  `title` varchar(512) NOT NULL COMMENT '短信标题',
  `content` text NOT NULL COMMENT '短信内容',
  `sms_type` int(11) NOT NULL COMMENT '短信类型: 1-绑定手机号,2-解除绑定,3-找回密码,4-失败反馈,5-成功反馈,6-用户反馈',
  `created_by` varchar(36) NOT NULL COMMENT '创建人ID',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态码:1-有效,2-无效',
  `remark` varchar(128) NOT NULL COMMENT '备注',
  PRIMARY KEY (`sys_sms_id`),
  KEY `idx_receiver_no` (`receiver_no`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统短信';

/*Table structure for table `mcp_sys_verifycode` */

DROP TABLE IF EXISTS `mcp_sys_verifycode`;

CREATE TABLE `mcp_sys_verifycode` (
  `verify_code_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '验证码ID',
  `app_sign` varchar(32) NOT NULL COMMENT 'APP应用标识',
  `mobile` varchar(16) NOT NULL COMMENT '手机号码',
  `email` varchar(256) NOT NULL COMMENT '邮箱地址',
  `merchant_code` varchar(256) NOT NULL COMMENT '商家编码',
  `verify_code` varchar(16) NOT NULL COMMENT '验证码',
  `expire_date` datetime DEFAULT NULL COMMENT '验证失效时间',
  `verified` int(11) NOT NULL DEFAULT '0' COMMENT '是否已验证:0-未验证,1-已验证',
  `verified_date` datetime DEFAULT NULL COMMENT '验证日期',
  `created_by` varchar(36) DEFAULT NULL COMMENT '创建人ID',
  `created_on` datetime NOT NULL COMMENT '创建日期',
  `modified_by` varchar(36) DEFAULT NULL COMMENT '修改人ID',
  `modified_on` timestamp NULL DEFAULT NULL COMMENT '修改时间',
  `time_stamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '时间戳',
  `remark` varchar(128) DEFAULT NULL COMMENT '备注',
  `terminal_code` varchar(64) DEFAULT NULL COMMENT '终端标识',
  PRIMARY KEY (`verify_code_id`),
  KEY `idx_mobile` (`mobile`),
  KEY `idx_email` (`email`(255))
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='验证码信息';

/*Table structure for table `mcp_sys_version` */

DROP TABLE IF EXISTS `mcp_sys_version`;

CREATE TABLE `mcp_sys_version` (
  `sys_version_id` int(11) NOT NULL AUTO_INCREMENT COMMENT '版本ID',
  `objec_type` int(11) NOT NULL COMMENT '版本对象类型:1-mp58,2-mp350,3-mp360,4-fp570',
  `object_version` varchar(16) NOT NULL COMMENT '版本号',
  `version_num` int(11) NOT NULL COMMENT '数字版本号',
  `version_file` varchar(256) NOT NULL COMMENT '版本文件',
  `is_force` int(11) NOT NULL COMMENT '是否强制升级:0-否,1-是',
  `created_on` datetime NOT NULL COMMENT '创建时间',
  `created_by` varchar(36) NOT NULL COMMENT '创建人',
  `status_code` int(11) NOT NULL DEFAULT '1' COMMENT '状态码:1-启用,2-停用,3-删除',
  `remark` varchar(256) NOT NULL COMMENT '备注',
  PRIMARY KEY (`sys_version_id`),
  UNIQUE KEY `udx_version` (`objec_type`,`object_version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='系统版本表';

/* Procedure structure for procedure `proc_sys_blacklist` */

/*!50003 DROP PROCEDURE IF EXISTS  `proc_sys_blacklist` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`mcp_admin`@`%` PROCEDURE `proc_sys_blacklist`(
IN  appsign VARCHAR(64),
IN  terminalcode VARCHAR(64),
IN  totalcount TINYINT
)
BEGIN
	SELECT COUNT(black_id),IFNULL(MAX(last_check_time),CURRENT_TIMESTAMP()) INTO @blackId,@ctime FROM mcp_sys_blacklist
	WHERE app_sign=appsign AND terminal_code=terminalcode LIMIT 1;
	
        IF(@blackId=0) THEN
		
	        INSERT INTO mcp_sys_blacklist(app_sign,terminal_code,counter,lock_expire_date,remark,last_check_time)
	        VALUES(appsign,terminalcode,0,CURRENT_TIMESTAMP(),'',CURRENT_TIMESTAMP());
	        
	END IF;
	
	
	IF(@blackId = 0) THEN	
	SET @ctime= DATE_ADD(CURRENT_TIMESTAMP(),INTERVAL -1 MINUTE);
	END IF;
			
	SELECT  COUNT(verify_code_id) INTO @fc FROM  mcp_sys_verifycode 
	WHERE app_sign=appsign AND terminal_code=terminalcode AND created_on >= @ctime AND verified = 0 ;
	
	SELECT  COUNT(verify_code_id) INTO @cc FROM  mcp_sys_verifycode
	WHERE app_sign=appsign AND terminal_code=terminalcode AND created_on >= @ctime AND verified = 1 ;   
		
	IF(@cc > 0 ) THEN
	
		UPDATE mcp_sys_blacklist SET counter=0,lock_expire_date=CURRENT_TIMESTAMP(),last_check_time=CURRENT_TIMESTAMP()
		WHERE app_sign=appsign AND terminal_code=terminalcode;
		
	END IF;
	
	IF(@cc <= 0 AND @fc > 0 ) THEN
		UPDATE mcp_sys_blacklist SET counter=IFNULL(counter+@fc,0),lock_expire_date=CURRENT_TIMESTAMP(),last_check_time=CURRENT_TIMESTAMP()
                WHERE app_sign=appsign AND terminal_code=terminalcode;
                
        END IF;
	        
        SELECT MAX(counter),MAX(lock_expire_date) INTO @lastcounter,@lastexpiredate FROM mcp_sys_blacklist WHERE app_sign=appsign AND terminal_code=terminalcode LIMIT 1;
                     
	IF(@lastcounter >= totalcount AND @lastexpiredate<=CURRENT_TIMESTAMP()) THEN
		UPDATE mcp_sys_blacklist SET lock_expire_date=DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL "0 24" DAY_HOUR),
		last_check_time=CURRENT_TIMESTAMP(),counter=0
                WHERE app_sign=appsign AND terminal_code=terminalcode;
        END IF;
      
        UPDATE mcp_sys_blacklist SET  last_check_time=CURRENT_TIMESTAMP() WHERE app_sign=appsign AND terminal_code=terminalcode;
                       
END */$$
DELIMITER ;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
