using DG.Tweening;
using UnityEngine;
using Math = System.Math;
using Random = UnityEngine.Random;
using System.Collections;
using Async;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    
    public static EnemyManager Instance { get; private set; }
    public Transform enemyContainer;
    
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;
    public GameObject bossRef;

    public List<EnemyView> enemyViews = new List<EnemyView>();

    [Header("Enemy Settings")]
    private float minSpawnInterval = 0.18f;
    private float maxSpawnInterval = 4f;
    private float maxLevel = 20f;
    private Coroutine spawnCoroutine;

    [HideInInspector] public int enemyKilled = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        
    }

    public EnemyView GenerateRandomEnemy(int level)
    {
        int suffix = Random.Range(0, enemyPrefabs.Length);
        var enemyView = Instantiate(enemyPrefabs[suffix],enemyContainer).GetComponent<EnemyView>();
        enemyView.Init("Enemy_0" + (suffix + 1),level);
        enemyViews.Add(enemyView);
        return enemyView;
    }

    public EnemyView GenerateSpecificEnemy(int suffix,int level)
    {
        var enemyView = Instantiate(enemyPrefabs[suffix], enemyContainer).GetComponent<EnemyView>();
        enemyView.Init("Enemy_0" + (suffix + 1),level);
        enemyViews.Add(enemyView);

        return enemyView;
    }
    public EnemyView GenerateSpecificEnemy(int suffix, Vector3 position)
    {
        var enemyView = Instantiate(enemyPrefabs[suffix], enemyContainer).GetComponent<EnemyView>();
        enemyView.Init("Enemy_0" + (suffix + 1), position);
        enemyViews.Add(enemyView);
        return enemyView;
    }
    private void Start()
    {
        if (!Tutorial.Instance.tutorial)
            StartSpawn();
    }

    public void StartSpawn()
    {   
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
        spawnCoroutine = StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1f);

        if (LevelManager.Instance.WaveHasBoss())
        {
            if (LevelManager.Instance.wave>20)
            {
                SpawnBoss(Random.Range(0, 5), 9);
            }
            else
            {
                int level = Mathf.Min(Mathf.Max(1, LevelManager.Instance.wave / 2), 9);
                SpawnBoss((level - 1) / 2, level);
            }
            while (true)
            {
                GenerateRandomEnemy(LevelManager.Instance.wave / 2 + 1);
                float waitTime = CalculateSpawnInterval()*3f;
                yield return new WaitForSeconds(waitTime);
            }
        }
        else if (LevelManager.Instance.wave == 0)
        {
            while (true)
            {
                GenerateSpecificEnemy(1, 1);
                float waitTime = 1f;
                yield return new WaitForSeconds(waitTime);
            }
        }
        else if(LevelManager.Instance.wave == 1)
        {
            while (true)
            {
                GenerateSpecificEnemy(0, 1);
                float waitTime = 1f;
                yield return new WaitForSeconds(waitTime);
            }
        }
        else if (LevelManager.Instance.wave == 2)
        {
            while (true)
            {
                GenerateSpecificEnemy(2, 1);
                float waitTime = 2f;
                yield return new WaitForSeconds(waitTime);
            }
        }
        else
        {
            while (true)
            {
                GenerateRandomEnemy(LevelManager.Instance.wave / 2 + 1);
                float waitTime = CalculateSpawnInterval();
                yield return new WaitForSeconds(waitTime);
            }
        }

    }

    private float CalculateSpawnInterval()
    {
        int currentlevel = LevelManager.Instance.wave;
        float safeCardCount = Mathf.Clamp(currentlevel, 2f, maxLevel);
        float minSpawnCount = 1f / maxSpawnInterval;
        float maxSpawnCount = 1f / minSpawnInterval;

        float calculatedInterval = minSpawnCount + (maxSpawnCount - minSpawnCount) * (safeCardCount - 1f) / (maxLevel - 1f);
        return 1f/calculatedInterval;
    }

    private void SpawnBoss(int suffix,int level)
    {
        var boss = Instantiate(bossPrefabs[suffix], enemyContainer);
        var bossView = boss.GetComponent<EnemyView>();
        if (suffix < 4)
        {
            bossView.Init("Enemy_Boss_01", level);
        }
        else
        {
            bossView.Init("Enemy_Boss_02", level);
        }
        bossRef = boss;
    }
    public void KillBoss()
    {
        if (bossRef != null) {
            var bossView = bossRef.GetComponent<EnemyView>();
            bossView.TakeHit(bossView.enemyData.MaxHealth);
        }
    }

    public void StopSpawn()
    {
        if (spawnCoroutine != null)
        {
            //Debug.Log("stop");
            StopCoroutine(spawnCoroutine);
        }
    }

    public void killAll()
    {
        foreach (Transform enemy in enemyContainer)
        {
            enemy.GetComponent<EnemyView>().ForceKill();
        }
    }
}
