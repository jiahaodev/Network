using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    //��ս��ť
    private Button startButton;
    //�˳���ť
    private Button closeButton;
    //�б�����
    private Transform content1;//��ɫ��Ӫ
    private Transform content2;//��ɫ��Ӫ
                               //�����Ϣ����
    private GameObject playerObj;

    //��ʼ��
    public override void OnInit()
    {
        skinPath = "RoomPanel";
        layer = PanelManager.Layer.Panel;
    }

    //��ʾ
    public override void OnShow(params object[] args)
    {
        //Ѱ�����
        startButton = skin.transform.Find("CtrlPanel/StartButton").GetComponent<Button>();
        closeButton = skin.transform.Find("CtrlPanel/CloseButton").GetComponent<Button>();
        content1 = skin.transform.Find("ListPanel/Scroll View1/Viewport/Content");
        content2 = skin.transform.Find("ListPanel/Scroll View2/Viewport/Content");
        playerObj = skin.transform.Find("Player").gameObject;
        //�����������Ϣ
        playerObj.SetActive(false);
        //��ť�¼�
        startButton.onClick.AddListener(OnStartClick);
        closeButton.onClick.AddListener(OnCloseClick);
        //Э�����
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);
        //���Ͳ�ѯ
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        NetManager.Send(msg);
    }

    //�ر�
    public override void OnClose()
    {
        //Э�����
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
    }

    //�յ�����б�Э��
    public void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        //�������б�
        for (int i = content1.childCount - 1; i >= 0; i--)
        {
            GameObject o = content1.GetChild(i).gameObject;
            Destroy(o);
        }
        for (int i = content2.childCount - 1; i >= 0; i--)
        {
            GameObject o = content2.GetChild(i).gameObject;
            Destroy(o);
        }
        //���������б�
        if (msg.players == null)
        {
            return;
        }
        for (int i = 0; i < msg.players.Length; i++)
        {
            GeneratePlayerInfo(msg.players[i]);
        }
    }

    //����һ�������Ϣ��Ԫ
    public void GeneratePlayerInfo(PlayerInfo playerInfo)
    {
        //��������
        GameObject o = Instantiate(playerObj);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;
        //������Ӫ 1-�� 2-��
        if (playerInfo.camp == 1)
        {
            o.transform.SetParent(content2);
        }
        else
        {
            o.transform.SetParent(content1);
        }
        //��ȡ���
        Transform trans = o.transform;
        Text idText = trans.Find("IdText").GetComponent<Text>();
        Image ownerImage = trans.Find("OwnerImage").GetComponent<Image>();
        Text scoreText = trans.Find("ScoreText").GetComponent<Text>();
        //�����Ϣ
        idText.text = playerInfo.id;
        if (playerInfo.isOwner == 1)
        {
            ownerImage.gameObject.SetActive(true);
        }
        else
        {
            ownerImage.gameObject.SetActive(false);
        }
        scoreText.text = playerInfo.win + "ʤ " + playerInfo.lost + "��";
    }

    //����˳���ť
    public void OnCloseClick()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    //�յ��˳�����Э��
    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        //�ɹ��˳�����
        if (msg.result == 0)
        {
            //PanelManager.Open<TipPanel>("�˳�����");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        //�˳�����ʧ��
        else
        {
            PanelManager.Open<TipPanel>("�˳�����ʧ��");
        }
    }

    //�����ս��ť
    public void OnStartClick()
    {
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }

    //�յ���ս����
    public void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        //��ս
        if (msg.result == 0)
        {
            //�رս���
            Close();
        }
        //��սʧ��
        else
        {
            PanelManager.Open<TipPanel>("��սʧ�ܣ��������ٶ���Ҫһ����ң�ֻ�жӳ����Կ�ʼս����");
        }
    }

}
