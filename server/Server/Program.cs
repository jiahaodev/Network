﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server");
            Console.WriteLine("[服务器]启动完成");
            new Final().Execute();  //最终版本
        }
    }
}
