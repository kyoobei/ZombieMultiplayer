using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionHandler : MonoBehaviour
{
    const float MIN_RAND_POS = 0.2f;
    const float MAX_RAND_POS = 0.5f;

    [SerializeField] SpawnEnemies spawnEnemies;
    [SerializeField] int numberOfInitialSpawn;
    GameObject patrolPointsHolder;

    List<GameObject> listOfPossiblePosition = new List<GameObject>();
    List<GameObject> listOfPatrolPoints = new List<GameObject>();

    bool hasSpawned;
    int maxSpawnPositionIndex;
    int spawnPositionIndex;
    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            listOfPossiblePosition.Add(transform.GetChild(i).gameObject);
        }
        if (GameObject.FindGameObjectWithTag("Patrol") != null)
        {
            patrolPointsHolder = GameObject.FindGameObjectWithTag("Patrol");
            for (int i = 0; i < patrolPointsHolder.transform.childCount; i++)
            {
                listOfPatrolPoints.Add(patrolPointsHolder.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            Debug.LogError("No object with Patrol tag");
        }
        
        if(listOfPossiblePosition.Count > 0)
            maxSpawnPositionIndex = listOfPossiblePosition.Count - 1;
    }
    private void Update()
    {
        StartSpawningEnemies();   
    }
    private void StartSpawningEnemies()
    {
        if (!hasSpawned)
        {
            for (int i = 0; i < numberOfInitialSpawn; i++)
            {
                GameObject getZombies = spawnEnemies.GetZombieClone();
                
                Zombie zombie = getZombies.GetComponent<Zombie>();
                zombie.listOfPatrolPoints = listOfPatrolPoints;
                
                getZombies.transform.position = GetRandomPositionBaseOnSpawnPoint
                    (
                        listOfPossiblePosition[spawnPositionIndex].transform.position
                    );

                if (spawnPositionIndex < maxSpawnPositionIndex)
                {
                    spawnPositionIndex++;
                }
                else if (spawnPositionIndex >= maxSpawnPositionIndex)
                {
                    spawnPositionIndex = 0;
                }
            }
            hasSpawned = true;
        }
    }
    private Vector3 GetRandomPositionBaseOnSpawnPoint(Vector3 possibleSpawnPoint)
    {
        Vector3 randPos = new Vector3
            (
                Random.Range(possibleSpawnPoint.x - MIN_RAND_POS, possibleSpawnPoint.x + MAX_RAND_POS),
                possibleSpawnPoint.y,
                Random.Range(possibleSpawnPoint.z - MIN_RAND_POS, possibleSpawnPoint.z + MAX_RAND_POS)
            );
        return randPos;
    }

}
