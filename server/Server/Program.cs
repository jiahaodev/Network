using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Hello Server");
        //Console.WriteLine("[服务器]启动完成");
        //new Final().Execute();  //最终版本

        //最终版本
        if (DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
        {
            return;
        }
        NetManager.StartLoop(8888);
    }
}

