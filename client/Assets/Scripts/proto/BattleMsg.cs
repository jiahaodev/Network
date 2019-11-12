/****************************************************
    文件：BattleMsg.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/13 0:37:0
	功能：移动消息
*****************************************************/

using UnityEngine;

public class MsgMove : MsgBase
{
    public MsgMove() { protoName = "MsgMove"; }

    public int x = 0;
    public int y = 0;
    public int z = 0;
}


public class MsgAttack : MsgBase
{
    public MsgAttack() { protoName = "MsgAttack"; }

    public string desc = "127.0.0.1:6543";
}