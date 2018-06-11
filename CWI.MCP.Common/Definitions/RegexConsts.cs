using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 正则常量
    /// </summary>
    public class RegexConsts
    {
        /// <summary>
        /// 全数字
        /// </summary>
        public const string ALL_NUM_PARTTERN = @"^[0-9]*$";

        /// <summary>
        /// 用户登录帐号名称表达式
        /// </summary>
        public const string USERACCOUNT_PATTERN = @"^[a-zA-Z0-9_\-]+$";

        /// <summary>
        /// 用户登录密码表达式
        /// </summary>
        public const string USERPASSWORD_PATTERN = @"^[a-zA-Z0-9_\-]+$";

        /// <summary>
        /// 手机号表达式
        /// </summary>
        public const string MOBILE_PATTERN = @"^1[3,4,5,7,8]{1}[0-9]{1}[0-9]{8}$";

        /// <summary>
        /// 邮箱地址表达式
        /// </summary>
        public const string EMAIL_PATTERN = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        /// <summary>
        /// url地址表达式
        /// </summary>
        public const string URL_PATTERN = @"((http|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";

        /// <summary>
        /// 真假值：0或1
        /// </summary>
        public const string NUM_BOOL = @"[0,1]";

        /// <summary>
        /// 整数：1或2
        /// </summary>
        public const string NUM_ONE_TWO = @"[1,2]";

        /// <summary>
        /// 大于零的整数
        /// </summary>
        public const string INT_FOR_GREAT_ZERO = @"^[1-9]{1}[0-9]*$";

        /// <summary>
        /// 大于一的整数
        /// </summary>
        public const string INT_FOR_GREAT_ONE = @"^[2-9]{1}[0-9]*$";

        /// <summary>
        /// 整数
        /// </summary>
        public const string INT = @"^[0-9]+$";

        /// <summary>
        /// 只能为汉字
        /// </summary>
        public const string IS_CHINESE = @"^[^\x00-\xff]+$";

        /// <summary>
        /// 四位增长的全数字编号(比如：资讯分类ID,城市ID)
        /// </summary>
        public const string CODE_WITH_DIGIT_FOR_FOUR_INCREASE = @"^(0|([0-9]{4})+)$";

        /// <summary>
        /// 评论分数(1.0-5.0)
        /// </summary>
        public const string COMMENT_SCORE = @"^[1-4]{1}(.[0-9]{1})?$|^[5]{1}(.[0]{1})?$";

        /// <summary>
        /// 区域ID
        /// </summary>
        public const string DISTRICT_ID = @"^(0|(\d{4})+)$";

        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = @"^(0|([a-zA-Z0-9\-]{36}))$";

        /// <summary>
        /// 年月
        /// </summary>
        public const string YEAR_MONTH = @"^[12]\d{3}(0[1-9])|1[0-2]$";

        /// <summary>
        /// 年月日
        /// </summary>
        public const string YEAR_MONTH_DAY = @"^[12]\d{3}-(0[1-9])|1[0-2]-(0[1-9])|[1-3][0-1]$";

        /// <summary>
        /// 排序值
        /// </summary>
        public const string ORDER_FIELD = @"^[0-6]{1}$";

        /// <summary>
        /// 排序类型
        /// </summary>
        public const string ORDER_TYPE = @"^[0,1]{1}$";

        /// <summary>
        /// 布尔类型 0，1
        /// </summary>
        public const string BOOL_TYPE = @"^[0,1]{1}$";

        /// <summary>
        /// 字符串默认
        /// </summary>
        public const string STRING_DEFAULT = @"^[0]{1}$";

        /// <summary>
        /// 金额（整数位最多十位，小数为最多为两位，可以无小数位）
        /// </summary>
        public const string AMOUNT_PATTERN = @"^(([0-9]|([1-9][0-9]{0,9}))((\.[0-9]{1,2})?))$";

        /// <summary>
        /// 百分比不含百分号（整数位最多2位，小数为最多为两位，可以无小数位）
        /// </summary>
        public const string PERCENTAGE_PATTERN = @"^(([0-9]|([1-9][0-9]{0,2}))((\.[0-9]{1,2})?))$";

        /// <summary>
        /// 打印机类型第一二位01代表文档，02代表图片，第三四位01代表黑白，02代表彩色，第五六位表示尺寸
        /// </summary>
        public const string PRINTER_TYPE_PATTERN = @"^[0-9]{4}[a-zA-Z0-9#]{2}$";

        /// <summary>
        /// 店铺名称正则（数字字母和汉字）
        /// </summary>
        public const string SHOP_NAME_PATTERN = @"^[a-zA-Z0-9\u4e00-\u9fa5]+$";

        /// <summary>
        /// 16进制组成
        /// </summary>
        public const string BINARY_16_PATTERN = @"^[a-fA-F0-9]{12}$";

        /// <summary>
        /// 支付宝帐号（手机号或者Emall）
        /// </summary>
        public const string ALIPAY_PATTERN = @"^(1[3,4,5,7,8]{1}[0-9]{1}[0-9]{8})|([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        /// <summary>
        /// 图片验证码
        /// </summary>
        public const string IMAGE_CODE_PATTERN = @"[a-zA-Z0-9]{4}$";

        /// <summary>
        /// 版本号
        /// </summary>
        public const string VERSION_PATTERN = @"^[0-9]+(\.[0-9]){2}$";

        /// <summary>
        /// JSON正则校验
        /// </summary>
        public const string JSON_PATTERN = @"";

        #region 日期时间正则

        /// <summary>
        /// 日期格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 日期一天中最大时间 yyyy-MM-dd 23:59:59 格式
        /// </summary>
        public const string DATE_LAST_TIME_FORMAT = "yyyy-MM-dd 23:59:59";

        /// <summary>
        /// 日期一天中最小时间 yyyy-MM-dd 00:00:00 格式
        /// </summary>
        public const string DATE_FIRST_TIME_FORMAT = "yyyy-MM-dd 00:00:00";

        /// <summary>
        /// 短日期 yyyy-MM-dd
        /// </summary>
        public const string SHORTDATE_FORMAT = "yyyy-MM-dd";

        /// <summary>
        /// 日期格式（yyyy-MM-dd HH:mm）,不带秒
        /// </summary>
        public const string DATETIME_FORMAT_WITHOUT_SECOND = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// 日期格式（MM-dd HH:mm）,不带年，秒
        /// </summary>
        public const string DATETIME_FORMAT_WITHOUT_YEAR_SECOND = "MM-dd HH:mm";

        /// <summary>
        /// 时间格式（HH:mm:ss），只有时间
        /// </summary>
        public const string TIME_FORMAT = "HH:mm:ss";

        /// <summary>
        /// 时间字符串格式(14位)
        /// </summary>
        public const string TIME_FORMAT_SECOND = "yyyyMMddHHmmss";

        /// <summary>
        /// 时间字符串格式(8位)
        /// </summary>
        public const string DATE_FORMAT_DAY = "yyyyMMdd";

        /// <summary>
        /// 时间字符串格式(4位,年月)
        /// </summary>
        public const string DATE_FORMAT_YEAR_MONTH = "yyyyMM";

        #endregion
    }
}
