using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


public class TestDB
{
    private static string id = "TestId";
    private static string pw = "123456";
    static JavaScriptSerializer Js = new JavaScriptSerializer();

    public void execute()
    {
        //连接数据库
        DbManager.Connect("tankgame", "127.0.0.1", 3306, "root", "");

        //添加玩家
        DbManager.Register(id, pw);

        //登陆检测
        DbManager.CheckPassword(id, pw);

        //创建游戏玩家数据
        DbManager.CreatePlayer(id);


        //获取玩家数据
        PlayerData playerData = DbManager.GetPlayerData(id);
        string playerDataStr = Js.Serialize(playerData);
        Console.WriteLine("PlayerData:" + playerDataStr);

        //更新玩家数据
        playerData.coin = 1000;
        playerData.win = 100;
        playerData.lost = 20;
        playerData.text = "Do Test";
        DbManager.UpdatePlayerData(id, playerData);



        Console.WriteLine("[数据库] Check Fininsh");
    }
}
