using fastdfs_client_net.fastdfs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace fastdfs_client_net.common
{
	/// <summary>
	/// 连接池
	/// </summary>
	class CPool
	{
		private List<Connection> inUse;
		private Stack<Connection> idle;
		private AutoResetEvent autoEvent = null;
		private IPEndPoint endPoint = null;
		private int maxConnection = 0;
		public CPool(IPEndPoint endPoint, int maxConnection)
		{
			autoEvent = new AutoResetEvent(false);
			inUse = new List<Connection>(maxConnection);
			idle = new Stack<Connection>(maxConnection);
			this.maxConnection = maxConnection;
			this.endPoint = endPoint;
		}
		private Connection GetPooldConncetion()
		{
			Connection result = null;
			lock ((idle as ICollection).SyncRoot)
			{
				if (idle.Count > 0)
					result = idle.Pop();
				//在result.Connected == false 时，重新建立连接，但 result.Connected 不能保证第一次出错时有用，只能防止第二次重连（这里建议最外部使用重试）
				if (result != null && ((int)(DateTime.Now - result.LastUseTime).TotalSeconds > Config.Connection_LifeTime || result.Connected == false))
				{
					foreach (Connection conn in idle)
					{
						conn.Close();
					}
					idle = new Stack<Connection>(maxConnection);
					result = null;
				}
			}
			lock ((inUse as ICollection).SyncRoot)
			{
				if (inUse.Count == maxConnection)
					return null;
				if (result == null)
				{
					result = new ConnectionWithTimeout(endPoint, Config.TcpTimeout).Connect();
					result.Pool = this;
				}
				inUse.Add(result);
			}
			return result;
		}

		internal Connection GetConnection()
		{
			int timeOut = Config.ConnectionTimeout * 1000;
			Connection result = null;
			Stopwatch watch = Stopwatch.StartNew();
			while (timeOut > 0)
			{
				result = GetPooldConncetion();
				if (result != null)
				{
					return result;
				}
				if (!autoEvent.WaitOne(timeOut, false))
					break;
				watch.Stop();
				timeOut = timeOut - (int)watch.ElapsedMilliseconds;
			}
			throw new Exception("GetConnection Time Out");
		}

		public void ReleaseConnection(Connection conn)
		{
			if (!conn.InUse)
			{
				try
				{
					Header header = new Header(0, Consts.FDFS_PROTO_CMD_QUIT, 0);
					byte[] buffer = header.ToByte();
					conn.GetStream().Write(buffer, 0, buffer.Length);
					conn.GetStream().Close();
				}
				catch
				{
				}
			}
			conn.Close();
			lock ((inUse as ICollection).SyncRoot)
			{
				inUse.Remove(conn);
			}
			autoEvent.Set();
		}
		public void CloseConnection(Connection conn)
		{
			conn.InUse = false;
			lock ((inUse as ICollection).SyncRoot)
			{
				inUse.Remove(conn);
			}
			lock ((idle as ICollection).SyncRoot)
			{
				idle.Push(conn);
			}
			autoEvent.Set();
		}
	}
}
