using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView_03 : EnemyView
{
    
    private bool movingRight = true; 
    private float leftLimit;
    private float rightLimit;
    private Camera mainCamera;
    // Enemy will be destroyed if lower than mainCamera.y+destroyY
    public float destroyY=-10f;
    // How many small enemies will be splited
    private int splitCount=3;
    // The radius of split
    private float splitRadius = 1.0f;
    
    public GameObject enemyView02Prefab;

    private void Start()
    {
        leftLimit = transform.position.x - 1.2f;
        rightLimit = transform.position.x + 1.2f;
        mainCamera = Camera.main;
    }
    
    protected override void Approching()
    {
       transform.Translate(Vector2.right * enemyData.Speed * Time.deltaTime * (movingRight ? 1 : -1));

        if (movingRight && transform.position.x >= rightLimit)
        {
            Flip();
        }
        else if (!movingRight && transform.position.x <= leftLimit)
        {
            Flip();
        }
        if (mainCamera != null && transform.position.y < mainCamera.transform.position.y +destroyY){
            Destroy(gameObject);
        }
    }

    protected override Vector3 GetSpawnPosition()
    {
        var playerPos = PlayerManager.Instance.playerView.transform.position;
        Vector3 spawnPosition;
        do
        {
            var g = PlatformGenerator.Instance.platforms[Random.Range(0, PlatformGenerator.Instance.platforms.Count)];

            // Collect all non-null platforms in the selected layer
            List<PlatformView> validPlatforms = new List<PlatformView>();
            foreach (var platform in g)
            {
                if (platform != null)
                {
                    validPlatforms.Add(platform);
                }
            }

            PlatformView chosenPlatform = validPlatforms[Random.Range(0, validPlatforms.Count)];
            spawnPosition = chosenPlatform.transform.position + new Vector3(0, 0.75f, 0);
        } while (Vector3.Distance(playerPos, spawnPosition) < SpawnDistance);
        return spawnPosition;
    }

    void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void SplitIntoSmallerEnemies()
    {
        Vector3 parentPosition = transform.position;
        for (int i = 0; i < splitCount; i++)
        {
            float rad = i * (360f / (1f * splitCount))*Mathf.Deg2Rad;
            Vector3 offset=new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * splitRadius;
            Vector3 spawnLocation = parentPosition + offset;
            var enemyView=EnemyManager.Instance.GenerateSpecificEnemy(0,level);
            enemyView.transform.position = spawnLocation;
        }
    }

    public override bool TakeHit(int bulletAttack)
    {
        base.TakeHit(bulletAttack);
        if (enemyData.Health <= 0) 
        {
            SplitIntoSmallerEnemies();
            return true;
        }
        return false;
    }
}