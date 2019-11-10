/****************************************************
    文件：MoveGameMain.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 19:5:7
	功能：用于测试NetManager
*****************************************************/

using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MoveGameMain : MonoBehaviour
{
    public GameObject humanPrefab;

    public BaseHuman myHuman;

    public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();

    private void Start()
    {
        //网络模块
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("List", OnList);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Attack", OnAttack);
        NetManager.AddListener("Die", OnDie);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
        //添加玩家自身
        InitHumanSelf();
        Thread.Sleep(500);
        //请求玩家列表
        NetManager.Send("List|");
    }

    private void InitHumanSelf() {
        //添加一个角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();

        //发送协议
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y;
        NetManager.Send(sendStr);

    }

    //收到玩家进入消息
    void OnEnter(string msgArgs)
    {
        Debug.Log("OnEnter:" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        //是自己
        if (desc == NetManager.GetDesc())
        {
            return;
        }
        //添加一个角色
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0,eulY,0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc,h);
    }

    //获得用户列表
    void OnList(string msgArgs) {
        Debug.Log("OnList:" + msgArgs);
        string[] split = msgArgs.Split(',');
        int count = (split.Length - 1) / 6;
        for (int i = 0; i < count; i++)
        {
            string desc = split[i * 6 + 0];
            float x = float.Parse(split[i * 6 + 1]);
            float y = float.Parse(split[i * 6 + 2]);
            float z = float.Parse(split[i * 6 + 3]);
            float eulY = float.Parse(split[i * 6 + 4]);
            int hp = int.Parse(split[i * 6 + 5]);
            //是自己
            if (desc == NetManager.GetDesc())
                continue;
            //添加一个角色
            GameObject obj = (GameObject)Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x,y,z);
            obj.transform.eulerAngles = new Vector3(0,eulY,0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc,h);
        }

    }


    void OnMove(string msgArgs)
    {
        Debug.Log("OnMove:" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        //移动
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        BaseHuman h = otherHumans[desc];
        Vector3 targetPos = new Vector3(x,y,z);
        h.MoveTo(targetPos);
    }


    void OnAttack(string msgArgs)
    {
        Debug.Log("OnAttack:" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float eulY = float.Parse(split[1]);
        //移动
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        SyncHuman h = (SyncHuman)otherHumans[desc];
        h.SyncAttack(eulY);
    }


    void OnDie(string msgArgs)
    {
        Debug.Log("OnLeave:" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string hitDesc = split[0];
        if (hitDesc == myHuman.desc)
        {
            Debug.Log("Game Over");
            return;
        }
        if (!otherHumans.ContainsKey(hitDesc))
        {
            return;
        }
        BaseHuman h = otherHumans[hitDesc];
        h.gameObject.SetActive(false);
    }

    

    void OnLeave(string msgArgs)
    {
        Debug.Log("OnLeave:" + msgArgs);
        //解析参数
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        //移动
        if (!otherHumans.ContainsKey(desc))
        {
            return;
        }
        BaseHuman h = otherHumans[desc];
        Destroy(h.gameObject);
        otherHumans.Remove(desc);
    }

    //驱动网络模块分发消息
    private void Update()
    {
        NetManager.Update();
    }
}