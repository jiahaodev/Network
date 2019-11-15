/****************************************************
	文件：ClientState.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:04   	
	功能：客户端连接状态
*****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientState
    {
        public Socket socket;
        public ByteArray readBuff = new ByteArray();
        //Ping
        public long lastPingTime = 0;
        //玩家
        public Player player;
    }
}
