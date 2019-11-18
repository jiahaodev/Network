/****************************************************
	文件：Echo.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/10 15:49   	
	功能：同步Echo（阻塞）
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Echo
    {
        public void Execute() {
            Console.WriteLine("Hello Echo");
            //Socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr,8888);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            while (true)
            {
                //Accept
                Socket connfd = listenfd.Accept();
                Console.WriteLine("[服务器]Accept");

                //Receive
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string readStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器接收]" + readStr);
                //Send
                string sendStr = System.DateTime.Now.ToString() + "    " + readStr ;
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
                connfd.Send(sendBytes);
                Console.WriteLine(sendStr);
            }


        }
    }
}
