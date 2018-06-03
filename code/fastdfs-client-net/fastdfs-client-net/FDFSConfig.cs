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
		public static string TrackerNodes { get { return ConfigureHelper.GetConfigureValue("FileServer_Nodes", ""); } }

		public static string DefaultGroupName { get { return ConfigureHelper.GetConfigureValue("FileServer_GroupName", "group1"); } }

		public static bool IsLogicDelete { get { return ConfigureHelper.GetConfigureBoolValue("FileServer_IsLogicDelete", false); } }

		public static string DownloadUrl { get { return ConfigureHelper.GetConfigureValue("FileServer_DownloadUrl", "http://192.168.12.215:8080/group1/"); } }

		public static string ProxyDownloadUrl { get { return ConfigureHelper.GetConfigureValue("FileServer_ProxyDownloadUrl", DownloadUrl); } }

		public static string FastDFSDownloadPath
		{
			get
			{
				string strPath = ConfigureHelper.GetConfigureValue("FileServer_TempFilePath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileServer_TempFile"));
				if (!Directory.Exists(strPath))
				{
					Directory.CreateDirectory(strPath);
				}
				return strPath;
			}
		}

		public static int FileServerFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("FileServer_FailRetryTime", 10); } }

		public static int FileServerUploadFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("FileServer_UploadFailRetryTime", FileServerFailRetryTime); } }

		public static int FileServerDownloadFailRetryTime { get { return ConfigureHelper.GetConfigureIntValue("FileServer_DownloadFailRetryTime", FileServerFailRetryTime); } }
	}
}
