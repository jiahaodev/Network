using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    //ս���е�̹��
    public static Dictionary<string, BaseTank> tanks = new Dictionary<string, BaseTank>();

    //��ʼ��
    public static void Init() {
        //��Ӽ���
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);

        NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }

    //���̹��
    public static void AddTank(string id, BaseTank tank)
    {
        tanks[id] = tank;
    }


    //ɾ��̹��
    public static void RemoveTank(string id)
    {
        tanks.Remove(id);
    }

    //��ȡ̹��
    public static BaseTank GetTank(string id)
    {
        if (tanks.ContainsKey(id))
        {
            return tanks[id];
        }
        return null;
    }

    //��ȡ��ҿ��Ƶ�̹��
    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.id);
    }
    //����ս��
    public static void Reset()
    {
        //����
        foreach (BaseTank tank in tanks.Values)
        {
            MonoBehaviour.Destroy(tank.gameObject);
        }
        //�б�
        tanks.Clear();
    }
    //��ʼս��
    public static void EnterBattle(MsgEnterBattle msg) {
        //����
        BattleManager.Reset();
        //�رս���
        PanelManager.Close("RoomPanel");//���Էŵ�����ϵͳ�ļ�����
        PanelManager.Close("ResultPanel");
        PanelManager.Close("KillPanel");
        PanelManager.Close("BattlePanel");
        PanelManager.Close("AimPanel");
        //����̹��
        for (int i = 0; i < msg.tanks.Length; i++)
        {
            GenerateTank(msg.tanks[i]);
        }
        //�򿪽���
        PanelManager.Open<BattlePanel>();
        PanelManager.Open<AimPanel>();
    }

    //����̹��
    public static void GenerateTank(TankInfo tankInfo)
    {
        //GameObject
        string objName = "Tank_" + tankInfo.id;
        GameObject tankObj = new GameObject(objName);
        //AddComponent
        BaseTank tank = null;
        if (tankInfo.id == GameMain.id)
        {
            tank = tankObj.AddComponent<CtrlTank>();
        }
        else
        {
            tank = tankObj.AddComponent<SyncTank>();
        }
        //camera
        if (tankInfo.id == GameMain.id)
        {
            CameraFollow cf = tankObj.AddComponent<CameraFollow>();
        }
        //����
        tank.camp = tankInfo.camp;
        tank.id = tankInfo.id;
        tank.hp = tankInfo.hp;
        //pos rotation
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        //init
        if (tankInfo.camp == 1)
        {
            tank.Init("tankPrefab");
        }
        else
        {
            tank.Init("tankPrefab2");
        }
        //�б�
        AddTank(tankInfo.id, tank);
    }

    //�յ�����ս��Э��
    public static void OnMsgEnterBattle(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;
        EnterBattle(msg);
    }

    //�յ�ս������Э��
    public static void OnMsgBattleResult(MsgBase msgBase)
    {
        MsgBattleResult msg = (MsgBattleResult)msgBase;
        //�ж���ʾʤ������ʧ��
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if (tank != null && tank.camp == msg.winCamp)
        {
            isWin = true;
        }
        //��ʾ����
        PanelManager.Open<ResultPanel>(isWin);
        //�رս���
        PanelManager.Close("AimPanel");
    }

    //�յ�����˳�Э��
    public static void OnMsgLeaveBattle(MsgBase msgBase)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle)msgBase;
        //����̹��
        BaseTank tank = GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //ɾ��̹��
        RemoveTank(msg.id);
        MonoBehaviour.Destroy(tank.gameObject);
    }

    //�յ�ͬ��Э��
    public static void OnMsgSyncTank(MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank)msgBase;
        //��ͬ���Լ�
        if (msg.id == GameMain.id)
        {
            return;
        }
        //����̹��
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //�ƶ�ͬ��
        tank.SyncPos(msg);
    }

    //�յ�����Э��
    public static void OnMsgFire(MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        //��ͬ���Լ�
        if (msg.id == GameMain.id)
        {
            return;
        }
        //����̹��
        SyncTank tank = (SyncTank)GetTank(msg.id);
        if (tank == null)
        {
            return;
        }
        //����
        tank.SyncFire(msg);
    }

    //�յ�����Э��
    public static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        //����̹��
        BaseTank tank = GetTank(msg.targetId);
        if (tank == null)
        {
            return;
        }
        bool isDie = tank.IsDie();
        //������
        tank.Attacked(msg.damage);
        //��ɱ��ʾ
        if (!isDie && tank.IsDie() && msg.id == GameMain.id)
        {
            PanelManager.Open<KillPanel>();
        }
    }


}