/****************************************************
    文件：TimerTest.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 13:24:54
	功能：Timer 定时任务
*****************************************************/

using UnityEngine;
using System.Threading;

public class TimerTest : MonoBehaviour 
{
    private void Start()
    {
        Timer timer = new Timer(Timeout,null,5000,0);
    }

    private void Timeout(object state) {
        Debug.Log("铃铃铃");
    }
}