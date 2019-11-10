using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class EventHandler
    {
        public static void OnDisconnect(ClientState c) {
            string desc = c.socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            MoveGame.Broadcast(c,sendStr);
        }


    }
}
