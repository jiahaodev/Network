/****************************************************
	文件：LoginMsg.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:40   	
	功能：注册/登陆/下线消息
*****************************************************/

//注册
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }
    //客户端发送
    public string id = "";
    public string pw = "";
    //服务端返回（0-成功，1-失败）
    public int result = 0;
}

//登陆
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }
    //客户端发送
    public string id = "";
    public string pw = "";
    //服务端返回（0-成功，1-失败）
    public int result = 0;
}

//踢下线（服务端推送）
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }
    //原因（0-其他人登陆统一账号）
    public int reason = 0;
}

