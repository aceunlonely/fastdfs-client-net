using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace fastdfs_client_net.common
{
	/// <summary>
	/// 连接简单封装
	/// </summary>
	class Connection : TcpClient
	{
		public CPool Pool { get; set; }
		public DateTime CreateTime{ get; set; }
		public DateTime LastUseTime { get; set; }
		public bool InUse { get; set; }
		public void Open()
		{
			if (InUse)
				throw new Exception("the connection is already in using");
			InUse = true;
			LastUseTime = DateTime.Now;
		}
		public new void Close()
		{
			Pool.CloseConnection(this);
		}

		public void Release()
		{
			Pool.ReleaseConnection(this);
		}
	}
}
