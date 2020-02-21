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
        NMClientDiscovery clientDiscovery;
        NMServerDiscovery serverDiscovery;

        enum NetworkTypeEnum
        {
            NONE,
            CLIENT,
            SERVER
        };
        NetworkTypeEnum networkType;

        public int GetCountOfClients
        {
            get { return numberOfClientsConnected; }
        }

        bool isInitialized;
        bool hasRecievedABroadcast;

        int numberOfClientsConnected;
        private void Start()
        {
            isInitialized = false;   
        }
        private void Update()
        {
            if(!isInitialized)
            {
                if (NMNetworkHandler.Instance.IsDoneCreating)
                {
                    SetNetworkType();
                }
            }
            else
            {
                //continue code here
            }
        }
        void SetNetworkType()
        {
            if(GetComponent<NMServerDiscovery>() == null)
            {
                //this type is not a server so get client
                clientDiscovery = GetComponent<NMClientDiscovery>();
                networkType = NetworkTypeEnum.CLIENT;
            }
            if(GetComponent<NMClientDiscovery>() == null)
            {
                //this type is not a client so it must be a server
                serverDiscovery = GetComponent<NMServerDiscovery>();
                networkType = NetworkTypeEnum.SERVER;
            }
            isInitialized = true;
        }
        private void OnClientRecievedABroadcast(string IPAddressRecieved, int portRecieved)
        {
            if (!hasRecievedABroadcast)
            {
                networkAddress = IPAddressRecieved;
                networkPort = portRecieved;
                hasRecievedABroadcast = true;
            }
        }
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

        #region SERVER METHODS
        #endregion
        #region CLIENT METHODS

        #endregion

    }
}
