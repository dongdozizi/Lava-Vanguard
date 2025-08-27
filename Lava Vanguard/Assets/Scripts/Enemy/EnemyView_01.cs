using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView_01 : EnemyView
{
    private readonly float waveAmplitude = 2f;
    private readonly float waveFrequency = 2f;
    private void Start()
    {
    }

    protected override void Approching()
    {
        var playerPos = PlayerManager.Instance.playerView.transform.position;
        var direction = (playerPos - transform.position).normalized;

        transform.position += Mathf.Sin(Time.time * waveFrequency) * Time.deltaTime * waveAmplitude * transform.up;
        transform.position += enemyData.Speed * Time.deltaTime * direction;
    }
    protected override Vector3 GetSpawnPosition()
    {
        var playerPos = PlayerManager.Instance.playerView.transform.position;
        Camera camera = Camera.main;
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        float buffer = 2f; // Extra distance off-screen
        Vector3 spawnPosition;
        do
        {
            int side = Random.Range(0, 4); // 0 = Left, 1 = Right, 2 = Top, 3 = Bottom
            switch (side)
            {
                case 0: // Left
                    spawnPosition = new Vector3(-cameraWidth / 2 - buffer, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0);
                    break;
                case 1: // Right
                    spawnPosition = new Vector3(cameraWidth / 2 + buffer, Random.Range(-cameraHeight / 2, cameraHeight / 2), 0);
                    break;
                case 2: // Top
                    spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), cameraHeight / 2 + buffer, 0);
                    break;
                case 3: // Bottom
                    spawnPosition = new Vector3(Random.Range(-cameraWidth / 2, cameraWidth / 2), -cameraHeight / 2 - buffer, 0);
                    break;
                default:
                    spawnPosition = Vector3.zero;
                    break;
            }
        }while (Vector3.Distance(playerPos, spawnPosition) < SpawnDistance);
        return camera.transform.position + spawnPosition;
    }

}
