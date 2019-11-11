/****************************************************
	文件：PollEcho.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/10 15:07   	
	功能：Poll Echo
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


    class PollEcho
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        public void Execute()
        {
            Console.WriteLine("Hello PollEcho");
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
                //检查listenfd
                if (listenfd.Poll(0, SelectMode.SelectRead))
                {
                    ReadListenfd(listenfd);
                }
                //检查clientfd
                foreach (ClientState s in clients.Values)
                {
                    Socket clientfd = s.socket;
                    if (clientfd.Poll(0, SelectMode.SelectRead))
                    {
                        if (!ReadClientfd(clientfd))
                        {
                            break;

                        }
                    }
                }

                //防止cpu占用过高
                System.Threading.Thread.Sleep(1);

            }
            //等待
            Console.ReadLine();
        }



        private static void ReadListenfd(Socket listenfd)
        {
            Socket clientfd = listenfd.Accept();
            string clientDesc = clientfd.RemoteEndPoint.ToString();
            Console.WriteLine("[服务器]Accept : " + clientDesc);

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

            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
            string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            foreach (ClientState s in clients.Values)
            {
                s.socket.Send(sendBytes);
            }
            //clientfd.Send(sendBytes);
            return true;
        }





        /********************************对照代码************************************/



        //Accept回调 
        private void AcceptCallback(IAsyncResult ar)
        {

            Socket listenfd = (Socket)ar.AsyncState;
            Socket clientfd = listenfd.EndAccept(ar); //去除对应的accept socket
            string clientDesc = clientfd.RemoteEndPoint.ToString();
            Console.WriteLine("[服务器]Accept : " + clientDesc);
            //clients列表
            ClientState state = new ClientState(clientfd);
            clients.Add(clientfd, state);
            //接收数据
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);

            //继续Accept
            listenfd.BeginAccept(AcceptCallback, listenfd);
        }
        //Receive回调
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState)ar.AsyncState;
                Socket clientfd = state.socket;
                string clientDesc = clientfd.RemoteEndPoint.ToString();
                int count = clientfd.EndReceive(ar);
                //客户端关闭（表示客户端socket断开）
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("[Socket]断开: " + clientDesc);
                    return;
                }

                string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
                string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
                foreach (ClientState s in clients.Values)
                {
                    s.socket.Send(sendBytes);
                }
                //clientfd.Send(sendBytes);

                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Receive fail " + ex.ToString());
            }

        }
    }
}
