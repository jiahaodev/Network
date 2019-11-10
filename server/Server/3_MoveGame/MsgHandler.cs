using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class MsgHandler
    {
        //处理进入信息
        public static void MsgEnter(ClientState c, string msgArgs) {
            Console.WriteLine("MsgEnter" + msgArgs);
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            //赋值
            c.hp = 100;
            c.x = x;
            c.y = y;
            c.z = z;
            c.eulY = eulY;

            //广播
            string sendStr = "Enter|" + msgArgs;
            MoveGame.Broadcast(c,sendStr);

        }

        //处理获取玩家列表的请求
        public static void MsgList(ClientState c, string msgArgs)
        {
            Console.WriteLine("MsgList" + msgArgs);
            string sendStr = "List|";
            foreach (ClientState cs in MoveGame.clients.Values) {
                sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
                sendStr += cs.x.ToString() + ",";
                sendStr += cs.y.ToString() + ",";
                sendStr += cs.z.ToString() + ",";
                sendStr += cs.eulY.ToString() + ",";
                sendStr += cs.hp.ToString() + ",";
            }
            Console.WriteLine("List:" + sendStr);
            MoveGame.Send(c,sendStr);
        }

        //处理移动信息
        public static void MsgMove(ClientState c, string msgArgs) {
            //解析参数
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            //赋值
            c.x = x;
            c.y = y;
            c.z = z;
            //广播
            string sendStr = "Move|" + msgArgs;
            MoveGame.Broadcast(c, sendStr);

        }


        //处理Attack消息
        public static void MsgAttack(ClientState c, string msgArgs)
        {
            //广播
            string sendStr = "Attack|" + msgArgs;
            MoveGame.Broadcast(c, sendStr);

        }

        //处理Hit消息
        public static void MsgHit(ClientState c, string msgArgs)
        {
            //解析参数
            string[] split = msgArgs.Split(',');
            string attDesc = split[0];
            string hitDesc = split[1];
            //找出被攻击的角色
            ClientState hitCS = null;
            foreach (ClientState cs in MoveGame.clients.Values)
            {
                if (cs.socket.RemoteEndPoint.ToString() == hitDesc)
                    hitCS = cs;
            }
            if (hitCS == null)
                return;
            //扣血
            hitCS.hp -= 25;
            //死亡
            if (hitCS.hp <= 0)
            {
                string sendStr = "Die|" + hitCS.socket.RemoteEndPoint.ToString();
                MoveGame.Broadcast(hitCS, sendStr);
            }

        }
    }
}
