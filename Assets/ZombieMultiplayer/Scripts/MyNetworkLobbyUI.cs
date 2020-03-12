using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyNetworkLobbyUI : MonoBehaviour
{
    [Header("Server Lobby UI")]
    [SerializeField] Button startGame;
    [SerializeField] Text numberOfConnectedClientsText;

    [Header("Client Lobby UI")]
    [SerializeField] Text clientStatusText;
    [SerializeField] Button readyButton;

    public delegate void OnStartButtonPressedDelegate();
    public event OnStartButtonPressedDelegate OnStartButtonPressed;

    public delegate void OnReadyButtonPressedDelegate();
    public event OnReadyButtonPressedDelegate OnReadyButtonPressed;

    public void AddListenerToReadyButton()
    {
        RemoveListenterToReadyButton();
        readyButton.onClick.AddListener(ReadyClient);
    }
    public void RemoveListenterToReadyButton()
    {
        readyButton.onClick.RemoveListener(ReadyClient);
    }
    public void AddListenerToStartButton()
    {
        RemoveListenerToStartButton();
        startGame.onClick.AddListener(StartGameOnServer);
    }
    public void RemoveListenerToStartButton()
    {
        startGame.onClick.RemoveListener(StartGameOnServer);
    }
    public void UpdateConnectedPlayersText(string textToDisplay)
    {
        numberOfConnectedClientsText.text = textToDisplay;
    }
    public void UpdateClientStatusText(string textToDisplay)
    {
        clientStatusText.text = textToDisplay;
    }
    public void StartGameOnServer()
    {
        OnStartButtonPressed?.Invoke();
    }
    public void ReadyClient()
    {
        OnReadyButtonPressed?.Invoke();
    }
}
