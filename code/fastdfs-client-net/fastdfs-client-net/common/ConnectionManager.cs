using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace fastdfs_client_net.common
{
	/// <summary>
	/// 连接管理器
	/// </summary>
	class ConnectionManager
	{
		 static Dictionary<IPEndPoint, CPool> trackerPools = new Dictionary<IPEndPoint, CPool>();
		 static Dictionary<IPEndPoint, CPool> storePools = new Dictionary<IPEndPoint, CPool>();

		//工作tracker集合
		private static List<IPEndPoint> listWorkTrackers = new List<IPEndPoint>();
		//故障tracker集合
		private static List<IPEndPoint> listBusyTrackers = new List<IPEndPoint>();

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="trackers">trackers</param>
		/// <returns></returns>
		static bool Initialize(List<IPEndPoint> trackers)
		{
			foreach (IPEndPoint point in trackers)
			{
				if (!trackerPools.ContainsKey(point))
					trackerPools.Add(point, new CPool(point, Config.Tracker_MaxConnection));
			}
			listWorkTrackers = trackers;
			return true;
		}

		/// <summary>
		/// tracker 获取，采用轮询方式，支持多个tracker切换
		/// </summary>
		/// <returns></returns>
		public static Connection GetTrackerConnection()
		{
			//随机获取，不采用这种方式
			//Random random = new Random();
			//int index = random.Next(trackerPools.Count);
			//Pool pool = trackerPools[listTrackers[index]];
			CPool pool = null;
			if (listBusyTrackers.Count > 0)
			{
				foreach (IPEndPoint point in listBusyTrackers)
				{
					//重新初始化,丢弃之前的连接
					trackerPools[point] = new CPool(point, Config.Tracker_MaxConnection);
					listWorkTrackers.Add(point);
				}
				listBusyTrackers.Clear();
			}
			while (listWorkTrackers.Count > 0)
			{
				//取第一个tracker
				pool = trackerPools[listWorkTrackers[0]];
				try
				{
					var conn = pool.GetConnection();

					return conn;
				}
				catch (Exception ex)
				{
					listBusyTrackers.Add(listWorkTrackers[0]);
					listWorkTrackers.RemoveAt(0);
				}
			}
			throw new Exception("没有可用的tracker节点，请查看tracker节点是否配置正确，或者服务集群状态！");
		}

		/// <summary>
		/// 获取storage的连接
		/// </summary>
		/// <param name="endPoint"></param>
		/// <returns></returns>
		public static Connection GetStorageConnection(IPEndPoint endPoint)
		{
			lock ((storePools as ICollection).SyncRoot)
			{
				if (!storePools.ContainsKey(endPoint))
				{
					CPool pool = new CPool(endPoint, Config.Storage_MaxConnection);
					storePools.Add(endPoint, pool);
				}
			}
			return storePools[endPoint].GetConnection();
		}
	}
}
