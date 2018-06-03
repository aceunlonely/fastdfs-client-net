using fastdfs_client_net.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// fastRequest
	/// </summary>
	class Request
	{
		public Header Header { get; set; }

		public byte[] Body { get; set; }

		public void SetBody(IList<int> lengthes, IList<byte[]> contents) {

			var length = lengthes.Sum();
			byte[] bodyBuffer = new byte[length];
			int offset = 0;
			for (int i = 0; i < lengthes.Count(); i++) {
				Array.Copy(contents[i], 0, bodyBuffer, offset, Math.Min(contents[i].Length, lengthes[i]));
				offset += lengthes[0];
			}
			Body = bodyBuffer;
		}

		/// <summary>
		/// 调用请求
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public byte[] Invoke(Connection connection) {
			try
			{
				connection.Open();
				NetworkStream stream = connection.GetStream();
				byte[] headerBuffer = Header.ToByte();
				stream.Write(headerBuffer, 0, headerBuffer.Length);
				stream.Write(Body, 0, Body.Length);
				//结果 //此处报错说明socket 有问题，获取服务端存在问题
				Header header = new Header(stream);
				if (header.Status != 0)
					throw new Exception(string.Format("Get Response Error,Error Code:{0} msg:{1}", header.Status, Errors.GetErrorInfo(header.Status)));
				byte[] body = new byte[header.Length];
				if (header.Length != 0)
					stream.Read(body, 0, (int)header.Length);

				connection.Close();
				return body;
			}
			catch (Exception ex)
			{
				connection.Release();
				throw ex;
			}

		}
	}
}
