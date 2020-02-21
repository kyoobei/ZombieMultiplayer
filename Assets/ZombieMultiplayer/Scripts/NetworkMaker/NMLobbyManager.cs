using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkMaker
{
    /// <summary>
    /// Inherits the "NetworkLobbyManager.cs" of UNET to extend the capabilites
    /// of UNET (will change in the future if ever)
    /// -made by Gelo
    /// </summary>
    public class NMLobbyManager : NetworkLobbyManager
    {
        public void StartAsAClient()
        {
            StartClient();
        }
        public void StartAsAServer()
        {
            StartServer();
        }
        public void GoToPlayScene()
        {
            ServerChangeScene(playScene);
        }
        public void GoToLobbyScene()
        {
            ServerChangeScene(lobbyScene);
        }
        
    }
}
