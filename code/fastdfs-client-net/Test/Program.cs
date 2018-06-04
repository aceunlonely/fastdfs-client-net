using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 10000; i < 90000; i++)
            {
                try
                {
                    Console.WriteLine("【" + i + "】1");
                    string file = fastdfs_client_net.Facade.Upload("D:/temp.xls");
                    Console.WriteLine("【" + i + "】2:" + file);
                    string temp = fastdfs_client_net.Facade.DownLoad(file, "E:/temp/t" + i + ".xls", true);
                    Console.WriteLine("【" + i + "】3:" + temp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                Thread.Sleep(200);
            }
        }
    }
}
