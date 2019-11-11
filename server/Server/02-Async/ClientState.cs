﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte[1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;

        public ClientState() { }
        public ClientState(Socket socket)
        {
            this.socket = socket;
        }
    }
}
