using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class MyNetworkLobbyPlayer : NetworkLobbyPlayer
{
    [SerializeField] Button readyButton;
    bool isOnline;
    bool isReady;
    private void Update()
    {
        //not yours
        if (!isLocalPlayer)
            return;

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
    }
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
}
