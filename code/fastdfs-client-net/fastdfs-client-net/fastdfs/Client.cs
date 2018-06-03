using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// fastdfs 客户端
	/// </summary>
	public class Client
	{
		/// <summary>
		/// 根据groupName获取可用的storage节点
		/// </summary>
		/// <param name="groupName"></param>
		/// <returns></returns>
		public static StorageNodeInfo GetAvailableStorageNode(string groupName)
		{
			return new TrackerCmds().QUERY_STORE_WITH_GROUP_ONE(groupName);
		}

		/// <summary>
		/// 上传文件
		/// </summary>
		/// <param name="groupName"></param>
		/// <param name="content"></param>
		/// <param name="fileExt">后缀</param>
		/// <returns>文件名</returns>
		public static string Upload(string groupName, byte[] content, string fileExt)
		{
			StorageNodeInfo storage = GetAvailableStorageNode(groupName);
			if (fileExt.StartsWith("."))
				fileExt = fileExt.TrimStart(new char[] { '.' });
			return new StorageCmds().UPLOAD_FILE(storage.EndPoint, storage.StorePathIndex, content.Length, fileExt, content);
		}

		/// <summary>
		/// 删除文件
		/// </summary>
		/// <param name="groupName">组名</param>
		/// <param name="fileName">文件名</param>
		public static void Remove(string groupName, string fileName)
		{
			StorageNodeInfo storage = new TrackerCmds().QUERY_UPDATE(groupName, fileName);
			new StorageCmds().DELETE_FILE(storage.EndPoint, groupName, fileName);
		}

		//download 不实现，fastdfs建议nginx方式获取文件
	}
}
