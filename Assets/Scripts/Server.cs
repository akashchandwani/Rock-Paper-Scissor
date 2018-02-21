using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Player {
	public string name;
	public string move;
	public int score;

	public Player() {
		name = null;
		move = null;
		score = 0;
	}

}

public class RuleMatrix {
	public Dictionary<string, int> rules;

	public RuleMatrix() {
		rules = new Dictionary<string, int> ();
		rules.Add("rockrock", 0);	
		rules.Add("rockpaper", 2);
		rules.Add("rockscissor", 1);
		rules.Add("paperrock", 1);
		rules.Add("paperpaper", 0);
		rules.Add("paperscissor", 2);
		rules.Add("scissorrock", 2);
		rules.Add("scissorpaper", 1);
		rules.Add("scissorscissor", 0);
	}

}

public class Server : MonoBehaviour
{

    private int hostId;
    private bool isStarted;
    private int reliableChannel;
    private int unreliableChannel;
    private int MAX_CONNECTION = 2;
    private int port = 5701;
    private Text ServerText;
	private int numberOfMovesReceived = 0;
	public Dictionary<int, Player> players = new Dictionary<int, Player> ();
    private byte error;

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
            case NetworkEventType.Nothing:         
                break;
            case NetworkEventType.ConnectEvent:    //2
                Debug.Log("Client connected with connection id : " + connectionId);
                ServerText.text = "Client connected with connection id : " + connectionId;
                getClientName(connectionId);
                break;
			case NetworkEventType.DataEvent:       
				string message = Encoding.Unicode.GetString (recBuffer, 0, dataSize);
				ServerText.text = "Data received from " + connectionId + " " + message;
				string[] messageInfo = message.Split ('|');
				string command = messageInfo [0];
				switch (command) {
					case "MOVE": 
						onMove (connectionId, messageInfo[1]);
						break;
					case "NAME":
						onConnection (connectionId, messageInfo[1]);
						break;
				}
                break;
            case NetworkEventType.DisconnectEvent: //4
                Debug.Log("Client " + connectionId + " disconnected");
                ServerText.text = "Client " + connectionId + " disconnected";
                break;
        }
    }

	void onConnection(int connectionId, string name) {
		Debug.Log ("On new player: " + name);
		Player newPlayer = new Player ();
		newPlayer.name = name;
		players.Add(connectionId, newPlayer); 
	}

	void onMove(int connectionId, string move) {
		Debug.Log ("On new move: " + move);
		Debug.Log ("Player connection ID:" + connectionId);
		foreach(var player in players)
		{
			Debug.Log (player.Value.name);
		}
		players [connectionId].move = move;
		numberOfMovesReceived = 0;
		foreach(var player in players)
		{
			if (player.Value.move != null)
				numberOfMovesReceived++;
		}
		Debug.Log (numberOfMovesReceived);
		if (numberOfMovesReceived == 2) {
			getResult ();
		}
		Debug.Log (players.ToString ());
	}

	void getResult() {
		RuleMatrix ruleMatrix = new RuleMatrix();
		string finalMove = "";
		foreach(var player in players)
		{
			finalMove += player.Value.move;
		}
		Debug.Log (finalMove);
		int result = ruleMatrix.rules [finalMove];
		Debug.Log (result);
	}
    public void getClientName(int connectionId)
    {
        string message = CommandConstants.GET_NAME + "|" + connectionId.ToString();

        byte[] msg = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostId, connectionId, reliableChannel, msg, message.Length * sizeof(char), out error);
    }
}
