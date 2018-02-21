using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private int hostId;
    private int connectionId;
    private bool isConnected;
    private int reliableChannel;
    private int unreliableChannel;
    private int MAX_CONNECTION = 2;
    private int port = 5701;
    private byte error;
	private Player player;

    public void Connect()
    {
        NetworkTransport.Init();
        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        unreliableChannel = cc.AddChannel(QosType.Unreliable);
        HostTopology topo = new HostTopology(cc, MAX_CONNECTION);
        hostId = NetworkTransport.AddHost(topo, 0);
        connectionId = NetworkTransport.Connect(hostId, "127.0.0.1", port, 0, out error);
        isConnected = true;
	}

    void Update()
    {
        if (!isConnected)
            return;
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
            case NetworkEventType.DataEvent:       //3
                string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                Debug.Log("data received from " + connectionId + " : " + message);
                string[] data = message.Split('|');
                switch (data[0])
                {
                    case CommandConstants.GET_NAME:
						player = new Player(int.Parse(data[1]), getName(), 0);
						string msg = CommandConstants.NAME + "|" + connectionId + "|" + getName();
                        Send(msg);
                        break;
                    default:
                        Debug.Log("Invalid Data Received");
                        break;
                }
                break;
        }
    }

    public string getName()
    {
        return GameObject.Find("NameField").GetComponent<InputField>().text;
    }

    void Send(string message)
    {
        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, reliableChannel, msg, message.Length * sizeof(char), out error);
    }

	void SendName()
    {
        string message = CommandConstants.NAME + "|" + connectionId + "|" + getName();
		Send(message);
    }

}
