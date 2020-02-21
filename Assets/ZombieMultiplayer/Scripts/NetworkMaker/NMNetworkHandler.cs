using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
        /// If this object should start as a server make sure
        /// to check this component and if this should start as a client
        /// this should be false
        /// </summary>
        public bool isAServer;
        /// <summary>
        /// Immidiately start server broadcast or use a button to start it
        /// </summary>
        public bool immidiateStartServerBroadcast;
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
        [Header("Related UI")]
        [SerializeField] Button startServerButton;
        [SerializeField] Button startGameButton;

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
            if(isAServer)
            {
                //create an object that handles all server stuff
                CreateAServerObject();
            }
            else if(!isAServer)
            {
                //create an object that handles all client stuff
                CreateAClientObject();
            }
        }
        #region METHODS FOR NETWORK OBJECT CREATION
        private protected void CreateAServerObject()
        {
            GameObject serverObject = new GameObject("SERVER_LobbyManager");
            NMLobbyManager _lobbyManager = serverObject.AddComponent<NMLobbyManager>();
            NMServerDiscovery _discovery = serverObject.AddComponent<NMServerDiscovery>();

            InitValueOfLobbyManager(_lobbyManager);
            _discovery.useNetworkManager = true;
            _discovery.showGUI = false;
            if(immidiateStartServerBroadcast)
            {
                _discovery.BroadcastOnStart = true;
            }
            else
            {
                if(startServerButton != null)
                    startServerButton.onClick.AddListener(_discovery.OnClickStartBroadcast);
                else
                {
                    Debug.LogWarning("No Start button has been assigned.. reverting to automatic" +
                        "broadcast");
                    _discovery.BroadcastOnStart = true;
                }
            }
            if(startGameButton == null)
            {
                Debug.LogError("No start game button");
            }
            else
            {
                startGameButton.onClick.AddListener(_discovery.StopBroadcast);

            }
            IsDoneCreating = true;
        }
        private protected void CreateAClientObject()
        {
            GameObject clientObject = new GameObject("CLIENT_LobbyManager");
            NMLobbyManager _lobbyManager = clientObject.AddComponent<NMLobbyManager>();
            NMClientDiscovery _discovery = clientObject.AddComponent<NMClientDiscovery>();
            InitValueOfLobbyManager(_lobbyManager);
            _discovery.showGUI = false;

            IsDoneCreating = true;
        }

        private protected void InitValueOfLobbyManager(NMLobbyManager lobbyManager)
        {
            lobbyManager.showLobbyGUI = false;
            lobbyManager.networkPort = (desiredPort.Equals(0)) ? NMDefaultConstants.DEFAULT_PORT : desiredPort;
            lobbyManager.maxPlayers = (maxPlayers.Equals(0)) ? NMDefaultConstants.DEFAULT_MAX_PLAYERCOUNT : maxPlayers;
            lobbyManager.minPlayers = (minPlayers.Equals(0)) ? NMDefaultConstants.DEFAULT_MIN_PLAYERCOUNT : minPlayers;
            lobbyManager.lobbyScene = lobbySceneToUse;
            lobbyManager.playScene = gameSceneToUse;

            lobbyManager.playerPrefab = playerClientPrefab;
            
            if(lobbyPrefab != null)
                lobbyManager.lobbyPlayerPrefab = lobbyPrefab;
            
            lobbyManager.autoCreatePlayer = false;
        }
        #endregion

    }
}
