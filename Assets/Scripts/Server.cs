using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{

    private int hostId;
    private bool isStarted;
    private int reliableChannel;
    private int unreliableChannel;
    private int MAX_CONNECTION = 2;
    private int port = 5701;
    private Text ServerText;

    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, port, null);
        isStarted = true;
        ServerText = GameObject.Find("ServerText").GetComponent<Text>();
        ServerText.text = "Server Started!";
    }

    void Update()
    {
        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.Nothing:         //1
                break;
            case NetworkEventType.ConnectEvent:    //2
				Debug.Log("Client connected with connection id - " + connectionId);
                ServerText.text = "Client connected with connection id - " + connectionId;
                break;
            case NetworkEventType.DataEvent:       //3
				ServerText.text = "data received from " + connectionId + Encoding.Unicode.GetString(recBuffer, 0, dataSize);;
                break;
            case NetworkEventType.DisconnectEvent: //4
                ServerText.text = "Client " + connectionId + "disconnected";
                break;
        }
    }
}
