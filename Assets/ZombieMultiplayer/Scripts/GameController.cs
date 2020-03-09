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
    };
    [SerializeField] GameUIController gameUIController;
    //[SerializeField] GameSpawner gameSpawner;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] private GameState gameState;
    public GameOwner gameOwner;
    public int gameSeconds;
    bool isDone;
    int gameSecondsHolder;

    
    private void Start()
    {
        if(isServer)
        {
            gameOwner = GameOwner.Server;
        }
        else
        {
            gameOwner = GameOwner.Client;
        }
    }
    private void Update()
    {
        if(gameOwner.Equals(GameOwner.None))
        {
            if (isServer)
            {
                gameOwner = GameOwner.Server;
            }
            else
            {
                gameOwner = GameOwner.Client;
            }
        }

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
                enemySpawner.InitializeEnemySpawn();
                //spawn a specific number of enemy at the start of the game
                enemySpawner.SummonEnemiesAtRandomPoint(20);

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
