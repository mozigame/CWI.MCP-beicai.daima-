#修改设备表，增加1.微信设备ID，2.是否授权，3.是否开通WiFi
ALTER TABLE `mcp_equipment`
ADD COLUMN `device_id` VARCHAR(64) NOT NULL COMMENT '微信设备ID' AFTER `check_code`,   
ADD COLUMN `is_auth` INT(11) NOT NULL DEFAULT '0' COMMENT '是否授权:0-未授权,1-已授权;' AFTER `is_bind` ,
ADD COLUMN `is_open_wifi` INT(11) NOT NULL COMMENT '是否开通WiFi:0-未开通,1-已开通;' AFTER `is_auth`;

#新增微信设备表
DROP TABLE IF EXISTS `mcp_weixin_device`;
CREATE TABLE `mcp_weixin_device` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `device_type` VARCHAR(36) NOT NULL COMMENT '设备类型',
  `device_id` VARCHAR(36) NOT NULL COMMENT '微信设备ID',
  `device_licence` VARCHAR(256) NOT NULL COMMENT '产品使用直连SDK时返回的设备证书',
  `qrticket` VARCHAR(64) NOT NULL COMMENT '二维码',
  `open_id` VARCHAR(64) NOT NULL COMMENT '微信OpenID',
  `created_on` DATETIME NOT NULL COMMENT '创建时间',
  `modified_on` DATETIME NOT NULL COMMENT '更新时间',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='微信设备表';

#新增微信公众号JSAPI票据表
DROP TABLE IF EXISTS `mcp_weixin_jsapiticket`;
CREATE TABLE `mcp_weixin_jsapiticket` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `app_id` VARCHAR(32) DEFAULT NULL,
  `jsapi_ticket` VARCHAR(128) DEFAULT NULL,
  `expires_time` DATETIME NOT NULL,
  `update_time` DATETIME NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='微信公众号JSAPI票据表';

#新增微信公众号令牌表
DROP TABLE IF EXISTS `mcp_weixin_token`;
CREATE TABLE `mcp_weixin_token` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `app_id` VARCHAR(32) DEFAULT NULL,
  `access_token` VARCHAR(128) DEFAULT NULL,
  `expires_time` DATETIME NOT NULL,
  `update_time` DATETIME NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='微信公众号令牌表';

#新增微信用户表
DROP TABLE IF EXISTS `mcp_weixin_user`;
CREATE TABLE `mcp_weixin_user` (
  `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `wx_type` INT(11) NOT NULL COMMENT '微信类型：0-客户端,1-商家端',
  `open_id` VARCHAR(64) NOT NULL COMMENT '微信OpenID',
  `user_id` BIGINT(20) NOT NULL COMMENT '用户ID',
  `terminal_code` VARCHAR(64) NOT NULL COMMENT '终端设备特征码，如：mac地址',
  `status_code` INT(11) NOT NULL DEFAULT '1' COMMENT '状态:1-已关注,2-取消关注',
  `created_on` DATETIME NOT NULL COMMENT '创建时间',
  `modified_on` DATETIME NOT NULL COMMENT '更新时间',
  `remark` VARCHAR(128) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  UNIQUE KEY `open_id` (`open_id`)
) ENGINE=INNODB DEFAULT CHARSET=utf8 COMMENT='微信用户表';