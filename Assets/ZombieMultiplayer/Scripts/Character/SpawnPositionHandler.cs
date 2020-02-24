using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionHandler : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfPossiblePosition = new List<GameObject>();
    [SerializeField] SpawnEnemies spawnEnemies;
    bool hasSpawned;
    private void Update()
    {
        if(!hasSpawned)
        {
            for(int i = 0; i < listOfPossiblePosition.Count; i++)
            {
                GameObject getZombies = spawnEnemies.GetZombieClone();
                getZombies.transform.position = listOfPossiblePosition[i].transform.position;
            }
            hasSpawned = true;
        }
    }

}
