using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfPossiblePosition = new List<GameObject>();
    [SerializeField] SpawnEnemies spawnEnemies;
    bool hasSpawned;
    int maxSpawnPositionIndex;
    int spawnPositionIndex;
    private void Start()
    {
        maxSpawnPositionIndex = listOfPossiblePosition.Count - 1;
    }
    private void Update()
    {
        if(!hasSpawned)
        {
            for(int i = 0; i < spawnEnemies.numberOfZombieCopy; i++)
            {
                GameObject getZombies = spawnEnemies.GetZombieClone();
                getZombies.transform.position = listOfPossiblePosition[spawnPositionIndex].transform.position;
                if (spawnPositionIndex < maxSpawnPositionIndex)
                {
                    spawnPositionIndex++;
                }
                else if(spawnPositionIndex >= maxSpawnPositionIndex)
                {
                    spawnPositionIndex = 0;
                }
            }
            hasSpawned = true;
        }
    }

}
