using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GameController : NetworkBehaviour
{
    /// <summary>
    /// this part deals with the numerous states of the game
    /// <para> Inital - use this zombie spawning or doing some
    /// introduciton for the game</para>
    /// <para> GameStart - players can freely move inside the game, server
    /// listens to clients for the state</para>
    /// <para> GameEnd - for the game conclusion if there is a survivor
    /// or total eliminate</para>
    /// </summary>
    private enum GameState
    {
        None,
        GameStart,
        GameEnd
    };
    public enum GameOwner
    {
        None,                   //not yet set who is the owner of the game
        Server,                 //server owns the game 
        Client,                 //client or the player itself owns the game
        Test                    //client still owns the game but should release enemy (singleplayer mode)
    };
    [SerializeField] GameUIController gameUIController;
    [SerializeField] GameSpawner gameSpawner;
    [SerializeField] private GameState gameState;
    public GameOwner gameOwner;
    public int gameSeconds;
    bool isDone;
    int gameSecondsHolder;
    private void Start()
    {
        if (!isServer)
            return;

        if(gameSeconds <= 0)
        {
            Debug.LogError("Put seconds on the GameController object");
            return;
        }
    }
    private void Update()
    {
        if (!isServer)
            return;
        
        if (gameSeconds <= 0)
        {
            Debug.LogError("Put seconds on the GameController object");
            return;
        }

        if (!isDone)
        {
            UpdateGamesettings();
        }
        else
        {
            UpdateGameStates();
        }
    }
    private void UpdateGamesettings()
    {
        switch(gameOwner)
        {
            case GameOwner.None:
                //do nothing
                gameState = GameState.None;
                gameUIController.DeactivateAllUI();
                break;
            case GameOwner.Client:
                gameSecondsHolder = gameSeconds;

                gameUIController.ActivateClientUi();
                gameState = GameState.GameStart;
                isDone = true;
                break;
            case GameOwner.Server:
                gameSecondsHolder = gameSeconds;

                gameUIController.ActivateServerUI();
                //gameSpawner.InitializeEnemySpawn();

                StartCoroutine(StartCountdownOnServer());
                gameState = GameState.GameStart;
                isDone = true;
                break;
            case GameOwner.Test:
                gameSecondsHolder = gameSeconds;

                gameSpawner.InitializePlayerSpawn();
                gameSpawner.InitializeEnemySpawn();
                gameSpawner.StartSpawningEnemiesLocally(1);
                gameSpawner.StartSpawningPlayerLocally
                    (
                        gameUIController.GetPlayerJoystick
                    );

                gameUIController.ActivateTestUI();
                StartCoroutine(StartCountdownOnServer());
                gameState = GameState.GameStart;
                isDone = true;
                break;
        }
    }
    IEnumerator StartCountdownOnServer()
    {
        while(gameSecondsHolder >= 1)
        {
            gameSecondsHolder -= 1;
            gameUIController.OnGameTimerDisplayUpdate(gameSecondsHolder);
            yield return new WaitForSeconds(1f);
            if(gameSecondsHolder <= 0)
            {
                gameState = GameState.GameEnd;
            }
        }
    }
    private void UpdateGameStates()
    {
        switch(gameState)
        {
            case GameState.GameStart:
                StartGame();
                break;
            case GameState.GameEnd:
                EndGame();
                break;
        }
    }
    void StartGame()
    {
        //do game start code here
    }
    void EndGame()
    {
        //do game end here
        Debug.Log("game has ended");
    }
    
}
