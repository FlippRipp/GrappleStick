using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenSpawns = 1;

    private float spawnTimer;
    
    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField] private List<GameObject> spawnedObjects;

    [SerializeField] private GameObject prefabToSpawn;
    
    
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > timeBetweenSpawns)
        {
            int i = Random.Range(0, spawnPoints.Length - 1);
            spawnedObjects.Add(Instantiate(prefabToSpawn, spawnPoints[i].position, spawnPoints[i].rotation));
            spawnTimer = 0;
        }
    }
}
