using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using P2PCommands;

namespace DemoForm
{
    public partial class Form1 : Form
    {
        public class ShowMsgBox : Command
        {
            public override string Name
            {
                get { return "MSGBOX"; }
                set { Name = value; }
            }

            public override void PerformAction()
            {
                MessageBox.Show("temp");
            }
        }
        private Networking _networking = new Networking();
        public Form1()
        {
            InitializeComponent();
            _networking.SetBroadcastIP("192.168.1.255");
            var ack = new DefaultAcknowledge();
            ack._network = _networking;
            _networking.RegisterCommand(ack);
            _networking.RegisterCommand(new ShowMsgBox());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ack = new DefaultAcknowledge();
            ack._network = _networking;
            var payload = new DefaultAcknowledgePayload();
            payload.IPAddress = "127.0.0.1";
            payload.Name = "Localhost";
            ack.Payload = payload;
            _networking.SendAll(ack);
            var msgbox = new ShowMsgBox();
            _networking.SendAll(msgbox);
        }
    }
}
