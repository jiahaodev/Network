using System;

public partial class EventHandler
{
    //处理玩家下线（或socket断开）
    public static void OnDisconnect(ClientState c) {
        Console.WriteLine("Close");
        //Player下线
        if (c.player != null)
        {
            //离开战场
            int roomId = c.player.roomId;
            if (roomId >= 0)
            {
                Room room = RoomManager.GetRoom(roomId);
                room.RemovePlayer(c.player.id);
            }
            //保存数据
            DbManager.UpdatePlayerData(c.player.id,c.player.data);
            //移除
            PlayerManager.RemovePlayer(c.player.id);
        }
    }


    //定时驱动（心跳包检测、房间内逻辑驱动更新）
    public static void OnTimer() {
        CheckPing();
        RoomManager.Update();
    }


    //Ping检查
    public static void CheckPing()
    {
        //获取现在的时间戳
        long timeNow = NetManager.GetTimeStamp();
        long checkInterval = NetManager.pingInterval * 4;
        //遍历，删除
        foreach (ClientState s in NetManager.clients.Values)
        {
            if (timeNow - s.lastPingTime > checkInterval)
            {
                Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
                NetManager.Close(s);
                return;
            }
        }
    }
}
