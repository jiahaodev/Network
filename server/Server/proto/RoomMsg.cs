/****************************************************
	文件：RoomMsg.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:47   	
	功能：房间消息
*****************************************************/

namespace Server
{
    //查询成绩
    class MsgGetAchieve : MsgBase
    {
        public MsgGetAchieve() { protoName = "MsgGetAchieve"; }
        //服务端回
        public int win = 0;
        public int lost = 0;
    }

    //房间信息
    class RoomInfo
    {
        public int id = 0;    //房间id
        public int count = 0; //人数
        public int status = 0;//状态0-准备中， 1-战斗中
    }

    //请求房间列表
    class MsgGetRoomList : MsgBase
    {
        public MsgGetRoomList() { protoName = "MsgGetRoomList"; }
        //服务端回
        public RoomInfo[] rooms;
    }

    //创建房间
    class MsgCreateRoom : MsgBase
    {
        public MsgCreateRoom() { protoName = "MsgCreateRoom"; }
        //服务端回
        public int result = 0;
    }

    //进入房间
    class MsgEnterRoom : MsgBase
    {
        public MsgEnterRoom() { protoName = "MsgEnterRoom"; }
        //客户端发
        public int id = 0;
        //服务端回
        public int result = 0;
    }

    //玩家信息
    [System.Serializable]
    class PlayerInfo
    {
        public string id = "";  //账号
        public int camp = 0;    //阵营
        public int win = 0;     //胜利数
        public int lost = 0;    //失败数
        public int isOwner = 0; //是否为房主
    }

    //获取房间信息
    class MsgGetRoomInfo : MsgBase
    {
        public MsgGetRoomInfo() { protoName = "MsgGetRoomInfo"; }
        //服务端回
        public PlayerInfo[] players;
    }

    //离开房间
    class MsgLeaveRoom : MsgBase
    {
        public MsgLeaveRoom() { protoName = "MsgLeaveRoom"; }
        //服务端回
        public int result = 0;
    }

    //开战
    class MsgStartBattle : MsgBase
    {
        public MsgStartBattle() { protoName = "MsgStartBattle"; }
        //服务端回
        public int result = 0;
    }

}
