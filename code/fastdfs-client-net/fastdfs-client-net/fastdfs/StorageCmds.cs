using fastdfs_client_net.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// storage 常用命令
	///		fastdfs的命令（接口）还不少，但实际客户端使用的并不多，不一一实现了，实现方式大差不差
	///		所有命令请参考源码：https://github.com/happyfish100/fastdfs/blob/master/tracker/tracker_proto.h
	/// </summary>
	public class StorageCmds
	{
		//todo
		//public void APPEND_FILE() { }

		/// <summary>
		/// delete file from storage server
		/// 
		/// Reqeust 
		///     Cmd: STORAGE_PROTO_CMD_DELETE_FILE 12
		///     Body:
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		///     @ filename bytes: filename
		/// Response
		///     Cmd: STORAGE_PROTO_CMD_RESP
		///     Status: 0 right other wrong
		///     Body: 
		///         
		/// </summary>
		public void DELETE_FILE(IPEndPoint iPEnd,string groupName,string fileName )
		{
			if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
				throw new Exception("groupName is too long: " + groupName.Length + "> FDFS_GROUP_NAME_MAX_LEN(" + Consts.FDFS_GROUP_NAME_MAX_LEN + ")");

			//连接
			Connection connection = ConnectionManager.GetStorageConnection(iPEnd);
			//请求头和体
			int[] lengthes = new int[] { Consts.FDFS_GROUP_NAME_MAX_LEN, fileName.Length };
			IList<byte[]> contents = new byte[][] { Helper.StringToByte(groupName), Helper.StringToByte(fileName) };
			Request req = new Request();
			req.SetBody(lengthes, contents);
			req.Header = new Header(lengthes.Sum(), Consts.STORAGE_PROTO_CMD_DELETE_FILE, 0);
			var res = req.Invoke(connection);
			
		}

		//todo
		//public void GET_METADATA() { }

		/// <summary>
		/// upload file to storage server
		/// 
		/// Reqeust 
		///     Cmd: UPLOAD_APPEND_FILE 23
		///     Body:
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
		///     @ filename
		///     @ file bytes: file content 
		/// Response
		///     Cmd: STORAGE_PROTO_CMD_RESP
		///     Status: 0 right other wrong
		///     Body: 
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		///     @ filename bytes: filename   
		/// </summary>
		//public void UPLOAD_APPEND_FILE() { }

		/// <summary>
		/// upload file to storage server
		/// 
		/// Reqeust 
		///     Cmd: STORAGE_PROTO_CMD_UPLOAD_FILE 11
		///     Body:
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: filename size
		///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: file bytes size
		///     @ filename
		///     @ file bytes: file content 
		/// Response
		///     Cmd: STORAGE_PROTO_CMD_RESP
		///     Status: 0 right other wrong
		///     Body: 
		///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
		///     @ filename bytes: filename   
		/// </summary>
		public string UPLOAD_FILE(IPEndPoint iPEnd,byte storePathIndex,int fileSize,string ext,byte[] content) {
			if (ext.Length > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
				throw new Exception("file ext is too long: max length is "+ Consts.FDFS_FILE_EXT_NAME_MAX_LEN);

			//连接
			Connection connection = ConnectionManager.GetStorageConnection(iPEnd);

            //扩展名
            //byte[] extBuffer = new byte[Consts.FDFS_FILE_EXT_NAME_MAX_LEN];
            //byte[] bse = Helper.StringToByte(ext);
            //int ext_name_len = bse.Length;
            //if (ext_name_len > Consts.FDFS_FILE_EXT_NAME_MAX_LEN)
            //{
            //    ext_name_len = Consts.FDFS_FILE_EXT_NAME_MAX_LEN;
            //}
            //Array.Copy(bse, 0, extBuffer, 0, ext_name_len);

			//请求头和体
			int[] lengthes = new int[] { 1, Consts.FDFS_PROTO_PKG_LEN_SIZE, Consts.FDFS_FILE_EXT_NAME_MAX_LEN,content.Length };
            IList<byte[]> contents = new byte[][] { new byte[] { storePathIndex }, Helper.LongToBuffer(fileSize), Helper.StringToByte(ext), content };
			Request req = new Request();
			req.SetBody(lengthes, contents);
			req.Header = new Header(lengthes.Sum(), Consts.STORAGE_PROTO_CMD_UPLOAD_FILE, 0);
			byte[] res = req.Invoke(connection);
			//byte[] groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
			//Array.Copy(res, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);
			//string groupName = Helper.ByteToString(groupNameBuffer).TrimEnd('\0');
			byte[] fileNameBuffer = new byte[res.Length - Consts.FDFS_GROUP_NAME_MAX_LEN];
			Array.Copy(res, Consts.FDFS_GROUP_NAME_MAX_LEN, fileNameBuffer, 0, fileNameBuffer.Length);
			string fileName = Helper.ByteToString(fileNameBuffer).TrimEnd('\0');
			return fileName;
		}


	}
}
