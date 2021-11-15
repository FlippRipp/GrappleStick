using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawner : MonoBehaviour
{
    [SerializeField] private float respawnTime = 5;

    [HideInInspector]
    public GameObject objectToRespawn;

    private bool respawning = false;
    private bool isSpawner = false;

    private void Start()
    {
        if (!isSpawner)
        {
            GameObject spawner = new GameObject();
            ReSpawner reSpawner = spawner.AddComponent<ReSpawner>();
            reSpawner.objectToRespawn = gameObject;
            reSpawner.isSpawner = true;
        }
    }

    private void Update()
    {
        if(!objectToRespawn) return;
        if (!objectToRespawn.activeSelf && !respawning)
        {
            StartCoroutine(Respawn());
        }
    }
    
    private IEnumerator Respawn()
    {
        respawning = true;
        yield return new WaitForSeconds(respawnTime);
        respawning = false;
        objectToRespawn.SetActive(true);
    }

}
