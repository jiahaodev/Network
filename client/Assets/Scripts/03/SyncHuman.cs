/****************************************************
    文件：SyncHuman.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 18:32:28
	功能：SyncHuman。 表示网络中的其他玩家
*****************************************************/

using UnityEngine;

public class SyncHuman : BaseHuman
{
    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();

    }

    public void SyncAttack(float eulY)
    {

        transform.eulerAngles = new Vector3(0, eulY, 0);
        Attack();

    }
}