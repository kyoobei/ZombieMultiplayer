using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool isAServer;
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
        Inital,
        GameStart,
        GameEnd
    };
    [SerializeField] private GameState gameState;

    private void Start()
    {
        gameState = GameState.Inital;   
    }
    private void Update()
    {
        switch(gameState)
        {
            case GameState.Inital:
                break;
            case GameState.GameStart:
                break;
            case GameState.GameEnd:
                break;
        }
    }
}
