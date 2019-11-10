/****************************************************
    文件：GameRoot.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 14:46:20
	功能：Nothing
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRoot : MonoBehaviour 
{

    public string sceneName = "MoveGame" ;

    private void Start()
    {

        DontDestroyOnLoad(this);//重要：启动后不需要销毁
        Debug.Log("Game Start ...");
        AsyncLoadScene(sceneName);

    }

    private void AsyncLoadScene(string sceneName, Action loaded = null)
    {
        AsyncOperation scenAsync = SceneManager.LoadSceneAsync(sceneName);
    }
}