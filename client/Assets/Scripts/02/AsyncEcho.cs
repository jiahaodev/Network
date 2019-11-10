/****************************************************
    文件：Echo.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 12:17:58
	功能：Echo
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;

public class AsyncEcho : MonoBehaviour 
{

    //UGUI
    public InputField InputField;
    public Text text;
    //定义套接字
    Socket socket;
    //接收缓冲区
    byte[] readBuff = new byte[1024];
    string recvStr = "";

    //点击连接按钮
    public void Connection (){
        //socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.BeginConnect("127.0.0.1",8888,ConnectCallback,socket);
    }

    //Connect回调
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
	        Socket socket = (Socket)ar.AsyncState;
	        socket.EndConnect(ar);
	        Debug.Log("Socket Connet Succ"); //连接成功
            //启动异步接收
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail" + ex.ToString());
        }
    }


    //Receive回调
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);//完成异步接收
            recvStr = System.Text.Encoding.UTF8.GetString(readBuff,0,count);

            socket.BeginReceive(readBuff,0,1024,0,ReceiveCallback,socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    public void Send()
    {
        //Send
        string sendStr = InputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        //多次发送测试
        //for (int i = 0; i < 100; i++)
        //{
        //    socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);

        //}
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        Debug.Log("Socket Send Finsh ");

    }

    //Send回调
    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndSend(ar);//完成异步发送
            Debug.Log("Socket Send succ " + count);
    
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Send fail" + ex.ToString());
        }
    }

    public void Update()
    {
        text.text = recvStr;
    }
}