using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MyNetworkLobbyPlayer : NetworkLobbyPlayer
{
    //[SerializeField] Button readyButton;
    //bool isOnline;
    [SerializeField] MyNetworkLobbyUI networkLobbyUI;
    bool isReady;
    private void Update()
    {
        //not yours
        if (!isLocalPlayer)
            return;

        if(!isReady)
        {
            // to automatic ready the clients
           // SendReadyToBeginMessage();
           if(GameObject.Find("NetworkLobbyUI") != null)
            {
                if(networkLobbyUI == null)
                {
                    networkLobbyUI = GameObject.Find("NetworkLobbyUI").GetComponent<MyNetworkLobbyUI>();
                    networkLobbyUI.AddListenerToReadyButton();
                    networkLobbyUI.OnReadyButtonPressed += SetLobbyPlayerToReady;
                }
            }
            isReady = true;
        }

        /*
        if(gameObject.activeInHierarchy)
        {
            if(!isOnline)
            {
                //if the btn has been found
                if(GameObject.Find("btnReady") != null)
                {
                    readyButton = GameObject.Find("btnReady").GetComponent<Button>();
                    AssignIsReady();
                    isOnline = false;
                }
            }
        }
        */
    }
    public void SetLobbyPlayerToReady()
    {
        if(networkLobbyUI != null)
            SendReadyToBeginMessage();
    }
    /*
    void AssignIsReady()
    {
        if(readyButton != null)
        {
            
            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnClickReadyButton);
        }
    }
    void OnClickReadyButton()
    {
        if (!isReady)
        {
            isReady = true;
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Not Ready";
            SendReadyToBeginMessage();
        }
        else if(isReady)
        {
            isReady = false;
            readyButton.transform.GetChild(0).GetComponent<Text>().text = "Ready";
            SendNotReadyToBeginMessage();
        }
    }
    */
}
