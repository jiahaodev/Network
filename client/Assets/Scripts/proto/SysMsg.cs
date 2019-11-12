/****************************************************
    文件：SysMsg.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/13 0:37:9
	功能：系统通信消息
*****************************************************/

public class MsgPing : MsgBase
{
    public MsgPing() { protoName = "MsgPing"; }
}


public class MsgPong : MsgBase
{
    public MsgPong() { protoName = "MsgPong"; }
}