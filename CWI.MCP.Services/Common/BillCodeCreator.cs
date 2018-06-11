//---------------------------------------------
// 版权信息：版权所有(C) 2012，PAIDUI.COM
// 变更历史：
//      姓名         日期             说明
// --------------------------------------------
//      周洲      2013/04/11          创建
//---------------------------------------------

using System;
using CWI.MCP.Common;
using Evt.Framework.Common;
using CWI.MCP.Models;

namespace  CWI.MCP.Services
{
    /// <summary>
    /// 一个接口,用于指示生成单据编号所需要的数据信息
    /// </summary>
    public abstract class BillCodeCreator
    {
        #region 属性

        /// <summary>
        /// 表名
        /// </summary>
        protected abstract string TableName { get; }

        /// <summary>
        /// 单号字段
        /// </summary>
        protected abstract string CodeField { get; }

        /// <summary>
        /// 流水号长度
        /// </summary>
        protected virtual int SerialLength { get { return 6; } }

        /// <summary>
        /// 单据前缀
        /// </summary>
        protected abstract string Prefix { get; }

        /// <summary>
        /// 其他条件
        /// </summary>
        protected virtual string OtherCondition { get { return string.Empty; } }

        #endregion

        /// <summary>
        /// 获取单据编号
        /// </summary>
        /// <returns>单据编号</returns>
        public static string GetBillCode(OrderType billType)
        {
            return BillCodeCreator.Create(billType).GetNewBillCode();
        }

        /// <summary>
        /// 创建单据编号生成器
        /// </summary>
        /// <param name="billType">单据类型</param>
        /// <returns></returns>
        private static BillCodeCreator Create(OrderType billType)
        {
            BillCodeCreator creator = null;
            var type = Type.GetType(string.Format("CWI.MCP.Services.{0}Code", billType.ToString()));
            creator = Activator.CreateInstance(type) as BillCodeCreator;
            return creator;
        }

        /// <summary>
        /// 生成单据编号
        /// </summary>
        /// <returns>单据编号</returns>
        private string GetNewBillCode()
        {
            int maxNum = 99999;
            ParameterCollection pc = new ParameterCollection();
            string strCondition = OtherCondition;
            var newCode = string.Empty;
            var lastCode = string.Empty;
            var querySql = string.Format("SELECT `{0}` FROM {1} {2}  ORDER BY `{0}` DESC LIMIT 0,1", CodeField, TableName, strCondition);
            lastCode = TryConvertUtil.ToString(DbUtil.DataManager.DataManagerMcp.IData.ExecuteScalar(querySql), string.Empty);
            var lastNum = -1;
            var prefixLength = 0;
            var billSerialCode = string.Empty;
            var day = CommonUtil.GetDBDateTime().Date.ToString("yyMMdd");
            if (ConfigUtil.IsTestModel)
            {
                day = CommonUtil.GetDBDateTime().Date.AddYears(-10).ToString("yyMMdd");//测试环境使用

                querySql = string.Format("SELECT `{0}` FROM {1} WHERE LEFT({0},{3})='{2}'  ORDER BY `{0}` DESC LIMIT 0,1", CodeField, TableName, Prefix + day, Prefix.Length + day.Length);
                lastCode = TryConvertUtil.ToString(DbUtil.DataManager.DataManagerMcp.IData.ExecuteScalar(querySql), string.Empty);
            }
            if (lastCode != null && !string.IsNullOrWhiteSpace(lastCode.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(Prefix))
                {
                    //单据号类似: T2013041100001
                    billSerialCode = lastCode.ToString().TrimStart(Prefix.ToCharArray());
                    prefixLength = Prefix.Length;
                }
                else
                {
                    //单据号类似: 130411000001
                    billSerialCode = lastCode;
                }

                var dateLen = 6;    //日期长度
                if (billSerialCode.Length == dateLen + SerialLength)
                {
                    //比较日期
                    var y = TryConvertUtil.ToInt(lastCode.Substring(prefixLength + 0, 2), 10);
                    if (ConfigUtil.IsTestModel)
                    {
                        var temp = TryConvertUtil.ToInt(lastCode.Substring(prefixLength + 0, 2), 10);
                        y = temp > 10 ? temp - 5 : temp;//测试环境使用
                    }
                    var m = TryConvertUtil.ToInt(lastCode.Substring(prefixLength + 2, 2), 1);
                    var d = TryConvertUtil.ToInt(lastCode.Substring(prefixLength + 4, 2), 1);
                    var n = billSerialCode.Substring(6);

                    //日期相等
                    if (day == (new DateTime(y, m, d)).ToString("yyMMdd"))
                    {
                        lastNum = TryConvertUtil.ToInt(n.TrimStart('0'), 1);
                    }
                }
            }
            if (lastNum == -1)
            {
                lastNum = 0;
            }
            lastNum += 1;
            if (lastNum > maxNum)
            {
                throw new BusinessException("当日数据异常");
            }
            newCode = string.Format("{0}{1}{2}", !string.IsNullOrWhiteSpace(Prefix) ? string.Format("{0}", Prefix) : string.Empty, day, lastNum.ToString().PadLeft(SerialLength, '0'));
            return newCode;
        }
    }

    /// <summary>
    /// 1.打印订单
    /// </summary>
    internal class PrintOrderCode : BillCodeCreator
    {
        protected override string TableName
        {
            get { return "mcp_order"; }
        }

        protected override string CodeField
        {
            get { return "order_id"; }
        }

        protected override string Prefix
        {
            get { return "P"; }
        }

        protected override int SerialLength
        {
            get { return 9; }
        }
    }
}
