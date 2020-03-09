using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MyNetworkLobbyManager : NetworkLobbyManager
{
    [SerializeField] MyNetworkDiscovery serverNetworkDiscovery;
    [SerializeField] MyNetworkLobbyUI networkLobbyUI;
    [SerializeField] Button startAsServerButton;
    [SerializeField] Button startAsClientButton;
    [SerializeField] Button readyButton;
    [SerializeField] Text numberOfConnectedClients;

    //only server has this list
    public List<NetworkConnection> networkConnectionList = new List<NetworkConnection>();
     private void Start()
    {
        //for mobile devices to not sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StopServer();

        startAsServerButton.onClick.AddListener(StartHosting);
        startAsClientButton.onClick.AddListener(StartClientListen);

    }
    private void Update()
    {
        if(GameObject.Find("NetworkLobbyUI") != null)
        {
            if(networkLobbyUI == null)
            {
                networkLobbyUI = GameObject.Find("NetworkLobbyUI").GetComponent<MyNetworkLobbyUI>();
                networkLobbyUI.AddListenerToStartButton();
                //networkLobbyUI.OnStartButtonPressed += OnLobbyServerPlayersReady;
            }
        }
        numberOfConnectedClients.text = networkConnectionList.Count.ToString();
    }
    public void StartHosting()
    {
        NetworkServer.Reset();
        StartServer();
        serverNetworkDiscovery.StartServerBroadcast();
    }
    public void StartClientListen()
    {
        serverNetworkDiscovery.StartClientListen();
    }

    #region SERVER OVERRIDES
    public override void OnLobbyServerConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
        Debug.Log("a client entered: " + conn.connectionId);
        if(!networkConnectionList.Contains(conn))
        {
            
            networkConnectionList.Add(conn);
        }
    }
    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);
        Debug.Log("a client disconnected:  " + conn.connectionId);
        if(networkConnectionList.Contains(conn))
        {
            networkConnectionList.Remove(conn);
        }
    }
    #endregion
    #region CLIENT OVVERIDES
    public override void OnLobbyClientEnter()
    {
        base.OnLobbyClientEnter();
        Debug.Log("called OnLobbyClientEnter");
        
    }
    public override void OnLobbyStartClient(NetworkClient lobbyClient)
    {
        base.OnLobbyStartClient(lobbyClient);
        Debug.Log("called OnLobbyStartClient");
    }
    public override void OnLobbyClientExit()
    {
        //usually when they enter the game itself player client exits
        base.OnLobbyClientExit();
        Debug.Log("called OnLobbyClientExit");
      
    }
    #endregion
    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
    }
}
