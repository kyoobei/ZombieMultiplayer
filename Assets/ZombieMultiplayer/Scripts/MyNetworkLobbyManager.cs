using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MyNetworkLobbyManager : NetworkLobbyManager
{
    [SerializeField] MyNetworkDiscovery serverNetworkDiscovery;
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

        startAsServerButton.onClick.AddListener(StartHosting);
        startAsClientButton.onClick.AddListener(StartClientListen);
    }
    private void Update()
    {
        numberOfConnectedClients.text = networkConnectionList.Count.ToString();
    }
    public void StartHosting()
    {
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
        if(!networkConnectionList.Contains(conn))
        {
            networkConnectionList.Add(conn);
        }
    }
    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        base.OnLobbyServerDisconnect(conn);
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
        
        
    }
    public override void OnLobbyClientExit()
    {
        base.OnLobbyClientExit();
    }
    #endregion
    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
    }
}
