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

        private static byte[] result = new byte[1024];
        static void Main(string[] args)
        {
            //设定服务器IP地址  


            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 8401)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }


            //通过 clientSocket 发送数据  

            try
            {
                //Thread.Sleep(1000);    //等待1秒钟  
                string sendMessage = "{type:106,time:" + DateTime.Now + "}";
                clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                Console.WriteLine("向服务器发送消息：{0}", sendMessage);
            }
            catch
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            //通过clientSocket接收数据  
            int receiveLength = clientSocket.Receive(result);
            Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));

            Console.WriteLine("再发送，按回车键");
            Console.ReadLine();
            try
            {
                //Thread.Sleep(1000);    //等待1秒钟  
                string sendMessage = "{type:106,time:" + DateTime.Now + "}";
                clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                Console.WriteLine("向服务器发送消息：{0}", sendMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("向服务器发送消息：{0}", ex.Message);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            //通过clientSocket接收数据  
            receiveLength = clientSocket.Receive(result);
            Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));

            Console.WriteLine("再发送，按回车键");
            Console.ReadLine();
            try
            {
                //Thread.Sleep(1000);    //等待1秒钟  
                string sendMessage = "{type:106,time:" + DateTime.Now + "}";
                clientSocket.Send(Encoding.ASCII.GetBytes(sendMessage));
                Console.WriteLine("向服务器发送消息：{0}", sendMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("向服务器发送消息：{0}", ex.Message);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }

            //通过clientSocket接收数据  
            receiveLength = clientSocket.Receive(result);
            Console.WriteLine("接收服务器消息：{0}", Encoding.ASCII.GetString(result, 0, receiveLength));


            Console.WriteLine("发送完毕，按回车键退出");
            Console.ReadLine();
        }

    }
}
