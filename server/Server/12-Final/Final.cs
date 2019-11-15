using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Final
{
    public void Execute()
    {
        if (DbManager.Connect("game", "127.0.0.1", 3306, "root", ""))
        {
            return;
        }
        NetManager.StartLoop(8888);
    }
}

