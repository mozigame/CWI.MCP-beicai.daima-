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

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 文件处理公共类
    /// </summary>
    public class TxtFileUtil
    {
        /// <summary>
        /// 文本文件编码
        /// </summary>
        private class TxtFileEncoding
        {
            public TxtFileEncoding()
            {
            }

            /// <summary>
            /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。
            /// </summary>
            /// <param name="fileName">文件名。</param>
            /// <returns></returns>
            public static Encoding GetEncoding(string fileName)
            {
                return GetEncoding(fileName, Encoding.Default);
            }

            /// <summary>
            /// 取得一个文本文件流的编码方式。
            /// </summary>
            /// <param name="stream">文本文件流。</param>
            /// <returns></returns>
            public static Encoding GetEncoding(FileStream stream)
            {
                return GetEncoding(stream, Encoding.Default);
            }

            /// <summary>
            /// 取得一个文本文件的编码方式。
            /// </summary>
            /// <param name="fileName">文件名。</param>
            /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
            /// <returns></returns>
            public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
                fs.Close();
                return targetEncoding;
            }

            /// <summary>
            /// 取得一个文本文件流的编码方式。
            /// </summary>
            /// <param name="stream">文本文件流。</param>
            /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
            /// <returns></returns>
            public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
            {
                Encoding targetEncoding = defaultEncoding;
                if (stream != null && stream.Length >= 2)
                {
                    //保存文件流的前4个字节
                    byte byte1 = 0;
                    byte byte2 = 0;
                    byte byte3 = 0;
                    byte byte4 = 0;
                    //保存当前Seek位置
                    long origPos = stream.Seek(0, SeekOrigin.Begin);
                    stream.Seek(0, SeekOrigin.Begin);

                    int nByte = stream.ReadByte();
                    byte1 = Convert.ToByte(nByte);
                    byte2 = Convert.ToByte(stream.ReadByte());
                    if (stream.Length >= 3)
                    {
                        byte3 = Convert.ToByte(stream.ReadByte());
                    }
                    if (stream.Length >= 4)
                    {
                        byte4 = Convert.ToByte(stream.ReadByte());
                    }
                    //根据文件流的前4个字节判断Encoding
                    //Unicode {0xFF, 0xFE};
                    //BE-Unicode {0xFE, 0xFF};
                    //UTF8 = {0xEF, 0xBB, 0xBF};
                    if (byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
                    {
                        targetEncoding = Encoding.BigEndianUnicode;
                    }
                    if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
                    {
                        targetEncoding = Encoding.Unicode;
                    }
                    if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
                    {
                        targetEncoding = Encoding.UTF8;
                    }
                    //恢复Seek位置      
                    stream.Seek(origPos, SeekOrigin.Begin);
                }
                return targetEncoding;
            }
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadText(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    return string.Empty;

                Encoding fileEncoding = TxtFileEncoding.GetEncoding(fileName, Encoding.UTF8);
                StreamReader sr = new StreamReader(fileName, fileEncoding);
                string str = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();

                return str;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 以文件流的形式将内容写入到指定文件中（如果该文件或文件夹不存在则创建）
        /// </summary>
        /// <param name="file">文件名和指定路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <returns>返回布尔值</returns>
        public static string WriteFile(string file, string fileContent)
        {
            FileInfo f = new FileInfo(file);
            // 如果文件所在的文件夹不存在则创建文件夹
            if (!Directory.Exists(f.DirectoryName)) Directory.CreateDirectory(f.DirectoryName);

            FileStream fStream = new FileStream(file, FileMode.Create, FileAccess.Write);
            StreamWriter sWriter = new StreamWriter(fStream, Encoding.UTF8);

            try
            {
                sWriter.Write(fileContent);
                return fileContent;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.ToString());
            }
            finally
            {
                sWriter.Flush();
                fStream.Flush();
                sWriter.Close();
                fStream.Close();
            }
        }

        /// <summary>
        /// 以文件流的形式将内容写入到指定文件中（如果该文件或文件夹不存在则创建）
        /// </summary>
        /// <param name="file">文件名和指定路径</param>
        /// <param name="fileContent">文件内容</param>
        /// <param name="Append">是否追加指定内容到该文件中</param>
        /// <returns>返回布尔值</returns>
        public static void WriteFile(string file, string fileContent, bool Append)
        {
            FileInfo f = new FileInfo(file);
            // 如果文件所在的文件夹不存在则创建文件夹
            if (!Directory.Exists(f.DirectoryName)) Directory.CreateDirectory(f.DirectoryName);

            StreamWriter sWriter = new StreamWriter(file, Append, Encoding.UTF8);

            try
            {
                sWriter.Write(fileContent);
            }
            catch (Exception exc)
            {
                throw new Exception(exc.ToString());
            }
            finally
            {
                sWriter.Flush();
                sWriter.Close();
            }
        }
    }
}
