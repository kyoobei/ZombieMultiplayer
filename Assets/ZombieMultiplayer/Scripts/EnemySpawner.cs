using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] ObjectPool enemyPool;
    [SerializeField] int enemyCountLimit;
    //holders of position
    GameObject enemyPatrolPointsHolder;
    [SerializeField] GameObject enemySpawnLocationHolder;
    //list of the position base on the holder
    List<GameObject> enemySpawnPositionList = new List<GameObject>();
    List<GameObject> enemyPatrolPositionList = new List<GameObject>();
    //enemy networked obects to return;
    List<GameObject> enemyNetworkedObjects = new List<GameObject>();
    
    /// <summary>
    /// Initialize enemy controls
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
    /// Spawn an enemy at a random spawn point
    /// </summary>
    /// <param name="numberOfEnemies"></param>
    public void SummonEnemiesAtRandomPoint(int numberOfEnemies)
    {
        //cannot summon anymore enemies
        if (enemyPool.GetCountOfReleasedClones > enemyCountLimit)
            return;

        int maxPositionList = enemySpawnPositionList.Count - 1;
        int currentPositionIndex = 0;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (currentPositionIndex <= maxPositionList)
            {
                currentPositionIndex++;
            }
            else if (currentPositionIndex > maxPositionList)
            {
                currentPositionIndex = 0;
            }
            Vector3 spawnPositionArea = enemySpawnPositionList[currentPositionIndex].transform.position;
            var enemySpawn = enemyPool.GetFromPool(spawnPositionArea);
            Zombie zombie = enemySpawn.GetComponent<Zombie>();
            zombie.listOfPatrolPoints = enemyPatrolPositionList;

            enemyNetworkedObjects.Add(enemySpawn);

            //spawn thru server
            NetworkServer.Spawn(enemySpawn, enemyPool.assetHashID);
        }
    }
    /// <summary>
    /// Spawn an enemy at a given position
    /// </summary>
    /// <param name="position"></param>
    public void SpawnEnemiesAtPosition(Vector3 position)
    {
        if (enemyPool.GetCountOfReleasedClones > enemyCountLimit)
            return;
    }
    /// <summary>
    /// Returns a specific gameobject to the pool
    /// </summary>
    /// <param name="enemyClone"></param>
    public void ReturnEnemyToPool(GameObject enemyClone)
    {
        //return a specific enemy clone to the pool
        enemyPool.UnSpawnObject(enemyClone);
        NetworkServer.UnSpawn(enemyClone);
    }
    /// <summary>
    /// Returns all enemy objects to the pool
    /// </summary>
    public void ReturnAllEnemyToPool()
    { 
        //returns all instance of the enemy to clean the area
        for(int i = 0; i < enemyNetworkedObjects.Count; i++)
        {
            NetworkServer.UnSpawn(enemyNetworkedObjects[i]);
            return;
        }
        enemyPool.UnSpawnAllObjects();
    }
}
