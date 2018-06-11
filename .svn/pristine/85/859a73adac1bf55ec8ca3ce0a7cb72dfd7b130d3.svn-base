using System;
using System.Text;
using System.IO;
using System.Web;

namespace  CWI.MCP.Common
{
    public static class FileOperateUtil
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="createPath">文件地址，从本站根目录开始相对地址  前面不需要加"/"</param>
        /// <param name="html">文件内容</param>
        /// <returns></returns>
        public static bool CreateFile(string createPath, string html)
        {
            bool flag = false;

            try
            {
                string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
                FileStream fs = new FileStream(path + createPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                //StreamWriter sw = new StreamWriter("test.txt", true);
                sw.Write(HttpUtility.UrlDecode(html, System.Text.Encoding.UTF8));
                sw.Close();
                fs.Close();
                flag = true;
            }
            catch { }
            return flag;
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="text">内容</param>
        public static void CreateFile(string filePath, string text, string Encod)
        {
            StreamWriter sw = null;
            //Business.Folder.Files.CreateFolder(filePath.Substring(0, filePath.LastIndexOf("\\") + 1));
            try
            {
                sw = new StreamWriter(filePath, false, Encoding.GetEncoding(Encod));
                sw.WriteLine(text);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                //logger.Error("创建文件出错 文件地址:" + filePath, ex);
                throw ex;
                //sw.Close();
            }

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">路径</param>

        public static bool DeleteFile(string filePath)
        {
            bool flag = false;
            try
            {
                File.Delete(filePath);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 读取 外部文件
        /// </summary>
        /// <param name="path">完整物理路径</param>
        /// <returns></returns>
        public static string ReadFileOut(string path, string encod = "utf-8")
        {
            //读取 外部文件
            StringBuilder html = new StringBuilder();
            try
            {
                using (StreamReader reader = new StreamReader(path, System.Text.Encoding.GetEncoding(encod)))
                {
                    while (reader.Peek() >= 0)
                    {
                        html.Append(((char)reader.Read()).ToString());
                    }
                }
            }
            catch { return null; }
            return html.ToString();
        }


        /// <summary>
        /// 读取 内部文件
        /// </summary>
        /// <param name="path">站内完整路径</param>
        /// <returns></returns>
        public static string ReadFileIn(string path, string encod = "utf-8")
        {
            string html = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + path, System.Text.Encoding.GetEncoding(encod));
                //BinaryReader br = new BinaryReader(sr);
                html = sr.ReadToEnd();
                sr.Close();
            }
            catch { }

            return html;
        }
    }
}
