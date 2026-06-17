using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public int amount;
        public float timeBetweenSpawns = 0.5f;
    }

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemySpawnInfo> enemies = new List<EnemySpawnInfo>();
        public float timeBeforeNextWave = 3f;
    }

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Settings")]
    [SerializeField] private bool startWavesAutomatically = true;
    [SerializeField] private float timeBeforeFirstWave = 2f;

    private int currentWaveIndex = 0;
    private int aliveEnemies = 0;
    private bool isSpawningWave = false;
    private bool wavesFinished = false;

    private void Start()
    {
        if (startWavesAutomatically)
        {
            StartCoroutine(StartFirstWaveAfterDelay());
        }
    }

    private IEnumerator StartFirstWaveAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeFirstWave);
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (isSpawningWave)
            return;

        if (wavesFinished)
            return;

        if (currentWaveIndex >= waves.Count)
        {
            wavesFinished = true;
            Debug.Log("Wszystkie fale ukoñczone!");
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isSpawningWave = true;

        Debug.Log("Start fali: " + wave.waveName);

        foreach (EnemySpawnInfo enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.amount; i++)
            {
                SpawnEnemy(enemyInfo.enemyPrefab);
                yield return new WaitForSeconds(enemyInfo.timeBetweenSpawns);
            }
        }

        isSpawningWave = false;

        Debug.Log("Wszyscy przeciwnicy z fali zostali stworzeni.");
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Brakuje enemyPrefab w WaveManager!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("Brakuje spawn pointów w WaveManager!");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(
            enemyPrefab,
            randomSpawnPoint.position,
            randomSpawnPoint.rotation
        );

        aliveEnemies++;

        Enemy_Health enemyHealth = enemy.GetComponent<Enemy_Health>();

        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDied += HandleEnemyDied;
        }
        else
        {
            Debug.LogWarning("Przeciwnik nie ma skryptu Enemy_Health!");
        }
    }

    private void HandleEnemyDied(Enemy_Health enemyHealth)
    {
        enemyHealth.OnEnemyDied -= HandleEnemyDied;

        aliveEnemies--;

        Debug.Log("Przeciwnik zabity. Pozosta³o: " + aliveEnemies);

        if (aliveEnemies <= 0 && !isSpawningWave)
        {
            GameRunStats.RegisterWaveSurvived();

            currentWaveIndex++;
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        if (currentWaveIndex >= waves.Count)
        {
            wavesFinished = true;
            Debug.Log("Tryb fal zakoñczony. Wszystkie fale pokonane!");
            yield break;
        }

        float delay = waves[currentWaveIndex - 1].timeBeforeNextWave;

        Debug.Log("Nastêpna fala za: " + delay + " sekund");

        yield return new WaitForSeconds(delay);

        StartNextWave();
    }
}