//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// xml操作类
    /// </summary>
    public static class XmlUtil
    {
        /// <summary>
        /// 加载XML文件
        /// </summary>
        /// <param name="filePath">xml文件路径</param>
        /// <returns>返回LoadXmlFile</returns>
        public static XmlDocument LoadXmlFile(string fileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            return xmlDoc;
        }

        /// <summary>
        /// 加载XML片段
        /// </summary>
        /// <param name="xml">xml片段</param>
        /// <returns>返回XmlDocument</returns>
        public static XmlDocument LoadXmlString(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            return xmlDoc;
        }
    }

    /// <summary>
    /// XML操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class XmlDAL<T> where T : new()
    {
        //编码类型
        private static Encoding code = Encoding.GetEncoding("gb2312");

        /// <summary>
        /// 获取XML格式数据信息
        /// </summary>
        /// <param name="obj">数据类型</param>
        public static string GetXmlString(T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Encoding = code;
            setting.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(stream, setting))
            {
                xs.Serialize(writer, obj);
            }
            return code.GetString(stream.ToArray());
        }

        #region 序列化&反序列化对象成字符串

        /// <summary> 
        /// 序列化对象 
        /// </summary> 
        /// <typeparam name=\"T\">对象类型</typeparam> 
        /// <param name=\"t\">对象</param> 
        /// <returns></returns> 
        public static string SerializeXml<T>(T t)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerializer xz = new XmlSerializer(t.GetType());
                xz.Serialize(sw, t);
                return sw.ToString();
            }
        }

        /// <summary> 
        /// 反序列化为对象 
        /// </summary> 
        /// <param name=\"type\">对象类型</param> 
        /// <param name=\"s\">对象序列化后的Xml字符串</param> 
        /// <returns></returns> 
        public static object DeserializeXml<T>(T t, string s)
        {
            using (StringReader sr = new StringReader(s))
            {
                XmlSerializer xz = new XmlSerializer(t.GetType());
                return xz.Deserialize(sr);
            }
        }

        /// <summary> 
        /// 反序列化为对象 
        /// </summary> 
        /// <param name=\"type\">对象类型</param> 
        /// <param name=\"s\">对象序列化后的Xml字符串</param> 
        /// <returns></returns> 
        public static object DeserializeXml(Type type, string s) { 
            using(StringReader sr = new StringReader(s)) {
                XmlSerializer xz = new XmlSerializer(type); 
                return xz.Deserialize(sr); 
            } 
        }

        #endregion

        #region 读写XML文件

        /// <summary>
        /// 写XML
        /// </summary>
        /// <param name="path">包含文件名的XML文件完整路径</param>
        /// <param name="obj">数据类型</param>
        public static void WriteXml(string path, T obj) 
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            StreamWriter myWriter = new StreamWriter(path, false, code);
            mySerializer.Serialize(myWriter, obj);
            myWriter.Close();
        }

        /// <summary>
        /// 读XML
        /// </summary>
        /// <param name="path">包含文件名的XML文件完整路径</param>
        /// <returns>数据类型</returns>
        public static T ReadXml(string path)
        {
            T obj;
            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            TextReader myReader = new StreamReader(path, code);
            obj = (T)mySerializer.Deserialize(myReader);
            myReader.Close();
            return obj;
        }

        #endregion
    } 
}
