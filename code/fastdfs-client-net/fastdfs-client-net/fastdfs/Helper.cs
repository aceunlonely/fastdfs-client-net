using fastdfs_client_net.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fastdfs_client_net.fastdfs
{
	/// <summary>
	/// fastdfs 通用帮助类
	/// </summary>
	public class Helper
	{
		/// <summary>
		/// Convert Long to byte[]
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static byte[] LongToBuffer(long l)
		{
			byte[] buffer = new byte[8];
			buffer[0] = (byte)((l >> 56) & 0xFF);
			buffer[1] = (byte)((l >> 48) & 0xFF);
			buffer[2] = (byte)((l >> 40) & 0xFF);
			buffer[3] = (byte)((l >> 32) & 0xFF);
			buffer[4] = (byte)((l >> 24) & 0xFF);
			buffer[5] = (byte)((l >> 16) & 0xFF);
			buffer[6] = (byte)((l >> 8) & 0xFF);
			buffer[7] = (byte)(l & 0xFF);

			return buffer;
		}
		/// <summary>
		/// Convert byte[] to Long
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <returns></returns>
		public static long BufferToLong(byte[] buffer, int offset)
		{
			return (((long)(buffer[offset] >= 0 ? buffer[offset] : 256 + buffer[offset])) << 56) |
				   (((long)(buffer[offset + 1] >= 0 ? buffer[offset + 1] : 256 + buffer[offset + 1])) << 48) |
				   (((long)(buffer[offset + 2] >= 0 ? buffer[offset + 2] : 256 + buffer[offset + 2])) << 40) |
				   (((long)(buffer[offset + 3] >= 0 ? buffer[offset + 3] : 256 + buffer[offset + 3])) << 32) |
				   (((long)(buffer[offset + 4] >= 0 ? buffer[offset + 4] : 256 + buffer[offset + 4])) << 24) |
				   (((long)(buffer[offset + 5] >= 0 ? buffer[offset + 5] : 256 + buffer[offset + 5])) << 16) |
				   (((long)(buffer[offset + 6] >= 0 ? buffer[offset + 6] : 256 + buffer[offset + 6])) << 8) |
				   ((buffer[offset + 7] >= 0 ? buffer[offset + 7] : 256 + buffer[offset + 7]));
		}

		public static string ByteToString(byte[] input)
		{
			char[] chars = Config.Charset.GetChars(input);
			string result = new string(chars, 0, input.Length);
			return result;
		}

		public static byte[] StringToByte(string input)
		{
			return Config.Charset.GetBytes(input);
		}
	}
}
