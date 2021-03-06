﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace P2PCommands
{
    public class Peer
    {
        public Peer(string IPaddress, string name)
        {
            IP = IPAddress.Parse(IPaddress);
            Name = name;
        }

        public Peer(IPAddress IPaddress, string name)
        {
            IP = IPaddress;
            Name = name;
        }

        public IPAddress IP { get; set; }
        public string Name { get; set; }
    }
}
