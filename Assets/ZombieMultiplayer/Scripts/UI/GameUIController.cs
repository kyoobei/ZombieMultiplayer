using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameUIController : MonoBehaviour
{
    [SerializeField] GameObject serverUI;
    [SerializeField] GameObject clientUI;
    [SerializeField] Text serverGameTimer;
    [SerializeField] Joystick playerJoyStick;
    public Joystick GetPlayerJoystick
    {
        get { return playerJoyStick; }
    }
    public void OnGameTimerDisplayUpdate(int totalSeconds)
    {
        int remainingMinute = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        if (remainingSeconds >= 10)
        {
            serverGameTimer.text = string.Format("{0}:{1}",
                remainingMinute, remainingSeconds);
        }
        else if(remainingSeconds < 10)
        {
            serverGameTimer.text = string.Format("{0}:0{1}",
                remainingMinute, remainingSeconds);
        }
    }

    public void ActivateClientUi()
    {
        serverUI.SetActive(false);
        clientUI.SetActive(true);
    }
    public void ActivateServerUI()
    {
        serverUI.SetActive(true);
        clientUI.SetActive(false);
    }
    public void ActivateTestUI()
    {
        serverUI.SetActive(true);
        clientUI.SetActive(true);
    }
    public void DeactivateAllUI()
    {
        serverUI.SetActive(false);
        clientUI.SetActive(false);
    }
}
