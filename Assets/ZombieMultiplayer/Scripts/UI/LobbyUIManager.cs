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

    [Header("CLIENT UI")]
    [SerializeField] Text clientStatus;

    bool initialized;
    void Start()
    {
        initialized = false;    
    }

    void Update()
    {
        if(!initialized)
        {
            if (NMNetworkHandler.Instance.isAServer)
                serverPanel.SetActive(true);
            else if(!NMNetworkHandler.Instance.isAServer)
                clientPanel.SetActive(true);

            initialized = true;
        }
    }
}
