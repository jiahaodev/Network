/****************************************************
	文件：SysMsg.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:36   	
	功能：心跳机制消息
*****************************************************/
namespace Server
{
    class MsgPing : MsgBase
    {
        public MsgPing()
        {
            protoName = "MsgPing";
        }
    }


    class MsgPong : MsgBase
    {
        public MsgPong()
        {
            protoName = "MsgPong";
        }
    }

}
