using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace P2PCommands
{
    public class DefaultAcknowledge : Command
    {
        public override string Name {
            get { return "ACK"; }
            set { Name = value; }
        }

        public Networking _network;

        public override void PerformAction()
        {

            if (_network != null)
            {
                _network.RegisterNode(new Peer((string)Payload.IPAddress, (string)Payload.Name));
            }
            else
            {
                throw new Exception("Network not set for Acknowledge command");
            }
        }
    }

    public class Networking
    {
        private UdpClient _client;

        /// <summary>
        /// Listents for messages and passes them off to a ProcessMessage Function that the user provides.
        /// </summary>
        private void Listener()
        {
            var anyNetoworkedComputer = new IPEndPoint(IPAddress.Any, _port);
            while(_client.Client.IsBound)
            {
                var data = _client.Receive(ref anyNetoworkedComputer);
                bool isData = data.Length > 0;
                if (isData)
                {
                    var ASCIIMessage = Encoding.ASCII.GetString(data);
                    var command = JsonConvert.DeserializeObject<Command>(ASCIIMessage);
                    var cmd = _knownCommands[command.Name];
                    cmd.networkPayload = command.networkPayload;
                    cmd.Payload = JsonConvert.DeserializeObject(command.networkPayload);
                    cmd.PerformAction();
                }
            }
        }

        #region Constructors
        private IPAddress _broadcastIP = IPAddress.Broadcast;
        public void SetBroadcastIP(string ip)
        {
            _broadcastIP = IPAddress.Parse(ip);
        }

        const int DEFAULT_PORT = 999;
        private int _port = DEFAULT_PORT;

        private void Init()
        {
            _client = new UdpClient(_port);
            Task.Run(() => Listener());    
        }

        public Networking()
        {
            Init();
        }

        public Networking(int port)
        {
            _port = port;
            Init();
        }

        public Networking(string broadcastIP, int port)
        {
            _port = port;
            SetBroadcastIP(broadcastIP);
            Init();
        }
        #endregion

        #region Peer Network
        private Dictionary<string, Peer> _knownPeers = new Dictionary<string, Peer>();
        /// <summary>
        /// Registers a peer to this entities known network.
        /// This is to allow peers to be aware of each other
        /// and in the future, communicate directly with one another.
        /// </summary>
        /// <param name="peer">Peer in the network</param>
        /// <returns>Bool indicating succeess of registration</returns>
        /// Dev notes: 
        /// return type should be custom type instead of bool, 
        public bool RegisterNode(Peer peer)
        {
            //node already in network, update name reference. Assume it's new IP.
            foreach(var knownPeer in _knownPeers)
            {
                if (peer.Name == knownPeer.Key)
                {
                    _knownPeers.Remove(knownPeer.Key);
                    _knownPeers.Add(peer.Name, peer);
                }
            }

            _knownPeers.Add(peer.Name, peer);
            return true;
        }

        public bool UnregisterNode(Peer peer)
        {
            foreach (var knownPeer in _knownPeers)
            {
                if (peer.Name == knownPeer.Key)
                {
                    _knownPeers.Remove(knownPeer.Key);
                    return true;
                }
            }

            return false; //no error, todo: throw exception instead.
        }
        #endregion

        private Dictionary<string, Command> _knownCommands = new Dictionary<string, Command>();

        public bool RegisterCommand(Command command)
        {
            try
            {
                _knownCommands[command.Name] = command;
                return true;
            } catch (Exception e)
            {
                throw new Exception("Failed registering command: " + e.Message);
            }
        }

        public void SendAll(Command command)
        {
            SendDirect(command, _broadcastIP);
        }

        public void SendDirect(Command command, IPAddress recepient)
        {
            using (var client = new UdpClient())
            {
                client.Connect(new IPEndPoint(recepient, _port));
                command.networkPayload = JsonConvert.SerializeObject(command.Payload);
                var cmd = JsonConvert.SerializeObject(command);
                var msg = Encoding.ASCII.GetBytes(cmd);
                client.Send(msg, msg.Length);
            }
        }

        public void SendDirect(Command command, string recepientName)
        {
            IPAddress recepient = _knownPeers[recepientName].IP;
            SendDirect(command, recepient);
        }

        public void SendDirect(Command command, Peer recepient)
        {
            SendDirect(command, recepient.IP);
        }
    }
}
