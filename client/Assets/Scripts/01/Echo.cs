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


public class Echo : MonoBehaviour 
{
    //定义套接字
    Socket socket;
    //UGUI
    public InputField InputField;
    public Text text;

    //点击连接按钮
    public void Connection (){
        //socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Connect
        socket.Connect("127.0.0.1",8888);
    }

    public void Send()
    {
        //Send
        string sendStr = InputField.text;
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
        socket.Send(sendBytes);
        //Receive
        byte[] readBuff = new byte[1024];
        int count = socket.Receive(readBuff);
        string recvStr = System.Text.Encoding.UTF8.GetString(readBuff,0,count);

        text.text = recvStr;
        //Close
        socket.Close();
    }
}