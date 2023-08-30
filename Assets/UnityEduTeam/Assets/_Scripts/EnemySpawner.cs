using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;

    private GameObject[] enemySpawnPoints;
    private int _spawnCount;
    
    void Start()
    {
        // init
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
        _spawnCount = enemySpawnPoints.Length;

        for (int i = 0; i < _spawnCount; i++)
        {
            Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)],
                enemySpawnPoints[i].transform.position,
                enemySpawnPoints[i].transform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // vérification si un enemy est mort et le cas échéant en faire spawn un nouveau à une position aléatoire
        // pour cela on compare le nombre théorique d'enemy avec le nombre actuel
        while (_spawnCount < enemySpawnPoints.Length)
        {
            int RandomNumber = Random.Range(0, enemySpawnPoints.Length);
            Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)],
                enemySpawnPoints[RandomNumber].transform.position,
                enemySpawnPoints[RandomNumber].transform.rotation);
            _spawnCount++; // un nouvel ennemi est instancié
        }
    }

    public void OnEnnemyDestroy()
    {
        _spawnCount--;
    }
}
