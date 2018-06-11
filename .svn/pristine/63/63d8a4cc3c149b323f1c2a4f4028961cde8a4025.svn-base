using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        private static byte[] result = new byte[4096];
        static void Main(string[] args)
        {
            //设定服务器IP地址  
#if DEBUG
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            //IPAddress ip = IPAddress.Parse("182.92.78.196");
#else
            IPAddress ip = IPAddress.Parse("120.76.25.48");
#endif

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8765)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }

            Console.WriteLine("请输入，按回车键结束！");
            string sendMessage = string.Empty;
            char key = (char)(Console.Read());
            while (key != 'q')
            {
                if (key == '\r' || key == '\n')
                {
                    Console.WriteLine("请输入，按回车键结束！");
                    key = (char)(Console.Read());
                    continue;
                }

                string did = string.Empty;
                string id = "0419b8f1e0efd06643d0a92751ea166e742b";
#if DEBUG
                //did = "17080373WV";
                did = "17080375WV";
#else
                did = "16020010TM";
#endif

                switch (key)
                {
                    case '0':
                        sendMessage = "{tp:100,did:\"" + did + "\"}";
                        break;
                    case '1':
                        sendMessage = "{tp:101,did:\"" + did + "\"}";
                        break;
                    case '2':
                        sendMessage = "{tp:102,did:\"" + did + "\",id:\"" + id + "\"}";
                        break;
                    case '3':
                        sendMessage = "{tp:103,did:\"" + did + "\",id:\"" + id + "\",code:3}";
                        break;
                    case '4':
                        sendMessage = "{tp:104,did:\"" + did + "\",code:2}";
                        break;
                    case '5':
                        sendMessage = "{tp:105,did:\"" + did + "\"}";
                        break;
                }
                try
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                    Console.WriteLine("向服务器发送消息：{0}", sendMessage);

                    //通过clientSocket接收数据  
                    int receiveLength = clientSocket.Receive(result);
                    Console.WriteLine("接收服务器消息：{0}", Encoding.GetEncoding("gb2312").GetString(result, 0, receiveLength));
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }

                Console.WriteLine("请输入，按回车键结束！");
                key = (char)(Console.Read());

                int rLength = clientSocket.Receive(result);
                if (rLength > 0)
                {
                    Console.WriteLine("接收服务器消息：{0}", Encoding.GetEncoding("gb2312").GetString(result, 0, rLength));
                }
            }
            Console.WriteLine("按回车键退出");
            Console.ReadLine();
        }

    }
}