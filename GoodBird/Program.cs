using System;

namespace GoodBird
{
    class Program
    {
        static GoodBirdServer server = new GoodBirdServer();
        /// <summary>
        /// 程序的主函数入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // 启动服务器
            server.Start();
            Loger.Log("服务器已启动...");

            /// <summary>
            /// 进程保持
            /// </summary>
            /// <value></value>
            try
            {
                while (true)
                {
                    Console.ReadLine();
                }
            }
            catch(Exception e){
                server.Stop();
            }
        }
    }
}
