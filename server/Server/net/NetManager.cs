using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class NetManager
    {
        //监听Socket
        public static Socket listenfd;
        //客户端Socket及状态信息
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        //Select的检查列表
        static List<Socket> checkRead = new List<Socket>();
        //ping间隔
        public static long pingInterval = 30; //30秒

        //启动端口监听（循环Loop）
        public static void StartLoop(int listenPort) {
            //初始化监听socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse("0.0.0.0"); //? todo why is zero
            IPEndPoint ipEp = new IPEndPoint(ipAdr,listenPort); //终端节点
            listenfd.Bind(ipEp);
            //Listen
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            //循环
            while (true)
            {
                ResetCheckRead();
                Socket.Select(checkRead, null, null, 1000);
                //检查可读对象
                for (int i = checkRead.Count - 1; i >= 0; i++)
                {
                    Socket s = checkRead[i];
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else {
                        ReadClientfd(s);
                    }
                }

            }
        }

        //填充checkRead列表
        private static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (ClientState s in clients.Values)
            {
                checkRead.Add(s.socket);
            }
        }

        //读取Listenfd
        private static void ReadListenfd(Socket listenfd)
        {
            try
            {
                Socket clientfd = listenfd.Accept();
                Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
                ClientState state = new ClientState();
                state.socket = clientfd;
                state.lastPingTime = GetTimeStamp();
                clients.Add(clientfd,state);
            }
            catch (Exception e)
            {
                Console.WriteLine("Accept fail" + e.ToString());
            }
        }

        //读取Clientfd 【将收到的消息写入ClientState对应的读缓冲区】
        private static void ReadClientfd(Socket cliendfd)
        {
            ClientState state = clients[cliendfd];
            ByteArray readBuff = state.readBuff;
            //接收
            int count = 0;
            //缓冲区不够，清除，若依旧不够，只能返回
            //但当单条协议超过缓冲区长度时会发生
            if (readBuff.remain <= 0 )
            {
                OnReceiveData(state);
                readBuff.MoveBytes();
            }
            if (readBuff.remain <= 0)
            {
                Console.WriteLine("Receive fail , maybe msg length > buff capacity");
                Close(state);
            }
           
            try
            {
                count = cliendfd.Receive(readBuff.bytes,readBuff.writeIdx,readBuff.remain,0);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Receive SocketException " + ex.ToString());
                Close(state);
                return;
            }

            //客户端关闭
            if (count <= 0)
            {
                Console.WriteLine("Socket Close " + cliendfd.RemoteEndPoint.ToString());
                Close(state);
                return;
            }

            //消息处理
            readBuff.writeIdx += count;
            //处理二进制消息
            OnReceiveData(state);
            //移动缓冲区
            readBuff.CheckAnMoveBytes();
        }

        //数据处理
        private static void OnReceiveData(ClientState state)
        {
            ByteArray readBuff = state.readBuff;
            //消息长度
            if (readBuff.length <= 2)
            {
                return;
            }
            //消息体长度
            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;

            //解析协议名

            //解析协议体

            //分发消息【对应监听模块进行对应的处理】

            //继续读取消息


        }

        private static void Close(ClientState state)
        {
            throw new NotImplementedException();
        }




        //关闭连接



        //接收（数据处理）

        //发送


        //定时器（启动定时任务）


        //获取时间戳
        public static long GetTimeStamp() {

            return null;
        }



    }
}
