using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// 错误列表
	///		fastdfs的错误码采用的是posix定义的错误码
	///		详细错误码，请参考源码：others/errorno.txt
	/// </summary>
	class Errors
	{
		/// <summary>
		/// 获取错误信息
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		 public static string GetErrorInfo(int code){
			string msg;
			switch (code)
			{
				case 28:
					msg = "服务器空间不足,请联系管理员";
					break;
				default:
					msg = "未知异常，请开发人员扩展";
					break;
			}
			return msg;
		}
	}
}
