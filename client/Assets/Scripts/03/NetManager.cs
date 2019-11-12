/****************************************************
    文件：NetManager.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 18:43:29
	功能：网络通信管理
*****************************************************/

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public  class SimpleNetManager
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static byte[] readBuff = new byte[1024];
    //委托类型
    public delegate void MsgListener(string str);
    //监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<string> msgList = new List<string>();

    //添加监听
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述
    public static string GetDesc()
    {
        if (socket == null) { return ""; }
        if (!socket.Connected) { return ""; }
        return socket.LocalEndPoint.ToString();
    }

    //连接
    public static void Connect(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    //接收回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
	        Socket socket = (Socket)ar.AsyncState;
	        int count = socket.EndReceive(ar);
	        string recvstr = System.Text.Encoding.UTF8.GetString(readBuff,0,count);
	        msgList.Add(recvstr);
	        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail " + ex.ToString()) ;
        }
    }

    //发送
    public static void Send(string sendStr) {
        if (socket == null){ return; }
        if (!socket.Connected){ return; }

        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendBytes);
    }


    //消息回调
    public static void Update() {
        if (msgList.Count <= 0)
        {
            return;
        }
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        Debug.Log("msgStr:" + msgStr);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }

    }
}