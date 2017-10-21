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
        private Networking _networking = new Networking();
        public Form1()
        {
            InitializeComponent();
            _networking.SetBroadcastIP("192.168.1.255");
            _networking.ProcessMessage = new Networking.NetworkResponse(network_event);
            _networking.RegisterCommand(new DefaultAcknowledge());
        }

        public void network_event(string msg)
        {

            MessageBox.Show(msg);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            _networking.SendAll(new DefaultAcknowledge());
        }
    }
}
