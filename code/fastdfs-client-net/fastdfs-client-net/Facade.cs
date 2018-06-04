using fastdfs_client_net.common;
using fastdfs_client_net.fastdfs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace fastdfs_client_net
{
    public class Facade
    {
        static Facade()
        {
            List<IPEndPoint> trackerIPs = new List<IPEndPoint>();
            try
            {
                if (string.IsNullOrEmpty(FDFSConfig.TrackerNodes) == false)
                {
                    string[] nodes = FDFSConfig.TrackerNodes.Split(new char[] { ',' });
                    foreach (string node in nodes)
                    {
                        if (node.IndexOf(':') > -1)
                        {
                            trackerIPs.Add(new IPEndPoint(IPAddress.Parse(node), 22122));
                        }
                        else
                        {
                            var ipAndPort = node.Split(new char[] { ':' });
                            if (ipAndPort.Length == 1)
                                trackerIPs.Add(new IPEndPoint(IPAddress.Parse(ipAndPort[0]), 22122));
                            else
                                trackerIPs.Add(new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("fastdfs配置存在问题，请检查配置:", ex);
            }
            ConnectionManager.Initialize(trackerIPs);
        }


        public static string Upload(string fileName, string groupName = "")
        {
            string fix = Path.GetExtension(fileName).TrimStart(new char[] { '.' });
            return Upload(fileName, fix, groupName);
        }

        public static string Upload(Stream stream, string fix, string groupName = "")
        {
            byte[] content = null;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                content = reader.ReadBytes((int)stream.Length);
            }
            string name = Upload(content, fix, groupName);
            TinyLog.TinyLog.Instance.Info(string.Format("{0}|{1}|{2}|{3}|{4}", "流文件", content.Length, "", name, "UPLOAD"));
            return name;
        }


        /// <summary>
        /// 上传文件，失败了会重试，默认重试10次
        /// </summary>
        /// <param name="filePath">文件名</param>
        /// <param name="fix">后缀</param>
        /// <param name="groupName">组名</param>
        /// <returns>服务器文件名</returns>
        public static string Upload(string filePath, string fix, string groupName = "")
        {
            byte[] content = null;
            if (File.Exists(filePath))
            {
                FileStream streamUpload = new FileStream(filePath, FileMode.Open);
                using (BinaryReader reader = new BinaryReader(streamUpload))
                {
                    content = reader.ReadBytes((int)streamUpload.Length);
                }
            }
            string name = Upload(content, fix, groupName);
            TinyLog.TinyLog.Instance.Info(string.Format("{0}|{1}|{2}|{3}|{4}", filePath, content.Length, "", name, "UPLOAD"));
            return name;
        }

        /// <summary>
        /// 上传文件，失败了会重试，默认重试10次
        /// </summary>
        /// <param name="oFileContent">btye数组</param>
        /// <param name="fix">后缀</param>
        /// <returns>服务器文件名</returns>
        public static string Upload(byte[] content, string fix, string groupName = "")
        {
            for (int i = 0; i < FDFSConfig.FileServerUploadFailRetryTime; i++)
            {
                try
                {
                    string gn = string.IsNullOrEmpty(groupName) ? FDFSConfig.DefaultGroupName : groupName;
                    return Client.Upload(gn, content, fix);
                }
                catch (Exception ex)
                {
                    TinyLog.TinyLog.Instance.Error(string.Format("文件上传出错,出错次数【{0}】：", (i + 1)) + ex.ToString());
                    Thread.Sleep(200 * i + 100);
                }
            }
            //上传最终失败
            TinyLog.TinyLog.Instance.Error("上传文件" + FDFSConfig.FileServerUploadFailRetryTime + "次，均失败，文件大小为：" + content.Length);
            return "";
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="targetFile">目标文件名</param>
        private static void InnerDownLoad(string file, string targetFile)
        {
            if (string.IsNullOrEmpty(file) || string.IsNullOrEmpty(targetFile))
            {
                throw new Exception("文件名不能为空");
            }

            string url = FDFSConfig.DownloadUrl + file;
            for (int i = 0; i < FDFSConfig.FileServerDownloadFailRetryTime; i++)
            {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        //设置http访问连接数限制
                        //System.Net.ServicePointManager.DefaultConnectionLimit = 50;
                        //关闭代理，解决第二次连接慢的问题
                        web.Proxy = null;
                        web.DownloadFile(url, targetFile);
                    }
                    TinyLog.TinyLog.Instance.Info(string.Format("{0}|{1}|{2}|{3}|{4}", file, "", "第" + (i + 1) + "次下载", targetFile, "DOWNLOAD"));
                    return;
                }
                catch (Exception ex)
                {
                    TinyLog.TinyLog.Instance.Error(string.Format("文件下载出错,出错次数【{0}】：", (i + 1)) + ex.ToString());
                }
                if (File.Exists(file) == false)
                {
                    Thread.Sleep(200 * i + 100);
                }
                else
                {
                    break;
                }
            }
            TinyLog.TinyLog.Instance.Error("下载文件" + FDFSConfig.FileServerDownloadFailRetryTime + "次，均失败，文件为：" + file + " 目标文件：" + targetFile);
        }

        /// <summary>
        /// 下载文件，如果源文件为本地文件，不进行下载
        /// </summary>
        /// <param name="file">文件名</param>
        /// <param name="targetFile">目标文件</param>
        /// <param name="isOverrideFile">如果文件存在，是否覆盖文件</param>
        /// <returns>实际文件名</returns>
        public static string DownLoad(string file, string targetFile, bool isOverrideFile = true)
        {
            //绝对路径不进行下载
            if (Path.IsPathRooted(file))
            {
                return file;
            }
            string strTgtFile = targetFile;
            if (Path.IsPathRooted(strTgtFile) == false)
            {
                strTgtFile = Path.Combine(FDFSConfig.FastDFSDownloadPath, strTgtFile);
            }
            if (!File.Exists(strTgtFile) || isOverrideFile)
            {
                InnerDownLoad(file, strTgtFile);
            }
            return strTgtFile;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="filePathName">服务器文件路径</param>
        /// <returns>是否成功</returns>
        public static bool Remove(string filePathName, string groupName = "")
        {
            if (string.IsNullOrEmpty(groupName))
                groupName = FDFSConfig.DefaultGroupName;
            //绝对路径直接删除
            try
            {
                if (Path.IsPathRooted(filePathName))
                {
                    if (File.Exists(filePathName))
                    {
                        File.Delete(filePathName);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (FDFSConfig.IsLogicDelete)
                    {
                        TinyLog.TinyLog.Instance.Info(string.Format("{0}|{1}|{2}|{3}|{4}", filePathName, "unkown", "group:" + groupName, filePathName, "LOGIC_DELETE"));

                    }
                    else
                    {
                        TinyLog.TinyLog.Instance.Info(string.Format("{0}|{1}|{2}|{3}|{4}", filePathName, "unkown", "group:" + groupName, filePathName, "REAL_DELETE"));
                        Client.Remove(groupName, filePathName);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                TinyLog.TinyLog.Instance.Error("删除文件异常:" + ex.ToString());
                return false;
            }
        }


    }
}
