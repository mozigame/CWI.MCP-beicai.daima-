// 版权信息：版权所有(C) 2011，Evervictory Tech
// 变更历史：
//      姓名              日期          说明
// --------------------------------------------------------
//    王军锋           2011/12/28       创建
// --------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using Evt.Framework.Common;
using System.IO;

namespace  CWI.MCP.Common
{
    /// <summary>
    /// 文件上传工具类
    /// </summary>
    public class UploadFileUtil
    {
        /// <summary>
        /// 同步锁静态变量
        /// </summary>
        private static object objSync = new object();

        /// <summary>
        /// 文件扩展名集合
        /// </summary>
        private static string[] fileExtName = {".jpg",".jpeg",".png"};

        /// <summary>
        /// 添加上传的图片，并返回图片添加成功后所在的虚拟目录 
        /// </summary>
        /// <param name="postedFile">从客户端上传过来的图片对象</param>
        /// <returns>图片添加成功后所在的虚拟目录</returns>
        public static string SaveFile(HttpPostedFileBase postedFile)
        {
            return SaveFile(postedFile, "Upload");
        }

        /// <summary>
        /// 添加上传的图片，并返回图片添加成功后所在的虚拟目录 
        /// </summary>
        /// <param name="postedFile">从客户端上传过来的图片对象</param>
        /// <param name="directory">目录名</param>
        /// <param name="isGzip">是否使用Gzip压缩过的图片文件</param>
        /// <returns>图片添加成功后所在的虚拟目录</returns>
        public static string SaveFile(HttpPostedFileBase postedFile, string directory)
        {
            if (postedFile == null || postedFile.ContentLength == 0)
            {
                throw new MessageException("文件对象不能为空！");
            }

            var extName = Path.GetExtension(postedFile.FileName);
            CheckIsValidFile(extName);
            CheckFileSize(postedFile);
            string virtualPath = GetVirtualPath(postedFile.FileName, directory);
            string physicalPath = GetPhysicalPath(virtualPath);
            postedFile.SaveAs(physicalPath);
            return virtualPath;
        }

        /// <summary>
        /// 校验文件扩展名是否支持
        /// </summary>
        /// <param name="extName">上传的图片名</param>
        public static void CheckIsValidFile(string extName)
        {
            if (!fileExtName.Contains(extName.ToLower()))
            {
                throw new MessageException(string.Format("{0}文件格式不支持，支持的文件格式有：{1}。", extName.ToLower(), ConfigUtil.GetConfig("AllowFileTypes").Replace(",", " ")));
            }
        }

        /// <summary>
        /// 校验上传文件大小
        /// </summary>
        /// <param name="fileUploader">上传文件</param>
        public static void CheckFileSize(HttpPostedFileBase fileUploader)
        {
            int fileSize = fileUploader.ContentLength;
            int maxSize = ConvertUtil.ToInt(ConfigUtil.GetConfig("UploadFileMaxSize"));
            if (maxSize <= 0)
                maxSize = 10 * 1024 * 1024; //10M

            if (fileSize > maxSize)
            {
                throw new MessageException(String.Format("您上传文件大小为{0}超过了单个文件最大限制{1}。", GetFileSizeFormat(fileSize), GetFileSizeFormat(maxSize)));
            }
        }

        /// <summary>
        /// 根据文件路径获取文件大小
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件大小</returns>
        public static long GetFileSize(string filePath)
        {
            long temp = 0;

            //判断当前路径所指向的是否为文件
            if (File.Exists(filePath) == false)
            {
                string[] str1 = Directory.GetFileSystemEntries(filePath);
                foreach (string s1 in str1)
                {
                    temp += GetFileSize(s1);
                }
            }
            else
            {
                //以获取其大小
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            return temp;
        }

        /// <summary>
        /// 获得格式的文件大小
        /// </summary>
        /// <param name="fileSizeBytes">用字节表示的文件的大小</param>
        /// <returns>格式化后的文件大小文本</returns>
        public static string GetFileSizeFormat(long fileSizeBytes)
        {
            if (fileSizeBytes > 1024 * 1024 * 1024)
            {
                return Math.Round((decimal)fileSizeBytes / (1024 * 1024 * 1024), 2).ToString() + "GB";
            }
            else if (fileSizeBytes > 1024 * 1024)
            {
                return Math.Round((decimal)fileSizeBytes / (1024 * 1024), 2).ToString() + "MB";
            }
            else if (fileSizeBytes > 1024)
            {
                return Math.Round((decimal)fileSizeBytes / 1024, 2).ToString() + "KB";
            }
            else
            {
                return fileSizeBytes.ToString() + "B";
            }
        }

        /// <summary>
        /// 获取文件虚拟路径
        /// </summary>
        /// <param name="fileName">客户端上传文件的完全限定名</param>
        /// <param name="directory">目录名</param>
        /// <returns>文件虚拟路径</returns>
        public static string GetVirtualPath(string fileName, string directory)
        {
            string savefileName = string.Empty;
            string extName = Path.GetExtension(fileName);
            string timeStamp = DateTime.Now.ToString(RegexConsts.TIME_FORMAT_SECOND);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            int fileNameLength = fileNameWithoutExtension.Length;
            if (fileNameLength > 100)
            {
                //用户名长度超过100个采取截断策略
                savefileName = string.Format("{0}_{1}{2}", fileNameWithoutExtension.Substring(fileNameLength - 100, 100), timeStamp, extName);
            }
            else
            {
                savefileName = fileName.Replace(extName, string.Format("_{0}{1}", timeStamp, extName));
            }

            return String.Format("/Content/{0}/{1}", directory, savefileName);
        }

        /// <summary>
        /// 获取物理虚拟路径
        /// </summary>
        /// <param name="virtualPath">虚拟地址，含文件的名称</param>
        /// <returns>文件绝对路径</returns>
        public static string GetPhysicalPath(string virtualPath)
        {
            //物理地址
            string physicalPath = HttpContext.Current.Server.MapPath(virtualPath);
            string[] path = physicalPath.Split('\\');
            string filePath = physicalPath.Replace("\\" + path[path.Length - 1], string.Empty);

            if (!Directory.Exists(filePath))
            {
                lock (objSync)
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                }
            }
            return physicalPath;
        }
    }
}