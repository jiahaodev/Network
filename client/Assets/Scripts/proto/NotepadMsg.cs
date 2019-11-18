/****************************************************
	文件：NotepadMsg.cs
	作者：JiahaoWu
	邮箱: jiahaodev@163.ccom
	日期：2019/11/15 23:46   	
	功能：记事本消息类（测试数据库记录功能）
*****************************************************/

//获取记事本内容
public class MsgGetText : MsgBase
{
    public MsgGetText() { protoName = "MsgGetText"; }
    //服务端回
    public string text = "";
}

//保存记事本内容
public class MsgSaveText : MsgBase
{
    public MsgSaveText() { protoName = "MsgSaveText"; }
    //客户端发
    public string text = "";
    //服务端回（0-成功 1-文字太长）
    public int result = 0;
}


