using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView_Boss_Bullet : EnemyView
{
    private Vector3 direction;
    private Vector3 startPosition;
    private float maxDistance;
    private void Start()
    {
    }

    public void Init(string ID, Vector3 startPosition, Vector3 direction, Quaternion rotation)
    {
        base.Init(ID, 1);
        transform.position = startPosition;
        transform.rotation = rotation;
        this.startPosition = startPosition;
        this.direction = direction;
        maxDistance = 20f;
    }

    protected override Vector3 GetSpawnPosition()
    {
        return Vector3.zero;
    }

    protected override void Approching()
    {
        transform.position += enemyData.Speed * Time.deltaTime * direction;
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }
}