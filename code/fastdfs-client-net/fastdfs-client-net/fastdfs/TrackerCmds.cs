using fastdfs_client_net.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// tracker 常用命令
	///		fastdfs的命令（接口）还不少，但实际客户端使用的并不多，不一一实现了，实现方式大差不差
	///		所有命令请参考源码：https://github.com/happyfish100/fastdfs/blob/master/tracker/tracker_proto.h
	/// </summary>
	public class TrackerCmds
	{
		/// <summary>
		/// query which storage server to store file
		/// 
		/// Reqeust 
		///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE 104
		///     Body: 
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		/// Response
		///     Cmd: TRACKER_PROTO_CMD_RESP
		///     Status: 0 right other wrong
		///     Body: 
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
		///     @ 1 byte: store path index on the storage server
		/// </summary>
		public StorageNodeInfo QUERY_STORE_WITH_GROUP_ONE(string groupName) {
			if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
			{
				throw new Exception("GroupName is too long");
			}
			//连接
			Connection connection = ConnectionManager.GetTrackerConnection();

			//请求头和体
			int[] lengthes = new int[] { Consts.FDFS_GROUP_NAME_MAX_LEN };
			IList<byte[]> contents = new byte[][] { Helper.StringToByte(groupName) };
			Request req = new Request();
			req.SetBody(lengthes, contents);
			req.Header = new Header(lengthes.Sum(), Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ONE, 0);
			byte[] res = req.Invoke(connection);
			//解析结果
			StorageNodeInfo node = new StorageNodeInfo();

			//byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
			//Array.Copy(res, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
			node.GroupName = groupName; //Helper.ByteToString(groupNameBuffer).TrimEnd('\0');
			byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
			Array.Copy(res, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
			string strIp = new string(Config.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');
			byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
			Array.Copy(res, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
				portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
			int intPort = (int)Helper.BufferToLong(portBuffer, 0);
			node.StorePathIndex = res[res.Length - 1];
			node.EndPoint = new IPEndPoint(IPAddress.Parse(strIp), intPort);

			return node;
		}

		/// <summary>
		/// query which storage server to update the file
		/// 
		/// Reqeust 
		///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE 103
		///     Body:
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes:  group name
		///     @ filename bytes: filename
		/// Response
		///     Cmd: TRACKER_PROTO_CMD_RESP
		///     Status: 0 right other wrong
		///     Body: 
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
		/// </summary>
		public StorageNodeInfo QUERY_UPDATE(string groupName,string serverFileName) {
			if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
				throw new Exception("GroupName is too long");

			//连接
			Connection connection = ConnectionManager.GetTrackerConnection();

			//请求头和体
			var fileNameBuffer = Helper.StringToByte(serverFileName);

			int[] lengthes = new int[] { Consts.FDFS_GROUP_NAME_MAX_LEN , fileNameBuffer.Length };
			IList<byte[]> contents = new byte[][] { Helper.StringToByte(groupName), fileNameBuffer };
			Request req = new Request();
			req.SetBody(lengthes, contents);
			req.Header = new Header(lengthes.Sum(), Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_UPDATE, 0);
			byte[] res = req.Invoke(connection);

			//解析结果
			StorageNodeInfo node = new StorageNodeInfo();

			//byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
			//Array.Copy(res, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
			node.GroupName = groupName; //Helper.ByteToString(groupNameBuffer).TrimEnd('\0');
			byte[] ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];
			Array.Copy(res, Consts.FDFS_GROUP_NAME_MAX_LEN, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);
			string strIp = new string(Config.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');
			byte[] portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];
			Array.Copy(res, Consts.FDFS_GROUP_NAME_MAX_LEN + Consts.IP_ADDRESS_SIZE - 1,
				portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);
			int intPort = (int)Helper.BufferToLong(portBuffer, 0);
			node.StorePathIndex = res[res.Length - 1];
			node.EndPoint = new IPEndPoint(IPAddress.Parse(strIp), intPort);

			return node;
		}
	}
}
