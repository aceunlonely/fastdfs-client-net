using fastdfs_client_net.thirds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace fastdfs_client_net
{
	/// <summary>
	/// fastDfs外部配置
	/// </summary>
	public class FDFSConfig
	{
		public static string TrackerNodes { get { return ConfigureHelper.GetConfigureValue("fastdfs_Nodes", ""); } }

		public static string DefaultGroupName { get { return ConfigureHelper.GetConfigureValue("fastdfs_GroupName", "group1"); } }

		public static bool IsLogicDelete { get { return ConfigureHelper.GetConfigureBoolValue("fastdfs_IsLogicDelete", false); } }

		public static string DownloadUrl { get { return ConfigureHelper.GetConfigureValue("fastdfs_DownloadUrl", "http://192.168.12.215:8080/group1/"); } }

		public static string ProxyDownloadUrl { get { return ConfigureHelper.GetConfigureValue("fastdfs_ProxyDownloadUrl", DownloadUrl); } }

		public static string FastDFSDownloadPath
		{
			get
			{
				string strPath = ConfigureHelper.GetConfigureValue("fastdfs_TempFilePath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fastdfs_TempFile"));
				if (!Directory.Exists(strPath))
				{
					Directory.CreateDirectory(strPath);
				}
				return strPath;
			}
		}

		public static int FileServerFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("fastdfs_FailRetryTime", 3); } }

		public static int FileServerUploadFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("fastdfs_UploadFailRetryTime", FileServerFailRetryTime); } }

		public static int FileServerDownloadFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("fastdfs_DownloadFailRetryTime", FileServerFailRetryTime); } }

        public static string FileServerLogPath { get { return ConfigureHelper.GetConfigureValue("fastdfs_LogPath", "FileServerLogs"); } }

        public static bool FileServerLogStatus { get { return ConfigureHelper.GetConfigureBoolValue("fastdfs_LogOn", true); } }

        public static bool FileServerLogArchiveStatus { get { return ConfigureHelper.GetConfigureBoolValue("fastdfs_LogArchiveOn", true); } }

        public static int FileServerLogArchiveSize { get { return ConfigureHelper.GetConfigureIntValue("fastdfs_LogArchiveSize", 104857600); } }
	}
}
