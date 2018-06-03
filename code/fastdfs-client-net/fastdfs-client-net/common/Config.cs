using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastdfs_client_net.common
{
	/// <summary>
	/// 内部配置
	/// </summary>
	class Config
	{
		internal static int Storage_MaxConnection = 20;
		internal static int Tracker_MaxConnection = 10;
		internal static int ConnectionTimeout = 5;    //Second
		internal static int Connection_LifeTime = 1800; //second
		internal static Encoding Charset = Encoding.UTF8;
		internal static int TcpTimeout = 500; //毫秒
	}
}
