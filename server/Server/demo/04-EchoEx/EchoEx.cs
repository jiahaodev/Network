/****************************************************
	文件：EchoEx.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/11 22:44   	
	功能：第4章  正确收发数据流
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class EchoEx
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        //CheckRead
        List<Socket> checkRead = new List<Socket>();

        public void Execute()
        {
            Console.WriteLine("Hello SelectEcho");
            //Socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("端口监听完成");


            //Accept
            //listenfd.BeginAccept(AcceptCallback, listenfd);


            while (true)
            {
                //填充checkRead列表
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (ClientState s in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                //select
                Socket.Select(checkRead, null, null, 1000);
                foreach (Socket s in checkRead)
                {
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
            }
            //等待
            Console.ReadLine();
        }



        private static void ReadListenfd(Socket listenfd)
        {
            Socket clientfd = listenfd.Accept();
            string clientDesc = clientfd.RemoteEndPoint.ToString();
            Console.WriteLine("[服务器]Accept : " + clientDesc);

            //等待（模拟网络阻塞）
            //Thread.Sleep(30*1000);

            ClientState state = new ClientState(clientfd);
            clients.Add(clientfd, state);
        }



        private static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            string clientDesc = clientfd.RemoteEndPoint.ToString();

            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("[Socket]断开: " + clientDesc);
                Console.WriteLine("Socket Receive fail " + ex.ToString());
            }


            if (count == 0)
            {
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("[Socket]断开: " + clientDesc);
                return false;
            }

            //广播
            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 2, count-2);            //这里对应有“长度信息法”处理粘包问题
            //string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            Console.WriteLine("Receive:" + recvStr);
            byte[] sendBytes = new byte[count];
            Array.Copy(state.readBuff,0,sendBytes,0,count);  //拷贝原来的数据，包含粘包处理
            //byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(recvStr);
            foreach (ClientState s in clients.Values)
            {
                s.socket.Send(sendBytes);
            }


            return true;
        }
    }
}
