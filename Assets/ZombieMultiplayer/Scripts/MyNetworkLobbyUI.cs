using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MyNetworkLobbyUI : MonoBehaviour
{
    //[SerializeField] Button startAsServerButton;
    //[SerializeField] Button startAsClientButton;
    //[SerializeField] Text numberOfConnectedClients;
    [SerializeField] Button startGame;
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
        startGame.onClick.AddListener(StartGame);
    }
    public void RemoveListenerToStartButton()
    {
        startGame.onClick.RemoveListener(StartGame);
    }
    public void StartGame()
    {
        Debug.Log("start the game");
        OnStartButtonPressed?.Invoke();
    }
    public void ReadyClient()
    {
        Debug.Log("im ready");
        OnReadyButtonPressed?.Invoke();
    }
}
