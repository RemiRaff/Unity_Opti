using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject [] EnemyPrefabs;

    private GameObject[] enemySpawnPoints;
    private int _spawnCount;
    
    void Start()
    {
        // init
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");
        _spawnCount = enemySpawnPoints.Length;

        for (int i = 0; i < _spawnCount; i++)
        {
            InstantiateEnnemy(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)],
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
            InstantiateEnnemy(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)],
                enemySpawnPoints[RandomNumber].transform.position,
                enemySpawnPoints[RandomNumber].transform.rotation);
            _spawnCount++; // un nouvel ennemi est instancié
        }
    }

    public void OnEnnemyDestroy()
    {
        _spawnCount--;
    }

    private void InstantiateEnnemy(GameObject enemy, Vector3 enemyPosition, Quaternion enemyRotation)
    {
        GameObject go = Instantiate(enemy, enemyPosition, enemyRotation);
        // init
        go.GetComponent<SimpleAI>().AddSpawnerEnnemies(this);
        go.GetComponent<SimpleAI>()._ennemyDeathEvent?.AddListener(OnEnnemyDestroy);
    }
}
