using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkMaker;
public class LobbyUIManager : MonoBehaviour
{
    [Header("LOBBY PANELS")]
    [SerializeField] GameObject serverPanel;
    [SerializeField] GameObject clientPanel;
    [SerializeField] GameObject selectionPanel;

    [Header("SERVER UI")]
    [SerializeField] Text numberOfConnectedClients;
    [SerializeField] Button startAsServerButton;
    [SerializeField] Button startServerBtn;
    [SerializeField] Button startGameBtn;

    [Header("CLIENT UI")]
    [SerializeField] Button startAsClientButton;
    [SerializeField] Text clientStatus;

    bool initialized;
    bool addEventToButtonSelection;

    void Start()
    { 
        initialized = false;    
    }

    void Update()
    {
        Init();
    }
    void Init()
    {
        if (!NMNetworkHandler.Instance.IsDoneCreating)
        {
            if(!addEventToButtonSelection)
            {
                startAsClientButton.onClick.AddListener
                    (
                        NMNetworkHandler.Instance.OnStartAsAClient
                    );
                startAsServerButton.onClick.AddListener
                    (
                        NMNetworkHandler.Instance.OnStartAsAServer
                    );
                addEventToButtonSelection = true;
            }
            //it means nm hasn't initialize in creating a lobby
        }
        else
        {
            /*
            if (!initialized)
            {

                /*
                if (NMNetworkHandler.Instance.isAServer)
                    serverPanel.SetActive(true);
                else if (!NMNetworkHandler.Instance.isAServer)
                {
                    OnStartupOfClient();
                    clientPanel.SetActive(true);
                }
                
                initialized = true;
            }
            */
        }

        
    }
    public void OnButtonPressedServerStart()
    {
        //if not yet started
        if (!initialized)
            return;

        NMNetworkHandler.Instance.StartBroadcastingServer();
        
    }
    /// <summary>
    /// Start networkdiscovery broadcast
    /// </summary>
    void OnServerStartBroadcast()
    {
        NMNetworkHandler.Instance.InitiateStartServer();
        NMNetworkHandler.Instance.StartBroadcastingServer();
    }
    /// <summary>
    /// Start networkdiscovery listen
    /// </summary>
    void OnClientStartListen()
    {
        NMNetworkHandler.Instance.InitiateStartClient();
    }
}
