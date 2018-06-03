using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace fastdfs_client_net.common
{
	/// <summary>
	/// 可超时的连接管理，默认的tcpclient在网络存在问题时，请求时间非常长，故采用能超时的连接
	/// </summary>
	class ConnectionWithTimeout
	{
		protected int _timeout_milliseconds;
		protected Connection connection;
		protected bool connected;
		protected Exception exception;
		protected IPEndPoint _localEP;
		public ConnectionWithTimeout(IPEndPoint localEP, int timeout_milliseconds)
		{
			this._localEP = localEP;
			_timeout_milliseconds = timeout_milliseconds;
		}

		public Connection Connect()
		{
			connected = false;
			exception = null;
			Thread thread = new Thread(new ThreadStart(BeginConnect));
			thread.IsBackground = true; // 作为后台线程处理
			thread.Start();
			// 等待如下的时间
			thread.Join(_timeout_milliseconds);
			if (connected == true)
			{
				// 如果成功就返回TcpClient对象
				thread.Abort();
				return connection;
			}
			if (exception != null)
			{
				// 如果失败就抛出错误
				thread.Abort();
				throw exception;
			}
			else
			{
				// 同样地抛出错误
				thread.Abort();
				throw new TimeoutException("TcpClient connection timed out");
			}
		}
		protected void BeginConnect()
		{
			try
			{
				connection = new Connection();
				connection.Connect(_localEP);
				// 标记成功，返回调用者
				connected = true;
			}
			catch (Exception ex)
			{
				// 标记失败
				exception = ex;
			}
		}
	}
}
