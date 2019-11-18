/****************************************************
	文件：ChatRoom.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/10 15:07   	
	功能：聊天室
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


    class ChatRoom
    {
        //监听Socket
        static Socket listenfd;
        //客户端Socket及状态信息
        static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        public void Execute()
        {
            Console.WriteLine("Hello ChatRoom");
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
            listenfd.BeginAccept(AcceptCallback, listenfd);

            //等待
            Console.ReadLine();


        }

        //Accept回调 
        private void AcceptCallback(IAsyncResult ar)
        {

            Socket listenfd = (Socket)ar.AsyncState;
            Socket clientfd = listenfd.EndAccept(ar); //去除对应的accept socket
            string clientDesc = clientfd.RemoteEndPoint.ToString() ;
            Console.WriteLine("[服务器]Accept : " + clientDesc);
            //clients列表
            ClientState state = new ClientState(clientfd);
            clients.Add(clientfd,state);
            //接收数据
            clientfd.BeginReceive(state.readBuff,0,1024,0,ReceiveCallback,state);

            //继续Accept
            listenfd.BeginAccept(AcceptCallback,listenfd);
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
	
	            string recvStr = System.Text.Encoding.UTF8.GetString(state.readBuff,0,count);
                string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + recvStr;
	            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
                foreach (ClientState s in clients.Values)
                {
                    s.socket.Send(sendBytes);
                }
	            //clientfd.Send(sendBytes);
	
	            clientfd.BeginReceive(state.readBuff,0,1024,0,ReceiveCallback,state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket Receive fail " + ex.ToString());
            }
         
        }
    }
}
