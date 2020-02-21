using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NetworkMaker
{
    /// <summary>
    /// This class inherits "NetworkDiscovery" of UNET. The purpose of this class
    /// is to setup the server and broadcast data over the network so that "Clients" can connect to the
    /// app or game
    /// - Made by Gelo
    /// </summary>
    public class NMServerDiscovery : NetworkDiscovery
    {
        NMLobbyManager lobbyManager;
        public bool BroadcastOnStart
        {
            set; get;
        }
        private void Start()
        {
            //since this game object is being spawned 
            if(GetComponent<NMLobbyManager>() == null)
            {
                lobbyManager = gameObject.AddComponent<NMLobbyManager>();
            }
            else
            {
                lobbyManager = gameObject.GetComponent<NMLobbyManager>();
            }

            if (BroadcastOnStart)
            {
                base.Initialize();
                StartAsServer();
            }
        }
        public void OnClickStartBroadcast()
        {
            //start broadcasting
            base.Initialize();
            StartAsServer();
        }
        public void OnClickStopBroadcast()
        {
            //stop broadcast to avoid unneccassary data
            StopBroadcast();
        }
    }
}
