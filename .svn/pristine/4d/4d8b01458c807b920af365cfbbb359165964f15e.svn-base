//---------------------------------------------
// 版权信息：版权所有(C) 2014，COOLWI.COM
// 变更历史：
//      姓名          日期              说明
// --------------------------------------------
//      王军锋        2014/08/06        创建
//---------------------------------------------
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public class SerializerUtil
    {
        /// <summary>
        /// 将指定的对象序列化为XML文件或二进制文件并返回执行状态。
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存路径</param>
        /// <param name="isBinaryFile">序列化后生成的文件类型是否为二进制文件，true为二进制文件，否则为xml文件或文本文件</param>
        /// <returns>返回执行状态</returns>
        public static bool Serialize(object o, string path, bool isBinaryFile)
        {
            bool flag = false;
            try
            {
                if (isBinaryFile)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        formatter.Serialize(stream, o);
                        flag = true;
                    }
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(o.GetType());
                    using (XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlSerializerNamespaces n = new XmlSerializerNamespaces();
                        n.Add("", "");
                        serializer.Serialize(writer, o, n);
                        flag = true;
                    }
                }
            }
            catch { flag = false; }
            return flag;
        }

        /// <summary>
        /// 将指定的对象序列化为XML格式的字符串并返回。
        /// </summary>
        /// <param name="o">待序列化的对象</param>
        /// <returns>返回序列化后的字符串</returns>
        public static string Serialize(object o)
        {
            string xml = "";
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                using (MemoryStream mem = new MemoryStream())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(mem, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlSerializerNamespaces n = new XmlSerializerNamespaces();
                        n.Add("", "");
                        serializer.Serialize(writer, o, n);

                        mem.Seek(0, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(mem))
                        {
                            xml = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch { xml = ""; }
            return xml;
        }

        /// <summary>
        /// 从指定的文件中反序列化出对应的对象并返回。
        /// </summary>
        /// <param name="t">要反序列化的对象类型</param>
        /// <param name="path">文件路径</param>
        /// <param name="isBinaryFile">反序列化的文件类型是否为二进制文件，true为二进制文件，否则为xml文件或文本文件</param>
        /// <returns>返回object</returns>
        public static T Deserialize<T>(string path, bool isBinaryFile) where T : class,new()
        {
            T o = null;
            try
            {
                if (File.Exists(path))
                {
                    if (!isBinaryFile)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        using (XmlTextReader reader = new XmlTextReader(path))
                        {
                            o = serializer.Deserialize(reader) as T;
                        }
                    }
                    else
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            o = formatter.Deserialize(stream) as T;
                        }
                    }
                }
            }
            catch { o = null; }
            return o;
        }

        ///<summary>
        /// 序列化 对象到字符串
        ///</summary>
        ///<param name="obj">泛型对象</param>
        ///<returns>序列化后的字符串</returns>
        public static string Serialize<T>(T obj)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                return Convert.ToBase64String(buffer);
            }
            catch (Exception ex)
            {
                return string.Empty;
                // throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        ///<summary>
        /// 反序列化 字符串到对象
        ///</summary>
        ///<param name="obj">泛型对象</param>
        ///<returns>反序列化出来的对象</returns>
        public static T DeserializeFromStr<T>(string str) where T : class
        {
            try
            {
                T obj = null;
                IFormatter formatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(str);
                MemoryStream stream = new MemoryStream(buffer);
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();

                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 将指定的xml格式的字符反序列化为对应的对象并返回。
        /// </summary>
        /// <param name="t">对象的类型</param>
        /// <param name="xml">待反序列化的xml格式的字符的内容</param>
        /// <returns>返回对应的对象</returns>
        public static T DeserializeFromXml<T>(string xml) where T : class,new()
        {
            T o = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    o = serializer.Deserialize(mem) as T;
                }
            }
            catch { o = null; }
            return o;
        }

        /// <summary>
        /// 将指定的对象序列化为XML文件，并返回执行状态。
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">生成的文件名称</param>
        /// <returns>返回执行状态</returns>
        public static bool XmlSerialize(object o, string path)
        {
            return SerializerUtil.Serialize(o, path, false);
        }

        /// <summary>
        /// 将指定XML文件，反序列化为对应的对象并返回。
        /// </summary>
        /// <param name="t">对象的类型</param>
        /// <param name="path">XML文件路径</param>
        /// <returns>返回对象</returns>
        public static T XmlDeserialize<T>(string path) where T : class,new()
        {
            return SerializerUtil.Deserialize<T>(path, false);
        }

        /// <summary>
        /// 将指定的对象序列化为二进制文件，并返回执行状态。
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">生成的文件名称</param>
        /// <returns>返回执行状态</returns>
        public static bool BinarySerialize(object o, string path)
        {
            return SerializerUtil.Serialize(o, path, true);
        }

        /// <summary>
        /// 将指定二进制文件，反序列化为对应的对象并返回。
        /// </summary>
        /// <param name="t">对象的类型</param>
        /// <param name="path">XML文件路径</param>
        /// <returns>返回对象</returns>
        public static T BinaryDeserialize<T>(string path) where T : class,new()
        {
            return SerializerUtil.Deserialize<T>(path, true);
        }

        /// <summary>
        /// 把对象序列化为字节数组
        /// </summary>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            return bytes;
        }

        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public static object DeserializeObject(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
}
