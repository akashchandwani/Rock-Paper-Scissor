using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Client : MonoBehaviour {

	private int hostId;
	private int connectionId;
    private bool isConnected;
    private int reliableChannel;
    private int unreliableChannel;
    private int MAX_CONNECTION = 2;
    private int port = 5701;
	private byte error;
    private Text ServerText;

	public void Connect() {
		NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, 0);
		connectionId = NetworkTransport.Connect(hostId, "127.0.0.1", port, 0, out error);
        isConnected = true;
	}
	
}
