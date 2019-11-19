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

        Test();

        /*

        //最终版本
        if (!DbManager.Connect("tankgame", "127.0.0.1", 3306, "root", ""))
        {
            return;
        }
        NetManager.StartLoop(8888);
        */
    }

    static void Test() {
        new TestDB().execute();

    }
}

