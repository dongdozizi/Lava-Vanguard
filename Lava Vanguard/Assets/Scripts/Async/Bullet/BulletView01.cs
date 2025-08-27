using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 普通
public class BulletView01 : BulletView
{
    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 3} },
         { 2, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 4} },
         { 3, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 5} },
         { 4, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 6} },
         { 5, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 7 } },
         { 6, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 8} },
         { 7, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 9} },
         { 8, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 10} },
         { 9, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f,speed=15f, attack = 11} },
    };

    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange +
            "\nSpeed: " + dataByLevel[level].speed +
            "\nAttack: " + dataByLevel[level].attack;
    }

    protected override void SetupBullet(int level)
    {
        lifeDistance = dataByLevel[level].lifeDistance;
        detectionRange = dataByLevel[level].detectionRange;
        speed = dataByLevel[level].speed;
        attack = dataByLevel[level].attack;
        SetFireDirection();
        ApplyInitialRotation();
    }

    private void ApplyInitialRotation()
    {
        if (hasTarget)
        {
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void MoveBullet()
    {
        transform.position += (Vector3)fireDirection* speed *Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }
        if (other.CompareTag("Enemy")||other.CompareTag("Boss"))
        {
            hasHit = true;
            Async.BulletManager.Instance.bulletHit1++;
            TriggerForwarder forwarder=other.GetComponent<TriggerForwarder>();
            EnemyView enemy;
            if (forwarder != null)
            {
                enemy = forwarder.parent;
            }
            else
            {
                enemy = other.GetComponent<EnemyView>();
            }
            if (enemy != null)
            {
                enemy.TakeHit(attack);
            }
            Destroy(gameObject);
        }
    }
}
