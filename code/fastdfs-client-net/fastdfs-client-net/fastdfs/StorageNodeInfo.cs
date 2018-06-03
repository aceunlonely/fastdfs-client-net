using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// fastdfs storage节点信息
	/// </summary>
	public class StorageNodeInfo
	{
		public string GroupName { get; set; }
		public IPEndPoint EndPoint { get; set; }
		public byte StorePathIndex { get; set; }
	}
}
