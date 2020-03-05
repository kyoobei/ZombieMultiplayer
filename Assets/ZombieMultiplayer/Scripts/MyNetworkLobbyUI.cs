using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyNetworkLobbyUI : MonoBehaviour
{
    [SerializeField] Button startAsServerButton;
    [SerializeField] Button startAsClientButton;
    [SerializeField] Text numberOfConnectedClients;

    bool hasBeenSet;
    private void Update()
    {
        if (!hasBeenSet)
        {
            if (MyNetworkLobbyManager.singleton != null)
            {

                hasBeenSet = true;
            }
        }
    }
}
