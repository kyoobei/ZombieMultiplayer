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

    [Header("SERVER UI")]
    [SerializeField] Text numberOfConnectedClients;
    [SerializeField] Button startServerBtn;
    [SerializeField] Button startGameBtn;

    [Header("CLIENT UI")]
    [SerializeField] Text clientStatus;

    bool initialized;

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
            return;

        if (!initialized)
        {
            if (NMNetworkHandler.Instance.isAServer)
                serverPanel.SetActive(true);
            else if (!NMNetworkHandler.Instance.isAServer)
            {
                OnStartupOfClient();
                clientPanel.SetActive(true);
            }

            initialized = true;
        }
    }
    public void OnButtonPressedServerStart()
    {
        //if not yet started
        if (!initialized)
            return;

        NMNetworkHandler.Instance.StartBroadcastingServer();
        NMNetworkHandler.Instance.InitiateStartServer();
    }
    void OnStartupOfClient()
    {
        NMNetworkHandler.Instance.InitiateStartClient();
    }
}
