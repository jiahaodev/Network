/****************************************************
    文件：EchoEx.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/11 22:41:33
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Threading;

public class EchoEx : MonoBehaviour
{
    //UGUI
    public InputField InputField;
    public Text text;
    //定义套接字
    Socket socket;
    //接收缓冲区
    byte[] readBuff = new byte[1024];
    //接收缓冲区的数据长度
    int bufferCount = 0;
    //显示文字
    string recvStr = "";

    //点击连接按钮
    public void Connection()
    {
        //socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.BeginConnect("127.0.0.1", 8888, ConnectCallback, socket);
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
            socket.BeginReceive(readBuff, bufferCount, 1024 - bufferCount, 0, ReceiveCallback, socket);
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
            bufferCount += count;
            //处理二进制数据
            //recvStr = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            OnReceiveData();

            //Thread.Sleep(10*1000); //模拟网络阻塞

            //继续接收数据
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    private void OnReceiveData() {
        Debug.Log("[Recv 1] bufferCount="+bufferCount);
        Debug.Log("[Recv 2] readbuff=" + BitConverter.ToString(readBuff));
        //消息长度
        if (bufferCount <= 2)
            return;
        Int16 bodyLength = BitConverter.ToInt16(readBuff, 0);
        Debug.Log("[Recv 3] bodyLength=" + bodyLength);
        //消息体
        if (bufferCount < 2 + bodyLength)
            return;  //还不是完整信息
        string s = System.Text.Encoding.UTF8.GetString(readBuff,2, bodyLength);
        Debug.Log("[Recv 4] s=" + s);
        //更新缓冲区
        int start = 2 + bodyLength;
        int count = bufferCount - start;
        Array.Copy(readBuff,start,readBuff,0,count);
        bufferCount -= start;
        Debug.Log("[Recv 5] bufferCount=" + bufferCount);
        //消息处理
        recvStr = s + "\n" + recvStr;
        //继续读取消息
        OnReceiveData();



    }


    //组装协议，采用长度信息法（长度 + 具体内容）,解决粘包问题
    public void Send()
    {
        
        string sendStr = InputField.text;
        byte[] bodyBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        Int16 len = (Int16)bodyBytes.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        //大小端编码
        if (!BitConverter.IsLittleEndian)
        {
            Debug.Log("[Send] Reverse lenBytes");
            lenBytes.Reverse();
        }
        else {
            Debug.Log("[Send]BitConverter.IsLittleEndian");
        }

        byte[] sendBytes = lenBytes.Concat(bodyBytes).ToArray();
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