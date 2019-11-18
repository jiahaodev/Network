/****************************************************
	文件：MoveGame.cs
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server
{


    class MoveGame
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        //CheckRead
        public List<Socket> checkRead = new List<Socket>();

        public void Execute()
        {
            Console.WriteLine("Hello MoveGame");
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

            ClientState state = new ClientState(clientfd);
            clients.Add(clientfd, state);
        }



        private static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            string clientDesc = clientfd.RemoteEndPoint.ToString();
            //接收消息
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {

                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("[Socket]断开: " + clientDesc);
                Console.WriteLine("Socket Receive fail " + ex.ToString());
                return false;
            }

            //客户端关闭
            if (count == 0)
            {

                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
                object[] ob = { state };
                mei.Invoke(null, ob);

                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("[Socket]断开: " + clientDesc);
                return false;
            }

            //消息处理
            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff, 0, count);
            Console.WriteLine("Recv:" + recvStr);
            string[] split = recvStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            return true;

        }


        //发送
        public static void Send(ClientState cs, string sendStr)
        {
            byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
            cs.socket.Send(sendBytes);
        }

        //广播
        public static void Broadcast(ClientState srcState, string sendStr)
        {
            foreach (ClientState cs in clients.Values)
            {
                if (cs.socket.Connected)
                {
                    Send(cs, sendStr);
                }
            }
        }


    }




}
