using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class MyNetworkDiscovery : NetworkDiscovery
{
    bool hasRecievedABroadcast;
    /// <summary>
    /// Add this to server button
    /// </summary>
    public void StartServerBroadcast()
    {
        base.StopBroadcast();
        base.Initialize();
        base.StartAsServer();
    }
    /// <summary>
    /// Add this to client button
    /// </summary>
    public void StartClientListen()
    {
        base.Initialize();
        base.StartAsClient();
    }
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        if (!hasRecievedABroadcast)
        {
            base.OnReceivedBroadcast(fromAddress, data);
            NetworkLobbyManager.singleton.networkAddress = fromAddress;
            NetworkLobbyManager.singleton.StartClient();
            hasRecievedABroadcast = true;
        }
    }
}
