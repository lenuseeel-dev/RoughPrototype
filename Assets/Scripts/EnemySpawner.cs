using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnMargin = 0.2f;
    public float spawnZ = 0f;

    public float timeBetweenWaves = 8f;
    public float enemySpawnDelay = 0.5f;

    public int firstWaveCount = 3;
    public int waveIncrease = 1;

    public int maxEnemies = 20; // ⭐ 추가 (최대 몬스터 수 제한)

    private Camera mainCamera;
    private int currentWave;
    private int enemiesToSpawn;
    private int spawnedThisWave;

    private float spawnTimer;
    private float waveTimer;

    private bool waveActive;

    void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera 없음");
        }
    }

    void Start()
    {
        firstWaveCount = GameSettings.FirstWaveOrcCount;
        waveIncrease = GameSettings.WaveOrcIncrease;
        timeBetweenWaves = GameSettings.TimeBetweenWaves;

        currentWave = 0;
        StartNextWave();
    }

    void Update()
    {
        if (enemyPrefab == null || mainCamera == null)
            return;

        // ⭐ 최대 몬스터 수 제한
        if (GameObject.FindGameObjectsWithTag("Enemy").Length >= maxEnemies)
            return;

        if (waveActive)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnedThisWave < enemiesToSpawn && spawnTimer <= 0f)
            {
                SpawnEnemy();
                spawnedThisWave++;
                spawnTimer = enemySpawnDelay;
            }

            if (spawnedThisWave >= enemiesToSpawn)
            {
                waveActive = false;
                waveTimer = timeBetweenWaves;
            }
        }
        else
        {
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0f)
            {
                StartNextWave();
            }
        }
    }

    private void StartNextWave()
    {
        currentWave++;

        enemiesToSpawn = firstWaveCount + waveIncrease * (currentWave - 1);

        spawnedThisWave = 0;
        spawnTimer = 0f;
        waveActive = true;

        Debug.Log($"Wave {currentWave} 시작 - {enemiesToSpawn}마리 생성");
    }

    private void SpawnEnemy()
    {
        Vector3 viewportPos = GetRandomViewportOutside();
        viewportPos.z = Mathf.Abs(mainCamera.transform.position.z - spawnZ);

        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewportPos);
        worldPos.z = spawnZ;

        GameObject spawnedEnemy = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

        // ⭐ 혹시 프리팹에 스포너 붙어있으면 비활성화
        EnemySpawner spawner = spawnedEnemy.GetComponent<EnemySpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }
    }

    private Vector3 GetRandomViewportOutside()
    {
        int side = Random.Range(0, 4);
        float x = 0f;
        float y = 0f;

        switch (side)
        {
            case 0: // 왼쪽
                x = -spawnMargin;
                y = Random.Range(-spawnMargin, 1f + spawnMargin);
                break;
            case 1: // 오른쪽
                x = 1f + spawnMargin;
                y = Random.Range(-spawnMargin, 1f + spawnMargin);
                break;
            case 2: // 아래
                x = Random.Range(-spawnMargin, 1f + spawnMargin);
                y = -spawnMargin;
                break;
            case 3: // 위
                x = Random.Range(-spawnMargin, 1f + spawnMargin);
                y = 1f + spawnMargin;
                break;
        }

        return new Vector3(x, y, 0f);
    }
}
