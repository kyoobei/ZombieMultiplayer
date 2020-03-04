using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawner : MonoBehaviour
{
    [Header("Enemy Spawn")]
    [SerializeField] GameObject enemySpawnLocationHolder;
    GameObject enemyPatrolPointsHolder;
    [Header("Player Spawn")]
    [SerializeField] GameObject playerSpawnLocationHolder;
    [Header("Single Player Mode")]
    [SerializeField] GameObject playerHolderPrefab;
    [SerializeField] SpawnEnemies spawnEnemies;

    List<GameObject> enemySpawnPositionList = new List<GameObject>();
    List<GameObject> enemyPatrolPositionList = new List<GameObject>();
    List<GameObject> playerSpawnPositionList = new List<GameObject>();

    /// <summary>
    /// Initialize spawn settings for enemy
    /// </summary>
    public void InitializeEnemySpawn()
    {
        if (GameObject.FindGameObjectWithTag("Patrol") != null)
        {
            enemyPatrolPointsHolder = GameObject.FindGameObjectWithTag("Patrol");
            for (int i = 0; i < enemyPatrolPointsHolder.transform.childCount; i++)
            {
                enemyPatrolPositionList.Add(enemyPatrolPointsHolder.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < enemySpawnLocationHolder.transform.childCount; i++)
        {
            enemySpawnPositionList.Add(enemySpawnLocationHolder.transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Initialize spawn settings for player
    /// </summary>
    public void InitializePlayerSpawn()
    {
        for (int i = 0; i < playerSpawnLocationHolder.transform.childCount; i++)
        {
            playerSpawnPositionList.Add(playerSpawnLocationHolder.transform.GetChild(i).gameObject);
        }
    }
    /// <summary>
    /// Spawn enemies locally or spawn enemies using single player;
    /// </summary>
    /// <param name="numberOfEnemiesToSpawn"></param>
    public void StartSpawningEnemiesLocally(int numberOfEnemiesToSpawn)
    {
        // no enemy spawn position list;
        if (enemySpawnPositionList.Count <= 0)
            return;

        int maxPositionListIndex = enemySpawnPositionList.Count - 1;
        int currentPositionIndex = 0;

        for(int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject spawnZombie = spawnEnemies.GetZombieClone();
            Zombie zombie = spawnZombie.GetComponent<Zombie>();
            zombie.listOfPatrolPoints = enemyPatrolPositionList;
           
            //spawn zombies randomly on the map
            if(currentPositionIndex <= maxPositionListIndex)
            {
                currentPositionIndex++;
            }
            else if(currentPositionIndex > maxPositionListIndex)
            {
                currentPositionIndex = 0;
            }
            spawnZombie.transform.position = enemySpawnPositionList[currentPositionIndex].transform.position;

        }
    }
    /// <summary>
    /// Spawn enemies over the networked. Use the server side to spawn enemies
    /// </summary>
    /// <param name="numberOfEnemiesToSpawn"></param>
    public void StartSpawningEnemiesNetwork(int numberOfEnemiesToSpawn)
    {
        if (enemySpawnPositionList.Count <= 0)
            return;

        //do logic here later
    }
    /// <summary>
    /// Spawn player locally or spawned player in single player
    /// </summary>
    public void StartSpawningPlayerLocally(Joystick playerJoystick)
    {
        // no player spawn list
        if (playerSpawnPositionList.Count <= 0)
            return;

        GameObject playerObject = Instantiate(playerHolderPrefab);
        Player player = playerObject.GetComponent<Player>();
        player.playerJoystick = playerJoystick;
        //remove this shit later
        playerObject.transform.position = playerSpawnPositionList[0].transform.position;
    }
}
