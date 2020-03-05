using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NetworkMaker
{
    /// <summary>
    /// This class is the backbone of client server relationship
    /// </summary>
    public class NMNetworkHandler : MonoBehaviour
    {
        #region SINGLETON OF NETWORK HANDLER
        private static NMNetworkHandler _instance;
        public static NMNetworkHandler Instance
        {
            get { return _instance; }
        }
        #endregion 
        [Header("NETWORK DATA")]
        /// <summary>
        /// Set a desired port for the game
        /// </summary>
        public int desiredPort;
        /// <summary>
        /// Set a max number of players allowed for the lobby
        /// </summary>
        public int maxPlayers;
        /// <summary>
        /// Set a minimum number of player/s to start the game
        /// </summary>
        public int minPlayers;
        [Header("Networked Prefabs")]
        [SerializeField] GameObject playerClientPrefab;
        [SerializeField] UnityEngine.Networking.NetworkLobbyPlayer lobbyPrefab;
        [Header("Scenes to use")]
        [SerializeField] public string lobbySceneToUse;
        [SerializeField] public string gameSceneToUse;

        delegate void OnPressedStartServerEvent();
        delegate void OnPressedStopServerEvent();
        delegate void OnPressedStartClientEvent();
        delegate void OnPressedStopClientEvent();

        event OnPressedStartServerEvent OnPressedStartServer;
        event OnPressedStopServerEvent OnPressedStopServer;
        event OnPressedStartClientEvent OnPressedStartClient;
        event OnPressedStopClientEvent OnPressedStopClient;

        public bool IsDoneCreating 
        {
            get; set;
        }
       
        public void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                IsDoneCreating = false;
                
                DontDestroyOnLoad(this.gameObject);
            }
            else if(_instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        public void OnStartAsAServer()
        {
            CreateAServerObject();
        }
        public void OnStartAsAClient()
        {
            CreateAClientObject();
        }

        #region VIRTUAL METHODS FOR SERVER/CLIENT CREATION
        protected virtual void CreateAServerObject()
        {
            //to stop it from creating new lobby manager.. lobby manager should have one instance
            if (GameObject.Find("SERVER_LobbyManager") == null)
            {
                GameObject serverObject = new GameObject("SERVER_LobbyManager");
                NMLobbyManager _lobbyManager = serverObject.AddComponent<NMLobbyManager>();
                NMServerDiscovery _discovery = serverObject.AddComponent<NMServerDiscovery>();

                AddEventsToServer(_lobbyManager);
                InitValueOfLobbyManager(_lobbyManager);
                _discovery.useNetworkManager = true;
                _discovery.showGUI = false;
                _discovery.BroadcastOnStart = true;
            }
            IsDoneCreating = true;
        }
        protected virtual void CreateAClientObject()
        {
            //to stop it from creating new lobby manager.. lobby manager should have one instance
            if (GameObject.Find("CLIENT_LobbyManager") == null)
            {
                GameObject clientObject = new GameObject("CLIENT_LobbyManager");
                NMLobbyManager _lobbyManager = clientObject.AddComponent<NMLobbyManager>();
                NMClientDiscovery _discovery = clientObject.AddComponent<NMClientDiscovery>();
                AddEventsToClient(_lobbyManager);
                InitValueOfLobbyManager(_lobbyManager);
                _discovery.showGUI = false;
            }
            IsDoneCreating = true;
        }

        protected virtual void InitValueOfLobbyManager(NMLobbyManager lobbyManager)
        {
            lobbyManager.showLobbyGUI = false;
            lobbyManager.networkPort = (desiredPort.Equals(0)) ? NMDefaultConstants.DEFAULT_PORT : desiredPort;
            lobbyManager.maxPlayers = (maxPlayers.Equals(0)) ? NMDefaultConstants.DEFAULT_MAX_PLAYERCOUNT : maxPlayers;
            lobbyManager.minPlayers = (minPlayers.Equals(0)) ? NMDefaultConstants.DEFAULT_MIN_PLAYERCOUNT : minPlayers;
            lobbyManager.lobbyScene = lobbySceneToUse;
            lobbyManager.playScene = gameSceneToUse;
            lobbyManager.gamePlayerPrefab = playerClientPrefab;

            //lobbyManager.

            if(lobbyPrefab != null)
                lobbyManager.lobbyPlayerPrefab = lobbyPrefab;
            
            lobbyManager.autoCreatePlayer = true;
        }
        #endregion
        #region CALLBACKS
        protected void AddEventsToServer(NMLobbyManager lobbyManager)
        {
            //remove first to make sure no other listening is happening
            RemoveEventsToServer(lobbyManager);

            OnPressedStartServer += lobbyManager.InitiateServerStart;
            OnPressedStopClient += lobbyManager.InitiateServerStop;
        }
        protected void RemoveEventsToServer(NMLobbyManager lobbyManager)
        {
            OnPressedStartServer -= lobbyManager.InitiateServerStart;
            OnPressedStopServer -= lobbyManager.InitiateServerStop;
        }
        protected void AddEventsToClient(NMLobbyManager lobbyManager)
        {
            //remove first to make sure no other listening is happening
            RemoveEventsToClient(lobbyManager);

            OnPressedStartClient += lobbyManager.InitiateClientStart;
            OnPressedStopClient += lobbyManager.InitiateClientStop;
        }
        protected void RemoveEventsToClient(NMLobbyManager lobbyManager)
        {
            OnPressedStartClient -= lobbyManager.InitiateClientStart;
            OnPressedStopClient -= lobbyManager.InitiateClientStop;
        }
        #endregion
        #region PUBLIC METHODS FOR SERVER
        /// <summary>
        /// Start broadcasting a message from the server
        /// </summary>
        public void StartBroadcastingServer()
        {
            if (GameObject.FindObjectOfType<NMServerDiscovery>() != null)
                GameObject.FindObjectOfType<NMServerDiscovery>().OnClickStartBroadcast();
            else
                Debug.LogError("No NMServerDiscovery on this object: " + gameObject.name);
        }
        /// <summary>
        /// Stop broadcasting a message from the server
        /// </summary>
        public void StopBroadcastingServer()
        {
            if (GameObject.FindObjectOfType<NMServerDiscovery>() != null)
                GameObject.FindObjectOfType<NMServerDiscovery>().OnClickStopBroadcast();
            else
                Debug.LogError("No NMServerDiscovery on this object: " + gameObject.name);
        }
        /// <summary>
        /// Initiate startup of Server
        /// </summary>
        public void InitiateStartServer()
        {
            OnPressedStartServer?.Invoke();
        }
        /// <summary>
        /// Initiate stopping the server
        /// </summary>
        public void InitiateStopServer()
        {
            OnPressedStopServer?.Invoke();
        }
        /// <summary>
        /// Initiate starting the client
        /// </summary>
        #endregion
        #region PUBLIC METHODS FOR CLIENTS
        public void InitiateStartClient()
        {
            OnPressedStartClient?.Invoke();
        }
        /// <summary>
        /// Initiate stopping the client
        /// </summary>
        public void InitiateStopClient()
        {
            OnPressedStopClient?.Invoke();
        }
        #endregion
    }
}
