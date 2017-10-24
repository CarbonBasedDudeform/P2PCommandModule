using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace P2PCommands
{
    //todo: commands need to be re-written
    //all they need is a name and a payload
    //name and object type is stored in dictionary so object types can be retrieved by passing in name
    //then payload maps to data
    //commands sent to peers are names and payload is plain ol' data
    //then objects can be created and populated with data and passed on to the NetworkResponse delegate which can handle Actions in response to the data.
    //commands could even bake a Action function into them, we just populate the object with the data and action is called.
    //this could remove the need for the delegate completely as you'd get something like this in the listener() function
    // name = 'ack', payload = [ ip: '127.0.0.1', name: 'derp' ]
    // var cmd = new _knownCommands[name](payload);
    // cmd.PerformAction();

    /// <summary>
    /// Interface for Commands that can be sent to peers.
    /// Commands must be convertible to a format to send over the network and be retrieved over the network.
    /// </summary>
    [DataContract]
    public class Command
    {
        //abstract public byte[] ConvertToSend();
        //abstract public void ConvertFromNetwork(byte[] data);
        [DataMember]
        public string networkPayload { get; set; }
        virtual public Payload Payload { get; set; }
        [DataMember]
        virtual public string Name { get; set; }
        virtual public void PerformAction()
        {
        }
    }

    public class Payload
    {

    }
}
